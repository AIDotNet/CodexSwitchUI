using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Threading;
using Avalonia.VisualTree;
using System.Windows.Input;

namespace CodexSwitchUI.Controls;

public class CodexIconButton : CodexButton
{
    public static readonly StyledProperty<bool> IsRoundProperty =
        AvaloniaProperty.Register<CodexIconButton, bool>(nameof(IsRound));

    static CodexIconButton()
    {
        IsRoundProperty.Changed.AddClassHandler<CodexIconButton>((button, _) => button.SyncIconClasses());
    }

    public CodexIconButton()
    {
        Size = CodexControlSize.Icon;
        SyncIconClasses();
    }

    public bool IsRound
    {
        get => GetValue(IsRoundProperty);
        set => SetValue(IsRoundProperty, value);
    }

    private void SyncIconClasses()
    {
        Classes.Set("round", IsRound);
    }
}

public sealed class CodexSegmentedControlValueChangedEventArgs(
    CodexSegmentedButton? oldItem,
    CodexSegmentedButton? newItem,
    int oldIndex,
    int newIndex,
    string? oldValue,
    string? newValue,
    CodexSegmentedControlValueChangeSource source = CodexSegmentedControlValueChangeSource.Programmatic) : EventArgs
{
    public CodexSegmentedButton? OldItem { get; } = oldItem;

    public CodexSegmentedButton? NewItem { get; } = newItem;

    public int OldIndex { get; } = oldIndex;

    public int NewIndex { get; } = newIndex;

    public string? OldValue { get; } = oldValue;

    public string? NewValue { get; } = newValue;

    public CodexSegmentedControlValueChangeSource Source { get; } = source;
}

public enum CodexSegmentedControlValueChangeSource
{
    Programmatic,
    Pointer,
    Keyboard
}

public sealed class CodexSideNavValueChangedEventArgs(
    CodexSideNavItem? oldItem,
    CodexSideNavItem? newItem,
    int oldIndex,
    int newIndex,
    string? oldValue,
    string? newValue,
    CodexSideNavValueChangeSource source = CodexSideNavValueChangeSource.Programmatic) : EventArgs
{
    public CodexSideNavItem? OldItem { get; } = oldItem;

    public CodexSideNavItem? NewItem { get; } = newItem;

    public int OldIndex { get; } = oldIndex;

    public int NewIndex { get; } = newIndex;

    public string? OldValue { get; } = oldValue;

    public string? NewValue { get; } = newValue;

    public CodexSideNavValueChangeSource Source { get; } = source;
}

public enum CodexSideNavValueChangeSource
{
    Programmatic,
    Pointer,
    Keyboard
}

public class CodexSideNav : ContentControl
{
    private bool _syncingSelection;

    public static readonly StyledProperty<string?> SelectedValueProperty =
        AvaloniaProperty.Register<CodexSideNav, string?>(nameof(SelectedValue));

    static CodexSideNav()
    {
        SelectedValueProperty.Changed.AddClassHandler<CodexSideNav>((nav, args) => nav.OnSelectedValueChanged(args));
    }

    public event EventHandler<CodexSideNavValueChangedEventArgs>? ValueChanged;

    public string? SelectedValue
    {
        get => GetValue(SelectedValueProperty);
        set => SetValue(SelectedValueProperty, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ContentProperty)
        {
            SyncItemsForCurrentValue();
        }
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        SyncItemsForCurrentValue();
    }

    internal bool SelectItem(CodexSideNavItem item, CodexSideNavValueChangeSource source = CodexSideNavValueChangeSource.Programmatic)
    {
        if (!item.CanSelect)
        {
            return false;
        }

        var items = GetSideNavItems();
        var oldItem = items.FirstOrDefault(candidate => candidate.IsSelected);
        var oldIndex = IndexOf(items, oldItem);
        var newIndex = IndexOf(items, item);
        var oldValue = ResolveItemValue(oldItem);
        var newValue = ResolveItemValue(item);

        if (ReferenceEquals(oldItem, item) && string.Equals(SelectedValue, newValue, StringComparison.Ordinal))
        {
            return false;
        }

        _syncingSelection = true;
        try
        {
            foreach (var candidate in items)
            {
                candidate.IsSelected = ReferenceEquals(candidate, item);
            }

            SetValue(SelectedValueProperty, newValue);
        }
        finally
        {
            _syncingSelection = false;
        }

        RaiseValueChanged(oldItem, item, oldIndex, newIndex, oldValue, newValue, source);
        return true;
    }

