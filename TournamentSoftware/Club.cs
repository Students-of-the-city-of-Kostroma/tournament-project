using System;
using SQLite;

namespace TournamentSoftware
{
    [Table("Club")]
    public class Club
    {
        [PrimaryKey, AutoIncrement]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("city")]
        public string City { get; set; }

        [Column("contact_information")]
        public string ContactInformation { get; set; }
    }
}