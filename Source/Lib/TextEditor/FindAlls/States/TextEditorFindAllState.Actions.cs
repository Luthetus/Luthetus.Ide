using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.FindAlls.States;

public partial record TextEditorFindAllState
{
    public record SetSearchQueryAction(string SearchQuery);
    public record SetStartingDirectoryPathAction(string StartingDirectoryPath);
    public record StartSearchAction;
    public record CancelSearchAction;
    public record ClearSearchAction;
    
    /// <summary>Intended for use only by the 'Effector'</summary>
    public record SetProgressBarModelAction(ProgressBarModel ProgressBarModel);
    
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
    public record FlushSearchResultsAction(List<TextEditorTextSpan> SearchResultList);
}