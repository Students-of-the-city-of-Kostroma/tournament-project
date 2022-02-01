using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using static TournamentSoftware.TournamentData;

namespace TournamentSoftware
{
    public class Subgrouping
    {
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
        private static string selectedNomination = "";
        private static string selectedCategory = "";
        public bool isPanelOpen = true;
        private List<string> rools = new List<string> { "Правило посевных бойцов", "Правило одноклубников", "Правило города" };
        private static List<string> selectedRools = new List<string> { "Правило посевных бойцов", "Правило одноклубников", "Правило города" };
        private List<Button> categoriesButtons = new List<Button>();
        private Button goNextButton = new Button();

        private Label CreateLabel(string content, int fontSize = 24)
        {
            Label label = new Label
            {
                Content = content,
                FontSize = fontSize,
                Margin = new Thickness(5)
            };
            return label;
        }

        public UIElement NominationsList()
        {
            Grid grid = new Grid();
            grid.Margin = new Thickness(5);
            grid.VerticalAlignment = VerticalAlignment.Stretch;
            grid.HorizontalAlignment = HorizontalAlignment.Stretch;

            int rowsCount = 0;

            foreach (NominationWrapper nomination in nominations)
            {
                string nominationName = nomination.Nomination.Name;
                RowDefinition row = new RowDefinition();
                Button nominationButton = new Button
                {
                    Margin = new Thickness(5),
                    Height = 30,
                    FontSize = 15,
                    Content = nominationName,
                    Tag = nominationName
                };
                nominationButton.Click += SelectNomination;
                grid.RowDefinitions.Add(row);
                grid.Children.Add(nominationButton);
                Grid.SetRow(nominationButton, rowsCount);
                rowsCount++;
            }
            return grid;
        }

        private void SelectNomination(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            string nomination = button.Tag.ToString();
            selectedNomination = nomination;
            ((MainWindow)Application.Current.MainWindow).SubgroupFormationLabel.Content = "Формирование групп. Номинация " + nomination;
            ShowCategoriesForNomination(selectedNomination);
        }

        private void PrepareCategoriesGrid() 
        {
            categoriesGrid.Children.Clear();
            categoriesGrid.RowDefinitions.Clear();
            categoriesButtons.Clear();
            selectedCategory = "";
        }

        private void ShowCategoriesForNomination(string nominationName)
        {
            PrepareCategoriesGrid();

            int rowsCount = 0;

            foreach (CategoryWrapper category in GetCategoriesFromNomination(nominationName))
            {
                RowDefinition row = new RowDefinition();
                Button categoryButton = new Button
                {
                    Margin = new Thickness(5),
                    Height = 30,
                    FontSize = 15,
                    Content = category.Name,
                    Tag = category.Name
                };

                if (category.ParticipantsCount() < 3)
                {
                    categoryButton.ToolTip = "В этой категории слишком мало участников";
                }
                categoryButton.Click += SelectCategory;
                categoriesButtons.Add(categoryButton);
                categoriesGrid.RowDefinitions.Add(row);
                categoriesGrid.Children.Add(categoryButton);
                Grid.SetRow(categoryButton, rowsCount);
                rowsCount++;
            }
            ColorCategoryButtons();
        }

