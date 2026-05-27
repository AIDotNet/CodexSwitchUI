using System.Collections.Specialized;
using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace CodexSwitchUI.Controls;

public class CodexInputGroup : ItemsControl
{
    public static readonly StyledProperty<CodexControlIntent> IntentProperty =
        AvaloniaProperty.Register<CodexInputGroup, CodexControlIntent>(nameof(Intent), CodexControlIntent.Default);

    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexInputGroup, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    static CodexInputGroup()
    {
        IntentProperty.Changed.AddClassHandler<CodexInputGroup>((group, _) =>
        {
            group.SyncClasses();
            group.SyncItemStates();
        });
        SizeProperty.Changed.AddClassHandler<CodexInputGroup>((group, _) =>
        {
            group.SyncClasses();
            group.SyncItemStates();
        });
    }

    public CodexInputGroup()
    {
        Focusable = false;
        ItemsView.CollectionChanged += OnItemsViewCollectionChanged;
        AddHandler(InputElement.GotFocusEvent, OnDescendantGotFocus, RoutingStrategies.Bubble, handledEventsToo: true);
        AddHandler(InputElement.LostFocusEvent, OnDescendantLostFocus, RoutingStrategies.Bubble, handledEventsToo: true);
        AutomationProperties.SetIsControlElementOverride(this, true);
        SyncClasses();
    }

    public CodexControlIntent Intent
    {
        get => GetValue(IntentProperty);
        set => SetValue(IntentProperty, value);
    }

    public CodexControlSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        return new CodexInputGroupText();
    }

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        recycleKey = null;
        return item is not Control;
    }

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);

        if (container is CodexInputGroupText text && item is not Control)
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

    private void OnDescendantGotFocus(object? sender, FocusChangedEventArgs e)
    {
        Classes.Set("has-focus-within", true);
    }

    private void OnDescendantLostFocus(object? sender, FocusChangedEventArgs e)
    {
        Dispatcher.UIThread.Post(() =>
        {
            if (this.IsAttachedToVisualTree())
            {
                Classes.Set("has-focus-within", false);
            }
        });
    }

    internal void SyncItemStates()
    {
        var items = GetGroupControls().ToArray();
        var count = items.Length;
        var hasBlockAddon = false;

        for (var index = 0; index < count; index++)
        {
            SyncItemState(items[index], index, count);
            hasBlockAddon |= items[index] is CodexInputGroupAddon
            {
                Align: CodexInputGroupAddonAlign.BlockStart or CodexInputGroupAddonAlign.BlockEnd
            };
        }

        Classes.Set("empty", count == 0);
        Classes.Set("has-items", count > 0);
        Classes.Set("inline", !hasBlockAddon);
        Classes.Set("block", hasBlockAddon);
    }

    private void SyncClasses()
    {
        CodexClassSync.SetIntent(Classes, Intent);
        CodexClassSync.SetSize(Classes, Size);
        Classes.Set("input-group", true);
    }

    private void SyncItemState(Control item, int index, int count)
    {
        var single = count == 1;
        var first = index == 0 && !single;
        var last = index == count - 1 && !single;
        var middle = index > 0 && index < count - 1;

        item.Classes.Set("group-item", true);
        item.Classes.Set("input-group-item", true);
        item.Classes.Set("group-single", single);
        item.Classes.Set("group-first", first);
        item.Classes.Set("group-middle", middle);
        item.Classes.Set("group-last", last);

        if (item is CodexTextBox textBox)
        {
            textBox.Classes.Set("input-group-control", true);

            if (IsSet(IntentProperty))
            {
                textBox.SetCurrentValue(CodexTextBox.IntentProperty, Intent);
            }

            if (IsSet(SizeProperty))
            {
                textBox.SetCurrentValue(CodexTextBox.SizeProperty, Size);
            }
        }

        if (item is CodexSelect select)
        {
            select.Classes.Set("input-group-control", true);

            if (IsSet(IntentProperty))
            {
                select.SetCurrentValue(CodexSelect.IntentProperty, Intent);
            }

            if (IsSet(SizeProperty))
            {
                select.SetCurrentValue(CodexSelect.SizeProperty, Size);
            }
        }

        if (item is CodexNativeSelect nativeSelect)
        {
            nativeSelect.Classes.Set("input-group-control", true);

            if (IsSet(IntentProperty))
            {
                nativeSelect.SetCurrentValue(CodexNativeSelect.IntentProperty, Intent);
            }

            if (IsSet(SizeProperty))
            {
                nativeSelect.SetCurrentValue(CodexNativeSelect.SizeProperty, Size);
            }
        }

        if (item is CodexButton button)
        {
            button.Classes.Set("input-group-action", true);

            if (IsSet(SizeProperty))
            {
                button.SetCurrentValue(CodexButton.SizeProperty, Size == CodexControlSize.Large ? CodexControlSize.Medium : CodexControlSize.Small);
            }
        }

        if (item is CodexInputGroupAddon addon)
        {
            addon.SetCurrentValue(CodexInputGroupAddon.SizeProperty, Size);
        }

        if (item is CodexInputGroupText text)
        {
            text.SetCurrentValue(CodexInputGroupText.SizeProperty, Size);
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
        item.Classes.Set("input-group-item", false);
        item.Classes.Set("group-single", false);
        item.Classes.Set("group-first", false);
        item.Classes.Set("group-middle", false);
        item.Classes.Set("group-last", false);
        item.Classes.Set("input-group-control", false);
        item.Classes.Set("input-group-action", false);
    }
}

