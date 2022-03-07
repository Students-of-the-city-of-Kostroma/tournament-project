using SQLite;

namespace TournamentSoftware.DB_Classes
{
    [Table("FightSystem")]
    public class FightSystem
    {
        [Column("id"), PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }
    }
}