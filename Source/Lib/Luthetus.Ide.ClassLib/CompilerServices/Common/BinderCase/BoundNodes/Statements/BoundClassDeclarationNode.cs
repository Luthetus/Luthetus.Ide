using Luthetus.Ide.ClassLib.CompilerServices.Common.General;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Statements;

public sealed record BoundClassDeclarationNode : ISyntaxNode
{
    public BoundClassDeclarationNode(
        ISyntaxToken identifierToken,
        BoundInheritanceStatementNode? boundInheritanceStatementNode,
        CompilationUnit? classBodyCompilationUnit)
    {
        IdentifierToken = identifierToken;
        BoundInheritanceStatementNode = boundInheritanceStatementNode;
        ClassBodyCompilationUnit = classBodyCompilationUnit;

        var childrenList = new List<ISyntax>(3)
        {
            IdentifierToken
        };

        if (BoundInheritanceStatementNode is not null)
            childrenList.Add(BoundInheritanceStatementNode);
        
        if (ClassBodyCompilationUnit is not null)
            childrenList.Add(ClassBodyCompilationUnit);

        Children = childrenList.ToImmutableArray();
    }

    public ISyntaxToken IdentifierToken { get; init; }
    public BoundInheritanceStatementNode? BoundInheritanceStatementNode { get; init; }
    public CompilationUnit? ClassBodyCompilationUnit { get; init; }

    public ImmutableArray<ISyntax> Children { get; init; }
    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.BoundClassDeclarationNode;
}
