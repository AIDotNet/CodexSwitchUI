using CodexSwitchUI.Tokens;
using Xunit;

namespace CodexSwitchUI.Tests;

public class ComponentStructureTests
{
    private static readonly string[] ExpectedTopLevelControls =
    [
        "CodexButton",
        "CodexButtonGroup",
        "CodexInputGroup",
        "CodexInputOtp",
        "CodexLabel",
        "CodexDropdownButton",
        "CodexSplitButton",
        "CodexField",
        "CodexTextBox",
        "CodexTextarea",
        "CodexSelect",
        "CodexCombobox",
        "CodexNativeSelect",
        "CodexCalendar",
        "CodexDatePicker",
        "CodexCheckBox",
        "CodexRadio",
        "CodexRadioGroup",
        "CodexSwitch",
        "CodexToggle",
        "CodexToggleGroup",
        "CodexSlider",
        "CodexTabs",
        "CodexBreadcrumb",
        "CodexNavigationMenu",
        "CodexMenubar",
        "CodexDirection",
        "CodexAspectRatio",
        "CodexCard",
        "CodexItem",
        "CodexCarousel",
        "CodexTooltip",
        "CodexHoverCard",
        "CodexPopover",
        "CodexDialog",
        "CodexAlertDialog",
        "CodexSheet",
        "CodexDrawer",
        "CodexToast",
        "CodexSonner",
        "CodexAlert",
        "CodexBadge",
        "CodexAvatar",
        "CodexAvatarGroup",
        "CodexSpinner",
        "CodexProgress",
        "CodexChart",
        "CodexBarChart",
        "CodexLineChart",
        "CodexRankedBarChart",
        "CodexUsagePieChart",
        "CodexTable",
        "CodexPagination",
        "CodexScrollArea",
        "CodexEmptyState",
        "CodexMenu",
        "CodexContextMenu",
        "CodexCommand",
        "CodexCommandDialog",
        "CodexAccordion",
        "CodexCollapsible",
        "CodexSeparator",
        "CodexKbd",
        "CodexSkeleton"
    ];

    private static readonly string[] Components =
    [
        "Button",
        "ButtonGroup",
        "InputGroup",
        "InputOtp",
        "Label",
        "DropdownButton",
        "SplitButton",
        "Field",
        "Input",
        "Textarea",
        "Select",
        "Combobox",
        "NativeSelect",
        "Calendar",
        "DatePicker",
        "Checkbox",
        "Radio",
        "RadioGroup",
        "Switch",
        "Toggle",
        "Slider",
        "Tabs",
        "Breadcrumb",
        "NavigationMenu",
        "Menubar",
        "Direction",
        "Resizable",
        "AspectRatio",
        "Card",
        "Item",
        "Carousel",
        "Tooltip",
        "HoverCard",
        "Popover",
        "Dialog",
        "AlertDialog",
        "Sheet",
        "Drawer",
        "Toast",
        "Sonner",
        "Alert",
        "Badge",
        "Avatar",
        "AvatarGroup",
        "Spinner",
        "Progress",
        "Chart",
        "BarChart",
        "LineChart",
        "RankedBarChart",
        "UsagePieChart",
        "Table",
        "Pagination",
        "ScrollArea",
        "EmptyState",
        "Menu",
        "ContextMenu",
        "Command",
        "CommandDialog",
        "Accordion",
        "Collapsible",
        "Separator",
        "Kbd",
        "Skeleton"
    ];

