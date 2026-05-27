using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;

namespace CodexSwitchUI.Controls;

public enum CodexAvatarLoadingStatus
{
    Idle,
    Loading,
    Loaded,
    Error
}

public sealed class CodexAvatarLoadingStatusChangedEventArgs(
    CodexAvatarLoadingStatus oldStatus,
    CodexAvatarLoadingStatus newStatus,
    string? imagePath,
    IImage? source,
    string? errorMessage) : EventArgs
{
    public CodexAvatarLoadingStatus OldStatus { get; } = oldStatus;

    public CodexAvatarLoadingStatus NewStatus { get; } = newStatus;

    public string? ImagePath { get; } = imagePath;

    public IImage? Source { get; } = source;

    public string? ErrorMessage { get; } = errorMessage;
}

public class CodexAvatar : CodexFrame
{
    private DispatcherTimer? _fallbackTimer;
    private bool _isApplyingImagePathSource;

    public static readonly StyledProperty<CodexControlVariant> VariantProperty =
        AvaloniaProperty.Register<CodexAvatar, CodexControlVariant>(nameof(Variant), CodexControlVariant.Secondary);

    public static readonly StyledProperty<string?> FallbackProperty =
        AvaloniaProperty.Register<CodexAvatar, string?>(nameof(Fallback));

    public static readonly StyledProperty<IImage?> SourceProperty =
        AvaloniaProperty.Register<CodexAvatar, IImage?>(nameof(Source));

    public static readonly StyledProperty<string?> ImagePathProperty =
        AvaloniaProperty.Register<CodexAvatar, string?>(nameof(ImagePath));

    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexAvatar, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    public static readonly StyledProperty<CodexControlVariant> StatusVariantProperty =
        AvaloniaProperty.Register<CodexAvatar, CodexControlVariant>(nameof(StatusVariant), CodexControlVariant.Success);

    public static readonly StyledProperty<bool> IsStatusVisibleProperty =
        AvaloniaProperty.Register<CodexAvatar, bool>(nameof(IsStatusVisible));

    public static readonly StyledProperty<bool> HasImageProperty =
        AvaloniaProperty.Register<CodexAvatar, bool>(nameof(HasImage));

    public static readonly StyledProperty<bool> HasFallbackProperty =
        AvaloniaProperty.Register<CodexAvatar, bool>(nameof(HasFallback));

    public static readonly StyledProperty<bool> IsFallbackVisibleProperty =
        AvaloniaProperty.Register<CodexAvatar, bool>(nameof(IsFallbackVisible));

    public static readonly StyledProperty<TimeSpan> FallbackDelayProperty =
        AvaloniaProperty.Register<CodexAvatar, TimeSpan>(nameof(FallbackDelay));

    public static readonly StyledProperty<CodexAvatarLoadingStatus> LoadingStatusProperty =
        AvaloniaProperty.Register<CodexAvatar, CodexAvatarLoadingStatus>(nameof(LoadingStatus));

    public static readonly StyledProperty<string?> LastLoadErrorProperty =
        AvaloniaProperty.Register<CodexAvatar, string?>(nameof(LastLoadError));

    static CodexAvatar()
    {
        VariantProperty.Changed.AddClassHandler<CodexAvatar>((avatar, _) => avatar.SyncClasses());
        FallbackProperty.Changed.AddClassHandler<CodexAvatar>((avatar, _) => avatar.SyncClassesAndFallback());
        SourceProperty.Changed.AddClassHandler<CodexAvatar>((avatar, _) => avatar.OnSourceChanged());
        ImagePathProperty.Changed.AddClassHandler<CodexAvatar>((avatar, args) =>
            avatar.OnImagePathChanged(args.OldValue as string, args.NewValue as string));
        SizeProperty.Changed.AddClassHandler<CodexAvatar>((avatar, _) => avatar.SyncClasses());
        StatusVariantProperty.Changed.AddClassHandler<CodexAvatar>((avatar, _) => avatar.SyncClasses());
        IsStatusVisibleProperty.Changed.AddClassHandler<CodexAvatar>((avatar, _) => avatar.SyncClasses());
        FallbackDelayProperty.Changed.AddClassHandler<CodexAvatar>((avatar, _) => avatar.SyncFallbackVisibility());
        LoadingStatusProperty.Changed.AddClassHandler<CodexAvatar>((avatar, args) => avatar.OnLoadingStatusChanged(args));
    }

