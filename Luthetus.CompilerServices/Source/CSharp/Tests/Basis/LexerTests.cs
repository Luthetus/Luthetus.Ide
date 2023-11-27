using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.CompilerServices.Lang.CSharp.LexerCase;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;

namespace Luthetus.CompilerServices.Lang.CSharp.Tests.Basis;

public class LexerTests
{
	[Fact]
	public void LEX_ArraySyntaxToken()
	{
		throw new NotImplementedException();
	}

	[Fact]
	public void LEX_AssociatedNameToken()
	{
		throw new NotImplementedException();
	}

	[Fact]
	public void LEX_AssociatedValueToken()
	{
		throw new NotImplementedException();
	}

	[Fact]
	public void LEX_BadToken()
	{
		throw new NotImplementedException();
	}

	[Fact]
	public void LEX_BangToken()
	{
		var resourceUri = new ResourceUri("UnitTests");
		var sourceText = "!";
		var lexer = new CSharpLexer(resourceUri, sourceText);
		lexer.Lex();

		Assert.Equal(2, lexer.SyntaxTokens.Length);
		var bangToken = (BangToken)lexer.SyntaxTokens[0];
		var endOfFileToken = (EndOfFileToken)lexer.SyntaxTokens[1];

		Assert.Equal(SyntaxKind.BangToken, bangToken.SyntaxKind);
	}

	[Fact]
	public void LEX_CloseAngleBracketToken()
	{
		var resourceUri = new ResourceUri("UnitTests");
		var sourceText = ">";
		var lexer = new CSharpLexer(resourceUri, sourceText);
		lexer.Lex();

		Assert.Equal(2, lexer.SyntaxTokens.Length);
		var closeAngleBracketToken = (CloseAngleBracketToken)lexer.SyntaxTokens[0];
		var endOfFileToken = (EndOfFileToken)lexer.SyntaxTokens[1];

		Assert.Equal(SyntaxKind.CloseAngleBracketToken, closeAngleBracketToken.SyntaxKind);
	}

	[Fact]
	public void LEX_CloseAssociatedGroupToken()
	{
		throw new NotImplementedException();
	}

	[Fact]
	public void LEX_CloseBraceToken()
	{
		var resourceUri = new ResourceUri("UnitTests");
		var sourceText = "}";
		var lexer = new CSharpLexer(resourceUri, sourceText);
		lexer.Lex();

		Assert.Equal(2, lexer.SyntaxTokens.Length);
		var closeBraceToken = (CloseBraceToken)lexer.SyntaxTokens[0];
		var endOfFileToken = (EndOfFileToken)lexer.SyntaxTokens[1];

		Assert.Equal(SyntaxKind.CloseBraceToken, closeBraceToken.SyntaxKind);
	}

	[Fact]
	public void LEX_CloseParenthesisToken()
	{
		var resourceUri = new ResourceUri("UnitTests");
		var sourceText = ")";
		var lexer = new CSharpLexer(resourceUri, sourceText);
		lexer.Lex();

		Assert.Equal(2, lexer.SyntaxTokens.Length);
		var closeParenthesisToken = (CloseParenthesisToken)lexer.SyntaxTokens[0];
		var endOfFileToken = (EndOfFileToken)lexer.SyntaxTokens[1];

		Assert.Equal(SyntaxKind.CloseParenthesisToken, closeParenthesisToken.SyntaxKind);
	}

	[Fact]
	public void LEX_CloseSquareBracketToken()
	{
		var resourceUri = new ResourceUri("UnitTests");
		var sourceText = "]";
		var lexer = new CSharpLexer(resourceUri, sourceText);
		lexer.Lex();

		Assert.Equal(2, lexer.SyntaxTokens.Length);
		var closeSquareBracketToken = (CloseSquareBracketToken)lexer.SyntaxTokens[0];
		var endOfFileToken = (EndOfFileToken)lexer.SyntaxTokens[1];

		Assert.Equal(SyntaxKind.CloseSquareBracketToken, closeSquareBracketToken.SyntaxKind);
	}

