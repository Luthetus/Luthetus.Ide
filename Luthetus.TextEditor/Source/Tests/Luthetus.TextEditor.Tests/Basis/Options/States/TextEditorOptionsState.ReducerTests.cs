using Luthetus.TextEditor.RazorLib.Options.States;
using Luthetus.TextEditor.Tests.Basis.TextEditors.Models.TextEditorServices;
using Microsoft.Extensions.DependencyInjection;
using Fluxor;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.Keymaps.Models;

namespace Luthetus.TextEditor.Tests.Basis.Options.States;

/// <summary>
/// <see cref="TextEditorOptionsState"/>
/// </summary>
public partial class TextEditorOptionsStateReducerTests
{
	/// <summary>
	/// <see cref="TextEditorOptionsState.Reducer.ReduceSetFontFamilyAction(TextEditorOptionsState, TextEditorOptionsState.SetFontFamilyAction)"/>
	/// </summary>
	[Fact]
	public void ReduceSetFontFamilyAction()
	{
        TextEditorServicesTestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

		var dispatcher = serviceProvider.GetRequiredService<IDispatcher>();
		var textEditorOptionsStateWrap = serviceProvider.GetRequiredService<IState<TextEditorOptionsState>>();

        var fontFamily = "monospace";
        var setFontFamilyAction = new TextEditorOptionsState.SetFontFamilyAction(fontFamily);

		Assert.NotEqual(
            fontFamily,
            textEditorOptionsStateWrap.Value.Options.CommonOptions.FontFamily);

		dispatcher.Dispatch(setFontFamilyAction);

        Assert.Equal(
            fontFamily,
            textEditorOptionsStateWrap.Value.Options.CommonOptions.FontFamily);
	}

	/// <summary>
	/// <see cref="TextEditorOptionsState.Reducer.ReduceSetFontSizeAction(TextEditorOptionsState, TextEditorOptionsState.SetFontSizeAction)"/>
	/// </summary>
	[Fact]
	public void ReduceSetFontSizeAction()
	{
        TextEditorServicesTestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

        var dispatcher = serviceProvider.GetRequiredService<IDispatcher>();
        var textEditorOptionsStateWrap = serviceProvider.GetRequiredService<IState<TextEditorOptionsState>>();

        var fontSizeInPixels = TextEditorOptionsState.DEFAULT_FONT_SIZE_IN_PIXELS + 1;
        var setFontSizeAction = new TextEditorOptionsState.SetFontSizeAction(fontSizeInPixels);

        Assert.NotEqual(
            fontSizeInPixels,
            textEditorOptionsStateWrap.Value.Options.CommonOptions.FontSizeInPixels);

        dispatcher.Dispatch(setFontSizeAction);

        Assert.Equal(
            fontSizeInPixels,
            textEditorOptionsStateWrap.Value.Options.CommonOptions.FontSizeInPixels);
	}

	/// <summary>
	/// <see cref="TextEditorOptionsState.Reducer.ReduceSetRenderStateKeyAction(TextEditorOptionsState, TextEditorOptionsState.SetRenderStateKeyAction)"/>
	/// </summary>
	[Fact]
	public void ReduceSetRenderStateKeyAction()
	{
        TextEditorServicesTestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

        var dispatcher = serviceProvider.GetRequiredService<IDispatcher>();
        var textEditorOptionsStateWrap = serviceProvider.GetRequiredService<IState<TextEditorOptionsState>>();

        var renderStateKey = Key<RenderState>.NewKey();
        var setRenderStateKeyAction = new TextEditorOptionsState.SetRenderStateKeyAction(renderStateKey);

        Assert.NotEqual(
            renderStateKey,
            textEditorOptionsStateWrap.Value.Options.RenderStateKey);

        dispatcher.Dispatch(setRenderStateKeyAction);

        Assert.Equal(
            renderStateKey,
            textEditorOptionsStateWrap.Value.Options.RenderStateKey);
	}

	/// <summary>
	/// <see cref="TextEditorOptionsState.Reducer.ReduceSetCursorWidthAction(TextEditorOptionsState, TextEditorOptionsState.SetCursorWidthAction)"/>
	/// </summary>
	[Fact]
	public void ReduceSetCursorWidthAction()
	{
        TextEditorServicesTestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

        var dispatcher = serviceProvider.GetRequiredService<IDispatcher>();
        var textEditorOptionsStateWrap = serviceProvider.GetRequiredService<IState<TextEditorOptionsState>>();

        var cursorWidthInPixels = TextEditorOptionsState.DEFAULT_CURSOR_WIDTH_IN_PIXELS + 1;
        var setCursorWidthAction = new TextEditorOptionsState.SetCursorWidthAction(cursorWidthInPixels);

        Assert.NotEqual(
            cursorWidthInPixels,
            textEditorOptionsStateWrap.Value.Options.CursorWidthInPixels);

        dispatcher.Dispatch(setCursorWidthAction);

        Assert.Equal(
            cursorWidthInPixels,
            textEditorOptionsStateWrap.Value.Options.CursorWidthInPixels);
	}

