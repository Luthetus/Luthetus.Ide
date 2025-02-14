using Luthetus.TextEditor.RazorLib.Options.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.TextEditor.RazorLib.Keymaps.Models;
using Luthetus.Common.RazorLib.Themes.Models;

namespace Luthetus.TextEditor.Tests.Basis.Options.Models;

/// <summary>
/// <see cref="TextEditorOptions"/>
/// </summary>
public class TextEditorOptionsTests
{
    /// <summary>
    /// <see cref="TextEditorOptions(CommonOptions, bool, bool, int?, double, Common.RazorLib.Keymaps.Models.Keymap, bool)"/>
    /// <br/>----<br/>
    /// <see cref="TextEditorOptions.CommonOptions"/>
    /// <see cref="TextEditorOptions.ShowWhitespace"/>
    /// <see cref="TextEditorOptions.ShowNewlines"/>
    /// <see cref="TextEditorOptions.TextEditorHeightInPixels"/>
    /// <see cref="TextEditorOptions.CursorWidthInPixels"/>
    /// <see cref="TextEditorOptions.Keymap"/>
    /// <see cref="TextEditorOptions.UseMonospaceOptimizations"/>
    /// <see cref="TextEditorOptions.RenderStateKey"/>
    /// </summary>
    [Fact]
	public void Constructor()
	{
        var commonOptions = new CommonOptions(
            TextEditorOptionsState.DEFAULT_FONT_SIZE_IN_PIXELS,
            TextEditorOptionsState.DEFAULT_ICON_SIZE_IN_PIXELS,
            ThemeFacts.VisualStudioDarkThemeClone.Key,
            null,
            ShowPanelTitles: false);

        var showWhitespace = false;
        var showNewlines = false;
        var textEditorHeightInPixels = 400;
        var cursorWidthInPixels = 5;
        var keymap = TextEditorKeymapFacts.VimKeymap;
        var useMonospaceOptimizations = true;

        var textEditorOptions = new TextEditorOptions(
            commonOptions,
            showWhitespace,
            showNewlines,
            textEditorHeightInPixels,
            cursorWidthInPixels,
            keymap,
            useMonospaceOptimizations);

        Assert.Equal(commonOptions, textEditorOptions.CommonOptions);
        Assert.Equal(showWhitespace, textEditorOptions.ShowWhitespace);
        Assert.Equal(showNewlines, textEditorOptions.ShowNewlines);
        Assert.Equal(textEditorHeightInPixels, textEditorOptions.TextEditorHeightInPixels);
        Assert.Equal(cursorWidthInPixels, textEditorOptions.CursorWidthInPixels);
        Assert.Equal(keymap, textEditorOptions.Keymap);
        Assert.Equal(useMonospaceOptimizations, textEditorOptions.UseMonospaceOptimizations);

        Assert.NotEqual(Key<RenderState>.Empty, textEditorOptions.RenderStateKey);

        // Assert setting of a new RenderStateKey
        {
            var outTextEditorOptions = textEditorOptions with
            {
                RenderStateKey = Key<RenderState>.NewKey()
            };

            Assert.NotEqual(textEditorOptions.RenderStateKey, outTextEditorOptions.RenderStateKey);
        }
	}
}
