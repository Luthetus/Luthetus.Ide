using System.Collections.Immutable;

namespace Luthetus.CompilerServices.Lang.DotNetSolution.Code;

public class AssociatedEntryGroupBuilder
{
    public AssociatedEntryGroupBuilder(AssociatedEntryGroupBuilder parent)
    {
        Parent = parent;
    }

    public List<IAssociatedEntry> AssociatedEntryBag { get; } = new();
    public AssociatedEntryGroupBuilder Parent { get; }

    public AssociatedEntryGroup Build()
    {
        var group = new AssociatedEntryGroup(AssociatedEntryBag.ToImmutableArray());

        Parent.AssociatedEntryBag.Add(group);
        return group;
    }
}
