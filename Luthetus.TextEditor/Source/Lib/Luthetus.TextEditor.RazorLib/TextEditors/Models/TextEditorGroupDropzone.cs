using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.JavaScriptObjects.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Groups.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

public class TextEditorGroupDropzone : IDropzone
{
	public TextEditorGroupDropzone(
		MeasuredHtmlElementDimensions measuredHtmlElementDimensions,
		Key<TextEditorGroup> textEditorGroupKey,
		ElementDimensions elementDimensions)
	{
		MeasuredHtmlElementDimensions = measuredHtmlElementDimensions;
		TextEditorGroupKey = textEditorGroupKey;
		ElementDimensions = elementDimensions;
	}

	public MeasuredHtmlElementDimensions MeasuredHtmlElementDimensions { get; }
	public Key<TextEditorGroup> TextEditorGroupKey { get; }
	public Key<IDropzone> DropzoneKey { get; }
	public ElementDimensions ElementDimensions { get; }
	public string CssClass { get; init; }
	public string CssStyle { get; }
}

