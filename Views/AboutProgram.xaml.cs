using System.Diagnostics;
using System.Windows;

namespace PasswordGenerator
{
    public partial class AboutProgram : Window
    {
        public AboutProgram()
        {
            InitializeComponent();
            // Localize UI text at runtime to avoid x:Static resolution issues during XAML compile
            try
            {
                AboutHeaderLabel.Content = PasswordGenerator.Resources.AboutTitle;
                AboutDescriptionTextBlock.Text = PasswordGenerator.Resources.AboutDescription;
                AboutExtraTextBlock.Text = PasswordGenerator.Resources.AboutExtra;
                AboutFooterLabel.Content = PasswordGenerator.Resources.AboutFooter;
                GitHubButton.ToolTip = PasswordGenerator.Resources.AboutGitHubTooltip;
            }
            catch
            {
                // ignore if resources not available
            }
        }

        private void GitHubButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "https://github.com/Qwaterek",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas otwierania strony: {ex.Message}");
            }
        }
    }
}