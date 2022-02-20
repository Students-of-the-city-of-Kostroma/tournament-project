using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using Excel = Microsoft.Office.Interop.Excel;
using System.Data;
using ExcelDataReader;
using System.Collections.ObjectModel;
using static TournamentSoftware.TournamentData;

namespace TournamentSoftware
{
    public class ParticipantsReagistrator
    {
        /// <summary>
        /// Записываем информацию о всех участнниках в файл
        /// </summary>
        public void BackupRegistrationTable<T>(ObservableCollection<T> elements, string path)
        {
            List<T> participantsArray = new List<T>();
            foreach (T participant in elements)
            {
                participantsArray.Add(participant);
            }
            string participantsArrayJson = JsonConvert.SerializeObject(participantsArray);
            File.WriteAllText(path, participantsArrayJson);
        }

        public void SaveFile(DataTable table)
        {
            SaveFileDialog SaveFileDialog = new SaveFileDialog();

            SaveFileDialog.Filter = "Файлы Excel (*.xlsx) | *.xlsx";

            bool? result = SaveFileDialog.ShowDialog();
            if (result == true)
            {
                string path = Path.GetFullPath(SaveFileDialog.FileName);
                SaveFileDialog_FileOk(path, table);
            }
        }

        private void SaveFileDialog_FileOk(string path, DataTable table)
        {
            Excel.Application excelapp = new Excel.Application();

            Excel.Workbook workbook = excelapp.Workbooks.Add(Type.Missing);

            Excel.Worksheet worksheet = workbook.ActiveSheet;

            // Header row
            for (int Idx = 0; Idx < table.Columns.Count; Idx++)
            {
                worksheet.Range["A1"].Offset[0, Idx].Value = table.Columns[Idx].Caption;
            }

            // Data Rows
            for (int Idx = 0; Idx < table.Rows.Count; Idx++)
            {
                worksheet.Range["A2"].Offset[Idx].Resize[1, table.Columns.Count].Value = table.Rows[Idx].ItemArray;
            }

            excelapp.AlertBeforeOverwriting = false;
            workbook.SaveAs(path);
            workbook.Close();
            excelapp.Quit();
        }

        public List<ParticipantWrapper> GetParticipantsFromBackup(string path)
        {
            StreamReader reader = new StreamReader(path);
            string json = reader.ReadToEnd();
            var participants = JsonConvert.DeserializeObject<List<ParticipantWrapper>>(json);
            reader.Close();
            return participants;
        }

        public List<JudgeWrapper> GetJudgesFromBackup(string path)
        {
            var judges = new List<JudgeWrapper>();
            if (!File.Exists(path))
                return judges;

            StreamReader reader = new StreamReader(path);
            string json = reader.ReadToEnd();
            judges = JsonConvert.DeserializeObject<List<JudgeWrapper>>(json);
            reader.Close();
            return judges;
        }

        public void LoadParticipantsFromFile(List<string> requiredColumnHeaders)
        {
            MessageBoxResult result = new MessageBoxResult();
            if (participants.Count > 0)
            {
               result = MessageBox.Show("Все предыдущие записи в таблице регистрации будут удалены. Вы хотите продолжить?",
                   "Предупреждение",
               MessageBoxButton.OKCancel,
               MessageBoxImage.Warning, MessageBoxResult.Cancel);
            }

            if (participants.Count == 0 || MessageBoxResult.OK == result)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "EXCEL Files (*.xlsx)|*.xlsx|EXCEL Files 2003 (*.xls)|*.xls|All files (*.*)|*.*";
                if (openFileDialog.ShowDialog() != true)
                    return;

                if (!CheckTableHeadersValid(openFileDialog.FileName, requiredColumnHeaders))
                {
                    MessageBox.Show("Не удалось прочитать таблицу! Попробуйте загрузить другой файл", "Ошибка");
                }
            }
        }
        private bool CheckTableHeadersValid(string fileName, List<string> requiredColumnHeaders)
        {
            nominations.Clear();
            FileStream stream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var reader = ExcelReaderFactory.CreateReader(stream);
            DataSet dataSet = reader.AsDataSet();
            var dataTable = dataSet.Tables[0];

            reader.Close();
            stream.Close();

            // колонки из подгружвемого файла
            DataColumnCollection loadedColumns = dataTable.Columns;

            // здесь сохрянятся номинации
            List<int> loadedNominationsIndexes = new List<int>();

            int requredColumnExists = 0;

            // строки из подгружаемого файла
            DataRowCollection loadedRows = dataTable.Rows;
            // идем по загруженным столбцам и смотрим - это обязательный столбец или номинация
            for (int i = 0; i < loadedColumns.Count; i++)
            {
                for (int j = 0; j < requiredColumnHeaders.Count; j++)
                {
                    // нашли обязательный столбец
                    if (loadedRows[0].ItemArray[i].Equals(requiredColumnHeaders[j]))
                    {
                        requredColumnExists++;
                        break;
                    }
                    else
                    {
                        if (j == requiredColumnHeaders.Count-1)
                        {
                            loadedNominationsIndexes.Add(i);
                        }
                    }
                }
            }
            // если нашлись все обязательные столбцы
            if (requredColumnExists == requiredColumnHeaders.Count)
            {
                participants.Clear();
                
                foreach (int i in loadedNominationsIndexes)
                {
                    string nominationName = loadedRows[0].ItemArray[i].ToString();
                    NominationWrapper nomination = new NominationWrapper(nominationName);
                    nominations.Add(nomination);
                }

                GetParticipants(loadedColumns, loadedRows, loadedNominationsIndexes);
                return true;
            }

            return false;
        }