    private static readonly StyleGuard[] HighRiskStyleGuards =
    [
        new("Button", true,
        [
            "<ControlTemplate TargetType=\"controls:CodexButton\"",
            "PART_Root",
            "PART_CodexButtonContent",
            "PART_LoadingIndicator",
            "PART_LeadingIcon",
            "PART_TrailingIcon",
            ":pointerover",
            ":pressed",
            ":focus",
            ":disabled"
        ]),
        new("ButtonGroup", true,
        [
            "<ControlTemplate TargetType=\"controls:CodexButtonGroup\"",
            "<ControlTemplate TargetType=\"controls:CodexButtonGroupText\"",
            "CodexButtonGroupSeparator",
            "PART_Root",
            "PART_ItemsPresenter",
            "PART_Text",
            "PART_TextContent",
            "group-single",
            "group-first",
            "group-middle",
            "group-last",
            "button-group-item",
            "horizontal",
            "vertical",
            "variant-outline",
            "controls|CodexTextBox.group-item",
            "controls|CodexSelect.group-item",
            "CodexSwitch.MotionDurationDefault",
            "CodexSwitch.MotionEaseOut"
        ]),
        new("InputGroup", true,
        [
            "<ControlTemplate TargetType=\"controls:CodexInputGroup\"",
            "<ControlTemplate TargetType=\"controls:CodexInputGroupAddon\"",
            "<ControlTemplate TargetType=\"controls:CodexInputGroupText\"",
            "CodexInputGroupButton",
            "PART_Root",
            "PART_ItemsPresenter",
            "PART_FocusRing",
            "PART_Addon",
            "PART_AddonContent",
            "PART_Text",
            "input-group-control",
            "input-group-addon",
            "input-group-button",
            "align-inline-start",
            "align-inline-end",
            "align-block-start",
            "align-block-end",
            "has-focus-within",
            "controls|CodexTextBox.input-group-control",
            "controls|CodexTextarea.input-group-control",
            "controls|CodexSelect.input-group-control",
            "CodexSwitch.MotionDurationDefault",
            "CodexSwitch.MotionEaseOut"
        ]),
        new("InputOtp", true,
        [
            "<ControlTemplate TargetType=\"controls:CodexInputOtp\"",
            "<ControlTemplate TargetType=\"controls:CodexInputOtpGroup\"",
            "<ControlTemplate TargetType=\"controls:CodexInputOtpSlot\"",
            "<ControlTemplate TargetType=\"controls:CodexInputOtpSeparator\"",
            "PART_Root",
            "PART_ItemsPresenter",
            "PART_GroupItemsPresenter",
            "PART_SlotRoot",
            "PART_Character",
            "PART_FocusRing",
            "PART_Separator",
            "input-otp",
            "input-otp-group",
            "input-otp-slot",
            "input-otp-separator",
            "active",
            "complete",
            "has-character",
            "slot-first",
            "slot-middle",
            "slot-last",
            "group-first",
            "group-middle",
            "group-last",
            ":focus-visible",
            ":pointerover",
            ":disabled",
            "TransformOperationsTransition",
            "CodexSwitch.MotionDurationDefault",
            "CodexSwitch.MotionEaseOut"
        ]),
        new("Label", true,
        [
            "<ControlTemplate TargetType=\"controls:CodexLabel\"",
            "PART_LabelLayout",
            "PART_Content",
            "PART_Required",
            "RecognizesAccessKey=\"True\"",
            "has-target",
            "target-disabled",
            "required",
            "intent-error",
            "intent-success",
            "intent-warning",
            "CodexSwitch.MotionDurationDefault",
            "CodexSwitch.MotionEaseOut"
        ]),
        new("DropdownButton", true,
        [
            "<ControlTemplate TargetType=\"controls:CodexDropdownButton\"",
            "PART_Root",
            "PART_Trigger",
            "PART_Chevron",
            "PART_Popup",
            "PART_Surface",
            "PART_DropDownContent",
            "PART_Arrow",
            "IsLightDismissEnabled=\"True\"",
            "open",
            "closed",
            "loading",
            "side-bottom",
            "align-center",
            "has-arrow",
            "TransformOperationsTransition",
            "controls:CodexButton"
        ]),
        new("SplitButton", true,
        [
            "<ControlTemplate TargetType=\"controls:CodexSplitButton\"",
            "PART_Root",
            "PART_ButtonGroup",
            "PART_PrimaryAction",
            "PART_MenuTrigger",
            "PART_Divider",
            "PART_Chevron",
            "PART_Popup",
            "PART_Surface",
            "PART_DropDownContent",
            "PART_Arrow",
            "IsLightDismissEnabled=\"True\"",
            "open",
            "closed",
            "loading",
            "primary-action-disabled",
            "can-open-dropdown",
            "side-bottom",
            "align-center",
            "has-arrow",
            "TransformOperationsTransition",
            "SplitButtonCornerRadiusPartConverter",
            "controls:CodexButton"
        ]),
        new("Field", true,
        [
            "<ControlTemplate TargetType=\"controls:CodexField\"",
            "<ControlTemplate TargetType=\"controls:CodexFieldGroup\"",
            "<ControlTemplate TargetType=\"controls:CodexFieldSet\"",
            "<ControlTemplate TargetType=\"controls:CodexFieldLegend\"",
            "<ControlTemplate TargetType=\"controls:CodexFieldContent\"",
            "<ControlTemplate TargetType=\"controls:CodexFieldTitle\"",
            "<ControlTemplate TargetType=\"controls:CodexFieldDescription\"",
            "<ControlTemplate TargetType=\"controls:CodexFieldSeparator\"",
            "<ControlTemplate TargetType=\"controls:CodexFieldError\"",
            "PART_Root",
            "PART_Layout",
            "PART_LabelRow",
            "PART_Label",
            "PART_Control",
            "PART_Description",
            "PART_Message",
            "PART_ItemsPresenter",
            "PART_Legend",
            "PART_ErrorRoot",
            "controls:CodexLabel",
            "field-group",
            "field-set",
            "field-legend",
            "field-content",
            "field-title",
            "field-description",
            "field-separator",
            "field-error",
            "has-label",
            "has-description",
            "has-message",
            "required",
            "invalid",
            "orientation-horizontal",
            "orientation-responsive",
            "intent-error",
            "intent-success",
            "intent-warning",
            "size-sm",
            "size-lg",
            "CodexSwitch.DisabledOpacity"
        ]),
        new("Input", true,
        [
            "<ControlTemplate TargetType=\"controls:CodexTextBox\"",
            "PART_BorderElement",
            "PART_ScrollViewer",
            "PART_Placeholder",
            "PART_TextPresenter",
            "SelectionBrush",
            ":pointerover",
            ":focus",
            ":disabled",
            "is-read-only"
        ]),
        new("Select", true,
        [
            "<ControlTemplate TargetType=\"controls:CodexSelect\"",
            "PART_Trigger",
            "PART_SelectedContentHost",
            "PART_Chevron",
            "PART_Popup",
            "PART_PopupBorder",
            "PART_ItemsPresenter",
            "<ControlTemplate TargetType=\"ComboBoxItem\"",
            "ComboBoxItem:selected",
            "ComboBoxItem:pointerover",
            ":dropdownopen",
            ":disabled"
        ]),
        new("NativeSelect", true,
        [
            "<ControlTemplate TargetType=\"controls:CodexNativeSelect\"",
            "PART_Trigger",
            "PART_SelectedContentHost",
            "PART_Chevron",
            "PART_Popup",
            "PART_PopupBorder",
            "PART_ItemsPresenter",
            "<ControlTheme TargetType=\"ComboBoxItem\"",
            "<ControlTemplate TargetType=\"controls:CodexNativeSelectOption\"",
            "<ControlTemplate TargetType=\"controls:CodexNativeSelectOptGroup\"",
            "native-select-option",
            "native-select-optgroup",
            "placeholder-visible",
            "has-selection",
            "invalid",
            "ComboBoxItem:selected",
            "ComboBoxItem:pointerover",
            ":dropdownopen",
            ":disabled",
            "TransformOperationsTransition"
        ]),
        new("Calendar", true,
        [
            "<ControlTemplate TargetType=\"controls:CodexCalendar\"",
            "<ControlTemplate TargetType=\"controls:CodexCalendarDayButton\"",
            "PART_Root",
            "PART_Header",
            "PART_PreviousButton",
            "PART_MonthTitle",
            "PART_NextButton",
            "PART_FocusRing",
            "PART_ItemsPresenter",
            "PART_DayRoot",
            "PART_DayRange",
            "PART_DayContent",
            "PART_DayFocusRing",
            "calendar",
            "calendar-day",
            "outside",
            "today",
            "selected",
            "range-start",
            "range-end",
            "range-middle",
            "booked",
            "unavailable",
            "can-activate",
            "command-blocked",
            "active",
            "blank",
            "week-numbers",
            ":focus-visible",
            ":pointerover",
            ":pressed",
            ":disabled",
            "TransformOperationsTransition",
            "CodexSwitch.MotionDurationDefault",
            "CodexSwitch.MotionEaseOut"
        ]),
        new("DatePicker", true,
        [
            "<ControlTemplate TargetType=\"controls:CodexDatePicker\"",
            "PART_InputGroup",
            "PART_FocusRing",
            "PART_Trigger",
            "PART_CalendarIcon",
            "PART_Text",
            "PART_Clear",
            "PART_ClearIcon",
            "PART_Chevron",
            "PART_Popup",
            "PART_PopupBorder",
            "PART_Calendar",
            "PART_Loading",
            "controls:CodexCalendar",
            "date-picker",
            "open",
            "closed",
            "has-selection",
            "placeholder-visible",
            "range",
            "single",
            "range-complete",
            "loading",
            "has-clear",
            ":focus-visible",
            ":pointerover",
            ":disabled",
            "TransformOperationsTransition",
            "CodexSwitch.MotionDurationDefault",
            "CodexSwitch.MotionEaseOut"
        ]),
        new("Checkbox", true,
        [
            "<ControlTemplate TargetType=\"controls:CodexCheckBox\"",
            "PART_Box",
            "PART_Check",
            "PART_Indeterminate",
            ":pointerover",
            ":checked",
            ":indeterminate",
            ":focus",
            ":disabled"
        ]),
        new("Radio", true,
        [
            "<ControlTemplate TargetType=\"controls:CodexRadio\"",
            "PART_Ring",
            "PART_Dot",
            ":pointerover",
            ":checked",
            ":focus",
            ":disabled"
        ]),
        new("Switch", true,
        [
            "<ControlTemplate TargetType=\"controls:CodexSwitch\"",
            "PART_Track",
            "PART_Thumb",
            ":pointerover",
            ":pressed",
            ":checked",
            ":focus",
            ":disabled"
        ]),
        new("Toggle", true,
        [
            "<ControlTemplate TargetType=\"controls:CodexToggle\"",
            "<ControlTemplate TargetType=\"controls:CodexToggleGroup\"",
            "PART_Root",
            "PART_Content",
            "PART_FocusRing",
            "PART_ItemsPresenter",
            "CodexToggleGroupItem",
            "type-single",
            "type-multiple",
            "state-on",
            "state-off",
            "roving",
            "loop",
            "variant-outline",
            ":pointerover",
            ":pressed",
            ":checked",
            ":focus",
            ":disabled",
            "TransformOperationsTransition"
        ]),
        new("Slider", true,
        [
            "<ControlTemplate TargetType=\"controls:CodexSlider\"",
            "PART_SliderRoot",
            "PART_Track",
            "PART_DecreaseButton",
            "PART_IncreaseButton",
            "PART_Thumb",
            ":pointerover",
            ":pressed",
            ":focus",
            ":disabled"
        ]),
        new("Tabs", true,
        [
            "PART_List",
            "PART_ContentTransitionHost",
            "PART_TriggerRoot",
            "PART_Indicator",
            "PART_VerticalIndicator",
            "PART_FocusRing",
            "TransitioningContentControl",
            "CrossFade",
            "BoxShadowsTransition",
            "variant-line",
            "activation-manual",
            "TabItem:pointerover",
            "CodexTabItem:focus-visible",
            "TabItem:selected",
            "TabItem:disabled"
        ]),
        new("Carousel", true,
        [
            "<ControlTemplate TargetType=\"controls:CodexCarousel\"",
            "<ControlTemplate TargetType=\"controls:CodexCarouselItem\"",
            "PART_Root",
            "PART_Viewport",
            "PART_ScrollViewer",
            "PART_ItemsPresenter",
            "PART_FocusRing",
            "PART_Controls",
            "PART_PreviousButton",
            "PART_NextButton",
            "PART_Status",
            "PART_ItemRoot",
            "PART_ItemContent",
            "carousel",
            "carousel-item",
            "selected",
            "before-selected",
            "after-selected",
            "loop",
            "can-previous",
            "can-next",
            "at-start",
            "at-end",
            "previous-disabled",
            "next-disabled",
            "show-navigation",
            "show-status",
            "vertical",
            ":focus-visible",
            ":pointerover",
            ":disabled",
            "TransformOperationsTransition",
            "BoxShadowsTransition",
            "CodexSwitch.MotionDurationDefault",
            "CodexSwitch.MotionEaseOut"
        ]),
        new("NavigationMenu", true,
        [
            "PART_List",
            "PART_ViewportPositioner",
            "PART_Viewport",
            "PART_Indicator",
            "PART_ContentTransitionHost",
            "TransitioningContentControl",
            "CompositePageTransition",
            "PageSlide",
            "CrossFade",
            "IsTransitionReversed",
            "motion-from-start",
            "motion-from-end",
            "controls|CodexNavigationMenuItem:pointerover",
            "controls|CodexNavigationMenuItem:focus-visible",
            "controls|CodexNavigationMenuItem.open",
            "controls|CodexNavigationMenuLink.active",
            "controls|CodexNavigationMenuItem:disabled"
        ]),
        new("Menubar", true,
        [
            "<ControlTemplate TargetType=\"controls:CodexMenubar\"",
            "<ControlTemplate TargetType=\"controls:CodexMenubarItem\"",
            "PART_Root",
            "PART_ItemsPresenter",
            "PART_ItemRoot",
            "PART_ItemGrid",
            "PART_Indicator",
            "PART_Check",
            "PART_Radio",
            "PART_Shortcut",
            "PART_SubMenuArrow",
            "PART_Popup",
            "PART_MenuSurface",
            "PART_MenuItemsPresenter",
            "IsLightDismissEnabled=\"False\"",
            "top-level",
            "menu-content-item",
            "active-menu",
            "menubar-checkbox-item",
            "menubar-radio-item",
            "horizontal",
            "vertical",
            "loading",
            "loop",
            "controls|CodexMenubar MenuItem:pointerover",
            "controls|CodexMenubar MenuItem:focus-visible",
            "controls|CodexMenubar controls|CodexMenubarItem.open",
            "controls|CodexMenubar controls|CodexMenubarItem:disabled",
            "TransformOperationsTransition",
            "CodexSwitch.MotionDurationDefault",
            "CodexSwitch.MotionEaseOut"
        ]),
        new("Resizable", true,
        [
            "controls|CodexResizablePanelGroup",
            "<ControlTemplate TargetType=\"controls:CodexResizablePanel\"",
            "<ControlTemplate TargetType=\"controls:CodexResizableHandle\"",
            "PART_PanelRoot",
            "PART_PanelContent",
            "PART_HandleRoot",
            "PART_HandleTrack",
            "PART_HandleGrip",
            "PART_FocusRing",
            "resizable-panel-group",
            "resizable-panel",
            "resizable-handle",
            "with-handle",
            "dragging",
            "horizontal",
            "vertical",
            ":focus-visible",
            ":pointerover",
            ":disabled",
            "CodexSwitch.MotionDurationDefault",
            "CodexSwitch.MotionEaseOut"
        ]),
        new("AspectRatio", false,
        [
            "<ControlTemplate TargetType=\"controls:CodexAspectRatio\"",
            "PART_Root",
            "PART_Viewport",
            "PART_ContentHost",
            "PART_Empty",
            "aspect-ratio",
            "has-content",
            "empty",
            "ratio-square",
            "ratio-video",
            "ratio-portrait",
            "ratio-landscape",
            "fit-width",
            "fit-height",
            "fit-contain",
            ":pointerover",
            ":disabled",
            "TransformOperationsTransition",
            "CodexSwitch.MotionDurationDefault",
            "CodexSwitch.MotionEaseOut"
        ]),
        new("Breadcrumb", true,
        [
            "<ControlTemplate TargetType=\"controls:CodexBreadcrumb\"",
            "<ControlTemplate TargetType=\"controls:CodexBreadcrumbList\"",
            "<ControlTemplate TargetType=\"controls:CodexBreadcrumbItem\"",
            "<ControlTemplate TargetType=\"controls:CodexBreadcrumbLink\"",
            "<ControlTemplate TargetType=\"controls:CodexBreadcrumbPage\"",
            "<ControlTemplate TargetType=\"controls:CodexBreadcrumbSeparator\"",
            "<ControlTemplate TargetType=\"controls:CodexBreadcrumbEllipsis\"",
            "PART_Navigation",
            "PART_List",
            "PART_Item",
            "PART_LinkRoot",
            "PART_PageRoot",
            "PART_Separator",
            "PART_EllipsisRoot",
            "breadcrumb-list",
            "breadcrumb-link",
            "breadcrumb-page",
            "breadcrumb-separator",
            "breadcrumb-ellipsis",
            "current",
            "has-href",
            ":pointerover",
            ":pressed",
            ":focus-visible",
            ":disabled",
            "CodexSwitch.DisabledOpacity"
        ]),
        new("Menu", true,
        [
            "PART_Surface",
            "PART_ItemRoot",
            "MenuItem:pointerover",
            "MenuItem:focus-visible",
            "MenuItem:selected",
            "MenuItem:disabled"
        ]),
        new("ContextMenu", true,
        [
            "PART_Surface",
            "PART_ItemsPresenter",
            "PART_ItemRoot",
            "PART_SubMenuSurface",
            "PART_SubMenuItemsPresenter",
            "context-menu-open",
            "submenu-open",
            "side-bottom",
            "side-left",
            "side-right",
            "side-top",
            "TransformOperationsTransition",
            "RenderTransformOrigin",
            "MenuItem:pointerover",
            "MenuItem:focus-visible",
            "MenuItem:selected",
            "MenuItem:disabled"
        ]),
        new("Command", true,
        [
            "PART_Surface",
            "PART_InputShell",
            "PART_Input",
            "PART_ItemRoot",
            "controls|CodexCommandInput",
            "<ControlTemplate TargetType=\"controls:CodexCommandInput\"",
            "controls|CodexCommandShortcut",
            "<ControlTemplate TargetType=\"controls:CodexCommandShortcut\"",
            "controls|CodexCommandSeparator",
            "controls|CodexCommandItem:pointerover",
            "controls|CodexCommandItem:focus-visible",
            "controls|CodexCommandItem.active",
            "controls|CodexCommandItem.filtered-out",
            "controls|CodexCommandItem:disabled",
            "SearchText",
            "MaxHeight",
            "ScrollViewer"
        ]),
        new("Item", true,
        [
            "<ControlTemplate TargetType=\"controls:CodexItem\"",
            "<ControlTemplate TargetType=\"controls:CodexItemGroup\"",
            "<ControlTemplate TargetType=\"controls:CodexItemMedia\"",
            "<ControlTemplate TargetType=\"controls:CodexItemSeparator\"",
            "PART_Surface",
            "PART_Header",
            "PART_Body",
            "PART_Media",
            "PART_Title",
            "PART_Description",
            "PART_Content",
            "PART_Actions",
            "PART_Footer",
            "PART_GroupSurface",
            "PART_ItemsPresenter",
            "PART_MediaRoot",
            "interactive",
            "selected",
            "loading",
            "can-activate",
            "has-media",
            ":pointerover",
            ":pressed",
            ":focus-visible",
            "TransformOperationsTransition"
        ]),
        new("Combobox", true,
        [
            "<ControlTemplate TargetType=\"controls:CodexCombobox\"",
            "<ControlTemplate TargetType=\"controls:CodexComboboxItem\"",
            "PART_InputGroup",
            "PART_Input",
            "PART_Clear",
            "PART_Trigger",
            "PART_Chevron",
            "PART_Popup",
            "PART_PopupBorder",
            "PART_Loading",
            "PART_Empty",
            "PART_List",
            "PART_ItemRoot",
            "PART_Check",
            "open",
            "closed",
            "has-selection",
            "has-text",
            "has-filtered-items",
            "empty",
            "loading",
            "auto-highlight",
            "highlight-on-hover",
            "close-on-select",
            "TransformOperationsTransition",
            "controls|CodexComboboxItem.highlighted",
            "controls|CodexComboboxItem.selected"
        ]),
        new("CommandDialog", true,
        [
            "<ControlTemplate TargetType=\"controls:CodexCommandDialog\"",
            "PART_Root",
            "PART_Trigger",
            "PART_Layer",
            "PART_Overlay",
            "PART_Surface",
            "PART_Command",
            "PART_Content",
            "PART_Close",
            "DismissCommand",
            "closed",
            "modal",
            "non-modal",
            "has-trigger",
            "loading",
            "TransformOperationsTransition",
            "controls:CodexCommand"
        ]),
        new("AlertDialog", true,
        [
            "<ControlTemplate TargetType=\"controls:CodexAlertDialog\"",
            "PART_Root",
            "PART_Trigger",
            "PART_Overlay",
            "PART_Surface",
            "PART_Header",
            "PART_Media",
            "PART_Title",
            "PART_Description",
            "PART_Content",
            "PART_Footer",
            "PART_Cancel",
            "PART_Action",
            "CancelDialogCommand",
            "ActionDialogCommand",
            "response-required",
            "outside-dismissable",
            "focus-cancel",
            "action-destructive",
            "has-trigger",
            "open",
            "closed",
            "loading",
            "TransformOperationsTransition",
            "controls:CodexButton"
        ]),
        new("Accordion", true,
        [
            "<ControlTemplate TargetType=\"controls:CodexAccordion\"",
            "<ControlTemplate TargetType=\"controls:CodexAccordionItem\"",
            "PART_Root",
            "PART_ItemsPresenter",
            "PART_ItemRoot",
            "PART_Trigger",
            "PART_Chevron",
            "PART_ContentClip",
            "PART_ContentMeasure",
            "PART_ContentPresenter",
            "type-single",
            "type-multiple",
            "collapsible",
            "open",
            "closed",
            "TransformOperationsTransition",
            ":pointerover",
            ":focus-visible",
            ":disabled"
        ]),
        new("Sheet", true,
        [
            "<ControlTemplate TargetType=\"controls:CodexSheet\"",
            "PART_Root",
            "PART_Trigger",
            "PART_Overlay",
            "PART_Surface",
            "PART_Header",
            "PART_Title",
            "PART_Description",
            "PART_Content",
            "PART_Action",
            "PART_Close",
            "DismissCommand",
            "open",
            "closed",
            "modal",
            "non-modal",
            "has-trigger",
            "side-right",
            "side-left",
            "side-top",
            "side-bottom",
            "TransformOperationsTransition"
        ]),
        new("Drawer", true,
        [
            "<ControlTemplate TargetType=\"controls:CodexDrawer\"",
            "PART_Root",
            "PART_Trigger",
            "PART_Layer",
            "PART_Overlay",
            "PART_Surface",
            "PART_Handle",
            "PART_Header",
            "PART_Title",
            "PART_Description",
            "PART_ContentScroll",
            "PART_Content",
            "PART_Footer",
            "PART_Close",
            "DismissCommand",
            "open",
            "closed",
            "modal",
            "non-modal",
            "has-trigger",
            "direction-bottom",
            "direction-top",
            "direction-right",
            "direction-left",
            "has-handle",
            "dragging",
            "drag-dismiss-ready",
            "scale-background",
            "TransformOperationsTransition"
        ]),
        new("HoverCard", true,
        [
            "<ControlTemplate TargetType=\"controls:CodexHoverCard\"",
            "PART_Root",
            "PART_Trigger",
            "PART_Surface",
            "PART_Content",
            "PART_Arrow",
            "open",
            "closed",
            "has-trigger",
            "has-content",
            "delayed-open",
            "delayed-close",
            "side-bottom",
            "align-center",
            "has-arrow",
            "TransformOperationsTransition"
        ]),
        new("Collapsible", true,
        [
            "<ControlTemplate TargetType=\"controls:CodexCollapsible\"",
            "PART_Trigger",
            "PART_Chevron",
            "PART_ContentClip",
            "PART_ContentMeasure",
            "PART_ContentPresenter",
            "open",
            "closed",
            "TransformOperationsTransition",
            ":pointerover",
            ":focus",
            ":disabled"
        ]),
        new("Progress", false,
        [
            "ControlTemplate",
            "PART_Track",
            "PART_Indicator",
            "PART_Text",
            ":disabled"
        ]),
        new("Chart", false,
        [
            "<ControlTemplate TargetType=\"controls:CodexChartContainer\"",
            "PART_Surface",
            "PART_Header",
            "PART_ChartContent",
            "PART_Tooltip",
            "PART_Footer",
            "PART_RefreshBar",
            "controls|CodexChartLegend",
            "controls|CodexChartLegendItem",
            "controls|CodexChartTooltipContent",
            "controls|CodexChartTooltipItem",
            "CodexSwitch.MotionDurationDefault",
            "CodexSwitch.MotionEaseOut"
        ]),
        new("Table", false,
        [
            "ControlTemplate",
            "PART_TableSurface",
            "PART_RowRoot",
            "controls|CodexTableHeader",
            "controls|CodexTableBody",
            "controls|CodexTableFooter",
            "controls|CodexTableRow:pointerover",
            "controls|CodexTableRow.selected"
        ]),
        new("Pagination", true,
        [
            "<ControlTemplate TargetType=\"controls:CodexPagination\"",
            "PART_Root",
            "PART_FirstButton",
            "PART_PreviousButton",
            "PART_Items",
            "PART_NextButton",
            "PART_LastButton",
            "controls:CodexPaginationPageButton",
            "PageItems",
            "CanGoPrevious",
            "CanGoNext",
            "current",
            "ellipsis",
            "first-page",
            "last-page",
            "loading",
            "compact",
            "show-first-last",
            "CodexSwitch.DisabledOpacity",
            "controls:CodexButton"
        ]),
        new("ScrollArea", true,
        [
            "<ControlTemplate TargetType=\"controls:CodexScrollArea\"",
            "PART_Root",
            "PART_Viewport",
            "PART_ScrollRoot",
            "PART_ContentPresenter",
            "PART_HorizontalScrollBar",
            "PART_VerticalScrollBar",
            "PART_Corner",
            "PART_Track",
            "PART_Thumb",
            "PART_ThumbSurface",
            "ScrollContentPresenter",
            "ScrollGestureRecognizer",
            "type-hover",
            "type-scroll",
            "scrolling",
            "inset-content",
            "can-scroll-x",
            "can-scroll-y",
            "at-top",
            "at-bottom",
            "CodexSwitch.DisabledOpacity"
        ]),
        new("EmptyState", true,
        [
            "<ControlTemplate TargetType=\"controls:CodexEmptyState\"",
            "PART_Surface",
            "PART_Layout",
            "PART_IconShell",
            "PART_Icon",
            "PART_Header",
            "PART_Title",
            "PART_Description",
            "PART_Content",
            "PART_Actions",
            "PART_Action",
            "PART_SecondaryAction",
            "has-icon",
            "has-title",
            "has-description",
            "has-header",
            "has-content",
            "has-action",
            "has-secondary-action",
            "has-actions",
            "can-action",
            "can-secondary-action",
            "action-command-blocked",
            "secondary-action-command-blocked",
            "command-blocked",
            "loading",
            "variant-success",
            "variant-warning",
            "CodexSwitch.DisabledOpacity",
            "controls:CodexButton"
        ])
    ];

    [Fact]
    public void EveryComponentHasOwnStyleFileAndThemeInclude()
    {
        var root = FindRepositoryRoot();
        var theme = File.ReadAllText(Path.Combine(root, "src", "CodexSwitchUI", "Themes", "ComponentStyles.axaml"));

        foreach (var component in Components)
        {
            var stylePath = Path.Combine(root, "src", "CodexSwitchUI", "Themes", "Controls", $"{component}.axaml");
            Assert.True(File.Exists(stylePath), $"Missing style file for {component}: {stylePath}");
            Assert.Contains($"Themes/Controls/{component}.axaml", theme);
        }
    }

    [Fact]
    public void EveryComponentStyleDeclaresMotionTransitions()
    {
        var root = FindRepositoryRoot();

        foreach (var component in Components)
        {
            var stylePath = Path.Combine(root, "src", "CodexSwitchUI", "Themes", "Controls", $"{component}.axaml");
            var style = File.ReadAllText(stylePath);
            Assert.Contains("Transitions", style);
        }
    }

    [Fact]
    public void ComponentStylesDoNotReferenceFluentOrBasedOnDefaults()
    {
        var root = FindRepositoryRoot();
        var failures = new List<string>();

        foreach (var component in Components)
        {
            var style = ReadStyle(root, component);

            if (style.Contains("BasedOn=", StringComparison.Ordinal)
                || style.Contains("Avalonia.Themes.Fluent", StringComparison.Ordinal)
                || style.Contains("FluentTheme", StringComparison.Ordinal))
            {
                failures.Add($"{component}: references a Fluent/BasedOn default style path.");
            }
        }

        AssertNoFailures(failures);
    }

