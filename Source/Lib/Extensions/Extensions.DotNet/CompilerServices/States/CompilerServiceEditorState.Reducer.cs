using Fluxor;

namespace Luthetus.Extensions.DotNet.CompilerServices.States;

public partial record CompilerServiceEditorState
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
