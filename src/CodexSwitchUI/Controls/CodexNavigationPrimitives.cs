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

public class CodexSideNavItem : Button
{
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

    public static readonly StyledProperty<double> IndicatorWidthProperty =
        AvaloniaProperty.Register<CodexSegmentedControl, double>(nameof(IndicatorWidth));

    public static readonly StyledProperty<double> IndicatorHeightProperty =
        AvaloniaProperty.Register<CodexSegmentedControl, double>(nameof(IndicatorHeight));

    public static readonly StyledProperty<Thickness> IndicatorMarginProperty =
        AvaloniaProperty.Register<CodexSegmentedControl, Thickness>(nameof(IndicatorMargin));

    public static readonly StyledProperty<bool> IsIndicatorVisibleProperty =
        AvaloniaProperty.Register<CodexSegmentedControl, bool>(nameof(IsIndicatorVisible));

    public CodexSegmentedControl()
    {
        LayoutUpdated += (_, _) => UpdateSelectionIndicator();
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
}

public class CodexSegmentedButton : Button
{
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

    public bool IsSelected
    {
        get => GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    protected override void OnClick()
    {
        base.OnClick();
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
