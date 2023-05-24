using Luthetus.Ide.ClassLib.CodeAnalysis.C.BinderCase.BoundNodes.Expression;
using Luthetus.Ide.ClassLib.CodeAnalysis.C.Syntax.SyntaxTokens;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CodeAnalysis.C.BinderCase.BoundNodes.Statements;

public class BoundVariableAssignmentStatementNode : ISyntaxNode
{
    public BoundVariableAssignmentStatementNode(
        ISyntaxToken identifierToken,
        IBoundExpressionNode boundExpressionNode)
    {
        IdentifierToken = identifierToken;
        BoundExpressionNode = boundExpressionNode;

        Children = new ISyntax[]
        {
            IdentifierToken,
            BoundExpressionNode
        }.ToImmutableArray();
    }

    public ISyntaxToken IdentifierToken { get; }
    public IBoundExpressionNode BoundExpressionNode { get; }

    public ImmutableArray<ISyntax> Children { get; }
    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.BoundVariableAssignmentStatementNode;
}
