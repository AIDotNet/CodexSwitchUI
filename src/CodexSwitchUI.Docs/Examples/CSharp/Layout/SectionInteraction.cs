using Avalonia.Controls;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;

public static class SectionInteractionSample
{
    public static Control BuildSectionComponentInteractionPreview()
    {
        var status = new CodexText
        {
            Role = CodexTextRole.Muted,
            Text = "Refresh the section or toggle its slots."
        };
        var version = 0;
        var section = new CodexSection
        {
            Title = "Provider routing",
            Description = "Action clicks update title, description, and body while layout stays stable.",
            Content = new CodexProgress { Value = 66, Variant = CodexControlVariant.Success }
        };

        var refresh = new CodexButton
        {
            Content = "Refresh",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Secondary
        };
        refresh.Click += (_, _) =>
        {
            version++;
            section.Title = version % 2 == 0 ? "Provider routing" : "Provider routing refreshed";
            section.Description = "Section header and body updated from one action event.";
            section.Content = new CodexCard
            {
                Title = $"Snapshot {version}",
                Description = "The content slot stayed inside the same layout band.",
                Content = new CodexProgress { Value = version % 2 == 0 ? 66 : 82, Variant = CodexControlVariant.Success }
            };
            status.Text = $"Section refreshed {version} time(s).";
        };

        var toggleActions = new CodexButton
        {
            Content = "Toggle actions",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Ghost
        };
        toggleActions.Click += (_, _) =>
        {
            section.Actions = section.Actions is null ? refresh : null;
            status.Text = section.Actions is null ? "Action slot hidden." : "Action slot restored.";
        };
        section.Actions = refresh;

        var emptySection = new CodexSection
        {
            Title = "Empty body",
            Description = "Header and action spacing remain stable with no content.",
            Actions = new CodexButton { Content = "Add", Size = CodexControlSize.Small }
        };
        var toggleEmpty = new CodexButton
        {
            Content = "Toggle body",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Secondary
        };
        toggleEmpty.Click += (_, _) =>
        {
            emptySection.Content = emptySection.Content is null
                ? new CodexBadge { Content = "Body restored", Variant = CodexControlVariant.Secondary }
                : null;
            status.Text = emptySection.Content is null ? "Empty section body cleared." : "Empty section body restored.";
        };

        return new StackPanel
        {
            Spacing = 10,
            Children =
            {
                status,
                section,
                toggleActions,
                emptySection,
                toggleEmpty
            }
        };
    }
}
