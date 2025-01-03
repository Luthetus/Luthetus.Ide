using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Utility;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed class TryStatementCatchNode : ICodeBlockOwner
{
	public TryStatementCatchNode(
		TryStatementNode? parent,
        KeywordToken keywordToken,
        OpenParenthesisToken openParenthesisToken,
        CloseParenthesisToken closeParenthesisToken,
        CodeBlockNode? codeBlockNode)
    {
    	Parent = parent;
        KeywordToken = keywordToken;
        CodeBlockNode = codeBlockNode;
        OpenParenthesisToken = openParenthesisToken;
        CloseParenthesisToken = closeParenthesisToken;
        CodeBlockNode = codeBlockNode;
    }

	private ISyntax[] _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;
	
	public KeywordToken KeywordToken { get; }
    public OpenParenthesisToken OpenParenthesisToken { get; }
    public VariableDeclarationNode? VariableDeclarationNode { get; private set; }
    public CloseParenthesisToken CloseParenthesisToken { get; }
    public OpenBraceToken OpenBraceToken { get; private set; }
    public CodeBlockNode? CodeBlockNode { get; private set; }

	// (2024-11-08)
	public CloseBraceToken CloseBraceToken { get; private set; }
	public StatementDelimiterToken StatementDelimiterToken { get; private set; }
	public bool IsSingleStatementBody => StatementDelimiterToken.ConstructorWasInvoked;

	public ScopeDirectionKind ScopeDirectionKind => ScopeDirectionKind.Down;
	public ISyntaxNode? Parent { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.TryStatementCatchNode;
    
    public TryStatementCatchNode SetVariableDeclarationNode(VariableDeclarationNode variableDeclarationNode)
    {
    	VariableDeclarationNode = variableDeclarationNode;
    	_childListIsDirty = true;
    	return this;
    }
    
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
    	
        var childCount = 0;
        if (KeywordToken.ConstructorWasInvoked)
            childCount++;
        if (CodeBlockNode is not null)
            childCount++;
        if (OpenParenthesisToken.ConstructorWasInvoked)
            childCount++;
        if (CloseParenthesisToken.ConstructorWasInvoked)
            childCount++;
        if (CodeBlockNode is not null)
            childCount++;
            
        var childList = new ISyntax[childCount];
		var i = 0;
		
		if (KeywordToken.ConstructorWasInvoked)
            childList[i++] = KeywordToken;
        if (CodeBlockNode is not null)
            childList[i++] = CodeBlockNode;
        if (OpenParenthesisToken.ConstructorWasInvoked)
            childList[i++] = OpenParenthesisToken;
        if (CloseParenthesisToken.ConstructorWasInvoked)
            childList[i++] = CloseParenthesisToken;
        if (CodeBlockNode is not null)
            childList[i++] = CodeBlockNode;
            
        _childList = childList;
        
    	_childListIsDirty = false;
    	return _childList;
    }
}
