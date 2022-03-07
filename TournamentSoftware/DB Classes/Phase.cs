using SQLite;

namespace TournamentSoftware.DB_Classes
{
    [Table("Phase")]
    public class Phase
    {
        [Column("id"), PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Column("number")]
        public int Number { get; set; }

        [Column("subgroup_id")]
        public int SubgroupId { get; set; }
    }
}
