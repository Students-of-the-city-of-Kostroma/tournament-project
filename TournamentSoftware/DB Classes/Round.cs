using SQLite;

namespace TournamentSoftware.DB_Classes
{
    [Table("Round")]
    public class Round
    {
        [Column("id"), PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Column("number")]
        public int Number { get; set; }

        [Column("battle_protocol_id"), Indexed]
        public int BattleProtocolId { get; set; }
    }
}
