using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace TournamentSoftware
{
    public class TournamentData
    {
        public static ObservableCollection<ParticipantFormModel> participantsList = new ObservableCollection<ParticipantFormModel>();
        public static ObservableCollection<NominationFormModel> nominations = new ObservableCollection<NominationFormModel>();
        public static void RemoveNomination(string nominationName)
        {
            try
            {
                NominationFormModel nomination = nominations.Single(n => n.Nomination.Name.Equals(nominationName));
                nominations.Remove(nomination);
            }
            catch (Exception e)
            {
                Console.WriteLine("Номинация " + nominationName + " не найдена: " + e.Message);
            }
        }

        public static void AddNomination(string nominationName)
        {
            NominationFormModel nomination = new NominationFormModel(nominationName);
            nominations.Add(nomination);
        }

        public static bool IsNominationExists(string nominationName)
        {
            return nominations.Any(n => n.Nomination.Name.Equals(nominationName));
        }
    }
}
