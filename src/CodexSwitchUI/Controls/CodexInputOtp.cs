using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;
using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace CodexSwitchUI.Controls;

[PseudoClasses(CodexFocusVisible.PseudoClass)]
public class CodexInputOtp : ItemsControl
{
    public const string DigitsPattern = "^[0-9]$";
    public const string DigitsAndLettersPattern = "^[A-Za-z0-9]$";

    public static readonly StyledProperty<string> TextProperty =
        AvaloniaProperty.Register<CodexInputOtp, string>(nameof(Text), string.Empty);

    public static readonly StyledProperty<int> MaxLengthProperty =
        AvaloniaProperty.Register<CodexInputOtp, int>(nameof(MaxLength), 6);

    public static readonly StyledProperty<string?> PatternProperty =
        AvaloniaProperty.Register<CodexInputOtp, string?>(nameof(Pattern));

    public static readonly StyledProperty<int> ActiveIndexProperty =
        AvaloniaProperty.Register<CodexInputOtp, int>(nameof(ActiveIndex));

    public static readonly StyledProperty<CodexControlIntent> IntentProperty =
        AvaloniaProperty.Register<CodexInputOtp, CodexControlIntent>(nameof(Intent), CodexControlIntent.Default);

    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexInputOtp, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    public static readonly StyledProperty<bool> IsInvalidProperty =
        AvaloniaProperty.Register<CodexInputOtp, bool>(nameof(IsInvalid));

    private bool _isNormalizingText;

    static CodexInputOtp()
    {
        TextProperty.Changed.AddClassHandler<CodexInputOtp>((input, _) => input.OnTextChanged());
        MaxLengthProperty.Changed.AddClassHandler<CodexInputOtp>((input, _) => input.OnMaxLengthChanged());
        PatternProperty.Changed.AddClassHandler<CodexInputOtp>((input, _) => input.OnTextChanged());
        ActiveIndexProperty.Changed.AddClassHandler<CodexInputOtp>((input, _) => input.SyncSlots());
        IntentProperty.Changed.AddClassHandler<CodexInputOtp>((input, _) => input.SyncClasses());
        SizeProperty.Changed.AddClassHandler<CodexInputOtp>((input, _) => input.SyncClassesAndSlots());
        IsInvalidProperty.Changed.AddClassHandler<CodexInputOtp>((input, _) => input.SyncClassesAndSlots());
    }

    public CodexInputOtp()
    {
        Focusable = true;
        ItemsView.CollectionChanged += OnItemsViewCollectionChanged;
        AutomationProperties.SetIsControlElementOverride(this, true);
        SyncClasses();
    }

    public string Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public int MaxLength
    {
        get => GetValue(MaxLengthProperty);
        set => SetValue(MaxLengthProperty, value);
    }

    public string? Pattern
    {
        get => GetValue(PatternProperty);
        set => SetValue(PatternProperty, value);
    }

    public int ActiveIndex
    {
        get => GetValue(ActiveIndexProperty);
        set => SetValue(ActiveIndexProperty, value);
    }

    public CodexControlIntent Intent
    {
        get => GetValue(IntentProperty);
        set => SetValue(IntentProperty, value);
    }

    public CodexControlSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    public bool IsInvalid
    {
        get => GetValue(IsInvalidProperty);
        set => SetValue(IsInvalidProperty, value);
    }

    public bool IsComplete => Text.Length >= MaxLength;

    public bool TryInsertText(string? input)
    {
        if (!IsEnabled || string.IsNullOrEmpty(input) || MaxLength <= 0)
        {
            return false;
        }

        var accepted = FilterInput(input).ToArray();
        if (accepted.Length == 0)
        {
            return false;
        }

        var builder = new StringBuilder(Text);
        var index = Math.Clamp(ActiveIndex, 0, Math.Min(builder.Length, MaxLength));
        var inserted = false;

        foreach (var character in accepted)
        {
            if (index >= MaxLength)
            {
                break;
            }

            if (index < builder.Length)
            {
                builder[index] = character;
            }
            else
            {
                builder.Append(character);
            }

            index++;
            inserted = true;
        }

        if (!inserted)
        {
            return false;
        }

        SetCurrentValue(TextProperty, builder.ToString());
        SetCurrentValue(ActiveIndexProperty, ClampActiveIndex(index));
        return true;
    }

