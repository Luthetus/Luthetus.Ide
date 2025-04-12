using Luthetus.Extensions.CompilerServices;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.Extensions.CompilerServices.Syntax;

public record struct TypeReference
{
	public TypeReference(
		SyntaxToken typeIdentifier,
		Type? valueType,
		GenericParameterListing genericParameterListing,
		bool isKeywordType,
		bool isTuple,
		bool hasQuestionMark,
		int arrayRank,
		bool isFabricated)
	{
		IsKeywordType = isKeywordType;
		TypeIdentifierToken = typeIdentifier;
		ValueType = valueType;
		GenericParameterListing = genericParameterListing;
		IsTuple = isTuple;
		HasQuestionMark = hasQuestionMark;
		ArrayRank = arrayRank;
		IsFabricated = isFabricated;
	}
	
	public TypeReference(TypeClauseNode typeClauseNode)
	{
		IsKeywordType = typeClauseNode.IsKeywordType;
		TypeIdentifierToken = typeClauseNode.TypeIdentifierToken;
		ValueType = typeClauseNode.ValueType;
		GenericParameterListing = typeClauseNode.GenericParameterListing;
		IsTuple = typeClauseNode.IsTuple;
		HasQuestionMark = typeClauseNode.HasQuestionMark;
		ArrayRank = typeClauseNode.ArrayRank;
		IsFabricated = typeClauseNode.IsFabricated;
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
