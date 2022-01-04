using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace TournamentSoftware
{
    /// <summary>
    /// Interaction logic for JudgesRegistrationWindow.xaml
    /// </summary>
    public partial class JudgesRegistrationWindow : Window
    {
        public static ObservableCollection<Judge> judgesList = new ObservableCollection<Judge>();
        private ParticipantsReagistrator reagistrator = new ParticipantsReagistrator();
        private string judgesBackupPath = "judgesBackup.json";
        private bool isJudgesSaved = false;

        public bool JudesSaved
        {
            get { return isJudgesSaved; }
            set { isJudgesSaved = value; }
        }

        private bool isPanelOpen = true;
        public JudgesRegistrationWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Добавление нового судьи
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addJude (object sender, RoutedEventArgs e)
        {
            Judge jude = new Judge
            {
                Name = "",
                Surname = "",
                Patronymic = "",
                Club = "",
                City = "",
            };

            judgesList.Add(jude);
        }

        /// <summary>
        /// Скрытие панели инструментов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (isPanelOpen)
            {
                mainGrid.ColumnDefinitions[1].Width = new GridLength(40, GridUnitType.Pixel);
                addJudgeButton.Visibility = Visibility.Hidden;
                isPanelOpen = false;
            }
            else {
                mainGrid.ColumnDefinitions[1].Width = new GridLength(100, GridUnitType.Pixel);
                addJudgeButton.Visibility = Visibility.Visible;
                isPanelOpen = true;
            }
        }

        /// <summary>
        /// При закрытии окна - запись судей в файл
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            reagistrator.backupRegistrationTable(judgesList, judgesBackupPath);
        }

        /// <summary>
        /// Сохранение судей в БД
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveJudgesList(object sender, RoutedEventArgs e)
        {
            isJudgesSaved = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!isJudgesSaved)
            {
                judgesList.Clear();
                List<Judge> judges = reagistrator.getJudgesFromBackup(judgesBackupPath);
                if (judges != null)
                {
                    foreach (Judge j in judges)
                    {
                        judgesList.Add(j);
                    }
                }
            }
            JudesTable.Items.Clear();
            JudesTable.ItemsSource = judgesList;
        }
    }
}
