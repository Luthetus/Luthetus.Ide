using Luthetus.Extensions.CompilerServices;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes;

/// <summary>
/// <see cref="TypeClauseNode"/> is used anywhere a type is referenced.
/// </summary>
public sealed class TypeClauseNode : IExpressionNode
{
	public TypeClauseNode(
		SyntaxToken typeIdentifier,
		Type? valueType,
		GenericParametersListingNode? genericParametersListingNode,
		bool isKeywordType)
	{
		// Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.TypeClauseNode++;
	
		IsKeywordType = isKeywordType;
		TypeIdentifierToken = typeIdentifier;
		ValueType = valueType;
		GenericParametersListingNode = genericParametersListingNode;
	}

	private IReadOnlyList<ISyntax> _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

	/// <summary>
	/// Given: 'int x = 2;'<br/>
	/// Then: 'int' is the <see cref="TypeIdentifierToken"/>
	/// And: <see cref="GenericParametersListingNode"/> would be null
	/// </summary>
	public SyntaxToken TypeIdentifierToken { get; }
	/// <summary>
	/// Given: 'int x = 2;'<br/>
	/// Then: 'typeof(int)' is the <see cref="ValueType"/>
	/// And: <see cref="GenericParametersListingNode"/> would be null
	///<br/>
	/// In short, <see cref="ValueType"/> is non-null when the
	/// <see cref="TypeIdentifierToken"/> maps to a C# primitive type.
	/// </summary>
	public Type? ValueType { get; private set; }
	/// <summary>
	/// Given: 'int[] x = 2;'<br/>
	/// Then: 'Array&lt;T&gt;' is the <see cref="TypeIdentifierToken"/><br/>
	/// And: '&lt;int&gt;' is the <see cref="GenericParametersListingNode"/>
	/// </summary>
	public GenericParametersListingNode? GenericParametersListingNode { get; private set; }

	public bool IsKeywordType { get; init; }

	public bool IsTuple { get; }

	TypeClauseNode IExpressionNode.ResultTypeClauseNode => TypeFacts.Pseudo.ToTypeClause();

	public bool HasQuestionMark { get; set; }
	public int ArrayRank { get; set; }

	public bool IsFabricated { get; init; }
	public SyntaxKind SyntaxKind => SyntaxKind.TypeClauseNode;

	public TypeClauseNode SetGenericParametersListingNode(GenericParametersListingNode genericParametersListingNode)
	{
		GenericParametersListingNode = genericParametersListingNode;

		_childListIsDirty = true;
		return this;
	}

	public TypeClauseNode SetValueType(Type? valueType)
	{
		ValueType = valueType;

		_childListIsDirty = true;
		return this;
	}

	public IReadOnlyList<ISyntax> GetChildList()
	{
		if (!_childListIsDirty)
			return _childList;

		var childCount = 1; // TypeIdentifierToken
		if (GenericParametersListingNode is not null)
			childCount++;

		var childList = new ISyntax[childCount];
		var i = 0;

		childList[i++] = TypeIdentifierToken;
		if (GenericParametersListingNode is not null)
			childList[i++] = GenericParametersListingNode;

		_childList = childList;

		_childListIsDirty = false;
		return _childList;
	}
}