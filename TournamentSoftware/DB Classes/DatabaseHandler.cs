using System;
using SQLite;
using TournamentSoftware.DB_Classes;

namespace TournamentSoftware
{
    public class DataBaseHandler
    {
        private SQLiteConnection _db;

        public DataBaseHandler(string dataBasePath)
        {
            _db = new SQLiteConnection(dataBasePath);

            _db.CreateTable<TournamentGrid>();
            _db.CreateTable<Nomination>();
            _db.CreateTable<Category>();
            _db.CreateTable<Group>();
            _db.CreateTable<Subgroup>();
            _db.CreateTable<Club>();
            _db.CreateTable<Participant>();
            _db.CreateTable<Judge>();
            _db.CreateTable<Fighter>();
            _db.CreateTable<BattleProtocol>();
            _db.CreateTable<Round>();
            _db.CreateTable<JudgeNote>();
            _db.CreateTable<FighterRoundResult>();
            _db.CreateTable<FighterRoundResult_Punishment>();
            _db.CreateTable<Punishment>();
        }

        public dynamic GetData(string request)
        {
            return _db.Query<TournamentGrid>(request);
        }

        public void AddTournamentGrid(TournamentGrid tournamentGrid)
        {
            _db.Insert(tournamentGrid);
        }

        public void GetTournamentGrids()
        {
            var tournamentGrids = _db.Query<TournamentGrid>("SELECT * FROM TournamentGrid");
            foreach (var tournamentGrid in tournamentGrids)
            {
                Console.WriteLine(tournamentGrid.ToString());
            }
        }

        public void AddNomination(Nomination nomination)
        {
            _db.Insert(nomination);
        }

        public void GetNominations()
        {
            var nominations = _db.Query<Nomination>("SELECT * FROM Nomination");
            foreach (var nomination in nominations)
            {
                Console.WriteLine(nomination.ToString());
            }
        }

        public void AddCategory(Category category)
        {
            _db.Insert(category);
        }

        public void GetCategorys()
        {
            var categorys = _db.Query<Category>("SELECT * FROM Category");
            foreach (var category in categorys)
            {
                Console.WriteLine(category.ToString());
            }
        }

        public void AddGroup(Group group)
        {
            _db.Insert(group);
        }

        public void GetGroups()
        {
            var groups = _db.Query<Group>("SELECT * FROM Group");
            foreach (var group in groups)
            {
                Console.WriteLine(group.ToString());
            }
        }

        public void AddSubgroup(Subgroup subgroup)
        {
            _db.Insert(subgroup);
        }

        public void GetSubgroups()
        {
            var subgroups = _db.Query<Subgroup>("SELECT * FROM Subgroup");
            foreach (var subgroup in subgroups)
            {
                Console.WriteLine(subgroup.ToString());
            }
        }

        public void AddClub(Club club)
        {
            _db.Insert(club);
        }

        public void GetClubs()
        {
            var clubs = _db.Query<Club>("SELECT * FROM Club");
            foreach (var club in clubs)
            {
                Console.WriteLine(club.ToString());
            }
        }

        public void AddParticipant(Participant participant)
        {
            _db.Insert(participant);
        }

        public void GetParticipants()
        {
            var participants = _db.Query<Participant>("SELECT * FROM Participant");
            foreach (var participant in participants)
            {
                Console.WriteLine(participant.ToString());
            }
        }

        public void AddJudge(Judge judge)
        {
            _db.Insert(judge);
        }

        public void GetJudges()
        {
            var judges = _db.Query<Judge>("SELECT * FROM Judge");
            foreach (var judge in judges)
            {
                Console.WriteLine(judge.ToString());
            }
        }

        public void AddFighter(Fighter fighter)
        {
            _db.Insert(fighter);
        }

        public void GetFighters()
        {
            var fighters = _db.Query<Fighter>("SELECT * FROM Fighter");
            foreach (var fighter in fighters)
            {
                Console.WriteLine(fighter.ToString());
            }
        }

        public void AddBattleProtocol(BattleProtocol tournamentGrid)
        {
            _db.Insert(tournamentGrid);
        }

        public void GetBattleProtocols()
        {
            var battleProtocols = _db.Query<BattleProtocol>("SELECT * FROM BattleProtocol");
            foreach (var battleProtocol in battleProtocols)
            {
                Console.WriteLine(battleProtocol.ToString());
            }
        }

        public void AddRound(Round round)
        {
            _db.Insert(round);
        }

        public void GetRounds()
        {
            var rounds = _db.Query<Round>("SELECT * FROM Round");
            foreach (var round in rounds)
            {
                Console.WriteLine(round.ToString());
            }
        }

        public void AddJudgeNote(JudgeNote judgeNote)
        {
            _db.Insert(judgeNote);
        }

        public void GetJudgeNotes()
        {
            var judgeNotes = _db.Query<JudgeNote>("SELECT * FROM JudgeNote");
            foreach (var judgeNote in judgeNotes)
            {
                Console.WriteLine(judgeNote.ToString());
            }
        }

        public void AddFighterRoundResult(FighterRoundResult fighterRoundResult)
        {
            _db.Insert(fighterRoundResult);
        }

        public void GetFighterRoundResults()
        {
            var fighterRoundResults = _db.Query<FighterRoundResult>("SELECT * FROM FighterRoundResult");
            foreach (var fighterRoundResult in fighterRoundResults)
            {
                Console.WriteLine(fighterRoundResult.ToString());
            }
        }

        public void AddFighterRoundResult_Punishment(FighterRoundResult_Punishment fighterRoundResult_Punishment)
        {
            _db.Insert(fighterRoundResult_Punishment);
        }

        public void GetFighterRoundResult_Punishments()
        {
            var fighterRoundResult_Punishments = _db.Query<FighterRoundResult_Punishment>("SELECT * FROM FighterRoundResult_Punishment");
            foreach (var fighterRoundResult_Punishment in fighterRoundResult_Punishments)
            {
                Console.WriteLine(fighterRoundResult_Punishment.ToString());
            }
        }

        public void AddPunishment(Punishment punishment)
        {
            _db.Insert(punishment);
        }

        public void GetPunishments()
        {
            var punishments = _db.Query<Punishment>("SELECT * FROM Punishment");
            foreach (var punishment in punishments)
            {
                Console.WriteLine(punishment.ToString());
            }
        }
    }
}