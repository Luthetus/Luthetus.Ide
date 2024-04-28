using System.Collections.Immutable;
using Fluxor;
using Luthetus.Common.RazorLib.Reflectives.Models;

namespace Luthetus.Common.RazorLib.Reflectives.States;

[FeatureState]
public partial record ReflectiveState
{
    public ReflectiveState()
    {
        ReflectiveModelList = ImmutableList<ReflectiveModel>.Empty;
    }

    public ImmutableList<ReflectiveModel> ReflectiveModelList { get; init; }
}