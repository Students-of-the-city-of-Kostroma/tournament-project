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
using System.Windows.Input;
using System.Collections;
using System.Windows.Media;
using System.Windows.Data;

namespace TournamentSoftware
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<Participant> participantsList = new ObservableCollection<Participant>();

        public ObservableCollection<Nomination> nominationsList = new ObservableCollection<Nomination>();
        public ObservableCollection<DataGridTemplateColumn> nominationsColumn = new ObservableCollection<DataGridTemplateColumn>();
        public MainWindow()
        {
            InitializeComponent();
            appGrid.Visibility = Visibility.Hidden;
            participantsList = new ObservableCollection<Participant>();
            registrationTable.DataContext = participantsList;
            registrationTable.CellEditEnding += RegistrationTable_CellEditEnding;
            Console.WriteLine("dgah");


        }

        private void ParticipantsList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
        }

        private void RegistrationTable_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            Console.WriteLine(e.Row.GetIndex());
        }

        private void NumericOnly(object sender, TextCompositionEventArgs e)
        {
            if (!int.TryParse(e.Text, out _))
            {
                e.Handled = true;
            }
            else
            {
                if (((TextBox)e.Source).Text.Equals("0"))
                {
                    ((TextBox)e.Source).Text = "";
                }
            }
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
                DateOfBirth = 0,
                Height = 0,
                Weight = 0,
                Kategory = "",
                Sex = "",
                CommonRating = 0,
                ClubRating = 0,
                AvailableSex = new string[2] { "М", "Ж" },
                IsSelected = false,
            };

            participantsList.Add(participant);
            registrationTable.ItemsSource = participantsList;
            exportButton.IsEnabled = true;
        }

        /// <summary>
        /// Удаление отмеченных участников
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteParticipant(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < participantsList.Count;)
            {
                if (participantsList[i].IsSelected)
                {
                    participantsList.Remove(participantsList[i]);
                }
                else
                {
                    i++;
                }
            }

            // если удалены все участники удаляем колонки с номинациями
            if (participantsList.Count == 0)
            {
                foreach (DataGridTemplateColumn column in nominationsColumn)
                {
                    registrationTable.Columns.Remove(column);
                }
            }
            delete.IsEnabled = false;
            selectorAllForDelete_Unchecked(sender, e);
            CheckBox newCheckBox = new CheckBox();
            newCheckBox.IsChecked = false;
            if (participantsList.Count == 0)
            {
                exportButton.IsEnabled = false;
            }
        }

        /// <summary>
        /// снятие галочек со всех участников
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selectorAllForDelete_Unchecked(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < participantsList.Count; i++)
            {
                participantsList[i].IsSelected = false;
            }
            for (int i = 0; i < participantsList.Count; i++)
            {
                Console.WriteLine(participantsList[i].IsSelected);
            }
            delete.IsEnabled = false;
            DataGridColumn newCol = checkboxCol;
            registrationTable.Columns.Remove(checkboxCol);
            registrationTable.Columns.Insert(0, newCol);

        }

        /// <summary>
        /// выделение всех участников для удаления
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selectorAllForDelete_Checked(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < participantsList.Count; i++)
            {
                participantsList[i].IsSelected = true;
            }
            for (int i = 0; i < participantsList.Count; i++)
            {
                Console.WriteLine(participantsList[i].IsSelected);
            }
            delete.IsEnabled = true;
        }

        private void mainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            participantsList = new ObservableCollection<Participant>();
            registrationTable.DataContext = participantsList;
            participantsList.CollectionChanged += ParticipantsList_CollectionChanged;
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
            for (int i = 0; i < participantsList.Count; i++)
            {
                if (participantsList[i].IsSelected)
                {
                    selectedCount++;
                }
            }

            if (selectedCount == 0)
            {
                delete.IsEnabled = false;
            }
        }


        /// <summary>
        /// Экспортируем тиблицу регистрации в файл
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveFile(object sender, RoutedEventArgs e)
        {
            SaveFileDialog SaveFileDialog = new SaveFileDialog();

            SaveFileDialog.Filter = "Файлы Excel (*.xls; *.xlsx) | *.xls; *.xlsx";

            bool? result = SaveFileDialog.ShowDialog();
            if (result == true)
            {
                string path = Path.GetFullPath(SaveFileDialog.FileName);
                SaveFileDialog_FileOk(path);
            }
        }

        private DataTable ToDataTable(ObservableCollection<Participant> participants)
        {
            DataTable dataTable = new DataTable();
            List<string> nominationsArray = new List<string>();
            for (int i = 0; i < nominationsColumn.Count; i++)
            {
                nominationsArray.Add(nominationsColumn[i].Header.ToString());
            }

            for (int i = 1; i < registrationTable.Columns.Count; i++)
            {
                dataTable.Columns.Add();
                string header = registrationTable.Columns[i].Header.ToString();
                Console.WriteLine(registrationTable.Columns[i].Header.ToString());
                dataTable.Columns[i - 1].Caption = header;
            }
            foreach (Participant participant in participants)
            {
                DataRow row = dataTable.NewRow();
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    string columnHeader = dataTable.Columns[i].Caption;
                    if (nominationsArray.Contains(columnHeader))
                    {
                        row[i] = participant.Nominations[columnHeader];
                    }
                    else
                    {
                        switch (columnHeader)
                        {
                            case "Имя":
                                row[i] = participant.Name;
                                break;
                            case "Фамилия":
                                row[i] = participant.Surname;
                                break;
                            case "Отчество":
                                row[i] = participant.Otchestvo;
                                break;
                            case "Псевдоним":
                                row[i] = participant.Psevdonim;
                                break;
                            case "Посевной":
                                row[i] = participant.Posevnoy;
                                break;
                            case "Пол":
                                row[i] = participant.Sex;
                                break;
                            case "Год рождения":
                                row[i] = participant.DateOfBirth.ToString();
                                break;
                            case "Клуб":
                                row[i] = participant.Club;
                                break;
                            case "Город":
                                row[i] = participant.City;
                                break;
                            case "Рост":
                                row[i] = participant.Height;
                                break;
                            case "Вес":
                                row[i] = participant.Weight;
                                break;
                            case "Категория":
                                row[i] = participant.Kategory;
                                break;
                            case "Рейтинг (общий)":
                                row[i] = participant.CommonRating;
                                break;
                            case "Рейтинг (клубный)":
                                row[i] = participant.ClubRating;
                                break;
                        }
                    }
                }
                dataTable.Rows.Add(row);
            }
            return dataTable;
        }

        private void SaveFileDialog_FileOk(string path)
        {


            Excel.Application excelapp = new Excel.Application();


            Excel.Workbook workbook = excelapp.Workbooks.Add(Type.Missing);

            Excel.Worksheet worksheet = workbook.ActiveSheet;

            DataTable table = ToDataTable(participantsList);

            // Header row
            for (int Idx = 0; Idx < table.Columns.Count; Idx++)
            {
                worksheet.Range["A1"].Offset[0, Idx].Value = table.Columns[Idx].Caption;
            }

            // Data Rows
            for (int Idx = 0; Idx < table.Rows.Count; Idx++)
            {
                // Console.WriteLine("Row " + Idx);
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

        public IEnumerable<DataGridRow> GetDataGridRows(DataGrid grid)
        {
            var itemsSource = grid.ItemsSource as IEnumerable;
            if (null == itemsSource) yield return null;
            foreach (var item in itemsSource)
            {
                var row = grid.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
                if (null != row) yield return row;
            }
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
            if (participantsList.Count > 0)
            {
                result = MessageBox.Show("Все предыдущие записи в таблице регистрации будут удалены. Вы хотите продолжить?", "Предупреждение",
               MessageBoxButton.OKCancel,
               MessageBoxImage.Warning, MessageBoxResult.Cancel);
            }

            if (participantsList.Count == 0 || MessageBoxResult.OK == result)
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
            // колонки из подгружвемого файла
            DataColumnCollection loadedColumns = dataTable.Columns;

            // здесь сохрянятся номинации
            List<int> loadedNominationsIndexes = new List<int>();

            int requredColumnExists = 0;
            // перед этим - удалить номинации!
            // ..

            // колонки из таблицы регистрации (все кроме названия номинаций)
            ObservableCollection<DataGridColumn> validationColumnsSet = registrationTable.Columns;
            // строки из подгружаемого файла
            DataRowCollection loadedRows = dataTable.Rows;
            // идем по загруженным столбцам и смотрим - это обязательный столбец или номинация
            for (int i = 0; i < loadedColumns.Count; i++)
            {
                Console.WriteLine(loadedRows[0].ItemArray[i]);
                for (int j = 1; j < validationColumnsSet.Count; j++)
                {
                    // нашли обязательный столбец
                    if (loadedRows[0].ItemArray[i].Equals(validationColumnsSet[j].Header))
                    {
                        requredColumnExists++;
                        break;
                    }
                    else
                    {
                        if (j == validationColumnsSet.Count - 1)
                        {
                            loadedNominationsIndexes.Add(i);
                        }
                    }
                }
            }
            Console.WriteLine(loadedNominationsIndexes.Count);
            // если нашлись все обязательные столбцы
            if (requredColumnExists == validationColumnsSet.Count - 1)
            {
                participantsList.Clear();
                // добавляем столбцы с номинациями в таблицу
                foreach (int i in loadedNominationsIndexes)
                {
                    string nominationName = loadedRows[0].ItemArray[i].ToString();
                    Binding bind = new Binding("Nominations[" + nominationName + "]");
                    bind.Mode = BindingMode.TwoWay;
                    DataGridTemplateColumn n = new DataGridTemplateColumn();
                    n.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
                    n.Header = nominationName;
                    FrameworkElementFactory checkBox = new FrameworkElementFactory(typeof(CheckBox));
                    checkBox.SetBinding(CheckBox.IsCheckedProperty, bind);
                    DataTemplate checkBoxTemplate = new DataTemplate();
                    checkBoxTemplate.VisualTree = checkBox;
                    n.CellTemplate = checkBoxTemplate;
                    registrationTable.Columns.Add(n);
                    nominationsColumn.Add(n);
                }

                // идем по строкам
                for (int i = 1; i < loadedRows.Count; i++)
                {
                    DataRow row = dataTable.Rows[i];
                    Participant newParticipant = new Participant();

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
                                newParticipant.Name = row.ItemArray[j].ToString();
                            }

                            if (loadedRows[0].ItemArray[j].Equals("Фамилия"))
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

                            if (dataTable.Rows[0].ItemArray[j].Equals("Категория"))
                            {
                                newParticipant.Kategory = row.ItemArray[j].ToString();
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

                            if (loadedRows[0].ItemArray[j].Equals("Рейтинг (клубный)"))
                            {
                                if (int.TryParse(row.ItemArray[j].ToString(), out _))
                                {
                                    int raiting = int.Parse(row.ItemArray[j].ToString());
                                    newParticipant.ClubRating = raiting;
                                }
                            }
                        }
                    }

                    participantsList.Add(newParticipant);
                    registrationTable.ItemsSource = participantsList;
                }
                stream.Close();
                reader.Close();
                exportButton.IsEnabled = true;
                return true;
            }
            else
            {
                Console.WriteLine("not valid");
            }

            return false;
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("in focus Рейтинг (общий) " + ((TextBox)e.Source).Text);
        }

        private void ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (((ComboBox)e.Source).SelectedItem != "M" && ((ComboBox)e.Source).SelectedItem != "Ж")
            {
                SolidColorBrush scb = new SolidColorBrush(Color.FromRgb(255, 221, 219));
                ((ComboBox)e.Source).Background = scb;
            }
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
