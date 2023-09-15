using Fluxor;
using Luthetus.Ide.RazorLib.ViewsCase.Models;

namespace Luthetus.Ide.RazorLib.FooterCase.States;

public partial record FooterState
{
    private class Reducer
    {
        [ReducerMethod]
        public static FooterState ReduceSetFooterStateViewAction(
            FooterState inFooterState,
            SetFooterStateViewAction setFooterStateViewAction)
        {
            return inFooterState with
            {
                ActiveView = setFooterStateViewAction.View
            };
        }
    }
}