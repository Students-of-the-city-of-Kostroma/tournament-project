using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TournamentSoftware.DB_Classes;
using static TournamentSoftware.TournamentData;

namespace TournamentSoftware.wrapperClasses
{
    public class BattleProtocolWrapper: INotifyPropertyChanged
    {
        private int numberOfCurrentRound;
        private BattleProtocol battleProtocol;
        private List<Judge> selectedJudges;

        public event PropertyChangedEventHandler PropertyChanged;

        public string[] FighterName { get; private set; }
        public ObservableCollection<Judge> AvailableJudges { get; set; }
        public ObservableCollection<RoundResultWrapper> RoundResult { get; set; }

        private List<Fighter> fighter = new List<Fighter>();
        private List<Participant> participant = new List<Participant>();

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

        public BattleProtocolWrapper(BattleProtocol selectedBattleProtocol)
        {
            battleProtocol = selectedBattleProtocol;

            fighter.Add(dataBaseHandler.Query<Fighter>("SELECT * FROM Fighter WHERE id=" + battleProtocol.RedFighterId + ";")[0]);
            fighter.Add(dataBaseHandler.Query<Fighter>("SELECT * FROM Fighter WHERE id=" + battleProtocol.BlueFighterId + ";")[0]);

            participant.Add(dataBaseHandler.Query<Participant>("SELECT * FROM Participant WHERE id=" + fighter[0].ParticipantId+ ";")[0]);
            participant.Add(dataBaseHandler.Query<Participant>("SELECT * FROM Participant WHERE id=" + fighter[1].ParticipantId + ";")[0]);

            FighterName = new string[participant.Count];

            for (int i = 0; i < FighterName.Length; i++)
                FighterName[i] = participant[i].Surname + " " + participant[i].Name;

            AvailableJudges = new ObservableCollection<Judge>();
            List<Judge> availableJudges = dataBaseHandler.Query<Judge>("SELECT * FROM Judge;");
            availableJudges.ForEach((availableJudge) => AvailableJudges.Add(availableJudge));

            RoundResult = new ObservableCollection<RoundResultWrapper>();
            AddRound();
            selectedJudges = new List<Judge>();

            NumberOfCurrentRound = 1;
        }

        public void AddRound()
        {
            RoundResult.Add(new RoundResultWrapper(FighterName));
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
