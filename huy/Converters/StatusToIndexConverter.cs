using System;
using System.Globalization;
using System.Windows.Data;

namespace huy.Converters
{
    public class StatusToIndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int status)
            {
                // Status: 1 = Active, 2 = Deleted
                return status - 1; // Convert to 0-based index
            }
            return 0; // Default to Active
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int index)
            {
                return index + 1; // Convert back to 1-based status
            }
            return 1; // Default to Active
        }
    }
} 