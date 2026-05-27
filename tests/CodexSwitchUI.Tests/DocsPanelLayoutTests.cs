using Xunit;

namespace CodexSwitchUI.Tests;

public class DocsPanelLayoutTests
{
    [Fact]
    public void DocsShellUsesCategorizedIndependentPageRegistry()
    {
        var source = ReadDocsSource("MainWindow.cs");
        var shell = ExtractMethod(source, "BuildDocsShell");

        Assert.Contains("private static readonly DocsCategory[] Categories", source);
        Assert.Contains("DocsPage", source);
        Assert.Contains("Page(\"overview.getting-started\"", source);
        Assert.Contains("Page(\"layout.application-shell\"", source);
        Assert.Contains("Page(\"layout.sidebar\"", source);
        Assert.Contains("Page(\"layout.sidebar-primitives\"", source);
        Assert.Contains("Page(\"layout.section\"", source);
        Assert.Contains("Page(\"layout.resizable\"", source);
        Assert.Contains("Page(\"forms.button\"", source);
        Assert.Contains("Page(\"forms.button-group\"", source);
        Assert.Contains("Page(\"forms.input-group\"", source);
        Assert.Contains("Page(\"forms.input-otp\"", source);
        Assert.Contains("Page(\"forms.label\"", source);
        Assert.Contains("Page(\"forms.icon-button\"", source);
        Assert.Contains("Page(\"forms.split-button\"", source);
        Assert.Contains("Page(\"forms.textbox\"", source);
        Assert.Contains("Page(\"forms.textarea\"", source);
        Assert.Contains("Page(\"forms.select\"", source);
        Assert.Contains("Page(\"forms.combobox\"", source);
        Assert.Contains("Page(\"forms.native-select\"", source);
        Assert.Contains("Page(\"forms.calendar\"", source);
        Assert.Contains("Page(\"forms.date-picker\"", source);
        Assert.Contains("Page(\"forms.checkbox\"", source);
        Assert.Contains("Page(\"forms.radio\"", source);
        Assert.Contains("Page(\"forms.radio-group\"", source);
        Assert.Contains("Page(\"forms.switch\"", source);
        Assert.Contains("Page(\"forms.toggle\"", source);
        Assert.Contains("Page(\"forms.toggle-group\"", source);
        Assert.Contains("Page(\"forms.slider\"", source);
        Assert.Contains("Page(\"feedback.alert\"", source);
        Assert.Contains("Page(\"feedback.toast\"", source);
        Assert.Contains("Page(\"feedback.sonner\"", source);
        Assert.Contains("Page(\"feedback.empty-state\"", source);
        Assert.Contains("Page(\"navigation.tabs\"", source);
        Assert.Contains("Page(\"navigation.breadcrumb\"", source);
        Assert.Contains("Page(\"navigation.side-nav\"", source);
        Assert.Contains("Page(\"navigation.segmented-control\"", source);
        Assert.Contains("Page(\"navigation.navigation-menu\"", source);
        Assert.Contains("Page(\"navigation.menubar\"", source);
        Assert.Contains("Page(\"navigation.dropdown\"", source);
        Assert.Contains("Page(\"navigation.menu\"", source);
        Assert.Contains("Page(\"navigation.context-menu\"", source);
        Assert.Contains("Page(\"navigation.command\"", source);
        Assert.Contains("Page(\"navigation.collapsible\"", source);
        Assert.Contains("Page(\"overlay.dialog\"", source);
        Assert.Contains("Page(\"overlay.command-dialog\"", source);
        Assert.Contains("Page(\"overlay.drawer\"", source);
        Assert.Contains("Page(\"overlay.popover\"", source);
        Assert.Contains("Page(\"overlay.tooltip\"", source);
        Assert.Contains("Page(\"overlay.hover-card\"", source);
        Assert.Contains("Page(\"data.card\"", source);
        Assert.Contains("Page(\"data.item\"", source);
        Assert.Contains("Page(\"data.aspect-ratio\"", source);
        Assert.Contains("Page(\"data.carousel\"", source);
        Assert.Contains("Page(\"data.bar-chart\"", source);
        Assert.Contains("Page(\"data.line-chart\"", source);
        Assert.Contains("Page(\"data.metric\"", source);
        Assert.Contains("Page(\"data.image-icon\"", source);
        Assert.Contains("Page(\"data.provider-card\"", source);
        Assert.Contains("Page(\"data.table\"", source);
        Assert.Contains("Page(\"data.pinned-table\"", source);
        Assert.Contains("Page(\"data.pagination\"", source);
        Assert.Contains("Page(\"data.scroll-area\"", source);
        Assert.Contains("Page(\"data.ranked-bar-chart\"", source);
        Assert.Contains("Page(\"data.usage-pie-chart\"", source);
        Assert.Contains("Page(\"data.usage-trend-chart\"", source);
        Assert.Contains("Page(\"primitives.typography\"", source);
        Assert.Contains("Page(\"primitives.focus-ring\"", source);
        Assert.Contains("Page(\"primitives.direction\"", source);
        Assert.Contains("Page(\"primitives.overlay\"", source);
        Assert.Contains("Page(\"tokens.motion\"", source);
        Assert.Contains("new ColumnDefinition(new GridLength(292))", shell);
        Assert.Contains("new ColumnDefinition(GridLength.Star)", shell);
        Assert.Contains("BuildPageNavigation()", shell);
        Assert.DoesNotContain("new ColumnDefinition(new GridLength(420))", shell);
        Assert.DoesNotContain("_rightRail", shell);
    }

    [Fact]
    public void SidebarNavigationUsesPageButtonsAndDoesNotRebuildTheScrollViewer()
    {
        var source = ReadDocsSource("MainWindow.cs");
        var navigation = ExtractMethod(source, "BuildPageNavigation");
        var navigate = ExtractMethod(source, "NavigateToPage");
        var refresh = ExtractMethod(source, "RefreshSidebarSelection");

        Assert.Contains("foreach (var category in Categories)", navigation);
        Assert.Contains("foreach (var page in category.Pages)", navigation);
        Assert.Contains("new CodexSidebarMenuButton", navigation);
        Assert.Contains("button.Click += (_, _) => NavigateToPage(page.Id);", navigation);
        Assert.Contains("_pageButtonsById[page.Id] = button;", navigation);
        Assert.Contains("ShowCachedPage(_pageSurface, _pageContentById, page.Id, () => BuildPageContent(page));", navigate);
        Assert.Contains("RefreshTopbar(page);", navigate);
        Assert.DoesNotContain("_pageHost.Content", navigate);
        Assert.DoesNotContain("_topbar.Child = BuildTopbar(page);", navigate);
        Assert.Contains("button.IsActive = pageId == _activePage.Id;", refresh);
        Assert.DoesNotContain("_pageButtonsById.Clear();", navigate);
    }

    [Fact]
    public void DocsNavigationCachesPagesToAvoidTransitionDetachCrashes()
    {
        var source = ReadDocsSource("MainWindow.cs");
        var shell = ExtractMethod(source, "BuildDocsShell");
        var navigate = ExtractMethod(source, "NavigateToPage");
        var showCachedPage = ExtractMethod(source, "ShowCachedPage");

        Assert.Contains("private readonly Grid _pageSurface = new();", source);
        Assert.Contains("private readonly Dictionary<string, Control> _pageContentById", source);
        Assert.Contains("Content = _pageSurface", shell);
        Assert.Contains("ShowCachedPage(_pageSurface", navigate);
        Assert.DoesNotContain("_rightRail", shell);
        Assert.DoesNotContain("BuildRightRail", source);
        Assert.Contains("control.IsVisible = false;", showCachedPage);
        Assert.Contains("host.Children.Add(control);", showCachedPage);
        Assert.Contains("child.IsVisible = ReferenceEquals(child, control);", showCachedPage);
    }

    [Fact]
    public void DocsTopbarUpdatesStableControlsToAvoidTransitionDetachCrashes()
    {
        var source = ReadDocsSource("MainWindow.cs");
        var shell = ExtractMethod(source, "BuildDocsShell");
        var topbar = ExtractMethod(source, "BuildTopbar");
        var navigate = ExtractMethod(source, "NavigateToPage");
        var applyTheme = ExtractMethod(source, "ApplyTheme");
        var refreshTopbar = ExtractMethod(source, "RefreshTopbar");

        Assert.Contains("private readonly CodexText _topbarTitle", source);
        Assert.Contains("private readonly CodexText _topbarMeta", source);
        Assert.Contains("private readonly CodexButton _lightThemeButton", source);
        Assert.Contains("_topbar.Child = BuildTopbar();", shell);
        Assert.Contains("_topbarTitle,", topbar);
        Assert.Contains("_topbarMeta", topbar);
        Assert.Contains("_lightThemeButton", topbar);
        Assert.Contains("RefreshTopbar(page);", navigate);
        Assert.Contains("RefreshTopbar(_activePage);", applyTheme);
        Assert.Contains("_topbarTitle.Text = page.Title;", refreshTopbar);
        Assert.Contains("_topbarMeta.Text = $\"{page.Category} / {page.SamplePath}\";", refreshTopbar);
        Assert.DoesNotContain("_topbar.Child = BuildTopbar(page);", source);
        Assert.DoesNotContain("_topbar.Child = BuildTopbar(_activePage);", source);
    }

    [Fact]
    public void InlineCodeLoadsAxamlSamplesIntoCopyableCodeBlock()
    {
        var mainWindow = ReadDocsSource("MainWindow.cs");
        var samples = ReadDocsSource(Path.Combine("Docs", "DocsCodeSamples.cs"));
        var inlineExample = ExtractMethod(mainWindow, "BuildInlineExample");

        Assert.Contains("DocsCodeSamples.Load(example.SamplePath)", inlineExample);
        Assert.Contains("new DocsCodeBlock", inlineExample);
        Assert.Contains("Title = example.SamplePath", inlineExample);
        Assert.Contains("Code = DocsCodeSamples.Load(example.SamplePath)", inlineExample);
        Assert.Contains("Path.Combine(root, \"Examples\", \"Axaml\", relativePath)", samples);
        Assert.Contains("AppContext.BaseDirectory", samples);
        Assert.Contains("CodexSwitchUI.Docs.csproj", samples);
        Assert.Contains("nestedDocsProject", samples);
        Assert.Contains("Path.GetDirectoryName(nestedDocsProject)!", samples);
        Assert.Contains("Missing AXAML sample", samples);
    }

    [Fact]
    public void PreviewSectionShowsInlineExpandableCodeForTheCurrentExample()
    {
        var mainWindow = ReadDocsSource("MainWindow.cs");
        var preview = ExtractMethod(mainWindow, "BuildPreviewSection");
        var inlineExample = ExtractMethod(mainWindow, "BuildInlineExample");

        Assert.Contains("foreach (var example in page.Examples)", preview);
        Assert.Contains("PreviewPanel(BuildInlineExample(example))", preview);
        Assert.Contains("new DocsCodeBlock", inlineExample);
        Assert.Contains("Title = example.SamplePath", inlineExample);
        Assert.Contains("Code = DocsCodeSamples.Load(example.SamplePath)", inlineExample);
        Assert.Contains("IsVisible = false", inlineExample);
        Assert.Contains("new CodexButton", inlineExample);
        Assert.Contains("Content = \"Show code\"", inlineExample);
        Assert.Contains("toggleCode.Click += (_, _) =>", inlineExample);
        Assert.Contains("codeBlock.IsVisible = !codeBlock.IsVisible;", inlineExample);
        Assert.Contains("toggleCode.Content = codeBlock.IsVisible ? \"Hide code\" : \"Show code\";", inlineExample);
        Assert.Contains("SectionHeader(example.Title, example.Description)", inlineExample);
        Assert.Contains("example.BuildPreview()", inlineExample);
        Assert.Contains("codeBlock", inlineExample);
    }

