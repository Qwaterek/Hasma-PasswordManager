using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using PasswordGenerator.Models;
using System.Diagnostics;
using System.Windows;

namespace PasswordGenerator.Services
{
    public enum BrowserType
    {
        Chrome,
        Firefox
    }

    public static class AutoLoginService
    {
        public static async Task LoginToAllAccountsAsync(IEnumerable<Account> accounts, BrowserType browserType = BrowserType.Firefox)
        {
            await Task.Run(() =>
            {
                IWebDriver driver = null;

                try
                {
                    // Inicjalizacja przeglądarki z szybkim ładowaniem (PageLoadStrategy.Eager)
                    switch (browserType)
                    {
                        case BrowserType.Firefox:
                            var firefoxOptions = new FirefoxOptions();
                            firefoxOptions.PageLoadStrategy = PageLoadStrategy.Eager;
                            driver = new FirefoxDriver(firefoxOptions);
                            break;

                        case BrowserType.Chrome:
                        default:
                            var chromeOptions = new ChromeOptions();
                            chromeOptions.PageLoadStrategy = PageLoadStrategy.Eager;
                            driver = new ChromeDriver(chromeOptions);
                            break;
                    }

                    if (driver == null) return;

                    // Bardzo krótki czas oczekiwania (brak marnowania czasu)
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(500);

                    bool isFirstAccount = true;

                    foreach (var account in accounts)
                    {
                        if (string.IsNullOrWhiteSpace(account.Title))
                            continue;

                        if (!isFirstAccount)
                        {
                            driver.SwitchTo().NewWindow(WindowType.Tab);
                        }
                        else
                        {
                            isFirstAccount = false;
                        }

                        LoginOnCurrentTab(driver, account);
                    }
                }
                catch (Exception ex)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show(
                            $"Wystąpił błąd podczas uruchamiania przeglądarki:\n\n{ex.Message}",
                            "Błąd AutoLogin",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    });
                }
            });
        }

        private static void LoginOnCurrentTab(IWebDriver driver, Account account)
        {
            try
            {
                string targetUrl = account.Title.StartsWith("http", StringComparison.OrdinalIgnoreCase)
                    ? account.Title
                    : "https://" + account.Title;

                driver.Navigate().GoToUrl(targetUrl);

                // 1. Szybkie sprawdzenie czy pola logowania są widoczne
                IWebElement userField = FindInputElementFast(driver, "user", "email", "login", "username");
                IWebElement passField = FindInputElementFast(driver, "password", "pass");

                // 2. Jeśli brakuje pól – jednorazowa próba kliknięcia "Zaloguj"
                if (userField == null || passField == null)
                {
                    if (TryClickLoginButtonFast(driver))
                    {
                        // Ponowne szybkie szukanie po kliknięciu
                        userField = FindInputElementFast(driver, "user", "email", "login", "username");
                        passField = FindInputElementFast(driver, "password", "pass");
                    }
                }

                // 3. Wpisywanie loginu / emaila
                if (userField != null)
                {
                    string identifier = !string.IsNullOrWhiteSpace(account.Username)
                        ? account.Username
                        : account.Email;

                    FastSendKeys(driver, userField, identifier);
                }

                // 4. Wpisywanie hasła i zatwierdzenie
                if (passField != null)
                {
                    FastSendKeys(driver, passField, account.Password);

                    if (!TryClickSubmitButtonFast(driver))
                    {
                        passField.SendKeys(Keys.Enter);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[AutoLoginService] Błąd na karcie {account.Title}: {ex.Message}");
            }
        }

        private static bool TryClickLoginButtonFast(IWebDriver driver)
        {
            string[] loginKeywords = { "zaloguj", "log in", "login", "sign in", "signin" };
            return FastClickElement(driver, loginKeywords);
        }

        private static bool TryClickSubmitButtonFast(IWebDriver driver)
        {
            string[] submitKeywords = { "zaloguj", "log in", "login", "sign in", "submit", "dalej" };
            return FastClickElement(driver, submitKeywords);
        }

        /// <summary>
        /// Zoptymalizowane klikanie — buduje jedno zapytanie XPath zamiast wielu w pętli.
        /// </summary>
        private static bool FastClickElement(IWebDriver driver, string[] keywords)
        {
            try
            {
                List<string> xpathConditions = new List<string>();
                foreach (var kw in keywords)
                {
                    xpathConditions.Add($"contains(translate(text(), 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), '{kw}')");
                }

                string combinedCondition = string.Join(" or ", xpathConditions);
                string combinedXPath = $"//button[{combinedCondition}] | //a[{combinedCondition}] | //input[@type='submit' or @type='button'][{combinedCondition}]";

                var elements = driver.FindElements(By.XPath(combinedXPath));

                foreach (var el in elements)
                {
                    if (el != null && el.Displayed && el.Enabled)
                    {
                        try
                        {
                            el.Click();
                        }
                        catch
                        {
                            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                            js.ExecuteScript("arguments[0].click();", el);
                        }
                        return true;
                    }
                }
            }
            catch
            {
                // Ignorujemy błędy i przechodzimy dalej
            }
            return false;
        }

        /// <summary>
        /// Łączy wszystkie słowa kluczowe w pojedyncze zapytanie XPath, ograniczając komunikację z przeglądarką.
        /// </summary>
        private static IWebElement FindInputElementFast(IWebDriver driver, params string[] keywords)
        {
            try
            {
                List<string> xpathConditions = new List<string>();
                foreach (var kw in keywords)
                {
                    xpathConditions.Add($"contains(@id, '{kw}') or contains(@name, '{kw}') or contains(@type, '{kw}')");
                }

                string combinedXPath = $"//input[{string.Join(" or ", xpathConditions)}]";
                var elements = driver.FindElements(By.XPath(combinedXPath));

                foreach (var el in elements)
                {
                    if (el != null && el.Displayed && el.Enabled)
                        return el;
                }
            }
            catch
            {
                // Ignorujemy
            }
            return null;
        }

        private static void FastSendKeys(IWebDriver driver, IWebElement element, string value)
        {
            try
            {
                element.Clear();
                element.SendKeys(value);
            }
            catch
            {
                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                js.ExecuteScript($"arguments[0].value='{value}';", element);
            }
        }
    }
}