using Avalonia.Controls;

namespace CodexSwitchUI.Controls;

internal static class CodexClassSync
{
    public static void SetVariant(Classes classes, CodexControlVariant variant)
    {
        classes.Set("variant-default", variant == CodexControlVariant.Default);
        classes.Set("variant-secondary", variant == CodexControlVariant.Secondary);
        classes.Set("variant-destructive", variant == CodexControlVariant.Destructive);
        classes.Set("variant-outline", variant == CodexControlVariant.Outline);
        classes.Set("variant-ghost", variant == CodexControlVariant.Ghost);
        classes.Set("variant-link", variant == CodexControlVariant.Link);
        classes.Set("variant-success", variant == CodexControlVariant.Success);
        classes.Set("variant-warning", variant == CodexControlVariant.Warning);
    }

    public static void SetIntent(Classes classes, CodexControlIntent intent)
    {
        classes.Set("intent-default", intent == CodexControlIntent.Default);
        classes.Set("intent-error", intent == CodexControlIntent.Error);
        classes.Set("intent-success", intent == CodexControlIntent.Success);
        classes.Set("intent-warning", intent == CodexControlIntent.Warning);
    }

    public static void SetSize(Classes classes, CodexControlSize size)
    {
        classes.Set("size-sm", size == CodexControlSize.Small);
        classes.Set("size-md", size == CodexControlSize.Medium);
        classes.Set("size-lg", size == CodexControlSize.Large);
        classes.Set("size-icon", size == CodexControlSize.Icon);
    }
}
