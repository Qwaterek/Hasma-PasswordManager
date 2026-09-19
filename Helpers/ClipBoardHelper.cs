using PasswordGenerator.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace PasswordGenerator.Helpers
{
    public static class ClipboardHelper
    {
        public static void CopyToClipboard(string textToCopy, string label = "hasło")
        {
            if (!string.IsNullOrEmpty(textToCopy))
            {
                Clipboard.SetText(textToCopy);
                ShowTooltipAtMouse(string.Format(PasswordGenerator.Resources.CopiedMessage, label.ToLower()));
            }
        }

        public static void HandleDataGridDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DependencyObject dep = (DependencyObject)e.OriginalSource;
            while (dep != null && !(dep is DataGridCell))
            {
                dep = VisualTreeHelper.GetParent(dep);
            }

            if (dep is DataGridCell cell && cell.DataContext is Account account)
            {
                string textToCopy = string.Empty;

                // Prefer reading the column's binding path when available (more robust than header text)
                if (cell.Column is DataGridBoundColumn boundCol)
                {
                    if (boundCol.Binding is Binding binding && binding.Path != null)
                    {
                        string path = binding.Path.Path;
                        switch (path)
                        {
                            case "Username":
                                textToCopy = account.Username;
                                break;
                            case "Email":
                                textToCopy = account.Email;
                                break;
                            case "Password":
                                textToCopy = account.Password;
                                break;
                            case "Title":
                                textToCopy = account.Title;
                                break;
                        }
                    }
                }

                // Fallback to header text comparisons (covers template columns and different localized headers)
                if (string.IsNullOrEmpty(textToCopy))
                {
                    string columnHeader = (cell.Column.Header ?? string.Empty).ToString();
                    columnHeader = columnHeader.Trim();

                    // ignore icon column
                    if (string.Equals(columnHeader, "Ikona", StringComparison.OrdinalIgnoreCase) || string.IsNullOrEmpty(columnHeader))
                        return;

                    // compare against known resource values and common English headers
                    if (string.Equals(columnHeader, PasswordGenerator.Resources.ColUser, StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(columnHeader, "User", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(columnHeader, "Username", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(columnHeader, "Nazwa Użytkownika", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(columnHeader, "Użytkownik", StringComparison.OrdinalIgnoreCase))
                    {
                        textToCopy = account.Username;
                    }
                    else if (string.Equals(columnHeader, PasswordGenerator.Resources.ColEmail, StringComparison.OrdinalIgnoreCase) ||
                             string.Equals(columnHeader, "Email", StringComparison.OrdinalIgnoreCase))
                    {
                        textToCopy = account.Email;
                    }
                    else if (string.Equals(columnHeader, PasswordGenerator.Resources.ColPassword, StringComparison.OrdinalIgnoreCase) ||
                             string.Equals(columnHeader, "Hasło", StringComparison.OrdinalIgnoreCase) ||
                             string.Equals(columnHeader, "Password", StringComparison.OrdinalIgnoreCase))
                    {
                        textToCopy = account.Password;
                    }
                    else if (string.Equals(columnHeader, PasswordGenerator.Resources.ColUrl, StringComparison.OrdinalIgnoreCase) ||
                             string.Equals(columnHeader, "URL / Serwis", StringComparison.OrdinalIgnoreCase) ||
                             string.Equals(columnHeader, "URL / Service", StringComparison.OrdinalIgnoreCase) ||
                             string.Equals(columnHeader, "URL", StringComparison.OrdinalIgnoreCase))
                    {
                        textToCopy = account.Title;
                    }
                }

                if (!string.IsNullOrEmpty(textToCopy))
                {
                    // Use the column header (if available) as label, otherwise a generic one
                    string label = (cell.Column.Header ?? "").ToString();
                    if (string.IsNullOrEmpty(label)) label = "value";
                    CopyToClipboard(textToCopy, label);
                }
            }
        }

        private static void ShowTooltipAtMouse(string message)
        {
            var popup = new Popup
            {
                Placement = PlacementMode.MousePoint,
                AllowsTransparency = true,
                IsOpen = true,
                StaysOpen = true
            };

            var border = new Border
            {
                Background = new SolidColorBrush(Color.FromArgb(230, 40, 40, 40)),
                CornerRadius = new CornerRadius(4),
                Padding = new Thickness(8, 4, 8, 4),
                Child = new TextBlock
                {
                    Text = message,
                    Foreground = Brushes.White,
                    FontSize = 12
                }
            };

            popup.Child = border;

            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(500)
            };

            timer.Tick += (s, args) =>
            {
                if (s is DispatcherTimer t)
                {
                    t.Stop();
                }
                popup.IsOpen = false;
            };

            timer.Start();
        }
    }
}