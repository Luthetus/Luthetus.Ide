namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

/// <summary>
/// Everytime one renders a unique <see cref="Luthetus.TextEditor.RazorLib.TextEditors.Displays.TextEditorViewModelDisplay"/>,
/// a unique identifier for the HTML elements is created.
///
/// That unique identifier, and other data, is on this class.
/// </summary>
public class TextEditorComponentData
{
	public TextEditorComponentData(
		string proportionalFontMeasurementsContainerElementId,
		Guid textEditorHtmlElementId,
		ViewModelDisplayOptions viewModelDisplayOptions)
	{
		ProportionalFontMeasurementsContainerElementId = proportionalFontMeasurementsContainerElementId;
		TextEditorHtmlElementId = textEditorHtmlElementId;
		ViewModelDisplayOptions = viewModelDisplayOptions;
	}

	public string ProportionalFontMeasurementsContainerElementId { get; }
	public Guid TextEditorHtmlElementId { get; }
	public ViewModelDisplayOptions ViewModelDisplayOptions { get; }
}
