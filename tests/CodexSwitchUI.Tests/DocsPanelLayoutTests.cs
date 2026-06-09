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
    public void DocsManualPointerReleasedExamplesUsePrimaryReleaseOnly()
    {
        var source = ReadDocsSource("MainWindow.cs");

        Assert.DoesNotContain("PointerReleased += (_, _)", source);
        Assert.Contains("PointerReleased += (_, args) => SelectRowFromPointer(args", source);
        Assert.Contains("args.GetCurrentPoint(card).Properties.PointerUpdateKind != PointerUpdateKind.LeftButtonReleased", source);
        Assert.Contains("args.GetCurrentPoint(row).Properties.PointerUpdateKind != PointerUpdateKind.LeftButtonReleased", source);
        Assert.Contains("args.Handled = true;", source);
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
    public void InlineCodeLoadsAxamlAndCompanionSamplesIntoCopyableCodeBlocks()
    {
        var mainWindow = ReadDocsSource("MainWindow.cs");
        var samples = ReadDocsSource(Path.Combine("Docs", "DocsCodeSamples.cs"));
        var project = ReadDocsSource("CodexSwitchUI.Docs.csproj");
        var inlineExample = ExtractMethod(mainWindow, "BuildInlineExample");

        Assert.Contains("foreach (var codeSample in example.CodeSamples)", inlineExample);
        Assert.Contains("new DocsCodeBlock", inlineExample);
        Assert.Contains("Title = codeSample.Title", inlineExample);
        Assert.Contains("Code = DocsCodeSamples.Load(codeSample.SamplePath)", inlineExample);
        Assert.Contains("Code(\"Feedback/AlertInteraction.cs\", \"CSharp/Feedback/AlertInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"Feedback/BadgeInteraction.cs\", \"CSharp/Feedback/BadgeInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"Feedback/AvatarInteraction.cs\", \"CSharp/Feedback/AvatarInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"Feedback/AvatarGroupInteraction.cs\", \"CSharp/Feedback/AvatarGroupInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"Feedback/EmptyStateInteraction.cs\", \"CSharp/Feedback/EmptyStateInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"Feedback/ToastInteraction.cs\", \"CSharp/Feedback/ToastInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"Feedback/SonnerInteraction.cs\", \"CSharp/Feedback/SonnerInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"Feedback/SpinnerInteraction.cs\", \"CSharp/Feedback/SpinnerInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"Feedback/ProgressInteraction.cs\", \"CSharp/Feedback/ProgressInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"Feedback/SkeletonInteraction.cs\", \"CSharp/Feedback/SkeletonInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"Overlay/DialogInteraction.cs\", \"CSharp/Overlay/DialogInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"Overlay/AlertDialogInteraction.cs\", \"CSharp/Overlay/AlertDialogInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"Overlay/SheetInteraction.cs\", \"CSharp/Overlay/SheetInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"Overlay/DrawerInteraction.cs\", \"CSharp/Overlay/DrawerInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"Overlay/CommandDialogInteraction.cs\", \"CSharp/Overlay/CommandDialogInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"Overlay/PopoverInteraction.cs\", \"CSharp/Overlay/PopoverInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"Overlay/TooltipInteraction.cs\", \"CSharp/Overlay/TooltipInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"Overlay/HoverCardInteraction.cs\", \"CSharp/Overlay/HoverCardInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"Navigation/NavigationMenuInteraction.cs\", \"CSharp/Navigation/NavigationMenuInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"Navigation/MenubarInteraction.cs\", \"CSharp/Navigation/MenubarInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"Navigation/CommandInteraction.cs\", \"CSharp/Navigation/CommandInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"Navigation/TabsInteraction.cs\", \"CSharp/Navigation/TabsInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"Navigation/BreadcrumbInteraction.cs\", \"CSharp/Navigation/BreadcrumbInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"Navigation/SideNavInteraction.cs\", \"CSharp/Navigation/SideNavInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"Navigation/DropdownButtonInteraction.cs\", \"CSharp/Navigation/DropdownButtonInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"Navigation/MenuInteraction.cs\", \"CSharp/Navigation/MenuInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"Navigation/ContextMenuInteraction.cs\", \"CSharp/Navigation/ContextMenuInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"Navigation/SegmentedControlInteraction.cs\", \"CSharp/Navigation/SegmentedControlInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"Navigation/AccordionInteraction.cs\", \"CSharp/Navigation/AccordionInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"Navigation/CollapsibleInteraction.cs\", \"CSharp/Navigation/CollapsibleInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"Navigation/SeparatorInteraction.cs\", \"CSharp/Navigation/SeparatorInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"Navigation/KbdInteraction.cs\", \"CSharp/Navigation/KbdInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"Primitives/TypographyInteraction.cs\", \"CSharp/Primitives/TypographyInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"Primitives/FocusRingInteraction.cs\", \"CSharp/Primitives/FocusRingInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"Primitives/DirectionInteraction.cs\", \"CSharp/Primitives/DirectionInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"Primitives/OverlayInteraction.cs\", \"CSharp/Primitives/OverlayInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"Tokens/MotionInteraction.cs\", \"CSharp/Tokens/MotionInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"Forms/SelectInteraction.cs\", \"CSharp/Forms/SelectInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"Forms/ComboboxInteraction.cs\", \"CSharp/Forms/ComboboxInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"Forms/NativeSelectInteraction.cs\", \"CSharp/Forms/NativeSelectInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"Forms/CalendarInteraction.cs\", \"CSharp/Forms/CalendarInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"Forms/DatePickerInteraction.cs\", \"CSharp/Forms/DatePickerInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"Forms/ButtonInteraction.cs\", \"CSharp/Forms/ButtonInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"Forms/ButtonGroupInteraction.cs\", \"CSharp/Forms/ButtonGroupInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"Forms/InputGroupInteraction.cs\", \"CSharp/Forms/InputGroupInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"Forms/InputOtpInteraction.cs\", \"CSharp/Forms/InputOtpInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"Forms/LabelInteraction.cs\", \"CSharp/Forms/LabelInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"Forms/IconButtonInteraction.cs\", \"CSharp/Forms/IconButtonInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"Forms/SplitButtonInteraction.cs\", \"CSharp/Forms/SplitButtonInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"Forms/FieldInteraction.cs\", \"CSharp/Forms/FieldInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"Forms/TextBoxInteraction.cs\", \"CSharp/Forms/TextBoxInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"Forms/TextareaInteraction.cs\", \"CSharp/Forms/TextareaInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"Forms/CheckboxInteraction.cs\", \"CSharp/Forms/CheckboxInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"Forms/RadioInteraction.cs\", \"CSharp/Forms/RadioInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"Forms/RadioGroupInteraction.cs\", \"CSharp/Forms/RadioGroupInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"Forms/SwitchInteraction.cs\", \"CSharp/Forms/SwitchInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"Forms/ToggleInteraction.cs\", \"CSharp/Forms/ToggleInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"Forms/ToggleGroupInteraction.cs\", \"CSharp/Forms/ToggleGroupInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"Forms/SliderInteraction.cs\", \"CSharp/Forms/SliderInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"Layout/ApplicationShellInteraction.cs\", \"CSharp/Layout/ApplicationShellInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"Layout/SidebarInteraction.cs\", \"CSharp/Layout/SidebarInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"Layout/SidebarPrimitivesInteraction.cs\", \"CSharp/Layout/SidebarPrimitivesInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"Layout/SectionInteraction.cs\", \"CSharp/Layout/SectionInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"Layout/ResizableInteraction.cs\", \"CSharp/Layout/ResizableInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"DataDisplay/CardInteraction.cs\", \"CSharp/DataDisplay/CardInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"DataDisplay/ItemInteraction.cs\", \"CSharp/DataDisplay/ItemInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"DataDisplay/AspectRatioInteraction.cs\", \"CSharp/DataDisplay/AspectRatioInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"DataDisplay/CarouselInteraction.cs\", \"CSharp/DataDisplay/CarouselInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"DataDisplay/ChartInteraction.cs\", \"CSharp/DataDisplay/ChartInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"DataDisplay/BarChartInteraction.cs\", \"CSharp/DataDisplay/BarChartInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"DataDisplay/LineChartInteraction.cs\", \"CSharp/DataDisplay/LineChartInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"DataDisplay/MetricInteraction.cs\", \"CSharp/DataDisplay/MetricInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"DataDisplay/ImageIconInteraction.cs\", \"CSharp/DataDisplay/ImageIconInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"DataDisplay/ProviderCardInteraction.cs\", \"CSharp/DataDisplay/ProviderCardInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"DataDisplay/PaginationInteraction.cs\", \"CSharp/DataDisplay/PaginationInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"DataDisplay/TableInteraction.cs\", \"CSharp/DataDisplay/TableInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"DataDisplay/DataTableInteraction.cs\", \"CSharp/DataDisplay/DataTableInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"DataDisplay/PinnedTableInteraction.cs\", \"CSharp/DataDisplay/PinnedTableInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"DataDisplay/ScrollAreaInteraction.cs\", \"CSharp/DataDisplay/ScrollAreaInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"DataDisplay/RankedBarChartInteraction.cs\", \"CSharp/DataDisplay/RankedBarChartInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"DataDisplay/UsagePieChartInteraction.cs\", \"CSharp/DataDisplay/UsagePieChartInteraction.cs\")", mainWindow);
        Assert.Contains("Code(\"DataDisplay/UsageTrendChartInteraction.cs\", \"CSharp/DataDisplay/UsageTrendChartInteraction.cs\")", mainWindow);
        Assert.Contains("Path.Combine(root, \"Examples\", relativePath.Replace('/', Path.DirectorySeparatorChar))", samples);
        Assert.Contains("Path.Combine(root, \"Examples\", \"Axaml\", relativePath.Replace('/', Path.DirectorySeparatorChar))", samples);
        Assert.Contains("AppContext.BaseDirectory", samples);
        Assert.Contains("CodexSwitchUI.Docs.csproj", samples);
        Assert.Contains("nestedDocsProject", samples);
        Assert.Contains("Path.GetDirectoryName(nestedDocsProject)!", samples);
        Assert.Contains("Missing code sample", samples);
        Assert.Contains("<Compile Remove=\"Examples\\CSharp\\**\\*.cs\" />", project);
        Assert.Contains("<None Include=\"Examples\\CSharp\\**\\*.cs\">", project);
    }

    [Fact]
    public void DocsNonStateSamplesDoNotDefaultDisclosureControlsOpen()
    {
        var openAttributes = new[]
        {
            "IsOpen=\"True\"",
            "IsSubMenuOpen=\"True\"",
            "IsDropDownOpen=\"True\"",
            "IsExpanded=\"True\""
        };
        var examplesRoot = Path.Combine(DocsRoot(), "Examples", "Axaml");
        var offenders = new List<string>();

        foreach (var path in Directory.EnumerateFiles(examplesRoot, "*.axaml", SearchOption.AllDirectories))
        {
            var fileName = Path.GetFileName(path);
            if (fileName.EndsWith("Anatomy.axaml", StringComparison.Ordinal)
                || fileName.EndsWith("States.axaml", StringComparison.Ordinal))
            {
                continue;
            }

            var lines = File.ReadAllLines(path);
            for (var index = 0; index < lines.Length; index++)
            {
                if (openAttributes.Any(attribute => lines[index].Contains(attribute, StringComparison.Ordinal)))
                {
                    offenders.Add($"{Path.GetRelativePath(examplesRoot, path)}:{index + 1}");
                }
            }
        }

        Assert.Empty(offenders);
    }

    [Fact]
    public void PreviewSectionShowsInlineExpandableCodeForTheCurrentExample()
    {
        var mainWindow = ReadDocsSource("MainWindow.cs");
        var preview = ExtractMethod(mainWindow, "BuildPreviewSection");
        var inlineExample = ExtractMethod(mainWindow, "BuildInlineExample");

        Assert.Contains("foreach (var example in page.Examples)", preview);
        Assert.Contains("PreviewPanel(BuildInlineExample(example))", preview);
        Assert.Contains("new StackPanel", inlineExample);
        Assert.Contains("new DocsCodeBlock", inlineExample);
        Assert.Contains("Title = codeSample.Title", inlineExample);
        Assert.Contains("Code = DocsCodeSamples.Load(codeSample.SamplePath)", inlineExample);
        Assert.Contains("IsVisible = false", inlineExample);
        Assert.Contains("new CodexButton", inlineExample);
        Assert.Contains("Content = \"Show code\"", inlineExample);
        Assert.Contains("toggleCode.Click += (_, _) =>", inlineExample);
        Assert.Contains("codeBlocks.IsVisible = !codeBlocks.IsVisible;", inlineExample);
        Assert.Contains("toggleCode.Content = codeBlocks.IsVisible ? \"Hide code\" : \"Show code\";", inlineExample);
        Assert.Contains("SectionHeader(example.Title, example.Description)", inlineExample);
        Assert.Contains("example.BuildPreview()", inlineExample);
        Assert.Contains("codeBlocks", inlineExample);

        var stackReturn = inlineExample[inlineExample.IndexOf("return new StackPanel", StringComparison.Ordinal)..];
        var previewIndex = stackReturn.IndexOf("example.BuildPreview()", StringComparison.Ordinal);
        var toggleIndex = stackReturn.IndexOf("toggleCode", StringComparison.Ordinal);
        var codeIndex = stackReturn.IndexOf("codeBlocks", toggleIndex, StringComparison.Ordinal);

        Assert.True(previewIndex >= 0, "Inline examples must render the component preview.");
        Assert.True(toggleIndex > previewIndex, "Inline examples must render the Show code button below the component preview.");
        Assert.True(codeIndex > toggleIndex, "Inline examples must render the current code blocks below the Show code button.");
    }

    [Fact]
    public void DocsShellCodeBlocksAndMatricesUseWebSpacing()
    {
        var mainWindow = ReadDocsSource("MainWindow.cs");
        var codeBlock = ReadDocsSource(Path.Combine("Controls", "DocsCodeBlock.cs"));
        var shell = ExtractMethod(mainWindow, "BuildDocsShell");
        var topbar = ExtractMethod(mainWindow, "BuildTopbar");
        var inlineExample = ExtractMethod(mainWindow, "BuildInlineExample");
        var addMatrixCell = ExtractMethod(mainWindow, "AddMatrixCell");
        var behaviorNotes = ExtractMethod(mainWindow, "BuildBehaviorNotes");

        Assert.Contains("private const double DocsDesktopGutter = 40;", mainWindow);
        Assert.Contains("private const double DocsContentVerticalPadding = 56;", mainWindow);
        Assert.Contains("private const double DocsInlineExampleGap = 16;", mainWindow);
        Assert.Contains("private const double DocsMatrixCellHorizontalPadding = 16;", mainWindow);
        Assert.Contains("private const double DocsMatrixCellVerticalPadding = 12;", mainWindow);
        Assert.Contains("Padding = new Thickness(DocsDesktopGutter, DocsContentVerticalPadding, DocsDesktopGutter, DocsContentVerticalPadding)", shell);
        Assert.Contains("Margin = new Thickness(DocsDesktopGutter, 0)", topbar);
        Assert.Contains("Spacing = DocsInlineExampleGap", inlineExample);
        Assert.DoesNotContain("Spacing = 14", inlineExample);
        Assert.Contains("new Thickness(DocsMatrixCellHorizontalPadding, DocsMatrixCellVerticalPadding)", addMatrixCell);
        Assert.Contains("new Thickness(DocsMatrixCellHorizontalPadding, DocsMatrixCellVerticalPadding)", behaviorNotes);

        Assert.Contains("private const double CodeInset = 16;", codeBlock);
        Assert.Contains("private const double LineNumberColumnWidth = 56;", codeBlock);
        Assert.Contains("Padding = new Thickness(0, CodeInset, 12, CodeInset)", codeBlock);
        Assert.Contains("Padding = new Thickness(0, CodeInset, CodeInset, CodeInset)", codeBlock);
        Assert.Contains("Padding = new Thickness(CodeInset, 0)", codeBlock);
        Assert.Contains("new ColumnDefinition(new GridLength(LineNumberColumnWidth))", codeBlock);
    }

    [Fact]
    public void FormsFoundationInteractionExamplesExposeCompanionCSharpSource()
    {
        var mainWindow = ReadDocsSource("MainWindow.cs");

        AssertFormsCompanion(mainWindow, "ButtonInteraction.cs",
            "CodexButton",
            "Click +=",
            "IsLoading",
            "Command = new SampleCommand",
            "CanExecute");
        AssertFormsCompanion(mainWindow, "ButtonGroupInteraction.cs",
            "CodexButtonGroup",
            "CodexButtonGroupSeparator",
            "loading.IsLoading",
            "new CodexIconButton");
        AssertFormsCompanion(mainWindow, "InputGroupInteraction.cs",
            "CodexInputGroup",
            "CodexInputGroupButton",
            "SelectionStart",
            "IsReadOnly = true",
            "IsEnabled = false");
        AssertFormsCompanion(mainWindow, "InputOtpInteraction.cs",
            "CodexInputOtp",
            "TryInsertText",
            "FocusSlot(2)",
            "Clear()",
            "CodexInputOtpSeparator");
        AssertFormsCompanion(mainWindow, "LabelInteraction.cs",
            "CodexLabel",
            "Target = terms",
            "Content = \"_Provider\"",
            "IsRequired = true",
            "Intent = CodexControlIntent.Error");
        AssertFormsCompanion(mainWindow, "IconButtonInteraction.cs",
            "CodexIconButton",
            "IsRound = true",
            "refresh.IsLoading",
            "Variant = CodexControlVariant.Destructive");
        AssertFormsCompanion(mainWindow, "SplitButtonInteraction.cs",
            "CodexSplitButton",
            "OpenChanged",
            "RestoreFocusRequested",
            "split.Open()",
            "split.Dismiss()",
            "CloseOnItemSelected = false");
        AssertFormsCompanion(mainWindow, "FieldInteraction.cs",
            "CodexField",
            "providerField.Intent",
            "providerField.Message",
            "providerName.IsEnabled",
            "CodexSelect");
        AssertFormsCompanion(mainWindow, "TextBoxInteraction.cs",
            "CodexTextBox",
            "TextChanged",
            "InnerLeftContent",
            "SelectionStart",
            "IsReadOnly = true");
        AssertFormsCompanion(mainWindow, "TextareaInteraction.cs",
            "CodexTextarea",
            "TextChanged",
            "MinLines = 5",
            "IsReadOnly = true",
            "IsEnabled = false");
    }

    [Fact]
    public void FeedbackLoadingInteractionExamplesExposeCompanionCSharpSource()
    {
        var mainWindow = ReadDocsSource("MainWindow.cs");

        AssertFeedbackCompanion(mainWindow, "EmptyStateInteraction.cs",
            "CodexEmptyState",
            "ActionRequested",
            "SecondaryActionRequested",
            "TryExecuteAction()",
            "TryExecuteSecondaryAction()",
            "ActionCommand = new SampleCommand",
            "CanExecute");
        AssertFeedbackCompanion(mainWindow, "SpinnerInteraction.cs",
            "CodexSpinner",
            "IsActive",
            "RotationDuration",
            "TimeSpan.Zero",
            "new CodexButton { Content = \"Refreshing\", IsLoading = true }");
        AssertFeedbackCompanion(mainWindow, "ProgressInteraction.cs",
            "CodexProgress",
            "Value = 36",
            "IsIndeterminate = true",
            "IndeterminateAnimationDuration",
            "TimeSpan.Zero",
            "IsEnabled = false");
        AssertFeedbackCompanion(mainWindow, "SkeletonInteraction.cs",
            "CodexSkeleton",
            "IsAnimated",
            "PulseDuration",
            "PulseLowOpacity",
            "ShimmerHighOpacity",
            "CornerRadius = new CornerRadius");
    }

    [Fact]
    public void NavigationSimpleInteractionExamplesExposeCompanionCSharpSource()
    {
        var mainWindow = ReadDocsSource("MainWindow.cs");

        AssertNavigationCompanion(mainWindow, "SegmentedControlInteraction.cs",
            "CodexSegmentedControl",
            "ValueChanged",
            "args.Source",
            "SelectedValue = \"preview\"",
            "Command = new SampleCommand",
            "CanExecute");
        AssertNavigationCompanion(mainWindow, "AccordionInteraction.cs",
            "CodexAccordion",
            "CodexAccordionItem",
            "ValueChanged",
            "args.NewValues",
            "ChangedValue",
            "CodexAccordionType.Multiple",
            "AnimationDuration = TimeSpan.Zero");
        AssertNavigationCompanion(mainWindow, "CollapsibleInteraction.cs",
            "CodexCollapsible",
            "OpenChanged",
            "args.Source",
            "Toggle()",
            "AnimationDuration",
            "TimeSpan.Zero",
            "IsEnabled = false");
        AssertNavigationCompanion(mainWindow, "SeparatorInteraction.cs",
            "CodexSeparator",
            "Orientation",
            "CodexControlSize",
            "double.NaN",
            "IsEnabled = false");
        AssertNavigationCompanion(mainWindow, "KbdInteraction.cs",
            "CodexKbdGroup",
            "CodexKbd",
            "switchSequence.Click",
            "toggleDensity.Click",
            "CodexCommandItem",
            "TrailingIcon");
    }

    [Fact]
    public void PrimitivesAndTokensInteractionExamplesExposeCompanionCSharpSource()
    {
        var mainWindow = ReadDocsSource("MainWindow.cs");

        AssertPrimitivesCompanion(mainWindow, "TypographyInteraction.cs",
            "CodexTextRole",
            "sample.Role",
            "TextWrapping",
            "cycleRole.Click",
            "toggleWrap.Click",
            "CodexTextRole.Code");
        AssertPrimitivesCompanion(mainWindow, "FocusRingInteraction.cs",
            "CodexFocusRing",
            "IsRingVisible",
            "RingThickness",
            "RingOffset",
            "target.Focus()",
            "CodexSwitchResourceKeys.SuccessBrush",
            "IsEnabled = false");
        AssertPrimitivesCompanion(mainWindow, "DirectionInteraction.cs",
            "CodexDirection",
            "CodexDirectionMode.RightToLeft",
            "DirectionChanged",
            "args.FlowDirection",
            "forceRtl.Click",
            "DirectionSurface",
            "IsEnabled = false");
        AssertTokensCompanion(mainWindow, "MotionInteraction.cs",
            "CodexMotion.ApplyOpacityTransition",
            "CodexMotion.ApplyTranslateYTransition",
            "ResolveDefaultDuration",
            "ResolveEaseOut",
            "TimeSpan.Zero",
            "TranslateTransform",
            "transform.Y");
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

        var applicationShellSample = Path.Combine(root, "Examples", "CSharp", "Layout", "ApplicationShellInteraction.cs");
        Assert.True(File.Exists(applicationShellSample), "Expected ApplicationShell companion C# interaction sample.");
        var applicationShellCode = File.ReadAllText(applicationShellSample);
        Assert.Contains("CodexSidebarMenuButton", applicationShellCode);
        Assert.Contains("IsActive = true", applicationShellCode);
        Assert.Contains("providers.Badge", applicationShellCode);
        Assert.Contains("newProvider.Click", applicationShellCode);
        Assert.Contains("section.Title", applicationShellCode);
        Assert.Contains("section.Content", applicationShellCode);

        var sidebarSample = Path.Combine(root, "Examples", "CSharp", "Layout", "SidebarInteraction.cs");
        Assert.True(File.Exists(sidebarSample), "Expected Sidebar companion C# interaction sample.");
        var sidebarCode = File.ReadAllText(sidebarSample);
        Assert.Contains("CodexSidebarProvider", sidebarCode);
        Assert.Contains("OpenChanged", sidebarCode);
        Assert.Contains("TryHandleShortcut(Key.B, KeyModifiers.Control)", sidebarCode);
        Assert.Contains("CodexSidebarTrigger", sidebarCode);
        Assert.Contains("CodexSidebarRail", sidebarCode);
        Assert.Contains("Command = new SampleCommand", sidebarCode);
        Assert.Contains("CanExecute", sidebarCode);

        var sidebarPrimitivesSample = Path.Combine(root, "Examples", "CSharp", "Layout", "SidebarPrimitivesInteraction.cs");
        Assert.True(File.Exists(sidebarPrimitivesSample), "Expected Sidebar primitives companion C# interaction sample.");
        var sidebarPrimitivesCode = File.ReadAllText(sidebarPrimitivesSample);
        Assert.Contains("CodexSidebarMenuButton", sidebarPrimitivesCode);
        Assert.Contains("CodexSidebarMenuAction", sidebarPrimitivesCode);
        Assert.Contains("IsShowOnHover = true", sidebarPrimitivesCode);
        Assert.Contains("CodexSidebarGroupAction", sidebarPrimitivesCode);
        Assert.Contains("activeRoutes.Badge", sidebarPrimitivesCode);
        Assert.Contains("CodexSidebarMenuSubButton", sidebarPrimitivesCode);
        Assert.Contains("IsEnabled = false", sidebarPrimitivesCode);

        var sectionSample = Path.Combine(root, "Examples", "CSharp", "Layout", "SectionInteraction.cs");
        Assert.True(File.Exists(sectionSample), "Expected Section companion C# interaction sample.");
        var sectionCode = File.ReadAllText(sectionSample);
        Assert.Contains("CodexSection", sectionCode);
        Assert.Contains("refresh.Click", sectionCode);
        Assert.Contains("section.Title", sectionCode);
        Assert.Contains("section.Description", sectionCode);
        Assert.Contains("section.Actions", sectionCode);
        Assert.Contains("emptySection.Content", sectionCode);

        var resizableSample = Path.Combine(root, "Examples", "CSharp", "Layout", "ResizableInteraction.cs");
        Assert.True(File.Exists(resizableSample), "Expected Resizable companion C# interaction sample.");
        var resizableCode = File.ReadAllText(resizableSample);
        Assert.Contains("CodexResizablePanelGroup", resizableCode);
        Assert.Contains("CodexResizableHandle", resizableCode);
        Assert.Contains("LayoutChanged", resizableCode);
        Assert.Contains("args.PanelSizes", resizableCode);
        Assert.Contains("ResizeHandleByPercent(handle, -10)", resizableCode);
        Assert.Contains("ResizeHandleByPercent(handle, 10)", resizableCode);
        Assert.Contains("TryHandleResizeKey", resizableCode);
        Assert.Contains("Orientation = Orientation.Vertical", resizableCode);

        var selectSample = Path.Combine(root, "Examples", "CSharp", "Forms", "SelectInteraction.cs");
        Assert.True(File.Exists(selectSample), "Expected Select companion C# interaction sample.");
        var selectCode = File.ReadAllText(selectSample);
        Assert.Contains("OpenChanged", selectCode);
        Assert.Contains("ValueChanged", selectCode);
        Assert.Contains("args.Source", selectCode);
        Assert.Contains("SelectedIndex = 1", selectCode);
        Assert.Contains("IsDropDownOpen = false", selectCode);
        Assert.Contains("IsEnabled = false", selectCode);

        var comboboxSample = Path.Combine(root, "Examples", "CSharp", "Forms", "ComboboxInteraction.cs");
        Assert.True(File.Exists(comboboxSample), "Expected Combobox companion C# interaction sample.");
        var comboboxCode = File.ReadAllText(comboboxSample);
        Assert.Contains("OpenChanged", comboboxCode);
        Assert.Contains("SelectionChanged", comboboxCode);
        Assert.Contains("InputValueChanged", comboboxCode);
        Assert.Contains("TryHandleInputKey(Key.Down)", comboboxCode);
        Assert.Contains("TryHandleInputKey(Key.Enter)", comboboxCode);
        Assert.Contains("ClearSelection()", comboboxCode);
        Assert.Contains("CloseOnSelect = false", comboboxCode);
        Assert.Contains("IsLoading = true", comboboxCode);

        var nativeSelectSample = Path.Combine(root, "Examples", "CSharp", "Forms", "NativeSelectInteraction.cs");
        Assert.True(File.Exists(nativeSelectSample), "Expected NativeSelect companion C# interaction sample.");
        var nativeSelectCode = File.ReadAllText(nativeSelectSample);
        Assert.Contains("CodexNativeSelect", nativeSelectCode);
        Assert.Contains("CodexNativeSelectOption", nativeSelectCode);
        Assert.Contains("OpenChanged", nativeSelectCode);
        Assert.Contains("ValueChanged", nativeSelectCode);
        Assert.Contains("SelectedIndex = 2", nativeSelectCode);
        Assert.Contains("IsDropDownOpen = true", nativeSelectCode);
        Assert.Contains("IsDropDownOpen = false", nativeSelectCode);
        Assert.Contains("IsEnabled = false", nativeSelectCode);

        var calendarSample = Path.Combine(root, "Examples", "CSharp", "Forms", "CalendarInteraction.cs");
        Assert.True(File.Exists(calendarSample), "Expected Calendar companion C# interaction sample.");
        var calendarCode = File.ReadAllText(calendarSample);
        Assert.Contains("SelectedDateChanged", calendarCode);
        Assert.Contains("DisplayDateChanged", calendarCode);
        Assert.Contains("ActiveDateChanged", calendarCode);
        Assert.Contains("RangeChanged", calendarCode);
        Assert.Contains("SelectDate(new DateTime(2026, 5, 25))", calendarCode);
        Assert.Contains("NavigateNextMonth()", calendarCode);
        Assert.Contains("CodexCalendarDayButton", calendarCode);
        Assert.Contains("Command = new SampleCommand", calendarCode);
        Assert.Contains("CanExecute", calendarCode);

        var datePickerSample = Path.Combine(root, "Examples", "CSharp", "Forms", "DatePickerInteraction.cs");
        Assert.True(File.Exists(datePickerSample), "Expected DatePicker companion C# interaction sample.");
        var datePickerCode = File.ReadAllText(datePickerSample);
        Assert.Contains("CodexDatePicker", datePickerCode);
        Assert.Contains("OpenChanged", datePickerCode);
        Assert.Contains("SelectedDateChanged", datePickerCode);
        Assert.Contains("RangeChanged", datePickerCode);
        Assert.Contains("TryHandleInputKey(Key.Down)", datePickerCode);
        Assert.Contains("TryHandleInputKey(Key.Escape)", datePickerCode);
        Assert.Contains("ClearSelection()", datePickerCode);
        Assert.Contains("IsLoading = true", datePickerCode);
        Assert.Contains("MinDate", datePickerCode);
        Assert.Contains("MaxDate", datePickerCode);

        var checkboxSample = Path.Combine(root, "Examples", "CSharp", "Forms", "CheckboxInteraction.cs");
        Assert.True(File.Exists(checkboxSample), "Expected Checkbox companion C# interaction sample.");
        var checkboxCode = File.ReadAllText(checkboxSample);
        Assert.Contains("CodexCheckBox", checkboxCode);
        Assert.Contains("CheckedStateChanged", checkboxCode);
        Assert.Contains("args.Source", checkboxCode);
        Assert.Contains("IsThreeState = true", checkboxCode);
        Assert.Contains("IsChecked = null", checkboxCode);
        Assert.Contains("IsEnabled = false", checkboxCode);

        var radioSample = Path.Combine(root, "Examples", "CSharp", "Forms", "RadioInteraction.cs");
        Assert.True(File.Exists(radioSample), "Expected Radio companion C# interaction sample.");
        var radioCode = File.ReadAllText(radioSample);
        Assert.Contains("CodexRadio", radioCode);
        Assert.Contains("GroupName = \"provider-route\"", radioCode);
        Assert.Contains("Checked +=", radioCode);
        Assert.Contains("IsChecked = true", radioCode);
        Assert.Contains("Intent = CodexControlIntent.Warning", radioCode);
        Assert.Contains("IsEnabled = false", radioCode);

        var radioGroupSample = Path.Combine(root, "Examples", "CSharp", "Forms", "RadioGroupInteraction.cs");
        Assert.True(File.Exists(radioGroupSample), "Expected RadioGroup companion C# interaction sample.");
        var radioGroupCode = File.ReadAllText(radioGroupSample);
        Assert.Contains("CodexRadioGroup", radioGroupCode);
        Assert.Contains("CodexRadioGroupItem", radioGroupCode);
        Assert.Contains("ValueChanged", radioGroupCode);
        Assert.Contains("args.Source", radioGroupCode);
        Assert.Contains("SelectedValue = \"balanced\"", radioGroupCode);
        Assert.Contains("IsLoading = !group.IsLoading", radioGroupCode);
        Assert.Contains("IsLoop = false", radioGroupCode);
        Assert.Contains("Orientation = Orientation.Horizontal", radioGroupCode);
        Assert.Contains("IsEnabled = false", radioGroupCode);

        var switchSample = Path.Combine(root, "Examples", "CSharp", "Forms", "SwitchInteraction.cs");
        Assert.True(File.Exists(switchSample), "Expected Switch companion C# interaction sample.");
        var switchCode = File.ReadAllText(switchSample);
        Assert.Contains("CodexSwitch", switchCode);
        Assert.Contains("CheckedChanged", switchCode);
        Assert.Contains("args.Source", switchCode);
        Assert.Contains("IsChecked = true", switchCode);
        Assert.Contains("streaming.IsChecked = false", switchCode);
        Assert.Contains("Size = CodexControlSize.Large", switchCode);
        Assert.Contains("IsEnabled = false", switchCode);

        var toggleSample = Path.Combine(root, "Examples", "CSharp", "Forms", "ToggleInteraction.cs");
        Assert.True(File.Exists(toggleSample), "Expected Toggle companion C# interaction sample.");
        var toggleCode = File.ReadAllText(toggleSample);
        Assert.Contains("CodexToggle", toggleCode);
        Assert.Contains("PressedChanged", toggleCode);
        Assert.Contains("args.Source", toggleCode);
        Assert.Contains("IsPressed = true", toggleCode);
        Assert.Contains("bookmark.IsPressed = false", toggleCode);
        Assert.Contains("Variant = CodexControlVariant.Outline", toggleCode);
        Assert.Contains("IsEnabled = false", toggleCode);

        var toggleGroupSample = Path.Combine(root, "Examples", "CSharp", "Forms", "ToggleGroupInteraction.cs");
        Assert.True(File.Exists(toggleGroupSample), "Expected ToggleGroup companion C# interaction sample.");
        var toggleGroupCode = File.ReadAllText(toggleGroupSample);
        Assert.Contains("CodexToggleGroup", toggleGroupCode);
        Assert.Contains("CodexToggleGroupItem", toggleGroupCode);
        Assert.Contains("ValueChanged", toggleGroupCode);
        Assert.Contains("args.Source", toggleGroupCode);
        Assert.Contains("args.NewValues", toggleGroupCode);
        Assert.Contains("Type = CodexToggleGroupType.Multiple", toggleGroupCode);
        Assert.Contains("SelectedValues = [\"bold\", \"italic\"]", toggleGroupCode);
        Assert.Contains("IsLoop = false", toggleGroupCode);
        Assert.Contains("Orientation = Orientation.Vertical", toggleGroupCode);
        Assert.Contains("IsEnabled = false", toggleGroupCode);

        var sliderSample = Path.Combine(root, "Examples", "CSharp", "Forms", "SliderInteraction.cs");
        Assert.True(File.Exists(sliderSample), "Expected Slider companion C# interaction sample.");
        var sliderCode = File.ReadAllText(sliderSample);
        Assert.Contains("CodexSlider", sliderCode);
        Assert.Contains("ValueChanging", sliderCode);
        Assert.Contains("ValueCommitted", sliderCode);
        Assert.Contains("args.Source", sliderCode);
        Assert.Contains("slider.Value = 24", sliderCode);
        Assert.Contains("slider.Value = 76", sliderCode);
        Assert.Contains("CommitValue()", sliderCode);
        Assert.Contains("Orientation = Orientation.Vertical", sliderCode);
        Assert.Contains("IsEnabled = false", sliderCode);

        var alertSample = Path.Combine(root, "Examples", "CSharp", "Feedback", "AlertInteraction.cs");
        Assert.True(File.Exists(alertSample), "Expected Alert companion C# interaction sample.");
        var alertCode = File.ReadAllText(alertSample);
        Assert.Contains("CodexAlert", alertCode);
        Assert.Contains("Action = acknowledge", alertCode);
        Assert.Contains("HasAction", alertCode);
        Assert.Contains("HasDescription", alertCode);
        Assert.Contains("Variant = CodexControlVariant.Warning", alertCode);
        Assert.Contains("Variant = CodexControlVariant.Success", alertCode);
        Assert.Contains("Description = null", alertCode);
        Assert.Contains("IsEnabled = false", alertCode);

        var badgeSample = Path.Combine(root, "Examples", "CSharp", "Feedback", "BadgeInteraction.cs");
        Assert.True(File.Exists(badgeSample), "Expected Badge companion C# interaction sample.");
        var badgeCode = File.ReadAllText(badgeSample);
        Assert.Contains("Activated", badgeCode);
        Assert.Contains("args.Source", badgeCode);
        Assert.Contains("TryActivate()", badgeCode);
        Assert.Contains("TryHandleActivationKey(Key.Enter)", badgeCode);
        Assert.Contains("Command = new SampleCommand", badgeCode);
        Assert.Contains("CommandParameter", badgeCode);
        Assert.Contains("CanExecute", badgeCode);
        Assert.Contains("IsInteractive = true", badgeCode);
        Assert.Contains("IsEnabled = false", badgeCode);
        Assert.Contains("StatusVariant", badgeCode);
        Assert.Contains("IsStatusVisible = true", badgeCode);

        var avatarSample = Path.Combine(root, "Examples", "CSharp", "Feedback", "AvatarInteraction.cs");
        Assert.True(File.Exists(avatarSample), "Expected Avatar companion C# interaction sample.");
        var avatarCode = File.ReadAllText(avatarSample);
        Assert.Contains("LoadingStatusChanged", avatarCode);
        Assert.Contains("OldStatus", avatarCode);
        Assert.Contains("NewStatus", avatarCode);
        Assert.Contains("ErrorMessage", avatarCode);
        Assert.Contains("ImagePath = IconPath", avatarCode);
        Assert.Contains("IconPath(\"missing-avatar.png\")", avatarCode);
        Assert.Contains("FallbackDelay = TimeSpan.FromMilliseconds(600)", avatarCode);
        Assert.Contains("FallbackDelay = TimeSpan.Zero", avatarCode);
        Assert.Contains("LoadingStatus = CodexAvatarLoadingStatus.Loading", avatarCode);
        Assert.Contains("LoadingStatus = CodexAvatarLoadingStatus.Error", avatarCode);
        Assert.Contains("Size = CodexControlSize.Large", avatarCode);

        var avatarGroupSample = Path.Combine(root, "Examples", "CSharp", "Feedback", "AvatarGroupInteraction.cs");
        Assert.True(File.Exists(avatarGroupSample), "Expected AvatarGroup companion C# interaction sample.");
        var avatarGroupCode = File.ReadAllText(avatarGroupSample);
        Assert.Contains("CodexAvatarGroup", avatarGroupCode);
        Assert.Contains("CodexAvatarGroupCount", avatarGroupCode);
        Assert.Contains("group.IsStacked", avatarGroupCode);
        Assert.Contains("group.Overlap", avatarGroupCode);
        Assert.Contains("group.ItemCount", avatarGroupCode);
        Assert.Contains("optionalMember.IsVisible", avatarGroupCode);
        Assert.Contains("group.InvalidateMeasure()", avatarGroupCode);
        Assert.Contains("VisibleAvatarGroupMembers", avatarGroupCode);
        Assert.Contains("IsEnabled = false", avatarGroupCode);

        var sonnerServiceSample = Path.Combine(root, "Examples", "CSharp", "Feedback", "SonnerInteraction.cs");
        Assert.True(File.Exists(sonnerServiceSample), "Expected Sonner companion C# service sample.");
        var sonnerServiceCode = File.ReadAllText(sonnerServiceSample);
        Assert.Contains("CodexSonnerService.Clear();", sonnerServiceCode);
        Assert.Contains("CodexSonnerService.Success", sonnerServiceCode);
        Assert.Contains("CodexSonnerService.Loading", sonnerServiceCode);

        var toastSample = Path.Combine(root, "Examples", "CSharp", "Feedback", "ToastInteraction.cs");
        Assert.True(File.Exists(toastSample), "Expected Toast companion C# interaction sample.");
        var toastCode = File.ReadAllText(toastSample);
        Assert.Contains("DismissCommand.Execute", toastCode);
        Assert.Contains("CloseCommand", toastCode);
        Assert.Contains("CloseOnEscape = false", toastCode);
        Assert.Contains("manualToast.IsOpen", toastCode);

        var dialogSample = Path.Combine(root, "Examples", "CSharp", "Overlay", "DialogInteraction.cs");
        Assert.True(File.Exists(dialogSample), "Expected Dialog companion C# interaction sample.");
        var dialogCode = File.ReadAllText(dialogSample);
        Assert.Contains("OpenChanged", dialogCode);
        Assert.Contains("RestoreFocusRequested", dialogCode);
        Assert.Contains("DismissCommand.Execute", dialogCode);
        Assert.Contains("closedDialog.IsOpen", dialogCode);

        var alertDialogSample = Path.Combine(root, "Examples", "CSharp", "Overlay", "AlertDialogInteraction.cs");
        Assert.True(File.Exists(alertDialogSample), "Expected AlertDialog companion C# interaction sample.");
        var alertDialogCode = File.ReadAllText(alertDialogSample);
        Assert.Contains("CancelCommand", alertDialogCode);
        Assert.Contains("ActionCommand", alertDialogCode);
        Assert.Contains("alertDialog.Cancel()", alertDialogCode);
        Assert.Contains("alertDialog.Confirm()", alertDialogCode);
        Assert.Contains("IsActionLoading", alertDialogCode);

        var sheetSample = Path.Combine(root, "Examples", "CSharp", "Overlay", "SheetInteraction.cs");
        Assert.True(File.Exists(sheetSample), "Expected Sheet companion C# interaction sample.");
        var sheetCode = File.ReadAllText(sheetSample);
        Assert.Contains("OpenChanged", sheetCode);
        Assert.Contains("RestoreFocusRequested", sheetCode);
        Assert.Contains("DismissCommand.Execute", sheetCode);
        Assert.Contains("sheet.Side", sheetCode);
        Assert.Contains("CloseOnEscape", sheetCode);
        Assert.Contains("DismissOnOutsidePointer", sheetCode);

        var drawerSample = Path.Combine(root, "Examples", "CSharp", "Overlay", "DrawerInteraction.cs");
        Assert.True(File.Exists(drawerSample), "Expected Drawer companion C# interaction sample.");
        var drawerCode = File.ReadAllText(drawerSample);
        Assert.Contains("OpenChanged", drawerCode);
        Assert.Contains("DragCompleted", drawerCode);
        Assert.Contains("BeginDrag()", drawerCode);
        Assert.Contains("DragBy(128)", drawerCode);
        Assert.Contains("CompleteDrag()", drawerCode);
        Assert.Contains("CloseOnDragDismiss = false", drawerCode);
        Assert.Contains("drawer.Direction", drawerCode);

        var commandDialogSample = Path.Combine(root, "Examples", "CSharp", "Overlay", "CommandDialogInteraction.cs");
        Assert.True(File.Exists(commandDialogSample), "Expected CommandDialog companion C# interaction sample.");
        var commandDialogCode = File.ReadAllText(commandDialogSample);
        Assert.Contains("OpenChanged", commandDialogCode);
        Assert.Contains("ItemSelected", commandDialogCode);
        Assert.Contains("CloseOnItemSelected = true", commandDialogCode);
        Assert.Contains("CloseOnItemSelected = false", commandDialogCode);
        Assert.Contains("IsLoading = true", commandDialogCode);
        Assert.Contains("RestoreFocusElement = trigger", commandDialogCode);
        Assert.Contains("CodexCommandLoading", commandDialogCode);

        var popoverSample = Path.Combine(root, "Examples", "CSharp", "Overlay", "PopoverInteraction.cs");
        Assert.True(File.Exists(popoverSample), "Expected Popover companion C# interaction sample.");
        var popoverCode = File.ReadAllText(popoverSample);
        Assert.Contains("OpenChanged", popoverCode);
        Assert.Contains("RestoreFocusRequested", popoverCode);
        Assert.Contains("DismissCommand.Execute", popoverCode);
        Assert.Contains("Open()", popoverCode);
        Assert.Contains("CloseOnEscape = false", popoverCode);
        Assert.Contains("DismissOnOutsidePointer = false", popoverCode);

        var tooltipSample = Path.Combine(root, "Examples", "CSharp", "Overlay", "TooltipInteraction.cs");
        Assert.True(File.Exists(tooltipSample), "Expected Tooltip companion C# interaction sample.");
        var tooltipCode = File.ReadAllText(tooltipSample);
        Assert.Contains("CodexTooltipProvider", tooltipCode);
        Assert.Contains("DelayDuration", tooltipCode);
        Assert.Contains("OpenChanged", tooltipCode);
        Assert.Contains("OpenDelay = TimeSpan.Zero", tooltipCode);
        Assert.Contains("Open()", tooltipCode);
        Assert.Contains("Dismiss()", tooltipCode);
        Assert.Contains("CloseOnEscape = false", tooltipCode);

        var hoverCardSample = Path.Combine(root, "Examples", "CSharp", "Overlay", "HoverCardInteraction.cs");
        Assert.True(File.Exists(hoverCardSample), "Expected HoverCard companion C# interaction sample.");
        var hoverCardCode = File.ReadAllText(hoverCardSample);
        Assert.Contains("OpenChanged", hoverCardCode);
        Assert.Contains("OpenDelay = TimeSpan.FromMilliseconds(700)", hoverCardCode);
        Assert.Contains("CloseDelay = TimeSpan.FromMilliseconds(300)", hoverCardCode);
        Assert.Contains("OpenDelay = TimeSpan.Zero", hoverCardCode);
        Assert.Contains("Align = CodexHoverCardAlign.Start", hoverCardCode);
        Assert.Contains("IsArrowVisible = false", hoverCardCode);
        Assert.Contains("IsEnabled = false", hoverCardCode);

        var navigationMenuSample = Path.Combine(root, "Examples", "CSharp", "Navigation", "NavigationMenuInteraction.cs");
        Assert.True(File.Exists(navigationMenuSample), "Expected NavigationMenu companion C# interaction sample.");
        var navigationMenuCode = File.ReadAllText(navigationMenuSample);
        Assert.Contains("ActiveItemChanged", navigationMenuCode);
        Assert.Contains("Activated", navigationMenuCode);
        Assert.Contains("ActivateItem", navigationMenuCode);
        Assert.Contains("CloseViewport", navigationMenuCode);
        Assert.Contains("Orientation = Orientation.Vertical", navigationMenuCode);
        Assert.Contains("openVertical.Click", navigationMenuCode);
        Assert.Contains("IsEnabled = false", navigationMenuCode);

        var menubarSample = Path.Combine(root, "Examples", "CSharp", "Navigation", "MenubarInteraction.cs");
        Assert.True(File.Exists(menubarSample), "Expected Menubar companion C# interaction sample.");
        var menubarCode = File.ReadAllText(menubarSample);
        Assert.Contains("ItemSelected", menubarCode);
        Assert.Contains("DidCloseOnSelect", menubarCode);
        Assert.Contains("ActiveMenuChanged", menubarCode);
        Assert.Contains("OpenMenu(view)", menubarCode);
        Assert.Contains("Dismiss()", menubarCode);
        Assert.Contains("IsLoading = true", menubarCode);

        var commandSample = Path.Combine(root, "Examples", "CSharp", "Navigation", "CommandInteraction.cs");
        Assert.True(File.Exists(commandSample), "Expected Command companion C# interaction sample.");
        var commandCode = File.ReadAllText(commandSample);
        Assert.Contains("ItemSelected", commandCode);
        Assert.Contains("TryHandleNavigationKey", commandCode);
        Assert.Contains("TrySelectActiveItem", commandCode);
        Assert.Contains("IsLoading = true", commandCode);
        Assert.Contains("CodexCommandInput", commandCode);
        Assert.Contains("CodexCommandEmpty", commandCode);
        Assert.Contains("Command = new SampleCommand", commandCode);
        Assert.Contains("CanExecute", commandCode);

        var tabsSample = Path.Combine(root, "Examples", "CSharp", "Navigation", "TabsInteraction.cs");
        Assert.True(File.Exists(tabsSample), "Expected Tabs companion C# interaction sample.");
        var tabsCode = File.ReadAllText(tabsSample);
        Assert.Contains("ValueChanged", tabsCode);
        Assert.Contains("ActivationMode = CodexTabsActivationMode.Manual", tabsCode);
        Assert.Contains("SelectedValue = \"preview\"", tabsCode);
        Assert.Contains("Orientation = Orientation.Vertical", tabsCode);
        Assert.Contains("IsLoop = false", tabsCode);
        Assert.Contains("IsEnabled = false", tabsCode);

        var breadcrumbSample = Path.Combine(root, "Examples", "CSharp", "Navigation", "BreadcrumbInteraction.cs");
        Assert.True(File.Exists(breadcrumbSample), "Expected Breadcrumb companion C# interaction sample.");
        var breadcrumbCode = File.ReadAllText(breadcrumbSample);
        Assert.Contains("LinkActivated", breadcrumbCode);
        Assert.Contains("TryActivate()", breadcrumbCode);
        Assert.Contains("IsCurrent = true", breadcrumbCode);
        Assert.Contains("CodexDropdownButton", breadcrumbCode);
        Assert.Contains("Command = new SampleCommand", breadcrumbCode);
        Assert.Contains("CanExecute", breadcrumbCode);

        var sideNavSample = Path.Combine(root, "Examples", "CSharp", "Navigation", "SideNavInteraction.cs");
        Assert.True(File.Exists(sideNavSample), "Expected SideNav companion C# interaction sample.");
        var sideNavCode = File.ReadAllText(sideNavSample);
        Assert.Contains("ValueChanged", sideNavCode);
        Assert.Contains("SelectedValue = \"sessions\"", sideNavCode);
        Assert.Contains("Detail = \"CanExecute=false\"", sideNavCode);
        Assert.Contains("Command = new SampleCommand", sideNavCode);
        Assert.Contains("IsEnabled = false", sideNavCode);
        Assert.Contains("Content = \"Dense label without icon\"", sideNavCode);

        var dropdownSample = Path.Combine(root, "Examples", "CSharp", "Navigation", "DropdownButtonInteraction.cs");
        Assert.True(File.Exists(dropdownSample), "Expected DropdownButton companion C# interaction sample.");
        var dropdownCode = File.ReadAllText(dropdownSample);
        Assert.Contains("OpenChanged", dropdownCode);
        Assert.Contains("RestoreFocusRequested", dropdownCode);
        Assert.Contains("RestoreFocusElement = trigger", dropdownCode);
        Assert.Contains("Open()", dropdownCode);
        Assert.Contains("Dismiss()", dropdownCode);
        Assert.Contains("CloseOnItemSelected = false", dropdownCode);
        Assert.Contains("IsLoading = true", dropdownCode);

        var menuSample = Path.Combine(root, "Examples", "CSharp", "Navigation", "MenuInteraction.cs");
        Assert.True(File.Exists(menuSample), "Expected Menu companion C# interaction sample.");
        var menuCode = File.ReadAllText(menuSample);
        Assert.Contains("ItemSelected", menuCode);
        Assert.Contains("DidCloseOnSelect", menuCode);
        Assert.Contains("MenuItemToggleType.CheckBox", menuCode);
        Assert.Contains("MenuItemToggleType.Radio", menuCode);
        Assert.Contains("Header = \"Focused submenu\"", menuCode);
        Assert.Contains("Header = \"Export blocked\"", menuCode);
        Assert.Contains("IsLoading = true", menuCode);
        Assert.Contains("Command = new SampleCommand", menuCode);

        var contextMenuSample = Path.Combine(root, "Examples", "CSharp", "Navigation", "ContextMenuInteraction.cs");
        Assert.True(File.Exists(contextMenuSample), "Expected ContextMenu companion C# interaction sample.");
        var contextMenuCode = File.ReadAllText(contextMenuSample);
        Assert.Contains("ItemSelected", contextMenuCode);
        Assert.Contains("DidCloseOnSelect", contextMenuCode);
        Assert.Contains("SubMenuPlacement = PlacementMode.RightEdgeAlignedTop", contextMenuCode);
        Assert.Contains("Placement = PlacementMode.Left", contextMenuCode);
        Assert.Contains("ContextMenuTarget(\"Right-click right side\"", contextMenuCode);
        Assert.Contains("ContextMenu = menu", contextMenuCode);
        Assert.Contains("IsInset = true", contextMenuCode);
        Assert.Contains("IsLoading = true", contextMenuCode);
        Assert.Contains("Command = new SampleCommand", contextMenuCode);

        var primitiveOverlaySample = Path.Combine(root, "Examples", "CSharp", "Primitives", "OverlayInteraction.cs");
        Assert.True(File.Exists(primitiveOverlaySample), "Expected Overlay primitive companion C# interaction sample.");
        var primitiveOverlayCode = File.ReadAllText(primitiveOverlaySample);
        Assert.Contains("CodexOverlay", primitiveOverlayCode);
        Assert.Contains("DismissCommand", primitiveOverlayCode);
        Assert.Contains("overlay.Dismiss()", primitiveOverlayCode);
        Assert.Contains("IsScrimVisible", primitiveOverlayCode);
        Assert.Contains("ScrimOpacity", primitiveOverlayCode);
        Assert.Contains("CloseOnEscape", primitiveOverlayCode);
        Assert.Contains("DismissOnOutsidePointer", primitiveOverlayCode);

        var cardSample = Path.Combine(root, "Examples", "CSharp", "DataDisplay", "CardInteraction.cs");
        Assert.True(File.Exists(cardSample), "Expected Card companion C# interaction sample.");
        var cardCode = File.ReadAllText(cardSample);
        Assert.Contains("CodexCard", cardCode);
        Assert.Contains("PointerReleased", cardCode);
        Assert.Contains("PointerUpdateKind.LeftButtonReleased", cardCode);
        Assert.Contains("configure.Click", cardCode);
        Assert.Contains("dynamicCard.Content", cardCode);
        Assert.Contains("IsEnabled = false", cardCode);

        var itemSample = Path.Combine(root, "Examples", "CSharp", "DataDisplay", "ItemInteraction.cs");
        Assert.True(File.Exists(itemSample), "Expected Item companion C# interaction sample.");
        var itemCode = File.ReadAllText(itemSample);
        Assert.Contains("CodexItem", itemCode);
        Assert.Contains("Activated", itemCode);
        Assert.Contains("ActivateCommandParameter = \"route\"", itemCode);
        Assert.Contains("TryHandleActivationKey(Key.Enter)", itemCode);
        Assert.Contains("Command = new SampleCommand", itemCode);
        Assert.Contains("CanExecute", itemCode);
        Assert.Contains("CodexItemGroup", itemCode);

        var carouselSample = Path.Combine(root, "Examples", "CSharp", "DataDisplay", "CarouselInteraction.cs");
        Assert.True(File.Exists(carouselSample), "Expected Carousel companion C# interaction sample.");
        var carouselCode = File.ReadAllText(carouselSample);
        Assert.Contains("CodexCarousel", carouselCode);
        Assert.Contains("SelectionChanged", carouselCode);
        Assert.Contains("PreviousCommand", carouselCode);
        Assert.Contains("NextCommand", carouselCode);
        Assert.Contains("Loop = loop", carouselCode);
        Assert.Contains("orientation: Orientation.Vertical", carouselCode);

        var providerCardSample = Path.Combine(root, "Examples", "CSharp", "DataDisplay", "ProviderCardInteraction.cs");
        Assert.True(File.Exists(providerCardSample), "Expected ProviderCard companion C# interaction sample.");
        var providerCardCode = File.ReadAllText(providerCardSample);
        Assert.Contains("CodexProviderCard", providerCardCode);
        Assert.Contains("Selected", providerCardCode);
        Assert.Contains("args.Source", providerCardCode);
        Assert.Contains("IsDragging = isDragging", providerCardCode);
        Assert.Contains("IsEnabled = isEnabled", providerCardCode);
        Assert.Contains("Command = new SampleCommand", providerCardCode);
        Assert.Contains("CanExecute", providerCardCode);

        var paginationSample = Path.Combine(root, "Examples", "CSharp", "DataDisplay", "PaginationInteraction.cs");
        Assert.True(File.Exists(paginationSample), "Expected Pagination companion C# interaction sample.");
        var paginationCode = File.ReadAllText(paginationSample);
        Assert.Contains("CodexPagination", paginationCode);
        Assert.Contains("PageChanged", paginationCode);
        Assert.Contains("SelectPage(interactive.Page == 21 ? 9 : 21)", paginationCode);
        Assert.Contains("IsCompact = true", paginationCode);
        Assert.Contains("IsLoading = true", paginationCode);
        Assert.Contains("CodexPaginationPageButton", paginationCode);
        Assert.Contains("Command = new SampleCommand", paginationCode);

        var tableSample = Path.Combine(root, "Examples", "CSharp", "DataDisplay", "TableInteraction.cs");
        Assert.True(File.Exists(tableSample), "Expected Table companion C# interaction sample.");
        var tableCode = File.ReadAllText(tableSample);
        Assert.Contains("CodexTable", tableCode);
        Assert.Contains("CodexTableRow", tableCode);
        Assert.Contains("PointerReleased", tableCode);
        Assert.Contains("PointerUpdateKind.LeftButtonReleased", tableCode);
        Assert.Contains("table.TransitionKey", tableCode);
        Assert.Contains("table.IsCompact", tableCode);
        Assert.Contains("table.IsHoverable", tableCode);
        Assert.Contains("IsEnabled = isEnabled", tableCode);

        var dataTableSample = Path.Combine(root, "Examples", "CSharp", "DataDisplay", "DataTableInteraction.cs");
        Assert.True(File.Exists(dataTableSample), "Expected DataTable companion C# interaction sample.");
        var dataTableCode = File.ReadAllText(dataTableSample);
        Assert.Contains("DataTableVisibleRows", dataTableCode);
        Assert.Contains("failedOnly = !failedOnly", dataTableCode);
        Assert.Contains("sortDescending = !sortDescending", dataTableCode);
        Assert.Contains("showAmount = !showAmount", dataTableCode);
        Assert.Contains("payment.IsSelected = !payment.IsSelected", dataTableCode);
        Assert.Contains("CodexDropdownButton", dataTableCode);
        Assert.Contains("CodexPagination", dataTableCode);
        Assert.Contains("TransitionKey", dataTableCode);

        var pinnedTableSample = Path.Combine(root, "Examples", "CSharp", "DataDisplay", "PinnedTableInteraction.cs");
        Assert.True(File.Exists(pinnedTableSample), "Expected PinnedTable companion C# interaction sample.");
        var pinnedTableCode = File.ReadAllText(pinnedTableSample);
        Assert.Contains("CodexPinnedTable", pinnedTableCode);
        Assert.Contains("StartCellTemplate", pinnedTableCode);
        Assert.Contains("MiddleCellTemplate", pinnedTableCode);
        Assert.Contains("EndCellTemplate", pinnedTableCode);
        Assert.Contains("table.IsLoading", pinnedTableCode);
        Assert.Contains("table.IsCompact", pinnedTableCode);
        Assert.Contains("table.TransitionKey", pinnedTableCode);
        Assert.Contains("FuncDataTemplate<PinnedProviderRow>", pinnedTableCode);

        var aspectRatioSample = Path.Combine(root, "Examples", "CSharp", "DataDisplay", "AspectRatioInteraction.cs");
        Assert.True(File.Exists(aspectRatioSample), "Expected AspectRatio companion C# interaction sample.");
        var aspectRatioCode = File.ReadAllText(aspectRatioSample);
        Assert.Contains("CodexAspectRatio", aspectRatioCode);
        Assert.Contains("RatioChanged", aspectRatioCode);
        Assert.Contains("CodexAspectRatioFitMode", aspectRatioCode);
        Assert.Contains("aspectRatio.Ratio = -1d", aspectRatioCode);
        Assert.Contains("aspectRatio.Content", aspectRatioCode);
        Assert.Contains("double.NaN", aspectRatioCode);

        var chartSample = Path.Combine(root, "Examples", "CSharp", "DataDisplay", "ChartInteraction.cs");
        Assert.True(File.Exists(chartSample), "Expected Chart companion C# interaction sample.");
        var chartCode = File.ReadAllText(chartSample);
        Assert.Contains("CodexChartContainer", chartCode);
        Assert.Contains("CodexUsagePieChart", chartCode);
        Assert.Contains("container.TransitionKey", chartCode);
        Assert.Contains("container.IsRefreshing", chartCode);
        Assert.Contains("ChartTooltip", chartCode);
        Assert.Contains("CodexChartIndicatorStyle", chartCode);

        var barChartSample = Path.Combine(root, "Examples", "CSharp", "DataDisplay", "BarChartInteraction.cs");
        Assert.True(File.Exists(barChartSample), "Expected BarChart companion C# interaction sample.");
        var barChartCode = File.ReadAllText(barChartSample);
        Assert.Contains("CodexBarChart", barChartCode);
        Assert.Contains("ActiveItemChanged", barChartCode);
        Assert.Contains("chart.ItemsSource", barChartCode);
        Assert.Contains("chart.Orientation", barChartCode);
        Assert.Contains("ShowGridLines", barChartCode);
        Assert.Contains("AnimationDuration", barChartCode);

        var lineChartSample = Path.Combine(root, "Examples", "CSharp", "DataDisplay", "LineChartInteraction.cs");
        Assert.True(File.Exists(lineChartSample), "Expected LineChart companion C# interaction sample.");
        var lineChartCode = File.ReadAllText(lineChartSample);
        Assert.Contains("CodexLineChart", lineChartCode);
        Assert.Contains("ActivePointChanged", lineChartCode);
        Assert.Contains("chart.ItemsSource", lineChartCode);
        Assert.Contains("chart.IsCompact", lineChartCode);
        Assert.Contains("chart.ShowArea", lineChartCode);
        Assert.Contains("chart.ShowDots", lineChartCode);
        Assert.Contains("AnimationDuration", lineChartCode);

        var metricSample = Path.Combine(root, "Examples", "CSharp", "DataDisplay", "MetricInteraction.cs");
        Assert.True(File.Exists(metricSample), "Expected Metric companion C# interaction sample.");
        var metricCode = File.ReadAllText(metricSample);
        Assert.Contains("CodexStatCard", metricCode);
        Assert.Contains("tokens.Value", metricCode);
        Assert.Contains("tokens.Detail", metricCode);
        Assert.Contains("tokens.Icon", metricCode);
        Assert.Contains("latency.Detail", metricCode);
        Assert.Contains("CodexAvatar", metricCode);

        var imageIconSample = Path.Combine(root, "Examples", "CSharp", "DataDisplay", "ImageIconInteraction.cs");
        Assert.True(File.Exists(imageIconSample), "Expected ImageIcon companion C# interaction sample.");
        var imageIconCode = File.ReadAllText(imageIconSample);
        Assert.Contains("CodexImageIcon", imageIconCode);
        Assert.Contains("ImageLoaded", imageIconCode);
        Assert.Contains("ImageLoadFailed", imageIconCode);
        Assert.Contains("icon.Path", imageIconCode);
        Assert.Contains("HasSource", imageIconCode);
        Assert.Contains("IsEnabled = false", imageIconCode);

        var scrollAreaSample = Path.Combine(root, "Examples", "CSharp", "DataDisplay", "ScrollAreaInteraction.cs");
        Assert.True(File.Exists(scrollAreaSample), "Expected ScrollArea companion C# interaction sample.");
        var scrollAreaCode = File.ReadAllText(scrollAreaSample);
        Assert.Contains("CodexScrollArea", scrollAreaCode);
        Assert.Contains("ScrollChanged", scrollAreaCode);
        Assert.Contains("ScrollToTop", scrollAreaCode);
        Assert.Contains("ScrollToBottom", scrollAreaCode);
        Assert.Contains("CodexScrollAreaType.Hover", scrollAreaCode);
        Assert.Contains("IsEnabled = false", scrollAreaCode);

        var rankedBarChartSample = Path.Combine(root, "Examples", "CSharp", "DataDisplay", "RankedBarChartInteraction.cs");
        Assert.True(File.Exists(rankedBarChartSample), "Expected RankedBarChart companion C# interaction sample.");
        var rankedBarChartCode = File.ReadAllText(rankedBarChartSample);
        Assert.Contains("CodexRankedBarChart", rankedBarChartCode);
        Assert.Contains("ActiveItemChanged", rankedBarChartCode);
        Assert.Contains("chart.ItemsSource", rankedBarChartCode);
        Assert.Contains("chart.IsCompact", rankedBarChartCode);
        Assert.Contains("MaxVisibleItems", rankedBarChartCode);
        Assert.Contains("EmptyText", rankedBarChartCode);

        var usagePieChartSample = Path.Combine(root, "Examples", "CSharp", "DataDisplay", "UsagePieChartInteraction.cs");
        Assert.True(File.Exists(usagePieChartSample), "Expected UsagePieChart companion C# interaction sample.");
        var usagePieChartCode = File.ReadAllText(usagePieChartSample);
        Assert.Contains("CodexUsagePieChart", usagePieChartCode);
        Assert.Contains("ActiveItemChanged", usagePieChartCode);
        Assert.Contains("chart.ItemsSource", usagePieChartCode);
        Assert.Contains("chart.AnimationDuration", usagePieChartCode);
        Assert.Contains("TimeSpan.Zero", usagePieChartCode);
        Assert.Contains("IsCompact", usagePieChartCode);

        var usageTrendChartSample = Path.Combine(root, "Examples", "CSharp", "DataDisplay", "UsageTrendChartInteraction.cs");
        Assert.True(File.Exists(usageTrendChartSample), "Expected UsageTrendChart companion C# interaction sample.");
        var usageTrendChartCode = File.ReadAllText(usageTrendChartSample);
        Assert.Contains("CsUsageTrendChart", usageTrendChartCode);
        Assert.Contains("UsageTrendChartGranularity", usageTrendChartCode);
        Assert.Contains("chart.IsRefreshing", usageTrendChartCode);
        Assert.Contains("chart.ItemsSource", usageTrendChartCode);
        Assert.Contains("Array.Empty<UsageTrendChartPoint>()", usageTrendChartCode);
        Assert.Contains("UsagePoint", usageTrendChartCode);
    }

    [Fact]
    public void DocsAxamlSamplesExposePublicCompositionPrimitivesDirectly()
    {
        var mainWindow = ReadDocsSource("MainWindow.cs");
        var buttonGroup = ReadDocsSource(Path.Combine("Examples", "Axaml", "Forms", "ButtonGroupAnatomy.axaml"));
        var chart = ReadDocsSource(Path.Combine("Examples", "Axaml", "DataDisplay", "Chart.axaml"));
        var command = ReadDocsSource(Path.Combine("Examples", "Axaml", "Navigation", "CommandAnatomy.axaml"));
        var field = ReadDocsSource(Path.Combine("Examples", "Axaml", "Forms", "FieldGroup.axaml"));
        var inputGroup = ReadDocsSource(Path.Combine("Examples", "Axaml", "Forms", "InputGroupAnatomy.axaml"));
        var inputOtp = ReadDocsSource(Path.Combine("Examples", "Axaml", "Forms", "InputOtpAnatomy.axaml"));
        var item = ReadDocsSource(Path.Combine("Examples", "Axaml", "DataDisplay", "ItemAnatomy.axaml"));
        var label = ReadDocsSource(Path.Combine("Examples", "Axaml", "Forms", "LabelAnatomy.axaml"));
        var menubar = ReadDocsSource(Path.Combine("Examples", "Axaml", "Navigation", "MenubarAnatomy.axaml"));
        var resizable = ReadDocsSource(Path.Combine("Examples", "Axaml", "Layout", "ResizableAnatomy.axaml"));
        var sidebar = ReadDocsSource(Path.Combine("Examples", "Axaml", "Layout", "SidebarPrimitivesAnatomy.axaml"));

        Assert.Contains("<controls:CodexButtonGroup", buttonGroup);
        Assert.Contains("<controls:CodexButtonGroupText", buttonGroup);
        Assert.Contains("<controls:CodexButtonGroupSeparator", buttonGroup);
        Assert.Contains("new CodexButtonGroup", mainWindow);
        Assert.Contains("new CodexButtonGroupText", mainWindow);
        Assert.Contains("new CodexButtonGroupSeparator", mainWindow);
        Assert.Contains("<controls:CodexChart ", chart);
        Assert.Contains("return new CodexChart", mainWindow);
        Assert.Contains("<controls:CodexCommandShortcut", command);
        Assert.Contains("new CodexCommandShortcut", mainWindow);
        Assert.Contains("<controls:CodexFieldLegend", field);
        Assert.Contains("new CodexFieldLegend", mainWindow);
        Assert.Contains("<controls:CodexInputGroup", inputGroup);
        Assert.Contains("<controls:CodexInputGroupAddon", inputGroup);
        Assert.Contains("<controls:CodexInputGroupInput", inputGroup);
        Assert.Contains("<controls:CodexInputGroupTextarea", inputGroup);
        Assert.Contains("<controls:CodexInputGroupButton", inputGroup);
        Assert.Contains("<controls:CodexInputGroupText", inputGroup);
        Assert.Contains("new CodexInputGroup", mainWindow);
        Assert.Contains("new CodexInputGroupAddon", mainWindow);
        Assert.Contains("new CodexInputGroupInput", mainWindow);
        Assert.Contains("new CodexInputGroupTextarea", mainWindow);
        Assert.Contains("new CodexInputGroupButton", mainWindow);
        Assert.Contains("new CodexInputGroupText", mainWindow);
        Assert.Contains("<controls:CodexInputOtp", inputOtp);
        Assert.Contains("<controls:CodexInputOtpGroup", inputOtp);
        Assert.Contains("<controls:CodexInputOtpSlot", inputOtp);
        Assert.Contains("<controls:CodexInputOtpSeparator", inputOtp);
        Assert.Contains("new CodexInputOtp", mainWindow);
        Assert.Contains("new CodexInputOtpGroup", mainWindow);
        Assert.Contains("new CodexInputOtpSlot", mainWindow);
        Assert.Contains("new CodexInputOtpSeparator", mainWindow);
        Assert.Contains("<controls:CodexItemHeader", item);
        Assert.Contains("<controls:CodexItemContent", item);
        Assert.Contains("<controls:CodexItemActions", item);
        Assert.Contains("new CodexItemHeader", mainWindow);
        Assert.Contains("new CodexItemContent", mainWindow);
        Assert.Contains("new CodexItemActions", mainWindow);
        Assert.Contains("<controls:CodexLabel", label);
        Assert.Contains("Target=", label);
        Assert.Contains("IsRequired=", label);
        Assert.Contains("new CodexLabel", mainWindow);
        Assert.Contains("<controls:CodexMenubarLabel", menubar);
        Assert.Contains("<controls:CodexMenubarGroup", menubar);
        Assert.Contains("<controls:CodexMenubarSeparator", menubar);
        Assert.Contains("<controls:CodexMenubarCheckboxItem", menubar);
        Assert.Contains("<controls:CodexMenubarRadioItem", menubar);
        Assert.Contains("new CodexMenubarLabel", mainWindow);
        Assert.Contains("new CodexMenubarGroup", mainWindow);
        Assert.Contains("new CodexMenubarCheckboxItem", mainWindow);
        Assert.Contains("new CodexMenubarRadioItem", mainWindow);
        Assert.Contains("<controls:CodexResizablePanelGroup", resizable);
        Assert.Contains("<controls:CodexResizablePanel", resizable);
        Assert.Contains("<controls:CodexResizableHandle", resizable);
        Assert.Contains("new CodexResizablePanelGroup", mainWindow);
        Assert.Contains("new CodexResizablePanel", mainWindow);
        Assert.Contains("new CodexResizableHandle", mainWindow);
        Assert.Contains("<controls:CodexSidebarGroupContent", sidebar);
        Assert.Contains("new CodexSidebarGroupContent", mainWindow);
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
        Assert.Contains("\"Layout/ResizableAnatomy.axaml\", BuildResizableAnatomyPreview", source);
        Assert.Contains("\"Layout/SidebarInteraction.axaml\"", source);
        Assert.Contains("BuildSidebarInteractionPreview", source);
        Assert.Contains("Code(\"Layout/SidebarInteraction.cs\", \"CSharp/Layout/SidebarInteraction.cs\")", source);
        Assert.Contains("\"Layout/ApplicationShellInteraction.axaml\"", source);
        Assert.Contains("BuildApplicationShellInteractionPreview", source);
        Assert.Contains("Code(\"Layout/ApplicationShellInteraction.cs\", \"CSharp/Layout/ApplicationShellInteraction.cs\")", source);
        Assert.Contains("\"Layout/SidebarPrimitivesInteraction.axaml\"", source);
        Assert.Contains("BuildSidebarPrimitivesInteractionPreview", source);
        Assert.Contains("Code(\"Layout/SidebarPrimitivesInteraction.cs\", \"CSharp/Layout/SidebarPrimitivesInteraction.cs\")", source);
        Assert.Contains("\"Layout/SectionInteraction.axaml\"", source);
        Assert.Contains("BuildSectionComponentInteractionPreview", source);
        Assert.Contains("Code(\"Layout/SectionInteraction.cs\", \"CSharp/Layout/SectionInteraction.cs\")", source);
        Assert.Contains("\"Layout/ResizableInteraction.axaml\"", source);
        Assert.Contains("BuildResizableInteractionPreview", source);
        Assert.Contains("Code(\"Layout/ResizableInteraction.cs\", \"CSharp/Layout/ResizableInteraction.cs\")", source);
        Assert.Contains("\"Forms/SplitButton.axaml\", BuildSplitButtonPreview", source);
        Assert.Contains("\"Forms/ButtonGroup.axaml\", BuildButtonGroupPreview", source);
        Assert.Contains("\"Forms/ButtonGroupAnatomy.axaml\", BuildButtonGroupAnatomyPreview", source);
        Assert.Contains("\"Forms/ButtonGroupComposition.axaml\", BuildButtonGroupCompositionPreview", source);
        Assert.Contains("\"Forms/ButtonGroupInteraction.axaml\", BuildButtonGroupInteractionPreview", source);
        Assert.Contains("\"Forms/InputGroup.axaml\", BuildInputGroupPreview", source);
        Assert.Contains("\"Forms/InputGroupAnatomy.axaml\", BuildInputGroupAnatomyPreview", source);
        Assert.Contains("\"Forms/InputGroupComposition.axaml\", BuildInputGroupCompositionPreview", source);
        Assert.Contains("\"Forms/InputGroupInteraction.axaml\", BuildInputGroupInteractionPreview", source);
        Assert.Contains("\"Forms/InputOtp.axaml\", BuildInputOtpPreview", source);
        Assert.Contains("\"Forms/InputOtpAnatomy.axaml\", BuildInputOtpAnatomyPreview", source);
        Assert.Contains("\"Forms/InputOtpComposition.axaml\", BuildInputOtpCompositionPreview", source);
        Assert.Contains("\"Forms/InputOtpInteraction.axaml\", BuildInputOtpInteractionPreview", source);
        Assert.Contains("\"Forms/Label.axaml\", BuildLabelPreview", source);
        Assert.Contains("\"Forms/LabelAnatomy.axaml\", BuildLabelAnatomyPreview", source);
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
        Assert.Contains("\"Forms/IconButtonAnatomy.axaml\", BuildIconButtonAnatomyPreview", source);
        Assert.Contains("\"Forms/IconButtonInteraction.axaml\", BuildIconButtonInteractionPreview", source);
        Assert.Contains("\"Forms/SplitButtonAnatomy.axaml\", BuildSplitButtonAnatomyPreview", source);
        Assert.Contains("\"Forms/FieldAnatomy.axaml\", BuildFieldAnatomyPreview", source);
        Assert.Contains("\"Forms/FieldGroup.axaml\", BuildFieldGroupPreview", source);
        Assert.Contains("\"Forms/FieldInteraction.axaml\", BuildFieldInteractionPreview", source);
        Assert.Contains("\"Forms/TextBoxAnatomy.axaml\", BuildTextBoxAnatomyPreview", source);
        Assert.Contains("\"Forms/TextBoxInteraction.axaml\", BuildTextBoxInteractionPreview", source);
        Assert.Contains("\"Forms/TextareaAnatomy.axaml\", BuildTextareaAnatomyPreview", source);
        Assert.Contains("\"Forms/TextareaInteraction.axaml\", BuildTextareaInteractionPreview", source);
        Assert.Contains("\"Forms/SelectAnatomy.axaml\", BuildSelectAnatomyPreview", source);
        Assert.Contains("\"Forms/SelectInteraction.axaml\"", source);
        Assert.Contains("BuildSelectInteractionPreview", source);
        Assert.Contains("Code(\"Forms/SelectInteraction.cs\", \"CSharp/Forms/SelectInteraction.cs\")", source);
        Assert.Contains("\"Forms/Combobox.axaml\", BuildComboboxPreview", source);
        Assert.Contains("\"Forms/ComboboxStates.axaml\", BuildComboboxStatesPreview", source);
        Assert.Contains("\"Forms/ComboboxAnatomy.axaml\", BuildComboboxAnatomyPreview", source);
        Assert.Contains("\"Forms/ComboboxInteraction.axaml\"", source);
        Assert.Contains("BuildComboboxInteractionPreview", source);
        Assert.Contains("Code(\"Forms/ComboboxInteraction.cs\", \"CSharp/Forms/ComboboxInteraction.cs\")", source);
        Assert.Contains("\"Forms/NativeSelect.axaml\", BuildNativeSelectPreview", source);
        Assert.Contains("\"Forms/NativeSelectAnatomy.axaml\", BuildNativeSelectAnatomyPreview", source);
        Assert.Contains("\"Forms/NativeSelectComposition.axaml\", BuildNativeSelectCompositionPreview", source);
        Assert.Contains("\"Forms/NativeSelectInteraction.axaml\"", source);
        Assert.Contains("BuildNativeSelectInteractionPreview", source);
        Assert.Contains("Code(\"Forms/NativeSelectInteraction.cs\", \"CSharp/Forms/NativeSelectInteraction.cs\")", source);
        Assert.Contains("\"Forms/Calendar.axaml\", BuildCalendarPreview", source);
        Assert.Contains("\"Forms/CalendarAnatomy.axaml\", BuildCalendarAnatomyPreview", source);
        Assert.Contains("\"Forms/CalendarComposition.axaml\", BuildCalendarCompositionPreview", source);
        Assert.Contains("\"Forms/CalendarInteraction.axaml\"", source);
        Assert.Contains("BuildCalendarInteractionPreview", source);
        Assert.Contains("Code(\"Forms/CalendarInteraction.cs\", \"CSharp/Forms/CalendarInteraction.cs\")", source);
        Assert.Contains("\"Forms/DatePicker.axaml\", BuildDatePickerPreview", source);
        Assert.Contains("\"Forms/DatePickerStates.axaml\", BuildDatePickerStatesPreview", source);
        Assert.Contains("\"Forms/DatePickerAnatomy.axaml\", BuildDatePickerAnatomyPreview", source);
        Assert.Contains("\"Forms/DatePickerInteraction.axaml\"", source);
        Assert.Contains("BuildDatePickerInteractionPreview", source);
        Assert.Contains("Code(\"Forms/DatePickerInteraction.cs\", \"CSharp/Forms/DatePickerInteraction.cs\")", source);
        Assert.Contains("\"Forms/SplitButtonInteraction.axaml\", BuildSplitButtonInteractionPreview", source);
        Assert.Contains("\"Forms/CheckboxAnatomy.axaml\", BuildCheckboxAnatomyPreview", source);
        Assert.Contains("\"Forms/CheckboxInteraction.axaml\"", source);
        Assert.Contains("BuildCheckboxInteractionPreview", source);
        Assert.Contains("Code(\"Forms/CheckboxInteraction.cs\", \"CSharp/Forms/CheckboxInteraction.cs\")", source);
        Assert.Contains("\"Forms/RadioAnatomy.axaml\", BuildRadioAnatomyPreview", source);
        Assert.Contains("\"Forms/RadioInteraction.axaml\"", source);
        Assert.Contains("BuildRadioInteractionPreview", source);
        Assert.Contains("Code(\"Forms/RadioInteraction.cs\", \"CSharp/Forms/RadioInteraction.cs\")", source);
        Assert.Contains("\"Forms/RadioGroupInteraction.axaml\"", source);
        Assert.Contains("BuildRadioGroupInteractionPreview", source);
        Assert.Contains("Code(\"Forms/RadioGroupInteraction.cs\", \"CSharp/Forms/RadioGroupInteraction.cs\")", source);
        Assert.Contains("\"Forms/SwitchAnatomy.axaml\", BuildSwitchAnatomyPreview", source);
        Assert.Contains("\"Forms/SwitchInteraction.axaml\"", source);
        Assert.Contains("BuildSwitchInteractionPreview", source);
        Assert.Contains("Code(\"Forms/SwitchInteraction.cs\", \"CSharp/Forms/SwitchInteraction.cs\")", source);
        Assert.Contains("\"Forms/ToggleAnatomy.axaml\", BuildToggleAnatomyPreview", source);
        Assert.Contains("\"Forms/ToggleInteraction.axaml\"", source);
        Assert.Contains("BuildToggleInteractionPreview", source);
        Assert.Contains("Code(\"Forms/ToggleInteraction.cs\", \"CSharp/Forms/ToggleInteraction.cs\")", source);
        Assert.Contains("\"Forms/ToggleGroupInteraction.axaml\"", source);
        Assert.Contains("BuildToggleGroupInteractionPreview", source);
        Assert.Contains("Code(\"Forms/ToggleGroupInteraction.cs\", \"CSharp/Forms/ToggleGroupInteraction.cs\")", source);
        Assert.Contains("\"Forms/SliderAnatomy.axaml\", BuildSliderAnatomyPreview", source);
        Assert.Contains("\"Forms/SliderInteraction.axaml\"", source);
        Assert.Contains("BuildSliderInteractionPreview", source);
        Assert.Contains("Code(\"Forms/SliderInteraction.cs\", \"CSharp/Forms/SliderInteraction.cs\")", source);
        Assert.Contains("\"Feedback/Alert.axaml\", BuildAlertPreview", source);
        Assert.Contains("\"Feedback/AlertAnatomy.axaml\", BuildAlertAnatomyPreview", source);
        Assert.Contains("\"Feedback/AlertInteraction.axaml\"", source);
        Assert.Contains("BuildAlertInteractionPreview", source);
        Assert.Contains("Code(\"Feedback/AlertInteraction.cs\", \"CSharp/Feedback/AlertInteraction.cs\")", source);
        Assert.Contains("\"Feedback/Toast.axaml\", BuildToastPreview", source);
        Assert.Contains("\"Feedback/ToastAnatomy.axaml\", BuildToastAnatomyPreview", source);
        Assert.Contains("\"Feedback/ToastInteraction.axaml\"", source);
        Assert.Contains("BuildToastInteractionPreview", source);
        Assert.Contains("Code(\"Feedback/ToastInteraction.cs\", \"CSharp/Feedback/ToastInteraction.cs\")", source);
        Assert.Contains("\"Feedback/Sonner.axaml\", BuildSonnerPreview", source);
        Assert.Contains("\"Feedback/SonnerAnatomy.axaml\", BuildSonnerAnatomyPreview", source);
        Assert.Contains("\"Feedback/SonnerInteraction.axaml\"", source);
        Assert.Contains("BuildSonnerInteractionPreview", source);
        Assert.Contains("Code(\"Feedback/SonnerInteraction.cs\", \"CSharp/Feedback/SonnerInteraction.cs\")", source);
        Assert.Contains("\"Feedback/BadgeInteraction.axaml\"", source);
        Assert.Contains("BuildBadgeInteractionPreview", source);
        Assert.Contains("Code(\"Feedback/BadgeInteraction.cs\", \"CSharp/Feedback/BadgeInteraction.cs\")", source);
        Assert.Contains("\"Feedback/BadgeAnatomy.axaml\", BuildBadgeAnatomyPreview", source);
        Assert.Contains("\"Feedback/AvatarInteraction.axaml\"", source);
        Assert.Contains("BuildAvatarInteractionPreview", source);
        Assert.Contains("Code(\"Feedback/AvatarInteraction.cs\", \"CSharp/Feedback/AvatarInteraction.cs\")", source);
        Assert.Contains("\"Feedback/AvatarAnatomy.axaml\", BuildAvatarAnatomyPreview", source);
        Assert.Contains("Page(\"feedback.avatar-group\"", source);
        Assert.Contains("\"Feedback/AvatarGroup.axaml\", BuildAvatarGroupPreview", source);
        Assert.Contains("\"Feedback/AvatarGroupAnatomy.axaml\", BuildAvatarGroupAnatomyPreview", source);
        Assert.Contains("\"Feedback/AvatarGroupInteraction.axaml\"", source);
        Assert.Contains("BuildAvatarGroupInteractionPreview", source);
        Assert.Contains("Code(\"Feedback/AvatarGroupInteraction.cs\", \"CSharp/Feedback/AvatarGroupInteraction.cs\")", source);
        Assert.Contains("\"Feedback/EmptyStateAnatomy.axaml\", BuildEmptyStateAnatomyPreview", source);
        Assert.Contains("\"Feedback/EmptyStateInteraction.axaml\", BuildEmptyStateInteractionPreview", source);
        Assert.Contains("\"Feedback/SpinnerAnatomy.axaml\", BuildSpinnerAnatomyPreview", source);
        Assert.Contains("\"Feedback/SpinnerInteraction.axaml\", BuildSpinnerInteractionPreview", source);
        Assert.Contains("\"Feedback/ProgressAnatomy.axaml\", BuildProgressAnatomyPreview", source);
        Assert.Contains("\"Feedback/ProgressInteraction.axaml\", BuildProgressInteractionPreview", source);
        Assert.Contains("\"Feedback/Skeleton.axaml\", BuildSkeletonPreview", source);
        Assert.Contains("\"Feedback/SkeletonAnatomy.axaml\", BuildSkeletonAnatomyPreview", source);
        Assert.Contains("Example(\"Skeleton interaction\"", source);
        Assert.Contains("\"Feedback/SkeletonInteraction.axaml\", BuildSkeletonInteractionPreview", source);
        Assert.Contains("\"Navigation/TabsAnatomy.axaml\", BuildTabsAnatomyPreview", source);
        Assert.Contains("\"Navigation/TabsInteraction.axaml\"", source);
        Assert.Contains("BuildTabsInteractionPreview", source);
        Assert.Contains("Code(\"Navigation/TabsInteraction.cs\", \"CSharp/Navigation/TabsInteraction.cs\")", source);
        Assert.Contains("\"Navigation/Breadcrumb.axaml\", BuildBreadcrumbPreview", source);
        Assert.Contains("\"Navigation/BreadcrumbAnatomy.axaml\", BuildBreadcrumbAnatomyPreview", source);
        Assert.Contains("\"Navigation/BreadcrumbInteraction.axaml\"", source);
        Assert.Contains("BuildBreadcrumbInteractionPreview", source);
        Assert.Contains("Code(\"Navigation/BreadcrumbInteraction.cs\", \"CSharp/Navigation/BreadcrumbInteraction.cs\")", source);
        Assert.Contains("\"Navigation/NavigationMenu.axaml\", BuildNavigationMenuPreview", source);
        Assert.Contains("\"Navigation/NavigationMenuInteraction.axaml\"", source);
        Assert.Contains("BuildNavigationMenuInteractionPreview", source);
        Assert.Contains("Code(\"Navigation/NavigationMenuInteraction.cs\", \"CSharp/Navigation/NavigationMenuInteraction.cs\")", source);
        Assert.Contains("\"Navigation/Menubar.axaml\", BuildMenubarPreview", source);
        Assert.Contains("\"Navigation/MenubarAnatomy.axaml\", BuildMenubarAnatomyPreview", source);
        Assert.Contains("\"Navigation/MenubarComposition.axaml\", BuildMenubarCompositionPreview", source);
        Assert.Contains("\"Navigation/MenubarInteraction.axaml\"", source);
        Assert.Contains("BuildMenubarInteractionPreview", source);
        Assert.Contains("Code(\"Navigation/MenubarInteraction.cs\", \"CSharp/Navigation/MenubarInteraction.cs\")", source);
        Assert.Contains("\"Navigation/SideNav.axaml\", BuildSideNavPreview", source);
        Assert.Contains("\"Navigation/SideNavAnatomy.axaml\", BuildSideNavAnatomyPreview", source);
        Assert.Contains("\"Navigation/SideNavInteraction.axaml\"", source);
        Assert.Contains("BuildSideNavInteractionPreview", source);
        Assert.Contains("Code(\"Navigation/SideNavInteraction.cs\", \"CSharp/Navigation/SideNavInteraction.cs\")", source);
        Assert.Contains("\"Navigation/SegmentedControl.axaml\", BuildSegmentedControlPreview", source);
        Assert.Contains("\"Navigation/SegmentedControlAnatomy.axaml\", BuildSegmentedControlAnatomyPreview", source);
        Assert.Contains("\"Navigation/SegmentedControlInteraction.axaml\", BuildSegmentedControlInteractionPreview", source);
        Assert.Contains("\"Navigation/NavigationMenuAnatomy.axaml\", BuildNavigationMenuAnatomyPreview", source);
        Assert.Contains("\"Navigation/DropdownButton.axaml\", BuildDropdownPreview", source);
        Assert.Contains("\"Navigation/DropdownButtonAnatomy.axaml\", BuildDropdownAnatomyPreview", source);
        Assert.Contains("\"Navigation/DropdownButtonInteraction.axaml\"", source);
        Assert.Contains("BuildDropdownInteractionPreview", source);
        Assert.Contains("Code(\"Navigation/DropdownButtonInteraction.cs\", \"CSharp/Navigation/DropdownButtonInteraction.cs\")", source);
        Assert.Contains("\"Navigation/Menu.axaml\", BuildMenuPreview", source);
        Assert.Contains("\"Navigation/MenuAnatomy.axaml\", BuildMenuAnatomyPreview", source);
        Assert.Contains("\"Navigation/MenuInteraction.axaml\"", source);
        Assert.Contains("BuildMenuInteractionPreview", source);
        Assert.Contains("Code(\"Navigation/MenuInteraction.cs\", \"CSharp/Navigation/MenuInteraction.cs\")", source);
        Assert.Contains("\"Navigation/ContextMenu.axaml\", BuildContextMenuPreview", source);
        Assert.Contains("\"Navigation/ContextMenuAnatomy.axaml\", BuildContextMenuAnatomyPreview", source);
        Assert.Contains("\"Navigation/ContextMenuInteraction.axaml\"", source);
        Assert.Contains("BuildContextMenuInteractionPreview", source);
        Assert.Contains("Code(\"Navigation/ContextMenuInteraction.cs\", \"CSharp/Navigation/ContextMenuInteraction.cs\")", source);
        Assert.Contains("\"Navigation/Command.axaml\", BuildCommandPreview", source);
        Assert.Contains("\"Navigation/CommandAnatomy.axaml\", BuildCommandAnatomyPreview", source);
        Assert.Contains("\"Navigation/CommandFiltering.axaml\", BuildCommandFilteringPreview", source);
        Assert.Contains("\"Navigation/CommandScrollable.axaml\", BuildCommandScrollablePreview", source);
        Assert.Contains("\"Navigation/CommandInteraction.axaml\"", source);
        Assert.Contains("BuildCommandInteractionPreview", source);
        Assert.Contains("Code(\"Navigation/CommandInteraction.cs\", \"CSharp/Navigation/CommandInteraction.cs\")", source);
        Assert.Contains("\"Navigation/Accordion.axaml\", BuildAccordionPreview", source);
        Assert.Contains("\"Navigation/AccordionAnatomy.axaml\", BuildAccordionAnatomyPreview", source);
        Assert.Contains("\"Navigation/AccordionInteraction.axaml\", BuildAccordionInteractionPreview", source);
        Assert.Contains("\"Navigation/Collapsible.axaml\", BuildCollapsiblePreview", source);
        Assert.Contains("\"Navigation/CollapsibleAnatomy.axaml\", BuildCollapsibleAnatomyPreview", source);
        Assert.Contains("\"Navigation/CollapsibleInteraction.axaml\", BuildCollapsibleInteractionPreview", source);
        Assert.Contains("\"Navigation/SeparatorAnatomy.axaml\", BuildSeparatorAnatomyPreview", source);
        Assert.Contains("\"Navigation/SeparatorInteraction.axaml\", BuildSeparatorInteractionPreview", source);
        Assert.Contains("\"Navigation/KbdInteraction.axaml\", BuildKbdInteractionPreview", source);
        Assert.Contains("\"Overlay/Dialog.axaml\", BuildDialogPreview", source);
        Assert.Contains("\"Overlay/DialogAnatomy.axaml\", BuildDialogAnatomyPreview", source);
        Assert.Contains("\"Overlay/DialogInteraction.axaml\"", source);
        Assert.Contains("BuildDialogInteractionPreview", source);
        Assert.Contains("Code(\"Overlay/DialogInteraction.cs\", \"CSharp/Overlay/DialogInteraction.cs\")", source);
        Assert.Contains("\"Overlay/AlertDialog.axaml\", BuildAlertDialogPreview", source);
        Assert.Contains("\"Overlay/AlertDialogAnatomy.axaml\", BuildAlertDialogAnatomyPreview", source);
        Assert.Contains("\"Overlay/AlertDialogInteraction.axaml\"", source);
        Assert.Contains("BuildAlertDialogInteractionPreview", source);
        Assert.Contains("Code(\"Overlay/AlertDialogInteraction.cs\", \"CSharp/Overlay/AlertDialogInteraction.cs\")", source);
        Assert.Contains("\"Overlay/Sheet.axaml\", BuildSheetPreview", source);
        Assert.Contains("\"Overlay/SheetStates.axaml\", BuildSheetStatesPreview", source);
        Assert.Contains("\"Overlay/SheetAnatomy.axaml\", BuildSheetAnatomyPreview", source);
        Assert.Contains("\"Overlay/SheetInteraction.axaml\"", source);
        Assert.Contains("BuildSheetInteractionPreview", source);
        Assert.Contains("Code(\"Overlay/SheetInteraction.cs\", \"CSharp/Overlay/SheetInteraction.cs\")", source);
        Assert.Contains("\"Overlay/Drawer.axaml\", BuildDrawerPreview", source);
        Assert.Contains("\"Overlay/DrawerStates.axaml\", BuildDrawerStatesPreview", source);
        Assert.Contains("\"Overlay/DrawerAnatomy.axaml\", BuildDrawerAnatomyPreview", source);
        Assert.Contains("\"Overlay/DrawerInteraction.axaml\"", source);
        Assert.Contains("BuildDrawerInteractionPreview", source);
        Assert.Contains("Code(\"Overlay/DrawerInteraction.cs\", \"CSharp/Overlay/DrawerInteraction.cs\")", source);
        Assert.Contains("\"Overlay/CommandDialog.axaml\", BuildCommandDialogPreview", source);
        Assert.Contains("\"Overlay/CommandDialogAnatomy.axaml\", BuildCommandDialogAnatomyPreview", source);
        Assert.Contains("\"Overlay/CommandDialogInteraction.axaml\"", source);
        Assert.Contains("BuildCommandDialogInteractionPreview", source);
        Assert.Contains("Code(\"Overlay/CommandDialogInteraction.cs\", \"CSharp/Overlay/CommandDialogInteraction.cs\")", source);
        Assert.Contains("\"Overlay/Popover.axaml\", BuildPopoverPreview", source);
        Assert.Contains("\"Overlay/PopoverAnatomy.axaml\", BuildPopoverAnatomyPreview", source);
        Assert.Contains("\"Overlay/PopoverInteraction.axaml\"", source);
        Assert.Contains("BuildPopoverInteractionPreview", source);
        Assert.Contains("Code(\"Overlay/PopoverInteraction.cs\", \"CSharp/Overlay/PopoverInteraction.cs\")", source);
        Assert.Contains("\"Overlay/Tooltip.axaml\", BuildTooltipPreview", source);
        Assert.Contains("\"Overlay/TooltipInteraction.axaml\"", source);
        Assert.Contains("BuildTooltipInteractionPreview", source);
        Assert.Contains("Code(\"Overlay/TooltipInteraction.cs\", \"CSharp/Overlay/TooltipInteraction.cs\")", source);
        Assert.Contains("\"Overlay/HoverCard.axaml\", BuildHoverCardPreview", source);
        Assert.Contains("\"Overlay/HoverCardInteraction.axaml\"", source);
        Assert.Contains("BuildHoverCardInteractionPreview", source);
        Assert.Contains("Code(\"Overlay/HoverCardInteraction.cs\", \"CSharp/Overlay/HoverCardInteraction.cs\")", source);
        Assert.Contains("\"DataDisplay/Card.axaml\", BuildCardPreview", source);
        Assert.Contains("\"DataDisplay/CardAnatomy.axaml\", BuildCardAnatomyPreview", source);
        Assert.Contains("\"DataDisplay/CardInteraction.axaml\"", source);
        Assert.Contains("BuildCardInteractionPreview", source);
        Assert.Contains("Code(\"DataDisplay/CardInteraction.cs\", \"CSharp/DataDisplay/CardInteraction.cs\")", source);
        Assert.Contains("\"DataDisplay/Item.axaml\", BuildItemPreview", source);
        Assert.Contains("\"DataDisplay/ItemStates.axaml\", BuildItemStatesPreview", source);
        Assert.Contains("\"DataDisplay/ItemAnatomy.axaml\", BuildItemAnatomyPreview", source);
        Assert.Contains("\"DataDisplay/ItemInteraction.axaml\"", source);
        Assert.Contains("BuildItemInteractionPreview", source);
        Assert.Contains("Code(\"DataDisplay/ItemInteraction.cs\", \"CSharp/DataDisplay/ItemInteraction.cs\")", source);
        Assert.Contains("\"DataDisplay/AspectRatio.axaml\", BuildAspectRatioPreview", source);
        Assert.Contains("\"DataDisplay/AspectRatioStates.axaml\", BuildAspectRatioStatesPreview", source);
        Assert.Contains("\"DataDisplay/AspectRatioAnatomy.axaml\", BuildAspectRatioAnatomyPreview", source);
        Assert.Contains("\"DataDisplay/AspectRatioInteraction.axaml\"", source);
        Assert.Contains("BuildAspectRatioInteractionPreview", source);
        Assert.Contains("Code(\"DataDisplay/AspectRatioInteraction.cs\", \"CSharp/DataDisplay/AspectRatioInteraction.cs\")", source);
        Assert.Contains("\"DataDisplay/Carousel.axaml\", BuildCarouselPreview", source);
        Assert.Contains("\"DataDisplay/CarouselAnatomy.axaml\", BuildCarouselAnatomyPreview", source);
        Assert.Contains("\"DataDisplay/CarouselComposition.axaml\", BuildCarouselCompositionPreview", source);
        Assert.Contains("\"DataDisplay/CarouselInteraction.axaml\"", source);
        Assert.Contains("BuildCarouselInteractionPreview", source);
        Assert.Contains("Code(\"DataDisplay/CarouselInteraction.cs\", \"CSharp/DataDisplay/CarouselInteraction.cs\")", source);
        Assert.Contains("\"DataDisplay/Chart.axaml\", BuildChartPreview", source);
        Assert.Contains("\"DataDisplay/ChartAnatomy.axaml\", BuildChartAnatomyPreview", source);
        Assert.Contains("\"DataDisplay/ChartInteraction.axaml\"", source);
        Assert.Contains("BuildChartInteractionPreview", source);
        Assert.Contains("Code(\"DataDisplay/ChartInteraction.cs\", \"CSharp/DataDisplay/ChartInteraction.cs\")", source);
        Assert.Contains("\"DataDisplay/BarChart.axaml\", BuildBarChartPreview", source);
        Assert.Contains("\"DataDisplay/BarChartAnatomy.axaml\", BuildBarChartAnatomyPreview", source);
        Assert.Contains("\"DataDisplay/BarChartInteraction.axaml\"", source);
        Assert.Contains("BuildBarChartInteractionPreview", source);
        Assert.Contains("Code(\"DataDisplay/BarChartInteraction.cs\", \"CSharp/DataDisplay/BarChartInteraction.cs\")", source);
        Assert.Contains("\"DataDisplay/LineChart.axaml\", BuildLineChartPreview", source);
        Assert.Contains("\"DataDisplay/LineChartAnatomy.axaml\", BuildLineChartAnatomyPreview", source);
        Assert.Contains("\"DataDisplay/LineChartInteraction.axaml\"", source);
        Assert.Contains("BuildLineChartInteractionPreview", source);
        Assert.Contains("Code(\"DataDisplay/LineChartInteraction.cs\", \"CSharp/DataDisplay/LineChartInteraction.cs\")", source);
        Assert.Contains("\"DataDisplay/Metric.axaml\", BuildMetricPreview", source);
        Assert.Contains("\"DataDisplay/MetricAnatomy.axaml\", BuildMetricAnatomyPreview", source);
        Assert.Contains("\"DataDisplay/MetricInteraction.axaml\"", source);
        Assert.Contains("BuildMetricInteractionPreview", source);
        Assert.Contains("Code(\"DataDisplay/MetricInteraction.cs\", \"CSharp/DataDisplay/MetricInteraction.cs\")", source);
        Assert.Contains("\"DataDisplay/ImageIcon.axaml\", BuildImageIconPreview", source);
        Assert.Contains("\"DataDisplay/ImageIconAnatomy.axaml\", BuildImageIconAnatomyPreview", source);
        Assert.Contains("\"DataDisplay/ImageIconInteraction.axaml\"", source);
        Assert.Contains("BuildImageIconInteractionPreview", source);
        Assert.Contains("Code(\"DataDisplay/ImageIconInteraction.cs\", \"CSharp/DataDisplay/ImageIconInteraction.cs\")", source);
        Assert.Contains("\"DataDisplay/ProviderCard.axaml\", BuildProviderCardPreview", source);
        Assert.Contains("\"DataDisplay/ProviderCardAnatomy.axaml\", BuildProviderCardAnatomyPreview", source);
        Assert.Contains("\"DataDisplay/ProviderCardInteraction.axaml\"", source);
        Assert.Contains("BuildProviderCardInteractionPreview", source);
        Assert.Contains("Code(\"DataDisplay/ProviderCardInteraction.cs\", \"CSharp/DataDisplay/ProviderCardInteraction.cs\")", source);
        Assert.Contains("\"DataDisplay/Table.axaml\", BuildTablePreview", source);
        Assert.Contains("\"DataDisplay/TableAnatomy.axaml\", BuildTableAnatomyPreview", source);
        Assert.Contains("\"DataDisplay/TableInteraction.axaml\"", source);
        Assert.Contains("BuildTableInteractionPreview", source);
        Assert.Contains("Code(\"DataDisplay/TableInteraction.cs\", \"CSharp/DataDisplay/TableInteraction.cs\")", source);
        Assert.Contains("\"DataDisplay/DataTable.axaml\", BuildDataTablePreview", source);
        Assert.Contains("\"DataDisplay/DataTableAnatomy.axaml\", BuildDataTableAnatomyPreview", source);
        Assert.Contains("\"DataDisplay/DataTableInteraction.axaml\"", source);
        Assert.Contains("BuildDataTableInteractionPreview", source);
        Assert.Contains("Code(\"DataDisplay/DataTableInteraction.cs\", \"CSharp/DataDisplay/DataTableInteraction.cs\")", source);
        Assert.Contains("\"DataDisplay/PinnedTable.axaml\", BuildPinnedTablePreview", source);
        Assert.Contains("\"DataDisplay/PinnedTableAnatomy.axaml\", BuildPinnedTableAnatomyPreview", source);
        Assert.Contains("\"DataDisplay/PinnedTableInteraction.axaml\"", source);
        Assert.Contains("BuildPinnedTableInteractionPreview", source);
        Assert.Contains("Code(\"DataDisplay/PinnedTableInteraction.cs\", \"CSharp/DataDisplay/PinnedTableInteraction.cs\")", source);
        Assert.Contains("\"DataDisplay/Pagination.axaml\", BuildPaginationPreview", source);
        Assert.Contains("\"DataDisplay/PaginationAnatomy.axaml\", BuildPaginationAnatomyPreview", source);
        Assert.Contains("\"DataDisplay/PaginationInteraction.axaml\"", source);
        Assert.Contains("BuildPaginationInteractionPreview", source);
        Assert.Contains("Code(\"DataDisplay/PaginationInteraction.cs\", \"CSharp/DataDisplay/PaginationInteraction.cs\")", source);
        Assert.Contains("\"DataDisplay/ScrollArea.axaml\", BuildScrollAreaPreview", source);
        Assert.Contains("\"DataDisplay/ScrollAreaAnatomy.axaml\", BuildScrollAreaAnatomyPreview", source);
        Assert.Contains("\"DataDisplay/ScrollAreaInteraction.axaml\"", source);
        Assert.Contains("BuildScrollAreaInteractionPreview", source);
        Assert.Contains("Code(\"DataDisplay/ScrollAreaInteraction.cs\", \"CSharp/DataDisplay/ScrollAreaInteraction.cs\")", source);
        Assert.Contains("\"DataDisplay/RankedBarChart.axaml\", BuildRankedBarChartPreview", source);
        Assert.Contains("\"DataDisplay/RankedBarChartAnatomy.axaml\", BuildRankedBarChartAnatomyPreview", source);
        Assert.Contains("\"DataDisplay/RankedBarChartInteraction.axaml\"", source);
        Assert.Contains("BuildRankedBarChartInteractionPreview", source);
        Assert.Contains("Code(\"DataDisplay/RankedBarChartInteraction.cs\", \"CSharp/DataDisplay/RankedBarChartInteraction.cs\")", source);
        Assert.Contains("\"DataDisplay/UsagePieChart.axaml\", BuildUsagePieChartPreview", source);
        Assert.Contains("\"DataDisplay/UsagePieChartAnatomy.axaml\", BuildUsagePieChartAnatomyPreview", source);
        Assert.Contains("\"DataDisplay/UsagePieChartInteraction.axaml\"", source);
        Assert.Contains("BuildUsagePieChartInteractionPreview", source);
        Assert.Contains("Code(\"DataDisplay/UsagePieChartInteraction.cs\", \"CSharp/DataDisplay/UsagePieChartInteraction.cs\")", source);
        Assert.Contains("\"DataDisplay/UsageTrendChart.axaml\", BuildUsageTrendChartPreview", source);
        Assert.Contains("\"DataDisplay/UsageTrendChartAnatomy.axaml\", BuildUsageTrendChartAnatomyPreview", source);
        Assert.Contains("\"DataDisplay/UsageTrendChartInteraction.axaml\"", source);
        Assert.Contains("BuildUsageTrendChartInteractionPreview", source);
        Assert.Contains("Code(\"DataDisplay/UsageTrendChartInteraction.cs\", \"CSharp/DataDisplay/UsageTrendChartInteraction.cs\")", source);
        Assert.Contains("\"Overview/GettingStartedAnatomy.axaml\", BuildOverviewAnatomyPreview", source);
        Assert.Contains("\"Overview/GettingStartedWorkflow.axaml\", BuildOverviewWorkflowPreview", source);
        Assert.Contains("\"Overview/GettingStartedSource.axaml\", BuildOverviewSourcePreview", source);
        Assert.Contains("\"Layout/ApplicationShellAnatomy.axaml\", BuildApplicationShellAnatomyPreview", source);
        Assert.Contains("\"Layout/SidebarPrimitivesAnatomy.axaml\", BuildSidebarPrimitivesAnatomyPreview", source);
        Assert.Contains("\"Layout/SectionAnatomy.axaml\", BuildSectionComponentAnatomyPreview", source);
        Assert.Contains("\"Primitives/TypographyAnatomy.axaml\", BuildTypographyAnatomyPreview", source);
        Assert.Contains("\"Primitives/FocusRingAnatomy.axaml\", BuildFocusRingAnatomyPreview", source);
        Assert.Contains("\"Primitives/DirectionAnatomy.axaml\", BuildDirectionAnatomyPreview", source);
        Assert.Contains("\"Primitives/OverlayAnatomy.axaml\", BuildOverlayPrimitiveAnatomyPreview", source);
        Assert.Contains("\"Tokens/MotionAnatomy.axaml\", BuildMotionAnatomyPreview", source);
        Assert.Contains("\"Primitives/Typography.axaml\", BuildTypographyPreview", source);
        Assert.Contains("\"Primitives/TypographyInteraction.axaml\", BuildTypographyInteractionPreview", source);
        Assert.Contains("\"Primitives/FocusRing.axaml\", BuildFocusRingPreview", source);
        Assert.Contains("\"Primitives/FocusRingInteraction.axaml\", BuildFocusRingInteractionPreview", source);
        Assert.Contains("\"Primitives/Direction.axaml\", BuildDirectionPreview", source);
        Assert.Contains("\"Primitives/DirectionInteraction.axaml\", BuildDirectionInteractionPreview", source);
        Assert.Contains("\"Primitives/Overlay.axaml\", BuildOverlayPrimitivePreview", source);
        Assert.Contains("\"Primitives/OverlayInteraction.axaml\"", source);
        Assert.Contains("BuildOverlayPrimitiveInteractionPreview", source);
        Assert.Contains("Code(\"Primitives/OverlayInteraction.cs\", \"CSharp/Primitives/OverlayInteraction.cs\")", source);
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
            "Overview/GettingStartedAnatomy.axaml",
            "Overview/GettingStartedWorkflow.axaml",
            "Overview/GettingStartedSource.axaml",
            "Layout/ApplicationShellAnatomy.axaml",
            "Layout/SidebarAnatomy.axaml",
            "Layout/SidebarPrimitivesAnatomy.axaml",
            "Layout/SectionAnatomy.axaml",
            "Forms/ButtonAnatomy.axaml",
            "Layout/ResizableAnatomy.axaml",
            "Forms/ButtonGroupAnatomy.axaml",
            "Forms/ButtonGroupComposition.axaml",
            "Forms/InputGroupAnatomy.axaml",
            "Forms/InputGroupComposition.axaml",
            "Forms/InputOtpAnatomy.axaml",
            "Forms/InputOtpComposition.axaml",
            "Forms/LabelAnatomy.axaml",
            "Forms/LabelComposition.axaml",
            "Forms/SelectAnatomy.axaml",
            "Forms/ComboboxAnatomy.axaml",
            "Forms/NativeSelectComposition.axaml",
            "Forms/CalendarComposition.axaml",
            "Forms/DatePickerAnatomy.axaml",
            "Forms/CheckboxAnatomy.axaml",
            "Forms/RadioAnatomy.axaml",
            "Forms/RadioGroupAnatomy.axaml",
            "Forms/SwitchAnatomy.axaml",
            "Forms/ToggleAnatomy.axaml",
            "Forms/ToggleGroupAnatomy.axaml",
            "Forms/SliderAnatomy.axaml",
            "Forms/FieldGroup.axaml",
            "DataDisplay/CardAnatomy.axaml",
            "DataDisplay/AspectRatioAnatomy.axaml",
            "DataDisplay/ItemAnatomy.axaml",
            "DataDisplay/MetricAnatomy.axaml",
            "DataDisplay/ImageIconAnatomy.axaml",
            "DataDisplay/ProviderCardAnatomy.axaml",
            "DataDisplay/DataTableAnatomy.axaml",
            "DataDisplay/ChartAnatomy.axaml",
            "DataDisplay/BarChartAnatomy.axaml",
            "DataDisplay/LineChartAnatomy.axaml",
            "DataDisplay/CarouselAnatomy.axaml",
            "DataDisplay/ScrollAreaAnatomy.axaml",
            "DataDisplay/PinnedTableAnatomy.axaml",
            "DataDisplay/PaginationAnatomy.axaml",
            "DataDisplay/RankedBarChartAnatomy.axaml",
            "DataDisplay/UsagePieChartAnatomy.axaml",
            "DataDisplay/UsageTrendChartAnatomy.axaml",
            "Navigation/MenubarAnatomy.axaml",
            "Forms/FieldAnatomy.axaml",
            "Feedback/ToastAnatomy.axaml",
            "Navigation/TabsAnatomy.axaml",
            "Navigation/BreadcrumbAnatomy.axaml",
            "Navigation/SideNavAnatomy.axaml",
            "Navigation/SegmentedControlAnatomy.axaml",
            "Navigation/NavigationMenuAnatomy.axaml",
            "Navigation/DropdownButtonAnatomy.axaml",
            "Navigation/CommandAnatomy.axaml",
            "Navigation/AccordionAnatomy.axaml",
            "Navigation/CollapsibleAnatomy.axaml",
            "Navigation/SeparatorAnatomy.axaml",
            "Navigation/KbdAnatomy.axaml",
            "Overlay/DialogAnatomy.axaml",
            "Overlay/AlertDialogAnatomy.axaml",
            "Overlay/SheetAnatomy.axaml",
            "Overlay/DrawerAnatomy.axaml",
            "Overlay/CommandDialogAnatomy.axaml",
            "Overlay/PopoverAnatomy.axaml",
            "Overlay/TooltipAnatomy.axaml",
            "Overlay/HoverCardAnatomy.axaml",
            "Primitives/TypographyAnatomy.axaml",
            "Primitives/FocusRingAnatomy.axaml",
            "Primitives/DirectionAnatomy.axaml",
            "Primitives/OverlayAnatomy.axaml",
            "Tokens/MotionAnatomy.axaml"
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
            "Feedback/SonnerInteraction.axaml",
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
        Assert.Contains("Forms/ButtonGroupAnatomy.axaml", allSamples);
        Assert.Contains("BuildButtonGroupAnatomyPreview", exampleCases);
        Assert.Contains("Forms/ButtonGroupComposition.axaml", allSamples);
        Assert.Contains("BuildButtonGroupCompositionPreview", exampleCases);
        Assert.Contains("Forms/InputGroupAnatomy.axaml", allSamples);
        Assert.Contains("BuildInputGroupAnatomyPreview", exampleCases);
        Assert.Contains("Forms/InputGroupComposition.axaml", allSamples);
        Assert.Contains("BuildInputGroupCompositionPreview", exampleCases);
        Assert.Contains("Forms/InputOtpAnatomy.axaml", allSamples);
        Assert.Contains("BuildInputOtpAnatomyPreview", exampleCases);
        Assert.Contains("Forms/InputOtpComposition.axaml", allSamples);
        Assert.Contains("BuildInputOtpCompositionPreview", exampleCases);
        Assert.Contains("Forms/LabelAnatomy.axaml", allSamples);
        Assert.Contains("BuildLabelAnatomyPreview", exampleCases);
        Assert.Contains("Forms/LabelComposition.axaml", allSamples);
        Assert.Contains("BuildLabelCompositionPreview", exampleCases);
        Assert.Contains("Forms/ComboboxAnatomy.axaml", allSamples);
        Assert.Contains("BuildComboboxAnatomyPreview", exampleCases);
        Assert.Contains("Forms/NativeSelectAnatomy.axaml", allSamples);
        Assert.Contains("BuildNativeSelectAnatomyPreview", exampleCases);
        Assert.Contains("Forms/NativeSelectComposition.axaml", allSamples);
        Assert.Contains("BuildNativeSelectCompositionPreview", exampleCases);
        Assert.Contains("Forms/CalendarAnatomy.axaml", allSamples);
        Assert.Contains("BuildCalendarAnatomyPreview", exampleCases);
        Assert.Contains("Forms/CalendarComposition.axaml", allSamples);
        Assert.Contains("BuildCalendarCompositionPreview", exampleCases);
        Assert.Contains("Forms/DatePickerAnatomy.axaml", allSamples);
        Assert.Contains("BuildDatePickerAnatomyPreview", exampleCases);
        Assert.Contains("DataDisplay/CarouselComposition.axaml", allSamples);
        Assert.Contains("DataDisplay/CarouselAnatomy.axaml", allSamples);
        Assert.Contains("BuildCarouselAnatomyPreview", exampleCases);
        Assert.Contains("BuildCarouselCompositionPreview", exampleCases);
        Assert.Contains("DataDisplay/ChartAnatomy.axaml", allSamples);
        Assert.Contains("BuildChartAnatomyPreview", exampleCases);
        Assert.Contains("DataDisplay/AspectRatioAnatomy.axaml", allSamples);
        Assert.Contains("BuildAspectRatioAnatomyPreview", exampleCases);
        Assert.Contains("Navigation/MenubarAnatomy.axaml", allSamples);
        Assert.Contains("BuildMenubarAnatomyPreview", exampleCases);
        Assert.Contains("Navigation/MenubarComposition.axaml", allSamples);
        Assert.Contains("BuildMenubarCompositionPreview", exampleCases);
        Assert.Contains("BuildFieldAnatomyPreview", exampleCases);
        Assert.Contains("Forms/FieldGroup.axaml", allSamples);
        Assert.Contains("BuildFieldGroupPreview", exampleCases);
        Assert.Contains("BuildToastAnatomyPreview", exampleCases);
        Assert.Contains("BuildApplicationShellInteractionPreview", exampleCases);
        Assert.Contains("BuildSidebarPrimitivesInteractionPreview", exampleCases);
        Assert.Contains("BuildSectionComponentInteractionPreview", exampleCases);
        Assert.Contains("Layout/ResizableAnatomy.axaml", allSamples);
        Assert.Contains("BuildResizableAnatomyPreview", exampleCases);
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
        Assert.Contains("Forms/IconButtonAnatomy.axaml", allSamples);
        Assert.Contains("BuildIconButtonAnatomyPreview", exampleCases);
        Assert.Contains("BuildIconButtonInteractionPreview", exampleCases);
        Assert.Contains("Forms/SplitButtonAnatomy.axaml", allSamples);
        Assert.Contains("BuildSplitButtonAnatomyPreview", exampleCases);
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
        Assert.Contains("Forms/RadioAnatomy.axaml", allSamples);
        Assert.Contains("BuildRadioAnatomyPreview", exampleCases);
        Assert.Contains("BuildRadioInteractionPreview", exampleCases);
        Assert.Contains("BuildRadioGroupAnatomyPreview", exampleCases);
        Assert.Contains("BuildRadioGroupInteractionPreview", exampleCases);
        Assert.Contains("Forms/SwitchAnatomy.axaml", allSamples);
        Assert.Contains("BuildSwitchAnatomyPreview", exampleCases);
        Assert.Contains("BuildSwitchInteractionPreview", exampleCases);
        Assert.Contains("Forms/ToggleAnatomy.axaml", allSamples);
        Assert.Contains("BuildToggleAnatomyPreview", exampleCases);
        Assert.Contains("Forms/TextBoxAnatomy.axaml", allSamples);
        Assert.Contains("BuildTextBoxAnatomyPreview", exampleCases);
        Assert.Contains("Forms/TextareaAnatomy.axaml", allSamples);
        Assert.Contains("BuildTextareaAnatomyPreview", exampleCases);
        Assert.Contains("Forms/ToggleGroup.axaml", allSamples);
        Assert.Contains("\"Forms/ToggleGroup.axaml\", BuildToggleGroupPreview", source);
        Assert.Contains("Forms/ToggleGroupStates.axaml", allSamples);
        Assert.Contains("BuildToggleGroupStatesPreview", exampleCases);
        Assert.Contains("Forms/ToggleGroupAnatomy.axaml", allSamples);
        Assert.Contains("BuildToggleGroupAnatomyPreview", exampleCases);
        Assert.Contains("Forms/ToggleGroupInteraction.axaml", allSamples);
        Assert.Contains("BuildToggleGroupInteractionPreview", exampleCases);
        Assert.Contains("BuildToggleInteractionPreview", exampleCases);
        Assert.Contains("Forms/SliderAnatomy.axaml", allSamples);
        Assert.Contains("BuildSliderAnatomyPreview", exampleCases);
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
        Assert.Contains("\"feedback.avatar-group\"", exampleCases);
        Assert.Contains("Feedback/AvatarGroupAnatomy.axaml", allSamples);
        Assert.Contains("BuildAvatarGroupAnatomyPreview", exampleCases);
        Assert.Contains("Feedback/AvatarGroupInteraction.axaml", allSamples);
        Assert.Contains("BuildAvatarGroupInteractionPreview", exampleCases);
        Assert.Contains("Feedback/EmptyStateAnatomy.axaml", allSamples);
        Assert.Contains("BuildEmptyStateAnatomyPreview", exampleCases);
        Assert.Contains("BuildEmptyStateInteractionPreview", exampleCases);
        Assert.Contains("BuildToastInteractionPreview", exampleCases);
        Assert.Contains("Feedback/SonnerAnatomy.axaml", allSamples);
        Assert.Contains("BuildSonnerAnatomyPreview", exampleCases);
        Assert.Contains("BuildSonnerInteractionPreview", exampleCases);
        Assert.Contains("Feedback/SpinnerAnatomy.axaml", allSamples);
        Assert.Contains("BuildSpinnerAnatomyPreview", exampleCases);
        Assert.Contains("BuildSpinnerInteractionPreview", exampleCases);
        Assert.Contains("Feedback/ProgressAnatomy.axaml", allSamples);
        Assert.Contains("BuildProgressAnatomyPreview", exampleCases);
        Assert.Contains("BuildProgressInteractionPreview", exampleCases);
        Assert.Contains("Feedback/SkeletonAnatomy.axaml", allSamples);
        Assert.Contains("BuildSkeletonAnatomyPreview", exampleCases);
        Assert.Contains("BuildSkeletonInteractionPreview", exampleCases);
        Assert.Contains("BuildNavigationMenuInteractionPreview", exampleCases);
        Assert.Contains("Navigation/NavigationMenuAnatomy.axaml", allSamples);
        Assert.Contains("BuildNavigationMenuAnatomyPreview", exampleCases);
        Assert.Contains("BuildMenubarInteractionPreview", exampleCases);
        Assert.Contains("Navigation/SideNavAnatomy.axaml", allSamples);
        Assert.Contains("BuildSideNavAnatomyPreview", exampleCases);
        Assert.Contains("BuildSideNavInteractionPreview", exampleCases);
        Assert.Contains("Navigation/SegmentedControlAnatomy.axaml", allSamples);
        Assert.Contains("BuildSegmentedControlAnatomyPreview", exampleCases);
        Assert.Contains("BuildSegmentedControlInteractionPreview", exampleCases);
        Assert.Contains("Navigation/DropdownButtonAnatomy.axaml", allSamples);
        Assert.Contains("BuildDropdownAnatomyPreview", exampleCases);
        Assert.Contains("BuildDropdownInteractionPreview", exampleCases);
        Assert.Contains("BuildMenuInteractionPreview", exampleCases);
        Assert.Contains("BuildContextMenuInteractionPreview", exampleCases);
        Assert.Contains("BuildCommandInteractionPreview", exampleCases);
        Assert.Contains("BuildAccordionInteractionPreview", exampleCases);
        Assert.Contains("Navigation/CollapsibleAnatomy.axaml", allSamples);
        Assert.Contains("BuildCollapsibleAnatomyPreview", exampleCases);
        Assert.Contains("BuildCollapsibleInteractionPreview", exampleCases);
        Assert.Contains("Navigation/SeparatorAnatomy.axaml", allSamples);
        Assert.Contains("BuildSeparatorAnatomyPreview", exampleCases);
        Assert.Contains("BuildSeparatorInteractionPreview", exampleCases);
        Assert.Contains("Navigation/KbdAnatomy.axaml", allSamples);
        Assert.Contains("BuildKbdAnatomyPreview", exampleCases);
        Assert.Contains("BuildKbdInteractionPreview", exampleCases);
        Assert.Contains("BuildCommandDialogInteractionPreview", exampleCases);
        Assert.Contains("BuildTooltipInteractionPreview", exampleCases);
        Assert.Contains("Overlay/HoverCardAnatomy.axaml", allSamples);
        Assert.Contains("BuildHoverCardAnatomyPreview", exampleCases);
        Assert.Contains("BuildHoverCardInteractionPreview", exampleCases);
        Assert.Contains("DataDisplay/CardAnatomy.axaml", allSamples);
        Assert.Contains("BuildCardAnatomyPreview", exampleCases);
        Assert.Contains("BuildCardInteractionPreview", exampleCases);
        Assert.Contains("BuildItemStatesPreview", exampleCases);
        Assert.Contains("BuildItemAnatomyPreview", exampleCases);
        Assert.Contains("BuildItemInteractionPreview", exampleCases);
        Assert.Contains("BuildAspectRatioStatesPreview", exampleCases);
        Assert.Contains("BuildAspectRatioInteractionPreview", exampleCases);
        Assert.Contains("DataDisplay/MetricAnatomy.axaml", allSamples);
        Assert.Contains("BuildMetricAnatomyPreview", exampleCases);
        Assert.Contains("BuildMetricInteractionPreview", exampleCases);
        Assert.Contains("DataDisplay/ImageIconAnatomy.axaml", allSamples);
        Assert.Contains("BuildImageIconAnatomyPreview", exampleCases);
        Assert.Contains("BuildImageIconInteractionPreview", exampleCases);
        Assert.Contains("DataDisplay/ProviderCardAnatomy.axaml", allSamples);
        Assert.Contains("BuildProviderCardAnatomyPreview", exampleCases);
        Assert.Contains("BuildProviderCardInteractionPreview", exampleCases);
        Assert.Contains("BuildTableInteractionPreview", exampleCases);
        Assert.Contains("BuildDataTableInteractionPreview", exampleCases);
        Assert.Contains("BuildChartInteractionPreview", exampleCases);
        Assert.Contains("BuildBarChartAnatomyPreview", exampleCases);
        Assert.Contains("BuildBarChartInteractionPreview", exampleCases);
        Assert.Contains("BuildLineChartAnatomyPreview", exampleCases);
        Assert.Contains("BuildLineChartInteractionPreview", exampleCases);
        Assert.Contains("DataDisplay/PinnedTableAnatomy.axaml", allSamples);
        Assert.Contains("BuildPinnedTableAnatomyPreview", exampleCases);
        Assert.Contains("BuildPinnedTableInteractionPreview", exampleCases);
        Assert.Contains("DataDisplay/PaginationAnatomy.axaml", allSamples);
        Assert.Contains("BuildPaginationAnatomyPreview", exampleCases);
        Assert.Contains("BuildPaginationInteractionPreview", exampleCases);
        Assert.Contains("DataDisplay/ScrollAreaAnatomy.axaml", allSamples);
        Assert.Contains("BuildScrollAreaAnatomyPreview", exampleCases);
        Assert.Contains("BuildScrollAreaInteractionPreview", exampleCases);
        Assert.Contains("DataDisplay/RankedBarChartAnatomy.axaml", allSamples);
        Assert.Contains("BuildRankedBarChartAnatomyPreview", exampleCases);
        Assert.Contains("BuildRankedBarChartInteractionPreview", exampleCases);
        Assert.Contains("DataDisplay/UsagePieChartAnatomy.axaml", allSamples);
        Assert.Contains("BuildUsagePieChartAnatomyPreview", exampleCases);
        Assert.Contains("BuildUsagePieChartInteractionPreview", exampleCases);
        Assert.Contains("DataDisplay/UsageTrendChartAnatomy.axaml", allSamples);
        Assert.Contains("BuildUsageTrendChartAnatomyPreview", exampleCases);
        Assert.Contains("BuildUsageTrendChartInteractionPreview", exampleCases);
        Assert.Contains("Overview/GettingStartedAnatomy.axaml", allSamples);
        Assert.Contains("BuildOverviewAnatomyPreview", exampleCases);
        Assert.Contains("Overview/GettingStartedWorkflow.axaml", allSamples);
        Assert.Contains("BuildOverviewWorkflowPreview", exampleCases);
        Assert.Contains("Overview/GettingStartedSource.axaml", allSamples);
        Assert.Contains("BuildOverviewSourcePreview", exampleCases);
        Assert.Contains("Layout/ApplicationShellAnatomy.axaml", allSamples);
        Assert.Contains("BuildApplicationShellAnatomyPreview", exampleCases);
        Assert.Contains("Layout/SidebarPrimitivesAnatomy.axaml", allSamples);
        Assert.Contains("BuildSidebarPrimitivesAnatomyPreview", exampleCases);
        Assert.Contains("Layout/SectionAnatomy.axaml", allSamples);
        Assert.Contains("BuildSectionComponentAnatomyPreview", exampleCases);
        Assert.Contains("Primitives/TypographyAnatomy.axaml", allSamples);
        Assert.Contains("BuildTypographyAnatomyPreview", exampleCases);
        Assert.Contains("Primitives/FocusRingAnatomy.axaml", allSamples);
        Assert.Contains("BuildFocusRingAnatomyPreview", exampleCases);
        Assert.Contains("Primitives/DirectionAnatomy.axaml", allSamples);
        Assert.Contains("BuildDirectionAnatomyPreview", exampleCases);
        Assert.Contains("Primitives/OverlayAnatomy.axaml", allSamples);
        Assert.Contains("BuildOverlayPrimitiveAnatomyPreview", exampleCases);
        Assert.Contains("Tokens/MotionAnatomy.axaml", allSamples);
        Assert.Contains("BuildMotionAnatomyPreview", exampleCases);
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
    public void EveryDocsPageExposesAtLeastFourInlineCodeExamples()
    {
        var source = ReadDocsSource("MainWindow.cs");
        var exampleCases = ExtractMethod(source, "ExampleCasesFor");

        foreach (var pageId in ExtractPageIds(source))
        {
            var caseCount = 1 + CountExamplesForPage(exampleCases, pageId);

            Assert.True(
                caseCount >= 4,
                $"Expected {pageId} to expose at least four rendered examples with local AXAML code reveal, found {caseCount}.");
        }
    }

    [Fact]
    public void DefaultDocsSamplesAndPreviewsDoNotForceDisclosureSurfacesOpen()
    {
        var source = ReadDocsSource("MainWindow.cs");
        var forbiddenAxaml = new[]
        {
            "IsOpen=\"True\"",
            "IsDropDownOpen=\"True\"",
            "IsSubMenuOpen=\"True\"",
            "IsExpanded=\"True\"",
            "Classes=\"context-menu-open\""
        };

        foreach (var sample in ExtractSamplePaths(source))
        {
            var path = Path.Combine(DocsRoot(), "Examples", "Axaml", sample.Replace('/', Path.DirectorySeparatorChar));
            var text = File.ReadAllText(path);

            foreach (var snippet in forbiddenAxaml)
            {
                Assert.DoesNotContain(snippet, text);
            }
        }

        var defaultPreviewMethods = new[]
        {
            "BuildSplitButtonPreview",
            "BuildNavigationMenuPreview",
            "BuildMenubarPreview",
            "BuildDropdownPreview",
            "BuildContextMenuPreview",
            "BuildCollapsiblePreview",
            "BuildAccordionPreview",
            "BuildDialogPreview",
            "BuildAlertDialogPreview",
            "BuildSheetPreview",
            "BuildDrawerPreview",
            "BuildCommandDialogPreview",
            "BuildPopoverPreview",
            "BuildTooltipPreview",
            "BuildHoverCardPreview",
            "BuildChartPreview",
            "BuildBarChartPreview",
            "BuildLineChartPreview",
            "BuildOverlayPrimitivePreview"
        };
        var forbiddenCode = new[]
        {
            "IsOpen = true",
            "IsDropDownOpen = true",
            "IsSubMenuOpen = true",
            ".ActivateItem(",
            ".OpenMenu(",
            "Classes.Add(\"context-menu-open\")"
        };

        foreach (var methodName in defaultPreviewMethods)
        {
            var method = ExtractMethod(source, methodName);
            foreach (var snippet in forbiddenCode)
            {
                Assert.DoesNotContain(snippet, method);
            }
        }
    }

    [Fact]
    public void FormInteractionDocsDoNotStartPopupExamplesOpen()
    {
        var source = ReadDocsSource("MainWindow.cs");
        var methods = new[]
        {
            "BuildSplitButtonInteractionPreview",
            "BuildSelectInteractionPreview",
            "BuildComboboxInteractionPreview",
            "BuildDatePickerInteractionPreview"
        };
        var forbiddenCode = new[]
        {
            "IsOpen = true,",
            "IsDropDownOpen = true,",
            "isOpen: true"
        };

        foreach (var methodName in methods)
        {
            var method = ExtractMethod(source, methodName);
            foreach (var snippet in forbiddenCode)
            {
                Assert.DoesNotContain(snippet, method);
            }
        }

        var docsRoot = DocsRoot();
        var csharpSamples = new[]
        {
            "SelectInteraction.cs",
            "ComboboxInteraction.cs",
            "SplitButtonInteraction.cs",
            "DatePickerInteraction.cs"
        };
        foreach (var sample in csharpSamples)
        {
            var text = File.ReadAllText(Path.Combine(docsRoot, "Examples", "CSharp", "Forms", sample));
            Assert.DoesNotContain("IsOpen = true,", text);
            Assert.DoesNotContain("IsDropDownOpen = true,", text);
        }

        var axamlSamples = new[]
        {
            "SelectInteraction.axaml",
            "ComboboxInteraction.axaml",
            "SplitButtonInteraction.axaml",
            "DatePickerInteraction.axaml"
        };
        foreach (var sample in axamlSamples)
        {
            var text = File.ReadAllText(Path.Combine(docsRoot, "Examples", "Axaml", "Forms", sample));
            Assert.DoesNotContain("IsOpen=\"True\"", text);
            Assert.DoesNotContain("IsDropDownOpen=\"True\"", text);
        }
    }

    [Fact]
    public void NavigationInteractionDocsDoNotStartDisclosureExamplesOpen()
    {
        var source = ReadDocsSource("MainWindow.cs");
        var methods = new[]
        {
            "BuildDropdownInteractionPreview",
            "BuildAccordionInteractionPreview",
            "BuildCollapsibleInteractionPreview",
            "BuildNavigationMenuInteractionPreview",
            "BuildMenubarInteractionPreview",
            "BuildMenuInteractionPreview",
            "BuildContextMenuInteractionPreview",
            "BuildBreadcrumbInteractionPreview"
        };
        var forbiddenCode = new[]
        {
            "IsOpen = true,",
            "IsSubMenuOpen = true,",
            ".Classes.Add(\"context-menu-open\")"
        };

        foreach (var methodName in methods)
        {
            var method = ExtractMethod(source, methodName);
            foreach (var snippet in forbiddenCode)
            {
                Assert.DoesNotContain(snippet, method);
            }
        }

        var docsRoot = DocsRoot();
        var csharpSamples = new[]
        {
            "DropdownButtonInteraction.cs",
            "AccordionInteraction.cs",
            "CollapsibleInteraction.cs",
            "NavigationMenuInteraction.cs",
            "MenubarInteraction.cs",
            "MenuInteraction.cs",
            "ContextMenuInteraction.cs",
            "BreadcrumbInteraction.cs"
        };
        foreach (var sample in csharpSamples)
        {
            var text = File.ReadAllText(Path.Combine(docsRoot, "Examples", "CSharp", "Navigation", sample));
            Assert.DoesNotContain("IsOpen = true,", text);
            Assert.DoesNotContain("IsSubMenuOpen = true,", text);
            Assert.DoesNotContain("Classes.Add(\"context-menu-open\")", text);
            Assert.DoesNotContain("\n        menu.ActivateItem(overview);", text);
            Assert.DoesNotContain("\n        menu.ActivateItem(components);", text);
            Assert.DoesNotContain("\n        vertical.ActivateItem(verticalSelected);", text);
            Assert.DoesNotContain("\n        menubar.OpenMenu(file);", text);
            Assert.DoesNotContain("contextMenu.Classes.Add(\"context-menu-open\");", text);
            Assert.DoesNotContain("leftMenu.Classes.Add(\"context-menu-open\");", text);
        }

        var axamlSamples = new[]
        {
            "DropdownButtonInteraction.axaml",
            "AccordionInteraction.axaml",
            "CollapsibleInteraction.axaml",
            "NavigationMenuInteraction.axaml",
            "MenubarInteraction.axaml",
            "MenuInteraction.axaml",
            "ContextMenuInteraction.axaml",
            "BreadcrumbInteraction.axaml"
        };
        foreach (var sample in axamlSamples)
        {
            var text = File.ReadAllText(Path.Combine(docsRoot, "Examples", "Axaml", "Navigation", sample));
            Assert.DoesNotContain("IsOpen=\"True\"", text);
            Assert.DoesNotContain("IsSubMenuOpen=\"True\"", text);
            Assert.DoesNotContain("Classes=\"context-menu-open\"", text);
        }
    }

    [Fact]
    public void OverlayInteractionDocsDoNotStartLayerExamplesOpen()
    {
        var source = ReadDocsSource("MainWindow.cs");
        var methods = new[]
        {
            "BuildDialogInteractionPreview",
            "BuildAlertDialogInteractionPreview",
            "BuildPopoverInteractionPreview",
            "BuildSheetInteractionPreview",
            "BuildDrawerInteractionPreview",
            "BuildCommandDialogInteractionPreview",
            "BuildTooltipInteractionPreview",
            "BuildHoverCardInteractionPreview"
        };

        foreach (var methodName in methods)
        {
            var method = ExtractMethod(source, methodName);
            Assert.DoesNotContain("IsOpen = true,", method);
        }

        var docsRoot = DocsRoot();
        var csharpSamples = new[]
        {
            "DialogInteraction.cs",
            "AlertDialogInteraction.cs",
            "PopoverInteraction.cs",
            "SheetInteraction.cs",
            "DrawerInteraction.cs",
            "CommandDialogInteraction.cs",
            "TooltipInteraction.cs",
            "HoverCardInteraction.cs"
        };
        foreach (var sample in csharpSamples)
        {
            var text = File.ReadAllText(Path.Combine(docsRoot, "Examples", "CSharp", "Overlay", sample));
            Assert.DoesNotContain("IsOpen = true,", text);
        }

        var axamlSamples = new[]
        {
            "DialogInteraction.axaml",
            "AlertDialogInteraction.axaml",
            "PopoverInteraction.axaml",
            "SheetInteraction.axaml",
            "DrawerInteraction.axaml",
            "CommandDialogInteraction.axaml",
            "TooltipInteraction.axaml",
            "HoverCardInteraction.axaml"
        };
        foreach (var sample in axamlSamples)
        {
            var text = File.ReadAllText(Path.Combine(docsRoot, "Examples", "Axaml", "Overlay", sample));
            Assert.DoesNotContain("IsOpen=\"True\"", text);
        }
    }

    [Fact]
    public void FeedbackDataAndPrimitiveInteractionDocsDoNotStartTransientExamplesOpen()
    {
        var source = ReadDocsSource("MainWindow.cs");

        var toastMethod = ExtractMethod(source, "BuildToastInteractionPreview");
        Assert.DoesNotContain("IsOpen = true,", toastMethod);
        Assert.Contains("IsOpen = false,", toastMethod);
        Assert.Contains("primaryToast.IsOpen = true;", toastMethod);

        var sonnerMethod = ExtractMethod(source, "BuildSonnerInteractionPreview");
        Assert.Contains("CodexSonnerService.Clear();", sonnerMethod);
        Assert.DoesNotContain("\n        CodexSonnerService.Success", sonnerMethod);
        Assert.DoesNotContain("\n        CodexSonnerService.Warning", sonnerMethod);
        Assert.DoesNotContain("\n        CodexSonnerService.Loading", sonnerMethod);
        Assert.Contains("success.Click", sonnerMethod);
        Assert.Contains("loading.Click", sonnerMethod);

        var chartMethod = ExtractMethod(source, "BuildChartInteractionPreview");
        Assert.DoesNotContain("var tooltipOpen = true;", chartMethod);
        Assert.DoesNotContain("ChartTooltip(\"Current slice\", true", chartMethod);
        Assert.Contains("var tooltipOpen = false;", chartMethod);

        var overlayMethod = ExtractMethod(source, "BuildOverlayPrimitiveInteractionPreview");
        Assert.DoesNotContain("IsOpen = true,", overlayMethod);
        Assert.Contains("IsOpen = false,", overlayMethod);
        Assert.Contains("overlay.IsOpen = true;", overlayMethod);

        var docsRoot = DocsRoot();
        var feedbackCSharp = new[]
        {
            "ToastInteraction.cs",
            "SonnerInteraction.cs"
        };
        foreach (var sample in feedbackCSharp)
        {
            var text = File.ReadAllText(Path.Combine(docsRoot, "Examples", "CSharp", "Feedback", sample));
            Assert.DoesNotContain("IsOpen = true,", text);
            Assert.DoesNotContain("\n    CodexSonnerService.Success", text);
            Assert.DoesNotContain("\n    CodexSonnerService.Warning", text);
            Assert.DoesNotContain("\n    CodexSonnerService.Loading", text);
        }

        var chartCSharp = File.ReadAllText(Path.Combine(docsRoot, "Examples", "CSharp", "DataDisplay", "ChartInteraction.cs"));
        Assert.DoesNotContain("var tooltipOpen = true;", chartCSharp);
        Assert.DoesNotContain("ChartTooltip(\"Current slice\", true", chartCSharp);

        var primitiveOverlayCSharp = File.ReadAllText(Path.Combine(docsRoot, "Examples", "CSharp", "Primitives", "OverlayInteraction.cs"));
        Assert.DoesNotContain("IsOpen = true,", primitiveOverlayCSharp);

        var axamlSamples = new[]
        {
            Path.Combine("Feedback", "ToastInteraction.axaml"),
            Path.Combine("Feedback", "SonnerInteraction.axaml"),
            Path.Combine("DataDisplay", "ChartInteraction.axaml"),
            Path.Combine("DataDisplay", "DataTableInteraction.axaml"),
            Path.Combine("Primitives", "OverlayInteraction.axaml")
        };
        foreach (var sample in axamlSamples)
        {
            var text = File.ReadAllText(Path.Combine(docsRoot, "Examples", "Axaml", sample));
            Assert.DoesNotContain("IsOpen=\"True\"", text);
        }
    }

    [Fact]
    public void DocsPageModelCarriesNavigationPreviewAndSectionContracts()
    {
        var model = ReadDocsSource(Path.Combine("Docs", "DocsPage.cs"));
        var mainWindow = ReadDocsSource("MainWindow.cs");

        Assert.Contains("internal sealed record DocsCategory(string Title, IReadOnlyList<DocsPage> Pages);", model);
        Assert.Contains("internal sealed record DocsCodeSnippet(string Title, string SamplePath);", model);
        Assert.Contains("string Id", model);
        Assert.Contains("string Category", model);
        Assert.Contains("string SamplePath", model);
        Assert.Contains("Func<Control> BuildPreview", model);
        Assert.Contains("internal sealed record DocsExampleCase(", model);
        Assert.Contains("string Title", model);
        Assert.Contains("string Description", model);
        Assert.Contains("IReadOnlyList<DocsCodeSnippet>? AdditionalCodeSamples", model);
        Assert.Contains("public IReadOnlyList<DocsCodeSnippet> CodeSamples", model);
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
        Assert.Contains("State(\"Command blocked\", \"Trigger\"", stateCases);
        Assert.Contains("\"layout.sidebar-primitives\"", stateCases);
        Assert.Contains("\"layout.resizable\"", stateCases);
        Assert.Contains("State(\"With handle\", \"Handle\"", stateCases);
        Assert.Contains("State(\"Loading\", \"In-flight action\"", stateCases);
        Assert.Contains("State(\"Focus-visible\", \"Keyboard focus\"", stateCases);
        Assert.Contains("\"forms.input-otp\"", stateCases);
        Assert.Contains("State(\"Pattern\", \"Root\"", stateCases);
        Assert.Contains("\"forms.select\"", stateCases);
        Assert.Contains("ValueChanged and OpenChanged report Pointer, Keyboard, or Programmatic source", stateCases);
        Assert.Contains("\"forms.native-select\"", stateCases);
        Assert.Contains("ValueChanged and OpenChanged report Pointer, Keyboard, or Programmatic option/popup source", stateCases);
        Assert.Contains("\"forms.combobox\"", stateCases);
        Assert.Contains("State(\"Highlighted\", \"Item\"", stateCases);
        Assert.Contains("SelectionChanged and OpenChanged include source metadata", stateCases);
        Assert.Contains("State(\"OptGroup\", \"List\"", stateCases);
        Assert.Contains("\"forms.calendar\"", stateCases);
        Assert.Contains("State(\"Range\", \"Day grid\"", stateCases);
        Assert.Contains("Command CanExecute=false applies command-blocked before day activation", stateCases);
        Assert.Contains("SelectedDateChanged, RangeChanged, DisplayDateChanged, and ActiveDateChanged report Pointer, Keyboard, or Programmatic source", stateCases);
        Assert.Contains("\"forms.date-picker\"", stateCases);
        Assert.Contains("State(\"Open\", \"Popover\"", stateCases);
        Assert.Contains("OpenChanged, SelectedDateChanged, and RangeChanged report Pointer, Keyboard, or Programmatic source", stateCases);
        Assert.Contains("\"forms.toggle-group\"", stateCases);
        Assert.Contains("ValueChanged reports Pointer, Keyboard, or Programmatic toggle source", stateCases);
        Assert.Contains("\"forms.toggle\"", stateCases);
        Assert.Contains("PressedChanged reports Pointer, Keyboard, or Programmatic pressed source", stateCases);
        Assert.Contains("\"forms.checkbox\"", stateCases);
        Assert.Contains("CheckedStateChanged reports Pointer, Keyboard, or Programmatic checked source", stateCases);
        Assert.Contains("\"forms.switch\"", stateCases);
        Assert.Contains("CheckedChanged reports Pointer, Keyboard, or Programmatic checked source", stateCases);
        Assert.Contains("\"forms.radio-group\"", stateCases);
        Assert.Contains("State(\"Value\", \"Root\"", stateCases);
        Assert.Contains("\"forms.icon-button\"", stateCases);
        Assert.Contains("State(\"FieldGroup\", \"Group\"", stateCases);
        Assert.Contains("State(\"FieldError\", \"Validation\"", stateCases);
        Assert.Contains("\"navigation.tabs\"", stateCases);
        Assert.Contains("State(\"Selected\", \"Tab item\"", stateCases);
        Assert.Contains("State(\"Source\", \"Event\"", stateCases);
        Assert.Contains("\"navigation.accordion\"", stateCases);
        Assert.Contains("State(\"Multiple\", \"Root\"", stateCases);
        Assert.Contains("\"navigation.menubar\"", stateCases);
        Assert.Contains("State(\"Checkbox\", \"Item\"", stateCases);
        Assert.Contains("\"navigation.command\"", stateCases);
        Assert.Contains("State(\"Search\", \"Root\"", stateCases);
        Assert.Contains("State(\"Separator\", \"CommandSeparator\"", stateCases);
        Assert.Contains("Command CanExecute=false applies command-blocked and removes the item from active selection", stateCases);
        Assert.Contains("\"navigation.side-nav\"", stateCases);
        Assert.Contains("Command CanExecute=false applies command-blocked and preserves the root value", stateCases);
        Assert.Contains("\"navigation.segmented-control\"", stateCases);
        Assert.Contains("Command CanExecute=false applies command-blocked while keeping selection controlled", stateCases);
        Assert.Contains("\"overlay.dialog\"", stateCases);
        Assert.Contains("State(\"Restore focus\", \"Trigger\"", stateCases);
        Assert.Contains("\"overlay.alert-dialog\"", stateCases);
        Assert.Contains("State(\"Cancel focus\", \"Least destructive action\"", stateCases);
        Assert.Contains("\"feedback.empty-state\"", stateCases);
        Assert.Contains("Primary and secondary ActionCommand CanExecute=false expose command-blocked action classes", stateCases);
        Assert.Contains("\"feedback.sonner\"", stateCases);
        Assert.Contains("State(\"Queue\", \"Service\"", stateCases);
        Assert.Contains("Non-loading toasts start a duration timer, while loading toasts stay open until host dismissal", stateCases);
        Assert.Contains("VisibleToasts, Expand, Gap, Offset, and Position own the viewport stack", stateCases);
        Assert.Contains("\"overlay.drawer\"", stateCases);
        Assert.Contains("State(\"Drag ready\", \"Gesture\"", stateCases);
        Assert.Contains("\"data.pagination\"", stateCases);
        Assert.Contains("State(\"Ellipsis\", \"Page item\"", stateCases);
        Assert.Contains("Command CanExecute=false applies command-blocked before page activation", stateCases);
        Assert.Contains("\"data.carousel\"", stateCases);
        Assert.Contains("\"data.item\"", stateCases);
        Assert.Contains("ActivateCommand CanExecute=false applies command-blocked and suppresses row activation", stateCases);
        Assert.Contains("\"data.provider-card\"", stateCases);
        Assert.Contains("State(\"Command blocked\", \"Row\"", stateCases);
        Assert.Contains("\"data.aspect-ratio\"", stateCases);
        Assert.Contains("State(\"Actions\", \"Trailing slot\"", stateCases);
        Assert.Contains("State(\"Fit mode\", \"Measure\"", stateCases);
        Assert.Contains("State(\"Loop\", \"Root\"", stateCases);
        Assert.Contains("at-start, at-end, previous-disabled, and next-disabled classes follow SelectedIndex before commands run", stateCases);
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
        Assert.Contains("Trigger and rail commands update can-toggle and command-blocked before ToggleOpen", eventCases);
        Assert.Contains("\"layout.sidebar-primitives\"", eventCases);
        Assert.Contains("\"layout.resizable\"", eventCases);
        Assert.Contains("Event(\"Primary drag\"", eventCases);
        Assert.Contains("Event(\"Pointer released\"", eventCases);
        Assert.Contains("Event(\"Space / Enter\"", eventCases);
        Assert.Contains("\"forms.input-otp\"", eventCases);
        Assert.Contains("Event(\"Paste text\"", eventCases);
        Assert.Contains("\"forms.select\"", eventCases);
        Assert.Contains("OpenChanged with pointer, keyboard, or programmatic source metadata", eventCases);
        Assert.Contains("SelectedIndex changes from host code emit source=Programmatic", eventCases);
        Assert.Contains("\"forms.native-select\"", eventCases);
        Assert.Contains("OpenChanged with pointer, keyboard, or programmatic source metadata", eventCases);
        Assert.Contains("SelectedIndex or SelectedItem changes from host code emit source=Programmatic", eventCases);
        Assert.Contains("\"forms.combobox\"", eventCases);
        Assert.Contains("source=Input", eventCases);
        Assert.Contains("source=Item", eventCases);
        Assert.Contains("Event(\"Arrow / Home / End\"", eventCases);
        Assert.Contains("Event(\"Option selection\"", eventCases);
        Assert.Contains("\"forms.calendar\"", eventCases);
        Assert.Contains("Event(\"PageUp / PageDown\"", eventCases);
        Assert.Contains("source=Pointer", eventCases);
        Assert.Contains("Day button commands update can-activate and command-blocked before SelectedDateChanged", eventCases);
        Assert.Contains("\"forms.date-picker\"", eventCases);
        Assert.Contains("Event(\"Backspace / Delete\"", eventCases);
        Assert.Contains("Clears the selected date or range with source=Keyboard", eventCases);
        Assert.Contains("\"forms.toggle-group\"", eventCases);
        Assert.Contains("Toggles the item with source=Pointer", eventCases);
        Assert.Contains("source=Keyboard", eventCases);
        Assert.Contains("\"forms.toggle\"", eventCases);
        Assert.Contains("PressedChanged\", \"Raises normalized old/new booleans plus Pointer, Keyboard, or Programmatic source", eventCases);
        Assert.Contains("\"forms.checkbox\"", eventCases);
        Assert.Contains("CheckedStateChanged\", \"Raises old/new bool? values plus Pointer, Keyboard, or Programmatic source", eventCases);
        Assert.Contains("\"forms.switch\"", eventCases);
        Assert.Contains("CheckedChanged\", \"Raises normalized old/new booleans plus Pointer, Keyboard, or Programmatic source", eventCases);
        Assert.Contains("\"forms.radio-group\"", eventCases);
        Assert.Contains("\"navigation.breadcrumb\"", eventCases);
        Assert.Contains("Command CanExecute=false applies command-blocked and suppresses LinkActivated", eventCases);
        Assert.Contains("\"navigation.accordion\"", eventCases);
        Assert.Contains("Event(\"ValueChanged\"", eventCases);
        Assert.Contains("ValueChanged\", \"Raises with old/new selected item, index, value, and source like Web onValueChange", eventCases);
        Assert.Contains("Event(\"Primary pointer release\"", eventCases);
        Assert.Contains("source=Programmatic", eventCases);
        Assert.Contains("\"forms.icon-button\"", eventCases);
        Assert.Contains("Event(\"Errors changed\"", eventCases);
        Assert.Contains("\"navigation.dropdown\"", eventCases);
        Assert.Contains("source=Pointer", eventCases);
        Assert.Contains("source=Selection", eventCases);
        Assert.Contains("Event(\"Escape\"", eventCases);
        Assert.Contains("\"overlay.alert-dialog\"", eventCases);
        Assert.Contains("Event(\"Cancel / action\"", eventCases);
        Assert.Contains("Host command CanExecuteChanged updates cancel and action button availability immediately", eventCases);
        Assert.Contains("\"feedback.badge\"", eventCases);
        Assert.Contains("Event(\"Primary pointer\"", eventCases);
        Assert.Contains("reports Pointer source", eventCases);
        Assert.Contains("reports Keyboard source", eventCases);
        Assert.Contains("TryActivate reports Programmatic source", eventCases);
        Assert.Contains("Event(\"CanExecute\"", eventCases);
        Assert.Contains("\"overlay.command-dialog\"", eventCases);
        Assert.Contains("Emits the selected command item, value, and source metadata before close-on-select dismissal", eventCases);
        Assert.Contains("\"feedback.empty-state\"", eventCases);
        Assert.Contains("Action commands update can-action, can-secondary-action, and command-blocked classes before requests", eventCases);
        Assert.Contains("\"feedback.sonner\"", eventCases);
        Assert.Contains("Event(\"Show\"", eventCases);
        Assert.Contains("insert newest first, and start timers when duration is nonzero", eventCases);
        Assert.Contains("Event(\"Limit trim\"", eventCases);
        Assert.Contains("Loading toasts default to zero duration and remain mounted until explicit dismissal", eventCases);
        Assert.Contains("\"data.provider-card\"", eventCases);
        Assert.Contains("reports Pointer source", eventCases);
        Assert.Contains("reports Keyboard source metadata", eventCases);
        Assert.Contains("TrySelect reports Programmatic source", eventCases);
        Assert.Contains("Command CanExecuteChanged updates can-select and command-blocked before row selection", eventCases);
        Assert.Contains("\"overlay.drawer\"", eventCases);
        Assert.Contains("Event(\"Primary handle drag\"", eventCases);
        Assert.Contains("Event(\"Primary drag release\"", eventCases);
        Assert.Contains("\"navigation.accordion\"", eventCases);
        Assert.Contains("Event(\"Programmatic state\"", eventCases);
        Assert.Contains("\"navigation.menubar\"", eventCases);
        Assert.Contains("Event(\"Enter / Space / Down\"", eventCases);
        Assert.Contains("\"navigation.command\"", eventCases);
        Assert.Contains("Event(\"Search text\"", eventCases);
        Assert.Contains("Event(\"Pointer enter\"", eventCases);
        Assert.Contains("Command item CanExecuteChanged updates can-select and command-blocked before ItemSelected", eventCases);
        Assert.Contains("content links use the same primary-release activation helper", eventCases);
        Assert.Contains("Event(\"Command blocked\"", eventCases);
        Assert.Contains("\"navigation.side-nav\"", eventCases);
        Assert.Contains("Command CanExecuteChanged updates can-select and command-blocked before row selection", eventCases);
        Assert.Contains("\"navigation.segmented-control\"", eventCases);
        Assert.Contains("Command-backed segments update can-select and command-blocked without moving the indicator", eventCases);
        Assert.Contains("\"data.pagination\"", eventCases);
        Assert.Contains("Event(\"Right / PageDown\"", eventCases);
        Assert.Contains("Page item commands update can-activate and command-blocked before PageChanged", eventCases);
        Assert.Contains("\"data.carousel\"", eventCases);
        Assert.Contains("\"data.item\"", eventCases);
        Assert.Contains("ActivateCommand CanExecuteChanged updates can-activate and command-blocked before suppressing Activated", eventCases);
        Assert.Contains("\"data.aspect-ratio\"", eventCases);
        Assert.Contains("Event(\"Enter / Space\"", eventCases);
        Assert.Contains("Event(\"Ratio changed\"", eventCases);
        Assert.Contains("Event(\"Loop edge\"", eventCases);
        Assert.Contains("Updates previous-disabled and next-disabled before suppressing unavailable moves when Loop is false", eventCases);
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

    private static IReadOnlyList<string> ExtractPageIds(string source)
    {
        var ids = new List<string>();
        const string marker = "Page(\"";
        var index = 0;
        while ((index = source.IndexOf(marker, index, StringComparison.Ordinal)) >= 0)
        {
            var idStart = index + marker.Length;
            var idEnd = source.IndexOf('"', idStart);
            Assert.True(idEnd >= 0, "Could not find page id closing quote.");
            ids.Add(source[idStart..idEnd]);
            index = idEnd + 1;
        }

        return ids;
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

    private static int CountExamplesForPage(string exampleCases, string pageId)
    {
        var key = $"\"{pageId}\" =>";
        var keyIndex = exampleCases.IndexOf(key, StringComparison.Ordinal);
        if (keyIndex < 0)
        {
            return 0;
        }

        var blockEnd = exampleCases.IndexOf("\n            ],", keyIndex, StringComparison.Ordinal);
        Assert.True(blockEnd >= 0, $"Could not find end of examples for {pageId}.");

        var block = exampleCases[keyIndex..blockEnd];
        const string marker = "Example(";
        var count = 0;
        var index = 0;
        while ((index = block.IndexOf(marker, index, StringComparison.Ordinal)) >= 0)
        {
            count++;
            index += marker.Length;
        }

        return count;
    }

    private static void AssertFormsCompanion(string mainWindow, string fileName, params string[] expectedSnippets)
    {
        Assert.Contains($"Code(\"Forms/{fileName}\", \"CSharp/Forms/{fileName}\")", mainWindow);

        var path = Path.Combine(DocsRoot(), "Examples", "CSharp", "Forms", fileName);
        Assert.True(File.Exists(path), $"Expected Forms companion C# interaction sample: {fileName}");

        var source = File.ReadAllText(path);
        foreach (var snippet in expectedSnippets)
        {
            Assert.Contains(snippet, source);
        }
    }

    private static void AssertFeedbackCompanion(string mainWindow, string fileName, params string[] expectedSnippets)
    {
        Assert.Contains($"Code(\"Feedback/{fileName}\", \"CSharp/Feedback/{fileName}\")", mainWindow);

        var path = Path.Combine(DocsRoot(), "Examples", "CSharp", "Feedback", fileName);
        Assert.True(File.Exists(path), $"Expected Feedback companion C# interaction sample: {fileName}");

        var source = File.ReadAllText(path);
        foreach (var snippet in expectedSnippets)
        {
            Assert.Contains(snippet, source);
        }
    }

    private static void AssertNavigationCompanion(string mainWindow, string fileName, params string[] expectedSnippets)
    {
        Assert.Contains($"Code(\"Navigation/{fileName}\", \"CSharp/Navigation/{fileName}\")", mainWindow);

        var path = Path.Combine(DocsRoot(), "Examples", "CSharp", "Navigation", fileName);
        Assert.True(File.Exists(path), $"Expected Navigation companion C# interaction sample: {fileName}");

        var source = File.ReadAllText(path);
        foreach (var snippet in expectedSnippets)
        {
            Assert.Contains(snippet, source);
        }
    }

    private static void AssertPrimitivesCompanion(string mainWindow, string fileName, params string[] expectedSnippets)
    {
        Assert.Contains($"Code(\"Primitives/{fileName}\", \"CSharp/Primitives/{fileName}\")", mainWindow);

        var path = Path.Combine(DocsRoot(), "Examples", "CSharp", "Primitives", fileName);
        Assert.True(File.Exists(path), $"Expected Primitives companion C# interaction sample: {fileName}");

        var source = File.ReadAllText(path);
        foreach (var snippet in expectedSnippets)
        {
            Assert.Contains(snippet, source);
        }
    }

    private static void AssertTokensCompanion(string mainWindow, string fileName, params string[] expectedSnippets)
    {
        Assert.Contains($"Code(\"Tokens/{fileName}\", \"CSharp/Tokens/{fileName}\")", mainWindow);

        var path = Path.Combine(DocsRoot(), "Examples", "CSharp", "Tokens", fileName);
        Assert.True(File.Exists(path), $"Expected Tokens companion C# interaction sample: {fileName}");

        var source = File.ReadAllText(path);
        foreach (var snippet in expectedSnippets)
        {
            Assert.Contains(snippet, source);
        }
    }

    private static string FindRepositoryRoot()
    {
        return TestRepository.FindRoot();
    }
}
