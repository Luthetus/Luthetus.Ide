using Fluxor;
using Luthetus.Ide.ClassLib.Context;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.Store.ContextCase;

[FeatureState]
public partial record ContextStates(ImmutableArray<ContextRecord> ActiveContextRecords)
{
    private ContextStates() : this(ImmutableArray<ContextRecord>.Empty)
    {
        ActiveContextRecords = new[]
        {
            ContextFacts.GlobalContext
        }.ToImmutableArray();
    }
}