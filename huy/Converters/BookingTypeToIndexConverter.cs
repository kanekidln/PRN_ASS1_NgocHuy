using System;
using System.Globalization;
using System.Windows.Data;

namespace huy.Converters
{
    public class BookingTypeToIndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int bookingType)
            {
                // BookingType: 1 = Online, 2 = Offline
                return bookingType - 1; // Convert to 0-based index
            }
            return 0; // Default to Online
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int index)
            {
                return index + 1; // Convert back to 1-based BookingType
            }
            return 1; // Default to Online
        }
    }
} 