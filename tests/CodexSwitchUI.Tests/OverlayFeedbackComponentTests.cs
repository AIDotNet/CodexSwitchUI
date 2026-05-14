using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Layout;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;
using Xunit;

namespace CodexSwitchUI.Tests;

public class OverlayFeedbackComponentTests
{
    [Fact]
    public void DialogPopoverAndToastExposeSlotState()
    {
        var dialog = new CodexDialog
        {
            Title = "Dialog title",
            Description = "Dialog description",
            Content = "Dialog content",
            Action = "Dialog action",
            CloseContent = "Close"
        };
        var popover = new CodexPopover
        {
            Title = "Popover title",
            Description = "Popover description",
            Content = "Popover content",
            Action = "Popover action",
            IsCloseVisible = false
        };
        var toast = new CodexToast
        {
            Title = "Toast title",
            Description = "Toast description",
            Content = "Toast content",
            Action = "Toast action",
            Icon = "Info",
            CloseContent = "Dismiss"
        };

        Assert.True(dialog.HasHeader);
        Assert.True(dialog.HasContent);
        Assert.True(dialog.HasAction);
        Assert.True(dialog.IsCloseVisible);
        Assert.Contains("has-close-content", dialog.Classes);
        Assert.True(popover.HasHeader);
        Assert.True(popover.HasContent);
        Assert.True(popover.HasAction);
        Assert.False(popover.IsCloseVisible);
        Assert.DoesNotContain("has-close", popover.Classes);
        Assert.True(toast.HasHeader);
        Assert.True(toast.HasContent);
        Assert.True(toast.HasAction);
        Assert.True(toast.HasIcon);
        Assert.Contains("has-close-content", toast.Classes);
        Assert.Contains("has-icon", toast.Classes);
    }

    [Fact]
    public void FeedbackControlsSyncVariantSizeAndStatusClasses()
    {
        var alert = new CodexAlert
        {
            Title = "Heads up",
            Description = "Check this detail.",
            Icon = "!",
            Action = "Review",
            Variant = CodexControlVariant.Destructive
        };
        var badge = new CodexBadge
        {
            Variant = CodexControlVariant.Ghost,
            Size = CodexControlSize.Large,
            IsStatusVisible = true,
            StatusVariant = CodexControlVariant.Warning
        };
        var avatar = new CodexAvatar
        {
            Fallback = "CS",
            Size = CodexControlSize.Icon,
            Variant = CodexControlVariant.Outline,
            IsStatusVisible = true,
            StatusVariant = CodexControlVariant.Destructive
        };
        var spinner = new CodexSpinner
        {
            Size = CodexControlSize.Large,
            IsActive = false,
            Label = "Loading records",
            StrokeThickness = 2.25
        };
        var progress = new CodexProgress
        {
            Variant = CodexControlVariant.Success,
            Size = CodexControlSize.Small,
            IsIndeterminate = true
        };
        var toast = new CodexToast { Variant = CodexControlVariant.Warning };
        CodexSonnerService.Clear();
        var sonner = new CodexSonner
        {
            Position = CodexSonnerPosition.TopCenter,
            Expand = false,
            RichColors = true,
            CloseButton = false,
            Gap = 12,
            Offset = new Thickness(24)
        };

        Assert.Contains("variant-destructive", alert.Classes);
        Assert.Contains("has-title", alert.Classes);
        Assert.Contains("has-description", alert.Classes);
        Assert.Contains("has-icon", alert.Classes);
        Assert.Contains("has-action", alert.Classes);
        Assert.Contains("variant-ghost", badge.Classes);
        Assert.Contains("size-lg", badge.Classes);
        Assert.Contains("status-visible", badge.Classes);
        Assert.Contains("status-warning", badge.Classes);
        Assert.DoesNotContain("variant-default", badge.Classes);
        Assert.Contains("has-fallback", avatar.Classes);
        Assert.Contains("size-icon", avatar.Classes);
        Assert.Contains("variant-outline", avatar.Classes);
        Assert.Contains("status-destructive", avatar.Classes);
        Assert.Contains("size-lg", spinner.Classes);
        Assert.Contains("paused", spinner.Classes);
        Assert.DoesNotContain("active", spinner.Classes);
        Assert.Equal("Loading records", spinner.Label);
        Assert.Equal("Loading records", AutomationProperties.GetName(spinner));
        Assert.Equal("idle", AutomationProperties.GetItemStatus(spinner));
        Assert.Contains("variant-success", progress.Classes);
        Assert.Contains("size-sm", progress.Classes);
        Assert.Contains("indeterminate", progress.Classes);
        Assert.Contains("variant-warning", toast.Classes);
        Assert.DoesNotContain("variant-default", toast.Classes);
        Assert.Contains("position-top-center", sonner.Classes);
        Assert.Contains("compact", sonner.Classes);
        Assert.Contains("rich-colors", sonner.Classes);
        Assert.Contains("close-hidden", sonner.Classes);
        Assert.Equal(HorizontalAlignment.Center, sonner.HorizontalAlignment);
        Assert.Equal(VerticalAlignment.Top, sonner.VerticalAlignment);
    }

