using SQLite;

namespace TournamentSoftware
{
    [Table("Nomination")]
    public class Nomination
    {
        [Column("id"), PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("tournament_grid_id"), Indexed]
        public int TournamentGridId { get; set; }
    }
}