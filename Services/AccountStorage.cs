using PasswordGenerator.Models;
using System.Collections.ObjectModel;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Windows;

namespace PasswordGenerator.Services
{
    public class AccountStorage
    {
        // Ścieżka do głównego pliku roboczego
        public static readonly string FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "accounts.json");

        // Klucz AES-256 NIE jest już zaszyty w kodzie - jest wyprowadzany z hasła głównego
        // użytkownika (zobacz MasterPasswordService) i przekazywany tutaj jako parametr.
        public static void SaveAccounts(ObservableCollection<Account> accounts, byte[] key)
        {
            try
            {
                string json = JsonSerializer.Serialize(accounts, new JsonSerializerOptions { WriteIndented = true });
                byte[] plainBytes = Encoding.UTF8.GetBytes(json);

                using (Aes aes = Aes.Create())
                {
                    aes.Key = key;
                    aes.GenerateIV();
                    byte[] iv = aes.IV;

                    using (MemoryStream ms = new MemoryStream())
                    {
                        ms.Write(iv, 0, iv.Length); // IV zapisywany na początku pliku
                        using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(plainBytes, 0, plainBytes.Length);
                            cs.FlushFinalBlock();
                        }

                        // Wymuszamy zapis na dysk
                        File.WriteAllBytes(FilePath, ms.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas zapisu do accounts.json: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static ObservableCollection<Account> LoadAccounts(byte[] key)
        {
            try
            {
                if (!File.Exists(FilePath))
                {
                    return new ObservableCollection<Account>();
                }

                byte[] fileBytes = File.ReadAllBytes(FilePath);

                if (fileBytes.Length < 16)
                    return new ObservableCollection<Account>();

                using (Aes aes = Aes.Create())
                {
                    aes.Key = key;

                    byte[] iv = new byte[16];
                    Array.Copy(fileBytes, 0, iv, 0, 16);
                    aes.IV = iv;

                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(fileBytes, 16, fileBytes.Length - 16);
                            cs.FlushFinalBlock();
                        }

                        string json = Encoding.UTF8.GetString(ms.ToArray());
                        var accounts = JsonSerializer.Deserialize<ObservableCollection<Account>>(json);
                        return accounts ?? new ObservableCollection<Account>();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Nie udało się odczytać pliku accounts.json przy użyciu podanego hasła głównego.\n\n" +
                    "Jeśli plik pochodzi sprzed wprowadzenia hasła głównego (starsza wersja programu), " +
                    "jest zaszyfrowany innym, nieaktualnym już kluczem i nie da się go odszyfrować obecnym hasłem.\n\n" +
                    "W takim wypadku usuń plik accounts.json z folderu programu i uruchom go ponownie - " +
                    "zacznie się z pustą listą kont.\n\n" +
                    $"Szczegóły techniczne: {ex.Message}",
                    "Błąd odczytu accounts.json",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return new ObservableCollection<Account>();
            }
        }
    }
}
