using SQLite;

namespace TournamentSoftware.DB_Classes
{
    [Table("GroupRule")]
    public class GroupRule
    {
        [Column("id"), PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }
    }
}
