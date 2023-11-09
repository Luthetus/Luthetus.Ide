using Fluxor;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using System.Collections.Immutable;

namespace Luthetus.Common.RazorLib.Contexts.States;

[FeatureState]
public partial record ContextState(
    ImmutableArray<ContextRecord> AllContextRecordsBag,
    ContextRecordKeyHeirarchy FocusedContextRecordKeyHeirarchy,
    ContextRecordKeyHeirarchy? InspectedContextRecordKeyHeirarchy,
    ImmutableArray<InspectContextRecordEntry> InspectContextRecordEntryBag,
    bool IsSelectingInspectionTarget)
{
    public ContextState() : this(
        ImmutableArray<ContextRecord>.Empty,
        new(ImmutableArray<Key<ContextRecord>>.Empty),
        null,
        ImmutableArray<InspectContextRecordEntry>.Empty,
        false)
    {
        FocusedContextRecordKeyHeirarchy = new ContextRecordKeyHeirarchy(new[]
        {
            ContextFacts.GlobalContext.ContextKey
        }.ToImmutableArray());

        AllContextRecordsBag = ContextFacts.AllContextRecordsBag;
    }
}