    private void OnSelectedValueChanged(AvaloniaPropertyChangedEventArgs args)
    {
        if (_syncingSelection)
        {
            return;
        }

        var items = GetSideNavItems();
        var oldValue = args.OldValue as string;
        var newValue = args.NewValue as string;
        var oldItem = items.FirstOrDefault(item => string.Equals(ResolveItemValue(item), oldValue, StringComparison.Ordinal));
        var newItem = items.FirstOrDefault(item => string.Equals(ResolveItemValue(item), newValue, StringComparison.Ordinal));
        var oldIndex = IndexOf(items, oldItem);
        var newIndex = IndexOf(items, newItem);

        _syncingSelection = true;
        try
        {
            foreach (var item in items)
            {
                item.IsSelected = ReferenceEquals(item, newItem);
            }
        }
        finally
        {
            _syncingSelection = false;
        }

        RaiseValueChanged(oldItem, newItem, oldIndex, newIndex, oldValue, newValue, CodexSideNavValueChangeSource.Programmatic);
    }

    private void SyncItemsForCurrentValue()
    {
        if (_syncingSelection)
        {
            return;
        }

        var items = GetSideNavItems();
        if (items.Count == 0)
        {
            return;
        }

        if (!string.IsNullOrWhiteSpace(SelectedValue))
        {
            var selectedItem = items.FirstOrDefault(item => string.Equals(ResolveItemValue(item), SelectedValue, StringComparison.Ordinal));

            _syncingSelection = true;
            try
            {
                foreach (var item in items)
                {
                    item.IsSelected = ReferenceEquals(item, selectedItem);
                }
            }
            finally
            {
                _syncingSelection = false;
            }

            return;
        }

        var existingSelection = items.FirstOrDefault(item => item.IsSelected);
        if (existingSelection is null)
        {
            return;
        }

        _syncingSelection = true;
        try
        {
            SetValue(SelectedValueProperty, ResolveItemValue(existingSelection));
        }
        finally
        {
            _syncingSelection = false;
        }
    }

    private IReadOnlyList<CodexSideNavItem> GetSideNavItems()
    {
        var logical = this.GetLogicalDescendants()
            .OfType<CodexSideNavItem>()
            .ToList();
        if (logical.Count > 0)
        {
            return logical;
        }

        var visual = this.GetVisualDescendants()
            .OfType<CodexSideNavItem>()
            .ToList();
        if (visual.Count > 0)
        {
            return visual;
        }

        return Content is CodexSideNavItem item ? [item] : [];
    }

    private void RaiseValueChanged(
        CodexSideNavItem? oldItem,
        CodexSideNavItem? newItem,
        int oldIndex,
        int newIndex,
        string? oldValue,
        string? newValue,
        CodexSideNavValueChangeSource source)
    {
        if (ReferenceEquals(oldItem, newItem) && string.Equals(oldValue, newValue, StringComparison.Ordinal))
        {
            return;
        }

        ValueChanged?.Invoke(
            this,
            new CodexSideNavValueChangedEventArgs(oldItem, newItem, oldIndex, newIndex, oldValue, newValue, source));
    }

    private static int IndexOf(IReadOnlyList<CodexSideNavItem> items, CodexSideNavItem? item)
    {
        if (item is null)
        {
            return -1;
        }

        for (var index = 0; index < items.Count; index++)
        {
            if (ReferenceEquals(items[index], item))
            {
                return index;
            }
        }

        return -1;
    }

