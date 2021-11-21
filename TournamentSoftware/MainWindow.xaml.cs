using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using SQLite;

namespace TournamentSoftware
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            appGrid.Visibility = Visibility.Hidden;
            goTournament.IsEnabled = false;
        }

        private void NumericOnly(System.Object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (!int.TryParse(e.Text, out _)) { e.Handled = false; }
        }

        public static int participantCount = 0;
        public static List<Participant> participantsList = new List<Participant>();
        private BindingList<Participant> p_list = new BindingList<Participant>();

        /// <summary>
        /// Переход к модулю регистрации
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void goRegistrate(object sender, RoutedEventArgs e)
        {
            startWindowLabel.Visibility = Visibility.Hidden;
            goRegistrateButton.Visibility = Visibility.Hidden;
            appGrid.Visibility = Visibility.Visible;
        }

        private void addParticipant(object sender, RoutedEventArgs e)
        {
            participantCount++;

            Participant participant = new Participant()
            {
                Name = "",
                Surname = "",
                Otchestvo = "",
                Psevdonim = "",
                Posevnoy = false,
                Club = "",
                City = "",
                DateOfBirth = -1,
                Height = -1,
                Weight = -1,
                Kategory = "",
                Sex = "",
                CommonRating = -1,
                ClubRating = -1,
                AvailableSex = new string[2] { "М", "Ж" },
            };

            p_list.Add(participant);

            // participantsList.Add(participant);

            registrationTable.ItemsSource = p_list;

            // DbConnection.connect<Participant>(participant as object);
        }

        private void deleteParticipant(object sender, RoutedEventArgs e)
        {
        }

        private void selectorAllForDelete_Unchecked(object sender, RoutedEventArgs e)
        {
            delete.IsEnabled = false;
        }

        private void selectorAllForDelete_Checked(object sender, RoutedEventArgs e)
        {
            delete.IsEnabled = true;
        }
    }

    public class DbConnection : Window
    {
        public static void connect<T>(object obj)
        {

            using (SQLiteConnection connection = new SQLiteConnection("db.db"))
            {
                connection.CreateTable<T>();
                connection.Insert(obj);
            }

        }
    }
}
