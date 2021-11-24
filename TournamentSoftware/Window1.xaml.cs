using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private ObservableCollection<Nomination> nominationsList = new ObservableCollection<Nomination>();
        private ObservableCollection<Nomination> deletedNominations = new ObservableCollection<Nomination>();
        private List<CheckBox> checkBoxes = new List<CheckBox>();
        ObservableCollection<DataGridColumn> mainWindowColumns = ((MainWindow)System.Windows.Application.Current.MainWindow).registrationTable.Columns;
        ObservableCollection<DataGridCheckBoxColumn> mainNominationsColumns = ((MainWindow)System.Windows.Application.Current.MainWindow).nominationsColumn;

        /// <summary>
        /// Закрываем окно настроек
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeSettingsWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Применить настройки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void setNewSettings(object sender, RoutedEventArgs e)
        {
            // скрываем ненужные столбцы или наоборот делаем их видимыми
            for (int i = 0; i < checkBoxes.Count; i++)
            {

                for (int j = 1; j < mainWindowColumns.Count; j++)
                {
                    if (mainWindowColumns[j].Header.Equals(checkBoxes[i].Content))
                    {
                        if (checkBoxes[i].IsChecked == true)
                        {
                            mainWindowColumns[j].Visibility = Visibility.Hidden;
                            break;
                        }
                        else
                        {
                            mainWindowColumns[j].Visibility = Visibility.Visible;
                            break;
                        }
                    }
                }

                if (checkBoxes[i].IsChecked == true)
                {
                    for (int j = 1; j < mainWindowColumns.Count; j++)
                    {
                        if (mainWindowColumns[j].Header.Equals(checkBoxes[i].Content))
                        {
                            mainWindowColumns[j].Visibility = Visibility.Hidden;
                            break;
                        }
                    }
                }
            }

            // добавляем новые номинации в таблицу регистрации
            for (int i = 0; i < nominationsList.Count; i++)
            {
                DataGridCheckBoxColumn dataGridCheckBoxColumn = new DataGridCheckBoxColumn();
                if (checkNominationNameValid(nominationsList[i].NominationName))
                {
                    if (!checkNominationAlreadyExists(nominationsList[i].NominationName))
                    {
                        dataGridCheckBoxColumn.Header = nominationsList[i].NominationName;
                        dataGridCheckBoxColumn.CanUserResize = false;
                        mainNominationsColumns.Add(dataGridCheckBoxColumn);
                        mainWindowColumns.Add(dataGridCheckBoxColumn);
                    }

                }
                else
                {
                    MessageBox.Show("Недопустимое название номинации " + nominationsList[i].NominationName +
                        "!\nТакое название уже имеет существующий столбец, номинация " + nominationsList[i].NominationName +
                        " не будет добавлена в таблицу", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            // удаляем номинации из таблицы регистрации
            for (int i = 0; i < deletedNominations.Count; i++)
            {
                for (int j = 0; j < mainNominationsColumns.Count; j++)
                {
                    if (deletedNominations[i].NominationName.Equals(mainNominationsColumns[j].Header))
                    {
                        mainWindowColumns.Remove(mainNominationsColumns[j]);
                        mainNominationsColumns.Remove(mainNominationsColumns[j]);
                    }
                }
            }

            this.Close();
        }

        private bool checkNominationAlreadyExists(string nominationName)
        {

            for (int i = 0; i < mainNominationsColumns.Count; i++)
            {
                if (mainNominationsColumns[i].Header.Equals(nominationName))
                {
                    return true;
                }
            }
            return false;
        }

        private bool checkNominationNameValid(string nominationName)
        {
            if (nominationName.Equals("Имя") ||
                nominationName.Equals("Фамилия") ||
                nominationName.Equals("Отчество") ||
                nominationName.Equals("Посевной") ||
                nominationName.Equals("Пол") ||
                nominationName.Equals("Год рождения") ||
                nominationName.Equals("Клуб") ||
                nominationName.Equals("Город") ||
                nominationName.Equals("Рост") ||
                nominationName.Equals("Вес") ||
                nominationName.Equals("Рейтинг (общий)") ||
                nominationName.Equals("Рейтинг (клубный)") ||
                nominationName.Equals("Псевдоним") ||
                nominationName.Equals("Категория") ||
                nominationName.Equals("")
                )
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Добавление новой номинации
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addNomination(object sender, RoutedEventArgs e)
        {
            Nomination nomination = new Nomination()
            {
                NominationName = "",
                IsSelected = false,
            };

            nominationsList.Add(nomination);
            nominationsGrid.ItemsSource = nominationsList;
        }

        /// <summary>
        /// Удаление выбранных номинаций
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteSelectedNominations(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < nominationsList.Count;)
            {
                if (nominationsList[i].IsSelected)
                {
                    nominationsList.Remove(nominationsList[i]);
                    deletedNominations.Add(nominationsList[i]);
                }
                else
                {
                    i++;
                }
            }
        }

        /// <summary>
        /// Выбираем все номинации на удаление
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selectAllForDelete_Checked(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < nominationsList.Count; i++)
            {
                nominationsList[i].IsSelected = true;
            }
        }

        /// <summary>
        /// Убираем все чеки у номинаций
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selectAllForDelete_Unchecked(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < nominationsList.Count; i++)
            {
                nominationsList[i].IsSelected = false;
            }
        }

        private void nominationUnchecked(object sender, RoutedEventArgs e)
        {

        }

        private void nominationChecked(object sender, RoutedEventArgs e)
        {

        }

        private void settingsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // загружаем чекбоксы для скрытия столбцов
            for (int i = 1; i < mainWindowColumns.Count; i++)
            {
                CheckBox checkBox = new CheckBox();
                checkBox.IsChecked = false;
                checkBox.Content = mainWindowColumns[i].Header;
                if (mainWindowColumns[i].Visibility == Visibility.Hidden)
                {
                    checkBox.IsChecked = true;
                }
                checkBox.Checked += CheckBox_Checked;
                checkBoxes.Add(checkBox);
                columnsNames.Items.Add(checkBox);
            }

            for (int i = 0; i < mainNominationsColumns.Count; i++)
            {
                Nomination nomination = new Nomination()
                {
                    NominationName = mainNominationsColumns[i].Header.ToString(),
                    IsSelected = false,
                };

                nominationsList.Add(nomination);
                nominationsGrid.ItemsSource = nominationsList;
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }
    }

    public class Nomination : INotifyPropertyChanged
    {
        private string _nominationName;
        private bool _isSelected;

        public string NominationName
        {
            get { return _nominationName; }
            set { _nominationName = value; OnPropertyChanged("NominationName"); }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set { _isSelected = value; OnPropertyChanged("IsSelected"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
