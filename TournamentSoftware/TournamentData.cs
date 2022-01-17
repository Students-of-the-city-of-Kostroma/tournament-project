using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TournamentSoftware
{
    public class TournamentData : Window
    {
        public static ObservableCollection<ParticipantWrapper> participants = new ObservableCollection<ParticipantWrapper>();
        public static ObservableCollection<NominationWrapper> nominations = new ObservableCollection<NominationWrapper>();
        public static List<Group> groups = new List<Group>();
        public static string cellsColor = "#F5F1DA";
        public static SolidColorBrush yellow = new SolidColorBrush(Color.FromRgb(255, 215, 0));
        public static SolidColorBrush white = new SolidColorBrush(Color.FromRgb(255, 255, 255));

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
                NominationWrapper nomination = nominations.Single(n => n.Nomination.Name.Equals(nominationName));
                nominations.Remove(nomination);
            }
            catch (Exception e)
            {
                Console.WriteLine("Номинация " + nominationName + " не найдена: " + e.Message);
            }
        }

        public static void AddNomination(string nominationName)
        {
            NominationWrapper nomination = new NominationWrapper(nominationName);
            nominations.Add(nomination);
        }

        public static bool IsNominationExists(string nominationName)
        {
            return nominations.Any(n => n.Nomination.Name.Equals(nominationName));
        }

        public static void AddNominationToParticipants(string nominationName)
        {
            foreach (ParticipantWrapper participant in participants)
            {
                participant.Nominations.Add(nominationName, false);
            }
        }

        public static void DeleteNominationFromPartisipants(string nominationName)
        {
            foreach (ParticipantWrapper partisipant in participants)
            {
                partisipant.Nominations.Remove(nominationName);
            }
        }

        public static void SetGroups()
        {
            foreach (NominationWrapper nomination in nominations)
            {
                foreach (ParticipantWrapper participant in participants)
                {
                    if (participant.RequestedToNomination(nomination.Nomination.Name))
                    {
                        if (IsGroupExists(nomination))
                        {
                            AddParticipantWithCategoryToGroup(nomination, participant);
                        }
                        else
                        {
                            AddNewGroup(nomination, participant);
                        }
                    }
                }
            }
        }

        private static void AddNewGroup(NominationWrapper nomination, ParticipantWrapper participant)
        {
            groups.Add(new Group(nomination, participant));
        }

        private static void AddParticipantWithCategoryToGroup(NominationWrapper nomination, ParticipantWrapper participant)
        {
            Group group = groups.Find(g => g.NominationWrapper.Equals(nomination));
            group.AddCategory(participant.Category);
            group.AddParticipantToCategory(participant);
        }

        private static bool IsGroupExists(NominationWrapper nomination)
        {
            return groups.Exists(group => group.NominationWrapper.Equals(nomination));
        }

        public static List<Category> GetCategoriesFromNomination(string nominationName)
        {
            return GetGroupByNomination(nominationName).Categories;
        }

        public static Category GetCategoryFromNomination(string nominationName, string categoryName)
        {
            return GetGroupByNomination(nominationName).GetCategory(categoryName);
        }

        public static Group GetGroupByNomination(string nominationName) {
            return groups.Find(group => group.NominationWrapper.Nomination.Name.Equals(nominationName));
        }

        public static List<ParticipantWrapper> GetParticipantsInCategoryAndNomination(string nominationName, string categoryName)
        {
            Category category = GetCategoryFromNomination(nominationName, categoryName);
            return category.GetAllParticipants();
        }

        public static List<string> GetCategoryNames()
        {
            List<string> categories = new List<string>();
            foreach (Group group in groups)
            {
                List<string> categoriesFromGroup = group.GetCategoryNames();
                foreach (string category in categoriesFromGroup)
                {
                    if (!categories.Contains(category))
                    {
                        categories.Add(category);
                    }
                }
            }
            return categories;
        }
    }
}
