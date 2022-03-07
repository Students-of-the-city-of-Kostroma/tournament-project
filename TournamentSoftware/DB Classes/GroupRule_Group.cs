using SQLite;

namespace TournamentSoftware.DB_Classes
{
    [Table("GroupRule_Group")]
    public class GroupRule_Group
    {
        [Column("id"), PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Column("group_id"), Indexed]
        public int GroupId { get; set; }

        [Column("group_role_id"), Indexed]
        public int GroupRoleId { get; set; }
    }
}
