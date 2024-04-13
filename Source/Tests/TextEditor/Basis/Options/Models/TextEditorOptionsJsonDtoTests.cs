using Luthetus.TextEditor.RazorLib.Options.Models;
using Luthetus.TextEditor.RazorLib.Keymaps.Models;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.Common.RazorLib.Themes.Models;
using Luthetus.TextEditor.RazorLib.Options.States;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.TextEditor.Tests.Basis.Options.Models;

/// <summary>
/// <see cref="TextEditorOptionsJsonDto"/>
/// </summary>
public class TextEditorOptionsJsonDtoTests
{
    /// <summary>
    /// <see cref="TextEditorOptionsJsonDto(Common.RazorLib.Options.Models.CommonOptionsJsonDto?, bool?, bool?, int?, double?, Common.RazorLib.Keymaps.Models.Keymap?, bool?)"/>
    /// <br/>----<br/>
	/// <see cref="TextEditorOptionsJsonDto.CommonOptionsJsonDto"/>
    /// <see cref="TextEditorOptionsJsonDto.ShowWhitespace"/>
    /// <see cref="TextEditorOptionsJsonDto.ShowNewlines"/>
    /// <see cref="TextEditorOptionsJsonDto.TextEditorHeightInPixels"/>
    /// <see cref="TextEditorOptionsJsonDto.CursorWidthInPixels"/>
    /// <see cref="TextEditorOptionsJsonDto.Keymap"/>
    /// <see cref="TextEditorOptionsJsonDto.UseMonospaceOptimizations"/>
	/// <see cref="TextEditorOptionsJsonDto.RenderStateKey"/>
    /// </summary>
    [Fact]
	public void TextEditorOptionsJsonDto_A()
	{
		var commonOptionsJsonDto = new CommonOptionsJsonDto();
		var showWhitespace = false;
		var showNewlines = false;
		var textEditorHeightInPixels = 400;
		var cursorWidthInPixels = 5;
		var keymap = TextEditorKeymapFacts.VimKeymap;
		var useMonospaceOptimizations = true;

        var optionsJsonDto = new TextEditorOptionsJsonDto(
            commonOptionsJsonDto,
			showWhitespace,
            showNewlines,
            textEditorHeightInPixels,
            cursorWidthInPixels,
			keymap,
            useMonospaceOptimizations);

        Assert.Equal(commonOptionsJsonDto, optionsJsonDto.CommonOptionsJsonDto);
        Assert.Equal(showWhitespace, optionsJsonDto.ShowWhitespace);
        Assert.Equal(showNewlines, optionsJsonDto.ShowNewlines);
        Assert.Equal(textEditorHeightInPixels, optionsJsonDto.TextEditorHeightInPixels);
        Assert.Equal(cursorWidthInPixels, optionsJsonDto.CursorWidthInPixels);
        Assert.Equal(keymap, optionsJsonDto.Keymap);
        Assert.Equal(useMonospaceOptimizations, optionsJsonDto.UseMonospaceOptimizations);

        Assert.NotEqual(Key<RenderState>.Empty, optionsJsonDto.RenderStateKey);

        // Assert setting of a new RenderStateKey
        {
            var outOptionsJsonDto = optionsJsonDto with
            {
                RenderStateKey = Key<RenderState>.NewKey()
            };

            Assert.NotEqual(optionsJsonDto.RenderStateKey, outOptionsJsonDto.RenderStateKey);
        }
    }

