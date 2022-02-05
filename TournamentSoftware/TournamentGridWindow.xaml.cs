﻿using System.Collections.Generic;
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
        private List<Button> categoryButtons = new List<Button>();
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
            if (roundsCount > 0)
            {
                StagesFormation();
            }
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
            Grid battleGrid = new Grid();
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
                Content = battle.RedParticipant.Participant.Name + " " + battle.RedParticipant.Participant.Surname
            };

            Label blueParticipant = new Label
            {
                Background = blue,
                Margin = new Thickness(5),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                Content = battle.BlueParticipant.Participant.Name + " " + battle.BlueParticipant.Participant.Surname,
            };

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

            BattlesFormation(roundNumber);
            List<BattleWrapper> battleWrappers = battles[roundNumber];
            foreach (BattleWrapper battle in battleWrappers)
            {
                RowDefinition ballteRow = new RowDefinition();
                ballteRow.Height = new GridLength(70, GridUnitType.Pixel);
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
            for (int i = 1; i <= roundsCount; i++)
            {
                ColumnDefinition column = new ColumnDefinition
                {
                    Width = new GridLength(200, GridUnitType.Pixel)
                };
                Grid stage = RoundGrid(i);
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

        private void SelectNomination(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            selectedNomination = button.Tag.ToString();
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
            ShowTournamentGrid((SubgroupWrapper)subgroupButton.Tag);
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

        private void ShowTournamentGrid(SubgroupWrapper subgroup)
        {
            tournamentGrid.Children.Clear();
            SetAddButtonToLastColumn();
        }

        private void SelectCategory(object sender, RoutedEventArgs e)
        {
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