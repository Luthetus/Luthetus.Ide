using Fluxor;

namespace Luthetus.TextEditor.RazorLib.SearchEngines.States;

public partial class TextEditorSearchEngineState
{
    public class Reducer
    {
        [ReducerMethod]
        public static TextEditorSearchEngineState ReduceRegisterAction(
            TextEditorSearchEngineState inState,
            RegisterAction registerAction)
        {
            var inSearchEngine = inState.SearchEngineList.FirstOrDefault(
                x => x.SearchEngineKey == registerAction.SearchEngine.SearchEngineKey);

            if (inSearchEngine is not null)
                return inState;

            var outSearchEngineList = inState.SearchEngineList.Add(registerAction.SearchEngine);

            return new TextEditorSearchEngineState(
                outSearchEngineList,
                inState.SearchQuery);
        }

        [ReducerMethod]
        public static TextEditorSearchEngineState ReduceDisposeAction(
            TextEditorSearchEngineState inState,
            DisposeAction disposeAction)
        {
            var existingSearchEngine = inState.SearchEngineList.FirstOrDefault(
                x => x.SearchEngineKey == disposeAction.SearchEngineKey);

            if (existingSearchEngine is null)
                return inState;

            var outSearchEngineList = inState.SearchEngineList.Remove(existingSearchEngine);

            return new TextEditorSearchEngineState(
                outSearchEngineList,
                inState.SearchQuery);
        }

        [ReducerMethod]
        public static TextEditorSearchEngineState ReduceSetSearchQueryAction(
            TextEditorSearchEngineState inState,
            SetSearchQueryAction setSearchQueryAction)
        {
            return new TextEditorSearchEngineState(
                inState.SearchEngineList,
                setSearchQueryAction.SearchQuery);
        }
    }
}