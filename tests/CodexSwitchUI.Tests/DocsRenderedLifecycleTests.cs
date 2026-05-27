using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Styling;
using Avalonia.VisualTree;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Docs.Controls;
using CodexSwitchUI.Docs;
using CodexSwitchUI.Themes;
using CodexSwitchUI.Tokens;
using Xunit;

namespace CodexSwitchUI.Tests;

public class DocsRenderedLifecycleTests
{
    private static readonly string[] RepresentativePages =
    [
        "overview.getting-started",
        "layout.application-shell",
        "layout.sidebar",
        "layout.resizable",
        "forms.button",
        "forms.button-group",
        "forms.input-group",
        "forms.input-otp",
        "forms.label",
        "forms.icon-button",
        "forms.checkbox",
        "forms.toggle",
        "forms.toggle-group",
        "forms.radio-group",
        "forms.select",
        "forms.combobox",
        "forms.native-select",
        "forms.calendar",
        "forms.date-picker",
        "feedback.toast",
        "navigation.breadcrumb",
        "navigation.segmented-control",
        "navigation.accordion",
        "navigation.menubar",
        "navigation.dropdown",
        "navigation.command",
        "overlay.dialog",
        "overlay.alert-dialog",
        "overlay.sheet",
        "overlay.drawer",
        "overlay.hover-card",
        "data.table",
        "data.data-table",
        "data.pinned-table",
        "data.carousel",
        "data.chart",
        "data.bar-chart",
        "data.line-chart",
        "data.item",
        "data.aspect-ratio",
        "data.image-icon",
        "data.provider-card",
        "data.pagination",
        "data.usage-pie-chart",
        "data.usage-trend-chart",
        "primitives.direction",
        "primitives.overlay",
        "tokens.motion"
    ];

    private static readonly CodexSwitchThemeMode[] RenderModes =
    [
        CodexSwitchThemeMode.Light,
        CodexSwitchThemeMode.Dark,
        CodexSwitchThemeMode.Custom
    ];

    private static readonly string[] MultiCasePages =
    [
        "layout.application-shell",
        "layout.sidebar",
        "layout.sidebar-primitives",
        "layout.section",
        "layout.resizable",
        "forms.button",
        "forms.button-group",
        "forms.input-group",
        "forms.input-otp",
        "forms.label",
        "forms.icon-button",
        "forms.field",
        "forms.select",
        "forms.combobox",
        "forms.native-select",
        "forms.calendar",
        "forms.date-picker",
        "forms.split-button",
        "forms.textbox",
        "forms.textarea",
        "forms.checkbox",
        "forms.radio",
        "forms.radio-group",
        "forms.switch",
        "forms.toggle",
        "forms.toggle-group",
        "forms.slider",
        "feedback.alert",
        "feedback.badge",
        "feedback.avatar",
        "feedback.empty-state",
        "feedback.toast",
        "feedback.sonner",
        "feedback.spinner",
        "feedback.progress",
        "feedback.skeleton",
        "navigation.tabs",
        "navigation.breadcrumb",
        "navigation.accordion",
        "navigation.menubar",
        "navigation.collapsible",
        "navigation.separator",
        "navigation.kbd",
        "navigation.dropdown",
        "overlay.dialog",
        "overlay.alert-dialog",
        "overlay.sheet",
        "overlay.drawer",
        "overlay.command-dialog",
        "overlay.popover",
        "overlay.tooltip",
        "overlay.hover-card",
        "data.card",
        "data.item",
        "data.aspect-ratio",
        "data.carousel",
        "data.chart",
        "data.bar-chart",
        "data.line-chart",
        "data.metric",
        "data.image-icon",
        "data.table",
        "data.data-table",
        "data.provider-card",
        "data.pinned-table",
        "data.pagination",
        "data.scroll-area",
        "data.ranked-bar-chart",
        "data.usage-pie-chart",
        "data.usage-trend-chart",
        "primitives.typography",
        "primitives.focus-ring",
        "primitives.direction",
        "primitives.overlay",
        "tokens.motion"
    ];

