using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Utility;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed class ForeachStatementNode : ICodeBlockOwner
{
    public ForeachStatementNode(
        KeywordToken foreachKeywordToken,
        OpenParenthesisToken openParenthesisToken,
        VariableDeclarationNode variableDeclarationNode,
        KeywordToken inKeywordToken,
        IExpressionNode expressionNode,
        CloseParenthesisToken closeParenthesisToken,
        CodeBlockNode? codeBlockNode)
    {
        ForeachKeywordToken = foreachKeywordToken;
        OpenParenthesisToken = openParenthesisToken;
        VariableDeclarationNode = variableDeclarationNode;
        InKeywordToken = inKeywordToken;
        ExpressionNode = expressionNode;
        CloseParenthesisToken = closeParenthesisToken;
        CodeBlockNode = codeBlockNode;
    }

	private ISyntax[] _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

    public KeywordToken ForeachKeywordToken { get; }
    public OpenParenthesisToken OpenParenthesisToken { get; }
    public VariableDeclarationNode VariableDeclarationNode { get; }
    public KeywordToken InKeywordToken { get; }
    public IExpressionNode ExpressionNode { get; }
    public CloseParenthesisToken CloseParenthesisToken { get; }
    public OpenBraceToken OpenBraceToken { get; private set; }
    public CodeBlockNode? CodeBlockNode { get; private set; }

	// (2024-11-08)
	public CloseBraceToken CloseBraceToken { get; private set; }
	public StatementDelimiterToken StatementDelimiterToken { get; private set; }
	public bool IsSingleStatementBody => StatementDelimiterToken.ConstructorWasInvoked;

	public ScopeDirectionKind ScopeDirectionKind => ScopeDirectionKind.Down;

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.ForeachStatementNode;
    
    public TypeClauseNode? GetReturnTypeClauseNode()
    {
    	return null;
    }
    
    // (2024-11-08)
	public ICodeBlockOwner SetOpenBraceToken(OpenBraceToken openBraceToken, DiagnosticBag diagnosticBag, TokenWalker tokenWalker)
	{
		if (StatementDelimiterToken.ConstructorWasInvoked)
			ICodeBlockOwner.ThrowMultipleScopeDelimiterException(diagnosticBag, tokenWalker);
	
		OpenBraceToken = openBraceToken;
    	
    	_childListIsDirty = true;
    	return this;
	}
	public ICodeBlockOwner SetCloseBraceToken(CloseBraceToken closeBraceToken, DiagnosticBag diagnosticBag, TokenWalker tokenWalker)
	{
		if (StatementDelimiterToken.ConstructorWasInvoked)
			ICodeBlockOwner.ThrowMultipleScopeDelimiterException(diagnosticBag, tokenWalker);
	
		CloseBraceToken = closeBraceToken;
    	
    	_childListIsDirty = true;
    	return this;
	}
	public ICodeBlockOwner SetStatementDelimiterToken(StatementDelimiterToken statementDelimiterToken, DiagnosticBag diagnosticBag, TokenWalker tokenWalker)
	{
		if (OpenBraceToken.ConstructorWasInvoked || CloseBraceToken.ConstructorWasInvoked)
			ICodeBlockOwner.ThrowMultipleScopeDelimiterException(diagnosticBag, tokenWalker);
	
		StatementDelimiterToken = statementDelimiterToken;
    	
    	_childListIsDirty = true;
    	return this;
	}
	public ICodeBlockOwner SetCodeBlockNode(CodeBlockNode codeBlockNode, DiagnosticBag diagnosticBag, TokenWalker tokenWalker)
	{
		if (CodeBlockNode is not null)
			ICodeBlockOwner.ThrowAlreadyAssignedCodeBlockNodeException(diagnosticBag, tokenWalker);
	
		CodeBlockNode = codeBlockNode;
    	
    	_childListIsDirty = true;
    	return this;
	}
    
    public ISyntax[] GetChildList()
    {
    	if (!_childListIsDirty)
    		return _childList;
    	
    	var childCount = 6; // ForeachKeywordToken, OpenParenthesisToken, VariableDeclarationNode, InKeywordToken, ExpressionNode, CloseParenthesisToken,
        if (OpenParenthesisToken.ConstructorWasInvoked)
            childCount++;
        if (CodeBlockNode is not null)
            childCount++;
            
        var childList = new ISyntax[childCount];
		var i = 0;

		childList[i++] = ForeachKeywordToken;
		childList[i++] = OpenParenthesisToken;
		childList[i++] = VariableDeclarationNode;
		childList[i++] = InKeywordToken;
		childList[i++] = ExpressionNode;
		childList[i++] = CloseParenthesisToken;
		if (OpenParenthesisToken.ConstructorWasInvoked)
            childList[i++] = OpenParenthesisToken;
        if (CodeBlockNode is not null)
            childList[i++] = CodeBlockNode;
            
        _childList = childList;
        
    	_childListIsDirty = false;
    	return _childList;
    }
}
