using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace CodexSwitchUI.Controls;

public sealed class CodexImageIconLoadedEventArgs(string? path, string? oldPath, IImage source) : EventArgs
{
    public string? Path { get; } = path;

    public string? OldPath { get; } = oldPath;

    public IImage Source { get; } = source;
}

public sealed class CodexImageIconLoadFailedEventArgs(string? path, string? oldPath, string errorMessage) : EventArgs
{
    public string? Path { get; } = path;

    public string? OldPath { get; } = oldPath;

    public string ErrorMessage { get; } = errorMessage;
}

public class CodexImageIcon : Image
{
    public static readonly StyledProperty<string?> PathProperty =
        AvaloniaProperty.Register<CodexImageIcon, string?>(nameof(Path));

    public static readonly StyledProperty<bool> HasSourceProperty =
        AvaloniaProperty.Register<CodexImageIcon, bool>(nameof(HasSource));

    public static readonly StyledProperty<bool> IsMissingProperty =
        AvaloniaProperty.Register<CodexImageIcon, bool>(nameof(IsMissing));

    public static readonly StyledProperty<bool> IsEmptyProperty =
        AvaloniaProperty.Register<CodexImageIcon, bool>(nameof(IsEmpty), true);

    public static readonly StyledProperty<string?> LastLoadErrorProperty =
        AvaloniaProperty.Register<CodexImageIcon, string?>(nameof(LastLoadError));

    static CodexImageIcon()
    {
        PathProperty.Changed.AddClassHandler<CodexImageIcon>((icon, args) =>
            icon.OnPathChanged(args.OldValue as string, args.NewValue as string));
        SourceProperty.Changed.AddClassHandler<CodexImageIcon>((icon, _) => icon.SyncImageState());
    }

    public CodexImageIcon()
    {
        SyncImageState();
    }

    public event EventHandler<CodexImageIconLoadedEventArgs>? ImageLoaded;

    public event EventHandler<CodexImageIconLoadFailedEventArgs>? ImageLoadFailed;

    public string? Path
    {
        get => GetValue(PathProperty);
        set => SetValue(PathProperty, value);
    }

    public bool HasSource => GetValue(HasSourceProperty);

    public bool IsMissing => GetValue(IsMissingProperty);

    public bool IsEmpty => GetValue(IsEmptyProperty);

    public string? LastLoadError => GetValue(LastLoadErrorProperty);

    private void OnPathChanged(string? oldPath, string? newPath)
    {
        var result = TryLoad(newPath);

        SetValue(LastLoadErrorProperty, result.ErrorMessage);
        Source = result.Source;
        SyncImageState();

        if (result.Source is not null)
        {
            ImageLoaded?.Invoke(this, new CodexImageIconLoadedEventArgs(newPath, oldPath, result.Source));
        }
        else if (result.ErrorMessage is not null)
        {
            ImageLoadFailed?.Invoke(this, new CodexImageIconLoadFailedEventArgs(newPath, oldPath, result.ErrorMessage));
        }
    }

    private void SyncImageState()
    {
        var hasSource = Source is not null;
        var isMissing = !string.IsNullOrWhiteSpace(Path) &&
                        !hasSource &&
                        !string.IsNullOrWhiteSpace(LastLoadError);

        SetValue(HasSourceProperty, hasSource);
        SetValue(IsMissingProperty, isMissing);
        SetValue(IsEmptyProperty, !hasSource);

        Classes.Set("image-icon", true);
        Classes.Set("has-source", hasSource);
        Classes.Set("missing-source", isMissing);
        Classes.Set("empty-source", !hasSource);
    }

    private static ImageLoadResult TryLoad(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return ImageLoadResult.Empty;
        }

        try
        {
            if (Uri.TryCreate(path, UriKind.Absolute, out var uri) &&
                string.Equals(uri.Scheme, "avares", StringComparison.OrdinalIgnoreCase))
            {
                using var stream = AssetLoader.Open(uri);
                return new ImageLoadResult(new Bitmap(stream), null);
            }

            if (!File.Exists(path))
            {
                return new ImageLoadResult(null, "Image asset was not found.");
            }

            using var fileStream = File.OpenRead(path);
            return new ImageLoadResult(new Bitmap(fileStream), null);
        }
        catch (Exception ex)
        {
            return new ImageLoadResult(null, ex.Message);
        }
    }

    private sealed record ImageLoadResult(IImage? Source, string? ErrorMessage)
    {
        public static ImageLoadResult Empty { get; } = new(null, null);
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
