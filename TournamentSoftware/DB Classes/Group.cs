using SQLite;

namespace TournamentSoftware
{
    [Table("Group")]
    public class Group
    {
        [Column("id"), PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("category_id"), Indexed]
        public int CategoryId { get; set; }
    }
}