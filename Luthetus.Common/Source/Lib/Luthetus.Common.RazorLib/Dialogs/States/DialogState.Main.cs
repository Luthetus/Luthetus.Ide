using System.Collections.Immutable;
using Fluxor;
using Luthetus.Common.RazorLib.Dialogs.Models;

namespace Luthetus.Common.RazorLib.Dialogs.States;

[FeatureState]
public partial record DialogState
{
    private DialogState()
    {
        DialogBag = ImmutableList<DialogRecord>.Empty;
    }

    public ImmutableList<DialogRecord> DialogBag { get; init; }
}