using System.Collections.Immutable;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.FindAlls.Models;

public record struct TextEditorFindAllState(
	string SearchQuery,
	string StartingDirectoryPath,
	ImmutableList<TextEditorTextSpan> SearchResultList,
	ProgressBarModel? ProgressBarModel)
{
	public static readonly Key<TreeViewContainer> TreeViewFindAllContainerKey = Key<TreeViewContainer>.NewKey();

    public TextEditorFindAllState() : this(
    	string.Empty,
    	string.Empty,
    	ImmutableList<TextEditorTextSpan>.Empty,
    	null)
    {
    }
}