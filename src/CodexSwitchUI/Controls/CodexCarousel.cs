using System.Collections.Specialized;
using System.Windows.Input;
using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace CodexSwitchUI.Controls;

public enum CodexCarouselSelectionChangeSource
{
    Programmatic,
    Previous,
    Next,
    First,
    Last,
    Keyboard
}

[PseudoClasses(CodexFocusVisible.PseudoClass)]
public class CodexCarousel : ItemsControl
{
    private readonly CarouselPartCommand _previousCommand;
    private readonly CarouselPartCommand _nextCommand;
    private bool _isNormalizing;
    private int? _pendingSelectedIndex;
    private CodexCarouselSelectionChangeSource? _pendingSelectionSource;

    public static readonly StyledProperty<int> SelectedIndexProperty =
        AvaloniaProperty.Register<CodexCarousel, int>(nameof(SelectedIndex));

    public static readonly StyledProperty<Orientation> OrientationProperty =
        AvaloniaProperty.Register<CodexCarousel, Orientation>(nameof(Orientation), Orientation.Horizontal);

    public static readonly StyledProperty<bool> LoopProperty =
        AvaloniaProperty.Register<CodexCarousel, bool>(nameof(Loop));

    public static readonly StyledProperty<bool> ShowNavigationProperty =
        AvaloniaProperty.Register<CodexCarousel, bool>(nameof(ShowNavigation), true);

    public static readonly StyledProperty<bool> ShowStatusProperty =
        AvaloniaProperty.Register<CodexCarousel, bool>(nameof(ShowStatus), true);

    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexCarousel, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    public static readonly StyledProperty<int> SlideCountProperty =
        AvaloniaProperty.Register<CodexCarousel, int>(nameof(SlideCount));

    public static readonly StyledProperty<bool> CanGoPreviousProperty =
        AvaloniaProperty.Register<CodexCarousel, bool>(nameof(CanGoPrevious));

    public static readonly StyledProperty<bool> CanGoNextProperty =
        AvaloniaProperty.Register<CodexCarousel, bool>(nameof(CanGoNext));

    public static readonly StyledProperty<bool> HasMultipleItemsProperty =
        AvaloniaProperty.Register<CodexCarousel, bool>(nameof(HasMultipleItems));

    public static readonly StyledProperty<string> StatusTextProperty =
        AvaloniaProperty.Register<CodexCarousel, string>(nameof(StatusText), "Slide 0 of 0");

    static CodexCarousel()
    {
        SelectedIndexProperty.Changed.AddClassHandler<CodexCarousel>((carousel, args) =>
            carousel.OnSelectedIndexChanged(args.OldValue is int oldIndex ? oldIndex : -1));
        OrientationProperty.Changed.AddClassHandler<CodexCarousel>((carousel, _) =>
        {
            carousel.SyncClasses();
            carousel.SyncItemStates();
        });
        LoopProperty.Changed.AddClassHandler<CodexCarousel>((carousel, _) => carousel.SyncState());
        ShowNavigationProperty.Changed.AddClassHandler<CodexCarousel>((carousel, _) => carousel.SyncClasses());
        ShowStatusProperty.Changed.AddClassHandler<CodexCarousel>((carousel, _) => carousel.SyncClasses());
        SizeProperty.Changed.AddClassHandler<CodexCarousel>((carousel, _) =>
        {
            carousel.SyncClasses();
            carousel.SyncItemStates();
        });
    }

    public CodexCarousel()
    {
        Focusable = true;
        ItemsView.CollectionChanged += OnItemsViewCollectionChanged;
        _previousCommand = new CarouselPartCommand(this, carousel => carousel.CanGoPrevious, carousel => carousel.GoPrevious());
        _nextCommand = new CarouselPartCommand(this, carousel => carousel.CanGoNext, carousel => carousel.GoNext());
        AutomationProperties.SetIsControlElementOverride(this, true);
        SyncState();
    }

    public event EventHandler<CodexCarouselSelectionChangedEventArgs>? SelectionChanged;

    public int SelectedIndex
    {
        get => GetValue(SelectedIndexProperty);
        set => SetValue(SelectedIndexProperty, value);
    }

    public Orientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    public bool Loop
    {
        get => GetValue(LoopProperty);
        set => SetValue(LoopProperty, value);
    }

    public bool ShowNavigation
    {
        get => GetValue(ShowNavigationProperty);
        set => SetValue(ShowNavigationProperty, value);
    }

