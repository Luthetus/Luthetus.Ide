using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Statements;

public sealed record BoundVariableDeclarationStatementNode : ISyntaxNode
{
    public BoundVariableDeclarationStatementNode(
        BoundClassDeclarationNode boundClassDeclarationNode,
        ISyntaxToken identifierToken,
        bool isInitialized)
    {
        BoundClassDeclarationNode = boundClassDeclarationNode;
        IdentifierToken = identifierToken;
        IsInitialized = isInitialized;

        Children = new ISyntax[]
        {
            BoundClassDeclarationNode,
            IdentifierToken
        }.ToImmutableArray();
    }

    public BoundClassDeclarationNode BoundClassDeclarationNode { get; init; }
    public ISyntaxToken IdentifierToken { get; init; }
    public bool IsInitialized { get; init; }

    public ImmutableArray<ISyntax> Children { get; init; }
    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.BoundVariableDeclarationStatementNode;
}
