using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

using System.Collections.Immutable;
using Fluxor;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

using System.Collections.Immutable;
using Fluxor;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.FindAlls.Models;

namespace Luthetus.TextEditor.RazorLib.FindAlls.Models;

public interface IFindAllService : IDisposable
{
	public event Action? FindAllStateChanged;
	
	public TextEditorFindAllState GetFindAllState();
    
    public void ReduceSetSearchQueryAction(string searchQuery);

    public void ReduceSetStartingDirectoryPathAction(string startingDirectoryPath);

    public void ReduceCancelSearchAction();

	/// <summary>Intended for use only by the 'Effector'</summary>
    public void ReduceSetProgressBarModelAction(ProgressBarModel progressBarModel);

	/// <summary>
    /// While the search task is being executed, any search results are being
    /// added to a list separate to that of the <see cref="TextEditorFindAllState"/>.
    ///
    /// Therefore, the UI will not update while the search task is running.
    /// To update the UI while the search task is running, use this action.
    ///
    /// This will pause the search, and MOVE any search results from
    /// the task's hidden list, to the <see cref="TextEditorFindAllState"/>'s public list.
    ///
    /// Then, the search will continue.
    /// </summary>
    public void ReduceFlushSearchResultsAction(List<TextEditorTextSpan> searchResultList);

    public void ReduceClearSearchAction();
	
	public Task HandleStartSearchAction();
}
