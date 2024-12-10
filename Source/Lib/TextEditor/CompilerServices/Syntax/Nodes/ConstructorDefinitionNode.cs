using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed class ConstructorDefinitionNode : ICodeBlockOwner
{
    public ConstructorDefinitionNode(
        TypeClauseNode returnTypeClauseNode,
        NameClauseToken nameToken,
        GenericArgumentsListingNode? genericArgumentsListingNode,
        FunctionArgumentsListingNode functionArgumentsListingNode,
        CodeBlockNode? codeBlockNode,
        ConstraintNode? constraintNode)
    {
        ReturnTypeClauseNode = returnTypeClauseNode;
        NameToken = nameToken;
        GenericArgumentsListingNode = genericArgumentsListingNode;
        FunctionArgumentsListingNode = functionArgumentsListingNode;
        CodeBlockNode = codeBlockNode;
        ConstraintNode = constraintNode;
    }

	private ISyntax[] _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

    public TypeClauseNode ReturnTypeClauseNode { get; }
    public NameClauseToken NameToken { get; }
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
    
    public void OnBoundScopeCreatedAndSetAsCurrent(IParserModel parserModel)
    {
		foreach (var argument in FunctionArgumentsListingNode.FunctionArgumentEntryNodeList)
		{
			parserModel.Binder.BindVariableDeclarationNode(argument.VariableDeclarationNode, parserModel);
		
			/*if (argument.IsOptional)
				parserModel.Binder.BindFunctionOptionalArgument(argument, parserModel);
			else
				parserModel.Binder.BindVariableDeclarationNode(argument.VariableDeclarationNode, parserModel);*/
				
			
		}
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
		childList[i++] = NameToken;
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
