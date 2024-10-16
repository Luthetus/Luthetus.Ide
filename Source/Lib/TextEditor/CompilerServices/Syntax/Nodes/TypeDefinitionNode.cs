using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

/// <summary>
/// <see cref="TypeDefinitionNode"/> is used anywhere a type is defined.
/// </summary>
public sealed record TypeDefinitionNode : ICodeBlockOwner
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
        
        SetChildList();
    }

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
    public FunctionArgumentsListingNode? PrimaryConstructorFunctionArgumentsListingNode { get; }
    /// <summary>
    /// The open brace for the body code block node.
    /// </summary>
    public OpenBraceToken OpenBraceToken { get; private set; }

    /// <summary>
    /// Given:<br/>
    /// public class Person : IPerson { ... }<br/><br/>
    /// Then: 'IPerson' is the <see cref="InheritedTypeClauseNode"/>
    /// </summary>
    public TypeClauseNode? InheritedTypeClauseNode { get; }
    public CodeBlockNode? CodeBlockNode { get; private set; }
    public bool IsInterface => StorageModifierKind == StorageModifierKind.Interface;

	public ScopeDirectionKind ScopeDirectionKind => ScopeDirectionKind.Both;

    public ImmutableArray<ISyntax> ChildList { get; private set; }
    public ISyntaxNode? Parent { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.TypeDefinitionNode;

    public string EncompassingNamespaceIdentifierString { get; set; }

    public ImmutableArray<FunctionDefinitionNode> GetFunctionDefinitionNodes()
    {
        if (CodeBlockNode is null)
            return ImmutableArray<FunctionDefinitionNode>.Empty;

        return CodeBlockNode.ChildList
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
    
    public ICodeBlockOwner WithCodeBlockNode(OpenBraceToken openBraceToken, CodeBlockNode codeBlockNode)
    {
    	OpenBraceToken = openBraceToken;
    	CodeBlockNode = codeBlockNode;
    	
    	SetChildList();
    	return this;
    }
    
    public void OnBoundScopeCreatedAndSetAsCurrent(IParserModel parserModel)
    {
    	// Do nothing.
    	return;
    }
    
    public void SetChildList()
    {
        var children = new List<ISyntax>
        {
            TypeIdentifierToken,
        };

        if (GenericArgumentsListingNode is not null)
            children.Add(GenericArgumentsListingNode);

        if (InheritedTypeClauseNode is not null)
            children.Add(InheritedTypeClauseNode);

        if (CodeBlockNode is not null)
            children.Add(CodeBlockNode);

        ChildList = children.ToImmutableArray();
    }
}