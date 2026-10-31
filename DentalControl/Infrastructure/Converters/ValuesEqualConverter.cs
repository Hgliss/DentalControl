using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DentalControl.Infrastructure.Converters;

/// <summary>Comparación visual compartida para resaltar la opción seleccionada.</summary>
public sealed class ValuesEqualConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) =>
        values.Length == 2 && values.All(value => value != DependencyProperty.UnsetValue) &&
        Equals(values[0], values[1]);

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
