using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace PasswordGenerator.Services
{
    public class VaultMeta
    {
        public string SaltBase64 { get; set; } = string.Empty;
        public int Iterations { get; set; }
        public string VerifierIvBase64 { get; set; } = string.Empty;
        public string VerifierDataBase64 { get; set; } = string.Empty;
    }

    public static class MasterPasswordService
    {
        private const int SaltSize = 16;                 // 128-bit salt
        private const int KeySize = 32;                   // 256-bit AES key
        private const int DefaultIterations = 310_000;    // OWASP-recommended minimum for PBKDF2-HMAC-SHA256
        private const string VerifierPlainText = "PasswordGenerator-vault-check-v1";

        public static readonly string MetaFilePath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "vault.meta.json");

        public static bool VaultExists() => File.Exists(MetaFilePath);

        public static byte[] DeriveKey(string password, byte[] salt, int iterations)
        {
            return Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                salt,
                iterations,
                HashAlgorithmName.SHA256,
                KeySize);
        }

        public static byte[] CreateNewVault(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
            int iterations = DefaultIterations;
            byte[] key = DeriveKey(password, salt, iterations);

            (byte[] iv, byte[] cipher) = EncryptVerifier(key);

            var meta = new VaultMeta
            {
                SaltBase64 = Convert.ToBase64String(salt),
                Iterations = iterations,
                VerifierIvBase64 = Convert.ToBase64String(iv),
                VerifierDataBase64 = Convert.ToBase64String(cipher)
            };

            File.WriteAllText(
                MetaFilePath,
                JsonSerializer.Serialize(meta, new JsonSerializerOptions { WriteIndented = true }));

            return key;
        }

        public static byte[]? TryUnlock(string password)
        {
            if (!VaultExists())
                return null;

            try
            {
                var meta = JsonSerializer.Deserialize<VaultMeta>(File.ReadAllText(MetaFilePath));
                if (meta == null || string.IsNullOrEmpty(meta.SaltBase64))
                    return null;

                byte[] salt = Convert.FromBase64String(meta.SaltBase64);
                byte[] key = DeriveKey(password, salt, meta.Iterations);

                byte[] iv = Convert.FromBase64String(meta.VerifierIvBase64);
                byte[] cipher = Convert.FromBase64String(meta.VerifierDataBase64);

                string plain = DecryptVerifier(key, iv, cipher);
                return plain == VerifierPlainText ? key : null;
            }
            catch
            {
                return null;
            }
        }

        private static (byte[] iv, byte[] cipher) EncryptVerifier(byte[] key)
        {
            using Aes aes = Aes.Create();
            aes.Key = key;
            aes.GenerateIV();

            using ICryptoTransform encryptor = aes.CreateEncryptor();
            byte[] plainBytes = Encoding.UTF8.GetBytes(VerifierPlainText);
            byte[] cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

            return (aes.IV, cipherBytes);
        }

        private static string DecryptVerifier(byte[] key, byte[] iv, byte[] cipherBytes)
        {
            using Aes aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;

            using ICryptoTransform decryptor = aes.CreateDecryptor();
            byte[] plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
            return Encoding.UTF8.GetString(plainBytes);
        }
    }
}