    private static IReadOnlyList<string> VisualFingerprintPages => AllRegisteredPageIds();

    private const string UpdateSnapshotsEnvironmentVariable = "CODEXSWITCHUI_UPDATE_DOCS_VISUAL_SNAPSHOTS";

    [Fact]
    public async Task DocsNavigationAndDarkThemeSwitchDoNotDetachAnimatedPageContent()
    {
        await using var session = HeadlessUnitTestSession.StartNew(typeof(OverlayRenderedLifecycleTestApp), AvaloniaTestIsolationLevel.PerTest);
        await session.Dispatch(() =>
        {
            EnsureCodexTheme();

            var window = new MainWindow();

            try
            {
                window.Show();

                InvokePrivate(window, "NavigateToPage", "navigation.dropdown");
                InvokePrivate(window, "NavigateToPage", "overlay.dialog");
                InvokePrivate(window, "NavigateToPage", "data.table");
                InvokePrivate(window, "NavigateToPage", "data.data-table");
                InvokePrivate(window, "NavigateToPage", "data.chart");
                InvokePrivate(window, "NavigateToPage", "data.pagination");
                InvokePrivate(window, "ApplyTheme", CodexSwitchThemeMode.Dark);
                InvokePrivate(window, "NavigateToPage", "tokens.motion");

                Assert.Equal(ThemeVariant.Dark, Application.Current?.RequestedThemeVariant);
            }
            finally
            {
                window.Close();
            }
        }, CancellationToken.None);
    }

    [Fact]
    public async Task DocsRepresentativePagesRenderScreenshotsAcrossThemes()
    {
        await using var session = HeadlessUnitTestSession.StartNew(typeof(OverlayRenderedLifecycleTestApp), AvaloniaTestIsolationLevel.PerTest);
        await session.Dispatch(() =>
        {
            EnsureCodexTheme();

            var window = new MainWindow();

            try
            {
                window.Show();

                foreach (var mode in RenderModes)
                {
                    InvokePrivate(window, "ApplyTheme", mode);
                    AssertThemeResources(window, mode);

                    foreach (var pageId in RepresentativePages)
                    {
                        InvokePrivate(window, "NavigateToPage", pageId);
                        CaptureAndAssertRenderedFrame(window, $"{mode}:{pageId}");
                    }
                }
            }
            finally
            {
                window.Close();
            }
        }, CancellationToken.None);
    }

    [Fact]
    public async Task DocsMultiCaseExamplesExpandInlineCodeAndRenderAcrossThemes()
    {
        await using var session = HeadlessUnitTestSession.StartNew(typeof(OverlayRenderedLifecycleTestApp), AvaloniaTestIsolationLevel.PerTest);
        await session.Dispatch(() =>
        {
            EnsureCodexTheme();

            var window = new MainWindow();

            try
            {
                window.Show();

                foreach (var mode in RenderModes)
                {
                    InvokePrivate(window, "ApplyTheme", mode);

                    foreach (var pageId in MultiCasePages)
                    {
                        InvokePrivate(window, "NavigateToPage", pageId);
                        window.UpdateLayout();

                        var pageRoot = GetCachedPageRoot(window, pageId);
                        var codeToggleButtons = LogicalDescendants<CodexButton>(pageRoot)
                            .Where(button => button.Content as string is "Show code" or "Hide code")
                            .ToArray();

                        var allButtonLabels = string.Join(
                            ", ",
                            LogicalDescendants<CodexButton>(pageRoot)
                                .Select(button => button.Content?.ToString() ?? "<null>"));
                        Assert.True(
                            codeToggleButtons.Length >= 2,
                            $"{pageId} should expose multiple inline example code toggles. Examples={GetActivePageExampleCount(window)} Buttons=[{allButtonLabels}]");

                        foreach (var button in codeToggleButtons.Where(button => button.Content as string == "Show code"))
                        {
                            InvokeProtectedClick(button);
                        }

                        window.UpdateLayout();

                        Assert.All(codeToggleButtons, button => Assert.Equal("Hide code", button.Content));

                        var visibleInlineCodeBlocks = LogicalDescendants<DocsCodeBlock>(pageRoot)
                            .Where(block => block.IsVisible)
                            .ToArray();
                        Assert.Equal(codeToggleButtons.Length, visibleInlineCodeBlocks.Length);
                        Assert.Contains(visibleInlineCodeBlocks, block => block.Title.EndsWith("States.axaml", StringComparison.Ordinal));
                        Assert.DoesNotContain(visibleInlineCodeBlocks, block => block.Code.Contains("Missing AXAML sample", StringComparison.Ordinal));
                        Assert.All(visibleInlineCodeBlocks, block =>
                            Assert.NotEmpty(VisibleDescendants<SelectableTextBlock>(block)));

                        CaptureAndAssertRenderedFrame(window, $"{mode}:{pageId}:expanded-code");
                    }
                }
            }
            finally
            {
                window.Close();
            }
        }, CancellationToken.None);
    }

