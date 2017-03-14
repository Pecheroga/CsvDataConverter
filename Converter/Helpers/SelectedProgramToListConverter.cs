using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using DataSource.Structure;

namespace Converter.Helpers
{
    internal sealed class SelectedProgramToListConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var program = value as Program;
            return new List<Program>
            {
                program
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
