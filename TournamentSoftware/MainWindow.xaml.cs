using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using SQLite;

namespace TournamentSoftware
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<Participant> participants_list = new ObservableCollection<Participant>();
        public ObservableCollection<Participant> ParticipantsCollection
        {
            get { return this.participants_list; }
        }
        public MainWindow()
        {
            InitializeComponent();
            appGrid.Visibility = Visibility.Hidden;
            goTournament.IsEnabled = false;
            participants_list = new ObservableCollection<Participant>();
            registrationTable.DataContext = participants_list;
        }

        private void NumericOnly(System.Object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (!int.TryParse(e.Text, out _)) { e.Handled = false; }
        }

        public static int participantCount = 0;
        public static List<Participant> participantsList = new List<Participant>();


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

        /// <summary>
        /// алгоритм добавление участника
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                IsSelected = false,
            };

            participants_list.Add(participant);
            registrationTable.ItemsSource = participants_list;

            // DbConnection.connect<Participant>(participant as object);
        }

        private void deleteParticipant(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Вего участников " + participants_list.Count);
            for (int i = 0; i < participants_list.Count;)
            {
                Console.WriteLine(participants_list[i].IsSelected);
                if (participants_list[i].IsSelected)
                {
                    Console.WriteLine(participants_list[i].Name);
                    participants_list.Remove(participants_list[i]);
                }
                else {
                    i++;
                }
            }
            delete.IsEnabled = false;
            selectorAllForDelete_Unchecked( sender, e);
            CheckBox newCheckBox = new CheckBox();
            newCheckBox.IsChecked = false;
        }

        private void selectorAllForDelete_Unchecked(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < participants_list.Count; i++)
            {
                participants_list[i].IsSelected = false;
            }
            for (int i = 0; i < participants_list.Count; i++)
            {
                Console.WriteLine(participants_list[i].IsSelected);
            }
            delete.IsEnabled = false;
            DataGridColumn newCol = checkboxCol;
            registrationTable.Columns.Remove(checkboxCol);
            registrationTable.Columns.Insert(0, newCol);

        }

        private void selectorAllForDelete_Checked(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < participants_list.Count; i++)
            {
                participants_list[i].IsSelected = true;
            }
            for (int i = 0; i < participants_list.Count; i++)
            {
                Console.WriteLine(participants_list[i].IsSelected);
            }
            delete.IsEnabled = true;
        }

        private void mainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            participants_list = new ObservableCollection<Participant>();
            registrationTable.DataContext = participants_list;
        }

        /// <summary>
        /// событие при чеке чекбокса участника
        /// разблокируем кнопку удалить
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void participantChecked(object sender, RoutedEventArgs e)
        {
            delete.IsEnabled = true;
        }

        /// <summary>
        /// событие при анчеке чекбокса участника
        /// если больше нет выбранных участников - блокируем кнопку удалить
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void participantUnchecked(object sender, RoutedEventArgs e)
        {
            int selectedCount = 0;
            for (int i = 0; i < participants_list.Count; i++) {
                if (participants_list[i].IsSelected) {
                    selectedCount++;
                }
            }

            if (selectedCount == 0) {
                delete.IsEnabled = false;
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
