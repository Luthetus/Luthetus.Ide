using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed class DoWhileStatementNode : ICodeBlockOwner
{
    public DoWhileStatementNode(
        KeywordToken doKeywordToken,
        OpenBraceToken openBraceToken,
        CodeBlockNode? codeBlockNode,
        KeywordToken whileKeywordToken,
        OpenParenthesisToken openParenthesisToken,
        IExpressionNode? expressionNode,
        CloseParenthesisToken closeParenthesisToken)
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
    public OpenBraceToken OpenBraceToken { get; private set; }
    public CodeBlockNode? CodeBlockNode { get; private set; }
    public KeywordToken WhileKeywordToken { get; private set; }
    public OpenParenthesisToken OpenParenthesisToken { get; private set; }
    public IExpressionNode? ExpressionNode { get; private set; }
    public CloseParenthesisToken CloseParenthesisToken { get; private set; }

	public ScopeDirectionKind ScopeDirectionKind => ScopeDirectionKind.Down;

    public ISyntax[] ChildList { get; private set; }
    public ISyntaxNode? Parent { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.DoWhileStatementNode;
    
    public TypeClauseNode? GetReturnTypeClauseNode()
    {
    	return null;
    }
    
    public ICodeBlockOwner SetCodeBlockNode(OpenBraceToken openBraceToken, CodeBlockNode codeBlockNode)
    {
    	OpenBraceToken = openBraceToken;
    	CodeBlockNode = codeBlockNode;
    	SetChildList();
    	return this;
    }
    
    public void OnBoundScopeCreatedAndSetAsCurrent(IParserModel parserModel)
    {
    	// Do nothing.
    	return;
    }
    
    public void SetChildList()
    {
    	var childCount = 1; // DoKeywordToken,
        if (OpenBraceToken.ConstructorWasInvoked)
            childCount++;
        if (CodeBlockNode is not null)
            childCount++;
        if (WhileKeywordToken.ConstructorWasInvoked)
            childCount++;
        if (OpenParenthesisToken.ConstructorWasInvoked)
            childCount++;
        if (ExpressionNode is not null)
            childCount++;
        if (CloseParenthesisToken.ConstructorWasInvoked)
            childCount++;
            
        var childList = new ISyntax[childCount];
		var i = 0;

		childList[i++] = DoKeywordToken;
		if (OpenBraceToken.ConstructorWasInvoked)
            childList[i++] = OpenBraceToken;
        if (CodeBlockNode is not null)
            childList[i++] = CodeBlockNode;
        if (WhileKeywordToken.ConstructorWasInvoked)
            childList[i++] = WhileKeywordToken;
        if (OpenParenthesisToken.ConstructorWasInvoked)
            childList[i++] = OpenParenthesisToken;
        if (ExpressionNode is not null)
            childList[i++] = ExpressionNode;
        if (CloseParenthesisToken.ConstructorWasInvoked)
            childList[i++] = CloseParenthesisToken;
            
        ChildList = childList;
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
