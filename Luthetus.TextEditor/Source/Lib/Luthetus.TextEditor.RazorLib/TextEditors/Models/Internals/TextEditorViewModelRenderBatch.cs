using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.TextEditor.RazorLib.Options.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

public record TextEditorRenderBatch(
    TextEditorModel? Model,
    TextEditorViewModel? ViewModel,
    TextEditorOptions? Options,
    string FontFamily,
    int FontSizeInPixels)
{
    public const string DEFAULT_FONT_FAMILY = "monospace";

    public string FontFamilyCssStyleString => $"font-family: {FontFamily};";
    public string FontSizeInPixelsCssStyleString => $"font-size: {FontSizeInPixels.ToCssValue()}px;";

    public bool IsValid =>
        Model is not null &&
        ViewModel is not null &&
        Options is not null;
}