    public void FocusSlot(int index)
    {
        SetCurrentValue(ActiveIndexProperty, ClampActiveIndex(index));
        Focus();
        SyncSlots();
    }

    public void Clear()
    {
        SetCurrentValue(TextProperty, string.Empty);
        SetCurrentValue(ActiveIndexProperty, 0);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        SyncSlots();
    }

    protected override void OnGotFocus(FocusChangedEventArgs e)
    {
        base.OnGotFocus(e);
        PseudoClasses.Set(CodexFocusVisible.PseudoClass, CodexFocusVisible.FromFocusChange(e));
        SetCurrentValue(ActiveIndexProperty, ClampActiveIndex(ActiveIndex));
        SyncSlots();
    }

    protected override void OnLostFocus(FocusChangedEventArgs e)
    {
        base.OnLostFocus(e);
        PseudoClasses.Set(CodexFocusVisible.PseudoClass, false);
        Dispatcher.UIThread.Post(() =>
        {
            if (this.IsAttachedToVisualTree() && !IsKeyboardFocusWithin)
            {
                ClearSlotActiveState();
            }
        });
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        PseudoClasses.Set(CodexFocusVisible.PseudoClass, false);
        base.OnPointerPressed(e);
        FocusSlot(ActiveIndex);
    }

    protected override void OnTextInput(TextInputEventArgs e)
    {
        if (TryInsertText(e.Text))
        {
            e.Handled = true;
        }

        base.OnTextInput(e);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (!IsEnabled)
        {
            base.OnKeyDown(e);
            return;
        }

        switch (e.Key)
        {
            case Key.Back:
                e.Handled = TryBackspace();
                break;
            case Key.Delete:
                e.Handled = TryDelete();
                break;
            case Key.Left:
                SetCurrentValue(ActiveIndexProperty, ClampActiveIndex(ActiveIndex - 1));
                e.Handled = true;
                break;
            case Key.Right:
                SetCurrentValue(ActiveIndexProperty, ClampActiveIndex(ActiveIndex + 1));
                e.Handled = true;
                break;
            case Key.Home:
                SetCurrentValue(ActiveIndexProperty, 0);
                e.Handled = true;
                break;
            case Key.End:
                SetCurrentValue(ActiveIndexProperty, ClampActiveIndex(Text.Length));
                e.Handled = true;
                break;
        }

        base.OnKeyDown(e);
    }

    internal void SyncClassesAndSlots()
    {
        SyncClasses();
        SyncSlots();
    }

    internal void SyncSlots()
    {
        var slots = GetSlots().ToArray();
        var text = Text;

        for (var order = 0; order < slots.Length; order++)
        {
            var slot = slots[order];
            var index = slot.Index >= 0 ? slot.Index : order;
            var character = index >= 0 && index < text.Length ? text[index].ToString() : string.Empty;
            slot.Owner = this;
            slot.SetCurrentValue(CodexInputOtpSlot.IndexProperty, index);
            slot.SetCurrentValue(CodexInputOtpSlot.CharacterProperty, character);
            slot.SetCurrentValue(CodexInputOtpSlot.IsActiveProperty, IsKeyboardFocusWithin && index == ActiveIndex);
            slot.SetCurrentValue(CodexInputOtpSlot.IsInvalidProperty, IsInvalid || Intent == CodexControlIntent.Error);
            slot.SetCurrentValue(CodexInputOtpSlot.SizeProperty, Size);
            slot.Classes.Set("slot-first", order == 0);
            slot.Classes.Set("slot-middle", order > 0 && order < slots.Length - 1);
            slot.Classes.Set("slot-last", order == slots.Length - 1);
        }

        foreach (var group in GetGroups())
        {
            group.Owner = this;
            group.SetCurrentValue(CodexInputOtpGroup.SizeProperty, Size);
            group.SyncItemStates();
        }

        foreach (var separator in GetSeparators())
        {
            separator.SetCurrentValue(CodexInputOtpSeparator.SizeProperty, Size);
        }

        SyncClasses();
    }

