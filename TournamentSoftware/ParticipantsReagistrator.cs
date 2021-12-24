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

namespace TournamentSoftware
{
    public class ParticipantsReagistrator
    {
        public List<string> nominationsNames = new List<string>();
        /// <summary>
        /// Записываем информацию о всех участнниках в файл
        /// </summary>
        public void backupRegistrationTable<T>(ObservableCollection<T> elements, string path)
        {
            List<T> participantsArray = new List<T>();
            foreach (T participant in elements)
            {
                participantsArray.Add(participant);
            }
            string participantsArrayJson = JsonConvert.SerializeObject(participantsArray);
            File.WriteAllText(path, participantsArrayJson);
        }

        public void saveFile(DataTable table)
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
                for (int i = 0; i < table.Rows[Idx].ItemArray.Length; i++)
                {
                    Console.WriteLine(i + " " + table.Rows[Idx].ItemArray[i]);
                }
                worksheet.Range["A2"].Offset[Idx].Resize[1, table.Columns.Count].Value = table.Rows[Idx].ItemArray;
            }

            excelapp.AlertBeforeOverwriting = false;
            workbook.SaveAs(path);
            workbook.Close();
            excelapp.Quit();
        }

        public List<ParticipantFormModel> getParticipantsFromBackup(string path)
        {
            StreamReader reader = new StreamReader(path);
            string json = reader.ReadToEnd();
            var participants = JsonConvert.DeserializeObject<List<ParticipantFormModel>>(json);
            reader.Close();
            return participants;
        }

        public List<Judge> getJudgesFromBackup(string path)
        {
            var judges = new List<Judge>();
            if (File.Exists(path))
            {
                StreamReader reader = new StreamReader(path);
                string json = reader.ReadToEnd();
                judges = JsonConvert.DeserializeObject<List<Judge>>(json);
                reader.Close();
            }
            return judges;
        }

        public void loadParticipantsFromFile(List<string> requiredColumnHeaders)
        {
            MessageBoxResult result = new MessageBoxResult();
            if (MainWindow.participantsList.Count > 0)
            {
                result = MessageBox.Show("Все предыдущие записи в таблице регистрации будут удалены. Вы хотите продолжить?", "Предупреждение",
               MessageBoxButton.OKCancel,
               MessageBoxImage.Warning, MessageBoxResult.Cancel);
            }

            if (MainWindow.participantsList.Count == 0 || MessageBoxResult.OK == result)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "EXCEL Files (*.xlsx)|*.xlsx|EXCEL Files 2003 (*.xls)|*.xls|All files (*.*)|*.*";
                if (openFileDialog.ShowDialog() != true)
                    return;

                if (!checkTableHeadersValid(openFileDialog.FileName, requiredColumnHeaders))
                {
                    MessageBox.Show("Не удалось прочитать таблицу! Попробуйте загрузить другой файл", "Ошибка");
                }
            }
        }
        private bool checkTableHeadersValid(string fileName, List<string> requiredColumnHeaders)
        {
            FileStream stream = File.Open(fileName, FileMode.Open, FileAccess.Read);
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
                MainWindow.participantsList.Clear();
                
                foreach (int i in loadedNominationsIndexes)
                {
                    string nominationName = loadedRows[0].ItemArray[i].ToString();
                    nominationsNames.Add(nominationName);
                }

                getParticipants(loadedColumns, loadedRows, loadedNominationsIndexes);
                return true;
            }

            return false;
        }

        private void getParticipants(
            DataColumnCollection loadedColumns, 
            DataRowCollection loadedRows, 
            List<int> loadedNominationsIndexes)
        {
            // идем по строкам
            for (int i = 1; i < loadedRows.Count; i++)
            {
                DataRow row = loadedRows[i];
                ParticipantFormModel newParticipant = new ParticipantFormModel();

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
                            newParticipant.Kategory = row.ItemArray[j].ToString();
                        }

                        if (loadedRows[0].ItemArray[j].Equals("Рост"))
                        {
                            if (int.TryParse(row.ItemArray[j].ToString(), out _))
                            {
                                int height = int.Parse(row.ItemArray[j].ToString());
                                if (height > 100)
                                {
                                    newParticipant.Participant.Height = height;
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
                                    newParticipant.Participant.Weight = weight;
                                }
                            }
                        }

                        if (loadedRows[0].ItemArray[j].Equals("Рейтинг (общий)"))
                        {
                            if (int.TryParse(row.ItemArray[j].ToString(), out _))
                            {
                                int raiting = int.Parse(row.ItemArray[j].ToString());
                                newParticipant.Participant.CommonRating = raiting;
                            }
                        }

                        if (loadedRows[0].ItemArray[j].Equals("Рейтинг (клубный)"))
                        {
                            if (int.TryParse(row.ItemArray[j].ToString(), out _))
                            {
                                int raiting = int.Parse(row.ItemArray[j].ToString());
                                newParticipant.Participant.ClubRating = raiting;
                            }
                        }
                    }
                }

                MainWindow.participantsList.Add(newParticipant);
            }
        }
    }
}
