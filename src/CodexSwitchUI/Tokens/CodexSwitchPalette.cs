namespace CodexSwitchUI.Tokens;

/// <summary>
/// shadcn-compatible semantic color slots. Values are CSS-style hex colors.
/// </summary>
public sealed record CodexSwitchPalette(
    string Background,
    string Foreground,
    string Card,
    string CardForeground,
    string Popover,
    string PopoverForeground,
    string Primary,
    string PrimaryForeground,
    string Secondary,
    string SecondaryForeground,
    string Muted,
    string MutedForeground,
    string Accent,
    string AccentForeground,
    string Destructive,
    string DestructiveForeground,
    string Border,
    string Input,
    string Ring,
    string Success,
    string SuccessForeground,
    string Warning,
    string WarningForeground)
{
    public static CodexSwitchPalette Light { get; } = new(
        Background: "#FFFFFFFF",
        Foreground: "#FF09090B",
        Card: "#FFFFFFFF",
        CardForeground: "#FF09090B",
        Popover: "#FFFFFFFF",
        PopoverForeground: "#FF09090B",
        Primary: "#FF18181B",
        PrimaryForeground: "#FFFAFAFA",
        Secondary: "#FFF4F4F5",
        SecondaryForeground: "#FF18181B",
        Muted: "#FFF4F4F5",
        MutedForeground: "#FF71717A",
        Accent: "#FFF4F4F5",
        AccentForeground: "#FF18181B",
        Destructive: "#FFEF4444",
        DestructiveForeground: "#FFFAFAFA",
        Border: "#FFE4E4E7",
        Input: "#FFE4E4E7",
        Ring: "#FFA1A1AA",
        Success: "#FF16A34A",
        SuccessForeground: "#FFFFFFFF",
        Warning: "#FFF59E0B",
        WarningForeground: "#FF09090B");

    public static CodexSwitchPalette Dark { get; } = new(
        Background: "#FF09090B",
        Foreground: "#FFFAFAFA",
        Card: "#FF09090B",
        CardForeground: "#FFFAFAFA",
        Popover: "#FF09090B",
        PopoverForeground: "#FFFAFAFA",
        Primary: "#FFFAFAFA",
        PrimaryForeground: "#FF18181B",
        Secondary: "#FF27272A",
        SecondaryForeground: "#FFFAFAFA",
        Muted: "#FF27272A",
        MutedForeground: "#FFA1A1AA",
        Accent: "#FF27272A",
        AccentForeground: "#FFFAFAFA",
        Destructive: "#FF7F1D1D",
        DestructiveForeground: "#FFFAFAFA",
        Border: "#FF27272A",
        Input: "#FF27272A",
        Ring: "#FFD4D4D8",
        Success: "#FF22C55E",
        SuccessForeground: "#FF052E16",
        Warning: "#FFFBBF24",
        WarningForeground: "#FF1C1917");

    public static CodexSwitchPalette Zinc { get; } = Light;
}
