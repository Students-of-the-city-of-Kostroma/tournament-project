using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace TournamentSoftware
{
    public class SubgroupsFormation
    {
        private Dictionary<string, List<Participant>> kategoryGroups = new Dictionary<string, List<Participant>>();
        private Grid kategorySettingsGrid = new Grid();
        private Label countInKategory = new Label();
        private TextBox countSubgroups = new TextBox();
        private string selectedNomination = "";
        public bool isPanelOpen = true;
        private List<string> rools = new List<string> { "Правило посевных бойцов", "Правило одноклубников", "Правило города" };
        private List<Button> kategoriesButtons = new List<Button>();
        private int lastClickedKategory = -1;
        private ParticipantsReagistrator registrator = new ParticipantsReagistrator();

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
            showKategorySettings();
            countInKategory.Content = kategoryGroups[kategory].Count;
            if (lastClickedKategory != -1)
            {
                kategoriesButtons[lastClickedKategory].Background = white;
            }
            lastClickedKategory = kategoriesButtons.IndexOf(button);
        }

        public void showKategorySettings()
        {
            
        }

        public UIElement kategorySettingsPanel()
        {
            kategorySettingsGrid = new Grid();
            RowDefinition row1 = new RowDefinition();
            RowDefinition row2 = new RowDefinition();
            RowDefinition row3 = new RowDefinition();
            RowDefinition row4 = new RowDefinition();

            row1.Height = new GridLength(80);
            row2.Height = new GridLength(100);
            row3.Height = new GridLength(80);
            row4.Height = new GridLength(1, GridUnitType.Star);

            Label startMessage = new Label { 
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
            Grid grid = new Grid();
            RowDefinition row1 = new RowDefinition();
            RowDefinition row2 = new RowDefinition();
            row1.Height = new GridLength(80);
            row2.Height = new GridLength(100);
            grid.RowDefinitions.Add(row1);
            grid.RowDefinitions.Add(row2);
            Label startMessage = new Label
            {
                Content = "Введите кол-во подгрупп",
                FontSize = 15,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center,
            };
            grid.Children.Add(startMessage);
            Grid.SetRow(startMessage, 1);
            return grid;
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

        public UIElement nominationsListGrid()
        {
            DataGrid nominationsList = new DataGrid();
            nominationsList.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            nominationsList.HeadersVisibility = DataGridHeadersVisibility.None;
            DataGridTemplateColumn column = new DataGridTemplateColumn();
            ObservableCollection<Nomination> nominationsNames = MainWindow.nominationsList;


            Binding bind = new Binding("Name")
            {
                Mode = BindingMode.TwoWay
            };

            FrameworkElementFactory button = new FrameworkElementFactory(typeof(Button));
            button.AddHandler(Button.ClickEvent, new RoutedEventHandler(selectNomination));
            button.SetBinding(ContentControl.ContentProperty, bind);
            DataTemplate buttonTemplate = new DataTemplate
            {
                VisualTree = button
            };

            column.CellTemplate = buttonTemplate;
            nominationsList.Columns.Add(column);
            nominationsList.ItemsSource = nominationsNames;
            return nominationsList;
        }

        public void selectNomination(object sender, RoutedEventArgs e)
        {
            selectedNomination = ((Button)e.Source).Content.ToString();
            ((MainWindow)Application.Current.MainWindow).SubgroupFormationLabel.Content = "Формирование групп. " + selectedNomination;
        }
    }
}
