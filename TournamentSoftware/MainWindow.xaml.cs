using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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

        public static int participantCount = 0;

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
            Label id = new Label();
            id.Content = participantCount;
            id.SetValue(Grid.RowProperty, participantCount);
            id.SetValue(Grid.ColumnProperty, 0);

            TextBox name = new TextBox();
            name.SetValue(Grid.RowProperty, participantCount);
            name.SetValue(Grid.ColumnProperty, 1);


            RowDefinition row = new RowDefinition();
            row.Height = new GridLength(50, GridUnitType.Pixel);
            Border border = new Border();
            border.BorderBrush = Brushes.Black;
            border.BorderThickness = new Thickness(1);
            border.SetValue(Grid.RowProperty, participantCount);
            border.SetValue(Grid.ColumnSpanProperty, 16);
            registrationTable.RowDefinitions.Add(row);
            registrationTable.Children.Add(id);
            registrationTable.Children.Add(name);
            registrationTable.Children.Add(border);

            Participant participant = new Participant()
            {
                Id = participantCount,
                Name = null,
                Surname = null,
                Otcestvo = null,
                Psevdonim = null,
                DateOfBirth = 0,
                Sex = null,
                Posevnoy = false,
                Club = null,
                City = null,
                Height = 0,
                Weight = 0,
                CommonRating = 0,
                ClubRating = 0,
                Nominations = "",
            };

            DbConnection.connect<Participant>(participant as object);
        }

        private void deleteParticipant(object sender, RoutedEventArgs e)
        {
            if (registrationTable.RowDefinitions.Count != 1) { 

            }
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
