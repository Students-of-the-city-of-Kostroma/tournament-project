using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using SQLite;
using Excel = Microsoft.Office.Interop.Excel;
using ExcelDataReader;
using System.IO;

namespace TournamentSoftware
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<Participant> participants_list = new ObservableCollection<Participant>();
        public ObservableCollection<Participant> ParticipantsCollection
        {
            get { return this.participants_list; }
        }
        public MainWindow()
        {
            InitializeComponent();
            appGrid.Visibility = Visibility.Hidden;
            // goTournament.IsEnabled = false;
            participants_list = new ObservableCollection<Participant>();
            registrationTable.DataContext = participants_list;
            registrationTable.CellEditEnding += RegistrationTable_CellEditEnding;
        }

        private void RegistrationTable_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            Console.WriteLine(e.Row.GetIndex());
        }

        private void NumericOnly(System.Object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (!int.TryParse(e.Text, out _)) { e.Handled = false; }
        }

        public static int participantCount = 0;

        /// <summary>
        /// Переход к модулю регистрации
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void goRegistrate(object sender, RoutedEventArgs e)
        {
            startWindowLabel.Visibility = Visibility.Hidden;
            goRegistrateButton.Visibility = Visibility.Hidden;
            appGrid.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// алгоритм добавление участника
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addParticipant(object sender, RoutedEventArgs e)
        {
            participantCount++;

            Participant participant = new Participant()
            {
                Name = "",
                Surname = "",
                Otchestvo = "",
                Psevdonim = "",
                Posevnoy = false,
                Club = "",
                City = "",
                DateOfBirth = -1,
                Height = -1,
                Weight = -1,
                Kategory = "",
                Sex = "",
                CommonRating = -1,
                ClubRating = -1,
                AvailableSex = new string[2] { "М", "Ж" },
                IsSelected = false,
            };

            participants_list.Add(participant);
            registrationTable.ItemsSource = participants_list;
            exportButton.IsEnabled = true;
            // DbConnection.connect<Participant>(participant as object);
        }

        private void deleteParticipant(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Вего участников " + participants_list.Count);
            for (int i = 0; i < participants_list.Count;)
            {
                Console.WriteLine(participants_list[i].IsSelected);
                if (participants_list[i].IsSelected)
                {
                    Console.WriteLine(participants_list[i].Name);
                    participants_list.Remove(participants_list[i]);
                }
                else
                {
                    i++;
                }
            }
            delete.IsEnabled = false;
            selectorAllForDelete_Unchecked(sender, e);
            CheckBox newCheckBox = new CheckBox();
            newCheckBox.IsChecked = false;
        }

        private void selectorAllForDelete_Unchecked(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < participants_list.Count; i++)
            {
                participants_list[i].IsSelected = false;
            }
            for (int i = 0; i < participants_list.Count; i++)
            {
                Console.WriteLine(participants_list[i].IsSelected);
            }
            delete.IsEnabled = false;
            DataGridColumn newCol = checkboxCol;
            registrationTable.Columns.Remove(checkboxCol);
            registrationTable.Columns.Insert(0, newCol);

        }

        private void selectorAllForDelete_Checked(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < participants_list.Count; i++)
            {
                participants_list[i].IsSelected = true;
            }
            for (int i = 0; i < participants_list.Count; i++)
            {
                Console.WriteLine(participants_list[i].IsSelected);
            }
            delete.IsEnabled = true;
        }

        private void mainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            participants_list = new ObservableCollection<Participant>();
            registrationTable.DataContext = participants_list;
        }

        /// <summary>
        /// событие при чеке чекбокса участника
        /// разблокируем кнопку удалить
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void participantChecked(object sender, RoutedEventArgs e)
        {
            delete.IsEnabled = true;
        }

        /// <summary>
        /// событие при анчеке чекбокса участника
        /// если больше нет выбранных участников - блокируем кнопку удалить
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void participantUnchecked(object sender, RoutedEventArgs e)
        {
            int selectedCount = 0;
            for (int i = 0; i < participants_list.Count; i++)
            {
                if (participants_list[i].IsSelected)
                {
                    selectedCount++;
                }
            }

            if (selectedCount == 0)
            {
                delete.IsEnabled = false;
            }
        }

        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                MessageBox.Show("Exception Occured while releasing object " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
        }


        /// <summary>
        /// Экспортируем тиблицу регистрации в файл
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportRegistrationTable(object sender, RoutedEventArgs e)
        {
            Excel.Application xlApp;
            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet;
            object misValue = System.Reflection.Missing.Value;

            xlApp = new Excel.Application();
            xlWorkBook = xlApp.Workbooks.Add(misValue);
            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            int i = 0;
            int j = 0;

            // registrationTable.Items.Count - количество строк
            for (i = 0; i <= registrationTable.Items.Count - 1; i++)
            {
                for (j = 0; j <= registrationTable.Columns.Count - 1; j++)
                {
                    DataGridCell cell = (DataGridCell)registrationTable.Columns[i].GetCellContent(j);
                    xlWorkSheet.Cells[i + 1, j + 1] = cell;
                }
            }

            xlWorkBook.SaveAs("D:\\csharp.net-informations.xls", Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
            xlWorkBook.Close(true, misValue, misValue);
            xlApp.Quit();

            releaseObject(xlWorkSheet);
            releaseObject(xlWorkBook);
            releaseObject(xlApp);
        }

        /// <summary>
        /// Открываем инструменты для редактирования лейаута модуля регистрации
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void showRegistrationModuleSettings(object sender, RoutedEventArgs e)
        {
            Window1 settings = new Window1();
            settings.Show();
        }

        /// <summary>
        /// Выбираем файл с таблицей
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenExcel_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = new MessageBoxResult();
            if (participants_list.Count > 0) {
                result = MessageBox.Show("Все предыдущие записи в таблице регистрации будут удалены. Вы хотите продолжить?", "Предупреждение",
               MessageBoxButton.OKCancel,
               MessageBoxImage.Warning, MessageBoxResult.Cancel);
            }
           
            if (participants_list.Count == 0 || MessageBoxResult.OK == result)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "EXCEL Files (*.xlsx)|*.xlsx|EXCEL Files 2003 (*.xls)|*.xls|All files (*.*)|*.*";
                if (openFileDialog.ShowDialog() != true)
                    return;

                if (!checkTableHeadersValid(openFileDialog.FileName))
                {
                    MessageBox.Show("Не удалось прочитать таблицу! Попробуйте загрузить другой файл", "Ошибка");
                }
            }
        }

        private bool checkTableHeadersValid(string fileName)
        {
            FileStream stream = File.Open(fileName, FileMode.Open, FileAccess.Read);
            var reader = ExcelReaderFactory.CreateReader(stream);
            DataSet dataSet = reader.AsDataSet();
            var dataTable = dataSet.Tables[0];
            DataColumnCollection loadedColumns = dataTable.Columns;
            int requredColumnExists = 0;
            ObservableCollection<DataGridColumn> validationColumnsSet = registrationTable.Columns;
            DataRowCollection rows = dataTable.Rows;
            for (int i = 1; i < validationColumnsSet.Count; i++)
            {
                for (int j = 0; j < loadedColumns.Count; j++)
                {
                    if (rows[0].ItemArray[j].Equals(validationColumnsSet[i].Header))
                    {
                        requredColumnExists++;
                        break;
                    }
                }
            }

            if (requredColumnExists == validationColumnsSet.Count - 1)
            {
                participants_list.Clear();
                for (int i = 1; i < dataTable.Rows.Count; i++)
                {
                    DataRow row = dataTable.Rows[i];
                    Participant newParticipant = new Participant();
                    for (int j = 0; j < dataTable.Columns.Count; j++)
                    {
                        if (dataTable.Rows[0].ItemArray[j].Equals("Имя"))
                        {
                            newParticipant.Name = row.ItemArray[j].ToString();
                        }

                        if (dataTable.Rows[0].ItemArray[j].Equals("Фамилия"))
                        {
                            newParticipant.Surname = row.ItemArray[j].ToString();
                        }

                        if (dataTable.Rows[0].ItemArray[j].Equals("Отчество"))
                        {
                            newParticipant.Otchestvo = row.ItemArray[j].ToString();
                        }

                        if (dataTable.Rows[0].ItemArray[j].Equals("Псевдоним"))
                        {
                            newParticipant.Psevdonim = row.ItemArray[j].ToString();
                        }

                        if (dataTable.Rows[0].ItemArray[j].Equals("Клуб"))
                        {
                            newParticipant.Club = row.ItemArray[j].ToString();
                        }

                        if (dataTable.Rows[0].ItemArray[j].Equals("Город"))
                        {
                            newParticipant.City = row.ItemArray[j].ToString();
                        }

                        if (dataTable.Rows[0].ItemArray[j].Equals("Посевной"))
                        {
                            if (bool.TryParse(row.ItemArray[j].ToString(), out _))
                            {
                                newParticipant.Posevnoy = bool.Parse(row.ItemArray[j].ToString());
                            }
                            else
                            {
                                newParticipant.Posevnoy = false;
                            }
                        }

                        if (dataTable.Rows[0].ItemArray[j].Equals("Пол"))
                        {
                            if (row.ItemArray[j].ToString().Equals("М") || row.ItemArray[j].ToString().Equals("Ж"))
                            {
                                newParticipant.Sex = row.ItemArray[j].ToString();
                            }
                        }

                        if (dataTable.Rows[0].ItemArray[j].Equals("Год рождения"))
                        {
                            if (int.TryParse(row.ItemArray[j].ToString(), out _))
                            {
                                int year = int.Parse(row.ItemArray[j].ToString());
                                if (year > 1900)
                                {
                                    newParticipant.DateOfBirth = year;
                                }
                            }
                        }

                        if (dataTable.Rows[0].ItemArray[j].Equals("Рост"))
                        {
                            if (int.TryParse(row.ItemArray[j].ToString(), out _))
                            {
                                int height = int.Parse(row.ItemArray[j].ToString());
                                if (height > 100)
                                {
                                    newParticipant.Height = height;
                                }
                            }
                        }

                        if (dataTable.Rows[0].ItemArray[j].Equals("Вес"))
                        {
                            if (int.TryParse(row.ItemArray[j].ToString(), out _))
                            {
                                int weight = int.Parse(row.ItemArray[j].ToString());
                                if (weight > 10)
                                {
                                    newParticipant.Weight = weight;
                                }
                            }
                        }

                        if (dataTable.Rows[0].ItemArray[j].Equals("Рейтинг (общий)"))
                        {
                            if (int.TryParse(row.ItemArray[j].ToString(), out _))
                            {
                                int raiting = int.Parse(row.ItemArray[j].ToString());
                                newParticipant.CommonRating = raiting;
                            }
                        }

                        if (dataTable.Rows[0].ItemArray[j].Equals("Рейтинг (клубный)"))
                        {
                            if (int.TryParse(row.ItemArray[j].ToString(), out _))
                            {
                                int raiting = int.Parse(row.ItemArray[j].ToString());
                                newParticipant.ClubRating = raiting;
                            }
                        }
                    }
                    participants_list.Add(newParticipant);
                    registrationTable.ItemsSource = participants_list;
                }
                stream.Close();
                reader.Close();
                return true;
            }
            else
            {
                Console.WriteLine("not valid");
            }

            return false;
        }
    }

    public class DbConnection : Window
    {
        public static void connect<T>(object obj)
        {

            using (SQLiteConnection connection = new SQLiteConnection("db.db"))
            {
                connection.CreateTable<T>();
                connection.Insert(obj);
            }

        }
    }
}
