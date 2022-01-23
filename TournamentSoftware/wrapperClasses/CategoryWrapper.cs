using System;
using System.Collections.Generic;

namespace TournamentSoftware
{
    public class CategoryWrapper
    {
        private List<ParticipantWrapper> participantsBuffer = new List<ParticipantWrapper>();
        private List<SubgroupWrapper> subgroups = new List<SubgroupWrapper>();
        public bool ContainsSubgroups { get { return subgroups.Count > 0; } }
        public string Name { get; set; }
        public List<SubgroupWrapper> Subgroups { get { return subgroups; } set { subgroups = value; } }
        public CategoryWrapper() { Subgroups = new List<SubgroupWrapper>(); }
        public CategoryWrapper(ParticipantWrapper participant)
        {
            Name = participant.Category;
            participantsBuffer.Add(participant);
        }

        public CategoryWrapper(string categoryName)
        {
            Name = categoryName;
        }

        public int ParticipantsCount()
        {
            int count = 0;
            foreach (SubgroupWrapper subgroup in Subgroups)
            {
                count += subgroup.Participants.Count;
            }
            foreach (ParticipantWrapper participant in participantsBuffer)
            {
                count += 1;
            }
            return count;
        }

        public List<ParticipantWrapper> GetAllParticipants()
        {
            List<ParticipantWrapper> participants = new List<ParticipantWrapper>();
            foreach (SubgroupWrapper subgroup in Subgroups)
            {
                foreach (ParticipantWrapper participant in subgroup.Participants)
                {
                    participants.Add(participant);
                }
            }
            foreach (ParticipantWrapper participant in participantsBuffer)
            {
                participants.Add(participant);
            }
            return participants;
        }

        public void AddParticipant(ParticipantWrapper participant)
        {
            participantsBuffer.Add(participant);
        }

        public void AddParticipant(ParticipantWrapper participant, string subgroupName)
        {
            if (!IsSubgroupExists(subgroupName))
            {
                AddSubgroup(subgroupName);
            }
            GetSubgroupByName(subgroupName).AddParticipant(participant);
        }

        public void AddSubgroup(string subgroupName)
        {
            if (!IsSubgroupExists(subgroupName))
            {
                subgroups.Add(new SubgroupWrapper() { Name = subgroupName });
            }
        }

        public bool IsSubgroupExists(string subgroupName)
        {
            return subgroups.Exists(subgroup => subgroup.Name.Equals(subgroupName));
        }

        public SubgroupWrapper GetSubgroupByName(string subgroupName)
        {
            return subgroups.Find(subgroup => subgroup.Name.Equals(subgroupName));
        }

        public List<ParticipantWrapper> GetParticipantsBySubgroup(string subgroupName)
        {
            return GetSubgroupByName(subgroupName).Participants;
        }

        public void RemoveAllSubgroups()
        {
            subgroups.Clear();
        }
    }
}
