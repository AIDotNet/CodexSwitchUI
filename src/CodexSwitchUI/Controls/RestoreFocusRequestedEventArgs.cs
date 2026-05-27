using Avalonia.Input;

namespace CodexSwitchUI.Controls;

public sealed class RestoreFocusRequestedEventArgs(
    IInputElement target,
    NavigationMethod navigationMethod,
    KeyModifiers keyModifiers) : EventArgs
{
    public IInputElement Target { get; } = target;

    public NavigationMethod NavigationMethod { get; } = navigationMethod;

    public KeyModifiers KeyModifiers { get; } = keyModifiers;
}

internal static class CodexFocusRestore
{
    internal static bool TryRestore(
        IInputElement? target,
        EventHandler<RestoreFocusRequestedEventArgs>? requested,
        object sender,
        NavigationMethod navigationMethod = NavigationMethod.Tab,
        KeyModifiers keyModifiers = KeyModifiers.None)
    {
        if (target is null || !target.Focusable || !target.IsEffectivelyEnabled || !target.IsEffectivelyVisible)
        {
            return false;
        }

        requested?.Invoke(sender, new RestoreFocusRequestedEventArgs(target, navigationMethod, keyModifiers));

        try
        {
            target.Focus(navigationMethod, keyModifiers);
            return true;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }
}
