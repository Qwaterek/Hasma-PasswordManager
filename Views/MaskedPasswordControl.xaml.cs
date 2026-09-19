using PasswordGenerator.Models;
using System.Windows.Controls;
using System.Windows.Input;

namespace PasswordGenerator
{
    public partial class MaskedPasswordControl : UserControl
    {
        public MaskedPasswordControl()
        {
            InitializeComponent();
        }
        private void Password_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock textBlock && textBlock.DataContext is Account account)
            {
                textBlock.Text = account.Password;
                e.Handled = true;
            }
        }
        private void Password_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock textBlock && textBlock.DataContext is Account account)
            {
                textBlock.Text = account.MaskedPassword;
                e.Handled = true;
            }
        }
        private void Password_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is TextBlock textBlock && textBlock.DataContext is Account account)
            {
                textBlock.Text = account.MaskedPassword;

            }
        }
    }
}