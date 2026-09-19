using PasswordGenerator.Helpers;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace PasswordGenerator
{
    /// <summary>
    /// Interaction logic for CreateFastPassword.xaml
    /// </summary>
    public partial class CreateFastPassword : Window
    {
        public string GeneratedPassword => TextBox_Password.Text;

        public CreateFastPassword()
        {
            InitializeComponent();

            // Localize window text
            try
            {
                HeaderLabel.Content = PasswordGenerator.Resources.FastPwd_HeaderLabel;
                CopyHintLabel.Content = PasswordGenerator.Resources.FastPwd_CopyHint;
                LengthLabel.Content = PasswordGenerator.Resources.FastPwd_LengthLabel;
                RefreshPasswordButton.ToolTip = PasswordGenerator.Resources.RefreshPasswordTooltip;
            }
            catch { }

            GenerateNewPassword();
        }
        private void LengthScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            int selectedLength = (int)Math.Round(e.NewValue);

            TextBox_Length.Text = selectedLength.ToString();
            GenerateNewPassword();
        }

        private void GenerateNewPassword()
        {
            if (int.TryParse(TextBox_Length.Text, out int length))
            {
                TextBox_Password.Text = PasswordGeneratorHelper.GenerateQuickPassword(length);
            }
        }
        private void TextBox_Password_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ClipboardHelper.CopyToClipboard(TextBox_Password.Text);
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
        private void RefreshPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            GenerateNewPassword();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}