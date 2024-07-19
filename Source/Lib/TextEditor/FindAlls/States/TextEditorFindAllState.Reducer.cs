using System.Collections.Immutable;
using Fluxor;

namespace Luthetus.TextEditor.RazorLib.FindAlls.States;

public partial record TextEditorFindAllState
{
    public class Reducer
    {
        [ReducerMethod]
        public static TextEditorFindAllState ReduceSetSearchQueryAction(
            TextEditorFindAllState inState,
            SetSearchQueryAction setSearchQueryAction)
        {
            return inState with
            {
            	SearchQuery = setSearchQueryAction.SearchQuery
            };
        }

        [ReducerMethod]
        public static TextEditorFindAllState ReduceSetStartingDirectoryPathAction(
            TextEditorFindAllState inState,
            SetStartingDirectoryPathAction setStartingDirectoryPathAction)
        {
            return inState with
            {
            	StartingDirectoryPath = setStartingDirectoryPathAction.StartingDirectoryPath
            };
        }

        [ReducerMethod]
        public static TextEditorFindAllState ReduceCancelSearchAction(
            TextEditorFindAllState inState,
            CancelSearchAction cancelSearchAction)
        {
        	inState._searchCancellationTokenSource.Cancel();
        	inState._searchCancellationTokenSource = new();
        	
            return inState with {};
        }

        [ReducerMethod]
        public static TextEditorFindAllState ReduceSetProgressBarModelAction(
            TextEditorFindAllState inState,
            SetProgressBarModelAction setProgressBarModelAction)
        {
            return inState with
            {
            	ProgressBarModel = setProgressBarModelAction.ProgressBarModel,
            };
        }

        [ReducerMethod]
        public static TextEditorFindAllState ReduceFlushSearchResultsAction(
            TextEditorFindAllState inState,
            FlushSearchResultsAction flushSearchResultsAction)
        {
        	List<string> localSearchResultList;
        	lock (inState._flushSearchResultsLock)
        	{
      		  localSearchResultList = new List<string>(inState.SearchResultList);
        		localSearchResultList.AddRange(flushSearchResultsAction.SearchResultList);
        		flushSearchResultsAction.SearchResultList.Clear();
        	}
        	
            return inState with
            {
            	SearchResultList = localSearchResultList.ToImmutableList()
            };
        }
    }
}