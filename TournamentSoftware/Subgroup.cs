using System.Collections.Generic;

namespace TournamentSoftware
{
    public class Subgroup
    {
        private string name = "";
        private List<ParticipantWrapper> participants = new List<ParticipantWrapper>();
        public Subgroup() { }
        public Subgroup(ParticipantWrapper participant) {
            participants = new List<ParticipantWrapper>() { participant };
        }

        public Subgroup(string subgroupName)
        {
            Name = subgroupName;
        }

        public string Name { get { return name; } set { name = value; } }

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
