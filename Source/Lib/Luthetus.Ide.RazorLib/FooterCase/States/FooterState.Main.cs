using Fluxor;
using Luthetus.Ide.RazorLib.ViewsCase.Models;

namespace Luthetus.Ide.RazorLib.FooterCase.States;

[FeatureState]
public partial record FooterState(View ActiveView)
{
    public FooterState() : this(ViewFacts.TerminalsView)
    {

    }
}