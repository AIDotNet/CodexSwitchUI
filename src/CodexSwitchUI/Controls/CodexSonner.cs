using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CodexSwitchUI.Tokens;

namespace CodexSwitchUI.Controls;

public class CodexSonner : StackPanel
{
    private const double ToastAnimationOffset = 10;
    private const double ToastExpandedMaxHeight = 1000;

    private readonly Dictionary<Guid, SonnerToastVisual> _toastVisuals = [];
    private readonly Dictionary<Guid, PropertyChangedEventHandler> _toastSubscriptions = [];

    public static readonly StyledProperty<CodexSonnerPosition> PositionProperty =
        AvaloniaProperty.Register<CodexSonner, CodexSonnerPosition>(nameof(Position), CodexSonnerPosition.BottomRight);

    public static readonly StyledProperty<int> VisibleToastsProperty =
        AvaloniaProperty.Register<CodexSonner, int>(nameof(VisibleToasts), 3);

    public static readonly StyledProperty<double> GapProperty =
        AvaloniaProperty.Register<CodexSonner, double>(nameof(Gap), 8);

    public static readonly StyledProperty<Thickness> OffsetProperty =
        AvaloniaProperty.Register<CodexSonner, Thickness>(nameof(Offset), new Thickness(16));

    public static readonly StyledProperty<bool> ExpandProperty =
        AvaloniaProperty.Register<CodexSonner, bool>(nameof(Expand), true);

    public static readonly StyledProperty<bool> RichColorsProperty =
        AvaloniaProperty.Register<CodexSonner, bool>(nameof(RichColors));

    public static readonly StyledProperty<bool> CloseButtonProperty =
        AvaloniaProperty.Register<CodexSonner, bool>(nameof(CloseButton), true);

    static CodexSonner()
    {
        PositionProperty.Changed.AddClassHandler<CodexSonner>((sonner, _) =>
        {
            sonner.SyncLayout();
            sonner.RefreshItems();
        });
        VisibleToastsProperty.Changed.AddClassHandler<CodexSonner>((sonner, _) => sonner.RefreshItems());
        GapProperty.Changed.AddClassHandler<CodexSonner>((sonner, _) => sonner.SyncLayout());
        OffsetProperty.Changed.AddClassHandler<CodexSonner>((sonner, _) => sonner.SyncLayout());
        ExpandProperty.Changed.AddClassHandler<CodexSonner>((sonner, _) =>
        {
            sonner.SyncClasses();
            sonner.RefreshItems();
        });
        RichColorsProperty.Changed.AddClassHandler<CodexSonner>((sonner, _) =>
        {
            sonner.SyncClasses();
            sonner.RefreshItems();
        });
        CloseButtonProperty.Changed.AddClassHandler<CodexSonner>((sonner, _) =>
        {
            sonner.SyncClasses();
            sonner.RefreshItems();
        });
    }

    public CodexSonner()
    {
        Orientation = Orientation.Vertical;
        SyncLayout();
        RefreshItems();
    }

    public CodexSonnerPosition Position
    {
        get => GetValue(PositionProperty);
        set => SetValue(PositionProperty, value);
    }

    public int VisibleToasts
    {
        get => GetValue(VisibleToastsProperty);
        set => SetValue(VisibleToastsProperty, value);
    }

    public double Gap
    {
        get => GetValue(GapProperty);
        set => SetValue(GapProperty, value);
    }

    public Thickness Offset
    {
        get => GetValue(OffsetProperty);
        set => SetValue(OffsetProperty, value);
    }

    public bool Expand
    {
        get => GetValue(ExpandProperty);
        set => SetValue(ExpandProperty, value);
    }

    public bool RichColors
    {
        get => GetValue(RichColorsProperty);
        set => SetValue(RichColorsProperty, value);
    }

