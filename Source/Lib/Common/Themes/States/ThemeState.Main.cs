using System.Collections.Immutable;
using Fluxor;
using Luthetus.Common.RazorLib.Themes.Models;

namespace Luthetus.Common.RazorLib.Themes.States;

/// <summary>
/// The list provided should not be modified after passing it as a parameter.
/// Make a shallow copy, and pass the shallow copy, if further modification of your list will be necessary.
/// </summary>
[FeatureState]
public partial record ThemeState(List<ThemeRecord> ThemeList)
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