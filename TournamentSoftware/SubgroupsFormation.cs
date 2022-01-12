using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static TournamentSoftware.TournamentData;

namespace TournamentSoftware
{
    public class SubgroupsFormation
    {
        public Dictionary<string, Dictionary<string, List<ParticipantFormModel>>> kategoryGroups = new Dictionary<string, Dictionary<string, List<ParticipantFormModel>>>();
        private Grid categoriesGrid = new Grid();
        private Grid categorySettingsGrid = new Grid();
        private Grid subgroupsSettingsGrid = new Grid { AllowDrop = true };
        private Label countInCategory = new Label { FontSize = 30, VerticalAlignment = VerticalAlignment.Center };
        private TextBox countSubgroups = new TextBox
        {
            Width = 50,
            Height = 50,
            FontSize = 30,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalContentAlignment = VerticalAlignment.Center
        };
        private string selectedNomination = "";
        public bool isPanelOpen = true;
        private List<string> rools = new List<string> { "Правило посевных бойцов", "Правило одноклубников", "Правило города" };
        private List<Button> categoriesButtons = new List<Button>();
        private int lastClickedCategory = -1;
        private static List<string> selectedRools = new List<string> { "Правило посевных бойцов", "Правило одноклубников", "Правило города" };
        private string selectedCategory = "";
        private Button goNextButton = new Button();
        SolidColorBrush yellow = new SolidColorBrush(Color.FromRgb(255, 215, 0));
        SolidColorBrush white = new SolidColorBrush(Color.FromRgb(255, 255, 255));
        private static Dictionary<string, string> errorsInSubgroups = new Dictionary<string, string>(); // нормер подгруппы - ошибка

        // получаем список катерогий
        public Dictionary<string, Dictionary<string, List<ParticipantFormModel>>> getCategories(ObservableCollection<ParticipantFormModel> participants)
        {
            foreach (ParticipantFormModel participant in participants)
            {
                foreach (KeyValuePair<string, bool> nomination in participant.Nominations)
                {
                    if (nomination.Value)
                    {
                        string nominationName = nomination.Key;
                        if (!kategoryGroups.ContainsKey(nominationName))
                        {
                            Dictionary<string, List<ParticipantFormModel>> dictionary = new Dictionary<string, List<ParticipantFormModel>>();
                            kategoryGroups.Add(nominationName, dictionary);
                        }

                        string kategory = participant.Kategory;
                        if (kategoryGroups[nominationName].ContainsKey(kategory))
                        {
                            kategoryGroups[nominationName][kategory].Add(participant);
                        }
                        else
                        {
                            List<ParticipantFormModel> list = new List<ParticipantFormModel>();
                            list.Add(participant);
                            kategoryGroups[nominationName].Add(kategory, list);
                        }
                    }
                }
            }
            return kategoryGroups;
        }

        private Label CreateLabel(string content, int fontSize = 24)
        {
            Label label = new Label();
            label.Content = content;
            label.FontSize = fontSize;
            label.Margin = new Thickness(5);
            return label;
        }

        public UIElement NominationsList()
        {
            Grid grid = new Grid();
            grid.Margin = new Thickness(5);
            grid.VerticalAlignment = VerticalAlignment.Stretch;
            grid.HorizontalAlignment = HorizontalAlignment.Stretch;

            int rowsCount = 0;

            foreach (NominationFormModel nomination in nominations)
            {
                string nominationName = nomination.Nomination.Name;
                RowDefinition row = new RowDefinition();
                Button nominationButton = new Button();
                nominationButton.Margin = new Thickness(5);
                nominationButton.Height = 30;
                nominationButton.FontSize = 15;
                nominationButton.Content = nominationName;
                nominationButton.Tag = nominationName;
                nominationButton.Click += NominationButton_Click;
                categoriesButtons.Add(nominationButton);
                grid.RowDefinitions.Add(row);
                grid.Children.Add(nominationButton);
                Grid.SetRow(nominationButton, rowsCount);
                rowsCount++;
            }
            return grid;
        }

        private void NominationButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            string nomination = button.Tag.ToString();
            selectedNomination = nomination;
            ((MainWindow)Application.Current.MainWindow).SubgroupFormationLabel.Content = "Формирование групп. Номинация " + nomination;
            ShowKategoriesForNomination(kategoryGroups[nomination]);
        }

