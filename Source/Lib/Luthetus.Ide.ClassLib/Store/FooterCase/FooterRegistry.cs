using Fluxor;
using Luthetus.Ide.ClassLib.Views;

namespace Luthetus.Ide.ClassLib.Store.FooterCase;

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