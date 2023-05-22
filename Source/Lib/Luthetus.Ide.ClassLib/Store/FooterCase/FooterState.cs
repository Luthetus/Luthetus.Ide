using Fluxor;
using Luthetus.Ide.ClassLib.Views;

namespace Luthetus.Ide.ClassLib.Store.FooterCase;

[FeatureState]
public record FooterState(View ActiveView)
{
    public FooterState() : this(ViewFacts.TerminalsView)
    {

    }

    public record SetFooterStateViewAction(View View);

    private class FooterStateReducer
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