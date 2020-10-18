using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;

namespace Zvv.Adani.Task.Wpf.ViewModel.Converters
{
    //https://stackoverflow.com/questions/1039636/how-to-bind-inverse-boolean-properties-in-wpf
    /// <summary>
    /// An <see cref="IValueConverter"/> object that inverses a <see cref="bool"/> value.
    /// </summary>
    [ValueConversion(typeof(bool), typeof(bool))]
    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (value is bool boolValue && targetType == typeof(bool))
                return !boolValue;
            else
                throw new InvalidOperationException("The source and the target must be a boolean");
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture) => Convert(value, targetType, parameter, culture);
    }
}