    public bool ShowStatus
    {
        get => GetValue(ShowStatusProperty);
        set => SetValue(ShowStatusProperty, value);
    }

    public CodexControlSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    public int SlideCount => GetValue(SlideCountProperty);

    public bool CanGoPrevious => GetValue(CanGoPreviousProperty);

    public bool CanGoNext => GetValue(CanGoNextProperty);

    public bool HasMultipleItems => GetValue(HasMultipleItemsProperty);

    public string StatusText => GetValue(StatusTextProperty);

    public ICommand PreviousCommand => _previousCommand;

    public ICommand NextCommand => _nextCommand;

    public bool SelectIndex(int index)
    {
        return SelectIndex(index, CodexCarouselSelectionChangeSource.Programmatic);
    }

    private bool SelectIndex(int index, CodexCarouselSelectionChangeSource source)
    {
        var normalized = NormalizeIndex(index);
        if (normalized < 0 || normalized == SelectedIndex)
        {
            return false;
        }

        _pendingSelectionSource = source;
        SelectedIndex = normalized;
        return true;
    }

    public bool GoPrevious()
    {
        return MovePrevious(CodexCarouselSelectionChangeSource.Previous);
    }

    private bool MovePrevious(CodexCarouselSelectionChangeSource source)
    {
        if (!CanGoPrevious)
        {
            return false;
        }

        var previous = SelectedIndex - 1;
        if (previous < 0 && Loop)
        {
            previous = SlideCount - 1;
        }

        return SelectIndex(previous, source);
    }

    public bool GoNext()
    {
        return MoveNext(CodexCarouselSelectionChangeSource.Next);
    }

    private bool MoveNext(CodexCarouselSelectionChangeSource source)
    {
        if (!CanGoNext)
        {
            return false;
        }

        var next = SelectedIndex + 1;
        if (next >= SlideCount && Loop)
        {
            next = 0;
        }

        return SelectIndex(next, source);
    }

    public bool GoFirst()
    {
        return SelectIndex(0, CodexCarouselSelectionChangeSource.First);
    }

    public bool GoLast()
    {
        return SelectIndex(SlideCount - 1, CodexCarouselSelectionChangeSource.Last);
    }

