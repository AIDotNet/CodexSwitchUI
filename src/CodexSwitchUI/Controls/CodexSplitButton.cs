using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using System.Windows.Input;

namespace CodexSwitchUI.Controls;

public sealed class CodexSplitButtonOpenChangedEventArgs(bool isOpen) : EventArgs
{
    public bool IsOpen { get; } = isOpen;
}

public class CodexSplitButton : ContentControl
{
    private CodexButton? _primaryAction;
    private CodexButton? _menuTrigger;
    private Control? _surface;
    private ICommand? _subscribedCommand;

    public static readonly StyledProperty<object?> DropDownContentProperty =
        AvaloniaProperty.Register<CodexSplitButton, object?>(nameof(DropDownContent));

    public static readonly StyledProperty<IDataTemplate?> DropDownContentTemplateProperty =
        AvaloniaProperty.Register<CodexSplitButton, IDataTemplate?>(nameof(DropDownContentTemplate));

    public static readonly StyledProperty<CodexControlVariant> VariantProperty =
        AvaloniaProperty.Register<CodexSplitButton, CodexControlVariant>(nameof(Variant), CodexControlVariant.Default);

    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexSplitButton, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    public static readonly StyledProperty<PlacementMode> PlacementProperty =
        AvaloniaProperty.Register<CodexSplitButton, PlacementMode>(nameof(Placement), PlacementMode.Bottom);

    public static readonly StyledProperty<CodexDropdownAlign> AlignProperty =
        AvaloniaProperty.Register<CodexSplitButton, CodexDropdownAlign>(nameof(Align), CodexDropdownAlign.Center);

    public static readonly StyledProperty<bool> IsOpenProperty =
        AvaloniaProperty.Register<CodexSplitButton, bool>(nameof(IsOpen));

    public static readonly StyledProperty<bool> IsLoadingProperty =
        AvaloniaProperty.Register<CodexSplitButton, bool>(nameof(IsLoading));

    public static readonly StyledProperty<bool> CloseOnEscapeProperty =
        AvaloniaProperty.Register<CodexSplitButton, bool>(nameof(CloseOnEscape), true);

    public static readonly StyledProperty<bool> CloseOnItemSelectedProperty =
        AvaloniaProperty.Register<CodexSplitButton, bool>(nameof(CloseOnItemSelected), true);

    public static readonly StyledProperty<bool> IsArrowVisibleProperty =
        AvaloniaProperty.Register<CodexSplitButton, bool>(nameof(IsArrowVisible));

    public static readonly StyledProperty<ICommand?> CommandProperty =
        AvaloniaProperty.Register<CodexSplitButton, ICommand?>(nameof(Command));

    public static readonly StyledProperty<object?> CommandParameterProperty =
        AvaloniaProperty.Register<CodexSplitButton, object?>(nameof(CommandParameter));

    public static readonly StyledProperty<IInputElement?> RestoreFocusElementProperty =
        AvaloniaProperty.Register<CodexSplitButton, IInputElement?>(nameof(RestoreFocusElement));

    public static readonly StyledProperty<bool> RestoreFocusOnDismissProperty =
        AvaloniaProperty.Register<CodexSplitButton, bool>(nameof(RestoreFocusOnDismiss), true);

    public static readonly StyledProperty<bool> HasDropDownContentProperty =
        AvaloniaProperty.Register<CodexSplitButton, bool>(nameof(HasDropDownContent));

    public static readonly StyledProperty<bool> CanOpenDropDownProperty =
        AvaloniaProperty.Register<CodexSplitButton, bool>(nameof(CanOpenDropDown));

    public static readonly StyledProperty<bool> IsPrimaryActionAvailableProperty =
        AvaloniaProperty.Register<CodexSplitButton, bool>(nameof(IsPrimaryActionAvailable), true);

    public static readonly StyledProperty<bool> HasRestoreFocusTargetProperty =
        AvaloniaProperty.Register<CodexSplitButton, bool>(nameof(HasRestoreFocusTarget));

    public static readonly RoutedEvent<RoutedEventArgs> ClickEvent =
        RoutedEvent.Register<CodexSplitButton, RoutedEventArgs>(nameof(Click), RoutingStrategies.Bubble);

    static CodexSplitButton()
    {
        DropDownContentProperty.Changed.AddClassHandler<CodexSplitButton>((button, _) => button.SyncSlotStates());
        VariantProperty.Changed.AddClassHandler<CodexSplitButton>((button, _) => button.SyncClasses());
        SizeProperty.Changed.AddClassHandler<CodexSplitButton>((button, _) => button.SyncClasses());
        PlacementProperty.Changed.AddClassHandler<CodexSplitButton>((button, _) => button.SyncClasses());
        AlignProperty.Changed.AddClassHandler<CodexSplitButton>((button, _) => button.SyncClasses());
        IsOpenProperty.Changed.AddClassHandler<CodexSplitButton>((button, args) => button.OnOpenChanged(args));
        IsLoadingProperty.Changed.AddClassHandler<CodexSplitButton>((button, _) => button.SyncActionStates());
        CloseOnItemSelectedProperty.Changed.AddClassHandler<CodexSplitButton>((button, _) => button.SyncClasses());
        IsArrowVisibleProperty.Changed.AddClassHandler<CodexSplitButton>((button, _) => button.SyncClasses());
        CommandProperty.Changed.AddClassHandler<CodexSplitButton>((button, args) => button.OnCommandChanged(args.OldValue as ICommand, args.NewValue as ICommand));
        CommandParameterProperty.Changed.AddClassHandler<CodexSplitButton>((button, _) => button.SyncActionStates());
        RestoreFocusElementProperty.Changed.AddClassHandler<CodexSplitButton>((button, _) => button.SyncRestoreFocusState());
        RestoreFocusOnDismissProperty.Changed.AddClassHandler<CodexSplitButton>((button, _) => button.SyncRestoreFocusState());
    }

