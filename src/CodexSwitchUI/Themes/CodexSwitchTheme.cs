using Avalonia.Markup.Xaml.Styling;
using Avalonia.Styling;
using Avalonia.Themes.Fluent;

namespace CodexSwitchUI.Themes;

/// <summary>
/// App style entry point. Add this to <c>Application.Styles</c> or XAML styles.
/// </summary>
public sealed class CodexSwitchTheme : Styles
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

        Add(new StyleInclude(new Uri("avares://CodexSwitchUI"))
        {
            Source = new Uri("avares://CodexSwitchUI/Themes/CodexSwitchTheme.axaml")
        });
    }
}
