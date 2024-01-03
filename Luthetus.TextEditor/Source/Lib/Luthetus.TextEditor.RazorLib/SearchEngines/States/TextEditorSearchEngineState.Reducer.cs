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
            var inSearchEngine = inState.SearchEngineBag.FirstOrDefault(
                x => x.SearchEngineKey == registerAction.SearchEngine.SearchEngineKey);

            if (inSearchEngine is not null)
                return inState;

            var outSearchEngineBag = inState.SearchEngineBag.Add(registerAction.SearchEngine);

            return new TextEditorSearchEngineState(
                outSearchEngineBag,
                inState.ActiveSearchEngineKey,
                inState.SearchQuery);
        }

        [ReducerMethod]
        public static TextEditorSearchEngineState ReduceDisposeAction(
            TextEditorSearchEngineState inState,
            DisposeAction disposeAction)
        {
            var existingSearchEngine = inState.SearchEngineBag.FirstOrDefault(
                x => x.SearchEngineKey == disposeAction.SearchEngineKey);

            if (existingSearchEngine is null)
                return inState;

            var outSearchEngineBag = inState.SearchEngineBag.Remove(existingSearchEngine);

            return new TextEditorSearchEngineState(
                outSearchEngineBag,
                inState.ActiveSearchEngineKey,
                inState.SearchQuery);
        }

        [ReducerMethod]
        public static TextEditorSearchEngineState ReduceSetActiveSearchEngineAction(
            TextEditorSearchEngineState inState,
            SetActiveSearchEngineAction setActiveSearchEngineAction)
        {
            return new TextEditorSearchEngineState(
                inState.SearchEngineBag,
                setActiveSearchEngineAction.SearchEngineKey,
                inState.SearchQuery);
        }

        [ReducerMethod]
        public static TextEditorSearchEngineState ReduceSetSearchQueryAction(
            TextEditorSearchEngineState inState,
            SetSearchQueryAction setSearchQueryAction)
        {
            return new TextEditorSearchEngineState(
                inState.SearchEngineBag,
                inState.ActiveSearchEngineKey,
                setSearchQueryAction.SearchQuery);
        }
    }
}