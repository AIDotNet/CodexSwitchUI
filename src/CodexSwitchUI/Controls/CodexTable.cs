using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using System.Collections;
using System.Collections.Generic;

namespace CodexSwitchUI.Controls;

public enum CodexTableCellAlignment
{
    Left,
    Center,
    Right
}

public class CodexTable : ContentControl
{
    public static readonly StyledProperty<bool> IsHoverableProperty =
        AvaloniaProperty.Register<CodexTable, bool>(nameof(IsHoverable), true);

    public static readonly StyledProperty<bool> IsStripedProperty =
        AvaloniaProperty.Register<CodexTable, bool>(nameof(IsStriped));

    public static readonly StyledProperty<bool> IsCompactProperty =
        AvaloniaProperty.Register<CodexTable, bool>(nameof(IsCompact));

    static CodexTable()
    {
        IsHoverableProperty.Changed.AddClassHandler<CodexTable>((table, _) => table.SyncClasses());
        IsStripedProperty.Changed.AddClassHandler<CodexTable>((table, _) => table.SyncClasses());
        IsCompactProperty.Changed.AddClassHandler<CodexTable>((table, _) => table.SyncClasses());
    }

    public CodexTable()
    {
        SyncClasses();
    }

    public bool IsHoverable
    {
        get => GetValue(IsHoverableProperty);
        set => SetValue(IsHoverableProperty, value);
    }

    public bool IsStriped
    {
        get => GetValue(IsStripedProperty);
        set => SetValue(IsStripedProperty, value);
    }

    public bool IsCompact
    {
        get => GetValue(IsCompactProperty);
        set => SetValue(IsCompactProperty, value);
    }

    private void SyncClasses()
    {
        Classes.Set("hoverable", IsHoverable);
        Classes.Set("striped", IsStriped);
        Classes.Set("compact", IsCompact);
    }
}

public class CodexPinnedTable : TemplatedControl
{
    private static readonly TimeSpan PageTransitionDuration = TimeSpan.FromMilliseconds(160);

    private ScrollViewer? _headerScrollViewer;
    private ScrollViewer? _bodyScrollViewer;
    private readonly List<Control> _transitionTargets = new();
    private bool _isSyncingScroll;
    private int _transitionVersion;

    public static readonly StyledProperty<IEnumerable?> ItemsSourceProperty =
        AvaloniaProperty.Register<CodexPinnedTable, IEnumerable?>(nameof(ItemsSource));

    public static readonly StyledProperty<object?> StartHeaderProperty =
        AvaloniaProperty.Register<CodexPinnedTable, object?>(nameof(StartHeader));

    public static readonly StyledProperty<object?> MiddleHeaderProperty =
        AvaloniaProperty.Register<CodexPinnedTable, object?>(nameof(MiddleHeader));

    public static readonly StyledProperty<object?> EndHeaderProperty =
        AvaloniaProperty.Register<CodexPinnedTable, object?>(nameof(EndHeader));

    public static readonly StyledProperty<IDataTemplate?> StartHeaderTemplateProperty =
        AvaloniaProperty.Register<CodexPinnedTable, IDataTemplate?>(nameof(StartHeaderTemplate));

    public static readonly StyledProperty<IDataTemplate?> MiddleHeaderTemplateProperty =
        AvaloniaProperty.Register<CodexPinnedTable, IDataTemplate?>(nameof(MiddleHeaderTemplate));

    public static readonly StyledProperty<IDataTemplate?> EndHeaderTemplateProperty =
        AvaloniaProperty.Register<CodexPinnedTable, IDataTemplate?>(nameof(EndHeaderTemplate));

    public static readonly StyledProperty<IDataTemplate?> StartCellTemplateProperty =
        AvaloniaProperty.Register<CodexPinnedTable, IDataTemplate?>(nameof(StartCellTemplate));

    public static readonly StyledProperty<IDataTemplate?> MiddleCellTemplateProperty =
        AvaloniaProperty.Register<CodexPinnedTable, IDataTemplate?>(nameof(MiddleCellTemplate));

    public static readonly StyledProperty<IDataTemplate?> EndCellTemplateProperty =
        AvaloniaProperty.Register<CodexPinnedTable, IDataTemplate?>(nameof(EndCellTemplate));

    public static readonly StyledProperty<GridLength> StartColumnWidthProperty =
        AvaloniaProperty.Register<CodexPinnedTable, GridLength>(nameof(StartColumnWidth), new GridLength(88));

    public static readonly StyledProperty<GridLength> EndColumnWidthProperty =
        AvaloniaProperty.Register<CodexPinnedTable, GridLength>(nameof(EndColumnWidth), new GridLength(52));

    public static readonly StyledProperty<double> MiddleMinWidthProperty =
        AvaloniaProperty.Register<CodexPinnedTable, double>(nameof(MiddleMinWidth), 640);

    public static readonly StyledProperty<double> HeaderHeightProperty =
        AvaloniaProperty.Register<CodexPinnedTable, double>(nameof(HeaderHeight), 40);

