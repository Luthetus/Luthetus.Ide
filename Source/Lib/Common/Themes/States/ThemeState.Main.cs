using System.Collections.Immutable;
using Fluxor;
using Luthetus.Common.RazorLib.Themes.Models;

namespace Luthetus.Common.RazorLib.Themes.States;

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