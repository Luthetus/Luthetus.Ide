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
		TypeClauseNode? inheritedTypeClauseNode,
		string namespaceName)
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
	public GenericParameterListing GenericParameterListing { get; set; }
	public FunctionArgumentListing FunctionArgumentListing { get; private set; }
	public FunctionArgumentListing PrimaryConstructorFunctionArgumentListing => FunctionArgumentListing;
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
	
	TypeClauseNode IExpressionNode.ResultTypeClauseNode => TypeFacts.Pseudo.ToTypeClause();

	public string EncompassingNamespaceIdentifierString { get; set; }

	// ICodeBlockOwner properties.
	public ScopeDirectionKind ScopeDirectionKind => ScopeDirectionKind.Both;
	public TextEditorTextSpan? OpenCodeBlockTextSpan { get; set; }
	public CodeBlockNode? CodeBlockNode { get; private set; }
	public TextEditorTextSpan? CloseCodeBlockTextSpan { get; set; }
	public int ScopeIndexKey { get; set; } = -1;

	public bool IsKeywordType { get; init; }
	
	/// <summary>
	/// TODO: TypeDefinitionNode(s) should use the expression loop to parse the...
	/// ...generic parameters. They currently use 'ParseTypes.HandleGenericArguments(...);'
	/// </summary>
	public bool IsParsingGenericParameters { get; set; }

	public void SetGenericParameterListing(GenericParameterListing genericParameterListing)
	{
		GenericParameterListing = genericParameterListing;
		_childListIsDirty = true;
	}
	
	public void SetGenericParameterListingCloseAngleBracketToken(SyntaxToken closeAngleBracketToken)
	{
		GenericParameterListing.SetCloseAngleBracketToken(closeAngleBracketToken);
		_childListIsDirty = true;
	}
	
	public void SetFunctionArgumentListing(FunctionArgumentListing functionArgumentListing)
	{
		FunctionArgumentListing = functionArgumentListing;
		
		_childListIsDirty = true;
		_memberListIsDirty = true;
	}

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
			genericParameterListing: default,
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
		if (GenericParameterListing.ConstructorWasInvoked)
		{
			childCount +=
				1 +                                                       // GenericParameterListing.OpenAngleBracketToken
				GenericParameterListing.GenericParameterEntryList.Count + // GenericParameterListing.GenericParameterEntryList.Count
				1;                                                        // GenericParameterListing.CloseAngleBracketToken
		}
		if (InheritedTypeClauseNode is not null)
			childCount++;
		if (CodeBlockNode is not null)
			childCount++;

		var childList = new ISyntax[childCount];
		var i = 0;

		childList[i++] = TypeIdentifierToken;
		if (GenericParameterListing.ConstructorWasInvoked)
		{
			childList[i++] = GenericParameterListing.OpenAngleBracketToken;
			foreach (var entry in GenericParameterListing.GenericParameterEntryList)
			{
				childList[i++] = entry.TypeClauseNode;
			}
			childList[i++] = GenericParameterListing.CloseAngleBracketToken;
		}
		if (InheritedTypeClauseNode is not null)
			childList[i++] = InheritedTypeClauseNode;
		if (CodeBlockNode is not null)
			childList[i++] = CodeBlockNode;

		_childList = childList;

		_childListIsDirty = false;
		return _childList;
	}
}