    internal static string? ResolveItemValue(CodexSideNavItem? item)
    {
        if (item is null)
        {
            return null;
        }

        if (!string.IsNullOrWhiteSpace(item.Value))
        {
            return item.Value;
        }

        return item.Content is string text ? text : item.Content?.ToString();
    }
}

public class CodexSideNavItem : Button
{
    private ICommand? _subscribedCommand;
    private bool _hasPrimaryPointerPress;
    private PointerUpdateKind? _pendingPointerReleaseKind;
    private bool _selectionHandledByPointerRelease;

    public static readonly StyledProperty<string?> ValueProperty =
        AvaloniaProperty.Register<CodexSideNavItem, string?>(nameof(Value));

    public static readonly StyledProperty<object?> IconProperty =
        AvaloniaProperty.Register<CodexSideNavItem, object?>(nameof(Icon));

    public static readonly StyledProperty<bool> IsSelectedProperty =
        AvaloniaProperty.Register<CodexSideNavItem, bool>(nameof(IsSelected));

    public static readonly StyledProperty<string?> DetailProperty =
        AvaloniaProperty.Register<CodexSideNavItem, string?>(nameof(Detail));

    public static readonly StyledProperty<bool> HasIconProperty =
        AvaloniaProperty.Register<CodexSideNavItem, bool>(nameof(HasIcon));

    public static readonly StyledProperty<bool> HasDetailProperty =
        AvaloniaProperty.Register<CodexSideNavItem, bool>(nameof(HasDetail));

    static CodexSideNavItem()
    {
        IconProperty.Changed.AddClassHandler<CodexSideNavItem>((item, _) => item.SyncClasses());
        IsSelectedProperty.Changed.AddClassHandler<CodexSideNavItem>((item, _) => item.SyncClasses());
        DetailProperty.Changed.AddClassHandler<CodexSideNavItem>((item, _) => item.SyncClasses());
        CommandProperty.Changed.AddClassHandler<CodexSideNavItem>((item, args) => item.OnCommandChanged(args.OldValue as ICommand, args.NewValue as ICommand));
        CommandParameterProperty.Changed.AddClassHandler<CodexSideNavItem>((item, _) => item.SyncClasses());
        IsEnabledProperty.Changed.AddClassHandler<CodexSideNavItem>((item, _) => item.SyncClasses());
    }

    public CodexSideNavItem()
    {
        SyncClasses();
    }

