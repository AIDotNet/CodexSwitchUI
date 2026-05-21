using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace CodexSwitchUI.Controls;

public class CodexImageIcon : Image
{
    public static readonly StyledProperty<string?> PathProperty =
        AvaloniaProperty.Register<CodexImageIcon, string?>(nameof(Path));

    public string? Path
    {
        get => GetValue(PathProperty);
        set => SetValue(PathProperty, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == PathProperty)
        {
            Source = TryLoad(Path);
        }
    }

    private static IImage? TryLoad(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return null;
        }

        try
        {
            if (Uri.TryCreate(path, UriKind.Absolute, out var uri) &&
                string.Equals(uri.Scheme, "avares", StringComparison.OrdinalIgnoreCase))
            {
                using var stream = AssetLoader.Open(uri);
                return new Bitmap(stream);
            }

            if (!File.Exists(path))
            {
                return null;
            }

            return new Bitmap(path);
        }
        catch
        {
            return null;
        }
    }
}

public class CodexStatCard : CodexFrame
{
    public static readonly StyledProperty<string?> LabelProperty =
        AvaloniaProperty.Register<CodexStatCard, string?>(nameof(Label));

    public static readonly StyledProperty<object?> ValueProperty =
        AvaloniaProperty.Register<CodexStatCard, object?>(nameof(Value));

    public static readonly StyledProperty<string?> DetailProperty =
        AvaloniaProperty.Register<CodexStatCard, string?>(nameof(Detail));

    public static readonly StyledProperty<object?> IconProperty =
        AvaloniaProperty.Register<CodexStatCard, object?>(nameof(Icon));

    public static readonly StyledProperty<IBrush?> AccentBrushProperty =
        AvaloniaProperty.Register<CodexStatCard, IBrush?>(nameof(AccentBrush));

    public static readonly StyledProperty<bool> HasDetailProperty =
        AvaloniaProperty.Register<CodexStatCard, bool>(nameof(HasDetail));

    public static readonly StyledProperty<bool> HasIconProperty =
        AvaloniaProperty.Register<CodexStatCard, bool>(nameof(HasIcon));

    static CodexStatCard()
    {
        DetailProperty.Changed.AddClassHandler<CodexStatCard>((card, _) => card.SyncSlots());
        IconProperty.Changed.AddClassHandler<CodexStatCard>((card, _) => card.SyncSlots());
    }

    public CodexStatCard()
    {
        SyncSlots();
    }

    public string? Label
    {
        get => GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    public object? Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public string? Detail
    {
        get => GetValue(DetailProperty);
        set => SetValue(DetailProperty, value);
    }

    public object? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public IBrush? AccentBrush
    {
        get => GetValue(AccentBrushProperty);
        set => SetValue(AccentBrushProperty, value);
    }

    public bool HasDetail => GetValue(HasDetailProperty);

    public bool HasIcon => GetValue(HasIconProperty);

    private void SyncSlots()
    {
        SetValue(HasDetailProperty, !string.IsNullOrWhiteSpace(Detail));
        SetValue(HasIconProperty, Icon is not null);
    }
}

public class CodexMetric : CodexFrame
{
    public static readonly StyledProperty<string?> LabelProperty =
        AvaloniaProperty.Register<CodexMetric, string?>(nameof(Label));

    public static readonly StyledProperty<object?> ValueProperty =
        AvaloniaProperty.Register<CodexMetric, object?>(nameof(Value));

    public static readonly StyledProperty<string?> DetailProperty =
        AvaloniaProperty.Register<CodexMetric, string?>(nameof(Detail));

    public static readonly StyledProperty<bool> HasDetailProperty =
        AvaloniaProperty.Register<CodexMetric, bool>(nameof(HasDetail));

    static CodexMetric()
    {
        DetailProperty.Changed.AddClassHandler<CodexMetric>((metric, _) => metric.SyncSlots());
    }

    public CodexMetric()
    {
        SyncSlots();
    }

    public string? Label
    {
        get => GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    public object? Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public string? Detail
    {
        get => GetValue(DetailProperty);
        set => SetValue(DetailProperty, value);
    }

    public bool HasDetail => GetValue(HasDetailProperty);

    private void SyncSlots()
    {
        SetValue(HasDetailProperty, !string.IsNullOrWhiteSpace(Detail));
    }
}
