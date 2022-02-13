﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static TournamentSoftware.ApplicationResourcesPaths;

namespace TournamentSoftware
{
    public class TournamentData : Window
    {
        public static ObservableCollection<ParticipantWrapper> participants = new ObservableCollection<ParticipantWrapper>();
        public static ObservableCollection<NominationWrapper> nominations = new ObservableCollection<NominationWrapper>();
        public static List<GroupWrapper> groups = new List<GroupWrapper>();
        public static List<string> fightingSystems = new List<string>() { "Круговая", "На вылет", "Смешанная" };
        public static string cellsColor = "#F5F1DA";
        public static SolidColorBrush white = new SolidColorBrush(Color.FromRgb(255, 255, 255));
        
        public static DataBaseHandler dataBaseHandler = new DataBaseHandler(dataBasePath);
        
        public static SolidColorBrush orange = new SolidColorBrush(Color.FromRgb(253, 172, 97));
        public static SolidColorBrush darkGreen = new SolidColorBrush(Color.FromRgb(128, 140, 108));
        public static SolidColorBrush beige = new SolidColorBrush(Color.FromRgb(227, 223, 200));
        public static SolidColorBrush red = new SolidColorBrush(Color.FromRgb(255, 105, 97));
        public static SolidColorBrush blue = new SolidColorBrush(Color.FromRgb(174, 198, 207));

        public static DataBaseHandler dataBaseHandler = new DataBaseHandler(dataBasePath);

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
        private static void ClearGroups()
        {
            groups.Clear();
        }

        public static void SetGroups()
        {
            ClearGroups();
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
            groups.Add(new GroupWrapper(nomination, participant));
        }

        private static void AddParticipantWithCategoryToGroup(NominationWrapper nomination, ParticipantWrapper participant)
        {
            GroupWrapper group = groups.Find(g => g.NominationWrapper.Equals(nomination));
            group.AddCategory(participant.Category);
            group.AddParticipantToCategory(participant);
        }

        private static bool IsGroupExists(NominationWrapper nomination)
        {
            return groups.Exists(group => group.NominationWrapper.Equals(nomination));
        }

        public static List<CategoryWrapper> GetCategoriesFromNomination(string nominationName)
        {
            return GetGroupByNomination(nominationName).Categories;
        }

        public static CategoryWrapper GetCategoryFromNomination(string nominationName, string categoryName)
        {
            return GetGroupByNomination(nominationName).GetCategory(categoryName);
        }

        public static GroupWrapper GetGroupByNomination(string nominationName) {
            return groups.Find(group => group.NominationWrapper.Nomination.Name.Equals(nominationName));
        }

        public static List<ParticipantWrapper> GetParticipantsInCategoryAndNomination(string nominationName, string categoryName)
        {
            CategoryWrapper category = GetCategoryFromNomination(nominationName, categoryName);
            return category.GetAllParticipants();
        }

        public static List<string> GetCategoryNames()
        {
            List<string> categories = new List<string>();
            foreach (GroupWrapper group in groups)
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
