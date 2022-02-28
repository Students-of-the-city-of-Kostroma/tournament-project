using SQLite;

namespace TournamentSoftware.DB_Classes
{
    [Table("GroupRule_Group")]
    public class GroupRule_Group
    {
        [Column("id"), PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Column("tournament_group_id"), Indexed]
        public int TournamentGroupId { get; set; }

        [Column("group_role_id"), Indexed]
        public int GroupRoleId { get; set; }
    }
}