    public bool CloseButton
    {
        get => GetValue(CloseButtonProperty);
        set => SetValue(CloseButtonProperty, value);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        ((INotifyCollectionChanged)CodexSonnerService.Toasts).CollectionChanged += OnToastsChanged;
        RefreshItems();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        ((INotifyCollectionChanged)CodexSonnerService.Toasts).CollectionChanged -= OnToastsChanged;
        foreach (var toast in CodexSonnerService.Toasts)
        {
            UnsubscribeToast(toast);
        }

        _toastVisuals.Clear();
        Children.Clear();
        base.OnDetachedFromVisualTree(e);
    }

    private void OnToastsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        RefreshItems();
    }

    private void SyncLayout()
    {
        Spacing = Gap;
        Margin = Offset;

        HorizontalAlignment = Position switch
        {
            CodexSonnerPosition.TopLeft or CodexSonnerPosition.BottomLeft => HorizontalAlignment.Left,
            CodexSonnerPosition.TopCenter or CodexSonnerPosition.BottomCenter => HorizontalAlignment.Center,
            _ => HorizontalAlignment.Right
        };

        VerticalAlignment = IsTopPosition(Position)
            ? VerticalAlignment.Top
            : VerticalAlignment.Bottom;

        SyncClasses();
    }

    private void SyncClasses()
    {
        Classes.Set("position-top-left", Position == CodexSonnerPosition.TopLeft);
        Classes.Set("position-top-center", Position == CodexSonnerPosition.TopCenter);
        Classes.Set("position-top-right", Position == CodexSonnerPosition.TopRight);
        Classes.Set("position-bottom-left", Position == CodexSonnerPosition.BottomLeft);
        Classes.Set("position-bottom-center", Position == CodexSonnerPosition.BottomCenter);
        Classes.Set("position-bottom-right", Position == CodexSonnerPosition.BottomRight);
        Classes.Set("expand", Expand);
        Classes.Set("compact", !Expand);
        Classes.Set("rich-colors", RichColors);
        Classes.Set("close-visible", CloseButton);
        Classes.Set("close-hidden", !CloseButton);
    }

    private void RefreshItems()
    {
        var count = Math.Max(1, VisibleToasts);
        var visibleItems = CodexSonnerService.Toasts
            .Take(Expand ? count : 1)
            .ToArray();

        if (!IsTopPosition(Position))
        {
            Array.Reverse(visibleItems);
        }

        var visibleIds = visibleItems.Select(toast => toast.Id).ToHashSet();
        foreach (var id in _toastVisuals.Keys.Where(id => !visibleIds.Contains(id)).ToArray())
        {
            RemoveVisual(id);
        }

        for (var index = 0; index < visibleItems.Length; index++)
        {
            var toast = visibleItems[index];
            var isNew = !_toastVisuals.ContainsKey(toast.Id);
            var visual = GetOrCreateVisual(toast);

            ApplyToastProperties(toast, visual.Toast);
            MoveVisualToIndex(visual.Host, index);

            if (toast.IsClosing)
            {
                CloseToastHost(visual.Host);
            }
            else if (isNew)
            {
                ScheduleOpen(toast.Id);
            }
            else if (!visual.Host.Classes.Contains("entering"))
            {
                OpenToastHost(visual.Host);
            }
        }

        Classes.Set("empty", visibleItems.Length == 0);
    }

    private SonnerToastVisual GetOrCreateVisual(CodexSonnerToast toast)
    {
        if (_toastVisuals.TryGetValue(toast.Id, out var visual))
        {
            return visual;
        }

        var toastControl = new CodexToast();
        ApplyToastProperties(toast, toastControl);

        visual = new SonnerToastVisual(CreateAnimatedHost(toastControl), toastControl, toast);
        _toastVisuals[toast.Id] = visual;
        SubscribeToast(toast);

        return visual;
    }

    private void ApplyToastProperties(CodexSonnerToast toast, CodexToast toastControl)
    {
        toastControl.Title = toast.Title;
        toastControl.Description = toast.Description;
        toastControl.Icon = CreateIcon(toast);
        toastControl.Action = BuildActions(toast);
        toastControl.CloseCommand = toast.DismissCommand;
        toastControl.IsCloseVisible = CloseButton && toast.IsCloseVisible && !toast.IsClosing;
        toastControl.Variant = ResolveVariant(toast);
    }

    private Border CreateAnimatedHost(CodexToast toast)
    {
        var host = new Border
        {
            Child = toast,
            ClipToBounds = true,
            Opacity = 0,
            MaxHeight = 0,
            Margin = ClosedMargin()
        };

        host.Classes.Set("sonner-toast", true);
        host.Classes.Set("entering", true);
        host.Classes.Set("open", false);
        host.Classes.Set("closing", false);

        return host;
    }

    private void ApplyToastState(CodexSonnerToast toast, Border host)
    {
        if (toast.IsClosing)
        {
            CloseToastHost(host);
        }
        else
        {
            OpenToastHost(host);
        }
    }

    private void ScheduleOpen(Guid id)
    {
        if (CodexSonnerService.EnterDuration <= TimeSpan.Zero)
        {
            if (_toastVisuals.TryGetValue(id, out var visual) && !visual.Model.IsClosing)
            {
                OpenToastHost(visual.Host);
            }

            return;
        }

        var timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(16)
        };

        timer.Tick += (_, _) =>
        {
            timer.Stop();
            if (_toastVisuals.TryGetValue(id, out var visual) && !visual.Model.IsClosing)
            {
                OpenToastHost(visual.Host);
            }
        };
        timer.Start();
    }

    private void OpenToastHost(Border host)
    {
        host.Classes.Set("entering", false);
        host.Classes.Set("open", true);
        host.Classes.Set("closing", false);
        host.Opacity = 1;
        host.MaxHeight = ToastExpandedMaxHeight;
        host.Margin = new Thickness(0);
    }

    private void CloseToastHost(Border host)
    {
        host.Classes.Set("entering", false);
        host.Classes.Set("open", false);
        host.Classes.Set("closing", true);
        host.Opacity = 0;
        host.MaxHeight = 0;
        host.Margin = ClosedMargin();
    }

    private Thickness ClosedMargin()
    {
        return IsTopPosition(Position)
            ? new Thickness(0, -ToastAnimationOffset, 0, ToastAnimationOffset)
            : new Thickness(0, ToastAnimationOffset, 0, -ToastAnimationOffset);
    }

    private void MoveVisualToIndex(Border host, int targetIndex)
    {
        var currentIndex = Children.IndexOf(host);
        if (currentIndex == targetIndex)
        {
            return;
        }

        if (currentIndex >= 0)
        {
            Children.RemoveAt(currentIndex);
        }

        Children.Insert(Math.Min(targetIndex, Children.Count), host);
    }

    private void SubscribeToast(CodexSonnerToast toast)
    {
        if (_toastSubscriptions.ContainsKey(toast.Id))
        {
            return;
        }

        PropertyChangedEventHandler handler = (_, e) =>
        {
            if (e.PropertyName is nameof(CodexSonnerToast.IsClosing) or nameof(CodexSonnerToast.IsOpen))
            {
                if (_toastVisuals.TryGetValue(toast.Id, out var visual))
                {
                    ApplyToastProperties(toast, visual.Toast);
                    ApplyToastState(toast, visual.Host);
                }
            }
        };

        toast.PropertyChanged += handler;
        _toastSubscriptions[toast.Id] = handler;
    }

    private void UnsubscribeToast(CodexSonnerToast toast)
    {
        if (!_toastSubscriptions.Remove(toast.Id, out var handler))
        {
            return;
        }

        toast.PropertyChanged -= handler;
    }

    private void RemoveVisual(Guid id)
    {
        if (_toastVisuals.Remove(id, out var visual))
        {
            Children.Remove(visual.Host);
            UnsubscribeToast(visual.Model);
        }
    }

    private CodexControlVariant ResolveVariant(CodexSonnerToast toast)
    {
        if (toast.VariantOverride is { } variant)
        {
            return variant;
        }

        return RichColors ? toast.Variant : CodexControlVariant.Default;
    }

    private static Control? BuildActions(CodexSonnerToast toast)
    {
        if (toast.ActionCommand is null && toast.CancelCommand is null)
        {
            return null;
        }

        var actions = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 6,
            VerticalAlignment = VerticalAlignment.Center
        };

        if (toast.CancelCommand is not null)
        {
            actions.Children.Add(new CodexButton
            {
                Content = toast.CancelLabel,
                Command = toast.CancelCommand,
                Size = CodexControlSize.Small,
                Variant = CodexControlVariant.Secondary
            });
        }

        if (toast.ActionCommand is not null)
        {
            actions.Children.Add(new CodexButton
            {
                Content = toast.ActionLabel,
                Command = toast.ActionCommand,
                Size = CodexControlSize.Small
            });
        }

        return actions;
    }

    private static object? CreateIcon(CodexSonnerToast toast)
    {
        if (toast.Icon is not null)
        {
            return toast.Icon;
        }

        if (toast.Type == CodexSonnerToastType.Default)
        {
            return null;
        }

        var brush = IconBrush(toast.Type);
        if (toast.Type == CodexSonnerToastType.Loading)
        {
            return new CodexSpinner
            {
                Size = CodexControlSize.Small,
                Label = "Loading toast",
                Foreground = brush
            };
        }

        return new Border
        {
            Width = 16,
            Height = 16,
            CornerRadius = new CornerRadius(8),
            BorderBrush = brush,
            BorderThickness = new Thickness(1),
            Child = new TextBlock
            {
                Text = IconText(toast.Type),
                FontSize = 10,
                FontWeight = FontWeight.SemiBold,
                Foreground = brush,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Center
            }
        };
    }

    private static string IconText(CodexSonnerToastType type)
    {
        return type switch
        {
            CodexSonnerToastType.Success => "OK",
            CodexSonnerToastType.Info => "i",
            CodexSonnerToastType.Warning => "!",
            CodexSonnerToastType.Error => "x",
            _ => "."
        };
    }

    private static IBrush IconBrush(CodexSonnerToastType type)
    {
        var key = type switch
        {
            CodexSonnerToastType.Success => CodexSwitchResourceKeys.SuccessBrush,
            CodexSonnerToastType.Warning => CodexSwitchResourceKeys.WarningBrush,
            CodexSonnerToastType.Error => CodexSwitchResourceKeys.DestructiveBrush,
            CodexSonnerToastType.Loading => CodexSwitchResourceKeys.MutedForegroundBrush,
            _ => CodexSwitchResourceKeys.ForegroundBrush
        };

        if (Application.Current?.TryFindResource(key, null, out var value) == true && value is IBrush brush)
        {
            return brush;
        }

        return type switch
        {
            CodexSonnerToastType.Success => Brushes.SeaGreen,
            CodexSonnerToastType.Warning => Brushes.DarkGoldenrod,
            CodexSonnerToastType.Error => Brushes.IndianRed,
            CodexSonnerToastType.Loading => Brushes.Gray,
            _ => Brushes.DimGray
        };
    }

    private static bool IsTopPosition(CodexSonnerPosition position)
    {
        return position is CodexSonnerPosition.TopLeft
            or CodexSonnerPosition.TopCenter
            or CodexSonnerPosition.TopRight;
    }

    private sealed record SonnerToastVisual(Border Host, CodexToast Toast, CodexSonnerToast Model);
}