    [Fact]
    public async Task DocsImageIconPageLoadsLinkedAvaresResources()
    {
        await using var session = HeadlessUnitTestSession.StartNew(typeof(OverlayRenderedLifecycleTestApp), AvaloniaTestIsolationLevel.PerTest);
        await session.Dispatch(() =>
        {
            EnsureCodexTheme();

            var window = new MainWindow();

            try
            {
                window.Show();
                InvokePrivate(window, "NavigateToPage", "data.image-icon");
                window.UpdateLayout();

                var pageRoot = GetCachedPageRoot(window, "data.image-icon");
                var imageIcons = VisualDescendants<CodexImageIcon>(pageRoot)
                    .Where(icon => icon.Path?.Contains("avares://CodexSwitchUI.Docs/Assets/icons/", StringComparison.Ordinal) == true)
                    .ToArray();

                Assert.True(imageIcons.Length >= 3, "Image icon docs should render linked avares provider icons.");
                Assert.Contains(imageIcons, icon => icon.Path?.EndsWith("openai.png", StringComparison.Ordinal) == true && icon.Source is not null);
                Assert.Contains(imageIcons, icon => icon.Path?.EndsWith("claude.png", StringComparison.Ordinal) == true && icon.Source is not null);
                Assert.Contains(imageIcons, icon => icon.Path?.EndsWith("codex-color.png", StringComparison.Ordinal) == true && icon.Source is not null);

                CaptureAndAssertRenderedFrame(window, "data.image-icon:linked-resources");
            }
            finally
            {
                window.Close();
            }
        }, CancellationToken.None);
    }

    [Fact]
    public async Task DocsCommandPageRendersStandaloneCommandInputAnatomy()
    {
        await using var session = HeadlessUnitTestSession.StartNew(typeof(OverlayRenderedLifecycleTestApp), AvaloniaTestIsolationLevel.PerTest);
        await session.Dispatch(() =>
        {
            EnsureCodexTheme();

            var window = new MainWindow();

            try
            {
                window.Show();
                InvokePrivate(window, "NavigateToPage", "navigation.command");
                window.UpdateLayout();

                var pageRoot = GetCachedPageRoot(window, "navigation.command");
                Assert.Contains(
                    LogicalDescendants<CodexCommandInput>(pageRoot),
                    input => input.Text == "provider" && input.PlaceholderText == "Filter commands...");
                Assert.Contains(
                    LogicalDescendants<CodexCommandLoading>(pageRoot),
                    loading => loading.Content as string == "Refreshing command results...");
                Assert.Contains(
                    LogicalDescendants<CodexCommandEmpty>(pageRoot),
                    empty => empty.Content as string == "No matching commands.");

                CaptureAndAssertRenderedFrame(window, "navigation.command:anatomy");
            }
            finally
            {
                window.Close();
            }
        }, CancellationToken.None);
    }

