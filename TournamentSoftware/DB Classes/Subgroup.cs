using SQLite;

namespace TournamentSoftware
{
    [Table("Subgroup")]
    public class Subgroup
    {
        [Column("id"), PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("group_id"), Indexed]
        public int GroupId { get; set; }
    }
}