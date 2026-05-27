using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using System.Linq;

namespace CodexSwitchUI.Controls;

public enum CodexPaginationItemKind
{
    Page,
    Ellipsis
}

public sealed record CodexPaginationItem(
    CodexPaginationItemKind Kind,
    int Page,
    string Label,
    bool IsCurrent,
    bool IsEnabled)
{
    public bool IsEllipsis => Kind == CodexPaginationItemKind.Ellipsis;
}

public enum CodexPaginationPageChangeSource
{
    Programmatic,
    PageItem,
    Previous,
    Next,
    First,
    Last,
    Keyboard
}

public sealed class CodexPaginationPageChangedEventArgs(
    int oldPage,
    int newPage,
    CodexPaginationPageChangeSource source = CodexPaginationPageChangeSource.Programmatic) : EventArgs
{
    public int OldPage { get; } = oldPage;

    public int NewPage { get; } = newPage;

    public CodexPaginationPageChangeSource Source { get; } = source;
}

public class CodexPagination : TemplatedControl
{
    private CodexButton? _firstButton;
    private CodexButton? _previousButton;
    private CodexButton? _nextButton;
    private CodexButton? _lastButton;
    private int _requestedPage = 1;
    private bool _isNormalizing;

    public static readonly StyledProperty<int> PageProperty =
        AvaloniaProperty.Register<CodexPagination, int>(nameof(Page), 1);

    public static readonly StyledProperty<int> PageCountProperty =
        AvaloniaProperty.Register<CodexPagination, int>(nameof(PageCount), 1);

    public static readonly StyledProperty<int> SiblingCountProperty =
        AvaloniaProperty.Register<CodexPagination, int>(nameof(SiblingCount), 1);

    public static readonly StyledProperty<int> BoundaryCountProperty =
        AvaloniaProperty.Register<CodexPagination, int>(nameof(BoundaryCount), 1);

    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexPagination, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    public static readonly StyledProperty<bool> IsLoadingProperty =
        AvaloniaProperty.Register<CodexPagination, bool>(nameof(IsLoading));

    public static readonly StyledProperty<bool> ShowFirstLastProperty =
        AvaloniaProperty.Register<CodexPagination, bool>(nameof(ShowFirstLast), true);

    public static readonly StyledProperty<bool> IsCompactProperty =
        AvaloniaProperty.Register<CodexPagination, bool>(nameof(IsCompact));

    public static readonly StyledProperty<IReadOnlyList<CodexPaginationItem>> PageItemsProperty =
        AvaloniaProperty.Register<CodexPagination, IReadOnlyList<CodexPaginationItem>>(nameof(PageItems), Array.Empty<CodexPaginationItem>());

    public static readonly StyledProperty<bool> CanGoPreviousProperty =
        AvaloniaProperty.Register<CodexPagination, bool>(nameof(CanGoPrevious));

    public static readonly StyledProperty<bool> CanGoNextProperty =
        AvaloniaProperty.Register<CodexPagination, bool>(nameof(CanGoNext));

    public static readonly StyledProperty<bool> IsFirstPageProperty =
        AvaloniaProperty.Register<CodexPagination, bool>(nameof(IsFirstPage), true);

    public static readonly StyledProperty<bool> IsLastPageProperty =
        AvaloniaProperty.Register<CodexPagination, bool>(nameof(IsLastPage), true);

    public static readonly StyledProperty<bool> HasEllipsisProperty =
        AvaloniaProperty.Register<CodexPagination, bool>(nameof(HasEllipsis));

