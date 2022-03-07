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
using System.Linq;
using TournamentSoftware.DB_Classes;
using static TournamentSoftware.ApplicationResourcesPaths;
using static TournamentSoftware.ApplicationStringValues;
using static TournamentSoftware.TournamentData;

namespace TournamentSoftware
{
    public partial class MainWindow : Window
    {
        private Subgrouping subgroupsFormation;
        private static ParticipantsReagistrator registrator = new ParticipantsReagistrator();
        public ApplicationState appState = new ApplicationState();
        private bool isPanelOpen = true;
        public bool stateExportButton;
        public bool stateDeleteParticipantButton;

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
            LoadGroupRules();
        }

        private void LoadGroupRules()
        {
            List<GroupRule> groupRules = dataBaseHandler.GetData<GroupRule>("SELECT * FROM GroupRule;");
            foreach (GroupRule groupRule in groupRules)
                rules.Add(groupRule);
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
        /// Проверка на валидность строки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StringOnly(object sender, TextCompositionEventArgs e)
        {
            char CheckString = char.ToLower(e.Text[0]);
            if ((CheckString >= 'a' && CheckString <= 'z') || (CheckString >= 'а' && CheckString <= 'я') || CheckString == 'ё')
                e.Handled = false;
            else
                e.Handled = true;
        }

        private void AddParticipant(object sender, RoutedEventArgs e)
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

        private void DeleteParticipant(object sender, RoutedEventArgs e)
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
            SelectorAllForDelete_Unchecked(sender, e);
        }

        /// <summary>
        /// снятие галочек со всех участников
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectorAllForDelete_Unchecked(object sender, RoutedEventArgs e)
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
        private void SelectorAllForDelete_Checked(object sender, RoutedEventArgs e)
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
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
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
        private void ParticipantChecked(object sender, RoutedEventArgs e)
        {
            deleteParticipantButton.IsEnabled = true;
        }

        /// <summary>
        /// событие при анчеке чекбокса участника
        /// если больше нет выбранных участников - блокируем кнопку удалить
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ParticipantUnchecked(object sender, RoutedEventArgs e)
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
        private void SaveFile(object sender, RoutedEventArgs e)
        {
            registrator.SaveFile(ToDataTable(participants));
        }

        private DataTable ToDataTable(ObservableCollection<ParticipantWrapper> participants)
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

