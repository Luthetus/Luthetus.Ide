using Luthetus.TextEditor.RazorLib.Options.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

public interface ITextEditorRenderBatch
{
	public const string DEFAULT_FONT_FAMILY = "monospace";

	public TextEditorModel? Model { get; }
	public TextEditorViewModel? ViewModel { get; }
    public TextEditorOptions? Options { get; }
	public string FontFamily { get; }
	public int FontSizeInPixels { get; }
	public ViewModelDisplayOptions ViewModelDisplayOptions { get; }
	public TextEditorViewModelDisplay.TextEditorEvents Events { get; }
    public string FontFamilyCssStyle { get; }
    public string FontSizeInPixelsCssStyle { get; }
    public string HeightCssStyle { get; }
    public double GutterWidthInPixels { get; }
    public bool IsValid { get; }
}
