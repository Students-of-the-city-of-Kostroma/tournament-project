using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentSoftware.wrapperClasses
{
    public class BattleWrapper
    {
        private ParticipantWrapper redParticipant;
        private ParticipantWrapper blueParticipant;

        public ParticipantWrapper RedParticipant { get { return redParticipant; } set { redParticipant = value; } }
        public ParticipantWrapper BlueParticipant { get { return blueParticipant; } set { blueParticipant = value; } }

        public BattleWrapper(ParticipantWrapper redParticipant, ParticipantWrapper blueParticipant)
        {
            this.redParticipant = redParticipant;
            this.blueParticipant = blueParticipant;
        }
    }
}
