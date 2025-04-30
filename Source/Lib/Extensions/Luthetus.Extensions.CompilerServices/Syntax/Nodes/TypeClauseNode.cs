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

	private bool _isFabricated;

	/// <summary>
	/// Given: 'int x = 2;'<br/>
	/// Then: 'int' is the <see cref="TypeIdentifierToken"/>
	/// And: <see cref="GenericParametersListingNode"/> would be null
	/// </summary>
	public SyntaxToken TypeIdentifierToken { get; set; }
	/// <summary>
	/// Given: 'int x = 2;'<br/>
	/// Then: 'typeof(int)' is the <see cref="ValueType"/>
	/// And: <see cref="GenericParametersListingNode"/> would be null
	///<br/>
	/// In short, <see cref="ValueType"/> is non-null when the
	/// <see cref="TypeIdentifierToken"/> maps to a C# primitive type.
	/// </summary>
	public Type? ValueType { get; set; }
	/// <summary>
	/// Given: 'int[] x = 2;'<br/>
	/// Then: 'Array&lt;T&gt;' is the <see cref="TypeIdentifierToken"/><br/>
	/// And: '&lt;int&gt;' is the <see cref="GenericParametersListingNode"/>
	/// </summary>
	public GenericParameterListing GenericParameterListing { get; set; }

	public bool IsKeywordType { get; set; }

	public bool IsTuple { get; set; }

	TypeReference IExpressionNode.ResultTypeReference => TypeFacts.Pseudo.ToTypeReference();

	public bool HasQuestionMark { get; set; }
	public int ArrayRank { get; set; }
	
	public bool IsBeingUsed { get; set; } = false;

	public bool IsFabricated
	{
		get
		{
			return _isFabricated;
		}
		init
		{
			_isFabricated = value;
		}
	}
	
	public SyntaxKind SyntaxKind => SyntaxKind.TypeClauseNode;
	
	public bool IsParsingGenericParameters { get; set; }

	public void SetSharedInstance(
		SyntaxToken typeIdentifier,
		Type? valueType,
		GenericParameterListing genericParameterListing,
		bool isKeywordType)
	{
		IsBeingUsed = true;
	
		TypeIdentifierToken = typeIdentifier;
		ValueType = valueType;
		GenericParameterListing = genericParameterListing;
		IsKeywordType = isKeywordType;
		IsTuple = false;
		HasQuestionMark = false;
		ArrayRank = 0;
		_isFabricated = false;
		IsParsingGenericParameters = false;
	}

	public TypeClauseNode SetValueType(Type? valueType)
	{
		ValueType = valueType;
		return this;
	}

	#if DEBUG	
	~TypeClauseNode()
	{
		Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.TypeClauseNode--;
	}
	#endif
}