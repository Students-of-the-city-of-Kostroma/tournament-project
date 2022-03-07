using SQLite;

namespace TournamentSoftware.DB_Classes
{
    [Table("Group")]
    public class Group
    {
        [Column("id"), PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Column("tournament_id"), Indexed]
        public int TournamentId { get; set; }

        [Column("nomination_id"), Indexed]
        public int NominationId { get; set; }

        [Column("category_id"), Indexed]
        public int CategoryId { get; set; }
    }
}