using PasswordGenerator.Services;
using System.Windows;
using System.Windows.Input;

namespace PasswordGenerator
{
    public partial class MasterPasswordWindow : Window
    {
        private readonly bool _isNewVault;
        private readonly bool _isChangingPassword;

        /// <summary>
        /// The AES key derived from the master password, set once DialogResult == true.
        /// </summary>
        public byte[]? DerivedKey { get; private set; }

        /// <param name="isNewVault">True when no vault.meta.json exists yet (first run) - shows the "set password" + confirm layout.</param>
        /// <param name="isChangingPassword">True when this dialog is used from "Change Master Password" rather than first-time setup - only changes the header text shown to the user.</param>
        public MasterPasswordWindow(bool isNewVault, bool isChangingPassword = false)
        {
            InitializeComponent();
            _isNewVault = isNewVault;
            _isChangingPassword = isChangingPassword;

            if (_isNewVault)
            {
                HeaderText.Text = _isChangingPassword ? PasswordGenerator.Resources.MasterPwd_ChangeHeader : PasswordGenerator.Resources.MasterPwd_NewHeader;
                SubHeaderText.Text = _isChangingPassword ? PasswordGenerator.Resources.MasterPwd_ChangeSubHeader : PasswordGenerator.Resources.MasterPwd_NewSubHeader;
                ConfirmPanel.Visibility = Visibility.Visible;
            }
            else
            {
                HeaderText.Text = PasswordGenerator.Resources.MasterPwd_UnlockHeader;
                SubHeaderText.Text = PasswordGenerator.Resources.MasterPwd_UnlockSubHeader;
                ConfirmPanel.Visibility = Visibility.Collapsed;
            }

            Loaded += (s, e) => PasswordBox1.Focus();
        }

        private void PasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TryAccept();
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e) => TryAccept();

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void TryAccept()
        {
            ErrorText.Text = string.Empty;
            string password = PasswordBox1.Password;

            if (string.IsNullOrEmpty(password))
            {
                ErrorText.Text = PasswordGenerator.Resources.MasterPwd_Err_Empty;
                return;
            }

            if (_isNewVault)
            {
                if (password.Length < 4)
                {
                    ErrorText.Text = PasswordGenerator.Resources.MasterPwd_Err_TooShort;
                    return;
                }

                if (password != PasswordBox2.Password)
                {
                    ErrorText.Text = PasswordGenerator.Resources.MasterPwd_Err_Mismatch;
                    PasswordBox2.Clear();
                    PasswordBox2.Focus();
                    return;
                }

                DerivedKey = MasterPasswordService.CreateNewVault(password);
                DialogResult = true;
                Close();
            }
            else
            {
                byte[]? key = MasterPasswordService.TryUnlock(password);
                if (key == null)
                {
                    ErrorText.Text = PasswordGenerator.Resources.MasterPwd_Err_Wrong;
                    PasswordBox1.Clear();
                    PasswordBox1.Focus();
                    return;
                }

                DerivedKey = key;
                DialogResult = true;
                Close();
            }
        }
    }
}
