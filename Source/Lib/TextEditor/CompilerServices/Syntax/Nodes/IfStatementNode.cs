using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed record IfStatementNode : ICodeBlockOwner
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

	public ScopeDirectionKind ScopeDirectionKind => ScopeDirectionKind.Down;

    public ImmutableArray<ISyntax> ChildList { get; }
    public ISyntaxNode? Parent { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.IfStatementNode;
}