	[Fact]
	public void LEX_ColonToken()
	{
		var resourceUri = new ResourceUri("UnitTests");
		var sourceText = ":";
		var lexer = new CSharpLexer(resourceUri, sourceText);
		lexer.Lex();

		Assert.Equal(2, lexer.SyntaxTokens.Length);
		var colonToken = (ColonToken)lexer.SyntaxTokens[0];
		var endOfFileToken = (EndOfFileToken)lexer.SyntaxTokens[1];

		Assert.Equal(SyntaxKind.ColonToken, colonToken.SyntaxKind);
	}

	[Fact]
	public void LEX_CommaToken()
	{
		var resourceUri = new ResourceUri("UnitTests");
		var sourceText = ",";
		var lexer = new CSharpLexer(resourceUri, sourceText);
		lexer.Lex();

		Assert.Equal(2, lexer.SyntaxTokens.Length);
		var commaToken = (CommaToken)lexer.SyntaxTokens[0];
		var endOfFileToken = (EndOfFileToken)lexer.SyntaxTokens[1];

		Assert.Equal(SyntaxKind.CommaToken, commaToken.SyntaxKind);
	}

	[Fact]
	public void LEX_CommentMultiLineToken()
	{
		throw new NotImplementedException();
	}

	[Fact]
	public void LEX_CommentSingleLineToken()
	{
		throw new NotImplementedException();
	}

	[Fact]
	public void LEX_DollarSignToken()
	{
		var resourceUri = new ResourceUri("UnitTests");
		var sourceText = "$";
		var lexer = new CSharpLexer(resourceUri, sourceText);
		lexer.Lex();

		Assert.Equal(2, lexer.SyntaxTokens.Length);
		var dollarSignToken = (DollarSignToken)lexer.SyntaxTokens[0];
		var endOfFileToken = (EndOfFileToken)lexer.SyntaxTokens[1];

		Assert.Equal(SyntaxKind.DollarSignToken, dollarSignToken.SyntaxKind);
	}

	[Fact]
	public void LEX_EndOfFileToken()
	{
		throw new NotImplementedException();
	}

	[Fact]
	public void LEX_EqualsEqualsToken()
	{
		var resourceUri = new ResourceUri("UnitTests");
		var sourceText = "==";
		var lexer = new CSharpLexer(resourceUri, sourceText);
		lexer.Lex();

		Assert.Equal(2, lexer.SyntaxTokens.Length);
		var equalsEqualsToken = (EqualsEqualsToken)lexer.SyntaxTokens[0];
		var endOfFileToken = (EndOfFileToken)lexer.SyntaxTokens[1];

		Assert.Equal(SyntaxKind.EqualsEqualsToken, equalsEqualsToken.SyntaxKind);
	}

	[Fact]
	public void LEX_EqualsToken()
	{
		var resourceUri = new ResourceUri("UnitTests");
		var sourceText = "=";
		var lexer = new CSharpLexer(resourceUri, sourceText);
		lexer.Lex();

		Assert.Equal(2, lexer.SyntaxTokens.Length);
		var equalsToken = (EqualsToken)lexer.SyntaxTokens[0];
		var endOfFileToken = (EndOfFileToken)lexer.SyntaxTokens[1];

		Assert.Equal(SyntaxKind.EqualsToken, equalsToken.SyntaxKind);
	}

	[Fact]
	public void LEX_IdentifierToken()
	{
		throw new NotImplementedException();
	}

	[Fact]
	public void LEX_KeywordContextualToken()
	{
		throw new NotImplementedException();
	}

	[Fact]
	public void LEX_KeywordToken()
	{
		throw new NotImplementedException();
	}

	[Fact]
	public void LEX_LibraryReferenceToken()
	{
		throw new NotImplementedException();
	}

	[Fact]
	public void LEX_MemberAccessToken()
	{
		var resourceUri = new ResourceUri("UnitTests");
		var sourceText = ".";
		var lexer = new CSharpLexer(resourceUri, sourceText);
		lexer.Lex();

		Assert.Equal(2, lexer.SyntaxTokens.Length);
		var memberAccessToken = (MemberAccessToken)lexer.SyntaxTokens[0];
		var endOfFileToken = (EndOfFileToken)lexer.SyntaxTokens[1];

		Assert.Equal(SyntaxKind.MemberAccessToken, memberAccessToken.SyntaxKind);
	}

