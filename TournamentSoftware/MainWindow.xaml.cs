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
using static TournamentSoftware.ApplicationResourcesPaths;
using static TournamentSoftware.ApplicationStringValues;
using static TournamentSoftware.TournamentData;
using System.Linq;

namespace TournamentSoftware
{
    public partial class MainWindow : Window
    {
        private Subgrouping subgroupsFormation;
        private static ParticipantsReagistrator registrator = new ParticipantsReagistrator();
        public ApplicationState appState = new ApplicationState();
        private bool isPanelOpen = true;

        public static ParticipantsReagistrator GetReagistrator { get { return registrator; } }
        public static ObservableCollection<ParticipantWrapper> GetPartisipants { get { return participants; } }

        private static List<string> requiredColumnsHeaders = new List<string>{
            name,
            surname,
            patronymic,
            leader,
            sex,
            dateOfBirth,
            club,
            city,
            height,
            weight,
            commonRating,
            clubRating,
            pseudonym,
            category
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

            ParticipantWrapper participantFormModel = new ParticipantWrapper()
            {
                Participant = participant,
                Club = "",
                City = "",
                AvailableSex = new string[2] { "М", "Ж" },
                IsSelected = false,
            };

            participants.Add(participantFormModel);
            registrationTable.ItemsSource = participants;
            exportButton.IsEnabled = true;
        }

        /// <summary>
        /// Удаление отмеченных участников
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteParticipant(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < participants.Count;)
            {
                if (participants[i].IsSelected)
                {
                    participants.Remove(participants[i]);
                }
                else
                {
                    i++;
                }
            }
            // если удалены все участники удаляем колонки с номинациями
            if (participants.Count == 0)
            {
                exportButton.IsEnabled = false;
            }

            deleteParticipantButton.IsEnabled = false;
            selectorAllForDelete_Unchecked(sender, e);
        }

