using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace CodexSwitchUI.Controls;

public enum CodexDropdownAlign
{
    Center,
    Start,
    End
}

public enum CodexDropdownButtonOpenChangeSource
{
    Programmatic,
    Pointer,
    Keyboard,
    Selection
}

public sealed class CodexDropdownButtonOpenChangedEventArgs(
    bool isOpen,
    CodexDropdownButtonOpenChangeSource source = CodexDropdownButtonOpenChangeSource.Programmatic) : EventArgs
{
    public bool IsOpen { get; } = isOpen;

    public CodexDropdownButtonOpenChangeSource Source { get; } = source;
}

public class CodexDropdownButton : ContentControl
{
    private CodexButton? _trigger;
    private Control? _surface;
    private CodexDropdownButtonOpenChangeSource? _pendingOpenChangeSource;

    public static readonly StyledProperty<object?> DropDownContentProperty =
        AvaloniaProperty.Register<CodexDropdownButton, object?>(nameof(DropDownContent));

    public static readonly StyledProperty<IDataTemplate?> DropDownContentTemplateProperty =
        AvaloniaProperty.Register<CodexDropdownButton, IDataTemplate?>(nameof(DropDownContentTemplate));

    public static readonly StyledProperty<CodexControlVariant> VariantProperty =
        AvaloniaProperty.Register<CodexDropdownButton, CodexControlVariant>(nameof(Variant), CodexControlVariant.Default);

    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexDropdownButton, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    public static readonly StyledProperty<PlacementMode> PlacementProperty =
        AvaloniaProperty.Register<CodexDropdownButton, PlacementMode>(nameof(Placement), PlacementMode.Bottom);

    public static readonly StyledProperty<CodexDropdownAlign> AlignProperty =
        AvaloniaProperty.Register<CodexDropdownButton, CodexDropdownAlign>(nameof(Align), CodexDropdownAlign.Center);

    public static readonly StyledProperty<bool> IsOpenProperty =
        AvaloniaProperty.Register<CodexDropdownButton, bool>(nameof(IsOpen));

    public static readonly StyledProperty<bool> IsLoadingProperty =
        AvaloniaProperty.Register<CodexDropdownButton, bool>(nameof(IsLoading));

    public static readonly StyledProperty<bool> CloseOnEscapeProperty =
        AvaloniaProperty.Register<CodexDropdownButton, bool>(nameof(CloseOnEscape), true);

    public static readonly StyledProperty<bool> CloseOnItemSelectedProperty =
        AvaloniaProperty.Register<CodexDropdownButton, bool>(nameof(CloseOnItemSelected), true);

    public static readonly StyledProperty<bool> IsArrowVisibleProperty =
        AvaloniaProperty.Register<CodexDropdownButton, bool>(nameof(IsArrowVisible));

    public static readonly StyledProperty<IInputElement?> RestoreFocusElementProperty =
        AvaloniaProperty.Register<CodexDropdownButton, IInputElement?>(nameof(RestoreFocusElement));

    public static readonly StyledProperty<bool> RestoreFocusOnDismissProperty =
        AvaloniaProperty.Register<CodexDropdownButton, bool>(nameof(RestoreFocusOnDismiss), true);

    public static readonly StyledProperty<bool> HasDropDownContentProperty =
        AvaloniaProperty.Register<CodexDropdownButton, bool>(nameof(HasDropDownContent));

    public static readonly StyledProperty<bool> HasRestoreFocusTargetProperty =
        AvaloniaProperty.Register<CodexDropdownButton, bool>(nameof(HasRestoreFocusTarget));

    static CodexDropdownButton()
    {
        DropDownContentProperty.Changed.AddClassHandler<CodexDropdownButton>((button, _) => button.SyncSlotStates());
        VariantProperty.Changed.AddClassHandler<CodexDropdownButton>((button, _) => button.SyncClasses());
        SizeProperty.Changed.AddClassHandler<CodexDropdownButton>((button, _) => button.SyncClasses());
        PlacementProperty.Changed.AddClassHandler<CodexDropdownButton>((button, _) => button.SyncClasses());
        AlignProperty.Changed.AddClassHandler<CodexDropdownButton>((button, _) => button.SyncClasses());
        IsOpenProperty.Changed.AddClassHandler<CodexDropdownButton>((button, args) => button.OnOpenChanged(args));
        IsLoadingProperty.Changed.AddClassHandler<CodexDropdownButton>((button, _) => button.SyncClasses());
        CloseOnItemSelectedProperty.Changed.AddClassHandler<CodexDropdownButton>((button, _) => button.SyncClasses());
        IsArrowVisibleProperty.Changed.AddClassHandler<CodexDropdownButton>((button, _) => button.SyncClasses());
        RestoreFocusElementProperty.Changed.AddClassHandler<CodexDropdownButton>((button, _) => button.SyncRestoreFocusState());
        RestoreFocusOnDismissProperty.Changed.AddClassHandler<CodexDropdownButton>((button, _) => button.SyncRestoreFocusState());
    }

