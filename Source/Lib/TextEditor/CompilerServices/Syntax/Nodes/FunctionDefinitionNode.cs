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
        IdentifierToken functionIdentifierToken,
        GenericArgumentsListingNode? genericArgumentsListingNode,
        FunctionArgumentsListingNode functionArgumentsListingNode,
        CodeBlockNode? codeBlockNode,
        ConstraintNode? constraintNode)
    {
        AccessModifierKind = accessModifierKind;
        ReturnTypeClauseNode = returnTypeClauseNode;
        FunctionIdentifierToken = functionIdentifierToken;
        GenericArgumentsListingNode = genericArgumentsListingNode;
        FunctionArgumentsListingNode = functionArgumentsListingNode;
        CodeBlockNode = codeBlockNode;
        ConstraintNode = constraintNode;
    }

	private ISyntax[] _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

    public AccessModifierKind AccessModifierKind { get; }
    public TypeClauseNode ReturnTypeClauseNode { get; }
    public IdentifierToken FunctionIdentifierToken { get; }
    public GenericArgumentsListingNode? GenericArgumentsListingNode { get; }
    public FunctionArgumentsListingNode FunctionArgumentsListingNode { get; }
    public ConstraintNode? ConstraintNode { get; private set; }
    public OpenBraceToken OpenBraceToken { get; private set; }
    public CodeBlockNode? CodeBlockNode { get; private set; }

	public ScopeDirectionKind ScopeDirectionKind => ScopeDirectionKind.Down;

    public ISyntaxNode? Parent { get; }

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
    
    public ICodeBlockOwner SetCodeBlockNode(OpenBraceToken openBraceToken, CodeBlockNode codeBlockNode)
    {
    	OpenBraceToken = openBraceToken;
    	CodeBlockNode = codeBlockNode;
    	
    	_childListIsDirty = true;
    	return this;
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
		childList[i++] = FunctionIdentifierToken;
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
