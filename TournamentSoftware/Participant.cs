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

//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Runtime.CompilerServices;

//namespace TournamentSoftware
//{
//    public class Participant : INotifyPropertyChanged
//    {
//        public Participant()
//        {
//        }

//        private bool _Posevnoy;
//        private string _Name;
//        private string _Surname;
//        private string _Otchestvo;
//        private string _Psevdonim;
//        private string _Club;
//        private string _City;
//        private string _Kategory;
//        private string _Sex;
//        private int _DateOfBirth;
//        private int _Height;
//        private int _Weight;
//        private int _CommonRating;
//        private int _ClubRating;
//        private bool _IsSelected;
//        private int _Id;
//        private Dictionary<string, bool> _Nominations = new Dictionary<string, bool>();

//        /// <summary>
//        /// id участника не отображается в таблице
//        /// </summary>
//        public int Id
//        {
//            get { return _Id; }
//            set { _Id = value; }
//        }

//        public string[] availableSex = new string[2] { "М", "Ж" };

//        public event PropertyChangedEventHandler PropertyChanged;
//        public void OnPropertyChanged([CallerMemberName] string prop = "")
//        {
//            if (PropertyChanged != null)
//                PropertyChanged(this, new PropertyChangedEventArgs(prop));
//        }

//        public Dictionary<string, bool> Nominations
//        {
//            get { return _Nominations; }
//            set { _Nominations = value; OnPropertyChanged("Nominations"); }
//        }

//        public bool IsSelected
//        {
//            get { return _IsSelected; }
//            set { _IsSelected = value; OnPropertyChanged("IsSelected"); }
//        }

//        public string[] AvailableSex
//        {
//            get { return availableSex; }
//            set { availableSex = value; OnPropertyChanged("AvailableSex"); }
//        }


//        public bool Posevnoy
//        {
//            get { return _Posevnoy; }
//            set { _Posevnoy = value; OnPropertyChanged("Posevnoy"); }
//        }

//        public string Name
//        {
//            get { return _Name; }
//            set { _Name = value; OnPropertyChanged("Name"); }

//        }

//        public string Surname
//        {
//            get { return _Surname; }
//            set { _Surname = value; OnPropertyChanged("Surname"); }

//        }

//        public string Otchestvo
//        {
//            get { return _Otchestvo; }
//            set { _Otchestvo = value; OnPropertyChanged("Otchestvo"); }

//        }

//        public string Psevdonim
//        {
//            get { return _Psevdonim; }
//            set { _Psevdonim = value; OnPropertyChanged("Psevdonim"); }

//        }

//        public string Club
//        {
//            get { return _Club; }
//            set { _Club = value; OnPropertyChanged("Club"); }

//        }

//        public string City
//        {
//            get { return _City; }
//            set { _City = value; OnPropertyChanged("City"); }

//        }

//        public string Kategory
//        {
//            get { return _Kategory; }
//            set { _Kategory = value; OnPropertyChanged("Kategory"); }

//        }

//        public string Sex
//        {
//            get { return _Sex; }
//            set { _Sex = value; OnPropertyChanged("Sex"); }

//        }

//        public int DateOfBirth
//        {
//            get { return _DateOfBirth; }
//            set { _DateOfBirth = value; OnPropertyChanged("DateOfBirth"); }

//        }

//        public int Height
//        {
//            get { return _Height; }
//            set { _Height = value; OnPropertyChanged("Height"); }

//        }

//        public int Weight
//        {
//            get { return _Weight; }
//            set { _Weight = value; OnPropertyChanged("Weight"); }

//        }

//        public int CommonRating
//        {
//            get { return _CommonRating; }
//            set { _CommonRating = value; OnPropertyChanged("CommonRating"); }

//        }

//        public int ClubRating
//        {
//            get { return _ClubRating; }
//            set { _ClubRating = value; OnPropertyChanged("ClubRating"); }

//        }
//    }
//}