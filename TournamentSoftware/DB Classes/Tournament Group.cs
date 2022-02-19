using SQLite;

namespace TournamentSoftware
{
    [Table("TournamentGroup")]
    public class TournamentGroup
    {
        [Column("id"), PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Column("tournament_grid_id"), Indexed]
        public int TournamentGridId { get; set; }

        [Column("nomination_id"), Indexed]
        public int NominationId { get; set; }

        [Column("category_id"), Indexed]
        public int CategoryId { get; set; }
    }
}