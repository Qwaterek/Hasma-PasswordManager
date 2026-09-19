using PasswordGenerator.Models;
using System.Diagnostics;
using System.Windows;

namespace PasswordGenerator.Services
{
    public class UrlLauncher
    {
        public static void OpenAccountUrl(Account account)
        {
            if (account == null || string.IsNullOrWhiteSpace(account.Title))
                return;
            string rawUrl = account.Title.Trim();
            try
            {
                if (!rawUrl.StartsWith("http://") && !rawUrl.StartsWith("https://"))
                {
                    rawUrl = "https://" + rawUrl;
                }

                Uri uri = new Uri(rawUrl);
                string baseDomain = $"{uri.Scheme}://{uri.Host}";

                string targetUrl = baseDomain;
                if (!string.IsNullOrWhiteSpace(account.Username))
                {
                    targetUrl = $"{baseDomain}/{account.Username.Trim('/')}";
                }

                Process.Start(new ProcessStartInfo
                {
                    FileName = targetUrl,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nie można otworzyć adresu URL: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
