using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace CodexSwitchUI.Controls;

public class CodexCommandDialog : CodexDialog
{
    public static readonly StyledProperty<string?> PlaceholderProperty =
        AvaloniaProperty.Register<CodexCommandDialog, string?>(nameof(Placeholder), "Type a command...");

    public static readonly StyledProperty<string?> SearchTextProperty =
        AvaloniaProperty.Register<CodexCommandDialog, string?>(nameof(SearchText));

    public static readonly StyledProperty<bool> ShouldFilterProperty =
        AvaloniaProperty.Register<CodexCommandDialog, bool>(nameof(ShouldFilter), true);

    public static readonly StyledProperty<bool> LoopNavigationProperty =
        AvaloniaProperty.Register<CodexCommandDialog, bool>(nameof(LoopNavigation));

    public static readonly StyledProperty<bool> IsLoadingProperty =
        AvaloniaProperty.Register<CodexCommandDialog, bool>(nameof(IsLoading));

    public static readonly StyledProperty<bool> CloseOnItemSelectedProperty =
        AvaloniaProperty.Register<CodexCommandDialog, bool>(nameof(CloseOnItemSelected), true);

    static CodexCommandDialog()
    {
        LoopNavigationProperty.Changed.AddClassHandler<CodexCommandDialog>((dialog, _) => dialog.SyncCommandClasses());
        IsLoadingProperty.Changed.AddClassHandler<CodexCommandDialog>((dialog, _) => dialog.SyncCommandClasses());
        CloseOnItemSelectedProperty.Changed.AddClassHandler<CodexCommandDialog>((dialog, _) => dialog.SyncCommandClasses());
    }

    public CodexCommandDialog()
    {
        IsCloseVisible = false;
        AddHandler(Button.ClickEvent, OnCommandItemClicked, RoutingStrategies.Bubble);
        SyncCommandClasses();
    }

    public event EventHandler<CodexCommandItemSelectedEventArgs>? ItemSelected;

    public string? Placeholder
    {
        get => GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }

    public string? SearchText
    {
        get => GetValue(SearchTextProperty);
        set => SetValue(SearchTextProperty, value);
    }

    public bool ShouldFilter
    {
        get => GetValue(ShouldFilterProperty);
        set => SetValue(ShouldFilterProperty, value);
    }

    public bool LoopNavigation
    {
        get => GetValue(LoopNavigationProperty);
        set => SetValue(LoopNavigationProperty, value);
    }

    public bool IsLoading
    {
        get => GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }

    public bool CloseOnItemSelected
    {
        get => GetValue(CloseOnItemSelectedProperty);
        set => SetValue(CloseOnItemSelectedProperty, value);
    }

    internal bool TryCloseFromCommandItem(CodexCommandItem item)
    {
        if (!CloseOnItemSelected || IsLoading || !IsOpen || !item.IsEnabled)
        {
            return false;
        }

        return Dismiss();
    }

    private void OnCommandItemClicked(object? sender, RoutedEventArgs e)
    {
        if (e.Source is CodexCommandItem item)
        {
            ItemSelected?.Invoke(this, new CodexCommandItemSelectedEventArgs(item, item.ResolveValue()));
            TryCloseFromCommandItem(item);
        }
    }

    private void SyncCommandClasses()
    {
        Classes.Set("loading", IsLoading);
        Classes.Set("close-on-select", CloseOnItemSelected);
        Classes.Set("loop", LoopNavigation);
    }
}
