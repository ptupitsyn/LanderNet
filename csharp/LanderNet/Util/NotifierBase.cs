﻿using System.ComponentModel;

namespace LanderNet.UI.Util
{
    internal class NotifierBase : INotifyPropertyChanged
    {
        #region public methods

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region protected methods

        protected void OnPropertyChanged(string propertyName)
        {
            // TODO: [CallerMemberName] in .NET 4.5
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}