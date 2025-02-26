using Luthetus.TextEditor.RazorLib.CompilerServices.Utility;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed class ConstructorDefinitionNode : ICodeBlockOwner
{
    public ConstructorDefinitionNode(
        TypeClauseNode returnTypeClauseNode,
        SyntaxToken functionIdentifier,
        GenericArgumentsListingNode? genericArgumentsListingNode,
        FunctionArgumentsListingNode functionArgumentsListingNode,
        CodeBlockNode? codeBlockNode,
        ConstraintNode? constraintNode)
    {
        ReturnTypeClauseNode = returnTypeClauseNode;
        FunctionIdentifier = functionIdentifier;
        GenericArgumentsListingNode = genericArgumentsListingNode;
        FunctionArgumentsListingNode = functionArgumentsListingNode;
        CodeBlockNode = codeBlockNode;
        ConstraintNode = constraintNode;
    }

	private IReadOnlyList<ISyntax> _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

    public TypeClauseNode ReturnTypeClauseNode { get; }
    public SyntaxToken FunctionIdentifier { get; }
    public GenericArgumentsListingNode? GenericArgumentsListingNode { get; }
    public FunctionArgumentsListingNode FunctionArgumentsListingNode { get; }
    public ConstraintNode? ConstraintNode { get; }
    
    // ICodeBlockOwner properties.
    public ScopeDirectionKind ScopeDirectionKind => ScopeDirectionKind.Down;
	public TextEditorTextSpan? OpenCodeBlockTextSpan { get; set; }
	public CodeBlockNode? CodeBlockNode { get; private set;  }
	public TextEditorTextSpan? CloseCodeBlockTextSpan { get; set; }
	public int? ScopeIndexKey { get; set; }
    
    /// <summary>
    /// public MyConstructor(string firstName)
    /// 	: base(firstName)
    /// {
    /// }
    ///
    /// This stores the indices of tokens that deliminate the parameters to the 'base()' invocation.
    /// The reason for this is the invocation needs to have 'string firstName' in scope.
    /// But, 'string firstName' doesn't come into scope until the '{' token.
    /// 
    /// So, remember where the parameters to the 'base()' invocation were,
    /// then later when 'string firstName' is in scope, parse the parameters.
    /// </summary>
    public (int OpenParenthesisIndex,  int CloseParenthesisIndex)? OtherConstructorInvocation { get; set; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.ConstructorDefinitionNode;
    
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
    
    public IReadOnlyList<ISyntax> GetChildList()
    {
    	if (!_childListIsDirty)
    		return _childList;
    	
    	// ReturnTypeClauseNode, FunctionIdentifier, ...FunctionArgumentsListingNode,
        var childCount = 3;
        if (GenericArgumentsListingNode is not null)
            childCount++;
        if (CodeBlockNode is not null)
            childCount++;
        if (ConstraintNode is not null)
            childCount++;
            
        var childList = new ISyntax[childCount];
		var i = 0;

		childList[i++] = ReturnTypeClauseNode;
		childList[i++] = FunctionIdentifier;
		if (GenericArgumentsListingNode is not null)
			childList[i++] = GenericArgumentsListingNode;
        childList[i++] = FunctionArgumentsListingNode;
        if (CodeBlockNode is not null)
            childList[i++] = CodeBlockNode;
        if (ConstraintNode is not null)
            childList[i++] = ConstraintNode;
            
        _childList = childList;
        
    	_childListIsDirty = false;
    	return _childList;
    }
}
