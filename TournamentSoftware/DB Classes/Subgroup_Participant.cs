using SQLite;

namespace TournamentSoftware.DB_Classes
{
    [Table("Subgroup_Participant")]
    public class Subgroup_Participant
    {
        [Column("id"), PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Column("subgroup_id"), Indexed]
        public int SubgroupId { get; set; }

        [Column("participant_id"), Indexed]
        public int ParticipantId { get; set; }
    }
}