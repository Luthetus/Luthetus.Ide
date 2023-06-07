using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Statements;

public sealed record BoundVariableDeclarationStatementNode : ISyntaxNode
{
    public BoundVariableDeclarationStatementNode(
        BoundClassReferenceNode boundClassReferenceNode,
        ISyntaxToken identifierToken,
        bool isInitialized)
    {
        BoundClassReferenceNode = boundClassReferenceNode;
        IdentifierToken = identifierToken;
        IsInitialized = isInitialized;

        Children = new ISyntax[]
        {
            BoundClassReferenceNode,
            IdentifierToken
        }.ToImmutableArray();
    }

    public BoundClassReferenceNode BoundClassReferenceNode { get; init; }
    public ISyntaxToken IdentifierToken { get; init; }
    public bool IsInitialized { get; init; }

    public ImmutableArray<ISyntax> Children { get; init; }
    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.BoundVariableDeclarationStatementNode;
}