    [Fact]
    public void DocsCodeBlockUsesEditorChromeLineNumbersAndClipboardCopy()
    {
        var source = ReadDocsSource(Path.Combine("Controls", "DocsCodeBlock.cs"));

        Assert.Contains("#0B1020", source);
        Assert.Contains("#10172A", source);
        Assert.Contains("Dot(\"#F87171\")", source);
        Assert.Contains("FontFamily = new FontFamily(\"Menlo, Consolas, monospace\")", source);
        Assert.Contains("new SelectableTextBlock", source);
        Assert.Contains("TextWrapping = TextWrapping.NoWrap", source);
        Assert.Contains("(index + 1).ToString()", source);
        Assert.Contains("HorizontalScrollBarVisibility = ScrollBarVisibility.Auto", source);
        Assert.Contains("VerticalScrollBarVisibility = ScrollBarVisibility.Auto", source);
        Assert.Contains("_copyButton.Click += async (_, _) => await CopyCode();", source);
        Assert.Contains("_titleBlock.Text = Title;", source);
        Assert.Contains("_codeText.Text = string.IsNullOrEmpty(normalizedCode) ? \" \" : normalizedCode;", source);
        Assert.Contains("TopLevel.GetTopLevel(this)?.Clipboard", source);
        Assert.Contains("await clipboard.SetTextAsync(Code);", source);
        Assert.Contains("_copyButton.Content = \"Copied\";", source);
    }

    [Fact]
    public void DocsVisualFingerprintsTrackEveryRegisteredPage()
    {
        var mainWindow = ReadDocsSource("MainWindow.cs");
        var source = File.ReadAllText(Path.Combine(TestRepository.FindRoot(), "tests", "CodexSwitchUI.Tests", "DocsRenderedLifecycleTests.cs"));
        var registeredPages = ExtractSamplePaths(mainWindow);

        Assert.True(registeredPages.Count >= 57, $"Expected broad Docs page visual coverage, found {registeredPages.Count} pages.");
        Assert.Contains("private static IReadOnlyList<string> VisualFingerprintPages => AllRegisteredPageIds();", source);
        Assert.Contains("typeof(MainWindow).GetField(\"Categories\", BindingFlags.Static | BindingFlags.NonPublic)", source);
        Assert.Contains("pageIds.Count >= 57", source);
    }

    [Fact]
    public void ExampleAxamlFilesAreStandaloneCopiedSamples()
    {
        var root = DocsRoot();
        var project = ReadDocsSource("CodexSwitchUI.Docs.csproj");
        var requiredSamples = ExtractAllAxamlSamplePaths(ReadDocsSource("MainWindow.cs"));

        Assert.Contains("<AvaloniaResource Remove=\"Examples\\Axaml\\**\\*.axaml\" />", project);
        Assert.Contains("<AvaloniaResource Include=\"..\\..\\..\\CodexSwitch\\Assets\\icons\\*.png\"", project);
        Assert.Contains("Link=\"Assets\\icons\\%(Filename)%(Extension)\"", project);
        Assert.Contains("<None Update=\"Examples\\Axaml\\**\\*.axaml\">", project);
        Assert.Contains("<CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>", project);

        foreach (var sample in requiredSamples)
        {
            var path = Path.Combine(root, "Examples", "Axaml", sample.Replace('/', Path.DirectorySeparatorChar));
            Assert.True(File.Exists(path), $"Expected Docs AXAML sample: {sample}");
            Assert.Contains("<", File.ReadAllText(path));
        }
    }

