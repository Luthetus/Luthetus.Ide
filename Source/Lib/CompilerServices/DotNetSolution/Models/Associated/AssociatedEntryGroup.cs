using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;

namespace Luthetus.CompilerServices.DotNetSolution.Models.Associated;

public record AssociatedEntryGroup : IAssociatedEntry
{
    public AssociatedEntryGroup(
        SyntaxToken openAssociatedGroupToken,
        ImmutableArray<IAssociatedEntry> associatedEntryList,
        SyntaxToken closeAssociatedGroupToken)
    {
        OpenAssociatedGroupToken = openAssociatedGroupToken;
        AssociatedEntryList = associatedEntryList;
        CloseAssociatedGroupToken = closeAssociatedGroupToken;
    }

    public SyntaxToken OpenAssociatedGroupToken { get; }
    public ImmutableArray<IAssociatedEntry> AssociatedEntryList { get; init; }
    public SyntaxToken CloseAssociatedGroupToken { get; }

    public AssociatedEntryKind AssociatedEntryKind => AssociatedEntryKind.Group;
}