    [Fact]
    public void LoadingFeedbackComponentsOwnNativeChromeAndMotionContracts()
    {
        var root = FindRepositoryRoot();
        var controls = Path.Combine(root, "src", "CodexSwitchUI", "Controls");
        var spinner = File.ReadAllText(Path.Combine(controls, "CodexSpinner.cs"));
        var skeleton = File.ReadAllText(Path.Combine(controls, "CodexSkeleton.cs"));
        var spinnerStyle = ReadStyle(root, "Spinner");
        var progressStyle = ReadStyle(root, "Progress");
        var skeletonStyle = ReadStyle(root, "Skeleton");

        Assert.Contains("public override void Render(DrawingContext context)", spinner);
        Assert.Contains("Focusable = false;", spinner);
        Assert.Contains("IsHitTestVisible = false;", spinner);
        Assert.Contains("CodexClassSync.SetSize(Classes, Size)", spinner);
        Assert.Contains("RotationDuration > TimeSpan.Zero", spinner);
        Assert.Contains("Property=\"IsHitTestVisible\" Value=\"False\"", spinnerStyle);
        Assert.Contains("Property=\"Focusable\" Value=\"False\"", spinnerStyle);
        Assert.Contains("Property=\"StrokeThickness\"", spinnerStyle);
        Assert.Contains("CodexSwitch.MotionDurationDefault", spinnerStyle);

        Assert.Contains("<ControlTemplate TargetType=\"controls:CodexProgress\">", progressStyle);
        Assert.Contains("FocusAdorner\" Value=\"{x:Null}", progressStyle);
        Assert.Contains("Property=\"BorderThickness\" Value=\"0\"", progressStyle);
        Assert.Contains("Property=\"Padding\" Value=\"0\"", progressStyle);
        Assert.Contains("PART_Track", progressStyle);
        Assert.Contains("PART_Indicator", progressStyle);
        Assert.Contains("PART_IndeterminateIndicator", progressStyle);
        Assert.Contains("IndeterminateAnimationDuration\" Value=\"{DynamicResource CodexSwitch.SkeletonShimmerDuration}", progressStyle);

        Assert.Contains("<ControlTemplate TargetType=\"controls:CodexSkeleton\">", skeletonStyle);
        Assert.Contains("PART_Surface", skeletonStyle);
        Assert.Contains("PART_Shimmer", skeletonStyle);
        Assert.Contains("CodexSwitch.SkeletonShimmerDuration", skeletonStyle);
        Assert.Contains("Focusable = false;", skeleton);
        Assert.Contains("IsHitTestVisible = false;", skeleton);
        Assert.Contains("IsAnimated && PulseDuration > TimeSpan.Zero", skeleton);

        foreach (var style in new[] { spinnerStyle, progressStyle, skeletonStyle })
        {
            Assert.DoesNotContain("BasedOn=", style);
            Assert.DoesNotContain("Avalonia.Themes.Fluent", style);
            Assert.DoesNotContain("FluentTheme", style);
        }
    }

    [Fact]
    public void FeedbackSurfaceActionChromeStaysScopedToCodexTemplates()
    {
        var root = FindRepositoryRoot();
        var emptyStateStyle = ReadStyle(root, "EmptyState");
        var sonnerStyle = ReadStyle(root, "Sonner");
        var toastStyle = ReadStyle(root, "Toast");

        Assert.Contains("<ControlTemplate TargetType=\"controls:CodexEmptyState\">", emptyStateStyle);
        Assert.Contains("FocusAdorner\" Value=\"{x:Null}", emptyStateStyle);
        Assert.Contains("controls:CodexButton x:Name=\"PART_Action\"", emptyStateStyle);
        Assert.Contains("controls:CodexButton x:Name=\"PART_SecondaryAction\"", emptyStateStyle);
        Assert.Contains("controls|CodexEmptyState:not(.can-action) /template/ controls|CodexButton#PART_Action", emptyStateStyle);
        Assert.Contains("controls|CodexEmptyState:not(.can-secondary-action) /template/ controls|CodexButton#PART_SecondaryAction", emptyStateStyle);
        Assert.Contains("controls|CodexEmptyState.action-command-blocked /template/ controls|CodexButton#PART_Action", emptyStateStyle);
        Assert.Contains("controls|CodexEmptyState.secondary-action-command-blocked /template/ controls|CodexButton#PART_SecondaryAction", emptyStateStyle);

        Assert.Contains("controls|CodexSonner Border.sonner-toast", sonnerStyle);
        Assert.Contains("controls|CodexSonner.close-hidden controls|CodexToast /template/ Button#PART_Close", sonnerStyle);
        Assert.DoesNotContain("<Style Selector=\"Button#PART_Close\"", sonnerStyle);
        Assert.DoesNotContain("<Style Selector=\"CodexToast", sonnerStyle);

        Assert.Contains("<Button x:Name=\"PART_Close\"", toastStyle);
        Assert.Contains("FocusAdorner=\"{x:Null}\"", toastStyle);
        Assert.Contains("<ControlTemplate TargetType=\"Button\">", toastStyle);
        Assert.Contains("PART_CloseSurface", toastStyle);
        Assert.Contains("PART_CloseIcon", toastStyle);
        Assert.Contains("PART_CloseContent", toastStyle);
    }

    [Fact]
    public void OverlayCloseButtonsOwnScopedHoverPressedAndMotionChrome()
    {
        var root = FindRepositoryRoot();
        var overlayStyles = new[]
        {
            ("Dialog", ReadStyle(root, "Dialog")),
            ("Popover", ReadStyle(root, "Popover")),
            ("Sheet", ReadStyle(root, "Sheet")),
            ("Drawer", ReadStyle(root, "Drawer")),
            ("CommandDialog", ReadStyle(root, "CommandDialog")),
            ("Toast", ReadStyle(root, "Toast"))
        };

        foreach (var (component, style) in overlayStyles)
        {
            var closeTemplatePath = component == "Popover" ? "Popup#PART_Popup " : string.Empty;

            Assert.Contains("<Button x:Name=\"PART_Close\"", style);
            Assert.Contains("FocusAdorner=\"{x:Null}\"", style);
            Assert.Contains("<ControlTemplate TargetType=\"Button\">", style);
            Assert.Contains("PART_CloseSurface", style);
            Assert.Contains("PART_CloseIcon", style);
            Assert.Contains("PART_CloseContent", style);
            Assert.Contains($"controls|Codex{component} /template/ {closeTemplatePath}Button#PART_Close", style);
            Assert.Contains($"controls|Codex{component} /template/ {closeTemplatePath}Button#PART_Close:pointerover", style);
            Assert.Contains($"controls|Codex{component} /template/ {closeTemplatePath}Button#PART_Close:pressed", style);
            Assert.Contains($"controls|Codex{component}.has-close-content /template/ {closeTemplatePath}PathIcon#PART_CloseIcon", style);
            Assert.Contains($"controls|Codex{component}.has-close-content /template/ {closeTemplatePath}ContentPresenter#PART_CloseContent", style);
            Assert.Contains("DoubleTransition Property=\"Opacity\" Duration=\"{DynamicResource CodexSwitch.MotionDurationDefault}", style);
            Assert.Contains("BrushTransition Property=\"Background\" Duration=\"{DynamicResource CodexSwitch.MotionDurationDefault}", style);
            Assert.Contains("CodexSwitch.MotionEaseOut", style);
            Assert.DoesNotContain("<Style Selector=\"Button#PART_Close", style);
            Assert.DoesNotContain("BasedOn=", style);
            Assert.DoesNotContain("Avalonia.Themes.Fluent", style);
            Assert.DoesNotContain("FluentTheme", style);
        }
    }

    [Fact]
    public void AvatarGroupOwnsIndependentStyleFileAndCountChrome()
    {
        var root = FindRepositoryRoot();
        var avatarStyle = ReadStyle(root, "Avatar");
        var avatarGroupStyle = ReadStyle(root, "AvatarGroup");
        var theme = File.ReadAllText(Path.Combine(root, "src", "CodexSwitchUI", "Themes", "ComponentStyles.axaml"));

        Assert.DoesNotContain("CodexAvatarGroup", avatarStyle);
        Assert.Contains("Themes/Controls/Avatar.axaml", theme);
        Assert.Contains("Themes/Controls/AvatarGroup.axaml", theme);

        Assert.Contains("controls|CodexAvatarGroup", avatarGroupStyle);
        Assert.Contains("controls|CodexAvatarGroup:disabled", avatarGroupStyle);
        Assert.Contains("controls|CodexAvatarGroup.size-sm", avatarGroupStyle);
        Assert.Contains("controls|CodexAvatarGroup.size-lg", avatarGroupStyle);
        Assert.Contains("controls|CodexAvatarGroup.size-icon", avatarGroupStyle);
        Assert.Contains("controls|CodexAvatar.avatar-group-item /template/ Border#PART_Surface", avatarGroupStyle);
        Assert.Contains("controls|CodexAvatarGroupCount", avatarGroupStyle);
        Assert.Contains("<ControlTemplate TargetType=\"controls:CodexAvatarGroupCount\">", avatarGroupStyle);
        Assert.Contains("PART_CountSurface", avatarGroupStyle);
        Assert.Contains("PART_CountContent", avatarGroupStyle);
        Assert.Contains("CodexSwitch.MotionDurationDefault", avatarGroupStyle);
        Assert.Contains("CodexSwitch.MotionEaseOut", avatarGroupStyle);
        Assert.DoesNotContain("BasedOn=", avatarGroupStyle);
        Assert.DoesNotContain("Avalonia.Themes.Fluent", avatarGroupStyle);
        Assert.DoesNotContain("FluentTheme", avatarGroupStyle);
    }

    [Fact]
    public void DatePickerAndComboboxRawButtonPartsOwnScopedChromeAndMotion()
    {
        var root = FindRepositoryRoot();
        var datePicker = ReadStyle(root, "DatePicker");
        var combobox = ReadStyle(root, "Combobox");

        foreach (var (component, style) in new[] { ("DatePicker", datePicker), ("Combobox", combobox) })
        {
            Assert.Contains("<Button x:Name=\"PART_Clear\"", style);
            Assert.Contains("<Button x:Name=\"PART_Trigger\"", style);
            Assert.Contains("<ControlTemplate TargetType=\"Button\">", style);
            Assert.Contains($"controls|Codex{component} /template/ Button#PART_Clear", style);
            Assert.Contains($"controls|Codex{component} /template/ Button#PART_Trigger", style);
            Assert.Contains("PART_ClearSurface", style);
            Assert.Contains("PART_TriggerSurface", style);
            Assert.Contains("PART_ClearIcon", style);
            Assert.Contains("PART_Chevron", style);
            Assert.Contains("CodexSwitch.MotionDurationDefault", style);
            Assert.Contains("CodexSwitch.MotionDurationFast", style);
            Assert.Contains("CodexSwitch.MotionEaseOut", style);
            Assert.DoesNotContain("<Style Selector=\"Button#PART_Clear", style);
            Assert.DoesNotContain("<Style Selector=\"Button#PART_Trigger", style);
            Assert.DoesNotContain("BasedOn=", style);
            Assert.DoesNotContain("Avalonia.Themes.Fluent", style);
            Assert.DoesNotContain("FluentTheme", style);
        }

        Assert.Contains("controls|CodexDatePicker /template/ Border#PART_ClearSurface", datePicker);
        Assert.Contains("controls|CodexDatePicker /template/ Border#PART_TriggerSurface", datePicker);
        Assert.Contains("controls|CodexDatePicker /template/ PathIcon#PART_CalendarIcon", datePicker);
        Assert.Contains("controls|CodexDatePicker /template/ PathIcon#PART_ClearIcon", datePicker);
        Assert.Contains("controls|CodexDatePicker /template/ PathIcon#PART_Chevron", datePicker);
        Assert.Contains("controls|CodexDatePicker /template/ Button#PART_Clear:pointerover", datePicker);
        Assert.Contains("controls|CodexDatePicker /template/ Button#PART_Clear:pressed", datePicker);
        Assert.Contains("TransformOperationsTransition Property=\"RenderTransform\"", datePicker);

        Assert.Contains("controls|CodexCombobox /template/ Border#PART_ClearSurface", combobox);
        Assert.Contains("controls|CodexCombobox /template/ Border#PART_TriggerSurface", combobox);
        Assert.Contains("controls|CodexCombobox /template/ PathIcon#PART_ClearIcon", combobox);
        Assert.Contains("controls|CodexCombobox /template/ PathIcon#PART_Chevron", combobox);
        Assert.Contains("controls|CodexCombobox /template/ Button#PART_Clear:pointerover", combobox);
        Assert.Contains("controls|CodexCombobox /template/ Button#PART_Clear:pressed", combobox);
    }

    [Fact]
    public void DropdownListScrollViewersOwnScopedTemplatesAndThumbMotion()
    {
        var root = FindRepositoryRoot();
        var dropdownStyles = new[]
        {
            ("Select", ReadStyle(root, "Select")),
            ("NativeSelect", ReadStyle(root, "NativeSelect")),
            ("Combobox", ReadStyle(root, "Combobox"))
        };

        foreach (var (component, style) in dropdownStyles)
        {
            Assert.Contains("<ScrollViewer x:Name=\"PART_Scroll\"", style);
            Assert.Contains($"controls|Codex{component} /template/ ScrollViewer#PART_Scroll", style);
            Assert.Contains("<ControlTemplate TargetType=\"ScrollViewer\">", style);
            Assert.Contains("PART_ScrollRoot", style);
            Assert.Contains("PART_ContentPresenter", style);
            Assert.Contains("PART_HorizontalScrollBar", style);
            Assert.Contains("PART_VerticalScrollBar", style);
            Assert.Contains($"controls|Codex{component} /template/ ScrollViewer#PART_Scroll ScrollBar", style);
            Assert.Contains($"controls|Codex{component} /template/ ScrollViewer#PART_Scroll ScrollBar:vertical", style);
            Assert.Contains($"controls|Codex{component} /template/ ScrollViewer#PART_Scroll ScrollBar:horizontal", style);
            Assert.Contains("<ControlTemplate TargetType=\"ScrollBar\">", style);
            Assert.Contains("PART_ScrollBarRoot", style);
            Assert.Contains("PART_Track", style);
            Assert.Contains("PART_PageUpButton", style);
            Assert.Contains("PART_PageDownButton", style);
            Assert.Contains("PART_PageLeftButton", style);
            Assert.Contains("PART_PageRightButton", style);
            Assert.Contains("Thumb#PART_Thumb", style);
            Assert.Contains("PART_ThumbSurface", style);
            Assert.Contains($"controls|Codex{component} /template/ ScrollViewer#PART_Scroll ScrollBar Thumb#PART_Thumb:pointerover", style);
            Assert.Contains("DoubleTransition Property=\"Opacity\" Duration=\"{DynamicResource CodexSwitch.MotionDurationDefault}", style);
            Assert.Contains("BrushTransition Property=\"Background\" Duration=\"{DynamicResource CodexSwitch.MotionDurationDefault}", style);
            Assert.Contains("CodexSwitch.MotionEaseOut", style);
            Assert.DoesNotContain("<Style Selector=\"ScrollViewer", style);
            Assert.DoesNotContain("<Style Selector=\"ScrollBar", style);
            Assert.DoesNotContain("BasedOn=", style);
            Assert.DoesNotContain("Avalonia.Themes.Fluent", style);
            Assert.DoesNotContain("FluentTheme", style);
        }
    }

    [Fact]
    public void TextInputStylesUseOnePlaceholderForegroundAlias()
    {
        var root = FindRepositoryRoot();
        var textInputStyles = new[] { "Input", "Textarea", "Select", "NativeSelect", "Command" };

        foreach (var component in textInputStyles)
        {
            var style = ReadStyle(root, component);

            Assert.Contains("PlaceholderForeground", style);
            Assert.DoesNotContain("WatermarkForeground", style);
        }
    }

    [Fact]
    public void TextInputsOwnPresenterPlaceholderSelectionAndFocusChrome()
    {
        var root = FindRepositoryRoot();
        var input = ReadStyle(root, "Input");
        var textarea = ReadStyle(root, "Textarea");

        foreach (var (component, style, target) in new[]
        {
            ("Input", input, "controls:CodexTextBox"),
            ("Textarea", textarea, "controls:CodexTextarea")
        })
        {
            Assert.Contains($"<ControlTemplate TargetType=\"{target}\">", style);
            Assert.Contains("FocusAdorner\" Value=\"{x:Null}", style);
            Assert.Contains("DataValidationErrors", style);
            Assert.Contains("PART_BorderElement", style);
            Assert.Contains("PART_ScrollViewer", style);
            Assert.Contains("PART_Placeholder", style);
            Assert.Contains("PART_TextPresenter", style);
            Assert.Contains("PART_FocusRing", style);
            Assert.Contains("PlaceholderForeground", style);
            Assert.Contains("SelectionBrush", style);
            Assert.Contains("SelectionForegroundBrush", style);
            Assert.Contains("CaretBrush", style);
            Assert.Contains("CodexSwitch.MotionDurationFast", style);
            Assert.Contains(":focus-visible /template/ Border#PART_FocusRing", style);
            Assert.DoesNotContain("WatermarkForeground", style);
            Assert.DoesNotContain("PART_ContentHost", style);
            Assert.DoesNotContain("PART_Watermark", style);
            Assert.DoesNotContain("BasedOn=", style);
            Assert.DoesNotContain("Avalonia.Themes.Fluent", style);
            Assert.DoesNotContain("FluentTheme", style);

            if (component == "Textarea")
            {
                Assert.Contains("AcceptsReturn\" Value=\"True\"", style);
                Assert.Contains("TextWrapping\" Value=\"Wrap\"", style);
                Assert.Contains("textarea-tall", style);
            }
        }
    }

