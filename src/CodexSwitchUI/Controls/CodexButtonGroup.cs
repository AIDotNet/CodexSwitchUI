using System.Collections.Specialized;
using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.VisualTree;

namespace CodexSwitchUI.Controls;

public class CodexButtonGroup : ItemsControl
{
    public static readonly StyledProperty<Orientation> OrientationProperty =
        AvaloniaProperty.Register<CodexButtonGroup, Orientation>(nameof(Orientation), Orientation.Horizontal);

    public static readonly StyledProperty<CodexControlVariant> VariantProperty =
        AvaloniaProperty.Register<CodexButtonGroup, CodexControlVariant>(nameof(Variant), CodexControlVariant.Default);

    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexButtonGroup, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    static CodexButtonGroup()
    {
        OrientationProperty.Changed.AddClassHandler<CodexButtonGroup>((group, _) =>
        {
            group.SyncClasses();
            group.SyncItemStates();
        });
        VariantProperty.Changed.AddClassHandler<CodexButtonGroup>((group, _) =>
        {
            group.SyncClasses();
            group.SyncItemStates();
        });
        SizeProperty.Changed.AddClassHandler<CodexButtonGroup>((group, _) =>
        {
            group.SyncClasses();
            group.SyncItemStates();
        });
    }

    public CodexButtonGroup()
    {
        Focusable = false;
        ItemsView.CollectionChanged += OnItemsViewCollectionChanged;
        AutomationProperties.SetIsControlElementOverride(this, true);
        SyncClasses();
    }

    public Orientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
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

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        return new CodexButtonGroupText();
    }

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        recycleKey = null;
        return item is not Control;
    }

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);

        if (container is CodexButtonGroupText text && item is not Control)
        {
            text.SetCurrentValue(ContentControl.ContentProperty, item);
        }

        SyncItemStates();
    }

    protected override void ClearContainerForItemOverride(Control element)
    {
        ClearItemState(element);
        base.ClearContainerForItemOverride(element);
        SyncItemStates();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        SyncItemStates();
    }

    private void OnItemsViewCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        SyncItemStates();
    }

    internal void SyncItemStates()
    {
        var items = GetGroupControls().ToArray();
        var count = items.Length;

        for (var index = 0; index < count; index++)
        {
            SyncItemState(items[index], index, count);
        }

        Classes.Set("empty", count == 0);
        Classes.Set("has-items", count > 0);
    }

    private void SyncClasses()
    {
        CodexClassSync.SetVariant(Classes, Variant);
        CodexClassSync.SetSize(Classes, Size);
        Classes.Set("button-group", true);
        Classes.Set("horizontal", Orientation == Orientation.Horizontal);
        Classes.Set("vertical", Orientation == Orientation.Vertical);
    }

    private void SyncItemState(Control item, int index, int count)
    {
        var single = count == 1;
        var first = index == 0 && !single;
        var last = index == count - 1 && !single;
        var middle = index > 0 && index < count - 1;

        item.Classes.Set("group-item", true);
        item.Classes.Set("button-group-item", true);
        item.Classes.Set("group-single", single);
        item.Classes.Set("group-first", first);
        item.Classes.Set("group-middle", middle);
        item.Classes.Set("group-last", last);
        item.Classes.Set("horizontal", Orientation == Orientation.Horizontal);
        item.Classes.Set("vertical", Orientation == Orientation.Vertical);

        if (item is CodexButton button)
        {
            if (IsSet(VariantProperty))
            {
                button.SetCurrentValue(CodexButton.VariantProperty, Variant);
            }

            if (IsSet(SizeProperty))
            {
                button.SetCurrentValue(CodexButton.SizeProperty, Size);
            }
        }

        if (item is CodexNativeSelect nativeSelect && IsSet(SizeProperty))
        {
            nativeSelect.SetCurrentValue(CodexNativeSelect.SizeProperty, Size);
        }

        if (item is CodexButtonGroupText text)
        {
            text.SetCurrentValue(CodexButtonGroupText.SizeProperty, Size);
        }

        if (item is CodexButtonGroupSeparator separator)
        {
            separator.SetCurrentValue(CodexSeparator.SizeProperty, Size);
            separator.SetCurrentValue(
                CodexSeparator.OrientationProperty,
                Orientation == Orientation.Horizontal ? Orientation.Vertical : Orientation.Horizontal);
        }
    }

    private IEnumerable<Control> GetGroupControls()
    {
        for (var index = 0; index < ItemsView.Count; index++)
        {
            if (GetGroupControlAt(index) is { } item)
            {
                yield return item;
            }
        }
    }

    private Control? GetGroupControlAt(int index)
    {
        if (index < 0 || index >= ItemsView.Count)
        {
            return null;
        }

        return ItemsView[index] as Control ?? ContainerFromIndex(index) as Control;
    }

    private static void ClearItemState(Control item)
    {
        item.Classes.Set("group-item", false);
        item.Classes.Set("button-group-item", false);
        item.Classes.Set("group-single", false);
        item.Classes.Set("group-first", false);
        item.Classes.Set("group-middle", false);
        item.Classes.Set("group-last", false);
        item.Classes.Set("horizontal", false);
        item.Classes.Set("vertical", false);
    }
}

public class CodexButtonGroupText : ContentControl
{
    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexButtonGroupText, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    static CodexButtonGroupText()
    {
        SizeProperty.Changed.AddClassHandler<CodexButtonGroupText>((text, _) => text.SyncClasses());
    }

    public CodexButtonGroupText()
    {
        IsHitTestVisible = false;
        Focusable = false;
        SyncClasses();
    }

    public CodexControlSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    private void SyncClasses()
    {
        Classes.Set("button-group-text", true);
        CodexClassSync.SetSize(Classes, Size);
    }
}

public class CodexButtonGroupSeparator : CodexSeparator
{
    public CodexButtonGroupSeparator()
    {
        IsHitTestVisible = false;
        Focusable = false;
        Orientation = Orientation.Vertical;
        Classes.Set("button-group-separator", true);
    }
}
