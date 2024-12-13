using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Utility;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed class ConstructorDefinitionNode : ICodeBlockOwner
{
    public ConstructorDefinitionNode(
        TypeClauseNode returnTypeClauseNode,
        IdentifierToken functionIdentifier,
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

	private ISyntax[] _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

    public TypeClauseNode ReturnTypeClauseNode { get; }
    public IdentifierToken FunctionIdentifier { get; }
    public GenericArgumentsListingNode? GenericArgumentsListingNode { get; }
    public FunctionArgumentsListingNode FunctionArgumentsListingNode { get; }
    public CodeBlockNode? CodeBlockNode { get; private set; }
    public ConstraintNode? ConstraintNode { get; }
    
    // (2024-11-08)
    public OpenBraceToken OpenBraceToken { get; private set; }
	public CloseBraceToken CloseBraceToken { get; private set; }
	public StatementDelimiterToken StatementDelimiterToken { get; private set; }
	public bool IsSingleStatementBody => StatementDelimiterToken.ConstructorWasInvoked;
    
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

	public ScopeDirectionKind ScopeDirectionKind => ScopeDirectionKind.Down;

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.ConstructorDefinitionNode;
    
    public TypeClauseNode? GetReturnTypeClauseNode()
    {
    	return ReturnTypeClauseNode;
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
