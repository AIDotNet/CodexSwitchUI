using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using System.Windows.Input;

namespace CodexSwitchUI.Controls;

public enum CodexDialogOpenChangeSource
{
    Programmatic,
    Pointer,
    Keyboard
}

public sealed class CodexDialogOpenChangedEventArgs(
    bool isOpen,
    CodexDialogOpenChangeSource source = CodexDialogOpenChangeSource.Programmatic) : EventArgs
{
    public bool IsOpen { get; } = isOpen;

    public CodexDialogOpenChangeSource Source { get; } = source;
}

public class CodexDialog : CodexFrame
{
    private Control? _triggerPresenter;
    private Control? _overlayPresenter;
    private CodexDialogOpenChangeSource? _pendingOpenChangeSource;

    public static readonly StyledProperty<object?> TriggerProperty =
        AvaloniaProperty.Register<CodexDialog, object?>(nameof(Trigger));

    public static readonly StyledProperty<IDataTemplate?> TriggerTemplateProperty =
        AvaloniaProperty.Register<CodexDialog, IDataTemplate?>(nameof(TriggerTemplate));

    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<CodexDialog, string?>(nameof(Title));

    public static readonly StyledProperty<string?> DescriptionProperty =
        AvaloniaProperty.Register<CodexDialog, string?>(nameof(Description));

    public static readonly StyledProperty<object?> ActionProperty =
        AvaloniaProperty.Register<CodexDialog, object?>(nameof(Action));

    public static readonly StyledProperty<object?> CloseContentProperty =
        AvaloniaProperty.Register<CodexDialog, object?>(nameof(CloseContent));

    public static readonly StyledProperty<ICommand?> CloseCommandProperty =
        AvaloniaProperty.Register<CodexDialog, ICommand?>(nameof(CloseCommand));

    public static readonly StyledProperty<ICommand?> DismissCommandProperty =
        AvaloniaProperty.Register<CodexDialog, ICommand?>(nameof(DismissCommand));

    public static readonly StyledProperty<bool> IsOpenProperty =
        AvaloniaProperty.Register<CodexDialog, bool>(nameof(IsOpen));

    public static readonly StyledProperty<bool> IsModalProperty =
        AvaloniaProperty.Register<CodexDialog, bool>(nameof(IsModal), true);

    public static readonly StyledProperty<bool> IsCloseVisibleProperty =
        AvaloniaProperty.Register<CodexDialog, bool>(nameof(IsCloseVisible), true);

    public static readonly StyledProperty<bool> CloseOnEscapeProperty =
        AvaloniaProperty.Register<CodexDialog, bool>(nameof(CloseOnEscape), true);

    public static readonly StyledProperty<bool> DismissOnOutsidePointerProperty =
        AvaloniaProperty.Register<CodexDialog, bool>(nameof(DismissOnOutsidePointer), true);

    public static readonly StyledProperty<IInputElement?> RestoreFocusElementProperty =
        AvaloniaProperty.Register<CodexDialog, IInputElement?>(nameof(RestoreFocusElement));

    public static readonly StyledProperty<bool> RestoreFocusOnDismissProperty =
        AvaloniaProperty.Register<CodexDialog, bool>(nameof(RestoreFocusOnDismiss), true);

    public static readonly StyledProperty<bool> HasTitleProperty =
        AvaloniaProperty.Register<CodexDialog, bool>(nameof(HasTitle));

    public static readonly StyledProperty<bool> HasDescriptionProperty =
        AvaloniaProperty.Register<CodexDialog, bool>(nameof(HasDescription));

    public static readonly StyledProperty<bool> HasHeaderProperty =
        AvaloniaProperty.Register<CodexDialog, bool>(nameof(HasHeader));

    public static readonly StyledProperty<bool> HasContentProperty =
        AvaloniaProperty.Register<CodexDialog, bool>(nameof(HasContent));

    public static readonly StyledProperty<bool> HasActionProperty =
        AvaloniaProperty.Register<CodexDialog, bool>(nameof(HasAction));

    public static readonly StyledProperty<bool> HasTriggerProperty =
        AvaloniaProperty.Register<CodexDialog, bool>(nameof(HasTrigger));

    public static readonly StyledProperty<bool> HasRestoreFocusTargetProperty =
        AvaloniaProperty.Register<CodexDialog, bool>(nameof(HasRestoreFocusTarget));

    static CodexDialog()
    {
        TriggerProperty.Changed.AddClassHandler<CodexDialog>((dialog, _) => dialog.SyncSlotStates());
        TitleProperty.Changed.AddClassHandler<CodexDialog>((dialog, _) => dialog.SyncSlotStates());
        DescriptionProperty.Changed.AddClassHandler<CodexDialog>((dialog, _) => dialog.SyncSlotStates());
        ContentControl.ContentProperty.Changed.AddClassHandler<CodexDialog>((dialog, _) => dialog.SyncSlotStates());
        ActionProperty.Changed.AddClassHandler<CodexDialog>((dialog, _) => dialog.SyncSlotStates());
        CloseContentProperty.Changed.AddClassHandler<CodexDialog>((dialog, _) => dialog.SyncSlotStates());
        IsOpenProperty.Changed.AddClassHandler<CodexDialog>((dialog, args) => dialog.OnOpenChanged(args));
        IsModalProperty.Changed.AddClassHandler<CodexDialog>((dialog, _) => dialog.SyncOpenState());
        IsCloseVisibleProperty.Changed.AddClassHandler<CodexDialog>((dialog, _) => dialog.SyncSlotStates());
        RestoreFocusElementProperty.Changed.AddClassHandler<CodexDialog>((dialog, _) => dialog.SyncRestoreFocusState());
        RestoreFocusOnDismissProperty.Changed.AddClassHandler<CodexDialog>((dialog, _) => dialog.SyncRestoreFocusState());
    }

