using Fluxor;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.Common.RazorLib.Themes.Models;
using Luthetus.TextEditor.RazorLib.Keymaps.Models;
using Luthetus.TextEditor.RazorLib.Options.Models;

namespace Luthetus.TextEditor.RazorLib.Options.States;

[FeatureState]
public partial class TextEditorOptionsState
{
    public TextEditorOptionsState()
    {
        Options = new TextEditorOptions(
            new CommonOptions(
                DEFAULT_FONT_SIZE_IN_PIXELS,
                DEFAULT_ICON_SIZE_IN_PIXELS,
                ThemeFacts.VisualStudioDarkThemeClone.Key,
                null),
            false,
            false,
            null,
            DEFAULT_CURSOR_WIDTH_IN_PIXELS,
            TextEditorKeymapFacts.DefaultKeymap,
            true);
    }

    public const int DEFAULT_FONT_SIZE_IN_PIXELS = 20;
    public const int DEFAULT_ICON_SIZE_IN_PIXELS = 18;
    public const double DEFAULT_CURSOR_WIDTH_IN_PIXELS = 2.5;

    public const int MINIMUM_FONT_SIZE_IN_PIXELS = 5;
    public const int MINIMUM_ICON_SIZE_IN_PIXELS = 5;

    public TextEditorOptions Options { get; set; }
}