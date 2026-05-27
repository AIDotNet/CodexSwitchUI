using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using System.Windows.Input;

namespace CodexSwitchUI.Controls;

public enum CodexPopoverAlign
{
    Center,
    Start,
    End
}

public sealed class CodexPopoverOpenChangedEventArgs(bool isOpen) : EventArgs
{
    public bool IsOpen { get; } = isOpen;
}

public class CodexPopover : CodexFrame
{
    private Control? _triggerPresenter;

    public static readonly StyledProperty<object?> TriggerProperty =
        AvaloniaProperty.Register<CodexPopover, object?>(nameof(Trigger));

    public static readonly StyledProperty<IDataTemplate?> TriggerTemplateProperty =
        AvaloniaProperty.Register<CodexPopover, IDataTemplate?>(nameof(TriggerTemplate));

    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<CodexPopover, string?>(nameof(Title));

    public static readonly StyledProperty<string?> DescriptionProperty =
        AvaloniaProperty.Register<CodexPopover, string?>(nameof(Description));

    public static readonly StyledProperty<object?> ActionProperty =
        AvaloniaProperty.Register<CodexPopover, object?>(nameof(Action));

    public static readonly StyledProperty<object?> CloseContentProperty =
        AvaloniaProperty.Register<CodexPopover, object?>(nameof(CloseContent));

    public static readonly StyledProperty<ICommand?> CloseCommandProperty =
        AvaloniaProperty.Register<CodexPopover, ICommand?>(nameof(CloseCommand));

    public static readonly StyledProperty<ICommand?> DismissCommandProperty =
        AvaloniaProperty.Register<CodexPopover, ICommand?>(nameof(DismissCommand));

    public static readonly StyledProperty<bool> IsOpenProperty =
        AvaloniaProperty.Register<CodexPopover, bool>(nameof(IsOpen));

    public static readonly StyledProperty<PlacementMode> PlacementProperty =
        AvaloniaProperty.Register<CodexPopover, PlacementMode>(nameof(Placement), PlacementMode.Bottom);

    public static readonly StyledProperty<CodexPopoverAlign> AlignProperty =
        AvaloniaProperty.Register<CodexPopover, CodexPopoverAlign>(nameof(Align), CodexPopoverAlign.Center);

    public static readonly StyledProperty<bool> IsArrowVisibleProperty =
        AvaloniaProperty.Register<CodexPopover, bool>(nameof(IsArrowVisible));

    public static readonly StyledProperty<bool> IsCloseVisibleProperty =
        AvaloniaProperty.Register<CodexPopover, bool>(nameof(IsCloseVisible), true);

    public static readonly StyledProperty<bool> CloseOnEscapeProperty =
        AvaloniaProperty.Register<CodexPopover, bool>(nameof(CloseOnEscape), true);

    public static readonly StyledProperty<bool> DismissOnOutsidePointerProperty =
        AvaloniaProperty.Register<CodexPopover, bool>(nameof(DismissOnOutsidePointer), true);

    public static readonly StyledProperty<IInputElement?> RestoreFocusElementProperty =
        AvaloniaProperty.Register<CodexPopover, IInputElement?>(nameof(RestoreFocusElement));

    public static readonly StyledProperty<bool> RestoreFocusOnDismissProperty =
        AvaloniaProperty.Register<CodexPopover, bool>(nameof(RestoreFocusOnDismiss), true);

    public static readonly StyledProperty<bool> HasTitleProperty =
        AvaloniaProperty.Register<CodexPopover, bool>(nameof(HasTitle));

    public static readonly StyledProperty<bool> HasDescriptionProperty =
        AvaloniaProperty.Register<CodexPopover, bool>(nameof(HasDescription));

    public static readonly StyledProperty<bool> HasHeaderProperty =
        AvaloniaProperty.Register<CodexPopover, bool>(nameof(HasHeader));

    public static readonly StyledProperty<bool> HasContentProperty =
        AvaloniaProperty.Register<CodexPopover, bool>(nameof(HasContent));

    public static readonly StyledProperty<bool> HasActionProperty =
        AvaloniaProperty.Register<CodexPopover, bool>(nameof(HasAction));

    public static readonly StyledProperty<bool> HasTriggerProperty =
        AvaloniaProperty.Register<CodexPopover, bool>(nameof(HasTrigger));

    public static readonly StyledProperty<bool> HasRestoreFocusTargetProperty =
        AvaloniaProperty.Register<CodexPopover, bool>(nameof(HasRestoreFocusTarget));

