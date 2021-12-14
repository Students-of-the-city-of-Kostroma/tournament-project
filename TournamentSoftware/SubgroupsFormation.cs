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
        private Dictionary<string, List<Participant>> kategoryGroups = new Dictionary<string, List<Participant>>();
        private Grid kategorySettingsGrid = new Grid();
        private Grid subgroupsSettingsGrid = new Grid();
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

        public Dictionary<string, List<Participant>> getKateoryGroups
        {
            get { return kategoryGroups; }
        }

        // получаем список катерогий
        private Dictionary<string, List<Participant>> getKategories(ObservableCollection<ParticipantFormModel> participants)
        {
            foreach (ParticipantFormModel participant in participants)
            {
                string kategory = participant.Kategory;
                if (kategoryGroups.ContainsKey(kategory))
                {
                    kategoryGroups[kategory].Add(participant.Participant);
                }
                else
                {
                    List<Participant> list = new List<Participant>();
                    list.Add(participant.Participant);
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
            ((MainWindow)Application.Current.MainWindow).SubgroupFormationLabel.Content = "Формирование групп. " + nomination;
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
                Button kategoryButton = new Button();
                kategoryButton.Margin = new Thickness(5);
                kategoryButton.Height = 30;
                kategoryButton.FontSize = 15;
                kategoryButton.Content = kategory;
                kategoryButton.Tag = kategory;
                kategoryButton.Click += KategoryButton_Click;
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
            SolidColorBrush solidBG = new SolidColorBrush(Color.FromRgb(255, 215, 0));
            SolidColorBrush white = new SolidColorBrush(Color.FromRgb(255, 255, 255));
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

            Button goNext = new Button();
            goNext.Margin = new Thickness(5);
            goNext.Content = "Продолжить";
            goNext.HorizontalAlignment = HorizontalAlignment.Center;
            goNext.VerticalAlignment = VerticalAlignment.Top;
            goNext.Height = 40;
            goNext.Click += GoNext_Click;

            kategorySettingsGrid.Children.Add(goNext);
            kategorySettingsGrid.Children.Add(rools);
            kategorySettingsGrid.Children.Add(countOfSubgroups);
            kategorySettingsGrid.Children.Add(countInLategoryGrid);
            Grid.SetRow(countInLategoryGrid, 0);
            Grid.SetRow(rools, 1);
            Grid.SetRow(countOfSubgroups, 2);
            Grid.SetRow(goNext, 3);
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
                Dictionary<string, List<Participant>> subgroups = new Dictionary<string, List<Participant>>();
                for (int i = 1; i <= _subgroups; i++) 
                {
                    subgroups.Add(i.ToString(), new List<Participant>());
                }
                List<Participant> participantsInKategory = kategoryGroups[selectedKategory];

                int lastAddedGroup = 1; // группа в которую последний раз добавляли участника

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

        private void setSubgroups(Dictionary<string, List<Participant>> subgroups)
        {
            subgroupsSettingsGrid.Children.Clear();
            subgroupsSettingsGrid.RowDefinitions.Clear();

            for (int i = 1; i <= subgroups.Count; i++)
            {
                RowDefinition row = new RowDefinition();

                Grid grid = new Grid();
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
                    Label l = createLabel(participant.Name + " " + participant.Surname + " " + participant.Patronymic, 20);
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
            }
        }

        public void selectNomination(object sender, RoutedEventArgs e)
        {
            selectedNomination = ((Button)e.Source).Content.ToString();
            ((MainWindow)Application.Current.MainWindow).SubgroupFormationLabel.Content = "Формирование групп. " + selectedNomination;
        }
    }
}
