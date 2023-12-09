using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes.Expression;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxNodes;

public sealed record IfStatementNodeTests
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

        ChildBag = childrenList.ToImmutableArray();
    }

    public KeywordToken KeywordToken { get; }
    public IExpressionNode ExpressionNode { get; }
    public CodeBlockNode? IfStatementBodyCodeBlockNode { get; }

    public ImmutableArray<ISyntax> ChildBag { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.IfStatementNode;
}