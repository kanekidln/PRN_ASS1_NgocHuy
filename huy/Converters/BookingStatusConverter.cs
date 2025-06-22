using System;
using System.Globalization;
using System.Windows.Data;

namespace huy.Converters
{
    public class BookingStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int bookingStatus)
            {
                return bookingStatus switch
                {
                    1 => "Active",
                    2 => "Completed",
                    3 => "Cancelled",
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
                    "Active" => 1,
                    "Completed" => 2,
                    "Cancelled" => 3,
                    _ => 1
                };
            }
            return 1;
        }
    }
} 