public enum CodexSonnerPosition
{
    TopLeft,
    TopCenter,
    TopRight,
    BottomLeft,
    BottomCenter,
    BottomRight
}

public enum CodexSonnerToastType
{
    Default,
    Success,
    Info,
    Warning,
    Error,
    Loading
}

public sealed class CodexSonnerOptions
{
    public string? Description { get; init; }

    public CodexControlVariant? Variant { get; init; }

    public object? Icon { get; init; }

    public CodexSonnerAction? Action { get; init; }

    public CodexSonnerAction? Cancel { get; init; }

    public TimeSpan? Duration { get; init; }

    public bool CloseButton { get; init; } = true;
}

public sealed class CodexSonnerAction
{
    public CodexSonnerAction(string label, Action callback)
        : this(label, new CodexSonnerCommand(callback))
    {
    }

    public CodexSonnerAction(string label, ICommand command)
    {
        Label = label;
        Command = command;
    }

    public string Label { get; }

    public ICommand Command { get; }
}

public sealed class CodexSonnerToast : INotifyPropertyChanged
{
    private DispatcherTimer? _timer;
    private DispatcherTimer? _dismissTimer;
    private bool _isClosing;

    internal CodexSonnerToast(
        string title,
        CodexSonnerToastType type,
        CodexSonnerOptions options)
    {
        Id = Guid.NewGuid();
        Title = title;
        Type = type;
        Description = options.Description;
        VariantOverride = options.Variant;
        Icon = options.Icon;
        ActionLabel = options.Action?.Label;
        ActionCommand = options.Action?.Command;
        CancelLabel = options.Cancel?.Label;
        CancelCommand = options.Cancel?.Command;
        IsCloseVisible = options.CloseButton;
        DismissCommand = new CodexSonnerCommand(() => CodexSonnerService.Dismiss(Id));
    }

