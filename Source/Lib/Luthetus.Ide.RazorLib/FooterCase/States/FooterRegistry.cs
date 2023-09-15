using Fluxor;
using Luthetus.Ide.RazorLib.ViewsCase.Models;

namespace Luthetus.Ide.RazorLib.FooterCase.States;

[FeatureState]
public record FooterRegistry(View ActiveView)
{
    public FooterRegistry() : this(ViewFacts.TerminalsView)
    {

    }

    public record SetFooterStateViewAction(View View);

    private class FooterStateReducer
    {
        [ReducerMethod]
        public static FooterRegistry ReduceSetFooterStateViewAction(
            FooterRegistry inFooterState,
            SetFooterStateViewAction setFooterStateViewAction)
        {
            return inFooterState with
            {
                ActiveView = setFooterStateViewAction.View
            };
        }
    }
}