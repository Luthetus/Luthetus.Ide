using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Expression;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed record IfStatementNode : ISyntaxNode
{
    public IfStatementNode(
        KeywordToken keywordToken,
        IExpressionNode expressionNode,
        CodeBlockNode? ifStatementBodyCodeBlockNode)
    {
        KeywordToken = keywordToken;
        ExpressionNode = expressionNode;
        IfStatementBodyCodeBlockNode = ifStatementBodyCodeBlockNode;

        var childrenList = new List<ISyntax>
        {
            KeywordToken,
            ExpressionNode,
        };

        if (IfStatementBodyCodeBlockNode is not null)
            childrenList.Add(IfStatementBodyCodeBlockNode);

        ChildList = childrenList.ToImmutableArray();
    }

    public KeywordToken KeywordToken { get; }
    public IExpressionNode ExpressionNode { get; }
    public CodeBlockNode? IfStatementBodyCodeBlockNode { get; }

    public ImmutableArray<ISyntax> ChildList { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.IfStatementNode;
}