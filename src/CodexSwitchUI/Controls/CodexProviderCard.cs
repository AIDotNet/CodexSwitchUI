using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;
using System.Windows.Input;

namespace CodexSwitchUI.Controls;

public enum CodexProviderCardSelectionSource
{
    Programmatic,
    Pointer,
    Keyboard
}

public sealed class CodexProviderCardSelectedEventArgs(
    object? commandParameter,
    CodexProviderCardSelectionSource source = CodexProviderCardSelectionSource.Programmatic) : EventArgs
{
    public object? CommandParameter { get; } = commandParameter;

    public CodexProviderCardSelectionSource Source { get; } = source;
}

public class CodexProviderCard : Button
{
    private ICommand? _subscribedCommand;
    private bool _hasPrimaryPointerPress;
    private PointerUpdateKind? _pendingPointerReleaseKind;
    private CodexProviderCardSelectionSource? _pendingSelectionSource;
    private bool _selectionHandledByPointerRelease;

    public static readonly StyledProperty<bool> IsActiveProperty =
        AvaloniaProperty.Register<CodexProviderCard, bool>(nameof(IsActive));

    public static readonly StyledProperty<bool> IsDraggingProperty =
        AvaloniaProperty.Register<CodexProviderCard, bool>(nameof(IsDragging));

    public static readonly StyledProperty<object?> LeadingProperty =
        AvaloniaProperty.Register<CodexProviderCard, object?>(nameof(Leading));

    public static readonly StyledProperty<object?> IconProperty =
        AvaloniaProperty.Register<CodexProviderCard, object?>(nameof(Icon));

    public static readonly StyledProperty<object?> HeaderProperty =
        AvaloniaProperty.Register<CodexProviderCard, object?>(nameof(Header));

    public static readonly StyledProperty<object?> MetaProperty =
        AvaloniaProperty.Register<CodexProviderCard, object?>(nameof(Meta));

    public static readonly StyledProperty<string?> DescriptionProperty =
        AvaloniaProperty.Register<CodexProviderCard, string?>(nameof(Description));

    public static readonly StyledProperty<object?> StatusProperty =
        AvaloniaProperty.Register<CodexProviderCard, object?>(nameof(Status));

    public static readonly StyledProperty<object?> UsageProperty =
        AvaloniaProperty.Register<CodexProviderCard, object?>(nameof(Usage));

    public static readonly StyledProperty<object?> ActionsProperty =
        AvaloniaProperty.Register<CodexProviderCard, object?>(nameof(Actions));

    public static readonly StyledProperty<bool> HasLeadingProperty =
        AvaloniaProperty.Register<CodexProviderCard, bool>(nameof(HasLeading));

    public static readonly StyledProperty<bool> HasIconProperty =
        AvaloniaProperty.Register<CodexProviderCard, bool>(nameof(HasIcon));

    public static readonly StyledProperty<bool> HasMetaProperty =
        AvaloniaProperty.Register<CodexProviderCard, bool>(nameof(HasMeta));

    public static readonly StyledProperty<bool> HasDescriptionProperty =
        AvaloniaProperty.Register<CodexProviderCard, bool>(nameof(HasDescription));

    public static readonly StyledProperty<bool> HasStatusProperty =
        AvaloniaProperty.Register<CodexProviderCard, bool>(nameof(HasStatus));

    public static readonly StyledProperty<bool> HasUsageProperty =
        AvaloniaProperty.Register<CodexProviderCard, bool>(nameof(HasUsage));

    public static readonly StyledProperty<bool> HasActionsProperty =
        AvaloniaProperty.Register<CodexProviderCard, bool>(nameof(HasActions));

    static CodexProviderCard()
    {
        IsActiveProperty.Changed.AddClassHandler<CodexProviderCard>((card, _) => card.SyncClasses());
        IsDraggingProperty.Changed.AddClassHandler<CodexProviderCard>((card, _) => card.SyncClasses());
        LeadingProperty.Changed.AddClassHandler<CodexProviderCard>((card, _) => card.SyncSlots());
        IconProperty.Changed.AddClassHandler<CodexProviderCard>((card, _) => card.SyncSlots());
        MetaProperty.Changed.AddClassHandler<CodexProviderCard>((card, _) => card.SyncSlots());
        DescriptionProperty.Changed.AddClassHandler<CodexProviderCard>((card, _) => card.SyncSlots());
        StatusProperty.Changed.AddClassHandler<CodexProviderCard>((card, _) => card.SyncSlots());
        UsageProperty.Changed.AddClassHandler<CodexProviderCard>((card, _) => card.SyncSlots());
        ActionsProperty.Changed.AddClassHandler<CodexProviderCard>((card, _) => card.SyncSlots());
        CommandProperty.Changed.AddClassHandler<CodexProviderCard>((card, args) => card.OnCommandChanged(args.OldValue as ICommand, args.NewValue as ICommand));
        CommandParameterProperty.Changed.AddClassHandler<CodexProviderCard>((card, _) => card.SyncClasses());
        IsEnabledProperty.Changed.AddClassHandler<CodexProviderCard>((card, _) => card.SyncClasses());
    }

    public CodexProviderCard()
    {
        SyncClasses();
        SyncSlots();
    }

    public event EventHandler<CodexProviderCardSelectedEventArgs>? Selected;

    public bool IsActive
    {
        get => GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }

    public bool IsDragging
    {
        get => GetValue(IsDraggingProperty);
        set => SetValue(IsDraggingProperty, value);
    }

    public object? Leading
    {
        get => GetValue(LeadingProperty);
        set => SetValue(LeadingProperty, value);
    }

    public object? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public object? Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public object? Meta
    {
        get => GetValue(MetaProperty);
        set => SetValue(MetaProperty, value);
    }

