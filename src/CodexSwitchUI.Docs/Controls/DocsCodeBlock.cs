using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input.Platform;
using Avalonia.Layout;
using Avalonia.Media;
using CodexSwitchUI.Controls;

namespace CodexSwitchUI.Docs.Controls;

public sealed class DocsCodeBlock : Border
{
    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<DocsCodeBlock, string>(nameof(Title), "Example.axaml");

    public static readonly StyledProperty<string> CodeProperty =
        AvaloniaProperty.Register<DocsCodeBlock, string>(nameof(Code), string.Empty);

    private readonly TextBlock _titleBlock;
    private readonly TextBlock _lineNumbers;
    private readonly SelectableTextBlock _codeText;
    private readonly CodexButton _copyButton;

    static DocsCodeBlock()
    {
        TitleProperty.Changed.AddClassHandler<DocsCodeBlock>((block, _) => block.Render());
        CodeProperty.Changed.AddClassHandler<DocsCodeBlock>((block, _) => block.Render());
    }

    public DocsCodeBlock()
    {
        Background = new SolidColorBrush(Color.Parse("#0B1020"));
        BorderBrush = new SolidColorBrush(Color.Parse("#27324A"));
        BorderThickness = new Thickness(1);
        CornerRadius = new CornerRadius(8);
        ClipToBounds = true;

        _titleBlock = new TextBlock
        {
            Text = Title,
            Foreground = new SolidColorBrush(Color.Parse("#CBD5E1")),
            FontFamily = new FontFamily("Menlo, Consolas, monospace"),
            FontSize = 12,
            VerticalAlignment = VerticalAlignment.Center
        };

        _lineNumbers = new TextBlock
        {
            Foreground = new SolidColorBrush(Color.Parse("#64748B")),
            FontFamily = new FontFamily("Menlo, Consolas, monospace"),
            FontSize = 12,
            TextAlignment = TextAlignment.Right,
            Padding = new Thickness(0, 2, 12, 2),
            IsHitTestVisible = false
        };

        _codeText = new SelectableTextBlock
        {
            Foreground = new SolidColorBrush(Color.Parse("#D6E4FF")),
            FontFamily = new FontFamily("Menlo, Consolas, monospace"),
            FontSize = 12,
            Padding = new Thickness(0, 2, 18, 2),
            TextWrapping = TextWrapping.NoWrap
        };
        Grid.SetColumn(_codeText, 1);

        _copyButton = new CodexButton
        {
            Content = "Copy",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Secondary
        };
        _copyButton.Click += async (_, _) => await CopyCode();

        Child = BuildChrome();
        Render();
    }

    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string Code
    {
        get => GetValue(CodeProperty);
        set => SetValue(CodeProperty, value);
    }

    private Control BuildChrome()
    {
        var root = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Star)
            }
        };

        var titlebarContent = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Auto),
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Auto)
            }
        };

        titlebarContent.Children.Add(new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 6,
            VerticalAlignment = VerticalAlignment.Center,
            Children =
            {
                Dot("#F87171"),
                Dot("#FBBF24"),
                Dot("#34D399")
            }
        });

        Grid.SetColumn(_titleBlock, 1);
        titlebarContent.Children.Add(_titleBlock);

        Grid.SetColumn(_copyButton, 2);
        titlebarContent.Children.Add(_copyButton);

        var titlebar = new Border
        {
            Height = 42,
            Background = new SolidColorBrush(Color.Parse("#10172A")),
            Padding = new Thickness(14, 0),
            Child = titlebarContent
        };

        var scroll = new ScrollViewer
        {
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            MaxHeight = 420,
            Content = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition(new GridLength(48)),
                    new ColumnDefinition(GridLength.Auto)
                },
                Children =
                {
                    _lineNumbers,
                    _codeText
                }
            }
        };
        Grid.SetRow(scroll, 1);

        root.Children.Add(titlebar);
        root.Children.Add(scroll);
        return root;
    }

    private static Border Dot(string color)
    {
        return new Border
        {
            Width = 10,
            Height = 10,
            CornerRadius = new CornerRadius(5),
            Background = new SolidColorBrush(Color.Parse(color))
        };
    }

    private void Render()
    {
        _titleBlock.Text = Title;
        var normalizedCode = Code.Replace("\r\n", "\n");
        var lines = normalizedCode.Split('\n');
        var lineCount = Math.Max(lines.Length, 1);
        var lineNumbers = new string[lineCount];
        for (var index = 0; index < lineCount; index++)
        {
            lineNumbers[index] = (index + 1).ToString();
        }

        _lineNumbers.Text = string.Join('\n', lineNumbers);
        _codeText.Text = string.IsNullOrEmpty(normalizedCode) ? " " : normalizedCode;
    }

    private async Task CopyCode()
    {
        var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
        if (clipboard is not null)
        {
            await clipboard.SetTextAsync(Code);
            _copyButton.Content = "Copied";
            return;
        }

        _copyButton.Content = "Unavailable";
    }
}
