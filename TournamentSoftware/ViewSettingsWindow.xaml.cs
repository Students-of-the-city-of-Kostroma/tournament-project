using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using static TournamentSoftware.ApplicationStringValues;
using static TournamentSoftware.TournamentData;

namespace TournamentSoftware
{
    public partial class ViewSettingsWindow : Window
    {
        public ViewSettingsWindow()
        {
            InitializeComponent();
        }

        private List<CheckBox> checkBoxes = new List<CheckBox>();
        private ObservableCollection<DataGridColumn> mainWindowColumns = ((MainWindow)Application.Current.MainWindow).registrationTable.Columns;
        private ObservableCollection<DataGridTemplateColumn> mainNominationsColumns = ((MainWindow)Application.Current.MainWindow).nominationsColumn;
        private ObservableCollection<NominationFormModel> newNominations = new ObservableCollection<NominationFormModel>();
        private List<NominationFormModel> nominationsForDelete = new List<NominationFormModel>();

        private void setColumnsVisibility()
        {
            for (int i = 0; i < checkBoxes.Count; i++)
            {
                setColumnVisibility(checkBoxes[i].Content.ToString(), checkBoxes[i].IsChecked == true);
            }
        }

        private void setColumnVisibility(string columnName, bool visibility)
        {
            try
            {
                mainWindowColumns.Single(predicate: column => column.Header == columnName).Visibility = visibility ? Visibility.Visible : Visibility.Hidden;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void SetNewSettings(object sender, RoutedEventArgs e)
        {
            setColumnsVisibility();

            // добавляем новые номинации в таблицу регистрации
            for (int i = 0; i < newNominations.Count; i++)
            {
                DataGridTemplateColumn nominationColumn = new DataGridTemplateColumn();
                string nominationName = newNominations[i].Nomination.Name;
                if (CheckNominationNameValid(nominationName))
                {
                    if (!IsNominationExists(nominationName))
                    {
                        nominationColumn.Header = nominationName;
                        nominationColumn.CanUserResize = false;
                        nominationColumn.Width = new DataGridLength(1, DataGridLengthUnitType.Star);

                        Binding bind = new Binding("Nominations[" + nominationName + "]");
                        bind.Mode = BindingMode.TwoWay;

                        var cellStyle = new Style(typeof(DataGridCell));
                        cellStyle.Setters.Add(new Setter()
                        {
                            Property = BackgroundProperty,
                            Value = (Brush)new BrushConverter().ConvertFrom("#F5F1DA")
                        });
                        nominationColumn.CellStyle = cellStyle;

                        FrameworkElementFactory checkBox = new FrameworkElementFactory(typeof(CheckBox));
                        checkBox.SetBinding(CheckBox.IsCheckedProperty, bind);
                        checkBox.SetValue(CheckBox.HorizontalAlignmentProperty, HorizontalAlignment.Center);
                        checkBox.SetValue(CheckBox.VerticalAlignmentProperty, VerticalAlignment.Center);
                        DataTemplate checkBoxTemplate = new DataTemplate();
                        checkBoxTemplate.VisualTree = checkBox;

                        nominationColumn.CellTemplate = checkBoxTemplate;

                        foreach (ParticipantFormModel p in participantsList)
                        {
                            p.Nominations.Add(nominationName, false);
                        }

                        //mainNominationsColumns.Add(nominationColumn);
                        mainWindowColumns.Add(nominationColumn);
                    }

                }
                else
                {
                    MessageBox.Show("Недопустимое название номинации " + nominationName +
                        "!\nТакое название уже имеет существующий столбец, номинация " + nominationName +
                        " не будет добавлена в таблицу", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            // удаляем номинации из таблицы регистрации
            for (int i = 0; i < nominationsForDelete.Count; i++)
            {
                for (int j = 0; j < mainNominationsColumns.Count; j++)
                {
                    string nominationName = nominationsForDelete[i].Nomination.Name;
                    // находим удаляемую колонку и удаляем из таблицы
                    if (nominationName.Equals(mainNominationsColumns[j].Header))
                    {
                        mainWindowColumns.Remove(mainNominationsColumns[j]);
                        // удаляем номинации у участников
                        foreach (ParticipantFormModel p in participantsList)
                        {
                            p.Nominations.Remove(nominationName);
                        }
                        RemoveNomination(nominationName);
                    }
                }
            }

            for (int i = 0; i < nominationsForDelete.Count; i++)
            {
                for (int j = 0; j < mainNominationsColumns.Count; j++)
                {
                    if (nominationsForDelete[i].Nomination.Name.Equals(mainNominationsColumns[j].Header))
                    {
                        mainNominationsColumns.Remove(mainNominationsColumns[j]);
                    }
                }
            }

            nominations.Clear();
            nominationsForDelete.Clear();

            Close();
        }

        private bool CheckNominationNameValid(string nominationName)
        {
            return !(stringsConstantsValues.Contains(nominationName) || nominationName.Equals(nominationName));
        }

        private void AddNomination(object sender, RoutedEventArgs e)
        {
            NominationFormModel nomination = new NominationFormModel()
            {
                IsSelected = false
            };

            newNominations.Add(nomination);
            nominationsGrid.ItemsSource = newNominations;
        }

        private void DeleteSelectedNominations(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < newNominations.Count;)
            {
                if (newNominations[i].IsSelected)
                {
                    nominationsForDelete.Add(newNominations[i]);
                    newNominations.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
            nominationsGrid.ItemsSource = newNominations;
        }

        private void SelectAllForDelete_Checked(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < nominations.Count; i++)
            {
                nominations[i].IsSelected = true;
            }
        }

        private void SelectAllForDelete_Unchecked(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < nominations.Count; i++)
            {
                nominations[i].IsSelected = false;
            }
        }

        private void SettingsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            CreateCheckBoxes();
            LoadNominations();
        }

        private void CreateCheckBoxes()
        {
            for (int i = 1; i < mainWindowColumns.Count; i++)
            {
                CheckBox checkBox = new CheckBox
                {
                    IsChecked = false,
                    Content = mainWindowColumns[i].Header
                };
                if (mainWindowColumns[i].Visibility == Visibility.Visible)
                {
                    checkBox.IsChecked = true;
                }
                checkBoxes.Add(checkBox);
                columnsNames.Items.Add(checkBox);
            }
        }

        private void LoadNominations()
        {
            for (int i = 0; i < nominations.Count; i++)
            {
                newNominations.Add(nominations[i]);
            }
            nominationsGrid.ItemsSource = newNominations;
        }
    }
}
