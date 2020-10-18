using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Zvv.Adani.Task.Base;

namespace Zvv.Adani.Task.Wpf.Model
{
    /// <summary>
    /// The model for representation a result of bytes summation for a separate file.
    /// </summary>
    class SumFileInfoGridModel:INotifyPropertyChanged
    {
        private SumFileInfo _data;
        private long _number;

        /// <summary>
        /// The result of the operation on bytes summation.
        /// </summary>
        public SumFileInfo Data
        {
            get => _data;
            set
            {
                _data = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Serial number of the result.
        /// </summary>
        public long Number
        {
            get => _number;
            set
            {
                _number = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Occurs if a property is changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string prop = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
