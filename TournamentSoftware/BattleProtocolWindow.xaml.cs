using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using TournamentSoftware.DB_Classes;
using TournamentSoftware.wrapperClasses;

namespace TournamentSoftware
{
    /// <summary>
    /// Interaction logic for BattleProtocolWindow.xaml
    /// </summary>
    public partial class BattleProtocolWindow : Window
    {
        public BattleProtocolWindow(BattleProtocol battleProtocol)
        {
            InitializeComponent();
            DataContext = new BattleProtocolWrapper();
            fightersNamesLabel.Content += "Фамилия И. 1/ Фамилия И. 2";
            SwitchRound(1);
            addJudgeButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        }

        private void SwitchRound(int numberOfRound)
        {
            ObservableCollection<RoundResultWrapper> roundResults = (DataContext as BattleProtocolWrapper).RoundResult;
            foreach (RoundResultWrapper roundResult in roundResults)
                if (roundResults.IndexOf(roundResult) == numberOfRound - 1)
                {
                    fightProtocolTable.ItemsSource = roundResult.FighterRoundResult;
                    (DataContext as BattleProtocolWrapper).NumberOfCurrentRound = numberOfRound;
                }
        }

        private void addRoundButton_Click(object sender, RoutedEventArgs e)
        {
            RoundResultWrapper newRoundResult = new RoundResultWrapper();
            (DataContext as BattleProtocolWrapper).RoundResult.Add(newRoundResult);
            SwitchRound((DataContext as BattleProtocolWrapper).RoundResult.Count);

            foreach (FighterRoundResultWrapper fighterRoundResult in newRoundResult.FighterRoundResult)
                for (int i = 0; i < (DataContext as BattleProtocolWrapper).SelectedJudges.Count; i++)
                    fighterRoundResult.JudgeScore.Add(0);
        }

        private void deleteRoundButton_Click(object sender, RoutedEventArgs e)
        {
            if ((DataContext as BattleProtocolWrapper).NumberOfCurrentRound > 1)
            {
                SwitchRound((DataContext as BattleProtocolWrapper).RoundResult.Count - 1);
                (DataContext as BattleProtocolWrapper).RoundResult.RemoveAt((DataContext as BattleProtocolWrapper).NumberOfCurrentRound - 1);
            }
        }

        private void prevRoundButton_Click(object sender, RoutedEventArgs e)
        {
            SwitchRound((DataContext as BattleProtocolWrapper).NumberOfCurrentRound - 1);
        }

        private void nextRoundButton_Click(object sender, RoutedEventArgs e)
        {
            SwitchRound((DataContext as BattleProtocolWrapper).NumberOfCurrentRound + 1);
        }

        private DataGridTemplateColumn GetJudgeColumn()
        {
            // Create The Column
            DataGridTemplateColumn judgeColumn = new DataGridTemplateColumn();

            Binding bindItemSource = new Binding();
            bindItemSource.Path = new PropertyPath("AvailableJudges");
            bindItemSource.Mode = BindingMode.TwoWay;

            // Create the ComboBox
            FrameworkElementFactory comboFactory = new FrameworkElementFactory(typeof(ComboBox));
            comboFactory.SetValue(ComboBox.DataContextProperty, DataContext as BattleProtocolWrapper);
            comboFactory.SetValue(ComboBox.NameProperty, "judgeCombobox_" + ((DataContext as BattleProtocolWrapper).SelectedJudges.Count - 1));
            comboFactory.SetValue(ComboBox.IsTextSearchEnabledProperty, false);
            comboFactory.SetValue(ComboBox.WidthProperty, 180.0);
            comboFactory.SetValue(ComboBox.DisplayMemberPathProperty, "Surname");
            comboFactory.SetValue(ComboBox.BorderThicknessProperty, new Thickness(0));
            comboFactory.SetBinding(ComboBox.ItemsSourceProperty, bindItemSource);
            comboFactory.AddHandler(ComboBox.SelectionChangedEvent, new SelectionChangedEventHandler(JudgeCombobox_SelectedChanged));

            DataTemplate comboTemplate = new DataTemplate();
            comboTemplate.VisualTree = comboFactory;

            // Create the TextBlock
            FrameworkElementFactory textFactory = new FrameworkElementFactory(typeof(TextBox));
            textFactory.SetValue(TextBox.BorderThicknessProperty, new Thickness(0));
            textFactory.AddHandler(TextBox.TextChangedEvent, new TextChangedEventHandler(ScoreTextBox_TextChanged));

            DataTemplate textTemplate = new DataTemplate();
            textTemplate.VisualTree = textFactory;

            // Set the Templates to the Column
            judgeColumn.HeaderTemplate = comboTemplate;
            judgeColumn.CellTemplate = textTemplate;

            return judgeColumn;
        }

