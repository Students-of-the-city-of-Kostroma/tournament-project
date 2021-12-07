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
        private Label countInKategory = new Label();
        private TextBox countSubgroups = new TextBox();
        private List<string> fightSystems = new List<string> {
            "Круговая",
            "На вылет",
            "Смешанный тип",
            "До двух поражений" };
        private List<string> rools = new List<string> { "Разводной", "Безразводной", "Сходовой"};

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

        private Grid createGrid()
        {
            Grid grid = new Grid();
            grid.HorizontalAlignment = HorizontalAlignment.Stretch;
            grid.VerticalAlignment = VerticalAlignment.Stretch;
            RowDefinition row1 = new RowDefinition();
            RowDefinition row2 = new RowDefinition();
            row1.Height = new GridLength(50);
            grid.RowDefinitions.Add(row1);
            grid.RowDefinitions.Add(row2);
            return grid;
        }

        private Label createLabel(string content, int fontSize = 24)
        {
            Label label = new Label();
            label.Content = content;
            label.FontSize = fontSize;
            label.Margin = new Thickness(5);
            return label;
        }

        public UIElement kategoryList()
        {
            Grid grid = createGrid();
            Label label = createLabel("Настройки категорий (подгрупп)");
            ListBox listBox = new ListBox();
            listBox.Padding = new Thickness(10);
            listBox.Margin = new Thickness(0, 0, 5, 0);
            listBox.HorizontalContentAlignment = HorizontalAlignment.Center;
            getKategories(MainWindow.participantsList);
            foreach (string kategory in kategoryGroups.Keys)
            {
                Button kategoryButton = new Button();
                kategoryButton.Width = 100;
                kategoryButton.Height = 50;
                kategoryButton.FontSize = 15;
                kategoryButton.Content = kategory;
                kategoryButton.Tag = kategory;
                kategoryButton.Click += KategoryButton_Click;
                listBox.Items.Add(kategoryButton);
            }
            grid.Children.Add(label);
            Grid.SetRow(label, 0);
            grid.Children.Add(listBox);
            Grid.SetRow(listBox, 1);
            return grid;
        }

        private void KategoryButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            string kategory = button.Tag.ToString();
            kategorySettingsGrid.Visibility = Visibility.Visible;
            countInKategory.Content = kategoryGroups[kategory].Count;
        }

        public UIElement kategorySettingsPanel()
        {
            kategorySettingsGrid = createGrid();
            Label label = createLabel("Параметры подгруппы");
            kategorySettingsGrid.Children.Add(label);
            Grid.SetRow(label, 0);
            SolidColorBrush solidBG = new SolidColorBrush(Color.FromArgb(0xFF, 0x23, 0x28, 0x2C));
            SolidColorBrush color1 = new SolidColorBrush(Color.FromArgb(0xFF, 255, 0, 0));

            countInKategory = createLabel("", 30);

            Grid grid = new Grid();
            grid.HorizontalAlignment = HorizontalAlignment.Stretch;
            grid.VerticalAlignment = VerticalAlignment.Stretch;
            RowDefinition row1 = new RowDefinition();
            row1.Height = new GridLength(80);
            RowDefinition row2 = new RowDefinition();
            row2.Height = new GridLength(50);
            RowDefinition row3 = new RowDefinition();
            row3.Height = new GridLength(105);
            RowDefinition row4 = new RowDefinition();
            row4.Height = new GridLength(1, GridUnitType.Star);
            grid.RowDefinitions.Add(row1);
            grid.RowDefinitions.Add(row2);
            grid.RowDefinitions.Add(row3);
            grid.RowDefinitions.Add(row4);

            Grid firstRow = new Grid();
            ColumnDefinition column1 = new ColumnDefinition();
            ColumnDefinition column2 = new ColumnDefinition();
            firstRow.ColumnDefinitions.Add(column1);
            firstRow.ColumnDefinitions.Add(column2);
            firstRow.VerticalAlignment = VerticalAlignment.Stretch;
            Label countInKategoryLabel = createLabel("Количество \nбойцов в \nгруппе", 15);
            countInKategoryLabel.HorizontalContentAlignment = HorizontalAlignment.Left;
            countInKategoryLabel.VerticalAlignment = VerticalAlignment.Stretch;
            countInKategory.VerticalAlignment = VerticalAlignment.Stretch;
            countInKategory.HorizontalContentAlignment = HorizontalAlignment.Right;
            countInKategory.VerticalContentAlignment = VerticalAlignment.Center;
            firstRow.Children.Add(countInKategory);
            firstRow.Children.Add(countInKategoryLabel);
            Grid.SetColumn(countInKategory, 0);
            Grid.SetColumn(countInKategoryLabel, 1);

            // строка с полем ввода количества групп
            Grid secondRow = new Grid();
            ColumnDefinition secondRowColumn1 = new ColumnDefinition();
            secondRowColumn1.Width = new GridLength(1, GridUnitType.Star);
            ColumnDefinition secondRowColumn2 = new ColumnDefinition();
            secondRowColumn2.Width = new GridLength(2, GridUnitType.Star);
            secondRow.ColumnDefinitions.Add(secondRowColumn1);
            secondRow.ColumnDefinitions.Add(secondRowColumn2);
            Label countSubgroupsLabel = createLabel("Количество подгрупп", 15);
            countSubgroupsLabel.HorizontalContentAlignment = HorizontalAlignment.Left;
            countSubgroupsLabel.VerticalContentAlignment = VerticalAlignment.Center;
            countSubgroups.Height = 40;
            countSubgroups.Width = 60;
            countSubgroups.HorizontalAlignment = HorizontalAlignment.Right;
            countSubgroups.FontSize = 30;
            countSubgroups.PreviewTextInput += CountSubgroups_PreviewTextInput;
            secondRow.Children.Add(countSubgroups);
            secondRow.Children.Add(countSubgroupsLabel);
            Grid.SetColumn(countSubgroupsLabel, 1);
            Grid.SetColumn(countSubgroups, 0);

            Grid thirdRow = new Grid();
            RowDefinition thirdRowRow1 = new RowDefinition();
            thirdRowRow1.Height = new GridLength(40);
            RowDefinition thirdRowRow2 = new RowDefinition();
            thirdRow.RowDefinitions.Add(thirdRowRow1);
            thirdRow.RowDefinitions.Add(thirdRowRow2);
            Label chooseRulesLabel = createLabel("Выбрать правила", 15);
            ListBox roolsListBox = new ListBox();
            foreach (string rool in rools)
            {
                RadioButton radioButton = new RadioButton();
                radioButton.Content = rool;
                roolsListBox.Items.Add(radioButton);
            }
            thirdRow.Children.Add(chooseRulesLabel);
            thirdRow.Children.Add(roolsListBox);
            Grid.SetRow(chooseRulesLabel, 0);
            Grid.SetRow(roolsListBox, 1);

            Label fightSystemLabel = createLabel("Выбор вида системы боев", 16);
            fightSystemLabel.HorizontalAlignment = HorizontalAlignment.Center;
            fightSystemLabel.VerticalAlignment = VerticalAlignment.Bottom;
            ComboBox fightSystemsComboBox = new ComboBox();
            fightSystemsComboBox.HorizontalAlignment = HorizontalAlignment.Stretch;
            fightSystemsComboBox.HorizontalContentAlignment = HorizontalAlignment.Center;
            fightSystemsComboBox.VerticalAlignment = VerticalAlignment.Top;
            fightSystemsComboBox.Margin = new Thickness(30, 0, 30, 10);
            fightSystemsComboBox.Height = 30;
            fightSystemsComboBox.FontSize = 15;
            fightSystemsComboBox.VerticalAlignment = VerticalAlignment.Top;
            fightSystemsComboBox.ItemsSource = fightSystems;

            Grid fightSystemRow = new Grid();
            RowDefinition fightSystemRow1 = new RowDefinition();
            RowDefinition fightSystemRow2 = new RowDefinition();
            fightSystemRow.RowDefinitions.Add(fightSystemRow1);
            fightSystemRow.RowDefinitions.Add(fightSystemRow2);
            fightSystemRow.Children.Add(fightSystemLabel);
            fightSystemRow.Children.Add(fightSystemsComboBox);
            Grid.SetRow(fightSystemLabel, 0);
            Grid.SetRow(fightSystemsComboBox, 1);
            fightSystemRow.VerticalAlignment = VerticalAlignment.Top;

            grid.Children.Add(fightSystemRow);
            Grid.SetRow(fightSystemRow, 3);

            grid.Children.Add(firstRow);
            Grid.SetRow(firstRow, 0);

            grid.Children.Add(secondRow);
            Grid.SetRow(secondRow, 1);

            grid.Children.Add(thirdRow);
            Grid.SetRow(thirdRow, 2);

            kategorySettingsGrid.Children.Add(grid);
            Grid.SetRow(grid, 1);

            kategorySettingsGrid.Visibility = Visibility.Hidden;

            return kategorySettingsGrid;
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

        public UIElement nominationsList()
        {
            Grid grid = createGrid();
            Label label = createLabel("Номинации");
            ListBox listBox = new ListBox();
            listBox.Padding = new Thickness(10);
            listBox.Margin = new Thickness(0, 0, 5, 0);
            listBox.HorizontalContentAlignment = HorizontalAlignment.Center;

            List<string> nominationsNames = MainWindow.GetReagistrator.nominationsNames;
            Console.WriteLine(nominationsNames.Count);
            foreach (string nomination in nominationsNames)
            {
                Button button = new Button();
                button.Content = nomination;
                button.Width = 100;
                button.Height = 50;
                button.FontSize = 15;
                listBox.Items.Add(button);
            }

            grid.Children.Add(label);
            grid.Children.Add(listBox);
            Grid.SetRow(label, 0);
            Grid.SetRow(listBox, 1);
            return grid;
        }
    }
}
