using System;
using System.Collections.Generic;
using System.Windows;

namespace TournamentSoftware
{
    /// <summary>
    /// Interaction logic for ErrorListWindow.xaml
    /// </summary>
    public partial class ErrorListWindow : Window
    {
        public ErrorListWindow()
        {
            InitializeComponent();
        }

        public void ShowErrors(List<string> errors)
        {
            foreach (string error in errors) 
            {
                errorList.Items.Add(error);
            }
            this.Show();
        }

        private void OK(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