    public CodexSplitButton()
    {
        Focusable = false;
        SyncClasses();
        SyncSlotStates();
        SyncRestoreFocusState();
    }

    public event EventHandler<RestoreFocusRequestedEventArgs>? RestoreFocusRequested;

    public event EventHandler<CodexSplitButtonOpenChangedEventArgs>? OpenChanged;

    public event EventHandler<RoutedEventArgs> Click
    {
        add => AddHandler(ClickEvent, value);
        remove => RemoveHandler(ClickEvent, value);
    }

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

    public ICommand? Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public object? CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
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

    public bool CanOpenDropDown => GetValue(CanOpenDropDownProperty);

    public bool IsPrimaryActionAvailable => GetValue(IsPrimaryActionAvailableProperty);

    public bool HasRestoreFocusTarget => GetValue(HasRestoreFocusTargetProperty);

    public bool Open()
    {
        if (!CanToggleDropDown())
        {
            return false;
        }

        IsOpen = true;
        return true;
    }

    public bool Dismiss()
    {
        if (!IsOpen)
        {
            return false;
        }

        IsOpen = false;
        return true;
    }

    public bool Toggle()
    {
        return IsOpen ? Dismiss() : Open();
    }

    public bool TryExecutePrimaryAction()
    {
        if (!CanExecutePrimaryAction())
        {
            return false;
        }

        RaiseEvent(new RoutedEventArgs(ClickEvent));
        Command?.Execute(CommandParameter);
        return true;
    }

    internal bool TryHandleDismissKey(Key key)
    {
        return key == Key.Escape && CloseOnEscape && Dismiss();
    }

    internal bool TryCloseFromDropDownAction(Button action)
    {
        if (!CloseOnItemSelected || IsLoading || !IsOpen || !action.IsEnabled)
        {
            return false;
        }

        return Dismiss();
    }

    internal bool TryCloseFromDropDownMenuItem(MenuItem item)
    {
        if (!CloseOnItemSelected || IsLoading || !IsOpen || !CodexMenuActivation.ShouldCloseOnSelect(item))
        {
            return false;
        }

        CodexMenuActivation.TryCloseOnSelect(item);
        return Dismiss();
    }

    public bool TryRestoreFocus()
    {
        return RestoreFocusOnDismiss
               && CodexFocusRestore.TryRestore(RestoreFocusElement ?? _menuTrigger ?? _primaryAction, RestoreFocusRequested, this);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        if (_primaryAction is not null)
        {
            _primaryAction.Click -= OnPrimaryActionClick;
        }

        if (_menuTrigger is not null)
        {
            _menuTrigger.Click -= OnMenuTriggerClick;
        }

        if (_surface is not null)
        {
            _surface.RemoveHandler(Button.ClickEvent, OnDropDownActionClicked);
            _surface.RemoveHandler(MenuItem.ClickEvent, OnDropDownMenuItemClicked);
        }

        base.OnApplyTemplate(e);

        _primaryAction = e.NameScope.Find<CodexButton>("PART_PrimaryAction");
        _menuTrigger = e.NameScope.Find<CodexButton>("PART_MenuTrigger");
        _surface = e.NameScope.Find<Control>("PART_Surface");

        if (_primaryAction is not null)
        {
            _primaryAction.Click += OnPrimaryActionClick;
        }

        if (_menuTrigger is not null)
        {
            _menuTrigger.Click += OnMenuTriggerClick;
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

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == IsEnabledProperty)
        {
            SyncActionStates();
        }
    }

    private void OnPrimaryActionClick(object? sender, RoutedEventArgs e)
    {
        TryExecutePrimaryAction();
    }

    private void OnMenuTriggerClick(object? sender, RoutedEventArgs e)
    {
        Toggle();
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
            OpenChanged?.Invoke(this, new CodexSplitButtonOpenChangedEventArgs(newValue));
        }

        if (args.OldValue is true && args.NewValue is false)
        {
            TryRestoreFocus();
        }
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

        SyncActionStates();
    }

    private void OnCommandCanExecuteChanged(object? sender, EventArgs e)
    {
        SyncActionStates();
    }

    private bool CanToggleDropDown()
    {
        return IsEnabled && !IsLoading && HasDropDownContent;
    }

    private bool CanExecutePrimaryAction()
    {
        return IsEnabled && !IsLoading && (Command?.CanExecute(CommandParameter) ?? true);
    }

    private void SyncActionStates()
    {
        SetValue(CanOpenDropDownProperty, CanToggleDropDown());
        SetValue(IsPrimaryActionAvailableProperty, CanExecutePrimaryAction());
        SyncClasses();
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
        Classes.Set("has-command", Command is not null);
        Classes.Set("restore-focus", RestoreFocusOnDismiss);
        Classes.Set("primary-action-disabled", !IsPrimaryActionAvailable);
        Classes.Set("can-open-dropdown", CanOpenDropDown);
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
        SyncActionStates();
    }

    private void SyncRestoreFocusState()
    {
        SetValue(HasRestoreFocusTargetProperty, (RestoreFocusElement ?? _menuTrigger ?? _primaryAction) is not null);
        Classes.Set("restore-focus", RestoreFocusOnDismiss);
        Classes.Set("has-restore-focus-target", HasRestoreFocusTarget);
    }

    private static bool HasValue(object? value)
    {
        return value is string text ? !string.IsNullOrWhiteSpace(text) : value is not null;
    }
}
