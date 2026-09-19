using PasswordGenerator.Helpers;
using PasswordGenerator.Models;
using PasswordGenerator.Services;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace PasswordGenerator
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<Account> SharedAccountsList { get; set; } = new ObservableCollection<Account>();
        private AccountSearch _accountSearch;

        // Klucz AES-256 wyprowadzony z hasła głównego (patrz MasterPasswordService),
        // przekazywany z App.xaml.cs po odblokowaniu magazynu.
        private byte[] _vaultKey;

        public MainWindow(byte[] vaultKey)
        {
            InitializeComponent();
            _vaultKey = vaultKey;
            SharedAccountsList = AccountStorage.LoadAccounts(_vaultKey);
            MainDataGrid.ItemsSource = SharedAccountsList;
            var view = CollectionViewSource.GetDefaultView(SharedAccountsList);
            _accountSearch = new AccountSearch(SharedAccountsList, SearchTextBox);
            MainDataGrid.ItemsSource = _accountSearch.AccountsView;
            this.Closing += MainWindow_Closing;
            // Update language menu check state
            try
            {
                string current = AppSettings.LoadCulture();
                LangEnglishMenuItem.IsChecked = current == "en-US";
                LangPolishMenuItem.IsChecked = current == "pl-PL";
                // set headers from resources
                LangEnglishMenuItem.Header = PasswordGenerator.Resources.LangEnglish;
                LangPolishMenuItem.Header = PasswordGenerator.Resources.LangPolish;
            }
            catch
            {
                // ignore if menu items not present or error
            }
        }

        // Command handlers for keyboard shortcuts
        private void Cmd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Cmd_OpenAccounts_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MenuItem_Click_konta(this, new RoutedEventArgs());
        }

        private void Cmd_AddAccount_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MenuItem_Click_DodajNoweKonto(this, new RoutedEventArgs());
        }

        private void Cmd_FastPassword_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MenuItem_Click_WygenerujSzybkieHaslo(this, new RoutedEventArgs());
        }

        private void Cmd_SaveConfig_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MenuItem_SaveConfig_Click(this, new RoutedEventArgs());
        }

        private void Cmd_LoadConfig_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MenuItem_LoadConfig_Click(this, new RoutedEventArgs());
        }

        private void Cmd_LoginAll_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MenuItem_Click_ZalogujWszystkie(this, new RoutedEventArgs());
        }

        private void Cmd_ShowShortcuts_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // Show Shortcuts window
            Shortcuts s = new Shortcuts { Owner = this };
            s.ShowDialog();
        }

        private void OpenUrl_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Account account)
            {
                UrlLauncher.OpenAccountUrl(account);
            }
        }

        private void MainDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ClipboardHelper.HandleDataGridDoubleClick(sender, e);
        }
        //skroty klawiszowe
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !SearchTextBox.IsFocused)
            {
                SearchTextBox.Focus();
                SearchTextBox.SelectAll();
                e.Handled = true;
            }
        }

        // --- Menu górne ---

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            AccountStorage.SaveAccounts(SharedAccountsList, _vaultKey);
        }

        private void MenuItem_Click_konta(object sender, RoutedEventArgs e)
        {
            AccountPanel panel = new AccountPanel(SharedAccountsList, _vaultKey);
            panel.Owner = this;
            panel.ShowDialog();
            AccountStorage.SaveAccounts(SharedAccountsList, _vaultKey);
        }

        private void MenuItem_Click_DodajNoweKonto(object sender, RoutedEventArgs e)
        {
            CreateAccount createAccount = new CreateAccount();
            createAccount.Owner = this;

            if (createAccount.ShowDialog() == true && createAccount.CreatedAccount != null)
            {
                SharedAccountsList.Add(createAccount.CreatedAccount);
                AccountStorage.SaveAccounts(SharedAccountsList, _vaultKey);
            }
        }

        // Obsługa wygenerowania szybkiego hasła i przekazania go do okna CreateAccount
        private void MenuItem_Click_WygenerujSzybkieHaslo(object sender, RoutedEventArgs e)
        {
            CreateFastPassword fastPasswordWindow = new CreateFastPassword
            {
                Owner = this
            };
            if (fastPasswordWindow.ShowDialog() == true)
            {
                string generatedPassword = fastPasswordWindow.GeneratedPassword;

                CreateAccount createAccountWindow = new CreateAccount(generatedPassword)
                {
                    Owner = this
                };
                if (createAccountWindow.ShowDialog() == true && createAccountWindow.CreatedAccount != null)
                {
                    SharedAccountsList.Add(createAccountWindow.CreatedAccount);
                    AccountStorage.SaveAccounts(SharedAccountsList, _vaultKey);
                }
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        { }

        private void MenuItem_SaveConfig_Click(object sender, RoutedEventArgs e)
        {
            // Prosimy o hasło, którym zostanie zaszyfrowany plik eksportu.
            // Plik NIE zawiera klucza - bez tego hasła nikt go nie odczyta.
            var passwordWindow = new PasswordPromptWindow(
                requireConfirmation: true,
                headerText: PasswordGenerator.Resources.ExportPwd_Header,
                subHeaderText: PasswordGenerator.Resources.ExportPwd_SubHeader)
            {
                Owner = this
            };

            if (passwordWindow.ShowDialog() != true)
            {
                return; // Użytkownik zrezygnował z zapisu
            }

            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Plik konfiguracyjny JSON (*.json)|*.json",
                DefaultExt = "json",
                FileName = "config_export.json"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                // Wczytujemy aktualną listę kont z pamięci/pliku roboczego
                ObservableCollection<Account> currentAccounts = AccountStorage.LoadAccounts(_vaultKey);

                // Zapisujemy plik konfiguracyjny zaszyfrowany kluczem wyprowadzonym z hasła
                ConfigManager.SaveConfigFile(currentAccounts, saveFileDialog.FileName, passwordWindow.Password);

                MessageBox.Show(
                    PasswordGenerator.Resources.ExportPwd_SuccessMsg,
                    PasswordGenerator.Resources.SuccessTitle,
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }

        // Wczytywanie przez ConfigManager i zapis do AccountStorage
        private void MenuItem_LoadConfig_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Plik konfiguracyjny JSON (*.json)|*.json",
                DefaultExt = "json"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                // 1. Prosimy o hasło, którym ten plik został zaszyfrowany przy eksporcie
                var passwordWindow = new PasswordPromptWindow(
                    requireConfirmation: false,
                    headerText: PasswordGenerator.Resources.ImportPwd_Header,
                    subHeaderText: PasswordGenerator.Resources.ImportPwd_SubHeader)
                {
                    Owner = this
                };

                if (passwordWindow.ShowDialog() != true)
                {
                    return;
                }

                // 2. Odczytujemy konta z pliku konfiguracyjnego kluczem wyprowadzonym z hasła
                ObservableCollection<Account>? importedAccounts =
                    ConfigManager.LoadConfigFile(openFileDialog.FileName, passwordWindow.Password);

                if (importedAccounts != null && importedAccounts.Count > 0)
                {
                    // 3. Wyszyszczenie i napełnienie głównej kolekcji (SharedAccountsList)
                    // Dzięki temu DataGrid i wyszukiwarka automatycznie wykryją nowe dane!
                    SharedAccountsList.Clear();
                    foreach (var account in importedAccounts)
                    {
                        SharedAccountsList.Add(account);
                    }

                    // 4. Natychmiastowy zapis nową wersją zaszyfrowaną do pliku roboczego accounts.json
                    AccountStorage.SaveAccounts(SharedAccountsList, _vaultKey);

                    MessageBox.Show(string.Format(PasswordGenerator.Resources.ImportPwd_SuccessMsg, SharedAccountsList.Count),
                                    PasswordGenerator.Resources.SuccessTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show(PasswordGenerator.Resources.ImportPwd_FailMsg, PasswordGenerator.Resources.WarningTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private async void MenuItem_Click_ZalogujWszystkie(object sender, RoutedEventArgs e)
        {
            if (SharedAccountsList == null || SharedAccountsList.Count == 0)
            {
                MessageBox.Show("Brak zapisanych kont do zalogowania.", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var confirmResult = MessageBox.Show(
                $"Czy na pewno chcesz otworzyć i zalogować się do {SharedAccountsList.Count} serwisów jednocześnie?",
                "Potwierdzenie",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirmResult == MessageBoxResult.Yes)
            {
                // Wywołanie dedykowanego serwisu
                await AutoLoginService.LoginToAllAccountsAsync(SharedAccountsList);
            }
        }

        private void MenuItem_Click_OProgramie(object sender, RoutedEventArgs e)
        {
            AboutProgram aboutWindow = new AboutProgram
            {
                Owner = this
            };
            aboutWindow.ShowDialog();
        }
        private void SetLanguage_English_Click(object sender, RoutedEventArgs e)
        {
            App.ChangeCulture("en-US");
            MessageBox.Show("Language changed to English. Reopen window to apply all changes.", "Language", MessageBoxButton.OK, MessageBoxImage.Information);
            try { LangEnglishMenuItem.IsChecked = true; LangPolishMenuItem.IsChecked = false; } catch { }
        }

        private void SetLanguage_Polish_Click(object sender, RoutedEventArgs e)
        {
            App.ChangeCulture("pl-PL");
            MessageBox.Show("Zmieniono język na Polski. Otwórz okno ponownie, aby zastosować zmiany.", "Język", MessageBoxButton.OK, MessageBoxImage.Information);
            try { LangEnglishMenuItem.IsChecked = false; LangPolishMenuItem.IsChecked = true; } catch { }
        }
        private void MenuItem_Click_SkrotyKlawiszowe(object sender, RoutedEventArgs e)
        {
        }

        private void MenuItem_ChangeMasterPassword_Click(object sender, RoutedEventArgs e)
        {
            // Krok 1: potwierdzamy tożsamość - prosimy o AKTUALNE hasło główne
            var currentPasswordWindow = new PasswordPromptWindow(
                requireConfirmation: false,
                headerText: PasswordGenerator.Resources.ChangeMasterPwd_CurrentHeader,
                subHeaderText: PasswordGenerator.Resources.ChangeMasterPwd_CurrentSubHeader)
            {
                Owner = this
            };

            if (currentPasswordWindow.ShowDialog() != true)
            {
                return;
            }

            if (MasterPasswordService.TryUnlock(currentPasswordWindow.Password) == null)
            {
                MessageBox.Show(PasswordGenerator.Resources.ChangeMasterPwd_WrongCurrent, PasswordGenerator.Resources.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Krok 2: prosimy o NOWE hasło (z potwierdzeniem) - ten sam formularz co przy pierwszym uruchomieniu,
            // ale w trybie "zmiana hasła" (inny nagłówek). Zaakceptowanie od razu generuje nową sól
            // i nadpisuje vault.meta.json nowym weryfikatorem.
            var newPasswordWindow = new MasterPasswordWindow(isNewVault: true, isChangingPassword: true)
            {
                Owner = this
            };

            if (newPasswordWindow.ShowDialog() != true || newPasswordWindow.DerivedKey == null)
            {
                return;
            }

            // Krok 3: dane kont mamy już w pamięci (SharedAccountsList) - wystarczy zapisać je
            // ponownie, tym razem nowym kluczem, i zapamiętać go jako aktualny klucz sesji.
            _vaultKey = newPasswordWindow.DerivedKey;
            AccountStorage.SaveAccounts(SharedAccountsList, _vaultKey);

            MessageBox.Show(PasswordGenerator.Resources.ChangeMasterPwd_Success, PasswordGenerator.Resources.SuccessTitle, MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}