namespace TournamentSoftware
{
    public class ApplicationState
    {
        private bool _isRegistrationComplited = false;
        private bool _isTournamentComplited = false;
        private string _tournamentName = "";

        public bool isRegistrationComplited {
            get { return _isRegistrationComplited; }
            set { _isRegistrationComplited = value; }
        }

        public bool IsTournamentComplited {
            get { return _isTournamentComplited; }
            set { _isTournamentComplited = value; }
        }

        public string TournamentName { 
        get { return _tournamentName; }
            set { _tournamentName = value; }
        }
    }
}
