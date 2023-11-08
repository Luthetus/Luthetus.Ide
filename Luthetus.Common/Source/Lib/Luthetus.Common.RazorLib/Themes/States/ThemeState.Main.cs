using System.Collections.Immutable;
using Fluxor;
using Luthetus.Common.RazorLib.Themes.Models;

namespace Luthetus.Common.RazorLib.Themes.States;

[FeatureState]
public partial record ThemeState(ImmutableList<ThemeRecord> ThemeBag)
{
    public ThemeState() : this(DefaultThemeRecordsBag)
    {
        
    }

    public static readonly ImmutableList<ThemeRecord> DefaultThemeRecordsBag = new[]
    {
        ThemeFacts.VisualStudioDarkThemeClone,
        ThemeFacts.VisualStudioLightThemeClone,
    }.ToImmutableList();
}