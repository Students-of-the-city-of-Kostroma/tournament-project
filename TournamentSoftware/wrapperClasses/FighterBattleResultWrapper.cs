using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TournamentSoftware
{
    public class FighterBattleResultWrapper : INotifyPropertyChanged
    {
        private uint firstJudge;
        private uint secondJudge;
        private uint thirdJudge;
        private uint warnings;
        private uint extraPoints;
        private uint retiredFighters;
        private uint earlyEndOfTheFight;
        private uint result;

        public event PropertyChangedEventHandler PropertyChanged;

        public uint FirstJudge
        {
            get { return firstJudge; }
            set { firstJudge = value; OnPropertyChanged("FirstJudge"); }
        }

        public uint SecondJudge
        {
            get { return secondJudge; }
            set { secondJudge = value; OnPropertyChanged("SecondJudge"); }
        }

        public uint ThirdJudge
        {
            get { return thirdJudge; }
            set { thirdJudge = value; OnPropertyChanged("ThirdJudge"); }
        }

        public uint Warnings
        {
            get { return warnings; }
            set { warnings = value; OnPropertyChanged("Warnings"); }
        }

        public uint ExtraPoints
        {
            get { return extraPoints; }
            set { extraPoints = value; OnPropertyChanged("ExtraPoints"); }
        }

        public uint RetiredFighters
        {
            get { return retiredFighters; }
            set { retiredFighters = value; OnPropertyChanged("RetiredFighters"); }
        }

        public uint EarlyEndOfTheFight
        {
            get { return earlyEndOfTheFight; }
            set { earlyEndOfTheFight = value; OnPropertyChanged("EarlyEndOfTheFight"); }
        }

        public uint Result
        {
            get { return result; }
            set { result = value; OnPropertyChanged("Result"); }
        }

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
