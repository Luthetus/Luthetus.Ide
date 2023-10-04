using Fluxor;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using System.Collections.Immutable;

namespace Luthetus.Common.RazorLib.Contexts.States;

/// <summary>
/// TODO: SphagettiCode - Do the member names need to be so obnoxiously long? (2023-09-19)
/// </summary>
[FeatureState]
public partial record ContextState(
    ImmutableArray<ContextRecord> AllContextRecordsBag,
    TargetContextRecordKeyAndHeirarchyBag FocusedKeyHeirarchyBag,
    TargetContextRecordKeyAndHeirarchyBag? InspectedKeyHeirarchyBag,
    ImmutableArray<InspectContextRecordEntry> InspectableKeyHeirarchyBag,
    bool IsSelectingInspectionTarget)
{
    private ContextState() : this(
        ImmutableArray<ContextRecord>.Empty,
        new(ImmutableArray<Key<ContextRecord>>.Empty),
        null,
        ImmutableArray<InspectContextRecordEntry>.Empty,
        false)
    {
        FocusedKeyHeirarchyBag = new TargetContextRecordKeyAndHeirarchyBag(new[]
        {
            ContextFacts.GlobalContext.ContextKey
        }.ToImmutableArray());

        AllContextRecordsBag = ContextFacts.AllContextRecordsBag;
    }
}