using Luthetus.Extensions.CompilerServices;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.Extensions.CompilerServices.Syntax;

public struct TypeReference
{
	public TypeReference(
		SyntaxToken typeIdentifier,
		Type? valueType,
		GenericParameterListing genericParameterListing,
		bool isKeywordType)
	{
		IsKeywordType = isKeywordType;
		TypeIdentifierToken = typeIdentifier;
		ValueType = valueType;
		GenericParameterListing = genericParameterListing;
	}

	public SyntaxToken TypeIdentifierToken { get; }
	public Type? ValueType { get; }
	public GenericParameterListing GenericParameterListing { get; }
	public bool IsKeywordType { get; }
	public bool IsTuple { get; }
	public bool HasQuestionMark { get; }
	public int ArrayRank { get; }
	public bool IsFabricated { get; }
}