    public static readonly StyledProperty<bool> IsCompactProperty =
        AvaloniaProperty.Register<CodexPinnedTable, bool>(nameof(IsCompact));

    public static readonly StyledProperty<bool> IsLoadingProperty =
        AvaloniaProperty.Register<CodexPinnedTable, bool>(nameof(IsLoading));

    public static readonly StyledProperty<object?> TransitionKeyProperty =
        AvaloniaProperty.Register<CodexPinnedTable, object?>(nameof(TransitionKey));

    public static readonly StyledProperty<double> TransitionOffsetProperty =
        AvaloniaProperty.Register<CodexPinnedTable, double>(nameof(TransitionOffset), 7);

    static CodexPinnedTable()
    {
        IsCompactProperty.Changed.AddClassHandler<CodexPinnedTable>((table, _) => table.SyncClasses());
        IsLoadingProperty.Changed.AddClassHandler<CodexPinnedTable>((table, _) => table.SyncClasses());
        TransitionKeyProperty.Changed.AddClassHandler<CodexPinnedTable>((table, _) => table.StartPageTransition());
    }

    public CodexPinnedTable()
    {
        ClipToBounds = true;
        SyncClasses();
    }

    public IEnumerable? ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public object? StartHeader
    {
        get => GetValue(StartHeaderProperty);
        set => SetValue(StartHeaderProperty, value);
    }

    public object? MiddleHeader
    {
        get => GetValue(MiddleHeaderProperty);
        set => SetValue(MiddleHeaderProperty, value);
    }

    public object? EndHeader
    {
        get => GetValue(EndHeaderProperty);
        set => SetValue(EndHeaderProperty, value);
    }

    public IDataTemplate? StartHeaderTemplate
    {
        get => GetValue(StartHeaderTemplateProperty);
        set => SetValue(StartHeaderTemplateProperty, value);
    }

    public IDataTemplate? MiddleHeaderTemplate
    {
        get => GetValue(MiddleHeaderTemplateProperty);
        set => SetValue(MiddleHeaderTemplateProperty, value);
    }

    public IDataTemplate? EndHeaderTemplate
    {
        get => GetValue(EndHeaderTemplateProperty);
        set => SetValue(EndHeaderTemplateProperty, value);
    }

    public IDataTemplate? StartCellTemplate
    {
        get => GetValue(StartCellTemplateProperty);
        set => SetValue(StartCellTemplateProperty, value);
    }

    public IDataTemplate? MiddleCellTemplate
    {
        get => GetValue(MiddleCellTemplateProperty);
        set => SetValue(MiddleCellTemplateProperty, value);
    }

    public IDataTemplate? EndCellTemplate
    {
        get => GetValue(EndCellTemplateProperty);
        set => SetValue(EndCellTemplateProperty, value);
    }

    public GridLength StartColumnWidth
    {
        get => GetValue(StartColumnWidthProperty);
        set => SetValue(StartColumnWidthProperty, value);
    }

    public GridLength EndColumnWidth
    {
        get => GetValue(EndColumnWidthProperty);
        set => SetValue(EndColumnWidthProperty, value);
    }

    public double MiddleMinWidth
    {
        get => GetValue(MiddleMinWidthProperty);
        set => SetValue(MiddleMinWidthProperty, value);
    }

    public double HeaderHeight
    {
        get => GetValue(HeaderHeightProperty);
        set => SetValue(HeaderHeightProperty, value);
    }

    public bool IsCompact
    {
        get => GetValue(IsCompactProperty);
        set => SetValue(IsCompactProperty, value);
    }

    public bool IsLoading
    {
        get => GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }

    public object? TransitionKey
    {
        get => GetValue(TransitionKeyProperty);
        set => SetValue(TransitionKeyProperty, value);
    }