    [Fact]
    public void EveryRegisteredMenuPageUsesAnIndependentSampleAndPreview()
    {
        var source = ReadDocsSource("MainWindow.cs");
        var samplePaths = ExtractSamplePaths(source);

        Assert.True(samplePaths.Count >= 57, $"Expected broad component coverage, found {samplePaths.Count} registered samples.");
        Assert.Equal(samplePaths.Count, samplePaths.Distinct(StringComparer.Ordinal).Count());
        Assert.DoesNotContain("BuildNavigationPlaceholder", source);
        Assert.DoesNotContain("BuildOverlayPlaceholder", source);
        Assert.DoesNotContain("BuildDataPlaceholder", source);
        Assert.DoesNotContain("Placeholder(", source);
        Assert.Contains("\"Layout/Sidebar.axaml\", BuildSidebarPreview", source);
        Assert.Contains("\"Layout/SidebarPrimitives.axaml\", BuildSidebarPrimitivesPreview", source);
        Assert.Contains("\"Layout/Resizable.axaml\", BuildResizablePreview", source);
        Assert.Contains("\"Layout/SidebarInteraction.axaml\", BuildSidebarInteractionPreview", source);
        Assert.Contains("\"Layout/ApplicationShellInteraction.axaml\", BuildApplicationShellInteractionPreview", source);
        Assert.Contains("\"Layout/SidebarPrimitivesInteraction.axaml\", BuildSidebarPrimitivesInteractionPreview", source);
        Assert.Contains("\"Layout/SectionInteraction.axaml\", BuildSectionComponentInteractionPreview", source);
        Assert.Contains("\"Layout/ResizableInteraction.axaml\", BuildResizableInteractionPreview", source);
        Assert.Contains("\"Forms/SplitButton.axaml\", BuildSplitButtonPreview", source);
        Assert.Contains("\"Forms/ButtonGroup.axaml\", BuildButtonGroupPreview", source);
        Assert.Contains("\"Forms/ButtonGroupComposition.axaml\", BuildButtonGroupCompositionPreview", source);
        Assert.Contains("\"Forms/ButtonGroupInteraction.axaml\", BuildButtonGroupInteractionPreview", source);
        Assert.Contains("\"Forms/InputGroup.axaml\", BuildInputGroupPreview", source);
        Assert.Contains("\"Forms/InputGroupComposition.axaml\", BuildInputGroupCompositionPreview", source);
        Assert.Contains("\"Forms/InputGroupInteraction.axaml\", BuildInputGroupInteractionPreview", source);
        Assert.Contains("\"Forms/InputOtp.axaml\", BuildInputOtpPreview", source);
        Assert.Contains("\"Forms/InputOtpComposition.axaml\", BuildInputOtpCompositionPreview", source);
        Assert.Contains("\"Forms/InputOtpInteraction.axaml\", BuildInputOtpInteractionPreview", source);
        Assert.Contains("\"Forms/Label.axaml\", BuildLabelPreview", source);
        Assert.Contains("\"Forms/LabelComposition.axaml\", BuildLabelCompositionPreview", source);
        Assert.Contains("\"Forms/LabelInteraction.axaml\", BuildLabelInteractionPreview", source);
        Assert.Contains("\"Forms/IconButton.axaml\", BuildIconButtonPreview", source);
        Assert.Contains("\"Forms/Textarea.axaml\", BuildTextareaPreview", source);
        Assert.Contains("\"Forms/Checkbox.axaml\", BuildCheckboxPreview", source);
        Assert.Contains("\"Forms/Radio.axaml\", BuildRadioPreview", source);
        Assert.Contains("\"Forms/RadioGroup.axaml\", BuildRadioGroupPreview", source);
        Assert.Contains("\"Forms/Switch.axaml\", BuildSwitchPreview", source);
        Assert.Contains("\"Forms/Toggle.axaml\", BuildTogglePreview", source);
        Assert.Contains("\"Forms/Slider.axaml\", BuildSliderPreview", source);
        Assert.Contains("\"Forms/Field.axaml\", BuildFieldPreview", source);
        Assert.Contains("\"Forms/ButtonAnatomy.axaml\", BuildButtonAnatomyPreview", source);
        Assert.Contains("\"Forms/ButtonInteraction.axaml\", BuildButtonInteractionPreview", source);
        Assert.Contains("\"Forms/IconButtonInteraction.axaml\", BuildIconButtonInteractionPreview", source);
        Assert.Contains("\"Forms/FieldAnatomy.axaml\", BuildFieldAnatomyPreview", source);
        Assert.Contains("\"Forms/FieldGroup.axaml\", BuildFieldGroupPreview", source);
        Assert.Contains("\"Forms/FieldInteraction.axaml\", BuildFieldInteractionPreview", source);
        Assert.Contains("\"Forms/TextBoxInteraction.axaml\", BuildTextBoxInteractionPreview", source);
        Assert.Contains("\"Forms/TextareaInteraction.axaml\", BuildTextareaInteractionPreview", source);
        Assert.Contains("\"Forms/SelectAnatomy.axaml\", BuildSelectAnatomyPreview", source);
        Assert.Contains("\"Forms/SelectInteraction.axaml\", BuildSelectInteractionPreview", source);
        Assert.Contains("\"Forms/Combobox.axaml\", BuildComboboxPreview", source);
        Assert.Contains("\"Forms/ComboboxStates.axaml\", BuildComboboxStatesPreview", source);
        Assert.Contains("\"Forms/ComboboxAnatomy.axaml\", BuildComboboxAnatomyPreview", source);
        Assert.Contains("\"Forms/ComboboxInteraction.axaml\", BuildComboboxInteractionPreview", source);
        Assert.Contains("\"Forms/NativeSelect.axaml\", BuildNativeSelectPreview", source);
        Assert.Contains("\"Forms/NativeSelectComposition.axaml\", BuildNativeSelectCompositionPreview", source);
        Assert.Contains("\"Forms/NativeSelectInteraction.axaml\", BuildNativeSelectInteractionPreview", source);
        Assert.Contains("\"Forms/Calendar.axaml\", BuildCalendarPreview", source);
        Assert.Contains("\"Forms/CalendarComposition.axaml\", BuildCalendarCompositionPreview", source);
        Assert.Contains("\"Forms/CalendarInteraction.axaml\", BuildCalendarInteractionPreview", source);
        Assert.Contains("\"Forms/DatePicker.axaml\", BuildDatePickerPreview", source);
        Assert.Contains("\"Forms/DatePickerStates.axaml\", BuildDatePickerStatesPreview", source);
        Assert.Contains("\"Forms/DatePickerAnatomy.axaml\", BuildDatePickerAnatomyPreview", source);
        Assert.Contains("\"Forms/DatePickerInteraction.axaml\", BuildDatePickerInteractionPreview", source);
        Assert.Contains("\"Forms/SplitButtonInteraction.axaml\", BuildSplitButtonInteractionPreview", source);
        Assert.Contains("\"Forms/CheckboxAnatomy.axaml\", BuildCheckboxAnatomyPreview", source);
        Assert.Contains("\"Forms/CheckboxInteraction.axaml\", BuildCheckboxInteractionPreview", source);
        Assert.Contains("\"Forms/RadioInteraction.axaml\", BuildRadioInteractionPreview", source);
        Assert.Contains("\"Forms/RadioGroupInteraction.axaml\", BuildRadioGroupInteractionPreview", source);
        Assert.Contains("\"Forms/SwitchInteraction.axaml\", BuildSwitchInteractionPreview", source);
        Assert.Contains("\"Forms/ToggleInteraction.axaml\", BuildToggleInteractionPreview", source);
        Assert.Contains("\"Forms/SliderInteraction.axaml\", BuildSliderInteractionPreview", source);
        Assert.Contains("\"Feedback/Alert.axaml\", BuildAlertPreview", source);
        Assert.Contains("\"Feedback/AlertAnatomy.axaml\", BuildAlertAnatomyPreview", source);
        Assert.Contains("\"Feedback/AlertInteraction.axaml\", BuildAlertInteractionPreview", source);
        Assert.Contains("\"Feedback/Toast.axaml\", BuildToastPreview", source);
        Assert.Contains("\"Feedback/ToastAnatomy.axaml\", BuildToastAnatomyPreview", source);
        Assert.Contains("\"Feedback/ToastInteraction.axaml\", BuildToastInteractionPreview", source);
        Assert.Contains("\"Feedback/Sonner.axaml\", BuildSonnerPreview", source);
        Assert.Contains("\"Feedback/SonnerLifecycle.axaml\", BuildSonnerLifecyclePreview", source);
        Assert.Contains("\"Feedback/BadgeInteraction.axaml\", BuildBadgeInteractionPreview", source);
        Assert.Contains("\"Feedback/BadgeAnatomy.axaml\", BuildBadgeAnatomyPreview", source);
        Assert.Contains("\"Feedback/AvatarInteraction.axaml\", BuildAvatarInteractionPreview", source);
        Assert.Contains("\"Feedback/AvatarAnatomy.axaml\", BuildAvatarAnatomyPreview", source);
        Assert.Contains("\"Feedback/EmptyStateInteraction.axaml\", BuildEmptyStateInteractionPreview", source);
        Assert.Contains("\"Feedback/SpinnerInteraction.axaml\", BuildSpinnerInteractionPreview", source);
        Assert.Contains("\"Feedback/ProgressInteraction.axaml\", BuildProgressInteractionPreview", source);
        Assert.Contains("\"Feedback/Skeleton.axaml\", BuildSkeletonPreview", source);
        Assert.Contains("\"Feedback/SkeletonInteraction.axaml\", BuildSkeletonInteractionPreview", source);
        Assert.Contains("\"Navigation/TabsAnatomy.axaml\", BuildTabsAnatomyPreview", source);
        Assert.Contains("\"Navigation/TabsInteraction.axaml\", BuildTabsInteractionPreview", source);
        Assert.Contains("\"Navigation/Breadcrumb.axaml\", BuildBreadcrumbPreview", source);
        Assert.Contains("\"Navigation/BreadcrumbAnatomy.axaml\", BuildBreadcrumbAnatomyPreview", source);
        Assert.Contains("\"Navigation/BreadcrumbInteraction.axaml\", BuildBreadcrumbInteractionPreview", source);
        Assert.Contains("\"Navigation/NavigationMenu.axaml\", BuildNavigationMenuPreview", source);
        Assert.Contains("\"Navigation/NavigationMenuInteraction.axaml\", BuildNavigationMenuInteractionPreview", source);
        Assert.Contains("\"Navigation/Menubar.axaml\", BuildMenubarPreview", source);
        Assert.Contains("\"Navigation/MenubarComposition.axaml\", BuildMenubarCompositionPreview", source);
        Assert.Contains("\"Navigation/MenubarInteraction.axaml\", BuildMenubarInteractionPreview", source);
        Assert.Contains("\"Navigation/SideNav.axaml\", BuildSideNavPreview", source);
        Assert.Contains("\"Navigation/SideNavInteraction.axaml\", BuildSideNavInteractionPreview", source);
        Assert.Contains("\"Navigation/SegmentedControl.axaml\", BuildSegmentedControlPreview", source);
        Assert.Contains("\"Navigation/SegmentedControlInteraction.axaml\", BuildSegmentedControlInteractionPreview", source);
        Assert.Contains("\"Navigation/NavigationMenuAnatomy.axaml\", BuildNavigationMenuAnatomyPreview", source);
        Assert.Contains("\"Navigation/DropdownButton.axaml\", BuildDropdownPreview", source);
        Assert.Contains("\"Navigation/DropdownButtonInteraction.axaml\", BuildDropdownInteractionPreview", source);
        Assert.Contains("\"Navigation/Menu.axaml\", BuildMenuPreview", source);
        Assert.Contains("\"Navigation/MenuAnatomy.axaml\", BuildMenuAnatomyPreview", source);
        Assert.Contains("\"Navigation/MenuInteraction.axaml\", BuildMenuInteractionPreview", source);
        Assert.Contains("\"Navigation/ContextMenu.axaml\", BuildContextMenuPreview", source);
        Assert.Contains("\"Navigation/ContextMenuAnatomy.axaml\", BuildContextMenuAnatomyPreview", source);
        Assert.Contains("\"Navigation/ContextMenuInteraction.axaml\", BuildContextMenuInteractionPreview", source);
        Assert.Contains("\"Navigation/Command.axaml\", BuildCommandPreview", source);
        Assert.Contains("\"Navigation/CommandAnatomy.axaml\", BuildCommandAnatomyPreview", source);
        Assert.Contains("\"Navigation/CommandFiltering.axaml\", BuildCommandFilteringPreview", source);
        Assert.Contains("\"Navigation/CommandScrollable.axaml\", BuildCommandScrollablePreview", source);
        Assert.Contains("\"Navigation/CommandInteraction.axaml\", BuildCommandInteractionPreview", source);
        Assert.Contains("\"Navigation/Accordion.axaml\", BuildAccordionPreview", source);
        Assert.Contains("\"Navigation/AccordionAnatomy.axaml\", BuildAccordionAnatomyPreview", source);
        Assert.Contains("\"Navigation/AccordionInteraction.axaml\", BuildAccordionInteractionPreview", source);
        Assert.Contains("\"Navigation/Collapsible.axaml\", BuildCollapsiblePreview", source);
        Assert.Contains("\"Navigation/CollapsibleInteraction.axaml\", BuildCollapsibleInteractionPreview", source);
        Assert.Contains("\"Navigation/SeparatorInteraction.axaml\", BuildSeparatorInteractionPreview", source);
        Assert.Contains("\"Navigation/KbdInteraction.axaml\", BuildKbdInteractionPreview", source);
        Assert.Contains("\"Overlay/Dialog.axaml\", BuildDialogPreview", source);
        Assert.Contains("\"Overlay/DialogAnatomy.axaml\", BuildDialogAnatomyPreview", source);
        Assert.Contains("\"Overlay/DialogInteraction.axaml\", BuildDialogInteractionPreview", source);
        Assert.Contains("\"Overlay/AlertDialog.axaml\", BuildAlertDialogPreview", source);
        Assert.Contains("\"Overlay/AlertDialogAnatomy.axaml\", BuildAlertDialogAnatomyPreview", source);
        Assert.Contains("\"Overlay/AlertDialogInteraction.axaml\", BuildAlertDialogInteractionPreview", source);
        Assert.Contains("\"Overlay/Sheet.axaml\", BuildSheetPreview", source);
        Assert.Contains("\"Overlay/SheetStates.axaml\", BuildSheetStatesPreview", source);
        Assert.Contains("\"Overlay/SheetAnatomy.axaml\", BuildSheetAnatomyPreview", source);
        Assert.Contains("\"Overlay/SheetInteraction.axaml\", BuildSheetInteractionPreview", source);
        Assert.Contains("\"Overlay/Drawer.axaml\", BuildDrawerPreview", source);
        Assert.Contains("\"Overlay/DrawerStates.axaml\", BuildDrawerStatesPreview", source);
        Assert.Contains("\"Overlay/DrawerAnatomy.axaml\", BuildDrawerAnatomyPreview", source);
        Assert.Contains("\"Overlay/DrawerInteraction.axaml\", BuildDrawerInteractionPreview", source);
        Assert.Contains("\"Overlay/CommandDialog.axaml\", BuildCommandDialogPreview", source);
        Assert.Contains("\"Overlay/CommandDialogAnatomy.axaml\", BuildCommandDialogAnatomyPreview", source);
        Assert.Contains("\"Overlay/CommandDialogInteraction.axaml\", BuildCommandDialogInteractionPreview", source);
        Assert.Contains("\"Overlay/Popover.axaml\", BuildPopoverPreview", source);
        Assert.Contains("\"Overlay/PopoverAnatomy.axaml\", BuildPopoverAnatomyPreview", source);
        Assert.Contains("\"Overlay/PopoverInteraction.axaml\", BuildPopoverInteractionPreview", source);
        Assert.Contains("\"Overlay/Tooltip.axaml\", BuildTooltipPreview", source);
        Assert.Contains("\"Overlay/TooltipInteraction.axaml\", BuildTooltipInteractionPreview", source);
        Assert.Contains("\"Overlay/HoverCard.axaml\", BuildHoverCardPreview", source);
        Assert.Contains("\"Overlay/HoverCardInteraction.axaml\", BuildHoverCardInteractionPreview", source);
        Assert.Contains("\"DataDisplay/Card.axaml\", BuildCardPreview", source);
        Assert.Contains("\"DataDisplay/Item.axaml\", BuildItemPreview", source);
        Assert.Contains("\"DataDisplay/ItemStates.axaml\", BuildItemStatesPreview", source);
        Assert.Contains("\"DataDisplay/ItemAnatomy.axaml\", BuildItemAnatomyPreview", source);
        Assert.Contains("\"DataDisplay/ItemInteraction.axaml\", BuildItemInteractionPreview", source);
        Assert.Contains("\"DataDisplay/AspectRatio.axaml\", BuildAspectRatioPreview", source);
        Assert.Contains("\"DataDisplay/AspectRatioStates.axaml\", BuildAspectRatioStatesPreview", source);
        Assert.Contains("\"DataDisplay/AspectRatioAnatomy.axaml\", BuildAspectRatioAnatomyPreview", source);
        Assert.Contains("\"DataDisplay/AspectRatioInteraction.axaml\", BuildAspectRatioInteractionPreview", source);
        Assert.Contains("\"DataDisplay/CardInteraction.axaml\", BuildCardInteractionPreview", source);
        Assert.Contains("\"DataDisplay/Carousel.axaml\", BuildCarouselPreview", source);
        Assert.Contains("\"DataDisplay/CarouselComposition.axaml\", BuildCarouselCompositionPreview", source);
        Assert.Contains("\"DataDisplay/CarouselInteraction.axaml\", BuildCarouselInteractionPreview", source);
        Assert.Contains("\"DataDisplay/Chart.axaml\", BuildChartPreview", source);
        Assert.Contains("\"DataDisplay/ChartAnatomy.axaml\", BuildChartAnatomyPreview", source);
        Assert.Contains("\"DataDisplay/ChartInteraction.axaml\", BuildChartInteractionPreview", source);
        Assert.Contains("\"DataDisplay/BarChart.axaml\", BuildBarChartPreview", source);
        Assert.Contains("\"DataDisplay/BarChartAnatomy.axaml\", BuildBarChartAnatomyPreview", source);
        Assert.Contains("\"DataDisplay/BarChartInteraction.axaml\", BuildBarChartInteractionPreview", source);
        Assert.Contains("\"DataDisplay/LineChart.axaml\", BuildLineChartPreview", source);
        Assert.Contains("\"DataDisplay/LineChartAnatomy.axaml\", BuildLineChartAnatomyPreview", source);
        Assert.Contains("\"DataDisplay/LineChartInteraction.axaml\", BuildLineChartInteractionPreview", source);
        Assert.Contains("\"DataDisplay/Metric.axaml\", BuildMetricPreview", source);
        Assert.Contains("\"DataDisplay/MetricInteraction.axaml\", BuildMetricInteractionPreview", source);
        Assert.Contains("\"DataDisplay/ImageIcon.axaml\", BuildImageIconPreview", source);
        Assert.Contains("\"DataDisplay/ImageIconInteraction.axaml\", BuildImageIconInteractionPreview", source);
        Assert.Contains("\"DataDisplay/ProviderCard.axaml\", BuildProviderCardPreview", source);
        Assert.Contains("\"DataDisplay/ProviderCardInteraction.axaml\", BuildProviderCardInteractionPreview", source);
        Assert.Contains("\"DataDisplay/Table.axaml\", BuildTablePreview", source);
        Assert.Contains("\"DataDisplay/TableAnatomy.axaml\", BuildTableAnatomyPreview", source);
        Assert.Contains("\"DataDisplay/TableInteraction.axaml\", BuildTableInteractionPreview", source);
        Assert.Contains("\"DataDisplay/DataTable.axaml\", BuildDataTablePreview", source);
        Assert.Contains("\"DataDisplay/DataTableAnatomy.axaml\", BuildDataTableAnatomyPreview", source);
        Assert.Contains("\"DataDisplay/DataTableInteraction.axaml\", BuildDataTableInteractionPreview", source);
        Assert.Contains("\"DataDisplay/PinnedTable.axaml\", BuildPinnedTablePreview", source);
        Assert.Contains("\"DataDisplay/PinnedTableInteraction.axaml\", BuildPinnedTableInteractionPreview", source);
        Assert.Contains("\"DataDisplay/Pagination.axaml\", BuildPaginationPreview", source);
        Assert.Contains("\"DataDisplay/PaginationInteraction.axaml\", BuildPaginationInteractionPreview", source);
        Assert.Contains("\"DataDisplay/ScrollArea.axaml\", BuildScrollAreaPreview", source);
        Assert.Contains("\"DataDisplay/ScrollAreaInteraction.axaml\", BuildScrollAreaInteractionPreview", source);
        Assert.Contains("\"DataDisplay/RankedBarChart.axaml\", BuildRankedBarChartPreview", source);
        Assert.Contains("\"DataDisplay/RankedBarChartInteraction.axaml\", BuildRankedBarChartInteractionPreview", source);
        Assert.Contains("\"DataDisplay/UsagePieChart.axaml\", BuildUsagePieChartPreview", source);
        Assert.Contains("\"DataDisplay/UsagePieChartInteraction.axaml\", BuildUsagePieChartInteractionPreview", source);
        Assert.Contains("\"DataDisplay/UsageTrendChart.axaml\", BuildUsageTrendChartPreview", source);
        Assert.Contains("\"DataDisplay/UsageTrendChartInteraction.axaml\", BuildUsageTrendChartInteractionPreview", source);
        Assert.Contains("\"Primitives/Typography.axaml\", BuildTypographyPreview", source);
        Assert.Contains("\"Primitives/TypographyInteraction.axaml\", BuildTypographyInteractionPreview", source);
        Assert.Contains("\"Primitives/FocusRing.axaml\", BuildFocusRingPreview", source);
        Assert.Contains("\"Primitives/FocusRingInteraction.axaml\", BuildFocusRingInteractionPreview", source);
        Assert.Contains("\"Primitives/Direction.axaml\", BuildDirectionPreview", source);
        Assert.Contains("\"Primitives/DirectionInteraction.axaml\", BuildDirectionInteractionPreview", source);
        Assert.Contains("\"Primitives/Overlay.axaml\", BuildOverlayPrimitivePreview", source);
        Assert.Contains("\"Primitives/OverlayInteraction.axaml\", BuildOverlayPrimitiveInteractionPreview", source);
        Assert.Contains("\"Tokens/Motion.axaml\", BuildTokenPreview", source);
        Assert.Contains("\"Tokens/MotionInteraction.axaml\", BuildMotionInteractionPreview", source);

        foreach (var sample in samplePaths)
        {
            var path = Path.Combine(DocsRoot(), "Examples", "Axaml", sample.Replace('/', Path.DirectorySeparatorChar));
            Assert.True(File.Exists(path), $"Registered docs sample does not exist: {sample}");
        }
    }

