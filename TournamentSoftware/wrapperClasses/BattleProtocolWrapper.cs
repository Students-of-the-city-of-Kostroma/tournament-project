using System.Collections.Generic;
using System.Collections.ObjectModel;
using static TournamentSoftware.TournamentData;

namespace TournamentSoftware.wrapperClasses
{
    public class BattleProtocolWrapper
    {
        public ObservableCollection<string> AvailableJudges { get; private set; }
        public ObservableCollection<FighterBattleResultWrapper> FighterBattleResult { get; private set; }

        public BattleProtocolWrapper()
        {
            AvailableJudges = new ObservableCollection<string>();
            FighterBattleResult = new ObservableCollection<FighterBattleResultWrapper>();

            List<Judge> availableJudges = dataBaseHandler.GetJudgesData("SELECT * FROM Judge;");
            availableJudges.ForEach(( availableJudge) => AvailableJudges.Add(availableJudge.Surname));

            for (int i = 0; i < 2; i++)
            {
                FighterBattleResult.Add( new FighterBattleResultWrapper()
                {
                    FirstJudge = 0,
                    SecondJudge = 0,
                    ThirdJudge = 0,
                    Warnings = 0,
                    ExtraPoints = 0,
                    RetiredFighters = 0,
                    EarlyEndOfTheFight = 0,
                    Result = 0
                });
            }
        }
    }
}