public enum CodexInputGroupAddonAlign
{
    InlineStart,
    InlineEnd,
    BlockStart,
    BlockEnd
}

public class CodexInputGroupAddon : ContentControl
{
    public static readonly StyledProperty<CodexInputGroupAddonAlign> AlignProperty =
        AvaloniaProperty.Register<CodexInputGroupAddon, CodexInputGroupAddonAlign>(nameof(Align), CodexInputGroupAddonAlign.InlineStart);

    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexInputGroupAddon, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    static CodexInputGroupAddon()
    {
        AlignProperty.Changed.AddClassHandler<CodexInputGroupAddon>((addon, _) =>
        {
            addon.SyncClasses();
            addon.FindAncestorOfType<CodexInputGroup>()?.SyncItemStates();
        });
        SizeProperty.Changed.AddClassHandler<CodexInputGroupAddon>((addon, _) => addon.SyncClasses());
    }

    public CodexInputGroupAddon()
    {
        Focusable = false;
        SyncClasses();
    }

    public CodexInputGroupAddonAlign Align
    {
        get => GetValue(AlignProperty);
        set => SetValue(AlignProperty, value);
    }

    public CodexControlSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    private void SyncClasses()
    {
        Classes.Set("input-group-addon", true);
        Classes.Set("align-inline-start", Align == CodexInputGroupAddonAlign.InlineStart);
        Classes.Set("align-inline-end", Align == CodexInputGroupAddonAlign.InlineEnd);
        Classes.Set("align-block-start", Align == CodexInputGroupAddonAlign.BlockStart);
        Classes.Set("align-block-end", Align == CodexInputGroupAddonAlign.BlockEnd);
        CodexClassSync.SetSize(Classes, Size);
    }
}

public class CodexInputGroupText : ContentControl
{
    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexInputGroupText, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    static CodexInputGroupText()
    {
        SizeProperty.Changed.AddClassHandler<CodexInputGroupText>((text, _) => text.SyncClasses());
    }

    public CodexInputGroupText()
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
        Classes.Set("input-group-text", true);
        CodexClassSync.SetSize(Classes, Size);
    }
}

public class CodexInputGroupButton : CodexButton
{
    public CodexInputGroupButton()
    {
        Variant = CodexControlVariant.Ghost;
        Size = CodexControlSize.Small;
        Classes.Set("input-group-button", true);
    }
}

public class CodexInputGroupInput : CodexTextBox
{
    public CodexInputGroupInput()
    {
        Classes.Set("input-group-control", true);
    }
}

public class CodexInputGroupTextarea : CodexTextarea
{
    public CodexInputGroupTextarea()
    {
        Classes.Set("input-group-control", true);
    }
}
