using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.JavaScriptObjects.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Groups.Models;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.PolymorphicUis.Models;

public class TextEditorViewModelPolymorphicDropzone : IPolymorphicDropzone
{
	public TextEditorViewModelPolymorphicDropzone(
		MeasuredHtmlElementDimensions measuredHtmlElementDimensions,
		ElementDimensions elementDimensions,
		Key<TextEditorGroup>? textEditorGroupKey)
	{
		MeasuredHtmlElementDimensions = measuredHtmlElementDimensions;
		DropzoneElementDimensions = elementDimensions;
		TextEditorGroupKey = textEditorGroupKey;
	}

	public MeasuredHtmlElementDimensions MeasuredHtmlElementDimensions { get; }
	public ElementDimensions DropzoneElementDimensions { get; }

	public string? CssClassString { get; set; }

	/// <summary>
	/// If the group key is not null, then upon mouse up, add the view model to the group,
	/// and set the view model key as being active.
	/// </summary>
	public Key<TextEditorGroup>? TextEditorGroupKey { get; }
}
