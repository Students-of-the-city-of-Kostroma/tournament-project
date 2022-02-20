using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using TournamentSoftware.wrapperClasses;
using static TournamentSoftware.TournamentData;

namespace TournamentSoftware
{
    public partial class TournamentGridWindow : Window
    {
        private bool isPanelOpen = true;
        private string selectedNomination = "";
        private string selectedCategory = "";
        private string selectedSubgroup = "";
        public static int roundsCount = 0;
        public static string fightingSystem = "";
        private int numberOfNextAddedPair = 0;
        private List<Button> categoryButtons = new List<Button>();
        private List<string> rools = new List<string>();
        private Button addStageButton = new Button
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Padding = new Thickness(10),
            BorderThickness = new Thickness(0),
            Background = white,
            FontSize = 24,
            Content = "+ Добавить этап",
        };
        private Dictionary<int, List<BattleWrapper>> battles = new Dictionary<int, List<BattleWrapper>>();
        public TournamentGridWindow()
        {
            InitializeComponent();
            WindowState = WindowState.Maximized;
            addStageButton.Click += AddStage;
        }

        private void AddStage(object sender, RoutedEventArgs e)
        {
            AddStageWindow addStageWindow = new AddStageWindow();
            addStageWindow.Closed += AddStageWindow_Closed;
            addStageWindow.Show();
            addStageButton.IsEnabled = false;
        }

        private void AddStageWindow_Closed(object sender, System.EventArgs e)
        {
            addStageButton.IsEnabled = true;
            if (fightingSystem.Equals("Круговая"))
            {
                if (roundsCount > 0)
                {
                    StagesFormation();
                }
                roundsCount = 0;
            }
            else if (fightingSystem.Equals("На вылет"))
            {
                roundsCount = 1;
                numberOfNextAddedPair = 0;
                battles.Clear();
                rools = GetCategoryFromNomination(selectedNomination, selectedCategory).Rools;
                StagesFormation();
            }
        }

        private int CountPairs(List<ParticipantWrapper> participants)
        {
            return (participants.Count % 2) + (participants.Count / 2);
        }

        private List<BattleWrapper> SortByCityAndClub(List<BattleWrapper> pairs, List<ParticipantWrapper> participants, int pairsCount)
        {
            Dictionary<string, List<ParticipantWrapper>> participantsByCity = Subgrouping.FilterParticipantsForCities(participants);
            foreach (KeyValuePair<string, List<ParticipantWrapper>> keyValue in participantsByCity)
            {
                List<ParticipantWrapper> participantsList = keyValue.Value;
                Dictionary<string, List<ParticipantWrapper>> participantsByClub = Subgrouping.FilterParticipantsForClubs(participantsList);
                foreach (KeyValuePair<string, List<ParticipantWrapper>> clubKeyValue in participantsByClub)
                {
                    List<ParticipantWrapper> participantsByClubList = clubKeyValue.Value;
                    pairs = SortToPairs(pairs, participantsByClubList, pairsCount);
                }
            }
            return pairs;
        }

        private void AddRedParticipantToPair(ref List<BattleWrapper> pairs, ref List<ParticipantWrapper> participants, int nextPairNumber, int pairsCount)
        {
            ParticipantWrapper participant = participants[participants.Count - 1];
            participants.RemoveAt(participants.Count - 1);
            BattleWrapper pair = new BattleWrapper(participant, null);
            pairs.Add(pair);
            Console.WriteLine("add red " + pairs.Count);
            numberOfNextAddedPair = nextPairNumber + 1;
            if (numberOfNextAddedPair >= pairsCount)
            {
                numberOfNextAddedPair = 0;
            }
        }
        private void AddBlueParticipantToPair(ref List<BattleWrapper> pairs, ref List<ParticipantWrapper> participants, int nextPairNumber, int pairsCount)
        {
            ParticipantWrapper participant = participants[participants.Count - 1];
            participants.RemoveAt(participants.Count - 1);
            Console.WriteLine(nextPairNumber + " " + pairs.Count);
            pairs[nextPairNumber].BlueParticipant = participant;
            numberOfNextAddedPair = nextPairNumber + 1;
            if (numberOfNextAddedPair >= pairsCount)
            {
                numberOfNextAddedPair = 0;
            }
        }

