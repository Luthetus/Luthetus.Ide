using Luthetus.Extensions.CompilerServices;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes;

/// <summary>
/// <see cref="TypeClauseNode"/> is used anywhere a type is referenced.
/// </summary>
public sealed class TypeClauseNode : IGenericParameterNode
{
	public TypeClauseNode(
		SyntaxToken typeIdentifier,
		Type? valueType,
		GenericParameterListing genericParameterListing,
		bool isKeywordType)
	{
		#if DEBUG
		Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.TypeClauseNode++;
		#endif
	
		IsKeywordType = isKeywordType;
		TypeIdentifierToken = typeIdentifier;
		ValueType = valueType;
		GenericParameterListing = genericParameterListing;
	}
	
	/// <summary>
	/// Various UI events can result in a 'TypeReference' needing to be shown on the UI.
	///
	/// In order to do this however, you'd have to cast 'TypeReference' as 'ISyntax',
	/// and this would cause boxing.
	///
	/// It is presumably preferred to just eat an "object-y" cost just once by creating a 'TypeClauseNode'.
	/// Lest it get boxed, unboxed, and boxed again -- over and over.
	///
	/// I'm going to use this constructor in the CSharpBinder expression logic temporarily so I can get it build.
	/// But any usage of this kind should probably be removed.
	/// </summary>
	public TypeClauseNode(TypeReference typeReference)
	{
		#if DEBUG
		Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.TypeClauseNode++;
		#endif
	
		IsKeywordType = typeReference.IsKeywordType;
		TypeIdentifierToken = typeReference.TypeIdentifierToken;
		ValueType = typeReference.ValueType;
		GenericParameterListing = typeReference.GenericParameterListing;
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
	public GenericParameterListing GenericParameterListing { get; set; }

	public bool IsKeywordType { get; init; }

	public bool IsTuple { get; }

	TypeReference IExpressionNode.ResultTypeReference => TypeFacts.Pseudo.ToTypeReference();

	public bool HasQuestionMark { get; set; }
	public int ArrayRank { get; set; }

	public bool IsFabricated { get; init; }
	public SyntaxKind SyntaxKind => SyntaxKind.TypeClauseNode;
	
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
		if (GenericParameterListing.ConstructorWasInvoked)
		{
			childCount +=
				1 +                                                       // GenericParameterListing.OpenAngleBracketToken
				// GenericParameterListing.GenericParameterEntryList.Count + // GenericParameterListing.GenericParameterEntryList.Count
				1;                                                        // GenericParameterListing.CloseAngleBracketToken
		}

		var childList = new ISyntax[childCount];
		var i = 0;

		childList[i++] = TypeIdentifierToken;
		if (GenericParameterListing.ConstructorWasInvoked)
		{
			childList[i++] = GenericParameterListing.OpenAngleBracketToken;
			/*foreach (var entry in GenericParameterListing.GenericParameterEntryList)
			{
				childList[i++] = entry.TypeClauseNode;
			}*/
			childList[i++] = GenericParameterListing.CloseAngleBracketToken;
		}

		_childList = childList;

		_childListIsDirty = false;
		return _childList;
	}
}