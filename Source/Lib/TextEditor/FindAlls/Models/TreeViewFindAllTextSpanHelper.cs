using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Groups.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.FindAlls.Models;

public static class TreeViewFindAllTextSpanHelper
{
	public static Task OpenInEditorOnClick(
		TreeViewFindAllTextSpan treeViewFindAllTextSpan,
		bool shouldSetFocusToEditor,
		ITextEditorService textEditorService,
		LuthetusTextEditorConfig textEditorConfig,
		IServiceProvider serviceProvider)
	{
		return textEditorService.OpenInEditorAsync(
			treeViewFindAllTextSpan.AbsolutePath.Value,
			true,
			treeViewFindAllTextSpan.Item.StartingIndexInclusive,
			new Category("main"),
			Key<TextEditorViewModel>.NewKey());
	}
}
