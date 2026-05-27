using Avalonia.Input;

namespace CodexSwitchUI.Controls;

internal static class CodexFocusVisible
{
    public const string PseudoClass = ":focus-visible";

    public static bool FromFocusChange(FocusChangedEventArgs args)
    {
        return args.NavigationMethod != NavigationMethod.Pointer;
    }
}
