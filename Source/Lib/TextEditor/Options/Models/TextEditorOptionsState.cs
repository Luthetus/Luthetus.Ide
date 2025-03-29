using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.Common.RazorLib.Themes.Models;
using Luthetus.TextEditor.RazorLib.Keymaps.Models;

namespace Luthetus.TextEditor.RazorLib.Options.Models;

public record struct TextEditorOptionsState
{
    public TextEditorOptionsState()
    {
        Options = new TextEditorOptions(
            new CommonOptions(
                DEFAULT_FONT_SIZE_IN_PIXELS,
                DEFAULT_ICON_SIZE_IN_PIXELS,
                Luthetus.Common.RazorLib.Options.Models.AppOptionsState.DEFAULT_RESIZE_HANDLE_WIDTH_IN_PIXELS,
                Luthetus.Common.RazorLib.Options.Models.AppOptionsState.DEFAULT_RESIZE_HANDLE_HEIGHT_IN_PIXELS,
                ThemeFacts.VisualStudioDarkThemeClone.Key,
                FontFamily: null,
                ShowPanelTitles: false),
            false,
            false,
            null,
            DEFAULT_CURSOR_WIDTH_IN_PIXELS,
            true,
            CharAndLineMeasurements: new(0, 0))
        {
        	Keymap = TextEditorKeymapFacts.DefaultKeymap,
        };
    }

    public const int DEFAULT_FONT_SIZE_IN_PIXELS = 20;
    public const int DEFAULT_ICON_SIZE_IN_PIXELS = 18;
    public const double DEFAULT_CURSOR_WIDTH_IN_PIXELS = 2.5;

    public const int MINIMUM_FONT_SIZE_IN_PIXELS = 5;
    public const int MINIMUM_ICON_SIZE_IN_PIXELS = 5;

    public TextEditorOptions Options { get; set; }
}