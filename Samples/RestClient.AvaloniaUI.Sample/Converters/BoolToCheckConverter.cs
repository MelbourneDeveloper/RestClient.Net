using System.Globalization;
using Avalonia.Data.Converters;

namespace RestClient.AvaloniaUI.Sample.Converters;

public class BoolToCheckConverter : IValueConverter
{
    public object? Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture
    ) => value is true ? "✅" : "⬜";

    public object? ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture
    ) => throw new NotImplementedException();
}
