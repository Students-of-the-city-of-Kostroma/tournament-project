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

            _db.CreateTable<Club>();
            _db.CreateTable<Participant>();
            _db.CreateTable<TournamentGrid>();
            _db.CreateTable<Nomination>();
            _db.CreateTable<Category>();
            _db.CreateTable<GroupRule>();
            _db.CreateTable<TournamentGroup>();
            _db.CreateTable<GroupRule_Group>();
            _db.CreateTable<Subgroup>();
            _db.CreateTable<Subgroup_Participant>();
            _db.CreateTable<Judge>();
            _db.CreateTable<Fighter>();
            _db.CreateTable<BattleProtocol>();
            _db.CreateTable<Round>();
            _db.CreateTable<JudgeNote>();
            _db.CreateTable<FighterRoundResult>();
            _db.CreateTable<FighterRoundResult_Punishment>();
            _db.CreateTable<Punishment>();

            AddGroupRules();
        }

        public void AddItem<T>(T item)
        {
            _db.Insert(item);
        }

        public List<T> GetData<T>(string request) where T : new ()
        {
            return _db.Query<T>(request);
        }

        private void AddGroupRules()
        {
            foreach (string rule in new List<string> { "Правило посевных бойцов", "Правило одноклубников", "Правило города" })
            {
                if (GetData<GroupRule>("SELECT * FROM GroupRule WHERE name=\"" + rule + "\";").Count == 0)
                {
                    GroupRule groupRule = new GroupRule();
                    groupRule.Name = rule;
                    AddItem(groupRule);
                }
            }
        }
    }
}