using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using static TournamentSoftware.ApplicationStringValues;
using static TournamentSoftware.TournamentData;

namespace TournamentSoftware
{
    public partial class ViewSettingsWindow : Window
    {
        public ViewSettingsWindow()
        {
            InitializeComponent();
        }

        private List<CheckBox> checkBoxes = new List<CheckBox>();
        ObservableCollection<DataGridColumn> mainWindowColumns = ((MainWindow)Application.Current.MainWindow).registrationTable.Columns;
        ObservableCollection<DataGridTemplateColumn> mainNominationsColumns = ((MainWindow)Application.Current.MainWindow).nominationsColumn;
        private List<NominationFormModel> newNominations = new List<NominationFormModel>();
        private List<NominationFormModel> nominationsForDelete = new List<NominationFormModel>();

        /// <summary>
        /// Закрываем окно настроек
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseSettingsWindow(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Применить настройки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetNewSettings(object sender, RoutedEventArgs e)
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
            for (int i = 0; i < newNominations.Count; i++)
            {
                DataGridTemplateColumn nominationColumn = new DataGridTemplateColumn();
                string nominationName = newNominations[i].Nomination.Name;
                if (CheckNominationNameValid(nominationName))
                {
                    if (!IsNominationExists(nominationName))
                    {
                        nominationColumn.Header = nominationName;
                        nominationColumn.CanUserResize = false;
                        nominationColumn.Width = new DataGridLength(1, DataGridLengthUnitType.Star);

                        Binding bind = new Binding("Nominations[" + nominationName + "]");
                        bind.Mode = BindingMode.TwoWay;

                        var cellStyle = new Style(typeof(DataGridCell));
                        cellStyle.Setters.Add(new Setter()
                        {
                            Property = BackgroundProperty,
                            Value = (Brush)new BrushConverter().ConvertFrom("#F5F1DA")
                        });
                        nominationColumn.CellStyle = cellStyle;

                        FrameworkElementFactory checkBox = new FrameworkElementFactory(typeof(CheckBox));
                        checkBox.SetBinding(CheckBox.IsCheckedProperty, bind);
                        checkBox.SetValue(CheckBox.HorizontalAlignmentProperty, HorizontalAlignment.Center);
                        checkBox.SetValue(CheckBox.VerticalAlignmentProperty, VerticalAlignment.Center);
                        DataTemplate checkBoxTemplate = new DataTemplate();
                        checkBoxTemplate.VisualTree = checkBox;

                        nominationColumn.CellTemplate = checkBoxTemplate;

                        foreach (ParticipantFormModel p in participantsList)
                        {
                            p.Nominations.Add(nominationName, false);
                        }

                        //mainNominationsColumns.Add(nominationColumn);
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
            for (int i = 0; i < nominationsForDelete.Count; i++)
            {
                for (int j = 0; j < mainNominationsColumns.Count; j++)
                {
                    string nominationName = nominationsForDelete[i].Nomination.Name;
                    // находим удаляемую колонку и удаляем из таблицы
                    if (nominationName.Equals(mainNominationsColumns[j].Header))
                    {
                        mainWindowColumns.Remove(mainNominationsColumns[j]);
                        // удаляем номинации у участников
                        foreach (ParticipantFormModel p in participantsList)
                        {
                            p.Nominations.Remove(nominationName);
                        }
                        RemoveNomination(nominationName);
                    }
                }
            }

            for (int i = 0; i < nominationsForDelete.Count; i++)
            {
                for (int j = 0; j < mainNominationsColumns.Count; j++)
                {
                    if (nominationsForDelete[i].Nomination.Name.Equals(mainNominationsColumns[j].Header))
                    {
                        mainNominationsColumns.Remove(mainNominationsColumns[j]);
                    }
                }
            }

            nominations.Clear();
            nominationsForDelete.Clear();

            Close();
        }

        /// <summary>
        /// Проверка что название номинации не совпадает с названием обязательных столбцов
        /// </summary>
        /// <param name="nominationName"></param>
        /// <returns></returns>
        private bool CheckNominationNameValid(string nominationName)
        {
            return !(stringsConstantsValues.Contains(nominationName) || nominationName.Equals(nominationName));
        }

        /// <summary>
        /// Добавление новой номинации
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddNomination(object sender, RoutedEventArgs e)
        {
            NominationFormModel nomination = new NominationFormModel()
            {
                IsSelected = false,
            };

            newNominations.Add(nomination);
            nominationsGrid.ItemsSource = newNominations;
        }

        /// <summary>
        /// Удаление выбранных номинаций
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteSelectedNominations(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < nominations.Count;)
            {
                if (nominations[i].IsSelected)
                {
                    nominationsForDelete.Add(nominations[i]);
                    nominations.Remove(nominations[i]);
                }
                else
                {
                    i++;
                }
            }
            nominationsGrid.ItemsSource = newNominations;
        }

        /// <summary>
        /// Выбираем все номинации на удаление
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectAllForDelete_Checked(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < nominations.Count; i++)
            {
                nominations[i].IsSelected = true;
            }
        }

        /// <summary>
        /// Убираем все чеки у номинаций
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectAllForDelete_Unchecked(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < nominations.Count; i++)
            {
                nominations[i].IsSelected = false;
            }
        }

        private void SettingsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            CreateCheckBoxes();
            LoadNominations();
        }

        private void CreateCheckBoxes()
        {
            for (int i = 1; i < mainWindowColumns.Count; i++)
            {
                CheckBox checkBox = new CheckBox
                {
                    IsChecked = false,
                    Content = mainWindowColumns[i].Header
                };
                if (mainWindowColumns[i].Visibility == Visibility.Visible)
                {
                    checkBox.IsChecked = true;
                }
                checkBoxes.Add(checkBox);
                columnsNames.Items.Add(checkBox);
            }
        }

        private void LoadNominations()
        {
            for (int i = 0; i < nominations.Count; i++)
            {
                newNominations.Add(nominations[i]);
            }
            nominationsGrid.ItemsSource = newNominations;
        }

        /// <summary>
        /// Чекбокс у номинации установлен
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NominationSelected(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// Чекбокс у номинации убран
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NominationUnselected(object sender, RoutedEventArgs e)
        {

        }
    }
}
