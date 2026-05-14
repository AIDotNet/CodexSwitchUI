using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;
using CodexSwitchUI.Controls;
using CodexSwitchUI.ECharts.Abstractions;
using CodexSwitchUI.ECharts.Controls;
using CodexSwitchUI.ECharts.Models;
using CodexSwitchUI.ECharts.Themes;
using CodexSwitchUI.Primitives;
using CodexSwitchUI.Themes;
using CodexSwitchUI.Tokens;

namespace CodexSwitchUI.Docs;

public sealed class MainWindow : Window
{
    private const double CoverageMatrixMinHeight = 320;
    private const double CoverageMatrixMaxHeight = 420;

    private static readonly NavCategory[] Categories =
    [
        new("Overview", ["Getting started", "Component map", "Motion baseline"]),
        new("Tokens", ["Color", "Typography", "Density", "Motion tokens"]),
        new("Forms", ["Button", "TextBox", "Select", "Checkbox / Radio", "Switch / Slider"]),
        new("Navigation", ["Tabs", "Navigation menu", "Collapsible", "Menu", "Context menu", "Command", "Side nav"]),
        new("Overlay", ["Dialog", "Popover", "Overlay", "Focus ring", "Open / closed"]),
        new("Feedback", ["Sonner", "Toast", "Badge", "Avatar", "Spinner", "Progress", "Skeleton", "Loading"]),
        new("Data Display", ["Avatar", "Table", "Ranked chart", "Card", "Separator", "Typography"]),
        new("Utilities", ["Card", "Separator", "Typography", "ECharts theme"]),
        new("Design Review", ["Strengths", "Motion check", "Next steps"])
    ];

    private readonly Border _sidebar = new();
    private readonly Border _topbar = new();
    private readonly ScrollViewer _scroll = new();
    private readonly Dictionary<string, CodexSidebarMenuButton> _navItemsByCategory = new(StringComparer.Ordinal);
    private CodexSwitchThemeMode _currentMode = CodexSwitchThemeMode.Light;
    private string _activeCategory = "Overview";

    public MainWindow()
    {
        Title = "CodexSwitchUI";
        Width = 1320;
        Height = 900;
        MinWidth = 1020;
        MinHeight = 700;

        Content = BuildShell();
        RefreshChrome();
    }

