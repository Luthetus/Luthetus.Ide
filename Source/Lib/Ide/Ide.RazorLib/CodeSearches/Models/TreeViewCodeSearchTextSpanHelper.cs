using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Groups.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.Ide.RazorLib.CodeSearches.Models;

public static class TreeViewCodeSearchTextSpanHelper
{
	public static Task OpenInEditorOnClick(
		TreeViewCodeSearchTextSpan treeViewCodeSearchTextSpan,
		bool shouldSetFocusToEditor,
		ITextEditorService textEditorService,
		LuthetusTextEditorConfig textEditorConfig,
		IServiceProvider serviceProvider)
	{
		return textEditorService.OpenInEditorAsync(
			treeViewCodeSearchTextSpan.AbsolutePath.Value,
			true,
			treeViewCodeSearchTextSpan.Item.StartingIndexInclusive,
			new Category("main"),
			Key<TextEditorViewModel>.NewKey());
	}
}