	/// <summary>
	/// <see cref="TextEditorOptionsState.Reducer.ReduceSetHeightAction(TextEditorOptionsState, TextEditorOptionsState.SetHeightAction)"/>
	/// </summary>
	[Fact]
	public void ReduceSetHeightAction()
	{
        TextEditorServicesTestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

        var dispatcher = serviceProvider.GetRequiredService<IDispatcher>();
        var textEditorOptionsStateWrap = serviceProvider.GetRequiredService<IState<TextEditorOptionsState>>();

        // non-null value
        {
            var heightInPixels = 500;
            var setHeightAction = new TextEditorOptionsState.SetHeightAction(heightInPixels);

            Assert.NotEqual(
                heightInPixels,
                textEditorOptionsStateWrap.Value.Options.TextEditorHeightInPixels);

            dispatcher.Dispatch(setHeightAction);

            Assert.Equal(
                heightInPixels,
                textEditorOptionsStateWrap.Value.Options.TextEditorHeightInPixels);
        }

        // null value
        {
            var heightInPixels = (int?)null;
            var setHeightAction = new TextEditorOptionsState.SetHeightAction(heightInPixels);

            Assert.NotEqual(
                heightInPixels,
                textEditorOptionsStateWrap.Value.Options.TextEditorHeightInPixels);

            dispatcher.Dispatch(setHeightAction);

            Assert.Equal(
                heightInPixels,
                textEditorOptionsStateWrap.Value.Options.TextEditorHeightInPixels);
        }
	}

	/// <summary>
	/// <see cref="TextEditorOptionsState.Reducer.ReduceSetThemeAction(TextEditorOptionsState, TextEditorOptionsState.SetThemeAction)"/>
	/// </summary>
	[Fact]
	public void ReduceSetThemeAction()
	{
        TextEditorServicesTestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

        var dispatcher = serviceProvider.GetRequiredService<IDispatcher>();
        var textEditorOptionsStateWrap = serviceProvider.GetRequiredService<IState<TextEditorOptionsState>>();

        var theme = LuthetusTextEditorCustomThemeFacts.LightTheme;
        var setThemeAction = new TextEditorOptionsState.SetThemeAction(theme);

        Assert.NotEqual(
            theme.Key,
            textEditorOptionsStateWrap.Value.Options.CommonOptions.ThemeKey);

        dispatcher.Dispatch(setThemeAction);

        Assert.Equal(
            theme.Key,
            textEditorOptionsStateWrap.Value.Options.CommonOptions.ThemeKey);
	}

	/// <summary>
	/// <see cref="TextEditorOptionsState.Reducer.ReduceSetKeymapAction(TextEditorOptionsState, TextEditorOptionsState.SetKeymapAction)"/>
	/// </summary>
	[Fact]
	public void ReduceSetKeymapAction()
	{
        TextEditorServicesTestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

        var dispatcher = serviceProvider.GetRequiredService<IDispatcher>();
        var textEditorOptionsStateWrap = serviceProvider.GetRequiredService<IState<TextEditorOptionsState>>();

        var keymap = TextEditorKeymapFacts.VimKeymap;
        var setKeymapAction = new TextEditorOptionsState.SetKeymapAction(keymap);

        Assert.NotEqual(
            keymap.Key,
            textEditorOptionsStateWrap.Value.Options.Keymap.Key);

        dispatcher.Dispatch(setKeymapAction);

        Assert.Equal(
            keymap.Key,
            textEditorOptionsStateWrap.Value.Options.Keymap.Key);
	}

