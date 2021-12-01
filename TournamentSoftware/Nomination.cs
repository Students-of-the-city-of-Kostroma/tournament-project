using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace TournamentSoftware
{
    [Table("Nomination")]
    public class Nomination : INotifyPropertyChanged
    {

        private int _tournamentGridId;
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        [Column("tournament_grid_id")]
        public int TournamentGridId
        {
            get
            { return _tournamentGridId; }
            set { _tournamentGridId = value; OnPropertyChanged("TournamentGridId"); }
        }

        private int _participantId;

        [Indexed]
        [Column("participant_id")]
        public int ParticipantId
        {
            get { return _participantId; }
            set { _participantId = value; OnPropertyChanged("ParticipantId"); }
        }

        private string _name;

        [Column("name")]
        public string Name { get { return _name; } set { _name = value; OnPropertyChanged("Name"); } }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}