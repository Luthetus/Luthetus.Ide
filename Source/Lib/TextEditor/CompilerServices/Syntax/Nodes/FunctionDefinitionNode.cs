using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

/// <summary>
/// TODO: Track the open and close braces for the function body.
/// </summary>
public sealed class FunctionDefinitionNode : ICodeBlockOwner
{
    public FunctionDefinitionNode(
        AccessModifierKind accessModifierKind,
        TypeClauseNode returnTypeClauseNode,
        NameClauseToken nameToken,
        GenericParametersListingNode? genericArgumentsListingNode,
        FunctionArgumentsListingNode functionArgumentsListingNode,
        CodeBlockNode? codeBlockNode,
        ConstraintNode? constraintNode)
    {
        AccessModifierKind = accessModifierKind;
        ReturnTypeClauseNode = returnTypeClauseNode;
        NameToken = nameToken;
        GenericArgumentsListingNode = genericArgumentsListingNode;
        FunctionArgumentsListingNode = functionArgumentsListingNode;
        CodeBlockNode = codeBlockNode;
        ConstraintNode = constraintNode;
    }

	private ISyntax[] _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

    public AccessModifierKind AccessModifierKind { get; }
    public TypeClauseNode ReturnTypeClauseNode { get; }
    public NameClauseToken NameToken { get; }
    public GenericParametersListingNode? GenericArgumentsListingNode { get; }
    public FunctionArgumentsListingNode FunctionArgumentsListingNode { get; }
    public ConstraintNode? ConstraintNode { get; private set; }
    public OpenBraceToken OpenBraceToken { get; private set; }
    public CodeBlockNode? CodeBlockNode { get; private set; }

	// (2024-11-08)
	public CloseBraceToken CloseBraceToken { get; private set; }
	public StatementDelimiterToken StatementDelimiterToken { get; private set; }
	public bool IsSingleStatementBody => StatementDelimiterToken.ConstructorWasInvoked;

	public ScopeDirectionKind ScopeDirectionKind => ScopeDirectionKind.Down;

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.FunctionDefinitionNode;
    
    public ICodeBlockOwner SetConstraintNode(ConstraintNode constraintNode)
    {
    	ConstraintNode = constraintNode;
    	
    	_childListIsDirty = true;
    	return this;
    }
    
    public TypeClauseNode? GetReturnTypeClauseNode()
    {
    	return ReturnTypeClauseNode;
    }
    
    public ICodeBlockOwner SetExpressionBody(CodeBlockNode codeBlockNode)
    {
    	CodeBlockNode = codeBlockNode;
    	
    	_childListIsDirty = true;
    	return this;
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
    	
    	var childCount = 3; // ReturnTypeClauseNode, FunctionIdentifierToken, ...FunctionArgumentsListingNode,
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
