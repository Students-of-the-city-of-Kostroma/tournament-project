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

        private void CreateCategoriesTabs()
        {
            categoryTabsGrid.Children.Clear();
            categoryTabsGrid.ColumnDefinitions.Clear();
            List<string> categoryNames = GetCategoryNames();
            foreach (string category in categoryNames)
            {
                Grid categoryGrid = new Grid();
                RowDefinition row1 = new RowDefinition();
                Button categoryButton = new Button
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Content = category
                };
                categoryButton.Click += SelectCategory;
                categoryGrid.RowDefinitions.Add(row1);
                categoryGrid.Children.Add(categoryButton);
                Grid.SetRow(categoryButton, 0);

                ColumnDefinition columnDefinition = new ColumnDefinition();
                categoryTabsGrid.ColumnDefinitions.Add(columnDefinition);
                categoryTabsGrid.Children.Add(categoryGrid);
                Grid.SetColumn(categoryGrid, categoryTabsGrid.ColumnDefinitions.Count - 1);
            }
        }

        private void SelectCategory(object sender, RoutedEventArgs e)
        {
        }
    }
}