        private void JudgeCombobox_SelectedChanged(object sender, RoutedEventArgs e)
        {
            int indexOfCombobox = Int32.Parse((sender as ComboBox).Name.Split('_')[1]);
            List<Judge> judges = (DataContext as BattleProtocolWrapper).SelectedJudges;
            judges[indexOfCombobox] = (sender as ComboBox).SelectedItem as Judge;
        }

        private void ScoreTextBox_TextChanged(object sender, RoutedEventArgs e)
        {
            int columnIndex = fightProtocolTable.CurrentColumn.DisplayIndex;
            int rowIndex = fightProtocolTable.SelectedIndex;

            int countOfRound = 1;
            foreach (RoundResultWrapper roundResult in (DataContext as BattleProtocolWrapper).RoundResult)
            {
                if ((DataContext as BattleProtocolWrapper).NumberOfCurrentRound == countOfRound)
                {
                    int countOfRoundResult = 0;
                    foreach (FighterRoundResultWrapper fighterRoundResult in roundResult.FighterRoundResult)
                    {
                        if (rowIndex == countOfRoundResult)
                        {
                            uint judgescore;
                            bool success = UInt32.TryParse((sender as TextBox).Text, out judgescore);
                            if (success)
                                fighterRoundResult.JudgeScore[columnIndex] = judgescore;
                            break;
                        }
                        countOfRoundResult++;
                    }
                    break;
                }
                countOfRound++;
            }
        }

        private void addJudgeButton_Click(object sender, RoutedEventArgs e)
        {
            List<Judge> judges = (DataContext as BattleProtocolWrapper).SelectedJudges;
            judges.Add(new Judge());

            foreach (RoundResultWrapper roundResult in (DataContext as BattleProtocolWrapper).RoundResult)
                foreach (FighterRoundResultWrapper fighterRoundResult in roundResult.FighterRoundResult)
                    fighterRoundResult.JudgeScore.Add(0);

            fightProtocolTable.Columns.Insert(judges.Count - 1, GetJudgeColumn());
        }

        private void deleteJudgButtone_Click(object sender, RoutedEventArgs e)
        {
            List<Judge> judges = (DataContext as BattleProtocolWrapper).SelectedJudges;
            if (judges.Count > 1)
            {
                fightProtocolTable.Columns.RemoveAt(judges.Count - 1);

                foreach (RoundResultWrapper roundResult in (DataContext as BattleProtocolWrapper).RoundResult)
                    foreach (FighterRoundResultWrapper fighterRoundResult in roundResult.FighterRoundResult)
                        fighterRoundResult.JudgeScore.RemoveAt(judges.Count - 2);

                judges.RemoveAt(judges.Count - 1);
            }
        }

        private void endTheFightButton_Click(object sender, RoutedEventArgs e)
        {
            BattleWrapper battle = ((object[])this.Tag)[0] as BattleWrapper;
            Label[] labels = ((object[])this.Tag)[1] as Label[];
            if (battle.Winner)
                labels[0].Content = $"П {labels[0].Content}";
            else
                labels[1].Content = $"П {labels[1].Content}";
            this.Close();
        }
    }
}