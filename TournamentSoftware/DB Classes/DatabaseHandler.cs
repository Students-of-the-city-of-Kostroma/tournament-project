using System;
using System.Collections.Generic;
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
            _db.CreateTable<TournamentGroup>();
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

        //public dynamic GetData(string request)
        //{
        //    return _db.Query(request);
        //}

        public void AddTournamentGrid(TournamentGrid tournamentGrid)
        {
            _db.Insert(tournamentGrid);
        }

        public List<TournamentGrid> GetTournamentGridsData(string request)
        {
            List<TournamentGrid> tournamentGrids = _db.Query<TournamentGrid>(request);
            return tournamentGrids;
        }

        public void AddNomination(Nomination nomination)
        {
            _db.Insert(nomination);
        }

        public List<Nomination> GetNominationsData(string request)
        {
            List<Nomination> nominations = _db.Query<Nomination>(request);
            return nominations;
        }

        public void AddCategory(Category category)
        {
            _db.Insert(category);
        }

        public List<Category> GetCategorysData(string request)
        {
            List<Category> categorys = _db.Query<Category>(request);
            return categorys;
        }

        public void AddGroup(TournamentGroup group)
        {
            _db.Insert(group);
        }

        public List<TournamentGroup> GetGroupsData(string request)
        {
            List<TournamentGroup> groups = _db.Query<TournamentGroup>(request);
            return groups;
        }

        public void AddSubgroup(Subgroup subgroup)
        {
            _db.Insert(subgroup);
        }

        public List<Subgroup> GetSubgroupsData(string request)
        {
            List<Subgroup> subgroups = _db.Query<Subgroup>(request);
            return subgroups;
        }

        public void AddClub(Club club)
        {
            _db.Insert(club);
        }

        public List<Club> GetClubsData(string request)
        {
            List<Club> clubs = _db.Query<Club>(request);
            return clubs;
        }

        public void AddParticipant(Participant participant)
        {
            _db.Insert(participant);
        }

        public List<Participant> GetParticipantsData(string request)
        {
            List<Participant> participants = _db.Query<Participant>(request);
            return participants;
        }

        public void AddJudge(Judge judge)
        {
            _db.Insert(judge);
        }

        public List<Judge> GetJudgesData(string request)
        {
            List<Judge> judges = _db.Query<Judge>(request);
            return judges;
        }

        public void AddFighter(Fighter fighter)
        {
            _db.Insert(fighter);
        }

        public List<Fighter> GetFightersData(string request)
        {
            List<Fighter> fighters = _db.Query<Fighter>(request);
            return fighters;
        }

        public void AddBattleProtocol(BattleProtocol tournamentGrid)
        {
            _db.Insert(tournamentGrid);
        }

        public List<BattleProtocol> GetBattleProtocolsData(string request)
        {
            List<BattleProtocol> battleProtocols = _db.Query<BattleProtocol>(request);
            return battleProtocols;
        }

        public void AddRound(Round round)
        {
            _db.Insert(round);
        }

        public List<Round> GetRoundsData(string request)
        {
            List<Round> rounds = _db.Query<Round>(request);
            return rounds;
        }

        public void AddJudgeNote(JudgeNote judgeNote)
        {
            _db.Insert(judgeNote);
        }

        public List<JudgeNote> GetJudgeNotesData(string request)
        {
            List<JudgeNote> judgeNotes = _db.Query<JudgeNote>(request);
            return judgeNotes;
        }

        public void AddFighterRoundResult(FighterRoundResult fighterRoundResult)
        {
            _db.Insert(fighterRoundResult);
        }

        public List<FighterRoundResult> GetFighterRoundResultsData(string request)
        {
            List<FighterRoundResult> fighterRoundResults = _db.Query<FighterRoundResult>(request);
            return fighterRoundResults;
        }

        public void AddFighterRoundResult_Punishment(FighterRoundResult_Punishment fighterRoundResult_Punishment)
        {
            _db.Insert(fighterRoundResult_Punishment);
        }

        public List<FighterRoundResult_Punishment> GetFighterRoundResult_PunishmentsData(string request)
        {
            List<FighterRoundResult_Punishment> fighterRoundResult_Punishments = _db.Query<FighterRoundResult_Punishment>(request);
            return fighterRoundResult_Punishments; 
        }

        public void AddPunishment(Punishment punishment)
        {
            _db.Insert(punishment);
        }

        public List<Punishment> GetPunishmentsData(string request)
        {
            List<Punishment> punishments = _db.Query<Punishment>(request);
            return punishments;
        }
    }
}