using System.Collections.Immutable;

namespace Luthetus.Common.RazorLib.Reflectives.Models;

public record struct ReflectiveState
{
    public ReflectiveState()
    {
        ReflectiveModelList = ImmutableList<ReflectiveModel>.Empty;
    }

    public ImmutableList<ReflectiveModel> ReflectiveModelList { get; init; }
}