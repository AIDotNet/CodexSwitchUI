using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using CodexSwitchUI.Themes;

namespace CodexSwitchUI.Docs;

public sealed class App : Application
{
    public override void Initialize()
    {
        Styles.Add(new CodexSwitchTheme());
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
