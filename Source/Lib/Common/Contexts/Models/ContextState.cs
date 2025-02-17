using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.Contexts.Models;

/// <summary>
/// The list provided should not be modified after passing it as a parameter.
/// Make a shallow copy, and pass the shallow copy, if further modification of your list will be necessary.
/// </summary>
public record struct ContextState(
    IReadOnlyList<ContextRecord> AllContextsList,
    ContextHeirarchy FocusedContextHeirarchy,
    ContextHeirarchy? InspectedContextHeirarchy,
    IReadOnlyList<InspectableContext> InspectableContextList,
    bool IsSelectingInspectionTarget)
{
    public ContextState() : this(
        Array.Empty<ContextRecord>(),
        new(Array.Empty<Key<ContextRecord>>()),
        null,
        Array.Empty<InspectableContext>(),
        false)
    {
        FocusedContextHeirarchy = new ContextHeirarchy(new List<Key<ContextRecord>>
        {
            ContextFacts.GlobalContext.ContextKey
        });

        AllContextsList = ContextFacts.AllContextsList;
    }
}