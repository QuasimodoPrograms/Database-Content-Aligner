using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DBelign
{
    internal class EmptyStringToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // If there is no parameter (default)...
            if (parameter == null)
            {
                // If the passed string is empty...
                if (value.ToString() == string.Empty)
                    return false;
                else
                    return true;
            }
            // If there is a reverse parameter...
            else if (parameter.ToString() == "reverse")
            {
                // If the passed string is empty...
                if (value.ToString() == string.Empty)
                    return true;
                else
                    return false;
            }
            // If there is another parameter...
            else
                // Throw an exception
                throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
