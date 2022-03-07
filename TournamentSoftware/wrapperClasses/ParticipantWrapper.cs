using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TournamentSoftware.DB_Classes;

namespace TournamentSoftware
{
    public class ParticipantWrapper : INotifyPropertyChanged
    {
        private string club;
        private string city;
        private string category;
        private Dictionary<string, bool> nominations = new Dictionary<string, bool>();

        public string Category
        {
            get { return category; }
            set { category = value; OnPropertyChanged("Category"); }
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

        public Dictionary<string, bool> Nominations
        {
            get { return nominations; }
            set { nominations = value; OnPropertyChanged("Nominations"); }
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

        public bool RequestedToNomination(string nominationName)
        {
            return nominations.ContainsKey(nominationName) && nominations[nominationName];
        }
    }
}
