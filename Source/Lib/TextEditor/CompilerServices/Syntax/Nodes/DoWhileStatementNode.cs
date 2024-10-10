using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed record DoWhileStatementNode : ICodeBlockOwner
{
    public DoWhileStatementNode(
        KeywordToken doKeywordToken,
        OpenBraceToken? openBraceToken,
        CodeBlockNode? codeBlockNode,
        KeywordToken? whileKeywordToken,
        OpenParenthesisToken? openParenthesisToken,
        IExpressionNode? expressionNode,
        CloseParenthesisToken? closeParenthesisToken)
    {
        DoKeywordToken = doKeywordToken;
        OpenBraceToken = openBraceToken;
        CodeBlockNode = codeBlockNode;
        WhileKeywordToken = whileKeywordToken;
        OpenParenthesisToken = openParenthesisToken;
        ExpressionNode = expressionNode;
        CloseParenthesisToken = closeParenthesisToken;

        SetChildList();
    }

    public KeywordToken DoKeywordToken { get; }
    public OpenBraceToken? OpenBraceToken { get; private set; }
    public CodeBlockNode? CodeBlockNode { get; private set; }
    public KeywordToken? WhileKeywordToken { get; private set; }
    public OpenParenthesisToken? OpenParenthesisToken { get; private set; }
    public IExpressionNode? ExpressionNode { get; private set; }
    public CloseParenthesisToken? CloseParenthesisToken { get; private set; }

	public ScopeDirectionKind ScopeDirectionKind => ScopeDirectionKind.Down;

    public ImmutableArray<ISyntax> ChildList { get; private set; }
    public ISyntaxNode? Parent { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.DoWhileStatementNode;
    
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
    
    public void OnBoundScopeCreatedAndSetAsCurrent(IParserModel parserModel)
    {
    	// Do nothing.
    	return;
    }
    
    public void SetChildList()
    {
    	var childrenList = new List<ISyntax>
        {
            DoKeywordToken,
        };

        if (OpenBraceToken is not null)
            childrenList.Add(OpenBraceToken);
            
        if (CodeBlockNode is not null)
            childrenList.Add(CodeBlockNode);
            
        if (WhileKeywordToken is not null)
            childrenList.Add(WhileKeywordToken);
            
        if (OpenParenthesisToken is not null)
            childrenList.Add(OpenParenthesisToken);
            
        if (ExpressionNode is not null)
            childrenList.Add(ExpressionNode);
            
        if (CloseParenthesisToken is not null)
            childrenList.Add(CloseParenthesisToken);

        ChildList = childrenList.ToImmutableArray();
    }
    
    public void SetWhileProperties(
		KeywordToken whileKeywordToken,
	    OpenParenthesisToken openParenthesisToken,
	    IExpressionNode expressionNode,
	    CloseParenthesisToken closeParenthesisToken)
    {
    	WhileKeywordToken = whileKeywordToken;
    	OpenParenthesisToken = openParenthesisToken;
	    ExpressionNode = expressionNode;
	    CloseParenthesisToken = closeParenthesisToken;
    }
}
