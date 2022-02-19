using SQLite;

namespace TournamentSoftware.DB_Classes
{
    [Table("JudgeNote")]
    public class JudgeNote
    {
        [Column("id"), PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Column("judge_id"), Indexed]
        public int JudgeId { get; set; }

        [Column("round_id"), Indexed]
        public int RoundId { get; set; }
    }
}
