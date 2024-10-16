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

        SetChildList();
    }

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

	public ScopeDirectionKind ScopeDirectionKind => ScopeDirectionKind.Down;

    public ISyntax[] ChildList { get; private set; }
    public ISyntaxNode? Parent { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.ForStatementNode;
    
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
            
        ChildList = childList;
    }
}
