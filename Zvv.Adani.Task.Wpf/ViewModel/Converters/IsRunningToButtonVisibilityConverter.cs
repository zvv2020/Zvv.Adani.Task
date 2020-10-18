using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace Zvv.Adani.Task.Wpf.ViewModel.Converters
{
    /// <summary>
    /// An <see cref="IValueConverter"/> object that converts a <see cref="bool"/> object to <see cref="Visibility"/> on definite rules.
    /// </summary>
    [ValueConversion(typeof(bool), typeof(Visibility))]
    class IsRunningToButtonVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// A <see cref="Visibility"/> value that will be returned if the source value is <see cref="true"/>.
        /// </summary>
        protected Visibility VisibilityForTrue { get; set; }

        /// <summary>
        /// A <see cref="Visibility"/> value that will be returned if the source value is <see cref="false"/>.
        /// </summary>
        protected Visibility VisibilityForFalse { get; set; }

        /// <summary>
        /// Creates an object of the class.
        /// </summary>
        /// <param name="visibilityForTrue">A <see cref="Visibility"/> value that will be returned if the source value is <see cref="true"/>.</param>
        /// <param name="visibilityForFalse">A <see cref="Visibility"/> value that will be returned if the source value is <see cref="false"/>.</param>
        public IsRunningToButtonVisibilityConverter(Visibility visibilityForTrue, Visibility visibilityForFalse)
        {
            VisibilityForTrue = visibilityForTrue;
            VisibilityForFalse = visibilityForFalse;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue && targetType == typeof(Visibility))
                return boolValue ? VisibilityForTrue : VisibilityForFalse;
            else
                throw new InvalidOperationException($"The source must be boolean, the targetType must be Visibility");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