        private void ShowKategoriesForNomination(Dictionary<string, List<ParticipantFormModel>> kategories)
        {
            categoriesGrid.Children.Clear();
            categoriesGrid.RowDefinitions.Clear();

            int rowsCount = 0;

            foreach (string kategory in kategoryGroups[selectedNomination].Keys)
            {
                RowDefinition row = new RowDefinition();
                Button kategoryButton = new Button
                {
                    Margin = new Thickness(5),
                    Height = 30,
                    FontSize = 15,
                    Content = kategory,
                    Tag = kategory
                };

                if (kategoryGroups[selectedNomination][kategory].Count == 1)
                {
                    kategoryButton.ToolTip = "В этой категории всего 1 участник";
                    kategoryButton.Click += CategoryButton_Click;
                }
                else
                {
                    kategoryButton.Click += CategoryButton_Click;
                }
                categoriesButtons.Add(kategoryButton);
                categoriesGrid.RowDefinitions.Add(row);
                categoriesGrid.Children.Add(kategoryButton);
                Grid.SetRow(kategoryButton, rowsCount);
                rowsCount++;
            }

        }

        public UIElement KategoryList()
        {
            categoriesGrid = new Grid();
            categoriesGrid.Margin = new Thickness(5);
            categoriesGrid.VerticalAlignment = VerticalAlignment.Stretch;
            categoriesGrid.HorizontalAlignment = HorizontalAlignment.Stretch;
            RowDefinition row1 = new RowDefinition();
            RowDefinition row2 = new RowDefinition();

            row1.Height = new GridLength(80, GridUnitType.Pixel);
            row2.Height = new GridLength(150, GridUnitType.Pixel);

            Label label = CreateLabel("Выберите номинацию", 15);
            label.VerticalAlignment = VerticalAlignment.Center;
            label.HorizontalAlignment = HorizontalAlignment.Center;

            categoriesGrid.RowDefinitions.Add(row1);
            categoriesGrid.RowDefinitions.Add(row2);

            categoriesGrid.Children.Add(label);
            Grid.SetRow(label, 1);

            return categoriesGrid;
        }

