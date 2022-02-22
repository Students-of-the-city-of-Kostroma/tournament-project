using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TournamentSoftware
{
    public class FighterRoundResultWrapper: INotifyPropertyChanged
    {
        private string rowHeader;
        private List<uint> judgeScore;
        private uint score;
        private uint difference;
        private uint point;
        private string comment;
        private uint countOfComment;
        private string note;
        private string total;

        public event PropertyChangedEventHandler PropertyChanged;
        public string RowHeader
        {
            get { return rowHeader; }
            set { rowHeader = value; OnPropertyChanged("RowHeader"); }
        }

        public List<uint> JudgeScore
        {
            get { return judgeScore; }
            set { judgeScore = value; }
        }

        public uint Score
        {
            get { return score; }
            set { score = value; OnPropertyChanged("Score"); }
        }

        public uint Difference
        {
            get { return difference; }
            set { difference = value; OnPropertyChanged("Difference"); }
        }

        public uint Point
        {
            get { return point; }
            set { point = value; OnPropertyChanged("Point"); }
        }

        public string Comment
        {
            get { return comment; }
            set { comment = value; OnPropertyChanged("Comment"); }
        }

        public uint CountOfComment
        {
            get { return countOfComment; }
            set { countOfComment = value; OnPropertyChanged("CountOfComment"); }
        }

        public string Note
        {
            get { return note; }
            set { note = value; OnPropertyChanged("Note"); }
        }

        public string Total
        {
            get { return total; }
            set { total = value; OnPropertyChanged("Total"); }
        }

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
