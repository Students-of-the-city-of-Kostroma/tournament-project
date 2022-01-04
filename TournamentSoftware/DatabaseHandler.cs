using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using static TournamentSoftware.ApplicationResourcesPaths;

namespace TournamentSoftware
{
    public class DataBaseHandler
    {
        private SQLiteConnection _db;

        public DataBaseHandler()
        {
            _db = new SQLiteConnection(dataBasePath);
            _db.CreateTable<Participant>();
            _db.CreateTable<Club>();
            _db.CreateTable<TournamentGrid>();
            _db.CreateTable<Nomination>();
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
                Console.WriteLine(club.Id + " " + club.Name + " " + club.City + " " + club.ContactInformation);
            }
        }

        public void AddParticipant(Participant participant)
        {
            _db.Insert(participant);
        }

        public void GetParticipants()
        {
            var participants = _db.Query<Participant>("SELECT * FROM Participant");
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
                Console.WriteLine(tournamentGrid.Id + " " + tournamentGrid.Name + " " + tournamentGrid.Date);
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
                Console.WriteLine(nomination.TournamentGridId + " " + nomination.ParticipantId + " " + nomination.Name);
            }
        }
    }
}