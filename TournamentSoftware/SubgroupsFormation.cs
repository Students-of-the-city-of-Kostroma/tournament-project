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

        public UIElement kategoryList()
        {
            Grid grid = new Grid();
            grid.HorizontalAlignment = HorizontalAlignment.Stretch;
            grid.VerticalAlignment = VerticalAlignment.Stretch;
            RowDefinition row1 = new RowDefinition();
            RowDefinition row2 = new RowDefinition();
            row1.Height = new GridLength(50);
            grid.RowDefinitions.Add(row1);
            grid.RowDefinitions.Add(row2);
            Label label = new Label();
            label.Content = "Настройки категорий (подгрупп)";
            label.FontSize = 24;
            ListBox listBox = new ListBox();
            listBox.Resources.Add(SystemColors.HighlightBrushKey, "red");
            listBox.Padding = new Thickness(10);
            listBox.HorizontalContentAlignment = HorizontalAlignment.Center;
            getKategories(MainWindow.participantsList);
            foreach (string kategory in kategoryGroups.Keys)
            {
                Button kategoryButton = new Button();
                kategoryButton.Width = 100;
                kategoryButton.Height = 50;
                kategoryButton.FontSize = 15;
                kategoryButton.Content = kategory;
                listBox.Items.Add(kategoryButton);
            }
            grid.Children.Add(label);
            Grid.SetRow(label, 0);
            grid.Children.Add(listBox);
            Grid.SetRow(listBox, 1);
            return grid;
        }

        public UIElement kategorySettingsPanel()
        {
            Grid grid = new Grid();
            return grid;
        }
    }
}
