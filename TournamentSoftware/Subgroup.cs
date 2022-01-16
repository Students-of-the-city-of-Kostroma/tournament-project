using System.Collections.Generic;

namespace TournamentSoftware
{
    public class Subgroup
    {
        private List<ParticipantWrapper> participants = new List<ParticipantWrapper>();
        public Subgroup() { }
        public Subgroup(ParticipantWrapper participant) {
            participants = new List<ParticipantWrapper>() { participant };
        }

        public string Name { get; set; }

        public List<ParticipantWrapper> Participants
        {
            get { return participants; }
            set { participants = value; }
        }

        public void AddParticipant(ParticipantWrapper participant) {
            participants.Add(participant);
        }

        public void RemoveParticipant(ParticipantWrapper participant)
        {
            participants.Remove(participant);
        }
    }
}