    public CodexDialog()
    {
        DismissCommand = new CodexDismissCommand(Dismiss);
        SyncOpenState();
        SyncSlotStates();
        SyncRestoreFocusState();
    }

    public event EventHandler<RestoreFocusRequestedEventArgs>? RestoreFocusRequested;

    public event EventHandler<CodexDialogOpenChangedEventArgs>? OpenChanged;

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

    public bool IsModal
    {
        get => GetValue(IsModalProperty);
        set => SetValue(IsModalProperty, value);
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
        Open(CodexDialogOpenChangeSource.Programmatic);
    }

    internal void Open(CodexDialogOpenChangeSource source)
    {
        if (!IsEnabled)
        {
            return;
        }

        RunWithOpenChangeSource(source, () => IsOpen = true);
    }

    public bool Toggle()
    {
        return Toggle(CodexDialogOpenChangeSource.Programmatic);
    }

    internal bool Toggle(CodexDialogOpenChangeSource source)
    {
        if (!IsEnabled)
        {
            return false;
        }

        if (IsOpen)
        {
            return Dismiss(source);
        }

        Open(source);
        return true;
    }

    public bool Dismiss()
    {
        return Dismiss(CodexDialogOpenChangeSource.Programmatic);
    }

    internal bool Dismiss(CodexDialogOpenChangeSource source)
    {
        if (!IsOpen)
        {
            return false;
        }

        RunWithOpenChangeSource(source, () => IsOpen = false);

        if (CloseCommand?.CanExecute(null) == true)
        {
            CloseCommand.Execute(null);
        }

        return true;
    }

    public bool TryRestoreFocus()
    {
        return RestoreFocusOnDismiss
               && CodexFocusRestore.TryRestore(RestoreFocusElement ?? _triggerPresenter ?? (Trigger as IInputElement), RestoreFocusRequested, this);
    }

    internal bool TryHandleDismissKey(Key key)
    {
        return key == Key.Escape && CloseOnEscape && Dismiss(CodexDialogOpenChangeSource.Keyboard);
    }

    internal bool TryDismissFromOutsidePointer()
    {
        return DismissOnOutsidePointer && Dismiss(CodexDialogOpenChangeSource.Pointer);
    }

    internal bool TryHandleTriggerKey(Key key)
    {
        return key is Key.Enter or Key.Space && Toggle(CodexDialogOpenChangeSource.Keyboard);
    }

    internal bool TryHandleTriggerPointerRelease(PointerUpdateKind updateKind)
    {
        return updateKind == PointerUpdateKind.LeftButtonReleased && Toggle(CodexDialogOpenChangeSource.Pointer);
    }

    internal bool TryToggleFromTrigger()
    {
        return Toggle();
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        if (_triggerPresenter is not null)
        {
            _triggerPresenter.RemoveHandler(InputElement.PointerReleasedEvent, OnTriggerPointerReleased);
            _triggerPresenter.RemoveHandler(InputElement.KeyDownEvent, OnTriggerKeyDown);
        }

        if (_overlayPresenter is not null)
        {
            _overlayPresenter.RemoveHandler(InputElement.PointerReleasedEvent, OnOverlayPointerReleased);
        }

        base.OnApplyTemplate(e);

        _triggerPresenter = e.NameScope.Find<Control>("PART_Trigger");
        _overlayPresenter = e.NameScope.Find<Control>("PART_Overlay");

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

        if (_overlayPresenter is not null)
        {
            _overlayPresenter.AddHandler(
                InputElement.PointerReleasedEvent,
                OnOverlayPointerReleased,
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
        var updateKind = e.GetCurrentPoint(_triggerPresenter ?? this).Properties.PointerUpdateKind;
        if (TryHandleTriggerPointerRelease(updateKind))
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

    private void OnOverlayPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (TryDismissFromOutsidePointer())
        {
            e.Handled = true;
        }
    }

    private void SyncOpenState()
    {
        Classes.Set("open", IsOpen);
        Classes.Set("closed", !IsOpen);
        Classes.Set("modal", IsModal);
        Classes.Set("non-modal", !IsModal);
        Classes.Set("trigger-open", HasTrigger && IsOpen);
        Classes.Set("trigger-closed", HasTrigger && !IsOpen);
    }

    private void OnOpenChanged(AvaloniaPropertyChangedEventArgs args)
    {
        SyncOpenState();

        if (args.OldValue is bool oldValue && oldValue != IsOpen)
        {
            OpenChanged?.Invoke(this, new CodexDialogOpenChangedEventArgs(IsOpen, CurrentOpenChangeSource));
        }

        if (args.OldValue is true && args.NewValue is false)
        {
            TryRestoreFocus();
        }
    }

    private void SyncRestoreFocusState()
    {
        SetValue(HasRestoreFocusTargetProperty, (RestoreFocusElement ?? _triggerPresenter ?? (Trigger as IInputElement)) is not null);
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

    private CodexDialogOpenChangeSource CurrentOpenChangeSource =>
        _pendingOpenChangeSource ?? CodexDialogOpenChangeSource.Programmatic;

    private void RunWithOpenChangeSource(CodexDialogOpenChangeSource source, Action action)
    {
        var previousSource = _pendingOpenChangeSource;
        _pendingOpenChangeSource = source;
        try
        {
            action();
        }
        finally
        {
            _pendingOpenChangeSource = previousSource;
        }
    }
}
