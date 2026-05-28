using Avalonia.Controls;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;

public static class InputGroupInteractionSample
{
    public static Control BuildInputGroupInteractionPreview()
    {
        var status = new CodexText
        {
            Role = CodexTextRole.Muted,
            Text = "Focus-within lives on the group while editing stays on the child input."
        };
        var searchCount = 0;
        var input = new CodexInputGroupInput
        {
            Text = "provider",
            SelectionStart = 0,
            SelectionEnd = 8
        };
        var search = new CodexInputGroupButton { Content = "Search" };
        search.Click += (_, _) =>
        {
            searchCount++;
            status.Text = $"Search {searchCount}: query={input.Text}.";
        };

        var loading = new CodexInputGroupButton
        {
            Content = "Save",
            IsLoading = true,
            LoadingContent = "Saving"
        };
        loading.Click += (_, _) => status.Text = "Loading add-on button should suppress activation.";

        return new StackPanel
        {
            Spacing = 10,
            Children =
            {
                status,
                new CodexInputGroup
                {
                    Items =
                    {
                        new CodexInputGroupAddon { Content = "Search" },
                        input,
                        new CodexInputGroupAddon
                        {
                            Align = CodexInputGroupAddonAlign.InlineEnd,
                            Content = search
                        }
                    }
                },
                new CodexInputGroup
                {
                    Items =
                    {
                        new CodexInputGroupInput { Text = "Saving changes..." },
                        new CodexInputGroupAddon
                        {
                            Align = CodexInputGroupAddonAlign.InlineEnd,
                            Content = loading
                        }
                    }
                },
                new CodexInputGroup
                {
                    Items =
                    {
                        new CodexInputGroupAddon { Content = "Route" },
                        new CodexInputGroupInput { Text = "/v1/responses", IsReadOnly = true },
                        new CodexInputGroupAddon
                        {
                            Align = CodexInputGroupAddonAlign.InlineEnd,
                            Content = new CodexInputGroupButton { Content = "Copy" }
                        }
                    }
                },
                new CodexInputGroup
                {
                    Items =
                    {
                        new CodexInputGroupInput { PlaceholderText = "Type to search" },
                        new CodexInputGroupAddon
                        {
                            Align = CodexInputGroupAddonAlign.InlineEnd,
                            Content = new CodexInputGroupButton { Content = "Search", IsEnabled = false }
                        }
                    }
                }
            }
        };
    }
}