    static CodexPagination()
    {
        PageProperty.Changed.AddClassHandler<CodexPagination>((pagination, args) => pagination.OnPageChanged(args.NewValue is int page ? page : pagination.Page));
        PageCountProperty.Changed.AddClassHandler<CodexPagination>((pagination, _) => pagination.NormalizeAndSync());
        SiblingCountProperty.Changed.AddClassHandler<CodexPagination>((pagination, _) => pagination.NormalizeAndSync());
        BoundaryCountProperty.Changed.AddClassHandler<CodexPagination>((pagination, _) => pagination.NormalizeAndSync());
        SizeProperty.Changed.AddClassHandler<CodexPagination>((pagination, _) => pagination.SyncState());
        IsLoadingProperty.Changed.AddClassHandler<CodexPagination>((pagination, _) => pagination.SyncState());
        ShowFirstLastProperty.Changed.AddClassHandler<CodexPagination>((pagination, _) => pagination.SyncState());
        IsCompactProperty.Changed.AddClassHandler<CodexPagination>((pagination, _) => pagination.SyncState());
    }

    public CodexPagination()
    {
        Focusable = true;
        NormalizeAndSync();
    }

    public event EventHandler<CodexPaginationPageChangedEventArgs>? PageChanged;

    public int Page
    {
        get => GetValue(PageProperty);
        set => SetValue(PageProperty, value);
    }

    public int PageCount
    {
        get => GetValue(PageCountProperty);
        set => SetValue(PageCountProperty, value);
    }

    public int SiblingCount
    {
        get => GetValue(SiblingCountProperty);
        set => SetValue(SiblingCountProperty, value);
    }

    public int BoundaryCount
    {
        get => GetValue(BoundaryCountProperty);
        set => SetValue(BoundaryCountProperty, value);
    }

    public CodexControlSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    public bool IsLoading
    {
        get => GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }

    public bool ShowFirstLast
    {
        get => GetValue(ShowFirstLastProperty);
        set => SetValue(ShowFirstLastProperty, value);
    }

    public bool IsCompact
    {
        get => GetValue(IsCompactProperty);
        set => SetValue(IsCompactProperty, value);
    }

    public IReadOnlyList<CodexPaginationItem> PageItems => GetValue(PageItemsProperty);

    public bool CanGoPrevious => GetValue(CanGoPreviousProperty);

    public bool CanGoNext => GetValue(CanGoNextProperty);

    public bool IsFirstPage => GetValue(IsFirstPageProperty);

    public bool IsLastPage => GetValue(IsLastPageProperty);

    public bool HasEllipsis => GetValue(HasEllipsisProperty);

    public bool SelectPage(int page)
    {
        return SelectPage(page, CodexPaginationPageChangeSource.Programmatic);
    }

    internal bool SelectPage(int page, CodexPaginationPageChangeSource source)
    {
        var target = NormalizePage(page);

        if (!CanNavigate() || target == Page || target < 1 || target > Math.Max(0, PageCount))
        {
            return false;
        }

        var oldPage = Page;
        Page = target;
        PageChanged?.Invoke(this, new CodexPaginationPageChangedEventArgs(oldPage, Page, source));
        return true;
    }

    public bool GoPrevious()
    {
        return CanGoPrevious && SelectPage(Page - 1, CodexPaginationPageChangeSource.Previous);
    }

    public bool GoNext()
    {
        return CanGoNext && SelectPage(Page + 1, CodexPaginationPageChangeSource.Next);
    }

    public bool GoFirst()
    {
        return CanGoPrevious && SelectPage(1, CodexPaginationPageChangeSource.First);
    }

    public bool GoLast()
    {
        return CanGoNext && SelectPage(PageCount, CodexPaginationPageChangeSource.Last);
    }

