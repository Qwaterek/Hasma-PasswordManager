using PasswordGenerator.Models;
using System.Windows;

namespace PasswordGenerator
{
    /// <summary>
    /// Interaction logic for Shortcuts.xaml
    /// </summary>
    public partial class Shortcuts : Window
    {
        public System.Collections.ObjectModel.ObservableCollection<ShortcutItem> Items { get; } = new System.Collections.ObjectModel.ObservableCollection<ShortcutItem>();

        public Shortcuts()
        {
            InitializeComponent();

            // Localize header and intro
            try
            {
                HeaderLabel.Content = PasswordGenerator.Resources.Shortcuts_Header;
                IntroText.Text = PasswordGenerator.Resources.Shortcuts_Intro;
                ShortcutColumn.Header = PasswordGenerator.Resources.Shortcuts_ColShortcut;
                DescriptionColumn.Header = PasswordGenerator.Resources.Shortcuts_ColDescription;
                FooterLabel.Content = PasswordGenerator.Resources.Shortcuts_Footer;
            }
            catch { }

            // Populate shortcuts list from resources
            Items.Add(new ShortcutItem { Shortcut = PasswordGenerator.Resources.Shortcut_AddAccount_Shortcut, Description = PasswordGenerator.Resources.Shortcut_AddAccount_Desc });
            Items.Add(new ShortcutItem { Shortcut = PasswordGenerator.Resources.Shortcut_OpenAccounts_Shortcut, Description = PasswordGenerator.Resources.Shortcut_OpenAccounts_Desc });
            Items.Add(new ShortcutItem { Shortcut = PasswordGenerator.Resources.Shortcut_FocusSearch_Shortcut, Description = PasswordGenerator.Resources.Shortcut_FocusSearch_Desc });
            Items.Add(new ShortcutItem { Shortcut = PasswordGenerator.Resources.Shortcut_FastPassword_Shortcut, Description = PasswordGenerator.Resources.Shortcut_FastPassword_Desc });
            Items.Add(new ShortcutItem { Shortcut = PasswordGenerator.Resources.Shortcut_SaveConfig_Shortcut, Description = PasswordGenerator.Resources.Shortcut_SaveConfig_Desc });
            Items.Add(new ShortcutItem { Shortcut = PasswordGenerator.Resources.Shortcut_LoadConfig_Shortcut, Description = PasswordGenerator.Resources.Shortcut_LoadConfig_Desc });
            Items.Add(new ShortcutItem { Shortcut = PasswordGenerator.Resources.Shortcut_LoginAll_Shortcut, Description = PasswordGenerator.Resources.Shortcut_LoginAll_Desc });
            Items.Add(new ShortcutItem { Shortcut = PasswordGenerator.Resources.Shortcut_ShowShortcuts_Shortcut, Description = PasswordGenerator.Resources.Shortcut_ShowShortcuts_Desc });
            Items.Add(new ShortcutItem { Shortcut = PasswordGenerator.Resources.Shortcut_CopyToClipboard_Shortcut, Description = PasswordGenerator.Resources.Shortcut_CopyToClipboard_Desc });
            Items.Add(new ShortcutItem { Shortcut = PasswordGenerator.Resources.Shortcut_RevealPassword_Shortcut, Description = PasswordGenerator.Resources.Shortcut_RevealPassword_Desc });

            ShortcutsList.ItemsSource = Items;
        }
    }
}
