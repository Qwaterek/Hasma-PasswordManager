using PasswordGenerator.Models;
using PasswordGenerator.Services;
using System.Globalization;
using System.Windows;

namespace PasswordGenerator
{
    public partial class App : Application
    {
        public static void ChangeCulture(string cultureCode)
        {
            CultureInfo culture = new CultureInfo(cultureCode);
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            // Przeładowujemy okna aplikacji, aby odświeżyć zaktualizowane zasoby
            foreach (Window window in Current.Windows)
            {
                if (window.IsInitialized)
                {
                    // Wymusza odświeżenie bindingów w WPF
                    window.Language = System.Windows.Markup.XmlLanguage.GetLanguage(culture.IetfLanguageTag);
                }
            }
            // Zapisz wybór języka
            AppSettings.SaveCulture(cultureCode);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Domyślny język to angielski, chyba że użytkownik zapisał inny wybór
            string savedCulture = AppSettings.LoadCulture();
            if (string.IsNullOrEmpty(savedCulture))
            {
                savedCulture = "en-US";
            }

            ChangeCulture(savedCulture);

            // Wymagamy hasła głównego przed pokazaniem głównego okna.
            // Jeśli to pierwsze uruchomienie - zakładamy nowy magazyn (vault.meta.json nie istnieje).
            bool isNewVault = !MasterPasswordService.VaultExists();
            var passwordWindow = new MasterPasswordWindow(isNewVault);
            bool? result = passwordWindow.ShowDialog();

            if (result != true || passwordWindow.DerivedKey == null)
            {
                // Użytkownik anulował albo nie podał poprawnego hasła - zamykamy program.
                Shutdown();
                return;
            }

            var mainWindow = new MainWindow(passwordWindow.DerivedKey);
            Current.MainWindow = mainWindow;
            mainWindow.Show();

            // Od teraz zamknięcie głównego okna ma normalnie kończyć program.
            ShutdownMode = ShutdownMode.OnMainWindowClose;
        }
    }
}