    [Fact]
    public async Task DocsVisualFingerprintsMatchBaselineAcrossThemes()
    {
        var actual = new Dictionary<string, string>(StringComparer.Ordinal);

        await using var session = HeadlessUnitTestSession.StartNew(typeof(OverlayRenderedLifecycleTestApp), AvaloniaTestIsolationLevel.PerTest);
        await session.Dispatch(() =>
        {
            EnsureCodexTheme();

            var window = new MainWindow();

            try
            {
                window.Show();

                foreach (var mode in RenderModes)
                {
                    ApplyVisualSnapshotTheme(window, mode);

                    foreach (var pageId in VisualFingerprintPages)
                    {
                        InvokePrivate(window, "NavigateToPage", pageId);
                        window.UpdateLayout();
                        ExpandInlineCodeExamples(window, pageId);
                        actual[$"{mode}:{pageId}:expanded"] = CaptureVisualFingerprint(window);
                    }
                }
            }
            finally
            {
                window.Close();
            }
        }, CancellationToken.None);

        if (ShouldUpdateVisualSnapshots())
        {
            WriteVisualSnapshotBaseline(actual);
            return;
        }

        var baseline = ReadVisualSnapshotBaseline();
        Assert.Equal(baseline.Signatures.Keys.Order(StringComparer.Ordinal), actual.Keys.Order(StringComparer.Ordinal));

        foreach (var (key, expected) in baseline.Signatures)
        {
            Assert.True(actual.TryGetValue(key, out var fingerprint), $"Missing visual fingerprint for {key}.");
            var distance = VisualFingerprintDistance(expected, fingerprint);
            Assert.True(
                distance <= 160,
                $"{key} visual fingerprint drifted by {distance}, above the allowed tolerance. Expected={expected} Actual={fingerprint}");
        }
    }

    private static void EnsureCodexTheme()
    {
        var application = Application.Current;
        Assert.NotNull(application);

        if (!application.Styles.OfType<CodexSwitchTheme>().Any())
        {
            application.Styles.Add(new CodexSwitchTheme());
        }

        CodexSwitchThemeManager.Current.Apply(application, CodexSwitchThemeMode.Light);
    }

    private static void CaptureAndAssertRenderedFrame(Window window, string label)
    {
        using var frame = Assert.IsAssignableFrom<Bitmap>(window.CaptureRenderedFrame());
        Assert.InRange(frame.PixelSize.Width, 1300, 1600);
        Assert.InRange(frame.PixelSize.Height, 860, 980);

        using var stream = new MemoryStream();
        frame.Save(stream);

        var png = stream.ToArray();
        Assert.True(png.Length > 4096, $"{label} should save a non-empty rendered PNG.");
        Assert.Equal([137, 80, 78, 71, 13, 10, 26, 10], png[..8]);

        using var decoded = new Bitmap(new MemoryStream(png));
        Assert.Equal(frame.PixelSize, decoded.PixelSize);

        var topLeft = ReadPixel(frame, x: 4, y: 4);
        Assert.True(topLeft.A > 0, $"{label} should paint an opaque shell background.");

        var luminance = (topLeft.R * 0.2126) + (topLeft.G * 0.7152) + (topLeft.B * 0.0722);
        if (CodexSwitchThemeManager.Current.Mode == CodexSwitchThemeMode.Dark)
        {
            Assert.True(luminance < 80, $"{label} should render a dark shell background.");
        }
        else
        {
            Assert.True(luminance > 180, $"{label} should render a light shell background.");
        }
    }

