using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Windows.Input;
using System.Windows.Data;
using Newtonsoft.Json;
using System.Windows.Media;
using System.Windows.Controls.Primitives;

namespace TournamentSoftware
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static ObservableCollection<ParticipantFormModel> participantsList = new ObservableCollection<ParticipantFormModel>();

        public static ObservableCollection<Nomination> nominationsList = new ObservableCollection<Nomination>();
        public ObservableCollection<DataGridTemplateColumn> nominationsColumn = new ObservableCollection<DataGridTemplateColumn>();
        public static string appStateJsonPath = "..\\..\\app.json";
        public static string dataBasePath = "..\\..\\db.db";
        public static string registrationBackupPath = "..\\..\\registrationBackup.json";
        private SubgroupsFormation subgroupsFormation;
        private static ParticipantsReagistrator registrator = new ParticipantsReagistrator();
        public ApplicationState appState = new ApplicationState();

        public static ParticipantsReagistrator GetReagistrator { get { return registrator; } }

        private static List<string> requiredColumnsHeaders = new List<string>{
            "Имя",
            "Фамилия",
            "Отчество",
            "Посевной",
            "Пол",
            "Год рождения",
            "Клуб",
            "Город",
            "Рост",
            "Вес",
            "Рейтинг (общий)",
            "Рейтинг (клубный)",
            "Псевдоним",
            "Категория"
        };

        public MainWindow()
        {
            InitializeComponent();
            Visibility = Visibility.Hidden;
            StartWindow startWindow = new StartWindow();
            startWindow.Show();
            startWindow.Closed += StartWindow_Closed;
        }

        private void StartWindow_Closed(object sender, EventArgs e)
        {
            Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Проверка введения числа
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NumericOnly(object sender, TextCompositionEventArgs e)
        {
            if (!int.TryParse(e.Text, out _))
            {
                e.Handled = true;
            }
            else
            {
                if (((TextBox)e.Source).Text.Equals("0"))
                {
                    ((TextBox)e.Source).Text = "";
                }
            }
        }

        /// <summary>
        /// алгоритм добавление участника
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addParticipant(object sender, RoutedEventArgs e)
        {
            Participant participant = new Participant()
            {
                Name = "",
                Surname = "",
                Patronymic = "",
                Pseudonym = "",
                Leader = false,
                DateOfBirth = 0,
                Height = 0,
                Weight = 0,
                CommonRating = 0,
                ClubRating = 0,
                ClubId = 0,
            };

            ParticipantFormModel participantFormModel = new ParticipantFormModel()
            {
                Participant = participant,
                Club = "",
                City = "",
                AvailableSex = new string[2] { "М", "Ж" },
                IsSelected = false,
            };

            participantsList.Add(participantFormModel);
            registrationTable.ItemsSource = participantsList;
            exportButton.IsEnabled = true;
        }

        /// <summary>
        /// Удаление отмеченных участников
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteParticipant(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < participantsList.Count;)
            {
                if (participantsList[i].IsSelected)
                {
                    participantsList.Remove(participantsList[i]);
                }
                else
                {
                    i++;
                }
            }
            // если удалены все участники удаляем колонки с номинациями
            if (participantsList.Count == 0)
            {
                deleteNominationsColumns();
                exportButton.IsEnabled = false;
            }

            delete.IsEnabled = false;
            selectorAllForDelete_Unchecked(sender, e);
        }

        /// <summary>
        /// Удаление всех колонок с номинациями
        /// </summary>
        private void deleteNominationsColumns()
        {
            foreach (DataGridTemplateColumn column in nominationsColumn)
            {
                registrationTable.Columns.Remove(column);
            }

            nominationsList.Clear();
        }

        /// <summary>
        /// снятие галочек со всех участников
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selectorAllForDelete_Unchecked(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < participantsList.Count; i++)
            {
                participantsList[i].IsSelected = false;
            }
            delete.IsEnabled = false;
            DataGridColumn newCol = checkboxCol;
            registrationTable.Columns.Remove(checkboxCol);
            registrationTable.Columns.Insert(0, newCol);
        }

        /// <summary>
        /// выделение всех участников для удаления
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selectorAllForDelete_Checked(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < participantsList.Count; i++)
            {
                participantsList[i].IsSelected = true;
            }
            delete.IsEnabled = true;
        }

        /// <summary>
        /// При закрытии приложения - запись в промежуточный файл
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // если были участники и не перешли к турниру
            if (participantsList.Count > 0 && !appState.IsTournamentComplited)
            {
                appState.isRegistrationComplited = false;
                registrator.backupRegistrationTable();
            }
            // если нет участников
            else
            {
                appState.isRegistrationComplited = true;
            }
            string json = JsonConvert.SerializeObject(appState);
            File.WriteAllText(appStateJsonPath, json);
        }

        private void readRegistrationFromBackup()
        {
            List<ParticipantFormModel> participants = registrator.getParticipantsFromBackup(registrationBackupPath);
            if (participants != null && participants.Count > 0)
            {
                foreach (ParticipantFormModel participant in participants)
                {
                    participantsList.Add(participant);
                    if (participant.Nominations.Count > 0)
                    {
                        foreach (string nomination in participant.Nominations.Keys)
                        {
                            addNominationColumn(nomination);
                            if (!registrator.nominationsNames.Contains(nomination))
                            {
                                registrator.nominationsNames.Add(nomination);
                            }
                        }
                    }
                }
                exportButton.IsEnabled = true;
            }
        }

        private void mainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            participantsList = new ObservableCollection<ParticipantFormModel>();

            // проверяем на каком этапе закрылось приложение в прошлый раз
            StreamReader r = new StreamReader(appStateJsonPath);
            string json = r.ReadToEnd();
            appState = JsonConvert.DeserializeObject<ApplicationState>(json);
            // если закончили на этапе регистрации
            if (!appState.isRegistrationComplited)
            {
                readRegistrationFromBackup();
                registrationTable.ItemsSource = participantsList;
            }
            // если остановились на турнирной сетке
            else if (!appState.IsTournamentComplited)
            {

            }
            r.Close();
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
            for (int i = 0; i < participantsList.Count; i++)
            {
                if (participantsList[i].IsSelected)
                {
                    selectedCount++;
                }
            }

            if (selectedCount == 0)
            {
                delete.IsEnabled = false;
            }
        }

        /// <summary>
        /// Экспортируем тиблицу регистрации в файл
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveFile(object sender, RoutedEventArgs e)
        {
            registrator.saveFile(toDataTable(participantsList));
        }

        private DataTable toDataTable(ObservableCollection<ParticipantFormModel> participants)
        {
            DataTable dataTable = new DataTable();
            List<string> nominationsArray = new List<string>();
            for (int i = 0; i < nominationsColumn.Count; i++)
            {
                nominationsArray.Add(nominationsColumn[i].Header.ToString());
            }

            for (int i = 1; i < registrationTable.Columns.Count; i++)
            {
                dataTable.Columns.Add();
                string header = registrationTable.Columns[i].Header.ToString();
                Console.WriteLine(registrationTable.Columns[i].Header.ToString());
                dataTable.Columns[i - 1].Caption = header;
            }
            foreach (ParticipantFormModel participant in participants)
            {
                DataRow row = dataTable.NewRow();
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    string columnHeader = dataTable.Columns[i].Caption;
                    if (nominationsArray.Contains(columnHeader))
                    {
                        row[i] = participant.Nominations[columnHeader];
                    }
                    else
                    {
                        switch (columnHeader)
                        {
                            case "Имя":
                                row[i] = participant.Participant.Name;
                                break;
                            case "Фамилия":
                                row[i] = participant.Participant.Surname;
                                break;
                            case "Отчество":
                                row[i] = participant.Participant.Patronymic;
                                break;
                            case "Псевдоним":
                                row[i] = participant.Participant.Pseudonym;
                                break;
                            case "Посевной":
                                row[i] = participant.Participant.Leader;
                                break;
                            case "Пол":
                                row[i] = participant.Participant.Sex;
                                break;
                            case "Год рождения":
                                row[i] = participant.Participant.DateOfBirth.ToString();
                                break;
                            case "Клуб":
                                row[i] = participant.Club;
                                break;
                            case "Город":
                                row[i] = participant.City;
                                break;
                            case "Рост":
                                row[i] = participant.Participant.Height;
                                break;
                            case "Вес":
                                row[i] = participant.Participant.Weight;
                                break;
                            case "Категория":
                                row[i] = participant.Kategory;
                                break;
                            case "Рейтинг (общий)":
                                row[i] = participant.Participant.CommonRating;
                                break;
                            case "Рейтинг (клубный)":
                                row[i] = participant.Participant.ClubRating;
                                break;
                        }
                    }
                }
                dataTable.Rows.Add(row);
            }
            return dataTable;
        }


        /// <summary>
        /// Открываем инструменты для редактирования лейаута модуля регистрации
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void showRegistrationModuleSettings(object sender, RoutedEventArgs e)
        {
            Window1 settings = new Window1();
            settings.Show();
        }

        /// <summary>
        /// Выбираем файл с таблицей
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenExcel_Click(object sender, RoutedEventArgs e)
        {
            registrator.loadParticipantsFromFile(requiredColumnsHeaders);
            registrationTable.ItemsSource = participantsList;
            List<string> nominations = registrator.nominationsNames;
            foreach (string nomination in nominations)
            {
                addNominationColumn(nomination);
            }
        }

        private bool checkNominationExists(string nominationName)
        {
            foreach (DataGridTemplateColumn column in nominationsColumn)
            {
                if (column.Header.Equals(nominationName)) return true;

            }
            return false;
        }

        /// <summary>
        /// Добавление колонки номинации
        /// </summary>
        /// <param name="nominationName"></param>
        private void addNominationColumn(string nominationName)
        {
            if (!checkNominationExists(nominationName))
            {
                Binding bind = new Binding("Nominations[" + nominationName + "]");
                bind.Mode = BindingMode.TwoWay;
                DataGridTemplateColumn n = new DataGridTemplateColumn();
                n.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
                n.Header = nominationName;

                var cellStyle = new Style(typeof(DataGridCell));
                cellStyle.Setters.Add(new Setter()
                {
                    Property = BackgroundProperty,
                    Value = (Brush) new BrushConverter().ConvertFrom("#F5F1DA")
                });
                n.CellStyle = cellStyle;

                FrameworkElementFactory checkBox = new FrameworkElementFactory(typeof(CheckBox));
                checkBox.SetBinding(CheckBox.IsCheckedProperty, bind);
                checkBox.SetValue(CheckBox.HorizontalAlignmentProperty, HorizontalAlignment.Center);
                checkBox.SetValue(CheckBox.VerticalAlignmentProperty, VerticalAlignment.Center);
                DataTemplate checkBoxTemplate = new DataTemplate();
                checkBoxTemplate.VisualTree = checkBox;
                n.CellTemplate = checkBoxTemplate;
                registrationTable.Columns.Add(n);
                nominationsColumn.Add(n);
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("in focus Рейтинг (общий) " + ((TextBox)e.Source).Text);
        }

        private void ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (((ComboBox)e.Source).SelectedItem != "M" && ((ComboBox)e.Source).SelectedItem != "Ж")
            {
                SolidColorBrush scb = new SolidColorBrush(Color.FromRgb(255, 221, 219));
                ((ComboBox)e.Source).Background = scb;
            }
        }

        /// <summary>
        /// Проверка заполнения всех обязательных полей у участников
        /// </summary>
        /// <returns></returns>
        private bool isRegistrationTableValid()
        {
            List<string> errors = new List<string>();
            int count = 1;
            foreach (ParticipantFormModel participant in participantsList)
            {
                if (participant.Participant.Name.Equals(""))
                {
                    errors.Add("Заполните имя участника на строке " + count);
                }

                if (participant.Participant.Surname.Equals(""))
                {
                    errors.Add("Заполните фамилию участника на строке " + count);
                }

                if (participant.Club.Equals(""))
                {
                    errors.Add("Заполните клуб участника на строке " + count);
                }

                if (participant.City.Equals(""))
                {
                    errors.Add("Заполните город участника на строке " + count);
                }

                if (participant.Participant.DateOfBirth < 1900 || participant.Participant.DateOfBirth > DateTime.Now.Year - 13)
                {
                    errors.Add("Некорректно заполнен год рождения участника на строке " + count);
                }

                if (!participant.Participant.Sex.Equals("М") && !participant.Participant.Sex.Equals("Ж"))
                {
                    errors.Add("Заполните пол участника на строке " + count + " " + participant.Participant.Sex);
                }

                if (participant.Kategory.Equals(""))
                {
                    errors.Add("Заполните категорию участника на строке " + count);
                }

                count++;
            }
            if (errors.Count > 0 && TornnamentNameTextBox.Text.Equals(""))
            {
                ErrorListWindow errorListWindow = new ErrorListWindow();
                errorListWindow.ShowErrors(errors);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Переходим к турнирной сетке
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void goTournament_Click(object sender, RoutedEventArgs e)
        {
            if (isRegistrationTableValid())
            {
                appState.isRegistrationComplited = true;
                appGrid.Visibility = Visibility.Hidden;
                SubgroupsFormationGrid.Children.Clear();
                SubgroupsFormationGridParent.Visibility = Visibility.Visible;
                subgroupsFormation = new SubgroupsFormation();
                UIElement nominationList = subgroupsFormation.nominationsList();
                SubgroupsFormationGrid.Children.Add(nominationList);
                Grid.SetRow(nominationList, 0);
                Grid.SetColumn(nominationList, 0);

                UIElement grid = subgroupsFormation.kategoryList();
                SubgroupsFormationGrid.Children.Add(grid);
                Grid.SetRow(grid, 0);
                Grid.SetColumn(grid, 1);

                UIElement kategoryParametersPanel = subgroupsFormation.kategorySettingsPanel();
                SubgroupsFormationGrid.Children.Add(kategoryParametersPanel);
                Grid.SetRow(kategoryParametersPanel, 0);
                Grid.SetColumn(kategoryParametersPanel, 2);
            }
            else
            {
                appState.isRegistrationComplited = false;
            }
        }

        private void DataGridTextColumn_PastingCellClipboardContent(object sender, DataGridCellClipboardEventArgs e)
        {
            Console.WriteLine(e.Content);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            appGrid.ColumnDefinitions[1].Width = new GridLength(50);

        }

        private void backToRegistratioinTable(object sender, RoutedEventArgs e)
        {
            SubgroupsFormationGridParent.Visibility = Visibility.Hidden;
            appGrid.Visibility = Visibility.Visible;
        }

        private void DateOfBirth_TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if ((sender as TextBox).Text == "" || Convert.ToInt32((sender as TextBox).Text) < 1900)
            {
                (sender as TextBox).Background = (Brush)new BrushConverter().ConvertFrom("#FFFFDDDB");
                if ((sender as TextBox).Text == "")
                {
                    (sender as TextBox).Text = "0";
                }
            }
            else 
            {
                (sender as TextBox).Background = (Brush)new BrushConverter().ConvertFrom("#FFF5F1DA");
            }
        }
    }
}
