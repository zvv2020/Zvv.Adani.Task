using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace Zvv.Adani.Task.Wpf.ViewModel.Converters
{
    /// <summary>
    /// Converts a flags set to <see cref="string"/> object that describes a status.
    /// </summary>
    [ValueConversion(typeof(object), typeof(string))]
    class StatusToStringConverter : IMultiValueConverter
    {
        /// <summary>
        /// Executes converting.
        /// </summary>
        /// <param name="values">Source values. The array must be two objects. 
        /// The first object must be the "isProcessing" <see cref="bool"/> flag. 
        /// The second object must be the "isStopping" <see cref="bool"/> flag.</param>
        /// <param name="targetType">The target type of the converter.</param>
        /// <param name="parameter">The parameter of the converter.</param>
        /// <param name="culture">The current culture of the converter.</param>
        /// <returns>A converted value.</returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values!=null && values.Length==2 && values[0] is bool isProcessing && values[1] is bool isStopping)
            {
                string result;
                if (isStopping)
                    result = "Stopping…";
                else if (isProcessing)
                    result = "Processing…";
                else
                    result = "Ready";
                return result;
            }
            else
                throw new InvalidOperationException("The source must contain 2 objects, both objects must be boolean.");
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}