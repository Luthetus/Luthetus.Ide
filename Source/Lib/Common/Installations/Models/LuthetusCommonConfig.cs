using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Themes.Models;

namespace Luthetus.Common.RazorLib.Installations.Models;

/// <remarks>
/// This class is an exception to the naming convention, "don't use the word 'Luthetus' in class names".
/// 
/// Reason for this exception: when one first starts interacting with this project,
/// 	this type might be one of the first types they interact with. So, the redundancy of namespace
/// 	and type containing 'Luthetus' feels reasonable here.
/// </remarks>
public record LuthetusCommonConfig
{
    /// <summary>The <see cref="Key{ThemeRecord}"/> to be used when the application starts</summary>
    public Key<ThemeRecord> InitialThemeKey { get; init; } = ThemeFacts.VisualStudioDarkThemeClone.Key;
    public DialogServiceOptions DialogServiceOptions { get; init; } = new();
}