    public CodexDropdownButton()
    {
        Focusable = false;
        SyncClasses();
        SyncSlotStates();
        SyncRestoreFocusState();
    }

    public event EventHandler<RestoreFocusRequestedEventArgs>? RestoreFocusRequested;

    public event EventHandler<CodexDropdownButtonOpenChangedEventArgs>? OpenChanged;

    public object? DropDownContent
    {
        get => GetValue(DropDownContentProperty);
        set => SetValue(DropDownContentProperty, value);
    }

    public IDataTemplate? DropDownContentTemplate
    {
        get => GetValue(DropDownContentTemplateProperty);
        set => SetValue(DropDownContentTemplateProperty, value);
    }

    public CodexControlVariant Variant
    {
        get => GetValue(VariantProperty);
        set => SetValue(VariantProperty, value);
    }

    public CodexControlSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    public PlacementMode Placement
    {
        get => GetValue(PlacementProperty);
        set => SetValue(PlacementProperty, value);
    }

    public CodexDropdownAlign Align
    {
        get => GetValue(AlignProperty);
        set => SetValue(AlignProperty, value);
    }

    public bool IsOpen
    {
        get => GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    public bool IsLoading
    {
        get => GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }

    public bool CloseOnEscape
    {
        get => GetValue(CloseOnEscapeProperty);
        set => SetValue(CloseOnEscapeProperty, value);
    }

    public bool CloseOnItemSelected
    {
        get => GetValue(CloseOnItemSelectedProperty);
        set => SetValue(CloseOnItemSelectedProperty, value);
    }

    public bool IsArrowVisible
    {
        get => GetValue(IsArrowVisibleProperty);
        set => SetValue(IsArrowVisibleProperty, value);
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

    public bool HasDropDownContent => GetValue(HasDropDownContentProperty);

    public bool HasRestoreFocusTarget => GetValue(HasRestoreFocusTargetProperty);

    public bool Open()
    {
        return Open(CodexDropdownButtonOpenChangeSource.Programmatic);
    }

    internal bool Open(CodexDropdownButtonOpenChangeSource source)
    {
        if (!CanToggle())
        {
            return false;
        }

        RunWithOpenChangeSource(source, () => IsOpen = true);
        return true;
    }

    public bool Dismiss()
    {
        return Dismiss(CodexDropdownButtonOpenChangeSource.Programmatic);
    }

    internal bool Dismiss(CodexDropdownButtonOpenChangeSource source)
    {
        if (!IsOpen)
        {
            return false;
        }

        RunWithOpenChangeSource(source, () => IsOpen = false);
        return true;
    }

    public bool Toggle()
    {
        return Toggle(CodexDropdownButtonOpenChangeSource.Programmatic);
    }

    internal bool Toggle(CodexDropdownButtonOpenChangeSource source)
    {
        return IsOpen ? Dismiss(source) : Open(source);
    }

    internal bool TryHandleDismissKey(Key key)
    {
        return key == Key.Escape && CloseOnEscape && Dismiss(CodexDropdownButtonOpenChangeSource.Keyboard);
    }

    internal bool TryHandleTriggerKey(Key key)
    {
        if (key is not (Key.Enter or Key.Space or Key.Down))
        {
            return false;
        }

        return Open(CodexDropdownButtonOpenChangeSource.Keyboard);
    }

    internal bool TryHandleTriggerPointerRelease(PointerUpdateKind updateKind)
    {
        return updateKind == PointerUpdateKind.LeftButtonReleased && Toggle(CodexDropdownButtonOpenChangeSource.Pointer);
    }

    internal bool TryCloseFromDropDownAction(Button action)
    {
        if (!CloseOnItemSelected || IsLoading || !IsOpen || !action.IsEnabled)
        {
            return false;
        }

        return Dismiss(CodexDropdownButtonOpenChangeSource.Selection);
    }

    internal bool TryCloseFromDropDownMenuItem(MenuItem item)
    {
        if (!CloseOnItemSelected || IsLoading || !IsOpen || !CodexMenuActivation.ShouldCloseOnSelect(item))
        {
            return false;
        }

        CodexMenuActivation.TryCloseOnSelect(item);
        return Dismiss(CodexDropdownButtonOpenChangeSource.Selection);
    }

    public bool TryRestoreFocus()
    {
        return RestoreFocusOnDismiss
               && CodexFocusRestore.TryRestore(RestoreFocusElement ?? _trigger, RestoreFocusRequested, this);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        if (_trigger is not null)
        {
            _trigger.RemoveHandler(InputElement.PointerReleasedEvent, OnTriggerPointerReleased);
            _trigger.RemoveHandler(InputElement.KeyDownEvent, OnTriggerKeyDown);
        }

        if (_surface is not null)
        {
            _surface.RemoveHandler(Button.ClickEvent, OnDropDownActionClicked);
            _surface.RemoveHandler(MenuItem.ClickEvent, OnDropDownMenuItemClicked);
        }

        base.OnApplyTemplate(e);

        _trigger = e.NameScope.Find<CodexButton>("PART_Trigger");
        _surface = e.NameScope.Find<Control>("PART_Surface");

        if (_trigger is not null)
        {
            _trigger.AddHandler(
                InputElement.PointerReleasedEvent,
                OnTriggerPointerReleased,
                RoutingStrategies.Bubble,
                handledEventsToo: true);
            _trigger.AddHandler(
                InputElement.KeyDownEvent,
                OnTriggerKeyDown,
                RoutingStrategies.Bubble,
                handledEventsToo: true);
        }

        if (_surface is not null)
        {
            _surface.AddHandler(
                Button.ClickEvent,
                OnDropDownActionClicked,
                RoutingStrategies.Bubble,
                handledEventsToo: true);
            _surface.AddHandler(
                MenuItem.ClickEvent,
                OnDropDownMenuItemClicked,
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
        var updateKind = e.GetCurrentPoint((Control?)_trigger ?? this).Properties.PointerUpdateKind;
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

    private void OnDropDownActionClicked(object? sender, RoutedEventArgs e)
    {
        if (e.Source is Button action)
        {
            TryCloseFromDropDownAction(action);
        }
    }

    private void OnDropDownMenuItemClicked(object? sender, RoutedEventArgs e)
    {
        if (e.Source is MenuItem item)
        {
            TryCloseFromDropDownMenuItem(item);
        }
    }

    private void OnOpenChanged(AvaloniaPropertyChangedEventArgs args)
    {
        SyncClasses();

        if (args.OldValue is bool oldValue && args.NewValue is bool newValue && oldValue != newValue)
        {
            OpenChanged?.Invoke(this, new CodexDropdownButtonOpenChangedEventArgs(newValue, CurrentOpenChangeSource));
        }

        if (args.OldValue is true && args.NewValue is false)
        {
            TryRestoreFocus();
        }
    }

    private bool CanToggle()
    {
        return IsEnabled && !IsLoading && HasDropDownContent;
    }

    private void SyncClasses()
    {
        CodexClassSync.SetVariant(Classes, Variant);
        CodexClassSync.SetSize(Classes, Size);
        Classes.Set("open", IsOpen);
        Classes.Set("closed", !IsOpen);
        Classes.Set("loading", IsLoading);
        Classes.Set("close-on-select", CloseOnItemSelected);
        Classes.Set("has-arrow", IsArrowVisible);
        Classes.Set("restore-focus", RestoreFocusOnDismiss);
        Classes.Set("align-center", Align == CodexDropdownAlign.Center);
        Classes.Set("align-start", Align == CodexDropdownAlign.Start);
        Classes.Set("align-end", Align == CodexDropdownAlign.End);
        Classes.Set("side-top", Placement is PlacementMode.Top or PlacementMode.TopEdgeAlignedLeft or PlacementMode.TopEdgeAlignedRight);
        Classes.Set("side-left", Placement is PlacementMode.Left or PlacementMode.LeftEdgeAlignedTop or PlacementMode.LeftEdgeAlignedBottom);
        Classes.Set("side-right", Placement is PlacementMode.Right or PlacementMode.RightEdgeAlignedTop or PlacementMode.RightEdgeAlignedBottom);
        Classes.Set("side-bottom", Placement is not (PlacementMode.Top or PlacementMode.TopEdgeAlignedLeft or PlacementMode.TopEdgeAlignedRight
            or PlacementMode.Left or PlacementMode.LeftEdgeAlignedTop or PlacementMode.LeftEdgeAlignedBottom
            or PlacementMode.Right or PlacementMode.RightEdgeAlignedTop or PlacementMode.RightEdgeAlignedBottom));
    }

    private void SyncSlotStates()
    {
        SetValue(HasDropDownContentProperty, HasValue(DropDownContent));
        Classes.Set("has-dropdown-content", HasDropDownContent);
    }

    private void SyncRestoreFocusState()
    {
        SetValue(HasRestoreFocusTargetProperty, (RestoreFocusElement ?? _trigger) is not null);
        Classes.Set("restore-focus", RestoreFocusOnDismiss);
        Classes.Set("has-restore-focus-target", HasRestoreFocusTarget);
    }

    private static bool HasValue(object? value)
    {
        return value is string text ? !string.IsNullOrWhiteSpace(text) : value is not null;
    }

    private CodexDropdownButtonOpenChangeSource CurrentOpenChangeSource =>
        _pendingOpenChangeSource ?? CodexDropdownButtonOpenChangeSource.Programmatic;

    private void RunWithOpenChangeSource(CodexDropdownButtonOpenChangeSource source, Action action)
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
