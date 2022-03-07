using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using static TournamentSoftware.TournamentData;

namespace TournamentSoftware.wrapperClasses
{
    public class BattleProtocolWrapper: INotifyPropertyChanged
    {
        private int numberOfCurrentRound;
        private List<Judge> selectedJudges;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Judge> AvailableJudges { get; set; }
        public ObservableCollection<RoundResultWrapper> RoundResult { get; set; }

        public List<Judge> SelectedJudges
        {
            get { return selectedJudges; }
            set
            {
                if (selectedJudges == value) return;
                selectedJudges = value;
                OnPropertyChanged("SelectedJudges");
            }
        }

        public BattleProtocolWrapper()
        {
            AvailableJudges = new ObservableCollection<Judge>();
            List<Judge> availableJudges = dataBaseHandler.Query<Judge>("SELECT * FROM Judge;");
            availableJudges.ForEach((availableJudge) => AvailableJudges.Add(availableJudge));

            RoundResult = new ObservableCollection<RoundResultWrapper>();
            RoundResult.Add(new RoundResultWrapper());

            selectedJudges = new List<Judge>();

            NumberOfCurrentRound = 1;
        }

        public int NumberOfCurrentRound 
        { 
            get { return numberOfCurrentRound; } 
            set { numberOfCurrentRound = value; OnPropertyChanged("NumberOfCurrentRound"); } 
        }

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
