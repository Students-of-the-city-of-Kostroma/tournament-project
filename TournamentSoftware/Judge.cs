using SQLite;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TournamentSoftware
{
    [Table("Judes")]
    public class Judge : INotifyPropertyChanged
    {
        private string name;
        private string surname;
        private string patronymic;
        private string club;
        private string city;
        private bool isCheked;

        public bool IsCheked
        {
            get { return isCheked; }
            set { isCheked = value; OnPropertyChanged("IsCheked"); }
        }
        public string Name
        {
            get { return name; }
            set { name = value; OnPropertyChanged("Name"); }
        }

        public string Surname
        {
            get { return surname; }
            set { surname = value; OnPropertyChanged("Surname"); }
        }

        public string Patronymic
        {
            get { return patronymic; }
            set { patronymic = value; OnPropertyChanged("Patronymic"); }
        }

        public string Club
        {
            get { return club; }
            set { club = value; OnPropertyChanged("Club"); }
        }

        public string City
        {
            get { return city; }
            set { city = value; OnPropertyChanged("City"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
