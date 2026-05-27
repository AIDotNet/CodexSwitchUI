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