    private static string CaptureVisualFingerprint(Window window)
    {
        using var frame = Assert.IsAssignableFrom<Bitmap>(window.CaptureRenderedFrame());
        var pixels = CopyPixels(frame);
        const int columns = 12;
        const int rows = 8;
        var width = frame.PixelSize.Width;
        var height = frame.PixelSize.Height;
        var builder = new StringBuilder($"{width}x{height}:");

        for (var row = 0; row < rows; row++)
        {
            for (var column = 0; column < columns; column++)
            {
                var startX = column * width / columns;
                var endX = (column + 1) * width / columns;
                var startY = row * height / rows;
                var endY = (row + 1) * height / rows;
                var count = 0;
                long red = 0;
                long green = 0;
                long blue = 0;

                for (var y = startY; y < endY; y++)
                {
                    for (var x = startX; x < endX; x++)
                    {
                        var offset = ((y * width) + x) * 4;
                        blue += pixels[offset];
                        green += pixels[offset + 1];
                        red += pixels[offset + 2];
                        count++;
                    }
                }

                AppendQuantizedColor(builder, red / count, green / count, blue / count);
            }
        }

        return builder.ToString();
    }

    private static Color ReadPixel(Bitmap frame, int x, int y)
    {
        var pixels = CopyPixels(frame);
        var width = frame.PixelSize.Width;

        var offset = ((y * width) + x) * 4;
        return Color.FromArgb(
            pixels[offset + 3],
            pixels[offset + 2],
            pixels[offset + 1],
            pixels[offset]);
    }

    private static byte[] CopyPixels(Bitmap frame)
    {
        var width = frame.PixelSize.Width;
        var height = frame.PixelSize.Height;
        var pixels = new byte[width * height * 4];

        using var framebuffer = new TestLockedFramebuffer(pixels, frame.PixelSize);
        frame.CopyPixels(framebuffer);
        return pixels;
    }

    private static void AssertThemeResources(Window window, CodexSwitchThemeMode mode)
    {
        var application = Application.Current;
        Assert.NotNull(application);

        Assert.Equal(
            mode == CodexSwitchThemeMode.Dark ? ThemeVariant.Dark : ThemeVariant.Light,
            application.RequestedThemeVariant);

        var shellBackground = Assert.IsType<SolidColorBrush>(window.Background);
        var resourceBackground = ResourceBrush(CodexSwitchResourceKeys.BackgroundBrush);
        Assert.Equal(resourceBackground.Color, shellBackground.Color);

        if (mode == CodexSwitchThemeMode.Dark)
        {
            Assert.True(IsDark(resourceBackground.Color));
            return;
        }

        Assert.True(IsLight(resourceBackground.Color));

        if (mode == CodexSwitchThemeMode.Custom)
        {
            var primary = ResourceBrush(CodexSwitchResourceKeys.PrimaryBrush);
            var ring = ResourceBrush(CodexSwitchResourceKeys.RingBrush);

            Assert.Equal(Color.Parse("#FF2563EB"), primary.Color);
            Assert.Equal(Color.Parse("#FF2563EB"), ring.Color);
        }
    }

    private static void ApplyVisualSnapshotTheme(MainWindow window, CodexSwitchThemeMode mode)
    {
        InvokePrivate(window, "ApplyTheme", mode);

        var options = CodexSwitchThemeOptions.ShadcnDefault with
        {
            ReducedMotion = true
        };

        if (mode == CodexSwitchThemeMode.Custom)
        {
            options = options with
            {
                CustomPalette = CodexSwitchPalette.Light with
                {
                    Primary = "#FF2563EB",
                    Ring = "#FF2563EB"
                },
                Radius = 6
            };
        }

        CodexSwitchThemeManager.Current.Apply(Application.Current!, mode, options);
        InvokePrivate(window, "RefreshThemeSurfaces");
    }

    private static SolidColorBrush ResourceBrush(string key)
    {
        var application = Application.Current;
        Assert.NotNull(application);

        return Assert.IsType<SolidColorBrush>(application.Resources[key]);
    }

    private static bool IsDark(Color color)
    {
        return color.R < 80 && color.G < 80 && color.B < 80;
    }

    private static bool IsLight(Color color)
    {
        return color.R > 180 && color.G > 180 && color.B > 180;
    }

    private static void InvokePrivate(object target, string methodName, params object[] args)
    {
        var method = target.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.NotNull(method);
        method.Invoke(target, args);
    }

