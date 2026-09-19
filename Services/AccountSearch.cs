using PasswordGenerator.Models;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Data;

namespace PasswordGenerator.Services
{
    public class AccountSearch
    {
        // Używamy ListCollectionView zamiast ICollectionView
        public ListCollectionView AccountsView { get; private set; }
        private readonly TextBox _searchTextBox;

        public AccountSearch(ObservableCollection<Account> accounts, TextBox searchTextBox)
        {
            _searchTextBox = searchTextBox;

            // Tworzymy NOWY, NIEZALEŻNY widok dla danej kolekcji
            AccountsView = new ListCollectionView(accounts);
            AccountsView.Filter = FilterAccounts;

            if (_searchTextBox != null)
            {
                _searchTextBox.TextChanged += (s, e) => AccountsView.Refresh();
            }
        }

        private bool FilterAccounts(object obj)
        {
            string query = _searchTextBox?.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(query))
                return true;

            if (obj is Account account)
            {
                string title = account.Title ?? string.Empty;
                string username = account.Username ?? string.Empty;
                string email = account.Email ?? string.Empty;

                bool matchesUrl = title.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0;
                bool matchesUser = username.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0;
                bool matchesEmail = email.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0;

                return matchesUrl || matchesUser || matchesEmail;
            }

            return false;
        }
    }
}