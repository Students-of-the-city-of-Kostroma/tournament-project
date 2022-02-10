using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using static TournamentSoftware.ApplicationResourcesPaths;

namespace TournamentSoftware
{
    /// <summary>
    /// Interaction logic for JudgesRegistrationWindow.xaml
    /// </summary>
    public partial class JudgesRegistrationWindow : Window
    {
        public static ObservableCollection<JudgeWrapper> judgesList = new ObservableCollection<JudgeWrapper>();
        private ParticipantsReagistrator reagistrator = new ParticipantsReagistrator();
        private bool isJudgesSaved = false;
        private int idAddedjudge = 0; //id судьи с которого началось добавление

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
        private void addJude(object sender, RoutedEventArgs e)
        {
            JudgeWrapper jude = new JudgeWrapper
            {
                Name = "",
                Surname = "",
                Patronymic = "",
                Club = "",
                City = "",
            };

            judgesList.Add(jude);
            if (idAddedjudge == 0)
                idAddedjudge = judgesList.Count - 1;
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
            if (idAddedjudge == 0)
            {
                reagistrator.BackupRegistrationTable(judgesList, judgesBackupPath);
                return;
            }
            var result = MessageBox.Show("Вы хотите сохранить изменения?", "Винимание!", MessageBoxButton.YesNoCancel);
            if (result == MessageBoxResult.Yes)
                reagistrator.BackupRegistrationTable(judgesList, judgesBackupPath);
            else if (result == MessageBoxResult.No)
                while (judgesList.Count != idAddedjudge)
                    judgesList.RemoveAt(idAddedjudge);
            else
                e.Cancel = true;
        }

        /// <summary>
        /// Сохранение судей в БД
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveJudgesList(object sender, RoutedEventArgs e)
        {
            idAddedjudge = 0;
            isJudgesSaved = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!isJudgesSaved)
            {
                judgesList.Clear();
                List<JudgeWrapper> judges = reagistrator.GetJudgesFromBackup(judgesBackupPath);
                if (judges != null)
                {
                    foreach (JudgeWrapper j in judges)
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
