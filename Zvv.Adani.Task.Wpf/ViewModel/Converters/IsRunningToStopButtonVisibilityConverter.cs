using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace Zvv.Adani.Task.Wpf.ViewModel.Converters
{
    /// <summary>
    /// An <see cref="IsRunningToButtonVisibilityConverter"/> for a stop button.
    /// </summary>
    [ValueConversion(typeof(bool), typeof(Visibility))]
    class IsRunningToStopButtonVisibilityConverter: IsRunningToButtonVisibilityConverter
    {
        public IsRunningToStopButtonVisibilityConverter() : base(Visibility.Visible, Visibility.Hidden) { }
    }
}
