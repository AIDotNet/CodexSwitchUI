using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Layout;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;
using CodexSwitchUI.Themes;
using System.Windows.Input;
using Xunit;

namespace CodexSwitchUI.Tests;

public class OverlayFeedbackComponentTests
{
    [Fact]
    public void DisclosureAndTooltipLikeComponentsDefaultClosed()
    {
        Assert.False(new CodexDialog().IsOpen);
        Assert.False(new CodexAlertDialog().IsOpen);
        Assert.False(new CodexCommandDialog().IsOpen);
        Assert.False(new CodexPopover().IsOpen);
        Assert.False(new CodexTooltip().IsOpen);
        Assert.False(new CodexHoverCard().IsOpen);
        Assert.False(new CodexSheet().IsOpen);
        Assert.False(new CodexDrawer().IsOpen);
        Assert.False(new CodexDropdownButton().IsOpen);
        Assert.False(new CodexSplitButton().IsOpen);
        Assert.False(new CodexCollapsible().IsOpen);
        Assert.False(new CodexAccordionItem().IsOpen);
        Assert.False(new CodexNavigationMenuItem().IsOpen);
        Assert.False(new CodexCombobox().IsOpen);
        Assert.False(new CodexSelect().IsDropDownOpen);
        Assert.False(new CodexNativeSelect().IsDropDownOpen);
        Assert.False(new CodexDatePicker().IsOpen);
        Assert.False(new CodexOverlay().IsOpen);
        Assert.False(new CodexChartTooltipContent().IsOpen);

        Assert.True(new CodexToast().IsOpen);
        Assert.True(new CodexSidebarProvider().IsOpen);
        Assert.True(new CodexSidebar().IsOpen);
        Assert.True(new CodexSidebarInset().IsOpen);
    }

