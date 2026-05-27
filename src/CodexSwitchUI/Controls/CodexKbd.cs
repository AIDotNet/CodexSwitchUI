using Avalonia;
using Avalonia.Controls;
using System.Collections.Specialized;

namespace CodexSwitchUI.Controls;

public class CodexKbdGroup : ItemsControl
{
    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexKbdGroup, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    static CodexKbdGroup()
    {
        SizeProperty.Changed.AddClassHandler<CodexKbdGroup>((group, _) => group.SyncClasses());
    }

    public CodexKbdGroup()
    {
        Focusable = false;
        ItemsView.CollectionChanged += OnItemsViewCollectionChanged;
        SyncClasses();
    }

    public CodexControlSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);
        SyncClasses();
    }

    protected override void ClearContainerForItemOverride(Control element)
    {
        base.ClearContainerForItemOverride(element);
        SyncClasses();
    }

    private void OnItemsViewCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        SyncClasses();
    }

    private void SyncClasses()
    {
        Classes.Set("kbd-group", true);
        CodexClassSync.SetSize(Classes, Size);
        Classes.Set("has-items", ItemsView.Count > 0);
        Classes.Set("empty", ItemsView.Count == 0);
    }
}

public class CodexKbd : ContentControl
{
    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexKbd, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    static CodexKbd()
    {
        SizeProperty.Changed.AddClassHandler<CodexKbd>((kbd, _) => kbd.SyncClasses());
    }

    public CodexKbd()
    {
        SyncClasses();
    }

    public CodexControlSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    private void SyncClasses()
    {
        CodexClassSync.SetSize(Classes, Size);
    }
}
