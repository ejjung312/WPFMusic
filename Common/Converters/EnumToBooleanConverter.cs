using System.Globalization;
using System.Windows.Data;

namespace Common.Converters
{
    public class EnumToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string strEnum = value.ToString();

            if (parameter != null && parameter.ToString().Equals(strEnum))
            {
                return true;
            } 
            else
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