        private void GetParticipants(
            DataColumnCollection loadedColumns, 
            DataRowCollection loadedRows, 
            List<int> loadedNominationsIndexes)
        {
            // идем по строкам
            for (int i = 1; i < loadedRows.Count; i++)
            {
                DataRow row = loadedRows[i];
                ParticipantWrapper newParticipant = new ParticipantWrapper();

                // идем по столбцам
                for (int j = 0; j < loadedColumns.Count; j++)
                {
                    // если это номинация
                    if (loadedNominationsIndexes.Contains(j))
                    {
                        if (bool.TryParse(row.ItemArray[j].ToString(), out _))
                        {
                            newParticipant.Nominations.Add(
                                loadedRows[0].ItemArray[j].ToString(),
                                bool.Parse(row.ItemArray[j].ToString())
                                );
                        }
                        else
                        {
                            newParticipant.Nominations.Add(
                               loadedRows[0].ItemArray[j].ToString(),
                               false);
                        }
                    }
                    else
                    {
                        if (loadedRows[0].ItemArray[j].Equals("Имя"))
                        {
                            newParticipant.Participant.Name = row.ItemArray[j].ToString();
                        }

                        if (loadedRows[0].ItemArray[j].Equals("Фамилия"))
                        {
                            newParticipant.Participant.Surname = row.ItemArray[j].ToString();
                        }

                        if (loadedRows[0].ItemArray[j].Equals("Отчество"))
                        {
                            newParticipant.Participant.Patronymic = row.ItemArray[j].ToString();
                        }

                        if (loadedRows[0].ItemArray[j].Equals("Псевдоним"))
                        {
                            newParticipant.Participant.Pseudonym = row.ItemArray[j].ToString();
                        }

                        if (loadedRows[0].ItemArray[j].Equals("Клуб"))
                        {
                            newParticipant.Club = row.ItemArray[j].ToString();
                        }

                        if (loadedRows[0].ItemArray[j].Equals("Город"))
                        {
                            newParticipant.City = row.ItemArray[j].ToString();
                        }

                        if (loadedRows[0].ItemArray[j].Equals("Посевной"))
                        {
                            if (bool.TryParse(row.ItemArray[j].ToString(), out _))
                            {
                                newParticipant.Participant.Leader = bool.Parse(row.ItemArray[j].ToString());
                            }
                            else
                            {
                                newParticipant.Participant.Leader = false;
                            }
                        }

                        if (loadedRows[0].ItemArray[j].Equals("Пол"))
                        {
                            if (row.ItemArray[j].ToString().Equals("М") || row.ItemArray[j].ToString().Equals("Ж"))
                            {
                                newParticipant.Participant.Sex = row.ItemArray[j].ToString();
                            }
                        }

                        if (loadedRows[0].ItemArray[j].Equals("Год рождения"))
                        {
                            if (int.TryParse(row.ItemArray[j].ToString(), out _))
                            {
                                int year = int.Parse(row.ItemArray[j].ToString());
                                if (year > 1900)
                                {
                                    newParticipant.Participant.DateOfBirth = year;
                                }
                            }
                        }

                        if (loadedRows[0].ItemArray[j].Equals("Категория"))
                        {
                            newParticipant.Category = row.ItemArray[j].ToString();
                        }

                        if (loadedRows[0].ItemArray[j].Equals("Рост"))
                        {
                            if (int.TryParse(row.ItemArray[j].ToString(), out _))
                            {
                                int height = int.Parse(row.ItemArray[j].ToString());
                                if (height > 100)
                                {
                                    newParticipant.Participant.Height = (uint)height;
                                }
                            }
                        }

                        if (loadedRows[0].ItemArray[j].Equals("Вес"))
                        {
                            if (int.TryParse(row.ItemArray[j].ToString(), out _))
                            {
                                int weight = int.Parse(row.ItemArray[j].ToString());
                                if (weight > 10)
                                {
                                    newParticipant.Participant.Weight = (uint)weight;
                                }
                            }
                        }

                        if (loadedRows[0].ItemArray[j].Equals("Рейтинг (общий)"))
                        {
                            if (int.TryParse(row.ItemArray[j].ToString(), out _))
                            {
                                int raiting = int.Parse(row.ItemArray[j].ToString());
                                newParticipant.Participant.CommonRating = (uint)raiting;
                            }
                        }

                        if (loadedRows[0].ItemArray[j].Equals("Рейтинг (клубный)"))
                        {
                            if (int.TryParse(row.ItemArray[j].ToString(), out _))
                            {
                                int raiting = int.Parse(row.ItemArray[j].ToString());
                                newParticipant.Participant.ClubRating = (uint)raiting;
                            }
                        }
                    }
                }

                participants.Add(newParticipant);
            }
        }
    }
}