    static CodexPopover()
    {
        TriggerProperty.Changed.AddClassHandler<CodexPopover>((popover, _) => popover.SyncSlotStates());
        TitleProperty.Changed.AddClassHandler<CodexPopover>((popover, _) => popover.SyncSlotStates());
        DescriptionProperty.Changed.AddClassHandler<CodexPopover>((popover, _) => popover.SyncSlotStates());
        ContentControl.ContentProperty.Changed.AddClassHandler<CodexPopover>((popover, _) => popover.SyncSlotStates());
        ActionProperty.Changed.AddClassHandler<CodexPopover>((popover, _) => popover.SyncSlotStates());
        CloseContentProperty.Changed.AddClassHandler<CodexPopover>((popover, _) => popover.SyncSlotStates());
        IsOpenProperty.Changed.AddClassHandler<CodexPopover>((popover, args) => popover.OnOpenChanged(args));
        PlacementProperty.Changed.AddClassHandler<CodexPopover>((popover, _) => popover.SyncOpenState());
        AlignProperty.Changed.AddClassHandler<CodexPopover>((popover, _) => popover.SyncOpenState());
        IsArrowVisibleProperty.Changed.AddClassHandler<CodexPopover>((popover, _) => popover.SyncOpenState());
        IsCloseVisibleProperty.Changed.AddClassHandler<CodexPopover>((popover, _) => popover.SyncSlotStates());
        RestoreFocusElementProperty.Changed.AddClassHandler<CodexPopover>((popover, _) => popover.SyncRestoreFocusState());
        RestoreFocusOnDismissProperty.Changed.AddClassHandler<CodexPopover>((popover, _) => popover.SyncRestoreFocusState());
    }

    public CodexPopover()
    {
        DismissCommand = new CodexDismissCommand(Dismiss);
        SyncOpenState();
        SyncSlotStates();
        SyncRestoreFocusState();
    }

    public event EventHandler<RestoreFocusRequestedEventArgs>? RestoreFocusRequested;

    public event EventHandler<CodexPopoverOpenChangedEventArgs>? OpenChanged;

    public object? Trigger
    {
        get => GetValue(TriggerProperty);
        set => SetValue(TriggerProperty, value);
    }

    public IDataTemplate? TriggerTemplate
    {
        get => GetValue(TriggerTemplateProperty);
        set => SetValue(TriggerTemplateProperty, value);
    }

