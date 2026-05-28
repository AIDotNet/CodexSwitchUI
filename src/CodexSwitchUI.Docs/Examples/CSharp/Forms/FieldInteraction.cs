using Avalonia.Controls;
using CodexSwitchUI.Controls;

public static class FieldInteractionSample
{
    public static Control BuildFieldInteractionPreview()
    {
        var providerName = new CodexTextBox
        {
            Text = string.Empty,
            PlaceholderText = "Provider name",
            MinWidth = 240
        };
        var providerField = new CodexField
        {
            Label = "Provider name",
            Description = "Required before the provider can be saved.",
            Message = "Provider name is required.",
            IsRequired = true,
            Intent = CodexControlIntent.Error,
            Content = providerName
        };

        var validate = new CodexButton
        {
            Content = "Validate",
            Size = CodexControlSize.Small
        };
        validate.Click += (_, _) =>
        {
            var isValid = !string.IsNullOrWhiteSpace(providerName.Text);
            providerField.Intent = isValid ? CodexControlIntent.Success : CodexControlIntent.Error;
            providerField.Message = isValid ? "Provider name is ready." : "Provider name is required.";
            providerName.Intent = isValid ? CodexControlIntent.Success : CodexControlIntent.Error;
        };

        var toggleDisabled = new CodexButton
        {
            Content = "Toggle disabled",
            Variant = CodexControlVariant.Secondary,
            Size = CodexControlSize.Small
        };
        toggleDisabled.Click += (_, _) =>
        {
            providerName.IsEnabled = !providerName.IsEnabled;
            providerField.Message = providerName.IsEnabled
                ? "Input is enabled again."
                : "Child input disabled while field spacing remains stable.";
        };

        return new StackPanel
        {
            Spacing = 10,
            Children =
            {
                providerField,
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 8,
                    Children = { validate, toggleDisabled }
                },
                new CodexField
                {
                    Label = "Child focus target",
                    Description = "Focusable children own focus-visible while the field keeps label and helper alignment.",
                    Message = "Select a default route.",
                    Content = new CodexSelect
                    {
                        ItemsSource = new[] { "Primary", "Fallback", "Disabled" },
                        SelectedIndex = 0,
                        MinWidth = 240
                    }
                },
                new CodexField
                {
                    Label = "Disabled child guard",
                    Description = "Field chrome remains visible while the child control uses disabled opacity.",
                    Message = "Inherited provider cannot be edited.",
                    Content = new CodexTextBox
                    {
                        Text = "Locked route",
                        IsEnabled = false,
                        MinWidth = 240
                    }
                }
            }
        };
    }
}
