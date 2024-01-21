using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes.Expression;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;

public sealed record ConstructorInvocationExpressionNode : IExpressionNode
{
    public ConstructorInvocationExpressionNode(
        KeywordToken newKeywordToken,
        TypeClauseNode typeClauseNode)
    {
        NewKeywordToken = newKeywordToken;
        ResultTypeClauseNode = typeClauseNode;

        var children = new List<ISyntax>
        {
            NewKeywordToken,
            ResultTypeClauseNode,
        };

        ChildList = children.ToImmutableArray();
    }

    public KeywordToken NewKeywordToken { get; }
    public TypeClauseNode ResultTypeClauseNode { get; }

    public ImmutableArray<ISyntax> ChildList { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.ConstructorInvocationExpressionNode;
}
