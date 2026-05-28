using Avalonia.Controls;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;

public static class AvatarInteractionSample
{
    public static Control BuildAvatarInteractionPreview()
    {
        var status = new CodexText
        {
            Role = CodexTextRole.Muted,
            Text = "Waiting for avatar image loading status."
        };
        var avatar = new CodexAvatar
        {
            Fallback = "AI",
            Variant = CodexControlVariant.Secondary,
            StatusVariant = CodexControlVariant.Success,
            IsStatusVisible = true
        };
        avatar.LoadingStatusChanged += (_, args) =>
        {
            status.Text = $"LoadingStatusChanged: {args.OldStatus} -> {args.NewStatus} ({IconFileName(args.ImagePath)}); error={args.ErrorMessage ?? "none"}.";
        };
        avatar.ImagePath = IconPath("openai.png");

        var presenceStep = 0;
        var rotatePresence = new CodexButton
        {
            Content = "Cycle image",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Secondary
        };
        rotatePresence.Click += (_, _) =>
        {
            presenceStep = (presenceStep + 1) % 3;
            var (fallback, path, statusVariant, variant) = presenceStep switch
            {
                1 => ("CL", IconPath("claude.png"), CodexControlVariant.Warning, CodexControlVariant.Outline),
                2 => ("ER", IconPath("missing-avatar.png"), CodexControlVariant.Destructive, CodexControlVariant.Destructive),
                _ => ("AI", IconPath("openai.png"), CodexControlVariant.Success, CodexControlVariant.Secondary)
            };

            avatar.Fallback = fallback;
            avatar.StatusVariant = statusVariant;
            avatar.Variant = variant;
            avatar.ImagePath = path;
        };

        var fallbackStatus = new CodexText
        {
            Role = CodexTextRole.Muted,
            Text = "Fallback is delayed while the avatar is loading."
        };
        var fallbackAvatar = new CodexAvatar
        {
            Fallback = "DL",
            Size = CodexControlSize.Large,
            IsStatusVisible = true,
            FallbackDelay = TimeSpan.FromMilliseconds(600),
            LoadingStatus = CodexAvatarLoadingStatus.Loading
        };
        var swapFallback = new CodexButton
        {
            Content = "Resolve fallback",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Secondary
        };
        swapFallback.Click += (_, _) =>
        {
            if (fallbackAvatar.LoadingStatus == CodexAvatarLoadingStatus.Loading)
            {
                fallbackAvatar.FallbackDelay = TimeSpan.Zero;
                fallbackAvatar.LoadingStatus = CodexAvatarLoadingStatus.Error;
                fallbackStatus.Text = "Fallback visible after avatar load error.";
            }
            else
            {
                fallbackAvatar.FallbackDelay = TimeSpan.FromMilliseconds(600);
                fallbackAvatar.LoadingStatus = CodexAvatarLoadingStatus.Loading;
                fallbackStatus.Text = "Fallback is delayed while the avatar is loading.";
            }
        };

        var sizeAvatar = new CodexAvatar
        {
            Fallback = "LG",
            Size = CodexControlSize.Large,
            Variant = CodexControlVariant.Default
        };
        var toggleSize = new CodexButton
        {
            Content = "Toggle size",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Secondary
        };
        toggleSize.Click += (_, _) =>
        {
            sizeAvatar.Size = sizeAvatar.Size == CodexControlSize.Small
                ? CodexControlSize.Large
                : sizeAvatar.Size == CodexControlSize.Large
                    ? CodexControlSize.Medium
                    : CodexControlSize.Small;
            sizeAvatar.Fallback = sizeAvatar.Size == CodexControlSize.Small ? "SM" : sizeAvatar.Size == CodexControlSize.Large ? "LG" : "MD";
            status.Text = $"Avatar size changed to {sizeAvatar.Size}.";
        };

        return new StackPanel
        {
            Spacing = 10,
            Children =
            {
                status,
                avatar,
                rotatePresence,
                fallbackStatus,
                fallbackAvatar,
                swapFallback,
                sizeAvatar,
                toggleSize,
                new CodexButton
                {
                    Content = "Assign owner",
                    IsEnabled = false,
                    LeadingIcon = new CodexAvatar
                    {
                        Fallback = "OP",
                        Size = CodexControlSize.Small,
                        IsStatusVisible = true
                    }
                }
            }
        };
    }

    private static string IconPath(string fileName)
    {
        return $"avares://CodexSwitchUI.Docs/Assets/icons/{fileName}";
    }

    private static string IconFileName(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return "none";
        }

        var slash = path.LastIndexOf('/');
        return slash >= 0 ? path[(slash + 1)..] : path;
    }
}
