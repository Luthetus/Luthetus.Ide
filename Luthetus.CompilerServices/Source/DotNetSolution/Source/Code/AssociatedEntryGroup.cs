using System.Collections.Immutable;

namespace Luthetus.CompilerServices.Lang.DotNetSolution.Code;

public record AssociatedEntryGroup : IAssociatedEntry
{
    public AssociatedEntryGroup(ImmutableArray<IAssociatedEntry> associatedEntryBag)
    {
        AssociatedEntryBag = associatedEntryBag;
    }

    public ImmutableArray<IAssociatedEntry> AssociatedEntryBag { get; init; }

    public AssociatedEntryKind AssociatedEntryKind => AssociatedEntryKind.Group;
}
