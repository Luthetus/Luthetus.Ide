using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

/// <summary>
/// <see cref="TypeDefinitionNode"/> is used anywhere a type is defined.
/// </summary>
public sealed class TypeDefinitionNode : ICodeBlockOwner
{
    public TypeDefinitionNode(
        AccessModifierKind accessModifierKind,
        bool hasPartialModifier,
        StorageModifierKind storageModifierKind,
        IdentifierToken typeIdentifier,
        Type? valueType,
        GenericArgumentsListingNode? genericArgumentsListingNode,
        FunctionArgumentsListingNode? primaryConstructorFunctionArgumentsListingNode,
        TypeClauseNode? inheritedTypeClauseNode,
		OpenBraceToken openBraceToken,
        CodeBlockNode? codeBlockNode)
    {
        AccessModifierKind = accessModifierKind;
        HasPartialModifier = hasPartialModifier;
        StorageModifierKind = storageModifierKind;
        TypeIdentifierToken = typeIdentifier;
        ValueType = valueType;
        GenericArgumentsListingNode = genericArgumentsListingNode;
        PrimaryConstructorFunctionArgumentsListingNode = primaryConstructorFunctionArgumentsListingNode;
        InheritedTypeClauseNode = inheritedTypeClauseNode;
        OpenBraceToken = openBraceToken;
        CodeBlockNode = codeBlockNode;
    }

	private ISyntax[] _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

	private TypeClauseNode? _toTypeClauseNodeResult;

    public AccessModifierKind AccessModifierKind { get; }
    public bool HasPartialModifier { get; }
    public StorageModifierKind StorageModifierKind { get; }
    /// <summary>
    /// Given: 'public class Person { /* class definition here */ }'<br/>
    /// Then: 'Person' is the <see cref="TypeIdentifierToken"/><br/>
    /// And: <see cref="GenericArgumentsListingNode"/> would be null
    /// </summary>
    public IdentifierToken TypeIdentifierToken { get; }
    public Type? ValueType { get; }
    /// <summary>
    /// Given: 'public struct Array&lt;T&gt; { /* struct definition here */ }'<br/>
    /// Then: 'Array&lt;T&gt;' is the <see cref="TypeIdentifierToken"/><br/>
    /// And: '&lt;T&gt;' is the <see cref="GenericArgumentsListingNode"/>
    /// </summary>
    public GenericArgumentsListingNode? GenericArgumentsListingNode { get; }
    public FunctionArgumentsListingNode? PrimaryConstructorFunctionArgumentsListingNode { get; private set; }
    /// <summary>
    /// The open brace for the body code block node.
    /// </summary>
    public OpenBraceToken OpenBraceToken { get; private set; }

    /// <summary>
    /// Given:<br/>
    /// public class Person : IPerson { ... }<br/><br/>
    /// Then: 'IPerson' is the <see cref="InheritedTypeClauseNode"/>
    /// </summary>
    public TypeClauseNode? InheritedTypeClauseNode { get; private set; }
    public CodeBlockNode? CodeBlockNode { get; private set; }
    public bool IsInterface => StorageModifierKind == StorageModifierKind.Interface;

	// (2024-11-08)
	public CloseBraceToken CloseBraceToken { get; private set; }
	public StatementDelimiterToken StatementDelimiterToken { get; private set; }
	public bool IsSingleStatementBody => StatementDelimiterToken.ConstructorWasInvoked;

	public ScopeDirectionKind ScopeDirectionKind => ScopeDirectionKind.Both;

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.TypeDefinitionNode;

    public string EncompassingNamespaceIdentifierString { get; set; }

    public ImmutableArray<FunctionDefinitionNode> GetFunctionDefinitionNodes()
    {
        if (CodeBlockNode is null)
            return ImmutableArray<FunctionDefinitionNode>.Empty;

        return CodeBlockNode.GetChildList()
            .Where(child => child.SyntaxKind == SyntaxKind.FunctionDefinitionNode)
            .Select(fd => (FunctionDefinitionNode)fd)
            .ToImmutableArray();
    }

    public TypeClauseNode ToTypeClause()
    {
    	return _toTypeClauseNodeResult ??= new TypeClauseNode(
            TypeIdentifierToken,
            ValueType,
            null);
    }
    
    public TypeClauseNode? GetReturnTypeClauseNode()
    {
    	return null;
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
    
    public void OnBoundScopeCreatedAndSetAsCurrent(IParserModel parserModel)
    {
    	if (PrimaryConstructorFunctionArgumentsListingNode is not null)
    	{
    		foreach (var argument in PrimaryConstructorFunctionArgumentsListingNode.FunctionArgumentEntryNodeList)
	    	{
	    		parserModel.Binder.BindVariableDeclarationNode(argument.VariableDeclarationNode, parserModel);
	    	
	    		/*if (argument.IsOptional)
	    			parserModel.Binder.BindFunctionOptionalArgument(argument, parserModel);
	    		else
	    			parserModel.Binder.BindVariableDeclarationNode(argument.VariableDeclarationNode, parserModel);*/
	    	}
    	}
    }
    
    public ICodeBlockOwner SetPrimaryConstructorFunctionArgumentsListingNode(FunctionArgumentsListingNode functionArgumentsListingNode)
    {
    	PrimaryConstructorFunctionArgumentsListingNode = functionArgumentsListingNode;
    	
    	_childListIsDirty = true;
    	return this;
    }
    
    public ICodeBlockOwner SetInheritedTypeClauseNode(TypeClauseNode typeClauseNode)
    {
    	InheritedTypeClauseNode = typeClauseNode;
    	
    	_childListIsDirty = true;
    	return this;
    }
    
    public ISyntax[] GetChildList()
    {
    	if (!_childListIsDirty)
    		return _childList;
    	
    	var childCount = 1; // TypeIdentifierToken
        if (GenericArgumentsListingNode is not null)
            childCount++;
        if (InheritedTypeClauseNode is not null)
            childCount++;
        if (CodeBlockNode is not null)
            childCount++;
            
        var childList = new ISyntax[childCount];
		var i = 0;
		
		childList[i++] = TypeIdentifierToken;
		if (GenericArgumentsListingNode is not null)
            childList[i++] = GenericArgumentsListingNode;
        if (InheritedTypeClauseNode is not null)
            childList[i++] = InheritedTypeClauseNode;
        if (CodeBlockNode is not null)
            childList[i++] = CodeBlockNode;

        _childList = childList;
        
    	_childListIsDirty = false;
    	return _childList;
    }
}