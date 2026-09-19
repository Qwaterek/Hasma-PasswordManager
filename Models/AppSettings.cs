using System.IO;
using System.Text.Json;

namespace PasswordGenerator.Models
{
    public static class AppSettings
    {
        private class Settings
        {
            public string Culture { get; set; } = "en-US";
        }

        private static readonly string SettingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");

        public static string LoadCulture()
        {
            try
            {
                if (!File.Exists(SettingsPath))
                    return "en-US";

                string json = File.ReadAllText(SettingsPath);
                var settings = JsonSerializer.Deserialize<Settings>(json);
                if (settings == null || string.IsNullOrEmpty(settings.Culture))
                    return "en-US";

                return settings.Culture;
            }
            catch
            {
                return "en-US";
            }
        }

        public static void SaveCulture(string culture)
        {
            try
            {
                var settings = new Settings { Culture = culture };
                string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(SettingsPath, json);
            }
            catch
            {
                // Ignore errors - not critical
            }
        }
    }
}