    [Fact]
    public void PopupFormControlsPublishOpenChangedSourceMetadata()
    {
        var root = FindRepositoryRoot();
        var controls = Path.Combine(root, "src", "CodexSwitchUI", "Controls");
        var select = File.ReadAllText(Path.Combine(controls, "CodexSelect.cs"));
        var nativeSelect = File.ReadAllText(Path.Combine(controls, "CodexNativeSelect.cs"));
        var combobox = File.ReadAllText(Path.Combine(controls, "CodexCombobox.cs"));

        Assert.Contains("public enum CodexSelectOpenChangeSource", select);
        Assert.Contains("public CodexSelectOpenChangeSource Source { get; } = source;", select);
        Assert.Contains("internal bool SetDropDownOpen(", select);
        Assert.Contains("_nextOpenChangeSource = CodexSelectOpenChangeSource.Pointer;", select);
        Assert.Contains("_nextOpenChangeSource = CodexSelectOpenChangeSource.Keyboard;", select);
        Assert.Contains("OpenChanged?.Invoke(this, new CodexSelectOpenChangedEventArgs(IsDropDownOpen, source));", select);

        Assert.Contains("public enum CodexNativeSelectOpenChangeSource", nativeSelect);
        Assert.Contains("public CodexNativeSelectOpenChangeSource Source { get; } = source;", nativeSelect);
        Assert.Contains("internal bool SetDropDownOpen(", nativeSelect);
        Assert.Contains("_nextOpenChangeSource = CodexNativeSelectOpenChangeSource.Pointer;", nativeSelect);
        Assert.Contains("_nextOpenChangeSource = CodexNativeSelectOpenChangeSource.Keyboard;", nativeSelect);
        Assert.Contains("OpenChanged?.Invoke(this, new CodexNativeSelectOpenChangedEventArgs(IsDropDownOpen, source));", nativeSelect);

        Assert.Contains("public enum CodexComboboxOpenChangeSource", combobox);
        Assert.Contains("public CodexComboboxOpenChangeSource Source { get; } = source;", combobox);
        Assert.Contains("Open(CodexComboboxOpenChangeSource.Keyboard)", combobox);
        Assert.Contains("Close(CodexComboboxOpenChangeSource.Keyboard)", combobox);
        Assert.Contains("TogglePopup(CodexComboboxOpenChangeSource.Pointer)", combobox);
        Assert.Contains("Open(CodexComboboxOpenChangeSource.Input)", combobox);
        Assert.Contains("Open(CodexComboboxOpenChangeSource.Focus)", combobox);
        Assert.Contains("Open(CodexComboboxOpenChangeSource.Clear)", combobox);
        Assert.Contains("_suppressOpenOnTextChange", combobox);
        Assert.Contains("Close(ToOpenChangeSource(source));", combobox);
        Assert.Contains("OpenChanged?.Invoke(this, new CodexComboboxOpenChangedEventArgs(IsOpen, CurrentOpenChangeSource));", combobox);
    }

    [Fact]
    public void IconAndSplitButtonsOwnButtonAndPopupChrome()
    {
        var root = FindRepositoryRoot();
        var controls = Path.Combine(root, "src", "CodexSwitchUI", "Controls");
        var navigationPrimitives = File.ReadAllText(Path.Combine(controls, "CodexNavigationPrimitives.cs"));
        var splitButton = File.ReadAllText(Path.Combine(controls, "CodexSplitButton.cs"));
        var buttonStyle = ReadStyle(root, "Button");
        var splitButtonStyle = ReadStyle(root, "SplitButton");
        var applicationShellStyle = ReadStyle(root, "ApplicationShell");

        Assert.Contains("public class CodexIconButton : CodexButton", navigationPrimitives);
        Assert.Contains("Size = CodexControlSize.Icon", navigationPrimitives);
        Assert.Contains("Classes.Set(\"round\", IsRound)", navigationPrimitives);
        Assert.Contains("controls|CodexIconButton", applicationShellStyle);
        Assert.Contains("controls|CodexIconButton.round", applicationShellStyle);
        Assert.Contains("<ControlTemplate TargetType=\"controls:CodexButton\">", buttonStyle);
        Assert.Contains("PART_Root", buttonStyle);
        Assert.Contains("PART_FocusRing", buttonStyle);
        Assert.Contains("PART_LoadingIndicator", buttonStyle);
        Assert.Contains("controls|CodexButton.size-icon", buttonStyle);

        Assert.Contains("public enum CodexSplitButtonOpenChangeSource", splitButton);
        Assert.Contains("public CodexSplitButtonOpenChangeSource Source { get; } = source;", splitButton);
        Assert.Contains("Open(CodexSplitButtonOpenChangeSource.Keyboard)", splitButton);
        Assert.Contains("Dismiss(CodexSplitButtonOpenChangeSource.Keyboard)", splitButton);
        Assert.Contains("Toggle(CodexSplitButtonOpenChangeSource.Pointer)", splitButton);
        Assert.Contains("Dismiss(CodexSplitButtonOpenChangeSource.Selection)", splitButton);
        Assert.Contains("<ControlTemplate TargetType=\"controls:CodexSplitButton\">", splitButtonStyle);
        Assert.Contains("controls:CodexButton x:Name=\"PART_PrimaryAction\"", splitButtonStyle);
        Assert.Contains("controls:CodexButton x:Name=\"PART_MenuTrigger\"", splitButtonStyle);
        Assert.Contains("PART_Divider", splitButtonStyle);
        Assert.Contains("PART_Chevron", splitButtonStyle);
        Assert.Contains("Popup x:Name=\"PART_Popup\"", splitButtonStyle);
        Assert.Contains("PART_Surface", splitButtonStyle);
        Assert.Contains("PART_Arrow", splitButtonStyle);
        Assert.Contains("IsLightDismissEnabled=\"True\"", splitButtonStyle);
        Assert.Contains("TransformOperationsTransition", splitButtonStyle);
        Assert.Contains("controls|CodexSplitButton.open /template/ Border#PART_Surface", splitButtonStyle);
        Assert.Contains("controls|CodexSplitButton:not(.can-open-dropdown) /template/ controls|CodexButton#PART_MenuTrigger", splitButtonStyle);
        Assert.Contains("TryHandleMenuTriggerKey(Key key)", splitButton);
        Assert.Contains("key is not (Key.Enter or Key.Space or Key.Down)", splitButton);
        Assert.Contains("TryHandleMenuTriggerPointerRelease(PointerUpdateKind updateKind)", splitButton);
        Assert.Contains("OnMenuTriggerPointerReleased", splitButton);
        Assert.Contains("OnMenuTriggerKeyDown", splitButton);
        Assert.Contains("InputElement.PointerReleasedEvent", splitButton);
        Assert.Contains("InputElement.KeyDownEvent", splitButton);

        foreach (var style in new[] { buttonStyle, splitButtonStyle, applicationShellStyle })
        {
            Assert.DoesNotContain("<Style Selector=\"Button", style);
            Assert.DoesNotContain("BasedOn=", style);
            Assert.DoesNotContain("Avalonia.Themes.Fluent", style);
            Assert.DoesNotContain("FluentTheme", style);
        }
    }

    [Fact]
    public void CalendarDayButtonsGateCommandActivationBeforeSelection()
    {
        var root = FindRepositoryRoot();
        var source = File.ReadAllText(Path.Combine(root, "src", "CodexSwitchUI", "Controls", "CodexCalendar.cs"));
        var style = ReadStyle(root, "Calendar");

        Assert.Contains("IsEnabledProperty.Changed.AddClassHandler<CodexCalendar>((calendar, _) => calendar.SyncDayStates());", source);
        Assert.Contains("internal bool CanSelectDate(DateTime date)", source);
        Assert.Contains("public class CodexCalendarDayButton : Button", source);
        Assert.Contains("CommandProperty.Changed.AddClassHandler<CodexCalendarDayButton>((button, args) => button.OnCommandChanged", source);
        Assert.Contains("internal bool CanActivate => IsEnabled", source);
        Assert.Contains("owner.SelectDate(Date, owner.CurrentChangeSource);", source);
        Assert.Contains("CodexCalendarChangeSource.Pointer", source);
        Assert.Contains("CodexCalendarChangeSource.Keyboard", source);
        Assert.Contains("Classes.Set(\"can-activate\", CanActivate);", source);
        Assert.Contains("Classes.Set(\"command-blocked\", Command is not null && IsEnabled && !IsBlank && !IsUnavailable && !CanExecuteCommand());", source);
        Assert.Contains("controls|CodexCalendarDayButton.command-blocked", style);
        Assert.Contains("controls|CodexCalendarDayButton.command-blocked:pointerover", style);
        Assert.Contains("controls|CodexCalendarDayButton.command-blocked:pressed", style);
    }

    [Fact]
    public void ChoiceToggleAndRangeControlsOwnPartsEventsAndNativeChrome()
    {
        var root = FindRepositoryRoot();
        var controls = Path.Combine(root, "src", "CodexSwitchUI", "Controls");
        var radio = File.ReadAllText(Path.Combine(controls, "CodexRadio.cs"));
        var switchControl = File.ReadAllText(Path.Combine(controls, "CodexSwitch.cs"));
        var toggle = File.ReadAllText(Path.Combine(controls, "CodexToggle.cs"));
        var slider = File.ReadAllText(Path.Combine(controls, "CodexSlider.cs"));
        var radioStyle = ReadStyle(root, "Radio");
        var switchStyle = ReadStyle(root, "Switch");
        var toggleStyle = ReadStyle(root, "Toggle");
        var sliderStyle = ReadStyle(root, "Slider");

        Assert.Contains("public class CodexRadio : RadioButton", radio);
        Assert.Contains("[PseudoClasses(CodexFocusVisible.PseudoClass)]", radio);
        Assert.Contains("CodexClassSync.SetIntent(Classes, Intent)", radio);
        Assert.Contains("CodexClassSync.SetSize(Classes, Size)", radio);
        Assert.Contains("PseudoClasses.Set(CodexFocusVisible.PseudoClass, false);", radio);
        Assert.Contains("<ControlTemplate TargetType=\"controls:CodexRadio\">", radioStyle);
        Assert.Contains("PART_Ring", radioStyle);
        Assert.Contains("PART_Dot", radioStyle);
        Assert.Contains("PART_FocusRing", radioStyle);
        Assert.Contains(":focus-visible /template/ Border#PART_FocusRing", radioStyle);

        Assert.Contains("public event EventHandler<CodexSwitchCheckedChangedEventArgs>? CheckedChanged;", switchControl);
        Assert.Contains("public enum CodexSwitchCheckedChangeSource", switchControl);
        Assert.Contains("public CodexSwitchCheckedChangeSource Source { get; }", switchControl);
        Assert.Contains("internal bool SetChecked(bool isChecked, CodexSwitchCheckedChangeSource source)", switchControl);
        Assert.Contains("IsCheckedProperty.Changed.AddClassHandler<CodexSwitch>", switchControl);
        Assert.Contains("HasContentProperty", switchControl);
        Assert.Contains("PseudoClasses.Set(CodexFocusVisible.PseudoClass, false);", switchControl);
        Assert.Contains("<ControlTemplate TargetType=\"controls:CodexSwitch\">", switchStyle);
        Assert.Contains("PART_Track", switchStyle);
        Assert.Contains("PART_Thumb", switchStyle);
        Assert.Contains("PART_FocusRing", switchStyle);
        Assert.Contains("PART_Content", switchStyle);
        Assert.Contains("controls|CodexSwitch:checked /template/ Border#PART_Thumb", switchStyle);

        Assert.Contains("public class CodexToggle : ToggleButton", toggle);
        Assert.Contains("public event EventHandler<CodexTogglePressedChangedEventArgs>? PressedChanged;", toggle);
        Assert.Contains("public enum CodexTogglePressedChangeSource", toggle);
        Assert.Contains("public CodexTogglePressedChangeSource Source { get; }", toggle);
        Assert.Contains("internal bool SetPressedState(bool isPressed, CodexTogglePressedChangeSource source)", toggle);
        Assert.Contains("TryHandleActivationKey(Key key)", toggle);
        Assert.Contains("Classes.Set(\"state-on\", pressed);", toggle);
        Assert.Contains("public partial class CodexToggleGroup : ItemsControl", toggle);
        Assert.Contains("public enum CodexToggleGroupValueChangeSource", toggle);
        Assert.Contains("public CodexToggleGroupValueChangeSource Source { get; }", toggle);
        Assert.Contains("private CodexToggleGroupValueChangeSource? _pendingValueChangeSource;", toggle);
        Assert.Contains("internal bool ToggleItem(CodexToggleGroupItem item, CodexToggleGroupValueChangeSource source)", toggle);
        Assert.Contains("ValueChanged?.Invoke(this, new CodexToggleGroupValueChangedEventArgs(oldValue, nextValue, oldValues, nextValues, source));", toggle);
        Assert.Contains("public class CodexToggleGroupItem : CodexToggle", toggle);
        Assert.Contains("TryHandlePointerActivation(PointerUpdateKind updateKind)", toggle);
        Assert.Contains("PointerUpdateKind.LeftButtonReleased", toggle);
        Assert.Contains("group.ToggleItem(this, CodexToggleGroupValueChangeSource.Keyboard)", toggle);
        Assert.Contains("group.ToggleItem(this, CodexToggleGroupValueChangeSource.Pointer)", toggle);
        Assert.Contains("<ControlTemplate TargetType=\"controls:CodexToggle\">", toggleStyle);
        Assert.Contains("PART_Root", toggleStyle);
        Assert.Contains("PART_Content", toggleStyle);
        Assert.Contains("PART_FocusRing", toggleStyle);
        Assert.Contains("<ControlTemplate TargetType=\"controls:CodexToggleGroup\">", toggleStyle);
        Assert.Contains("PART_ItemsPresenter", toggleStyle);

        Assert.Contains("public event EventHandler<CodexSliderValueChangingEventArgs>? ValueChanging;", slider);
        Assert.Contains("public event EventHandler<CodexSliderValueCommittedEventArgs>? ValueCommitted;", slider);
        Assert.Contains("TryBeginPointerChange(PointerUpdateKind updateKind)", slider);
        Assert.Contains("TryCommitPointerValue(PointerUpdateKind updateKind)", slider);
        Assert.Contains("PointerUpdateKind.LeftButtonPressed", slider);
        Assert.Contains("PointerUpdateKind.LeftButtonReleased", slider);
        Assert.Contains("CommitValue(\"pointer\")", slider);
        Assert.Contains("CommitValue(\"keyboard\")", slider);
        Assert.Contains("Classes.Set(\"dragging\", true);", slider);
        Assert.Contains("Classes.Set(\"dragging\", false);", slider);
        Assert.Contains("Classes.Set(\"has-value\", Value > Minimum);", slider);
        Assert.Contains("<ControlTemplate TargetType=\"controls:CodexSlider\">", sliderStyle);
        Assert.Contains("PART_SliderRoot", sliderStyle);
        Assert.Contains("PART_TrackBackground", sliderStyle);
        Assert.Contains("PART_TrackFill", sliderStyle);
        Assert.Contains("PART_DecreaseButton", sliderStyle);
        Assert.Contains("PART_IncreaseButton", sliderStyle);
        Assert.Contains("PART_Thumb", sliderStyle);
        Assert.Contains("controls|CodexSlider:focus-visible /template/ Border#PART_SliderRoot", sliderStyle);

        foreach (var (component, style) in new[]
        {
            ("Radio", radioStyle),
            ("Switch", switchStyle),
            ("Toggle", toggleStyle),
            ("Slider", sliderStyle)
        })
        {
            Assert.Contains("CodexSwitch.MotionDuration", style);
            Assert.Contains("CodexSwitch.MotionEaseOut", style);
            Assert.Contains("CodexSwitch.DisabledOpacity", style);
            Assert.DoesNotContain("BasedOn=", style);
            Assert.DoesNotContain("Avalonia.Themes.Fluent", style);
            Assert.DoesNotContain("FluentTheme", style);
            Assert.DoesNotContain($"<Style Selector=\"{component}", style);
        }
    }

