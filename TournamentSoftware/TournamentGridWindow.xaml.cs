using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using static TournamentSoftware.TournamentData;

namespace TournamentSoftware
{
    public partial class TournamentGridWindow : Window
    {
        private bool isPanelOpen = true;
        private string selectedNomination = "";
        private string selectedCategory = "";
        private List<Button> categoryButtons = new List<Button>();
        public TournamentGridWindow()
        {
            InitializeComponent();
            WindowState = WindowState.Maximized;
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
                };
                subgroupsGrid.ColumnDefinitions.Add(column);
                subgroupsGrid.Children.Add(subgroupButton);
                Grid.SetColumn(subgroupButton, subgroupsGrid.ColumnDefinitions.Count - 1);
            }
            return subgroupsGrid;
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