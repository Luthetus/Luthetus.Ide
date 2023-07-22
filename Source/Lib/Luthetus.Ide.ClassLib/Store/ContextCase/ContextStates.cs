using Fluxor;
using Luthetus.Ide.ClassLib.Context;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.Store.ContextCase;

[FeatureState]
public record ContextStates(ImmutableArray<ContextRecord> ActiveContextRecords)
{
    public ContextStates() : this(ImmutableArray<ContextRecord>.Empty)
    {
        ActiveContextRecords = new[]
        {
        ContextFacts.GlobalContext
    }.ToImmutableArray();
    }
}