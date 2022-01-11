using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TournamentSoftware
{
    public class NominationFormModel : INotifyPropertyChanged
    {
        public NominationFormModel(string nominationName)
        {
            Nomination.Name = nominationName;
        }

        public NominationFormModel()
        {
        }

        private Nomination _nomination = new Nomination()
        {
            Name = "",
            Id = 0,
            ParticipantId = 0,
        };
        private bool _isSelected;

        public Nomination Nomination
        {
            get { return _nomination; }
            set { _nomination = value; OnPropertyChanged("Nomination"); }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set { _isSelected = value; OnPropertyChanged("IsSelected"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
