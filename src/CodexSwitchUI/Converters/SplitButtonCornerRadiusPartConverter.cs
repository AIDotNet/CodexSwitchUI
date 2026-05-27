using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace CodexSwitchUI.Converters;

public enum SplitButtonCornerRadiusPart
{
    Left,
    Right
}

public sealed class SplitButtonCornerRadiusPartConverter : IValueConverter
{
    public SplitButtonCornerRadiusPart Part { get; set; }

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not CornerRadius radius)
        {
            return value ?? new CornerRadius();
        }

        var part = parameter is SplitButtonCornerRadiusPart typedPart
            ? typedPart
            : Enum.TryParse(parameter?.ToString(), ignoreCase: true, out SplitButtonCornerRadiusPart parsedPart)
                ? parsedPart
                : Part;

        return part == SplitButtonCornerRadiusPart.Right
            ? new CornerRadius(0, radius.TopRight, radius.BottomRight, 0)
            : new CornerRadius(radius.TopLeft, 0, 0, radius.BottomLeft);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
