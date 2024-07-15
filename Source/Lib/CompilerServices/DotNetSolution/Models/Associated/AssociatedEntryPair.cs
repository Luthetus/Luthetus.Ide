using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

namespace Luthetus.CompilerServices.DotNetSolution.Models.Associated;

public record AssociatedEntryPair : IAssociatedEntry
{
    public AssociatedEntryPair(
        AssociatedNameToken associatedNameToken,
        AssociatedValueToken associatedValueToken)
    {
        AssociatedNameToken = associatedNameToken;
        AssociatedValueToken = associatedValueToken;
    }

    public AssociatedNameToken AssociatedNameToken { get; init; }
    public AssociatedValueToken AssociatedValueToken { get; init; }

    public AssociatedEntryKind AssociatedEntryKind => AssociatedEntryKind.Pair;
}