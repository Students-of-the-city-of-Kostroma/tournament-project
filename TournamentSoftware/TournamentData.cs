using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TournamentSoftware
{
    public class TournamentData : Window
    {
        public static ObservableCollection<ParticipantFormModel> participantsList = new ObservableCollection<ParticipantFormModel>();
        public static ObservableCollection<NominationFormModel> nominations = new ObservableCollection<NominationFormModel>();
        public static string cellsColor = "#F5F1DA";

        public static Style GetCellStyle()
        {
            Style cellStyle = new Style(typeof(DataGridCell));
            cellStyle.Setters.Add(new Setter()
            {
                Property = BackgroundProperty,
                Value = (Brush)new BrushConverter().ConvertFrom(cellsColor)
            });

            return cellStyle;
        }

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

        public static void AddNominationToParticipants(string nominationName)
        {
            foreach (ParticipantFormModel participant in participantsList)
            {
                participant.Nominations.Add(nominationName, false);
            }
        }

        public static void DeleteNominationFromPartisipants(string nominationName)
        {
            foreach (ParticipantFormModel partisipant in participantsList)
            {
                partisipant.Nominations.Remove(nominationName);
            }
        }
    }
}
