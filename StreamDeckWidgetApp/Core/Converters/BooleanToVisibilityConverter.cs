using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace StreamDeckWidgetApp.Core.Converters;

public class BooleanToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool isVisible = value != null && (value is bool b ? b : true); // Nesne null deðilse veya true ise
        
        // Eðer parameter "Inverse" ise tam tersini yap
        if (parameter?.ToString() == "Inverse")
            isVisible = !isVisible;

        return isVisible ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
