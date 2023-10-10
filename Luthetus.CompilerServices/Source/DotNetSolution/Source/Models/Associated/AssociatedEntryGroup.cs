using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;
using System.Collections.Immutable;

namespace Luthetus.CompilerServices.Lang.DotNetSolution.Models.Associated;

public record AssociatedEntryGroup : IAssociatedEntry
{
    public AssociatedEntryGroup(
        OpenAssociatedGroupToken openAssociatedGroupToken,
        ImmutableArray<IAssociatedEntry> associatedEntryBag,
        CloseAssociatedGroupToken? closeAssociatedGroupToken)
    {
        OpenAssociatedGroupToken = openAssociatedGroupToken;
        AssociatedEntryBag = associatedEntryBag;
        CloseAssociatedGroupToken = closeAssociatedGroupToken;
    }

    public OpenAssociatedGroupToken OpenAssociatedGroupToken { get; }
    public ImmutableArray<IAssociatedEntry> AssociatedEntryBag { get; init; }
    public CloseAssociatedGroupToken? CloseAssociatedGroupToken { get; }

    public AssociatedEntryKind AssociatedEntryKind => AssociatedEntryKind.Group;
}
