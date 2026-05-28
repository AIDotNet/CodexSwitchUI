using Avalonia.Controls;
using Avalonia.Layout;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;
using System.Windows.Input;

public static class PaginationInteractionSample
{
    public static Control BuildPaginationInteractionPreview()
    {
        var eventStatus = Muted("PageChanged: interact with page buttons, action buttons, keyboard, or host jump.");
        var interactive = new CodexPagination
        {
            Page = 9,
            PageCount = 42,
            SiblingCount = 2,
            BoundaryCount = 1
        };
        interactive.PageChanged += (_, args) =>
        {
            eventStatus.Text = $"PageChanged: {args.Source} {args.OldPage} -> {args.NewPage}.";
        };

        var hostJump = new CodexButton
        {
            Content = "Host jump",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Outline,
            HorizontalAlignment = HorizontalAlignment.Left
        };
        hostJump.Click += (_, _) => interactive.SelectPage(interactive.Page == 21 ? 9 : 21);

        return new StackPanel
        {
            Spacing = 10,
            Children =
            {
                Muted("Home/End, arrow keys, primary action releases, page buttons, and host calls emit source-aware PageChanged events."),
                interactive,
                eventStatus,
                hostJump,
                new CodexPagination { Page = 3, PageCount = 8, IsCompact = true, ShowFirstLast = false, Size = CodexControlSize.Small },
                new CodexPagination { Page = 6, PageCount = 18, IsLoading = true },
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 8,
                    Children =
                    {
                        Muted("Command blocked"),
                        new CodexPaginationPageButton
                        {
                            Content = "7",
                            Page = 7,
                            Command = new SampleCommand(() => eventStatus.Text = "Blocked page executed", () => false)
                        }
                    }
                },
                new CodexPagination { Page = 18, PageCount = 18, BoundaryCount = 2 }
            }
        };
    }

    private static CodexText Muted(string text)
    {
        return new CodexText
        {
            Role = CodexTextRole.Muted,
            Text = text
        };
    }

    private sealed class SampleCommand(Action execute, Func<bool>? canExecute = null) : ICommand
    {
        public event EventHandler? CanExecuteChanged
        {
            add { }
            remove { }
        }

        public bool CanExecute(object? parameter) => canExecute?.Invoke() ?? true;

        public void Execute(object? parameter) => execute();
    }
}
