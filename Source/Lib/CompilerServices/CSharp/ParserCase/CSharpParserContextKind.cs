namespace Luthetus.CompilerServices.CSharp.ParserCase;

public enum CSharpParserContextKind
{
	None,
	ForceParseNextIdentifierAsTypeClauseNode,
	ForceParseNextIdentifierAsVariableReferenceNode,
    ForceParseGenericParameters,
    ForceStatementExpression
}