    [Fact]
    public void DocsSamplesCoverEveryTopLevelComponentFamily()
    {
        var source = ReadDocsSource("MainWindow.cs");
        var samplePaths = ExtractSamplePaths(source);
        var expectedComponentSamples = new[]
        {
            "Layout/ApplicationShell.axaml",
            "Layout/SidebarPrimitives.axaml",
            "Layout/Section.axaml",
            "Layout/Resizable.axaml",
            "Forms/Button.axaml",
            "Forms/ButtonGroup.axaml",
            "Forms/InputGroup.axaml",
            "Forms/InputOtp.axaml",
            "Forms/Label.axaml",
            "Forms/IconButton.axaml",
            "Forms/SplitButton.axaml",
            "Forms/Field.axaml",
            "Forms/TextBox.axaml",
            "Forms/Textarea.axaml",
            "Forms/Select.axaml",
            "Forms/Combobox.axaml",
            "Forms/NativeSelect.axaml",
            "Forms/Calendar.axaml",
            "Forms/DatePicker.axaml",
            "Forms/Checkbox.axaml",
            "Forms/Radio.axaml",
            "Forms/RadioGroup.axaml",
            "Forms/Switch.axaml",
            "Forms/Toggle.axaml",
            "Forms/ToggleGroup.axaml",
            "Forms/Slider.axaml",
            "Navigation/Tabs.axaml",
            "Navigation/Breadcrumb.axaml",
            "Navigation/SideNav.axaml",
            "Navigation/SegmentedControl.axaml",
            "Navigation/NavigationMenu.axaml",
            "Navigation/Menubar.axaml",
            "Navigation/DropdownButton.axaml",
            "Navigation/Menu.axaml",
            "Navigation/ContextMenu.axaml",
            "Navigation/Command.axaml",
            "Navigation/Accordion.axaml",
            "Navigation/Collapsible.axaml",
            "Navigation/Separator.axaml",
            "Navigation/Kbd.axaml",
            "Overlay/Dialog.axaml",
            "Overlay/AlertDialog.axaml",
            "Overlay/Sheet.axaml",
            "Overlay/Drawer.axaml",
            "Overlay/CommandDialog.axaml",
            "Overlay/Popover.axaml",
            "Overlay/Tooltip.axaml",
            "Overlay/HoverCard.axaml",
            "Feedback/Alert.axaml",
            "Feedback/Badge.axaml",
            "Feedback/Avatar.axaml",
            "Feedback/EmptyState.axaml",
            "Feedback/Toast.axaml",
            "Feedback/Sonner.axaml",
            "Feedback/Spinner.axaml",
            "Feedback/Progress.axaml",
            "Feedback/Skeleton.axaml",
            "DataDisplay/Card.axaml",
            "DataDisplay/Item.axaml",
            "DataDisplay/AspectRatio.axaml",
            "DataDisplay/Carousel.axaml",
            "DataDisplay/Chart.axaml",
            "DataDisplay/BarChart.axaml",
            "DataDisplay/LineChart.axaml",
            "DataDisplay/Metric.axaml",
            "DataDisplay/ImageIcon.axaml",
            "DataDisplay/ProviderCard.axaml",
            "DataDisplay/Table.axaml",
            "DataDisplay/DataTable.axaml",
            "DataDisplay/PinnedTable.axaml",
            "DataDisplay/Pagination.axaml",
            "DataDisplay/ScrollArea.axaml",
            "DataDisplay/RankedBarChart.axaml",
            "DataDisplay/UsagePieChart.axaml",
            "DataDisplay/UsageTrendChart.axaml",
            "Primitives/Typography.axaml",
            "Primitives/FocusRing.axaml",
            "Primitives/Direction.axaml",
            "Primitives/Overlay.axaml"
        };

        foreach (var sample in expectedComponentSamples)
        {
            Assert.Contains(sample, samplePaths);
        }
    }

