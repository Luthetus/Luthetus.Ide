using Luthetus.TextEditor.RazorLib.CompilerServices.Utility;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

/// <summary>
/// While parsing expression, it is necessary that there exists a node
/// that indicates a lambda expression is being parsed.
///
/// This type might be more along the lines of a "builder" type.
/// It is meant to be made when starting lambda expression,
/// then the primary expression can be equal to an instae of this type.
///
/// This then directs the parser accordingly until the lambda expression
/// is fully parsed.
///
/// At this point, it is planned that a FunctionDefinitionNode will be
/// made, and a 'MethodGroupExpressionNode' (this type does not yet exist) will be returned as the
/// primary expression.
/// </summary>
public sealed class LambdaExpressionNode : IExpressionNode, ICodeBlockOwner
{
    public LambdaExpressionNode(TypeClauseNode resultTypeClauseNode)
    {
    	ResultTypeClauseNode = resultTypeClauseNode;
    }

	private ISyntax[] _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

    public TypeClauseNode ResultTypeClauseNode { get; }
    
    /// <summary>
    /// () => "Abc";
    ///     Then this property is true;
    ///
    /// () => { return "Abc" };
    ///     Then this property is false;
    /// </summary>
    public bool CodeBlockNodeIsExpression { get; set; } = true;
    public bool HasReadParameters { get; set; }
    public List<IVariableDeclarationNode> VariableDeclarationNodeList { get; } = new();

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.LambdaExpressionNode;
    
	public TypeClauseNode ReturnTypeClauseNode { get; }
	
	// ICodeBlockOwner properties.
	public ScopeDirectionKind ScopeDirectionKind => ScopeDirectionKind.Down;
	public TextEditorTextSpan? OpenCodeBlockTextSpan { get; set; }
	public CodeBlockNode? CodeBlockNode { get; private set; }
	public TextEditorTextSpan? CloseCodeBlockTextSpan { get; set; }
	public int? ScopeIndexKey { get; set; }
	
    #region ICodeBlockOwner_Methods
    public TypeClauseNode? GetReturnTypeClauseNode()
    {
    	return ReturnTypeClauseNode;
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
    
    public void AddVariableDeclarationNode(IVariableDeclarationNode variableDeclarationNode)
    {
    	VariableDeclarationNodeList.Add(variableDeclarationNode);
    	_childListIsDirty = true;
    }
    
    public ISyntax[] GetChildList()
    {
    	if (!_childListIsDirty)
    		return _childList;
    	
    	// ResultTypeClauseNode, VariableDeclarationNodeList.Count
    	var childCount = 
    		1 +                                // ResultTypeClauseNode
    		VariableDeclarationNodeList.Count; // VariableDeclarationNodeList.Count
    	
    	var childList = new ISyntax[childCount];
		var i = 0;

		childList[i++] = ResultTypeClauseNode;
		foreach (var item in VariableDeclarationNodeList)
		{
			childList[i++] = item;
		}
		
		_childList = childList;

    	_childListIsDirty = false;
    	return _childList;
    }
}
