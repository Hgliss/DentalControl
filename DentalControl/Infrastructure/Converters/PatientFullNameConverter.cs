using System.Globalization;
using System.Windows.Data;

namespace DentalControl.Infrastructure.Converters;

/// <summary>Presenta el nombre completo omitiendo los componentes opcionales vacíos.</summary>
public sealed class PatientFullNameConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) =>
        string.Join(" ", values.OfType<string>().Where(value => !string.IsNullOrWhiteSpace(value)).Select(value => value.Trim()));

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) =>
        throw new NotSupportedException("El nombre completo del listado es de solo lectura.");
}
