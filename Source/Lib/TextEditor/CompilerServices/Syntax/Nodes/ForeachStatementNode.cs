using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Utility;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

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

	// ICodeBlockOwner properties.
	public ScopeDirectionKind ScopeDirectionKind => ScopeDirectionKind.Down;
	public TextEditorTextSpan? OpenCodeBlockTextSpan { get; set; }
	public CodeBlockNode? CodeBlockNode { get; private set; }
	public TextEditorTextSpan? CloseCodeBlockTextSpan { get; set; }
	public int? ScopeIndexKey { get; set; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.ForeachStatementNode;
    
    #region ICodeBlockOwner_Methods
    public TypeClauseNode? GetReturnTypeClauseNode()
    {
    	return null;
    }
    
	public ICodeBlockOwner SetOpenCodeBlockTextSpan(TextEditorTextSpan? openCodeBlockTextSpan, DiagnosticBag diagnosticBag, TokenWalker tokenWalker)
	{
		if (OpenCodeBlockTextSpan is not null)
			ICodeBlockOwner.ThrowMultipleScopeDelimiterException(diagnosticBag, tokenWalker);
	
		OpenCodeBlockTextSpan = openCodeBlockTextSpan;
    	
    	_childListIsDirty = true;
    	return this;
	}
	
	public ICodeBlockOwner SetCloseCodeBlockTextSpan(TextEditorTextSpan? closeCodeBlockTextSpan, DiagnosticBag diagnosticBag, TokenWalker tokenWalker)
	{
		if (CloseCodeBlockTextSpan is not null)
			ICodeBlockOwner.ThrowMultipleScopeDelimiterException(diagnosticBag, tokenWalker);
	
		CloseCodeBlockTextSpan = closeCodeBlockTextSpan;
    	
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
	#endregion
    
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
