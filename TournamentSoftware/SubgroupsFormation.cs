using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace TournamentSoftware
{
    public class SubgroupsFormation
    {
        // хранит участников в категории
        private Dictionary<string, List<Participant>> kategoryGroups = new Dictionary<string, List<Participant>>();
        private Grid kategorySettingsGrid = new Grid();
        private Label countInKategory = new Label();

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

        private Label createLabel(string content)
        {
            Label label = new Label();
            label.Content = content;
            label.FontSize = 24;
            label.Margin = new Thickness(5);
            return label;
        }

        public UIElement kategoryList()
        {
            Grid grid = createGrid();
            Label label = createLabel("Настройки категорий (подгрупп)");
            ListBox listBox = new ListBox();
            listBox.Padding = new Thickness(10);
            listBox.Margin = new Thickness(10);
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
            countInKategory.Content = kategoryGroups[kategory].Count;
        }

        public UIElement kategorySettingsPanel()
        {
            kategorySettingsGrid = createGrid();
            Label label = createLabel("Параметры подгруппы");
            kategorySettingsGrid.Children.Add(label);
            Grid.SetRow(label, 0);

            Grid grid = new Grid();
            grid.HorizontalAlignment = HorizontalAlignment.Stretch;
            grid.VerticalAlignment = VerticalAlignment.Stretch;
            RowDefinition row1 = new RowDefinition();
            row1.Height = new GridLength(100);
            RowDefinition row2 = new RowDefinition();
            RowDefinition row3 = new RowDefinition();
            RowDefinition row4 = new RowDefinition();
            grid.RowDefinitions.Add(row1);
            grid.RowDefinitions.Add(row2);
            grid.RowDefinitions.Add(row3);
            grid.RowDefinitions.Add(row4);

            Grid firstRow = new Grid();
            ColumnDefinition column1 = new ColumnDefinition();
            ColumnDefinition column2 = new ColumnDefinition();
            firstRow.ColumnDefinitions.Add(column1);
            firstRow.ColumnDefinitions.Add(column2);
            Label countInKategoryLabel = createLabel("Количество участников в категории");
            countInKategoryLabel.FontSize = 12;
            firstRow.Children.Add(countInKategory);
            firstRow.Children.Add(countInKategoryLabel);
            Grid.SetColumn(countInKategory, 0);
            Grid.SetColumn(countInKategoryLabel, 1);

            grid.Children.Add(firstRow);
            Grid.SetRow(firstRow, 0);
            row1.Height = new GridLength(50);

            kategorySettingsGrid.Children.Add(grid);
            Grid.SetRow(grid, 1);

            return kategorySettingsGrid;
        }

        public UIElement nominationsList()
        {
            Grid grid = createGrid();
            Label label = createLabel("Номинации");
            ListBox listBox = new ListBox();
            listBox.Padding = new Thickness(10);
            listBox.Margin = new Thickness(10);
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
