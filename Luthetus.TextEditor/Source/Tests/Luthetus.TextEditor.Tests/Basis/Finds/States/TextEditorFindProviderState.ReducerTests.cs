using Fluxor;

namespace Luthetus.TextEditor.Tests.Basis.Finds.States;

public partial class TextEditorFindProviderStateTests
{
    private class Reducer
    {
        [ReducerMethod]
        public static TextEditorFindProviderState ReduceRegisterAction(
            TextEditorFindProviderState inState,
            RegisterAction registerAction)
        {
            var inFindProvider = inState.FindProviderBag.FirstOrDefault(
                x => x.FindProviderKey == registerAction.FindProvider.FindProviderKey);

            if (inFindProvider is not null)
                return inState;

            var outFindProviderBag = inState.FindProviderBag.Add(registerAction.FindProvider);

            return new TextEditorFindProviderState(
                outFindProviderBag,
                inState.ActiveFindProviderKey,
                inState.SearchQuery);
        }

        [ReducerMethod]
        public static TextEditorFindProviderState ReduceDisposeAction(
            TextEditorFindProviderState inState,
            DisposeAction disposeAction)
        {
            var existingFindProvider = inState.FindProviderBag.FirstOrDefault(
                x => x.FindProviderKey == disposeAction.FindProviderKey);

            if (existingFindProvider is null)
                return inState;

            var outFindProviderBag = inState.FindProviderBag.Remove(existingFindProvider);

            return new TextEditorFindProviderState(
                outFindProviderBag,
                inState.ActiveFindProviderKey,
                inState.SearchQuery);
        }

        [ReducerMethod]
        public static TextEditorFindProviderState ReduceSetActiveFindProviderAction(
            TextEditorFindProviderState inState,
            SetActiveFindProviderAction setActiveFindProviderAction)
        {
            return new TextEditorFindProviderState(
                inState.FindProviderBag,
                setActiveFindProviderAction.FindProviderKey,
                inState.SearchQuery);
        }

        [ReducerMethod]
        public static TextEditorFindProviderState ReduceSetSearchQueryAction(
            TextEditorFindProviderState inState,
            SetSearchQueryAction setSearchQueryAction)
        {
            return new TextEditorFindProviderState(
                inState.FindProviderBag,
                inState.ActiveFindProviderKey,
                setSearchQueryAction.SearchQuery);
        }
    }
}