    private static IReadOnlyList<T> VisibleDescendants<T>(Visual root)
        where T : Visual
    {
        return root.GetVisualDescendants()
            .OfType<T>()
            .Where(descendant => descendant.IsEffectivelyVisible)
            .ToArray();
    }

    private static IReadOnlyList<T> VisualDescendants<T>(Visual root)
        where T : Visual
    {
        return root.GetVisualDescendants()
            .OfType<T>()
            .ToArray();
    }

    private static IReadOnlyList<T> LogicalDescendants<T>(ILogical root)
        where T : class, ILogical
    {
        return root.GetLogicalDescendants()
            .OfType<T>()
            .ToArray();
    }

    private static Control GetCachedPageRoot(MainWindow window, string pageId)
    {
        var field = typeof(MainWindow).GetField("_pageContentById", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.NotNull(field);

        var pages = Assert.IsAssignableFrom<Dictionary<string, Control>>(field.GetValue(window));
        Assert.True(pages.TryGetValue(pageId, out var pageRoot), $"Expected cached docs page root for {pageId}.");
        return pageRoot;
    }

    private static void ExpandInlineCodeExamples(MainWindow window, string pageId)
    {
        var pageRoot = GetCachedPageRoot(window, pageId);
        var codeToggleButtons = LogicalDescendants<CodexButton>(pageRoot)
            .Where(button => button.Content as string is "Show code" or "Hide code")
            .ToArray();

        Assert.NotEmpty(codeToggleButtons);

        foreach (var button in codeToggleButtons.Where(button => button.Content as string == "Show code"))
        {
            InvokeProtectedClick(button);
        }

        window.UpdateLayout();
        Assert.All(codeToggleButtons, button => Assert.Equal("Hide code", button.Content));
    }

    private static int GetActivePageExampleCount(MainWindow window)
    {
        var field = typeof(MainWindow).GetField("_activePage", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.NotNull(field);

        var activePage = field.GetValue(window);
        Assert.NotNull(activePage);

        var examples = activePage.GetType().GetProperty("Examples")?.GetValue(activePage);
        return Assert.IsAssignableFrom<IReadOnlyCollection<object>>(examples).Count;
    }

    private static IReadOnlyList<string> AllRegisteredPageIds()
    {
        var field = typeof(MainWindow).GetField("Categories", BindingFlags.Static | BindingFlags.NonPublic);
        Assert.NotNull(field);

        var categories = Assert.IsAssignableFrom<System.Collections.IEnumerable>(field.GetValue(null));
        var pageIds = new List<string>();

        foreach (var category in categories)
        {
            Assert.NotNull(category);

            var pagesProperty = category.GetType().GetProperty("Pages");
            Assert.NotNull(pagesProperty);

            var pages = Assert.IsAssignableFrom<System.Collections.IEnumerable>(pagesProperty.GetValue(category));
            foreach (var page in pages)
            {
                Assert.NotNull(page);

                var idProperty = page.GetType().GetProperty("Id");
                Assert.NotNull(idProperty);

                pageIds.Add(Assert.IsType<string>(idProperty.GetValue(page)));
            }
        }

        Assert.True(pageIds.Count >= 57, $"Expected broad Docs page visual coverage, found {pageIds.Count} pages.");
        Assert.Equal(pageIds.Count, pageIds.Distinct(StringComparer.Ordinal).Count());
        return pageIds;
    }

    private static void InvokeProtectedClick(CodexButton button)
    {
        var method = typeof(CodexButton).GetMethod("OnClick", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.NotNull(method);
        method.Invoke(button, []);
    }

    private static void AppendQuantizedColor(StringBuilder builder, long red, long green, long blue)
    {
        builder.Append(ToHexNibble(red / 16));
        builder.Append(ToHexNibble(green / 16));
        builder.Append(ToHexNibble(blue / 16));
    }

    private static char ToHexNibble(long value)
    {
        var nibble = Math.Clamp((int)value, 0, 15);
        return (char)(nibble < 10 ? '0' + nibble : 'a' + (nibble - 10));
    }

    private static int VisualFingerprintDistance(string expected, string actual)
    {
        var expectedSeparator = expected.IndexOf(':', StringComparison.Ordinal);
        var actualSeparator = actual.IndexOf(':', StringComparison.Ordinal);
        Assert.True(expectedSeparator > 0, "Expected visual fingerprint should include dimensions.");
        Assert.True(actualSeparator > 0, "Actual visual fingerprint should include dimensions.");
        Assert.Equal(expected[..expectedSeparator], actual[..actualSeparator]);

        var expectedCells = expected[(expectedSeparator + 1)..];
        var actualCells = actual[(actualSeparator + 1)..];
        Assert.Equal(expectedCells.Length, actualCells.Length);

        var distance = 0;
        for (var index = 0; index < expectedCells.Length; index++)
        {
            var delta = Math.Abs(FromHexNibble(expectedCells[index]) - FromHexNibble(actualCells[index]));
            distance += Math.Max(0, delta - 1);
        }

        return distance;
    }

    private static int FromHexNibble(char value)
    {
        if (value is >= '0' and <= '9')
        {
            return value - '0';
        }

        if (value is >= 'a' and <= 'f')
        {
            return 10 + value - 'a';
        }

        throw new FormatException($"Invalid visual fingerprint nibble: {value}");
    }

    private static bool ShouldUpdateVisualSnapshots()
    {
        return string.Equals(
            Environment.GetEnvironmentVariable(UpdateSnapshotsEnvironmentVariable),
            "1",
            StringComparison.Ordinal);
    }

    private static DocsVisualFingerprintBaseline ReadVisualSnapshotBaseline()
    {
        var path = VisualSnapshotBaselinePath();
        Assert.True(File.Exists(path), $"Missing docs visual fingerprint baseline: {path}");

        var baseline = JsonSerializer.Deserialize<DocsVisualFingerprintBaseline>(File.ReadAllText(path), JsonOptions());
        Assert.NotNull(baseline);
        Assert.NotNull(baseline.Signatures);
        Assert.Equal(1, baseline.Version);
        return baseline;
    }

    private static void WriteVisualSnapshotBaseline(IReadOnlyDictionary<string, string> signatures)
    {
        var path = VisualSnapshotBaselinePath();
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);

        var baseline = new DocsVisualFingerprintBaseline
        {
            Version = 1,
            Signatures = signatures.OrderBy(pair => pair.Key, StringComparer.Ordinal)
                .ToDictionary(pair => pair.Key, pair => pair.Value, StringComparer.Ordinal)
        };
        File.WriteAllText(path, JsonSerializer.Serialize(baseline, JsonOptions()) + Environment.NewLine);
    }

    private static JsonSerializerOptions JsonOptions()
    {
        return new JsonSerializerOptions
        {
            WriteIndented = true
        };
    }

    private static string VisualSnapshotBaselinePath()
    {
        return Path.Combine(TestRepository.FindRoot(), "tests", "CodexSwitchUI.Tests", "Snapshots", "DocsVisualFingerprints.json");
    }

    private sealed class DocsVisualFingerprintBaseline
    {
        public int Version { get; init; }

        public Dictionary<string, string> Signatures { get; init; } = [];
    }

    private sealed class TestLockedFramebuffer(byte[] pixels, PixelSize size) : ILockedFramebuffer
    {
        private readonly GCHandle _handle = GCHandle.Alloc(pixels, GCHandleType.Pinned);

        public IntPtr Address => _handle.AddrOfPinnedObject();

        public PixelSize Size { get; } = size;

        public int RowBytes => Size.Width * 4;

        public Vector Dpi { get; } = new(96, 96);

        public PixelFormat Format => PixelFormats.Bgra8888;

        public AlphaFormat AlphaFormat => AlphaFormat.Premul;

        public void Dispose()
        {
            if (_handle.IsAllocated)
            {
                _handle.Free();
            }
        }
    }
}
