using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TournamentSoftware.DB_Classes;

namespace TournamentSoftware.wrapperClasses
{
    public class BattleWrapper
    {
        private ParticipantWrapper redParticipant;
        private ParticipantWrapper blueParticipant;

        private bool winner;
        private BattleProtocol battleProtocol = new BattleProtocol();
        public BattleProtocol BattleProtocol
        {
            get { return battleProtocol; }
            set { battleProtocol = value; }
        }

        public ParticipantWrapper RedParticipant { get { return redParticipant; } set { redParticipant = value; } }
        public ParticipantWrapper BlueParticipant { get { return blueParticipant; } set { blueParticipant = value; } }
        
        /// <summary>
        /// Тестовое, true - red, false - blue
        /// </summary>
        public bool Winner { get { return winner; } set { winner = value; } }

        public BattleWrapper(ParticipantWrapper redParticipant, ParticipantWrapper blueParticipant)
        {
            this.redParticipant = redParticipant;
            this.blueParticipant = blueParticipant;
            this.winner = true;
        }
    }
}