        private void CategoryButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            button.Background = yellow;
            string kategory = button.Tag.ToString();
            selectedCategory = kategory;
            countInCategory.Content = kategoryGroups[selectedNomination][kategory].Count;
            ShowCategorySettings();
            if (lastClickedCategory != -1)
            {
                categoriesButtons[lastClickedCategory].Background = white;
            }
            countSubgroups.Text = "";
            lastClickedCategory = categoriesButtons.IndexOf(button);
        }

        public void ShowCategorySettings()
        {
            subgroupsSettingsGrid.Children.Clear();
            categorySettingsGrid.Children.Clear();
            categorySettingsGrid.ShowGridLines = true;
            var parent = VisualTreeHelper.GetParent(countInCategory);
            var parentGrid = parent as Grid;
            if (parentGrid != null)
            {
                parentGrid.Children.Clear();
            }

            var goNextBtn_Parent = VisualTreeHelper.GetParent(goNextButton);
            var goNextBtn_ParentGrid = goNextBtn_Parent as Grid;
            if (goNextBtn_ParentGrid != null)
            {
                goNextBtn_ParentGrid.Children.Clear();
            }

            Grid countInLategoryGrid = new Grid();
            ColumnDefinition column1 = new ColumnDefinition();
            ColumnDefinition column2 = new ColumnDefinition();
            countInLategoryGrid.ColumnDefinitions.Add(column1);
            countInLategoryGrid.ColumnDefinitions.Add(column2);
            countInLategoryGrid.Children.Add(countInCategory);
            countInCategory.HorizontalAlignment = HorizontalAlignment.Right;
            Label label1 = CreateLabel("Количество\nбойцов в\nгруппе", 15);
            label1.HorizontalContentAlignment = HorizontalAlignment.Left;
            label1.HorizontalAlignment = HorizontalAlignment.Left;
            label1.VerticalAlignment = VerticalAlignment.Center;
            countInLategoryGrid.Children.Add(label1);
            Grid.SetColumn(countInCategory, 0);
            Grid.SetColumn(label1, 1);

            var countOfSubgroups = CountOfSubgroupsGrid();
            var rools = ChooseRools();

            goNextButton = new Button();
            goNextButton.IsEnabled = false;
            goNextButton.Margin = new Thickness(5);
            goNextButton.Content = "Продолжить";
            goNextButton.HorizontalAlignment = HorizontalAlignment.Center;
            goNextButton.VerticalAlignment = VerticalAlignment.Top;
            goNextButton.Height = 40;
            goNextButton.Click += GoNext_Click;

            categorySettingsGrid.Children.Add(goNextButton);
            categorySettingsGrid.Children.Add(rools);
            categorySettingsGrid.Children.Add(countOfSubgroups);
            categorySettingsGrid.Children.Add(countInLategoryGrid);
            Grid.SetRow(countInLategoryGrid, 0);
            Grid.SetRow(rools, 1);
            Grid.SetRow(countOfSubgroups, 2);
            Grid.SetRow(goNextButton, 3);
        }

        /// <summary>
        /// Тасование массива
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rng"></param>
        /// <param name="array"></param>
        public List<T> Shuffle<T>(Random rng, List<T> array)
        {
            int n = array.Count;
            while (n > 1)
            {
                int k = rng.Next(n--);
                T temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }

            return array;
        }

        private static void checkRool(Dictionary<string, List<ParticipantFormModel>> subgroups, string roolName, ParticipantFormModel controlPartisipant)
        {
            switch (roolName)
            {
                case "Правило города":
                    Console.WriteLine("Проверка правила города");
                    foreach (KeyValuePair<string, List<ParticipantFormModel>> keyValuePair in subgroups)
                    {
                        if (keyValuePair.Value.FindAll(p => p.City.Equals(controlPartisipant.City)).Count > 0)
                        {
                            errorsInSubgroups.Add(keyValuePair.Key, "Нарушено правило города");
                        }
                    }
                    break;
                case "Правило посевных бойцов":
                    foreach (KeyValuePair<string, List<ParticipantFormModel>> keyValuePair in subgroups)
                    {
                        if (keyValuePair.Value.FindAll(p => p.Participant.Leader.Equals(controlPartisipant.Participant.Leader)).Count > 0)
                        {
                            errorsInSubgroups.Add(keyValuePair.Key, "Нарушено правило посевных бойцов");
                        }
                    }
                    break;
                case "Правило одноклубников":
                    foreach (KeyValuePair<string, List<ParticipantFormModel>> keyValuePair in subgroups)
                    {
                        if (keyValuePair.Value.FindAll(p => p.Club.Equals(controlPartisipant.Club)).Count > 0)
                        {
                            errorsInSubgroups.Add(keyValuePair.Key, "Нарушено правило одноклубников");
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private static void ParticipantsSort(ref Dictionary<string, List<ParticipantFormModel>> subgroups,
            ref List<ParticipantFormModel> participants, ref int lastAddedGroup, int _subgroups, string roolName,
            Dictionary<string, List<ParticipantFormModel>> filtered = null)
        {
            int lastAddedGroup1 = lastAddedGroup;
            Dictionary<string, List<ParticipantFormModel>> subgroupsCopy = subgroups;
            if (filtered == null)
            {
                participants.ForEach(participant =>
                {
                    if (lastAddedGroup1 > _subgroups)
                    {
                        subgroupsCopy["1"].Add(participant);
                        lastAddedGroup1 = 2;
                    }
                    else
                    {
                        subgroupsCopy[lastAddedGroup1.ToString()].Add(participant);
                        lastAddedGroup1++;
                    }
                    checkRool(subgroupsCopy, roolName, participant);
                });
            }
            else
            {
                foreach (KeyValuePair<string, List<ParticipantFormModel>> participantsList in filtered)
                {
                    participantsList.Value.ForEach(participant =>
                    {
                        if (lastAddedGroup1 > _subgroups)
                        {
                            subgroupsCopy["1"].Add(participant);
                            lastAddedGroup1 = 2;
                        }
                        else
                        {
                            subgroupsCopy[lastAddedGroup1.ToString()].Add(participant);
                            lastAddedGroup1++;
                        }
                        checkRool(subgroupsCopy, roolName, participant);
                    });
                    
                }
            }
            subgroups = subgroupsCopy;
            lastAddedGroup = lastAddedGroup1;
        }

        private static void ParticipantsSortWithRools(ref Dictionary<string, List<ParticipantFormModel>> subgroups, ref List<ParticipantFormModel> participants, ref int lastAddedGroup, int _subgroups)
        {
            if (selectedRools.Contains("Правило города"))
            {
                Dictionary<string, List<ParticipantFormModel>> city = FilterParticipantsForCities(participants);
                foreach (KeyValuePair<string, List<ParticipantFormModel>> entry in city)
                {
                    if (selectedRools.Contains("Правило одноклубников"))
                    {
                        Dictionary<string, List<ParticipantFormModel>> club = FilterParticipantsForClubs(entry.Value);
                        ParticipantsSort(ref subgroups, ref participants, ref lastAddedGroup, _subgroups, "Правило одноклубников", club);
                    }
                    else
                    {
                        ParticipantsSort(ref subgroups, ref participants, ref lastAddedGroup, _subgroups, "Правило города");
                    }
                }
            }
            else
            {
                if (selectedRools.Contains("Правило одноклубников"))
                {
                    Dictionary<string, List<ParticipantFormModel>> club = FilterParticipantsForClubs(participants);
                    ParticipantsSort(ref subgroups, ref participants, ref lastAddedGroup, _subgroups, "Правило одноклубников", club);
                }
                else
                {
                    ParticipantsSort(ref subgroups, ref participants, ref lastAddedGroup, _subgroups, "");
                }
            }
        }

        /// <summary>
        /// Формирование подгрупп
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GoNext_Click(object sender, RoutedEventArgs e)
        {
            int _subgroups = int.Parse(countSubgroups.Text);
            int _countInKategory = int.Parse(countInCategory.Content.ToString());
            if (_countInKategory > 0 && _subgroups > 0 && _countInKategory / _subgroups >= 2)
            {
                Dictionary<string, List<ParticipantFormModel>> subgroups = new Dictionary<string, List<ParticipantFormModel>>();
                for (int i = 1; i <= _subgroups; i++)
                {
                    subgroups.Add(i.ToString(), new List<ParticipantFormModel>());
                }
                List<ParticipantFormModel> participantsInKategory = kategoryGroups[selectedNomination][selectedCategory];

                int lastAddedGroup = 1; // группа в которую последний раз добавляли участника

                var rand = new Random();

                participantsInKategory = Shuffle(rand, participantsInKategory);

                List<ParticipantFormModel> posevParticipants = participantsInKategory.FindAll(p => p.Participant.Leader == true);
                List<ParticipantFormModel> not_posevParticipants = participantsInKategory.FindAll(p => p.Participant.Leader == false);

                // учет правила посевных бойцов
                if (selectedRools.Contains("Правило посевных бойцов"))
                {
                    ParticipantsSortWithRools(ref subgroups, ref posevParticipants, ref lastAddedGroup, _subgroups);

                    ParticipantsSortWithRools(ref subgroups, ref not_posevParticipants, ref lastAddedGroup, _subgroups);
                }

                SetSubgroups(subgroups);
            }
            else
            {
                MessageBox.Show("Введено некорректное количество подгрупп!", "Ошибка", MessageBoxButton.OK);
            }
        }

        private static Dictionary<string, List<ParticipantFormModel>> FilterParticipantsForCities(List<ParticipantFormModel> participants)
        {
            Dictionary<string, List<ParticipantFormModel>> participantsForCities = new Dictionary<string, List<ParticipantFormModel>>();
            participants.ForEach(participant =>
            {
                string city = participant.City;
                if (participantsForCities.ContainsKey(city))
                {
                    participantsForCities[city].Add(participant);
                }
                else
                {
                    List<ParticipantFormModel> list = new List<ParticipantFormModel>();
                    list.Add(participant);
                    participantsForCities.Add(city, list);
                }
            });
            return participantsForCities;
        }

        private static Dictionary<string, List<ParticipantFormModel>> FilterParticipantsForClubs(List<ParticipantFormModel> participants)
        {
            Dictionary<string, List<ParticipantFormModel>> participantsForClubs = new Dictionary<string, List<ParticipantFormModel>>();
            participants.ForEach(participant =>
            {
                string club = participant.Club;
                if (participantsForClubs.ContainsKey(club))
                {
                    participantsForClubs[club].Add(participant);
                }
                else
                {
                    List<ParticipantFormModel> list = new List<ParticipantFormModel>();
                    list.Add(participant);
                    participantsForClubs.Add(club, list);
                }
            });
            return participantsForClubs;
        }

        private void SetSubgroups(Dictionary<string, List<ParticipantFormModel>> subgroups)
        {
            subgroupsSettingsGrid.Children.Clear();
            subgroupsSettingsGrid.ShowGridLines = true;
            subgroupsSettingsGrid.RowDefinitions.Clear();
            subgroupsSettingsGrid.Drop += SubgroupsSettingsGrid_Drop;
            subgroupsSettingsGrid.DragOver += SubgroupsSettingsGrid_DragOver;

            for (int i = 1; i <= subgroups.Count; i++)
            {
                RowDefinition row = new RowDefinition();

                Grid grid = new Grid();
                grid.ShowGridLines = true;
                grid.DragOver += Grid_DragOver;
                grid.Margin = new Thickness(5);
                RowDefinition r = new RowDefinition();
                Label label = CreateLabel("Подгруппа" + i, 25);
                label.HorizontalAlignment = HorizontalAlignment.Left;
                grid.RowDefinitions.Add(r);
                grid.Children.Add(label);
                Grid.SetRow(label, grid.RowDefinitions.Count - 1);

                SolidColorBrush solidBG = new SolidColorBrush(Color.FromRgb(255, 215, 0));

                subgroups[i.ToString()].ForEach(participant =>
                {
                    Label l = CreateLabel(participant.Participant.Name + " " + participant.Participant.Surname + " " + participant.Participant.Patronymic, 20);
                    l.ToolTip = "Посев: " + participant.Participant.Leader + "\nКлуб: " + participant.Club + "\nГород: " + participant.City;
                    l.Background = solidBG;
                    l.Margin = new Thickness(10, 5, 5, 5);
                    l.HorizontalAlignment = HorizontalAlignment.Right;
                    RowDefinition rr = new RowDefinition();
                    grid.RowDefinitions.Add(rr);
                    grid.Children.Add(l);
                    Grid.SetRow(l, grid.RowDefinitions.Count - 1);
                });

                subgroupsSettingsGrid.RowDefinitions.Add(row);
                subgroupsSettingsGrid.Children.Add(grid);
                Grid.SetRow(grid, subgroupsSettingsGrid.RowDefinitions.Count - 1);
            }

        }

        private void Grid_DragOver(object sender, DragEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void SubgroupsSettingsGrid_DragOver(object sender, DragEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void SubgroupsSettingsGrid_Drop(object sender, DragEventArgs e)
        {
            throw new NotImplementedException();
        }

        private UIElement ChooseRools()
        {
            Grid grid = new Grid();
            RowDefinition row1 = new RowDefinition();
            row1.Height = new GridLength(40, GridUnitType.Pixel);
            grid.RowDefinitions.Add(row1);

            Label label = CreateLabel("Выбор правил", 15);
            label.HorizontalAlignment = HorizontalAlignment.Center;

            grid.Children.Add(label);
            Grid.SetRow(label, 0);

            rools.ForEach(rool =>
            {
                RowDefinition row = new RowDefinition { Height = new GridLength(30, GridUnitType.Pixel) };
                CheckBox checkBox = new CheckBox { Content = rool, Tag = rool };
                checkBox.Margin = new Thickness(5, 0, 0, 0);
                checkBox.IsChecked = true;
                checkBox.Checked += CheckBox_Checked;
                checkBox.Unchecked += CheckBox_Unchecked;

                grid.RowDefinitions.Add(row);
                grid.Children.Add(checkBox);
                Grid.SetRow(checkBox, grid.RowDefinitions.Count - 1);
            });

            return grid;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            string rool = checkBox.Tag.ToString();
            if (selectedRools.Contains(rool)) selectedRools.Remove(rool);
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            string rool = checkBox.Tag.ToString();
            if (!selectedRools.Contains(rool)) selectedRools.Add(rool);
        }

        private UIElement CountOfSubgroupsGrid()
        {
            var parent = VisualTreeHelper.GetParent(countSubgroups);
            var parentGrid = parent as Grid;
            if (parentGrid != null)
            {
                parentGrid.Children.Clear();
            }

            Grid countOfSubgroupsGrid = new Grid();
            ColumnDefinition column1 = new ColumnDefinition();
            column1.Width = new GridLength(1, GridUnitType.Star);
            ColumnDefinition column2 = new ColumnDefinition();
            column2.Width = new GridLength(1.5, GridUnitType.Star);
            countOfSubgroupsGrid.ColumnDefinitions.Add(column1);
            countOfSubgroupsGrid.ColumnDefinitions.Add(column2);

            countSubgroups.TextChanged += CountSubgroups_TextChanged;
            countSubgroups.PreviewTextInput += CountSubgroups_PreviewTextInput;

            Label label = CreateLabel("Количество подгрупп", 15);
            label.HorizontalAlignment = HorizontalAlignment.Left;
            label.VerticalAlignment = VerticalAlignment.Center;

            countOfSubgroupsGrid.Children.Add(countSubgroups);
            countOfSubgroupsGrid.Children.Add(label);
            Grid.SetColumn(countSubgroups, 0);
            Grid.SetColumn(label, 1);
            return countOfSubgroupsGrid;
        }

        private void CountSubgroups_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!int.TryParse(countSubgroups.Text, out _))
            {
                e.Handled = true;
                goNextButton.IsEnabled = false;
            }
            else
            {
                goNextButton.IsEnabled = true;
            }
        }

        public UIElement CategorySettingsPanel()
        {
            selectedCategory = "";
            selectedNomination = "";
            var parent = VisualTreeHelper.GetParent(countInCategory);
            var parentGrid = parent as Grid;
            if (parentGrid != null)
            {
                parentGrid.Children.Clear();
            }

            parent = VisualTreeHelper.GetParent(countSubgroups);
            parentGrid = parent as Grid;
            if (parentGrid != null)
            {
                parentGrid.Children.Clear();
            }

            subgroupsSettingsGrid.Children.Clear();
            categorySettingsGrid.Children.Clear();
            categorySettingsGrid.RowDefinitions.Clear();
            categorySettingsGrid = new Grid();
            RowDefinition row1 = new RowDefinition();
            RowDefinition row2 = new RowDefinition();
            RowDefinition row3 = new RowDefinition();
            RowDefinition row4 = new RowDefinition();

            row1.Height = new GridLength(80, GridUnitType.Pixel);
            row2.Height = new GridLength(150, GridUnitType.Pixel);
            row3.Height = new GridLength(80, GridUnitType.Pixel);
            row4.Height = new GridLength(1, GridUnitType.Star);

            Label startMessage = new Label
            {
                Content = "Выберите подгруппу",
                FontSize = 15,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            categorySettingsGrid.RowDefinitions.Add(row1);
            categorySettingsGrid.RowDefinitions.Add(row2);
            categorySettingsGrid.RowDefinitions.Add(row3);
            categorySettingsGrid.RowDefinitions.Add(row4);

            categorySettingsGrid.Children.Add(startMessage);
            Grid.SetRow(startMessage, 1);
            return categorySettingsGrid;
        }

        public UIElement SubgroupSettings()
        {
            RowDefinition row1 = new RowDefinition();
            RowDefinition row2 = new RowDefinition();
            row1.Height = new GridLength(80);
            row2.Height = new GridLength(100);
            subgroupsSettingsGrid.RowDefinitions.Add(row1);
            subgroupsSettingsGrid.RowDefinitions.Add(row2);
            Label startMessage = new Label
            {
                Content = "Введите кол-во подгрупп",
                FontSize = 15,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center,
            };
            subgroupsSettingsGrid.Children.Add(startMessage);
            Grid.SetRow(startMessage, 1);
            return subgroupsSettingsGrid;
        }

        /// <summary>
        /// Ввод только чисел
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CountSubgroups_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (!int.TryParse(e.Text, out _))
            {
                e.Handled = true;
                goNextButton.IsEnabled = false;
            }
            else
            {
                goNextButton.IsEnabled = true;
            }
        }

        public void SelectNomination(object sender, RoutedEventArgs e)
        {
            selectedNomination = ((Button)e.Source).Content.ToString();
            ((MainWindow)Application.Current.MainWindow).SubgroupFormationLabel.Content = "Формирование групп. " + selectedNomination;
        }
    }
}
