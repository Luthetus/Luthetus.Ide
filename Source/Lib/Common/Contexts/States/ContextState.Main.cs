using Fluxor;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using System.Collections.Immutable;

namespace Luthetus.Common.RazorLib.Contexts.States;

/// <summary>
/// The list provided should not be modified after passing it as a parameter.
/// Make a shallow copy, and pass the shallow copy, if further modification of your list will be necessary.
/// </summary>
[FeatureState]
public partial record ContextState(
    List<ContextRecord> AllContextsList,
    ContextHeirarchy FocusedContextHeirarchy,
    ContextHeirarchy? InspectedContextHeirarchy,
    List<InspectableContext> InspectableContextList,
    bool IsSelectingInspectionTarget)
{
    public ContextState() : this(
        new List<ContextRecord>(),
        new(new List<Key<ContextRecord>>()),
        null,
        new List<InspectableContext>(),
        false)
    {
        FocusedContextHeirarchy = new ContextHeirarchy(new List<Key<ContextRecord>>
        {
            ContextFacts.GlobalContext.ContextKey
        });

        AllContextsList = ContextFacts.AllContextsList;
    }
}