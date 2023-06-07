using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax.SyntaxTokens;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Statements;

public sealed record BoundInheritanceStatementNode : ISyntaxNode
{
    public BoundInheritanceStatementNode(
        BoundClassDeclarationNode parentBoundClassDeclarationNode)
    {
        ParentBoundClassDeclarationNode = parentBoundClassDeclarationNode;
        
        Children = new ISyntax[]
        {
            ParentBoundClassDeclarationNode
        }.ToImmutableArray();
    }

    public BoundClassDeclarationNode ParentBoundClassDeclarationNode { get; init; }

    public ImmutableArray<ISyntax> Children { get; init; }
    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.BoundInheritanceStatementNode;
}
