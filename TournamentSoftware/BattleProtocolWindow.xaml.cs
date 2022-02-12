using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using TournamentSoftware.wrapperClasses;

namespace TournamentSoftware
{
    /// <summary>
    /// Interaction logic for BattleProtocolWindow.xaml
    /// </summary>
    public partial class BattleProtocolWindow : Window
    {
        public BattleProtocolWindow()
        {
            InitializeComponent();
            DataContext = new BattleProtocolWrapper();
        }

        private void AddJudgeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void DeleteJudgeMenuItem_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
