using SQLite;

namespace TournamentSoftware.DB_Classes
{
    [Table("Subgroup")]
    public class Subgroup
    {
        [Column("id"), PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("fight_system_id")]
        public int FightSystemId { get; set; }

        [Column("group_id"), Indexed]
        public int GroupId { get; set; }
    }
}