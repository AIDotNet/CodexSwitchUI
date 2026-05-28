using Avalonia.Controls;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;

public static class ButtonGroupInteractionSample
{
    public static Control BuildButtonGroupInteractionPreview()
    {
        var status = new CodexText
        {
            Role = CodexTextRole.Muted,
            Text = "Each child keeps its own activation path inside the group."
        };
        var archiveCount = 0;

        var archive = new CodexButton { Content = "Archive", Variant = CodexControlVariant.Outline };
        archive.Click += (_, _) =>
        {
            archiveCount++;
            status.Text = $"Archive clicked {archiveCount} time(s).";
        };

        var snooze = new CodexButton { Content = "Snooze", Variant = CodexControlVariant.Outline };
        snooze.Click += (_, _) => status.Text = "Snooze clicked.";

        var loading = new CodexButton
        {
            Content = "Sync",
            Variant = CodexControlVariant.Secondary,
            IsLoading = true,
            LoadingContent = "Syncing"
        };
        loading.Click += (_, _) => status.Text = "Loading child should suppress activation.";

        var toggleLoading = new CodexButton
        {
            Content = "Toggle sync",
            Variant = CodexControlVariant.Ghost,
            Size = CodexControlSize.Small
        };
        toggleLoading.Click += (_, _) =>
        {
            loading.IsLoading = !loading.IsLoading;
            status.Text = loading.IsLoading ? "Sync is locked." : "Sync is available.";
        };

        return new StackPanel
        {
            Spacing = 10,
            Children =
            {
                status,
                new CodexButtonGroup
                {
                    Items =
                    {
                        archive,
                        snooze,
                        new CodexButtonGroupSeparator(),
                        loading
                    }
                },
                toggleLoading,
                new CodexButtonGroup
                {
                    Variant = CodexControlVariant.Outline,
                    Items =
                    {
                        new CodexButton { Content = "Preview" },
                        new CodexButton { Content = "Code" },
                        new CodexButton { Content = "Events" }
                    }
                },
                new CodexButtonGroup
                {
                    Items =
                    {
                        new CodexButtonGroup
                        {
                            Variant = CodexControlVariant.Outline,
                            Items =
                            {
                                new CodexButton { Content = "List" },
                                new CodexButton { Content = "Grid" }
                            }
                        },
                        new CodexIconButton { Content = "+", Variant = CodexControlVariant.Secondary },
                        new CodexButton { Content = "Disabled", IsEnabled = false }
                    }
                }
            }
        };
    }
}