    public string? Description
    {
        get => GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public object? Status
    {
        get => GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }

    public object? Usage
    {
        get => GetValue(UsageProperty);
        set => SetValue(UsageProperty, value);
    }

    public object? Actions
    {
        get => GetValue(ActionsProperty);
        set => SetValue(ActionsProperty, value);
    }

    public bool HasLeading => GetValue(HasLeadingProperty);

    public bool HasIcon => GetValue(HasIconProperty);

    public bool HasMeta => GetValue(HasMetaProperty);

    public bool HasDescription => GetValue(HasDescriptionProperty);

    public bool HasStatus => GetValue(HasStatusProperty);

    public bool HasUsage => GetValue(HasUsageProperty);

    public bool HasActions => GetValue(HasActionsProperty);

    protected override void OnClick()
    {
        if (_pendingPointerReleaseKind is { } updateKind)
        {
            if (updateKind != PointerUpdateKind.LeftButtonReleased || !CanSelect())
            {
                return;
            }

            base.OnClick();
            _selectionHandledByPointerRelease = TryHandlePointerActivation(updateKind);
            return;
        }

        if (!CanSelect())
        {
            return;
        }

        base.OnClick();
        _ = TrySelect(_pendingSelectionSource ?? CodexProviderCardSelectionSource.Programmatic);
    }

    internal bool TrySelect()
    {
        return TrySelect(CodexProviderCardSelectionSource.Programmatic);
    }

    internal bool TrySelect(CodexProviderCardSelectionSource source)
    {
        if (!CanSelect())
        {
            return false;
        }

        SelectSiblingCards();
        Selected?.Invoke(this, new CodexProviderCardSelectedEventArgs(CommandParameter, source));
        return true;
    }

    internal bool TryHandlePointerActivation(PointerUpdateKind updateKind)
    {
        if (updateKind != PointerUpdateKind.LeftButtonReleased)
        {
            return false;
        }

        return TrySelect(CodexProviderCardSelectionSource.Pointer);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (e.Key is Key.Enter or Key.Space)
        {
            RunWithSelectionSource(CodexProviderCardSelectionSource.Keyboard, () => base.OnKeyDown(e));
            return;
        }

        base.OnKeyDown(e);
    }

    protected override void OnKeyUp(KeyEventArgs e)
    {
        if (e.Key is Key.Enter or Key.Space)
        {
            RunWithSelectionSource(CodexProviderCardSelectionSource.Keyboard, () => base.OnKeyUp(e));
            return;
        }

        base.OnKeyUp(e);
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        _hasPrimaryPointerPress = e.GetCurrentPoint(this).Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonPressed;
        base.OnPointerPressed(e);
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        var updateKind = e.GetCurrentPoint(this).Properties.PointerUpdateKind;
        var canActivateFromPointer = _hasPrimaryPointerPress && IsPointerOver;
        _hasPrimaryPointerPress = false;

        if (canActivateFromPointer)
        {
            _pendingPointerReleaseKind = updateKind;
            try
            {
                base.OnPointerReleased(e);
            }
            finally
            {
                _pendingPointerReleaseKind = null;
            }

            if (_selectionHandledByPointerRelease)
            {
                _selectionHandledByPointerRelease = false;
                e.Handled = true;
            }

            return;
        }

        base.OnPointerReleased(e);
    }

    private bool CanSelect()
    {
        return IsEnabled
               && !IsDragging
               && (Command?.CanExecute(CommandParameter) ?? true);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        if (_subscribedCommand is not null)
        {
            _subscribedCommand.CanExecuteChanged -= OnCommandCanExecuteChanged;
            _subscribedCommand = null;
        }

        base.OnDetachedFromVisualTree(e);
    }

    private void SelectSiblingCards()
    {
        var parent = this.GetLogicalParent();
        if (parent is null)
        {
            IsActive = true;
            return;
        }

        foreach (var child in parent.GetLogicalChildren())
        {
            if (child is CodexProviderCard card)
            {
                card.IsActive = ReferenceEquals(card, this);
            }
        }
    }

    private void SyncClasses()
    {
        Classes.Set("active", IsActive);
        Classes.Set("dragging", IsDragging);
        Classes.Set("can-select", CanSelect());
        Classes.Set("command-blocked", Command is not null && IsEnabled && !IsDragging && !CanSelect());
    }

    private void SyncSlots()
    {
        SetValue(HasLeadingProperty, Leading is not null);
        SetValue(HasIconProperty, Icon is not null);
        SetValue(HasMetaProperty, Meta is not null);
        SetValue(HasDescriptionProperty, !string.IsNullOrWhiteSpace(Description));
        SetValue(HasStatusProperty, Status is not null);
        SetValue(HasUsageProperty, Usage is not null);
        SetValue(HasActionsProperty, Actions is not null);
    }

    private void OnCommandChanged(ICommand? oldCommand, ICommand? newCommand)
    {
        if (ReferenceEquals(oldCommand, newCommand))
        {
            return;
        }

        if (_subscribedCommand is not null)
        {
            _subscribedCommand.CanExecuteChanged -= OnCommandCanExecuteChanged;
        }

        _subscribedCommand = newCommand;

        if (_subscribedCommand is not null)
        {
            _subscribedCommand.CanExecuteChanged += OnCommandCanExecuteChanged;
        }

        SyncClasses();
    }

    private void OnCommandCanExecuteChanged(object? sender, EventArgs e)
    {
        SyncClasses();
    }

    private void RunWithSelectionSource(CodexProviderCardSelectionSource source, Action action)
    {
        var previousSource = _pendingSelectionSource;
        _pendingSelectionSource = source;
        try
        {
            action();
        }
        finally
        {
            _pendingSelectionSource = previousSource;
        }
    }
}
