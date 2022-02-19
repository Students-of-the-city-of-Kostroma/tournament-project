namespace TournamentSoftware
{
    public class ApplicationState
    {
        private bool _isRegistrationComplited = false;
        private bool _isTournamentComplited = false;
        private string _tournamentName = "";
        private int _windowHeight = -1;
        private int _windowWidth = -1;

        public bool isRegistrationComplited {
            get { return _isRegistrationComplited; }
            set { _isRegistrationComplited = value; }
        }

        public bool IsTournamentComplited {
            get { return _isTournamentComplited; }
            set { _isTournamentComplited = value; }
        }

        public int WindowHeight
        {
            get { return _windowHeight; }
            set { _windowHeight = value; }
        }

        public int WindowWidth
        {
            get { return _windowWidth; }
            set { _windowWidth = value; }
        }

        public string TournamentName { 
        get { return _tournamentName; }
            set { _tournamentName = value; }
        }
    }
}