    [Fact]
    public void SonnerServiceCreatesActionToastsAndDismissesThem()
    {
        CodexSonnerService.Clear();
        var actionRan = false;

        var toast = CodexSonnerService.Toast("Event has been created", new CodexSonnerOptions
        {
            Description = "Sunday, December 03, 2023 at 9:00 AM",
            Action = new CodexSonnerAction("Undo", () => actionRan = true),
            Duration = TimeSpan.Zero
        });

        Assert.Single(CodexSonnerService.Toasts);
        Assert.Equal("Event has been created", toast.Title);
        Assert.Equal("Sunday, December 03, 2023 at 9:00 AM", toast.Description);
        Assert.Equal(CodexSonnerToastType.Default, toast.Type);
        Assert.NotNull(toast.ActionCommand);

        toast.ActionCommand!.Execute(null);
        Assert.True(actionRan);

        toast.DismissCommand.Execute(null);
        Assert.True(toast.IsClosing);
        Assert.Single(CodexSonnerService.Toasts);
        CodexSonnerService.Clear();
    }

    [Fact]
    public void SonnerHostRendersVisibleToastsWithIconAndVariant()
    {
        CodexSonnerService.Clear();
        CodexSonnerService.Success("Saved", new CodexSonnerOptions
        {
            Description = "Changes synced.",
            Duration = TimeSpan.Zero
        });

        var sonner = new CodexSonner
        {
            RichColors = true,
            VisibleToasts = 3
        };

        var host = Assert.IsType<Border>(Assert.Single(sonner.Children));
        Assert.Contains("sonner-toast", host.Classes);
        Assert.Contains("entering", host.Classes);
        Assert.DoesNotContain("open", host.Classes);

        var rendered = Assert.IsType<CodexToast>(host.Child);
        Assert.Equal("Saved", rendered.Title);
        Assert.Equal("Changes synced.", rendered.Description);
        Assert.True(rendered.HasIcon);
        Assert.Equal(CodexControlVariant.Success, rendered.Variant);

        CodexSonnerService.Clear();
    }

    [Fact]
    public void OverlayAndFocusRingExposeReusableVisualProperties()
    {
        var overlay = new CodexOverlay
        {
            IsOpen = false,
            IsScrimVisible = false,
            ScrimOpacity = 0.42
        };
        var focusRing = new CodexFocusRing
        {
            RingThickness = new Thickness(3),
            RingOffset = new Thickness(4),
            IsRingVisible = false
        };

        Assert.Contains("is-closed", overlay.Classes);
        Assert.DoesNotContain("is-open", overlay.Classes);
        Assert.False(overlay.IsScrimVisible);
        Assert.Equal(0.42, overlay.ScrimOpacity);
        Assert.Equal(new Thickness(3), focusRing.RingThickness);
        Assert.Equal(new Thickness(4), focusRing.RingOffset);
        Assert.False(focusRing.IsRingVisible);
    }

