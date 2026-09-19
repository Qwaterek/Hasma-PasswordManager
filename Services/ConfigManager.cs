using PasswordGenerator.Models;
using System.Collections.ObjectModel;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Windows;

namespace PasswordGenerator.Services
{
    // Plik eksportu przechowuje TYLKO sól i liczbę iteracji potrzebne do
    // ponownego wyprowadzenia klucza z hasła - nigdy sam klucz.
    public class ConfigPayload
    {
        public string SaltBase64 { get; set; } = string.Empty;
        public int Iterations { get; set; }
        public string IvBase64 { get; set; } = string.Empty;
        public string EncryptedData { get; set; } = string.Empty;
    }

    public static class ConfigManager
    {
        private const int SaltSize = 16;
        private const int KeySize = 32;
        private const int Iterations = 310_000; // PBKDF2-HMAC-SHA256, zgodnie z zaleceniami OWASP

        private static byte[] DeriveKey(string password, byte[] salt, int iterations)
        {
            return Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                salt,
                iterations,
                HashAlgorithmName.SHA256,
                KeySize);
        }

        /// <summary>
        /// Zapisuje podaną listę kont do pliku eksportu, szyfrując je kluczem
        /// wyprowadzonym z podanego hasła. Plik NIE zawiera klucza - bez hasła
        /// dane są nie do odzyskania.
        /// </summary>
        public static void SaveConfigFile(ObservableCollection<Account> accounts, string filePath, string password)
        {
            try
            {
                byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
                byte[] key = DeriveKey(password, salt, Iterations);

                string json = JsonSerializer.Serialize(accounts, new JsonSerializerOptions { WriteIndented = true });
                byte[] plainBytes = Encoding.UTF8.GetBytes(json);

                using (Aes aes = Aes.Create())
                {
                    aes.Key = key;
                    aes.GenerateIV();

                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(plainBytes, 0, plainBytes.Length);
                            cs.FlushFinalBlock();
                        }

                        var payload = new ConfigPayload
                        {
                            SaltBase64 = Convert.ToBase64String(salt),
                            Iterations = Iterations,
                            IvBase64 = Convert.ToBase64String(aes.IV),
                            EncryptedData = Convert.ToBase64String(ms.ToArray())
                        };

                        string configJson = JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true });
                        File.WriteAllText(filePath, configJson);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas zapisu pliku konfiguracyjnego: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Odczytuje konta z pliku konfiguracyjnego, wyprowadzając klucz z podanego hasła.
        /// Zwraca null, jeśli hasło jest błędne albo plik jest uszkodzony.
        /// </summary>
        public static ObservableCollection<Account>? LoadConfigFile(string filePath, string password)
        {
            try
            {
                if (!File.Exists(filePath))
                    return null;

                string configJson = File.ReadAllText(filePath);
                var payload = JsonSerializer.Deserialize<ConfigPayload>(configJson);

                if (payload == null || string.IsNullOrEmpty(payload.SaltBase64))
                    return null;

                byte[] salt = Convert.FromBase64String(payload.SaltBase64);
                byte[] key = DeriveKey(password, salt, payload.Iterations);
                byte[] iv = Convert.FromBase64String(payload.IvBase64);
                byte[] encryptedBytes = Convert.FromBase64String(payload.EncryptedData);

                using (Aes aes = Aes.Create())
                {
                    aes.Key = key;
                    aes.IV = iv;

                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(encryptedBytes, 0, encryptedBytes.Length);
                            cs.FlushFinalBlock();
                        }

                        string json = Encoding.UTF8.GetString(ms.ToArray());
                        return JsonSerializer.Deserialize<ObservableCollection<Account>>(json);
                    }
                }
            }
            catch
            {
                // Błędne hasło daje błąd deszyfrowania (bad padding) - to oczekiwane
                // i po prostu oznacza "złe hasło", więc nie pokazujemy tu MessageBoxa,
                // tylko oddajemy null - komunikat pokazuje wywołujący kod w MainWindow.
                return null;
            }
        }
    }
}