    [Fact]
    public void DocsPagesSupportMultipleStateExamplesWithOwnSamples()
    {
        var source = ReadDocsSource("MainWindow.cs");
        var exampleCases = ExtractMethod(source, "ExampleCasesFor");
        var allSamples = ExtractAllAxamlSamplePaths(source);
        var stateSamples = new[]
        {
            "Layout/ApplicationShellStates.axaml",
            "Layout/SidebarStates.axaml",
            "Layout/SidebarPrimitivesStates.axaml",
            "Layout/SectionStates.axaml",
            "Layout/ResizableStates.axaml",
            "Forms/ButtonStates.axaml",
            "Forms/ButtonGroupStates.axaml",
            "Forms/InputGroupStates.axaml",
            "Forms/InputOtpStates.axaml",
            "Forms/LabelStates.axaml",
            "Forms/IconButtonStates.axaml",
            "Forms/SplitButtonStates.axaml",
            "Forms/TextBoxStates.axaml",
            "Forms/TextareaStates.axaml",
            "Forms/SelectStates.axaml",
            "Forms/ComboboxStates.axaml",
            "Forms/NativeSelectStates.axaml",
            "Forms/CalendarStates.axaml",
            "Forms/DatePickerStates.axaml",
            "Forms/FieldStates.axaml",
            "Forms/CheckboxStates.axaml",
            "Forms/RadioStates.axaml",
            "Forms/RadioGroupStates.axaml",
            "Forms/SwitchStates.axaml",
            "Forms/ToggleStates.axaml",
            "Forms/ToggleGroupStates.axaml",
            "Forms/SliderStates.axaml",
            "Feedback/AlertStates.axaml",
            "Feedback/BadgeStates.axaml",
            "Feedback/AvatarStates.axaml",
            "Feedback/EmptyStateStates.axaml",
            "Feedback/ToastStates.axaml",
            "Feedback/SonnerStates.axaml",
            "Feedback/SpinnerStates.axaml",
            "Feedback/ProgressStates.axaml",
            "Feedback/SkeletonStates.axaml",
            "Navigation/TabsStates.axaml",
            "Navigation/BreadcrumbStates.axaml",
            "Navigation/SideNavStates.axaml",
            "Navigation/SegmentedControlStates.axaml",
            "Navigation/NavigationMenuStates.axaml",
            "Navigation/MenubarStates.axaml",
            "Navigation/DropdownButtonStates.axaml",
            "Navigation/MenuStates.axaml",
            "Navigation/ContextMenuStates.axaml",
            "Navigation/CommandStates.axaml",
            "Navigation/AccordionStates.axaml",
            "Navigation/CollapsibleStates.axaml",
            "Navigation/SeparatorStates.axaml",
            "Navigation/KbdStates.axaml",
            "Overlay/DialogStates.axaml",
            "Overlay/AlertDialogStates.axaml",
            "Overlay/SheetStates.axaml",
            "Overlay/DrawerStates.axaml",
            "Overlay/CommandDialogStates.axaml",
            "Overlay/PopoverStates.axaml",
            "Overlay/TooltipStates.axaml",
            "Overlay/HoverCardStates.axaml",
            "DataDisplay/CardStates.axaml",
            "DataDisplay/ItemStates.axaml",
            "DataDisplay/AspectRatioStates.axaml",
            "DataDisplay/CarouselStates.axaml",
            "DataDisplay/ChartStates.axaml",
            "DataDisplay/BarChartStates.axaml",
            "DataDisplay/LineChartStates.axaml",
            "DataDisplay/MetricStates.axaml",
            "DataDisplay/ImageIconStates.axaml",
            "DataDisplay/ProviderCardStates.axaml",
            "DataDisplay/PinnedTableStates.axaml",
            "DataDisplay/PaginationStates.axaml",
            "DataDisplay/TableStates.axaml",
            "DataDisplay/DataTableStates.axaml",
            "DataDisplay/ScrollAreaStates.axaml",
            "DataDisplay/RankedBarChartStates.axaml",
            "DataDisplay/UsagePieChartStates.axaml",
            "DataDisplay/UsageTrendChartStates.axaml",
            "Primitives/TypographyStates.axaml",
            "Primitives/FocusRingStates.axaml",
            "Primitives/DirectionStates.axaml",
            "Primitives/OverlayStates.axaml",
            "Tokens/MotionStates.axaml"
        };
        var anatomySamples = new[]
        {
            "Layout/SidebarAnatomy.axaml",
            "Forms/ButtonAnatomy.axaml",
            "Layout/ResizableComposition.axaml",
            "Forms/ButtonGroupComposition.axaml",
            "Forms/InputGroupComposition.axaml",
            "Forms/InputOtpComposition.axaml",
            "Forms/LabelComposition.axaml",
            "Forms/SelectAnatomy.axaml",
            "Forms/ComboboxAnatomy.axaml",
            "Forms/NativeSelectComposition.axaml",
            "Forms/CalendarComposition.axaml",
            "Forms/DatePickerAnatomy.axaml",
            "Forms/CheckboxAnatomy.axaml",
            "Forms/RadioGroupAnatomy.axaml",
            "Forms/ToggleGroupAnatomy.axaml",
            "Forms/FieldGroup.axaml",
            "DataDisplay/AspectRatioAnatomy.axaml",
            "DataDisplay/ItemAnatomy.axaml",
            "DataDisplay/DataTableAnatomy.axaml",
            "DataDisplay/ChartAnatomy.axaml",
            "DataDisplay/BarChartAnatomy.axaml",
            "DataDisplay/LineChartAnatomy.axaml",
            "DataDisplay/CarouselComposition.axaml",
            "Navigation/MenubarComposition.axaml",
            "Forms/FieldAnatomy.axaml",
            "Feedback/ToastAnatomy.axaml",
            "Navigation/TabsAnatomy.axaml",
            "Navigation/BreadcrumbAnatomy.axaml",
            "Navigation/NavigationMenuAnatomy.axaml",
            "Navigation/CommandAnatomy.axaml",
            "Navigation/AccordionAnatomy.axaml",
            "Navigation/KbdAnatomy.axaml",
            "Overlay/DialogAnatomy.axaml",
            "Overlay/AlertDialogAnatomy.axaml",
            "Overlay/SheetAnatomy.axaml",
            "Overlay/DrawerAnatomy.axaml",
            "Overlay/CommandDialogAnatomy.axaml",
            "Overlay/PopoverAnatomy.axaml",
            "Overlay/TooltipAnatomy.axaml",
            "Overlay/HoverCardAnatomy.axaml"
        };
        var interactionSamples = new[]
        {
            "Layout/ApplicationShellInteraction.axaml",
            "Layout/SidebarInteraction.axaml",
            "Layout/SidebarPrimitivesInteraction.axaml",
            "Layout/SectionInteraction.axaml",
            "Layout/ResizableInteraction.axaml",
            "Forms/ButtonInteraction.axaml",
            "Forms/ButtonGroupInteraction.axaml",
            "Forms/InputGroupInteraction.axaml",
            "Forms/InputOtpInteraction.axaml",
            "Forms/LabelInteraction.axaml",
            "Forms/IconButtonInteraction.axaml",
            "Forms/TextBoxInteraction.axaml",
            "Forms/TextareaInteraction.axaml",
            "Forms/SelectInteraction.axaml",
            "Forms/ComboboxInteraction.axaml",
            "Forms/NativeSelectInteraction.axaml",
            "Forms/CalendarInteraction.axaml",
            "Forms/DatePickerInteraction.axaml",
            "Forms/FieldInteraction.axaml",
            "Forms/SplitButtonInteraction.axaml",
            "Forms/CheckboxInteraction.axaml",
            "Forms/RadioInteraction.axaml",
            "Forms/RadioGroupInteraction.axaml",
            "Forms/SwitchInteraction.axaml",
            "Forms/ToggleInteraction.axaml",
            "Forms/ToggleGroupInteraction.axaml",
            "Forms/SliderInteraction.axaml",
            "Feedback/AlertInteraction.axaml",
            "Feedback/BadgeInteraction.axaml",
            "Feedback/AvatarInteraction.axaml",
            "Feedback/EmptyStateInteraction.axaml",
            "Feedback/ToastInteraction.axaml",
            "Feedback/SonnerLifecycle.axaml",
            "Feedback/SpinnerInteraction.axaml",
            "Feedback/ProgressInteraction.axaml",
            "Feedback/SkeletonInteraction.axaml",
            "Navigation/TabsInteraction.axaml",
            "Navigation/BreadcrumbInteraction.axaml",
            "Navigation/NavigationMenuInteraction.axaml",
            "Navigation/MenubarInteraction.axaml",
            "Navigation/SideNavInteraction.axaml",
            "Navigation/SegmentedControlInteraction.axaml",
            "Navigation/DropdownButtonInteraction.axaml",
            "Navigation/MenuInteraction.axaml",
            "Navigation/ContextMenuInteraction.axaml",
            "Navigation/CommandFiltering.axaml",
            "Navigation/CommandScrollable.axaml",
            "Navigation/CommandInteraction.axaml",
            "Navigation/AccordionInteraction.axaml",
            "Navigation/CollapsibleInteraction.axaml",
            "Navigation/SeparatorInteraction.axaml",
            "Navigation/KbdInteraction.axaml",
            "Overlay/DialogInteraction.axaml",
            "Overlay/AlertDialogInteraction.axaml",
            "Overlay/SheetInteraction.axaml",
            "Overlay/DrawerInteraction.axaml",
            "Overlay/CommandDialogInteraction.axaml",
            "Overlay/PopoverInteraction.axaml",
            "Overlay/TooltipInteraction.axaml",
            "Overlay/HoverCardInteraction.axaml",
            "DataDisplay/CardInteraction.axaml",
            "DataDisplay/ItemInteraction.axaml",
            "DataDisplay/AspectRatioInteraction.axaml",
            "DataDisplay/CarouselInteraction.axaml",
            "DataDisplay/ChartInteraction.axaml",
            "DataDisplay/BarChartInteraction.axaml",
            "DataDisplay/LineChartInteraction.axaml",
            "DataDisplay/MetricInteraction.axaml",
            "DataDisplay/ImageIconInteraction.axaml",
            "DataDisplay/ProviderCardInteraction.axaml",
            "DataDisplay/TableInteraction.axaml",
            "DataDisplay/DataTableInteraction.axaml",
            "DataDisplay/PinnedTableInteraction.axaml",
            "DataDisplay/PaginationInteraction.axaml",
            "DataDisplay/ScrollAreaInteraction.axaml",
            "DataDisplay/RankedBarChartInteraction.axaml",
            "DataDisplay/UsagePieChartInteraction.axaml",
            "DataDisplay/UsageTrendChartInteraction.axaml",
            "Primitives/TypographyInteraction.axaml",
            "Primitives/FocusRingInteraction.axaml",
            "Primitives/DirectionInteraction.axaml",
            "Primitives/OverlayInteraction.axaml",
            "Tokens/MotionInteraction.axaml"
        };

        Assert.Contains("new List<DocsExampleCase>", exampleCases);
        Assert.Contains("Example(\"Default\"", exampleCases);
        Assert.Contains("examples.AddRange(pageId switch", exampleCases);
        Assert.Contains("DataDisplay/TableAnatomy.axaml", allSamples);
        Assert.Contains("BuildTableAnatomyPreview", exampleCases);
        Assert.Contains("DataDisplay/DataTableAnatomy.axaml", allSamples);
        Assert.Contains("BuildDataTableAnatomyPreview", exampleCases);
        Assert.Contains("DataDisplay/DataTableInteraction.axaml", allSamples);
        Assert.Contains("BuildDataTableInteractionPreview", exampleCases);
        Assert.Contains("DataDisplay/ChartAnatomy.axaml", allSamples);
        Assert.Contains("BuildChartAnatomyPreview", exampleCases);
        Assert.Contains("DataDisplay/ChartInteraction.axaml", allSamples);
        Assert.Contains("BuildChartInteractionPreview", exampleCases);
        Assert.Contains("DataDisplay/BarChartAnatomy.axaml", allSamples);
        Assert.Contains("BuildBarChartAnatomyPreview", exampleCases);
        Assert.Contains("DataDisplay/BarChartInteraction.axaml", allSamples);
        Assert.Contains("BuildBarChartInteractionPreview", exampleCases);
        Assert.Contains("Navigation/MenuAnatomy.axaml", allSamples);
        Assert.Contains("BuildMenuAnatomyPreview", exampleCases);
        Assert.Contains("Navigation/ContextMenuAnatomy.axaml", allSamples);
        Assert.Contains("BuildContextMenuAnatomyPreview", exampleCases);
        Assert.Contains("Navigation/CommandAnatomy.axaml", allSamples);
        Assert.Contains("BuildCommandAnatomyPreview", exampleCases);
        Assert.Contains("Navigation/CommandFiltering.axaml", allSamples);
        Assert.Contains("BuildCommandFilteringPreview", exampleCases);
        Assert.Contains("Navigation/CommandScrollable.axaml", allSamples);
        Assert.Contains("BuildCommandScrollablePreview", exampleCases);
        Assert.Contains("Navigation/AccordionAnatomy.axaml", allSamples);
        Assert.Contains("BuildAccordionAnatomyPreview", exampleCases);
        Assert.Contains("BuildButtonAnatomyPreview", exampleCases);
        Assert.Contains("Forms/ButtonGroupComposition.axaml", allSamples);
        Assert.Contains("BuildButtonGroupCompositionPreview", exampleCases);
        Assert.Contains("Forms/InputGroupComposition.axaml", allSamples);
        Assert.Contains("BuildInputGroupCompositionPreview", exampleCases);
        Assert.Contains("Forms/InputOtpComposition.axaml", allSamples);
        Assert.Contains("BuildInputOtpCompositionPreview", exampleCases);
        Assert.Contains("Forms/LabelComposition.axaml", allSamples);
        Assert.Contains("BuildLabelCompositionPreview", exampleCases);
        Assert.Contains("Forms/ComboboxAnatomy.axaml", allSamples);
        Assert.Contains("BuildComboboxAnatomyPreview", exampleCases);
        Assert.Contains("Forms/NativeSelectComposition.axaml", allSamples);
        Assert.Contains("BuildNativeSelectCompositionPreview", exampleCases);
        Assert.Contains("Forms/CalendarComposition.axaml", allSamples);
        Assert.Contains("BuildCalendarCompositionPreview", exampleCases);
        Assert.Contains("Forms/DatePickerAnatomy.axaml", allSamples);
        Assert.Contains("BuildDatePickerAnatomyPreview", exampleCases);
        Assert.Contains("DataDisplay/CarouselComposition.axaml", allSamples);
        Assert.Contains("BuildCarouselCompositionPreview", exampleCases);
        Assert.Contains("DataDisplay/ChartAnatomy.axaml", allSamples);
        Assert.Contains("BuildChartAnatomyPreview", exampleCases);
        Assert.Contains("DataDisplay/AspectRatioAnatomy.axaml", allSamples);
        Assert.Contains("BuildAspectRatioAnatomyPreview", exampleCases);
        Assert.Contains("Navigation/MenubarComposition.axaml", allSamples);
        Assert.Contains("BuildMenubarCompositionPreview", exampleCases);
        Assert.Contains("BuildFieldAnatomyPreview", exampleCases);
        Assert.Contains("Forms/FieldGroup.axaml", allSamples);
        Assert.Contains("BuildFieldGroupPreview", exampleCases);
        Assert.Contains("BuildToastAnatomyPreview", exampleCases);
        Assert.Contains("BuildApplicationShellInteractionPreview", exampleCases);
        Assert.Contains("BuildSidebarPrimitivesInteractionPreview", exampleCases);
        Assert.Contains("BuildSectionComponentInteractionPreview", exampleCases);
        Assert.Contains("Layout/ResizableComposition.axaml", allSamples);
        Assert.Contains("BuildResizableCompositionPreview", exampleCases);
        Assert.Contains("BuildResizableInteractionPreview", exampleCases);
        Assert.Contains("BuildTabsAnatomyPreview", exampleCases);
        Assert.Contains("BuildTabsInteractionPreview", exampleCases);
        Assert.Contains("BuildBreadcrumbAnatomyPreview", exampleCases);
        Assert.Contains("BuildBreadcrumbInteractionPreview", exampleCases);
        Assert.Contains("BuildDialogAnatomyPreview", exampleCases);
        Assert.Contains("Overlay/AlertDialogAnatomy.axaml", allSamples);
        Assert.Contains("BuildAlertDialogAnatomyPreview", exampleCases);
        Assert.Contains("BuildSheetAnatomyPreview", exampleCases);
        Assert.Contains("BuildDrawerAnatomyPreview", exampleCases);
        Assert.Contains("Overlay/CommandDialogAnatomy.axaml", allSamples);
        Assert.Contains("BuildCommandDialogAnatomyPreview", exampleCases);
        Assert.Contains("BuildPopoverAnatomyPreview", exampleCases);
        Assert.Contains("BuildDialogInteractionPreview", exampleCases);
        Assert.Contains("BuildAlertDialogInteractionPreview", exampleCases);
        Assert.Contains("BuildSheetInteractionPreview", exampleCases);
        Assert.Contains("BuildDrawerInteractionPreview", exampleCases);
        Assert.Contains("BuildPopoverInteractionPreview", exampleCases);
        Assert.Contains("Overlay/TooltipAnatomy.axaml", allSamples);
        Assert.Contains("BuildTooltipAnatomyPreview", exampleCases);
        Assert.Contains("BuildTextBoxInteractionPreview", exampleCases);
        Assert.Contains("BuildTextareaInteractionPreview", exampleCases);
        Assert.Contains("BuildButtonInteractionPreview", exampleCases);
        Assert.Contains("BuildButtonGroupInteractionPreview", exampleCases);
        Assert.Contains("BuildInputGroupInteractionPreview", exampleCases);
        Assert.Contains("BuildInputOtpInteractionPreview", exampleCases);
        Assert.Contains("BuildLabelInteractionPreview", exampleCases);
        Assert.Contains("BuildIconButtonInteractionPreview", exampleCases);
        Assert.Contains("BuildSelectInteractionPreview", exampleCases);
        Assert.Contains("Forms/SelectAnatomy.axaml", allSamples);
        Assert.Contains("BuildSelectAnatomyPreview", exampleCases);
        Assert.Contains("BuildComboboxStatesPreview", exampleCases);
        Assert.Contains("BuildComboboxInteractionPreview", exampleCases);
        Assert.Contains("BuildNativeSelectInteractionPreview", exampleCases);
        Assert.Contains("BuildCalendarInteractionPreview", exampleCases);
        Assert.Contains("BuildDatePickerInteractionPreview", exampleCases);
        Assert.Contains("BuildCarouselInteractionPreview", exampleCases);
        Assert.Contains("BuildFieldInteractionPreview", exampleCases);
        Assert.Contains("BuildSplitButtonInteractionPreview", exampleCases);
        Assert.Contains("BuildCheckboxAnatomyPreview", exampleCases);
        Assert.Contains("BuildCheckboxInteractionPreview", exampleCases);
        Assert.Contains("BuildRadioInteractionPreview", exampleCases);
        Assert.Contains("BuildRadioGroupAnatomyPreview", exampleCases);
        Assert.Contains("BuildRadioGroupInteractionPreview", exampleCases);
        Assert.Contains("BuildSwitchInteractionPreview", exampleCases);
        Assert.Contains("Forms/ToggleGroup.axaml", allSamples);
        Assert.Contains("\"Forms/ToggleGroup.axaml\", BuildToggleGroupPreview", source);
        Assert.Contains("Forms/ToggleGroupStates.axaml", allSamples);
        Assert.Contains("BuildToggleGroupStatesPreview", exampleCases);
        Assert.Contains("Forms/ToggleGroupAnatomy.axaml", allSamples);
        Assert.Contains("BuildToggleGroupAnatomyPreview", exampleCases);
        Assert.Contains("Forms/ToggleGroupInteraction.axaml", allSamples);
        Assert.Contains("BuildToggleGroupInteractionPreview", exampleCases);
        Assert.Contains("BuildToggleInteractionPreview", exampleCases);
        Assert.Contains("BuildSliderInteractionPreview", exampleCases);
        Assert.Contains("BuildAlertInteractionPreview", exampleCases);
        Assert.Contains("Feedback/AlertAnatomy.axaml", allSamples);
        Assert.Contains("BuildAlertAnatomyPreview", exampleCases);
        Assert.Contains("BuildBadgeInteractionPreview", exampleCases);
        Assert.Contains("Feedback/BadgeAnatomy.axaml", allSamples);
        Assert.Contains("BuildBadgeAnatomyPreview", exampleCases);
        Assert.Contains("BuildAvatarInteractionPreview", exampleCases);
        Assert.Contains("Feedback/AvatarAnatomy.axaml", allSamples);
        Assert.Contains("BuildAvatarAnatomyPreview", exampleCases);
        Assert.Contains("BuildEmptyStateInteractionPreview", exampleCases);
        Assert.Contains("BuildToastInteractionPreview", exampleCases);
        Assert.Contains("BuildSonnerLifecyclePreview", exampleCases);
        Assert.Contains("BuildSpinnerInteractionPreview", exampleCases);
        Assert.Contains("BuildProgressInteractionPreview", exampleCases);
        Assert.Contains("BuildSkeletonInteractionPreview", exampleCases);
        Assert.Contains("BuildNavigationMenuInteractionPreview", exampleCases);
        Assert.Contains("Navigation/NavigationMenuAnatomy.axaml", allSamples);
        Assert.Contains("BuildNavigationMenuAnatomyPreview", exampleCases);
        Assert.Contains("BuildMenubarInteractionPreview", exampleCases);
        Assert.Contains("BuildSideNavInteractionPreview", exampleCases);
        Assert.Contains("BuildSegmentedControlInteractionPreview", exampleCases);
        Assert.Contains("BuildDropdownInteractionPreview", exampleCases);
        Assert.Contains("BuildMenuInteractionPreview", exampleCases);
        Assert.Contains("BuildContextMenuInteractionPreview", exampleCases);
        Assert.Contains("BuildCommandInteractionPreview", exampleCases);
        Assert.Contains("BuildAccordionInteractionPreview", exampleCases);
        Assert.Contains("BuildCollapsibleInteractionPreview", exampleCases);
        Assert.Contains("BuildSeparatorInteractionPreview", exampleCases);
        Assert.Contains("Navigation/KbdAnatomy.axaml", allSamples);
        Assert.Contains("BuildKbdAnatomyPreview", exampleCases);
        Assert.Contains("BuildKbdInteractionPreview", exampleCases);
        Assert.Contains("BuildCommandDialogInteractionPreview", exampleCases);
        Assert.Contains("BuildTooltipInteractionPreview", exampleCases);
        Assert.Contains("Overlay/HoverCardAnatomy.axaml", allSamples);
        Assert.Contains("BuildHoverCardAnatomyPreview", exampleCases);
        Assert.Contains("BuildHoverCardInteractionPreview", exampleCases);
        Assert.Contains("BuildCardInteractionPreview", exampleCases);
        Assert.Contains("BuildItemStatesPreview", exampleCases);
        Assert.Contains("BuildItemAnatomyPreview", exampleCases);
        Assert.Contains("BuildItemInteractionPreview", exampleCases);
        Assert.Contains("BuildAspectRatioStatesPreview", exampleCases);
        Assert.Contains("BuildAspectRatioInteractionPreview", exampleCases);
        Assert.Contains("BuildMetricInteractionPreview", exampleCases);
        Assert.Contains("BuildImageIconInteractionPreview", exampleCases);
        Assert.Contains("BuildProviderCardInteractionPreview", exampleCases);
        Assert.Contains("BuildTableInteractionPreview", exampleCases);
        Assert.Contains("BuildDataTableInteractionPreview", exampleCases);
        Assert.Contains("BuildChartInteractionPreview", exampleCases);
        Assert.Contains("BuildBarChartAnatomyPreview", exampleCases);
        Assert.Contains("BuildBarChartInteractionPreview", exampleCases);
        Assert.Contains("BuildLineChartAnatomyPreview", exampleCases);
        Assert.Contains("BuildLineChartInteractionPreview", exampleCases);
        Assert.Contains("BuildPinnedTableInteractionPreview", exampleCases);
        Assert.Contains("BuildPaginationInteractionPreview", exampleCases);
        Assert.Contains("BuildScrollAreaInteractionPreview", exampleCases);
        Assert.Contains("BuildRankedBarChartInteractionPreview", exampleCases);
        Assert.Contains("BuildUsagePieChartInteractionPreview", exampleCases);
        Assert.Contains("BuildUsageTrendChartInteractionPreview", exampleCases);
        Assert.Contains("BuildTypographyInteractionPreview", exampleCases);
        Assert.Contains("BuildFocusRingInteractionPreview", exampleCases);
        Assert.Contains("BuildDirectionInteractionPreview", exampleCases);
        Assert.Contains("BuildOverlayPrimitiveInteractionPreview", exampleCases);
        Assert.Contains("BuildMotionInteractionPreview", exampleCases);
        Assert.True(stateSamples.Length >= 53, $"Expected state samples for every component page, found {stateSamples.Length}.");
        Assert.Contains("\"forms.button\"", exampleCases);
        Assert.Contains("\"forms.button-group\"", exampleCases);
        Assert.Contains("\"forms.input-group\"", exampleCases);
        Assert.Contains("\"forms.input-otp\"", exampleCases);
        Assert.Contains("\"forms.label\"", exampleCases);
        Assert.Contains("\"forms.combobox\"", exampleCases);
        Assert.Contains("\"forms.native-select\"", exampleCases);
        Assert.Contains("\"forms.calendar\"", exampleCases);
        Assert.Contains("\"forms.date-picker\"", exampleCases);
        Assert.Contains("\"forms.icon-button\"", exampleCases);
        Assert.Contains("\"forms.split-button\"", exampleCases);
        Assert.Contains("\"forms.checkbox\"", exampleCases);
        Assert.Contains("\"forms.radio-group\"", exampleCases);
        Assert.Contains("\"forms.toggle\"", exampleCases);
        Assert.Contains("\"forms.toggle-group\"", exampleCases);
        Assert.Contains("\"layout.application-shell\"", exampleCases);
        Assert.Contains("\"layout.sidebar\"", exampleCases);
        Assert.Contains("\"layout.sidebar-primitives\"", exampleCases);
        Assert.Contains("\"layout.resizable\"", exampleCases);
        Assert.Contains("\"navigation.side-nav\"", exampleCases);
        Assert.Contains("\"navigation.breadcrumb\"", exampleCases);
        Assert.Contains("\"navigation.segmented-control\"", exampleCases);
        Assert.Contains("\"navigation.menubar\"", exampleCases);
        Assert.Contains("\"navigation.accordion\"", exampleCases);
        Assert.Contains("\"feedback.badge\"", exampleCases);
        Assert.Contains("\"navigation.menu\"", exampleCases);
        Assert.Contains("\"overlay.popover\"", exampleCases);
        Assert.Contains("\"overlay.dialog\"", exampleCases);
        Assert.Contains("\"overlay.alert-dialog\"", exampleCases);
        Assert.Contains("\"overlay.sheet\"", exampleCases);
        Assert.Contains("\"overlay.drawer\"", exampleCases);
        Assert.Contains("\"data.metric\"", exampleCases);
        Assert.Contains("\"data.item\"", exampleCases);
        Assert.Contains("\"data.aspect-ratio\"", exampleCases);
        Assert.Contains("\"data.carousel\"", exampleCases);
        Assert.Contains("\"data.chart\"", exampleCases);
        Assert.Contains("\"data.bar-chart\"", exampleCases);
        Assert.Contains("\"data.line-chart\"", exampleCases);
        Assert.Contains("\"data.image-icon\"", exampleCases);
        Assert.Contains("\"data.provider-card\"", exampleCases);
        Assert.Contains("\"data.data-table\"", exampleCases);
        Assert.Contains("\"data.pinned-table\"", exampleCases);
        Assert.Contains("\"data.pagination\"", exampleCases);
        Assert.Contains("\"data.usage-pie-chart\"", exampleCases);
        Assert.Contains("\"data.usage-trend-chart\"", exampleCases);
        Assert.Contains("\"primitives.typography\"", exampleCases);
        Assert.Contains("\"primitives.focus-ring\"", exampleCases);
        Assert.Contains("\"primitives.direction\"", exampleCases);
        Assert.Contains("\"primitives.overlay\"", exampleCases);
        Assert.Contains("\"tokens.motion\"", exampleCases);
        Assert.Contains("return examples;", exampleCases);

        foreach (var sample in stateSamples)
        {
            Assert.Contains(sample, allSamples);

            var path = Path.Combine(DocsRoot(), "Examples", "Axaml", sample.Replace('/', Path.DirectorySeparatorChar));
            Assert.True(File.Exists(path), $"State docs sample does not exist: {sample}");
            Assert.Contains("<", File.ReadAllText(path));
        }

        foreach (var sample in anatomySamples)
        {
            Assert.Contains(sample, allSamples);

            var path = Path.Combine(DocsRoot(), "Examples", "Axaml", sample.Replace('/', Path.DirectorySeparatorChar));
            Assert.True(File.Exists(path), $"Anatomy docs sample does not exist: {sample}");
            Assert.Contains("<", File.ReadAllText(path));
        }

        foreach (var sample in interactionSamples)
        {
            Assert.Contains(sample, allSamples);

            var path = Path.Combine(DocsRoot(), "Examples", "Axaml", sample.Replace('/', Path.DirectorySeparatorChar));
            Assert.True(File.Exists(path), $"Interaction docs sample does not exist: {sample}");
            Assert.Contains("<", File.ReadAllText(path));
        }
    }

