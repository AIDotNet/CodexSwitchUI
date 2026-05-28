using Avalonia.Controls;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;

public static class ImageIconInteractionSample
{
    private const string DocsIconBase = "avares://CodexSwitchUI.Docs/Assets/icons/";

    public static Control BuildImageIconInteractionPreview()
    {
        var status = Muted("Waiting for provider image lifecycle event.");
        var icon = new CodexImageIcon
        {
            Width = 48,
            Height = 48
        };
        icon.ImageLoaded += (_, args) =>
        {
            status.Text = $"ImageLoaded: {IconFileName(args.Path)} loaded; old path was {IconFileName(args.OldPath)}.";
        };
        icon.ImageLoadFailed += (_, args) =>
        {
            status.Text = $"ImageLoadFailed: {IconFileName(args.Path)} - {args.ErrorMessage}";
        };
        icon.Path = IconPath("openai.png");

        var step = 0;
        var switchProvider = Button("Switch provider");
        switchProvider.Click += (_, _) =>
        {
            step = (step + 1) % 4;
            var (label, file) = step switch
            {
                1 => ("Claude", "claude.png"),
                2 => ("Gemini", "gemini.png"),
                3 => ("Codex", "codex-color.png"),
                _ => ("OpenAI", "openai.png")
            };
            icon.Path = IconPath(file);
            status.Text = $"{label}: {status.Text}";
        };

        var missingStatus = Muted("Waiting for missing asset event.");
        var missing = new CodexImageIcon { Width = 48, Height = 48 };
        missing.ImageLoadFailed += (_, args) =>
        {
            missingStatus.Text = $"ImageLoadFailed: {IconFileName(args.Path)} - {args.ErrorMessage}";
        };
        missing.Path = IconPath("missing-provider.png");

        var sizeStatus = Muted("Size: 32px; HasSource=True.");
        var sized = new CodexImageIcon
        {
            Path = IconPath("claude.png"),
            Width = 32,
            Height = 32
        };
        var toggleSize = Button("Toggle size");
        toggleSize.Click += (_, _) =>
        {
            var next = Math.Abs(sized.Width - 32) < 0.1 ? 48 : Math.Abs(sized.Width - 48) < 0.1 ? 24 : 32;
            sized.Width = next;
            sized.Height = next;
            sizeStatus.Text = $"Size: {next}px; HasSource={sized.HasSource}.";
        };

        return new StackPanel
        {
            Spacing = 10,
            Children =
            {
                status,
                icon,
                switchProvider,
                missingStatus,
                missing,
                sizeStatus,
                sized,
                toggleSize,
                new CodexButton
                {
                    Content = "Open provider",
                    LeadingIcon = new CodexImageIcon { Path = IconPath("codex-color.png"), Width = 24, Height = 24 },
                    IsEnabled = false
                }
            }
        };
    }

    private static string IconPath(string iconName) => DocsIconBase + iconName;

    private static string IconFileName(string? iconPath)
    {
        return string.IsNullOrWhiteSpace(iconPath) ? "none" : iconPath[(iconPath.LastIndexOf('/') + 1)..];
    }

    private static CodexButton Button(string label)
    {
        return new CodexButton { Content = label, Size = CodexControlSize.Small, Variant = CodexControlVariant.Secondary };
    }

    private static CodexText Muted(string text)
    {
        return new CodexText { Role = CodexTextRole.Muted, Text = text };
    }
}
