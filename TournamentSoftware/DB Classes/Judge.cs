using SQLite;

namespace TournamentSoftware
{
    [Table("Judge")]
    public class Judge
    {
        [Column("id"), PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Column("surname")]
        public string Surname { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("patronymic")]
        public string Patronymic { get; set; }

        [Column("club_id"), Indexed]
        public int ClubId { get; set; }
    }
}