using Xunit;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.GenericLexer.SyntaxActors;

public class GenericSyntaxTreeTests
{
	[Fact]
	public void Constructor()
	{
		//public GenericSyntaxTree(GenericLanguageDefinition genericLanguageDefinition)
		throw new NotImplementedException();
	}

	[Fact]
	public void GenericLanguageDefinition()
	{
		//public GenericLanguageDefinition GenericLanguageDefinition { get; }
		throw new NotImplementedException();
	}

	[Fact]
	public void ParseText()
	{
		//public virtual GenericSyntaxUnit ParseText(ResourceUri resourceUri, string content)
		throw new NotImplementedException();
	}

	[Fact]
	public void ParseCommentSingleLine()
	{
		//public virtual GenericCommentSingleLineSyntax ParseCommentSingleLine(
		//	StringWalker stringWalker,
		//	LuthetusDiagnosticBag diagnosticBag)
		throw new NotImplementedException();
	}

	[Fact]
	public void ParseCommentMultiLine()
	{
		//public virtual GenericCommentMultiLineSyntax ParseCommentMultiLine(
		//       StringWalker stringWalker,
		//       LuthetusDiagnosticBag diagnosticBag)
		throw new NotImplementedException();
	}

	[Fact]
	public void ParseString()
	{
		//public virtual GenericStringSyntax ParseString(
		//	StringWalker stringWalker,
		//	LuthetusDiagnosticBag diagnosticBag)
		throw new NotImplementedException();
	}

	[Fact]
	public void TryParseKeyword()
	{
		//private bool TryParseKeyword(
		//	StringWalker stringWalker,
		//	LuthetusDiagnosticBag diagnosticBag,
		//	out GenericKeywordSyntax? genericKeywordSyntax)
		throw new NotImplementedException();
	}

	[Fact]
	public void TryParseFunctionIdentifier()
	{
		//private bool TryParseFunctionIdentifier(
		//	StringWalker stringWalker,
		//	LuthetusDiagnosticBag diagnosticBag,
		//	out GenericFunctionSyntax? genericFunctionSyntax)
		throw new NotImplementedException();
	}

	[Fact]
	public void ParsePreprocessorDirective()
	{
		//private GenericPreprocessorDirectiveSyntax ParsePreprocessorDirective(
		//	StringWalker stringWalker,
		//	LuthetusDiagnosticBag diagnosticBag)
		throw new NotImplementedException();
	}

	[Fact]
	public void TryParsePreprocessorDirectiveDeliminationExtendedSyntaxes()
	{
		//private bool TryParsePreprocessorDirectiveDeliminationExtendedSyntaxes(
		//	StringWalker stringWalker,
		//	LuthetusDiagnosticBag diagnosticBag,
		//	out IGenericSyntax? genericSyntax)
		throw new NotImplementedException();
	}
}