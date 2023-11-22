using System.Collections.Immutable;
using Fluxor;
using Luthetus.Common.RazorLib.ComponentRunners.Models;

namespace Luthetus.Common.RazorLib.ComponentRunners.States;

[FeatureState]
public partial record ComponentRunnerState
{
    public ComponentRunnerState()
    {
        ComponentRunnerDisplayStateBag = ImmutableList<ComponentRunnerDisplayState>.Empty;
    }

    public ImmutableList<ComponentRunnerDisplayState> ComponentRunnerDisplayStateBag { get; init; }
}