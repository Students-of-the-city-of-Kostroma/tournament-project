using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TournamentSoftware
{
    public class SubgroupsFormation
    {
        private Dictionary<string, List<ParticipantFormModel>> kategoryGroups = new Dictionary<string, List<ParticipantFormModel>>();
        private Grid kategorySettingsGrid = new Grid();
        private Grid subgroupsSettingsGrid = new Grid { AllowDrop = true };
        private Label countInKategory = new Label { FontSize = 30, VerticalAlignment = VerticalAlignment.Center };
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
        private List<Button> kategoriesButtons = new List<Button>();
        private int lastClickedKategory = -1;
        private List<string> selectedRools = new List<string> { "Правило посевных бойцов", "Правило одноклубников", "Правило города" };
        private string selectedKategory = "";
        private Button goNextButton = new Button();
        SolidColorBrush solidBG = new SolidColorBrush(Color.FromRgb(255, 215, 0));
        SolidColorBrush white = new SolidColorBrush(Color.FromRgb(255, 255, 255));
        SolidColorBrush red = new SolidColorBrush(Color.FromRgb(255, 0, 0));

        private bool passPosevRule = true;
        private bool passClubRule = true;
        private bool passCityRule = true;

        public Dictionary<string, List<ParticipantFormModel>> getKateoryGroups
        {
            get { return kategoryGroups; }
        }

        // получаем список катерогий
        private Dictionary<string, List<ParticipantFormModel>> getKategories(ObservableCollection<ParticipantFormModel> participants)
        {
            foreach (ParticipantFormModel participant in participants)
            {
                string kategory = participant.Kategory;
                if (kategoryGroups.ContainsKey(kategory))
                {
                    kategoryGroups[kategory].Add(participant);
                }
                else
                {
                    List<ParticipantFormModel> list = new List<ParticipantFormModel>();
                    list.Add(participant);
                    kategoryGroups.Add(kategory, list);
                }
            }

            return kategoryGroups;
        }

        private Label createLabel(string content, int fontSize = 24)
        {
            Label label = new Label();
            label.Content = content;
            label.FontSize = fontSize;
            label.Margin = new Thickness(5);
            return label;
        }

        public UIElement nominationsList()
        {
            Grid grid = new Grid();
            grid.Margin = new Thickness(5);
            grid.VerticalAlignment = VerticalAlignment.Stretch;
            grid.HorizontalAlignment = HorizontalAlignment.Stretch;

            int rowsCount = 0;

            List<string> nominationsNames = MainWindow.GetReagistrator.nominationsNames;
            foreach (string name in nominationsNames)
            {
                RowDefinition row = new RowDefinition();
                Button nominationButton = new Button();
                nominationButton.Margin = new Thickness(5);
                nominationButton.Height = 30;
                nominationButton.FontSize = 15;
                nominationButton.Content = name;
                nominationButton.Tag = name;
                nominationButton.Click += NominationButton_Click;
                kategoriesButtons.Add(nominationButton);
                grid.RowDefinitions.Add(row);
                grid.Children.Add(nominationButton);
                Grid.SetRow(nominationButton, rowsCount);
                rowsCount++;
            }
            return grid;
        }

        private void NominationButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            string nomination = button.Tag.ToString();
            selectedNomination = nomination;
            ((MainWindow)Application.Current.MainWindow).SubgroupFormationLabel.Content = "Формирование групп. Номинация " + nomination;
        }

        public UIElement kategoryList()
        {
            Grid kategoriesGrid = new Grid();
            kategoriesGrid.Margin = new Thickness(5);
            kategoriesGrid.VerticalAlignment = VerticalAlignment.Stretch;
            kategoriesGrid.HorizontalAlignment = HorizontalAlignment.Stretch;

            int rowsCount = 0;

            getKategories(MainWindow.participantsList);
            foreach (string kategory in kategoryGroups.Keys)
            {
                RowDefinition row = new RowDefinition();
                Button kategoryButton = new Button ();
                kategoryButton.Margin = new Thickness(5);
                kategoryButton.Height = 30;
                kategoryButton.FontSize = 15;
                kategoryButton.Content = kategory;
                kategoryButton.Tag = kategory;

                if (kategoryGroups[kategory].Count == 1)
                {
                    kategoryButton.ToolTip = "В этой категории всего 1 участник";
                    kategoryButton.Click += KategoryButton_Click;
                }
                else 
                {
                    kategoryButton.Click += KategoryButton_Click;
                }
                kategoriesButtons.Add(kategoryButton);
                kategoriesGrid.RowDefinitions.Add(row);
                kategoriesGrid.Children.Add(kategoryButton);
                Grid.SetRow(kategoryButton, rowsCount);
                rowsCount++;
            }

            return kategoriesGrid;
        }

        private void KategoryButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            button.Background = solidBG;
            string kategory = button.Tag.ToString();
            selectedKategory = kategory;
            countInKategory.Content = kategoryGroups[kategory].Count;
            showKategorySettings();
            if (lastClickedKategory != -1)
            {
                kategoriesButtons[lastClickedKategory].Background = white;
            }
            countSubgroups.Text = "";
            lastClickedKategory = kategoriesButtons.IndexOf(button);
        }

        public void showKategorySettings()
        {
            subgroupsSettingsGrid.Children.Clear();
            kategorySettingsGrid.Children.Clear();
            kategorySettingsGrid.ShowGridLines = true;
            var parent = VisualTreeHelper.GetParent(countInKategory);
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
            countInLategoryGrid.Children.Add(countInKategory);
            countInKategory.HorizontalAlignment = HorizontalAlignment.Right;
            Label label1 = createLabel("Количество\nбойцов в\nгруппе", 15);
            label1.HorizontalContentAlignment = HorizontalAlignment.Left;
            label1.HorizontalAlignment = HorizontalAlignment.Left;
            label1.VerticalAlignment = VerticalAlignment.Center;
            countInLategoryGrid.Children.Add(label1);
            Grid.SetColumn(countInKategory, 0);
            Grid.SetColumn(label1, 1);

            var countOfSubgroups = countOfSubgroupsGrid();
            var rools = chooseRools();

            goNextButton = new Button();
            goNextButton.IsEnabled = false;
            goNextButton.Margin = new Thickness(5);
            goNextButton.Content = "Продолжить";
            goNextButton.HorizontalAlignment = HorizontalAlignment.Center;
            goNextButton.VerticalAlignment = VerticalAlignment.Top;
            goNextButton.Height = 40;
            goNextButton.Click += GoNext_Click;

            kategorySettingsGrid.Children.Add(goNextButton);
            kategorySettingsGrid.Children.Add(rools);
            kategorySettingsGrid.Children.Add(countOfSubgroups);
            kategorySettingsGrid.Children.Add(countInLategoryGrid);
            Grid.SetRow(countInLategoryGrid, 0);
            Grid.SetRow(rools, 1);
            Grid.SetRow(countOfSubgroups, 2);
            Grid.SetRow(goNextButton, 3);
        }

        /// <summary>
        /// Формирование подгрупп
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GoNext_Click(object sender, RoutedEventArgs e)
        {
            int _subgroups = int.Parse(countSubgroups.Text);
            int _countInKategory = int.Parse(countInKategory.Content.ToString());
            if (_countInKategory>0 && _subgroups>0 && _countInKategory / _subgroups >= 2)
            {
                Dictionary<string, List<ParticipantFormModel>> subgroups = new Dictionary<string, List<ParticipantFormModel>>();
                for (int i = 1; i <= _subgroups; i++) 
                {
                    subgroups.Add(i.ToString(), new List<ParticipantFormModel>());
                }
                List<ParticipantFormModel> participantsInKategory = kategoryGroups[selectedKategory];

                int lastAddedGroup = 1; // группа в которую последний раз добавляли участника

                checkRules(_subgroups, participantsInKategory);

                Button button = kategoriesButtons.Find(btn => btn.Tag.ToString().Equals(selectedKategory));

                string str = "";

                if (!passPosevRule)
                {
                    button.Background = red;
                    str += "Нарушение правила посевных бойцов";
                    button.ToolTip = str;
                }

                if (!passClubRule)
                {
                    button.Background = red;
                    str += "\nНарушение правила одноклубников";
                    button.ToolTip = str;
                }

                if (!passCityRule)
                {
                    button.Background = red;
                    str += "\nНарушение правила города";
                    button.ToolTip = str;
                }

                participantsInKategory.ForEach(participant => {
                    if (lastAddedGroup > _subgroups)
                    {
                        subgroups["1"].Add(participant);
                        lastAddedGroup = 2;
                    }
                    else 
                    {
                        subgroups[lastAddedGroup.ToString()].Add(participant);
                        lastAddedGroup++;
                    }
                });

                setSubgroups(subgroups);
            }
            else 
            {
                MessageBox.Show("Введено некорректное количество подгрупп!", "Ошибка", MessageBoxButton.OK);
            }
        }



        private bool checkRules(int subgroupsCount, List<ParticipantFormModel> participants)
        {
            Dictionary<string, int> club_count = new Dictionary<string, int>();
            Dictionary<string, int> city_count = new Dictionary<string, int>();
            int posevCount = 0;
            // количество посевных в подгруппе
            participants.ForEach(p=> {
                string club = p.Club;
                string city = p.City;
                // подсчет одноклубников
                if (club_count.ContainsKey(club))
                {
                    int count = club_count[club] + 1;
                    club_count[club] = count;
                }
                else
                {
                    club_count.Add(club, 1);
                }

                // подсчет одногородних
                if (city_count.ContainsKey(city))
                {
                    int count = city_count[city] + 1;
                    city_count[city] = count;
                }
                else
                {
                    city_count.Add(city,1);
                }

                // подсчет посевных
                if (p.Participant.Leader)
                {
                    posevCount++;
                }
            });

            foreach (KeyValuePair<string, int> entry in club_count)
            {
                if (entry.Value > subgroupsCount)
                {
                    passClubRule = false;
                    passCityRule = false;
                }
            }
            
            foreach (KeyValuePair<string, int> entry in city_count)
            {
                if (entry.Value > subgroupsCount)
                {
                    passCityRule = false;
                }
            }

            passPosevRule = posevCount <= subgroupsCount;

            return posevCount <= subgroupsCount && passClubRule;
        }

        private void setSubgroups(Dictionary<string, List<ParticipantFormModel>> subgroups)
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
                Label label = createLabel("Подгруппа" + i, 25);
                label.HorizontalAlignment = HorizontalAlignment.Left;
                grid.RowDefinitions.Add(r);
                grid.Children.Add(label);
                Grid.SetRow(label, grid.RowDefinitions.Count - 1);

                SolidColorBrush solidBG = new SolidColorBrush(Color.FromRgb(255, 215, 0));
                Console.WriteLine("В подгруппе" + i + " -> " + subgroups[i.ToString()].Count + " участников");

                subgroups[i.ToString()].ForEach(participant => {
                    Label l = createLabel(participant.Participant.Name + " " + participant.Participant.Surname + " " + participant.Participant.Patronymic, 20);
                    l.ToolTip = "Посев: " + participant.Participant.Leader+"\nКлуб: " + participant.Club + "\nГород: " + participant.City;
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

        private UIElement chooseRools()
        {
            Grid grid = new Grid();
            RowDefinition row1 = new RowDefinition();
            row1.Height = new GridLength(40, GridUnitType.Pixel);
            grid.RowDefinitions.Add(row1);
            
            Label label = createLabel("Выбор правил", 15);
            label.HorizontalAlignment = HorizontalAlignment.Center;

            grid.Children.Add(label);
            Grid.SetRow(label, 0);

            rools.ForEach(rool =>
            {
                RowDefinition row = new RowDefinition { Height = new GridLength(30, GridUnitType.Pixel) };
                CheckBox checkBox = new CheckBox { Content = rool, Tag = rool };
                checkBox.Margin = new Thickness(5,0,0,0);
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

        private UIElement countOfSubgroupsGrid()
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

            Label label = createLabel("Количество подгрупп", 15);
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

        public UIElement kategorySettingsPanel()
        {
            subgroupsSettingsGrid.Children.Clear();
            kategorySettingsGrid.Children.Clear();
            kategorySettingsGrid = new Grid();
            RowDefinition row1 = new RowDefinition();
            RowDefinition row2 = new RowDefinition();
            RowDefinition row3 = new RowDefinition();
            RowDefinition row4 = new RowDefinition();

            row1.Height = new GridLength(80);
            row2.Height = new GridLength(150);
            row3.Height = new GridLength(80);
            row4.Height = new GridLength(1, GridUnitType.Star);

            Label startMessage = new Label
            {
                Content = "Выберите подгруппу",
                FontSize = 15,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center,
            };

            kategorySettingsGrid.RowDefinitions.Add(row1);
            kategorySettingsGrid.RowDefinitions.Add(row2);
            kategorySettingsGrid.RowDefinitions.Add(row3);
            kategorySettingsGrid.RowDefinitions.Add(row4);

            kategorySettingsGrid.Children.Add(startMessage);
            Grid.SetRow(startMessage, 1);
            return kategorySettingsGrid;
        }

        public UIElement subgroupSettings()
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

        public void selectNomination(object sender, RoutedEventArgs e)
        {
            selectedNomination = ((Button)e.Source).Content.ToString();
            ((MainWindow)Application.Current.MainWindow).SubgroupFormationLabel.Content = "Формирование групп. " + selectedNomination;
        }
    }
}
