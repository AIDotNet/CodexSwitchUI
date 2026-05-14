using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml.Styling;
using CodexSwitchUI.Themes;

namespace CodexSwitchUI.Docs;

public sealed class App : Application
{
    public override void Initialize()
    {
        Styles.Add(new CodexSwitchTheme());
        Styles.Add(new StyleInclude(new Uri("avares://CodexSwitchUI.Docs/App"))
        {
            Source = new Uri("avares://CodexSwitchUI.ECharts/Themes/UsageTrendChart.axaml")
        });
        CodexSwitchThemeManager.Current.Apply(this, CodexSwitchThemeMode.Light);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }
}
