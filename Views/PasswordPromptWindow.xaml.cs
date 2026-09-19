using System.Windows;
using System.Windows.Input;

namespace PasswordGenerator
{
    public partial class PasswordPromptWindow : Window
    {
        private readonly bool _requireConfirmation;
        private const int MinLength = 6;

        /// <summary>
        /// The password entered by the user, set once DialogResult == true.
        /// </summary>
        public string Password { get; private set; } = string.Empty;

        public PasswordPromptWindow(bool requireConfirmation, string headerText, string subHeaderText)
        {
            InitializeComponent();
            _requireConfirmation = requireConfirmation;

            Title = headerText;
            HeaderText.Text = headerText;
            SubHeaderText.Text = subHeaderText;
            ConfirmPanel.Visibility = requireConfirmation ? Visibility.Visible : Visibility.Collapsed;

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
                ErrorText.Text = PasswordGenerator.Resources.PwdPrompt_Err_Empty;
                return;
            }

            if (_requireConfirmation)
            {
                if (password.Length < MinLength)
                {
                    ErrorText.Text = PasswordGenerator.Resources.PwdPrompt_Err_TooShort;
                    return;
                }

                if (password != PasswordBox2.Password)
                {
                    ErrorText.Text = PasswordGenerator.Resources.PwdPrompt_Err_Mismatch;
                    PasswordBox2.Clear();
                    PasswordBox2.Focus();
                    return;
                }
            }

            Password = password;
            DialogResult = true;
            Close();
        }
    }
}
