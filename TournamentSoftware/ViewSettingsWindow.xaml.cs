using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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
        private ObservableCollection<NominationFormModel> allNominations = new ObservableCollection<NominationFormModel>();
        private List<NominationFormModel> newNominations = new List<NominationFormModel>();
        private List<NominationFormModel> deletedNominations = new List<NominationFormModel>();

        private void SetColumnsVisibility()
        {
            foreach (CheckBox checkBox in checkBoxes)
            {
                SetColumnVisibility(checkBox.Content.ToString(), checkBox.IsChecked == true);
            }
        }

        private void SetColumnVisibility(string columnName, bool visibility)
        {
            try
            {
                mainWindowColumns.Single(column => column.Header == columnName).Visibility = visibility ? Visibility.Visible : Visibility.Hidden;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void SetNewSettings(object sender, RoutedEventArgs e)
        {
            SetColumnsVisibility();
            DeleteNominatios();
            AddNominations();
            Close();
        }

        private DataGridTemplateColumn CreateColumn(string header, Style cellStyle)
        {
            DataGridTemplateColumn column = new DataGridTemplateColumn
            {
                Header = header,
                CanUserResize = false,
                Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                CellStyle = cellStyle
            };
            return column;
        }

        private DataTemplate CheckBoxTemplate(Binding bind)
        {
            FrameworkElementFactory checkBox = new FrameworkElementFactory(typeof(CheckBox));
            checkBox.SetBinding(CheckBox.IsCheckedProperty, bind);
            checkBox.SetValue(CheckBox.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            checkBox.SetValue(CheckBox.VerticalAlignmentProperty, VerticalAlignment.Center);
            DataTemplate checkBoxTemplate = new DataTemplate
            {
                VisualTree = checkBox
            };

            return checkBoxTemplate;
        }

        private Binding CreateBinding(string bindingPath)
        {
            Binding bind = new Binding(bindingPath)
            {
                Mode = BindingMode.TwoWay
            };
            return bind;
        }

        private void AddNominationColumn(string nominationName)
        {
            DataGridTemplateColumn nominationColumn = CreateColumn(nominationName, GetCellStyle());
            Binding binding = CreateBinding("Nominations[" + nominationName + "]");
            DataTemplate checkBoxTemplate = CheckBoxTemplate(binding);
            nominationColumn.CellTemplate = checkBoxTemplate;
            mainWindowColumns.Add(nominationColumn);
        }

        private void AddNominations()
        {
            foreach (NominationFormModel nomination in newNominations)
            {
                string nominationName = nomination.Nomination.Name;
                if (CheckNominationNameValid(nominationName) && !IsNominationExists(nominationName))
                {
                    AddNominationToParticipants(nominationName);
                    AddNominationColumn(nominationName);
                    TournamentData.AddNomination(nominationName);
                }
                else
                {
                    MessageBox.Show("Недопустимое название номинации " + nominationName +
                        "!\nТакое название уже имеет существующий столбец, номинация " + nominationName +
                        " не будет добавлена в таблицу", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void DeleteNominatios()
        {
            foreach (NominationFormModel nominationForDelete in deletedNominations)
            {
                DeleteNomination(nominationForDelete.Nomination.Name);
            }
        }

        private void DeleteNomination(string nominationName)
        {
            RemoveColumn(nominationName);
            DeleteNominationFromPartisipants(nominationName);
            RemoveNomination(nominationName);
        }

        private void RemoveColumn(string columnName)
        {
            mainWindowColumns.Remove(mainWindowColumns.Single(column => column.Header == columnName));
        }

        private bool CheckNominationNameValid(string nominationName)
        {
            return !(stringsConstantsValues.Contains(nominationName) || nominationName.Equals(""));
        }

        private void AddNomination(object sender, RoutedEventArgs e)
        {
            NominationFormModel nomination = new NominationFormModel()
            {
                IsSelected = false
            };

            allNominations.Add(nomination);
            newNominations.Add(nomination);
            nominationsGrid.ItemsSource = allNominations;
        }

        private void RemoveSelectedNominations(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < allNominations.Count;)
            {
                if (allNominations[i].IsSelected)
                {
                    deletedNominations.Add(allNominations[i]);
                    allNominations.Remove(allNominations[i]);
                }
                else
                {
                    i++;
                }
            }
            nominationsGrid.ItemsSource = allNominations;
        }

        private void SelectAllForDelete_Checked(object sender, RoutedEventArgs e)
        {
            foreach (NominationFormModel nomination in allNominations)
            {
                nomination.IsSelected = true;
            }
        }

        private void SelectAllForDelete_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (NominationFormModel nomination in allNominations)
            {
                nomination.IsSelected = false;
            }
        }

        private void SettingsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            allNominations.Clear();
            newNominations.Clear();
            deletedNominations.Clear();
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
            foreach (NominationFormModel nomination in nominations)
            {
                allNominations.Add(nomination);
            }
            nominationsGrid.ItemsSource = allNominations;
        }
    }
}
