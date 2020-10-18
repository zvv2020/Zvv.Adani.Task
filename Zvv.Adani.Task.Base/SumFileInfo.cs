using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Serialization;

namespace Zvv.Adani.Task.Base
{
    /// <summary>
    /// Represents result of an operation on bytes summation for a separate file.
    /// </summary>
    public struct SumFileInfo:INotifyPropertyChanged
    {
        private string _fileName;
        private ulong _sumOfBytes;

        /// <summary>
        /// The file name.
        /// </summary>
        [XmlAttribute]
        public string FileName
        {
            get => _fileName;
            set
            {
                _fileName = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Received sum of bytes of the file.
        /// </summary>
        [XmlAttribute]
        public ulong SumOfBytes
        {
            get => _sumOfBytes;
            set
            {
                _sumOfBytes = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Occurs if a property is changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string prop = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        public override string ToString() => $"{FileName} : {SumOfBytes}";
    }
}
