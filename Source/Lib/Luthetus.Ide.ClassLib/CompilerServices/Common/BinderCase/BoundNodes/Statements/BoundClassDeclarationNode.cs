using Luthetus.Ide.ClassLib.CompilerServices.Common.General;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Statements;

public sealed record BoundClassDeclarationNode : ISyntaxNode
{
    public BoundClassDeclarationNode(
        ISyntaxToken identifierToken)
    {
        IdentifierToken = identifierToken;

        Children = new ISyntax[]
        {
            IdentifierToken
        }.ToImmutableArray();
    }

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

    public ISyntaxToken IdentifierToken { get; }
    public BoundInheritanceStatementNode? BoundInheritanceStatementNode { get; }
    public CompilationUnit? ClassBodyCompilationUnit { get; }

    public ImmutableArray<ISyntax> Children { get; }
    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.BoundClassDeclarationNode;

    public BoundClassDeclarationNode WithClassBody(
        CompilationUnit classBodyCompilationUnit)
    {
        return new BoundClassDeclarationNode(
            IdentifierToken,
            BoundInheritanceStatementNode,
            classBodyCompilationUnit);
    }
    
    public BoundClassDeclarationNode WithBoundInheritanceStatementNode(
        BoundInheritanceStatementNode boundInheritanceStatementNode)
    {
        return new BoundClassDeclarationNode(
            IdentifierToken,
            boundInheritanceStatementNode,
            ClassBodyCompilationUnit);
    }
}
