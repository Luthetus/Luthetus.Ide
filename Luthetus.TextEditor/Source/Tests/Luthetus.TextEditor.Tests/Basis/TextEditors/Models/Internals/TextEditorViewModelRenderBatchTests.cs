using Xunit;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.TextEditor.RazorLib.Options.Models;
using Luthetus.TextEditor.RazorLib.Options.States;
using Luthetus.TextEditor.Tests.Basis.TextEditors.Models.TextEditorServices;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.Common.RazorLib.Themes.Models;
using Luthetus.TextEditor.RazorLib.Keymaps.Models;
using System.Reflection;
using Luthetus.Common.RazorLib.Dimensions.Models;

namespace Luthetus.TextEditor.Tests.Basis.TextEditors.Models.Internals;

/// <summary>
/// <see cref="TextEditorRenderBatch"/>
/// </summary>
public class TextEditorRenderBatchTests
{
    /// <summary>
    /// <see cref="TextEditorRenderBatch(TextEditorModel?, TextEditorViewModel?, TextEditorOptions?, string, int)"/>
	/// <br/>----<br/>
    /// <see cref="TextEditorRenderBatch.Model"/>
    /// <see cref="TextEditorRenderBatch.ViewModel"/>
    /// <see cref="TextEditorRenderBatch.Options"/>
    /// <see cref="TextEditorRenderBatch.FontFamily"/>
    /// <see cref="TextEditorRenderBatch.FontSizeInPixels"/>
    /// <see cref="TextEditorRenderBatch.FontFamilyCssStyleString"/>
    /// <see cref="TextEditorRenderBatch.FontSizeInPixelsCssStyleString"/>
    /// <see cref="TextEditorRenderBatch.IsValid"/>
    /// </summary>
    [Fact]
	public void Constructor()
	{
        TextEditorServicesTestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

		var options = new TextEditorOptions(
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

		var fontFamily = "monospace";
		var fontSizeInPixels = 20;

        var renderBatch = new TextEditorRenderBatch(
            inModel,
            inViewModel,
            options,
            fontFamily,
            fontSizeInPixels);

        Assert.Equal(inModel, renderBatch.Model);
		Assert.Equal(inViewModel, renderBatch.ViewModel);
		Assert.Equal(options, renderBatch.Options);
		Assert.Equal(fontFamily, renderBatch.FontFamily);
		Assert.Equal(fontSizeInPixels, renderBatch.FontSizeInPixels);
		Assert.Equal($"font-family: {fontFamily};", renderBatch.FontFamilyCssStyleString);
		Assert.Equal($"font-size: {fontSizeInPixels.ToCssValue()}px;", renderBatch.FontSizeInPixelsCssStyleString);
		Assert.True(renderBatch.IsValid);

        // Assert case(s) where renderBatch.IsValid should be false
        {
            // Model is null
            Assert.False(new TextEditorRenderBatch(
					null,
					inViewModel,
					options,
					fontFamily,
					fontSizeInPixels)
				.IsValid);

            // ViewModel is null
            Assert.False(new TextEditorRenderBatch(
                    inModel,
                    null,
                    options,
                    fontFamily,
                    fontSizeInPixels)
                .IsValid);

            // Options is null
            Assert.False(new TextEditorRenderBatch(
                    inModel,
                    inViewModel,
                    null,
                    fontFamily,
                    fontSizeInPixels)
                .IsValid);
        }
	}
}