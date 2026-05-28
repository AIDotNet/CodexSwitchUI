using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;
using System;
using System.Windows.Input;

public static class CommandInteractionSample
{
    public static Control BuildCommandInteractionPreview()
    {
        var status = new CodexText
        {
            Role = CodexTextRole.Muted,
            Text = "ItemSelected: select a command item."
        };

        var command = new CodexCommand
        {
            Placeholder = "Search providers...",
            SearchText = "provider",
            LoopNavigation = true,
            Content = new CodexCommandList
            {
                Items =
                {
                    new CodexCommandGroup
                    {
                        Header = "Providers",
                        Items =
                        {
                            new CodexCommandItem { Content = "OpenAI", Value = "openai", Keywords = "provider model", Icon = "O", Shortcut = "Enter" },
                            new CodexCommandItem { Content = "Claude", Value = "claude", Keywords = "provider model", Icon = "C", Shortcut = "Cmd+2", IsActive = true },
                            new CodexCommandItem { Content = "Local", Value = "local", Keywords = "offline provider", Icon = "L" }
                        }
                    },
                    new CodexCommandSeparator { AlwaysRender = true },
                    new CodexCommandGroup
                    {
                        Header = "Disabled results",
                        Items =
                        {
                            new CodexCommandItem { Content = "Unavailable action", Icon = "X", IsEnabled = false },
                            new CodexCommandItem
                            {
                                Content = "Command blocked",
                                Icon = "B",
                                Shortcut = "CanExecute=false",
                                Command = new SampleCommand(() => status.Text = "Blocked command executed.", () => false)
                            }
                        }
                    }
                }
            }
        };
        command.ItemSelected += (_, args) =>
        {
            status.Text = $"ItemSelected: {args.Value ?? args.Item.Content} source={args.Source}.";
        };

        var keyboardSelect = new CodexButton
        {
            Content = "Move + select",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Secondary
        };
        keyboardSelect.Click += (_, _) =>
        {
            command.TryHandleNavigationKey(Key.Down);
            command.TrySelectActiveItem();
        };

        var loading = new CodexCommand
        {
            Placeholder = "Loading suppresses selection...",
            IsLoading = true,
            Content = new CodexCommandList
            {
                Items =
                {
                    new CodexCommandGroup
                    {
                        Header = "Loading gate",
                        Items =
                        {
                            new CodexCommandItem { Content = "Refresh usage", Icon = "R", Shortcut = "Cmd+R" },
                            new CodexCommandItem { Content = "Open settings", Icon = "S" }
                        }
                    },
                    new CodexCommandLoading { Content = "Refreshing command results..." }
                }
            }
        };

        var input = new CodexCommandInput
        {
            PlaceholderText = "Filter commands...",
            Text = "provider"
        };
        var empty = new CodexCommand
        {
            Placeholder = "No results",
            Content = new CodexCommandList
            {
                Items =
                {
                    new CodexCommandEmpty { Content = "No matching commands." }
                }
            }
        };

        return new StackPanel
        {
            Spacing = 10,
            Children =
            {
                command,
                status,
                keyboardSelect,
                loading,
                input,
                empty
            }
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
