using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentSoftware
{
    public class ApplicationState
    {
        private bool _isRegistrationComplited = false;
        private bool _isTournamentComplited = false;

        public bool isRegistrationComplited {
            get { return _isRegistrationComplited; }
            set { _isRegistrationComplited = value; }
        }

        public bool IsTournamentComplited {
            get { return _isTournamentComplited; }
            set { _isTournamentComplited = value; }
        }
    }
}