    internal bool TryHandleNavigationKey(Key key)
    {
        return key switch
        {
            Key.Home => CanGoPrevious && SelectPage(1, CodexPaginationPageChangeSource.Keyboard),
            Key.End => CanGoNext && SelectPage(PageCount, CodexPaginationPageChangeSource.Keyboard),
            Key.Left or Key.PageUp => CanGoPrevious && SelectPage(Page - 1, CodexPaginationPageChangeSource.Keyboard),
            Key.Right or Key.PageDown => CanGoNext && SelectPage(Page + 1, CodexPaginationPageChangeSource.Keyboard),
            _ => false
        };
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        DetachButtons();

        base.OnApplyTemplate(e);

        _firstButton = e.NameScope.Find<CodexButton>("PART_FirstButton");
        _previousButton = e.NameScope.Find<CodexButton>("PART_PreviousButton");
        _nextButton = e.NameScope.Find<CodexButton>("PART_NextButton");
        _lastButton = e.NameScope.Find<CodexButton>("PART_LastButton");

        AttachButtons();
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (TryHandleNavigationKey(e.Key))
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
            SyncState();
        }
    }

    private void AttachButtons()
    {
        if (_firstButton is not null)
        {
            _firstButton.Click += OnFirstClicked;
        }

        if (_previousButton is not null)
        {
            _previousButton.Click += OnPreviousClicked;
        }

        if (_nextButton is not null)
        {
            _nextButton.Click += OnNextClicked;
        }

        if (_lastButton is not null)
        {
            _lastButton.Click += OnLastClicked;
        }
    }

    private void DetachButtons()
    {
        if (_firstButton is not null)
        {
            _firstButton.Click -= OnFirstClicked;
        }

        if (_previousButton is not null)
        {
            _previousButton.Click -= OnPreviousClicked;
        }

        if (_nextButton is not null)
        {
            _nextButton.Click -= OnNextClicked;
        }

        if (_lastButton is not null)
        {
            _lastButton.Click -= OnLastClicked;
        }
    }

    private void OnFirstClicked(object? sender, RoutedEventArgs e)
    {
        GoFirst();
    }

    private void OnPreviousClicked(object? sender, RoutedEventArgs e)
    {
        GoPrevious();
    }

    private void OnNextClicked(object? sender, RoutedEventArgs e)
    {
        GoNext();
    }

    private void OnLastClicked(object? sender, RoutedEventArgs e)
    {
        GoLast();
    }

    private void OnPageChanged(int page)
    {
        if (!_isNormalizing)
        {
            _requestedPage = page;
        }

        NormalizeAndSync();
    }

    private void NormalizeAndSync()
    {
        if (_isNormalizing)
        {
            SyncState();
            return;
        }

        var normalizedPage = NormalizePage(_requestedPage);
        var normalizedSiblingCount = Math.Max(0, SiblingCount);
        var normalizedBoundaryCount = Math.Max(0, BoundaryCount);

        if (normalizedPage == Page
            && normalizedSiblingCount == SiblingCount
            && normalizedBoundaryCount == BoundaryCount)
        {
            SyncState();
            return;
        }

        _isNormalizing = true;
        Page = normalizedPage;
        SiblingCount = normalizedSiblingCount;
        BoundaryCount = normalizedBoundaryCount;
        _isNormalizing = false;
        SyncState();
    }

    private int NormalizePage(int page)
    {
        if (PageCount <= 0)
        {
            return 0;
        }

        return Math.Clamp(page, 1, PageCount);
    }

    private bool CanNavigate()
    {
        return IsEnabled && !IsLoading && PageCount > 0;
    }

    private void SyncState()
    {
        var items = BuildPageItems();
        var canNavigate = CanNavigate();
        var isFirstPage = Page <= 1 || PageCount <= 0;
        var isLastPage = Page >= PageCount || PageCount <= 0;

        SetValue(PageItemsProperty, items);
        SetValue(CanGoPreviousProperty, canNavigate && !isFirstPage);
        SetValue(CanGoNextProperty, canNavigate && !isLastPage);
        SetValue(IsFirstPageProperty, isFirstPage);
        SetValue(IsLastPageProperty, isLastPage);
        SetValue(HasEllipsisProperty, items.Any(item => item.IsEllipsis));

        CodexClassSync.SetSize(Classes, Size);
        Classes.Set("loading", IsLoading);
        Classes.Set("compact", IsCompact);
        Classes.Set("show-first-last", ShowFirstLast);
        Classes.Set("first-page", isFirstPage);
        Classes.Set("last-page", isLastPage);
        Classes.Set("single-page", PageCount <= 1);
        Classes.Set("empty", PageCount <= 0);
        Classes.Set("has-ellipsis", HasEllipsis);
        Classes.Set("can-previous", CanGoPrevious);
        Classes.Set("can-next", CanGoNext);
    }

    private IReadOnlyList<CodexPaginationItem> BuildPageItems()
    {
        if (PageCount <= 0)
        {
            return Array.Empty<CodexPaginationItem>();
        }

        if (PageCount <= Math.Max(5, BoundaryCount * 2 + SiblingCount * 2 + 3))
        {
            return Enumerable.Range(1, PageCount)
                .Select(page => new CodexPaginationItem(
                    CodexPaginationItemKind.Page,
                    page,
                    page.ToString(),
                    page == Page,
                    IsEnabled: true))
                .ToArray();
        }

        var pages = new SortedSet<int>();
        AddRange(pages, 1, BoundaryCount);
        AddRange(pages, Page - SiblingCount, Page + SiblingCount);
        AddRange(pages, PageCount - BoundaryCount + 1, PageCount);

        var items = new List<CodexPaginationItem>();
        var previousPage = 0;

        foreach (var page in pages.Where(page => page >= 1 && page <= PageCount))
        {
            if (previousPage > 0 && page - previousPage > 1)
            {
                items.Add(new CodexPaginationItem(
                    CodexPaginationItemKind.Ellipsis,
                    previousPage + 1,
                    "...",
                    IsCurrent: false,
                    IsEnabled: false));
            }

            items.Add(new CodexPaginationItem(
                CodexPaginationItemKind.Page,
                page,
                page.ToString(),
                page == Page,
                IsEnabled: true));

            previousPage = page;
        }

        return items;
    }

    private static void AddRange(ISet<int> pages, int start, int end)
    {
        for (var page = start; page <= end; page++)
        {
            pages.Add(page);
        }
    }
}