        private void ShowRegistrationModuleSettings(object sender, RoutedEventArgs e)
        {
            ViewSettingsWindow settings = new ViewSettingsWindow();
            settings.Owner = this;
            settings.Show();

            stateDeleteParticipantButton = deleteParticipantButton.IsEnabled;
            stateExportButton = exportButton.IsEnabled;

            addParticipantButton.IsEnabled = false;
            loadFromFile.IsEnabled = false;
            deleteParticipantButton.IsEnabled = false;
            exportButton.IsEnabled = false;
            viewSettingsButton.IsEnabled = false;
            goHomeButton.IsEnabled = false;
            goTournament.IsEnabled = false;
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

                var checkboxStyle = new Style(typeof(Border));
                checkboxStyle.Setters.Add(new Setter()
                {
                    Property = Border.CornerRadiusProperty,
                    Value = 3
                });
                CornerRadiusConverter a = new CornerRadiusConverter();
                FrameworkElementFactory checkBox = new FrameworkElementFactory(typeof(CheckBox));
                checkBox.SetBinding(CheckBox.IsCheckedProperty, bind);
                checkBox.SetValue(CheckBox.HorizontalAlignmentProperty, HorizontalAlignment.Center);
                checkBox.SetValue(CheckBox.VerticalAlignmentProperty, VerticalAlignment.Center);
                checkBox.SetValue(Border.CornerRadiusProperty, a.ConvertFromString("3"));
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

        #region Проверка на валидность строки
        private string CheckingForAnEmptyString(string checking, string errortext, int count)
        {
            if (checking == null || checking.Equals(""))
                return $"{errortext} {count}";
            return null;
        }
        private string CheckingDateOfBirth(int checking, int count)
        {
            if (checking < 1900 || checking > DateTime.Now.Year - 13)
                return $"Некорректно заполнен год рождения участника на строке {count}";
            return null;
        }
        private string CheckingSex(string checking, int count)
        {
            if (checking == null || (!checking.Equals("М") && !checking.Equals("Ж")))
                return $"Заполните пол участника на строке {count}";
            return null;
        }

        private string CheckingNomination(ParticipantWrapper checking, int count)
        {
            if (checking.Nominations == null)
                return $"Выберите номинацию участника на строке {count}";
            else
            {
                int countTrue = 0;
                foreach (KeyValuePair<string, bool> keyValuePair in checking.Nominations)
                    if (keyValuePair.Value)
                        countTrue++;
                if (countTrue == 0)
                    return $"Выберите номинацию участника на строке {count}";
            }
            return null;
        }

        private List<string> CheckingForErrorsInRow(ParticipantWrapper participant, int count)
        {
            List<string> result = new List<string>();

            result.Add(CheckingForAnEmptyString(participant.Participant.Name, "Заполните имя участника на строке", count));
            result.Add(CheckingForAnEmptyString(participant.Participant.Surname, "Заполните фамилию участника на строке", count));
            result.Add(CheckingForAnEmptyString(participant.Club, "Заполните клуб участника на строке", count));
            result.Add(CheckingForAnEmptyString(participant.City, "Заполните город участника на строке", count));
            result.Add(CheckingDateOfBirth(participant.Participant.DateOfBirth, count));
            result.Add(CheckingSex(participant.Participant.Sex, count));
            result.Add(CheckingForAnEmptyString(participant.Category, "Заполните категорию участника на строке", count));
            result.Add(CheckingNomination(participant, count));

            return result.Where(x => x != null).ToList();
        }
        #endregion
        private bool IsRegistrationTableValid()
        {
            List<string> errors = new List<string>();
            int count = 1;
            if (nominations.Count == 0)
                errors.Add("Добавьте хотябы 1 номинацию");
            else
                foreach (ParticipantWrapper participant in participants)
                {
                    errors.AddRange(CheckingForErrorsInRow(participant, count));
                    count++;
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

        private void ClearSubgroupingLayout()
        {
            categoriesStackPanel.Children.Clear();
            categorySettingsGrid.Children.Clear();
            subgroupsStackPanel.Children.Clear();
            nominationsStackPanel.Children.Clear();
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

                ClearSubgroupingLayout();

                SubgroupFormationLabel.Content = "Формирование групп";
                SubgroupsFormationGridParent.Visibility = Visibility.Visible;
                subgroupsFormation = new Subgrouping();

                SetGroups();

                UIElement kategories = subgroupsFormation.CategoryList();
                categoriesStackPanel.Children.Add(kategories);

                UIElement categorySettingsPanel = subgroupsFormation.CategorySettingsPanel();
                categorySettingsGrid.Children.Add(categorySettingsPanel);
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
            var VisibilityOnSite = Visibility.Hidden;
            if (isPanelOpen)
                appGrid.ColumnDefinitions[1].Width = new GridLength(40);
            else
            {
                VisibilityOnSite = Visibility.Visible;
                appGrid.ColumnDefinitions[1].Width = new GridLength(160);
            }

            isPanelOpen = !isPanelOpen;

            exportButton.Visibility = VisibilityOnSite;
            TournamentNameLabel.Visibility = VisibilityOnSite;
            TournamentNameTextBox.Visibility = VisibilityOnSite;
            addParticipantButton.Visibility = VisibilityOnSite;
            loadFromFile.Visibility = VisibilityOnSite;
            deleteParticipantButton.Visibility = VisibilityOnSite;
            viewSettingsButton.Visibility = VisibilityOnSite;
            goHomeButton.Visibility = VisibilityOnSite;
            goTournament.Visibility = VisibilityOnSite;
        }

        private void BackToRegistratioinTable(object sender, RoutedEventArgs e)
        {
            SubgroupsFormationGridParent.Visibility = Visibility.Hidden;
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
            if (dataBaseHandler.GetData<TournamentGrid>("SELECT * FROM TournamentGrid WHERE name=\"" + TournamentNameTextBox.Text + "\";").Count != 0)
            {
                MessageBox.Show("Имя турнира " + TournamentNameTextBox.Text + " неуникально.", "Ошибка");
                return;
            }
            TournamentGrid tournamentGrid = new TournamentGrid();
            tournamentGrid.Name = TournamentNameTextBox.Text;
            tournamentGrid.Type = "type";
            tournamentGrid.Date = new DateTime();
            dataBaseHandler.AddItem(tournamentGrid);
            tournamentGrid = dataBaseHandler.GetData<TournamentGrid>("SELECT id FROM TournamentGrid WHERE name=\"" + tournamentGrid.Name + "\";")[0];

            // добавление участников
            for (int i = 0; i < participants.Count; i++)
            {
                Club club = new Club();
                if (dataBaseHandler.GetData<Club>("SELECT * FROM Club WHERE name=\"" + participants[i].Club + "\" AND city=\"" + participants[i].City + "\";").Count == 0)
                {
                    club.Name = participants[i].Club;
                    club.City = participants[i].City;
                    dataBaseHandler.AddItem(club);
                }
                club.Id = dataBaseHandler.GetData<Club>("SELECT * FROM Club WHERE name=\"" + participants[i].Club + "\" AND city=\"" + participants[i].City + "\";")[0].Id;

                if (dataBaseHandler.GetData<Participant>("SELECT * FROM Participant WHERE surname=\"" + participants[i].Participant.Surname + "\" AND name=\"" + participants[i].Participant.Name + "\" AND date_of_birth=\"" + participants[i].Participant.DateOfBirth + "\";").Count != 0)
                {
                    continue;
                }
                Participant participant = new Participant();
                participant.Surname = participants[i].Participant.Surname;
                participant.Name = participants[i].Participant.Name;
                participant.Patronymic = participants[i].Participant.Patronymic;
                participant.Pseudonym = participants[i].Participant.Pseudonym;
                participant.Leader = participants[i].Participant.Leader;
                participant.Sex = participants[i].Participant.Sex;
                participant.DateOfBirth = participants[i].Participant.DateOfBirth;
                participant.Height = participants[i].Participant.Height;
                participant.Weight = participants[i].Participant.Weight;
                participant.CommonRating = participants[i].Participant.CommonRating;
                participant.ClubRating = participants[i].Participant.ClubRating;
                participant.ClubId = club.Id;
                dataBaseHandler.AddItem(participant);
            }

            // номинации
            foreach (GroupWrapper groupWrapper in groups)
            {
                Nomination nomination = new Nomination();
                if (dataBaseHandler.GetData<Nomination>("SELECT * FROM Nomination WHERE name=\"" + groupWrapper.NominationWrapper.Nomination.Name + "\";").Count == 0)
                {
                    nomination.Name = groupWrapper.NominationWrapper.Nomination.Name;
                    dataBaseHandler.AddItem(nomination);
                }
                nomination = dataBaseHandler.GetData<Nomination>("SELECT id FROM Nomination WHERE name=\"" + groupWrapper.NominationWrapper.Nomination.Name + "\";")[0];

                // категории
                List<CategoryWrapper> categories = groupWrapper.Categories;
                foreach (CategoryWrapper categoryWrapper in categories)
                {
                    Category category = new Category();
                    if (dataBaseHandler.GetData<Category>("SELECT * FROM Category WHERE name=\"" + categoryWrapper.Category.Name + "\";").Count == 0)
                    {
                        category.Name = categoryWrapper.Category.Name;
                        dataBaseHandler.AddItem(category);
                    }
                    category = dataBaseHandler.GetData<Category>("SELECT * FROM Category WHERE name=\"" + categoryWrapper.Category.Name + "\";")[0];

                    // добавление групп 
                    TournamentGroup tournamentGroup = new TournamentGroup();
                    tournamentGroup.TournamentGridId = tournamentGrid.Id;
                    tournamentGroup.NominationId = nomination.Id;
                    tournamentGroup.CategoryId = category.Id;
                    dataBaseHandler.AddItem(tournamentGroup);
                    tournamentGroup = dataBaseHandler.GetData<TournamentGroup>("SELECT * FROM TournamentGroup WHERE tournament_grid_id=" + tournamentGroup.TournamentGridId + " AND nomination_id=" + tournamentGroup.NominationId + " AND category_id=" + tournamentGroup.CategoryId + ";")[0];

                    //привязка к группам правил
                    foreach (GroupRule groupRule in categoryWrapper.SelectedRules)
                    {
                        GroupRule_Group groupRule_Group = new GroupRule_Group();
                        groupRule_Group.TournamentGroupId = tournamentGroup.Id;
                        groupRule_Group.GroupRoleId = groupRule.Id;
                        dataBaseHandler.AddItem(groupRule_Group);
                    }

                    // добавление подгрупп 
                    List<SubgroupWrapper> subgroups = categoryWrapper.Subgroups;
                    foreach (SubgroupWrapper subgroupWrapper in subgroups)
                    {
                        Subgroup subgroup = new Subgroup();
                        subgroup.Name = subgroupWrapper.Subgroup.Name;
                        subgroup.GroupId = tournamentGroup.Id;
                        dataBaseHandler.AddItem(subgroup);
                        subgroup = dataBaseHandler.GetData<Subgroup>("SELECT * FROM Subgroup WHERE name=\"" + subgroup.Name + "\" AND group_id=" + subgroup.GroupId + ";")[0];

                        foreach (ParticipantWrapper participantWrapper in subgroupWrapper.Participants)
                        {
                            Subgroup_Participant subgroup_participant = new Subgroup_Participant();
                            subgroup_participant.SubgroupId = subgroup.Id;
                            subgroup_participant.ParticipantId = dataBaseHandler.GetData<Participant>("SELECT * FROM Participant WHERE surname=\"" + participantWrapper.Participant.Surname + "\" AND name=\"" + participantWrapper.Participant.Name + "\" AND date_of_birth=\"" + participantWrapper.Participant.DateOfBirth + "\";")[0].Id;
                            dataBaseHandler.AddItem(subgroup_participant);
                        }
                    }
                }
            }

            Tournament = tournamentGrid;

            TournamentGridWindow tournamentGridWindow = new TournamentGridWindow();
            tournamentGridWindow.Show();
            Close();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if ((sender as TextBox).Text != "" && !int.TryParse((sender as TextBox).Text, out _))
            {
                (sender as TextBox).Text = "";
                MessageBox.Show("Строка должна содержать только цифры!", "Ошибка");
                return;
            }

            if ((sender as TextBox).Text == "" || Convert.ToInt32((sender as TextBox).Text) < 1900 || Convert.ToInt32((sender as TextBox).Text) > DateTime.Now.Year - 13)
            {
                (sender as TextBox).Background = (Brush)new BrushConverter().ConvertFrom("#FFFFDDDB");
            }
            else
            {
                (sender as TextBox).Background = (Brush)new BrushConverter().ConvertFrom("#FFF5F1DA");
            }
        }

        private void SaveSubgroups(object sender, RoutedEventArgs e)
        {
            subgroupsFormation.SaveSubgroup();
        }
    }
}