using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TournamentSoftware
{
    /// <summary>
    /// Interaction logic for TournamentGridWindow.xaml
    /// </summary>
    public partial class TournamentGridWindow : Window
    {

        private bool isPanelOpen = true;
        private string selectedNomination = "";
        private string selectedKategory = "";
        private Dictionary<string, Dictionary<string, List<ParticipantFormModel>>> nominationsDictionary;
        public TournamentGridWindow()
        {
            InitializeComponent();
        }

        public void Show(Dictionary<string, Dictionary<string, List<ParticipantFormModel>>> nominationsDictionary)
        {
            this.nominationsDictionary = nominationsDictionary;
            this.Show();
        }

        private void hideInstrumentsPanel(object sender, RoutedEventArgs e)
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
            showNominationsList();
        }

        private void showNominationsList()
        {
            ParticipantsReagistrator participantsReagistrator = MainWindow.GetReagistrator;
            List<string> nominationsList = participantsReagistrator.nominationsNames;
            nominationsList.ForEach(nomination => {
                Button button = new Button();
                button.Content = nomination;
                button.Margin = new Thickness(5);
                button.Tag = nomination;
                button.Click += selectNomination;
                nominationsStackPanel.Children.Add(button);
            });
        }

        private void selectNomination(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            selectedNomination = button.Tag.ToString();
            createKategoriesTabs();
        }

        private void judgesRegistrationButton_Click(object sender, RoutedEventArgs e)
        {
            JudgesRegistrationWindow judgesRegistrationWindow = new JudgesRegistrationWindow();
            judgesRegistrationWindow.Show();
        }

        private void createKategoriesTabs()
        {
            Dictionary<string, List<ParticipantFormModel>> kategories = nominationsDictionary[selectedNomination];
            tabsGridRow.Children.Clear();
            tabsGridRow.ColumnDefinitions.Clear();
            foreach (KeyValuePair<string, List<ParticipantFormModel>> keyValuePair in kategories)
            {
                Grid kategoryGrid = new Grid();
                RowDefinition row1 = new RowDefinition();
                Button button = new Button();
                button.HorizontalAlignment = HorizontalAlignment.Stretch;
                button.VerticalAlignment = VerticalAlignment.Stretch;
                button.Content = keyValuePair.Key;
                button.Click += Button_Click;
                kategoryGrid.RowDefinitions.Add(row1);
                kategoryGrid.Children.Add(button);
                Grid.SetRow(button, 0);

                ColumnDefinition columnDefinition = new ColumnDefinition();
                tabsGridRow.ColumnDefinitions.Add(columnDefinition);
                tabsGridRow.Children.Add(kategoryGrid);
                Grid.SetColumn(kategoryGrid, tabsGridRow.ColumnDefinitions.Count-1);

            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
           
        }
    }
}
