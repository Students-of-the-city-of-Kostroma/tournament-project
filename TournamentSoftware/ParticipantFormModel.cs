using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TournamentSoftware
{
    public class ParticipantFormModel : INotifyPropertyChanged
    {
        private string _Club;
        private string _City;
        private string _Kategory;

        public string Kategory
        {
            get { return _Kategory; }
            set { _Kategory = value; OnPropertyChanged("Kategory"); }

        }
        public string Club
        {
            get { return _Club; }
            set { _Club = value; OnPropertyChanged("Club"); }

        }
        public string City
        {
            get { return _City; }
            set { _City = value; OnPropertyChanged("City"); }

        }

        private Participant participant = new Participant() 
        {
            Name = "",
            Surname = "",
            Patronymic = "",
            Pseudonym = "",
            Leader = false,
            DateOfBirth = 0,
            Height = 0,
            Weight = 0,
            CommonRating = 0,
            ClubRating = 0,
            ClubId = 0,
        };
        public Participant Participant {
            get { return participant; }
            set { participant = value; OnPropertyChanged("Participant"); }
        }

        private Dictionary<string, bool> _Nominations = new Dictionary<string, bool>();
        public Dictionary<string, bool> Nominations
        {
            get { return _Nominations; }
            set { _Nominations = value; OnPropertyChanged("Nominations"); }
        }

        private string[] availableSex = new string[2] { "М", "Ж" };
        public string[] AvailableSex
        {
            get { return availableSex; }
            set { availableSex = value; OnPropertyChanged("AvailableSex"); }
        }

        private bool _IsSelected;

        public bool IsSelected
        {
            get { return _IsSelected; }
            set { _IsSelected = value; OnPropertyChanged("IsSelected"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
