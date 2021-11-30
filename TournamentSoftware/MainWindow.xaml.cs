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

namespace TournamentSoftware
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<Participant> participantsList = new ObservableCollection<Participant>();
        public ObservableCollection<Participant> ParticipantsCollection
        {
            get { return this.participantsList; }
        }
        public ObservableCollection<Nomination> nominationsList = new ObservableCollection<Nomination>();
        public ObservableCollection<DataGridCheckBoxColumn> nominationsColumn = new ObservableCollection<DataGridCheckBoxColumn>();
        public MainWindow()
        {
            InitializeComponent();
            appGrid.Visibility = Visibility.Hidden;
            participantsList = new ObservableCollection<Participant>();
            registrationTable.DataContext = participantsList;
            registrationTable.CellEditEnding += RegistrationTable_CellEditEnding;

        }

        private void ParticipantsList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Console.WriteLine("информация об участниках изменена");
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
                Console.WriteLine(((TextBox)e.Source).Text);
                if (((TextBox)e.Source).Text.Equals("0"))
                {
                    Console.WriteLine("0");
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
            // DbConnection.connect<Participant>(participant as object);
        }

        /// <summary>
        /// Удаление отмеченных участников
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteParticipant(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Вего участников " + participantsList.Count);
            for (int i = 0; i < participantsList.Count;)
            {
                Console.WriteLine(participantsList[i].IsSelected);
                if (participantsList[i].IsSelected)
                {
                    Console.WriteLine(participantsList[i].Name);
                    participantsList.Remove(participantsList[i]);
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
            // SaveFileDialog.FileOk += SaveFileDialog_FileOk;

            bool? result = SaveFileDialog.ShowDialog();
            if (result == true)
            {
                string path = Path.GetFullPath(SaveFileDialog.FileName);
                SaveFileDialog_FileOk(path);
            }
        }

        private void SaveFileDialog_FileOk(string path)
        {

            //string Destination = path;
            //registrationTable.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader;
            //registrationTable.SelectAllCells();


            //ApplicationCommands.Copy.Execute(null, registrationTable);
            //String resultat = (string)Clipboard.GetData(DataFormats.CommaSeparatedValue);
            //String result = (string)Clipboard.GetData(DataFormats.Text);
            //System.IO.StreamWriter file1 = new System.IO.StreamWriter(Destination);
            //file1.WriteLine(result.Replace(',', ' '));
            //file1.Close();

            Excel.Application excelapp = new Excel.Application();
            Excel.Workbook workbook = excelapp.Workbooks.Add();
            Excel.Worksheet worksheet = workbook.ActiveSheet;


            registrationTable.Columns.RemoveAt(0);
            for (int i = 1; i < registrationTable.Items.Count + 1; i++)
            {
                for (int j = 1; j < registrationTable.Columns.Count + 1; j++)
                {
                    DataGridColumn col = registrationTable.Columns[j - 1];
                    if (col.Header.Equals("Посевной"))
                    {
                        if (col.GetCellContent(registrationTable.Items[i - 1]) != null)
                        {
                            Console.WriteLine("Посевной " + (col.GetCellContent(registrationTable.Items[i - 1]) as ContentPresenter).Content);
                            //bool? text = (col.GetCellContent(registrationTable.Items[i - 1]) as CheckBox).IsChecked;
                            //worksheet.Rows[i].Columns[j] = text.ToString();
                            //Console.WriteLine(text);
                        }
                        else
                        {
                            Console.WriteLine("cell content is null");
                        }
                    }
                    else if (col.Header.Equals("Пол"))
                    {
                        if (col.GetCellContent(registrationTable.Items[i - 1]) != null)
                        {
                            string text = (col.GetCellContent(registrationTable.Items[i - 1]) as ComboBox).SelectedItem.ToString();
                            worksheet.Rows[i].Columns[j] = text;
                            Console.WriteLine(text);
                        }
                        else
                        {
                            Console.WriteLine("cell content is null");
                        }
                    }
                    else
                    {
                        Console.WriteLine("колонка " + (j - 1) + " " + col.Header);
                        string text = "txt";
                        if (col.GetCellContent(registrationTable.Items[i - 1]) != null)
                        {
                           text = (col.GetCellContent(registrationTable.Items[i - 1]) as TextBlock).Text;
                            worksheet.Rows[i].Columns[j] = text;
                        }
                        else
                        {
                            Console.WriteLine("cell content is null");
                        }
                    }
                }
            }

            excelapp.AlertBeforeOverwriting = false;
            workbook.SaveAs(path);
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

        private void exportRegistrationTable()
        {
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
                participantsList.Clear();
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

                        if (dataTable.Rows[0].ItemArray[j].Equals("Рейтинг (клубный)"))
                        {
                            if (int.TryParse(row.ItemArray[j].ToString(), out _))
                            {
                                int raiting = int.Parse(row.ItemArray[j].ToString());
                                newParticipant.ClubRating = raiting;
                            }
                        }
                    }
                    participantsList.Add(newParticipant);
                    registrationTable.ItemsSource = participantsList;
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

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("in focus Рейтинг (общий) " + ((TextBox)e.Source).Text);
        }

        private void ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (((ComboBox)e.Source).SelectedItem != "M" && ((ComboBox)e.Source).SelectedItem != "Ж")
            {
                SolidColorBrush scb = new SolidColorBrush(Color.FromRgb(255, 221, 219));
                Console.WriteLine("back");
                ((ComboBox)e.Source).Background = scb;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            appGrid.ColumnDefinitions[1].Width = new GridLength(50);

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
