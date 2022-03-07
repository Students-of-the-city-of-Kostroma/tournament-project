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

        [Column("phase_id")]
        public int PhaseId { get; set; }

        [Column("red_fighter_id"), Indexed]
        public int RedFighterId { get; set; }

        [Column("blue_fighter_id"), Indexed]
        public int BlueFighterId { get; set; }
    }
}