    [Fact]
    public void DocsPageModelCarriesNavigationPreviewAndSectionContracts()
    {
        var model = ReadDocsSource(Path.Combine("Docs", "DocsPage.cs"));
        var mainWindow = ReadDocsSource("MainWindow.cs");

        Assert.Contains("internal sealed record DocsCategory(string Title, IReadOnlyList<DocsPage> Pages);", model);
        Assert.Contains("string Id", model);
        Assert.Contains("string Category", model);
        Assert.Contains("string SamplePath", model);
        Assert.Contains("Func<Control> BuildPreview", model);
        Assert.Contains("internal sealed record DocsExampleCase(", model);
        Assert.Contains("string Title", model);
        Assert.Contains("string Description", model);
        Assert.Contains("IReadOnlyList<DocsExampleCase> Examples", model);
        Assert.Contains("IReadOnlyList<string> Sections", model);
        Assert.Contains("IReadOnlyList<string> BehaviorNotes", model);
        Assert.Contains("internal sealed record DocsStateCase(string State, string Surface, string Contract);", model);
        Assert.Contains("internal sealed record DocsEventCase(string Input, string Expected);", model);
        Assert.Contains("IReadOnlyList<DocsStateCase> StateCases", model);
        Assert.Contains("IReadOnlyList<DocsEventCase> EventCases", model);
        Assert.Contains("StateCasesFor(id)", mainWindow);
        Assert.Contains("EventCasesFor(id)", mainWindow);
        Assert.Contains("ExampleCasesFor(id, samplePath, preview)", mainWindow);
        Assert.Contains("BuildBehaviorNotes(page)", mainWindow);
        Assert.Contains("BuildStateMatrix(page)", mainWindow);
        Assert.Contains("BuildEventMatrix(page)", mainWindow);
        Assert.Contains("Escape closes the popup", mainWindow);
        Assert.Contains("Home and End move to the first and last page.", mainWindow);
    }

