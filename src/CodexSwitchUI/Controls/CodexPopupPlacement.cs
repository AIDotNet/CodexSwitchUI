using Avalonia.Controls;

namespace CodexSwitchUI.Controls;

internal enum CodexPopupAlignment
{
    Center,
    Start,
    End
}

internal static class CodexPopupPlacement
{
    public static PlacementMode Resolve(PlacementMode placement, CodexPopoverAlign align)
    {
        return Resolve(placement, align.ToPopupAlignment());
    }

    public static PlacementMode Resolve(PlacementMode placement, CodexHoverCardAlign align)
    {
        return Resolve(placement, align.ToPopupAlignment());
    }

    public static PlacementMode Resolve(PlacementMode placement, CodexDropdownAlign align)
    {
        return Resolve(placement, align.ToPopupAlignment());
    }

    private static PlacementMode Resolve(PlacementMode placement, CodexPopupAlignment align)
    {
        var side = SideFromPlacementOrNull(placement);
        if (side is null || align == CodexPopupAlignment.Center && IsEdgeAligned(placement))
        {
            return placement;
        }

        return side.Value switch
        {
            CodexPopupSide.Top => align switch
            {
                CodexPopupAlignment.Start => PlacementMode.TopEdgeAlignedLeft,
                CodexPopupAlignment.End => PlacementMode.TopEdgeAlignedRight,
                _ => PlacementMode.Top
            },
            CodexPopupSide.Left => align switch
            {
                CodexPopupAlignment.Start => PlacementMode.LeftEdgeAlignedTop,
                CodexPopupAlignment.End => PlacementMode.LeftEdgeAlignedBottom,
                _ => PlacementMode.Left
            },
            CodexPopupSide.Right => align switch
            {
                CodexPopupAlignment.Start => PlacementMode.RightEdgeAlignedTop,
                CodexPopupAlignment.End => PlacementMode.RightEdgeAlignedBottom,
                _ => PlacementMode.Right
            },
            _ => align switch
            {
                CodexPopupAlignment.Start => PlacementMode.BottomEdgeAlignedLeft,
                CodexPopupAlignment.End => PlacementMode.BottomEdgeAlignedRight,
                _ => PlacementMode.Bottom
            }
        };
    }

    private static CodexPopupSide? SideFromPlacementOrNull(PlacementMode placement)
    {
        return placement switch
        {
            PlacementMode.Top or PlacementMode.TopEdgeAlignedLeft or PlacementMode.TopEdgeAlignedRight => CodexPopupSide.Top,
            PlacementMode.Bottom or PlacementMode.BottomEdgeAlignedLeft or PlacementMode.BottomEdgeAlignedRight => CodexPopupSide.Bottom,
            PlacementMode.Left or PlacementMode.LeftEdgeAlignedTop or PlacementMode.LeftEdgeAlignedBottom => CodexPopupSide.Left,
            PlacementMode.Right or PlacementMode.RightEdgeAlignedTop or PlacementMode.RightEdgeAlignedBottom => CodexPopupSide.Right,
            _ => null
        };
    }

    private static bool IsEdgeAligned(PlacementMode placement)
    {
        return placement is PlacementMode.TopEdgeAlignedLeft
            or PlacementMode.TopEdgeAlignedRight
            or PlacementMode.BottomEdgeAlignedLeft
            or PlacementMode.BottomEdgeAlignedRight
            or PlacementMode.LeftEdgeAlignedTop
            or PlacementMode.LeftEdgeAlignedBottom
            or PlacementMode.RightEdgeAlignedTop
            or PlacementMode.RightEdgeAlignedBottom;
    }

    private static CodexPopupAlignment ToPopupAlignment(this CodexPopoverAlign align)
    {
        return align switch
        {
            CodexPopoverAlign.Start => CodexPopupAlignment.Start,
            CodexPopoverAlign.End => CodexPopupAlignment.End,
            _ => CodexPopupAlignment.Center
        };
    }

    private static CodexPopupAlignment ToPopupAlignment(this CodexHoverCardAlign align)
    {
        return align switch
        {
            CodexHoverCardAlign.Start => CodexPopupAlignment.Start,
            CodexHoverCardAlign.End => CodexPopupAlignment.End,
            _ => CodexPopupAlignment.Center
        };
    }

    private static CodexPopupAlignment ToPopupAlignment(this CodexDropdownAlign align)
    {
        return align switch
        {
            CodexDropdownAlign.Start => CodexPopupAlignment.Start,
            CodexDropdownAlign.End => CodexPopupAlignment.End,
            _ => CodexPopupAlignment.Center
        };
    }

    private enum CodexPopupSide
    {
        Bottom,
        Top,
        Left,
        Right
    }
}