    [Fact]
    public void NavigationPrimitivesOwnSelectionDisclosureAndSeparatorChrome()
    {
        var root = FindRepositoryRoot();
        var controls = Path.Combine(root, "src", "CodexSwitchUI", "Controls");
        var navigationPrimitives = File.ReadAllText(Path.Combine(controls, "CodexNavigationPrimitives.cs"));
        var dropdown = File.ReadAllText(Path.Combine(controls, "CodexDropdownButton.cs"));
        var collapsible = File.ReadAllText(Path.Combine(controls, "CodexCollapsible.cs"));
        var separator = File.ReadAllText(Path.Combine(controls, "CodexSeparator.cs"));
        var applicationShellStyle = ReadStyle(root, "ApplicationShell");
        var dropdownStyle = ReadStyle(root, "DropdownButton");
        var collapsibleStyle = ReadStyle(root, "Collapsible");
        var separatorStyle = ReadStyle(root, "Separator");

        Assert.Contains("public class CodexSideNav : ContentControl", navigationPrimitives);
        Assert.Contains("public event EventHandler<CodexSideNavValueChangedEventArgs>? ValueChanged;", navigationPrimitives);
        Assert.Contains("public enum CodexSideNavValueChangeSource", navigationPrimitives);
        Assert.Contains("public CodexSideNavValueChangeSource Source { get; } = source;", navigationPrimitives);
        Assert.Contains("internal bool SelectItem(CodexSideNavItem item, CodexSideNavValueChangeSource source = CodexSideNavValueChangeSource.Programmatic)", navigationPrimitives);
        Assert.Contains("RaiseValueChanged(oldItem, item, oldIndex, newIndex, oldValue, newValue, source);", navigationPrimitives);
        Assert.Contains("public class CodexSideNavItem : Button", navigationPrimitives);
        Assert.Contains("CommandProperty.Changed.AddClassHandler<CodexSideNavItem>((item, args) => item.OnCommandChanged", navigationPrimitives);
        Assert.Contains("internal bool CanSelect => IsEnabled && (Command?.CanExecute(CommandParameter) ?? true);", navigationPrimitives);
        Assert.Contains("internal bool TryHandlePointerActivation(PointerUpdateKind updateKind)", navigationPrimitives);
        Assert.Contains("TrySelect(CodexSideNavValueChangeSource.Pointer)", navigationPrimitives);
        Assert.Contains("TrySelect(CodexSideNavValueChangeSource.Keyboard)", navigationPrimitives);
        Assert.Contains("PointerUpdateKind.LeftButtonReleased", navigationPrimitives);
        Assert.Contains("protected override void OnPointerPressed(PointerPressedEventArgs e)", navigationPrimitives);
        Assert.Contains("protected override void OnPointerReleased(PointerReleasedEventArgs e)", navigationPrimitives);
        Assert.Contains("_hasPrimaryPointerPress && IsPointerOver", navigationPrimitives);
        Assert.Contains("return owner.SelectItem(this, source);", navigationPrimitives);
        Assert.Contains("Classes.Set(\"selected\", IsSelected);", navigationPrimitives);
        Assert.Contains("Classes.Set(\"can-select\", CanSelect);", navigationPrimitives);
        Assert.Contains("Classes.Set(\"command-blocked\", Command is not null && IsEnabled && !CanSelect);", navigationPrimitives);
        Assert.Contains("<ControlTemplate TargetType=\"controls:CodexSideNav\">", applicationShellStyle);
        Assert.Contains("<ControlTemplate TargetType=\"controls:CodexSideNavItem\">", applicationShellStyle);
        Assert.Contains("PART_Root", applicationShellStyle);
        Assert.Contains("HasIcon", applicationShellStyle);
        Assert.Contains("HasDetail", applicationShellStyle);
        Assert.Contains("controls|CodexSideNavItem.selected", applicationShellStyle);
        Assert.Contains("controls|CodexSideNavItem.command-blocked", applicationShellStyle);

        Assert.Contains("public class CodexSegmentedControl : ContentControl", navigationPrimitives);
        Assert.Contains("public enum CodexSegmentedControlValueChangeSource", navigationPrimitives);
        Assert.Contains("public CodexSegmentedControlValueChangeSource Source { get; } = source;", navigationPrimitives);
        Assert.Contains("IndicatorWidthProperty", navigationPrimitives);
        Assert.Contains("IndicatorMarginProperty", navigationPrimitives);
        Assert.Contains("QueueSelectionIndicatorUpdate()", navigationPrimitives);
        Assert.Contains("internal bool SelectButton(", navigationPrimitives);
        Assert.Contains("CodexSegmentedControlValueChangeSource source = CodexSegmentedControlValueChangeSource.Programmatic", navigationPrimitives);
        Assert.Contains("RaiseValueChanged(oldButton, button, oldIndex, newIndex, oldValue, newValue, source);", navigationPrimitives);
        Assert.Contains("public class CodexSegmentedButton : Button", navigationPrimitives);
        Assert.Contains("CommandProperty.Changed.AddClassHandler<CodexSegmentedButton>((button, args) => button.OnCommandChanged", navigationPrimitives);
        Assert.Contains("internal bool CanSelect => IsEnabled && (Command?.CanExecute(CommandParameter) ?? true);", navigationPrimitives);
        Assert.Contains("TrySelect(CodexSegmentedControlValueChangeSource.Pointer)", navigationPrimitives);
        Assert.Contains("TrySelect(CodexSegmentedControlValueChangeSource.Keyboard)", navigationPrimitives);
        Assert.Contains("return owner.SelectButton(this, source);", navigationPrimitives);
        Assert.Contains("if (updateKind != PointerUpdateKind.LeftButtonReleased || Command is not null)", navigationPrimitives);
        Assert.Contains("Classes.Set(\"can-select\", CanSelect);", navigationPrimitives);
        Assert.Contains("Classes.Set(\"command-blocked\", Command is not null && IsEnabled && !CanSelect);", navigationPrimitives);
        Assert.Contains("<ControlTemplate TargetType=\"controls:CodexSegmentedControl\">", applicationShellStyle);
        Assert.Contains("PART_IndicatorHost", applicationShellStyle);
        Assert.Contains("PART_Indicator", applicationShellStyle);
        Assert.Contains("<ControlTemplate TargetType=\"controls:CodexSegmentedButton\">", applicationShellStyle);
        Assert.Contains("controls|CodexSegmentedButton.selected", applicationShellStyle);
        Assert.Contains("controls|CodexSegmentedButton.command-blocked", applicationShellStyle);
        Assert.Contains("CodexSwitch.MotionDurationSlow", applicationShellStyle);

        Assert.Contains("public event EventHandler<CodexDropdownButtonOpenChangedEventArgs>? OpenChanged;", dropdown);
        Assert.Contains("public enum CodexDropdownButtonOpenChangeSource", dropdown);
        Assert.Contains("public CodexDropdownButtonOpenChangeSource Source { get; } = source;", dropdown);
        Assert.Contains("Open(CodexDropdownButtonOpenChangeSource.Keyboard)", dropdown);
        Assert.Contains("Dismiss(CodexDropdownButtonOpenChangeSource.Keyboard)", dropdown);
        Assert.Contains("Toggle(CodexDropdownButtonOpenChangeSource.Pointer)", dropdown);
        Assert.Contains("Dismiss(CodexDropdownButtonOpenChangeSource.Selection)", dropdown);
        Assert.Contains("public event EventHandler<RestoreFocusRequestedEventArgs>? RestoreFocusRequested;", dropdown);
        Assert.Contains("TryCloseFromDropDownAction", dropdown);
        Assert.Contains("TryCloseFromDropDownMenuItem", dropdown);
        Assert.Contains("TryHandleTriggerKey(Key key)", dropdown);
        Assert.Contains("key is not (Key.Enter or Key.Space or Key.Down)", dropdown);
        Assert.Contains("TryHandleTriggerPointerRelease(PointerUpdateKind updateKind)", dropdown);
        Assert.Contains("OnTriggerPointerReleased", dropdown);
        Assert.Contains("OnTriggerKeyDown", dropdown);
        Assert.Contains("InputElement.PointerReleasedEvent", dropdown);
        Assert.Contains("InputElement.KeyDownEvent", dropdown);
        Assert.Contains("TryRestoreFocus()", dropdown);
        Assert.Contains("<ControlTemplate TargetType=\"controls:CodexDropdownButton\">", dropdownStyle);
        Assert.Contains("PART_Trigger", dropdownStyle);
        Assert.Contains("PART_Chevron", dropdownStyle);
        Assert.Contains("Popup x:Name=\"PART_Popup\"", dropdownStyle);
        Assert.Contains("PART_Surface", dropdownStyle);
        Assert.Contains("PART_DropDownContent", dropdownStyle);
        Assert.Contains("PART_Arrow", dropdownStyle);
        Assert.Contains("IsLightDismissEnabled=\"True\"", dropdownStyle);

        Assert.Contains("public event EventHandler<CodexCollapsibleOpenChangedEventArgs>? OpenChanged;", collapsible);
        Assert.Contains("public enum CodexCollapsibleOpenChangeSource", collapsible);
        Assert.Contains("public CodexCollapsibleOpenChangeSource Source", collapsible);
        Assert.Contains("TryHandleTriggerKey(Key key)", collapsible);
        Assert.Contains("TryHandleTriggerPointerRelease(PointerUpdateKind updateKind)", collapsible);
        Assert.Contains("Toggle(CodexCollapsibleOpenChangeSource.Keyboard)", collapsible);
        Assert.Contains("Toggle(CodexCollapsibleOpenChangeSource.Pointer)", collapsible);
        Assert.Contains("PointerUpdateKind.LeftButtonReleased", collapsible);
        Assert.Contains("Properties.PointerUpdateKind", collapsible);
        Assert.Contains("InputElement.PointerReleasedEvent", collapsible);
        Assert.DoesNotContain("InputElement.PointerPressedEvent, OnTriggerPointerPressed", collapsible);
        Assert.Contains("RequestContentMeasure()", collapsible);
        Assert.Contains("StopHeightAnimation()", collapsible);
        Assert.Contains("AnimationDurationProperty", collapsible);
        Assert.Contains("<ControlTemplate TargetType=\"controls:CodexCollapsible\">", collapsibleStyle);
        Assert.Contains("PART_TriggerLayout", collapsibleStyle);
        Assert.Contains("PART_Trigger", collapsibleStyle);
        Assert.Contains("PART_Chevron", collapsibleStyle);
        Assert.Contains("PART_ContentClip", collapsibleStyle);
        Assert.Contains("PART_ContentMeasure", collapsibleStyle);
        Assert.Contains("PART_ContentPresenter", collapsibleStyle);
        Assert.Contains("Property=\"AnimationDuration\" Value=\"{DynamicResource CodexSwitch.MotionDurationSlow}\"", collapsibleStyle);

        Assert.Contains("public class CodexSeparator : TemplatedControl", separator);
        Assert.Contains("OrientationProperty", separator);
        Assert.Contains("CodexClassSync.SetSize(Classes, Size);", separator);
        Assert.Contains("<Style Selector=\"controls|CodexSeparator\">", separatorStyle);
        Assert.Contains("PART_Line", separatorStyle);
        Assert.Contains("controls|CodexSeparator.vertical", separatorStyle);
        Assert.Contains("controls|CodexSeparator.size-sm.horizontal", separatorStyle);
        Assert.Contains("controls|CodexSeparator.size-lg.vertical", separatorStyle);

        foreach (var style in new[] { applicationShellStyle, dropdownStyle, collapsibleStyle, separatorStyle })
        {
            Assert.Contains("CodexSwitch.MotionDuration", style);
            Assert.Contains("CodexSwitch.MotionEaseOut", style);
            Assert.DoesNotContain("BasedOn=", style);
            Assert.DoesNotContain("Avalonia.Themes.Fluent", style);
            Assert.DoesNotContain("FluentTheme", style);
        }
    }

    [Fact]
    public void TabsValueChangedCarriesWebStyleSourceMetadata()
    {
        var root = FindRepositoryRoot();
        var tabs = File.ReadAllText(Path.Combine(root, "src", "CodexSwitchUI", "Controls", "CodexTabs.cs"));

        Assert.Contains("public enum CodexTabsValueChangeSource", tabs);
        Assert.Contains("public CodexTabsValueChangeSource Source { get; } = source;", tabs);
        Assert.Contains("private CodexTabsValueChangeSource? _pendingValueChangeSource;", tabs);
        Assert.Contains("internal bool SelectItem(CodexTabItem item, CodexTabsValueChangeSource source = CodexTabsValueChangeSource.Programmatic)", tabs);
        Assert.Contains("internal bool SelectIndex(int index, CodexTabsValueChangeSource source = CodexTabsValueChangeSource.Programmatic)", tabs);
        Assert.Contains("SelectIndex(nextIndex, CodexTabsValueChangeSource.Keyboard);", tabs);
        Assert.Contains("ValueChanged?.Invoke(this, new CodexTabsValueChangedEventArgs(oldItem, newItem, oldIndex, newIndex, oldValue, newValue, source));", tabs);
        Assert.Contains("TrySelect(CodexTabsValueChangeSource.Keyboard)", tabs);
        Assert.Contains("TrySelect(CodexTabsValueChangeSource.Pointer)", tabs);
        Assert.Contains("TryHandlePointerActivation(PointerUpdateKind updateKind)", tabs);
        Assert.Contains("PointerUpdateKind.LeftButtonReleased", tabs);
    }

