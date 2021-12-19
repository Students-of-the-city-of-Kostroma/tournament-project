using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TournamentSoftware
{

    /// <summary>
    /// Логика взаимодействия для StartWindow.xaml
    /// </summary>
    public partial class StartWindow : Window
    {
        bool InstrumentPanelIsClosed = false;
        public StartWindow()
        {
            InitializeComponent();
        }
        private void goRegistrate(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            Close();
            mainWindow.Show();
        }
        private void startWindow_Loaded(object sender, RoutedEventArgs e)
        {

        }
        private void HideInstrumentPanel (object sender, RoutedEventArgs e)
        {
            if (InstrumentPanelIsClosed) 
            {
                InstrumentPanelIsClosed = false;
                startWindowGrid.ColumnDefinitions[1].Width = new GridLength(300);
                HideInstrumentPanelButton2.Visibility = Visibility.Hidden;
                HideInstrumentPanelButton.Visibility = Visibility.Visible;
            }
            else
            {
                InstrumentPanelIsClosed = true;
                startWindowGrid.ColumnDefinitions[1].Width = new GridLength(0);
                HideInstrumentPanelButton.Visibility = Visibility.Hidden;
                HideInstrumentPanelButton2.Visibility = Visibility.Visible;
            }

        }
    }
}