    public double TransitionOffset
    {
        get => GetValue(TransitionOffsetProperty);
        set => SetValue(TransitionOffsetProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        if (_bodyScrollViewer is not null)
            _bodyScrollViewer.ScrollChanged -= OnBodyScrollChanged;

        base.OnApplyTemplate(e);

        _headerScrollViewer = e.NameScope.Find<ScrollViewer>("PART_HeaderScrollViewer");
        _bodyScrollViewer = e.NameScope.Find<ScrollViewer>("PART_BodyScrollViewer");
        _transitionTargets.Clear();
        AddTransitionTarget(e.NameScope.Find<Control>("PART_StartItemsControl"));
        AddTransitionTarget(_bodyScrollViewer);
        AddTransitionTarget(e.NameScope.Find<Control>("PART_EndItemsControl"));

        if (_bodyScrollViewer is not null)
            _bodyScrollViewer.ScrollChanged += OnBodyScrollChanged;

        SyncHeaderScroll();
    }

    private void AddTransitionTarget(Control? target)
    {
        if (target is null)
            return;

        target.Transitions =
        [
            new DoubleTransition
            {
                Property = OpacityProperty,
                Duration = PageTransitionDuration,
                Easing = new CubicEaseOut()
            }
        ];

        if (target.RenderTransform is not TranslateTransform transform)
        {
            transform = new TranslateTransform();
            target.RenderTransform = transform;
        }

        transform.Transitions =
        [
            new DoubleTransition
            {
                Property = TranslateTransform.YProperty,
                Duration = PageTransitionDuration,
                Easing = new CubicEaseOut()
            }
        ];

        _transitionTargets.Add(target);
    }

    private void OnBodyScrollChanged(object? sender, ScrollChangedEventArgs e)
    {
        SyncHeaderScroll();
    }

    private void SyncHeaderScroll()
    {
        if (_isSyncingScroll || _headerScrollViewer is null || _bodyScrollViewer is null)
            return;

        try
        {
            _isSyncingScroll = true;
            _headerScrollViewer.Offset = new Vector(_bodyScrollViewer.Offset.X, _headerScrollViewer.Offset.Y);
        }
        finally
        {
            _isSyncingScroll = false;
        }
    }

    private void StartPageTransition()
    {
        if (_transitionTargets.Count == 0)
            return;

        var version = ++_transitionVersion;
        foreach (var target in _transitionTargets)
        {
            target.Opacity = 0.72;
            if (target.RenderTransform is TranslateTransform transform)
                transform.Y = Math.Max(0, TransitionOffset);
        }

        Dispatcher.UIThread.Post(() =>
        {
            if (version != _transitionVersion)
                return;

            foreach (var target in _transitionTargets)
            {
                target.Opacity = 1;
                if (target.RenderTransform is TranslateTransform transform)
                    transform.Y = 0;
            }
        }, DispatcherPriority.Render);
    }

    private void SyncClasses()
    {
        Classes.Set("compact", IsCompact);
        Classes.Set("loading", IsLoading);
    }
}

public class CodexTableHeader : ContentControl
{
}

public class CodexTableBody : ItemsControl
{
}

public class CodexTableFooter : ContentControl
{
}

public class CodexTableRow : ContentControl
{
    public static readonly StyledProperty<bool> IsSelectedProperty =
        AvaloniaProperty.Register<CodexTableRow, bool>(nameof(IsSelected));

    static CodexTableRow()
    {
        IsSelectedProperty.Changed.AddClassHandler<CodexTableRow>((row, _) => row.SyncClasses());
    }

    public CodexTableRow()
    {
        SyncClasses();
    }

    public bool IsSelected
    {
        get => GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    private void SyncClasses()
    {
        Classes.Set("selected", IsSelected);
    }
}

public class CodexTableHead : ContentControl
{
    public static readonly StyledProperty<CodexTableCellAlignment> AlignmentProperty =
        AvaloniaProperty.Register<CodexTableHead, CodexTableCellAlignment>(nameof(Alignment));

    static CodexTableHead()
    {
        AlignmentProperty.Changed.AddClassHandler<CodexTableHead>((head, _) => head.SyncClasses());
    }

    public CodexTableHead()
    {
        SyncClasses();
    }

    public CodexTableCellAlignment Alignment
    {
        get => GetValue(AlignmentProperty);
        set => SetValue(AlignmentProperty, value);
    }

    private void SyncClasses()
    {
        Classes.Set("align-left", Alignment == CodexTableCellAlignment.Left);
        Classes.Set("align-center", Alignment == CodexTableCellAlignment.Center);
        Classes.Set("align-right", Alignment == CodexTableCellAlignment.Right);
        HorizontalContentAlignment = Alignment switch
        {
            CodexTableCellAlignment.Center => HorizontalAlignment.Center,
            CodexTableCellAlignment.Right => HorizontalAlignment.Right,
            _ => HorizontalAlignment.Left
        };
    }
}

public class CodexTableCell : ContentControl
{
    public static readonly StyledProperty<CodexTableCellAlignment> AlignmentProperty =
        AvaloniaProperty.Register<CodexTableCell, CodexTableCellAlignment>(nameof(Alignment));

    static CodexTableCell()
    {
        AlignmentProperty.Changed.AddClassHandler<CodexTableCell>((cell, _) => cell.SyncClasses());
    }

    public CodexTableCell()
    {
        SyncClasses();
    }

    public CodexTableCellAlignment Alignment
    {
        get => GetValue(AlignmentProperty);
        set => SetValue(AlignmentProperty, value);
    }

    private void SyncClasses()
    {
        Classes.Set("align-left", Alignment == CodexTableCellAlignment.Left);
        Classes.Set("align-center", Alignment == CodexTableCellAlignment.Center);
        Classes.Set("align-right", Alignment == CodexTableCellAlignment.Right);
        HorizontalContentAlignment = Alignment switch
        {
            CodexTableCellAlignment.Center => HorizontalAlignment.Center,
            CodexTableCellAlignment.Right => HorizontalAlignment.Right,
            _ => HorizontalAlignment.Left
        };
    }
}

public class CodexTableCaption : ContentControl
{
}
