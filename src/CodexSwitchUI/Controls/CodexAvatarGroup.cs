using Avalonia;
using Avalonia.Controls;

namespace CodexSwitchUI.Controls;

public class CodexAvatarGroup : Panel
{
    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexAvatarGroup, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    public static readonly StyledProperty<double> OverlapProperty =
        AvaloniaProperty.Register<CodexAvatarGroup, double>(nameof(Overlap), 10d);

    public static readonly StyledProperty<bool> IsStackedProperty =
        AvaloniaProperty.Register<CodexAvatarGroup, bool>(nameof(IsStacked), true);

    public static readonly StyledProperty<int> ItemCountProperty =
        AvaloniaProperty.Register<CodexAvatarGroup, int>(nameof(ItemCount));

    static CodexAvatarGroup()
    {
        SizeProperty.Changed.AddClassHandler<CodexAvatarGroup>((group, _) =>
        {
            group.SyncClasses();
            group.InvalidateMeasure();
        });
        OverlapProperty.Changed.AddClassHandler<CodexAvatarGroup>((group, _) => group.InvalidateMeasure());
        IsStackedProperty.Changed.AddClassHandler<CodexAvatarGroup>((group, _) =>
        {
            group.SyncClasses();
            group.InvalidateMeasure();
        });
        AffectsMeasure<CodexAvatarGroup>(SizeProperty, OverlapProperty, IsStackedProperty);
    }

    public CodexAvatarGroup()
    {
        SyncClasses();
    }

    public CodexControlSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    public double Overlap
    {
        get => GetValue(OverlapProperty);
        set => SetValue(OverlapProperty, value);
    }

    public bool IsStacked
    {
        get => GetValue(IsStackedProperty);
        set => SetValue(IsStackedProperty, value);
    }

    public int ItemCount => GetValue(ItemCountProperty);

    protected override Size MeasureOverride(Size availableSize)
    {
        SyncChildren();

        var width = 0d;
        var height = 0d;
        var index = 0;
        var overlap = EffectiveOverlap();

        foreach (var child in VisibleChildren())
        {
            child.Measure(availableSize);
            var desired = child.DesiredSize;
            width += index == 0 || !IsStacked ? desired.Width : Math.Max(0, desired.Width - overlap);
            height = Math.Max(height, desired.Height);
            index++;
        }

        return new Size(width, height);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        SyncChildren();

        var x = 0d;
        var index = 0;
        var overlap = EffectiveOverlap();

        foreach (var child in VisibleChildren())
        {
            var desired = child.DesiredSize;
            var height = double.IsFinite(finalSize.Height) && finalSize.Height > 0
                ? finalSize.Height
                : desired.Height;
            child.Arrange(new Rect(x, Math.Max(0, (height - desired.Height) / 2d), desired.Width, desired.Height));
            x += IsStacked ? Math.Max(0, desired.Width - overlap) : desired.Width;
            index++;
        }

        return finalSize;
    }

    private IEnumerable<Control> VisibleChildren()
    {
        return Children.Where(child => child.IsVisible);
    }

    private double EffectiveOverlap()
    {
        return Math.Max(0, Overlap);
    }

    private void SyncChildren()
    {
        var visible = VisibleChildren().ToArray();
        SetValue(ItemCountProperty, visible.Length);
        SyncClasses();

        for (var index = 0; index < visible.Length; index++)
        {
            var child = visible[index];
            child.Classes.Set("avatar-group-item", true);
            child.Classes.Set("group-first", index == 0);
            child.Classes.Set("group-middle", index > 0 && index < visible.Length - 1);
            child.Classes.Set("group-last", index == visible.Length - 1);
            child.SetValue(ZIndexProperty, index);

            if (child is CodexAvatar avatar)
            {
                avatar.SetCurrentValue(CodexAvatar.SizeProperty, Size);
            }
            else if (child is CodexAvatarGroupCount count)
            {
                count.SetCurrentValue(CodexAvatarGroupCount.SizeProperty, Size);
            }
        }
    }

    private void SyncClasses()
    {
        CodexClassSync.SetSize(Classes, Size);
        Classes.Set("stacked", IsStacked);
        Classes.Set("inline", !IsStacked);
        Classes.Set("empty", ItemCount == 0);
        Classes.Set("has-items", ItemCount > 0);
    }
}

public class CodexAvatarGroupCount : CodexFrame
{
    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexAvatarGroupCount, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    public static readonly StyledProperty<int> CountProperty =
        AvaloniaProperty.Register<CodexAvatarGroupCount, int>(nameof(Count));

    static CodexAvatarGroupCount()
    {
        SizeProperty.Changed.AddClassHandler<CodexAvatarGroupCount>((count, _) => count.SyncClasses());
        CountProperty.Changed.AddClassHandler<CodexAvatarGroupCount>((count, _) => count.SyncCount());
    }

    public CodexAvatarGroupCount()
    {
        SyncCount();
        SyncClasses();
    }

    public CodexControlSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    public int Count
    {
        get => GetValue(CountProperty);
        set => SetValue(CountProperty, value);
    }

    private void SyncCount()
    {
        if (Content is null && Count > 0)
        {
            SetCurrentValue(ContentProperty, $"+{Count}");
        }

        SyncClasses();
    }

    private void SyncClasses()
    {
        CodexClassSync.SetSize(Classes, Size);
        Classes.Set("has-count", Count > 0 || Content is not null);
        Classes.Set("empty", Count <= 0 && Content is null);
    }
}
