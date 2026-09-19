using PasswordGenerator.Models;
using PasswordGenerator.Services;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace PasswordGenerator
{
    public partial class AccountPanel : Window
    {
        public ObservableCollection<Account> AccountsList { get; set; }
        private readonly AccountSearch _accountSearch;
        private readonly byte[] _vaultKey;

        public AccountPanel(ObservableCollection<Account> sharedList, byte[] vaultKey)
        {
            InitializeComponent();
            AccountsList = sharedList;
            _vaultKey = vaultKey;

            _accountSearch = new AccountSearch(AccountsList, SearchTextBox);
            G_Accounts_Panel.ItemsSource = _accountSearch.AccountsView;
            // Localize headers and buttons at runtime
            try
            {
                UrlColumn.Header = PasswordGenerator.Resources.AccountPanel_UrlHeader;
                UserColumn.Header = PasswordGenerator.Resources.AccountPanel_UserHeader;
                EmailColumn.Header = PasswordGenerator.Resources.AccountPanel_EmailHeader;
                PasswordColumn.Header = PasswordGenerator.Resources.AccountPanel_PasswordHeader;
                AddAccountButton.Content = PasswordGenerator.Resources.AccountPanel_AddButton;
                AddAccountButton.ToolTip = PasswordGenerator.Resources.AccountPanel_AddButtonTooltip;
                SearchPlaceholderTextBlock.Text = PasswordGenerator.Resources.SearchPlaceholder;
            }
            catch { }
        }

        private void G_Accounts_Panel_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (G_Accounts_Panel.SelectedItem is Account selectedAccount)
            {
                var dialog = new CreateAccount(selectedAccount) { Owner = this };

                if (dialog.ShowDialog() == true)
                {
                    if (dialog.IsDeleted)
                    {
                        AccountsList.Remove(selectedAccount);
                    }
                    AccountStorage.SaveAccounts(AccountsList, _vaultKey);
                    _accountSearch.AccountsView.Refresh();
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var createAccountWindow = new CreateAccount { Owner = this };
            if (createAccountWindow.ShowDialog() == true && createAccountWindow.CreatedAccount != null)
            {
                AccountsList.Add(createAccountWindow.CreatedAccount);
                AccountStorage.SaveAccounts(AccountsList, _vaultKey);
            }
        }
    }
}