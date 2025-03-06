using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.Extensions.CompilerServices.Syntax;

namespace Luthetus.CompilerServices.DotNetSolution.Models.Associated;

public record AssociatedEntryPair : IAssociatedEntry
{
    public AssociatedEntryPair(
        SyntaxToken associatedNameToken,
        SyntaxToken associatedValueToken)
    {
        AssociatedNameToken = associatedNameToken;
        AssociatedValueToken = associatedValueToken;
    }

    public SyntaxToken AssociatedNameToken { get; init; }
    public SyntaxToken AssociatedValueToken { get; init; }

    public AssociatedEntryKind AssociatedEntryKind => AssociatedEntryKind.Pair;
}