using System.Text;

namespace PasswordGenerator.Helpers
{
    public static class PasswordGeneratorHelper
    {
        private static readonly Random _random = new Random();

        private static readonly Dictionary<char, char> LeetReplacements = new Dictionary<char, char>
        {
            { 'a', '4' },
            { 'e', '3' },
            { 'i', '1' },
            { 'o', '0' },
            { 's', '5' },
            { 't', '7' }
        };

        private static readonly string[] EasyWords = new string[]
        {
            // Zwierzęta (25)
            "pies", "kot", "ptak", "ryba", "kon", "krowa", "owca", "wilk", "lis", "zajac",
            "niedzwiedz", "sokol", "orzel", "motyl", "pszczola", "pajak", "mucha", "zaba", "zolw", "smok",
            "jelen", "sarna", "borsuk", "jeza", "kura",

            // Rzeczy / Przymiotniki (25)
            "biurko", "krzeslo", "lampa", "zegar", "obraz", "lustro", "torba", "plecak", "klucz", "telefon",
            "ekran", "myszka", "okno", "drzwi", "cieply", "zimny", "szybki", "wolny", "duzy", "maly",
            "dobry", "nowy", "jasny", "ciemny", "mlody",

            // Wypieki (25)
            "chleb", "bulka", "paczek", "drozdzowka", "rogal", "sernik", "makowiec", "szarlotka", "piernik", "tort",
            "babka", "tart", "paczek", "ptys", "eklerka", "muffin", "beza", "sznek", "paczus", "paluch",
            "chlebek", "strucla", "wafelek", "biszkopt", "krakers",

            // Miasta (10)
            "warszawa", "krakow", "wroclaw", "poznan", "gdansk", "szczecin", "bydgoszcz", "lublin", "katowice", "bialystok",

            // Owoce i Warzywa (15)
            "jablko", "gruszka", "banan", "cytryna", "marchew", "ogorek", "pomidor", "cebula", "czosnek", "ziemniak",
            "sliwka", "truskawka", "malina", "borowka", "kapusta",

            // Natura / Inne (25)
            "pogoda", "slonce", "deszcz", "chmura", "wiatr", "ziemia", "woda", "ogien", "kamien", "drzewo",
            "kwiat", "trawa", "las", "gora", "rzeka", "morze", "jezioro", "piasek", "wiosna", "lato",
            "jesien", "zima", "dzien", "noc", "gwiazda"
        };

        private const string Lowercase = "abcdefghijklmnopqrstuvwxyz";
        private const string Uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string Digits = "0123456789";
        private const string SpecialCharacters = "!@#$%^&*()-+";

        public enum PasswordStrength
        {
            Weak,
            Medium,
            Strong
        }

        public static string Generate(PasswordStrength strength)
        {
            string charSet = Lowercase + Uppercase;
            int length = 8;

            switch (strength)
            {
                case PasswordStrength.Weak:
                    length = 8;
                    charSet = Lowercase + Uppercase;
                    break;

                case PasswordStrength.Medium:
                    length = 12;
                    charSet = Lowercase + Uppercase + Digits;
                    break;

                case PasswordStrength.Strong:
                    length = 16;
                    charSet = Lowercase + Uppercase + Digits + SpecialCharacters;
                    break;
            }

            StringBuilder sb = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                int index = _random.Next(charSet.Length);
                sb.Append(charSet[index]);
            }

            return sb.ToString();
        }

        public static string GenerateQuickPassword(int targetLength)
        {
            StringBuilder raw = new StringBuilder();

            while (raw.Length < targetLength)
            {
                string word = EasyWords[_random.Next(EasyWords.Length)];
                raw.Append(word);
            }

            string baseResult = raw.ToString().Substring(0, targetLength);
            char[] chars = baseResult.ToCharArray();

            for (int i = 0; i < chars.Length; i++)
            {
                char c = chars[i];

                if (LeetReplacements.ContainsKey(c) && _random.Next(100) < 10)
                {
                    chars[i] = LeetReplacements[c];
                }
                else if (char.IsLetter(c) && _random.Next(100) < 25)
                {
                    chars[i] = char.ToUpper(c);
                }
            }

            return new string(chars);
        }

        public static string RefreshPassword(PasswordStrength strength)
        {
            return Generate(strength);
        }
    }
}