using Fluxor;

namespace Luthetus.TextEditor.RazorLib.FindAlls.States;

public partial class TextEditorFindAllState
{
    public class Reducer
    {
        [ReducerMethod]
        public static TextEditorFindAllState ReduceRegisterAction(
            TextEditorFindAllState inState,
            RegisterAction registerAction)
        {
            var inSearchEngine = inState.SearchEngineList.FirstOrDefault(
                x => x.Key == registerAction.SearchEngine.Key);

            if (inSearchEngine is not null)
                return inState;

            var outSearchEngineList = inState.SearchEngineList.Add(registerAction.SearchEngine);

            return new TextEditorFindAllState(
                outSearchEngineList,
                inState.SearchQuery,
                inState.StartingDirectoryPath,
                inState.Options);
        }

        [ReducerMethod]
        public static TextEditorFindAllState ReduceDisposeAction(
            TextEditorFindAllState inState,
            DisposeAction disposeAction)
        {
            var existingSearchEngine = inState.SearchEngineList.FirstOrDefault(
                x => x.Key == disposeAction.SearchEngineKey);

            if (existingSearchEngine is null)
                return inState;

            var outSearchEngineList = inState.SearchEngineList.Remove(existingSearchEngine);

            return new TextEditorFindAllState(
                outSearchEngineList,
                inState.SearchQuery,
                inState.StartingDirectoryPath,
                inState.Options);
        }

        [ReducerMethod]
        public static TextEditorFindAllState ReduceSetSearchQueryAction(
            TextEditorFindAllState inState,
            SetSearchQueryAction setSearchQueryAction)
        {
            return new TextEditorFindAllState(
                inState.SearchEngineList,
                setSearchQueryAction.SearchQuery,
                inState.StartingDirectoryPath,
                inState.Options);
        }

        [ReducerMethod]
        public static TextEditorFindAllState ReduceSetStartingDirectoryPathAction(
            TextEditorFindAllState inState,
            SetStartingDirectoryPathAction setStartingDirectoryPathAction)
        {
            return new TextEditorFindAllState(
                inState.SearchEngineList,
                inState.SearchQuery,
                setStartingDirectoryPathAction.StartingDirectoryPath,
                inState.Options);
        }
    }
}