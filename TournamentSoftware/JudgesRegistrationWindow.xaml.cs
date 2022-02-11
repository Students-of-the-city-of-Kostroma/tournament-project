﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using static TournamentSoftware.ApplicationResourcesPaths;
using static TournamentSoftware.TournamentData;

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
            JudgeWrapper jude = new JudgeWrapper
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
            reagistrator.BackupRegistrationTable(judgesList, judgesBackupPath);
        }

        /// <summary>
        /// Сохранение судей в БД
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveJudgesList(object sender, RoutedEventArgs e)
        {
            isJudgesSaved = true;
            
            for (int i = 0; i < judgesList.Count; i++)
            {
                if (judgesList[i].Surname != "" && judgesList[i].Name != "")
                {
                    Club club = new Club();
                    if (judgesList[i].Club != "" && judgesList[i].City != "")
                    {
                        if (dataBaseHandler.GetClubsData("SELECT * FROM Club WHERE name=\"" + judgesList[i].Club + "\" AND city=\"" + judgesList[i].City + "\";").Count == 0)
                        {
                            club.Name = judgesList[i].Club;
                            club.City = judgesList[i].City;
                            dataBaseHandler.AddClub(club);
                        }
                        club = dataBaseHandler.GetClubsData("SELECT * FROM Club WHERE name=\"" + judgesList[i].Club + "\" AND city=\"" + judgesList[i].City + "\";")[0];
                    }


                    if (dataBaseHandler.GetJudgesData("SELECT * FROM Judge WHERE surname=\"" + judgesList[i].Surname + "\" AND name=\"" + judgesList[i].Name + "\";").Count == 0)
                    {
                        Judge judge = new Judge();
                        judge.Surname = judgesList[i].Surname;
                        judge.Name = judgesList[i].Name;
                        judge.Patronymic = judgesList[i].Patronymic;
                        if (club.Id != 0)
                        {
                            judge.ClubId = club.Id;
                        }
                        dataBaseHandler.AddJudge(judge);
                    }
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            judgesList.Clear();

            List<Judge> judges = dataBaseHandler.GetJudgesData("SELECT * FROM Judge");
            judges.ForEach(judge => {
                JudgeWrapper judgeWrapper = new JudgeWrapper { Name = judge.Name, Surname = judge.Surname, Patronymic = judge.Patronymic };
                List<Club> club = dataBaseHandler.GetClubsData("SELECT * FROM Club WHERE id=" + judge.ClubId + ";");
                if (club.Count != 0)
                {
                    judgeWrapper.Club = club[0].Name;
                    judgeWrapper.City = club[0].City;
                }
                else 
                {
                    judgeWrapper.Club = "";
                    judgeWrapper.City = "";
                }
                judgesList.Add(judgeWrapper);
            });

            JudesTable.Items.Clear();
            JudesTable.ItemsSource = judgesList;
        }
    }
}
