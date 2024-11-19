using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed class ForStatementNode : ICodeBlockOwner
{
    public ForStatementNode(
        KeywordToken keywordToken,
        OpenParenthesisToken openParenthesisToken,
        ImmutableArray<ISyntax> initializationSyntaxList,
        StatementDelimiterToken initializationStatementDelimiterToken,
        IExpressionNode conditionExpressionNode,
        StatementDelimiterToken conditionStatementDelimiterToken,
        IExpressionNode updationExpressionNode,
        CloseParenthesisToken closeParenthesisToken,
        CodeBlockNode? codeBlockNode)
    {
        KeywordToken = keywordToken;
        OpenParenthesisToken = openParenthesisToken;
        InitializationSyntaxList = initializationSyntaxList;
        InitializationStatementDelimiterToken = initializationStatementDelimiterToken;
        ConditionExpressionNode = conditionExpressionNode;
        ConditionStatementDelimiterToken = conditionStatementDelimiterToken;
        UpdationExpressionNode = updationExpressionNode;
        CloseParenthesisToken = closeParenthesisToken;
        CodeBlockNode = codeBlockNode;
    }

	private ISyntax[] _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

    public KeywordToken KeywordToken { get; }
    public OpenParenthesisToken OpenParenthesisToken { get; }
    public ImmutableArray<ISyntax> InitializationSyntaxList { get; }
    public StatementDelimiterToken InitializationStatementDelimiterToken { get; }
    public IExpressionNode ConditionExpressionNode { get; }
    public StatementDelimiterToken ConditionStatementDelimiterToken { get; }
    public IExpressionNode UpdationExpressionNode { get; }
    public CloseParenthesisToken CloseParenthesisToken { get; }
    public OpenBraceToken OpenBraceToken { get; private set; }
    public CodeBlockNode? CodeBlockNode { get; private set; }

	// (2024-11-08)
	public CloseBraceToken CloseBraceToken { get; private set; }
	public StatementDelimiterToken StatementDelimiterToken { get; private set; }
	public bool IsSingleStatementBody => StatementDelimiterToken.ConstructorWasInvoked;

	

	public ScopeDirectionKind ScopeDirectionKind => ScopeDirectionKind.Down;

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.ForStatementNode;
    
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
    
    public int GetStartInclusiveIndex()
    {
    }
    
    public int GetEndExclusiveIndex()
    {
    }
    
    public ISyntax[] GetChildList()
    {
    	if (!_childListIsDirty)
    		return _childList;
    	
    	// KeywordToken, OpenParenthesisToken, InitializationSyntaxList.Length, InitializationStatementDelimiterToken,
        // ConditionExpressionNode, ConditionStatementDelimiterToken, UpdationExpressionNode, CloseParenthesisToken,
        var childCount =
        	1 +                               // KeywordToken,
        	1 +                               // OpenParenthesisToken,
        	InitializationSyntaxList.Length + // InitializationSyntaxList.Length
        	1 +                               // InitializationStatementDelimiterToken,
        	1 +                               // ConditionExpressionNode,
        	1 +                               // ConditionStatementDelimiterToken,
        	1 +                               // UpdationExpressionNode,
        	1;                                // CloseParenthesisToken,
        
        if (OpenBraceToken.ConstructorWasInvoked)
            childCount++;
        if (CodeBlockNode is not null)
            childCount++;
            
        var childList = new ISyntax[childCount];
		var i = 0;

		childList[i++] = KeywordToken;
		childList[i++] = OpenParenthesisToken;
		foreach (var item in InitializationSyntaxList)
		{
			childList[i++] = item;
		}
		childList[i++] = InitializationStatementDelimiterToken;
        childList[i++] = ConditionExpressionNode;
		childList[i++] = ConditionStatementDelimiterToken;
		childList[i++] = UpdationExpressionNode;
		childList[i++] = CloseParenthesisToken;
		if (OpenBraceToken.ConstructorWasInvoked)
            childList[i++] = OpenBraceToken;
        if (CodeBlockNode is not null)
            childList[i++] = CodeBlockNode;
            
        _childList = childList;
        
    	_childListIsDirty = false;
    	return _childList;
    }
}
