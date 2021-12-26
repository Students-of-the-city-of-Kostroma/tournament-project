using SQLite;

namespace TournamentSoftware.DB_Classes
{
    [Table("BattleProtocol")]
    public class BattleProtocol
    {
        [Column("id"), PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Column("number")]
        public int Number { get; set; }

        [Column("system")]
        public string System { get; set; }

        [Column("group_id"), Indexed]
        public int GroupId { get; set; }

        [Column("fighter_id"), Indexed]
        public int FighterId { get; set; }
    }
}