        public UIElement CategoryList()
        {
            categoriesGrid = new Grid
            {
                Margin = new Thickness(5),
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
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

        private void ColorCategoryButtons()
        {
            categoriesButtons.ForEach(button =>
            {
                button.Background = !selectedCategory.Equals("") && button.Tag.ToString().Equals(selectedCategory) ? darkGreen :
                GetCategoryFromNomination(selectedNomination, button.Tag.ToString()).ParticipantsCount() < 3 ? white :
                beige;
            });
        }

        private void SelectCategory(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            string kategory = button.Tag.ToString();
            selectedCategory = kategory;
            countInCategory.Content = GetCategoryFromNomination(selectedNomination, kategory).ParticipantsCount();
            ShowCategorySettings();
            ColorCategoryButtons();
            countSubgroups.Text = "";
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
            goNextButton.Click += SubgroupsFormation;

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
        /// <param name="random"></param>
        /// <param name="array"></param>
        /// <returns></returns>
        public List<T> Shuffle<T>(Random random, List<T> array)
        {
            int count = array.Count;
            while (count > 1)
            {
                int randomNumber = random.Next(count--);
                T temp = array[count];
                array[count] = array[randomNumber];
                array[randomNumber] = temp;
            }
            return array;
        }

        private static void CheckRoolAndLogErrors(string subgroupName, string roolName, ParticipantWrapper controlPartisipant)
        {
            Console.WriteLine("Проверка правила " + roolName + " для подгруппы " + subgroupName);
            List<ParticipantWrapper> participants = GetCategoryFromNomination(selectedNomination, selectedCategory).GetParticipantsBySubgroup(subgroupName);
            SubgroupWrapper subgroup = GetGroupByNomination(selectedNomination).GetSubgroupByCategory(selectedCategory, subgroupName);
            switch (roolName)
            {
                case "Правило города":
                    if (participants.Exists(participant => participant.City.Equals(controlPartisipant.City) && !participant.Equals(controlPartisipant)))
                    {
                        subgroup.AddError("Нарушено правило города");
                    }
                    break;
                case "Правило посевных бойцов":
                    if (participants.Exists(participant => (participant.Participant.Leader.Equals(true) && !participant.Equals(controlPartisipant))) && controlPartisipant.Participant.Leader)
                    {
                        subgroup.AddError("Нарушено правило посевных бойцов");
                    }
                    break;
                case "Правило одноклубников":
                    if (participants.Exists(participant => participant.Club.Equals(controlPartisipant.Club) && !participant.Equals(controlPartisipant)))
                    {
                        subgroup.AddError("Нарушено правило одноклубников");
                    }
                    break;
                default:
                    break;
            }
        }

        private static void ParticipantsSort(List<ParticipantWrapper> participants, ref int lastAddedGroup, Dictionary<string, List<ParticipantWrapper>> filteredParticipants = null)
        {
            CategoryWrapper category = GetCategoryFromNomination(selectedNomination, selectedCategory);
            int subgroupsCount = category.Subgroups.Count;
            int lastAddedGroup1 = lastAddedGroup;

            if (filteredParticipants == null)
            {
                participants.ForEach(participant =>
                {
                    string subgroupName = "";
                    if (lastAddedGroup1 > subgroupsCount)
                    {
                        subgroupName = "Подгруппа 1";
                        category.GetSubgroupByName(subgroupName).AddParticipant(participant);
                        lastAddedGroup1 = 2;
                    }
                    else
                    {
                        subgroupName = "Подгруппа " + lastAddedGroup1;
                        category.GetSubgroupByName(subgroupName).AddParticipant(participant);
                        lastAddedGroup1++;
                    }
                    foreach (string roolName in selectedRools)
                    {
                        CheckRoolAndLogErrors(subgroupName, roolName, participant);
                    }
                });
            }
            else
            {
                foreach (KeyValuePair<string, List<ParticipantWrapper>> participantsList in filteredParticipants)
                {
                    participantsList.Value.ForEach(participant =>
                    {
                        string subgroupName = "";
                        if (lastAddedGroup1 > subgroupsCount)
                        {
                            subgroupName = "Подгруппа 1";
                            category.GetSubgroupByName(subgroupName).AddParticipant(participant);
                            lastAddedGroup1 = 2;
                        }
                        else
                        {
                            subgroupName = "Подгруппа " + lastAddedGroup1;
                            category.GetSubgroupByName(subgroupName).AddParticipant(participant);
                            lastAddedGroup1++;
                        }
                        foreach (string roolName in selectedRools)
                        {
                            CheckRoolAndLogErrors(subgroupName, roolName, participant);
                        }
                    });
                }
            }
            lastAddedGroup = lastAddedGroup1;
        }

        private static void ParticipantsSortWithRools(List<ParticipantWrapper> participants, ref int lastAddedGroup)
        {
            if (selectedRools.Contains("Правило города"))
            {
                // город - участники
                Dictionary<string, List<ParticipantWrapper>> city = FilterParticipantsForCities(participants);
                foreach (KeyValuePair<string, List<ParticipantWrapper>> entry in city)
                {
                    if (selectedRools.Contains("Правило одноклубников"))
                    {
                        Dictionary<string, List<ParticipantWrapper>> club = FilterParticipantsForClubs(entry.Value);
                        ParticipantsSort(entry.Value, ref lastAddedGroup, club);
                    }
                    else
                    {
                        ParticipantsSort(entry.Value, ref lastAddedGroup);
                    }
                }
            }
            else
            {
                if (selectedRools.Contains("Правило одноклубников"))
                {
                    Dictionary<string, List<ParticipantWrapper>> club = FilterParticipantsForClubs(participants);
                    ParticipantsSort(participants, ref lastAddedGroup, club);
                }
                else
                {
                    ParticipantsSort(participants, ref lastAddedGroup);
                }
            }
        }

        private void RemoveShadowFromCategoryButtons()
        {
            categoriesButtons.ForEach(button => {
                button.Effect = null;
            });
        }

        private void AddShadowToCategoryButtonWithErrors(CategoryWrapper category)
        {
            string categoryName = category.Name;
            if (category.Subgroups.Exists(subgroup => subgroup.Errors.Count > 0))
            {
                Button btn = categoriesButtons.Find(button => button.Tag.ToString().Equals(categoryName));

                btn.Effect = new DropShadowEffect
                {
                    Color = orange.Color,
                    Opacity = 0.5,
                    BlurRadius = 10,
                };
            }
        }

        private void AddShadowToCategoryButtonsWithErrors()
        {
            List<CategoryWrapper> categories = GetCategoriesFromNomination(selectedNomination);
            categories.ForEach(category => {
                AddShadowToCategoryButtonWithErrors(category);
            });
        }

        private void PrepareCategoryForSubgrouping()
        {
            CategoryWrapper category = GetCategoryFromNomination(selectedNomination, selectedCategory);
            if (category.ContainsSubgroups)
            {
                category.RemoveAllSubgroups();
            }
        }

        /// <summary>
        /// Формирование подгрупп
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SubgroupsFormation(object sender, RoutedEventArgs e)
        {
            int _subgroups = int.Parse(countSubgroups.Text);
            int _countInKategory = int.Parse(countInCategory.Content.ToString());
            PrepareCategoryForSubgrouping();
            if (_countInKategory > 0 && _subgroups > 0 && _countInKategory / _subgroups >= 2)
            {
                for (int i = 1; i <= _subgroups; i++)
                {
                    GetCategoryFromNomination(selectedNomination, selectedCategory).AddSubgroup("Подгруппа " + i);
                }
                var rand = new Random();
                List<ParticipantWrapper> participantsInCategory = Shuffle(rand, GetParticipantsInCategoryAndNomination(selectedNomination, selectedCategory));

                int lastAddedGroup = 1; // группа в которую последний раз добавляли участника

                List<ParticipantWrapper> posevParticipants = participantsInCategory.FindAll(p => p.Participant.Leader == true);
                List<ParticipantWrapper> not_posevParticipants = participantsInCategory.FindAll(p => p.Participant.Leader == false);

                // учет правила посевных бойцов
                if (selectedRools.Contains("Правило посевных бойцов"))
                {
                    ParticipantsSortWithRools(posevParticipants, ref lastAddedGroup);

                    ParticipantsSortWithRools(not_posevParticipants, ref lastAddedGroup);
                }
                else
                {
                    // без учета посевных бойцов
                    ParticipantsSortWithRools(participantsInCategory, ref lastAddedGroup);
                }

                ShowSubgroups();
                RemoveShadowFromCategoryButtons();
                AddShadowToCategoryButtonsWithErrors();
            }
            else
            {
                MessageBox.Show("Введено некорректное количество подгрупп!", "Ошибка", MessageBoxButton.OK);
            }
        }

        private static Dictionary<string, List<ParticipantWrapper>> FilterParticipantsForCities(List<ParticipantWrapper> participants)
        {
            Dictionary<string, List<ParticipantWrapper>> participantsForCities = new Dictionary<string, List<ParticipantWrapper>>();
            participants.ForEach(participant =>
            {
                string city = participant.City;
                if (participantsForCities.ContainsKey(city))
                {
                    participantsForCities[city].Add(participant);
                }
                else
                {
                    List<ParticipantWrapper> list = new List<ParticipantWrapper>();
                    list.Add(participant);
                    participantsForCities.Add(city, list);
                }
            });
            return participantsForCities;
        }

        private static Dictionary<string, List<ParticipantWrapper>> FilterParticipantsForClubs(List<ParticipantWrapper> participants)
        {
            Dictionary<string, List<ParticipantWrapper>> participantsForClubs = new Dictionary<string, List<ParticipantWrapper>>();
            participants.ForEach(participant =>
            {
                string club = participant.Club;
                if (participantsForClubs.ContainsKey(club))
                {
                    participantsForClubs[club].Add(participant);
                }
                else
                {
                    List<ParticipantWrapper> list = new List<ParticipantWrapper>();
                    list.Add(participant);
                    participantsForClubs.Add(club, list);
                }
            });
            return participantsForClubs;
        }

        private void ShowSubgroups()
        {
            subgroupsSettingsGrid.Children.Clear();
            subgroupsSettingsGrid.ShowGridLines = true;
            subgroupsSettingsGrid.RowDefinitions.Clear();
            subgroupsSettingsGrid.Drop += SubgroupsSettingsGrid_Drop;
            subgroupsSettingsGrid.DragOver += SubgroupsSettingsGrid_DragOver;
            int subgroupsCount = GetCategoryFromNomination(selectedNomination, selectedCategory).Subgroups.Count;
            List<SubgroupWrapper> subgroups = GetCategoryFromNomination(selectedNomination, selectedCategory).Subgroups;
            for (int i = 0; i < subgroupsCount; i++)
            {
                RowDefinition row = new RowDefinition();

                Grid grid = new Grid();
                grid.ShowGridLines = true;
                grid.DragOver += Grid_DragOver;
                grid.Margin = new Thickness(5);

                Grid headerSubgroup = new Grid();
                ColumnDefinition headerSubgroupCol1 = new ColumnDefinition();
                ColumnDefinition headerSubgroupCol2 = new ColumnDefinition();

                RowDefinition gridFirstRow = new RowDefinition();

                Label subgroupName = CreateLabel(subgroups[i].Name, 25);
                subgroupName.HorizontalAlignment = HorizontalAlignment.Left;

                string errors = "";
                List<string> errorsBySubgroup = subgroups[i].Errors;
                foreach (string str in errorsBySubgroup)
                {
                    errors += str + "\n";
                }
                Label errorList = CreateLabel(errors, 10);
                errorList.HorizontalAlignment = HorizontalAlignment.Right;

                headerSubgroup.ColumnDefinitions.Add(headerSubgroupCol1);
                headerSubgroup.ColumnDefinitions.Add(headerSubgroupCol2);
                headerSubgroup.Children.Add(subgroupName);
                Grid.SetColumn(subgroupName, 0);

                headerSubgroup.Children.Add(errorList);
                Grid.SetColumn(errorList, 1);

                grid.RowDefinitions.Add(gridFirstRow);
                grid.Children.Add(headerSubgroup);
                Grid.SetRow(headerSubgroup, grid.RowDefinitions.Count - 1);

                SolidColorBrush solidBG = new SolidColorBrush(Color.FromRgb(255, 215, 0));

                subgroups[i].Participants.ForEach(participant =>
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
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0,45,0,0)
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
    }
}
