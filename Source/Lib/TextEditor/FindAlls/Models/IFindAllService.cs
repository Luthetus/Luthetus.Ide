using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.FindAlls.Models;

public interface IFindAllService : IDisposable
{
	public event Action? FindAllStateChanged;
	
	public TextEditorFindAllState GetFindAllState();
    
    public void SetSearchQuery(string searchQuery);

    public void SetStartingDirectoryPath(string startingDirectoryPath);

    public void CancelSearch();

	/// <summary>Intended for use only by the 'Effector'</summary>
    public void SetProgressBarModel(ProgressBarModel progressBarModel);

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
    public void FlushSearchResults(List<TextEditorTextSpan> searchResultList);

    public void ClearSearch();
	
	public Task HandleStartSearchAction();
}
