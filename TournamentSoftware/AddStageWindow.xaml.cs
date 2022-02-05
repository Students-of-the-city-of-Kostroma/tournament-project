﻿using System;
using System.Collections.Generic;
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
using static TournamentSoftware.TournamentData;

namespace TournamentSoftware
{
    /// <summary>
    /// Interaction logic for AddStageWindow.xaml
    /// </summary>
    public partial class AddStageWindow : Window
    {
        public AddStageWindow()
        {
            InitializeComponent();
            fightingSystemsCombobox.ItemsSource = fightingSystems;
        }

        private void NumericOnly(object sender, TextCompositionEventArgs e)
        {
            if (!int.TryParse(e.Text, out _))
            {
                e.Handled = true;
            }
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Apply(object sender, RoutedEventArgs e)
        {
            TournamentGridWindow.roundsCount = int.Parse(rounds.Text);
            TournamentGridWindow.fightingSystem = fightingSystemsCombobox.Text;
            Close();
        }
    }
}