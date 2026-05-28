using Avalonia.Controls;
using Avalonia.Layout;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;

public static class DrawerInteractionSample
{
    public static Control BuildDrawerInteractionPreview()
    {
        var trigger = new CodexButton
        {
            Content = "Open drawer",
            Size = CodexControlSize.Small
        };
        var status = new CodexText
        {
            Role = CodexTextRole.Muted,
            Text = "Drawer is open at the bottom edge."
        };
        var drawer = new CodexDrawer
        {
            Trigger = trigger,
            Title = "Route actions",
            Description = "Drag the handle past the threshold or dismiss through the shared dialog command.",
            Direction = CodexDrawerDirection.Bottom,
            IsOpen = true,
            RestoreFocusElement = trigger,
            Content = new CodexText
            {
                Role = CodexTextRole.Muted,
                Text = "The handle gesture updates drag state before deciding whether to close."
            },
            Action = new CodexButton { Content = "Submit", Size = CodexControlSize.Small }
        };

        drawer.OpenChanged += (_, args) =>
        {
            status.Text = args.IsOpen
                ? $"Drawer opened at the {drawer.Direction.ToString().ToLowerInvariant()} edge."
                : "Drawer closed and left the trigger mounted.";
        };
        drawer.DragCompleted += (_, args) =>
        {
            status.Text = args.Dismissed
                ? $"Drag dismissed at {args.DragOffset:0}px and requested close."
                : $"Drag settled at {args.DragOffset:0}px without closing.";
        };
        drawer.RestoreFocusRequested += (_, _) =>
        {
            status.Text = "Drawer dismissed and requested focus restoration to the trigger.";
        };

        var dragShort = new CodexButton
        {
            Content = "Drag 48px",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Secondary
        };
        dragShort.Click += (_, _) =>
        {
            drawer.BeginDrag();
            drawer.DragBy(48);
            drawer.CompleteDrag();
        };

        var dragDismiss = new CodexButton
        {
            Content = "Drag 128px",
            Size = CodexControlSize.Small
        };
        dragDismiss.Click += (_, _) =>
        {
            drawer.IsOpen = true;
            drawer.BeginDrag();
            drawer.DragBy(128);
            drawer.CompleteDrag();
        };

        var cycleDirection = new CodexButton
        {
            Content = "Cycle direction",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Ghost
        };
        cycleDirection.Click += (_, _) =>
        {
            drawer.Direction = drawer.Direction switch
            {
                CodexDrawerDirection.Bottom => CodexDrawerDirection.Right,
                CodexDrawerDirection.Right => CodexDrawerDirection.Top,
                CodexDrawerDirection.Top => CodexDrawerDirection.Left,
                _ => CodexDrawerDirection.Bottom
            };
            drawer.IsOpen = true;
            status.Text = $"Drawer direction changed to {drawer.Direction}; edge classes updated.";
        };

        var manualDrawer = new CodexDrawer
        {
            Title = "Manual drawer",
            Direction = CodexDrawerDirection.Right,
            IsOpen = true,
            CloseOnEscape = false,
            DismissOnOutsidePointer = false,
            CloseOnDragDismiss = false,
            Content = new CodexText
            {
                Role = CodexTextRole.Muted,
                Text = "Host code decides when this drawer closes."
            }
        };

        return new StackPanel
        {
            Spacing = 10,
            Children =
            {
                status,
                drawer,
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 8,
                    Children =
                    {
                        dragShort,
                        dragDismiss,
                        cycleDirection
                    }
                },
                manualDrawer
            }
        };
    }
}