    [Fact]
    public void DataDisplaySurfaceControlsOwnSlotsEventsAndNativeChrome()
    {
        var root = FindRepositoryRoot();
        var controls = Path.Combine(root, "src", "CodexSwitchUI", "Controls");
        var card = File.ReadAllText(Path.Combine(controls, "CodexCard.cs"));
        var item = File.ReadAllText(Path.Combine(controls, "CodexItem.cs"));
        var displayPrimitives = File.ReadAllText(Path.Combine(controls, "CodexDisplayPrimitives.cs"));
        var providerCard = File.ReadAllText(Path.Combine(controls, "CodexProviderCard.cs"));
        var scrollArea = File.ReadAllText(Path.Combine(controls, "CodexScrollArea.cs"));
        var cardStyle = ReadStyle(root, "Card");
        var itemStyle = ReadStyle(root, "Item");
        var applicationShellStyle = ReadStyle(root, "ApplicationShell");
        var scrollAreaStyle = ReadStyle(root, "ScrollArea");

        Assert.Contains("public class CodexCard : CodexFrame", card);
        Assert.Contains("IsInteractiveProperty.Changed.AddClassHandler<CodexCard>", card);
        Assert.Contains("Classes.Set(\"interactive\", IsInteractive);", card);
        Assert.Contains("SetValue(HasHeaderProperty, hasTitle || hasDescription);", card);
        Assert.Contains("SetValue(HasContentProperty, HasValue(Content));", card);
        Assert.Contains("SetValue(HasFooterProperty, HasValue(Footer));", card);
        Assert.Contains("<ControlTemplate>", cardStyle);
        Assert.Contains("PART_Surface", cardStyle);
        Assert.Contains("PART_Header", cardStyle);
        Assert.Contains("PART_Title", cardStyle);
        Assert.Contains("PART_Description", cardStyle);
        Assert.Contains("PART_Content", cardStyle);
        Assert.Contains("PART_Footer", cardStyle);
        Assert.Contains("controls|CodexCard.interactive:pointerover /template/ Border#PART_Surface", cardStyle);

        Assert.Contains("public class CodexItem : ContentControl", item);
        Assert.Contains("public enum CodexItemActivationSource", item);
        Assert.Contains("public CodexItemActivationSource Source { get; } = source;", item);
        Assert.Contains("TryActivate(CodexItemActivationSource.Programmatic)", item);
        Assert.Contains("TryActivate(CodexItemActivationSource.Keyboard)", item);
        Assert.Contains("TryActivate(CodexItemActivationSource.Pointer)", item);
        Assert.Contains("TryHandlePointerActivation(PointerUpdateKind updateKind, object? source = null)", item);
        Assert.Contains("updateKind != PointerUpdateKind.LeftButtonReleased", item);
        Assert.Contains("Properties.PointerUpdateKind", item);
        Assert.Contains("ShouldIgnoreActivation(source)", item);
        Assert.Contains("GetVisualAncestors()", item);
        Assert.Contains("GetLogicalParent()", item);
        Assert.Contains("or CodexBadge", item);
        Assert.Contains("public bool TryHandleActivationKey(Key key)", item);
        Assert.Contains("Classes.Set(\"command-blocked\", ActivateCommand is not null && IsInteractive && IsEnabled && !IsLoading && !CanActivate);", item);
        Assert.Contains("controls|CodexItem.command-blocked", itemStyle);
        Assert.Contains("controls|CodexItem.command-blocked:pointerover /template/ Border#PART_Surface", itemStyle);
        Assert.Contains("controls|CodexItem.selected.command-blocked:pointerover", itemStyle);

        Assert.Contains("public class CodexImageIcon : Image", displayPrimitives);
        Assert.Contains("public event EventHandler<CodexImageIconLoadedEventArgs>? ImageLoaded;", displayPrimitives);
        Assert.Contains("public event EventHandler<CodexImageIconLoadFailedEventArgs>? ImageLoadFailed;", displayPrimitives);
        Assert.Contains("Classes.Set(\"image-icon\", true);", displayPrimitives);
        Assert.Contains("Classes.Set(\"has-source\", hasSource);", displayPrimitives);
        Assert.Contains("Classes.Set(\"missing-source\", isMissing);", displayPrimitives);
        Assert.Contains("Classes.Set(\"empty-source\", !hasSource);", displayPrimitives);
        Assert.Contains("controls|CodexImageIcon.has-source", applicationShellStyle);
        Assert.Contains("controls|CodexImageIcon.empty-source", applicationShellStyle);
        Assert.Contains("controls|CodexImageIcon.missing-source", applicationShellStyle);

        Assert.Contains("public class CodexStatCard : CodexFrame", displayPrimitives);
        Assert.Contains("HasDetailProperty", displayPrimitives);
        Assert.Contains("HasIconProperty", displayPrimitives);
        Assert.Contains("public class CodexMetric : CodexFrame", displayPrimitives);
        Assert.Contains("<ControlTemplate TargetType=\"controls:CodexStatCard\">", applicationShellStyle);
        Assert.Contains("PART_Layout", applicationShellStyle);
        Assert.Contains("PART_Label", applicationShellStyle);
        Assert.Contains("PART_Icon", applicationShellStyle);
        Assert.Contains("PART_Value", applicationShellStyle);
        Assert.Contains("PART_Detail", applicationShellStyle);
        Assert.Contains("<ControlTemplate TargetType=\"controls:CodexMetric\">", applicationShellStyle);
        Assert.Contains("PART_Root", applicationShellStyle);

        Assert.Contains("public class CodexProviderCard : Button", providerCard);
        Assert.Contains("public enum CodexProviderCardSelectionSource", providerCard);
        Assert.Contains("public sealed class CodexProviderCardSelectedEventArgs", providerCard);
        Assert.Contains("public CodexProviderCardSelectionSource Source { get; } = source;", providerCard);
        Assert.Contains("public event EventHandler<CodexProviderCardSelectedEventArgs>? Selected;", providerCard);
        Assert.Contains("CommandProperty.Changed.AddClassHandler<CodexProviderCard>((card, args) => card.OnCommandChanged", providerCard);
        Assert.Contains("CommandParameterProperty.Changed.AddClassHandler<CodexProviderCard>((card, _) => card.SyncClasses());", providerCard);
        Assert.Contains("CanExecuteChanged += OnCommandCanExecuteChanged", providerCard);
        Assert.Contains("protected override void OnClick()", providerCard);
        Assert.Contains("internal bool TrySelect()", providerCard);
        Assert.Contains("TrySelect(CodexProviderCardSelectionSource.Programmatic)", providerCard);
        Assert.Contains("TrySelect(CodexProviderCardSelectionSource.Pointer)", providerCard);
        Assert.Contains("RunWithSelectionSource(CodexProviderCardSelectionSource.Keyboard", providerCard);
        Assert.Contains("internal bool TryHandlePointerActivation(PointerUpdateKind updateKind)", providerCard);
        Assert.Contains("updateKind != PointerUpdateKind.LeftButtonReleased", providerCard);
        Assert.Contains("protected override void OnPointerPressed(PointerPressedEventArgs e)", providerCard);
        Assert.Contains("PointerUpdateKind.LeftButtonPressed", providerCard);
        Assert.Contains("protected override void OnPointerReleased(PointerReleasedEventArgs e)", providerCard);
        Assert.Contains("Properties.PointerUpdateKind", providerCard);
        Assert.Contains("_hasPrimaryPointerPress && IsPointerOver", providerCard);
        Assert.Contains("_pendingPointerReleaseKind = updateKind;", providerCard);
        Assert.Contains("_pendingPointerReleaseKind is { } updateKind", providerCard);
        Assert.Contains("_selectionHandledByPointerRelease = TryHandlePointerActivation(updateKind);", providerCard);
        Assert.Contains("_selectionHandledByPointerRelease", providerCard);
        Assert.Contains("e.Handled = true;", providerCard);
        Assert.Contains("private bool CanSelect()", providerCard);
        Assert.Contains("Command?.CanExecute(CommandParameter) ?? true", providerCard);
        Assert.Contains("SelectSiblingCards();", providerCard);
        Assert.Contains("Classes.Set(\"active\", IsActive);", providerCard);
        Assert.Contains("Classes.Set(\"dragging\", IsDragging);", providerCard);
        Assert.Contains("Classes.Set(\"can-select\", CanSelect());", providerCard);
        Assert.Contains("Classes.Set(\"command-blocked\", Command is not null && IsEnabled && !IsDragging && !CanSelect());", providerCard);
        Assert.Contains("SetValue(HasLeadingProperty, Leading is not null);", providerCard);
        Assert.Contains("SetValue(HasActionsProperty, Actions is not null);", providerCard);
        Assert.Contains("<ControlTemplate TargetType=\"controls:CodexProviderCard\">", applicationShellStyle);
        Assert.Contains("PART_Leading", applicationShellStyle);
        Assert.Contains("PART_IconShell", applicationShellStyle);
        Assert.Contains("PART_HeaderLine", applicationShellStyle);
        Assert.Contains("PART_HeaderPresenter", applicationShellStyle);
        Assert.Contains("PART_Meta", applicationShellStyle);
        Assert.Contains("PART_Status", applicationShellStyle);
        Assert.Contains("PART_Usage", applicationShellStyle);
        Assert.Contains("PART_Actions", applicationShellStyle);
        Assert.Contains("controls|CodexProviderCard:focus-visible /template/ Border#PART_Surface", applicationShellStyle);
        Assert.Contains("controls|CodexProviderCard:disabled", applicationShellStyle);
        Assert.Contains("controls|CodexProviderCard.command-blocked", applicationShellStyle);
        Assert.Contains("controls|CodexProviderCard.active.command-blocked:pointerover", applicationShellStyle);

        Assert.Contains("public event EventHandler<ScrollChangedEventArgs>? ScrollChanged;", scrollArea);
        Assert.Contains("Classes.Set(\"type-hover\", Type == CodexScrollAreaType.Hover);", scrollArea);
        Assert.Contains("Classes.Set(\"scrolling\", IsScrolling);", scrollArea);
        Assert.Contains("Classes.Set(\"can-scroll-y\", CanScrollVertically);", scrollArea);
        Assert.Contains("<ControlTemplate TargetType=\"controls:CodexScrollArea\">", scrollAreaStyle);
        Assert.Contains("PART_Viewport", scrollAreaStyle);
        Assert.Contains("PART_ContentPresenter", scrollAreaStyle);
        Assert.Contains("PART_HorizontalScrollBar", scrollAreaStyle);
        Assert.Contains("PART_VerticalScrollBar", scrollAreaStyle);
        Assert.Contains("PART_ThumbSurface", scrollAreaStyle);
        Assert.Contains("ScrollGestureRecognizer", scrollAreaStyle);

        foreach (var style in new[] { cardStyle, applicationShellStyle, scrollAreaStyle })
        {
            Assert.Contains("CodexSwitch.MotionDuration", style);
            Assert.Contains("CodexSwitch.MotionEaseOut", style);
            Assert.DoesNotContain("BasedOn=", style);
            Assert.DoesNotContain("Avalonia.Themes.Fluent", style);
            Assert.DoesNotContain("FluentTheme", style);
            Assert.DoesNotContain("<Style Selector=\"Button", style);
            Assert.DoesNotContain("<Style Selector=\"ScrollBar", style);
        }

        Assert.Contains("CodexSwitch.DisabledOpacity", applicationShellStyle);
        Assert.Contains("CodexSwitch.DisabledOpacity", scrollAreaStyle);
    }

    [Fact]
    public void DataDisplayTablePaginationAndChartPrimitivesOwnChromeAndMotion()
    {
        var root = FindRepositoryRoot();
        var controls = Path.Combine(root, "src", "CodexSwitchUI", "Controls");
        var eChartsControls = Path.Combine(root, "src", "CodexSwitchUI.ECharts", "Controls");
        var table = File.ReadAllText(Path.Combine(controls, "CodexTable.cs"));
        var pagination = File.ReadAllText(Path.Combine(controls, "CodexPagination.cs"));
        var rankedBarChart = File.ReadAllText(Path.Combine(controls, "CodexRankedBarChart.cs"));
        var usagePieChart = File.ReadAllText(Path.Combine(controls, "CodexUsagePieChart.cs"));
        var usageTrendChart = File.ReadAllText(Path.Combine(eChartsControls, "CsUsageTrendChart.cs"));
        var tableStyle = ReadStyle(root, "Table");
        var paginationStyle = ReadStyle(root, "Pagination");
        var rankedBarChartStyle = ReadStyle(root, "RankedBarChart");
        var usagePieChartStyle = ReadStyle(root, "UsagePieChart");

        Assert.Contains("public class CodexPinnedTable : TemplatedControl", table);
        Assert.Contains("ScrollChanged -= OnBodyScrollChanged", table);
        Assert.Contains("ScrollChanged += OnBodyScrollChanged", table);
        Assert.Contains("SyncHeaderScroll();", table);
        Assert.Contains("CodexMotion.ResolveDefaultDuration(this)", table);
        Assert.Contains("<ControlTemplate TargetType=\"controls:CodexPinnedTable\">", tableStyle);
        Assert.Contains("PART_StartHeader", tableStyle);
        Assert.Contains("PART_HeaderScrollViewer", tableStyle);
        Assert.Contains("PART_MiddleHeader", tableStyle);
        Assert.Contains("PART_MiddleHeaderContent", tableStyle);
        Assert.Contains("PART_BodyScrollViewer", tableStyle);
        Assert.Contains("PART_MiddleItemsControl", tableStyle);
        Assert.Contains("PART_EndItemsControl", tableStyle);
        Assert.Contains("controls|CodexPinnedTable /template/ ScrollViewer", tableStyle);
        Assert.Contains("<ControlTemplate TargetType=\"ScrollViewer\">", tableStyle);
        Assert.Contains("PART_HorizontalScrollBar", tableStyle);
        Assert.Contains("PART_VerticalScrollBar", tableStyle);
        Assert.Contains("PART_ThumbSurface", tableStyle);
        Assert.Contains("ScrollGestureRecognizer", tableStyle);

        Assert.Contains("public event EventHandler<CodexPaginationPageChangedEventArgs>? PageChanged;", pagination);
        Assert.Contains("TryHandleActionPointerRelease(PointerUpdateKind updateKind, CodexPaginationPageChangeSource source)", pagination);
        Assert.Contains("TryHandleActionKey(Key key, CodexPaginationPageChangeSource source)", pagination);
        Assert.Contains("TryHandleNavigationKey(Key key)", pagination);
        Assert.Contains("Key.Left or Key.PageUp", pagination);
        Assert.Contains("Key.Right or Key.PageDown", pagination);
        Assert.Contains("InputElement.PointerReleasedEvent", pagination);
        Assert.Contains("InputElement.KeyDownEvent", pagination);
        Assert.Contains("PointerUpdateKind.LeftButtonReleased", pagination);
        Assert.DoesNotContain(".Click += OnFirstClicked", pagination);
        Assert.DoesNotContain(".Click += OnPreviousClicked", pagination);
        Assert.DoesNotContain(".Click += OnNextClicked", pagination);
        Assert.DoesNotContain(".Click += OnLastClicked", pagination);
        Assert.Contains("public class CodexPaginationPageButton : CodexButton", pagination);
        Assert.Contains("internal bool CanSelectPageItem(int page)", pagination);
        Assert.Contains("internal bool CanActivate => IsEnabled", pagination);
        Assert.Contains("CommandProperty.Changed.AddClassHandler<CodexPaginationPageButton>((button, args) => button.OnCommandChanged", pagination);
        Assert.Contains("Classes.Set(\"can-activate\", CanActivate);", pagination);
        Assert.Contains("Classes.Set(\"command-blocked\", Command is not null && IsEnabled && !IsLoading && !IsEllipsis && !IsCurrent && !CanExecuteCommand());", pagination);
        Assert.Contains("<ControlTemplate TargetType=\"controls:CodexPagination\">", paginationStyle);
        Assert.Contains("PART_FirstButton", paginationStyle);
        Assert.Contains("PART_PreviousButton", paginationStyle);
        Assert.Contains("PART_Items", paginationStyle);
        Assert.Contains("PART_NextButton", paginationStyle);
        Assert.Contains("PART_LastButton", paginationStyle);
        Assert.Contains("controls|CodexPaginationPageButton.current", paginationStyle);
        Assert.Contains("controls|CodexPaginationPageButton.ellipsis", paginationStyle);
        Assert.Contains("controls|CodexPaginationPageButton.command-blocked", paginationStyle);

        Assert.Contains("public event EventHandler<CodexRankedBarChartActiveItemChangedEventArgs>? ActiveItemChanged;", rankedBarChart);
        Assert.Contains("Classes.Set(\"compact\", IsCompact);", rankedBarChart);
        Assert.Contains("Classes.Set(\"has-active-row\", ActiveIndex >= 0);", rankedBarChart);
        Assert.Contains("Classes.Set(\"empty\", _items.Length == 0", rankedBarChart);
        Assert.Contains("controls|CodexRankedBarChart.has-active-row", rankedBarChartStyle);
        Assert.Contains("controls|CodexRankedBarChart.compact", rankedBarChartStyle);
        Assert.Contains("CodexSwitch.DisabledOpacity", rankedBarChartStyle);

        Assert.Contains("public event EventHandler<CodexUsagePieChartActiveItemChangedEventArgs>? ActiveItemChanged;", usagePieChart);
        Assert.Contains("AnimationDurationProperty", usagePieChart);
        Assert.Contains("Classes.Set(\"compact\", IsCompact);", usagePieChart);
        Assert.Contains("Classes.Set(\"has-active-slice\", ActiveIndex >= 0);", usagePieChart);
        Assert.Contains("Classes.Set(\"empty\", _items.Length == 0", usagePieChart);
        Assert.Contains("controls|CodexUsagePieChart.has-active-slice", usagePieChartStyle);
        Assert.Contains("Property=\"AnimationDuration\" Value=\"{DynamicResource CodexSwitch.MotionDurationSlow}\"", usagePieChartStyle);
        Assert.Contains("CodexSwitch.DisabledOpacity", usagePieChartStyle);

        Assert.Contains("public sealed class CsUsageTrendChart : Control", usageTrendChart);
        Assert.Contains("ItemsSourceProperty", usageTrendChart);
        Assert.Contains("GranularityProperty", usageTrendChart);
        Assert.Contains("IsRefreshingProperty", usageTrendChart);
        Assert.Contains("PointerMoved += OnPointerMoved;", usageTrendChart);
        Assert.Contains("DrawTooltip", usageTrendChart);

        foreach (var style in new[] { tableStyle, paginationStyle, rankedBarChartStyle, usagePieChartStyle })
        {
            Assert.Contains("CodexSwitch.MotionDuration", style);
            Assert.Contains("CodexSwitch.MotionEaseOut", style);
            Assert.DoesNotContain("BasedOn=", style);
            Assert.DoesNotContain("Avalonia.Themes.Fluent", style);
            Assert.DoesNotContain("FluentTheme", style);
            Assert.DoesNotContain("<Style Selector=\"ScrollViewer", style);
            Assert.DoesNotContain("<Style Selector=\"ScrollBar", style);
        }
    }

    [Fact]
    public void OverlayTriggerPointerReleaseContractsUsePrimaryButtonOnly()
    {
        var root = FindRepositoryRoot();
        var controls = Path.Combine(root, "src", "CodexSwitchUI", "Controls");

        foreach (var control in new[] { "CodexDialog.cs", "CodexPopover.cs" })
        {
            var source = File.ReadAllText(Path.Combine(controls, control));

            Assert.Contains("TryHandleTriggerPointerRelease(PointerUpdateKind updateKind)", source);
            Assert.Contains("OpenChangeSource", source);
            Assert.Contains("Source { get; }", source);
            Assert.Contains("PointerUpdateKind.LeftButtonReleased && Toggle(", source);
            Assert.Contains("Properties.PointerUpdateKind", source);
            Assert.Contains("TryHandleTriggerPointerRelease(updateKind)", source);
        }
    }

