using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Themes.Models;

namespace Luthetus.Common.RazorLib.Installations.Models;

public record LuthetusCommonConfig
{
    /// <summary>The <see cref="Key{ThemeRecord}"/> to be used when the application starts</summary>
    public Key<ThemeRecord> InitialThemeKey { get; init; } = ThemeFacts.VisualStudioDarkThemeClone.Key;
    public LuthetusCommonFactories CommonFactories { get; init; } = new();
    public DialogServiceOptions DialogServiceOptions { get; init; } = new();
}
