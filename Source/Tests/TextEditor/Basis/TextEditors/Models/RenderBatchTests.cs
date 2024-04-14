﻿using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

namespace Luthetus.TextEditor.Tests.Basis.TextEditors.Models;

/// <summary>
/// <see cref="RenderBatch"/>
/// </summary>
public class RenderBatchTests
{
    /// <summary>
    /// <see cref="TextEditorRenderBatch(TextEditorModel?, TextEditorViewModel?, TextEditorOptions?, string, int)"/>
	/// <br/>----<br/>
    /// <see cref="RenderBatch.Model"/>
    /// <see cref="RenderBatch.ViewModel"/>
    /// <see cref="RenderBatch.Options"/>
    /// <see cref="RenderBatch.FontFamily"/>
    /// <see cref="RenderBatch.FontSizeInPixels"/>
    /// <see cref="RenderBatch.FontFamilyCssStyle"/>
    /// <see cref="RenderBatch.FontSizeInPixelsCssStyle"/>
    /// <see cref="RenderBatch.IsValid"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        throw new NotImplementedException("Test was broken on (2024-04-08)");
        //      TextEditorServicesTestsHelper.InitializeTextEditorServicesTestsHelper(
        //          out var textEditorService,
        //          out var inModel,
        //          out var inViewModel,
        //          out var serviceProvider);

        //var options = new TextEditorOptions(
        //          new CommonOptions(
        //              TextEditorOptionsState.DEFAULT_FONT_SIZE_IN_PIXELS,
        //              TextEditorOptionsState.DEFAULT_ICON_SIZE_IN_PIXELS,
        //              ThemeFacts.VisualStudioDarkThemeClone.Key,
        //              null),
        //          false,
        //          false,
        //          null,
        //          TextEditorOptionsState.DEFAULT_CURSOR_WIDTH_IN_PIXELS,
        //          TextEditorKeymapFacts.DefaultKeymap,
        //          true);

        //var fontFamily = "monospace";
        //var fontSizeInPixels = 20;

        //      var renderBatch = new TextEditorRenderBatch(
        //          inModel,
        //          inViewModel,
        //          options,
        //          fontFamily,
        //          fontSizeInPixels,
        //          null);

        //      Assert.Equal(inModel, renderBatch.Model);
        //Assert.Equal(inViewModel, renderBatch.ViewModel);
        //Assert.Equal(options, renderBatch.Options);
        //Assert.Equal(fontFamily, renderBatch.FontFamily);
        //Assert.Equal(fontSizeInPixels, renderBatch.FontSizeInPixels);
        //Assert.Equal($"font-family: {fontFamily};", renderBatch.FontFamilyCssStyle);
        //Assert.Equal($"font-size: {fontSizeInPixels.ToCssValue()}px;", renderBatch.FontSizeInPixelsCssStyle);
        //Assert.True(renderBatch.IsValid);

        //      // Assert case(s) where renderBatch.IsValid should be false
        //      {
        //          // Model is null
        //          Assert.False(new TextEditorRenderBatch(
        //			null,
        //			inViewModel,
        //			options,
        //			fontFamily,
        //			fontSizeInPixels,
        //                  null)
        //		.IsValid);

        //          // ViewModel is null
        //          Assert.False(new TextEditorRenderBatch(
        //                  inModel,
        //                  null,
        //                  options,
        //                  fontFamily,
        //                  fontSizeInPixels,
        //                  null)
        //              .IsValid);

        //          // Options is null
        //          Assert.False(new TextEditorRenderBatch(
        //                  inModel,
        //                  inViewModel,
        //                  null,
        //                  fontFamily,
        //                  fontSizeInPixels,
        //                  null)
        //              .IsValid);
        //      }
    }
}