    [Fact]
    public void OverlayAndFeedbackStylesDeclareTemplatesAndMotion()
    {
        var root = FindRepositoryRoot();

        AssertStyle(Path.Combine(root, "src", "CodexSwitchUI", "Themes", "Controls", "Dialog.axaml"),
            "PART_Surface", "PART_Header", "PART_Title", "PART_Description", "PART_Content", "PART_Action", "PART_Close", "Transitions");
        AssertStyle(Path.Combine(root, "src", "CodexSwitchUI", "Themes", "Controls", "Popover.axaml"),
            "PART_Surface", "PART_Header", "PART_Title", "PART_Description", "PART_Content", "PART_Action", "PART_Close", "Transitions");
        AssertStyle(Path.Combine(root, "src", "CodexSwitchUI", "Themes", "Controls", "Toast.axaml"),
            "PART_Surface", "PART_Status", "PART_Icon", "PART_Title", "PART_Description", "PART_Content", "PART_Action", "PART_Close", "Transitions");
        AssertStyleContains(Path.Combine(root, "src", "CodexSwitchUI", "Themes", "Controls", "Sonner.axaml"),
            "controls|CodexSonner", "position-bottom-right", "position-top-center", "rich-colors", "close-hidden", "sonner-toast", "MaxHeight", "ThicknessTransition", "Transitions");
        AssertStyle(Path.Combine(root, "src", "CodexSwitchUI", "Themes", "Controls", "Alert.axaml"),
            "PART_Surface", "PART_Icon", "PART_DefaultIcon", "PART_Title", "PART_Description", "PART_Content", "PART_Action", "variant-destructive", "variant-success", "variant-warning", "Transitions");
        AssertStyle(Path.Combine(root, "src", "CodexSwitchUI", "Themes", "Controls", "Badge.axaml"),
            "PART_Surface", "PART_Status", "size-lg", "status-warning", "variant-secondary", "variant-destructive", "variant-outline", "variant-success", "variant-warning", "variant-ghost");
        AssertStyle(Path.Combine(root, "src", "CodexSwitchUI", "Themes", "Controls", "Avatar.axaml"),
            "PART_Surface", "PART_Image", "PART_Fallback", "PART_Status", "status-destructive", "variant-outline");
        AssertStyleContains(Path.Combine(root, "src", "CodexSwitchUI", "Themes", "Controls", "Spinner.axaml"),
            "controls|CodexSpinner", "CodexSwitch.ForegroundBrush", "StrokeThickness", "size-sm", "size-lg", "paused", "DoubleTransition", "BrushTransition");
        AssertStyle(Path.Combine(root, "src", "CodexSwitchUI", "Themes", "Controls", "Progress.axaml"),
            "PART_Track", "PART_Indicator", "PART_IndeterminateIndicator", "DoubleTransition", "BrushTransition");
        AssertStyle(Path.Combine(root, "src", "CodexSwitchUI", "Themes", "Controls", "Skeleton.axaml"),
            "PART_Surface", "PART_Shimmer", "CodexSwitch.AccentBrush", "PulseOpacity", "PulseDuration", "CodexSwitch.SkeletonShimmerDuration", "ShimmerOpacity", "ShimmerBrush");
        AssertStyle(Path.Combine(root, "src", "CodexSwitchUI", "Themes", "Primitives", "Overlay.axaml"),
            "PART_Scrim", "CodexSwitch.ForegroundBrush", "ScrimBrush", "ScrimOpacity", "is-open", "is-closed");
        AssertStyle(Path.Combine(root, "src", "CodexSwitchUI", "Themes", "Primitives", "FocusRing.axaml"),
            "PART_Ring", "PART_Content", "RingBrush", "RingThickness", "RingOffset");
    }

    [Fact]
    public void OverlayFeedbackStylesDoNotExposeDefaultTemplateSurface()
    {
        var root = FindRepositoryRoot();
        var files = new[]
        {
            "Dialog.axaml",
            "Popover.axaml",
            "Toast.axaml",
            "Alert.axaml",
            "Badge.axaml",
            "Avatar.axaml",
            "Progress.axaml",
            "Skeleton.axaml"
        };

        foreach (var file in files)
        {
            var style = File.ReadAllText(Path.Combine(root, "src", "CodexSwitchUI", "Themes", "Controls", file));
            Assert.Contains("ControlTemplate", style);
            Assert.Contains("PART_", style);
            Assert.DoesNotContain("Fluent", style);
            Assert.Contains("CodexSwitch.", style);
        }

        var overlay = File.ReadAllText(Path.Combine(root, "src", "CodexSwitchUI", "Themes", "Primitives", "Overlay.axaml"));
        var focusRing = File.ReadAllText(Path.Combine(root, "src", "CodexSwitchUI", "Themes", "Primitives", "FocusRing.axaml"));

        Assert.DoesNotContain("#99000000", overlay);
        Assert.DoesNotContain("Margin=\"{TemplateBinding RingOffset}\" Content", focusRing);
    }

    private static void AssertStyle(string path, params string[] expectedFragments)
    {
        var style = File.ReadAllText(path);

        Assert.Contains("ControlTemplate", style);
        foreach (var fragment in expectedFragments)
        {
            Assert.Contains(fragment, style);
        }
    }

    private static void AssertStyleContains(string path, params string[] expectedFragments)
    {
        var style = File.ReadAllText(path);

        foreach (var fragment in expectedFragments)
        {
            Assert.Contains(fragment, style);
        }
    }

    private static string FindRepositoryRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current is not null)
        {
            if (File.Exists(Path.Combine(current.FullName, "CodexSwitchUI.slnx")))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new DirectoryNotFoundException("Could not locate repository root from test output directory.");
    }
}
