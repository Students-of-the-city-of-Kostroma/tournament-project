﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentSoftware.wrapperClasses
{
    public class RoundResultWrapper
    {
        public ObservableCollection<FighterRoundResultWrapper> FighterRoundResult { get; private set; }
        public RoundResultWrapper(string[] rowHeader)
        {
            FighterRoundResult = new ObservableCollection<FighterRoundResultWrapper>();
            for (int i = 0; i < 2; i++)
                FighterRoundResult.Add(new FighterRoundResultWrapper(rowHeader[i]));
        }
    }
}