	[Fact]
	public void LEX_MinusMinusToken()
	{
		var resourceUri = new ResourceUri("UnitTests");
		var sourceText = "--";
		var lexer = new CSharpLexer(resourceUri, sourceText);
		lexer.Lex();

		Assert.Equal(2, lexer.SyntaxTokens.Length);
		var minusMinusToken = (MinusMinusToken)lexer.SyntaxTokens[0];
		var endOfFileToken = (EndOfFileToken)lexer.SyntaxTokens[1];

		Assert.Equal(SyntaxKind.MinusMinusToken, minusMinusToken.SyntaxKind);
	}

	[Fact]
	public void LEX_MinusToken()
	{
		var resourceUri = new ResourceUri("UnitTests");
		var sourceText = "-";
		var lexer = new CSharpLexer(resourceUri, sourceText);
		lexer.Lex();

		Assert.Equal(2, lexer.SyntaxTokens.Length);
		var minusToken = (MinusToken)lexer.SyntaxTokens[0];
		var endOfFileToken = (EndOfFileToken)lexer.SyntaxTokens[1];

		Assert.Equal(SyntaxKind.MinusToken, minusToken.SyntaxKind);
	}

	[Fact]
	public void LEX_NumericLiteralToken()
	{
		var resourceUri = new ResourceUri("UnitTests");
		var sourceText = "2";
		var lexer = new CSharpLexer(resourceUri, sourceText);
		lexer.Lex();

		Assert.Equal(2, lexer.SyntaxTokens.Length);
		var numericLiteralToken = (NumericLiteralToken)lexer.SyntaxTokens[0];
		var endOfFileToken = (EndOfFileToken)lexer.SyntaxTokens[1];

		Assert.Equal(SyntaxKind.NumericLiteralToken, numericLiteralToken.SyntaxKind);
	}

	[Fact]
	public void LEX_OpenAngleBracketToken()
	{
		var resourceUri = new ResourceUri("UnitTests");
		var sourceText = "<";
		var lexer = new CSharpLexer(resourceUri, sourceText);
		lexer.Lex();

		Assert.Equal(2, lexer.SyntaxTokens.Length);
		var openAngleBracketToken = (OpenAngleBracketToken)lexer.SyntaxTokens[0];
		var endOfFileToken = (EndOfFileToken)lexer.SyntaxTokens[1];

		Assert.Equal(SyntaxKind.OpenAngleBracketToken, openAngleBracketToken.SyntaxKind);
	}

	[Fact]
	public void LEX_OpenAssociatedGroupToken()
	{
		throw new NotImplementedException();
	}

	[Fact]
	public void LEX_OpenBraceToken()
	{
		var resourceUri = new ResourceUri("UnitTests");
		var sourceText = "{";
		var lexer = new CSharpLexer(resourceUri, sourceText);
		lexer.Lex();

		Assert.Equal(2, lexer.SyntaxTokens.Length);
		var openBraceToken = (OpenBraceToken)lexer.SyntaxTokens[0];
		var endOfFileToken = (EndOfFileToken)lexer.SyntaxTokens[1];

		Assert.Equal(SyntaxKind.OpenBraceToken, openBraceToken.SyntaxKind);
	}

	[Fact]
	public void LEX_OpenParenthesisToken()
	{
		var resourceUri = new ResourceUri("UnitTests");
		var sourceText = "(";
		var lexer = new CSharpLexer(resourceUri, sourceText);
		lexer.Lex();

		Assert.Equal(2, lexer.SyntaxTokens.Length);
		var openParenthesisToken = (OpenParenthesisToken)lexer.SyntaxTokens[0];
		var endOfFileToken = (EndOfFileToken)lexer.SyntaxTokens[1];

		Assert.Equal(SyntaxKind.OpenParenthesisToken, openParenthesisToken.SyntaxKind);
	}

	[Fact]
	public void LEX_OpenSquareBracketToken()
	{
		var resourceUri = new ResourceUri("UnitTests");
		var sourceText = "[";
		var lexer = new CSharpLexer(resourceUri, sourceText);
		lexer.Lex();

		Assert.Equal(2, lexer.SyntaxTokens.Length);
		var openSquareBracketToken = (OpenSquareBracketToken)lexer.SyntaxTokens[0];
		var endOfFileToken = (EndOfFileToken)lexer.SyntaxTokens[1];

		Assert.Equal(SyntaxKind.OpenSquareBracketToken, openSquareBracketToken.SyntaxKind);
	}