        private List<BattleWrapper> SortToPairs(List<BattleWrapper> pairs, List<ParticipantWrapper> participants, int pairsCount)
        {
            if (participants.Count > pairsCount)
            {
                Console.WriteLine(numberOfNextAddedPair + " " + pairsCount);
                for (int i = numberOfNextAddedPair; i < pairsCount && participants.Count > 0; i++)
                {

                    AddRedParticipantToPair(ref pairs, ref participants, i, pairsCount);
                }
                for (int i = 0; i < pairsCount && participants.Count > 0; i++)
                {
                    AddBlueParticipantToPair(ref pairs, ref participants, i, pairsCount);
                }
            }
            else
            {
                for (int i = numberOfNextAddedPair; i < pairsCount && participants.Count > 0; i++)
                {
                    if (pairs.Count == pairsCount)
                    {
                        AddBlueParticipantToPair(ref pairs, ref participants, i, pairsCount);
                    }
                    else
                    {
                        AddRedParticipantToPair(ref pairs, ref participants, i, pairsCount);
                    }
                }
            }
            return pairs;
        }

        private List<BattleWrapper> SortByCity(List<BattleWrapper> pairs, List<ParticipantWrapper> participants, int pairsCount)
        {
            Dictionary<string, List<ParticipantWrapper>> participantsByCity = Subgrouping.FilterParticipantsForCities(participants);
            foreach (KeyValuePair<string, List<ParticipantWrapper>> keyValue in participantsByCity)
            {
                List<ParticipantWrapper> participantsList = keyValue.Value;
                pairs = SortToPairs(pairs, participantsList, pairsCount);
            }
            return pairs;
        }

        private List<BattleWrapper> SortByClub(List<BattleWrapper> pairs, List<ParticipantWrapper> participants, int pairsCount)
        {
            Dictionary<string, List<ParticipantWrapper>> participantsByClub = Subgrouping.FilterParticipantsForClubs(participants);
            foreach (KeyValuePair<string, List<ParticipantWrapper>> keyValue in participantsByClub)
            {
                List<ParticipantWrapper> participantsList = keyValue.Value;
                pairs = SortToPairs(pairs, participantsList, pairsCount);
            }
            return pairs;
        }

        private List<BattleWrapper> PairsFromation()
        {
            List<ParticipantWrapper> participants = GetCategoryFromNomination(selectedNomination, selectedCategory)
                .GetParticipantsBySubgroup(selectedSubgroup);
            int pairsCount = CountPairs(participants);
            List<BattleWrapper> pairs = new List<BattleWrapper>();

            if (rools.Contains("Правило посевных бойцов"))
            {
                List<ParticipantWrapper> posevParticipants = participants.FindAll(participant => participant.Participant.Leader == true);
                List<ParticipantWrapper> notPosevParticipants = participants.FindAll(participant => participant.Participant.Leader == false);

                if (rools.Contains("Правило города"))
                {
                    pairs = rools.Contains("Правило одноклубников") ?
                        SortByCityAndClub(pairs, posevParticipants, pairsCount) : SortByCity(pairs, posevParticipants, pairsCount);
                    pairs = rools.Contains("Правило одноклубников") ?
                        SortByCityAndClub(pairs, notPosevParticipants, pairsCount) : SortByCity(pairs, notPosevParticipants, pairsCount);
                }
                else
                {
                    pairs = rools.Contains("Правило одноклубников") ?
                        SortByClub(pairs, posevParticipants, pairsCount) : SortToPairs(pairs, posevParticipants, pairsCount);
                    pairs = rools.Contains("Правило одноклубников") ?
                        SortByClub(pairs, notPosevParticipants, pairsCount) : SortToPairs(pairs, notPosevParticipants, pairsCount);
                }
            }
            else if (rools.Contains("Правило города"))
            {
                if (rools.Contains("Правило одноклубников"))
                {
                    pairs = SortByCityAndClub(pairs, participants, pairsCount);
                }
                else
                {
                    pairs = SortByCity(pairs, participants, pairsCount);
                }
            }
            else if (rools.Contains("Правило одноклубников"))
            {
                pairs = SortByClub(pairs, participants, pairsCount);
            }
            else
            {
                pairs = SortToPairs(pairs, participants, pairsCount);
            }
            return pairs;
        }

