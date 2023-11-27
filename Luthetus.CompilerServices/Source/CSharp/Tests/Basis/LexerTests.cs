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
		throw new NotImplementedException();
	}

	[Fact]
	public void LEX_CloseAngleBracketToken()
	{
		throw new NotImplementedException();
	}

	[Fact]
	public void LEX_CloseAssociatedGroupToken()
	{
		throw new NotImplementedException();
	}

	[Fact]
	public void LEX_CloseBraceToken()
	{
		throw new NotImplementedException();
	}

	[Fact]
	public void LEX_CloseParenthesisToken()
	{
		throw new NotImplementedException();
	}

	[Fact]
	public void LEX_CloseSquareBracketToken()
	{
		throw new NotImplementedException();
	}

	[Fact]
	public void LEX_ColonToken()
	{
		throw new NotImplementedException();
	}

	[Fact]
	public void LEX_CommaToken()
	{
		throw new NotImplementedException();
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
		throw new NotImplementedException();
	}

	[Fact]
	public void LEX_EndOfFileToken()
	{
		throw new NotImplementedException();
	}

	[Fact]
	public void LEX_EqualsEqualsToken()
	{
		throw new NotImplementedException();
	}

	[Fact]
	public void LEX_EqualsToken()
	{
		throw new NotImplementedException();
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
		throw new NotImplementedException();
	}

	[Fact]
	public void LEX_MinusMinusToken()
	{
		throw new NotImplementedException();
	}

	[Fact]
	public void LEX_MinusToken()
	{
		throw new NotImplementedException();
	}

	[Fact]
	public void LEX_NumericLiteralToken()
	{
		throw new NotImplementedException();
	}

	[Fact]
	public void LEX_OpenAngleBracketToken()
	{
		throw new NotImplementedException();
	}

	[Fact]
	public void LEX_OpenAssociatedGroupToken()
	{
		throw new NotImplementedException();
	}

	[Fact]
	public void LEX_OpenBraceToken()
	{
		throw new NotImplementedException();
	}

	[Fact]
	public void LEX_OpenParenthesisToken()
	{
		throw new NotImplementedException();
	}

	[Fact]
	public void LEX_OpenSquareBracketToken()
	{
		throw new NotImplementedException();
	}

	[Fact]
	public void LEX_PlusPlusToken()
	{
		throw new NotImplementedException();
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
		throw new NotImplementedException();
	}

	[Fact]
	public void LEX_QuestionMarkToken()
	{
		throw new NotImplementedException();
	}

	[Fact]
	public void LEX_StarToken()
	{
		throw new NotImplementedException();
	}

	[Fact]
	public void LEX_StatementDelimiterToken()
	{
		throw new NotImplementedException();
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
