using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes.Expression;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;

/// <summary>
/// TODO: Is this class being used?...
/// ...It seems <see cref="VariableReferenceNode"/> is a replacement for...
/// ...this class and that not all the code has been changed yet.
/// </summary>
public class VariableExpressionNode : IExpressionNode
{
    public VariableExpressionNode(TypeClauseNode typeClauseNode)
    {
        ResultTypeClauseNode = typeClauseNode;

        var childList = new List<ISyntax>
        {
            ResultTypeClauseNode
        };

        ChildList = childList.ToImmutableArray();
    }

    public TypeClauseNode ResultTypeClauseNode { get; }

    public ImmutableArray<ISyntax> ChildList { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.VariableExpressionNode;
}
