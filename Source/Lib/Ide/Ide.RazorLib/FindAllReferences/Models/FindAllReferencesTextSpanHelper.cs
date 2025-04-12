using System.Text;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.Ide.RazorLib.FindAllReferences.Models;

/// <summary>
/// Do not duplicate this code from 'OutputTextSpanHelper'.
/// </summary>
public static class FindAllReferencesTextSpanHelper
{
	public static Task OpenInEditorOnClick(
		TreeViewFindAllReferences treeViewFindAllReferences,
		bool shouldSetFocusToEditor,
		ITextEditorService textEditorService)
	{
		textEditorService.WorkerArbitrary.PostUnique(nameof(FindAllReferencesTextSpanHelper), async editContext =>
		{
			await textEditorService.OpenInEditorAsync(
				editContext,
				treeViewFindAllReferences.Item.Value,
				shouldSetFocusToEditor,
				cursorPositionIndex: null,
				new Category("main"),
				Key<TextEditorViewModel>.NewKey());
		});
		return Task.CompletedTask;
	}
}