    private Control BuildShell()
    {
        var root = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(new GridLength(284)),
                new ColumnDefinition(GridLength.Star)
            }
        };

        _sidebar.Child = BuildSidebar();
        _sidebar.BorderThickness = new Thickness(0, 0, 1, 0);
        Grid.SetColumn(_sidebar, 0);

        var workspace = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition(new GridLength(76)),
                new RowDefinition(GridLength.Star)
            }
        };
        Grid.SetColumn(workspace, 1);

        _topbar.Padding = new Thickness(28, 16);
        _topbar.BorderThickness = new Thickness(0, 0, 0, 1);
        _topbar.Child = BuildTopbar();

        _scroll.Content = BuildContent();
        _scroll.Padding = new Thickness(28, 24, 28, 44);

        Grid.SetRow(_topbar, 0);
        Grid.SetRow(_scroll, 1);
        workspace.Children.Add(_topbar);
        workspace.Children.Add(_scroll);

        root.Children.Add(_sidebar);
        root.Children.Add(workspace);
        return root;
    }

    private Control BuildSidebar()
    {
        _navItemsByCategory.Clear();

        var nav = new StackPanel
        {
            Spacing = 18,
            Margin = new Thickness(20, 22)
        };

        nav.Children.Add(new StackPanel
        {
            Spacing = 5,
            Children =
            {
                new CodexText
                {
                    Text = "CodexSwitchUI",
                    Role = CodexTextRole.Title
                },
                new CodexText
                {
                    Text = "Avalonia desktop component workbench.",
                    Role = CodexTextRole.Muted,
                    TextWrapping = TextWrapping.Wrap
                }
            }
        });

        nav.Children.Add(new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 8,
            Children =
            {
                new CodexBadge { Content = "alpha", Variant = CodexControlVariant.Secondary },
                new CodexBadge { Content = "docs", Variant = CodexControlVariant.Outline },
                new CodexBadge { Content = "motion", Variant = CodexControlVariant.Success }
            }
        });

        nav.Children.Add(new CodexSeparator());

        var menu = new CodexSidebarMenu
        {
            ItemsSource = Categories
                .Select(category => new CodexSidebarMenuItem { Content = BuildNavCategory(category) })
                .ToArray(),
            HorizontalAlignment = HorizontalAlignment.Stretch
        };
        nav.Children.Add(new CodexSidebarGroup
        {
            Content = new StackPanel
            {
                Spacing = 4,
                Children =
                {
                    new CodexSidebarGroupLabel { Content = "Components" },
                    new CodexSidebarGroupContent { Content = menu }
                }
            }
        });

        nav.Children.Add(InfoPanel(
            "Docs-only motion preview",
            "Samples attach instance transitions so hover, focus, checked, and feedback states can be reviewed before the shared component styles grow first-class transition tokens."));

        return new ScrollViewer
        {
            Content = nav
        };
    }

    private CodexSidebarMenuButton BuildNavCategory(NavCategory category)
    {
        var isActive = _activeCategory == category.Title;
        var item = new CodexSidebarMenuButton
        {
            Content = new TextBlock { Text = category.Title, TextTrimming = TextTrimming.CharacterEllipsis },
            Icon = NavDot(isActive ? CodexSwitchResourceKeys.PrimaryBrush : CodexSwitchResourceKeys.MutedForegroundBrush),
            IsActive = isActive,
            HorizontalAlignment = HorizontalAlignment.Stretch
        };
        _navItemsByCategory[category.Title] = item;
        item.Click += (_, _) => Navigate(category.Title);
        ToolTip.SetTip(item, string.Join(" / ", category.Items));

        return item;
    }

    private Control BuildTopbar()
    {
        var grid = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Auto)
            }
        };

        var title = new StackPanel { Spacing = 2 };
        title.Children.Add(new CodexText { Text = _activeCategory == "Overview" ? "Component Library Workbench" : _activeCategory, Role = CodexTextRole.Subtitle });
        title.Children.Add(new CodexText
        {
            Text = _activeCategory == "Overview"
                ? $"All categories visible. Active theme: {_currentMode}."
                : $"Focused category. Active theme: {_currentMode}. Hover, focus, checked, and feedback samples preview docs-only motion.",
            Role = CodexTextRole.Muted,
            TextWrapping = TextWrapping.Wrap
        });

        var actions = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 8,
            HorizontalAlignment = HorizontalAlignment.Right
        };

        actions.Children.Add(ThemeButton("Light", CodexSwitchThemeMode.Light));
        actions.Children.Add(ThemeButton("Dark", CodexSwitchThemeMode.Dark));
        actions.Children.Add(ThemeButton("Custom", CodexSwitchThemeMode.Custom));

        Grid.SetColumn(title, 0);
        Grid.SetColumn(actions, 1);
        grid.Children.Add(title);
        grid.Children.Add(actions);

        return grid;
    }

    private CodexButton ThemeButton(string label, CodexSwitchThemeMode mode)
    {
        var button = WithMotion(new CodexButton
        {
            Content = label,
            Variant = _currentMode == mode
                ? CodexControlVariant.Default
                : mode == CodexSwitchThemeMode.Custom
                    ? CodexControlVariant.Outline
                    : CodexControlVariant.Secondary,
            Size = CodexControlSize.Small
        });
        button.Click += (_, _) => ApplyTheme(mode);
        return button;
    }

    private Control BuildContent()
    {
        var stack = new StackPanel { Spacing = 30 };

        if (_activeCategory == "Overview")
        {
            stack.Children.Add(BuildOverviewSection());
            stack.Children.Add(BuildDesignTokensSection());
            stack.Children.Add(BuildFormsSection());
            stack.Children.Add(BuildNavigationSection());
            stack.Children.Add(BuildOverlaySection());
            stack.Children.Add(BuildFeedbackSection());
            stack.Children.Add(BuildDataDisplaySection());
            stack.Children.Add(BuildUtilitiesSection());
            stack.Children.Add(BuildDesignReviewSection());
            return stack;
        }

        stack.Children.Add(BuildCategoryIntro(_activeCategory));
        stack.Children.Add(_activeCategory switch
        {
            "Tokens" => BuildDesignTokensSection(),
            "Forms" => BuildFormsSection(),
            "Navigation" => BuildNavigationSection(),
            "Overlay" => BuildOverlaySection(),
            "Feedback" => BuildFeedbackSection(),
            "Data Display" => BuildDataDisplaySection(),
            "Utilities" => BuildUtilitiesSection(),
            "Design Review" => BuildDesignReviewSection(),
            _ => BuildOverviewSection()
        });

        return stack;
    }

    private Control BuildCategoryIntro(string category)
    {
        var description = category switch
        {
            "Tokens" => "Semantic colors, type scale, density, and the proposed transition vocabulary that should eventually live in the shared theme.",
            "Forms" => "Input-heavy controls with hover, focus, disabled, checked, and intent states.",
            "Navigation" => "Wayfinding controls for tabs, menus, command surfaces, and docs-style side navigation.",
            "Overlay" => "Layered surfaces that should feel quiet, direct, and easy to dismiss.",
            "Feedback" => "Toasts, badges, avatar fallback, progress, skeleton, intent, animation, and loading states that communicate status without heavy chrome.",
            "Data Display" => "Compact presentation patterns for people, rows, metrics, cards, separators, typography, and operational summaries.",
            "Utilities" => "Low-level layout primitives, typography roles, separators, card surfaces, and adapters used by higher-level UI.",
            "Design Review" => "A scan-friendly audit of what is already working and what the next worker should tighten.",
            _ => "A quick map of CodexSwitchUI controls, runtime themes, and motion expectations."
        };

        return new StackPanel
        {
            Spacing = 8,
            Children =
            {
                new CodexText { Text = category, Role = CodexTextRole.Title },
                new CodexText
                {
                    Text = description,
                    Role = CodexTextRole.Muted,
                    TextWrapping = TextWrapping.Wrap,
                    MaxWidth = 760
                }
            }
        };
    }

    private Control BuildOverviewSection()
    {
        return CategorySection(
            "Overview",
            "A dense desktop gallery that documents the component set, states, theme switching, and motion expectations.",
            Case(
                "Workbench Orientation",
                "The first screen is a working component index, not a marketing page. Theme buttons rebuild every live sample against the current token palette.",
                PreviewSurface(new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition(GridLength.Star),
                        new ColumnDefinition(new GridLength(280))
                    },
                    ColumnSpacing = 18,
                    Children =
                    {
                        new StackPanel
                        {
                            Spacing = 12,
                            Children =
                            {
                                new CodexText { Text = "shadcn language, Avalonia primitives", Role = CodexTextRole.Title },
                                new CodexText
                                {
                                    Text = "Semantic resources drive button, input, overlay, feedback, and data-display controls across light, dark, and custom runtime themes.",
                                    Role = CodexTextRole.Muted,
                                    TextWrapping = TextWrapping.Wrap
                                },
                                new StackPanel
                                {
                                    Orientation = Orientation.Horizontal,
                                    Spacing = 8,
                                    Children =
                                    {
                                        new CodexBadge { Content = "net8" },
                                        new CodexBadge { Content = "net9", Variant = CodexControlVariant.Secondary },
                                        new CodexBadge { Content = "net10", Variant = CodexControlVariant.Outline },
                                        new CodexBadge { Content = "runtime theme", Variant = CodexControlVariant.Success }
                                    }
                                }
                            }
                        },
                        At(ThemeTokenStrip(), 1, 0)
                    }
                }),
                """
                app.Styles.Add(new CodexSwitchTheme());
                CodexSwitchThemeManager.Current.Apply(app, CodexSwitchThemeMode.Light);
                """),
            Case(
                "Component Map",
                "A quick count of what the docs now exercise. The map uses real CodexSwitchUI badges and control surfaces.",
                PreviewSurface(new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition(GridLength.Star),
                        new ColumnDefinition(GridLength.Star),
                        new ColumnDefinition(GridLength.Star)
                    },
                    ColumnSpacing = 12,
                    Children =
                    {
                        At(MetricTile("9", "categories", "overview through review"), 0, 0),
                        At(MetricTile("20+", "controls", "native Avalonia types"), 1, 0),
                        At(MetricTile("4", "motion states", "hover, focus, checked, feedback"), 2, 0)
                    }
                }),
                """
                // Docs groups cases by category:
                Overview, Tokens, Forms, Navigation, Overlay,
                Feedback, Data Display, Utilities, Design Review.
                """),
            Case(
                "Motion Baseline",
                "Hover the buttons, tab into the input, toggle the switch, and replay the toast. These docs previews do not change the public component styles.",
                PreviewSurface(BuildMotionBaseline()),
                """
                button.Transitions =
                [
                    new DoubleTransition { Property = Visual.OpacityProperty },
                    new BrushTransition { Property = TemplatedControl.BackgroundProperty },
                    new BrushTransition { Property = TemplatedControl.BorderBrushProperty }
                ];
                """));
    }

    private Control BuildDesignTokensSection()
    {
        return CategorySection(
            "Tokens",
            "Semantic slots stay close to shadcn naming while Avalonia controls bind through DynamicResource.",
            Case(
                "Color Tokens",
                "The live palette uses semantic resource keys rather than component-specific colors.",
                PreviewSurface(new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition(GridLength.Star),
                        new ColumnDefinition(GridLength.Star)
                    },
                    ColumnSpacing = 12,
                    RowSpacing = 10,
                    Children =
                    {
                        At(TokenSwatch("Background", CodexSwitchResourceKeys.BackgroundBrush, "app canvas"), 0, 0),
                        At(TokenSwatch("Foreground", CodexSwitchResourceKeys.ForegroundBrush, "body text"), 1, 0),
                        At(TokenSwatch("Primary", CodexSwitchResourceKeys.PrimaryBrush, "default actions"), 0, 1),
                        At(TokenSwatch("Secondary", CodexSwitchResourceKeys.SecondaryBrush, "quiet surfaces"), 1, 1),
                        At(TokenSwatch("Muted", CodexSwitchResourceKeys.MutedBrush, "sidebars and code"), 0, 2),
                        At(TokenSwatch("Border", CodexSwitchResourceKeys.BorderBrush, "thin dividers"), 1, 2),
                        At(TokenSwatch("Success", CodexSwitchResourceKeys.SuccessBrush, "positive state"), 0, 3),
                        At(TokenSwatch("Warning", CodexSwitchResourceKeys.WarningBrush, "attention state"), 1, 3)
                    }
                }),
                """
                Background="{DynamicResource CodexSwitch.BackgroundBrush}"
                BorderBrush="{DynamicResource CodexSwitch.BorderBrush}"
                Foreground="{DynamicResource CodexSwitch.ForegroundBrush}"
                """),
            Case(
                "Typography And Density",
                "CodexText roles map to a small scale. Control density comes from theme options rather than one-off margins.",
                PreviewSurface(new StackPanel
                {
                    Spacing = 12,
                    Children =
                    {
                        new CodexText { Text = "Title: Component surface", Role = CodexTextRole.Title },
                        new CodexText { Text = "Subtitle: Forms / Button", Role = CodexTextRole.Subtitle },
                        new CodexText { Text = "Body: The default role stays legible for dense desktop docs.", Role = CodexTextRole.Body, TextWrapping = TextWrapping.Wrap },
                        new CodexText { Text = "Muted: Secondary copy uses the muted foreground token.", Role = CodexTextRole.Muted },
                        new CodexText { Text = "<CodexButton Size=\"Small\" />", Role = CodexTextRole.Code },
                        new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            Spacing = 8,
                            Children =
                            {
                                WithMotion(new CodexButton { Content = "Small", Size = CodexControlSize.Small, Variant = CodexControlVariant.Outline }),
                                WithMotion(new CodexButton { Content = "Medium", Size = CodexControlSize.Medium, Variant = CodexControlVariant.Outline }),
                                WithMotion(new CodexButton { Content = "Large", Size = CodexControlSize.Large, Variant = CodexControlVariant.Outline })
                            }
                        }
                    }
                }),
                """
                var options = CodexSwitchThemeOptions.ShadcnDefault with
                {
                    Radius = 6,
                    Density = CodexSwitchDensity.Default
                };
                """),
            Case(
                "Motion Tokens",
                "Docs preview the public timing system: short hover, medium focus, deliberate overlay and feedback entrance.",
                PreviewSurface(new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition(GridLength.Star),
                        new ColumnDefinition(GridLength.Star)
                    },
                    ColumnSpacing = 12,
                    RowSpacing = 12,
                    Children =
                    {
                        At(MotionToken("hover", "120ms", "CubicEaseOut", "opacity + background"), 0, 0),
                        At(MotionToken("focus", "140ms", "CubicEaseOut", "border + ring"), 1, 0),
                        At(MotionToken("checked", "160ms", "CubicEaseOut", "track + thumb"), 0, 1),
                        At(MotionToken("feedback", "180ms", "CubicEaseOut", "opacity + offset"), 1, 1)
                    }
                }),
                """
                // Public motion tokens:
                CodexSwitch.MotionDurationFast = 120ms
                CodexSwitch.MotionDurationDefault = 150ms
                CodexSwitch.MotionDurationSlow = 220ms
                CodexSwitch.MotionEaseOut = CubicEaseOut
                CodexSwitch.MotionEaseInOut = CubicEaseInOut
                """));
    }

    private Control BuildFormsSection()
    {
        return CategorySection(
            "Forms",
            "Form controls show variants, intent states, density, keyboard focus, checked behavior, and docs-only transition previews.",
            Case(
                "Button Variants",
                "Hover and press each button. The docs attaches transitions to opacity, background, border brush, and border thickness at the sample level.",
                PreviewSurface(BuildButtonVariants()),
                """
                <controls:CodexButton Content="Save" />
                <controls:CodexButton Content="Cancel" Variant="Secondary" />
                <controls:CodexButton Content="Delete" Variant="Destructive" />
                """),
            Case(
                "Forms State Matrix",
                "A single pass for every form control: default, hover/focus target, disabled, size, intent, checked, and unchecked samples are live controls.",
                PreviewSurface(BuildFormsStateMatrix()),
                """
                <controls:CodexTextBox Intent="Warning" Size="Large" />
                <controls:CodexSelect IsEnabled="False" />
                <controls:CodexCheckBox IsChecked="True" />
                <controls:CodexSwitch IsChecked="False" />
                """),
            Case(
                "TextBox Intent And Focus",
                "Tab into the inputs to inspect focus motion. Error, success, and warning states are semantic intents, not manual brush overrides.",
                PreviewSurface(new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition(GridLength.Star),
                        new ColumnDefinition(GridLength.Star)
                    },
                    RowDefinitions =
                    {
                        new RowDefinition(GridLength.Auto),
                        new RowDefinition(GridLength.Auto)
                    },
                    ColumnSpacing = 14,
                    RowSpacing = 14,
                    Children =
                    {
                        At(Field("Default", WithMotion(new CodexTextBox { PlaceholderText = "name@example.com" })), 0, 0),
                        At(Field("Error", WithMotion(new CodexTextBox { Text = "invalid value", Intent = CodexControlIntent.Error })), 1, 0),
                        At(Field("Success", WithMotion(new CodexTextBox { Text = "ready to submit", Intent = CodexControlIntent.Success })), 0, 1),
                        At(Field("Warning", WithMotion(new CodexTextBox { Text = "review before saving", Intent = CodexControlIntent.Warning })), 1, 1)
                    }
                }),
                """
                <controls:CodexTextBox PlaceholderText="Email" />
                <controls:CodexTextBox Intent="Error" Text="Invalid" />
                """),
            Case(
                "Select, Checkbox, And Radio",
                "Selection controls share the same semantic foreground, muted copy, and compact spacing. Checked states should gain smooth transition support in the shared theme.",
                PreviewSurface(new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition(GridLength.Star),
                        new ColumnDefinition(GridLength.Star)
                    },
                    ColumnSpacing = 18,
                    Children =
                    {
                        At(new StackPanel
                        {
                            Spacing = 10,
                            Children =
                            {
                                Field("Density", WithMotion(new CodexSelect
                                {
                                    SelectedIndex = 1,
                                    ItemsSource = new[] { "Compact", "Default", "Comfortable" }
                                })),
                                Field("Size", WithMotion(new CodexSelect
                                {
                                    Size = CodexControlSize.Small,
                                    SelectedIndex = 0,
                                    ItemsSource = new[] { "Small select", "Medium select" }
                                }))
                            }
                        }, 0, 0),
                        At(new StackPanel
                        {
                            Spacing = 10,
                            Children =
                            {
                                new CodexCheckBox { Content = "Enable notifications", IsChecked = true },
                                new CodexCheckBox { Content = "Muted while disabled", IsEnabled = false },
                                new CodexRadio { Content = "Desktop target", IsChecked = true, GroupName = "target" },
                                new CodexRadio { Content = "Browser target", GroupName = "target" }
                            }
                        }, 1, 0)
                    }
                }),
                """
                <controls:CodexSelect ItemsSource="{Binding Densities}" />
                <controls:CodexCheckBox Content="Enable notifications" />
                <controls:CodexRadio GroupName="target" />
                """),
            Case(
                "Switch And Slider",
                "Toggle the switch and drag the slider. The switch background is previewed with a transition; thumb movement should be handled by the public component style later.",
                PreviewSurface(new StackPanel
                {
                    Spacing = 16,
                    Children =
                    {
                        new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            Spacing = 12,
                            Children =
                            {
                                WithMotion(new CodexSwitch { IsChecked = true }),
                                new CodexText { Text = "Runtime theme sync", Role = CodexTextRole.Muted, VerticalAlignment = VerticalAlignment.Center }
                            }
                        },
                        Field("Volume", WithMotion(new CodexSlider { Minimum = 0, Maximum = 100, Value = 68 })),
                        new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            Spacing = 8,
                            Children =
                            {
                                new CodexBadge { Content = "checked motion target", Variant = CodexControlVariant.Outline },
                                new CodexBadge { Content = "120-160ms", Variant = CodexControlVariant.Secondary }
                            }
                        }
                    }
                }),
                """
                <controls:CodexSwitch IsChecked="True" />
                <controls:CodexSlider Minimum="0" Maximum="100" Value="68" />
                """));
    }

    private Control BuildNavigationSection()
    {
        return CategorySection(
            "Navigation",
            "Navigation components remain quiet and predictable, with clear active states and compact controls.",
            TabsDocsExample(
                "Tabs",
                "Matches the Radix UI tabs preview: muted list, active background, focus ring, disabled opacity, and content cross-fade.",
                BuildTabsOverviewPreview(),
                """
                <controls:CodexTabs SelectedIndex="0">
                    <TabItem Header="Overview" />
                    <TabItem Header="Analytics" />
                    <TabItem Header="Reports" />
                    <TabItem Header="Settings" />
                </controls:CodexTabs>
                """),
            TabsDocsExample(
                "Line",
                "Use Variant=\"Line\" for the Radix line-style TabsList behavior.",
                BuildTabsLinePreview(),
                """
                <controls:CodexTabs Variant="Line">
                    <TabItem Header="Overview" />
                    <TabItem Header="Analytics" />
                    <TabItem Header="Reports" />
                </controls:CodexTabs>
                """),
            TabsDocsExample(
                "Vertical",
                "Use Orientation=\"Vertical\" for vertical tabs; the line variant moves the active indicator to the trailing edge.",
                BuildTabsVerticalPreview(),
                """
                <controls:CodexTabs Orientation="Vertical" SelectedIndex="2">
                    <TabItem Header="Account" />
                    <TabItem Header="Password" />
                    <TabItem Header="Notifications" />
                </controls:CodexTabs>
                """),
            TabsDocsExample(
                "Disabled",
                "Disabled TabItem keeps the shadcn opacity and cursor behavior without falling back to Avalonia defaults.",
                BuildTabsDisabledPreview(),
                """
                <controls:CodexTabs>
                    <TabItem Header="Home" />
                    <TabItem Header="Disabled" IsEnabled="False" />
                </controls:CodexTabs>
                """),
            TabsDocsExample(
                "Icons",
                "Headers can compose icons and text while the trigger template still owns spacing, focus, and selected state.",
                BuildTabsIconsPreview(),
                """
                <controls:CodexTabs>
                    <TabItem>
                        <TabItem.Header>
                            <StackPanel Orientation="Horizontal" Spacing="6">
                                <PathIcon Data="{StaticResource AppWindowIcon}" />
                                <TextBlock Text="Preview" />
                            </StackPanel>
                        </TabItem.Header>
                    </TabItem>
                </controls:CodexTabs>
                """),
            TabsDocsExample(
                "RTL",
                "FlowDirection=\"RightToLeft\" mirrors the trigger order while retaining active state and content animation.",
                BuildTabsRtlPreview(),
                """
                <controls:CodexTabs FlowDirection="RightToLeft">
                    <TabItem Header="Account" />
                    <TabItem Header="Password" />
                </controls:CodexTabs>
                """),
            TabsDocsExample(
                "Navigation Menu",
                "The default case is a horizontal navigation root: triggers sit in one row, share one viewport, slide from start/end, and resize the viewport with the active item.",
                BuildNavigationMenuPreview(),
                """
                <controls:CodexNavigationMenu Orientation="Horizontal">
                    <controls:CodexNavigationMenuItem Header="Components" ViewportWidth="500">
                        <StackPanel Orientation="Horizontal" Spacing="8">
                            <controls:CodexNavigationMenuLink Content="Tabs" />
                            <controls:CodexNavigationMenuLink Content="Menu" />
                        </StackPanel>
                    </controls:CodexNavigationMenuItem>
                </controls:CodexNavigationMenu>
                """),
            Case(
                "Collapsible",
                "A Radix-style disclosure keeps the trigger compact, rotates the chevron on open, and animates content height from the measured panel size.",
                PreviewSurface(BuildCollapsiblePreview()),
                """
                <controls:CodexCollapsible Header="Repository" IsOpen="True">
                    <StackPanel Spacing="6">
                        <controls:CodexButton Variant="Ghost" Content="main" />
                        <controls:CodexButton Variant="Ghost" Content="release" />
                    </StackPanel>
                </controls:CodexCollapsible>
                """),
            Case(
                "Menu And Command",
                "Menu now owns the dropdown popup surface, submenu arrow, checked indicator, shortcut rail, and open animation.",
                PreviewSurface(new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition(GridLength.Star),
                        new ColumnDefinition(GridLength.Star)
                    },
                    ColumnSpacing = 14,
                    Children =
                    {
                        At(new StackPanel
                        {
                            Spacing = 10,
                            Children =
                            {
                                new CodexText { Text = "CodexMenu", Role = CodexTextRole.Subtitle },
                                new CodexMenu
                                {
                                    ItemsSource = new[]
                                    {
                                        new CodexMenuItem
                                        {
                                            Header = "View",
                                            IsSubMenuOpen = true,
                                            ItemsSource = new object[]
                                            {
                                                new CodexMenuItem { Header = "Overview", Shortcut = "G O" },
                                                new CodexMenuItem { Header = "Design Review", Shortcut = "G D" },
                                                new CodexMenuSeparator(),
                                                new CodexMenuItem { Header = "Checked item", ToggleType = MenuItemToggleType.CheckBox, IsChecked = true }
                                            }
                                        },
                                        new CodexMenuItem
                                        {
                                            Header = "Theme",
                                            ItemsSource = new[]
                                            {
                                                new CodexMenuItem { Header = "Light", ToggleType = MenuItemToggleType.Radio, IsChecked = true, GroupName = "theme" },
                                                new CodexMenuItem { Header = "Dark", ToggleType = MenuItemToggleType.Radio, GroupName = "theme" }
                                            }
                                        }
                                    }
                                }
                            }
                        }, 0, 0),
                        At(WithMotion(new CodexCommand
                        {
                            Placeholder = "Search commands...",
                            Content = new StackPanel
                            {
                                Spacing = 6,
                                Children =
                                {
                                    WithMotion(new CodexCommandItem { Content = "Open dialog", HorizontalAlignment = HorizontalAlignment.Stretch }),
                                    WithMotion(new CodexCommandItem { Content = "Switch theme", HorizontalAlignment = HorizontalAlignment.Stretch }),
                                    WithMotion(new CodexCommandItem { Content = "Run design review", HorizontalAlignment = HorizontalAlignment.Stretch })
                                }
                            }
                        }), 1, 0)
                    }
                }),
                """
                <controls:CodexMenu>
                    <controls:CodexMenuItem Header="View" Shortcut="G V" />
                </controls:CodexMenu>
                <controls:CodexCommand Placeholder="Search commands..." />
                """),
            Case(
                "Context Menu",
                "Radix context-menu motion is mapped to Avalonia with side-aware translate, 0.95 zoom, fade-in, delayed open classes, and transform origins that follow the popup side.",
                PreviewSurface(BuildContextMenuPreview()),
                """
                <Border>
                    <Border.ContextMenu>
                        <controls:CodexContextMenu>
                            <controls:CodexContextMenuLabel Content="Canvas" />
                            <controls:CodexContextMenuItem Header="Back" Shortcut="Alt+Left" />
                            <controls:CodexContextMenuItem Header="View" />
                        </controls:CodexContextMenu>
                    </Border.ContextMenu>
                </Border>
                """),
            Case(
                "Sidebar And Segmented Controls",
                "The extracted app-shell primitives cover a 220px navigation rail, shadcn-style sidebar menu buttons, icon-only actions, field labels, keyboard chips, and compact segmented ranges.",
                PreviewSurface(BuildApplicationSidebarPreview()),
                """
                <controls:CodexSidebar>
                    <controls:CodexSidebarMenu>
                        <controls:CodexSidebarMenuItem>
                            <controls:CodexSidebarMenuButton Content="Home" IsActive="True" />
                        </controls:CodexSidebarMenuItem>
                    </controls:CodexSidebarMenu>
                </controls:CodexSidebar>
                <controls:CodexSegmentedControl>
                    <controls:CodexSegmentedButton Content="24h" IsSelected="True" />
                </controls:CodexSegmentedControl>
                """),
            Case(
                "Docs Side Navigation Pattern",
                "The left menu uses CodexSidebarMenu primitives with shadcn sidebar-menu active, hover, focus, action, badge, and disabled states without Avalonia MenuItem chrome.",
                PreviewSurface(BuildDocsSideNavigationPreview()),
                """
                <controls:CodexSidebarGroup>
                    <controls:CodexSidebarGroupLabel Content="Components" />
                    <controls:CodexSidebarGroupContent>
                        <controls:CodexSidebarMenu>
                            <controls:CodexSidebarMenuItem>
                                <controls:CodexSidebarMenuButton Content="Overview" IsActive="True" />
                            </controls:CodexSidebarMenuItem>
                        </controls:CodexSidebarMenu>
                    </controls:CodexSidebarGroupContent>
                </controls:CodexSidebarGroup>
                """),
            Case(
                "Navigation State Matrix",
                "Tabs, menu, command rows, and sidebar buttons show active, inactive, hover, focusable, and disabled navigation states.",
                PreviewSurface(BuildNavigationStateMatrix()),
                """
                <controls:CodexTabs SelectedIndex="1" />
                <controls:CodexCommand Placeholder="Search..." />
                <controls:CodexButton Variant="Ghost" IsEnabled="False" />
                """));
    }

    private Control BuildApplicationSidebarPreview()
    {
        var sidebar = new CodexSidebar
        {
            Width = 220,
            Content = new StackPanel
            {
                Spacing = 14,
                Children =
                {
                    new StackPanel
                    {
                        Spacing = 4,
                        Children =
                        {
                            new CodexText { Text = "CodexSwitch", Role = CodexTextRole.Subtitle },
                            new CodexText { Text = "Local proxy workspace", Role = CodexTextRole.Muted, TextWrapping = TextWrapping.Wrap }
                        }
                    },
                    new CodexSegmentedControl
                    {
                        Content = new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            Spacing = 4,
                            Children =
                            {
                                new CodexSegmentedButton { Content = "Codex", IsSelected = true },
                                new CodexSegmentedButton { Content = "Claude" }
                            }
                        }
                    },
                    new CodexSidebarGroup
                    {
                        Content = new CodexSidebarGroupContent
                        {
                            Content = new CodexSidebarMenu
                            {
                                ItemsSource = new object[]
                                {
                                    new CodexSidebarMenuItem { Content = SidebarMenuButton("Home", true, "live", CodexSwitchResourceKeys.PrimaryBrush) },
                                    new CodexSidebarMenuItem { Content = SidebarMenuButton("Logs", false, "134", CodexSwitchResourceKeys.MutedForegroundBrush) },
                                    new CodexSidebarMenuItem { Content = SidebarMenuButton("Models", false, null, CodexSwitchResourceKeys.MutedForegroundBrush) },
                                    new CodexSidebarMenuItem { Content = SidebarMenuButton("Settings", false, null, CodexSwitchResourceKeys.MutedForegroundBrush, isEnabled: false) }
                                }
                            }
                        }
                    },
                    new CodexSeparator(),
                    new CodexButton
                    {
                        Content = "Add provider",
                        Variant = CodexControlVariant.Outline,
                        Size = CodexControlSize.Small,
                        HorizontalAlignment = HorizontalAlignment.Stretch
                    }
                }
            }
        };

        var section = new CodexSection
        {
            Title = "Home",
            Description = "Status, filters, and actions use library-owned primitives.",
            Actions = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Spacing = 6,
                Children =
                {
                    new CodexIconButton { Content = "R", IsRound = true },
                    new CodexIconButton { Content = "+", Variant = CodexControlVariant.Default }
                }
            },
            Content = new StackPanel
            {
                Spacing = 14,
                Children =
                {
                    new Grid
                    {
                        ColumnDefinitions =
                        {
                            new ColumnDefinition(GridLength.Star),
                            new ColumnDefinition(GridLength.Star)
                        },
                        ColumnSpacing = 12,
                        Children =
                        {
                            new CodexField
                            {
                                Label = "Provider",
                                Description = "Small density select",
                                Content = new CodexSelect
                                {
                                    Size = CodexControlSize.Small,
                                    SelectedIndex = 0,
                                    ItemsSource = new[] { "OpenAI", "Anthropic", "Custom" }
                                }
                            },
                            At(new StackPanel
                            {
                                Spacing = 6,
                                Children =
                                {
                                    new CodexText { Text = "Shortcuts", Role = CodexTextRole.Muted },
                                    new StackPanel
                                    {
                                        Orientation = Orientation.Horizontal,
                                        Spacing = 6,
                                        Children =
                                        {
                                            new CodexKbd { Content = "G" },
                                            new CodexKbd { Content = "H" },
                                            new CodexKbd { Content = "Enter" }
                                        }
                                    }
                                }
                            }, 1, 0)
                        }
                    },
                    new CodexSegmentedControl
                    {
                        HorizontalAlignment = HorizontalAlignment.Left,
                        Content = new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            Spacing = 4,
                            Children =
                            {
                                new CodexSegmentedButton { Content = "24h", IsSelected = true },
                                new CodexSegmentedButton { Content = "7d" },
                                new CodexSegmentedButton { Content = "30d" }
                            }
                        }
                    }
                }
            }
        };

        return new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(new GridLength(220)),
                new ColumnDefinition(GridLength.Star)
            },
            ColumnSpacing = 14,
            Children =
            {
                sidebar,
                At(section, 1, 0)
            }
        };
    }

    private Control TabsDocsExample(string title, string caption, Control preview, string code)
    {
        var grid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Auto)
            },
            RowSpacing = 14
        };

        grid.Children.Add(new StackPanel
        {
            Spacing = 4,
            Children =
            {
                new CodexText { Text = title, Role = CodexTextRole.Subtitle },
                new CodexText
                {
                    Text = caption,
                    Role = CodexTextRole.Muted,
                    TextWrapping = TextWrapping.Wrap,
                    MaxWidth = 840
                }
            }
        });

        var frameGrid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition(new GridLength(260)),
                new RowDefinition(new GridLength(104))
            }
        };

        preview.HorizontalAlignment = HorizontalAlignment.Center;
        preview.VerticalAlignment = VerticalAlignment.Center;

        frameGrid.Children.Add(new Border
        {
            Padding = new Thickness(24),
            Child = preview
        });

        var codePanel = new Border
        {
            Background = Brush(CodexSwitchResourceKeys.MutedBrush),
            BorderBrush = Brush(CodexSwitchResourceKeys.BorderBrush),
            BorderThickness = new Thickness(0, 1, 0, 0),
            Padding = new Thickness(18, 18, 18, 12),
            Child = TabsCodePreview(code)
        };
        Grid.SetRow(codePanel, 1);
        frameGrid.Children.Add(codePanel);

        var viewCode = WithMotion(new CodexButton
        {
            Content = "View Code",
            Variant = CodexControlVariant.Outline,
            Size = CodexControlSize.Small
        });
        Grid.SetRow(viewCode, 1);
        viewCode.HorizontalAlignment = HorizontalAlignment.Center;
        viewCode.VerticalAlignment = VerticalAlignment.Top;
        viewCode.Margin = new Thickness(0, -18, 0, 0);
        frameGrid.Children.Add(viewCode);

        var frame = new Border
        {
            Background = Brush(CodexSwitchResourceKeys.BackgroundBrush),
            BorderBrush = Brush(CodexSwitchResourceKeys.BorderBrush),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(8),
            ClipToBounds = true,
            Child = frameGrid
        };
        Grid.SetRow(frame, 1);
        grid.Children.Add(frame);

        return grid;
    }

    private Control TabsCodePreview(string code)
    {
        var lines = code.Trim().Replace("\r\n", "\n").Split('\n').Take(3).ToArray();
        var grid = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(new GridLength(36)),
                new ColumnDefinition(GridLength.Star)
            },
            RowSpacing = 7
        };

        for (var i = 0; i < lines.Length; i++)
        {
            grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));

            var lineOpacity = i == 0 ? 1 : i == 1 ? 0.72 : 0.42;
            var number = new CodexText
            {
                Text = (i + 1).ToString(),
                Role = CodexTextRole.Code,
                Opacity = lineOpacity * 0.7,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            var source = new CodexText
            {
                Text = lines[i].TrimEnd(),
                Role = CodexTextRole.Code,
                Opacity = lineOpacity,
                TextWrapping = TextWrapping.NoWrap
            };

            grid.Children.Add(At(number, 0, i));
            grid.Children.Add(At(source, 1, i));
        }

        return grid;
    }

    private Control BuildTabsOverviewPreview()
    {
        return WithMotion(new CodexTabs
        {
            Width = 420,
            SelectedIndex = 0,
            ItemsSource = new[]
            {
                TabsSummaryItem(
                    "Overview",
                    "Overview",
                    "View your key metrics and recent project activity. Track progress across all your active projects.",
                    "You have 12 active projects and 3 pending tasks."),
                TabsSummaryItem(
                    "Analytics",
                    "Analytics",
                    "Compare traffic, conversion, and retention changes across the current reporting window.",
                    "Audience quality is up 8 percent from the previous period."),
                TabsSummaryItem(
                    "Reports",
                    "Reports",
                    "Review generated summaries, export packets, and delivery status for stakeholder updates.",
                    "Two reports are ready to share."),
                TabsSummaryItem(
                    "Settings",
                    "Settings",
                    "Manage workspace defaults, notification routing, and dashboard preferences.",
                    "Changes sync across this device.")
            }
        });
    }

    private Control BuildTabsLinePreview()
    {
        return WithMotion(new CodexTabs
        {
            Variant = CodexTabsVariant.Line,
            SelectedIndex = 0,
            ItemsSource = new[]
            {
                new TabItem { Header = "Overview" },
                new TabItem { Header = "Analytics" },
                new TabItem { Header = "Reports" }
            }
        });
    }

    private Control BuildTabsVerticalPreview()
    {
        return WithMotion(new CodexTabs
        {
            Orientation = Orientation.Vertical,
            SelectedIndex = 2,
            ItemsSource = new[]
            {
                new TabItem { Header = "Account" },
                new TabItem { Header = "Password" },
                new TabItem { Header = "Notifications" }
            }
        });
    }

    private Control BuildTabsDisabledPreview()
    {
        return WithMotion(new CodexTabs
        {
            SelectedIndex = 0,
            ItemsSource = new[]
            {
                new TabItem { Header = "Home" },
                new TabItem { Header = "Disabled", IsEnabled = false }
            }
        });
    }

    private Control BuildTabsIconsPreview()
    {
        return WithMotion(new CodexTabs
        {
            SelectedIndex = 0,
            ItemsSource = new[]
            {
                new TabItem { Header = TabsIconHeader("Preview", "M3 4H21V20H3Z M5 8H19V18H5Z") },
                new TabItem { Header = TabsIconHeader("Code", "M8 7L3 12L8 17V13L6 12L8 11V7ZM16 7V11L18 12L16 13V17L21 12L16 7Z") }
            }
        });
    }

    private Control BuildTabsRtlPreview()
    {
        return WithMotion(new CodexTabs
        {
            FlowDirection = FlowDirection.RightToLeft,
            SelectedIndex = 0,
            ItemsSource = new[]
            {
                new TabItem { Header = "Account" },
                new TabItem { Header = "Password" },
                new TabItem { Header = "Notifications" }
            }
        });
    }

    private Control BuildNavigationMenuPreview()
    {
        var components = new CodexNavigationMenuItem
        {
            Header = "Components",
            ViewportWidth = 500,
            ViewportMinHeight = 172,
            Content = NavigationMenuHorizontalContent(
                "Components",
                "Shared viewport with directional content motion.",
                new CodexNavigationMenuLink { Content = "Tabs", Description = "Trigger list and cross-fade content." },
                new CodexNavigationMenuLink { Content = "Menu", Description = "Popup surface and submenu motion." },
                new CodexNavigationMenuLink { Content = "Command", Description = "Search input and active rows." })
        };

        var patterns = new CodexNavigationMenuItem
        {
            Header = "Patterns",
            ViewportWidth = 420,
            ViewportMinHeight = 150,
            Content = NavigationMenuHorizontalContent(
                "Patterns",
                "Width and height follow the active content.",
                new CodexNavigationMenuLink { Content = "Docs nav", Description = "Category-first navigation." },
                new CodexNavigationMenuLink { Content = "Workbench", Description = "Compact command surfaces." })
        };

        var menu = WithMotion(new CodexNavigationMenu
        {
            Orientation = Orientation.Horizontal,
            ItemsSource = new[]
            {
                components,
                patterns,
                new CodexNavigationMenuItem { Header = "Changelog" }
            }
        });

        menu.ActivateItem(components);
        return menu;
    }

    private Control BuildNavigationMenuStatePreview()
    {
        var components = new CodexNavigationMenuItem
        {
            Header = "Components",
            ViewportWidth = 500,
            ViewportMinHeight = 172,
            Content = NavigationMenuHorizontalContent(
                "Components",
                "Shared viewport with directional content motion.",
                new CodexNavigationMenuLink { Content = "Tabs", Description = "Trigger list and cross-fade content." },
                new CodexNavigationMenuLink { Content = "Menu", Description = "Popup surface and submenu motion." },
                new CodexNavigationMenuLink { Content = "Command", Description = "Search input and active rows." })
        };

        var patterns = new CodexNavigationMenuItem
        {
            Header = "Patterns",
            ViewportWidth = 420,
            ViewportMinHeight = 150,
            Content = NavigationMenuHorizontalContent(
                "Patterns",
                "Width and height follow the active content.",
                new CodexNavigationMenuLink { Content = "Docs nav", Description = "Category-first navigation." },
                new CodexNavigationMenuLink { Content = "Workbench", Description = "Compact command surfaces." })
        };

        var menu = WithMotion(new CodexNavigationMenu
        {
            Orientation = Orientation.Horizontal,
            ItemsSource = new[]
            {
                components,
                patterns,
                new CodexNavigationMenuItem { Header = "Disabled", IsEnabled = false },
                new CodexNavigationMenuItem { Header = "Link" }
            }
        });

        menu.ActivateItem(components);
        return menu;
    }

    private Control NavigationMenuHorizontalContent(string title, string description, params CodexNavigationMenuLink[] links)
    {
        var columnCount = Math.Min(Math.Max(links.Length, 1), 3);
        var grid = new Grid
        {
            ColumnSpacing = 8,
            RowSpacing = 8
        };

        for (var column = 0; column < columnCount; column++)
        {
            grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        }

        var rowCount = (int)Math.Ceiling(links.Length / (double)columnCount);
        for (var row = 0; row < rowCount; row++)
        {
            grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        }

        for (var index = 0; index < links.Length; index++)
        {
            links[index].HorizontalAlignment = HorizontalAlignment.Stretch;
            grid.Children.Add(At(links[index], index % columnCount, index / columnCount));
        }

        return new Border
        {
            Padding = new Thickness(16),
            Child = new StackPanel
            {
                Spacing = 12,
                Children =
                {
                    new StackPanel
                    {
                        Spacing = 3,
                        Children =
                        {
                            new CodexText { Text = title, Role = CodexTextRole.Subtitle },
                            new CodexText
                            {
                                Text = description,
                                Role = CodexTextRole.Muted,
                                TextWrapping = TextWrapping.Wrap
                            }
                        }
                    },
                    grid
                }
            }
        };
    }

    private Control BuildContextMenuPreview()
    {
        var contextMenu = new CodexContextMenu
        {
            Placement = PlacementMode.BottomEdgeAlignedLeft,
            Width = 236,
            ItemsSource = new object[]
            {
                new CodexContextMenuLabel { Content = "Canvas" },
                new CodexContextMenuItem { Header = "Back", Shortcut = "Alt+Left", IsEnabled = false },
                new CodexContextMenuItem { Header = "Reload", Shortcut = "Ctrl+R" },
                new CodexContextMenuItem
                {
                    Header = "More tools",
                    IsSubMenuOpen = true,
                    SubMenuPlacement = PlacementMode.RightEdgeAlignedTop,
                    ItemsSource = new object[]
                    {
                        new CodexContextMenuItem { Header = "Inspect", Shortcut = "Ctrl+Shift+I" },
                        new CodexContextMenuItem { Header = "Command menu", Shortcut = "Ctrl+K" },
                        new CodexContextMenuSeparator(),
                        new CodexContextMenuItem { Header = "Pin", ToggleType = MenuItemToggleType.CheckBox, IsChecked = true }
                    }
                },
                new CodexContextMenuSeparator(),
                new CodexContextMenuItem { Header = "Light", ToggleType = MenuItemToggleType.Radio, IsChecked = true, GroupName = "context-theme" },
                new CodexContextMenuItem { Header = "Dark", ToggleType = MenuItemToggleType.Radio, GroupName = "context-theme" }
            }
        };
        contextMenu.Classes.Add("context-menu-open");

        return new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(new GridLength(260))
            },
            ColumnSpacing = 16,
            Children =
            {
                new Border
                {
                    MinHeight = 210,
                    Background = Brush(CodexSwitchResourceKeys.MutedBrush),
                    BorderBrush = Brush(CodexSwitchResourceKeys.BorderBrush),
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(6),
                    Padding = new Thickness(16),
                    Child = new StackPanel
                    {
                        Spacing = 8,
                        Children =
                        {
                            new CodexText { Text = "Right-click target", Role = CodexTextRole.Subtitle },
                            new CodexText
                            {
                                Text = "The visible menu on the right shows the same attached context menu with the open state forced for motion review.",
                                Role = CodexTextRole.Muted,
                                TextWrapping = TextWrapping.Wrap
                            },
                            new CodexBadge { Content = "side-bottom + submenu-right", Variant = CodexControlVariant.Outline }
                        }
                    }
                },
                At(contextMenu, 1, 0)
            }
        };
    }

    private TabItem TabsSummaryItem(string header, string title, string description, string note)
    {
        return new TabItem
        {
            Header = header,
            Content = new CodexCard
            {
                Width = 400,
                Padding = new Thickness(16),
                Title = title,
                Description = description,
                Content = new CodexText
                {
                    Text = note,
                    Role = CodexTextRole.Muted,
                    TextWrapping = TextWrapping.Wrap
                }
            }
        };
    }

    private static StackPanel TabsIconHeader(string label, string data)
    {
        return new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 6,
            Children =
            {
                new PathIcon
                {
                    Data = StreamGeometry.Parse(data),
                    Width = 14,
                    Height = 14,
                    VerticalAlignment = VerticalAlignment.Center
                },
                new TextBlock
                {
                    Text = label,
                    FontWeight = FontWeight.SemiBold,
                    VerticalAlignment = VerticalAlignment.Center
                }
            }
        };
    }

    private Control BuildCollapsiblePreview()
    {
        return new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Star)
            },
            ColumnSpacing = 16,
            Children =
            {
                At(new StackPanel
                {
                    Spacing = 8,
                    Children =
                    {
                        WithMotion(new CodexCollapsible
                        {
                            Header = "Repository",
                            IsOpen = true,
                            Content = CollapsibleContent(
                                new CodexButton { Content = "main", Variant = CodexControlVariant.Ghost, Size = CodexControlSize.Small, HorizontalAlignment = HorizontalAlignment.Stretch, HorizontalContentAlignment = HorizontalAlignment.Left },
                                new CodexButton { Content = "release", Variant = CodexControlVariant.Ghost, Size = CodexControlSize.Small, HorizontalAlignment = HorizontalAlignment.Stretch, HorizontalContentAlignment = HorizontalAlignment.Left },
                                new CodexButton { Content = "feature/collapsible", Variant = CodexControlVariant.Secondary, Size = CodexControlSize.Small, HorizontalAlignment = HorizontalAlignment.Stretch, HorizontalContentAlignment = HorizontalAlignment.Left })
                        }),
                        WithMotion(new CodexCollapsible
                        {
                            Header = "Animation notes",
                            Content = CollapsibleContent(
                                new CodexText
                                {
                                    Text = "Content stays mounted while height animates to zero, mirroring Radix's measured-height CSS variable pattern.",
                                    Role = CodexTextRole.Muted,
                                    TextWrapping = TextWrapping.Wrap
                                },
                                new StackPanel
                                {
                                    Orientation = Orientation.Horizontal,
                                    Spacing = 8,
                                    Children =
                                    {
                                        new CodexBadge { Content = "height", Variant = CodexControlVariant.Outline },
                                        new CodexBadge { Content = "200ms", Variant = CodexControlVariant.Secondary },
                                        new CodexBadge { Content = "ease-out", Variant = CodexControlVariant.Success }
                                    }
                                })
                        })
                    }
                }, 0, 0),
                At(new StackPanel
                {
                    Spacing = 8,
                    Children =
                    {
                        WithMotion(new CodexCollapsible
                        {
                            Header = "Small density",
                            Size = CodexControlSize.Small,
                            IsOpen = true,
                            Content = CollapsibleContent(new CodexText
                            {
                                Text = "Small triggers keep the same chevron rotation and measured-height content animation.",
                                Role = CodexTextRole.Muted,
                                TextWrapping = TextWrapping.Wrap
                            })
                        }),
                        WithMotion(new CodexCollapsible
                        {
                            Header = "Disabled",
                            IsEnabled = false,
                            Content = CollapsibleContent(new CodexText
                            {
                                Text = "Disabled state inherits the shared opacity token.",
                                Role = CodexTextRole.Muted,
                                TextWrapping = TextWrapping.Wrap
                            })
                        })
                    }
                }, 1, 0)
            }
        };
    }

    private Control CollapsibleContent(params Control[] controls)
    {
        var stack = new StackPanel
        {
            Spacing = 6
        };

        foreach (var control in controls)
        {
            stack.Children.Add(control);
        }

        return stack;
    }

    private Control BuildOverlaySection()
    {
        return CategorySection(
            "Overlay",
            "Dialog, popover, overlay, and focus surfaces share border, card, popover, and ring resources.",
            Case(
                "Dialog",
                "Dialog content uses the card surface. Overlay enter/exit animation should come from the shared component style later.",
                PreviewSurface(new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition(GridLength.Star),
                        new ColumnDefinition(new GridLength(170))
                    },
                    ColumnSpacing = 14,
                    Children =
                    {
                        WithMotion(new CodexDialog
                        {
                            Content = new StackPanel
                            {
                                Spacing = 12,
                                Children =
                                {
                                    new CodexText { Text = "Edit profile", Role = CodexTextRole.Subtitle },
                                    new CodexText
                                    {
                                        Text = "Dialog surfaces use card, border, popover, and foreground tokens.",
                                        Role = CodexTextRole.Muted,
                                        TextWrapping = TextWrapping.Wrap
                                    },
                                    new StackPanel
                                    {
                                        Orientation = Orientation.Horizontal,
                                        Spacing = 8,
                                        Children =
                                        {
                                            WithMotion(new CodexButton { Content = "Save", Size = CodexControlSize.Small }),
                                            WithMotion(new CodexButton { Content = "Cancel", Size = CodexControlSize.Small, Variant = CodexControlVariant.Secondary })
                                        }
                                    }
                                }
                            }
                        }),
                        At(new CodexOverlay
                        {
                            Height = 120,
                            Content = new CodexText
                            {
                                Text = "overlay",
                                Role = CodexTextRole.Muted,
                                HorizontalAlignment = HorizontalAlignment.Center,
                                VerticalAlignment = VerticalAlignment.Center
                            }
                        }, 1, 0)
                    }
                }),
                """
                <controls:CodexDialog>
                    <controls:CodexButton Content="Save" />
                </controls:CodexDialog>
                """),
            Case(
                "Popover",
                "Popover stays compact and token driven. Hover the action rows to inspect state transitions on nested controls.",
                PreviewSurface(WithMotion(new CodexPopover
                {
                    Content = new StackPanel
                    {
                        Spacing = 10,
                        Children =
                        {
                            new CodexText { Text = "Inspect component", Role = CodexTextRole.Subtitle },
                            new CodexText
                            {
                                Text = "Designed as a primitive for select, menu, command, and tooltip composition.",
                                Role = CodexTextRole.Muted,
                                TextWrapping = TextWrapping.Wrap
                            },
                            new CodexSeparator(),
                            WithMotion(new CodexButton { Content = "Open usage", Variant = CodexControlVariant.Ghost, HorizontalAlignment = HorizontalAlignment.Stretch, HorizontalContentAlignment = HorizontalAlignment.Left }),
                            WithMotion(new CodexButton { Content = "Copy snippet", Variant = CodexControlVariant.Ghost, HorizontalAlignment = HorizontalAlignment.Stretch, HorizontalContentAlignment = HorizontalAlignment.Left })
                        }
                    }
                })),
                """
                <controls:CodexPopover>
                    <StackPanel Spacing="10">...</StackPanel>
                </controls:CodexPopover>
                """),
            Case(
                "Focus Ring",
                "Focus treatment is already tokenized through RingBrush and FocusThickness. The shared style should animate ring opacity and offset.",
                PreviewSurface(new StackPanel
                {
                    Spacing = 12,
                    Children =
                    {
                        new CodexFocusRing
                        {
                            RingBrush = Brush(CodexSwitchResourceKeys.RingBrush),
                            RingThickness = new Thickness(2),
                            Content = WithMotion(new CodexTextBox { Text = "Tab here to inspect focus", Width = 260 })
                        },
                        new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            Spacing = 8,
                            Children =
                            {
                                new CodexBadge { Content = "focus", Variant = CodexControlVariant.Outline },
                                new CodexBadge { Content = "ring token", Variant = CodexControlVariant.Secondary },
                                new CodexBadge { Content = "ring transition", Variant = CodexControlVariant.Success }
                            }
                        }
                    }
                }),
                """
                <primitives:CodexFocusRing RingThickness="2">
                    <controls:CodexTextBox />
                </primitives:CodexFocusRing>
                """),
            Case(
                "Overlay State Matrix",
                "Overlay primitives show default surface, disabled action, focus ring, variant actions, and warning/destructive decisions inside layers.",
                PreviewSurface(BuildOverlayStateMatrix()),
                """
                <controls:CodexDialog Title="Danger zone" />
                <controls:CodexPopover>
                    <controls:CodexButton IsEnabled="False" />
                </controls:CodexPopover>
                """),
            Case(
                "Open / Closed Layer States",
                "Dialog and popover examples are shown as open surfaces next to closed triggers so animation and disabled states are inspectable without a modal host.",
                PreviewSurface(BuildOverlayOpenClosedMatrix()),
                """
                // Docs state model:
                Open = rendered surface
                Closed = trigger + muted placeholder
                Animation = opacity / border transition preview
                """));
    }

    private Control BuildFeedbackSection()
    {
        return CategorySection(
            "Feedback",
            "Feedback components communicate status with semantic variants, restrained borders, and short motion.",
            Case(
                "Toast Variants",
                "The toast stack uses real CodexToast controls. Border color changes are tokenized by variant.",
                PreviewSurface(new StackPanel
                {
                    Spacing = 10,
                    Children =
                    {
                        Toast("Theme saved", "Custom tokens are active across the control surface.", CodexControlVariant.Default),
                        Toast("Validation failed", "Destructive intent only swaps semantic resources.", CodexControlVariant.Destructive),
                        Toast("Build passed", "All target frameworks share the same component surface.", CodexControlVariant.Success),
                        Toast("Token drift", "Warnings highlight theme differences without changing layout.", CodexControlVariant.Warning)
                    }
                }),
                """
                <controls:CodexToast Variant="Success">
                    <TextBlock Text="Build passed" />
                </controls:CodexToast>
                """),
            Case(
                "Sonner Toaster",
                "A global CodexSonner viewport mirrors shadcn's Toaster pattern: place one host near the app root, then call the toast service from any action.",
                PreviewSurface(BuildSonnerDemo()),
                """
                <controls:CodexSonner Position="BottomRight" RichColors="True" />

                CodexSonnerService.Toast("Event has been created", new CodexSonnerOptions
                {
                    Description = "Sunday, December 03, 2023 at 9:00 AM",
                    Action = new CodexSonnerAction("Undo", () => Console.WriteLine("Undo"))
                });
                """),
            Case(
                "Feedback Motion Replay",
                "Click Replay to animate a docs-only toast opacity transition. The public toast style should later own enter, exit, and stack motion.",
                PreviewSurface(BuildFeedbackMotionReplay()),
                """
                toast.Transitions =
                [
                    new DoubleTransition
                    {
                        Property = Visual.OpacityProperty,
                        Duration = TimeSpan.FromMilliseconds(180)
                    }
                ];
                """),
            Case(
                "Spinner",
                "Spinner follows the shadcn/Radix pattern: LoaderIcon geometry, current foreground color, a status label, and a one-second spin.",
                PreviewSurface(BuildSpinnerShowcase()),
                """
                <controls:CodexSpinner Label="Loading" />
                <controls:CodexSpinner Size="Large"
                                        Foreground="{DynamicResource CodexSwitch.MutedForegroundBrush}" />
                <controls:CodexButton IsLoading="True">
                    Submit
                </controls:CodexButton>
                """),
            Case(
                "Badge, Progress, Skeleton",
                "Badges encode state; progress and skeleton use quiet surfaces that should animate without becoming noisy.",
                PreviewSurface(new StackPanel
                {
                    Spacing = 16,
                    Children =
                    {
                        new WrapPanel
                        {
                            Children =
                            {
                                new CodexBadge { Content = "default", Margin = new Thickness(4) },
                                new CodexBadge { Content = "secondary", Variant = CodexControlVariant.Secondary, Margin = new Thickness(4) },
                                new CodexBadge { Content = "outline", Variant = CodexControlVariant.Outline, Margin = new Thickness(4) },
                                new CodexBadge { Content = "success", Variant = CodexControlVariant.Success, Margin = new Thickness(4) },
                                new CodexBadge { Content = "warning", Variant = CodexControlVariant.Warning, Margin = new Thickness(4) },
                                new CodexBadge { Content = "destructive", Variant = CodexControlVariant.Destructive, Margin = new Thickness(4) }
                            }
                        },
                        WithMotion(new CodexProgress { Minimum = 0, Maximum = 100, Value = 72, Width = 360 }),
                        new StackPanel
                        {
                            Spacing = 7,
                            Children =
                            {
                                new CodexSkeleton { Height = 12, Width = 260 },
                                new CodexSkeleton { Height = 12, Width = 180 },
                                new CodexSkeleton { Height = 12, Width = 320 }
                            }
                        }
                    }
                }),
                """
                <controls:CodexBadge Variant="Warning" />
                <controls:CodexProgress Value="72" />
                <controls:CodexSkeleton Width="260" Height="12" />
                """),
            Case(
                "Feedback State Matrix",
                "Feedback states cover default, success, warning, destructive, loading, disabled replay, and motion review targets.",
                PreviewSurface(BuildFeedbackStateMatrix()),
                """
                <controls:CodexToast Variant="Warning" />
                <controls:CodexBadge Variant="Success" />
                <controls:CodexProgress IsEnabled="False" />
                """),
            Case(
                "Feedback, Identity, And Loading",
                "Toast, badge, avatar, progress, and skeleton cover open, closed, intent, animation, and loading review states in one pass.",
                PreviewSurface(BuildFeedbackOperationalMatrix()),
                """
                <controls:CodexToast Variant="Success" />
                <controls:CodexBadge Variant="Warning" />
                <controls:CodexAvatar Fallback="CS" />
                <controls:CodexProgress Value="64" />
                <controls:CodexSkeleton />
                """));
    }

    private Control BuildSpinnerShowcase()
    {
        return new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Star)
            },
            RowDefinitions =
            {
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Auto)
            },
            ColumnSpacing = 12,
            RowSpacing = 12,
            Children =
            {
                At(StateTile(
                    "Size",
                    "Small, default, and large sizes keep the same LoaderIcon proportions.",
                    SpinnerRow("Loading small", CodexControlSize.Small),
                    SpinnerRow("Loading default"),
                    SpinnerRow("Loading large", CodexControlSize.Large)), 0, 0),
                At(StateTile(
                    "Button",
                    "Loading buttons reuse CodexSpinner and inherit the button foreground.",
                    WithMotion(new CodexButton
                    {
                        Content = "Submit",
                        IsLoading = true
                    }),
                    WithMotion(new CodexButton
                    {
                        Content = "Refresh",
                        LeadingIcon = new CodexSpinner
                        {
                            Size = CodexControlSize.Small,
                            Label = "Refreshing"
                        },
                        Variant = CodexControlVariant.Outline
                    })), 1, 0),
                At(StateTile(
                    "Badge",
                    "Compact status chips can carry an inline spinner without changing badge height.",
                    new CodexBadge
                    {
                        Content = new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            Spacing = 6,
                            Children =
                            {
                                new CodexSpinner
                                {
                                    Size = CodexControlSize.Small,
                                    Label = "Syncing",
                                    Foreground = Brush(CodexSwitchResourceKeys.PrimaryForegroundBrush)
                                },
                                new TextBlock
                                {
                                    Text = "Syncing",
                                    Foreground = Brush(CodexSwitchResourceKeys.PrimaryForegroundBrush)
                                }
                            }
                        }
                    },
                    new CodexBadge
                    {
                        Content = new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            Spacing = 6,
                            Children =
                            {
                                new CodexSpinner
                                {
                                    Size = CodexControlSize.Small,
                                    Label = "Queued",
                                    IsActive = false
                                },
                                new TextBlock { Text = "Queued" }
                            }
                        },
                        Variant = CodexControlVariant.Outline
                    }), 0, 1),
                At(StateTile(
                    "Input Group",
                    "Use a leading spinner when an input is searching or validating.",
                    new Border
                    {
                        Background = Brush(CodexSwitchResourceKeys.BackgroundBrush),
                        BorderBrush = Brush(CodexSwitchResourceKeys.BorderBrush),
                        BorderThickness = new Thickness(1),
                        CornerRadius = new CornerRadius(6),
                        Padding = new Thickness(12, 8),
                        Child = new Grid
                        {
                            ColumnDefinitions =
                            {
                                new ColumnDefinition(GridLength.Auto),
                                new ColumnDefinition(GridLength.Star),
                                new ColumnDefinition(GridLength.Auto)
                            },
                            ColumnSpacing = 10,
                            Children =
                            {
                                new CodexSpinner
                                {
                                    Size = CodexControlSize.Small,
                                    Label = "Searching components",
                                    Foreground = Brush(CodexSwitchResourceKeys.MutedForegroundBrush)
                                },
                                At(new CodexText
                                {
                                    Text = "Searching registry",
                                    Role = CodexTextRole.Muted
                                }, 1, 0),
                                At(new CodexBadge
                                {
                                    Content = "live",
                                    Variant = CodexControlVariant.Secondary
                                }, 2, 0)
                            }
                        }
                    }), 1, 1),
                At(StateTile(
                    "Empty",
                    "Empty states can center the spinner above a short status and a secondary action.",
                    new StackPanel
                    {
                        Spacing = 10,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Children =
                        {
                            new CodexSpinner
                            {
                                Size = CodexControlSize.Large,
                                Label = "Loading results"
                            },
                            new CodexText
                            {
                                Text = "Loading results",
                                Role = CodexTextRole.Subtitle,
                                HorizontalAlignment = HorizontalAlignment.Center
                            },
                            new CodexText
                            {
                                Text = "The list will appear here when the query finishes.",
                                Role = CodexTextRole.Muted,
                                TextWrapping = TextWrapping.Wrap,
                                TextAlignment = TextAlignment.Center,
                                MaxWidth = 260
                            },
                            WithMotion(new CodexButton
                            {
                                Content = "Cancel",
                                Variant = CodexControlVariant.Secondary,
                                Size = CodexControlSize.Small
                            })
                        }
                    }), 0, 2),
                At(StateTile(
                    "RTL",
                    "Right-to-left rows keep the icon/text order natural.",
                    new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        FlowDirection = FlowDirection.RightToLeft,
                        Spacing = 8,
                        Children =
                        {
                            new CodexSpinner
                            {
                                Label = "Loading RTL",
                                Foreground = Brush(CodexSwitchResourceKeys.MutedForegroundBrush)
                            },
                            new CodexText
                            {
                                Text = "Loading RTL",
                                Role = CodexTextRole.Muted
                            }
                        }
                    }), 1, 2)
            }
        };
    }

    private Control SpinnerRow(
        string text,
        CodexControlSize size = CodexControlSize.Medium,
        string? foregroundKey = null,
        bool isActive = true)
    {
        var spinner = new CodexSpinner
        {
            Size = size,
            Label = text,
            IsActive = isActive,
            Foreground = foregroundKey is null
                ? Brush(CodexSwitchResourceKeys.ForegroundBrush)
                : Brush(foregroundKey)
        };

        return new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 8,
            Children =
            {
                spinner,
                new CodexText
                {
                    Text = text,
                    Role = CodexTextRole.Muted
                }
            }
        };
    }

    private Control BuildDashboardProviderCardPreview()
    {
        return new StackPanel
        {
            Spacing = 14,
            Children =
            {
                new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition(GridLength.Star),
                        new ColumnDefinition(GridLength.Star),
                        new ColumnDefinition(GridLength.Star)
                    },
                    ColumnSpacing = 12,
                    Children =
                    {
                        WithMotion(new CodexStatCard
                        {
                            Label = "Proxy status",
                            Value = "Running",
                            Detail = "127.0.0.1:11434",
                            Icon = NavDot(CodexSwitchResourceKeys.SuccessBrush),
                            AccentBrush = Brush(CodexSwitchResourceKeys.SuccessBrush)
                        }),
                        At(WithMotion(new CodexStatCard
                        {
                            Label = "Live requests",
                            Value = "128 rpm",
                            Detail = "small density",
                            Icon = NavDot(CodexSwitchResourceKeys.PrimaryBrush),
                            AccentBrush = Brush(CodexSwitchResourceKeys.PrimaryBrush)
                        }), 1, 0),
                        At(WithMotion(new CodexStatCard
                        {
                            Label = "Total cost",
                            Value = "$0.84",
                            Detail = "30 day window",
                            Icon = NavDot(CodexSwitchResourceKeys.WarningBrush),
                            AccentBrush = Brush(CodexSwitchResourceKeys.WarningBrush)
                        }), 2, 0)
                    }
                },
                new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition(GridLength.Star),
                        new ColumnDefinition(GridLength.Star)
                    },
                    ColumnSpacing = 12,
                    Children =
                    {
                        ProviderCardPreview(
                            "OpenAI",
                            "https://api.openai.com/v1",
                            "oai",
                            true,
                            false,
                            true),
                        At(ProviderCardPreview(
                            "Anthropic",
                            "https://api.anthropic.com",
                            "ant",
                            false,
                            true,
                            false), 1, 0)
                    }
                }
            }
        };
    }

    private CodexProviderCard ProviderCardPreview(string header, string description, string fallback, bool active, bool dragging, bool actionEnabled)
    {
        return WithMotion(new CodexProviderCard
        {
            Header = header,
            Description = description,
            IsActive = active,
            IsDragging = dragging,
            Leading = new CodexKbd { Content = active ? "1" : "2" },
            Icon = ProviderIcon(fallback),
            Meta = new CodexBadge { Content = "responses", Variant = CodexControlVariant.Secondary },
            Status = new CodexBadge
            {
                Content = active ? "active" : "standby",
                Variant = active ? CodexControlVariant.Success : CodexControlVariant.Outline
            },
            Usage = new CodexMetric
            {
                Label = "Quota",
                Value = active ? "76%" : "42%",
                Detail = dragging ? "dragging state" : "live sample"
            },
            Actions = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Spacing = 6,
                Children =
                {
                    new CodexIconButton { Content = "E", Variant = CodexControlVariant.Ghost, IsEnabled = actionEnabled },
                    new CodexIconButton { Content = "M", Variant = CodexControlVariant.Outline }
                }
            }
        });
    }

    private Control ProviderIcon(string fallback)
    {
        return new Border
        {
            Width = 30,
            Height = 30,
            CornerRadius = new CornerRadius(7),
            Background = Brush(CodexSwitchResourceKeys.MutedBrush),
            BorderBrush = Brush(CodexSwitchResourceKeys.BorderBrush),
            BorderThickness = new Thickness(1),
            Child = new Grid
            {
                Children =
                {
                    new CodexImageIcon
                    {
                        Width = 18,
                        Height = 18,
                        Path = string.Empty,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    },
                    new CodexText
                    {
                        Text = fallback,
                        Role = CodexTextRole.Code,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    }
                }
            }
        };
    }

    private Control BuildRankedBarChartPreview()
    {
        var providerItems = new[]
        {
            new CodexRankedBarChartItem("OpenAI", 128, "128", "84.5K tokens / $0.42 / 99.1%"),
            new CodexRankedBarChartItem("Anthropic", 74, "74", "52.8K tokens / $0.31 / 98.6%"),
            new CodexRankedBarChartItem("Gemini", 36, "36", "18.4K tokens / $0.09 / 97.8%"),
            new CodexRankedBarChartItem("Local", 12, "12", "6.1K tokens / $0.00 / 100%")
        };
        var modelItems = new[]
        {
            new CodexRankedBarChartItem("gpt-5.5", 84500, "84.5K", "128 requests / $0.42"),
            new CodexRankedBarChartItem("claude-sonnet-4-5", 52800, "52.8K", "74 requests / $0.31"),
            new CodexRankedBarChartItem("gemini-2.5-pro", 18400, "18.4K", "36 requests / $0.09")
        };

        return new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Star)
            },
            RowDefinitions =
            {
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Auto)
            },
            ColumnSpacing = 12,
            RowSpacing = 12,
            Children =
            {
                new CodexRankedBarChart
                {
                    ItemsSource = providerItems,
                    EmptyText = "No provider usage",
                    Height = 226
                },
                At(new CodexRankedBarChart
                {
                    ItemsSource = modelItems,
                    EmptyText = "No model usage",
                    IsCompact = true,
                    Height = 206
                }, 1, 0),
                At(new CodexRankedBarChart
                {
                    EmptyText = "Empty state",
                    IsEnabled = false,
                    Height = 112,
                    MaxVisibleItems = 3
                }, 0, 1)
            }
        };
    }

    private Control BuildDataDisplaySection()
    {
        return CategorySection(
            "Data Display",
            "Data display examples keep density high while preserving borders, alignment, and scan paths.",
            Case(
                "Avatar And Metrics",
                "Avatar fallback and metric tiles demonstrate compact identity and summary patterns.",
                PreviewSurface(new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition(new GridLength(220)),
                        new ColumnDefinition(GridLength.Star)
                    },
                    ColumnSpacing = 16,
                    Children =
                    {
                        new StackPanel
                        {
                            Spacing = 12,
                            Children =
                            {
                                new CodexAvatar { Fallback = "CS" },
                                new CodexText { Text = "CodexSwitchUI", Role = CodexTextRole.Subtitle },
                                new CodexText { Text = "Component docs owner", Role = CodexTextRole.Muted }
                            }
                        },
                        At(new Grid
                        {
                            ColumnDefinitions =
                            {
                                new ColumnDefinition(GridLength.Star),
                                new ColumnDefinition(GridLength.Star)
                            },
                            ColumnSpacing = 12,
                            RowSpacing = 12,
                            Children =
                            {
                                At(MetricTile("23", "resources", "semantic slots"), 0, 0),
                                At(MetricTile("8", "variants", "button + badge"), 1, 0),
                                At(MetricTile("3", "themes", "light / dark / custom"), 0, 1),
                                At(MetricTile("4", "motion states", "documented here"), 1, 1)
                            }
                        }, 1, 0)
                    }
                }),
                """
                <controls:CodexAvatar Fallback="CS" />
                <controls:CodexBadge Content="desktop" />
                """),
            Case(
                "Table Foundation",
                "CodexTable owns the reusable table surface while callers provide header/body row layout, matching the app table pattern without keeping CsTable local.",
                PreviewSurface(BuildTablePreview()),
                """
                var table = new CodexTable
                {
                    Content = new StackPanel { Children = { header, body } }
                };
                """),
            Case(
                "Ranked Bar Chart",
                "CodexRankedBarChart renders compact top-N summaries as one lightweight drawing surface for dashboard provider, model, and segment breakdowns.",
                PreviewSurface(BuildRankedBarChartPreview()),
                """
                <controls:CodexRankedBarChart
                    ItemsSource="{Binding ProviderUsageChartItems}"
                    EmptyText="{Binding EmptyText}"
                    IsCompact="True" />
                """),
            Case(
                "Dashboard Metrics And Provider Card",
                "Home-page components are documented as reusable layout primitives: stat cards, metrics, image-icon slots, provider-card slots, active state, dragging state, disabled actions, and compact density.",
                PreviewSurface(BuildDashboardProviderCardPreview()),
                """
                <controls:CodexStatCard Label="Requests" Value="128 rpm" />
                <controls:CodexProviderCard
                    Header="OpenAI"
                    Description="https://api.openai.com/v1"
                    IsActive="True">
                    <controls:CodexProviderCard.Icon>
                        <controls:CodexImageIcon Path="{Binding IconPath}" />
                    </controls:CodexProviderCard.Icon>
                </controls:CodexProviderCard>
                """),
            Case(
                "Data Display State Matrix",
                "Avatar fallback, empty rows, dense table rows, success/warning metrics, and disabled row actions are visible in one inspection pass.",
                PreviewSurface(BuildDataDisplayStateMatrix()),
                """
                <controls:CodexAvatar Fallback="CS" />
                <controls:CodexTable>
                    <StackPanel>
                        <controls:CodexTableHeader />
                        <controls:CodexTableBody ItemsSource="{Binding Rows}" />
                    </StackPanel>
                </controls:CodexTable>
                <controls:CodexBadge Variant="Warning" />
                """),
            Case(
                "Card, Separator, And Typography In Data",
                "Data views often need supporting surfaces: a card frame, horizontal and vertical separators, and title/body/muted/code typography roles.",
                PreviewSurface(BuildDataUtilityMatrix()),
                """
                <controls:CodexCard />
                <controls:CodexSeparator />
                <primitives:CodexText Role="Code" />
                """));
    }

    private Control BuildUtilitiesSection()
    {
        var echartsTheme = CodexSwitchEChartsTheme.FromCurrentTheme();

        return CategorySection(
            "Utilities",
            "Utility primitives keep layout, dividers, card surfaces, and chart adapters aligned with the same semantic tokens.",
            Case(
                "Card And Separator",
                "Cards use a 1px border, compact radius, and token surfaces. Separators keep dense layouts readable.",
                PreviewSurface(new StackPanel
                {
                    Spacing = 14,
                    Children =
                    {
                        WithMotion(new CodexCard
                        {
                            Padding = new Thickness(16),
                            Content = new StackPanel
                            {
                                Spacing = 8,
                                Children =
                                {
                                    new CodexText { Text = "Desktop first", Role = CodexTextRole.Subtitle },
                                    new CodexText
                                    {
                                        Text = "Cards should frame repeated docs cases, inspector panels, or actual grouped controls.",
                                        Role = CodexTextRole.Muted,
                                        TextWrapping = TextWrapping.Wrap
                                    }
                                }
                            }
                        }),
                        new CodexSeparator(),
                        new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            Spacing = 12,
                            Children =
                            {
                                new CodexText { Text = "Left", Role = CodexTextRole.Muted },
                                new CodexSeparator { Orientation = Orientation.Vertical, Height = 24 },
                                new CodexText { Text = "Right", Role = CodexTextRole.Muted }
                            }
                        }
                    }
                }),
                """
                <controls:CodexCard>
                    <controls:CodexSeparator />
                </controls:CodexCard>
                """),
            Case(
                "ECharts Theme Adapter",
                "The ECharts extension derives background and series colors from the active CodexSwitchUI theme.",
                PreviewSurface(new StackPanel
                {
                    Spacing = 12,
                    Children =
                    {
                        new CodexText { Text = $"ECharts background: {echartsTheme.BackgroundColor}", Role = CodexTextRole.Code, TextWrapping = TextWrapping.Wrap },
                        new CodexText { Text = $"ECharts series: {string.Join(", ", echartsTheme.Color.Take(4))}", Role = CodexTextRole.Code, TextWrapping = TextWrapping.Wrap },
                        new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            Spacing = 8,
                            Children =
                            {
                                ThemeAction("Apply custom", CodexSwitchThemeMode.Custom, CodexControlVariant.Outline),
                                ThemeAction("Reset light", CodexSwitchThemeMode.Light, CodexControlVariant.Secondary)
                            }
                        }
                    }
                }),
                """
                var echartsTheme = CodexSwitchEChartsTheme.FromCurrentTheme();
                option.BackgroundColor = echartsTheme.BackgroundColor;
                """),
            Case(
                "ECharts Usage Trend Chart",
                "Usage dashboards can consume the ECharts package control through a neutral point contract while keeping the production legend and tooltip behavior.",
                PreviewSurface(BuildEChartsUsageTrendChartExample()),
                """
                <echarts:CsUsageTrendChart
                    ItemsSource="{Binding TrendPoints}"
                    Granularity="{Binding UsageTrendGranularity}"
                    TokensLabel="Tokens"
                    CostLabel="Cost"
                    CacheHitRateLabel="Cache hit rate"
                    OutputTpsLabel="Output TPS" />
                """),
            Case(
                "Utilities State Matrix",
                "Cards, separators, typography, focus wrappers, and ECharts token output are checked across default, vertical, muted, code, and disabled states.",
                PreviewSurface(BuildUtilitiesStateMatrix()),
                """
                <controls:CodexCard />
                <controls:CodexSeparator Orientation="Vertical" />
                <primitives:CodexText Role="Code" />
                """));
    }

    private Control BuildDesignReviewSection()
    {
        return CategorySection(
            "Design Review",
            "A practical audit based on the current code, theme files, and guard tests.",
            Case(
                "What Is Working",
                "The current library already has a clear shadcn-like foundation.",
                PreviewSurface(ReviewList(
                    ("done", "Semantic color slots mirror shadcn naming and are exposed through stable resource keys."),
                    ("done", "Core controls inherit from native Avalonia primitives, so keyboard/focus behavior is not reinvented."),
                    ("done", "Button, badge, toast, input, select, and switch variants are class-based and easy to scan in XAML."),
                    ("done", "Light, dark, custom, density, radius, and ECharts theme integration are already runtime-driven."),
                    ("done", "Desktop docs now group controls by task category instead of a flat demo list."))),
                """
                // Strength:
                CodexSwitchResourceKeys.PrimaryBrush
                CodexSwitchThemeManager.Current.Apply(app, mode, options);
                """),
            Case(
                "Motion / Token / Transition Check",
                "Motion is now part of the public token contract; component styles use short shadcn-like transitions and tests keep the contract visible.",
                PreviewSurface(ReviewList(
                    ("done", "Duration, easing, disabled opacity, ring offset, overlay opacity, and reduced-motion tokens are public resources."),
                    ("done", "Button, TextBox, Select, Tabs, NavigationMenu, Menu, ContextMenu, Command, Collapsible, Table, and feedback surfaces declare transitions."),
                    ("done", "Switch, Checkbox, Radio, Slider, Progress, and Skeleton own checked/loading/indicator transitions."),
                    ("done", "Dialog, Popover, Toast, Overlay, and Skeleton expose motion-related token hooks."),
                    ("gap", "Next pass should replace the remaining literal XAML durations with shared token-backed transition factories."))),
                """
                // Suggested public resources:
                CodexSwitch.MotionDurationFast
                CodexSwitch.MotionDurationDefault
                CodexSwitch.MotionDurationSlow
                CodexSwitch.MotionEaseOut
                CodexSwitch.MotionEaseInOut
                CodexSwitch.ReducedMotion
                """),
            Case(
                "Detail Coverage Matrix",
                "Each component is tracked against Template, Tokens, Variants, States, Motion, docs example, and tests so review gaps are explicit.",
                PreviewSurface(BuildCoverageMatrix()),
                """
                // Review axes:
                Template, Tokens, Variants, States, Motion,
                Docs example, Tests
                """),
            Case(
                "Avalonia Default Style Residue Check",
                "This checklist calls out template parts that must be fully owned by CodexSwitchUI so native Avalonia chrome does not leak through.",
                PreviewSurface(BuildDefaultStyleResidueAudit()),
                """
                // Anti-leak rule:
                if a control exposes chrome, popup, thumb,
                indicator, row, or content presenter parts,
                document whether CodexSwitchUI owns them.
                """),
            Case(
                "Theme Token And Custom Theme Example",
                "Theme review keeps semantic resources, template parts, animation timing, and custom palette overrides visible together.",
                PreviewSurface(BuildThemeTokenReviewExample()),
                """
                var options = CodexSwitchThemeOptions.ShadcnDefault with
                {
                    Radius = 8,
                    Density = CodexSwitchDensity.Comfortable,
                    CustomPalette = CodexSwitchPalette.Light with
                    {
                        Primary = "#FF2563EB",
                        Ring = "#FF60A5FA"
                    }
                };
                """),
            Case(
                "Next Implementation Pass",
                "Recommended next steps for the main worker after docs are merged.",
                PreviewSurface(ReviewList(
                    ("next", "Promote docs-only transition timings into theme resources and reference them from component styles."),
                    ("next", "Replace Switch thumb margin jumps with a transform/translate transition in the template."),
                    ("next", "Add visual tests or screenshots for hover, focus, checked, and overlay states in light and dark themes."),
                    ("next", "Expand CodexTable into a documented header/body/row API once the component contract is stable."),
                    ("next", "Add reduced-motion handling before making overlay and feedback animations more expressive."))),
                """
                // Keep Docs honest:
                show current behavior, preview target behavior,
                and label public-style work as a follow-up.
                """));
    }

    private Control BuildEChartsUsageTrendChartExample()
    {
        return new Grid
        {
            RowDefinitions =
            {
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Auto)
            },
            RowSpacing = 14,
            Children =
            {
                At(new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition(GridLength.Star),
                        new ColumnDefinition(GridLength.Auto)
                    },
                    Children =
                    {
                        At(new WrapPanel
                        {
                            Children =
                            {
                                ChartLegendItem("#60A5FA", "Input"),
                                ChartLegendItem("#A78BFA", "Cache hit"),
                                ChartLegendItem("#F59E0B", "Cache write"),
                                ChartLegendItem("#34D399", "Output"),
                                ChartLegendItem("#22D3EE", "Reasoning"),
                                ChartLegendLine("#F472B6", "Cost")
                            }
                        }, 0, 0),
                        At(new CodexBadge { Content = "Local records", Variant = CodexControlVariant.Outline }, 1, 0)
                    }
                }, 0, 0),
                At(new CsUsageTrendChart
                {
                    ItemsSource = CreateUsageTrendChartSample(),
                    Granularity = UsageTrendChartGranularity.Hour,
                    TokensLabel = "Tokens",
                    RequestsLabel = "Requests",
                    CostLabel = "Cost",
                    InputLabel = "Input",
                    CachedInputLabel = "Cache hit",
                    CacheCreationInputLabel = "Cache write",
                    CacheHitRateLabel = "Cache hit rate",
                    OutputTpsLabel = "Output TPS",
                    OutputLabel = "Output",
                    ReasoningLabel = "Reasoning",
                    EmptyText = "No usage records in this range",
                    RefreshingText = "Refreshing",
                    Height = 300
                }, 0, 1),
                At(new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition(GridLength.Star),
                        new ColumnDefinition(GridLength.Star),
                        new ColumnDefinition(GridLength.Star)
                    },
                    ColumnSpacing = 10,
                    Children =
                    {
                        At(MetricTile("79.2K", "Tokens", "stacked input and output series"), 0, 0),
                        At(MetricTile("24.6%", "Cache hit", "tooltip parity with dashboard"), 1, 0),
                        At(MetricTile("$0.1441", "Cost", "right-axis pink trend line"), 2, 0)
                    }
                }, 0, 2)
            }
        };
    }

    private static UsageTrendChartPoint[] CreateUsageTrendChartSample()
    {
        var start = new DateTimeOffset(2026, 5, 14, 0, 0, 0, TimeSpan.Zero);
        var points = new UsageTrendChartPoint[12];

        for (var index = 0; index < points.Length; index++)
        {
            var activity = index switch
            {
                < 2 => 0,
                < 5 => 1,
                < 9 => 2,
                _ => 1
            };
            var wave = Math.Sin(index / 1.65d) + 1.35d;
            var input = activity * (long)Math.Round(940 + wave * 760 + index * 95);
            var cached = activity * (long)Math.Round(input * (0.18d + index % 4 * 0.045d));
            var cacheWrite = activity * (long)Math.Round(input * (0.08d + index % 3 * 0.025d));
            var output = activity * (long)Math.Round(560 + wave * 430 + index * 58);
            var reasoning = activity * (long)Math.Round(output * (0.16d + index % 2 * 0.08d));
            var totalTokens = input + cached + cacheWrite + output + reasoning;

            points[index] = new UsageTrendChartPoint
            {
                Timestamp = start.AddHours(index * 2),
                Requests = activity == 0 ? 0 : 2 + index % 5,
                InputTokens = input,
                CachedInputTokens = cached,
                CacheCreationInputTokens = cacheWrite,
                OutputTokens = output,
                ReasoningOutputTokens = reasoning,
                OutputDurationMs = activity == 0 ? 0 : 1_900 + index * 120,
                Cost = totalTokens == 0 ? 0m : Math.Round(totalTokens / 1_000_000m * 1.82m, 4)
            };
        }

        return points;
    }

    private Control ChartLegendItem(string color, string label)
    {
        return new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 7,
            Margin = new Thickness(0, 0, 16, 6),
            Children =
            {
                new Border
                {
                    Width = 8,
                    Height = 8,
                    CornerRadius = new CornerRadius(4),
                    Background = new SolidColorBrush(Color.Parse(color)),
                    VerticalAlignment = VerticalAlignment.Center
                },
                new CodexText { Text = label, Role = CodexTextRole.Muted }
            }
        };
    }

    private Control ChartLegendLine(string color, string label)
    {
        return new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 7,
            Margin = new Thickness(0, 0, 16, 6),
            Children =
            {
                new Border
                {
                    Width = 18,
                    Height = 2,
                    Background = new SolidColorBrush(Color.Parse(color)),
                    VerticalAlignment = VerticalAlignment.Center
                },
                new CodexText { Text = label, Role = CodexTextRole.Muted }
            }
        };
    }

    private Control BuildFormsStateMatrix()
    {
        return new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Star)
            },
            RowDefinitions =
            {
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Auto)
            },
            ColumnSpacing = 12,
            RowSpacing = 12,
            Children =
            {
                At(StateTile(
                    "Button",
                    "Default, hover, variant, size, disabled.",
                    WithMotion(new CodexButton { Content = "Default" }),
                    WithMotion(new CodexButton { Content = "Hover target", Variant = CodexControlVariant.Outline }),
                    WithMotion(new CodexButton { Content = "Small", Size = CodexControlSize.Small, Variant = CodexControlVariant.Secondary }),
                    WithMotion(new CodexButton { Content = "Disabled", IsEnabled = false })), 0, 0),
                At(StateTile(
                    "TextBox",
                    "Default, focus, disabled, size, error, success, warning.",
                    WithMotion(new CodexTextBox { PlaceholderText = "Default value" }),
                    WithMotion(new CodexTextBox { Text = "Large focus target", Size = CodexControlSize.Large }),
                    WithMotion(new CodexTextBox { Text = "Disabled", IsEnabled = false }),
                    WithMotion(new CodexTextBox { Text = "Error", Intent = CodexControlIntent.Error }),
                    WithMotion(new CodexTextBox { Text = "Success", Intent = CodexControlIntent.Success }),
                    WithMotion(new CodexTextBox { Text = "Warning", Intent = CodexControlIntent.Warning })), 1, 0),
                At(StateTile(
                    "Select",
                    "Default, small, large, disabled.",
                    WithMotion(new CodexSelect { SelectedIndex = 0, ItemsSource = new[] { "Default option", "Second option" } }),
                    WithMotion(new CodexSelect { Size = CodexControlSize.Small, SelectedIndex = 0, ItemsSource = new[] { "Small select", "Another" } }),
                    WithMotion(new CodexSelect { Size = CodexControlSize.Large, SelectedIndex = 0, ItemsSource = new[] { "Large select", "Another" } }),
                    WithMotion(new CodexSelect { IsEnabled = false, SelectedIndex = 0, ItemsSource = new[] { "Disabled select" } })), 0, 1),
                At(StateTile(
                    "Checkbox / Radio",
                    "Checked, unchecked, disabled, grouped choice.",
                    new CodexCheckBox { Content = "Checked", IsChecked = true },
                    new CodexCheckBox { Content = "Unchecked" },
                    new CodexCheckBox { Content = "Disabled checked", IsChecked = true, IsEnabled = false },
                    new CodexRadio { Content = "Radio selected", IsChecked = true, GroupName = "forms-state" },
                    new CodexRadio { Content = "Radio option", GroupName = "forms-state" },
                    new CodexRadio { Content = "Radio disabled", IsEnabled = false, GroupName = "forms-state" }), 1, 1),
                At(StateTile(
                    "Switch",
                    "Checked, unchecked, disabled.",
                    new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        Spacing = 12,
                        Children =
                        {
                            WithMotion(new CodexSwitch { IsChecked = true }),
                            WithMotion(new CodexSwitch()),
                            WithMotion(new CodexSwitch { IsChecked = true, IsEnabled = false })
                        }
                    }), 0, 2),
                At(StateTile(
                    "Slider",
                    "Value, focus target, disabled.",
                    WithMotion(new CodexSlider { Minimum = 0, Maximum = 100, Value = 32 }),
                    WithMotion(new CodexSlider { Minimum = 0, Maximum = 100, Value = 74 }),
                    WithMotion(new CodexSlider { Minimum = 0, Maximum = 100, Value = 50, IsEnabled = false })), 1, 2)
            }
        };
    }

    private Control BuildNavigationStateMatrix()
    {
        return new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Star)
            },
            RowDefinitions =
            {
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Auto)
            },
            ColumnSpacing = 12,
            RowSpacing = 12,
            Children =
            {
                At(StateTile(
                    "Tabs",
                    "Default tab, selected tab, disabled tab.",
                    WithMotion(new CodexTabs
                    {
                        SelectedIndex = 1,
                        ItemsSource = new[]
                        {
                            new TabItem { Header = "Default", Content = new CodexText { Text = "Default tab content", Role = CodexTextRole.Muted } },
                            new TabItem { Header = "Selected", Content = new CodexText { Text = "Selected tab content", Role = CodexTextRole.Muted } },
                            new TabItem { Header = "Disabled", IsEnabled = false, Content = new CodexText { Text = "Disabled content", Role = CodexTextRole.Muted } }
                        }
                    })), 0, 0),
                At(StateTile(
                    "Menu",
                    "Top-level menu, owned submenu popup, checked item, disabled item.",
                    new CodexMenu
                    {
                        ItemsSource = new[]
                        {
                            new CodexMenuItem
                            {
                                Header = "File",
                                IsSubMenuOpen = true,
                                ItemsSource = new[]
                                {
                                    new CodexMenuItem { Header = "Open", Shortcut = "Ctrl+O" },
                                    new CodexMenuItem { Header = "Autosave", ToggleType = MenuItemToggleType.CheckBox, IsChecked = true },
                                    new CodexMenuItem { Header = "Export", IsEnabled = false }
                                }
                            },
                            new CodexMenuItem { Header = "Review" }
                        }
                    }), 1, 0),
                At(StateTile(
                    "Command",
                    "Search field, hoverable rows, disabled action.",
                    WithMotion(new CodexCommand
                    {
                        Placeholder = "Find component...",
                        Content = new StackPanel
                        {
                            Spacing = 4,
                            Children =
                            {
                                WithMotion(new CodexCommandItem { Content = "Open Forms", HorizontalAlignment = HorizontalAlignment.Stretch }),
                                WithMotion(new CodexCommandItem { Content = "Open Overlay", HorizontalAlignment = HorizontalAlignment.Stretch }),
                                WithMotion(new CodexCommandItem { Content = "Disabled command", IsEnabled = false, HorizontalAlignment = HorizontalAlignment.Stretch })
                            }
                        }
                    })), 0, 1),
                At(StateTile(
                    "Side Navigation Menu",
                    "CodexMenu active, inactive, hover, and disabled states.",
                    BuildDocsSideNavigationPreview(includeDisabled: true)), 1, 1),
                At(StateTile(
                    "Collapsible",
                    "Open, closed, disabled, and density states use one measured-height content path.",
                    WithMotion(new CodexCollapsible
                    {
                        Header = "Open section",
                        IsOpen = true,
                        Content = CollapsibleContent(new CodexText
                        {
                            Text = "This content animates from its measured height.",
                            Role = CodexTextRole.Muted,
                            TextWrapping = TextWrapping.Wrap
                        })
                    }),
                    WithMotion(new CodexCollapsible
                    {
                        Header = "Closed section",
                        Size = CodexControlSize.Small,
                        Content = CollapsibleContent(new CodexText
                        {
                            Text = "Closed content remains available for measurement when toggled.",
                            Role = CodexTextRole.Muted,
                            TextWrapping = TextWrapping.Wrap
                        })
                    })), 0, 2),
                At(StateTile(
                    "Navigation Menu",
                    "Open trigger, shared viewport, animated indicator, and disabled trigger.",
                    BuildNavigationMenuStatePreview()), 1, 2)
            }
        };
    }

    private Control BuildOverlayStateMatrix()
    {
        return new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Star)
            },
            RowDefinitions =
            {
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Auto)
            },
            ColumnSpacing = 12,
            RowSpacing = 12,
            Children =
            {
                At(StateTile(
                    "Dialog",
                    "Default surface, variant actions, disabled action.",
                    WithMotion(new CodexDialog
                    {
                        Title = "Danger zone",
                        Description = "Dialog title and description remain token driven.",
                        Content = new StackPanel
                        {
                            Spacing = 8,
                            Children =
                            {
                                new CodexText { Text = "Confirm workspace reset", Role = CodexTextRole.Subtitle },
                                new CodexText { Text = "Review destructive and secondary actions together.", Role = CodexTextRole.Muted, TextWrapping = TextWrapping.Wrap },
                                new StackPanel
                                {
                                    Orientation = Orientation.Horizontal,
                                    Spacing = 8,
                                    Children =
                                    {
                                        WithMotion(new CodexButton { Content = "Reset", Variant = CodexControlVariant.Destructive, Size = CodexControlSize.Small }),
                                        WithMotion(new CodexButton { Content = "Cancel", Variant = CodexControlVariant.Secondary, Size = CodexControlSize.Small }),
                                        WithMotion(new CodexButton { Content = "Disabled", IsEnabled = false, Size = CodexControlSize.Small })
                                    }
                                }
                            }
                        }
                    })), 0, 0),
                At(StateTile(
                    "Popover",
                    "Compact layer, focus field, disabled row.",
                    WithMotion(new CodexPopover
                    {
                        Content = new StackPanel
                        {
                            Spacing = 8,
                            Children =
                            {
                                new CodexText { Text = "Popover settings", Role = CodexTextRole.Subtitle },
                                WithMotion(new CodexTextBox { PlaceholderText = "Focus inside layer" }),
                                WithMotion(new CodexButton { Content = "Copy token", Variant = CodexControlVariant.Ghost, HorizontalAlignment = HorizontalAlignment.Stretch, HorizontalContentAlignment = HorizontalAlignment.Left }),
                                WithMotion(new CodexButton { Content = "Disabled item", Variant = CodexControlVariant.Ghost, IsEnabled = false, HorizontalAlignment = HorizontalAlignment.Stretch, HorizontalContentAlignment = HorizontalAlignment.Left })
                            }
                        }
                    })), 1, 0),
                At(StateTile(
                    "Overlay",
                    "Scrim primitive, warning status, content slot.",
                    new CodexOverlay
                    {
                        Height = 120,
                        Content = new StackPanel
                        {
                            Spacing = 8,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center,
                            Children =
                            {
                                new CodexBadge { Content = "overlay", Variant = CodexControlVariant.Warning },
                                new CodexText { Text = "Content remains inspectable", Role = CodexTextRole.Muted }
                            }
                        }
                    }), 0, 1),
                At(StateTile(
                    "Focus Ring",
                    "Default, focus target, disabled child.",
                    new CodexFocusRing
                    {
                        RingBrush = Brush(CodexSwitchResourceKeys.RingBrush),
                        RingThickness = new Thickness(2),
                        Content = WithMotion(new CodexTextBox { PlaceholderText = "Tab focus" })
                    },
                    new CodexFocusRing
                    {
                        RingBrush = Brush(CodexSwitchResourceKeys.RingBrush),
                        RingThickness = new Thickness(2),
                        Content = WithMotion(new CodexButton { Content = "Disabled focus", IsEnabled = false })
                    }), 1, 1)
            }
        };
    }

    private Control BuildOverlayOpenClosedMatrix()
    {
        return new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Star)
            },
            RowDefinitions =
            {
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Auto)
            },
            ColumnSpacing = 12,
            RowSpacing = 12,
            Children =
            {
                At(StateTile(
                    "Dialog open",
                    "Rendered surface with intent actions and animation badges.",
                    WithMotion(new CodexDialog
                    {
                        Title = "Confirm publish",
                        Description = "Open state: content, footer actions, border, radius, and foreground tokens are visible.",
                        Action = new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            Spacing = 8,
                            Children =
                            {
                                WithMotion(new CodexButton { Content = "Publish", Size = CodexControlSize.Small }),
                                WithMotion(new CodexButton { Content = "Cancel", Size = CodexControlSize.Small, Variant = CodexControlVariant.Secondary })
                            }
                        },
                        Content = new CodexText { Text = "Animation target: fade in surface, keep focus inside.", Role = CodexTextRole.Muted, TextWrapping = TextWrapping.Wrap }
                    }),
                    new CodexBadge { Content = "open", Variant = CodexControlVariant.Success },
                    new CodexBadge { Content = "animation 160ms", Variant = CodexControlVariant.Outline }), 0, 0),
                At(StateTile(
                    "Dialog closed",
                    "Closed state keeps a trigger, muted target slot, and disabled loading action visible.",
                    WithMotion(new CodexButton { Content = "Open dialog", Variant = CodexControlVariant.Outline }),
                    WithMotion(new CodexButton { Content = "Saving...", IsLoading = true, LoadingContent = "Saving", Size = CodexControlSize.Small }),
                    ClosedState("Closed dialog host")), 1, 0),
                At(StateTile(
                    "Popover open",
                    "Compact layer with focusable input, active command, and disabled row.",
                    WithMotion(new CodexPopover
                    {
                        Content = new StackPanel
                        {
                            Spacing = 8,
                            Children =
                            {
                                new CodexText { Text = "Token picker", Role = CodexTextRole.Subtitle },
                                WithMotion(new CodexTextBox { PlaceholderText = "Search tokens" }),
                                WithMotion(new CodexButton { Content = "Primary / Ring", Variant = CodexControlVariant.Ghost, HorizontalAlignment = HorizontalAlignment.Stretch, HorizontalContentAlignment = HorizontalAlignment.Left }),
                                WithMotion(new CodexButton { Content = "Disabled token", Variant = CodexControlVariant.Ghost, IsEnabled = false, HorizontalAlignment = HorizontalAlignment.Stretch, HorizontalContentAlignment = HorizontalAlignment.Left })
                            }
                        }
                    }),
                    new CodexBadge { Content = "open", Variant = CodexControlVariant.Success }), 0, 1),
                At(StateTile(
                    "Popover closed",
                    "Closed state has a trigger, hidden surface placeholder, and animation target callout.",
                    WithMotion(new CodexButton { Content = "Open popover", Variant = CodexControlVariant.Secondary }),
                    ClosedState("Closed popover layer"),
                    new CodexBadge { Content = "fade + translate target", Variant = CodexControlVariant.Warning }), 1, 1)
            }
        };
    }

    private Control BuildFeedbackStateMatrix()
    {
        return new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Star)
            },
            RowDefinitions =
            {
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Auto)
            },
            ColumnSpacing = 12,
            RowSpacing = 12,
            Children =
            {
                At(StateTile(
                    "Toast",
                    "Default, success, warning, destructive.",
                    Toast("Default", "Neutral feedback.", CodexControlVariant.Default),
                    Toast("Success", "Operation completed.", CodexControlVariant.Success),
                    Toast("Warning", "Review before saving.", CodexControlVariant.Warning),
                    Toast("Destructive", "Action failed.", CodexControlVariant.Destructive)), 0, 0),
                At(StateTile(
                    "Badge",
                    "Variants and disabled status chip.",
                    new WrapPanel
                    {
                        Children =
                        {
                            new CodexBadge { Content = "default", Margin = new Thickness(4) },
                            new CodexBadge { Content = "secondary", Variant = CodexControlVariant.Secondary, Margin = new Thickness(4) },
                            new CodexBadge { Content = "outline", Variant = CodexControlVariant.Outline, Margin = new Thickness(4) },
                            new CodexBadge { Content = "success", Variant = CodexControlVariant.Success, Margin = new Thickness(4) },
                            new CodexBadge { Content = "warning", Variant = CodexControlVariant.Warning, Margin = new Thickness(4) },
                            new CodexBadge { Content = "disabled", IsEnabled = false, Margin = new Thickness(4) }
                        }
                    }), 1, 0),
                At(StateTile(
                    "Alert",
                    "Default, destructive, success, and action slot.",
                    new CodexAlert { Title = "Heads up", Description = "You can add components to your app using the CLI.", Action = new CodexButton { Content = "View", Size = CodexControlSize.Small, Variant = CodexControlVariant.Outline } },
                    new CodexAlert { Title = "Error", Description = "Your session has expired.", Variant = CodexControlVariant.Destructive },
                    new CodexAlert { Title = "Published", Description = "The design tokens are synced.", Variant = CodexControlVariant.Success }), 0, 1),
                At(StateTile(
                    "Progress",
                    "Default, success-adjacent label, disabled.",
                    WithMotion(new CodexProgress { Minimum = 0, Maximum = 100, Value = 38 }),
                    WithMotion(new CodexProgress { Minimum = 0, Maximum = 100, Value = 82 }),
                    WithMotion(new CodexProgress { Minimum = 0, Maximum = 100, Value = 58, IsEnabled = false })), 0, 2),
                At(StateTile(
                    "Skeleton",
                    "Default, size variation, loading stack.",
                    new CodexSkeleton { Height = 12, Width = 260 },
                    new CodexSkeleton { Height = 16, Width = 190 },
                    new CodexSkeleton { Height = 38, Width = 300 },
                    new CodexBadge { Content = "shimmer token", Variant = CodexControlVariant.Success }), 1, 2)
            }
        };
    }

    private Control BuildFeedbackOperationalMatrix()
    {
        return new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Star)
            },
            RowDefinitions =
            {
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Auto)
            },
            ColumnSpacing = 12,
            RowSpacing = 12,
            Children =
            {
                At(StateTile(
                    "Toast open / closed",
                    "Open toast, closed placeholder, intent variants, and replayable animation.",
                    Toast("Open toast", "Success intent with a visible surface.", CodexControlVariant.Success),
                    ClosedState("Closed toast slot"),
                    BuildFeedbackMotionReplay()), 0, 0),
                At(StateTile(
                    "Badge intent",
                    "Default, success, warning, destructive, and disabled badges.",
                    new WrapPanel
                    {
                        Children =
                        {
                            new CodexBadge { Content = "default", Margin = new Thickness(4) },
                            new CodexBadge { Content = "success", Variant = CodexControlVariant.Success, Margin = new Thickness(4) },
                            new CodexBadge { Content = "warning", Variant = CodexControlVariant.Warning, Margin = new Thickness(4) },
                            new CodexBadge { Content = "destructive", Variant = CodexControlVariant.Destructive, Margin = new Thickness(4) },
                            new CodexBadge { Content = "disabled", IsEnabled = false, Margin = new Thickness(4) }
                        }
                    }), 1, 0),
                At(StateTile(
                    "Avatar fallback / loading",
                    "Avatar fallback, unavailable identity, and loading skeleton together.",
                    new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        Spacing = 10,
                        Children =
                        {
                            new CodexAvatar { Fallback = "CS" },
                            new CodexAvatar { Fallback = "UI" },
                            new CodexAvatar { Fallback = "NA", IsEnabled = false }
                        }
                    },
                    new StackPanel
                    {
                        Spacing = 6,
                        Children =
                        {
                            new CodexSkeleton { Width = 160, Height = 12 },
                            new CodexSkeleton { Width = 110, Height = 12 }
                        }
                    }), 0, 1),
                At(StateTile(
                    "Progress loading",
                    "Determinate, nearly complete, and disabled loading states.",
                    WithMotion(new CodexProgress { Minimum = 0, Maximum = 100, Value = 12 }),
                    WithMotion(new CodexProgress { Minimum = 0, Maximum = 100, Value = 64 }),
                    WithMotion(new CodexProgress { Minimum = 0, Maximum = 100, Value = 90, IsEnabled = false }),
                    new CodexBadge { Content = "loading", Variant = CodexControlVariant.Secondary }), 1, 1),
                At(StateTile(
                    "Skeleton loading",
                    "Skeleton stack sized like a real list item, plus animation target metadata.",
                    new CodexSkeleton { Width = 280, Height = 14 },
                    new CodexSkeleton { Width = 220, Height = 14 },
                    new CodexSkeleton { Width = 300, Height = 36 },
                    new CodexBadge { Content = "pulse / shimmer target", Variant = CodexControlVariant.Warning }), 0, 2),
                At(StateTile(
                    "Loading actions",
                    "Feedback often pairs loading controls with disabled or busy actions.",
                    WithMotion(new CodexButton { Content = "Loading", IsLoading = true, LoadingContent = "Loading" }),
                    WithMotion(new CodexButton { Content = "Disabled", IsEnabled = false, Variant = CodexControlVariant.Secondary }),
                    new CodexBadge { Content = "reduced motion required", Variant = CodexControlVariant.Outline }), 1, 2)
            }
        };
    }

    private Control BuildDataDisplayStateMatrix()
    {
        return new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Star)
            },
            RowDefinitions =
            {
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Auto)
            },
            ColumnSpacing = 12,
            RowSpacing = 12,
            Children =
            {
                At(StateTile(
                    "Avatar",
                    "Fallback, alternate initials, disabled wrapper.",
                    new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        Spacing = 10,
                        Children =
                        {
                            new CodexAvatar { Fallback = "CS" },
                            new CodexAvatar { Fallback = "UI" },
                            new CodexAvatar { Fallback = "NA", IsEnabled = false }
                        }
                    }), 0, 0),
                At(StateTile(
                    "Metrics",
                    "Default, success, warning labels.",
                    new Grid
                    {
                        ColumnDefinitions =
                        {
                            new ColumnDefinition(GridLength.Star),
                            new ColumnDefinition(GridLength.Star)
                        },
                        ColumnSpacing = 8,
                        RowSpacing = 8,
                        Children =
                        {
                            At(MetricTile("20", "styles", "included and guarded"), 0, 0),
                            At(MetricTile("0", "blocking gaps", "template residue"), 1, 0),
                            At(new CodexBadge { Content = "success", Variant = CodexControlVariant.Success }, 0, 1),
                            At(new CodexBadge { Content = "warning", Variant = CodexControlVariant.Warning }, 1, 1)
                        }
                    }), 1, 0),
                At(StateTile(
                    "Table",
                    "Header, row, disabled action.",
                    new CodexTable
                    {
                        Content = new StackPanel
                        {
                            Children =
                            {
                                TableRow("Name", "State", "Action", true),
                                TableRow("Button", "Ready", "Hover / focus guarded", false),
                                TableRow("Select", "Ready", "Popup + editable template owned", false),
                                new Border
                                {
                                    BorderBrush = Brush(CodexSwitchResourceKeys.BorderBrush),
                                    BorderThickness = new Thickness(0, 0, 0, 1),
                                    Padding = new Thickness(10, 8),
                                    Child = WithMotion(new CodexButton { Content = "Disabled row action", IsEnabled = false, Size = CodexControlSize.Small })
                                }
                            }
                        }
                    }), 0, 1),
                At(StateTile(
                    "Empty / Warning",
                    "Empty state, warning badge, disabled command.",
                    new CodexText { Text = "No rows match the current filter.", Role = CodexTextRole.Muted },
                    new CodexBadge { Content = "warning", Variant = CodexControlVariant.Warning },
                    WithMotion(new CodexButton { Content = "Export empty set", IsEnabled = false, Size = CodexControlSize.Small })), 1, 1)
            }
        };
    }

    private Control BuildDataUtilityMatrix()
    {
        return new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Star)
            },
            RowDefinitions =
            {
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Auto)
            },
            ColumnSpacing = 12,
            RowSpacing = 12,
            Children =
            {
                At(StateTile(
                    "Card",
                    "Used as a bounded data inspector, not a decorative page section.",
                    WithMotion(new CodexCard
                    {
                        Padding = new Thickness(14),
                        Content = new StackPanel
                        {
                            Spacing = 8,
                            Children =
                            {
                                new CodexText { Text = "Release health", Role = CodexTextRole.Subtitle },
                                new CodexText { Text = "Templates owned: 19 / 24", Role = CodexTextRole.Muted },
                                new CodexBadge { Content = "review", Variant = CodexControlVariant.Warning }
                            }
                        }
                    })), 0, 0),
                At(StateTile(
                    "Separator",
                    "Horizontal and vertical separators keep dense data blocks scannable.",
                    new CodexSeparator(),
                    new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        Spacing = 12,
                        Children =
                        {
                            new CodexText { Text = "Active", Role = CodexTextRole.Muted },
                            new CodexSeparator { Orientation = Orientation.Vertical, Height = 24 },
                            new CodexText { Text = "Archived", Role = CodexTextRole.Muted }
                        }
                    }), 1, 0),
                At(StateTile(
                    "Typography",
                    "Title, subtitle, body, muted, and code roles in a data context.",
                    new CodexText { Text = "Component audit", Role = CodexTextRole.Title },
                    new CodexText { Text = "Navigation / Tabs", Role = CodexTextRole.Subtitle },
                    new CodexText { Text = "The row uses semantic tokens and compact density.", Role = CodexTextRole.Body, TextWrapping = TextWrapping.Wrap },
                    new CodexText { Text = "Muted helper copy", Role = CodexTextRole.Muted },
                    new CodexText { Text = "PART_Indicator", Role = CodexTextRole.Code }), 0, 1),
                At(StateTile(
                    "Table with support primitives",
                    "Table rows can sit beside cards, badges, separators, and type roles.",
                    BuildTablePreview()), 1, 1)
            }
        };
    }

    private Control BuildUtilitiesStateMatrix()
    {
        var echartsTheme = CodexSwitchEChartsTheme.FromCurrentTheme();

        return new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Star)
            },
            RowDefinitions =
            {
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Auto)
            },
            ColumnSpacing = 12,
            RowSpacing = 12,
            Children =
            {
                At(StateTile(
                    "Card",
                    "Default surface, disabled content.",
                    WithMotion(new CodexCard
                    {
                        Padding = new Thickness(14),
                        Content = new StackPanel
                        {
                            Spacing = 8,
                            Children =
                            {
                                new CodexText { Text = "Card surface", Role = CodexTextRole.Subtitle },
                                WithMotion(new CodexButton { Content = "Disabled action", IsEnabled = false, Size = CodexControlSize.Small })
                            }
                        }
                    })), 0, 0),
                At(StateTile(
                    "Separator",
                    "Horizontal, vertical, muted spacing.",
                    new CodexSeparator(),
                    new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        Spacing = 12,
                        Children =
                        {
                            new CodexText { Text = "Left", Role = CodexTextRole.Muted },
                            new CodexSeparator { Orientation = Orientation.Vertical, Height = 26 },
                            new CodexText { Text = "Right", Role = CodexTextRole.Muted }
                        }
                    }), 1, 0),
                At(StateTile(
                    "Typography",
                    "Title, subtitle, body, muted, code.",
                    new CodexText { Text = "Title role", Role = CodexTextRole.Title },
                    new CodexText { Text = "Subtitle role", Role = CodexTextRole.Subtitle },
                    new CodexText { Text = "Body role", Role = CodexTextRole.Body },
                    new CodexText { Text = "Muted role", Role = CodexTextRole.Muted },
                    new CodexText { Text = "<CodexText Role=\"Code\" />", Role = CodexTextRole.Code }), 0, 1),
                At(StateTile(
                    "ECharts Theme",
                    "Token-derived adapter values.",
                    new CodexText { Text = $"Background: {echartsTheme.BackgroundColor}", Role = CodexTextRole.Code, TextWrapping = TextWrapping.Wrap },
                    new CodexText { Text = $"Text: {echartsTheme.TextColor}", Role = CodexTextRole.Code, TextWrapping = TextWrapping.Wrap },
                    new CodexText { Text = $"Series: {string.Join(", ", echartsTheme.Color.Take(3))}", Role = CodexTextRole.Code, TextWrapping = TextWrapping.Wrap }), 1, 1)
            }
        };
    }

    private Control BuildCoverageMatrix()
    {
        var headers = new[] { "Component", "Template", "Tokens", "Variants", "States", "Motion", "Docs example", "Tests" };
        var rows = new[]
        {
            new[] { "Button", "yes", "yes", "yes", "yes", "yes", "yes", "yes" },
            new[] { "TextBox", "yes", "yes", "part", "yes", "yes", "yes", "yes" },
            new[] { "Select", "yes", "yes", "part", "yes", "yes", "yes", "yes" },
            new[] { "Checkbox", "yes", "yes", "na", "yes", "yes", "yes", "yes" },
            new[] { "Radio", "yes", "yes", "na", "yes", "yes", "yes", "yes" },
            new[] { "Switch", "yes", "yes", "na", "yes", "yes", "yes", "yes" },
            new[] { "Slider", "yes", "yes", "na", "yes", "yes", "yes", "yes" },
            new[] { "Tabs", "yes", "yes", "na", "yes", "yes", "yes", "yes" },
            new[] { "Menu", "yes", "yes", "na", "yes", "yes", "yes", "yes" },
            new[] { "ContextMenu", "yes", "yes", "na", "yes", "yes", "yes", "yes" },
            new[] { "Command", "yes", "yes", "na", "yes", "yes", "yes", "yes" },
            new[] { "Collapsible", "yes", "yes", "na", "yes", "yes", "yes", "yes" },
            new[] { "Dialog", "yes", "yes", "na", "yes", "yes", "yes", "yes" },
            new[] { "Popover", "yes", "yes", "na", "yes", "yes", "yes", "yes" },
            new[] { "Overlay", "yes", "yes", "na", "yes", "yes", "yes", "yes" },
            new[] { "FocusRing", "yes", "yes", "na", "yes", "yes", "yes", "yes" },
            new[] { "Toast", "yes", "yes", "yes", "yes", "yes", "yes", "yes" },
            new[] { "Badge", "yes", "yes", "yes", "yes", "yes", "yes", "yes" },
            new[] { "Spinner", "yes", "yes", "na", "yes", "yes", "yes", "yes" },
            new[] { "Progress", "yes", "yes", "na", "yes", "yes", "yes", "yes" },
            new[] { "Skeleton", "yes", "yes", "na", "yes", "yes", "yes", "yes" },
            new[] { "Avatar", "yes", "yes", "na", "yes", "yes", "yes", "yes" },
            new[] { "Table", "yes", "yes", "na", "yes", "yes", "yes", "yes" },
            new[] { "Card", "yes", "yes", "na", "yes", "yes", "yes", "yes" },
            new[] { "Separator", "yes", "yes", "na", "yes", "na", "yes", "yes" },
            new[] { "ECharts", "na", "yes", "part", "part", "na", "yes", "yes" }
        };

        var grid = new Grid { ColumnSpacing = 0, RowSpacing = 0 };
        grid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(128)));
        for (var i = 1; i < headers.Length; i++)
        {
            grid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(104)));
        }

        AddCoverageRow(grid, 0, headers, true);
        for (var i = 0; i < rows.Length; i++)
        {
            AddCoverageRow(grid, i + 1, rows[i], false);
        }

        return new ScrollViewer
        {
            MinHeight = CoverageMatrixMinHeight,
            MaxHeight = CoverageMatrixMaxHeight,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            Content = grid
        };
    }

    private Control BuildDefaultStyleResidueAudit()
    {
        return new StackPanel
        {
            Spacing = 10,
            Children =
            {
                TemplateAuditRow("Forms", "TextBox", "Own border/content host/watermark/selection brushes; current style still leans on Avalonia text editor template.", "high"),
                TemplateAuditRow("Forms", "Select", "Own toggle chrome, popup, selected presenter, dropdown item hover/selected/disabled states.", "high"),
                TemplateAuditRow("Forms", "Slider", "Own track, fill, thumb, pointerover/focus/disabled visuals, and tick/keyboard states.", "high"),
                TemplateAuditRow("Forms", "Checkbox / Radio / Switch", "Indicators are custom; finish indeterminate, disabled foreground, pointerover, and checked motion.", "medium"),
                TemplateAuditRow("Navigation", "Tabs", "Own header panel, selected indicator, disabled tab, content presenter, and focus ring.", "high"),
                TemplateAuditRow("Navigation", "NavigationMenu", "Own trigger list, shared viewport, indicator, directional page transition, and viewport size animation.", "high"),
                TemplateAuditRow("Navigation", "Menu", "Own menu item template, submenu popup, separator, checked/icon area, gesture text, and disabled rows.", "high"),
                TemplateAuditRow("Navigation", "ContextMenu", "Own right-click surface, side-aware entry transform, submenu popup, radio/check indicators, and shortcut rail.", "high"),
                TemplateAuditRow("Navigation", "Command", "Own input slot, row item states, empty/loading/no-result content, and keyboard highlight.", "medium"),
                TemplateAuditRow("Navigation", "Collapsible", "Own trigger, chevron, clipped content host, measured-height open/close animation, and disabled state.", "medium"),
                TemplateAuditRow("Overlay", "Dialog / Popover / Overlay", "Own scrim, host placement, close affordance, elevation, outside-click, escape, and reduced-motion states.", "medium"),
                TemplateAuditRow("Feedback", "Spinner / Progress / Skeleton", "Own spinner glyph/motion, progress track/indicator, and skeleton shimmer/pulse behavior.", "medium"),
                TemplateAuditRow("Feedback", "Toast", "Own viewport stack, close button, timer/progress affordance, enter/exit, and keyboard dismiss state.", "medium"),
                TemplateAuditRow("Data Display", "Table", "Own header, row, cell, hover, selected, sorted, empty, and disabled row templates.", "high"),
                TemplateAuditRow("Data Display", "Avatar", "Own image clip, fallback, status dot, missing-image, disabled, and size variants.", "medium"),
                TemplateAuditRow("Utilities", "Card / Separator / Text", "Mostly owned through simple primitives; keep typography and separator orientation free of platform defaults.", "low"),
                TemplateAuditRow("Utilities", "ECharts", "Mirror chart tooltip, legend, axis, grid, emphasis, and disabled series colors from theme tokens.", "medium")
            }
        };
    }

    private Control BuildThemeTokenReviewExample()
    {
        return new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Star)
            },
            ColumnSpacing = 12,
            Children =
            {
                StateTile(
                    "Theme tokens",
                    "Semantic resources that component templates should reference instead of local brushes.",
                    TokenSwatch("Primary", CodexSwitchResourceKeys.PrimaryBrush, "default action / selected state"),
                    TokenSwatch("Ring", CodexSwitchResourceKeys.RingBrush, "focus and active outline"),
                    TokenSwatch("Popover", CodexSwitchResourceKeys.PopoverBrush, "menu, command, overlay surface"),
                    TokenSwatch("Border", CodexSwitchResourceKeys.BorderBrush, "row, card, separator border")),
                At(StateTile(
                    "Custom theme example",
                    "Runtime override example used by the Docs theme buttons.",
                    CodeBlock("""
                    var options = CodexSwitchThemeOptions.ShadcnDefault with
                    {
                        Radius = 8,
                        Density = CodexSwitchDensity.Comfortable,
                        CustomPalette = CodexSwitchPalette.Light with
                        {
                            Primary = "#FF2563EB",
                            Ring = "#FF60A5FA",
                            Accent = "#FFEFF6FF"
                        }
                    };
                    """),
                    new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        Spacing = 8,
                        Children =
                        {
                            ThemeAction("Apply custom", CodexSwitchThemeMode.Custom, CodexControlVariant.Outline),
                            ThemeAction("Reset light", CodexSwitchThemeMode.Light, CodexControlVariant.Secondary)
                        }
                    },
                    new CodexBadge { Content = "theme token review", Variant = CodexControlVariant.Success }), 1, 0)
            }
        };
    }

    private Control ClosedState(string text)
    {
        return new Border
        {
            Background = Brush(CodexSwitchResourceKeys.MutedBrush),
            BorderBrush = Brush(CodexSwitchResourceKeys.BorderBrush),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(6),
            Padding = new Thickness(10, 8),
            Opacity = 0.7,
            Child = new CodexText
            {
                Text = text,
                Role = CodexTextRole.Muted,
                TextWrapping = TextWrapping.Wrap
            }
        };
    }

    private Control StateTile(string title, string caption, params Control[] controls)
    {
        var stack = new StackPanel { Spacing = 8 };
        stack.Children.Add(new CodexText { Text = title, Role = CodexTextRole.Subtitle });
        stack.Children.Add(new CodexText
        {
            Text = caption,
            Role = CodexTextRole.Muted,
            TextWrapping = TextWrapping.Wrap
        });

        foreach (var control in controls)
        {
            stack.Children.Add(control);
        }

        return new Border
        {
            Background = Brush(CodexSwitchResourceKeys.CardBrush),
            BorderBrush = Brush(CodexSwitchResourceKeys.BorderBrush),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(6),
            Padding = new Thickness(12),
            Child = stack
        };
    }

    private void AddCoverageRow(Grid grid, int rowIndex, string[] cells, bool header)
    {
        grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));

        for (var columnIndex = 0; columnIndex < cells.Length; columnIndex++)
        {
            var cell = columnIndex == 0 || header
                ? MatrixTextCell(cells[columnIndex], header)
                : MatrixStatusCell(cells[columnIndex]);

            Grid.SetColumn(cell, columnIndex);
            Grid.SetRow(cell, rowIndex);
            grid.Children.Add(cell);
        }
    }

    private Control MatrixTextCell(string text, bool header)
    {
        return new Border
        {
            MinHeight = 34,
            BorderBrush = Brush(CodexSwitchResourceKeys.BorderBrush),
            BorderThickness = new Thickness(0, 0, 1, 1),
            Padding = new Thickness(8, 6),
            Background = header ? Brush(CodexSwitchResourceKeys.MutedBrush) : Brushes.Transparent,
            Child = new CodexText
            {
                Text = text,
                Role = header ? CodexTextRole.Subtitle : CodexTextRole.Body,
                TextWrapping = TextWrapping.Wrap
            }
        };
    }

    private Control MatrixStatusCell(string status)
    {
        var variant = status switch
        {
            "yes" => CodexControlVariant.Success,
            "part" => CodexControlVariant.Warning,
            "gap" => CodexControlVariant.Destructive,
            _ => CodexControlVariant.Secondary
        };

        return new Border
        {
            MinHeight = 34,
            BorderBrush = Brush(CodexSwitchResourceKeys.BorderBrush),
            BorderThickness = new Thickness(0, 0, 1, 1),
            Padding = new Thickness(8, 5),
            Child = new CodexBadge
            {
                Content = status,
                Variant = variant,
                HorizontalAlignment = HorizontalAlignment.Left
            }
        };
    }

    private Control TemplateAuditRow(string category, string component, string parts, string priority)
    {
        var variant = priority switch
        {
            "high" => CodexControlVariant.Destructive,
            "medium" => CodexControlVariant.Warning,
            _ => CodexControlVariant.Secondary
        };

        return new Border
        {
            BorderBrush = Brush(CodexSwitchResourceKeys.BorderBrush),
            BorderThickness = new Thickness(0, 0, 0, 1),
            Padding = new Thickness(0, 0, 0, 10),
            Child = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition(new GridLength(112)),
                    new ColumnDefinition(new GridLength(150)),
                    new ColumnDefinition(GridLength.Star),
                    new ColumnDefinition(new GridLength(92))
                },
                ColumnSpacing = 10,
                Children =
                {
                    new CodexBadge { Content = category, Variant = CodexControlVariant.Outline },
                    At(new CodexText { Text = component, Role = CodexTextRole.Subtitle, TextWrapping = TextWrapping.Wrap }, 1, 0),
                    At(new CodexText { Text = parts, Role = CodexTextRole.Muted, TextWrapping = TextWrapping.Wrap }, 2, 0),
                    At(new CodexBadge { Content = priority, Variant = variant }, 3, 0)
                }
            }
        };
    }

    private Control BuildMotionBaseline()
    {
        return new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Star)
            },
            RowDefinitions =
            {
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Auto)
            },
            ColumnSpacing = 14,
            RowSpacing = 14,
            Children =
            {
                At(new StackPanel
                {
                    Spacing = 8,
                    Children =
                    {
                        new CodexText { Text = "Hover / Press", Role = CodexTextRole.Subtitle },
                        WithMotion(new CodexButton { Content = "Hover me" }),
                        WithMotion(new CodexButton { Content = "Ghost action", Variant = CodexControlVariant.Ghost })
                    }
                }, 0, 0),
                At(new StackPanel
                {
                    Spacing = 8,
                    Children =
                    {
                        new CodexText { Text = "Focus", Role = CodexTextRole.Subtitle },
                        WithMotion(new CodexTextBox { PlaceholderText = "Tab into this field" })
                    }
                }, 1, 0),
                At(new StackPanel
                {
                    Spacing = 8,
                    Children =
                    {
                        new CodexText { Text = "Checked", Role = CodexTextRole.Subtitle },
                        new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            Spacing = 12,
                            Children =
                            {
                                WithMotion(new CodexSwitch { IsChecked = true }),
                                new CodexCheckBox { Content = "Selected", IsChecked = true }
                            }
                        }
                    }
                }, 0, 1),
                At(new StackPanel
                {
                    Spacing = 8,
                    Children =
                    {
                        new CodexText { Text = "Feedback", Role = CodexTextRole.Subtitle },
                        Toast("Saved", "Replayable in the Feedback category.", CodexControlVariant.Success)
                    }
                }, 1, 1)
            }
        };
    }

    private Control BuildButtonVariants()
    {
        var variants = new[]
        {
            CodexControlVariant.Default,
            CodexControlVariant.Secondary,
            CodexControlVariant.Outline,
            CodexControlVariant.Ghost,
            CodexControlVariant.Destructive,
            CodexControlVariant.Success,
            CodexControlVariant.Warning
        };

        var panel = new WrapPanel();
        foreach (var variant in variants)
        {
            panel.Children.Add(WithMotion(new CodexButton
            {
                Content = variant.ToString(),
                Variant = variant,
                Margin = new Thickness(4),
                HorizontalAlignment = HorizontalAlignment.Stretch
            }));
        }

        panel.Children.Add(WithMotion(new CodexButton
        {
            Content = "Disabled",
            IsEnabled = false,
            Margin = new Thickness(4),
            HorizontalAlignment = HorizontalAlignment.Stretch
        }));

        panel.Children.Add(WithMotion(new CodexButton
        {
            Content = "+",
            Size = CodexControlSize.Icon,
            Variant = CodexControlVariant.Secondary,
            Margin = new Thickness(4)
        }));

        return panel;
    }

    private Control BuildSonnerDemo()
    {
        CodexSonnerService.Clear();
        CodexSonnerService.Toast("Event has been created", new CodexSonnerOptions
        {
            Description = "Sunday, December 03, 2023 at 9:00 AM",
            Action = new CodexSonnerAction("Undo", () => { }),
            Duration = TimeSpan.Zero
        });
        CodexSonnerService.Success("Build passed", new CodexSonnerOptions
        {
            Description = "All target frameworks share the same component surface.",
            Duration = TimeSpan.Zero
        });

        var defaultButton = WithMotion(new CodexButton
        {
            Content = "Show toast",
            Variant = CodexControlVariant.Outline,
            Size = CodexControlSize.Small
        });
        defaultButton.Click += (_, _) => CodexSonnerService.Toast("Event has been created", new CodexSonnerOptions
        {
            Description = "Sunday, December 03, 2023 at 9:00 AM",
            Action = new CodexSonnerAction("Undo", () => CodexSonnerService.Info("Undo queued", new CodexSonnerOptions
            {
                Description = "The demo action fired from the toast button.",
                Duration = TimeSpan.FromSeconds(2)
            }))
        });

        var successButton = WithMotion(new CodexButton
        {
            Content = "Success",
            Variant = CodexControlVariant.Success,
            Size = CodexControlSize.Small
        });
        successButton.Click += (_, _) => CodexSonnerService.Success("Saved", new CodexSonnerOptions
        {
            Description = "Changes are now available in the current theme."
        });

        var warningButton = WithMotion(new CodexButton
        {
            Content = "Warning",
            Variant = CodexControlVariant.Warning,
            Size = CodexControlSize.Small
        });
        warningButton.Click += (_, _) => CodexSonnerService.Warning("Token drift", new CodexSonnerOptions
        {
            Description = "Review the custom palette before publishing."
        });

        var errorButton = WithMotion(new CodexButton
        {
            Content = "Error",
            Variant = CodexControlVariant.Destructive,
            Size = CodexControlSize.Small
        });
        errorButton.Click += (_, _) => CodexSonnerService.Error("Validation failed", new CodexSonnerOptions
        {
            Description = "One required field is missing.",
            Cancel = new CodexSonnerAction("Dismiss", CodexSonnerService.Clear)
        });

        return new Grid
        {
            MinHeight = 300,
            RowDefinitions =
            {
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Star)
            },
            RowSpacing = 16,
            Children =
            {
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 8,
                    Children =
                    {
                        defaultButton,
                        successButton,
                        warningButton,
                        errorButton
                    }
                },
                At(new Border
                {
                    Background = Brush(CodexSwitchResourceKeys.MutedBrush),
                    BorderBrush = Brush(CodexSwitchResourceKeys.BorderBrush),
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(8),
                    Padding = new Thickness(14),
                    Child = new Grid
                    {
                        Children =
                        {
                            new StackPanel
                            {
                                Spacing = 6,
                                Children =
                                {
                                    new CodexText { Text = "App root viewport", Role = CodexTextRole.Subtitle },
                                    new CodexText
                                    {
                                        Text = "The host stays pinned to the selected corner while service calls append to the stack.",
                                        Role = CodexTextRole.Muted,
                                        TextWrapping = TextWrapping.Wrap,
                                        MaxWidth = 360
                                    }
                                }
                            },
                            new CodexSonner
                            {
                                Position = CodexSonnerPosition.BottomRight,
                                RichColors = true,
                                VisibleToasts = 3,
                                Offset = new Thickness(0)
                            }
                        }
                    }
                }, 0, 1)
            }
        };
    }

    private Control BuildFeedbackMotionReplay()
    {
        var toast = Toast("Saved", "Opacity transition is attached in Docs only.", CodexControlVariant.Success);
        toast.Opacity = 0.58;

        var replay = WithMotion(new CodexButton
        {
            Content = "Replay toast",
            Size = CodexControlSize.Small
        });
        replay.Click += (_, _) => toast.Opacity = toast.Opacity < 0.95 ? 1 : 0.58;

        return new StackPanel
        {
            Spacing = 12,
            Children =
            {
                toast,
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 8,
                    Children =
                    {
                        replay,
                        new CodexBadge { Content = "feedback motion", Variant = CodexControlVariant.Outline },
                        new CodexBadge { Content = "tokenized motion", Variant = CodexControlVariant.Success }
                    }
                }
            }
        };
    }

    private Control BuildTablePreview()
    {
        var rows = new Control[]
        {
            TableRow("Component", "Category", "Motion state", true),
            TableRow("CodexButton", "Forms", "hover / pressed / focus", false),
            TableRow("CodexSwitch", "Forms", "checked track + thumb", false),
            TableRow("CodexToast", "Feedback", "surface transition + variants", false),
            TableRow("CodexTabs", "Navigation", "trigger + indicator transition", false),
            TableRow("CodexNavigationMenu", "Navigation", "viewport slide + size transition", false)
        };
        var stack = new StackPanel();
        foreach (var row in rows)
            stack.Children.Add(row);

        return new CodexTable
        {
            Content = stack
        };
    }

    private Control TableRow(string first, string second, string third, bool header)
    {
        return new Border
        {
            BorderBrush = Brush(CodexSwitchResourceKeys.BorderBrush),
            BorderThickness = new Thickness(0, 0, 0, 1),
            Padding = new Thickness(10, 8),
            Child = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition(new GridLength(1.25, GridUnitType.Star)),
                    new ColumnDefinition(GridLength.Star),
                    new ColumnDefinition(new GridLength(1.4, GridUnitType.Star))
                },
                ColumnSpacing = 12,
                Children =
                {
                    At(new CodexText { Text = first, Role = header ? CodexTextRole.Subtitle : CodexTextRole.Body }, 0, 0),
                    At(new CodexText { Text = second, Role = header ? CodexTextRole.Subtitle : CodexTextRole.Muted }, 1, 0),
                    At(new CodexText { Text = third, Role = header ? CodexTextRole.Subtitle : CodexTextRole.Muted, TextWrapping = TextWrapping.Wrap }, 2, 0)
                }
            }
        };
    }

    private Control CategorySection(string title, string caption, params Control[] cases)
    {
        var stack = new StackPanel { Spacing = 14 };
        stack.Children.Add(new StackPanel
        {
            Spacing = 4,
            Children =
            {
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 8,
                    Children =
                    {
                        new CodexText { Text = title, Role = CodexTextRole.Title },
                        new CodexBadge { Content = $"{cases.Length} cases", Variant = CodexControlVariant.Secondary }
                    }
                },
                new CodexText
                {
                    Text = caption,
                    Role = CodexTextRole.Muted,
                    TextWrapping = TextWrapping.Wrap,
                    MaxWidth = 840
                }
            }
        });

        foreach (var item in cases)
        {
            stack.Children.Add(item);
        }

        return stack;
    }

    private Control Case(string title, string caption, Control preview, string code)
    {
        var grid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Auto)
            },
            RowSpacing = 14
        };

        grid.Children.Add(new StackPanel
        {
            Spacing = 4,
            Children =
            {
                new CodexText { Text = title, Role = CodexTextRole.Subtitle },
                new CodexText
                {
                    Text = caption,
                    Role = CodexTextRole.Muted,
                    TextWrapping = TextWrapping.Wrap,
                    MaxWidth = 840
                }
            }
        });

        var body = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(new GridLength(360))
            },
            ColumnSpacing = 16
        };
        Grid.SetRow(body, 1);
        body.Children.Add(At(preview, 0, 0));
        body.Children.Add(At(CodeBlock(code), 1, 0));
        grid.Children.Add(body);

        return new Border
        {
            Background = Brush(CodexSwitchResourceKeys.CardBrush),
            BorderBrush = Brush(CodexSwitchResourceKeys.BorderBrush),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(18),
            Child = grid
        };
    }

    private Control PreviewSurface(Control content)
    {
        return new Border
        {
            Background = Brush(CodexSwitchResourceKeys.BackgroundBrush),
            BorderBrush = Brush(CodexSwitchResourceKeys.BorderBrush),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(6),
            Padding = new Thickness(16),
            Child = content
        };
    }

    private Control CodeBlock(string code)
    {
        return new Border
        {
            Background = Brush(CodexSwitchResourceKeys.MutedBrush),
            BorderBrush = Brush(CodexSwitchResourceKeys.BorderBrush),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(6),
            Padding = new Thickness(12),
            Child = new CodexText
            {
                Text = code.Trim(),
                Role = CodexTextRole.Code,
                FontSize = 12,
                LineHeight = 18,
                TextWrapping = TextWrapping.Wrap
            }
        };
    }

    private Control ThemeTokenStrip()
    {
        var panel = new StackPanel { Spacing = 8 };
        foreach (var item in new[]
                 {
                     ("Primary", CodexSwitchResourceKeys.PrimaryBrush),
                     ("Secondary", CodexSwitchResourceKeys.SecondaryBrush),
                     ("Destructive", CodexSwitchResourceKeys.DestructiveBrush),
                     ("Success", CodexSwitchResourceKeys.SuccessBrush),
                     ("Warning", CodexSwitchResourceKeys.WarningBrush)
                 })
        {
            panel.Children.Add(new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition(new GridLength(96)),
                    new ColumnDefinition(GridLength.Star)
                },
                ColumnSpacing = 8,
                Children =
                {
                    new CodexText { Text = item.Item1, Role = CodexTextRole.Muted, VerticalAlignment = VerticalAlignment.Center },
                    At(new Border
                    {
                        Height = 24,
                        CornerRadius = new CornerRadius(5),
                        Background = Brush(item.Item2),
                        BorderBrush = Brush(CodexSwitchResourceKeys.BorderBrush),
                        BorderThickness = new Thickness(1)
                    }, 1, 0)
                }
            });
        }

        return panel;
    }

    private Control MetricTile(string value, string label, string caption)
    {
        return new Border
        {
            Background = Brush(CodexSwitchResourceKeys.CardBrush),
            BorderBrush = Brush(CodexSwitchResourceKeys.BorderBrush),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(6),
            Padding = new Thickness(14),
            Child = new StackPanel
            {
                Spacing = 5,
                Children =
                {
                    new CodexText { Text = value, Role = CodexTextRole.Title },
                    new CodexText { Text = label, Role = CodexTextRole.Subtitle },
                    new CodexText { Text = caption, Role = CodexTextRole.Muted, TextWrapping = TextWrapping.Wrap }
                }
            }
        };
    }

    private Control TokenSwatch(string name, string resourceKey, string note)
    {
        return new Border
        {
            Background = Brush(CodexSwitchResourceKeys.CardBrush),
            BorderBrush = Brush(CodexSwitchResourceKeys.BorderBrush),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(6),
            Padding = new Thickness(10),
            Child = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition(new GridLength(42)),
                    new ColumnDefinition(GridLength.Star)
                },
                ColumnSpacing = 10,
                Children =
                {
                    new Border
                    {
                        Width = 34,
                        Height = 34,
                        CornerRadius = new CornerRadius(5),
                        Background = Brush(resourceKey),
                        BorderBrush = Brush(CodexSwitchResourceKeys.BorderBrush),
                        BorderThickness = new Thickness(1)
                    },
                    At(new StackPanel
                    {
                        Spacing = 2,
                        Children =
                        {
                            new CodexText { Text = name, Role = CodexTextRole.Subtitle },
                            new CodexText { Text = note, Role = CodexTextRole.Muted, TextWrapping = TextWrapping.Wrap }
                        }
                    }, 1, 0)
                }
            }
        };
    }

    private Control MotionToken(string name, string duration, string easing, string target)
    {
        return new Border
        {
            Background = Brush(CodexSwitchResourceKeys.CardBrush),
            BorderBrush = Brush(CodexSwitchResourceKeys.BorderBrush),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(6),
            Padding = new Thickness(12),
            Child = new StackPanel
            {
                Spacing = 8,
                Children =
                {
                    new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        Spacing = 8,
                        Children =
                        {
                            new CodexBadge { Content = name, Variant = CodexControlVariant.Outline },
                            new CodexBadge { Content = duration, Variant = CodexControlVariant.Secondary }
                        }
                    },
                    new CodexText { Text = easing, Role = CodexTextRole.Code },
                    new CodexText { Text = target, Role = CodexTextRole.Muted, TextWrapping = TextWrapping.Wrap }
                }
            }
        };
    }

    private Control Field(string label, Control input)
    {
        return new StackPanel
        {
            Spacing = 6,
            Children =
            {
                new CodexText { Text = label, Role = CodexTextRole.Muted },
                input
            }
        };
    }

    private CodexToast Toast(string title, string caption, CodexControlVariant variant)
    {
        return WithMotion(new CodexToast
        {
            Variant = variant,
            Content = new StackPanel
            {
                Spacing = 4,
                Children =
                {
                    new CodexText { Text = title, Role = CodexTextRole.Subtitle },
                    new CodexText { Text = caption, Role = CodexTextRole.Muted, TextWrapping = TextWrapping.Wrap }
                }
            }
        });
    }

    private Control NavDot(string resourceKey)
    {
        return new Border
        {
            Width = 8,
            Height = 8,
            CornerRadius = new CornerRadius(4),
            Background = Brush(resourceKey),
            VerticalAlignment = VerticalAlignment.Center
        };
    }

    private CodexSidebarMenuButton SidebarMenuButton(
        string label,
        bool isActive,
        string? badge,
        string iconBrush,
        bool isEnabled = true)
    {
        return WithMotion(new CodexSidebarMenuButton
        {
            Content = new TextBlock { Text = label, TextTrimming = TextTrimming.CharacterEllipsis },
            Icon = NavDot(iconBrush),
            Badge = string.IsNullOrWhiteSpace(badge) ? null : new CodexSidebarMenuBadge { Content = badge },
            IsActive = isActive,
            IsEnabled = isEnabled,
            HorizontalAlignment = HorizontalAlignment.Stretch
        });
    }

    private Control BuildDocsSideNavigationPreview(bool includeDisabled = false)
    {
        var items = new List<CodexSidebarMenuItem>
        {
            new() { Content = SidebarMenuButton("Overview", true, null, CodexSwitchResourceKeys.PrimaryBrush) },
            new() { Content = SidebarMenuButton("Tokens", false, null, CodexSwitchResourceKeys.MutedForegroundBrush) },
            new() { Content = SidebarMenuButton("Forms", false, "3", CodexSwitchResourceKeys.MutedForegroundBrush) },
            new()
            {
                Content = new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition(GridLength.Star),
                        new ColumnDefinition(GridLength.Auto)
                    },
                    Children =
                    {
                        SidebarMenuButton("Feedback", false, null, CodexSwitchResourceKeys.MutedForegroundBrush),
                        At(new CodexSidebarMenuAction
                        {
                            Content = "...",
                            IsShowOnHover = true,
                            HorizontalAlignment = HorizontalAlignment.Right
                        }, 1, 0)
                    }
                }
            },
            new() { Content = SidebarMenuButton("Design Review", false, null, CodexSwitchResourceKeys.MutedForegroundBrush) }
        };

        if (includeDisabled)
            items.Add(new CodexSidebarMenuItem
            {
                Content = SidebarMenuButton(
                    "Disabled category",
                    false,
                    null,
                    CodexSwitchResourceKeys.MutedForegroundBrush,
                    isEnabled: false)
            });

        return new CodexSidebarGroup
        {
            Content = new StackPanel
            {
                Spacing = 4,
                Children =
                {
                    new CodexSidebarGroupLabel { Content = "Components" },
                    new CodexSidebarGroupContent
                    {
                        Content = new CodexSidebarMenu
                        {
                            ItemsSource = items,
                            HorizontalAlignment = HorizontalAlignment.Stretch
                        }
                    }
                }
            }
        };
    }

    private CodexButton ThemeAction(string label, CodexSwitchThemeMode mode, CodexControlVariant variant)
    {
        var button = WithMotion(new CodexButton
        {
            Content = label,
            Variant = variant,
            Size = CodexControlSize.Small
        });
        button.Click += (_, _) => ApplyTheme(mode);
        return button;
    }

    private Control InfoPanel(string title, string caption)
    {
        return new Border
        {
            Background = Brush(CodexSwitchResourceKeys.CardBrush),
            BorderBrush = Brush(CodexSwitchResourceKeys.BorderBrush),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(14),
            Child = new StackPanel
            {
                Spacing = 8,
                Children =
                {
                    new CodexText { Text = title, Role = CodexTextRole.Subtitle },
                    new CodexText
                    {
                        Text = caption,
                        Role = CodexTextRole.Muted,
                        TextWrapping = TextWrapping.Wrap
                    }
                }
            }
        };
    }

    private Control ReviewList(params (string State, string Text)[] items)
    {
        var stack = new StackPanel { Spacing = 10 };
        foreach (var item in items)
        {
            var variant = item.State switch
            {
                "done" => CodexControlVariant.Success,
                "gap" => CodexControlVariant.Warning,
                _ => CodexControlVariant.Secondary
            };

            stack.Children.Add(new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition(new GridLength(86)),
                    new ColumnDefinition(GridLength.Star)
                },
                ColumnSpacing = 10,
                Children =
                {
                    new CodexBadge { Content = item.State, Variant = variant },
                    At(new CodexText
                    {
                        Text = item.Text,
                        Role = CodexTextRole.Muted,
                        TextWrapping = TextWrapping.Wrap
                    }, 1, 0)
                }
            });
        }

        return stack;
    }

    private T WithMotion<T>(T control)
        where T : TemplatedControl
    {
        control.Transitions = new Transitions
        {
            new DoubleTransition
            {
                Property = Visual.OpacityProperty,
                Duration = TimeSpan.FromMilliseconds(120),
                Easing = new CubicEaseOut()
            },
            new BrushTransition
            {
                Property = TemplatedControl.BackgroundProperty,
                Duration = TimeSpan.FromMilliseconds(140),
                Easing = new CubicEaseOut()
            },
            new BrushTransition
            {
                Property = TemplatedControl.BorderBrushProperty,
                Duration = TimeSpan.FromMilliseconds(140),
                Easing = new CubicEaseOut()
            },
            new ThicknessTransition
            {
                Property = TemplatedControl.BorderThicknessProperty,
                Duration = TimeSpan.FromMilliseconds(140),
                Easing = new CubicEaseOut()
            }
        };

        return control;
    }

    private Control At(Control control, int column, int row)
    {
        Grid.SetColumn(control, column);
        Grid.SetRow(control, row);
        return control;
    }

    private void Navigate(string category)
    {
        if (_activeCategory == category)
        {
            return;
        }

        _activeCategory = category;
        RefreshSidebarSelection();
        _topbar.Child = BuildTopbar();
        _scroll.Content = BuildContent();
        _scroll.Offset = new Vector(0, 0);
        RefreshChrome();
    }

    private void RefreshSidebarSelection()
    {
        foreach (var category in Categories)
        {
            if (_navItemsByCategory.TryGetValue(category.Title, out var item))
            {
                item.IsActive = category.Title == _activeCategory;
            }
        }
    }

    private void ApplyTheme(CodexSwitchThemeMode mode)
    {
        _currentMode = mode;
        var options = mode == CodexSwitchThemeMode.Custom
            ? CodexSwitchThemeOptions.ShadcnDefault with
            {
                Radius = 8,
                Density = CodexSwitchDensity.Comfortable,
                CustomPalette = CodexSwitchPalette.Light with
                {
                    Primary = "#FF2563EB",
                    PrimaryForeground = "#FFFFFFFF",
                    Ring = "#FF60A5FA",
                    Accent = "#FFEFF6FF",
                    AccentForeground = "#FF172554",
                    Secondary = "#FFEFF6FF",
                    SecondaryForeground = "#FF172554"
                }
            }
            : CodexSwitchThemeOptions.ShadcnDefault;

        CodexSwitchThemeManager.Current.Apply(Application.Current!, mode, options);
        _sidebar.Child = BuildSidebar();
        _topbar.Child = BuildTopbar();
        _scroll.Content = BuildContent();
        RefreshChrome();
    }

    private void RefreshChrome()
    {
        Background = Brush(CodexSwitchResourceKeys.BackgroundBrush);
        _sidebar.Background = Brush(CodexSwitchResourceKeys.MutedBrush);
        _sidebar.BorderBrush = Brush(CodexSwitchResourceKeys.BorderBrush);
        _topbar.Background = Brush(CodexSwitchResourceKeys.BackgroundBrush);
        _topbar.BorderBrush = Brush(CodexSwitchResourceKeys.BorderBrush);
        _scroll.Background = Brush(CodexSwitchResourceKeys.BackgroundBrush);
    }

    private IBrush Brush(string key)
    {
        if (Application.Current?.Resources.TryGetResource(key, null, out var value) == true && value is IBrush brush)
        {
            return brush;
        }

        return Brushes.Transparent;
    }

    private sealed record NavCategory(string Title, string[] Items);
}
