using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PasswordGenerator.Models
{
    public enum AccessLevel
    {
        Standard,
        Readonly,
        Admin
    }
    public class Account : INotifyPropertyChanged
    {
        private int _id;
        private string _title;
        private string _username;
        private string _email;
        private string _password;
        public DateTime LastModified { get; set; } = DateTime.Now;

        public string LastModifiedFormatted => LastModified.ToString("yyyy-MM-dd HH:mm");

        public string FaviconUrl
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Title))
                    return null;


                string domain = Title.Replace("http://", "").Replace("https://", "").Split('/')[0];
                return $"https://www.google.com/s2/favicons?domain={domain}&sz=32";
            }
        }

        public int Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(); }
        }
        public string Title
        {
            get => _title;
            set { _title = value; OnPropertyChanged(); }
        }
        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }
        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); }
        }
        public string MaskedPassword => new string('*', _password?.Length ?? 0);
        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        }

    }
}
