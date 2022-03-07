using System.Collections.Generic;
using TournamentSoftware.DB_Classes;

namespace TournamentSoftware
{
    public class SubgroupWrapper
    {
        private Subgroup subgroup = new Subgroup();
        private List<ParticipantWrapper> participants = new List<ParticipantWrapper>();
        private List<string> errors = new List<string>();
        public SubgroupWrapper() { }
        public SubgroupWrapper(ParticipantWrapper participant) {
            participants = new List<ParticipantWrapper>() { participant };
        }

        public SubgroupWrapper(string subgroupName)
        {
            subgroup.Name = subgroupName;
        }

        public Subgroup Subgroup { get { return subgroup; } set { subgroup = value; } }

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