    public string? Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public object? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public bool IsSelected
    {
        get => GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    public string? Detail
    {
        get => GetValue(DetailProperty);
        set => SetValue(DetailProperty, value);
    }

    public bool HasIcon => GetValue(HasIconProperty);

    public bool HasDetail => GetValue(HasDetailProperty);

    internal bool CanSelect => IsEnabled && (Command?.CanExecute(CommandParameter) ?? true);

    protected override void OnClick()
    {
        if (_pendingPointerReleaseKind is { } updateKind)
        {
            if (updateKind != PointerUpdateKind.LeftButtonReleased || !CanSelect)
            {
                return;
            }

            base.OnClick();
            _selectionHandledByPointerRelease = TryHandlePointerActivation(updateKind);
            return;
        }

        if (!CanSelect)
        {
            return;
        }

        base.OnClick();
        _ = TrySelect(CodexSideNavValueChangeSource.Keyboard);
    }

    internal bool TryHandlePointerActivation(PointerUpdateKind updateKind)
    {
        if (updateKind != PointerUpdateKind.LeftButtonReleased)
        {
            return false;
        }

        return TrySelect(CodexSideNavValueChangeSource.Pointer);
    }

    internal bool TrySelect(CodexSideNavValueChangeSource source = CodexSideNavValueChangeSource.Programmatic)
    {
        if (!CanSelect)
        {
            return false;
        }

        var owner = this.GetVisualAncestors().OfType<CodexSideNav>().FirstOrDefault()
            ?? this.GetLogicalAncestors().OfType<CodexSideNav>().FirstOrDefault();

        if (owner is not null)
        {
            return owner.SelectItem(this, source);
        }

        SelectSiblingItems();
        return true;
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

    private void SelectSiblingItems()
    {
        if (!CanSelect)
        {
            return;
        }

        var parent = this.GetLogicalParent();
        if (parent is null)
        {
            IsSelected = true;
            return;
        }

        foreach (var child in parent.GetLogicalChildren())
        {
            if (child is CodexSideNavItem item)
            {
                item.IsSelected = ReferenceEquals(item, this);
            }
        }
    }

    private void SyncClasses()
    {
        Classes.Set("selected", IsSelected);
        Classes.Set("can-select", CanSelect);
        Classes.Set("command-blocked", Command is not null && IsEnabled && !CanSelect);
        SetValue(HasIconProperty, Icon is not null);
        SetValue(HasDetailProperty, !string.IsNullOrWhiteSpace(Detail));
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
}

public class CodexSegmentedControl : ContentControl
{
    private Control? _indicatorHost;
    private bool _syncingSelection;

    public static readonly StyledProperty<string?> SelectedValueProperty =
        AvaloniaProperty.Register<CodexSegmentedControl, string?>(nameof(SelectedValue));

    public static readonly StyledProperty<double> IndicatorWidthProperty =
        AvaloniaProperty.Register<CodexSegmentedControl, double>(nameof(IndicatorWidth));

    public static readonly StyledProperty<double> IndicatorHeightProperty =
        AvaloniaProperty.Register<CodexSegmentedControl, double>(nameof(IndicatorHeight));

    public static readonly StyledProperty<Thickness> IndicatorMarginProperty =
        AvaloniaProperty.Register<CodexSegmentedControl, Thickness>(nameof(IndicatorMargin));

    public static readonly StyledProperty<bool> IsIndicatorVisibleProperty =
        AvaloniaProperty.Register<CodexSegmentedControl, bool>(nameof(IsIndicatorVisible));

    static CodexSegmentedControl()
    {
        SelectedValueProperty.Changed.AddClassHandler<CodexSegmentedControl>((control, args) => control.OnSelectedValueChanged(args));
    }

    public CodexSegmentedControl()
    {
        LayoutUpdated += (_, _) => UpdateSelectionIndicator();
    }

    public event EventHandler<CodexSegmentedControlValueChangedEventArgs>? ValueChanged;

    public string? SelectedValue
    {
        get => GetValue(SelectedValueProperty);
        set => SetValue(SelectedValueProperty, value);
    }

    public double IndicatorWidth => GetValue(IndicatorWidthProperty);

    public double IndicatorHeight => GetValue(IndicatorHeightProperty);

    public Thickness IndicatorMargin => GetValue(IndicatorMarginProperty);

    public bool IsIndicatorVisible => GetValue(IsIndicatorVisibleProperty);

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _indicatorHost = e.NameScope.Find<Control>("PART_IndicatorHost");
        QueueSelectionIndicatorUpdate();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ContentProperty)
        {
            QueueSelectionIndicatorUpdate();
        }
    }

    internal void QueueSelectionIndicatorUpdate()
    {
        Dispatcher.UIThread.Post(UpdateSelectionIndicator, DispatcherPriority.Loaded);
    }

    internal bool SelectButton(
        CodexSegmentedButton button,
        CodexSegmentedControlValueChangeSource source = CodexSegmentedControlValueChangeSource.Programmatic)
    {
        if (!button.CanSelect)
        {
            return false;
        }

        var buttons = GetSegmentedButtons();
        var oldButton = buttons.FirstOrDefault(candidate => candidate.IsSelected);
        var oldIndex = IndexOf(buttons, oldButton);
        var newIndex = IndexOf(buttons, button);
        var oldValue = ResolveButtonValue(oldButton);
        var newValue = ResolveButtonValue(button);

        if (ReferenceEquals(oldButton, button) && string.Equals(SelectedValue, newValue, StringComparison.Ordinal))
        {
            QueueSelectionIndicatorUpdate();
            return false;
        }

        _syncingSelection = true;
        try
        {
            foreach (var candidate in buttons)
            {
                candidate.IsSelected = ReferenceEquals(candidate, button);
            }

            SetValue(SelectedValueProperty, newValue);
        }
        finally
        {
            _syncingSelection = false;
        }

        QueueSelectionIndicatorUpdate();
        RaiseValueChanged(oldButton, button, oldIndex, newIndex, oldValue, newValue, source);
        return true;
    }

    internal void UpdateSelectionIndicator()
    {
        var selected = this.GetVisualDescendants()
            .OfType<CodexSegmentedButton>()
            .FirstOrDefault(button => button.IsSelected && button.IsVisible);

        if (_indicatorHost is null
            || selected?.TranslatePoint(new Point(0, 0), _indicatorHost) is not { } position
            || selected.Bounds.Width <= 0
            || selected.Bounds.Height <= 0)
        {
            SetValue(IsIndicatorVisibleProperty, false);
            return;
        }

        SetValue(IndicatorWidthProperty, selected.Bounds.Width);
        SetValue(IndicatorHeightProperty, selected.Bounds.Height);
        SetValue(IndicatorMarginProperty, new Thickness(position.X, position.Y, 0, 0));
        SetValue(IsIndicatorVisibleProperty, true);
    }

    private void OnSelectedValueChanged(AvaloniaPropertyChangedEventArgs args)
    {
        if (_syncingSelection)
        {
            return;
        }

        var buttons = GetSegmentedButtons();
        var oldValue = args.OldValue as string;
        var newValue = args.NewValue as string;
        var oldButton = buttons.FirstOrDefault(button => string.Equals(ResolveButtonValue(button), oldValue, StringComparison.Ordinal));
        var newButton = buttons.FirstOrDefault(button => string.Equals(ResolveButtonValue(button), newValue, StringComparison.Ordinal));
        var oldIndex = IndexOf(buttons, oldButton);
        var newIndex = IndexOf(buttons, newButton);

        _syncingSelection = true;
        try
        {
            foreach (var button in buttons)
            {
                button.IsSelected = ReferenceEquals(button, newButton);
            }
        }
        finally
        {
            _syncingSelection = false;
        }

        QueueSelectionIndicatorUpdate();
        RaiseValueChanged(oldButton, newButton, oldIndex, newIndex, oldValue, newValue, CodexSegmentedControlValueChangeSource.Programmatic);
    }

    private IReadOnlyList<CodexSegmentedButton> GetSegmentedButtons()
    {
        var logical = this.GetLogicalDescendants()
            .OfType<CodexSegmentedButton>()
            .ToList();
        if (logical.Count > 0)
        {
            return logical;
        }

        var visual = this.GetVisualDescendants()
            .OfType<CodexSegmentedButton>()
            .ToList();
        if (visual.Count > 0)
        {
            return visual;
        }

        return Content is CodexSegmentedButton button ? [button] : [];
    }

    private void RaiseValueChanged(
        CodexSegmentedButton? oldButton,
        CodexSegmentedButton? newButton,
        int oldIndex,
        int newIndex,
        string? oldValue,
        string? newValue,
        CodexSegmentedControlValueChangeSource source)
    {
        if (ReferenceEquals(oldButton, newButton) && string.Equals(oldValue, newValue, StringComparison.Ordinal))
        {
            return;
        }

        ValueChanged?.Invoke(
            this,
            new CodexSegmentedControlValueChangedEventArgs(oldButton, newButton, oldIndex, newIndex, oldValue, newValue, source));
    }

    private static int IndexOf(IReadOnlyList<CodexSegmentedButton> buttons, CodexSegmentedButton? button)
    {
        if (button is null)
        {
            return -1;
        }

        for (var index = 0; index < buttons.Count; index++)
        {
            if (ReferenceEquals(buttons[index], button))
            {
                return index;
            }
        }

        return -1;
    }

    internal static string? ResolveButtonValue(CodexSegmentedButton? button)
    {
        if (button is null)
        {
            return null;
        }

        if (!string.IsNullOrWhiteSpace(button.Value))
        {
            return button.Value;
        }

        return button.Content is string text ? text : button.Content?.ToString();
    }
}

public class CodexSegmentedButton : Button
{
    private ICommand? _subscribedCommand;
    private bool _hasPrimaryPointerPress;
    private PointerUpdateKind? _pendingPointerReleaseKind;
    private bool _selectionHandledByPointerRelease;

