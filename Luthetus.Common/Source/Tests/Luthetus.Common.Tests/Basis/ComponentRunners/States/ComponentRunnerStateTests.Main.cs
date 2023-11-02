using System.Collections.Immutable;
using Fluxor;
using Luthetus.Common.RazorLib.ComponentRunners.Internals.Classes;

namespace Luthetus.Common.RazorLib.ComponentRunners.States;

[FeatureState]
public partial record ComponentRunnerStateTests
{
    private ComponentRunnerState()
    {
        ComponentRunnerDisplayStateBag = ImmutableList<ComponentRunnerDisplayState>.Empty;
    }

    public ImmutableList<ComponentRunnerDisplayState> ComponentRunnerDisplayStateBag { get; init; }
}