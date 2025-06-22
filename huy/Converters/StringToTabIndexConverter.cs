using System;
using System.Globalization;
using System.Windows.Data;

namespace  huy.Converters
{
    public class StringToTabIndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string tabName)
            {
                return tabName switch
                {
                    "Profile" => 0,
                    "Password" => 1,
                    "BookRoom" => 2,
                    "BookingHistory" => 3,
                    _ => 0
                };
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int index)
            {
                return index switch
                {
                    0 => "Profile",
                    1 => "Password",
                    2 => "BookRoom",
                    3 => "BookingHistory",
                    _ => "Profile"
                };
            }
            return "Profile";
        }
    }
} 