    public Guid Id { get; }

    public string Title { get; }

    public string? Description { get; }

    public CodexSonnerToastType Type { get; }

    public CodexControlVariant Variant => Type switch
    {
        CodexSonnerToastType.Success => CodexControlVariant.Success,
        CodexSonnerToastType.Warning => CodexControlVariant.Warning,
        CodexSonnerToastType.Error => CodexControlVariant.Destructive,
        CodexSonnerToastType.Loading => CodexControlVariant.Secondary,
        _ => CodexControlVariant.Default
    };

    public CodexControlVariant? VariantOverride { get; }

    public object? Icon { get; }

    public string? ActionLabel { get; }

    public ICommand? ActionCommand { get; }

    public string? CancelLabel { get; }

    public ICommand? CancelCommand { get; }

    public bool IsCloseVisible { get; }

    public bool IsClosing
    {
        get => _isClosing;
        private set
        {
            if (_isClosing == value)
            {
                return;
            }

            _isClosing = value;
            OnPropertyChanged(nameof(IsClosing));
            OnPropertyChanged(nameof(IsOpen));
        }
    }

    public bool IsOpen => !IsClosing;

    public ICommand DismissCommand { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    internal void StartTimer(TimeSpan duration)
    {
        StopTimer();

        _timer = new DispatcherTimer
        {
            Interval = duration
        };
        _timer.Tick += (_, _) =>
        {
            StopTimer();
            CodexSonnerService.Dismiss(Id);
        };
        _timer.Start();
    }

    internal void BeginDismiss(TimeSpan exitDuration, Action<CodexSonnerToast> remove)
    {
        if (IsClosing)
        {
            return;
        }

        StopTimer();
        StopDismissTimer();
        IsClosing = true;

        if (exitDuration <= TimeSpan.Zero)
        {
            remove(this);
            return;
        }

        _dismissTimer = new DispatcherTimer
        {
            Interval = exitDuration
        };
        _dismissTimer.Tick += (_, _) =>
        {
            StopDismissTimer();
            remove(this);
        };
        _dismissTimer.Start();
    }

    internal void StopTimers()
    {
        StopTimer();
        StopDismissTimer();
    }

    private void StopTimer()
    {
        _timer?.Stop();
        _timer = null;
    }

    private void StopDismissTimer()
    {
        _dismissTimer?.Stop();
        _dismissTimer = null;
    }

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public static class CodexSonnerService
{
    private static readonly ObservableCollection<CodexSonnerToast> MutableToasts = [];
    private static TimeSpan? _enterDurationOverride;
    private static TimeSpan? _exitDurationOverride;

    public static readonly ReadOnlyObservableCollection<CodexSonnerToast> Toasts = new(MutableToasts);

    public static TimeSpan DefaultDuration { get; set; } = TimeSpan.FromSeconds(4);

    public static TimeSpan EnterDuration
    {
        get => _enterDurationOverride ?? CodexMotion.ResolveDefaultDuration();
        set => _enterDurationOverride = value;
    }

    public static TimeSpan ExitDuration
    {
        get => _exitDurationOverride ?? CodexMotion.ResolveDefaultDuration();
        set => _exitDurationOverride = value;
    }

    public static int ToastLimit { get; set; } = 8;

    public static CodexSonnerToast Toast(string title, CodexSonnerOptions? options = null)
    {
        return Show(title, CodexSonnerToastType.Default, options);
    }

    public static CodexSonnerToast Success(string title, CodexSonnerOptions? options = null)
    {
        return Show(title, CodexSonnerToastType.Success, options);
    }

    public static CodexSonnerToast Info(string title, CodexSonnerOptions? options = null)
    {
        return Show(title, CodexSonnerToastType.Info, options);
    }

    public static CodexSonnerToast Warning(string title, CodexSonnerOptions? options = null)
    {
        return Show(title, CodexSonnerToastType.Warning, options);
    }

    public static CodexSonnerToast Error(string title, CodexSonnerOptions? options = null)
    {
        return Show(title, CodexSonnerToastType.Error, options);
    }

    public static CodexSonnerToast Loading(string title, CodexSonnerOptions? options = null)
    {
        return Show(title, CodexSonnerToastType.Loading, options);
    }

    public static CodexSonnerToast Show(string title, CodexSonnerToastType type, CodexSonnerOptions? options = null)
    {
        var toastOptions = options ?? new CodexSonnerOptions();
        var toast = new CodexSonnerToast(title, type, toastOptions);

        MutableToasts.Insert(0, toast);
        TrimToLimit();

        var duration = toastOptions.Duration ?? (type == CodexSonnerToastType.Loading ? TimeSpan.Zero : DefaultDuration);
        if (duration > TimeSpan.Zero)
        {
            toast.StartTimer(duration);
        }

        return toast;
    }

    public static void Dismiss(Guid id)
    {
        var toast = MutableToasts.FirstOrDefault(item => item.Id == id);
        if (toast is null)
        {
            return;
        }

        toast.BeginDismiss(ExitDuration, RemoveToast);
    }

    public static void Dismiss(CodexSonnerToast toast)
    {
        Dismiss(toast.Id);
    }

    public static void Clear()
    {
        foreach (var toast in MutableToasts)
        {
            toast.StopTimers();
        }

        MutableToasts.Clear();
    }

    private static void TrimToLimit()
    {
        var limit = Math.Max(1, ToastLimit);
        while (MutableToasts.Count(toast => !toast.IsClosing) > limit)
        {
            var toast = MutableToasts.LastOrDefault(item => !item.IsClosing);
            if (toast is null)
            {
                return;
            }

            toast.BeginDismiss(ExitDuration, RemoveToast);
        }
    }

    private static void RemoveToast(CodexSonnerToast toast)
    {
        toast.StopTimers();
        MutableToasts.Remove(toast);
    }
}

internal sealed class CodexSonnerCommand : ICommand
{
    private readonly Action _execute;

    public CodexSonnerCommand(Action execute)
    {
        _execute = execute;
    }

    public event EventHandler? CanExecuteChanged
    {
        add { }
        remove { }
    }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        _execute();
    }
}