    [Fact]
    public void AlertDialogPartCommandsForwardHostCanExecuteChanges()
    {
        var root = FindRepositoryRoot();
        var source = File.ReadAllText(Path.Combine(root, "src", "CodexSwitchUI", "Controls", "CodexAlertDialog.cs"));

        Assert.Contains("CancelCommandProperty.Changed.AddClassHandler<CodexAlertDialog>((dialog, args) => dialog.OnCancelCommandChanged", source);
        Assert.Contains("ActionCommandProperty.Changed.AddClassHandler<CodexAlertDialog>((dialog, args) => dialog.OnActionCommandChanged", source);
        Assert.Contains("newCommand.CanExecuteChanged += OnPartCommandCanExecuteChanged;", source);
        Assert.Contains("subscribedCommand.CanExecuteChanged -= OnPartCommandCanExecuteChanged;", source);
        Assert.Contains("private void OnPartCommandCanExecuteChanged(object? sender, EventArgs e)", source);
        Assert.Contains("RaisePartCommandStateChanged();", source);
        Assert.DoesNotContain("CancelCommandProperty.Changed.AddClassHandler<CodexAlertDialog>((dialog, _) => dialog.RaisePartCommandStateChanged());", source);
    }

    [Fact]
    public void NavigationMenuTopLevelPointerActivationUsesPrimaryReleaseOnly()
    {
        var root = FindRepositoryRoot();
        var source = File.ReadAllText(Path.Combine(root, "src", "CodexSwitchUI", "Controls", "CodexNavigationMenu.cs"));

        Assert.Contains("TryHandlePointerRelease(PointerUpdateKind updateKind", source);
        Assert.Contains("updateKind != PointerUpdateKind.LeftButtonReleased", source);
        Assert.Contains("Properties.PointerUpdateKind", source);
        Assert.Contains("if (TryHandlePointerRelease(updateKind))", source);
        Assert.Contains("public bool CanActivateLink => IsEnabled && !HasContent && (Command?.CanExecute(CommandParameter) ?? true);", source);
        Assert.Contains("CommandProperty.Changed.AddClassHandler<CodexNavigationMenuItem>((item, args) => item.OnCommandChanged", source);
        Assert.Contains("Classes.Set(\"command-blocked\", !hasContent && !CanActivateLink);", source);
        Assert.Contains("updateKind == PointerUpdateKind.LeftButtonPressed", source);
        Assert.Contains("Focus(NavigationMethod.Pointer, KeyModifiers.None);", source);
        Assert.DoesNotContain("Properties.IsLeftButtonPressed", source);
        Assert.DoesNotContain("TryActivateLink())\n            {\n                Activate();", source);
        Assert.DoesNotContain("FindOwner()?.CloseViewport();\n            }\n\n            e.Handled = true;", source);
    }

    [Fact]
    public void NavigationMenuContentLinkPointerActivationUsesPrimaryReleaseOnly()
    {
        var root = FindRepositoryRoot();
        var source = File.ReadAllText(Path.Combine(root, "src", "CodexSwitchUI", "Controls", "CodexNavigationMenu.cs"));

        Assert.Contains("TryHandlePointerActivation(PointerUpdateKind updateKind)", source);
        Assert.Contains("updateKind != PointerUpdateKind.LeftButtonReleased", source);
        Assert.Contains("Properties.PointerUpdateKind", source);
        Assert.Contains("if (TryHandlePointerActivation(updateKind))", source);
        Assert.Contains("public bool CanActivate => IsEnabled && (Command?.CanExecute(CommandParameter) ?? true);", source);
        Assert.Contains("CommandProperty.Changed.AddClassHandler<CodexNavigationMenuLink>((link, args) => link.OnCommandChanged", source);
        Assert.Contains("CanExecuteChanged += OnCommandCanExecuteChanged", source);
        Assert.Contains("Classes.Set(\"command-blocked\", !CanActivate);", source);
        Assert.DoesNotContain("Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonReleased\n            && TryActivate()", source);
    }

    [Fact]
    public void BreadcrumbLinkActivationHonorsCommandCanExecuteBeforePublishing()
    {
        var root = FindRepositoryRoot();
        var source = File.ReadAllText(Path.Combine(root, "src", "CodexSwitchUI", "Controls", "CodexBreadcrumb.cs"));
        var style = ReadStyle(root, "Breadcrumb");

        Assert.Contains("public enum CodexBreadcrumbLinkActivationSource", source);
        Assert.Contains("public CodexBreadcrumbLinkActivationSource Source { get; } = source;", source);
        Assert.Contains("public bool CanActivate => !IsCurrent && IsEnabled && (Command?.CanExecute(CommandParameter) ?? true);", source);
        Assert.Contains("CommandProperty.Changed.AddClassHandler<CodexBreadcrumbLink>((link, args) => link.OnCommandChanged", source);
        Assert.Contains("CanExecuteChanged += OnCommandCanExecuteChanged", source);
        Assert.Contains("internal bool TryHandlePointerActivation(PointerUpdateKind updateKind)", source);
        Assert.Contains("updateKind != PointerUpdateKind.LeftButtonReleased", source);
        Assert.Contains("TryActivate(CodexBreadcrumbLinkActivationSource.Pointer)", source);
        Assert.Contains("NotifyOwner(_pendingActivationSource ?? CodexBreadcrumbLinkActivationSource.Keyboard);", source);
        Assert.Contains("NotifyOwner(_pendingActivationSource ?? CodexBreadcrumbLinkActivationSource.Pointer);", source);
        Assert.Contains("protected override void OnPointerReleased(PointerReleasedEventArgs e)", source);
        Assert.Contains("_hasPrimaryPointerPress && IsPointerOver", source);
        Assert.Contains("Properties.PointerUpdateKind", source);
        Assert.Contains("Classes.Set(\"can-activate\", CanActivate);", source);
        Assert.Contains("Classes.Set(\"command-blocked\", Command is not null && !IsCurrent && IsEnabled && !CanActivate);", source);
        Assert.Contains("controls|CodexBreadcrumbLink.command-blocked", style);
        Assert.DoesNotContain("if (IsCurrent || !IsEnabled)\n        {\n            return;\n        }\n\n        var owner", source);
    }

    [Fact]
    public void CommandItemSelectionPublishesSourceMetadataAndPrimaryPointerRelease()
    {
        var root = FindRepositoryRoot();
        var source = File.ReadAllText(Path.Combine(root, "src", "CodexSwitchUI", "Controls", "CodexCommand.cs"));

        Assert.Contains("public enum CodexCommandItemSelectSource", source);
        Assert.Contains("public CodexCommandItemSelectSource Source { get; } = source;", source);
        Assert.Contains("active?.TrySelect(CodexCommandItemSelectSource.Keyboard)", source);
        Assert.Contains("NotifyItemSelected(this, source)", source);
        Assert.Contains("CodexCommandDialog.FindOwner(this)?.NotifyItemSelected(this, source);", source);
        Assert.Contains("internal bool TryHandlePointerActivation(PointerUpdateKind updateKind)", source);
        Assert.Contains("updateKind != PointerUpdateKind.LeftButtonReleased", source);
        Assert.Contains("PointerUpdateKind.LeftButtonPressed", source);
        Assert.Contains("_hasPrimaryPointerPress && IsPointerOver", source);
        Assert.Contains("TrySelect(CodexCommandItemSelectSource.Pointer)", source);
        Assert.Contains("TrySelect(CodexCommandItemSelectSource.Keyboard)", source);
    }

    [Fact]
    public void CommandDialogForwardsCommandItemSelectionSourceMetadata()
    {
        var root = FindRepositoryRoot();
        var source = File.ReadAllText(Path.Combine(root, "src", "CodexSwitchUI", "Controls", "CodexCommandDialog.cs"));

        Assert.Contains("internal void NotifyItemSelected(CodexCommandItem item, CodexCommandItemSelectSource source)", source);
        Assert.Contains("new CodexCommandItemSelectedEventArgs(item, item.ResolveValue(), source)", source);
        Assert.Contains("TryCloseFromCommandItem(item, source);", source);
        Assert.Contains("private static CodexDialogOpenChangeSource ToOpenChangeSource(CodexCommandItemSelectSource source)", source);
        Assert.Contains("internal static CodexCommandDialog? FindOwner(CodexCommandItem item)", source);
        Assert.Contains("item.GetLogicalParent()", source);
        Assert.Contains("item.GetVisualParent()", source);
        Assert.DoesNotContain("new CodexCommandItemSelectedEventArgs(item, item.ResolveValue()))", source);
    }

    [Fact]
    public void ResizableHandlePointerDragContractUsesPrimaryButtonOnly()
    {
        var root = FindRepositoryRoot();
        var source = File.ReadAllText(Path.Combine(root, "src", "CodexSwitchUI", "Controls", "CodexResizable.cs"));

        Assert.Contains("TryBeginResize(PointerUpdateKind updateKind", source);
        Assert.Contains("TryEndResize(PointerUpdateKind updateKind", source);
        Assert.Contains("updateKind != PointerUpdateKind.LeftButtonPressed", source);
        Assert.Contains("updateKind != PointerUpdateKind.LeftButtonReleased", source);
        Assert.Contains("Properties.PointerUpdateKind", source);
        Assert.Contains("if (!TryBeginResize(updateKind, startPoint, owner))", source);
        Assert.Contains("if (!TryEndResize(updateKind))", source);
        Assert.DoesNotContain("owner.BeginResize(this);\n        e.Handled = true;", source);
        Assert.DoesNotContain("owner.EndResize(this);\n        }\n\n        e.Pointer.Capture(null);", source);
    }

    [Fact]
    public void DrawerHandlePointerDragContractUsesPrimaryButtonOnly()
    {
        var root = FindRepositoryRoot();
        var source = File.ReadAllText(Path.Combine(root, "src", "CodexSwitchUI", "Controls", "CodexDrawer.cs"));

        Assert.Contains("TryBeginHandleDrag(PointerUpdateKind updateKind", source);
        Assert.Contains("TryCompleteHandleDrag(PointerUpdateKind updateKind", source);
        Assert.Contains("updateKind != PointerUpdateKind.LeftButtonPressed", source);
        Assert.Contains("updateKind != PointerUpdateKind.LeftButtonReleased", source);
        Assert.Contains("Properties.PointerUpdateKind", source);
        Assert.Contains("if (!TryBeginHandleDrag(updateKind, e.GetPosition(this), e.Pointer))", source);
        Assert.Contains("if (!TryCompleteHandleDrag(updateKind, e.Pointer))", source);
        Assert.DoesNotContain("Properties.IsLeftButtonPressed is false", source);
        Assert.DoesNotContain("CompleteDrag();\n        _dragStart = null;\n        _dragPointer = null;\n        e.Handled = true;", source);
    }

    [Fact]
    public void BadgePointerActivationContractUsesPrimaryReleaseOnly()
    {
        var root = FindRepositoryRoot();
        var source = File.ReadAllText(Path.Combine(root, "src", "CodexSwitchUI", "Controls", "CodexBadge.cs"));

        Assert.Contains("public enum CodexBadgeActivationSource", source);
        Assert.Contains("public CodexBadgeActivationSource Source { get; } = source;", source);
        Assert.Contains("TryActivate(CodexBadgeActivationSource.Programmatic)", source);
        Assert.Contains("TryActivate(CodexBadgeActivationSource.Pointer)", source);
        Assert.Contains("TryActivate(CodexBadgeActivationSource.Keyboard)", source);
        Assert.Contains("public bool TryHandleActivationKey(Key key)", source);
        Assert.Contains("TryHandlePointerActivation(PointerUpdateKind updateKind)", source);
        Assert.Contains("updateKind != PointerUpdateKind.LeftButtonReleased", source);
        Assert.Contains("Properties.PointerUpdateKind", source);
        Assert.Contains("PointerUpdateKind.LeftButtonPressed", source);
        Assert.Contains("if (TryHandlePointerActivation(updateKind))", source);
        Assert.DoesNotContain("Properties.IsLeftButtonPressed", source);
        Assert.DoesNotContain("Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonReleased\n            && TryActivate()", source);
    }

    [Fact]
    public void MenubarTopLevelPointerReleaseContractUsesPrimaryButtonOnly()
    {
        var root = FindRepositoryRoot();
        var source = File.ReadAllText(Path.Combine(root, "src", "CodexSwitchUI", "Controls", "CodexMenubar.cs"));

        Assert.Contains("TryHandleTopLevelPointerRelease(CodexMenubarItem item, PointerUpdateKind updateKind)", source);
        Assert.Contains("updateKind != PointerUpdateKind.LeftButtonReleased", source);
        Assert.Contains("Properties.PointerUpdateKind", source);
        Assert.Contains("TryHandleTopLevelPointerRelease(this, updateKind)", source);
        Assert.DoesNotContain("FindOwner(this)?.ToggleMenu(this)", source);
    }

    [Fact]
    public void MenuLeafPointerSelectionContractsUsePrimaryButtonOnly()
    {
        var root = FindRepositoryRoot();
        var controls = Path.Combine(root, "src", "CodexSwitchUI", "Controls");

        foreach (var control in new[] { "CodexMenu.cs", "CodexContextMenu.cs" })
        {
            var source = File.ReadAllText(Path.Combine(controls, control));

            Assert.Contains("TryHandlePointerSelection(PointerUpdateKind updateKind)", source);
            Assert.Contains("updateKind != PointerUpdateKind.LeftButtonReleased", source);
            Assert.Contains("Properties.PointerUpdateKind", source);
            Assert.Contains("TryHandlePointerSelection(updateKind)", source);
            Assert.DoesNotContain("_pendingSelectSource = CodexMenuItemSelectSource.Pointer;\n        try", source);
        }
    }

    [Fact]
    public void DisabledStatesUseSemanticOpacityToken()
    {
        var root = FindRepositoryRoot();
        var failures = new List<string>();

        foreach (var component in Components)
        {
            var style = ReadStyle(root, component);

            if (style.Contains(":disabled", StringComparison.Ordinal)
                && !style.Contains("CodexSwitch.DisabledOpacity", StringComparison.Ordinal))
            {
                failures.Add($"{component}: disabled state does not use CodexSwitch.DisabledOpacity.");
            }

            if (style.Contains("Property=\"Opacity\" Value=\"0.5\"", StringComparison.Ordinal))
            {
                failures.Add($"{component}: disabled opacity is hard-coded instead of tokenized.");
            }
        }

        AssertNoFailures(failures);
    }

    [Fact]
    public void ThemeTokenFilesDeclareMotionResourceKeys()
    {
        var root = FindRepositoryRoot();
        var baseTokens = File.ReadAllText(Path.Combine(root, "src", "CodexSwitchUI", "Tokens", "BaseTokens.axaml"));
        var lightTheme = File.ReadAllText(Path.Combine(root, "src", "CodexSwitchUI", "Themes", "Light.axaml"));
        var darkTheme = File.ReadAllText(Path.Combine(root, "src", "CodexSwitchUI", "Themes", "Dark.axaml"));

        var baseTokenKeys = new[]
        {
            CodexSwitchResourceKeys.MotionDurationFast,
            CodexSwitchResourceKeys.MotionDurationDefault,
            CodexSwitchResourceKeys.MotionDurationSlow,
            CodexSwitchResourceKeys.MotionEaseOut,
            CodexSwitchResourceKeys.MotionEaseInOut,
            CodexSwitchResourceKeys.DisabledOpacity,
            CodexSwitchResourceKeys.RingOffset,
            CodexSwitchResourceKeys.OverlayOpacity,
            CodexSwitchResourceKeys.PopoverEnterOffset,
            CodexSwitchResourceKeys.DialogEnterOffset,
            CodexSwitchResourceKeys.ToastEnterOffset,
            CodexSwitchResourceKeys.SkeletonShimmerDuration,
            CodexSwitchResourceKeys.SkeletonShimmerOpacity,
            CodexSwitchResourceKeys.ReducedMotion
        };

        foreach (var key in baseTokenKeys)
        {
            Assert.Contains($"x:Key=\"{key}\"", baseTokens);
        }

        Assert.Contains($"x:Key=\"{CodexSwitchResourceKeys.SkeletonShimmerBrush}\"", lightTheme);
        Assert.Contains($"x:Key=\"{CodexSwitchResourceKeys.SkeletonShimmerBrush}\"", darkTheme);
    }

    [Fact]
    public void ComponentStylesUseTokenizedMotionDurations()
    {
        var root = FindRepositoryRoot();
        var controls = Path.Combine(root, "src", "CodexSwitchUI", "Themes", "Controls");
        var failures = new List<string>();

        foreach (var path in Directory.EnumerateFiles(controls, "*.axaml").OrderBy(Path.GetFileName))
        {
            var fileName = Path.GetFileName(path);
            var lines = File.ReadLines(path).Select((line, index) => (Line: line, Number: index + 1));

            foreach (var (line, number) in lines)
            {
                if (!line.Contains("Duration=\"0:0:0.", StringComparison.Ordinal))
                {
                    continue;
                }

                if (line.Contains("<CrossFade ", StringComparison.Ordinal)
                    || line.Contains("<PageSlide ", StringComparison.Ordinal))
                {
                    continue;
                }

                failures.Add($"{fileName}:{number}: hard-coded component motion duration should use CodexSwitch.MotionDuration*.");
            }
        }

        AssertNoFailures(failures);
    }

