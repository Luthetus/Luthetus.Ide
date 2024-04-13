using Fluxor;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.Common.RazorLib.Themes.Models;

namespace Luthetus.Common.RazorLib.Options.States;

[FeatureState]
public partial record AppOptionsState(CommonOptions Options)
{
    public const int DEFAULT_FONT_SIZE_IN_PIXELS = 20;
    public const int DEFAULT_ICON_SIZE_IN_PIXELS = 18;
    public const int MINIMUM_FONT_SIZE_IN_PIXELS = 5;
    public const int MINIMUM_ICON_SIZE_IN_PIXELS = 5;

    public static readonly CommonOptions DefaultCommonOptions = new(
        DEFAULT_FONT_SIZE_IN_PIXELS,
        DEFAULT_ICON_SIZE_IN_PIXELS,
        ThemeFacts.VisualStudioDarkThemeClone.Key,
        null);

    public AppOptionsState() : this(DefaultCommonOptions)
    {
    }
}