using System.Collections.Immutable;

namespace Luthetus.CompilerServices.Lang.DotNetSolution.Code;

public class AssociatedEntryGroupBuilder
{
    public AssociatedEntryGroupBuilder(Action<AssociatedEntryGroup> onAfterBuildAction)
    {
        OnAfterBuildAction = onAfterBuildAction;
    }

    public Action<AssociatedEntryGroup> OnAfterBuildAction { get; }
    public List<IAssociatedEntry> AssociatedEntryBag { get; } = new();

    public virtual AssociatedEntryGroup Build()
    {
        var group = new AssociatedEntryGroup(AssociatedEntryBag.ToImmutableArray());

        OnAfterBuildAction.Invoke(group);
        return group;
    }
}