    public static readonly StyledProperty<string?> ValueProperty =
        AvaloniaProperty.Register<CodexSegmentedButton, string?>(nameof(Value));

    public static readonly StyledProperty<bool> IsSelectedProperty =
        AvaloniaProperty.Register<CodexSegmentedButton, bool>(nameof(IsSelected));

    static CodexSegmentedButton()
    {
        IsSelectedProperty.Changed.AddClassHandler<CodexSegmentedButton>((button, _) => button.SyncClasses());
        CommandProperty.Changed.AddClassHandler<CodexSegmentedButton>((button, args) => button.OnCommandChanged(args.OldValue as ICommand, args.NewValue as ICommand));
        CommandParameterProperty.Changed.AddClassHandler<CodexSegmentedButton>((button, _) => button.SyncClasses());
        IsEnabledProperty.Changed.AddClassHandler<CodexSegmentedButton>((button, _) => button.SyncClasses());
    }

    public CodexSegmentedButton()
    {
        SyncClasses();
    }

    public string? Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public bool IsSelected
    {
        get => GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    internal bool CanSelect => IsEnabled && (Command?.CanExecute(CommandParameter) ?? true);

    protected override void OnClick()
    {
        if (_pendingPointerReleaseKind is { } updateKind)
        {
            if (updateKind != PointerUpdateKind.LeftButtonReleased || !CanSelect)
            {
                return;
            }

            base.OnClick();
            if (Command is null)
            {
                _selectionHandledByPointerRelease = TryHandlePointerActivation(updateKind);
            }

            return;
        }

        if (!CanSelect)
        {
            return;
        }

        base.OnClick();

        if (Command is not null)
        {
            return;
        }

        _ = TrySelect(CodexSegmentedControlValueChangeSource.Keyboard);
    }

    internal bool TryHandlePointerActivation(PointerUpdateKind updateKind)
    {
        if (updateKind != PointerUpdateKind.LeftButtonReleased || Command is not null)
        {
            return false;
        }

        return TrySelect(CodexSegmentedControlValueChangeSource.Pointer);
    }

    internal bool TrySelect(CodexSegmentedControlValueChangeSource source = CodexSegmentedControlValueChangeSource.Programmatic)
    {
        if (!CanSelect || Command is not null)
        {
            return false;
        }

        var owner = this.GetVisualAncestors().OfType<CodexSegmentedControl>().FirstOrDefault()
            ?? this.GetLogicalAncestors().OfType<CodexSegmentedControl>().FirstOrDefault();

        if (owner is not null)
        {
            return owner.SelectButton(this, source);
        }

        SelectSiblingButtons();
        return true;
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

    private void SelectSiblingButtons()
    {
        if (!CanSelect)
        {
            return;
        }

        var parent = this.GetLogicalParent();
        if (parent is null)
        {
            IsSelected = true;
            return;
        }

        foreach (var child in parent.GetLogicalChildren())
        {
            if (child is CodexSegmentedButton button)
            {
                button.IsSelected = ReferenceEquals(button, this);
            }
        }

        this.GetVisualAncestors()
            .OfType<CodexSegmentedControl>()
            .FirstOrDefault()
            ?.QueueSelectionIndicatorUpdate();
    }

    private void SyncClasses()
    {
        Classes.Set("selected", IsSelected);
        Classes.Set("can-select", CanSelect);
        Classes.Set("command-blocked", Command is not null && IsEnabled && !CanSelect);
        this.GetVisualAncestors()
            .OfType<CodexSegmentedControl>()
            .FirstOrDefault()
            ?.QueueSelectionIndicatorUpdate();
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
}
