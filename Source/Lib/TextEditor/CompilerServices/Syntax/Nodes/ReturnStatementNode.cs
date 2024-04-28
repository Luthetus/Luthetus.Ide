using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Expression;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed record ReturnStatementNode : ISyntaxNode
{
    public ReturnStatementNode(KeywordToken keywordToken, IExpressionNode expressionNode)
    {
        KeywordToken = keywordToken;
        ExpressionNode = expressionNode;

        ChildList = new ISyntax[]
        {
            KeywordToken,
            ExpressionNode
        }.ToImmutableArray();
    }

    public KeywordToken KeywordToken { get; }
    public IExpressionNode ExpressionNode { get; }

    public ImmutableArray<ISyntax> ChildList { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.ReturnStatementNode;
}