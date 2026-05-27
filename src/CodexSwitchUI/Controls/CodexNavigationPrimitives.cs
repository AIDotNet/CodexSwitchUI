using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.LogicalTree;
using Avalonia.Threading;
using Avalonia.VisualTree;

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
    string? newValue) : EventArgs
{
    public CodexSegmentedButton? OldItem { get; } = oldItem;

    public CodexSegmentedButton? NewItem { get; } = newItem;

    public int OldIndex { get; } = oldIndex;

    public int NewIndex { get; } = newIndex;

    public string? OldValue { get; } = oldValue;

    public string? NewValue { get; } = newValue;
}

public sealed class CodexSideNavValueChangedEventArgs(
    CodexSideNavItem? oldItem,
    CodexSideNavItem? newItem,
    int oldIndex,
    int newIndex,
    string? oldValue,
    string? newValue) : EventArgs
{
    public CodexSideNavItem? OldItem { get; } = oldItem;

    public CodexSideNavItem? NewItem { get; } = newItem;

    public int OldIndex { get; } = oldIndex;

    public int NewIndex { get; } = newIndex;

    public string? OldValue { get; } = oldValue;

    public string? NewValue { get; } = newValue;
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

    internal void SelectItem(CodexSideNavItem item)
    {
        if (!item.IsEnabled)
        {
            return;
        }

        var items = GetSideNavItems();
        var oldItem = items.FirstOrDefault(candidate => candidate.IsSelected);
        var oldIndex = IndexOf(items, oldItem);
        var newIndex = IndexOf(items, item);
        var oldValue = ResolveItemValue(oldItem);
        var newValue = ResolveItemValue(item);

        if (ReferenceEquals(oldItem, item) && string.Equals(SelectedValue, newValue, StringComparison.Ordinal))
        {
            return;
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

        RaiseValueChanged(oldItem, item, oldIndex, newIndex, oldValue, newValue);
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

        RaiseValueChanged(oldItem, newItem, oldIndex, newIndex, oldValue, newValue);
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
        string? newValue)
    {
        if (ReferenceEquals(oldItem, newItem) && string.Equals(oldValue, newValue, StringComparison.Ordinal))
        {
            return;
        }

        ValueChanged?.Invoke(
            this,
            new CodexSideNavValueChangedEventArgs(oldItem, newItem, oldIndex, newIndex, oldValue, newValue));
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

    protected override void OnClick()
    {
        base.OnClick();

        var owner = this.GetVisualAncestors().OfType<CodexSideNav>().FirstOrDefault()
            ?? this.GetLogicalAncestors().OfType<CodexSideNav>().FirstOrDefault();

        if (owner is not null)
        {
            owner.SelectItem(this);
            return;
        }

        SelectSiblingItems();
    }

    private void SelectSiblingItems()
    {
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
        SetValue(HasIconProperty, Icon is not null);
        SetValue(HasDetailProperty, !string.IsNullOrWhiteSpace(Detail));
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

    internal void SelectButton(CodexSegmentedButton button)
    {
        if (!button.IsEnabled)
        {
            return;
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
            return;
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
        RaiseValueChanged(oldButton, button, oldIndex, newIndex, oldValue, newValue);
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
        RaiseValueChanged(oldButton, newButton, oldIndex, newIndex, oldValue, newValue);
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
        string? newValue)
    {
        if (ReferenceEquals(oldButton, newButton) && string.Equals(oldValue, newValue, StringComparison.Ordinal))
        {
            return;
        }

        ValueChanged?.Invoke(
            this,
            new CodexSegmentedControlValueChangedEventArgs(oldButton, newButton, oldIndex, newIndex, oldValue, newValue));
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
    public static readonly StyledProperty<string?> ValueProperty =
        AvaloniaProperty.Register<CodexSegmentedButton, string?>(nameof(Value));

    public static readonly StyledProperty<bool> IsSelectedProperty =
        AvaloniaProperty.Register<CodexSegmentedButton, bool>(nameof(IsSelected));

    static CodexSegmentedButton()
    {
        IsSelectedProperty.Changed.AddClassHandler<CodexSegmentedButton>((button, _) => button.SyncClasses());
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

    protected override void OnClick()
    {
        base.OnClick();

        if (Command is not null)
        {
            return;
        }

        var owner = this.GetVisualAncestors().OfType<CodexSegmentedControl>().FirstOrDefault()
            ?? this.GetLogicalAncestors().OfType<CodexSegmentedControl>().FirstOrDefault();

        if (owner is not null)
        {
            owner.SelectButton(this);
            return;
        }

        SelectSiblingButtons();
    }

    private void SelectSiblingButtons()
    {
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
        this.GetVisualAncestors()
            .OfType<CodexSegmentedControl>()
            .FirstOrDefault()
            ?.QueueSelectionIndicatorUpdate();
    }
}
