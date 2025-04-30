using Luthetus.TextEditor.RazorLib.Options.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

public class TextEditorRenderBatchConstants
{
	public TextEditorRenderBatchConstants(
		TextEditorOptions textEditorOptions,
		string fontFamily,
		int fontSizeInPixels,
		ViewModelDisplayOptions viewModelDisplayOptions,
		TextEditorComponentData componentData)
	{
		TextEditorOptions = textEditorOptions;
		FontFamily = fontFamily;
		FontSizeInPixels = fontSizeInPixels;
		ViewModelDisplayOptions = viewModelDisplayOptions;
		ComponentData = componentData;
	}

	public TextEditorOptions TextEditorOptions { get; }
	public string FontFamily { get; }
	public int FontSizeInPixels  { get; }
	public ViewModelDisplayOptions ViewModelDisplayOptions { get; }
	public TextEditorComponentData ComponentData { get; }
}