    [Fact]
    public void PrimitiveStylesUseTokenizedMotionDurations()
    {
        var root = FindRepositoryRoot();
        var primitives = Path.Combine(root, "src", "CodexSwitchUI", "Themes", "Primitives");
        var failures = new List<string>();

        foreach (var path in Directory.EnumerateFiles(primitives, "*.axaml").OrderBy(Path.GetFileName))
        {
            var fileName = Path.GetFileName(path);
            var lines = File.ReadLines(path).Select((line, index) => (Line: line, Number: index + 1));

            foreach (var (line, number) in lines)
            {
                if (line.Contains("Duration=\"0:0:0.", StringComparison.Ordinal))
                {
                    failures.Add($"{fileName}:{number}: hard-coded primitive motion duration should use CodexSwitch.MotionDuration*.");
                }
            }
        }

        AssertNoFailures(failures);
    }

    [Fact]
    public void RuntimeTableTransitionsUseThemeMotionResources()
    {
        var root = FindRepositoryRoot();
        var source = File.ReadAllText(Path.Combine(root, "src", "CodexSwitchUI", "Controls", "CodexTable.cs"));
        var motion = File.ReadAllText(Path.Combine(root, "src", "CodexSwitchUI", "Tokens", "CodexMotion.cs"));

        Assert.Contains("CodexSwitchResourceKeys.MotionDurationDefault", motion);
        Assert.Contains("CodexSwitchResourceKeys.MotionEaseOut", motion);
        Assert.Contains("Application.Current?.TryFindResource", motion);
        Assert.Contains("CodexMotion.ResolveDefaultDuration", source);
        Assert.Contains("CodexMotion.ResolveEaseOut", source);
        Assert.Contains("duration <= TimeSpan.Zero", source);
        Assert.Contains("ApplyContentTransitionResources", source);
        Assert.Contains("ApplyPageTransitionResources", source);
        Assert.Contains("CodexMotion.ApplyOpacityTransition", source);
        Assert.Contains("CodexMotion.ApplyTranslateYTransition", source);
        Assert.DoesNotContain("TimeSpan.FromMilliseconds(150)", source);
        Assert.DoesNotContain("TimeSpan.FromMilliseconds(160)", source);
        Assert.DoesNotContain("new CubicEaseOut()", source);
    }

    [Fact]
    public void RuntimeShowCloseAnimationsUseThemeMotionResources()
    {
        var root = FindRepositoryRoot();
        var controls = Path.Combine(root, "src", "CodexSwitchUI", "Controls");
        var collapsible = File.ReadAllText(Path.Combine(controls, "CodexCollapsible.cs"));
        var sonner = File.ReadAllText(Path.Combine(controls, "CodexSonner.cs"));
        var skeleton = File.ReadAllText(Path.Combine(controls, "CodexSkeleton.cs"));
        var usagePieChart = File.ReadAllText(Path.Combine(controls, "CodexUsagePieChart.cs"));
        var barChart = File.ReadAllText(Path.Combine(controls, "CodexBarChart.cs"));
        var collapsibleStyle = ReadStyle(root, "Collapsible");
        var skeletonStyle = ReadStyle(root, "Skeleton");
        var usagePieChartStyle = ReadStyle(root, "UsagePieChart");
        var barChartStyle = ReadStyle(root, "BarChart");

        Assert.Contains("CodexSwitchThemeOptions.ShadcnDefault.MotionDurationSlow", collapsible);
        Assert.Contains("Property=\"AnimationDuration\" Value=\"{DynamicResource CodexSwitch.MotionDurationSlow}\"", collapsibleStyle);
        Assert.Contains("CodexMotion.ResolveDefaultDuration()", sonner);
        Assert.Contains("CodexSonnerService.EnterDuration <= TimeSpan.Zero", sonner);
        Assert.Contains("exitDuration <= TimeSpan.Zero", sonner);
        Assert.Contains("CodexSwitchThemeOptions.ShadcnDefault.SkeletonShimmerDuration", skeleton);
        Assert.Contains("Property=\"PulseDuration\" Value=\"{DynamicResource CodexSwitch.SkeletonShimmerDuration}\"", skeletonStyle);
        Assert.Contains("CodexSwitchThemeOptions.ShadcnDefault.MotionDurationSlow", usagePieChart);
        Assert.Contains("Property=\"AnimationDuration\" Value=\"{DynamicResource CodexSwitch.MotionDurationSlow}\"", usagePieChartStyle);
        Assert.Contains("AnimationDuration <= TimeSpan.Zero", usagePieChart);
        Assert.Contains("CodexSwitchThemeOptions.ShadcnDefault.MotionDurationSlow", barChart);
        Assert.Contains("Property=\"AnimationDuration\" Value=\"{DynamicResource CodexSwitch.MotionDurationSlow}\"", barChartStyle);
        Assert.Contains("AnimationDuration <= TimeSpan.Zero", barChart);
        Assert.DoesNotContain("DefaultAnimationDuration = TimeSpan.FromMilliseconds(200)", collapsible);
        Assert.DoesNotContain("EnterDuration { get; set; } = TimeSpan.FromMilliseconds(180)", sonner);
        Assert.DoesNotContain("ExitDuration { get; set; } = TimeSpan.FromMilliseconds(160)", sonner);
        Assert.DoesNotContain("DefaultPulseDuration = TimeSpan.FromSeconds(2)", skeleton);
        Assert.DoesNotContain("ChartAnimationDuration = TimeSpan.FromMilliseconds(520)", usagePieChart);
    }

    [Fact]
    public void RuntimeTimingConstantsHaveExplicitMotionClassification()
    {
        var root = FindRepositoryRoot();
        var controls = Path.Combine(root, "src", "CodexSwitchUI", "Controls");
        var allowed = new Dictionary<string, string[]>
        {
            ["CodexCollapsible.cs"] =
            [
                "FrameInterval = TimeSpan.FromMilliseconds(16)"
            ],
            ["CodexSkeleton.cs"] =
            [
                "FrameInterval = TimeSpan.FromMilliseconds(16)"
            ],
            ["CodexUsagePieChart.cs"] =
            [
                "AnimationFrameInterval = TimeSpan.FromMilliseconds(16)"
            ],
            ["CodexBarChart.cs"] =
            [
                "AnimationFrameInterval = TimeSpan.FromMilliseconds(16)"
            ],
            ["CodexLineChart.cs"] =
            [
                "AnimationFrameInterval = TimeSpan.FromMilliseconds(16)"
            ],
            ["CodexSonner.cs"] =
            [
                "Interval = TimeSpan.FromMilliseconds(16)",
                "DefaultDuration { get; set; } = TimeSpan.FromSeconds(4)"
            ],
            ["CodexHoverCard.cs"] =
            [
                "TimeSpan.FromMilliseconds(700)",
                "TimeSpan.FromMilliseconds(300)"
            ],
            ["CodexScrollArea.cs"] =
            [
                "ScrollIdleDelay = TimeSpan.FromMilliseconds(650)"
            ]
        };

        var failures = new List<string>();
        foreach (var (fileName, allowedFragments) in allowed)
        {
            var path = Path.Combine(controls, fileName);
            foreach (var (line, number) in File.ReadLines(path).Select((line, index) => (Line: line, Number: index + 1)))
            {
                if (!line.Contains("TimeSpan.FromMilliseconds", StringComparison.Ordinal)
                    && !line.Contains("TimeSpan.FromSeconds", StringComparison.Ordinal))
                {
                    continue;
                }

                if (!allowedFragments.Any(fragment => line.Contains(fragment, StringComparison.Ordinal)))
                {
                    failures.Add($"{fileName}:{number}: runtime timing constant needs tokenization or allowlist classification.");
                }
            }
        }

        AssertNoFailures(failures);
    }

    [Fact]
    public void RenderedMotionLifecycleTestsCoverRuntimeParitySurfaces()
    {
        var root = FindRepositoryRoot();
        var source = File.ReadAllText(Path.Combine(root, "tests", "CodexSwitchUI.Tests", "MotionRenderedLifecycleTests.cs"));
        var menuRendered = File.ReadAllText(Path.Combine(root, "tests", "CodexSwitchUI.Tests", "MenuRenderedLifecycleTests.cs"));
        var tableRendered = File.ReadAllText(Path.Combine(root, "tests", "CodexSwitchUI.Tests", "TableRenderedLayoutTests.cs"));

        Assert.Contains("ReducedMotionRuntimeSurfacesRenderFinalStatesInMountedTree", source);
        Assert.Contains("TokenizedRuntimeMotionSurfacesExposeIntermediateStatesWhenMotionIsEnabled", source);
        Assert.Contains("CodexTable", source);
        Assert.Contains("CodexCollapsible", source);
        Assert.Contains("CodexSkeleton", source);
        Assert.Contains("CodexUsagePieChart", source);
        Assert.Contains("CodexBarChart", source);
        Assert.Contains("CodexLineChart", source);
        Assert.Contains("CodexSonner", source);
        Assert.Contains("CodexHoverCard", source);
        Assert.Contains("CodexScrollArea", source);
        Assert.Contains("HoverCardRenderedDelayStatesMatchWebOpenCloseTiming", source);
        Assert.Contains("ScrollAreaRenderedVisibilityStatesUseHoverAndScrollMotionClasses", source);
        Assert.Contains("CaptureRenderedFrame", source);
        Assert.Contains("CodexSwitchThemeOptions.ShadcnDefault with { ReducedMotion = reducedMotion }", source);
        Assert.Contains("FindSubMenuSurface", menuRendered);
        Assert.Contains("window.CaptureRenderedFrame()", menuRendered);
        Assert.Contains("TableHeadAndCellAlignmentRenderAtExpectedColumnPositions", tableRendered);
        Assert.Contains("AssertCenterAligned", tableRendered);
        Assert.Contains("AssertRightAligned", tableRendered);
    }

    [Fact]
    public void EveryComponentHasOwnClassFile()
    {
        var root = FindRepositoryRoot();
        var controls = Path.Combine(root, "src", "CodexSwitchUI", "Controls");
        var classNames = Components.Select(component => component == "Input" ? "TextBox" : component);

        foreach (var className in classNames)
        {
            var filePath = Path.Combine(controls, $"Codex{className}.cs");
            Assert.True(File.Exists(filePath), $"Missing component class file: {filePath}");
        }
    }

    [Fact]
    public void EveryExpectedTopLevelControlHasOwnClassFile()
    {
        var root = FindRepositoryRoot();
        var controls = Path.Combine(root, "src", "CodexSwitchUI", "Controls");

        foreach (var className in ExpectedTopLevelControls)
        {
            var filePath = Path.Combine(controls, $"{className}.cs");
            Assert.True(File.Exists(filePath), $"Missing top-level control class file: {filePath}");
        }
    }

    [Fact]
    public void EveryPublicCodexControlClassHasAStyleSelector()
    {
        var root = FindRepositoryRoot();
        var controls = Path.Combine(root, "src", "CodexSwitchUI", "Controls");
        var styles = string.Join(
            Environment.NewLine,
            Directory.EnumerateFiles(Path.Combine(root, "src", "CodexSwitchUI", "Themes", "Controls"), "*.axaml")
                .Select(File.ReadAllText));

        var controlClasses = Directory.EnumerateFiles(controls, "Codex*.cs")
            .SelectMany(path => File.ReadLines(path))
            .Select(line => line.Trim())
            .Where(line => line.StartsWith("public class Codex", StringComparison.Ordinal)
                           || line.StartsWith("public abstract class Codex", StringComparison.Ordinal))
            .Select(line => line.Split([' ', ':', '('], StringSplitOptions.RemoveEmptyEntries)
                .First(token => token.StartsWith("Codex", StringComparison.Ordinal)))
            .Where(className => className is not "CodexFrame")
            .Where(className => !className.StartsWith("CodexControl", StringComparison.Ordinal))
            .Distinct()
            .OrderBy(className => className)
            .ToArray();

        var missing = controlClasses
            .Where(className => !styles.Contains($"controls|{className}", StringComparison.Ordinal))
            .ToArray();

        Assert.Empty(missing);
    }

    [Fact]
    public void HighRiskComponentsOwnTemplatesFocusAdornersAndCriticalParts()
    {
        var root = FindRepositoryRoot();
        var failures = new List<string>();

        foreach (var guard in HighRiskStyleGuards)
        {
            var style = ReadStyle(root, guard.Component);

            if (!style.Contains("ControlTemplate", StringComparison.Ordinal))
            {
                failures.Add($"{guard.Component}: missing ControlTemplate.");
            }

            if (!style.Contains("Transitions", StringComparison.Ordinal))
            {
                failures.Add($"{guard.Component}: missing Transitions.");
            }

            if (guard.RequiresFocusAdorner && !style.Contains("FocusAdorner\" Value=\"{x:Null}", StringComparison.Ordinal))
            {
                failures.Add($"{guard.Component}: missing FocusAdorner null guard; Fluent focus chrome may leak.");
            }

            foreach (var fragment in guard.RequiredFragments)
            {
                if (!style.Contains(fragment, StringComparison.Ordinal))
                {
                    failures.Add($"{guard.Component}: missing '{fragment}'.");
                }
            }

            if (style.Contains("BasedOn=", StringComparison.Ordinal)
                || style.Contains("Avalonia.Themes.Fluent", StringComparison.Ordinal)
                || style.Contains("FluentTheme", StringComparison.Ordinal))
            {
                failures.Add($"{guard.Component}: references a Fluent/BasedOn default style path.");
            }
        }

        AssertNoFailures(failures);
    }

    [Fact]
    public void HighRiskNativeItemSelectorsAreScopedAndTemplated()
    {
        var root = FindRepositoryRoot();
        var select = ReadStyle(root, "Select");
        var nativeSelect = ReadStyle(root, "NativeSelect");
        var menu = ReadStyle(root, "Menu");
        var contextMenu = ReadStyle(root, "ContextMenu");
        var menubar = ReadStyle(root, "Menubar");
        var tabs = ReadStyle(root, "Tabs");

        Assert.Contains("<ControlTemplate TargetType=\"ComboBoxItem\"", select);
        Assert.Contains("ComboBoxItem:selected", select);
        Assert.Contains("<ControlTemplate TargetType=\"ComboBoxItem\"", nativeSelect);
        Assert.Contains("<ControlTemplate TargetType=\"controls:CodexNativeSelectOption\"", nativeSelect);
        Assert.Contains("<ControlTemplate TargetType=\"controls:CodexNativeSelectOptGroup\"", nativeSelect);
        Assert.Contains("ComboBoxItem:selected", nativeSelect);

        Assert.DoesNotContain("<Style Selector=\"MenuItem\"", menu);
        Assert.Contains("controls|CodexMenu MenuItem", menu);
        Assert.Contains("PART_ItemRoot", menu);

        Assert.DoesNotContain("<Style Selector=\"MenuItem\"", contextMenu);
        Assert.Contains("controls|CodexContextMenu MenuItem", contextMenu);
        Assert.Contains("PART_ItemRoot", contextMenu);

        Assert.DoesNotContain("<Style Selector=\"MenuItem\"", menubar);
        Assert.Contains("controls|CodexMenubar MenuItem", menubar);
        Assert.Contains("PART_ItemRoot", menubar);

        Assert.DoesNotContain("<Style Selector=\"TabItem\"", tabs);
        Assert.Contains("controls|CodexTabs controls|CodexTabItem", tabs);
        Assert.Contains("PART_TriggerRoot", tabs);
    }

    [Fact]
    public void HighRiskNestedNativePartsDoNotDependOnDefaultTemplates()
    {
        var root = FindRepositoryRoot();
        var failures = new List<string>();
        var select = ReadStyle(root, "Select");

        if (select.Contains("<TextBox x:Name=\"PART_EditableTextBox\"", StringComparison.Ordinal))
        {
            failures.Add("Select: PART_EditableTextBox is a raw TextBox without its own template; editable mode may leak Fluent textbox chrome.");
        }

        AssertNoFailures(failures);
    }

    private static string FindRepositoryRoot()
    {
        return TestRepository.FindRoot();
    }

    private static string ReadStyle(string root, string component)
    {
        return File.ReadAllText(Path.Combine(root, "src", "CodexSwitchUI", "Themes", "Controls", $"{component}.axaml"));
    }

    private static void AssertNoFailures(IReadOnlyCollection<string> failures)
    {
        Assert.True(failures.Count == 0, string.Join(Environment.NewLine, failures));
    }

    private sealed record StyleGuard(string Component, bool RequiresFocusAdorner, string[] RequiredFragments);
}