    public CodexAvatar()
    {
        SyncClassesAndFallback();
    }

    public event EventHandler<CodexAvatarLoadingStatusChangedEventArgs>? LoadingStatusChanged;

    public CodexControlVariant Variant
    {
        get => GetValue(VariantProperty);
        set => SetValue(VariantProperty, value);
    }

    public string? Fallback
    {
        get => GetValue(FallbackProperty);
        set => SetValue(FallbackProperty, value);
    }

    public IImage? Source
    {
        get => GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    public string? ImagePath
    {
        get => GetValue(ImagePathProperty);
        set => SetValue(ImagePathProperty, value);
    }

    public CodexControlSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    public CodexControlVariant StatusVariant
    {
        get => GetValue(StatusVariantProperty);
        set => SetValue(StatusVariantProperty, value);
    }

    public bool IsStatusVisible
    {
        get => GetValue(IsStatusVisibleProperty);
        set => SetValue(IsStatusVisibleProperty, value);
    }

    public bool HasImage => GetValue(HasImageProperty);

    public bool HasFallback => GetValue(HasFallbackProperty);

    public bool IsFallbackVisible => GetValue(IsFallbackVisibleProperty);

    public TimeSpan FallbackDelay
    {
        get => GetValue(FallbackDelayProperty);
        set => SetValue(FallbackDelayProperty, value);
    }

    public CodexAvatarLoadingStatus LoadingStatus
    {
        get => GetValue(LoadingStatusProperty);
        set => SetValue(LoadingStatusProperty, value);
    }

    public string? LastLoadError => GetValue(LastLoadErrorProperty);

    private void OnImagePathChanged(string? oldPath, string? newPath)
    {
        StopFallbackTimer();
        SetValue(LastLoadErrorProperty, null);
        SetSourceFromImagePath(null);

        if (string.IsNullOrWhiteSpace(newPath))
        {
            SetLoadingStatus(CodexAvatarLoadingStatus.Idle);
            return;
        }

        SetLoadingStatus(CodexAvatarLoadingStatus.Loading);

        var result = TryLoad(newPath);
        SetValue(LastLoadErrorProperty, result.ErrorMessage);
        SetSourceFromImagePath(result.Source);
        SetLoadingStatus(result.Source is not null
            ? CodexAvatarLoadingStatus.Loaded
            : CodexAvatarLoadingStatus.Error);
    }

    private void OnSourceChanged()
    {
        if (_isApplyingImagePathSource)
        {
            SyncClassesAndFallback();
            return;
        }

        StopFallbackTimer();
        SetValue(LastLoadErrorProperty, null);
        SetLoadingStatus(Source is not null
            ? CodexAvatarLoadingStatus.Loaded
            : CodexAvatarLoadingStatus.Idle);
    }

    private void OnLoadingStatusChanged(AvaloniaPropertyChangedEventArgs args)
    {
        var oldStatus = args.OldValue is CodexAvatarLoadingStatus oldValue
            ? oldValue
            : CodexAvatarLoadingStatus.Idle;
        var newStatus = args.NewValue is CodexAvatarLoadingStatus newValue
            ? newValue
            : LoadingStatus;

        SyncClassesAndFallback();

        if (oldStatus != newStatus)
        {
            LoadingStatusChanged?.Invoke(
                this,
                new CodexAvatarLoadingStatusChangedEventArgs(
                    oldStatus,
                    newStatus,
                    ImagePath,
                    Source,
                    LastLoadError));
        }
    }

    private void SetSourceFromImagePath(IImage? source)
    {
        _isApplyingImagePathSource = true;
        try
        {
            Source = source;
        }
        finally
        {
            _isApplyingImagePathSource = false;
        }
    }

    private void SetLoadingStatus(CodexAvatarLoadingStatus status)
    {
        if (LoadingStatus == status)
        {
            SyncClassesAndFallback();
            return;
        }

        SetValue(LoadingStatusProperty, status);
    }

    private void SyncClassesAndFallback()
    {
        SyncClasses();
        SyncFallbackVisibility();
    }

    private void SyncClasses()
    {
        var hasImage = Source is not null;
        var hasFallback = !string.IsNullOrWhiteSpace(Fallback);

        SetValue(HasImageProperty, hasImage);
        SetValue(HasFallbackProperty, hasFallback);
        Classes.Set("avatar", true);
        CodexClassSync.SetVariant(Classes, Variant);
        CodexClassSync.SetSize(Classes, Size);
        SetStatusVariantClasses();
        Classes.Set("has-image", hasImage);
        Classes.Set("has-fallback", hasFallback);
        Classes.Set("status-visible", IsStatusVisible);
        Classes.Set("fallback-visible", IsFallbackVisible);
        Classes.Set("fallback-delayed", FallbackDelay > TimeSpan.Zero);
        Classes.Set("loading", LoadingStatus == CodexAvatarLoadingStatus.Loading);
        Classes.Set("loaded", LoadingStatus == CodexAvatarLoadingStatus.Loaded);
        Classes.Set("error", LoadingStatus == CodexAvatarLoadingStatus.Error);
        Classes.Set("idle", LoadingStatus == CodexAvatarLoadingStatus.Idle);
    }

    private void SyncFallbackVisibility()
    {
        var canShowFallback = HasFallback && Source is null && LoadingStatus != CodexAvatarLoadingStatus.Loaded;
        var shouldDelay = canShowFallback &&
                          LoadingStatus == CodexAvatarLoadingStatus.Loading &&
                          FallbackDelay > TimeSpan.Zero;

        if (shouldDelay)
        {
            SetValue(IsFallbackVisibleProperty, false);
            ScheduleFallbackDelay();
        }
        else
        {
            StopFallbackTimer();
            SetValue(IsFallbackVisibleProperty, canShowFallback);
        }

        Classes.Set("fallback-visible", IsFallbackVisible);
    }

    private void ScheduleFallbackDelay()
    {
        StopFallbackTimer();

        _fallbackTimer = new DispatcherTimer
        {
            Interval = FallbackDelay
        };
        _fallbackTimer.Tick += OnFallbackTimerTick;
        _fallbackTimer.Start();
    }

    private void StopFallbackTimer()
    {
        if (_fallbackTimer is null)
        {
            return;
        }

        _fallbackTimer.Stop();
        _fallbackTimer.Tick -= OnFallbackTimerTick;
        _fallbackTimer = null;
    }

    private void OnFallbackTimerTick(object? sender, EventArgs e)
    {
        StopFallbackTimer();

        if (LoadingStatus == CodexAvatarLoadingStatus.Loading && Source is null && HasFallback)
        {
            SetValue(IsFallbackVisibleProperty, true);
            SyncClasses();
        }
    }

    private static ImageLoadResult TryLoad(string path)
    {
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
                return new ImageLoadResult(null, "Avatar image was not found.");
            }

            using var fileStream = File.OpenRead(path);
            return new ImageLoadResult(new Bitmap(fileStream), null);
        }
        catch (Exception ex)
        {
            return new ImageLoadResult(null, ex.Message);
        }
    }

    private void SetStatusVariantClasses()
    {
        Classes.Set("status-default", StatusVariant == CodexControlVariant.Default);
        Classes.Set("status-secondary", StatusVariant == CodexControlVariant.Secondary);
        Classes.Set("status-destructive", StatusVariant == CodexControlVariant.Destructive);
        Classes.Set("status-outline", StatusVariant == CodexControlVariant.Outline);
        Classes.Set("status-ghost", StatusVariant == CodexControlVariant.Ghost);
        Classes.Set("status-link", StatusVariant == CodexControlVariant.Link);
        Classes.Set("status-success", StatusVariant == CodexControlVariant.Success);
        Classes.Set("status-warning", StatusVariant == CodexControlVariant.Warning);
    }

    private sealed record ImageLoadResult(IImage? Source, string? ErrorMessage);
}
