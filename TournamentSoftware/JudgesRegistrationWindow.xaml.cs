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
    /// Interaction logic for JudgesRegistrationWindow.xaml
    /// </summary>
    public partial class JudgesRegistrationWindow : Window
    {
        public static ObservableCollection<Jude> judesList = new ObservableCollection<Jude>();
        public JudgesRegistrationWindow()
        {
            InitializeComponent();
            JudesTable.ItemsSource = judesList;
        }

        /// <summary>
        /// Добавление нового судьи
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addJude (object sender, RoutedEventArgs e)
        {
            Jude jude = new Jude
            {
                Name = "",
                Surname = "",
                Patronymic = "",
                Club = "",
                City = "",
            };

            judesList.Add(jude);
        }
    }
}
