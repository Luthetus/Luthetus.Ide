using Fluxor;

namespace Luthetus.Ide.Tests.Basis.CompilerServices.States;

public class CompilerServiceEditorStateReducerTests
{
    public class Reducer
    {
        [ReducerMethod]
        public CompilerServiceEditorState ReduceSetTextEditorViewModelKeyAction(
            CompilerServiceEditorState inState,
            SetTextEditorViewModelKeyAction setTextEditorViewModelKeyAction)
        {
            return inState with
            { 
                TextEditorViewModelKey = setTextEditorViewModelKeyAction.TextEditorViewModelKey
            };
        }
    }
}
