using Luthetus.TextEditor.RazorLib.CompilerServices.Utility;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed class ForStatementNode : ICodeBlockOwner
{
    public ForStatementNode(
        SyntaxToken keywordToken,
        SyntaxToken openParenthesisToken,
        IReadOnlyList<ISyntax> initializationSyntaxList,
        SyntaxToken initializationStatementDelimiterToken,
        IExpressionNode conditionExpressionNode,
        SyntaxToken conditionStatementDelimiterToken,
        IExpressionNode updationExpressionNode,
        SyntaxToken closeParenthesisToken,
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

	private IReadOnlyList<ISyntax> _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

    public SyntaxToken KeywordToken { get; }
    public SyntaxToken OpenParenthesisToken { get; }
    public IReadOnlyList<ISyntax> InitializationSyntaxList { get; }
    public SyntaxToken InitializationStatementDelimiterToken { get; }
    public IExpressionNode ConditionExpressionNode { get; }
    public SyntaxToken ConditionStatementDelimiterToken { get; }
    public IExpressionNode UpdationExpressionNode { get; }
    public SyntaxToken CloseParenthesisToken { get; }

	// ICodeBlockOwner properties.
	public ScopeDirectionKind ScopeDirectionKind => ScopeDirectionKind.Down;
	public TextEditorTextSpan? OpenCodeBlockTextSpan { get; set; }
	public CodeBlockNode? CodeBlockNode { get; private set; }
	public TextEditorTextSpan? CloseCodeBlockTextSpan { get; set; }
	public int? ScopeIndexKey { get; set; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.ForStatementNode;
    
    #region ICodeBlockOwner_Methods
    public TypeClauseNode? GetReturnTypeClauseNode()
    {
    	return null;
    }
    
	public ICodeBlockOwner SetOpenCodeBlockTextSpan(TextEditorTextSpan? openCodeBlockTextSpan, List<TextEditorDiagnostic> diagnosticList, TokenWalker tokenWalker)
	{
		if (OpenCodeBlockTextSpan is not null)
			ICodeBlockOwner.ThrowMultipleScopeDelimiterException(diagnosticList, tokenWalker);
	
		OpenCodeBlockTextSpan = openCodeBlockTextSpan;
    	
    	_childListIsDirty = true;
    	return this;
	}
	
	public ICodeBlockOwner SetCloseCodeBlockTextSpan(TextEditorTextSpan? closeCodeBlockTextSpan, List<TextEditorDiagnostic> diagnosticList, TokenWalker tokenWalker)
	{
		if (CloseCodeBlockTextSpan is not null)
			ICodeBlockOwner.ThrowMultipleScopeDelimiterException(diagnosticList, tokenWalker);
	
		CloseCodeBlockTextSpan = closeCodeBlockTextSpan;
    	
    	_childListIsDirty = true;
    	return this;
	}
	
	public ICodeBlockOwner SetCodeBlockNode(CodeBlockNode codeBlockNode, List<TextEditorDiagnostic> diagnosticList, TokenWalker tokenWalker)
	{
		if (CodeBlockNode is not null)
			ICodeBlockOwner.ThrowAlreadyAssignedCodeBlockNodeException(diagnosticList, tokenWalker);
	
		CodeBlockNode = codeBlockNode;
    	
    	_childListIsDirty = true;
    	return this;
	}
	#endregion
    
    public IReadOnlyList<ISyntax> GetChildList()
    {
    	if (!_childListIsDirty)
    		return _childList;
    	
    	// KeywordToken, OpenParenthesisToken, InitializationSyntaxList.Length, InitializationStatementDelimiterToken,
        // ConditionExpressionNode, ConditionStatementDelimiterToken, UpdationExpressionNode, CloseParenthesisToken,
        var childCount =
        	1 +                               // KeywordToken,
        	1 +                               // OpenParenthesisToken,
        	InitializationSyntaxList.Count +  // InitializationSyntaxList.Length
        	1 +                               // InitializationStatementDelimiterToken,
        	1 +                               // ConditionExpressionNode,
        	1 +                               // ConditionStatementDelimiterToken,
        	1 +                               // UpdationExpressionNode,
        	1;                                // CloseParenthesisToken,
        
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
		if (CodeBlockNode is not null)
            childList[i++] = CodeBlockNode;
            
        _childList = childList;
        
    	_childListIsDirty = false;
    	return _childList;
    }
}
