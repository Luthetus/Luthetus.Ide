using System.Collections.Immutable;
using Fluxor;
using Luthetus.Common.RazorLib.Themes.Models;

namespace Luthetus.Common.RazorLib.Themes.States;

[FeatureState]
public partial record ThemeStateTests(ImmutableList<ThemeRecord> ThemeBag)
{
    private ThemeState() : this(DefaultThemeRecordsBag)
    {
        
    }

    public static readonly ImmutableList<ThemeRecord> DefaultThemeRecordsBag = new[]
    {
        ThemeFacts.VisualStudioDarkThemeClone,
        ThemeFacts.VisualStudioLightThemeClone,
    }.ToImmutableList();
}