public class CodexPaginationPageButton : CodexButton
{
    public static readonly StyledProperty<int> PageProperty =
        AvaloniaProperty.Register<CodexPaginationPageButton, int>(nameof(Page));

    public static readonly StyledProperty<bool> IsCurrentProperty =
        AvaloniaProperty.Register<CodexPaginationPageButton, bool>(nameof(IsCurrent));

    public static readonly StyledProperty<bool> IsEllipsisProperty =
        AvaloniaProperty.Register<CodexPaginationPageButton, bool>(nameof(IsEllipsis));

    static CodexPaginationPageButton()
    {
        PageProperty.Changed.AddClassHandler<CodexPaginationPageButton>((button, _) => button.SyncPageClasses());
        IsCurrentProperty.Changed.AddClassHandler<CodexPaginationPageButton>((button, _) => button.SyncPageClasses());
        IsEllipsisProperty.Changed.AddClassHandler<CodexPaginationPageButton>((button, _) => button.SyncPageClasses());
    }

    public CodexPaginationPageButton()
    {
        Variant = CodexControlVariant.Ghost;
        Size = CodexControlSize.Icon;
        SyncPageClasses();
    }

    public int Page
    {
        get => GetValue(PageProperty);
        set => SetValue(PageProperty, value);
    }

    public bool IsCurrent
    {
        get => GetValue(IsCurrentProperty);
        set => SetValue(IsCurrentProperty, value);
    }

    public bool IsEllipsis
    {
        get => GetValue(IsEllipsisProperty);
        set => SetValue(IsEllipsisProperty, value);
    }

    protected override void OnClick()
    {
        if (IsEllipsis || IsCurrent)
        {
            return;
        }

        FindPagination()?.SelectPage(Page, CodexPaginationPageChangeSource.PageItem);
    }

    private CodexPagination? FindPagination()
    {
        for (var parent = this.GetLogicalParent(); parent is not null; parent = parent.GetLogicalParent())
        {
            if (parent is CodexPagination pagination)
            {
                return pagination;
            }
        }

        return null;
    }

    private void SyncPageClasses()
    {
        Classes.Set("page-item", !IsEllipsis);
        Classes.Set("current", IsCurrent);
        Classes.Set("ellipsis", IsEllipsis);
    }
}