    private void OnItemsViewCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        SyncSlots();
    }

    private void OnTextChanged()
    {
        if (_isNormalizingText)
        {
            return;
        }

        var normalized = NormalizeText(Text);
        if (!string.Equals(Text, normalized, StringComparison.Ordinal))
        {
            _isNormalizingText = true;
            SetCurrentValue(TextProperty, normalized);
            _isNormalizingText = false;
        }

        SetCurrentValue(ActiveIndexProperty, ClampActiveIndex(ActiveIndex));
        SyncSlots();
    }

    private void OnMaxLengthChanged()
    {
        if (MaxLength < 1)
        {
            SetCurrentValue(MaxLengthProperty, 1);
            return;
        }

        OnTextChanged();
    }

    private bool TryBackspace()
    {
        if (Text.Length == 0)
        {
            return false;
        }

        var index = ActiveIndex <= 0 ? 0 : Math.Min(ActiveIndex - 1, Text.Length - 1);
        SetCurrentValue(TextProperty, Text.Remove(index, 1));
        SetCurrentValue(ActiveIndexProperty, ClampActiveIndex(index));
        return true;
    }

    private bool TryDelete()
    {
        if (Text.Length == 0 || ActiveIndex >= Text.Length)
        {
            return false;
        }

        SetCurrentValue(TextProperty, Text.Remove(ActiveIndex, 1));
        SetCurrentValue(ActiveIndexProperty, ClampActiveIndex(ActiveIndex));
        return true;
    }

    private string NormalizeText(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        return new string(FilterInput(value).Take(MaxLength).ToArray());
    }

    private IEnumerable<char> FilterInput(string input)
    {
        foreach (var character in input)
        {
            if (!char.IsControl(character) && MatchesPattern(character))
            {
                yield return character;
            }
        }
    }

    private bool MatchesPattern(char character)
    {
        if (string.IsNullOrWhiteSpace(Pattern))
        {
            return true;
        }

        try
        {
            return Regex.IsMatch(character.ToString(), Pattern, RegexOptions.CultureInvariant);
        }
        catch (ArgumentException)
        {
            return true;
        }
    }

    private int ClampActiveIndex(int index)
    {
        var maxIndex = Math.Max(0, MaxLength - 1);
        return Math.Clamp(index, 0, maxIndex);
    }

    private void ClearSlotActiveState()
    {
        foreach (var slot in GetSlots())
        {
            slot.SetCurrentValue(CodexInputOtpSlot.IsActiveProperty, false);
        }
    }

    private void SyncClasses()
    {
        CodexClassSync.SetIntent(Classes, Intent);
        CodexClassSync.SetSize(Classes, Size);
        Classes.Set("input-otp", true);
        Classes.Set("invalid", IsInvalid);
        Classes.Set("complete", IsComplete);
        Classes.Set("has-value", Text.Length > 0);
    }

    private IEnumerable<CodexInputOtpGroup> GetGroups()
    {
        return GetControls().OfType<CodexInputOtpGroup>();
    }

    private IEnumerable<CodexInputOtpSeparator> GetSeparators()
    {
        return GetControls().OfType<CodexInputOtpSeparator>();
    }

    private IEnumerable<CodexInputOtpSlot> GetSlots()
    {
        foreach (var control in GetControls())
        {
            if (control is CodexInputOtpSlot slot)
            {
                yield return slot;
            }

            if (control is CodexInputOtpGroup group)
            {
                foreach (var item in group.GetSlots())
                {
                    yield return item;
                }
            }
        }
    }

    private IEnumerable<Control> GetControls()
    {
        for (var index = 0; index < ItemsView.Count; index++)
        {
            if (ItemsView[index] is Control item)
            {
                yield return item;
            }
            else if (ContainerFromIndex(index) is Control container)
            {
                yield return container;
            }
        }
    }
}

public class CodexInputOtpGroup : ItemsControl
{
    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexInputOtpGroup, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    static CodexInputOtpGroup()
    {
        SizeProperty.Changed.AddClassHandler<CodexInputOtpGroup>((group, _) => group.SyncItemStates());
    }

    public CodexInputOtpGroup()
    {
        Focusable = false;
        ItemsView.CollectionChanged += OnItemsViewCollectionChanged;
        SyncClasses();
    }

    public CodexControlSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    internal CodexInputOtp? Owner { get; set; }

