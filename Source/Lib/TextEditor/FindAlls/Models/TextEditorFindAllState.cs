using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.FindAlls.Models;

public record struct TextEditorFindAllState(
	string SearchQuery,
	string StartingDirectoryPath,
	List<TextEditorTextSpan> SearchResultList,
	ProgressBarModel? ProgressBarModel)
{
	public static readonly Key<TreeViewContainer> TreeViewFindAllContainerKey = Key<TreeViewContainer>.NewKey();

    public TextEditorFindAllState() : this(
    	string.Empty,
    	string.Empty,
    	new(),
    	null)
    {
    }
}