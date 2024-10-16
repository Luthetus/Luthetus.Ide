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
    	var childrenList = new List<ISyntax>
        {
            KeywordToken,
	        OpenParenthesisToken,
	    };
	    
	    childrenList.AddRange(InitializationSyntaxList);
	    
	    childrenList.AddRange(new ISyntax[] 
	    {
	        InitializationStatementDelimiterToken,
	        ConditionExpressionNode,
	        ConditionStatementDelimiterToken,
	        UpdationExpressionNode,
	        CloseParenthesisToken,
	    });

        if (OpenBraceToken.ConstructorWasInvoked)
            childrenList.Add(OpenBraceToken);
        
        if (CodeBlockNode is not null)
            childrenList.Add(CodeBlockNode);

        ChildList = childrenList.ToImmutableArray();
    }
}
