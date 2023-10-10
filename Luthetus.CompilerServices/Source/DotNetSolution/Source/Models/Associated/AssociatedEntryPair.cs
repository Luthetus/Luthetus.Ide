using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;

namespace Luthetus.CompilerServices.Lang.DotNetSolution.Models.Associated;

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