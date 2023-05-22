using Luthetus.Ide.ClassLib.CodeAnalysis.C.Syntax;
using Luthetus.Ide.ClassLib.CodeAnalysis.C.Syntax.SyntaxNodes;
using Luthetus.Ide.ClassLib.CodeAnalysis.C.Syntax.SyntaxTokens;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CodeAnalysis.C.BinderCase.BoundNodes.Statements;

public class BoundFunctionDeclarationNode : ISyntaxNode
{
    public BoundFunctionDeclarationNode(
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

    public BoundFunctionDeclarationNode(
        BoundTypeNode boundTypeNode,
        ISyntaxToken identifierToken,
        CompilationUnit functionBodyCompilationUnit)
    {
        BoundTypeNode = boundTypeNode;
        IdentifierToken = identifierToken;
        FunctionBodyCompilationUnit = functionBodyCompilationUnit;

        Children = new ISyntax[]
        {
            BoundTypeNode,
            IdentifierToken,
            FunctionBodyCompilationUnit
        }.ToImmutableArray();
    }

    public BoundTypeNode BoundTypeNode { get; }
    public ISyntaxToken IdentifierToken { get; }
    public CompilationUnit? FunctionBodyCompilationUnit { get; }

    public ImmutableArray<ISyntax> Children { get; }
    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.BoundFunctionDeclarationNode;

    public BoundFunctionDeclarationNode WithFunctionBody(
        CompilationUnit functionBodyCompilationUnit)
    {
        return new BoundFunctionDeclarationNode(
            BoundTypeNode,
            IdentifierToken,
            functionBodyCompilationUnit);
    }
}
