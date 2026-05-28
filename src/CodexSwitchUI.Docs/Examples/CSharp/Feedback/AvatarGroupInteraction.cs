using Avalonia.Controls;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;

public static class AvatarGroupInteractionSample
{
    public static Control BuildAvatarGroupInteractionPreview()
    {
        var status = new CodexText
        {
            Role = CodexTextRole.Muted,
            Text = "Stacked group: overlap=12; visible members=4."
        };
        var optionalMember = new CodexAvatar
        {
            Fallback = "QA",
            Variant = CodexControlVariant.Warning,
            StatusVariant = CodexControlVariant.Warning,
            IsStatusVisible = true
        };
        var group = new CodexAvatarGroup
        {
            Size = CodexControlSize.Medium,
            Overlap = 12,
            Children =
            {
                new CodexAvatar { Fallback = "CN", ImagePath = IconPath("openai.png"), IsStatusVisible = true },
                new CodexAvatar { Fallback = "LR", ImagePath = IconPath("claude.png"), Variant = CodexControlVariant.Secondary },
                optionalMember,
                new CodexAvatarGroupCount { Count = 5 }
            }
        };

        var toggleStacking = new CodexButton
        {
            Content = "Toggle stacking",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Secondary
        };
        toggleStacking.Click += (_, _) =>
        {
            group.IsStacked = !group.IsStacked;
            status.Text = group.IsStacked
                ? $"Stacked group: overlap={group.Overlap:0}; visible members={VisibleAvatarGroupMembers(group)}; item-count={group.ItemCount}."
                : $"Inline group: full-width members={VisibleAvatarGroupMembers(group)}; item-count={group.ItemCount}.";
        };

        var changeOverlap = new CodexButton
        {
            Content = "Cycle overlap",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Secondary
        };
        changeOverlap.Click += (_, _) =>
        {
            group.Overlap = group.Overlap >= 16 ? 6 : group.Overlap + 5;
            status.Text = $"Overlap changed to {group.Overlap:0}; visible members={VisibleAvatarGroupMembers(group)}.";
        };

        var toggleMember = new CodexButton
        {
            Content = "Toggle member",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Secondary
        };
        toggleMember.Click += (_, _) =>
        {
            optionalMember.IsVisible = !optionalMember.IsVisible;
            group.InvalidateMeasure();
            status.Text = $"Optional member {(optionalMember.IsVisible ? "shown" : "hidden")}; visible members={VisibleAvatarGroupMembers(group)}.";
        };

        return new StackPanel
        {
            Spacing = 12,
            Children =
            {
                status,
                group,
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 8,
                    Children =
                    {
                        toggleStacking,
                        changeOverlap,
                        toggleMember
                    }
                },
                new CodexButton
                {
                    Content = "Assign reviewers",
                    IsEnabled = false,
                    LeadingIcon = BuildAvatarGroupStack(CodexControlSize.Small, overlap: 8, isStacked: true, includeCount: true)
                }
            }
        };
    }

    private static CodexAvatarGroup BuildAvatarGroupStack(CodexControlSize size, double overlap, bool isStacked, bool includeCount)
    {
        var group = new CodexAvatarGroup
        {
            Size = size,
            Overlap = overlap,
            IsStacked = isStacked,
            Children =
            {
                new CodexAvatar { Fallback = "CN", ImagePath = IconPath("openai.png"), IsStatusVisible = true },
                new CodexAvatar { Fallback = "LR", ImagePath = IconPath("claude.png"), Variant = CodexControlVariant.Secondary },
                new CodexAvatar { Fallback = "ER", Variant = CodexControlVariant.Outline }
            }
        };

        if (includeCount)
        {
            group.Children.Add(new CodexAvatarGroupCount { Count = 3 });
        }

        return group;
    }

    private static int VisibleAvatarGroupMembers(CodexAvatarGroup group)
    {
        var count = 0;
        foreach (var child in group.Children)
        {
            if (child.IsVisible)
            {
                count++;
            }
        }

        return count;
    }

    private static string IconPath(string fileName)
    {
        return $"avares://CodexSwitchUI.Docs/Assets/icons/{fileName}";
    }
}
