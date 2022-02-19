using SQLite;

namespace TournamentSoftware.DB_Classes
{
    [Table("Punishment")]
    public class Punishment
    {
        [Column("id"), PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Column("reason")]
        public string Reason { get; set; }

        [Column("type")]
        public string Type { get; set; }

        [Column("penalty_points")]
        public int PenaltyPointsId { get; set; }
    }
}
