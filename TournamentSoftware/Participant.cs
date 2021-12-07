using System.ComponentModel;
using System.Runtime.CompilerServices;
using SQLite;

namespace TournamentSoftware
{
    [Table("Participant")]
    public class Participant : INotifyPropertyChanged
    {
        private int id;

        [PrimaryKey, AutoIncrement]
        [Column("id")]
        public int Id { get { return id; } set { id = value; OnPropertyChanged("Id"); } }

        private string surname;

        [Column("surname")]
        public string Surname { get { return surname; } set { surname = value; OnPropertyChanged("Surname"); } }

        private string name;

        [Column("name")]
        public string Name { get { return name; } set { name = value; OnPropertyChanged("Name"); } }

        private string patronymic;

        [Column("patronymic")]
        public string Patronymic { get { return patronymic; } set { patronymic = value;  OnPropertyChanged("Patronymic"); } }

        private string pseudonym;

        [Column("pseudonym")]
        public string Pseudonym { get { return pseudonym; } set { pseudonym = value; OnPropertyChanged("Pseudonym"); } }

        private bool leader;

        [Column("leader")]
        public bool Leader { get { return leader; } set { leader = value; OnPropertyChanged("Leader"); } }

        private string sex;

        [Column("sex")]
        public string Sex { get { return sex; } set { sex = value; OnPropertyChanged("Sex"); } }

        private int dateOfBirth;

        [Column("date_of_birth")]
        public int DateOfBirth { get { return dateOfBirth; } set { dateOfBirth = value; OnPropertyChanged("DateOfBirth"); } }

        private int height;

        [Column("height")]
        public int Height { get { return height; } set { height = value; OnPropertyChanged("Height"); } }

        private int weight;

        [Column("weight")]
        public int Weight { get { return weight; } set { weight = value; OnPropertyChanged("Weight"); } }

        private int commonRating;

        [Column("common_rating")]
        public int CommonRating { 
            get { return commonRating; } 
            set { commonRating = value; OnPropertyChanged("CommonRating"); } }

        private int clubRating;

        [Column("club_rating")]
        public int ClubRating 
        { get { return clubRating; } set 
            { clubRating = value; OnPropertyChanged("ClubRating"); } }

        private int clubId;

        [Indexed]
        [Column("club_id")]
        public int ClubId { get { return clubId; } set { clubId = value; OnPropertyChanged("ClubId"); } }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

    }
}