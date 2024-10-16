using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed class LiteralExpressionNode : IExpressionNode
{
    public LiteralExpressionNode(ISyntaxToken literalSyntaxToken, TypeClauseNode typeClauseNode)
    {
        LiteralSyntaxToken = literalSyntaxToken;
        ResultTypeClauseNode = typeClauseNode;

        var children = new List<ISyntax>
        {
            LiteralSyntaxToken,
            ResultTypeClauseNode
        };

        ChildList = children.ToImmutableArray();
    }

    public ISyntaxToken LiteralSyntaxToken { get; }
    public TypeClauseNode ResultTypeClauseNode { get; }

    public ImmutableArray<ISyntax> ChildList { get; }
    public ISyntaxNode? Parent { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.LiteralExpressionNode;
}
