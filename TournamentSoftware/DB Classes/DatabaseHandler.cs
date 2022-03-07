using System;
using System.Collections.Generic;
using SQLite;
using TournamentSoftware.DB_Classes;

namespace TournamentSoftware.DB_Classes
{
    public class DataBaseHandler
    {
        private SQLiteConnection _db;

        public DataBaseHandler(string dataBasePath)
        {
            _db = new SQLiteConnection(dataBasePath);

            _db.CreateTable<Club>();
            _db.CreateTable<Participant>();
            _db.CreateTable<Tournament>();
            _db.CreateTable<Nomination>();
            _db.CreateTable<Category>();
            _db.CreateTable<GroupRule>();
            _db.CreateTable<Group>();
            _db.CreateTable<GroupRule_Group>();
            _db.CreateTable<Subgroup>();
            _db.CreateTable<Subgroup_Participant>();
            _db.CreateTable<Judge>();
            _db.CreateTable<FightSystem>();
            _db.CreateTable<Fighter>();
            _db.CreateTable<Phase>();
            _db.CreateTable<BattleProtocol>();
            _db.CreateTable<Round>();
            _db.CreateTable<JudgeNote>();
            _db.CreateTable<FighterRoundResult>();
            _db.CreateTable<FighterRoundResult_Punishment>();
            _db.CreateTable<Punishment>();

            AddGroupRules();
            AddFightSystem();
        }

        public void Insert<T>(T item)
        {
            _db.Insert(item);
        }

        public List<T> Query<T>(string request) where T : new ()
        {
            return _db.Query<T>(request);
        }

        private void AddGroupRules()
        {
            foreach (string rule in new List<string> { "Правило посевных бойцов", "Правило одноклубников", "Правило города" })
            {
                if (Query<GroupRule>("SELECT * FROM GroupRule WHERE name=\"" + rule + "\";").Count == 0)
                {
                    GroupRule groupRule = new GroupRule();
                    groupRule.Name = rule;
                    Insert(groupRule);
                }
            }
        }

        private void AddFightSystem()
        {
            foreach (string system in new List<string> { "Круговая", "На вылет", "Смешанная" })
            {
                if (Query<FightSystem>("SELECT * FROM FightSystem WHERE name=\"" + system + "\";").Count == 0)
                {
                    FightSystem fightSystem = new FightSystem();
                    fightSystem.Name = system;
                    Insert(fightSystem);
                }
            }
        }
    }
}