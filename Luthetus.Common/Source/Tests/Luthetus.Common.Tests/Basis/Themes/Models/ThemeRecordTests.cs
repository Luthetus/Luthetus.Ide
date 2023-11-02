using Luthetus.Common.RazorLib.Keys.Models;
using System.Collections.Immutable;

namespace Luthetus.Common.RazorLib.Themes.Models;

public record ThemeRecordTests(
    Key<ThemeRecord> Key,
    string DisplayName,
    string CssClassString,
    ThemeContrastKind ThemeContrastKind,
    ThemeColorKind ThemeColorKind,
    ImmutableList<ThemeScope> ThemeScopeBag);