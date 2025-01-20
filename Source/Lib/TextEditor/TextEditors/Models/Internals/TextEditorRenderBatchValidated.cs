using Luthetus.TextEditor.RazorLib.Options.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

public sealed class TextEditorRenderBatchValidated : ITextEditorRenderBatch
{
    public TextEditorRenderBatchValidated(TextEditorRenderBatchUnsafe textEditorRenderBatchUnsafe)
    {
        Model = textEditorRenderBatchUnsafe.Model ?? throw new NullReferenceException();
        ViewModel = textEditorRenderBatchUnsafe.ViewModel ?? throw new NullReferenceException();
        Options = textEditorRenderBatchUnsafe.Options ?? throw new NullReferenceException();
        FontFamily = textEditorRenderBatchUnsafe.FontFamily;
        FontSizeInPixels = textEditorRenderBatchUnsafe.FontSizeInPixels;
        ViewModelDisplayOptions = textEditorRenderBatchUnsafe.ViewModelDisplayOptions;
		ComponentData = textEditorRenderBatchUnsafe.ComponentData;
        FontFamilyCssStyle = textEditorRenderBatchUnsafe.FontFamilyCssStyle;
        FontSizeInPixelsCssStyle = textEditorRenderBatchUnsafe.FontSizeInPixelsCssStyle;
        HeightCssStyle = textEditorRenderBatchUnsafe.HeightCssStyle;
        GutterWidthInPixels = textEditorRenderBatchUnsafe.GutterWidthInPixels;
		IsValid = textEditorRenderBatchUnsafe.IsValid;
		// Func<Task> RestoreFocusToTextEditor
	    // Func<MenuKind, bool, Task> SetShouldDisplayMenuAsync
	    // ImmutableArray<HeaderButtonKind>? HeaderButtonKinds
    }

    public TextEditorModel Model { get; }
	public TextEditorViewModel ViewModel { get; }
	public TextEditorOptions Options { get; }
	public string FontFamily { get; }
	public int FontSizeInPixels { get; }
	public ViewModelDisplayOptions ViewModelDisplayOptions { get; }
	public TextEditorComponentData ComponentData { get; }
	public string FontFamilyCssStyle { get; }
	public string FontSizeInPixelsCssStyle { get; }
	public string HeightCssStyle { get; }
	public double GutterWidthInPixels { get; }
	public bool IsValid { get; }
}