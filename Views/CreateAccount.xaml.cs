using PasswordGenerator.Helpers;
using PasswordGenerator.Models;
using System.Windows;
using System.Windows.Controls;

namespace PasswordGenerator
{
    public partial class CreateAccount : Window
    {
        public Account CreatedAccount { get; private set; }
        public bool IsDeleted { get; private set; } = false;
        private readonly Account _existingAccount;

        public CreateAccount()
        {
            InitializeComponent();
            // Localize UI elements at runtime
            try
            {
                Title = PasswordGenerator.Resources.CreateAccountTitle;
                CreateHeaderLabel.Content = PasswordGenerator.Resources.CreateAccountTitle;
                LabelUrl.Content = PasswordGenerator.Resources.CreateUrlLabel;
                LabelUsername.Content = PasswordGenerator.Resources.CreateUsernameLabel;
                LabelEmail.Content = PasswordGenerator.Resources.CreateEmailLabel;
                LabelPassword.Content = PasswordGenerator.Resources.CreatePasswordLabel;
                RefreshPasswordButton.ToolTip = PasswordGenerator.Resources.RefreshPasswordTooltip;
                GeneratePassword.Content = PasswordGenerator.Resources.GeneratePassword;
                Weak.Content = PasswordGenerator.Resources.Weak; Weak.ToolTip = PasswordGenerator.Resources.WeakTooltip;
                Medium.Content = PasswordGenerator.Resources.Medium; Medium.ToolTip = PasswordGenerator.Resources.MediumTooltip;
                Strong.Content = PasswordGenerator.Resources.Strong; Strong.ToolTip = PasswordGenerator.Resources.StrongTooltip;
                CreateFastPassword.Content = PasswordGenerator.Resources.CreateFastPassword;
                CreateFastPassword.ToolTip = PasswordGenerator.Resources.CreateFastPasswordTooltip;
                LastModifiedTextBlock.Text = PasswordGenerator.Resources.LastModifiedText;
                if (DeleteButton != null) DeleteButton.Content = PasswordGenerator.Resources.DeleteAccount;
                Back.Content = PasswordGenerator.Resources.Cancel;
            }
            catch { }
        }

        public CreateAccount(string initialPassword) : this()
        {
            if (GeneratePassword != null)
            {
                GeneratePassword.IsChecked = true;
            }

            if (CreateFastPassword != null)
            {
                CreateFastPassword.IsChecked = true;
            }
            if (!string.IsNullOrEmpty(initialPassword))
            {
                TextBox_Password.Text = initialPassword;
            }
        }
        private void ChceckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        public CreateAccount(Account accountToEdit) : this()
        {
            _existingAccount = accountToEdit;

            TextBox_URL.Text = _existingAccount.Title;
            TextBox_UserName.Text = _existingAccount.Username;
            TextBox_Email.Text = _existingAccount.Email;
            TextBox_Password.Text = _existingAccount.Password;
            LastModifiedTextBlock.Text = string.Format(PasswordGenerator.Resources.LastModifiedWithDate, _existingAccount.LastModifiedFormatted);
            LastModifiedTextBlock.Visibility = Visibility.Visible;

            Title = PasswordGenerator.Resources.EditAccountTitle;
            if (DeleteButton != null)
            {
                DeleteButton.Visibility = Visibility.Visible;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TextBox_UserName.Text) || string.IsNullOrEmpty(TextBox_Password.Text))
            {
                MessageBox.Show(PasswordGenerator.Resources.ValidationRequiredFields, PasswordGenerator.Resources.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (_existingAccount != null)
            {
                _existingAccount.Title = TextBox_URL.Text;
                _existingAccount.Username = TextBox_UserName.Text;
                _existingAccount.Email = TextBox_Email.Text;

                if (_existingAccount.Password != TextBox_Password.Text)
                {
                    _existingAccount.LastModified = DateTime.Now;
                }

                _existingAccount.Password = TextBox_Password.Text;
                CreatedAccount = _existingAccount;
            }
            else
            {
                CreatedAccount = new Account
                {
                    Title = TextBox_URL.Text,
                    Username = TextBox_UserName.Text,
                    Email = TextBox_Email.Text,
                    Password = TextBox_Password.Text,
                    LastModified = DateTime.Now
                };
            }

            DialogResult = true;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                PasswordGenerator.Resources.DeleteConfirmMessage,
                PasswordGenerator.Resources.DeleteConfirmTitle,
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                IsDeleted = true;
                DialogResult = true;
            }
        }

        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e) { }
        private void TextBox_TextChanged_2(object sender, TextChangedEventArgs e) { }
        private void TextBox_TextChanged_3(object sender, TextChangedEventArgs e) { }
        private void TextBox_TextChanged_4(object sender, TextChangedEventArgs e) { }



        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (GeneratePassword != null && GeneratePassword.IsChecked == true)
            {
                ApplyGeneratedPassword();
            }
        }
        private void GeneratePassword_Changed(object sender, RoutedEventArgs e)
        {
            if (GeneratePassword != null && GeneratePassword.IsChecked == true)
            {
                ApplyGeneratedPassword();
            }
        }

        private void ApplyGeneratedPassword()
        {
            if (CreateFastPassword != null && CreateFastPassword.IsChecked == true)
            {
                // Domyślna długość np. 12 znaków
                TextBox_Password.Text = PasswordGeneratorHelper.GenerateQuickPassword(10);
                return;
            }

            // 2. W przeciwnym wypadku generujemy domyślnie według poziomu trudności (Weak/Medium/Strong)
            PasswordGeneratorHelper.PasswordStrength strength = PasswordGeneratorHelper.PasswordStrength.Medium;
            if (Weak.IsChecked == true)
                strength = PasswordGeneratorHelper.PasswordStrength.Weak;
            else if (Strong.IsChecked == true)
                strength = PasswordGeneratorHelper.PasswordStrength.Strong;

            TextBox_Password.Text = PasswordGeneratorHelper.Generate(strength);
        }

        private void RefreshPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            if (GeneratePassword != null && GeneratePassword.IsChecked == true)
            {
                ApplyGeneratedPassword();
            }
        }
    }
}