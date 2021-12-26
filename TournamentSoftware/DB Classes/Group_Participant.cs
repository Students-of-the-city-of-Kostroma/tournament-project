using SQLite;

namespace TournamentSoftware.DB_Classes
{
    [Table("Group_Participant")]
    public class Group_Participant
    {
        [Column("group_id"), Indexed]
        public int GroupId { get; set; }

        [Column("participant_id"), Indexed]
        public int ParticipantId { get; set; }
    }
}
