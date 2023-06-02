using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Statements;

public sealed record BoundVariableDeclarationStatementNode : ISyntaxNode
{
    public BoundVariableDeclarationStatementNode(
        BoundTypeNode boundTypeNode,
        ISyntaxToken identifierToken,
        bool isInitialized)
    {
        BoundTypeNode = boundTypeNode;
        IdentifierToken = identifierToken;
        IsInitialized = isInitialized;

        Children = new ISyntax[]
        {
            BoundTypeNode,
            IdentifierToken
        }.ToImmutableArray();
    }

    public BoundTypeNode BoundTypeNode { get; init; }
    public ISyntaxToken IdentifierToken { get; init; }
    public bool IsInitialized { get; init; }

    public ImmutableArray<ISyntax> Children { get; init; }
    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.BoundVariableDeclarationStatementNode;
}