    [Fact]
    public void DialogPopoverAndToastExposeSlotState()
    {
        var dialog = new CodexDialog
        {
            Title = "Dialog title",
            Description = "Dialog description",
            Content = "Dialog content",
            Action = "Dialog action",
            IsOpen = true,
            CloseContent = "Close"
        };
        var popover = new CodexPopover
        {
            Title = "Popover title",
            Description = "Popover description",
            Content = "Popover content",
            Action = "Popover action",
            IsOpen = true,
            IsCloseVisible = false
        };
        var tooltip = new CodexTooltip
        {
            Content = "Tooltip content",
            Placement = PlacementMode.Bottom,
            Size = CodexControlSize.Small,
            IsOpen = false,
            IsArrowVisible = true
        };
        var hoverCard = new CodexHoverCard
        {
            Trigger = "Preview",
            Content = "Hover card content",
            Placement = PlacementMode.Right,
            Align = CodexHoverCardAlign.Start
        };
        var sheet = new CodexSheet
        {
            Title = "Provider filters",
            Description = "Slide from the active edge.",
            Content = "Filter controls",
            Action = "Apply",
            CloseContent = "Close",
            IsOpen = true,
            Side = CodexSheetSide.Left
        };
        var drawer = new CodexDrawer
        {
            Title = "Provider usage",
            Description = "Drag from the handle.",
            Content = "Drawer content",
            Action = "Submit",
            CloseContent = "Close",
            IsOpen = true,
            Direction = CodexDrawerDirection.Left
        };
        var alertDialog = new CodexAlertDialog
        {
            Title = "Delete provider?",
            Description = "Requires an explicit response.",
            Media = "!",
            Content = "This cannot be undone.",
            ActionContent = "Delete",
            IsOpen = true,
            ActionVariant = CodexControlVariant.Destructive
        };
        var dropdown = new CodexDropdownButton
        {
            Content = "Actions",
            DropDownContent = "Dropdown content",
            Placement = PlacementMode.Bottom,
            Align = CodexDropdownAlign.End,
            IsArrowVisible = true
        };
        var splitButton = new CodexSplitButton
        {
            Content = "Run",
            DropDownContent = "More actions",
            Placement = PlacementMode.Right,
            Align = CodexDropdownAlign.Start,
            IsArrowVisible = true
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
        var emptyState = new CodexEmptyState
        {
            Icon = "!",
            Title = "No providers",
            Description = "Add a provider to start routing requests.",
            Content = "Provider routing depends on at least one enabled provider.",
            Action = "Add provider",
            SecondaryAction = "Import"
        };

        Assert.True(dialog.HasHeader);
        Assert.True(dialog.HasContent);
        Assert.True(dialog.HasAction);
        Assert.True(dialog.IsOpen);
        Assert.Contains("open", dialog.Classes);
        Assert.True(dialog.IsCloseVisible);
        Assert.Contains("has-close-content", dialog.Classes);
        Assert.True(popover.HasHeader);
        Assert.True(popover.HasContent);
        Assert.True(popover.HasAction);
        Assert.True(popover.IsOpen);
        Assert.Contains("open", popover.Classes);
        Assert.False(popover.IsCloseVisible);
        Assert.DoesNotContain("has-close", popover.Classes);
        Assert.True(tooltip.HasContent);
        Assert.False(tooltip.Focusable);
        Assert.False(tooltip.IsHitTestVisible);
        Assert.Contains("closed", tooltip.Classes);
        Assert.Contains("side-bottom", tooltip.Classes);
        Assert.Contains("size-sm", tooltip.Classes);
        Assert.Contains("has-arrow", tooltip.Classes);
        Assert.Contains("has-content", tooltip.Classes);
        Assert.True(hoverCard.HasTrigger);
        Assert.True(hoverCard.HasContent);
        Assert.False(hoverCard.IsOpen);
        Assert.Equal(TimeSpan.FromMilliseconds(700), hoverCard.OpenDelay);
        Assert.Equal(TimeSpan.FromMilliseconds(300), hoverCard.CloseDelay);
        Assert.Contains("closed", hoverCard.Classes);
        Assert.Contains("side-right", hoverCard.Classes);
        Assert.Contains("align-start", hoverCard.Classes);
        Assert.Contains("has-arrow", hoverCard.Classes);
        Assert.True(sheet.HasHeader);
        Assert.True(sheet.HasContent);
        Assert.True(sheet.HasAction);
        Assert.True(sheet.IsOpen);
        Assert.Equal(CodexSheetSide.Left, sheet.Side);
        Assert.Contains("open", sheet.Classes);
        Assert.Contains("side-left", sheet.Classes);
        Assert.Contains("has-close-content", sheet.Classes);
        Assert.True(drawer.HasHeader);
        Assert.True(drawer.HasContent);
        Assert.True(drawer.HasAction);
        Assert.True(drawer.IsOpen);
        Assert.Equal(CodexDrawerDirection.Left, drawer.Direction);
        Assert.True(drawer.IsHandleVisible);
        Assert.True(drawer.ShouldScaleBackground);
        Assert.True(drawer.CloseOnDragDismiss);
        Assert.Contains("open", drawer.Classes);
        Assert.Contains("direction-left", drawer.Classes);
        Assert.Contains("has-handle", drawer.Classes);
        Assert.Contains("scale-background", drawer.Classes);
        Assert.Contains("close-on-drag", drawer.Classes);
        Assert.Contains("has-close-content", drawer.Classes);
        Assert.True(alertDialog.HasHeader);
        Assert.True(alertDialog.HasContent);
        Assert.True(alertDialog.HasMedia);
        Assert.True(alertDialog.HasCancelContent);
        Assert.True(alertDialog.HasActionContent);
        Assert.True(alertDialog.IsOpen);
        Assert.False(alertDialog.IsCloseVisible);
        Assert.False(alertDialog.DismissOnOutsidePointer);
        Assert.True(alertDialog.FocusCancelOnOpen);
        Assert.Contains("alert-dialog", alertDialog.Classes);
        Assert.Contains("response-required", alertDialog.Classes);
        Assert.Contains("focus-cancel", alertDialog.Classes);
        Assert.Contains("action-destructive", alertDialog.Classes);
        Assert.Contains("has-media", alertDialog.Classes);
        Assert.True(dropdown.HasDropDownContent);
        Assert.False(dropdown.IsOpen);
        Assert.Contains("closed", dropdown.Classes);
        Assert.Contains("side-bottom", dropdown.Classes);
        Assert.Contains("align-end", dropdown.Classes);
        Assert.Contains("has-arrow", dropdown.Classes);
        Assert.Contains("close-on-select", dropdown.Classes);
        Assert.True(splitButton.HasDropDownContent);
        Assert.True(splitButton.CanOpenDropDown);
        Assert.True(splitButton.IsPrimaryActionAvailable);
        Assert.False(splitButton.IsOpen);
        Assert.Contains("closed", splitButton.Classes);
        Assert.Contains("side-right", splitButton.Classes);
        Assert.Contains("align-start", splitButton.Classes);
        Assert.Contains("has-arrow", splitButton.Classes);
        Assert.Contains("can-open-dropdown", splitButton.Classes);
        Assert.True(toast.HasHeader);
        Assert.True(toast.HasContent);
        Assert.True(toast.HasAction);
        Assert.True(toast.IsOpen);
        Assert.Contains("open", toast.Classes);
        Assert.True(toast.HasIcon);
        Assert.Contains("has-close-content", toast.Classes);
        Assert.Contains("has-icon", toast.Classes);
        Assert.True(emptyState.HasIcon);
        Assert.True(emptyState.HasTitle);
        Assert.True(emptyState.HasDescription);
        Assert.True(emptyState.HasHeader);
        Assert.True(emptyState.HasContent);
        Assert.True(emptyState.HasAction);
        Assert.True(emptyState.HasSecondaryAction);
        Assert.True(emptyState.HasActions);
        Assert.True(emptyState.CanExecuteAction);
        Assert.True(emptyState.CanExecuteSecondaryAction);
        Assert.Contains("has-header", emptyState.Classes);
        Assert.Contains("has-action", emptyState.Classes);
        Assert.Contains("has-secondary-action", emptyState.Classes);
        Assert.Contains("has-actions", emptyState.Classes);
        Assert.Contains("can-action", emptyState.Classes);
    }

    [Fact]
    public void DismissableOverlayControlsCloseOnCommandEscapeAndOutsidePointer()
    {
        var dialogClosed = 0;
        var popoverClosed = 0;
        var sheetClosed = 0;
        var drawerClosed = 0;
        var toastClosed = 0;
        var alertDialogClosed = 0;
        var dialog = new CodexDialog { IsOpen = true, CloseCommand = new TestCommand(() => dialogClosed++) };
        var popover = new CodexPopover { IsOpen = true, CloseCommand = new TestCommand(() => popoverClosed++) };
        var sheet = new CodexSheet { IsOpen = true, CloseCommand = new TestCommand(() => sheetClosed++) };
        var drawer = new CodexDrawer { IsOpen = true, CloseCommand = new TestCommand(() => drawerClosed++) };
        var toast = new CodexToast { CloseCommand = new TestCommand(() => toastClosed++) };
        var alertDialog = new CodexAlertDialog { IsOpen = true, CloseCommand = new TestCommand(() => alertDialogClosed++) };
        var tooltip = new CodexTooltip { IsOpen = true };

        Assert.NotNull(dialog.DismissCommand);
        dialog.DismissCommand!.Execute(null);
        Assert.False(dialog.IsOpen);
        Assert.Contains("closed", dialog.Classes);
        Assert.Equal(1, dialogClosed);

        popover.CloseOnEscape = false;
        Assert.False(popover.TryHandleDismissKey(Key.Escape));
        Assert.True(popover.IsOpen);
        popover.CloseOnEscape = true;
        Assert.True(popover.TryHandleDismissKey(Key.Escape));
        Assert.False(popover.IsOpen);
        Assert.Equal(1, popoverClosed);

        sheet.CloseOnEscape = false;
        Assert.False(sheet.TryHandleDismissKey(Key.Escape));
        Assert.True(sheet.IsOpen);
        sheet.CloseOnEscape = true;
        Assert.True(sheet.TryHandleDismissKey(Key.Escape));
        Assert.False(sheet.IsOpen);
        Assert.Equal(1, sheetClosed);

        drawer.CloseOnEscape = false;
        Assert.False(drawer.TryHandleDismissKey(Key.Escape));
        Assert.True(drawer.IsOpen);
        drawer.CloseOnEscape = true;
        Assert.True(drawer.TryHandleDismissKey(Key.Escape));
        Assert.False(drawer.IsOpen);
        Assert.Equal(1, drawerClosed);

        toast.CloseOnEscape = true;
        Assert.True(toast.TryHandleDismissKey(Key.Escape));
        Assert.False(toast.IsOpen);
        Assert.Equal(1, toastClosed);

        dialog.IsOpen = true;
        dialog.DismissOnOutsidePointer = false;
        Assert.False(dialog.TryDismissFromOutsidePointer());
        Assert.True(dialog.IsOpen);
        dialog.DismissOnOutsidePointer = true;
        Assert.True(dialog.TryDismissFromOutsidePointer());
        Assert.False(dialog.IsOpen);
        Assert.Equal(2, dialogClosed);

        sheet.IsOpen = true;
        sheet.DismissOnOutsidePointer = false;
        Assert.False(sheet.TryDismissFromOutsidePointer());
        Assert.True(sheet.IsOpen);
        sheet.DismissOnOutsidePointer = true;
        Assert.True(sheet.TryDismissFromOutsidePointer());
        Assert.False(sheet.IsOpen);
        Assert.Equal(2, sheetClosed);

        drawer.IsOpen = true;
        drawer.DismissOnOutsidePointer = false;
        Assert.False(drawer.TryDismissFromOutsidePointer());
        Assert.True(drawer.IsOpen);
        drawer.DismissOnOutsidePointer = true;
        Assert.True(drawer.TryDismissFromOutsidePointer());
        Assert.False(drawer.IsOpen);
        Assert.Equal(2, drawerClosed);

        Assert.False(alertDialog.DismissOnOutsidePointer);
        Assert.False(alertDialog.TryDismissFromOutsidePointer());
        Assert.True(alertDialog.IsOpen);
        Assert.True(alertDialog.TryHandleDismissKey(Key.Escape));
        Assert.False(alertDialog.IsOpen);
        Assert.Equal(1, alertDialogClosed);

        tooltip.CloseOnEscape = true;
        Assert.True(tooltip.TryHandleDismissKey(Key.Escape));
        Assert.False(tooltip.IsOpen);
        Assert.Contains("closed", tooltip.Classes);
    }

    [Fact]
    public void OverlayOpenChangedEventsExposeSourceMetadata()
    {
        var dialogChanges = new List<(bool IsOpen, CodexDialogOpenChangeSource Source)>();
        var dialog = new CodexDialog();
        dialog.OpenChanged += (_, args) => dialogChanges.Add((args.IsOpen, args.Source));

        dialog.Open();
        Assert.True(dialog.TryHandleDismissKey(Key.Escape));
        Assert.True(dialog.TryHandleTriggerPointerRelease(PointerUpdateKind.LeftButtonReleased));
        Assert.True(dialog.TryDismissFromOutsidePointer());

        Assert.Equal(
            [
                (true, CodexDialogOpenChangeSource.Programmatic),
                (false, CodexDialogOpenChangeSource.Keyboard),
                (true, CodexDialogOpenChangeSource.Pointer),
                (false, CodexDialogOpenChangeSource.Pointer)
            ],
            dialogChanges);

        var sheetChanges = new List<(bool IsOpen, CodexDialogOpenChangeSource Source)>();
        var sheet = new CodexSheet();
        sheet.OpenChanged += (_, args) => sheetChanges.Add((args.IsOpen, args.Source));

        Assert.True(sheet.TryHandleTriggerKey(Key.Enter));
        Assert.True(sheet.Dismiss());
        Assert.True(sheet.TryHandleTriggerPointerRelease(PointerUpdateKind.LeftButtonReleased));

        Assert.Equal(
            [
                (true, CodexDialogOpenChangeSource.Keyboard),
                (false, CodexDialogOpenChangeSource.Programmatic),
                (true, CodexDialogOpenChangeSource.Pointer)
            ],
            sheetChanges);

        var drawerChanges = new List<(bool IsOpen, CodexDialogOpenChangeSource Source)>();
        var drawer = new CodexDrawer { IsOpen = true };
        drawer.OpenChanged += (_, args) => drawerChanges.Add((args.IsOpen, args.Source));

        Assert.True(drawer.DragBy(128));
        Assert.True(drawer.CompleteDrag());

        Assert.Equal([(false, CodexDialogOpenChangeSource.Pointer)], drawerChanges);

        var commandChanges = new List<(bool IsOpen, CodexDialogOpenChangeSource Source)>();
        var commandDialog = new CodexCommandDialog { IsOpen = true };
        commandDialog.OpenChanged += (_, args) => commandChanges.Add((args.IsOpen, args.Source));

        commandDialog.NotifyItemSelected(
            new CodexCommandItem { Content = "Open provider" },
            CodexCommandItemSelectSource.Keyboard);

        Assert.Equal([(false, CodexDialogOpenChangeSource.Keyboard)], commandChanges);
    }

    [Fact]
    public void PopoverTooltipAndHoverCardOpenChangedEventsExposeSourceMetadata()
    {
        var popoverChanges = new List<(bool IsOpen, CodexPopoverOpenChangeSource Source)>();
        var popover = new CodexPopover();
        popover.OpenChanged += (_, args) => popoverChanges.Add((args.IsOpen, args.Source));

        popover.Open();
        Assert.True(popover.TryHandleTriggerKey(Key.Space));
        Assert.True(popover.TryHandleTriggerPointerRelease(PointerUpdateKind.LeftButtonReleased));
        Assert.True(popover.TryDismissFromOutsidePointer());

        Assert.Equal(
            [
                (true, CodexPopoverOpenChangeSource.Programmatic),
                (false, CodexPopoverOpenChangeSource.Keyboard),
                (true, CodexPopoverOpenChangeSource.Pointer),
                (false, CodexPopoverOpenChangeSource.Pointer)
            ],
            popoverChanges);

        var tooltipChanges = new List<(bool IsOpen, CodexTooltipOpenChangeSource Source)>();
        var tooltip = new CodexTooltip
        {
            Trigger = "Hover",
            Content = "Open details",
            OpenDelay = TimeSpan.Zero,
            CloseDelay = TimeSpan.Zero
        };
        tooltip.OpenChanged += (_, args) => tooltipChanges.Add((args.IsOpen, args.Source));

        Assert.True(tooltip.RequestOpen());
        Assert.True(tooltip.TryHandleDismissKey(Key.Escape));
        Assert.True(tooltip.RequestFocusOpen());
        Assert.True(tooltip.RequestClose(CodexTooltipOpenChangeSource.Focus));
        tooltip.Open();
        Assert.True(tooltip.TryHandleDismissKey(Key.Enter));

        Assert.Equal(
            [
                (true, CodexTooltipOpenChangeSource.Pointer),
                (false, CodexTooltipOpenChangeSource.Keyboard),
                (true, CodexTooltipOpenChangeSource.Focus),
                (false, CodexTooltipOpenChangeSource.Focus),
                (true, CodexTooltipOpenChangeSource.Programmatic),
                (false, CodexTooltipOpenChangeSource.Keyboard)
            ],
            tooltipChanges);

        var hoverChanges = new List<(bool IsOpen, CodexHoverCardOpenChangeSource Source)>();
        var hoverCard = new CodexHoverCard
        {
            Trigger = "Provider",
            Content = "Preview",
            OpenDelay = TimeSpan.Zero,
            CloseDelay = TimeSpan.Zero
        };
        hoverCard.OpenChanged += (_, args) => hoverChanges.Add((args.IsOpen, args.Source));

        Assert.True(hoverCard.RequestOpen());
        Assert.True(hoverCard.RequestClose(CodexHoverCardOpenChangeSource.Focus));
        hoverCard.Open();
        Assert.True(hoverCard.TryHandleDismissKey(Key.Escape));

        Assert.Equal(
            [
                (true, CodexHoverCardOpenChangeSource.Pointer),
                (false, CodexHoverCardOpenChangeSource.Focus),
                (true, CodexHoverCardOpenChangeSource.Programmatic),
                (false, CodexHoverCardOpenChangeSource.Keyboard)
            ],
            hoverChanges);
    }

    [Fact]
    public void DialogTriggerOpenChangedAndKeyboardToggleMirrorWebRoot()
    {
        var changes = new List<bool>();
        var dialog = new CodexDialog
        {
            Trigger = new CodexButton { Content = "Edit profile" },
            Title = "Edit profile",
            Description = "Make changes to your profile here.",
            Content = "Dialog body",
            Action = new CodexButton { Content = "Save changes" }
        };
        dialog.OpenChanged += (_, args) => changes.Add(args.IsOpen);

        Assert.False(dialog.IsOpen);
        Assert.True(dialog.IsModal);
        Assert.True(dialog.HasTrigger);
        Assert.True(dialog.HasContent);
        Assert.Contains("closed", dialog.Classes);
        Assert.Contains("modal", dialog.Classes);
        Assert.Contains("has-trigger", dialog.Classes);
        Assert.Contains("trigger-closed", dialog.Classes);

        Assert.True(dialog.TryToggleFromTrigger());
        Assert.True(dialog.IsOpen);
        Assert.Equal([true], changes);
        Assert.Contains("open", dialog.Classes);
        Assert.Contains("trigger-open", dialog.Classes);

        Assert.True(dialog.TryHandleTriggerKey(Key.Enter));
        Assert.False(dialog.IsOpen);
        Assert.Equal([true, false], changes);

        Assert.True(dialog.TryHandleTriggerKey(Key.Space));
        Assert.True(dialog.IsOpen);
        Assert.Equal([true, false, true], changes);

        dialog.CloseOnEscape = false;
        Assert.False(dialog.TryHandleDismissKey(Key.Escape));
        Assert.True(dialog.IsOpen);

        dialog.CloseOnEscape = true;
        Assert.True(dialog.TryHandleDismissKey(Key.Escape));
        Assert.False(dialog.IsOpen);
        Assert.Equal([true, false, true, false], changes);

        dialog.IsModal = false;
        Assert.Contains("non-modal", dialog.Classes);
        Assert.DoesNotContain("modal", dialog.Classes);

        dialog.IsEnabled = false;
        Assert.False(dialog.TryToggleFromTrigger());
        Assert.False(dialog.IsOpen);
    }

    [Fact]
    public void PopoverTriggerOpenChangedAndKeyboardToggleMirrorWebRoot()
    {
        var changes = new List<bool>();
        var popover = new CodexPopover
        {
            Trigger = new CodexButton { Content = "Open dimensions" },
            Title = "Dimensions",
            Description = "Update layout constraints.",
            Content = "Popover body",
            Placement = PlacementMode.Right,
            Align = CodexPopoverAlign.Start,
            IsArrowVisible = true
        };
        popover.OpenChanged += (_, args) => changes.Add(args.IsOpen);

        Assert.False(popover.IsOpen);
        Assert.Contains("closed", popover.Classes);
        Assert.True(popover.HasTrigger);
        Assert.True(popover.HasContent);
        Assert.Contains("has-trigger", popover.Classes);
        Assert.Contains("trigger-closed", popover.Classes);
        Assert.Contains("side-right", popover.Classes);
        Assert.Contains("align-start", popover.Classes);
        Assert.Contains("has-arrow", popover.Classes);

        Assert.True(popover.TryToggleFromTrigger());
        Assert.True(popover.IsOpen);
        Assert.Equal([true], changes);
        Assert.Contains("open", popover.Classes);
        Assert.Contains("trigger-open", popover.Classes);

        Assert.True(popover.TryHandleTriggerKey(Key.Enter));
        Assert.False(popover.IsOpen);
        Assert.Equal([true, false], changes);

        Assert.True(popover.TryHandleTriggerKey(Key.Space));
        Assert.True(popover.IsOpen);
        Assert.Equal([true, false, true], changes);

        popover.CloseOnEscape = false;
        Assert.False(popover.TryHandleDismissKey(Key.Escape));
        Assert.True(popover.IsOpen);

        popover.CloseOnEscape = true;
        Assert.True(popover.TryHandleDismissKey(Key.Escape));
        Assert.False(popover.IsOpen);
        Assert.Equal([true, false, true, false], changes);

        popover.IsEnabled = false;
        Assert.False(popover.TryToggleFromTrigger());
        Assert.False(popover.IsOpen);
    }

    [Fact]
    public void DialogSheetDrawerAndPopoverTriggersOnlyUsePrimaryPointerRelease()
    {
        var dialog = new CodexDialog { Trigger = new CodexButton { Content = "Open dialog" } };
        var alertDialog = new CodexAlertDialog { Trigger = new CodexButton { Content = "Delete provider" } };
        var commandDialog = new CodexCommandDialog { Trigger = new CodexButton { Content = "Open command" } };
        var sheet = new CodexSheet { Trigger = new CodexButton { Content = "Open sheet" } };
        var drawer = new CodexDrawer { Trigger = new CodexButton { Content = "Open drawer" } };
        var popover = new CodexPopover { Trigger = new CodexButton { Content = "Open popover" } };

        Assert.False(dialog.TryHandleTriggerPointerRelease(PointerUpdateKind.RightButtonReleased));
        Assert.False(dialog.IsOpen);
        Assert.False(alertDialog.TryHandleTriggerPointerRelease(PointerUpdateKind.MiddleButtonReleased));
        Assert.False(alertDialog.IsOpen);
        Assert.False(commandDialog.TryHandleTriggerPointerRelease(PointerUpdateKind.RightButtonReleased));
        Assert.False(commandDialog.IsOpen);
        Assert.False(sheet.TryHandleTriggerPointerRelease(PointerUpdateKind.MiddleButtonReleased));
        Assert.False(sheet.IsOpen);
        Assert.False(drawer.TryHandleTriggerPointerRelease(PointerUpdateKind.RightButtonReleased));
        Assert.False(drawer.IsOpen);
        Assert.False(popover.TryHandleTriggerPointerRelease(PointerUpdateKind.MiddleButtonReleased));
        Assert.False(popover.IsOpen);

        Assert.True(dialog.TryHandleTriggerPointerRelease(PointerUpdateKind.LeftButtonReleased));
        Assert.True(dialog.IsOpen);
        Assert.True(alertDialog.TryHandleTriggerPointerRelease(PointerUpdateKind.LeftButtonReleased));
        Assert.True(alertDialog.IsOpen);
        Assert.True(commandDialog.TryHandleTriggerPointerRelease(PointerUpdateKind.LeftButtonReleased));
        Assert.True(commandDialog.IsOpen);
        Assert.True(sheet.TryHandleTriggerPointerRelease(PointerUpdateKind.LeftButtonReleased));
        Assert.True(sheet.IsOpen);
        Assert.True(drawer.TryHandleTriggerPointerRelease(PointerUpdateKind.LeftButtonReleased));
        Assert.True(drawer.IsOpen);
        Assert.True(popover.TryHandleTriggerPointerRelease(PointerUpdateKind.LeftButtonReleased));
        Assert.True(popover.IsOpen);

        dialog.IsEnabled = false;
        alertDialog.IsEnabled = false;
        commandDialog.IsEnabled = false;
        sheet.IsEnabled = false;
        drawer.IsEnabled = false;
        popover.IsEnabled = false;

        Assert.False(dialog.TryHandleTriggerPointerRelease(PointerUpdateKind.LeftButtonReleased));
        Assert.True(dialog.IsOpen);
        Assert.False(alertDialog.TryHandleTriggerPointerRelease(PointerUpdateKind.LeftButtonReleased));
        Assert.True(alertDialog.IsOpen);
        Assert.False(commandDialog.TryHandleTriggerPointerRelease(PointerUpdateKind.LeftButtonReleased));
        Assert.True(commandDialog.IsOpen);
        Assert.False(sheet.TryHandleTriggerPointerRelease(PointerUpdateKind.LeftButtonReleased));
        Assert.True(sheet.IsOpen);
        Assert.False(drawer.TryHandleTriggerPointerRelease(PointerUpdateKind.LeftButtonReleased));
        Assert.True(drawer.IsOpen);
        Assert.False(popover.TryHandleTriggerPointerRelease(PointerUpdateKind.LeftButtonReleased));
        Assert.True(popover.IsOpen);
    }

    [Fact]
    public void TooltipTriggerProviderAndDelaysMirrorWebComposition()
    {
        var changes = new List<bool>();
        var provider = new CodexTooltipProvider
        {
            DelayDuration = TimeSpan.Zero,
            SkipDelayDuration = TimeSpan.FromMilliseconds(250),
            DisableHoverableContent = true
        };
        var tooltip = new CodexTooltip
        {
            Trigger = new CodexButton { Content = "Hover" },
            Content = "Add to library",
            Placement = PlacementMode.Right,
            IsArrowVisible = true,
            CloseDelay = TimeSpan.FromMilliseconds(100)
        };
        provider.Content = tooltip;
        tooltip.OpenChanged += (_, args) => changes.Add(args.IsOpen);

        Assert.Contains("tooltip-provider", provider.Classes);
        Assert.Contains("instant-open", provider.Classes);
        Assert.Contains("skip-delay", provider.Classes);
        Assert.Contains("hoverable-disabled", provider.Classes);
        Assert.False(tooltip.IsOpen);
        Assert.Contains("closed", tooltip.Classes);
        Assert.True(tooltip.HasTrigger);
        Assert.True(tooltip.HasContent);
        Assert.True(tooltip.IsHitTestVisible);
        Assert.Contains("has-trigger", tooltip.Classes);
        Assert.DoesNotContain("instant-close", tooltip.Classes);
        Assert.Contains("delayed-close", tooltip.Classes);
        Assert.Contains("side-right", tooltip.Classes);

        Assert.True(tooltip.RequestOpen());
        Assert.True(tooltip.IsOpen);
        Assert.Equal([true], changes);
        Assert.Contains("open", tooltip.Classes);
        Assert.Contains("instant-open", tooltip.Classes);
        Assert.Contains("hoverable-disabled", tooltip.Classes);
        Assert.True(tooltip.RequestClose());
        Assert.True(tooltip.IsOpen);
        Assert.True(tooltip.TryHandleDismissKey(Key.Escape));
        Assert.False(tooltip.IsOpen);
        Assert.Equal([true, false], changes);
        Assert.Contains("closed", tooltip.Classes);

        tooltip.Open();
        Assert.True(tooltip.TryHandleDismissKey(Key.Enter));
        Assert.False(tooltip.IsOpen);
        Assert.Equal([true, false, true, false], changes);

        tooltip.Open();
        Assert.True(tooltip.TryHandleDismissKey(Key.Space));
        Assert.False(tooltip.IsOpen);
        Assert.Equal([true, false, true, false, true, false], changes);

        tooltip.OpenDelay = TimeSpan.FromSeconds(10);
        Assert.True(tooltip.RequestFocusOpen());
        Assert.True(tooltip.IsOpen);
        Assert.Equal([true, false, true, false, true, false, true], changes);

        tooltip.CloseOnEscape = false;
        Assert.False(tooltip.TryHandleDismissKey(Key.Escape));
        Assert.True(tooltip.IsOpen);

        tooltip.IsEnabled = false;
        tooltip.IsOpen = false;
        Assert.False(tooltip.RequestOpen());
        Assert.False(tooltip.IsOpen);

        var defaultTooltip = new CodexTooltip();
        Assert.False(defaultTooltip.IsOpen);
        Assert.Contains("closed", defaultTooltip.Classes);
    }

    [Fact]
    public void AlertDialogCancelAndActionMirrorWebResponsePaths()
    {
        var cancelCount = 0;
        var actionCount = 0;
        var closeCount = 0;
        var changes = new List<bool>();
        var defaultAlertDialog = new CodexAlertDialog();

        Assert.False(defaultAlertDialog.IsOpen);
        Assert.True(defaultAlertDialog.IsModal);
        Assert.False(defaultAlertDialog.DismissOnOutsidePointer);
        Assert.Contains("closed", defaultAlertDialog.Classes);
        Assert.Contains("modal", defaultAlertDialog.Classes);
        Assert.Contains("response-required", defaultAlertDialog.Classes);
        Assert.False(defaultAlertDialog.TryDismissFromOutsidePointer());

        var alertDialog = new CodexAlertDialog
        {
            Trigger = new CodexButton { Content = "Delete route" },
            IsOpen = true,
            CancelCommand = new TestCommand(() => cancelCount++),
            ActionCommand = new TestCommand(() => actionCount++),
            CloseCommand = new TestCommand(() => closeCount++),
            ActionVariant = CodexControlVariant.Destructive,
            Size = CodexControlSize.Small
        };
        alertDialog.OpenChanged += (_, args) => changes.Add(args.IsOpen);

        Assert.True(alertDialog.CanCancel());
        Assert.True(alertDialog.CanAction());
        Assert.True(alertDialog.HasTrigger);
        Assert.Contains("has-trigger", alertDialog.Classes);
        Assert.Contains("trigger-open", alertDialog.Classes);
        Assert.Contains("size-sm", alertDialog.Classes);
        Assert.Contains("action-destructive", alertDialog.Classes);
        Assert.Contains("response-required", alertDialog.Classes);
        Assert.True(alertDialog.CancelDialogCommand.CanExecute(null));
        alertDialog.CancelDialogCommand.Execute(null);
        Assert.False(alertDialog.IsOpen);
        Assert.Equal([false], changes);
        Assert.Equal(1, cancelCount);
        Assert.Equal(0, actionCount);
        Assert.Equal(1, closeCount);

        alertDialog.IsOpen = true;
        Assert.Equal([false, true], changes);
        alertDialog.CloseOnAction = false;
        Assert.True(alertDialog.Confirm());
        Assert.True(alertDialog.IsOpen);
        Assert.Equal(1, actionCount);
        Assert.Equal(1, closeCount);
        Assert.Contains("close-on-cancel", alertDialog.Classes);
        Assert.DoesNotContain("close-on-action", alertDialog.Classes);

        alertDialog.IsActionLoading = true;
        Assert.False(alertDialog.CanAction());
        Assert.False(alertDialog.ActionDialogCommand.CanExecute(null));
        Assert.False(alertDialog.Confirm());
        Assert.Equal(1, actionCount);
        Assert.Contains("loading", alertDialog.Classes);
        Assert.Contains("action-loading", alertDialog.Classes);

        alertDialog.DismissOnOutsidePointer = true;
        Assert.Contains("outside-dismissable", alertDialog.Classes);
        Assert.DoesNotContain("response-required", alertDialog.Classes);

        alertDialog.IsModal = false;
        Assert.True(alertDialog.IsModal);
        Assert.Contains("modal", alertDialog.Classes);
    }

    [Fact]
    public void AlertDialogPartCommandsMirrorHostCanExecuteChanges()
    {
        var cancelCount = 0;
        var actionCount = 0;
        var cancelCommand = new TestCommand(() => cancelCount++);
        var actionCommand = new TestCommand(() => actionCount++);
        var alertDialog = new CodexAlertDialog
        {
            IsOpen = true,
            CancelCommand = cancelCommand,
            ActionCommand = actionCommand,
            CloseOnCancel = false,
            CloseOnAction = false
        };
        var cancelCanExecuteChanges = 0;
        var actionCanExecuteChanges = 0;
        alertDialog.CancelDialogCommand.CanExecuteChanged += (_, _) => cancelCanExecuteChanges++;
        alertDialog.ActionDialogCommand.CanExecuteChanged += (_, _) => actionCanExecuteChanges++;

        Assert.True(alertDialog.CancelDialogCommand.CanExecute(null));
        Assert.True(alertDialog.ActionDialogCommand.CanExecute(null));

        cancelCommand.CanExecuteValue = false;
        cancelCommand.RaiseCanExecuteChanged();

        Assert.False(alertDialog.CanCancel());
        Assert.False(alertDialog.CancelDialogCommand.CanExecute(null));
        Assert.True(alertDialog.CanAction());
        Assert.True(alertDialog.ActionDialogCommand.CanExecute(null));
        Assert.False(alertDialog.Cancel());
        Assert.Equal(0, cancelCount);
        Assert.Equal(0, actionCount);
        Assert.True(cancelCanExecuteChanges > 0);
        Assert.True(actionCanExecuteChanges > 0);

        cancelCommand.CanExecuteValue = true;
        actionCommand.CanExecuteValue = false;
        actionCommand.RaiseCanExecuteChanged();

        Assert.True(alertDialog.CanCancel());
        Assert.True(alertDialog.CancelDialogCommand.CanExecute(null));
        Assert.False(alertDialog.CanAction());
        Assert.False(alertDialog.ActionDialogCommand.CanExecute(null));
        Assert.False(alertDialog.Confirm());
        Assert.Equal(0, cancelCount);
        Assert.Equal(0, actionCount);

        actionCommand.CanExecuteValue = true;
        actionCommand.RaiseCanExecuteChanged();

        Assert.True(alertDialog.Cancel());
        Assert.True(alertDialog.Confirm());
        Assert.Equal(1, cancelCount);
        Assert.Equal(1, actionCount);
    }

    [Fact]
    public void DismissableSurfacesRequestFocusRestoreAfterClose()
    {
        var trigger = new CodexButton { Content = "Open palette" };
        var dialog = new CodexDialog { IsOpen = true, RestoreFocusElement = trigger };
        RestoreFocusRequestedEventArgs? dialogFocus = null;
        dialog.RestoreFocusRequested += (_, args) => dialogFocus = args;

        Assert.True(dialog.HasRestoreFocusTarget);
        Assert.Contains("restore-focus", dialog.Classes);
        Assert.Contains("has-restore-focus-target", dialog.Classes);
        Assert.True(dialog.Dismiss());
        Assert.NotNull(dialogFocus);
        Assert.Same(trigger, dialogFocus.Target);
        Assert.Equal(NavigationMethod.Tab, dialogFocus.NavigationMethod);
        Assert.Equal(KeyModifiers.None, dialogFocus.KeyModifiers);

        var popoverTrigger = new CodexButton { Content = "Open popover" };
        var popover = new CodexPopover { IsOpen = true, RestoreFocusElement = popoverTrigger };
        var popoverRestored = 0;
        popover.RestoreFocusRequested += (_, args) =>
        {
            popoverRestored++;
            Assert.Same(popoverTrigger, args.Target);
        };

        Assert.True(popover.TryHandleDismissKey(Key.Escape));
        Assert.Equal(1, popoverRestored);

        var sheetTrigger = new CodexButton { Content = "Open filters" };
        var sheet = new CodexSheet { IsOpen = true, RestoreFocusElement = sheetTrigger };
        RestoreFocusRequestedEventArgs? sheetFocus = null;
        sheet.RestoreFocusRequested += (_, args) => sheetFocus = args;

        Assert.True(sheet.Dismiss());
        Assert.NotNull(sheetFocus);
        Assert.Same(sheetTrigger, sheetFocus.Target);

        var drawerTrigger = new CodexButton { Content = "Open drawer" };
        var drawer = new CodexDrawer { IsOpen = true, RestoreFocusElement = drawerTrigger };
        RestoreFocusRequestedEventArgs? drawerFocus = null;
        drawer.RestoreFocusRequested += (_, args) => drawerFocus = args;

        Assert.True(drawer.Dismiss());
        Assert.NotNull(drawerFocus);
        Assert.Same(drawerTrigger, drawerFocus.Target);

        var alertDialogTrigger = new CodexButton { Content = "Open alert" };
        var alertDialog = new CodexAlertDialog { IsOpen = true, RestoreFocusElement = alertDialogTrigger };
        RestoreFocusRequestedEventArgs? alertDialogFocus = null;
        alertDialog.RestoreFocusRequested += (_, args) => alertDialogFocus = args;

        Assert.True(alertDialog.TryHandleDismissKey(Key.Escape));
        Assert.NotNull(alertDialogFocus);
        Assert.Same(alertDialogTrigger, alertDialogFocus.Target);

        var commandTrigger = new CodexButton { Content = "Open command" };
        var commandDialog = new CodexCommandDialog { IsOpen = true, RestoreFocusElement = commandTrigger };
        RestoreFocusRequestedEventArgs? commandDialogFocus = null;
        commandDialog.RestoreFocusRequested += (_, args) => commandDialogFocus = args;

        Assert.True(commandDialog.TryCloseFromCommandItem(new CodexCommandItem { Content = "Open provider" }));
        Assert.NotNull(commandDialogFocus);
        Assert.Same(commandTrigger, commandDialogFocus.Target);

        var dropdownTrigger = new CodexButton { Content = "Actions" };
        var dropdown = new CodexDropdownButton
        {
            Content = "Actions",
            DropDownContent = new StackPanel(),
            RestoreFocusElement = dropdownTrigger
        };
        var dropdownRestoreRequests = 0;
        RestoreFocusRequestedEventArgs? dropdownFocus = null;
        dropdown.RestoreFocusRequested += (_, args) =>
        {
            dropdownRestoreRequests++;
            dropdownFocus = args;
        };

        Assert.True(dropdown.Open());
        Assert.True(dropdown.TryCloseFromDropDownAction(new CodexButton { Content = "Open" }));
        Assert.NotNull(dropdownFocus);
        Assert.Same(dropdownTrigger, dropdownFocus.Target);
        Assert.Equal(1, dropdownRestoreRequests);

        Assert.True(dropdown.Open());
        dropdown.IsOpen = false;
        Assert.Equal(2, dropdownRestoreRequests);

        var splitTrigger = new CodexButton { Content = "More" };
        var splitButton = new CodexSplitButton
        {
            Content = "Run",
            DropDownContent = new StackPanel(),
            RestoreFocusElement = splitTrigger
        };
        var splitRestoreRequests = 0;
        RestoreFocusRequestedEventArgs? splitFocus = null;
        splitButton.RestoreFocusRequested += (_, args) =>
        {
            splitRestoreRequests++;
            splitFocus = args;
        };

        Assert.True(splitButton.Open());
        Assert.True(splitButton.TryCloseFromDropDownAction(new CodexButton { Content = "Schedule" }));
        Assert.NotNull(splitFocus);
        Assert.Same(splitTrigger, splitFocus.Target);
        Assert.Equal(1, splitRestoreRequests);

        Assert.True(splitButton.Open());
        splitButton.IsOpen = false;
        Assert.Equal(2, splitRestoreRequests);

        var disabledTrigger = new CodexButton { Content = "Disabled", IsEnabled = false };
        var disabledDialog = new CodexDialog { RestoreFocusElement = disabledTrigger };
        var disabledRestoreRequests = 0;
        disabledDialog.RestoreFocusRequested += (_, _) => disabledRestoreRequests++;

        Assert.False(disabledDialog.TryRestoreFocus());
        Assert.Equal(0, disabledRestoreRequests);

        var optOutDialog = new CodexDialog
        {
            IsOpen = true,
            RestoreFocusElement = trigger,
            RestoreFocusOnDismiss = false
        };
        var optOutRestoreRequests = 0;
        optOutDialog.RestoreFocusRequested += (_, _) => optOutRestoreRequests++;

        Assert.DoesNotContain("restore-focus", optOutDialog.Classes);
        Assert.True(optOutDialog.Dismiss());
        Assert.Equal(0, optOutRestoreRequests);
    }

    [Fact]
    public void SheetTracksSideClassesAndDialogDismissContract()
    {
        var changes = new List<bool>();
        var sheet = new CodexSheet
        {
            Trigger = new CodexButton { Content = "Open filters" },
            Title = "Filters",
            Description = "Provider filters",
            Content = "Body",
            Action = "Apply"
        };
        sheet.OpenChanged += (_, args) => changes.Add(args.IsOpen);

        Assert.False(sheet.IsOpen);
        Assert.Equal(CodexSheetSide.Right, sheet.Side);
        Assert.True(sheet.HasTrigger);
        Assert.Contains("side-right", sheet.Classes);
        Assert.Contains("has-trigger", sheet.Classes);
        Assert.Contains("trigger-closed", sheet.Classes);
        Assert.Contains("closed", sheet.Classes);

        Assert.True(sheet.TryToggleFromTrigger());
        Assert.True(sheet.IsOpen);
        Assert.Equal([true], changes);
        Assert.Contains("open", sheet.Classes);
        Assert.Contains("trigger-open", sheet.Classes);

        sheet.Side = CodexSheetSide.Top;
        Assert.Contains("side-top", sheet.Classes);
        Assert.DoesNotContain("side-right", sheet.Classes);

        sheet.Side = CodexSheetSide.Bottom;
        Assert.Contains("side-bottom", sheet.Classes);
        Assert.DoesNotContain("side-top", sheet.Classes);

        sheet.Side = CodexSheetSide.Left;
        Assert.Contains("side-left", sheet.Classes);
        Assert.DoesNotContain("side-bottom", sheet.Classes);

        Assert.True(sheet.Dismiss());
        Assert.False(sheet.IsOpen);
        Assert.Equal([true, false], changes);
        Assert.Contains("closed", sheet.Classes);

        Assert.True(sheet.TryHandleTriggerKey(Key.Enter));
        Assert.True(sheet.IsOpen);
        Assert.Equal([true, false, true], changes);

        Assert.True(sheet.TryHandleTriggerKey(Key.Space));
        Assert.False(sheet.IsOpen);
        Assert.Equal([true, false, true, false], changes);
    }

    [Fact]
    public void DrawerTracksDirectionHandleDragAndDialogDismissContract()
    {
        var closed = 0;
        var changes = new List<bool>();
        var completedDrags = new List<CodexDrawerDragCompletedEventArgs>();
        var drawer = new CodexDrawer
        {
            Trigger = new CodexButton { Content = "Open drawer" },
            Title = "Usage details",
            Description = "Drag from the handle.",
            Content = "Body",
            Action = "Save",
            CloseCommand = new TestCommand(() => closed++),
            Direction = CodexDrawerDirection.Bottom,
            DragDismissThreshold = 96
        };
        drawer.OpenChanged += (_, args) => changes.Add(args.IsOpen);
        drawer.DragCompleted += (_, args) => completedDrags.Add(args);

        Assert.False(drawer.IsOpen);
        Assert.True(drawer.HasTrigger);
        Assert.Equal(CodexDrawerDirection.Bottom, drawer.Direction);
        Assert.Contains("direction-bottom", drawer.Classes);
        Assert.Contains("has-handle", drawer.Classes);
        Assert.Contains("close-on-drag", drawer.Classes);
        Assert.Contains("scale-background", drawer.Classes);
        Assert.Contains("has-trigger", drawer.Classes);
        Assert.Contains("trigger-closed", drawer.Classes);
        Assert.Contains("closed", drawer.Classes);

        Assert.True(drawer.TryToggleFromTrigger());
        Assert.True(drawer.IsOpen);
        Assert.Equal([true], changes);
        Assert.Contains("open", drawer.Classes);
        Assert.Contains("trigger-open", drawer.Classes);

        drawer.Direction = CodexDrawerDirection.Top;
        Assert.Contains("direction-top", drawer.Classes);
        Assert.DoesNotContain("direction-bottom", drawer.Classes);

        drawer.Direction = CodexDrawerDirection.Right;
        Assert.Contains("direction-right", drawer.Classes);
        Assert.DoesNotContain("direction-top", drawer.Classes);

        drawer.Direction = CodexDrawerDirection.Left;
        Assert.Contains("direction-left", drawer.Classes);
        Assert.DoesNotContain("direction-right", drawer.Classes);

        Assert.True(drawer.BeginDrag());
        Assert.True(drawer.IsDragging);
        Assert.Contains("dragging", drawer.Classes);
        Assert.True(drawer.DragBy(72));
        Assert.Equal(72, drawer.DragOffset);
        Assert.False(drawer.IsDragDismissReady);
        Assert.DoesNotContain("drag-dismiss-ready", drawer.Classes);
        Assert.False(drawer.CompleteDrag());
        Assert.True(drawer.IsOpen);
        Assert.False(drawer.IsDragging);
        Assert.Equal(0, drawer.DragOffset);
        Assert.Single(completedDrags);
        Assert.False(completedDrags[0].Dismissed);
        Assert.Equal(72, completedDrags[0].DragOffset);
        Assert.Equal(0, closed);

        Assert.True(drawer.BeginDrag());
        Assert.True(drawer.DragBy(128));
        Assert.True(drawer.IsDragDismissReady);
        Assert.Contains("drag-dismiss-ready", drawer.Classes);
        Assert.True(drawer.CompleteDrag());
        Assert.False(drawer.IsOpen);
        Assert.Equal(1, closed);
        Assert.Equal([true, false], changes);
        Assert.Equal(2, completedDrags.Count);
        Assert.True(completedDrags[1].Dismissed);
        Assert.Equal(128, completedDrags[1].DragOffset);

        Assert.True(drawer.TryHandleTriggerKey(Key.Enter));
        Assert.True(drawer.IsOpen);
        Assert.Equal([true, false, true], changes);

        Assert.True(drawer.TryHandleTriggerKey(Key.Space));
        Assert.False(drawer.IsOpen);
        Assert.Equal([true, false, true, false], changes);
        Assert.Equal(2, closed);

        drawer.IsOpen = true;
        drawer.CloseOnDragDismiss = false;
        Assert.DoesNotContain("close-on-drag", drawer.Classes);
        Assert.True(drawer.BeginDrag());
        Assert.True(drawer.DragBy(160));
        Assert.True(drawer.IsDragDismissReady);
        Assert.False(drawer.CompleteDrag());
        Assert.True(drawer.IsOpen);
        Assert.Equal(2, closed);
        Assert.Equal([true, false, true, false, true], changes);
        Assert.Equal(3, completedDrags.Count);
        Assert.False(completedDrags[2].Dismissed);

        drawer.IsHandleVisible = false;
        Assert.DoesNotContain("has-handle", drawer.Classes);
        Assert.False(drawer.BeginDrag());
    }

    [Fact]
    public void DrawerHandleDragPointerContractUsesPrimaryButtonOnly()
    {
        var closed = 0;
        var completedDrags = new List<CodexDrawerDragCompletedEventArgs>();
        var drawer = new CodexDrawer
        {
            IsOpen = true,
            DragDismissThreshold = 32,
            CloseCommand = new TestCommand(() => closed++)
        };
        drawer.DragCompleted += (_, args) => completedDrags.Add(args);

        Assert.False(drawer.TryBeginHandleDrag(PointerUpdateKind.RightButtonPressed, new Point(0, 0)));
        Assert.False(drawer.TryBeginHandleDrag(PointerUpdateKind.MiddleButtonPressed, new Point(0, 0)));
        Assert.False(drawer.IsDragging);

        Assert.True(drawer.TryBeginHandleDrag(PointerUpdateKind.LeftButtonPressed, new Point(0, 0)));
        Assert.True(drawer.IsDragging);
        Assert.True(drawer.DragBy(64));
        Assert.True(drawer.IsDragDismissReady);

        Assert.False(drawer.TryCompleteHandleDrag(PointerUpdateKind.RightButtonReleased));
        Assert.True(drawer.IsOpen);
        Assert.True(drawer.IsDragging);
        Assert.Equal(64, drawer.DragOffset);
        Assert.Empty(completedDrags);
        Assert.Equal(0, closed);

        Assert.False(drawer.TryCompleteHandleDrag(PointerUpdateKind.MiddleButtonReleased));
        Assert.True(drawer.IsOpen);
        Assert.True(drawer.IsDragging);
        Assert.Equal(64, drawer.DragOffset);
        Assert.Empty(completedDrags);
        Assert.Equal(0, closed);

        Assert.True(drawer.TryCompleteHandleDrag(PointerUpdateKind.LeftButtonReleased));
        Assert.False(drawer.IsOpen);
        Assert.False(drawer.IsDragging);
        Assert.Single(completedDrags);
        Assert.True(completedDrags[0].Dismissed);
        Assert.Equal(64, completedDrags[0].DragOffset);
        Assert.Equal(1, closed);
        Assert.False(drawer.TryCompleteHandleDrag(PointerUpdateKind.LeftButtonReleased));

        drawer.IsOpen = true;
        drawer.IsEnabled = false;
        Assert.False(drawer.TryBeginHandleDrag(PointerUpdateKind.LeftButtonPressed, new Point(0, 0)));

        drawer.IsEnabled = true;
        drawer.IsHandleVisible = false;
        Assert.False(drawer.TryBeginHandleDrag(PointerUpdateKind.LeftButtonPressed, new Point(0, 0)));

        drawer.IsHandleVisible = true;
        drawer.IsOpen = false;
        Assert.False(drawer.TryBeginHandleDrag(PointerUpdateKind.LeftButtonPressed, new Point(0, 0)));
    }

    [Fact]
    public void HoverCardUsesRadixOpenCloseDelaysAndDismissSemantics()
    {
        var changes = new List<bool>();
        var hoverCard = new CodexHoverCard
        {
            Trigger = "Provider",
            Content = "Preview content",
            OpenDelay = TimeSpan.Zero,
            CloseDelay = TimeSpan.Zero,
            Placement = PlacementMode.Left,
            Align = CodexHoverCardAlign.End
        };
        hoverCard.OpenChanged += (_, args) => changes.Add(args.IsOpen);

        Assert.False(hoverCard.IsOpen);
        Assert.Contains("closed", hoverCard.Classes);
        Assert.Contains("has-trigger", hoverCard.Classes);
        Assert.Contains("has-content", hoverCard.Classes);
        Assert.Contains("instant-open", hoverCard.Classes);
        Assert.Contains("instant-close", hoverCard.Classes);
        Assert.True(hoverCard.RequestOpen());
        Assert.True(hoverCard.IsOpen);
        Assert.Equal([true], changes);
        Assert.Contains("open", hoverCard.Classes);
        Assert.Contains("side-left", hoverCard.Classes);
        Assert.Contains("align-end", hoverCard.Classes);

        Assert.True(hoverCard.RequestClose());
        Assert.False(hoverCard.IsOpen);
        Assert.Equal([true, false], changes);
        Assert.Contains("closed", hoverCard.Classes);

        hoverCard.Open();
        hoverCard.CloseOnEscape = false;
        Assert.False(hoverCard.TryHandleDismissKey(Key.Escape));
        Assert.True(hoverCard.IsOpen);
        Assert.Equal([true, false, true], changes);

        hoverCard.CloseOnEscape = true;
        Assert.True(hoverCard.TryHandleDismissKey(Key.Escape));
        Assert.False(hoverCard.IsOpen);
        Assert.Equal([true, false, true, false], changes);

        hoverCard.IsEnabled = false;
        Assert.False(hoverCard.RequestOpen());
        Assert.False(hoverCard.IsOpen);
    }

    [Fact]
    public void PopupAlignResolvesToWebSideAlignedPlacement()
    {
        var cases = new[]
        {
            (PlacementMode.Bottom, PlacementMode.Bottom, PlacementMode.BottomEdgeAlignedLeft, PlacementMode.BottomEdgeAlignedRight),
            (PlacementMode.Top, PlacementMode.Top, PlacementMode.TopEdgeAlignedLeft, PlacementMode.TopEdgeAlignedRight),
            (PlacementMode.Left, PlacementMode.Left, PlacementMode.LeftEdgeAlignedTop, PlacementMode.LeftEdgeAlignedBottom),
            (PlacementMode.Right, PlacementMode.Right, PlacementMode.RightEdgeAlignedTop, PlacementMode.RightEdgeAlignedBottom)
        };

        foreach (var (placement, center, start, end) in cases)
        {
            AssertPopoverPlacement(placement, CodexPopoverAlign.Center, center);
            AssertPopoverPlacement(placement, CodexPopoverAlign.Start, start);
            AssertPopoverPlacement(placement, CodexPopoverAlign.End, end);
            AssertHoverCardPlacement(placement, CodexHoverCardAlign.Center, center);
            AssertHoverCardPlacement(placement, CodexHoverCardAlign.Start, start);
            AssertHoverCardPlacement(placement, CodexHoverCardAlign.End, end);
            AssertDropdownPlacement(placement, CodexDropdownAlign.Center, center);
            AssertDropdownPlacement(placement, CodexDropdownAlign.Start, start);
            AssertDropdownPlacement(placement, CodexDropdownAlign.End, end);
            AssertSplitPlacement(placement, CodexDropdownAlign.Center, center);
            AssertSplitPlacement(placement, CodexDropdownAlign.Start, start);
            AssertSplitPlacement(placement, CodexDropdownAlign.End, end);
        }

        static void AssertPopoverPlacement(PlacementMode placement, CodexPopoverAlign align, PlacementMode expected)
        {
            var popover = new CodexPopover { Placement = placement, Align = align };
            Assert.Equal(expected, popover.EffectivePlacement);
        }

        static void AssertHoverCardPlacement(PlacementMode placement, CodexHoverCardAlign align, PlacementMode expected)
        {
            var hoverCard = new CodexHoverCard { Placement = placement, Align = align };
            Assert.Equal(expected, hoverCard.EffectivePlacement);
        }

        static void AssertDropdownPlacement(PlacementMode placement, CodexDropdownAlign align, PlacementMode expected)
        {
            var dropdown = new CodexDropdownButton
            {
                DropDownContent = new StackPanel(),
                Placement = placement,
                Align = align
            };
            Assert.Equal(expected, dropdown.EffectivePlacement);
        }

        static void AssertSplitPlacement(PlacementMode placement, CodexDropdownAlign align, PlacementMode expected)
        {
            var splitButton = new CodexSplitButton
            {
                DropDownContent = new StackPanel(),
                Placement = placement,
                Align = align
            };
            Assert.Equal(expected, splitButton.EffectivePlacement);
        }
    }

    [Fact]
    public void TooltipHoverCardAndPopoverUsePopupSideOffsetPlacement()
    {
        var root = FindRepositoryRoot();
        var tooltipStyle = File.ReadAllText(Path.Combine(root, "src", "CodexSwitchUI", "Themes", "Controls", "Tooltip.axaml"));
        var hoverCardStyle = File.ReadAllText(Path.Combine(root, "src", "CodexSwitchUI", "Themes", "Controls", "HoverCard.axaml"));
        var popoverStyle = File.ReadAllText(Path.Combine(root, "src", "CodexSwitchUI", "Themes", "Controls", "Popover.axaml"));
        var dropdownButtonStyle = File.ReadAllText(Path.Combine(root, "src", "CodexSwitchUI", "Themes", "Controls", "DropdownButton.axaml"));
        var splitButtonStyle = File.ReadAllText(Path.Combine(root, "src", "CodexSwitchUI", "Themes", "Controls", "SplitButton.axaml"));
        var hoverCardSource = File.ReadAllText(Path.Combine(root, "src", "CodexSwitchUI", "Controls", "CodexHoverCard.cs"));

        Assert.Contains("<Popup x:Name=\"PART_Popup\"", tooltipStyle);
        Assert.Contains("PlacementTarget=\"PART_Trigger\"", tooltipStyle);
        Assert.Contains("Placement=\"{TemplateBinding Placement}\"", tooltipStyle);
        Assert.Contains("controls|CodexTooltip.has-trigger.side-bottom /template/ Popup#PART_Popup Border#PART_Surface", tooltipStyle);
        Assert.Contains("<Setter Property=\"Margin\" Value=\"0,4,0,0\" />", tooltipStyle);
        Assert.Contains("controls|CodexTooltip.has-trigger.side-right /template/ Popup#PART_Popup Border#PART_Surface", tooltipStyle);
        Assert.Contains("<Setter Property=\"Margin\" Value=\"4,0,0,0\" />", tooltipStyle);
        Assert.DoesNotContain("-188", tooltipStyle);
        Assert.DoesNotContain("0,36,0,0", tooltipStyle);
        Assert.DoesNotContain("0,-36,0,0", tooltipStyle);

        Assert.Contains("<Popup x:Name=\"PART_Popup\"", hoverCardStyle);
        Assert.Contains("PlacementTarget=\"PART_Trigger\"", hoverCardStyle);
        Assert.Contains("Placement=\"{TemplateBinding EffectivePlacement}\"", hoverCardStyle);
        Assert.DoesNotContain("Placement=\"{TemplateBinding Placement}\"", hoverCardStyle);
        Assert.Contains("controls|CodexHoverCard.side-bottom /template/ Popup#PART_Popup Border#PART_Surface", hoverCardStyle);
        Assert.Contains("<Setter Property=\"Margin\" Value=\"0,4,0,0\" />", hoverCardStyle);
        Assert.Contains("controls|CodexHoverCard.side-right /template/ Popup#PART_Popup Border#PART_Surface", hoverCardStyle);
        Assert.Contains("<Setter Property=\"Margin\" Value=\"4,0,0,0\" />", hoverCardStyle);
        Assert.DoesNotContain("-248", hoverCardStyle);
        Assert.DoesNotContain("-112", hoverCardStyle);
        Assert.DoesNotContain("0,38,0,0", hoverCardStyle);
        Assert.DoesNotContain("-14", hoverCardStyle);
        Assert.DoesNotContain("0,32,0,0", hoverCardStyle);

        Assert.Contains("<Popup x:Name=\"PART_Popup\"", popoverStyle);
        Assert.Contains("PlacementTarget=\"PART_Trigger\"", popoverStyle);
        Assert.Contains("Placement=\"{TemplateBinding EffectivePlacement}\"", popoverStyle);
        Assert.DoesNotContain("Placement=\"{TemplateBinding Placement}\"", popoverStyle);
        Assert.Contains("controls|CodexPopover.has-trigger.side-bottom /template/ Popup#PART_Popup Border#PART_Surface", popoverStyle);
        Assert.Contains("<Setter Property=\"Margin\" Value=\"0,4,0,0\" />", popoverStyle);
        Assert.Contains("controls|CodexPopover.has-trigger.side-right /template/ Popup#PART_Popup Border#PART_Surface", popoverStyle);
        Assert.Contains("<Setter Property=\"Margin\" Value=\"4,0,0,0\" />", popoverStyle);
        Assert.DoesNotContain("RowDefinitions=\"Auto,Auto\"", popoverStyle);
        Assert.DoesNotContain("Margin\" Value=\"0,8,0,0\"", popoverStyle);

        Assert.Contains("e.NameScope.Find<Border>(\"PART_Surface\")", hoverCardSource);
        Assert.Contains("PointerEntered += OnSurfacePointerEntered", hoverCardSource);
        Assert.Contains("PointerExited += OnSurfacePointerExited", hoverCardSource);

        Assert.Contains("Placement=\"{TemplateBinding EffectivePlacement}\"", dropdownButtonStyle);
        Assert.DoesNotContain("Placement=\"{TemplateBinding Placement}\"", dropdownButtonStyle);
        Assert.Contains("Placement=\"{TemplateBinding EffectivePlacement}\"", splitButtonStyle);
        Assert.DoesNotContain("Placement=\"{TemplateBinding Placement}\"", splitButtonStyle);
    }

    [Fact]
    public void DropdownButtonMirrorsDropdownMenuOpenDismissAndSelectSemantics()
    {
        var changes = new List<(bool IsOpen, CodexDropdownButtonOpenChangeSource Source)>();
        var dropdown = new CodexDropdownButton
        {
            Content = "Actions",
            DropDownContent = new StackPanel(),
            Placement = PlacementMode.Right,
            Align = CodexDropdownAlign.Start,
            IsArrowVisible = true
        };
        var enabledAction = new CodexButton { Content = "Open" };
        var disabledAction = new CodexButton { Content = "Disabled", IsEnabled = false };
        dropdown.OpenChanged += (_, args) => changes.Add((args.IsOpen, args.Source));

        Assert.False(dropdown.IsOpen);
        Assert.False(dropdown.TryHandleTriggerPointerRelease(PointerUpdateKind.RightButtonReleased));
        Assert.False(dropdown.TryHandleTriggerPointerRelease(PointerUpdateKind.MiddleButtonReleased));
        Assert.False(dropdown.IsOpen);
        Assert.Empty(changes);
        Assert.True(dropdown.TryHandleTriggerPointerRelease(PointerUpdateKind.LeftButtonReleased));
        Assert.True(dropdown.IsOpen);
        Assert.Equal([(true, CodexDropdownButtonOpenChangeSource.Pointer)], changes);
        Assert.Contains("open", dropdown.Classes);
        Assert.Contains("side-right", dropdown.Classes);
        Assert.Contains("align-start", dropdown.Classes);

        Assert.False(dropdown.TryCloseFromDropDownAction(disabledAction));
        Assert.True(dropdown.IsOpen);
        Assert.Equal([(true, CodexDropdownButtonOpenChangeSource.Pointer)], changes);

        Assert.True(dropdown.TryCloseFromDropDownAction(enabledAction));
        Assert.False(dropdown.IsOpen);
        Assert.Equal(
            [
                (true, CodexDropdownButtonOpenChangeSource.Pointer),
                (false, CodexDropdownButtonOpenChangeSource.Selection)
            ],
            changes);
        Assert.Contains("closed", dropdown.Classes);

        dropdown.Open();
        Assert.Equal((true, CodexDropdownButtonOpenChangeSource.Programmatic), changes[^1]);
        dropdown.CloseOnEscape = false;
        Assert.False(dropdown.TryHandleDismissKey(Key.Escape));
        Assert.True(dropdown.IsOpen);
        Assert.Equal((true, CodexDropdownButtonOpenChangeSource.Programmatic), changes[^1]);

        dropdown.CloseOnEscape = true;
        Assert.True(dropdown.TryHandleDismissKey(Key.Escape));
        Assert.False(dropdown.IsOpen);
        Assert.Equal((false, CodexDropdownButtonOpenChangeSource.Keyboard), changes[^1]);

        dropdown.IsLoading = true;
        Assert.False(dropdown.Open());
        Assert.False(dropdown.TryHandleTriggerPointerRelease(PointerUpdateKind.LeftButtonReleased));
        Assert.False(dropdown.TryHandleTriggerKey(Key.Enter));
        Assert.False(dropdown.TryHandleTriggerKey(Key.Down));
        Assert.False(dropdown.IsOpen);
        Assert.Equal((false, CodexDropdownButtonOpenChangeSource.Keyboard), changes[^1]);

        dropdown.IsLoading = false;
        dropdown.DropDownContent = null;
        Assert.False(dropdown.Open());
        Assert.False(dropdown.TryHandleTriggerPointerRelease(PointerUpdateKind.LeftButtonReleased));
        Assert.False(dropdown.TryHandleTriggerKey(Key.Down));
    }

    [Fact]
    public void SplitButtonSeparatesPrimaryActionFromDropdownSemantics()
    {
        var changes = new List<(bool IsOpen, CodexSplitButtonOpenChangeSource Source)>();
        var primaryClicks = 0;
        var commandExecutions = 0;
        var command = new TestCommand(() => commandExecutions++);
        var splitButton = new CodexSplitButton
        {
            Content = "Run",
            Command = command,
            CommandParameter = "primary",
            DropDownContent = new StackPanel(),
            Placement = PlacementMode.Top,
            Align = CodexDropdownAlign.End,
            IsArrowVisible = true
        };
        var enabledAction = new CodexButton { Content = "Open" };
        var disabledAction = new CodexButton { Content = "Disabled", IsEnabled = false };
        splitButton.OpenChanged += (_, args) => changes.Add((args.IsOpen, args.Source));
        splitButton.Click += (_, _) => primaryClicks++;

        Assert.False(splitButton.IsOpen);
        Assert.True(splitButton.CanOpenDropDown);
        Assert.True(splitButton.TryExecutePrimaryAction());
        Assert.Equal(1, primaryClicks);
        Assert.Equal(1, commandExecutions);
        Assert.False(splitButton.IsOpen);
        Assert.Contains("has-command", splitButton.Classes);

        Assert.False(splitButton.TryHandleMenuTriggerPointerRelease(PointerUpdateKind.RightButtonReleased));
        Assert.False(splitButton.TryHandleMenuTriggerPointerRelease(PointerUpdateKind.MiddleButtonReleased));
        Assert.False(splitButton.IsOpen);
        Assert.Empty(changes);
        Assert.True(splitButton.TryHandleMenuTriggerPointerRelease(PointerUpdateKind.LeftButtonReleased));
        Assert.True(splitButton.IsOpen);
        Assert.Equal([(true, CodexSplitButtonOpenChangeSource.Pointer)], changes);
        Assert.Contains("open", splitButton.Classes);
        Assert.Contains("side-top", splitButton.Classes);
        Assert.Contains("align-end", splitButton.Classes);

        Assert.False(splitButton.TryCloseFromDropDownAction(disabledAction));
        Assert.True(splitButton.IsOpen);
        Assert.Equal([(true, CodexSplitButtonOpenChangeSource.Pointer)], changes);

        Assert.True(splitButton.TryCloseFromDropDownAction(enabledAction));
        Assert.False(splitButton.IsOpen);
        Assert.Equal(
            [
                (true, CodexSplitButtonOpenChangeSource.Pointer),
                (false, CodexSplitButtonOpenChangeSource.Selection)
            ],
            changes);
        Assert.Contains("closed", splitButton.Classes);

        splitButton.Open();
        Assert.Equal((true, CodexSplitButtonOpenChangeSource.Programmatic), changes[^1]);
        splitButton.CloseOnEscape = false;
        Assert.False(splitButton.TryHandleDismissKey(Key.Escape));
        Assert.True(splitButton.IsOpen);
        Assert.Equal((true, CodexSplitButtonOpenChangeSource.Programmatic), changes[^1]);

        splitButton.CloseOnEscape = true;
        Assert.True(splitButton.TryHandleDismissKey(Key.Escape));
        Assert.False(splitButton.IsOpen);
        Assert.Equal((false, CodexSplitButtonOpenChangeSource.Keyboard), changes[^1]);

        splitButton.IsLoading = true;
        Assert.False(splitButton.TryExecutePrimaryAction());
        Assert.False(splitButton.Open());
        Assert.False(splitButton.TryHandleMenuTriggerPointerRelease(PointerUpdateKind.LeftButtonReleased));
        Assert.False(splitButton.TryHandleMenuTriggerKey(Key.Enter));
        Assert.False(splitButton.TryHandleMenuTriggerKey(Key.Down));
        Assert.Contains("loading", splitButton.Classes);
        Assert.Equal((false, CodexSplitButtonOpenChangeSource.Keyboard), changes[^1]);
        Assert.Equal(1, commandExecutions);

        splitButton.IsLoading = false;
        command.CanExecuteValue = false;
        command.RaiseCanExecuteChanged();
        Assert.False(splitButton.IsPrimaryActionAvailable);
        Assert.Contains("primary-action-disabled", splitButton.Classes);
        Assert.False(splitButton.TryExecutePrimaryAction());
    }

    [Fact]
    public void DropdownAndSplitButtonCloseMenuLeafItemsThroughWebSelectSemantics()
    {
        var dropdownChanges = new List<(bool IsOpen, CodexDropdownButtonOpenChangeSource Source)>();
        var splitChanges = new List<(bool IsOpen, CodexSplitButtonOpenChangeSource Source)>();
        var dropdown = new CodexDropdownButton
        {
            Content = "Actions",
            DropDownContent = new CodexMenu()
        };
        var splitButton = new CodexSplitButton
        {
            Content = "Run",
            DropDownContent = new CodexMenu()
        };
        var dropdownSubmenu = new CodexMenuItem { Header = "Export" };
        var splitSubmenu = new CodexMenuItem { Header = "Queue" };
        var dropdownLeaf = new CodexMenuItem { Header = "Archive" };
        var splitLeaf = new CodexMenuItem { Header = "Schedule" };
        dropdownSubmenu.Items.Add(new CodexMenuItem { Header = "JSON" });
        splitSubmenu.Items.Add(new CodexMenuItem { Header = "Later" });
        dropdown.OpenChanged += (_, args) => dropdownChanges.Add((args.IsOpen, args.Source));
        splitButton.OpenChanged += (_, args) => splitChanges.Add((args.IsOpen, args.Source));

        Assert.True(dropdown.Open());
        Assert.False(dropdown.TryCloseFromDropDownMenuItem(dropdownSubmenu));
        Assert.True(dropdown.IsOpen);
        Assert.True(dropdown.TryCloseFromDropDownMenuItem(dropdownLeaf));
        Assert.False(dropdown.IsOpen);
        Assert.Equal((false, CodexDropdownButtonOpenChangeSource.Selection), dropdownChanges[^1]);

        Assert.True(splitButton.Open());
        Assert.False(splitButton.TryCloseFromDropDownMenuItem(splitSubmenu));
        Assert.True(splitButton.IsOpen);
        Assert.True(splitButton.TryCloseFromDropDownMenuItem(splitLeaf));
        Assert.False(splitButton.IsOpen);
        Assert.Equal((false, CodexSplitButtonOpenChangeSource.Selection), splitChanges[^1]);

        dropdown.CloseOnItemSelected = false;
        Assert.True(dropdown.Open());
        Assert.False(dropdown.TryCloseFromDropDownMenuItem(dropdownLeaf));
        Assert.True(dropdown.IsOpen);

        splitButton.CloseOnItemSelected = false;
        Assert.True(splitButton.Open());
        Assert.False(splitButton.TryCloseFromDropDownMenuItem(splitLeaf));
        Assert.True(splitButton.IsOpen);
    }

    [Fact]
    public void OverlayPrimitiveDismissesFromEscapeAndOutsidePointer()
    {
        var dismissed = 0;
        var overlay = new CodexOverlay
        {
            IsOpen = true,
            DismissCommand = new TestCommand(() => dismissed++)
        };
        var inside = new Border();

        Assert.True(overlay.TryHandleDismissKey(Key.Escape));
        Assert.False(overlay.IsOpen);
        Assert.Equal(1, dismissed);
        Assert.Contains("is-closed", overlay.Classes);

        overlay.IsOpen = true;
        overlay.CloseOnEscape = false;
        Assert.False(overlay.TryHandleDismissKey(Key.Escape));
        Assert.True(overlay.IsOpen);

        overlay.DismissOnOutsidePointer = false;
        Assert.False(overlay.TryDismissFromOutsidePointer(inside));
        Assert.True(overlay.IsOpen);

        overlay.DismissOnOutsidePointer = true;
        Assert.True(overlay.TryDismissFromOutsidePointer(inside));
        Assert.False(overlay.IsOpen);
        Assert.Equal(2, dismissed);
    }

    [Fact]
    public void CommandDialogMirrorsCommandAndDialogCloseSemantics()
    {
        var closed = 0;
        var changes = new List<bool>();
        var dialog = new CodexCommandDialog
        {
            Trigger = new CodexButton { Content = "Open command menu" },
            Placeholder = "Search commands...",
            CloseCommand = new TestCommand(() => closed++)
        };
        dialog.OpenChanged += (_, args) => changes.Add(args.IsOpen);
        var enabledItem = new CodexCommandItem { Content = "Open provider" };
        var disabledItem = new CodexCommandItem { Content = "Disabled action", IsEnabled = false };

        Assert.False(dialog.IsOpen);
        Assert.True(dialog.HasTrigger);
        Assert.False(dialog.IsCloseVisible);
        Assert.True(dialog.CloseOnItemSelected);
        Assert.Contains("close-on-select", dialog.Classes);
        Assert.Contains("has-trigger", dialog.Classes);
        Assert.Contains("trigger-closed", dialog.Classes);
        Assert.Contains("closed", dialog.Classes);
        Assert.DoesNotContain("loading", dialog.Classes);

        Assert.True(dialog.TryToggleFromTrigger());
        Assert.True(dialog.IsOpen);
        Assert.Equal([true], changes);
        Assert.Contains("open", dialog.Classes);
        Assert.Contains("trigger-open", dialog.Classes);

        Assert.True(dialog.TryHandleTriggerKey(Key.Enter));
        Assert.False(dialog.IsOpen);
        Assert.Equal(1, closed);
        Assert.Equal([true, false], changes);

        Assert.True(dialog.TryHandleTriggerKey(Key.Space));
        Assert.True(dialog.IsOpen);
        Assert.Equal([true, false, true], changes);

        dialog.SearchText = "provider";
        dialog.ShouldFilter = false;
        dialog.LoopNavigation = true;
        Assert.Equal("provider", dialog.SearchText);
        Assert.False(dialog.ShouldFilter);
        Assert.Contains("loop", dialog.Classes);

        Assert.False(dialog.TryCloseFromCommandItem(disabledItem));
        Assert.True(dialog.IsOpen);
        Assert.Equal(1, closed);

        Assert.True(dialog.TryCloseFromCommandItem(enabledItem));
        Assert.False(dialog.IsOpen);
        Assert.Contains("closed", dialog.Classes);
        Assert.Equal(2, closed);
        Assert.Equal([true, false, true, false], changes);

        dialog.IsOpen = true;
        dialog.IsLoading = true;
        Assert.Contains("loading", dialog.Classes);
        Assert.False(dialog.TryCloseFromCommandItem(enabledItem));
        Assert.True(dialog.IsOpen);

        dialog.IsLoading = false;
        dialog.CloseOnItemSelected = false;
        Assert.DoesNotContain("close-on-select", dialog.Classes);
        Assert.False(dialog.TryCloseFromCommandItem(enabledItem));
        Assert.True(dialog.IsOpen);

        Assert.True(dialog.TryHandleDismissKey(Key.Escape));
        Assert.False(dialog.IsOpen);
        Assert.Equal(3, closed);
    }

    [Fact]
    public async Task CommandDialogForwardsCommandItemSelectionSourceMetadata()
    {
        await using var session = HeadlessUnitTestSession.StartNew(typeof(OverlayRenderedLifecycleTestApp), AvaloniaTestIsolationLevel.PerTest);
        await session.Dispatch(() =>
        {
            EnsureCodexTheme();

            var selectedValues = new List<string?>();
            var selectedSources = new List<CodexCommandItemSelectSource>();
            var pointer = new CodexCommandItem { Content = "Pointer", Value = "pointer" };
            var keyboard = new CodexCommandItem { Content = "Keyboard", Value = "keyboard" };
            var programmatic = new CodexCommandItem { Content = "Programmatic", Value = "programmatic" };
            var disabled = new CodexCommandItem { Content = "Disabled", Value = "disabled", IsEnabled = false };
            var dialog = new CodexCommandDialog
            {
                IsOpen = true,
                CloseOnItemSelected = false,
                Content = new CodexCommandList
                {
                    Items =
                    {
                        pointer,
                        keyboard,
                        programmatic,
                        disabled
                    }
                }
            };
            dialog.ItemSelected += (_, args) =>
            {
                selectedValues.Add(args.Value);
                selectedSources.Add(args.Source);
            };
            var window = ShowWindow(dialog);

            try
            {
                Assert.False(pointer.TryHandlePointerActivation(PointerUpdateKind.RightButtonReleased));
                Assert.False(pointer.TryHandlePointerActivation(PointerUpdateKind.MiddleButtonReleased));
                Assert.Empty(selectedSources);

                Assert.True(pointer.TryHandlePointerActivation(PointerUpdateKind.LeftButtonReleased));
                InvokeButtonClick(keyboard);
                Assert.True(programmatic.TrySelect());
                Assert.False(disabled.TrySelect());

                Assert.Equal(["pointer", "keyboard", "programmatic"], selectedValues);
                Assert.Equal(
                    [
                        CodexCommandItemSelectSource.Pointer,
                        CodexCommandItemSelectSource.Keyboard,
                        CodexCommandItemSelectSource.Programmatic
                    ],
                    selectedSources);
                Assert.True(dialog.IsOpen);
            }
            finally
            {
                window.Close();
            }
        }, CancellationToken.None);
    }

    [Fact]
    public void EmptyStateActionsRespectLoadingDisabledAndCommandCanExecute()
    {
        var actionExecutions = 0;
        var secondaryExecutions = 0;
        var actionEvents = 0;
        var secondaryEvents = 0;
        var actionCommand = new TestCommand(() => actionExecutions++);
        var secondaryCommand = new TestCommand(() => secondaryExecutions++);
        var emptyState = new CodexEmptyState
        {
            Title = "No sessions",
            Description = "Create a session to start.",
            Action = "Create session",
            SecondaryAction = "Import sessions",
            ActionCommand = actionCommand,
            SecondaryActionCommand = secondaryCommand
        };
        emptyState.ActionRequested += (_, _) => actionEvents++;
        emptyState.SecondaryActionRequested += (_, _) => secondaryEvents++;

        Assert.True(emptyState.TryExecuteAction());
        Assert.True(emptyState.TryExecuteSecondaryAction());
        Assert.Equal(1, actionExecutions);
        Assert.Equal(1, secondaryExecutions);
        Assert.Equal(1, actionEvents);
        Assert.Equal(1, secondaryEvents);

        actionCommand.CanExecuteValue = false;
        actionCommand.RaiseCanExecuteChanged();
        Assert.False(emptyState.CanExecuteAction);
        Assert.Contains("has-action", emptyState.Classes);
        Assert.DoesNotContain("can-action", emptyState.Classes);
        Assert.Contains("action-command-blocked", emptyState.Classes);
        Assert.DoesNotContain("secondary-action-command-blocked", emptyState.Classes);
        Assert.Contains("command-blocked", emptyState.Classes);
        Assert.False(emptyState.TryExecuteAction());
        Assert.Equal(1, actionExecutions);

        actionCommand.CanExecuteValue = true;
        actionCommand.RaiseCanExecuteChanged();
        Assert.DoesNotContain("action-command-blocked", emptyState.Classes);
        Assert.DoesNotContain("command-blocked", emptyState.Classes);

        secondaryCommand.CanExecuteValue = false;
        secondaryCommand.RaiseCanExecuteChanged();
        Assert.False(emptyState.CanExecuteSecondaryAction);
        Assert.Contains("secondary-action-command-blocked", emptyState.Classes);
        Assert.Contains("command-blocked", emptyState.Classes);
        Assert.False(emptyState.TryExecuteSecondaryAction());
        Assert.Equal(1, secondaryExecutions);

        secondaryCommand.CanExecuteValue = true;
        secondaryCommand.RaiseCanExecuteChanged();
        emptyState.IsLoading = true;
        Assert.False(emptyState.CanExecuteAction);
        Assert.False(emptyState.CanExecuteSecondaryAction);
        Assert.Contains("loading", emptyState.Classes);
        Assert.DoesNotContain("command-blocked", emptyState.Classes);
        Assert.False(emptyState.TryExecuteAction());
        Assert.False(emptyState.TryExecuteSecondaryAction());

        emptyState.IsLoading = false;
        emptyState.IsEnabled = false;
        Assert.False(emptyState.CanExecuteAction);
        Assert.False(emptyState.TryExecuteAction());

        var secondaryOnly = new CodexEmptyState
        {
            Description = "Only a quiet secondary path is available.",
            SecondaryAction = "Learn more"
        };

        Assert.False(secondaryOnly.HasTitle);
        Assert.True(secondaryOnly.HasHeader);
        Assert.False(secondaryOnly.HasAction);
        Assert.True(secondaryOnly.HasSecondaryAction);
        Assert.True(secondaryOnly.HasActions);
        Assert.DoesNotContain("has-action", secondaryOnly.Classes);
        Assert.Contains("has-actions", secondaryOnly.Classes);
        Assert.True(secondaryOnly.TryExecuteSecondaryAction());
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
        var avatarGroup = new CodexAvatarGroup
        {
            Size = CodexControlSize.Small,
            Overlap = 12,
            Children =
            {
                new CodexAvatar { Fallback = "CN" },
                new CodexAvatar { Fallback = "LR" },
                new CodexAvatarGroupCount { Count = 3 }
            }
        };
        avatarGroup.Measure(new Avalonia.Size(240, 80));
        var avatarGroupCount = avatarGroup.Children.OfType<CodexAvatarGroupCount>().Single();
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
        var emptyState = new CodexEmptyState
        {
            Variant = CodexControlVariant.Success,
            Size = CodexControlSize.Large,
            Icon = "✓",
            Title = "All clear",
            Action = "Refresh"
        };
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
        Assert.Contains("size-sm", avatarGroup.Classes);
        Assert.Contains("stacked", avatarGroup.Classes);
        Assert.Contains("has-items", avatarGroup.Classes);
        Assert.Equal(3, avatarGroup.ItemCount);
        Assert.Equal(CodexControlSize.Small, ((CodexAvatar)avatarGroup.Children[0]).Size);
        Assert.Contains("avatar-group-item", avatarGroup.Children[0].Classes);
        Assert.Contains("group-first", avatarGroup.Children[0].Classes);
        Assert.Contains("group-last", avatarGroupCount.Classes);
        Assert.Equal("+3", avatarGroupCount.Content);
        Assert.Contains("has-count", avatarGroupCount.Classes);
        Assert.Contains("idle", avatar.Classes);
        Assert.Contains("fallback-visible", avatar.Classes);
        Assert.Contains("size-lg", spinner.Classes);
        Assert.Contains("paused", spinner.Classes);
        Assert.DoesNotContain("active", spinner.Classes);
        Assert.Equal("Loading records", spinner.Label);
        Assert.Equal("Loading records", AutomationProperties.GetName(spinner));
        Assert.Equal("idle", AutomationProperties.GetItemStatus(spinner));
        Assert.Contains("variant-success", progress.Classes);
        Assert.Contains("size-sm", progress.Classes);
        Assert.Contains("indeterminate", progress.Classes);
        Assert.Equal(CodexSwitchThemeOptions.ShadcnDefault.SkeletonShimmerDuration, progress.IndeterminateAnimationDuration);
        Assert.Equal(72, progress.IndeterminateIndicatorWidth);
        Assert.Contains("variant-warning", toast.Classes);
        Assert.DoesNotContain("variant-default", toast.Classes);
        Assert.Contains("variant-success", emptyState.Classes);
        Assert.Contains("size-lg", emptyState.Classes);
        Assert.Contains("has-icon", emptyState.Classes);
        Assert.Contains("has-title", emptyState.Classes);
        Assert.Contains("has-header", emptyState.Classes);
        Assert.Contains("has-actions", emptyState.Classes);
        Assert.True(emptyState.CanExecuteAction);
        Assert.Contains("position-top-center", sonner.Classes);
        Assert.Contains("compact", sonner.Classes);
        Assert.Contains("rich-colors", sonner.Classes);
        Assert.Contains("close-hidden", sonner.Classes);
        Assert.Equal(HorizontalAlignment.Center, sonner.HorizontalAlignment);
        Assert.Equal(VerticalAlignment.Top, sonner.VerticalAlignment);
    }

    [Fact]
    public async Task AvatarImagePathLoadingStatusAndFallbackDelayMirrorWebAvatarImage()
    {
        await using var session = HeadlessUnitTestSession.StartNew(typeof(OverlayRenderedLifecycleTestApp), AvaloniaTestIsolationLevel.PerTest);
        await session.Dispatch(() =>
        {
            var appRoot = Path.GetFullPath(Path.Combine(FindRepositoryRoot(), "..", "CodexSwitch"));
            var iconPath = Path.Combine(appRoot, "Assets", "icons", "openai.png");
            var missingPath = Path.Combine(appRoot, "Assets", "icons", "missing-avatar.png");
            var changes = new List<CodexAvatarLoadingStatusChangedEventArgs>();
            var avatar = new CodexAvatar
            {
                Fallback = "AI",
                FallbackDelay = TimeSpan.FromMilliseconds(600)
            };

            avatar.LoadingStatusChanged += (_, args) => changes.Add(args);

            Assert.True(File.Exists(iconPath));
            Assert.False(File.Exists(missingPath));
            Assert.Equal(CodexAvatarLoadingStatus.Idle, avatar.LoadingStatus);
            Assert.True(avatar.IsFallbackVisible);
            Assert.Contains("idle", avatar.Classes);
            Assert.Contains("fallback-visible", avatar.Classes);

            avatar.ImagePath = iconPath;

            Assert.Equal(CodexAvatarLoadingStatus.Loaded, avatar.LoadingStatus);
            Assert.True(avatar.HasImage);
            Assert.False(avatar.IsFallbackVisible);
            Assert.Null(avatar.LastLoadError);
            Assert.Contains("loaded", avatar.Classes);
            Assert.Contains("has-image", avatar.Classes);
            Assert.Contains(changes, change => change.OldStatus == CodexAvatarLoadingStatus.Idle && change.NewStatus == CodexAvatarLoadingStatus.Loading);
            Assert.Contains(changes, change => change.OldStatus == CodexAvatarLoadingStatus.Loading && change.NewStatus == CodexAvatarLoadingStatus.Loaded);

            avatar.ImagePath = missingPath;

            Assert.Equal(CodexAvatarLoadingStatus.Error, avatar.LoadingStatus);
            Assert.False(avatar.HasImage);
            Assert.True(avatar.IsFallbackVisible);
            Assert.NotNull(avatar.LastLoadError);
            Assert.Contains("error", avatar.Classes);
            Assert.Contains("fallback-visible", avatar.Classes);
            var errorChange = changes.Last(change => change.NewStatus == CodexAvatarLoadingStatus.Error);
            Assert.Equal(missingPath, errorChange.ImagePath);
            Assert.Equal(avatar.LastLoadError, errorChange.ErrorMessage);

            avatar.LoadingStatus = CodexAvatarLoadingStatus.Loading;

            Assert.False(avatar.IsFallbackVisible);
            Assert.Contains("loading", avatar.Classes);
            Assert.Contains("fallback-delayed", avatar.Classes);

            avatar.FallbackDelay = TimeSpan.Zero;

            Assert.True(avatar.IsFallbackVisible);
            Assert.Contains("fallback-visible", avatar.Classes);

            avatar.ImagePath = "";

            Assert.Equal(CodexAvatarLoadingStatus.Idle, avatar.LoadingStatus);
            Assert.Null(avatar.LastLoadError);
            Assert.True(avatar.IsFallbackVisible);
        }, CancellationToken.None);
    }

    [Fact]
    public void BadgeCanMirrorWebLinkActivationWhenInteractive()
    {
        var commandActivations = 0;
        var eventParameters = new List<object?>();
        var eventSources = new List<CodexBadgeActivationSource>();
        var command = new TestCommand(() => commandActivations++);
        var badge = new CodexBadge
        {
            Content = "Open provider",
            Variant = CodexControlVariant.Link,
            IsInteractive = true,
            Command = command,
            CommandParameter = "provider-route"
        };
        badge.Activated += (_, args) =>
        {
            eventParameters.Add(args.CommandParameter);
            eventSources.Add(args.Source);
        };

        Assert.True(badge.Focusable);
        Assert.True(badge.CanActivate);
        Assert.Contains("interactive", badge.Classes);
        Assert.Contains("can-activate", badge.Classes);
        Assert.DoesNotContain("command-blocked", badge.Classes);

        Assert.True(badge.TryActivate());

        Assert.Equal(1, commandActivations);
        Assert.Equal(["provider-route"], eventParameters);
        Assert.Equal([CodexBadgeActivationSource.Programmatic], eventSources);

        command.CanExecuteValue = false;
        command.RaiseCanExecuteChanged();

        Assert.False(badge.CanActivate);
        Assert.Contains("command-blocked", badge.Classes);
        Assert.DoesNotContain("can-activate", badge.Classes);
        Assert.False(badge.TryActivate());
        Assert.Equal(1, commandActivations);

        badge.Command = null;
        badge.IsInteractive = true;

        Assert.True(badge.CanActivate);
        Assert.True(badge.TryHandleActivationKey(Key.Enter));
        Assert.False(badge.TryHandleActivationKey(Key.Escape));
        Assert.Equal(["provider-route", "provider-route"], eventParameters);
        Assert.Equal(
            [
                CodexBadgeActivationSource.Programmatic,
                CodexBadgeActivationSource.Keyboard
            ],
            eventSources);
    }

    [Fact]
    public void BadgePointerActivationUsesPrimaryReleaseOnly()
    {
        var commandActivations = 0;
        var eventParameters = new List<object?>();
        var eventSources = new List<CodexBadgeActivationSource>();
        var command = new TestCommand(() => commandActivations++);
        var badge = new CodexBadge
        {
            Content = "Open provider",
            Variant = CodexControlVariant.Link,
            IsInteractive = true,
            Command = command,
            CommandParameter = "provider-route"
        };
        badge.Activated += (_, args) =>
        {
            eventParameters.Add(args.CommandParameter);
            eventSources.Add(args.Source);
        };

        Assert.False(badge.TryHandlePointerActivation(PointerUpdateKind.RightButtonReleased));
        Assert.False(badge.TryHandlePointerActivation(PointerUpdateKind.MiddleButtonReleased));
        Assert.Equal(0, commandActivations);
        Assert.Empty(eventParameters);

        Assert.True(badge.TryHandlePointerActivation(PointerUpdateKind.LeftButtonReleased));
        Assert.Equal(1, commandActivations);
        Assert.Equal(["provider-route"], eventParameters);
        Assert.Equal([CodexBadgeActivationSource.Pointer], eventSources);

        command.CanExecuteValue = false;
        command.RaiseCanExecuteChanged();

        Assert.False(badge.TryHandlePointerActivation(PointerUpdateKind.LeftButtonReleased));
        Assert.Equal(1, commandActivations);
        Assert.Equal(["provider-route"], eventParameters);
        Assert.Equal([CodexBadgeActivationSource.Pointer], eventSources);

        badge.Command = null;
        badge.IsInteractive = false;

        Assert.False(badge.TryHandlePointerActivation(PointerUpdateKind.LeftButtonReleased));
        Assert.Equal(["provider-route"], eventParameters);
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
            "PART_Root", "PART_Trigger", "PART_Overlay", "PART_Surface", "PART_Header", "PART_Title", "PART_Description", "PART_Content", "PART_Action", "PART_Close", "DismissCommand", "closed", "modal", "non-modal", "has-trigger", "TransformOperationsTransition", "Transitions");
        AssertStyle(Path.Combine(root, "src", "CodexSwitchUI", "Themes", "Controls", "Popover.axaml"),
            "PART_Root", "PART_Trigger", "PART_Popup", "PART_Surface", "PART_Header", "PART_Title", "PART_Description", "PART_Content", "PART_Action", "PART_Close", "PART_Arrow", "DismissCommand", "side-top", "side-bottom", "side-left", "side-right", "align-start", "align-center", "align-end", "closed", "has-trigger", "has-arrow", "TransformOperationsTransition", "Transitions");
        AssertStyle(Path.Combine(root, "src", "CodexSwitchUI", "Themes", "Controls", "CommandDialog.axaml"),
            "PART_Root", "PART_Trigger", "PART_Layer", "PART_Overlay", "PART_Surface", "PART_Command", "PART_Content", "PART_Close", "SearchText", "ShouldFilter", "LoopNavigation", "DismissCommand", "closed", "modal", "non-modal", "has-trigger", "loading", "TransformOperationsTransition", "Transitions");
        AssertStyle(Path.Combine(root, "src", "CodexSwitchUI", "Themes", "Controls", "Drawer.axaml"),
            "PART_Root", "PART_Trigger", "PART_Layer", "PART_Overlay", "PART_Surface", "PART_Handle", "PART_Header", "PART_Title", "PART_Description", "PART_ContentScroll", "PART_Content", "PART_Footer", "PART_Close", "DismissCommand", "closed", "modal", "non-modal", "has-trigger", "direction-bottom", "direction-top", "direction-right", "direction-left", "has-handle", "dragging", "drag-dismiss-ready", "scale-background", "close-on-drag", "TransformOperationsTransition", "Transitions");
        AssertStyle(Path.Combine(root, "src", "CodexSwitchUI", "Themes", "Controls", "Tooltip.axaml"),
            "CodexTooltipProvider", "PART_Root", "PART_Trigger", "PART_Popup", "PART_Surface", "PART_Content", "PART_Arrow", "side-top", "side-bottom", "side-left", "side-right", "closed", "has-trigger", "has-arrow", "TransformOperationsTransition", "Transitions");
        AssertStyle(Path.Combine(root, "src", "CodexSwitchUI", "Themes", "Controls", "HoverCard.axaml"),
            "PART_Root", "PART_Trigger", "PART_Popup", "PART_Surface", "PART_Content", "PART_Arrow", "side-top", "side-bottom", "side-left", "side-right", "align-start", "align-center", "align-end", "closed", "has-trigger", "has-arrow", "delayed-open", "delayed-close", "TransformOperationsTransition", "Transitions");
        AssertStyle(Path.Combine(root, "src", "CodexSwitchUI", "Themes", "Controls", "DropdownButton.axaml"),
            "PART_Root", "PART_Trigger", "PART_Chevron", "PART_Popup", "PART_Surface", "PART_DropDownContent", "PART_Arrow", "IsLightDismissEnabled", "side-top", "side-bottom", "side-left", "side-right", "align-start", "align-center", "align-end", "closed", "has-arrow", "TransformOperationsTransition", "Transitions");
        AssertStyle(Path.Combine(root, "src", "CodexSwitchUI", "Themes", "Controls", "SplitButton.axaml"),
            "PART_Root", "PART_ButtonGroup", "PART_PrimaryAction", "PART_MenuTrigger", "PART_Divider", "PART_Chevron", "PART_Popup", "PART_Surface", "PART_DropDownContent", "PART_Arrow", "IsLightDismissEnabled", "side-top", "side-bottom", "side-left", "side-right", "align-start", "align-center", "align-end", "closed", "has-arrow", "primary-action-disabled", "can-open-dropdown", "TransformOperationsTransition", "Transitions");
        AssertStyle(Path.Combine(root, "src", "CodexSwitchUI", "Themes", "Controls", "Toast.axaml"),
            "PART_Surface", "PART_Status", "PART_Icon", "PART_Title", "PART_Description", "PART_Content", "PART_Action", "PART_Close", "DismissCommand", "closed", "TransformOperationsTransition", "Transitions");
        AssertStyleContains(Path.Combine(root, "src", "CodexSwitchUI", "Themes", "Controls", "Sonner.axaml"),
            "controls|CodexSonner", "position-bottom-right", "position-top-center", "rich-colors", "close-hidden", "sonner-toast", "MaxHeight", "ThicknessTransition", "Transitions");
        AssertStyle(Path.Combine(root, "src", "CodexSwitchUI", "Themes", "Controls", "Alert.axaml"),
            "PART_Surface", "PART_Icon", "PART_DefaultIcon", "PART_Title", "PART_Description", "PART_Content", "PART_Action", "variant-destructive", "variant-success", "variant-warning", "Transitions");
        AssertStyle(Path.Combine(root, "src", "CodexSwitchUI", "Themes", "Controls", "Badge.axaml"),
            "PART_Surface", "PART_Status", "interactive", "can-activate", "command-blocked", "focus-visible", "size-lg", "status-warning", "variant-secondary", "variant-destructive", "variant-outline", "variant-success", "variant-warning", "variant-ghost");
        AssertStyle(Path.Combine(root, "src", "CodexSwitchUI", "Themes", "Controls", "EmptyState.axaml"),
            "PART_Surface", "PART_IconShell", "PART_Header", "PART_Title", "PART_Description", "PART_Content", "PART_Action", "PART_SecondaryAction", "has-header", "has-actions", "can-action", "action-command-blocked", "secondary-action-command-blocked", "command-blocked", "variant-success", "variant-warning", "Transitions");
        AssertStyle(Path.Combine(root, "src", "CodexSwitchUI", "Themes", "Controls", "Avatar.axaml"),
            "PART_Surface", "PART_Image", "PART_Fallback", "PART_Status", "fallback-visible", "fallback-delayed", "loading", "loaded", "error", "status-destructive", "variant-outline");
        AssertStyleContains(Path.Combine(root, "src", "CodexSwitchUI", "Themes", "Controls", "Spinner.axaml"),
            "controls|CodexSpinner", "CodexSwitch.ForegroundBrush", "StrokeThickness", "size-sm", "size-lg", "paused", "DoubleTransition", "BrushTransition");
        AssertStyle(Path.Combine(root, "src", "CodexSwitchUI", "Themes", "Controls", "Progress.axaml"),
            "PART_Track", "PART_Indicator", "PART_IndeterminateIndicator", "IndeterminateAnimationDuration", "IndeterminateIndicatorWidth", "IndeterminateIndicatorMargin", "DoubleTransition", "BrushTransition");
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
            "CommandDialog.axaml",
            "Tooltip.axaml",
            "HoverCard.axaml",
            "DropdownButton.axaml",
            "SplitButton.axaml",
            "Toast.axaml",
            "Alert.axaml",
            "Badge.axaml",
            "EmptyState.axaml",
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
        return TestRepository.FindRoot();
    }

    private static void EnsureCodexTheme()
    {
        var application = Application.Current;
        Assert.NotNull(application);

        if (!application.Styles.OfType<CodexSwitchTheme>().Any())
        {
            application.Styles.Add(new CodexSwitchTheme());
        }
    }

    private static Window ShowWindow(params Control[] controls)
    {
        var root = new StackPanel
        {
            Spacing = 16,
            Margin = new Thickness(18)
        };

        foreach (var control in controls)
        {
            root.Children.Add(control);
        }

        var window = new Window
        {
            Width = 720,
            Height = 720,
            Content = root
        };

        window.Show();
        return window;
    }

    private static void InvokeButtonClick(Button button)
    {
        var onClick = button.GetType().GetMethod("OnClick", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        Assert.NotNull(onClick);
        onClick.Invoke(button, null);
    }

    private sealed class TestCommand(Action execute) : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public bool CanExecuteValue { get; set; } = true;

        public bool CanExecute(object? parameter)
        {
            return CanExecuteValue;
        }

        public void Execute(object? parameter)
        {
            execute();
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
