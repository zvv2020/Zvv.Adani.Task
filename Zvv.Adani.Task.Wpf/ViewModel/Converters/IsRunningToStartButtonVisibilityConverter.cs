using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace Zvv.Adani.Task.Wpf.ViewModel.Converters
{
    /// <summary>
    /// An <see cref="IsRunningToButtonVisibilityConverter"/> for a start button.
    /// </summary>
    [ValueConversion(typeof(bool),typeof(Visibility))]
    class IsRunningToStartButtonVisibilityConverter : IsRunningToButtonVisibilityConverter
    {
        public IsRunningToStartButtonVisibilityConverter():base(Visibility.Hidden, Visibility.Visible) { }
    }
}
