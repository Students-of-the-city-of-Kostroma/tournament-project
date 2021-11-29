using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

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
        ObservableCollection<DataGridColumn> mainWindowColumns = ((MainWindow)Application.Current.MainWindow).registrationTable.Columns;
        ObservableCollection<DataGridTemplateColumn> mainNominationsColumns = ((MainWindow)Application.Current.MainWindow).nominationsColumn;
        ObservableCollection<Participant> participants = ((MainWindow)Application.Current.MainWindow).participantsList;

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
                        if (checkBoxes[i].IsChecked == false)
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
            }

            // добавляем новые номинации в таблицу регистрации
            for (int i = 0; i < nominationsList.Count; i++)
            {
                DataGridTemplateColumn nominationColumn = new DataGridTemplateColumn();
                string nominationName = nominationsList[i].NominationName;
                if (checkNominationNameValid(nominationName))
                {
                    if (!checkNominationAlreadyExists(nominationName))
                    {
                        nominationColumn.Header = nominationName;
                        nominationColumn.CanUserResize = false;
                        nominationColumn.Width = new DataGridLength(1, DataGridLengthUnitType.Star);

                        Binding bind = new Binding("Nominations[" + nominationName + "]");
                        bind.Mode = BindingMode.TwoWay;

                        FrameworkElementFactory checkBox = new FrameworkElementFactory(typeof(CheckBox));
                        checkBox.SetBinding(CheckBox.IsCheckedProperty, bind);
                        DataTemplate checkBoxTemplate = new DataTemplate();
                        checkBoxTemplate.VisualTree = checkBox;

                        nominationColumn.CellTemplate = checkBoxTemplate;

                        foreach (Participant p in participants)
                        {
                            p.Nominations.Add(nominationName, false);
                        }

                        mainNominationsColumns.Add(nominationColumn);
                        mainWindowColumns.Add(nominationColumn);
                    }

                }
                else
                {
                    MessageBox.Show("Недопустимое название номинации " + nominationName +
                        "!\nТакое название уже имеет существующий столбец, номинация " + nominationName +
                        " не будет добавлена в таблицу", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            // удаляем номинации из таблицы регистрации
            for (int i = 0; i < deletedNominations.Count; i++)
            {
                for (int j = 0; j < mainNominationsColumns.Count; j++)
                {
                    // находим удаляемую колонку и удаляем из таблицы
                    if (deletedNominations[i].NominationName.Equals(mainNominationsColumns[j].Header))
                    {
                        Console.WriteLine("delete " + deletedNominations[i].NominationName);
                        mainWindowColumns.Remove(mainNominationsColumns[j]);
                        // удаляем номинации у участников
                        foreach (Participant p in participants)
                        {
                            p.Nominations.Remove(deletedNominations[i].NominationName);
                        }
                        // mainNominationsColumns.Remove(mainNominationsColumns[j]);
                    }
                }
            }

            for (int i = 0; i < deletedNominations.Count; i++)
            {
                for (int j = 0; j < mainNominationsColumns.Count; j++)
                {
                    if (deletedNominations[i].NominationName.Equals(mainNominationsColumns[j].Header))
                    {
                        mainNominationsColumns.Remove(mainNominationsColumns[j]);
                    }
                }
            }

            nominationsList.Clear();
            deletedNominations.Clear();

            this.Close();
        }

        /// <summary>
        /// Проверка что такой номинации еще не существует
        /// </summary>
        /// <param name="nominationName"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Проверка что название номинации не совпадает с названием обязательных столбцов
        /// </summary>
        /// <param name="nominationName"></param>
        /// <returns></returns>
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
                    deletedNominations.Add(nominationsList[i]);
                    nominationsList.Remove(nominationsList[i]);

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

        private void settingsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // загружаем чекбоксы для скрытия столбцов
            for (int i = 1; i < mainWindowColumns.Count; i++)
            {
                CheckBox checkBox = new CheckBox();
                checkBox.IsChecked = false;
                checkBox.Content = mainWindowColumns[i].Header;
                if (mainWindowColumns[i].Visibility == Visibility.Visible)
                {
                    checkBox.IsChecked = true;
                }
                checkBoxes.Add(checkBox);
                columnsNames.Items.Add(checkBox);
            }

            // загружаем столбцы номинаций
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
