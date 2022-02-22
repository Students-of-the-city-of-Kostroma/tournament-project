using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentSoftware.wrapperClasses
{
    public class RoundResultWrapper
    {
        public ObservableCollection<FighterRoundResultWrapper> FighterRoundResult { get; private set; }
        public RoundResultWrapper()
        {
            FighterRoundResult = new ObservableCollection<FighterRoundResultWrapper>();
            for (int i = 0; i < 2; i++)
            {
                FighterRoundResult.Add(new FighterRoundResultWrapper()
                {
                    RowHeader = "Фамилия И.",
                    JudgeScore = new List<uint>(),
                    Score = 0,
                    Difference = 0,
                    Point = 0,
                    Comment = "",
                    CountOfComment = 0,
                    Note = "",
                    Total = ""
                });
            }
        }
    }
}
