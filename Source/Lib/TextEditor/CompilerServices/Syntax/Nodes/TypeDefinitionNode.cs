using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Utility;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

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
        TypeClauseNode? inheritedTypeClauseNode)
    {
        AccessModifierKind = accessModifierKind;
        HasPartialModifier = hasPartialModifier;
        StorageModifierKind = storageModifierKind;
        TypeIdentifierToken = typeIdentifier;
        ValueType = valueType;
        GenericArgumentsListingNode = genericArgumentsListingNode;
        PrimaryConstructorFunctionArgumentsListingNode = primaryConstructorFunctionArgumentsListingNode;
        InheritedTypeClauseNode = inheritedTypeClauseNode;
    }

	private ISyntax[] _childList = Array.Empty<ISyntax>();
	private ISyntaxNode[] _memberList = Array.Empty<ISyntaxNode>();
	private bool _childListIsDirty = true;
	private bool _memberListIsDirty = true;

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
    /// Given:<br/>
    /// public class Person : IPerson { ... }<br/><br/>
    /// Then: 'IPerson' is the <see cref="InheritedTypeClauseNode"/>
    /// </summary>
    public TypeClauseNode? InheritedTypeClauseNode { get; private set; }
    public bool IsInterface => StorageModifierKind == StorageModifierKind.Interface;

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.TypeDefinitionNode;

    public string EncompassingNamespaceIdentifierString { get; set; }
    
    // ICodeBlockOwner properties.
	public ScopeDirectionKind ScopeDirectionKind => ScopeDirectionKind.Both;
	public TextEditorTextSpan? OpenCodeBlockTextSpan { get; set; }
	public CodeBlockNode? CodeBlockNode { get; private set; }
	public TextEditorTextSpan? CloseCodeBlockTextSpan { get; set; }
	public int? ScopeIndexKey { get; set; }

    public ImmutableArray<FunctionDefinitionNode> GetFunctionDefinitionNodes()
    {
        if (CodeBlockNode is null)
            return ImmutableArray<FunctionDefinitionNode>.Empty;

        return CodeBlockNode.GetChildList()
            .Where(child => child.SyntaxKind == SyntaxKind.FunctionDefinitionNode)
            .Select(fd => (FunctionDefinitionNode)fd)
            .ToImmutableArray();
    }
    
    public ISyntaxNode[] GetMemberList()
    {
    	if (CodeBlockNode is null)
            return Array.Empty<ISyntaxNode>();
            
        if (!_memberListIsDirty)
        	return _memberList;

        return _memberList = CodeBlockNode.GetChildList()
            .Where(child => child.SyntaxKind == SyntaxKind.FunctionDefinitionNode ||
            				child.SyntaxKind == SyntaxKind.VariableDeclarationNode ||
            				child.SyntaxKind == SyntaxKind.TypeDefinitionNode)
            .Select(x => (ISyntaxNode)x)
            .ToArray();
    }

    public TypeClauseNode ToTypeClause()
    {
    	return _toTypeClauseNodeResult ??= new TypeClauseNode(
            TypeIdentifierToken,
            ValueType,
            null);
    }
    
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
    
    public ICodeBlockOwner SetPrimaryConstructorFunctionArgumentsListingNode(FunctionArgumentsListingNode functionArgumentsListingNode)
    {
    	PrimaryConstructorFunctionArgumentsListingNode = functionArgumentsListingNode;
    	
    	_childListIsDirty = true;
    	_memberListIsDirty = true;
    	return this;
    }
    
    public ICodeBlockOwner SetInheritedTypeClauseNode(TypeClauseNode typeClauseNode)
    {
    	InheritedTypeClauseNode = typeClauseNode;
    	
    	_childListIsDirty = true;
    	_memberListIsDirty = true;
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