using System.Collections.Generic;

namespace TournamentSoftware
{
    public class SubgroupWrapper
    {
        private string name = "";
        private List<ParticipantWrapper> participants = new List<ParticipantWrapper>();
        private List<string> errors = new List<string>();
        public SubgroupWrapper() { }
        public SubgroupWrapper(ParticipantWrapper participant) {
            participants = new List<ParticipantWrapper>() { participant };
        }

        public SubgroupWrapper(string subgroupName)
        {
            Name = subgroupName;
        }

        public string Name { get { return name; } set { name = value; } }

        public List<ParticipantWrapper> Participants
        {
            get { return participants; }
            set { participants = value; }
        }

        public List<string> Errors
        {
            get { return errors; }
        }

        public void AddParticipant(ParticipantWrapper participant) {
            participants.Add(participant);
        }

        public void RemoveParticipant(ParticipantWrapper participant)
        {
            participants.Remove(participant);
        }

        public void AddError(string errorMessage)
        {
            if (!errors.Contains(errorMessage))
            {
                errors.Add(errorMessage);
            }
        }

        public void RemoveError(string errorMessage)
        {
            if (!errors.Contains(errorMessage))
            {
                errors.Remove(errorMessage);
            }
        }

        public void ClearErrors()
        {
            errors.Clear();
        }
    }
}
