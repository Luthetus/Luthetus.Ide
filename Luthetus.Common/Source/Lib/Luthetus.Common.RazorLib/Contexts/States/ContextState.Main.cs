using Fluxor;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using System.Collections.Immutable;

namespace Luthetus.Common.RazorLib.Contexts.States;

[FeatureState]
public partial record ContextState(
    ImmutableArray<ContextRecord> AllContextsList,
    ContextHeirarchy FocusedContextHeirarchy,
    ContextHeirarchy? InspectedContextHeirarchy,
    ImmutableArray<InspectableContext> InspectableContextList,
    bool IsSelectingInspectionTarget)
{
    public ContextState() : this(
        ImmutableArray<ContextRecord>.Empty,
        new(ImmutableArray<Key<ContextRecord>>.Empty),
        null,
        ImmutableArray<InspectableContext>.Empty,
        false)
    {
        FocusedContextHeirarchy = new ContextHeirarchy(new[]
        {
            ContextFacts.GlobalContext.ContextKey
        }.ToImmutableArray());

        AllContextsList = ContextFacts.AllContextsList;
    }
}