        private Grid ParticipantsInSubgroup()
        {
            Grid participantsGrid = new Grid();
            List<ParticipantWrapper> participants = GetCategoryFromNomination(selectedNomination, selectedCategory)
                .GetParticipantsBySubgroup(selectedSubgroup);
            RowDefinition headerRow = new RowDefinition
            {
                Height = new GridLength(70, GridUnitType.Pixel)
            };
            Label header = new Label
            {
                Content = "Список участников",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                FontSize = 20
            };
            participantsGrid.RowDefinitions.Add(headerRow);
            participantsGrid.Children.Add(header);
            Grid.SetRow(header, 0);
            foreach (ParticipantWrapper participant in participants)
            {
                RowDefinition participantRow = new RowDefinition
                {
                    Height = new GridLength(35, GridUnitType.Pixel)
                };
                Label participantLabel = new Label
                {
                    Content = participant.Participant.Name + " " + participant.Participant.Surname,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(5),
                    Background = beige
                };
                participantsGrid.RowDefinitions.Add(participantRow);
                participantsGrid.Children.Add(participantLabel);
                Grid.SetRow(participantLabel, participantsGrid.RowDefinitions.Count - 1);
            }
            return participantsGrid;
        }

        private void SetParticipantsList()
        {
            ColumnDefinition participantsListColumn = new ColumnDefinition();
            participantsListColumn.Width = new GridLength(350, GridUnitType.Pixel);
            Grid participantsList = ParticipantsInSubgroup();
            tournamentGrid.ColumnDefinitions.Add(participantsListColumn);
            tournamentGrid.Children.Add(participantsList);
            Grid.SetColumn(participantsList, 0);
        }

        private Grid RoundHeader(int roundNumber)
        {
            Grid roundHeader = new Grid();
            RowDefinition roundNumberRow = new RowDefinition
            {
                Height = new GridLength(1, GridUnitType.Star),
            };
            RowDefinition fightingSystemRow = new RowDefinition
            {
                Height = new GridLength(1, GridUnitType.Star)
            };
            Label roundNumberLabel = new Label
            {
                Content = "Круг " + roundNumber,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 18,
            };
            Label fightingSystemLabel = new Label
            {
                Content = "Система: " + fightingSystem,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 14,
            };
            roundHeader.RowDefinitions.Add(roundNumberRow);
            roundHeader.RowDefinitions.Add(fightingSystemRow);
            roundHeader.Children.Add(roundNumberLabel);
            Grid.SetRow(roundNumberLabel, 0);
            roundHeader.Children.Add(fightingSystemLabel);
            Grid.SetRow(fightingSystemLabel, 1);
            return roundHeader;
        }

        private Grid BattleGrid(BattleWrapper battle)
        {
            Grid battleGrid = new Grid
            {
                Margin = new Thickness(0, 0, 0, 10)
            };
            Button battleProtocolButton = new Button
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Content = "+"
            };
            ColumnDefinition protocolColumn = new ColumnDefinition
            {
                Width = new GridLength(30, GridUnitType.Pixel)
            };
            ColumnDefinition participantsColumn = new ColumnDefinition();

            Grid participantsGrid = new Grid();
            RowDefinition redParticipantRow = new RowDefinition();
            RowDefinition blueParticipantRow = new RowDefinition();
            Label redParticipant = new Label
            {
                Background = red,
                Margin = new Thickness(5),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                Content = battle.RedParticipant.Participant.Name + " " + battle.RedParticipant.Participant.Surname,
            };

            Label blueParticipant = new Label
            {
                Background = blue,
                Margin = new Thickness(5),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                Content = battle.BlueParticipant != null ?
                battle.BlueParticipant.Participant.Name + " " + battle.BlueParticipant.Participant.Surname : "нет пары",
            };

            if (battle.RedParticipant != null)
            {
                redParticipant.ToolTip = "Посев: " + battle.RedParticipant.Participant.Leader + "\nКлуб: "
                    + battle.RedParticipant.Club + "\nГород: " + battle.RedParticipant.City;
            }

            if (battle.BlueParticipant != null)
            {
                blueParticipant.ToolTip = "Посев: " + battle.BlueParticipant.Participant.Leader + "\nКлуб: "
                    + battle.BlueParticipant.Club + "\nГород: " + battle.BlueParticipant.City;
            }

            participantsGrid.RowDefinitions.Add(redParticipantRow);
            participantsGrid.RowDefinitions.Add(blueParticipantRow);
            participantsGrid.Children.Add(redParticipant);
            Grid.SetRow(redParticipant, 0);
            participantsGrid.Children.Add(blueParticipant);
            Grid.SetRow(blueParticipant, 1);

