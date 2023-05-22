using Luthetus.Ide.ClassLib.CodeAnalysis.C.BinderCase.BoundNodes;
using Luthetus.Ide.ClassLib.CodeAnalysis.C.Syntax;
using Luthetus.Ide.ClassLib.CodeAnalysis.C.Syntax.SyntaxNodes;
using Luthetus.Ide.ClassLib.CodeAnalysis.C.Syntax.SyntaxTokens;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CodeAnalysis.C.BinderCase.BoundNodes.Statements;

public class BoundVariableDeclarationStatementNode : ISyntaxNode
{
    public BoundVariableDeclarationStatementNode(
        BoundTypeNode boundTypeNode,
        ISyntaxToken identifierToken)
    {
        BoundTypeNode = boundTypeNode;
        IdentifierToken = identifierToken;

        Children = new ISyntax[]
        {
            BoundTypeNode,
            IdentifierToken
        }.ToImmutableArray();
    }

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

    public BoundTypeNode BoundTypeNode { get; }
    public ISyntaxToken IdentifierToken { get; }
    public bool IsInitialized { get; }

    public ImmutableArray<ISyntax> Children { get; }
    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.BoundVariableDeclarationStatementNode;

    public BoundVariableDeclarationStatementNode WithIsInitialized(
        bool isInitialized)
    {
        return new BoundVariableDeclarationStatementNode(
            BoundTypeNode,
            IdentifierToken,
            isInitialized);
    }
}
