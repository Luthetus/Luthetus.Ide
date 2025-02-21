namespace Luthetus.Common.RazorLib.Themes.Models;

/// <summary>
/// The list provided should not be modified after passing it as a parameter.
/// Make a shallow copy, and pass the shallow copy, if further modification of your list will be necessary.
/// </summary>
public record struct ThemeState(IReadOnlyList<ThemeRecord> ThemeList)
{
    public ThemeState() : this(DefaultThemeRecordsList)
    {
        
    }

    public static readonly List<ThemeRecord> DefaultThemeRecordsList = new()
    {
        ThemeFacts.VisualStudioDarkThemeClone,
        ThemeFacts.VisualStudioLightThemeClone,
    };
}