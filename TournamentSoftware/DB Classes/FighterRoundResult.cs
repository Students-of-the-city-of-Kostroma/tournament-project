using SQLite;

namespace TournamentSoftware.DB_Classes
{
    [Table("FighterRoundResult")]
    public class FighterRoundResult
    {
        [Column("id"), PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Column("score")]
        public int Score { get; set; }

        [Column("judge_note_id"), Indexed]
        public int JudgeNoteId { get; set; }

        [Column("fighter_id"), Indexed]
        public int FighterId { get; set; }
    }
}