        /// <summary>
        /// снятие галочек со всех участников
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selectorAllForDelete_Unchecked(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < participants.Count; i++)
            {
                participants[i].IsSelected = false;
            }
            deleteParticipantButton.IsEnabled = false;
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
            for (int i = 0; i < participants.Count; i++)
            {
                participants[i].IsSelected = true;
            }
            deleteParticipantButton.IsEnabled = true;
        }

        /// <summary>
        /// При закрытии приложения - запись в промежуточный файл
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // если были участники и не перешли к турниру
            if (participants.Count > 0 && !appState.IsTournamentComplited)
            {
                appState.isRegistrationComplited = false;
                registrator.BackupRegistrationTable(participants, registrationBackupPath);
            }
            // если нет участников
            else
            {
                appState.isRegistrationComplited = true;
            }
            appState.WindowWidth = (int)this.ActualWidth;
            appState.WindowHeight = (int)this.ActualHeight;
            appState.TournamentName = TournamentNameTextBox.Text;
            string json = JsonConvert.SerializeObject(appState);
            File.WriteAllText(appStateJsonPath, json);
        }

        private void ReadRegistrationFromBackup()
        {
            if (!File.Exists(registrationBackupPath))
                return;
            
            List<ParticipantWrapper> participants = registrator.GetParticipantsFromBackup(registrationBackupPath);
            if (participants != null && participants.Count > 0)
            {
                foreach (ParticipantWrapper participant in participants)
                {
                    TournamentData.participants.Add(participant);
                    if (participant.Nominations.Count > 0)
                    {
                        foreach (string nomination in participant.Nominations.Keys)
                        {
                            AddNominationColumn(nomination);
                            if (!IsNominationExists(nomination))
                            {
                                AddNomination(nomination);
                            }
                        }
                    }
                }
                exportButton.IsEnabled = true;
            }
        }

        private bool CorrectWindowSize()
        {
            return appState.WindowWidth > 0
                && appState.WindowHeight > 0;
        }

        private bool IsFullScreened() {
            var screenWidth = SystemParameters.WorkArea.Width;
            var screenHeight = SystemParameters.WorkArea.Height;
            return appState.WindowWidth >= screenWidth && appState.WindowHeight >= screenHeight;
        }

        private void setWindowSize()
        {
            if (CorrectWindowSize())
            {
                if (IsFullScreened()) {
                    Application.Current.MainWindow.WindowState = WindowState.Maximized;
                    return;
                }
                Width = appState.WindowWidth;
                Height = appState.WindowHeight;
            }
        }

        private void mainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            participants = new ObservableCollection<ParticipantWrapper>();

            if (!File.Exists(appStateJsonPath))
                return;

            // проверяем на каком этапе закрылось приложение в прошлый раз
            StreamReader r = new StreamReader(appStateJsonPath);
            string json = r.ReadToEnd();
            appState = JsonConvert.DeserializeObject<ApplicationState>(json);
            TournamentNameTextBox.Text = appState.TournamentName;

            setWindowSize();

            // если закончили на этапе регистрации
            if (!appState.isRegistrationComplited)
            {
                ReadRegistrationFromBackup();
                registrationTable.ItemsSource = participants;
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
            deleteParticipantButton.IsEnabled = true;
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
            for (int i = 0; i < participants.Count; i++)
            {
                if (participants[i].IsSelected)
                {
                    selectedCount++;
                }
            }

            if (selectedCount == 0)
            {
                deleteParticipantButton.IsEnabled = false;
            }
        }

        /// <summary>
        /// Экспортируем тиблицу регистрации в файл
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveFile(object sender, RoutedEventArgs e)
        {
            registrator.SaveFile(toDataTable(participants));
        }

        private DataTable toDataTable(ObservableCollection<ParticipantWrapper> participants)
        {
            DataTable dataTable = new DataTable();

            for (int i = 1; i < registrationTable.Columns.Count; i++)
            {
                dataTable.Columns.Add();
                string header = registrationTable.Columns[i].Header.ToString();
                dataTable.Columns[i - 1].Caption = header;
            }
            foreach (ParticipantWrapper participant in participants)
            {
                DataRow row = dataTable.NewRow();
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    string columnHeader = dataTable.Columns[i].Caption;
                    if (IsNominationExists(columnHeader))
                    {
                        row[i] = participant.Nominations[columnHeader];
                    }
                    else
                    {
                        switch (columnHeader)
                        {
                            case name:
                                row[i] = participant.Participant.Name;
                                break;
                            case surname:
                                row[i] = participant.Participant.Surname;
                                break;
                            case patronymic:
                                row[i] = participant.Participant.Patronymic;
                                break;
                            case pseudonym:
                                row[i] = participant.Participant.Pseudonym;
                                break;
                            case leader:
                                row[i] = participant.Participant.Leader;
                                break;
                            case sex:
                                row[i] = participant.Participant.Sex;
                                break;
                            case dateOfBirth:
                                row[i] = participant.Participant.DateOfBirth.ToString();
                                break;
                            case club:
                                row[i] = participant.Club;
                                break;
                            case city:
                                row[i] = participant.City;
                                break;
                            case height:
                                row[i] = participant.Participant.Height;
                                break;
                            case weight:
                                row[i] = participant.Participant.Weight;
                                break;
                            case category:
                                row[i] = participant.Category;
                                break;
                            case commonRating:
                                row[i] = participant.Participant.CommonRating;
                                break;
                            case clubRating:
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
            ViewSettingsWindow settings = new ViewSettingsWindow();
            settings.Show();
        }

        /// <summary>
        /// Выбираем файл с таблицей
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenExcel_Click(object sender, RoutedEventArgs e)
        {
            registrator.LoadParticipantsFromFile(requiredColumnsHeaders);
            registrationTable.ItemsSource = participants;
            foreach (NominationWrapper nomination in nominations)
            {
                AddNominationColumn(nomination.Nomination.Name);
            }
        }

        /// <summary>
        /// Добавление колонки номинации
        /// </summary>
        /// <param name="nominationName"></param>
        private void AddNominationColumn(string nominationName)
        {
            if (!IsNominationExists(nominationName))
            {
                Binding bind = new Binding("Nominations[" + nominationName + "]");
                bind.Mode = BindingMode.TwoWay;
                bind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

                DataGridTemplateColumn n = new DataGridTemplateColumn();
                n.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
                n.Header = nominationName;

                var cellStyle = new Style(typeof(DataGridCell));
                cellStyle.Setters.Add(new Setter()
                {
                    Property = BackgroundProperty,
                    Value = (Brush)new BrushConverter().ConvertFrom("#F5F1DA")
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
            }
        }

        private void ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (!((ComboBox)e.Source).SelectedItem.Equals("M") && !((ComboBox)e.Source).SelectedItem.Equals("Ж"))
            {
                SolidColorBrush scb = new SolidColorBrush(Color.FromRgb(255, 221, 219));
                ((ComboBox)e.Source).Background = scb;
            }
        }

        /// <summary>
        /// Проверка заполнения всех обязательных полей у участников
        /// </summary>
        /// <returns></returns>
        private bool IsRegistrationTableValid()
        {
            List<string> errors = new List<string>();
            int count = 1;
            if (nominations.Count != 0)
            {
                foreach (ParticipantWrapper participant in participants)
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

                    if (participant.Participant.Sex == null || !participant.Participant.Sex.Equals("М") && !participant.Participant.Sex.Equals("Ж"))
                    {
                        errors.Add("Заполните пол участника на строке " + count + " " + participant.Participant.Sex);
                    }

                    if (participant.Category == null || participant.Category.Equals(""))
                    {
                        errors.Add("Заполните категорию участника на строке " + count);
                    }

                    if (participant.Nominations == null)
                    {
                        errors.Add("Выберите номинацию участника на строке " + count);
                    }
                    else
                    {
                        int countTrue = 0;
                        foreach (KeyValuePair<string, bool> keyValuePair in participant.Nominations)
                        {
                            if (keyValuePair.Value)
                            {
                                countTrue++;
                            }
                        }

                        if (countTrue == 0)
                        {
                            errors.Add("Выберите номинацию участника на строке " + count);
                        }
                    }

                    count++;
                }
            }
            else
            {
                errors.Add("Добавьте хотябы 1 номинацию");
            }
            if (TournamentNameTextBox.Text.Equals(""))
            {
                errors.Add("Введите название турнира");
            }
            if (errors.Count > 0)
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
        private void GoTournament_Click(object sender, RoutedEventArgs e)
        {
            if (IsRegistrationTableValid())
            {
                appState.isRegistrationComplited = true;
                appGrid.Visibility = Visibility.Hidden;
                if (SubgroupsFormationGrid.Children.Count > 5)
                {
                    while (SubgroupsFormationGrid.Children.Count != 5)
                    {
                        SubgroupsFormationGrid.Children.RemoveAt(4);
                    }
                }
                nominationsStackPanel.Children.Clear();
                kategoriesStackPanel.Children.Clear();
                subgroupsStackPanel.Children.Clear();
                SubgroupFormationLabel.Content = "Формирование групп";
                SubgroupsFormationGridParent.Visibility = Visibility.Visible;
                subgroupsFormation = new Subgrouping();

                SetGroups();

                UIElement kategories = subgroupsFormation.CategoryList();
                kategoriesStackPanel.Children.Add(kategories);

                UIElement categorySettingsPanel = subgroupsFormation.CategorySettingsPanel();
                SubgroupsFormationGrid.Children.Add(categorySettingsPanel);
                Grid.SetColumn(categorySettingsPanel, 1);
                Grid.SetRow(categorySettingsPanel, 1);

                UIElement subgroupSettingsPanel = subgroupsFormation.SubgroupSettings();
                subgroupsStackPanel.Children.Add(subgroupSettingsPanel);

                UIElement nominations = subgroupsFormation.NominationsList();
                nominationsStackPanel.Children.Add(nominations);
            }
            else
            {
                appState.isRegistrationComplited = false;
            }
        }

        /// <summary>
        /// Управление боковой панелью
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HideInstrumentsPanel(object sender, RoutedEventArgs e)
        {
            if (isPanelOpen)
            {
                appGrid.ColumnDefinitions[1].Width = new GridLength(40);
                exportButton.Visibility = Visibility.Hidden;
                TournamentNameLabel.Visibility = Visibility.Hidden;
                TournamentNameTextBox.Visibility = Visibility.Hidden;
                addParticipantButton.Visibility = Visibility.Hidden;
                loadFromFile.Visibility = Visibility.Hidden;
                deleteParticipantButton.Visibility = Visibility.Hidden;
                viewSettingsButton.Visibility = Visibility.Hidden;
                goHomeButton.Visibility = Visibility.Hidden;
                goTournament.Visibility = Visibility.Hidden;
                isPanelOpen = false;
            }
            else
            {
                appGrid.ColumnDefinitions[1].Width = new GridLength(160);
                exportButton.Visibility = Visibility.Visible;
                TournamentNameLabel.Visibility = Visibility.Visible;
                TournamentNameTextBox.Visibility = Visibility.Visible;
                addParticipantButton.Visibility = Visibility.Visible;
                loadFromFile.Visibility = Visibility.Visible;
                deleteParticipantButton.Visibility = Visibility.Visible;
                viewSettingsButton.Visibility = Visibility.Visible;
                goHomeButton.Visibility = Visibility.Visible;
                goTournament.Visibility = Visibility.Visible;
                isPanelOpen = true;
            }
        }

        private void BackToRegistratioinTable(object sender, RoutedEventArgs e)
        {
            SubgroupsFormationGridParent.Visibility = Visibility.Hidden;
            while (SubgroupsFormationGrid.Children.Count >= 6)
            {
                SubgroupsFormationGrid.Children.RemoveAt(5);
            }
            appGrid.Visibility = Visibility.Visible;
        }

        private void TournamentNameTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            appState.TournamentName = TournamentNameTextBox.Text;
        }

        /// <summary>
        /// Скрытие панели инструментов для окна формирования категорий
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HideInstrimentsPanelButtonSubgroups(object sender, RoutedEventArgs e)
        {
            if (subgroupsFormation.isPanelOpen)
            {
                SubgroupsFormationGridParent.ColumnDefinitions[1].Width = new GridLength(30, GridUnitType.Pixel);
                instrumentsPanelGrid.Visibility = Visibility.Hidden;
                subgroupsFormation.isPanelOpen = false;
            }
            else
            {
                SubgroupsFormationGridParent.ColumnDefinitions[1].Width = new GridLength(160, GridUnitType.Pixel);
                instrumentsPanelGrid.Visibility = Visibility.Visible;
                subgroupsFormation.isPanelOpen = true;
            }
        }

        private void СreateTournamentGrid(object sender, RoutedEventArgs e)
        {
            TournamentGridWindow tournamentGridWindow = new TournamentGridWindow();
            tournamentGridWindow.Show();
            Close();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if ((sender as TextBox).Text == "" || Convert.ToInt32((sender as TextBox).Text) < 1900 || Convert.ToInt32((sender as TextBox).Text) > DateTime.Now.Year - 13)
            {
                (sender as TextBox).Background = (Brush)new BrushConverter().ConvertFrom("#FFFFDDDB");
            }
            else
            {
                (sender as TextBox).Background = (Brush)new BrushConverter().ConvertFrom("#FFF5F1DA");
            }
        }
    }
}
