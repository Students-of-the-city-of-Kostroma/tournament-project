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
            if (isPanelOpen)
            {
                mainGrid.ColumnDefinitions[1].Width = new GridLength(60);
                controlsGrid.Visibility = Visibility.Hidden;
                isPanelOpen = false;
            }
            else
            {
                mainGrid.ColumnDefinitions[1].Width = new GridLength(160);
                controlsGrid.Visibility = Visibility.Visible;
                isPanelOpen = true;
            }
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
                Button button = new Button();
                button.Content = nominationName;
                button.Margin = new Thickness(5);
                button.Tag = nominationName;
                button.Click += SelectNomination;
                nominationsStackPanel.Children.Add(button);
            }
        }

        private void SelectNomination(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            selectedNomination = button.Tag.ToString();
            CreateKategoriesTabs();
        }

        private void JudgesRegistrationButton_Click(object sender, RoutedEventArgs e)
        {
            JudgesRegistrationWindow judgesRegistrationWindow = new JudgesRegistrationWindow();
            judgesRegistrationWindow.Show();
        }

        private void CreateKategoriesTabs()
        {
            tabsGridRow.Children.Clear();
            tabsGridRow.ColumnDefinitions.Clear();
            List<string> categoryNames = GetCategoryNames();
            foreach (string category in categoryNames)
            {
                Grid kategoryGrid = new Grid();
                RowDefinition row1 = new RowDefinition();
                Button button = new Button();
                button.HorizontalAlignment = HorizontalAlignment.Stretch;
                button.VerticalAlignment = VerticalAlignment.Stretch;
                button.Content = category;
                button.Click += Button_Click;
                kategoryGrid.RowDefinitions.Add(row1);
                kategoryGrid.Children.Add(button);
                Grid.SetRow(button, 0);

                ColumnDefinition columnDefinition = new ColumnDefinition();
                tabsGridRow.ColumnDefinitions.Add(columnDefinition);
                tabsGridRow.Children.Add(kategoryGrid);
                Grid.SetColumn(kategoryGrid, tabsGridRow.ColumnDefinitions.Count - 1);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
        }

        private void openFightProtocolButton_Click(object sender, RoutedEventArgs e)
        {
            BattleProtocolWindow battleProtocolWindow = new BattleProtocolWindow();
            battleProtocolWindow.Show();
        }
    }
}