    internal bool TryHandleNavigationKey(Key key)
    {
        return key switch
        {
            Key.Home => SelectIndex(0, CodexCarouselSelectionChangeSource.Keyboard),
            Key.End => SelectIndex(SlideCount - 1, CodexCarouselSelectionChangeSource.Keyboard),
            Key.Left or Key.PageUp when Orientation == Orientation.Horizontal => MovePrevious(CodexCarouselSelectionChangeSource.Keyboard),
            Key.Right or Key.PageDown when Orientation == Orientation.Horizontal => MoveNext(CodexCarouselSelectionChangeSource.Keyboard),
            Key.Up or Key.PageUp when Orientation == Orientation.Vertical => MovePrevious(CodexCarouselSelectionChangeSource.Keyboard),
            Key.Down or Key.PageDown when Orientation == Orientation.Vertical => MoveNext(CodexCarouselSelectionChangeSource.Keyboard),
            _ => false
        };
    }

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        return new CodexCarouselItem();
    }

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        recycleKey = null;
        return item is not CodexCarouselItem;
    }

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);

        if (container is CodexCarouselItem carouselItem)
        {
            if (item is not CodexCarouselItem)
            {
                carouselItem.SetCurrentValue(ContentControl.ContentProperty, item);
            }

            SyncItemState(carouselItem, index);
        }
    }

    protected override void ClearContainerForItemOverride(Control element)
    {
        if (element is CodexCarouselItem item)
        {
            item.Owner = null;
            item.ClearValue(CodexCarouselItem.IndexProperty);
            item.ClearValue(CodexCarouselItem.IsSelectedProperty);
        }

        base.ClearContainerForItemOverride(element);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        SyncState();
        ScrollSelectedIntoView();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _pendingSelectedIndex = null;
        SyncState();
        ScrollSelectedIntoView();
    }

    protected override void OnGotFocus(FocusChangedEventArgs e)
    {
        base.OnGotFocus(e);
        PseudoClasses.Set(CodexFocusVisible.PseudoClass, CodexFocusVisible.FromFocusChange(e));
    }

    protected override void OnLostFocus(FocusChangedEventArgs e)
    {
        base.OnLostFocus(e);
        PseudoClasses.Set(CodexFocusVisible.PseudoClass, false);
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        PseudoClasses.Set(CodexFocusVisible.PseudoClass, false);
        base.OnPointerPressed(e);
        Focus();
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

    private void OnItemsViewCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (_pendingSelectedIndex is int pendingIndex)
        {
            if (ItemsView.Count > pendingIndex)
            {
                _pendingSelectedIndex = null;
            }
            else
            {
                SyncState(preservePendingIndex: true);
                return;
            }
        }

        SyncState();
    }

    private void OnSelectedIndexChanged(int oldIndex)
    {
        if (_isNormalizing)
        {
            return;
        }

        if (ItemsView.Count == 0 && SelectedIndex >= 0)
        {
            _pendingSelectedIndex = SelectedIndex;
            SyncState(preservePendingIndex: true);
            return;
        }

        _pendingSelectedIndex = null;
        var normalized = NormalizeIndex(SelectedIndex);
        if (normalized != SelectedIndex)
        {
            _isNormalizing = true;
            SetCurrentValue(SelectedIndexProperty, normalized);
            _isNormalizing = false;
            return;
        }

        SyncState();
        ScrollSelectedIntoView();
        if (oldIndex != SelectedIndex)
        {
            var source = _pendingSelectionSource ?? CodexCarouselSelectionChangeSource.Programmatic;
            _pendingSelectionSource = null;
            SelectionChanged?.Invoke(
                this,
                new CodexCarouselSelectionChangedEventArgs(
                    oldIndex,
                    SelectedIndex,
                    ItemAt(oldIndex),
                    ItemAt(SelectedIndex),
                    source));
        }
    }

    private object? ItemAt(int index)
    {
        return index >= 0 && index < ItemsView.Count ? ItemsView[index] : null;
    }

    private void SyncState(bool preservePendingIndex = false)
    {
        var itemCount = ItemsView.Count;
        var normalizedIndex = preservePendingIndex ? SelectedIndex : NormalizeIndex(SelectedIndex);
        if (normalizedIndex != SelectedIndex)
        {
            _isNormalizing = true;
            SetCurrentValue(SelectedIndexProperty, normalizedIndex);
            _isNormalizing = false;
        }

        var selectedIndexIsValid = SelectedIndex >= 0 && SelectedIndex < itemCount;
        SetValue(SlideCountProperty, itemCount);
        SetValue(HasMultipleItemsProperty, itemCount > 1);
        SetValue(CanGoPreviousProperty, selectedIndexIsValid && itemCount > 1 && (Loop || SelectedIndex > 0));
        SetValue(CanGoNextProperty, selectedIndexIsValid && itemCount > 1 && (Loop || SelectedIndex < itemCount - 1));
        SetValue(StatusTextProperty, itemCount == 0
            ? "Slide 0 of 0"
            : selectedIndexIsValid
                ? $"Slide {SelectedIndex + 1} of {itemCount}"
                : $"Slide 0 of {itemCount}");

        SyncClasses();
        SyncItemStates();
        _previousCommand.RaiseCanExecuteChanged();
        _nextCommand.RaiseCanExecuteChanged();
    }

    private void SyncClasses()
    {
        var hasSelection = SlideCount > 0 && SelectedIndex >= 0 && SelectedIndex < SlideCount;

        CodexClassSync.SetSize(Classes, Size);
        Classes.Set("carousel", true);
        Classes.Set("horizontal", Orientation == Orientation.Horizontal);
        Classes.Set("vertical", Orientation == Orientation.Vertical);
        Classes.Set("loop", Loop);
        Classes.Set("can-previous", CanGoPrevious);
        Classes.Set("can-next", CanGoNext);
        Classes.Set("previous-disabled", !CanGoPrevious);
        Classes.Set("next-disabled", !CanGoNext);
        Classes.Set("at-start", !hasSelection || SelectedIndex <= 0);
        Classes.Set("at-end", !hasSelection || SelectedIndex >= SlideCount - 1);
        Classes.Set("has-items", SlideCount > 0);
        Classes.Set("empty", SlideCount == 0);
        Classes.Set("has-multiple", HasMultipleItems);
        Classes.Set("show-navigation", ShowNavigation);
        Classes.Set("hide-navigation", !ShowNavigation);
        Classes.Set("show-status", ShowStatus);
        Classes.Set("hide-status", !ShowStatus);
    }

    private void SyncItemStates()
    {
        for (var index = 0; index < ItemsView.Count; index++)
        {
            if (GetItemControl(index) is { } item)
            {
                SyncItemState(item, index);
            }
        }
    }

    private void SyncItemState(CodexCarouselItem item, int index)
    {
        item.Owner = this;
        item.SetCurrentValue(CodexCarouselItem.IndexProperty, index);
        item.SetCurrentValue(CodexCarouselItem.IsSelectedProperty, index == SelectedIndex);
        item.SetCurrentValue(CodexCarouselItem.OrientationProperty, Orientation);
        item.SetCurrentValue(CodexCarouselItem.SizeProperty, Size);
        item.SyncClasses();
    }

    private CodexCarouselItem? GetItemControl(int index)
    {
        return ItemsView[index] as CodexCarouselItem
               ?? ContainerFromIndex(index) as CodexCarouselItem;
    }

    private int NormalizeIndex(int index)
    {
        if (ItemsView.Count == 0)
        {
            return -1;
        }

        return Math.Clamp(index, 0, ItemsView.Count - 1);
    }

    private void ScrollSelectedIntoView()
    {
        if (SelectedIndex < 0 || SelectedIndex >= ItemsView.Count)
        {
            return;
        }

        Dispatcher.UIThread.Post(() =>
        {
            if (!this.IsAttachedToVisualTree())
            {
                return;
            }

            var item = GetItemControl(SelectedIndex);
            item?.BringIntoView();
        }, DispatcherPriority.Loaded);
    }

    private sealed class CarouselPartCommand(
        CodexCarousel carousel,
        Func<CodexCarousel, bool> canExecute,
        Func<CodexCarousel, bool> execute) : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return canExecute(carousel);
        }

        public void Execute(object? parameter)
        {
            execute(carousel);
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}

