using SQLite;

namespace TournamentSoftware
{
    [Table("FighterRoundResultPunishment")]
    public class FighterRoundResult_Punishment
    {
        [Column("fighter_round_result_id"), Indexed]
        public int FighterRoundResultId { get; set; }

        [Column("punishment_id"), Indexed]
        public int PunishmentId { get; set; }
    }
}