	/// <summary>
	/// <see cref="TextEditorOptionsState.Reducer.ReduceSetShowWhitespaceAction(TextEditorOptionsState, TextEditorOptionsState.SetShowWhitespaceAction)"/>
	/// </summary>
	[Fact]
	public void ReduceSetShowWhitespaceAction()
	{
        TextEditorServicesTestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

        var dispatcher = serviceProvider.GetRequiredService<IDispatcher>();
        var textEditorOptionsStateWrap = serviceProvider.GetRequiredService<IState<TextEditorOptionsState>>();

        // true
        {
            var showWhitespace = true;
            var setShowWhitespaceAction = new TextEditorOptionsState.SetShowWhitespaceAction(showWhitespace);

            Assert.NotEqual(
                showWhitespace,
                textEditorOptionsStateWrap.Value.Options.ShowWhitespace);

            dispatcher.Dispatch(setShowWhitespaceAction);

            Assert.Equal(
                showWhitespace,
                textEditorOptionsStateWrap.Value.Options.ShowWhitespace);
        }

        // false
        {
            var showWhitespace = false;
            var setShowWhitespaceAction = new TextEditorOptionsState.SetShowWhitespaceAction(showWhitespace);

            Assert.NotEqual(
                showWhitespace,
                textEditorOptionsStateWrap.Value.Options.ShowWhitespace);

            dispatcher.Dispatch(setShowWhitespaceAction);

            Assert.Equal(
                showWhitespace,
                textEditorOptionsStateWrap.Value.Options.ShowWhitespace);
        }
	}

	/// <summary>
	/// <see cref="TextEditorOptionsState.Reducer.ReduceSetShowNewlinesAction(TextEditorOptionsState, TextEditorOptionsState.SetShowNewlinesAction)"/>
	/// </summary>
	[Fact]
	public void ReduceSetShowNewlinesAction()
	{
        TextEditorServicesTestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

        var dispatcher = serviceProvider.GetRequiredService<IDispatcher>();
        var textEditorOptionsStateWrap = serviceProvider.GetRequiredService<IState<TextEditorOptionsState>>();

        // true
        {
            var showNewlines = true;
            var setShowNewlinesAction = new TextEditorOptionsState.SetShowNewlinesAction(showNewlines);

            Assert.NotEqual(
                showNewlines,
                textEditorOptionsStateWrap.Value.Options.ShowNewlines);

            dispatcher.Dispatch(setShowNewlinesAction);

            Assert.Equal(
                showNewlines,
                textEditorOptionsStateWrap.Value.Options.ShowNewlines);
        }

        // false
        {
            var showNewlines = false;
            var setShowNewlinesAction = new TextEditorOptionsState.SetShowNewlinesAction(showNewlines);

            Assert.NotEqual(
                showNewlines,
                textEditorOptionsStateWrap.Value.Options.ShowNewlines);

            dispatcher.Dispatch(setShowNewlinesAction);

            Assert.Equal(
                showNewlines,
                textEditorOptionsStateWrap.Value.Options.ShowNewlines);
        }
	}

	/// <summary>
	/// <see cref="TextEditorOptionsState.Reducer.ReduceSetUseMonospaceOptimizationsAction(TextEditorOptionsState, TextEditorOptionsState.SetUseMonospaceOptimizationsAction)"/>
	/// </summary>
	[Fact]
	public void ReduceSetUseMonospaceOptimizationsAction()
	{
        TextEditorServicesTestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

        var dispatcher = serviceProvider.GetRequiredService<IDispatcher>();
        var textEditorOptionsStateWrap = serviceProvider.GetRequiredService<IState<TextEditorOptionsState>>();

        // false # The default value for UseMonospaceOptimizations is true so one must do the 'false' case first.
        {
            var useMonospaceOptimizations = false;
            var setUseMonospaceOptimizationsAction = new TextEditorOptionsState.SetUseMonospaceOptimizationsAction(useMonospaceOptimizations);

            Assert.NotEqual(
                useMonospaceOptimizations,
                textEditorOptionsStateWrap.Value.Options.UseMonospaceOptimizations);

            dispatcher.Dispatch(setUseMonospaceOptimizationsAction);

            Assert.Equal(
                useMonospaceOptimizations,
                textEditorOptionsStateWrap.Value.Options.UseMonospaceOptimizations);
        }

        // true
        {
            var useMonospaceOptimizations = true;
            var setUseMonospaceOptimizationsAction = new TextEditorOptionsState.SetUseMonospaceOptimizationsAction(useMonospaceOptimizations);

            Assert.NotEqual(
                useMonospaceOptimizations,
                textEditorOptionsStateWrap.Value.Options.UseMonospaceOptimizations);

            dispatcher.Dispatch(setUseMonospaceOptimizationsAction);

            Assert.Equal(
                useMonospaceOptimizations,
                textEditorOptionsStateWrap.Value.Options.UseMonospaceOptimizations);
        }
    }
}