public sealed class CodexCarouselSelectionChangedEventArgs(
    int oldIndex,
    int newIndex,
    object? oldItem = null,
    object? newItem = null,
    CodexCarouselSelectionChangeSource source = CodexCarouselSelectionChangeSource.Programmatic) : EventArgs
{
    public int OldIndex { get; } = oldIndex;

    public int NewIndex { get; } = newIndex;

    public object? OldItem { get; } = oldItem;

    public object? NewItem { get; } = newItem;

    public CodexCarouselSelectionChangeSource Source { get; } = source;
}

public class CodexCarouselItem : ContentControl
{
    public static readonly StyledProperty<int> IndexProperty =
        AvaloniaProperty.Register<CodexCarouselItem, int>(nameof(Index), -1);

    public static readonly StyledProperty<bool> IsSelectedProperty =
        AvaloniaProperty.Register<CodexCarouselItem, bool>(nameof(IsSelected));

    public static readonly StyledProperty<Orientation> OrientationProperty =
        AvaloniaProperty.Register<CodexCarouselItem, Orientation>(nameof(Orientation), Orientation.Horizontal);

    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexCarouselItem, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    static CodexCarouselItem()
    {
        IndexProperty.Changed.AddClassHandler<CodexCarouselItem>((item, _) => item.SyncClasses());
        IsSelectedProperty.Changed.AddClassHandler<CodexCarouselItem>((item, _) => item.SyncClasses());
        OrientationProperty.Changed.AddClassHandler<CodexCarouselItem>((item, _) => item.SyncClasses());
        SizeProperty.Changed.AddClassHandler<CodexCarouselItem>((item, _) => item.SyncClasses());
    }

    public CodexCarouselItem()
    {
        Focusable = false;
        SyncClasses();
    }

    public int Index
    {
        get => GetValue(IndexProperty);
        set => SetValue(IndexProperty, value);
    }

    public bool IsSelected
    {
        get => GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    public Orientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    public CodexControlSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    internal CodexCarousel? Owner { get; set; }

    internal void SyncClasses()
    {
        CodexClassSync.SetSize(Classes, Size);
        Classes.Set("carousel-item", true);
        Classes.Set("selected", IsSelected);
        Classes.Set("before-selected", Owner is not null && Index >= 0 && Index < Owner.SelectedIndex);
        Classes.Set("after-selected", Owner is not null && Index > Owner.SelectedIndex);
        Classes.Set("horizontal", Orientation == Orientation.Horizontal);
        Classes.Set("vertical", Orientation == Orientation.Vertical);
    }
}
