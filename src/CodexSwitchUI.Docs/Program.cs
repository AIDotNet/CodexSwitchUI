using Avalonia;
using Avalonia.Media;
using CodexSwitchUI.Tokens;

namespace CodexSwitchUI.Docs;

internal static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .With(new FontManagerOptions
            {
                DefaultFamilyName = CodexSwitchFonts.DefaultFontFamily,
                FontFallbacks =
                [
                    new FontFallback { FontFamily = new FontFamily(CodexSwitchFonts.DefaultFontFamily) }
                ]
            })
            .LogToTrace();
    }
}