    [Fact]
    public void DocsPagesRenderStateAndEventMatricesForWebParityContracts()
    {
        var source = ReadDocsSource("MainWindow.cs");
        var stateCases = ExtractMethod(source, "StateCasesFor");
        var eventCases = ExtractMethod(source, "EventCasesFor");
        var buildStateMatrix = ExtractMethod(source, "BuildStateMatrix");
        var buildEventMatrix = ExtractMethod(source, "BuildEventMatrix");

        Assert.Contains("\"forms.button\"", stateCases);
        Assert.Contains("\"layout.application-shell\"", stateCases);
        Assert.Contains("\"layout.sidebar\"", stateCases);
        Assert.Contains("\"layout.sidebar-primitives\"", stateCases);
        Assert.Contains("\"layout.resizable\"", stateCases);
        Assert.Contains("State(\"With handle\", \"Handle\"", stateCases);
        Assert.Contains("State(\"Loading\", \"In-flight action\"", stateCases);
        Assert.Contains("State(\"Focus-visible\", \"Keyboard focus\"", stateCases);
        Assert.Contains("\"forms.input-otp\"", stateCases);
        Assert.Contains("State(\"Pattern\", \"Root\"", stateCases);
        Assert.Contains("\"forms.native-select\"", stateCases);
        Assert.Contains("\"forms.combobox\"", stateCases);
        Assert.Contains("State(\"Highlighted\", \"Item\"", stateCases);
        Assert.Contains("State(\"OptGroup\", \"List\"", stateCases);
        Assert.Contains("\"forms.calendar\"", stateCases);
        Assert.Contains("State(\"Range\", \"Day grid\"", stateCases);
        Assert.Contains("\"forms.date-picker\"", stateCases);
        Assert.Contains("State(\"Open\", \"Popover\"", stateCases);
        Assert.Contains("\"forms.radio-group\"", stateCases);
        Assert.Contains("State(\"Value\", \"Root\"", stateCases);
        Assert.Contains("\"forms.icon-button\"", stateCases);
        Assert.Contains("State(\"FieldGroup\", \"Group\"", stateCases);
        Assert.Contains("State(\"FieldError\", \"Validation\"", stateCases);
        Assert.Contains("\"navigation.tabs\"", stateCases);
        Assert.Contains("State(\"Selected\", \"Tab item\"", stateCases);
        Assert.Contains("\"navigation.accordion\"", stateCases);
        Assert.Contains("State(\"Multiple\", \"Root\"", stateCases);
        Assert.Contains("\"navigation.menubar\"", stateCases);
        Assert.Contains("State(\"Checkbox\", \"Item\"", stateCases);
        Assert.Contains("\"navigation.command\"", stateCases);
        Assert.Contains("State(\"Search\", \"Root\"", stateCases);
        Assert.Contains("State(\"Separator\", \"CommandSeparator\"", stateCases);
        Assert.Contains("\"navigation.segmented-control\"", stateCases);
        Assert.Contains("\"overlay.dialog\"", stateCases);
        Assert.Contains("State(\"Restore focus\", \"Trigger\"", stateCases);
        Assert.Contains("\"overlay.alert-dialog\"", stateCases);
        Assert.Contains("State(\"Cancel focus\", \"Least destructive action\"", stateCases);
        Assert.Contains("\"overlay.drawer\"", stateCases);
        Assert.Contains("State(\"Drag ready\", \"Gesture\"", stateCases);
        Assert.Contains("\"data.pagination\"", stateCases);
        Assert.Contains("State(\"Ellipsis\", \"Page item\"", stateCases);
        Assert.Contains("\"data.carousel\"", stateCases);
        Assert.Contains("\"data.item\"", stateCases);
        Assert.Contains("\"data.aspect-ratio\"", stateCases);
        Assert.Contains("State(\"Actions\", \"Trailing slot\"", stateCases);
        Assert.Contains("State(\"Fit mode\", \"Measure\"", stateCases);
        Assert.Contains("State(\"Loop\", \"Root\"", stateCases);
        Assert.Contains("\"data.bar-chart\"", stateCases);
        Assert.Contains("State(\"Active bar\", \"Tooltip\"", stateCases);
        Assert.Contains("\"data.line-chart\"", stateCases);
        Assert.Contains("State(\"Active point\", \"Tooltip\"", stateCases);
        Assert.Contains("\"data.pinned-table\"", stateCases);
        Assert.Contains("State(\"Header sync\", \"Middle header\"", stateCases);
        Assert.Contains("\"data.usage-trend-chart\"", stateCases);
        Assert.Contains("\"data.usage-pie-chart\"", stateCases);
        Assert.Contains("State(\"Active slice\", \"Tooltip\"", stateCases);
        Assert.Contains("\"data.ranked-bar-chart\"", stateCases);
        Assert.Contains("State(\"Active row\", \"Tooltip\"", stateCases);
        Assert.Contains("\"data.image-icon\"", stateCases);
        Assert.Contains("State(\"Refreshing\", \"Overlay\"", stateCases);
        Assert.Contains("\"primitives.direction\"", stateCases);
        Assert.Contains("State(\"RTL\", \"Provider\"", stateCases);
        Assert.Contains("\"primitives.overlay\"", stateCases);
        Assert.Contains("\"tokens.motion\"", stateCases);
        Assert.Contains("State(\"Reduced motion\", \"Runtime option\"", stateCases);

        Assert.Contains("\"forms.button\"", eventCases);
        Assert.Contains("\"layout.application-shell\"", eventCases);
        Assert.Contains("\"layout.sidebar\"", eventCases);
        Assert.Contains("\"layout.sidebar-primitives\"", eventCases);
        Assert.Contains("\"layout.resizable\"", eventCases);
        Assert.Contains("Event(\"Pointer drag\"", eventCases);
        Assert.Contains("Event(\"Pointer released\"", eventCases);
        Assert.Contains("Event(\"Space / Enter\"", eventCases);
        Assert.Contains("\"forms.input-otp\"", eventCases);
        Assert.Contains("Event(\"Paste text\"", eventCases);
        Assert.Contains("\"forms.native-select\"", eventCases);
        Assert.Contains("\"forms.combobox\"", eventCases);
        Assert.Contains("Event(\"Arrow / Home / End\"", eventCases);
        Assert.Contains("Event(\"Option selection\"", eventCases);
        Assert.Contains("\"forms.calendar\"", eventCases);
        Assert.Contains("Event(\"PageUp / PageDown\"", eventCases);
        Assert.Contains("\"forms.date-picker\"", eventCases);
        Assert.Contains("Event(\"Backspace / Delete\"", eventCases);
        Assert.Contains("\"forms.radio-group\"", eventCases);
        Assert.Contains("\"navigation.accordion\"", eventCases);
        Assert.Contains("Event(\"ValueChanged\"", eventCases);
        Assert.Contains("\"forms.icon-button\"", eventCases);
        Assert.Contains("Event(\"Errors changed\"", eventCases);
        Assert.Contains("\"navigation.dropdown\"", eventCases);
        Assert.Contains("Event(\"Escape\"", eventCases);
        Assert.Contains("\"overlay.alert-dialog\"", eventCases);
        Assert.Contains("Event(\"Cancel / action\"", eventCases);
        Assert.Contains("\"overlay.drawer\"", eventCases);
        Assert.Contains("Event(\"Handle drag\"", eventCases);
        Assert.Contains("Event(\"Drag release\"", eventCases);
        Assert.Contains("\"navigation.accordion\"", eventCases);
        Assert.Contains("Event(\"Programmatic state\"", eventCases);
        Assert.Contains("\"navigation.menubar\"", eventCases);
        Assert.Contains("Event(\"Enter / Space / Down\"", eventCases);
        Assert.Contains("\"navigation.command\"", eventCases);
        Assert.Contains("Event(\"Search text\"", eventCases);
        Assert.Contains("Event(\"Pointer enter\"", eventCases);
        Assert.Contains("\"navigation.segmented-control\"", eventCases);
        Assert.Contains("\"data.pagination\"", eventCases);
        Assert.Contains("Event(\"Right / PageDown\"", eventCases);
        Assert.Contains("\"data.carousel\"", eventCases);
        Assert.Contains("\"data.item\"", eventCases);
        Assert.Contains("\"data.aspect-ratio\"", eventCases);
        Assert.Contains("Event(\"Enter / Space\"", eventCases);
        Assert.Contains("Event(\"Ratio changed\"", eventCases);
        Assert.Contains("Event(\"Loop edge\"", eventCases);
        Assert.Contains("\"data.bar-chart\"", eventCases);
        Assert.Contains("Event(\"ActiveItemChanged\"", eventCases);
        Assert.Contains("\"data.line-chart\"", eventCases);
        Assert.Contains("Event(\"ActivePointChanged\"", eventCases);
        Assert.Contains("\"data.usage-pie-chart\"", eventCases);
        Assert.Contains("Event(\"ActiveItemChanged\"", eventCases);
        Assert.Contains("\"data.ranked-bar-chart\"", eventCases);
        Assert.Contains("\"data.pinned-table\"", eventCases);
        Assert.Contains("Event(\"Body scroll\"", eventCases);
        Assert.Contains("\"data.usage-trend-chart\"", eventCases);
        Assert.Contains("\"data.image-icon\"", eventCases);
        Assert.Contains("Event(\"Refresh toggled\"", eventCases);
        Assert.Contains("\"primitives.direction\"", eventCases);
        Assert.Contains("Event(\"Direction changed\"", eventCases);
        Assert.Contains("\"primitives.overlay\"", eventCases);

        Assert.Contains("SectionHeader(\"State matrix\"", buildStateMatrix);
        Assert.Contains("page.StateCases.Count", buildStateMatrix);
        Assert.Contains("AddMatrixHeader(grid, \"State\", 0);", buildStateMatrix);
        Assert.Contains("AddMatrixCell(grid, state.Contract", buildStateMatrix);
        Assert.Contains("SectionHeader(\"Event matrix\"", buildEventMatrix);
        Assert.Contains("page.EventCases.Count", buildEventMatrix);
        Assert.Contains("AddMatrixHeader(grid, \"Input\", 0);", buildEventMatrix);
        Assert.Contains("AddMatrixCell(grid, item.Expected", buildEventMatrix);
    }

