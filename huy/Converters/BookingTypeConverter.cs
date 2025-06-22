using System;
using System.Globalization;
using System.Windows.Data;

namespace huy.Converters
{
    public class BookingTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int bookingType)
            {
                return bookingType switch
                {
                    1 => "Online",
                    2 => "Offline",
                    _ => "Unknown"
                };
            }
            return "Unknown";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string strValue)
            {
                return strValue switch
                {
                    "Online" => 1,
                    "Offline" => 2,
                    _ => 1
                };
            }
            return 1;
        }
    }
} 