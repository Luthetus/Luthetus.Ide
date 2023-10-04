using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes.Expression;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;

public class VariableExpressionNode : IExpressionNode
{
    public VariableExpressionNode(TypeClauseNode typeClauseNode)
    {
        TypeClauseNode = typeClauseNode;
    }

    public TypeClauseNode TypeClauseNode { get; }

    public ImmutableArray<ISyntax> ChildBag { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.VariableExpressionNode;
}
