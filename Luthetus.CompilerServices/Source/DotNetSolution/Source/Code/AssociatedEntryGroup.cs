using System.Collections.Immutable;

namespace Luthetus.CompilerServices.Lang.DotNetSolution.Code;

public record AssociatedEntryGroup : IAssociatedEntry
{
    public ImmutableArray<IAssociatedEntry> AssociatedEntryBag { get; init; }

    public AssociatedEntryKind AssociatedEntryKind => AssociatedEntryKind.Group;
}