    [Fact]
    public void DocsShellUsesThemeResourcesForDarkModeSurfaces()
    {
        var source = ReadDocsSource("MainWindow.cs");
        var applyTheme = ExtractMethod(source, "ApplyTheme");
        var refreshTheme = ExtractMethod(source, "RefreshThemeSurfaces");

        Assert.Contains("ThemeBrush(CodexSwitchResourceKeys.BackgroundBrush)", source);
        Assert.Contains("CodexSwitchResourceKeys.CardBrush", source);
        Assert.Contains("CodexSwitchResourceKeys.MutedBrush", source);
        Assert.Contains("CodexSwitchResourceKeys.BorderBrush", source);
        Assert.Contains("RefreshThemeSurfaces();", applyTheme);
        Assert.DoesNotContain("Content = BuildDocsShell();", applyTheme);
        Assert.Contains("Background = ThemeBrush(CodexSwitchResourceKeys.BackgroundBrush);", refreshTheme);
        Assert.Contains("ApplyThemeBorder(border, backgroundKey, borderKey);", refreshTheme);
        Assert.DoesNotContain("#F8FAFC", source);
        Assert.DoesNotContain("#FFFFFF", source);
        Assert.DoesNotContain("#E2E8F0", source);
        Assert.DoesNotContain("#F1F5F9", source);
    }

    private static string ReadDocsSource(string relativePath)
    {
        return File.ReadAllText(Path.Combine(DocsRoot(), relativePath));
    }

    private static string DocsRoot()
    {
        return Path.Combine(FindRepositoryRoot(), "src", "CodexSwitchUI.Docs");
    }

    private static string ExtractMethod(string source, string methodName)
    {
        var signatures = new[]
        {
            $"private Control {methodName}(",
            $"private static Control {methodName}(",
            $"private IReadOnlyList<DocsStateCase> {methodName}(",
            $"private static IReadOnlyList<DocsStateCase> {methodName}(",
            $"private IReadOnlyList<DocsEventCase> {methodName}(",
            $"private static IReadOnlyList<DocsEventCase> {methodName}(",
            $"private IReadOnlyList<DocsExampleCase> {methodName}(",
            $"private static IReadOnlyList<DocsExampleCase> {methodName}(",
            $"private void {methodName}(",
            $"private static void {methodName}(",
            $"private async Task {methodName}("
        };

        var start = signatures
            .Select(signature => source.IndexOf(signature, StringComparison.Ordinal))
            .Where(index => index >= 0)
            .DefaultIfEmpty(-1)
            .Min();

        Assert.True(start >= 0, $"Could not find {methodName}.");

        var braceStart = source.IndexOf('{', start);
        Assert.True(braceStart >= 0, $"Could not find opening brace for {methodName}.");

        var depth = 0;
        for (var i = braceStart; i < source.Length; i++)
        {
            if (source[i] == '{')
            {
                depth++;
            }
            else if (source[i] == '}')
            {
                depth--;
                if (depth == 0)
                {
                    return source[start..(i + 1)];
                }
            }
        }

        throw new InvalidOperationException($"Could not find closing brace for {methodName}.");
    }

    private static IReadOnlyList<string> ExtractSamplePaths(string source)
    {
        var paths = new List<string>();
        const string marker = "Page(\"";
        var index = 0;
        while ((index = source.IndexOf(marker, index, StringComparison.Ordinal)) >= 0)
        {
            var sampleMarker = ".axaml\"";
            var sampleEnd = source.IndexOf(sampleMarker, index, StringComparison.Ordinal);
            Assert.True(sampleEnd >= 0, "Could not find page sample path.");

            var sampleStart = source.LastIndexOf('"', sampleEnd - 1);
            Assert.True(sampleStart >= 0, "Could not find page sample path opening quote.");
            paths.Add(source[(sampleStart + 1)..(sampleEnd + ".axaml".Length)]);
            index = sampleEnd + sampleMarker.Length;
        }

        return paths;
    }

    private static IReadOnlyList<string> ExtractAllAxamlSamplePaths(string source)
    {
        var paths = new List<string>();
        const string sampleMarker = ".axaml\"";
        var index = 0;
        while ((index = source.IndexOf(sampleMarker, index, StringComparison.Ordinal)) >= 0)
        {
            var sampleStart = source.LastIndexOf('"', index - 1);
            Assert.True(sampleStart >= 0, "Could not find AXAML sample path opening quote.");

            var path = source[(sampleStart + 1)..(index + ".axaml".Length)];
            if (path.Contains('/', StringComparison.Ordinal))
            {
                paths.Add(path);
            }

            index += sampleMarker.Length;
        }

        return paths.Distinct(StringComparer.Ordinal).ToArray();
    }

    private static string FindRepositoryRoot()
    {
        return TestRepository.FindRoot();
    }
}
