using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.Extensions.CompilerServices.Utility;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes;

/// <summary>
/// <see cref="TypeDefinitionNode"/> is used anywhere a type is defined.
/// </summary>
public sealed class TypeDefinitionNode : ICodeBlockOwner
{
	public TypeDefinitionNode(
		AccessModifierKind accessModifierKind,
		bool hasPartialModifier,
		StorageModifierKind storageModifierKind,
		SyntaxToken typeIdentifier,
		Type? valueType,
		GenericArgumentsListingNode? genericArgumentsListingNode,
		FunctionArgumentsListingNode? primaryConstructorFunctionArgumentsListingNode,
		TypeClauseNode? inheritedTypeClauseNode,
		string namespaceName)
	{
		// Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.TypeDefinitionNode++;
	
		AccessModifierKind = accessModifierKind;
		HasPartialModifier = hasPartialModifier;
		StorageModifierKind = storageModifierKind;
		TypeIdentifierToken = typeIdentifier;
		ValueType = valueType;
		GenericArgumentsListingNode = genericArgumentsListingNode;
		PrimaryConstructorFunctionArgumentsListingNode = primaryConstructorFunctionArgumentsListingNode;
		InheritedTypeClauseNode = inheritedTypeClauseNode;
		NamespaceName = namespaceName;
	}

	private IReadOnlyList<ISyntax> _childList = Array.Empty<ISyntax>();
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
	public SyntaxToken TypeIdentifierToken { get; }
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
	public string NamespaceName { get; }
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

	public bool IsKeywordType { get; init; }

	public FunctionDefinitionNode[] GetFunctionDefinitionNodes()
	{
		if (CodeBlockNode is null)
			return Array.Empty<FunctionDefinitionNode>();

		return CodeBlockNode.GetChildList()
			.Where(child => child.SyntaxKind == SyntaxKind.FunctionDefinitionNode)
			.Select(fd => (FunctionDefinitionNode)fd)
			.ToArray();
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
			genericParametersListingNode: null,
			isKeywordType: IsKeywordType);
	}

	#region ICodeBlockOwner_Methods
	public TypeClauseNode? GetReturnTypeClauseNode()
	{
		return null;
	}

	public ICodeBlockOwner SetOpenCodeBlockTextSpan(TextEditorTextSpan? openCodeBlockTextSpan, List<TextEditorDiagnostic> diagnosticList, TokenWalker tokenWalker)
	{
		if (OpenCodeBlockTextSpan is not null)
			ICodeBlockOwner.ThrowMultipleScopeDelimiterException(diagnosticList, tokenWalker);

		OpenCodeBlockTextSpan = openCodeBlockTextSpan;

		_childListIsDirty = true;
		return this;
	}

	public ICodeBlockOwner SetCloseCodeBlockTextSpan(TextEditorTextSpan? closeCodeBlockTextSpan, List<TextEditorDiagnostic> diagnosticList, TokenWalker tokenWalker)
	{
		if (CloseCodeBlockTextSpan is not null)
			ICodeBlockOwner.ThrowMultipleScopeDelimiterException(diagnosticList, tokenWalker);

		CloseCodeBlockTextSpan = closeCodeBlockTextSpan;

		_childListIsDirty = true;
		return this;
	}

	public ICodeBlockOwner SetCodeBlockNode(CodeBlockNode codeBlockNode, List<TextEditorDiagnostic> diagnosticList, TokenWalker tokenWalker)
	{
		if (CodeBlockNode is not null)
			ICodeBlockOwner.ThrowAlreadyAssignedCodeBlockNodeException(diagnosticList, tokenWalker);

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

	public IReadOnlyList<ISyntax> GetChildList()
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