    /// <summary>
    /// <see cref="TextEditorOptionsJsonDto()"/>
    /// <br/>----<br/>
	/// <see cref="TextEditorOptionsJsonDto.CommonOptionsJsonDto"/>
    /// <see cref="TextEditorOptionsJsonDto.ShowWhitespace"/>
    /// <see cref="TextEditorOptionsJsonDto.ShowNewlines"/>
    /// <see cref="TextEditorOptionsJsonDto.TextEditorHeightInPixels"/>
    /// <see cref="TextEditorOptionsJsonDto.CursorWidthInPixels"/>
    /// <see cref="TextEditorOptionsJsonDto.Keymap"/>
    /// <see cref="TextEditorOptionsJsonDto.UseMonospaceOptimizations"/>
	/// <see cref="TextEditorOptionsJsonDto.RenderStateKey"/>
    /// </summary>
    [Fact]
	public void TextEditorOptionsJsonDto_B()
	{
		var optionsJsonDto = new TextEditorOptionsJsonDto();

		Assert.Null(optionsJsonDto.CommonOptionsJsonDto);
		Assert.Null(optionsJsonDto.ShowWhitespace);
		Assert.Null(optionsJsonDto.ShowNewlines);
		Assert.Null(optionsJsonDto.TextEditorHeightInPixels);
		Assert.Null(optionsJsonDto.CursorWidthInPixels);
		Assert.Null(optionsJsonDto.Keymap);
        Assert.Null(optionsJsonDto.UseMonospaceOptimizations);

        Assert.NotEqual(Key<RenderState>.Empty, optionsJsonDto.RenderStateKey);

        // Assert setting of a new RenderStateKey
        {
            var outOptionsJsonDto = optionsJsonDto with
            {
                RenderStateKey = Key<RenderState>.NewKey()
            };

            Assert.NotEqual(optionsJsonDto.RenderStateKey, outOptionsJsonDto.RenderStateKey);
        }
    }

    /// <summary>
    /// <see cref="TextEditorOptionsJsonDto(TextEditorOptions)"/>
    /// <br/>----<br/>
	/// <see cref="TextEditorOptionsJsonDto.CommonOptionsJsonDto"/>
    /// <see cref="TextEditorOptionsJsonDto.ShowWhitespace"/>
    /// <see cref="TextEditorOptionsJsonDto.ShowNewlines"/>
    /// <see cref="TextEditorOptionsJsonDto.TextEditorHeightInPixels"/>
    /// <see cref="TextEditorOptionsJsonDto.CursorWidthInPixels"/>
    /// <see cref="TextEditorOptionsJsonDto.Keymap"/>
    /// <see cref="TextEditorOptionsJsonDto.UseMonospaceOptimizations"/>
	/// <see cref="TextEditorOptionsJsonDto.RenderStateKey"/>
    /// </summary>
    [Fact]
	public void TextEditorOptionsJsonDto_C()
	{
        var textEditorOptions = new TextEditorOptions(
            new CommonOptions(
                TextEditorOptionsState.DEFAULT_FONT_SIZE_IN_PIXELS,
                TextEditorOptionsState.DEFAULT_ICON_SIZE_IN_PIXELS,
                ThemeFacts.VisualStudioDarkThemeClone.Key,
                null),
            false,
            false,
            null,
            TextEditorOptionsState.DEFAULT_CURSOR_WIDTH_IN_PIXELS,
            TextEditorKeymapFacts.DefaultKeymap,
            true);

		var textEditorOptionsJsonDto = new TextEditorOptionsJsonDto(textEditorOptions);

        Assert.Equal(textEditorOptions.ShowWhitespace, textEditorOptionsJsonDto.ShowWhitespace);
        Assert.Equal(textEditorOptions.ShowNewlines, textEditorOptionsJsonDto.ShowNewlines);
        Assert.Equal(textEditorOptions.TextEditorHeightInPixels, textEditorOptionsJsonDto.TextEditorHeightInPixels);
        Assert.Equal(textEditorOptions.CursorWidthInPixels, textEditorOptionsJsonDto.CursorWidthInPixels);
        Assert.Equal(textEditorOptions.Keymap, textEditorOptionsJsonDto.Keymap);
        Assert.Equal(textEditorOptions.UseMonospaceOptimizations, textEditorOptionsJsonDto.UseMonospaceOptimizations);

        Assert.NotEqual(Key<RenderState>.Empty, textEditorOptionsJsonDto.RenderStateKey);

        // Assert setting of a new RenderStateKey
        {
            var outTextEditorOptionsJsonDto = textEditorOptionsJsonDto with
            {
                RenderStateKey = Key<RenderState>.NewKey()
            };

            Assert.NotEqual(textEditorOptionsJsonDto.RenderStateKey, outTextEditorOptionsJsonDto.RenderStateKey);
        }
    }
}