            battleGrid.ColumnDefinitions.Add(protocolColumn);
            battleGrid.Children.Add(battleProtocolButton);
            Grid.SetColumn(battleProtocolButton, 0);
            battleGrid.ColumnDefinitions.Add(participantsColumn);
            battleGrid.Children.Add(participantsGrid);
            Grid.SetColumn(participantsGrid, 2);
            return battleGrid;
        }

        private Grid RoundGrid(int roundNumber)
        {
            Grid roundGrid = new Grid();
            RowDefinition headerRow = new RowDefinition
            {
                Height = new GridLength(70, GridUnitType.Pixel)
            };
            Grid header = RoundHeader(roundNumber);
            roundGrid.RowDefinitions.Add(headerRow);
            roundGrid.Children.Add(header);
            Grid.SetRow(header, 0);

            BattlesFormation(roundNumber); // распределение участников по парам
            List<BattleWrapper> battleWrappers = battles[roundNumber];
            foreach (BattleWrapper battle in battleWrappers)
            {
                RowDefinition ballteRow = new RowDefinition
                {
                    Height = new GridLength(80, GridUnitType.Pixel)
                };
                Grid battleGrid = BattleGrid(battle);
                roundGrid.RowDefinitions.Add(ballteRow);
                roundGrid.Children.Add(battleGrid);
                Grid.SetRow(battleGrid, roundGrid.RowDefinitions.Count - 1);
            }
            return roundGrid;
        }

        private void StagesFormation()
        {
            tournamentGrid.Children.Remove(addStageButton);
            tournamentGrid.ColumnDefinitions.RemoveAt(tournamentGrid.ColumnDefinitions.Count - 1);
            int lastRoundNumber = battles.Count;
            for (int i = 1; i <= roundsCount; i++)
            {
                ColumnDefinition column = new ColumnDefinition
                {
                    Width = new GridLength(200, GridUnitType.Pixel)
                };
                Grid stage = RoundGrid(i + lastRoundNumber);
                tournamentGrid.ColumnDefinitions.Add(column);
                tournamentGrid.Children.Add(stage);
                Grid.SetColumn(stage, tournamentGrid.ColumnDefinitions.Count - 1);
            }
            SetAddButtonToLastColumn();
        }

        private void SetAddButtonToLastColumn()
        {
            ColumnDefinition columnWithAddButton = new ColumnDefinition
            {
                Width = tournamentGrid.ColumnDefinitions.Count > 0 ?
                new GridLength(200, GridUnitType.Pixel) : new GridLength(1, GridUnitType.Star)
            };
            tournamentGrid.ColumnDefinitions.Add(columnWithAddButton);
            tournamentGrid.Children.Add(addStageButton);
            Grid.SetColumn(addStageButton, tournamentGrid.ColumnDefinitions.Count - 1);
        }

        private void BattlesFormation(int roundNumber)
        {
            if (fightingSystem.Equals("Круговая"))
            {
                List<ParticipantWrapper> participants = GetCategoryFromNomination(selectedNomination, selectedCategory)
                .GetParticipantsBySubgroup(selectedSubgroup);
                List<BattleWrapper> battleWrappers = new List<BattleWrapper>();
                for (int i = 0; i < participants.Count - 1; i++)
                {
                    ParticipantWrapper redParticipant = participants[i];
                    for (int j = i + 1; j < participants.Count; j++)
                    {
                        ParticipantWrapper blueParticipant = participants[j];
                        BattleWrapper battle = new BattleWrapper(redParticipant, blueParticipant);
                        battleWrappers.Add(battle);
                    }
                }
                battles.Add(roundNumber, battleWrappers);
            }
            else if (fightingSystem.Equals("На вылет"))
            {
                List<BattleWrapper> battleWrappers = PairsFromation();
                battles.Add(1, battleWrappers);
            }
        }

        private void HideInstrumentsPanel(object sender, RoutedEventArgs e)
        {
            mainGrid.ColumnDefinitions[1].Width = isPanelOpen ? new GridLength(60) : new GridLength(160);
            controlsGrid.Visibility = isPanelOpen ? Visibility.Hidden : Visibility.Visible;
            isPanelOpen = !isPanelOpen;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ShowNominationsList();
        }

        private void ShowNominationsList()
        {
            ParticipantsReagistrator participantsReagistrator = MainWindow.GetReagistrator;
            foreach (NominationWrapper nomination in nominations)
            {
                string nominationName = nomination.Nomination.Name;
                Button niminationButton = new Button
                {
                    Content = nominationName,
                    Margin = new Thickness(5),
                    Tag = nominationName
                };
                niminationButton.Click += SelectNomination;
                nominationsStackPanel.Children.Add(niminationButton);
            }
        }

        private void CleanTournamentGrid()
        {
            tournamentGrid.ColumnDefinitions.Clear();
            tournamentGrid.Children.Clear();
            battles.Clear();
        }

        private void SelectNomination(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            selectedNomination = button.Tag.ToString();
            selectedCategory = "";
            selectedSubgroup = "";
            CleanTournamentGrid();
            CreateCategoriesTabs();
        }

        private void JudgesRegistrationButton_Click(object sender, RoutedEventArgs e)
        {
            JudgesRegistrationWindow judgesRegistrationWindow = new JudgesRegistrationWindow();
            judgesRegistrationWindow.Show();
        }

        private UIElement SubgroupTabs(CategoryWrapper category)
        {
            Grid subgroupsGrid = new Grid();
            foreach (SubgroupWrapper subgroup in category.Subgroups)
            {
                ColumnDefinition column = new ColumnDefinition();
                Button subgroupButton = new Button
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Content = subgroup.Name,
                    Tag = subgroup,
                };
                subgroupButton.Click += SelectSubgroup;
                subgroupsGrid.ColumnDefinitions.Add(column);
                subgroupsGrid.Children.Add(subgroupButton);
                Grid.SetColumn(subgroupButton, subgroupsGrid.ColumnDefinitions.Count - 1);
            }
            return subgroupsGrid;
        }

        private void ColorSubgroupButtons(Button selectedSubgroup)
        {
            Grid subgroupTabs = (Grid)selectedSubgroup.Parent;
            foreach (UIElement subgroupButton in subgroupTabs.Children)
            {
                (subgroupButton as Button).Background = subgroupButton.Equals(selectedSubgroup) ? darkGreen : white;
            }
        }

        private void SelectSubgroup(object sender, RoutedEventArgs e)
        {
            Button subgroupButton = sender as Button;
            selectedSubgroup = subgroupButton.Content.ToString();
            ColorSubgroupButtons(subgroupButton);
            ShowTournamentGrid();
        }

        private UIElement CategoryTab(CategoryWrapper category)
        {
            Grid categoryTab = new Grid();

            RowDefinition categoryButtonRow = new RowDefinition
            {
                Height = new GridLength(2, GridUnitType.Star)
            };
            RowDefinition subgroupsRow = new RowDefinition
            {
                Height = new GridLength(0, GridUnitType.Star)
            };

            Button categoryButton = new Button
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Content = category.Name,
                Tag = categoryTab
            };
            categoryButton.Click += SelectCategory;
            categoryButtons.Add(categoryButton);

            Grid subgroupsGrid = (Grid)SubgroupTabs(category);

            categoryTab.RowDefinitions.Add(categoryButtonRow);
            categoryTab.RowDefinitions.Add(subgroupsRow);
            categoryTab.Children.Add(categoryButton);
            Grid.SetRow(categoryButton, 0);
            categoryTab.Children.Add(subgroupsGrid);
            Grid.SetRow(subgroupsGrid, 1);

            return categoryTab;
        }

        private void CreateCategoriesTabs()
        {
            selectedCategory = "";
            categoryButtons.Clear();
            categoryTabsGrid.Children.Clear();
            categoryTabsGrid.ColumnDefinitions.Clear();
            List<CategoryWrapper> categoryNames = GetCategoriesFromNomination(selectedNomination);
            foreach (CategoryWrapper category in categoryNames)
            {
                Grid categoryGrid = (Grid)CategoryTab(category);
                ColumnDefinition columnDefinition = new ColumnDefinition();
                categoryTabsGrid.ColumnDefinitions.Add(columnDefinition);
                categoryTabsGrid.Children.Add(categoryGrid);
                Grid.SetColumn(categoryGrid, categoryTabsGrid.ColumnDefinitions.Count - 1);
            }
        }

        private void HideSubgroups(Button categoryButton)
        {
            Grid categoryTab = (Grid)categoryButton.Tag;
            categoryTab.RowDefinitions[1].Height = new GridLength(0, GridUnitType.Star);
        }

        private void ShowTournamentGrid()
        {
            CleanTournamentGrid();
            SetParticipantsList();
            SetAddButtonToLastColumn();
        }

        private void SelectCategory(object sender, RoutedEventArgs e)
        {
            CleanTournamentGrid();
            selectedSubgroup = "";
            Button categoryButton = sender as Button;
            Grid categoryTab = (Grid)categoryButton.Tag;
            categoryTab.RowDefinitions[1].Height = new GridLength(1, GridUnitType.Star);
            selectedCategory = categoryButton.Content.ToString();

            foreach (Button button in categoryButtons)
            {
                if (!button.Equals(categoryButton))
                {
                    HideSubgroups(button);
                }
            }
        }
    }
}