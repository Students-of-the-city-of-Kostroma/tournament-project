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
    }
}