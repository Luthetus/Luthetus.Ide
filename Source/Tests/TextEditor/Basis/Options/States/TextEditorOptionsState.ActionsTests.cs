using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.Keymaps.Models;

namespace Luthetus.TextEditor.Tests.Basis.Options.States;

/// <summary>
/// <see cref="TextEditorOptionsState"/>
/// </summary>
public class TextEditorOptionsStateActionsTests
{
	/// <summary>
	/// <see cref="TextEditorOptionsState.SetFontFamilyAction"/>
	/// </summary>
	[Fact]
	public void SetFontFamilyAction()
	{
		var fontFamily = "monospace";
        var setFontFamilyAction = new TextEditorOptionsState.SetFontFamilyAction(fontFamily);
        Assert.Equal(fontFamily, setFontFamilyAction.FontFamily);
	}

	/// <summary>
	/// <see cref="TextEditorOptionsState.SetFontSizeAction"/>
	/// </summary>
	[Fact]
	public void SetFontSizeAction()
	{
        var fontSizeInPixels = TextEditorOptionsState.DEFAULT_FONT_SIZE_IN_PIXELS + 1;
        var setFontSizeAction = new TextEditorOptionsState.SetFontSizeAction(fontSizeInPixels);
        Assert.Equal(fontSizeInPixels, setFontSizeAction.FontSizeInPixels);
	}

	/// <summary>
	/// <see cref="TextEditorOptionsState.SetCursorWidthAction"/>
	/// </summary>
	[Fact]
	public void SetCursorWidthAction()
	{
        var cursorWidthInPixels = TextEditorOptionsState.DEFAULT_CURSOR_WIDTH_IN_PIXELS + 1;
        var setCursorWidthAction = new TextEditorOptionsState.SetCursorWidthAction(cursorWidthInPixels);
        Assert.Equal(cursorWidthInPixels, setCursorWidthAction.CursorWidthInPixels);
	}

	/// <summary>
	/// <see cref="TextEditorOptionsState.SetRenderStateKeyAction"/>
	/// </summary>
	[Fact]
	public void SetRenderStateKeyAction()
	{
        var renderStateKey = Key<RenderState>.NewKey();
        var setRenderStateKeyAction = new TextEditorOptionsState.SetRenderStateKeyAction(renderStateKey);
        Assert.Equal(renderStateKey, setRenderStateKeyAction.RenderStateKey);
	}

	/// <summary>
	/// <see cref="TextEditorOptionsState.SetHeightAction"/>
	/// </summary>
	[Fact]
	public void SetHeightAction()
	{
		// non-null value
		{
            var heightInPixels = 500;
            var setHeightAction = new TextEditorOptionsState.SetHeightAction(heightInPixels);
            Assert.Equal(heightInPixels, setHeightAction.HeightInPixels);
        }

        // null value
        {
            var heightInPixels = (int?)null;
            var setHeightAction = new TextEditorOptionsState.SetHeightAction(heightInPixels);
            Assert.Equal(heightInPixels, setHeightAction.HeightInPixels);
        }
	}

	/// <summary>
	/// <see cref="TextEditorOptionsState.SetThemeAction"/>
	/// </summary>
	[Fact]
	public void SetThemeAction()
	{
        var theme = LuthetusTextEditorCustomThemeFacts.LightTheme;
        var setThemeAction = new TextEditorOptionsState.SetThemeAction(theme);
        Assert.Equal(theme, setThemeAction.Theme);
	}

	/// <summary>
	/// <see cref="TextEditorOptionsState.SetKeymapAction"/>
	/// </summary>
	[Fact]
	public void SetKeymapAction()
	{
        var keymap = TextEditorKeymapFacts.VimKeymap;
        var setKeymapAction = new TextEditorOptionsState.SetKeymapAction(keymap);
        Assert.Equal(keymap, setKeymapAction.Keymap);
	}

	/// <summary>
	/// <see cref="TextEditorOptionsState.SetShowWhitespaceAction"/>
	/// </summary>
	[Fact]
	public void SetShowWhitespaceAction()
	{
		// true
		{
            var showWhitespace = true;
            var setShowWhitespaceAction = new TextEditorOptionsState.SetShowWhitespaceAction(showWhitespace);
            Assert.Equal(showWhitespace, setShowWhitespaceAction.ShowWhitespace);
        }
		
		// false
		{
            var showWhitespace = false;
            var setShowWhitespaceAction = new TextEditorOptionsState.SetShowWhitespaceAction(showWhitespace);
            Assert.Equal(showWhitespace, setShowWhitespaceAction.ShowWhitespace);
        }
	}

	/// <summary>
	/// <see cref="TextEditorOptionsState.SetShowNewlinesAction"/>
	/// </summary>
	[Fact]
	public void SetShowNewlinesAction()
    {
		// true
        {
            var showNewlines = true;
            var setShowNewlinesAction = new TextEditorOptionsState.SetShowNewlinesAction(showNewlines);
            Assert.Equal(showNewlines, setShowNewlinesAction.ShowNewlines);
        }

        // false
        {
            var showNewlines = false;
            var setShowNewlinesAction = new TextEditorOptionsState.SetShowNewlinesAction(showNewlines);
            Assert.Equal(showNewlines, setShowNewlinesAction.ShowNewlines);
        }
	}

	/// <summary>
	/// <see cref="TextEditorOptionsState.SetUseMonospaceOptimizationsAction"/>
	/// </summary>
	[Fact]
	public void SetUseMonospaceOptimizationsAction()
	{
        // true
        {
            var useMonospaceOptimizations = true;
            var setUseMonospaceOptimizationsAction = new TextEditorOptionsState.SetUseMonospaceOptimizationsAction(useMonospaceOptimizations);
            Assert.Equal(useMonospaceOptimizations, setUseMonospaceOptimizationsAction.UseMonospaceOptimizations);
        }

        // false
        {
            var useMonospaceOptimizations = false;
            var setUseMonospaceOptimizationsAction = new TextEditorOptionsState.SetUseMonospaceOptimizationsAction(useMonospaceOptimizations);
            Assert.Equal(useMonospaceOptimizations, setUseMonospaceOptimizationsAction.UseMonospaceOptimizations);
        }
	}
}