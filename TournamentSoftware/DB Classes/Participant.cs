using SQLite;

namespace TournamentSoftware
{
    [Table("Participant")]
    public class Participant
    {
        [Column("id"), PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Column("surname")]
        public string Surname { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("patronymic")]
        public string Patronymic { get; set; }

        [Column("pseudonym")]
        public string Pseudonym { get; set; }

        [Column("leader")]
        public bool Leader { get; set; }

        [Column("sex")]
        public string Sex { get; set; }

        [Column("date_of_birth")]
        public int DateOfBirth { get; set; }

        [Column("height")]
        public uint Height { get; set; }

        [Column("weight")]
        public uint Weight { get; set; }

        [Column("common_rating")]
        public uint CommonRating { get; set; }

        [Column("club_rating")]
        public uint ClubRating 
        { get; set; }

        [Column("club_id"), Indexed]
        public int ClubId { get; set; }
    }
}