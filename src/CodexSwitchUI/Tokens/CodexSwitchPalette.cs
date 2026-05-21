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
    public string ProviderCardActive { get; init; } = "#FFEFF6FF";

    public string ProviderCardActiveHover { get; init; } = "#FFDBEAFE";

    public string ProviderCardActivePressed { get; init; } = "#FFBFDBFE";

    public string ProviderCardActiveBorder { get; init; } = "#FF93C5FD";

    public string ProviderCardActiveForeground { get; init; } = "#FF0F172A";

    public string ProviderCardActiveMutedForeground { get; init; } = "#FF475569";

    public string ProviderCardActiveIcon { get; init; } = "#FFDBEAFE";

    public string ProviderUsage { get; init; } = "#FFFFFFFF";

    public string ProviderUsageHover { get; init; } = "#FFF8FAFC";

    public string ProviderUsageBorder { get; init; } = "#FFBFDBFE";

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
        WarningForeground: "#FF09090B")
    {
        ProviderCardActive = "#FFEFF6FF",
        ProviderCardActiveHover = "#FFDBEAFE",
        ProviderCardActivePressed = "#FFBFDBFE",
        ProviderCardActiveBorder = "#FF93C5FD",
        ProviderCardActiveForeground = "#FF0F172A",
        ProviderCardActiveMutedForeground = "#FF475569",
        ProviderCardActiveIcon = "#FFDBEAFE",
        ProviderUsage = "#FFFFFFFF",
        ProviderUsageHover = "#FFF8FAFC",
        ProviderUsageBorder = "#FFBFDBFE"
    };

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
        WarningForeground: "#FF1C1917")
    {
        ProviderCardActive = "#FF0F172A",
        ProviderCardActiveHover = "#FF172033",
        ProviderCardActivePressed = "#FF1E293B",
        ProviderCardActiveBorder = "#FF2563EB",
        ProviderCardActiveForeground = "#FFF8FAFC",
        ProviderCardActiveMutedForeground = "#FFCBD5E1",
        ProviderCardActiveIcon = "#FF1E293B",
        ProviderUsage = "#CC020617",
        ProviderUsageHover = "#E6020617",
        ProviderUsageBorder = "#FF334155"
    };

    public static CodexSwitchPalette Zinc { get; } = Light;
}
