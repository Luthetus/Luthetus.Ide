using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.CompilerServices.Lang.CSharp.LexerCase;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;

namespace Luthetus.CompilerServices.Lang.CSharp.Tests.Basis;

public class CustomLexerTests
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

	[Theory]
	[InlineData("a")]
	[InlineData("b")]
	[InlineData("c")]
	[InlineData("d")]
	[InlineData("e")]
	[InlineData("f")]
	[InlineData("g")]
	[InlineData("h")]
	[InlineData("i")]
	[InlineData("j")]
	[InlineData("k")]
	[InlineData("l")]
	[InlineData("m")]
	[InlineData("n")]
	[InlineData("o")]
	[InlineData("p")]
	[InlineData("q")]
	[InlineData("r")]
	[InlineData("s")]
	[InlineData("t")]
	[InlineData("u")]
	[InlineData("v")]
	[InlineData("w")]
	[InlineData("x")]
	[InlineData("y")]
	[InlineData("z")]
	[InlineData("A")]
	[InlineData("B")]
	[InlineData("C")]
	[InlineData("D")]
	[InlineData("E")]
	[InlineData("F")]
	[InlineData("G")]
	[InlineData("H")]
	[InlineData("I")]
	[InlineData("J")]
	[InlineData("K")]
	[InlineData("L")]
	[InlineData("M")]
	[InlineData("N")]
	[InlineData("O")]
	[InlineData("P")]
	[InlineData("Q")]
	[InlineData("R")]
	[InlineData("S")]
	[InlineData("T")]
	[InlineData("U")]
	[InlineData("V")]
	[InlineData("W")]
	[InlineData("X")]
	[InlineData("Y")]
	[InlineData("Z")]
	public void LEX_IdentifierToken(string sourceText)
	{
		var resourceUri = new ResourceUri("UnitTests");
		var lexer = new CSharpLexer(resourceUri, sourceText);
		lexer.Lex();

		Assert.Equal(2, lexer.SyntaxTokens.Length);
		var identifierToken = (IdentifierToken)lexer.SyntaxTokens[0];
		var endOfFileToken = (EndOfFileToken)lexer.SyntaxTokens[1];

		Assert.Equal(SyntaxKind.IdentifierToken, identifierToken.SyntaxKind);
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
		var resourceUri = new ResourceUri("UnitTests");
		var sourceText = "\"Hello World!\"";
		var lexer = new CSharpLexer(resourceUri, sourceText);
		lexer.Lex();

		Assert.Equal(2, lexer.SyntaxTokens.Length);
		var stringLiteralToken = (StringLiteralToken)lexer.SyntaxTokens[0];
		var endOfFileToken = (EndOfFileToken)lexer.SyntaxTokens[1];

		Assert.Equal(SyntaxKind.StringLiteralToken, stringLiteralToken.SyntaxKind);
	}

	[Fact]
	public void LEX_TriviaToken()
	{
		throw new NotImplementedException();
	}
	
	[Fact]
	public void LEX_EscapedStrings()
	{
        var sourceText = @"public const string Tag = ""`'\"";luth_clipboard"";".ReplaceLineEndings("\n");
        var resourceUri = new ResourceUri("UnitTests");
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();

		throw new NotImplementedException();
    }

	[Fact]
	public void LEX_ClassDefinition()
	{
        var sourceText = @"public class MyClass
{
}".ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri("UnitTests");
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();

        // Tokens: 'public' 'class' 'MyClass' '{' '}' 'EndOfFileToken'
        Assert.Equal(6, lexer.SyntaxTokens.Length);
	}
}
