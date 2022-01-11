using SQLite;

namespace TournamentSoftware
{
    [Table("Category")]
    public class Category
    {
        [Column("id"), PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }
    }
}