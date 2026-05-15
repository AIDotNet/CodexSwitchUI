using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Avalonia.Themes.Fluent;

namespace CodexSwitchUI.Themes;

/// <summary>
/// App style entry point. Add this to <c>Application.Styles</c> or XAML styles.
/// </summary>
public sealed partial class CodexSwitchTheme : Styles
{
    public CodexSwitchTheme()
        : this(includeFluentBaseTheme: true)
    {
    }

    public CodexSwitchTheme(bool includeFluentBaseTheme)
    {
        if (includeFluentBaseTheme)
        {
            Add(new FluentTheme());
        }

        AvaloniaXamlLoader.Load(this);
    }
}