	[Fact]
	public void LEX_PlusPlusToken()
	{
		var resourceUri = new ResourceUri("UnitTests");
		var sourceText = "++";
		var lexer = new CSharpLexer(resourceUri, sourceText);
		lexer.Lex();

		Assert.Equal(2, lexer.SyntaxTokens.Length);
		var plusPlusToken = (PlusPlusToken)lexer.SyntaxTokens[0];
		var endOfFileToken = (EndOfFileToken)lexer.SyntaxTokens[1];

		Assert.Equal(SyntaxKind.PlusPlusToken, plusPlusToken.SyntaxKind);
	}

	[Fact]
	public void LEX_PlusToken()
	{
		var resourceUri = new ResourceUri("UnitTests");
		var sourceText = "+";
		var lexer = new CSharpLexer(resourceUri, sourceText);
		lexer.Lex();

		Assert.Equal(2, lexer.SyntaxTokens.Length);
		var plusToken = (PlusToken)lexer.SyntaxTokens[0];
		var endOfFileToken = (EndOfFileToken)lexer.SyntaxTokens[1];

		Assert.Equal(SyntaxKind.PlusToken, plusToken.SyntaxKind);
	}

	[Fact]
	public void LEX_PreprocessorDirectiveToken()
	{
		throw new NotImplementedException();
	}

	[Fact]
	public void LEX_QuestionMarkQuestionMarkToken()
	{
		var resourceUri = new ResourceUri("UnitTests");
		var sourceText = "??";
		var lexer = new CSharpLexer(resourceUri, sourceText);
		lexer.Lex();

		Assert.Equal(2, lexer.SyntaxTokens.Length);
		var questionMarkQuestionMarkToken = (QuestionMarkQuestionMarkToken)lexer.SyntaxTokens[0];
		var endOfFileToken = (EndOfFileToken)lexer.SyntaxTokens[1];

		Assert.Equal(SyntaxKind.QuestionMarkQuestionMarkToken, questionMarkQuestionMarkToken.SyntaxKind);
	}

	[Fact]
	public void LEX_QuestionMarkToken()
	{
		var resourceUri = new ResourceUri("UnitTests");
		var sourceText = "?";
		var lexer = new CSharpLexer(resourceUri, sourceText);
		lexer.Lex();

		Assert.Equal(2, lexer.SyntaxTokens.Length);
		var questionMarkToken = (QuestionMarkToken)lexer.SyntaxTokens[0];
		var endOfFileToken = (EndOfFileToken)lexer.SyntaxTokens[1];

		Assert.Equal(SyntaxKind.QuestionMarkToken, questionMarkToken.SyntaxKind);
	}

	[Fact]
	public void LEX_StarToken()
	{
		var resourceUri = new ResourceUri("UnitTests");
		var sourceText = "*";
		var lexer = new CSharpLexer(resourceUri, sourceText);
		lexer.Lex();

		Assert.Equal(2, lexer.SyntaxTokens.Length);
		var starToken = (StarToken)lexer.SyntaxTokens[0];
		var endOfFileToken = (EndOfFileToken)lexer.SyntaxTokens[1];

		Assert.Equal(SyntaxKind.StarToken, starToken.SyntaxKind);
	}

	[Fact]
	public void LEX_StatementDelimiterToken()
	{
		var resourceUri = new ResourceUri("UnitTests");
		var sourceText = ";";
		var lexer = new CSharpLexer(resourceUri, sourceText);
		lexer.Lex();

		Assert.Equal(2, lexer.SyntaxTokens.Length);
		var statementDelimiterToken = (StatementDelimiterToken)lexer.SyntaxTokens[0];
		var endOfFileToken = (EndOfFileToken)lexer.SyntaxTokens[1];

		Assert.Equal(SyntaxKind.StatementDelimiterToken, statementDelimiterToken.SyntaxKind);
	}

	[Fact]
	public void LEX_StringLiteralToken()
	{
		throw new NotImplementedException();
	}

	[Fact]
	public void LEX_TriviaToken()
	{
		throw new NotImplementedException();
	}
}
