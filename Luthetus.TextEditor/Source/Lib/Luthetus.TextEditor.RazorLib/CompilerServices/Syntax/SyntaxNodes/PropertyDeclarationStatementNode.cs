using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;

public sealed record PropertyDeclarationStatementNode : ISyntaxNode
{
    public PropertyDeclarationStatementNode(
        TypeClauseNode typeClauseNode,
        IdentifierToken identifierToken,
        bool isInitialized)
    {
        TypeClauseNode = typeClauseNode;
        IdentifierToken = identifierToken;
        IsInitialized = isInitialized;

        ChildBag = new ISyntax[]
        {
            TypeClauseNode,
            IdentifierToken,
        }.ToImmutableArray();
    }

    public TypeClauseNode TypeClauseNode { get; }
    public IdentifierToken IdentifierToken { get; }
    public bool IsInitialized { get; }

    public ImmutableArray<ISyntax> ChildBag { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.PropertyDeclarationStatementNode;
}
