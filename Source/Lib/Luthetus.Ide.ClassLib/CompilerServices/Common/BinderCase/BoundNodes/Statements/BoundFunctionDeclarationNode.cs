using Luthetus.Ide.ClassLib.CompilerServices.Common.General;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Statements;

public sealed record BoundFunctionDeclarationNode : ISyntaxNode
{
    public BoundFunctionDeclarationNode(
        BoundTypeNode boundTypeNode,
        ISyntaxToken identifierToken,
        BoundFunctionArgumentsNode boundFunctionArgumentsNode,
        CompilationUnit? functionBodyCompilationUnit)
    {
        BoundTypeNode = boundTypeNode;
        IdentifierToken = identifierToken;
        BoundFunctionArgumentsNode = boundFunctionArgumentsNode;
        FunctionBodyCompilationUnit = functionBodyCompilationUnit;

        var childrenList = new List<ISyntax>(3)
        {
            BoundTypeNode,
            IdentifierToken
        };

        if (FunctionBodyCompilationUnit is not null)
            childrenList.Add(FunctionBodyCompilationUnit);

        Children = childrenList.ToImmutableArray();
    }

    public BoundTypeNode BoundTypeNode { get; init; }
    public ISyntaxToken IdentifierToken { get; init; }
    public BoundFunctionArgumentsNode BoundFunctionArgumentsNode { get; init; }
    public CompilationUnit? FunctionBodyCompilationUnit { get; init; }

    public ImmutableArray<ISyntax> Children { get; init; }
    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.BoundFunctionDeclarationNode;
}
