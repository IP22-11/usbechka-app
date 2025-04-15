using System;
using System.Globalization;
using System.Windows.Data;

namespace usbechka_app.pages
{
    public class BoolToStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isReserved)
            {
                return isReserved ? "Занят" : "Свободен";
            }
            return "Неизвестно";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