    public string? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string? Description
    {
        get => GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public object? Action
    {
        get => GetValue(ActionProperty);
        set => SetValue(ActionProperty, value);
    }

    public object? CloseContent
    {
        get => GetValue(CloseContentProperty);
        set => SetValue(CloseContentProperty, value);
    }

    public ICommand? CloseCommand
    {
        get => GetValue(CloseCommandProperty);
        set => SetValue(CloseCommandProperty, value);
    }

    public ICommand? DismissCommand
    {
        get => GetValue(DismissCommandProperty);
        private set => SetValue(DismissCommandProperty, value);
    }

    public bool IsOpen
    {
        get => GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    public PlacementMode Placement
    {
        get => GetValue(PlacementProperty);
        set => SetValue(PlacementProperty, value);
    }

    public CodexPopoverAlign Align
    {
        get => GetValue(AlignProperty);
        set => SetValue(AlignProperty, value);
    }

    public bool IsArrowVisible
    {
        get => GetValue(IsArrowVisibleProperty);
        set => SetValue(IsArrowVisibleProperty, value);
    }

    public bool IsCloseVisible
    {
        get => GetValue(IsCloseVisibleProperty);
        set => SetValue(IsCloseVisibleProperty, value);
    }

    public bool CloseOnEscape
    {
        get => GetValue(CloseOnEscapeProperty);
        set => SetValue(CloseOnEscapeProperty, value);
    }

    public bool DismissOnOutsidePointer
    {
        get => GetValue(DismissOnOutsidePointerProperty);
        set => SetValue(DismissOnOutsidePointerProperty, value);
    }

    public IInputElement? RestoreFocusElement
    {
        get => GetValue(RestoreFocusElementProperty);
        set => SetValue(RestoreFocusElementProperty, value);
    }

    public bool RestoreFocusOnDismiss
    {
        get => GetValue(RestoreFocusOnDismissProperty);
        set => SetValue(RestoreFocusOnDismissProperty, value);
    }

    public bool HasTitle => GetValue(HasTitleProperty);

    public bool HasDescription => GetValue(HasDescriptionProperty);

    public bool HasHeader => GetValue(HasHeaderProperty);

    public bool HasContent => GetValue(HasContentProperty);

    public bool HasAction => GetValue(HasActionProperty);

    public bool HasTrigger => GetValue(HasTriggerProperty);

    public bool HasRestoreFocusTarget => GetValue(HasRestoreFocusTargetProperty);

    public void Open()
    {
        if (!IsEnabled)
        {
            return;
        }

        IsOpen = true;
    }

    public bool Toggle()
    {
        if (!IsEnabled)
        {
            return false;
        }

        if (IsOpen)
        {
            return Dismiss();
        }

        Open();
        return true;
    }

    public bool Dismiss()
    {
        if (!IsOpen)
        {
            return false;
        }

        IsOpen = false;

        if (CloseCommand?.CanExecute(null) == true)
        {
            CloseCommand.Execute(null);
        }

        return true;
    }

    public bool TryRestoreFocus()
    {
        return RestoreFocusOnDismiss
               && CodexFocusRestore.TryRestore(RestoreFocusElement ?? _triggerPresenter ?? Trigger as IInputElement, RestoreFocusRequested, this);
    }

    internal bool TryHandleDismissKey(Key key)
    {
        return key == Key.Escape && CloseOnEscape && Dismiss();
    }

    internal bool TryHandleTriggerKey(Key key)
    {
        return key is Key.Enter or Key.Space && Toggle();
    }

    internal bool TryToggleFromTrigger()
    {
        return Toggle();
    }

    internal bool TryDismissFromOutsidePointer()
    {
        return DismissOnOutsidePointer && Dismiss();
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        if (_triggerPresenter is not null)
        {
            _triggerPresenter.RemoveHandler(InputElement.PointerReleasedEvent, OnTriggerPointerReleased);
            _triggerPresenter.RemoveHandler(InputElement.KeyDownEvent, OnTriggerKeyDown);
        }

        base.OnApplyTemplate(e);

        _triggerPresenter = e.NameScope.Find<Control>("PART_Trigger");

        if (_triggerPresenter is not null)
        {
            _triggerPresenter.AddHandler(
                InputElement.PointerReleasedEvent,
                OnTriggerPointerReleased,
                RoutingStrategies.Bubble,
                handledEventsToo: true);
            _triggerPresenter.AddHandler(
                InputElement.KeyDownEvent,
                OnTriggerKeyDown,
                RoutingStrategies.Bubble,
                handledEventsToo: true);
        }

        SyncRestoreFocusState();
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (TryHandleDismissKey(e.Key))
        {
            e.Handled = true;
            return;
        }

        base.OnKeyDown(e);
    }

    private void OnTriggerPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (TryToggleFromTrigger())
        {
            e.Handled = true;
        }
    }

    private void OnTriggerKeyDown(object? sender, KeyEventArgs e)
    {
        if (TryHandleTriggerKey(e.Key))
        {
            e.Handled = true;
        }
    }

    private void SyncOpenState()
    {
        Classes.Set("open", IsOpen);
        Classes.Set("closed", !IsOpen);
        Classes.Set("trigger-open", HasTrigger && IsOpen);
        Classes.Set("trigger-closed", HasTrigger && !IsOpen);
        Classes.Set("has-arrow", IsArrowVisible);
        Classes.Set("align-center", Align == CodexPopoverAlign.Center);
        Classes.Set("align-start", Align == CodexPopoverAlign.Start);
        Classes.Set("align-end", Align == CodexPopoverAlign.End);
        Classes.Set("side-top", Placement is PlacementMode.Top or PlacementMode.TopEdgeAlignedLeft or PlacementMode.TopEdgeAlignedRight);
        Classes.Set("side-left", Placement is PlacementMode.Left or PlacementMode.LeftEdgeAlignedTop or PlacementMode.LeftEdgeAlignedBottom);
        Classes.Set("side-right", Placement is PlacementMode.Right or PlacementMode.RightEdgeAlignedTop or PlacementMode.RightEdgeAlignedBottom);
        Classes.Set("side-bottom", Placement is not (PlacementMode.Top or PlacementMode.TopEdgeAlignedLeft or PlacementMode.TopEdgeAlignedRight
            or PlacementMode.Left or PlacementMode.LeftEdgeAlignedTop or PlacementMode.LeftEdgeAlignedBottom
            or PlacementMode.Right or PlacementMode.RightEdgeAlignedTop or PlacementMode.RightEdgeAlignedBottom));
    }

    private void OnOpenChanged(AvaloniaPropertyChangedEventArgs args)
    {
        SyncOpenState();

        if (args.OldValue is bool oldValue && oldValue != IsOpen)
        {
            OpenChanged?.Invoke(this, new CodexPopoverOpenChangedEventArgs(IsOpen));
        }

        if (args.OldValue is true && args.NewValue is false)
        {
            TryRestoreFocus();
        }
    }

    private void SyncRestoreFocusState()
    {
        SetValue(HasRestoreFocusTargetProperty, (RestoreFocusElement ?? _triggerPresenter ?? Trigger as IInputElement) is not null);
        Classes.Set("restore-focus", RestoreFocusOnDismiss);
        Classes.Set("has-restore-focus-target", HasRestoreFocusTarget);
    }

    private void SyncSlotStates()
    {
        var hasTitle = HasValue(Title);
        var hasDescription = HasValue(Description);
        var hasTrigger = HasValue(Trigger);

        SetValue(HasTitleProperty, hasTitle);
        SetValue(HasDescriptionProperty, hasDescription);
        SetValue(HasHeaderProperty, hasTitle || hasDescription);
        SetValue(HasContentProperty, HasValue(Content));
        SetValue(HasActionProperty, HasValue(Action));
        SetValue(HasTriggerProperty, hasTrigger);
        Classes.Set("has-title", hasTitle);
        Classes.Set("has-description", hasDescription);
        Classes.Set("has-header", HasHeader);
        Classes.Set("has-content", HasContent);
        Classes.Set("has-action", HasAction);
        Classes.Set("has-trigger", HasTrigger);
        Classes.Set("has-close", IsCloseVisible);
        Classes.Set("has-close-content", HasValue(CloseContent));
        SyncOpenState();
        SyncRestoreFocusState();
    }

    private static bool HasValue(object? value)
    {
        return value is string text ? !string.IsNullOrWhiteSpace(text) : value is not null;
    }
}
