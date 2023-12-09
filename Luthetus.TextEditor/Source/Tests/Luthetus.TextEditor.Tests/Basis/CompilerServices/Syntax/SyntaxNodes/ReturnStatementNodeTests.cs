using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes.Expression;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxNodes;

public sealed record ReturnStatementNodeTests
{
    public ReturnStatementNode(KeywordToken keywordToken, IExpressionNode expressionNode)
    {
        KeywordToken = keywordToken;
        ExpressionNode = expressionNode;

        ChildBag = new ISyntax[]
        {
            KeywordToken,
            ExpressionNode
        }.ToImmutableArray();
    }

    public KeywordToken KeywordToken { get; }
    public IExpressionNode ExpressionNode { get; }

    public ImmutableArray<ISyntax> ChildBag { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.ReturnStatementNode;
}