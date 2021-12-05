using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TournamentSoftware
{
    public class SubgroupsFormation
    {
        // хранит участников в категории
        private Dictionary<string, List<Participant>> kategoryGroups = new Dictionary<string, List<Participant>>();

        // получаем список катерогий
        private Dictionary<string, List<Participant>> GetKategories(ObservableCollection<ParticipantFormModel> participants)
        {
            foreach (ParticipantFormModel participant in participants)
            {
                string kategory = participant.Kategory;
                if (kategoryGroups.ContainsKey(kategory))
                {
                    kategoryGroups[kategory].Add(participant.Participant);
                }
                else
                {
                    List<Participant> list = new List<Participant>();
                    list.Add(participant.Participant);
                    kategoryGroups.Add(kategory, list);
                }
            }

            return kategoryGroups;
        }
    }
}
