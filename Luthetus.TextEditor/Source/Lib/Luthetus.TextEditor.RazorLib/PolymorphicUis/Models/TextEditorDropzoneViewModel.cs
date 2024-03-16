using Luthetus.Common.RazorLib.Drags.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.JavaScriptObjects.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.PolymorphicViewModels.Models;
using Luthetus.TextEditor.RazorLib.Groups.Models;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.PolymorphicUis.Models;

public class TextEditorDropzoneViewModel : IDropzoneViewModel
{
	public TextEditorDropzoneViewModel(
		MeasuredHtmlElementDimensions measuredHtmlElementDimensions,
		ElementDimensions elementDimensions,
		Key<TextEditorGroup>? textEditorGroupKey,
		string? cssClassString,
		IPolymorphicViewModel? polymorphicViewModel)
	{
		MeasuredHtmlElementDimensions = measuredHtmlElementDimensions;
		DropzoneElementDimensions = elementDimensions;
		TextEditorGroupKey = textEditorGroupKey;
		CssClassString = cssClassString;
		PolymorphicViewModel = polymorphicViewModel;
	}

	public IPolymorphicViewModel? PolymorphicViewModel { get; init; }
	public Key<IDropzoneViewModel> Key { get; init; } = Key<IDropzoneViewModel>.NewKey();
	public MeasuredHtmlElementDimensions MeasuredHtmlElementDimensions { get; }
	public ElementDimensions DropzoneElementDimensions { get; }

	public string? CssClassString { get; set; }

	/// <summary>
	/// If the group key is not null, then upon mouse up, add the view model to the group,
	/// and set the view model key as being active.
	/// </summary>
	public Key<TextEditorGroup>? TextEditorGroupKey { get; }
}
