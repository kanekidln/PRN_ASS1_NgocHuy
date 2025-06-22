using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace huy.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                // Nếu có parameter và parameter là "Inverse", đảo ngược kết quả
                bool inverse = parameter != null && parameter.ToString() == "Inverse";
                
                if (inverse)
                {
                    return boolValue ? Visibility.Collapsed : Visibility.Visible;
                }
                else
                {
                    return boolValue ? Visibility.Visible : Visibility.Collapsed;
                }
            }
            
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                // Nếu có parameter và parameter là "Inverse", đảo ngược kết quả
                bool inverse = parameter != null && parameter.ToString() == "Inverse";
                
                if (inverse)
                {
                    return visibility != Visibility.Visible;
                }
                else
                {
                    return visibility == Visibility.Visible;
                }
            }
            
            return false;
        }
    }
} 