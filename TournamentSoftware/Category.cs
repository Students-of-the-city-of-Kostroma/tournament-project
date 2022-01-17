using System;
using System.Collections.Generic;

namespace TournamentSoftware
{
    public class Category
    {
        private List<ParticipantWrapper> participantsBuffer = new List<ParticipantWrapper>();
        private List<Subgroup> subgroups = new List<Subgroup>();
        public string Name { get; set; }
        public List<Subgroup> Subgroups { get { return subgroups; } set { subgroups = value; } }
        public Category() { Subgroups = new List<Subgroup>(); }
        public Category(ParticipantWrapper participant)
        {
            Name = participant.Category;
            participantsBuffer.Add(participant);
        }

        public Category(string categoryName)
        {
            Name = categoryName;
        }

        public int ParticipantsCount()
        {
            int count = 0;
            foreach (Subgroup subgroup in Subgroups)
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
            foreach (Subgroup subgroup in Subgroups)
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
                Console.WriteLine("Создание подгруппы " + subgroupName);
                subgroups.Add(new Subgroup() { Name = subgroupName });
            }
        }

        public bool IsSubgroupExists(string subgroupName)
        {
            return subgroups.Exists(subgroup => subgroup.Name.Equals(subgroupName));
        }

        public Subgroup GetSubgroupByName(string subgroupName)
        {
            Console.WriteLine(subgroupName);
            return subgroups.Find(subgroup => subgroup.Name.Equals(subgroupName));
        }

        public List<ParticipantWrapper> GetParticipantsBySubgroup(string subgroupName)
        {
            return GetSubgroupByName(subgroupName).Participants;
        }
    }
}
