using Luthetus.Ide.ClassLib.Context;

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