    internal IEnumerable<CodexInputOtpSlot> GetSlots()
    {
        for (var index = 0; index < ItemsView.Count; index++)
        {
            if (ItemsView[index] is CodexInputOtpSlot slot)
            {
                yield return slot;
            }
            else if (ContainerFromIndex(index) is CodexInputOtpSlot container)
            {
                yield return container;
            }
        }
    }

    internal void SyncItemStates()
    {
        var slots = GetSlots().ToArray();

        for (var index = 0; index < slots.Length; index++)
        {
            slots[index].Classes.Set("group-first", index == 0);
            slots[index].Classes.Set("group-middle", index > 0 && index < slots.Length - 1);
            slots[index].Classes.Set("group-last", index == slots.Length - 1);
            slots[index].SetCurrentValue(CodexInputOtpSlot.SizeProperty, Size);
        }

        SyncClasses();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        Owner?.SyncSlots();
        SyncItemStates();
    }

    private void OnItemsViewCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        Owner?.SyncSlots();
        SyncItemStates();
    }

    private void SyncClasses()
    {
        CodexClassSync.SetSize(Classes, Size);
        Classes.Set("input-otp-group", true);
        Classes.Set("has-slots", GetSlots().Any());
    }
}

public class CodexInputOtpSlot : ContentControl
{
    public static readonly StyledProperty<int> IndexProperty =
        AvaloniaProperty.Register<CodexInputOtpSlot, int>(nameof(Index), -1);

    public static readonly StyledProperty<string> CharacterProperty =
        AvaloniaProperty.Register<CodexInputOtpSlot, string>(nameof(Character), string.Empty);

    public static readonly StyledProperty<bool> IsActiveProperty =
        AvaloniaProperty.Register<CodexInputOtpSlot, bool>(nameof(IsActive));

    public static readonly StyledProperty<bool> IsInvalidProperty =
        AvaloniaProperty.Register<CodexInputOtpSlot, bool>(nameof(IsInvalid));

    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexInputOtpSlot, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    static CodexInputOtpSlot()
    {
        IndexProperty.Changed.AddClassHandler<CodexInputOtpSlot>((slot, _) => slot.SyncClasses());
        CharacterProperty.Changed.AddClassHandler<CodexInputOtpSlot>((slot, _) => slot.SyncClasses());
        IsActiveProperty.Changed.AddClassHandler<CodexInputOtpSlot>((slot, _) => slot.SyncClasses());
        IsInvalidProperty.Changed.AddClassHandler<CodexInputOtpSlot>((slot, _) => slot.SyncClasses());
        SizeProperty.Changed.AddClassHandler<CodexInputOtpSlot>((slot, _) => slot.SyncClasses());
    }

    public CodexInputOtpSlot()
    {
        Focusable = false;
        SyncClasses();
    }

    public int Index
    {
        get => GetValue(IndexProperty);
        set => SetValue(IndexProperty, value);
    }

    public string Character
    {
        get => GetValue(CharacterProperty);
        set => SetValue(CharacterProperty, value);
    }

    public bool IsActive
    {
        get => GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }

    public bool IsInvalid
    {
        get => GetValue(IsInvalidProperty);
        set => SetValue(IsInvalidProperty, value);
    }

    public CodexControlSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    internal CodexInputOtp? Owner { get; set; }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        Owner?.FocusSlot(Index);
        e.Handled = true;
    }

    private void SyncClasses()
    {
        CodexClassSync.SetSize(Classes, Size);
        Classes.Set("input-otp-slot", true);
        Classes.Set("active", IsActive);
        Classes.Set("invalid", IsInvalid);
        Classes.Set("has-character", !string.IsNullOrEmpty(Character));
    }
}

public class CodexInputOtpSeparator : ContentControl
{
    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexInputOtpSeparator, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    static CodexInputOtpSeparator()
    {
        SizeProperty.Changed.AddClassHandler<CodexInputOtpSeparator>((separator, _) => separator.SyncClasses());
    }

    public CodexInputOtpSeparator()
    {
        Focusable = false;
        IsHitTestVisible = false;
        Content = "-";
        SyncClasses();
    }

    public CodexControlSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    private void SyncClasses()
    {
        CodexClassSync.SetSize(Classes, Size);
        Classes.Set("input-otp-separator", true);
    }
}
