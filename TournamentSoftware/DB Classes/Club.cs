using SQLite;

namespace TournamentSoftware.DB_Classes 
{
    [Table("Club")]
    public class Club
    {
        [Column("id"), PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("city")]
        public string City { get; set; }
    }
}