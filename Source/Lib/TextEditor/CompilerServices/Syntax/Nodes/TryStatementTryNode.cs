using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed class TryStatementTryNode : ICodeBlockOwner
{
	public TryStatementTryNode(
		TryStatementNode? parent,
        KeywordToken keywordToken,
        CodeBlockNode? codeBlockNode)
    {
    	Parent = parent;
        KeywordToken = keywordToken;
        CodeBlockNode = codeBlockNode;
    }

	private ISyntax[] _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

    public KeywordToken KeywordToken { get; }
    public OpenBraceToken OpenBraceToken { get; private set; }
    public CodeBlockNode? CodeBlockNode { get; private set; }

	// (2024-11-08)
	public CloseBraceToken CloseBraceToken { get; private set; }
	public StatementDelimiterToken StatementDelimiterToken { get; private set; }
	public bool IsSingleStatementBody => StatementDelimiterToken.ConstructorWasInvoked;

	public ScopeDirectionKind ScopeDirectionKind => ScopeDirectionKind.Down;
	public ISyntaxNode? Parent { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.TryStatementTryNode;
    
    public TypeClauseNode? GetReturnTypeClauseNode()
    {
    	return null;
    }
    
    public void OnBoundScopeCreatedAndSetAsCurrent(IParserModel parserModel)
    {
    	// Do nothing.
    	return;
    }
    
    // (2024-11-08)
	public ICodeBlockOwner SetOpenBraceToken(OpenBraceToken openBraceToken, IParserModel parserModel)
	{
		if (StatementDelimiterToken.ConstructorWasInvoked)
			ICodeBlockOwner.ThrowMultipleScopeDelimiterException(parserModel);
	
		OpenBraceToken = openBraceToken;
    	
    	_childListIsDirty = true;
    	return this;
	}
	public ICodeBlockOwner SetCloseBraceToken(CloseBraceToken closeBraceToken, IParserModel parserModel)
	{
		if (StatementDelimiterToken.ConstructorWasInvoked)
			ICodeBlockOwner.ThrowMultipleScopeDelimiterException(parserModel);
	
		CloseBraceToken = closeBraceToken;
    	
    	_childListIsDirty = true;
    	return this;
	}
	public ICodeBlockOwner SetStatementDelimiterToken(StatementDelimiterToken statementDelimiterToken, IParserModel parserModel)
	{
		if (OpenBraceToken.ConstructorWasInvoked || CloseBraceToken.ConstructorWasInvoked)
			ICodeBlockOwner.ThrowMultipleScopeDelimiterException(parserModel);
	
		StatementDelimiterToken = statementDelimiterToken;
    	
    	_childListIsDirty = true;
    	return this;
	}
	public ICodeBlockOwner SetCodeBlockNode(CodeBlockNode codeBlockNode, IParserModel parserModel)
	{
		if (CodeBlockNode is not null)
			ICodeBlockOwner.ThrowAlreadyAssignedCodeBlockNodeException(parserModel);
	
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
        if (OpenBraceToken.ConstructorWasInvoked)
            childCount++;
        if (CodeBlockNode is not null)
            childCount++;
            
        var childList = new ISyntax[childCount];
		var i = 0;
		
		if (KeywordToken.ConstructorWasInvoked)
            childList[i++] = KeywordToken;
        if (OpenBraceToken.ConstructorWasInvoked)
            childList[i++] = OpenBraceToken;
        if (CodeBlockNode is not null)
        	childList[i++] = CodeBlockNode;
            
        _childList = childList;
        
    	_childListIsDirty = false;
    	return _childList;
    }
}
