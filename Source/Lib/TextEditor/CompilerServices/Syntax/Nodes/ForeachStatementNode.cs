using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed record ForeachStatementNode : ICodeBlockOwner
{
    public ForeachStatementNode(
        KeywordToken foreachKeywordToken,
        OpenParenthesisToken openParenthesisToken,
        IdentifierToken identifierToken,
        KeywordToken inKeywordToken,
        IExpressionNode expressionNode,
        CloseParenthesisToken closeParenthesisToken,
        CodeBlockNode? codeBlockNode)
    {
        ForeachKeywordToken = foreachKeywordToken;
        OpenParenthesisToken = openParenthesisToken;
        IdentifierToken = identifierToken;
        InKeywordToken = inKeywordToken;
        ExpressionNode = expressionNode;
        CloseParenthesisToken = closeParenthesisToken;
        CodeBlockNode = codeBlockNode;

        SetChildList();
    }

    public KeywordToken ForeachKeywordToken { get; }
    public OpenParenthesisToken OpenParenthesisToken { get; }
    public IdentifierToken IdentifierToken { get; }
    public KeywordToken InKeywordToken { get; }
    public IExpressionNode ExpressionNode { get; }
    public CloseParenthesisToken CloseParenthesisToken { get; }
    public CodeBlockNode? CodeBlockNode { get; private set; }
    public OpenBraceToken? OpenBraceToken { get; private set; }

	public ScopeDirectionKind ScopeDirectionKind => ScopeDirectionKind.Down;

    public ImmutableArray<ISyntax> ChildList { get; private set; }
    public ISyntaxNode? Parent { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.ForeachStatementNode;
    
    public TypeClauseNode? GetReturnTypeClauseNode()
    {
    	return null;
    }
    
    public ICodeBlockOwner WithCodeBlockNode(OpenBraceToken openBraceToken, CodeBlockNode codeBlockNode)
    {
    	OpenBraceToken = openBraceToken;
    	CodeBlockNode = codeBlockNode;
    	return this;
    }
    
    public void SetChildList()
    {
    	var childrenList = new List<ISyntax>
        {
            ForeachKeywordToken,
            OpenParenthesisToken,
            IdentifierToken,
            InKeywordToken,
            ExpressionNode,
            CloseParenthesisToken,
        };

		if (OpenParenthesisToken is not null)
            childrenList.Add(OpenParenthesisToken);

        if (CodeBlockNode is not null)
            childrenList.Add(CodeBlockNode);

        ChildList = childrenList.ToImmutableArray();
    }
}
