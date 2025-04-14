using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.Extensions.CompilerServices.Utility;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes;

/// <summary>
/// <see cref="TypeDefinitionNode"/> is used anywhere a type is defined.
/// </summary>
public sealed class TypeDefinitionNode : ICodeBlockOwner, IFunctionDefinitionNode, IGenericParameterNode
{
	public TypeDefinitionNode(
		AccessModifierKind accessModifierKind,
		bool hasPartialModifier,
		StorageModifierKind storageModifierKind,
		SyntaxToken typeIdentifier,
		Type? valueType,
		GenericParameterListing genericParameterListing,
		FunctionArgumentListing primaryConstructorFunctionArgumentListing,
		TypeReference inheritedTypeReference,
		string namespaceName
		// FindAllReferences
		// , HashSet<ResourceUri>? referenceHashSet
		)
	{
		#if DEBUG
		Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.TypeDefinitionNode++;
		#endif
	
		AccessModifierKind = accessModifierKind;
		HasPartialModifier = hasPartialModifier;
		StorageModifierKind = storageModifierKind;
		TypeIdentifierToken = typeIdentifier;
		ValueType = valueType;
		GenericParameterListing = genericParameterListing;
		FunctionArgumentListing = primaryConstructorFunctionArgumentListing;
		InheritedTypeReference = inheritedTypeReference;
		NamespaceName = namespaceName;
		
		// FindAllReferences
		// ReferenceHashSet = referenceHashSet;
	}

	private ISyntaxNode[] _memberList = Array.Empty<ISyntaxNode>();
	private bool _memberListIsDirty = true;

	private TypeClauseNode? _toTypeClauseResult;
	
	private bool _hasCalculatedToTypeReference = false;
	private TypeReference _toTypeReferenceResult;

	public AccessModifierKind AccessModifierKind { get; }
	public bool HasPartialModifier { get; }
	public StorageModifierKind StorageModifierKind { get; }
	/// <summary>
	/// Given: 'public class Person { /* class definition here */ }'<br/>
	/// Then: 'Person' is the <see cref="TypeIdentifierToken"/><br/>
	/// And: <see cref="GenericArgumentsListingNode"/> would be null
	/// </summary>
	public SyntaxToken TypeIdentifierToken { get; }
	public Type? ValueType { get; }
	/// <summary>
	/// Given: 'public struct Array&lt;T&gt; { /* struct definition here */ }'<br/>
	/// Then: 'Array&lt;T&gt;' is the <see cref="TypeIdentifierToken"/><br/>
	/// And: '&lt;T&gt;' is the <see cref="GenericArgumentsListingNode"/>
	/// </summary>
	public GenericParameterListing GenericParameterListing { get; set; }
	public FunctionArgumentListing FunctionArgumentListing { get; set; }
	public FunctionArgumentListing PrimaryConstructorFunctionArgumentListing => FunctionArgumentListing;
	/// <summary>
	/// Given:<br/>
	/// public class Person : IPerson { ... }<br/><br/>
	/// Then: 'IPerson' is the <see cref="InheritedTypeClauseNode"/>
	/// </summary>
	public TypeReference InheritedTypeReference { get; private set; }
	public string NamespaceName { get; }
	public bool IsInterface => StorageModifierKind == StorageModifierKind.Interface;

	public bool IsFabricated { get; init; }
	public SyntaxKind SyntaxKind => SyntaxKind.TypeDefinitionNode;
	
	TypeReference IExpressionNode.ResultTypeReference => TypeFacts.Pseudo.ToTypeReference();

	/// <summary>
	/// TODO: Where is this used? ('NamespaceName' already exists and seems to be the one to keep).
	/// </summary>
	public string EncompassingNamespaceIdentifierString { get; set; }
	
	/// <summary>
	/// Any files that contain a reference to this TypeDefinitionNode should
	/// have their ResourceUri in this.
	/// </summary>
	// FindAllReferences
	// public HashSet<ResourceUri>? ReferenceHashSet { get; set; }

	// ICodeBlockOwner properties.
	public ScopeDirectionKind ScopeDirectionKind => ScopeDirectionKind.Both;
	public TextEditorTextSpan OpenCodeBlockTextSpan { get; set; }
	public CodeBlock CodeBlock { get; set; }
	public TextEditorTextSpan CloseCodeBlockTextSpan { get; set; }
	public int ScopeIndexKey { get; set; } = -1;

	public bool IsKeywordType { get; init; }
	
	/// <summary>
	/// TODO: TypeDefinitionNode(s) should use the expression loop to parse the...
	/// ...generic parameters. They currently use 'ParseTypes.HandleGenericArguments(...);'
	/// </summary>
	public bool IsParsingGenericParameters { get; set; }

	public void SetFunctionArgumentListing(FunctionArgumentListing functionArgumentListing)
	{
		FunctionArgumentListing = functionArgumentListing;
		_memberListIsDirty = true;
	}

	public FunctionDefinitionNode[] GetFunctionDefinitionNodes()
	{
		if (!CodeBlock.ConstructorWasInvoked)
			return Array.Empty<FunctionDefinitionNode>();

		return CodeBlock.ChildList
			.Where(child => child.SyntaxKind == SyntaxKind.FunctionDefinitionNode)
			.Select(fd => (FunctionDefinitionNode)fd)
			.ToArray();
	}

	public ISyntaxNode[] GetMemberList()
	{
		if (!CodeBlock.ConstructorWasInvoked)
			return Array.Empty<ISyntaxNode>();

		if (!_memberListIsDirty)
			return _memberList;

		return _memberList = CodeBlock.ChildList
			.Where(child => child.SyntaxKind == SyntaxKind.FunctionDefinitionNode ||
							child.SyntaxKind == SyntaxKind.VariableDeclarationNode ||
							child.SyntaxKind == SyntaxKind.TypeDefinitionNode)
			.Select(x => (ISyntaxNode)x)
			.ToArray();
	}

	public TypeClauseNode ToTypeClause()
	{
		return _toTypeClauseResult ??= new TypeClauseNode(
			TypeIdentifierToken,
			ValueType,
			genericParameterListing: default,
			isKeywordType: IsKeywordType);
	}
	
	public TypeReference ToTypeReference()
	{
		if (!_hasCalculatedToTypeReference)
			_toTypeReferenceResult = new TypeReference(ToTypeClause());
		
		return _toTypeReferenceResult;
	}

	#region ICodeBlockOwner_Methods
	public TypeReference GetReturnTypeReference()
	{
		return TypeFacts.Empty.ToTypeReference();
	}
	#endregion

	public ICodeBlockOwner SetInheritedTypeReference(TypeReference typeReference)
	{
		InheritedTypeReference = typeReference;

		_memberListIsDirty = true;
		return this;
	}
}