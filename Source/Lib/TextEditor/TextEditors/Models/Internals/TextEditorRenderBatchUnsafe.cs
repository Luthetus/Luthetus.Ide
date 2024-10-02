using System.Text;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.TextEditor.RazorLib.Options.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

public record TextEditorRenderBatchUnsafe(
        TextEditorModel? Model,
        TextEditorViewModel? ViewModel,
        TextEditorOptions? Options,
        string FontFamily,
        int FontSizeInPixels,
        ViewModelDisplayOptions ViewModelDisplayOptions,
		TextEditorComponentData ComponentData)
		// Func<Task> RestoreFocusToTextEditor
	    // Func<MenuKind, bool, Task> SetShouldDisplayMenuAsync
	    // ImmutableArray<HeaderButtonKind>? HeaderButtonKinds
    : ITextEditorRenderBatch
{
    private double? _gutterWidthInPixels;

    public string FontFamilyCssStyle => $"font-family: {FontFamily};";
    public string FontSizeInPixelsCssStyle => $"font-size: {FontSizeInPixels.ToCssValue()}px;";
    public string HeightCssStyle => GetHeightCssStyle();
    public double GutterWidthInPixels => _gutterWidthInPixels ??= GetGutterWidthInPixels();

    public bool IsValid =>
        Model is not null &&
        ViewModel is not null &&
        Options is not null;

    private string GetHeightCssStyle()
    {
        // Start with a calc statement and a value of 100%
        var heightBuilder = new StringBuilder("height: calc(100%");

        if (ViewModelDisplayOptions.HeaderComponent is not null)
            heightBuilder.Append(" - var(--luth_te_text-editor-header-height)");

        if (ViewModelDisplayOptions.IncludeFooterHelperComponent)
            heightBuilder.Append(" - var(--luth_te_text-editor-footer-height)");

        // Close the calc statement, and the height style attribute
        heightBuilder.Append(");");

        return heightBuilder.ToString();
    }

    private double GetGutterWidthInPixels()
    {
        if (!ViewModelDisplayOptions.IncludeGutterComponent)
            return 0;

        var mostDigitsInARowLineNumber = Model!.LineCount.ToString().Length;

        var gutterWidthInPixels = mostDigitsInARowLineNumber *
            ViewModel!.CharAndLineMeasurements.CharacterWidth;

        gutterWidthInPixels += TextEditorModel.GUTTER_PADDING_LEFT_IN_PIXELS + TextEditorModel.GUTTER_PADDING_RIGHT_IN_PIXELS;

        return gutterWidthInPixels;
    }
}
