namespace TournamentSoftware
{
    public class Participant
    {
        public Participant()
        {
        }

        private bool _Posevnoy;
        private string _Name;
        private string _Surname;
        private string _Otchestvo;
        private string _Psevdonim;
        private string _Club;
        private string _City;
        private string _Kategory;
        private string _Sex;
        private int _DateOfBirth;
        private int _Height;
        private int _Weight;
        private int _CommonRating;
        private int _ClubRating;

        public string[] availableSex = new string[2] { "М", "Ж"};

        public string[] AvailableSex {
            get { return availableSex; }
            set { availableSex = value; }
        }


        public bool Posevnoy
        {
            get { return _Posevnoy; }
            set { _Posevnoy = value; }
        }

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }

        }

        public string Surname
        {
            get { return _Surname; }
            set { _Surname = value; }

        }

        public string Otchestvo
        {
            get { return _Otchestvo; }
            set { _Otchestvo = value; }

        }

        public string Psevdonim
        {
            get { return _Psevdonim; }
            set { _Psevdonim = value; }

        }

        public string Club
        {
            get { return _Club; }
            set { _Club = value; }

        }

        public string City
        {
            get { return _City; }
            set { _City = value; }

        }

        public string Kategory
        {
            get { return _Kategory; }
            set { _Kategory = value; }

        }

        public string Sex
        {
            get { return _Sex; }
            set { _Sex = value; }

        }

        public int DateOfBirth
        {
            get { return _DateOfBirth; }
            set { _DateOfBirth = value; }

        }

        public int Height
        {
            get { return _Height; }
            set { _Height = value; }

        }

        public int Weight
        {
            get { return _Weight; }
            set { _Weight = value; }

        }

        public int CommonRating
        {
            get { return _CommonRating; }
            set { _CommonRating = value; }

        }

        public int ClubRating
        {
            get { return _ClubRating; }
            set { _ClubRating = value; }

        }
    }
}