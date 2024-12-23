using System.Text;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.CompilerServices.CSharp.LexerCase;
using Luthetus.CompilerServices.CSharp.ParserCase;
using Luthetus.CompilerServices.CSharp.CompilerServiceCase;

namespace Luthetus.CompilerServices.CSharp.Tests.SmokeTests.Lexers;

public class LexerTests
{
	public class Test
	{
		public Test(string sourceText)
		{
			SourceText = sourceText;
			ResourceUri = new ResourceUri("./unitTesting.txt");
			CompilationUnit.LexerOutput = CSharpLexer.Lex(ResourceUri, SourceText);
	        CSharpParser.Parse(CompilationUnit);
		}
		
		public string SourceText { get; set; }
		public ResourceUri ResourceUri { get; set; }
		public CSharpLexerOutput LexerOutput { get; set; }
		public IBinder Binder => CompilationUnit.Binder;
		public CSharpCompilationUnit CompilationUnit { get; set; }
	}
	
	/// <summary>
	/// Goal: Track the OpenBraceToken and CloseBraceToken pairs that occur in the text. (2024-12-03)
	///
	/// Purpose: Expected to be a noticeable optimization to the Parser speed.
	///
	/// Measurements:
	/// - Before: 63.973 seconds | 65.777 seconds | 64.548 seconds
	/// - After: 
	///
	/// Conclusion: 
	/// 
	/// ==================
	/// </summary>
	///
	/// <remarks>
	/// It could be useful to add a timer to the 'ProgressBarModel.cs'.
	///
	/// Just by tracking the startingDateTime and the (endDateTime ?? currentDateTime),
	/// then every render of the notification, the difference between them can be displayed.
	/// </remarks>
	[Fact]
    public void TrackBraceTokenPairs()
    {
        var test = new Test(
@"
TODO
".ReplaceLineEndings("\n"));
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		throw new NotImplementedException();
    }
    
    /// <summary>
    /// Include the new escape sequence syntax,
    /// i.e.: '""' or two double quotes, is interpreted as one double quotes.
    /// </summary>
    [Fact]
    public void InterpolatedString()
    {
    	// Edge cases:
    	// 	- '{{'
    	// 	- ending in a expression, when done lexing with close brace token,
    	// 		ensure the end double quotes character is not skipped over by accident.
        var test = new Test(
@"
$""a {{ a {3 + 3}"";
".ReplaceLineEndings("\n"));
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		throw new NotImplementedException();
    }
    
    /// <summary>
    /// Include the new escape sequence syntax,
    /// i.e.: '{{' or two open brace characters, is interpreted as one open brace character (the same is true for close brace).
    /// </summary>
    [Fact]
    public void VerbatimString()
    {
        var test = new Test(
@"
var y = @""\n""""\t"";
".ReplaceLineEndings("\n"));
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		throw new NotImplementedException();
    }
    
    /// <summary>
    /// https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/strings/#raw-string-literals
    /// Include the new escape sequence syntax,
    /// i.e.: the count of double quotes that deliminate the start of the string, is to be matched for the end of the string.
    ///       As well, interpolation can be modified to by changing the amount of '$' or dollar sign characters
    ///           that appear prior to the double quotes.
    /// </summary>
    [Fact]
    public void RawString()
    {
        var test = new Test(
@"
TODO
".ReplaceLineEndings("\n"));
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		throw new NotImplementedException();
    }
	
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
    public void LEX_PipeToken()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "|";
        var lexerOutput = CSharpLexer.Lex(resourceUri, sourceText);

        Assert.Equal(2, lexerOutput.SyntaxTokenList.Count);
        var pipeToken = (PipeToken)lexerOutput.SyntaxTokenList[0];
        var endOfFileToken = (EndOfFileToken)lexerOutput.SyntaxTokenList[1];

        Assert.Equal(SyntaxKind.PipeToken, pipeToken.SyntaxKind);
    }

    [Fact]
    public void LEX_PipePipeToken()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "||";
        var lexerOutput = CSharpLexer.Lex(resourceUri, sourceText);

        Assert.Equal(2, lexerOutput.SyntaxTokenList.Count);
        var pipePipeToken = (PipePipeToken)lexerOutput.SyntaxTokenList[0];
        var endOfFileToken = (EndOfFileToken)lexerOutput.SyntaxTokenList[1];

        Assert.Equal(SyntaxKind.PipePipeToken, pipePipeToken.SyntaxKind);
    }
    
    [Fact]
    public void LEX_AmpersandToken()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "&";
        var lexerOutput = CSharpLexer.Lex(resourceUri, sourceText);

        Assert.Equal(2, lexerOutput.SyntaxTokenList.Count);
        var ampersandToken = (AmpersandToken)lexerOutput.SyntaxTokenList[0];
        var endOfFileToken = (EndOfFileToken)lexerOutput.SyntaxTokenList[1];

        Assert.Equal(SyntaxKind.AmpersandToken, ampersandToken.SyntaxKind);
    }

    [Fact]
    public void LEX_AmpersandAmpersandToken()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "&&";
        var lexerOutput = CSharpLexer.Lex(resourceUri, sourceText);

        Assert.Equal(2, lexerOutput.SyntaxTokenList.Count);
        var ampersandAmpersandToken = (AmpersandAmpersandToken)lexerOutput.SyntaxTokenList[0];
        var endOfFileToken = (EndOfFileToken)lexerOutput.SyntaxTokenList[1];

        Assert.Equal(SyntaxKind.AmpersandAmpersandToken, ampersandAmpersandToken.SyntaxKind);
    }

    [Fact]
    public void LEX_BangToken()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "!";
        var lexerOutput = CSharpLexer.Lex(resourceUri, sourceText);

        Assert.Equal(2, lexerOutput.SyntaxTokenList.Count);
        var bangToken = (BangToken)lexerOutput.SyntaxTokenList[0];
        var endOfFileToken = (EndOfFileToken)lexerOutput.SyntaxTokenList[1];

        Assert.Equal(SyntaxKind.BangToken, bangToken.SyntaxKind);
    }
    
    [Fact]
    public void LEX_BangEqualsToken()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "!=";
        var lexerOutput = CSharpLexer.Lex(resourceUri, sourceText);

        Assert.Equal(2, lexerOutput.SyntaxTokenList.Count);
        var bangEqualsToken = (BangEqualsToken)lexerOutput.SyntaxTokenList[0];
        var endOfFileToken = (EndOfFileToken)lexerOutput.SyntaxTokenList[1];

        Assert.Equal(SyntaxKind.BangEqualsToken, bangEqualsToken.SyntaxKind);
    }
    
    [Fact]
    public void LEX_CloseAngleBracketEqualsToken()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = ">=";
        var lexerOutput = CSharpLexer.Lex(resourceUri, sourceText);

        Assert.Equal(2, lexerOutput.SyntaxTokenList.Count);
        var closeAngleBracketEqualsToken = (CloseAngleBracketEqualsToken)lexerOutput.SyntaxTokenList[0];
        var endOfFileToken = (EndOfFileToken)lexerOutput.SyntaxTokenList[1];

        Assert.Equal(SyntaxKind.CloseAngleBracketEqualsToken, closeAngleBracketEqualsToken.SyntaxKind);
    }
    
    [Fact]
    public void LEX_OpenAngleBracketEqualsToken()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "<=";
        var lexerOutput = CSharpLexer.Lex(resourceUri, sourceText);

        Assert.Equal(2, lexerOutput.SyntaxTokenList.Count);
        var openAngleBracketEqualsToken = (OpenAngleBracketEqualsToken)lexerOutput.SyntaxTokenList[0];
        var endOfFileToken = (EndOfFileToken)lexerOutput.SyntaxTokenList[1];

        Assert.Equal(SyntaxKind.OpenAngleBracketEqualsToken, openAngleBracketEqualsToken.SyntaxKind);
    }

    [Fact]
    public void LEX_CloseAngleBracketToken()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = ">";
        var lexerOutput = CSharpLexer.Lex(resourceUri, sourceText);

        Assert.Equal(2, lexerOutput.SyntaxTokenList.Count);
        var closeAngleBracketToken = (CloseAngleBracketToken)lexerOutput.SyntaxTokenList[0];
        var endOfFileToken = (EndOfFileToken)lexerOutput.SyntaxTokenList[1];

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
        var lexerOutput = CSharpLexer.Lex(resourceUri, sourceText);

        Assert.Equal(2, lexerOutput.SyntaxTokenList.Count);
        var closeBraceToken = (CloseBraceToken)lexerOutput.SyntaxTokenList[0];
        var endOfFileToken = (EndOfFileToken)lexerOutput.SyntaxTokenList[1];

        Assert.Equal(SyntaxKind.CloseBraceToken, closeBraceToken.SyntaxKind);
    }

    [Fact]
    public void LEX_CloseParenthesisToken()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = ")";
        var lexerOutput = CSharpLexer.Lex(resourceUri, sourceText);

        Assert.Equal(2, lexerOutput.SyntaxTokenList.Count);
        var closeParenthesisToken = (CloseParenthesisToken)lexerOutput.SyntaxTokenList[0];
        var endOfFileToken = (EndOfFileToken)lexerOutput.SyntaxTokenList[1];

        Assert.Equal(SyntaxKind.CloseParenthesisToken, closeParenthesisToken.SyntaxKind);
    }

    [Fact]
    public void LEX_CloseSquareBracketToken()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "]";
        var lexerOutput = CSharpLexer.Lex(resourceUri, sourceText);

        Assert.Equal(2, lexerOutput.SyntaxTokenList.Count);
        var closeSquareBracketToken = (CloseSquareBracketToken)lexerOutput.SyntaxTokenList[0];
        var endOfFileToken = (EndOfFileToken)lexerOutput.SyntaxTokenList[1];

        Assert.Equal(SyntaxKind.CloseSquareBracketToken, closeSquareBracketToken.SyntaxKind);
    }

    [Fact]
    public void LEX_ColonToken()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = ":";
        var lexerOutput = CSharpLexer.Lex(resourceUri, sourceText);

        Assert.Equal(2, lexerOutput.SyntaxTokenList.Count);
        var colonToken = (ColonToken)lexerOutput.SyntaxTokenList[0];
        var endOfFileToken = (EndOfFileToken)lexerOutput.SyntaxTokenList[1];

        Assert.Equal(SyntaxKind.ColonToken, colonToken.SyntaxKind);
    }

    [Fact]
    public void LEX_CommaToken()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = ",";
        var lexerOutput = CSharpLexer.Lex(resourceUri, sourceText);

        Assert.Equal(2, lexerOutput.SyntaxTokenList.Count);
        var commaToken = (CommaToken)lexerOutput.SyntaxTokenList[0];
        var endOfFileToken = (EndOfFileToken)lexerOutput.SyntaxTokenList[1];

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
        var lexerOutput = CSharpLexer.Lex(resourceUri, sourceText);

        Assert.Equal(2, lexerOutput.SyntaxTokenList.Count);
        var dollarSignToken = (DollarSignToken)lexerOutput.SyntaxTokenList[0];
        var endOfFileToken = (EndOfFileToken)lexerOutput.SyntaxTokenList[1];

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
        var lexerOutput = CSharpLexer.Lex(resourceUri, sourceText);

        Assert.Equal(2, lexerOutput.SyntaxTokenList.Count);
        var equalsEqualsToken = (EqualsEqualsToken)lexerOutput.SyntaxTokenList[0];
        var endOfFileToken = (EndOfFileToken)lexerOutput.SyntaxTokenList[1];

        Assert.Equal(SyntaxKind.EqualsEqualsToken, equalsEqualsToken.SyntaxKind);
    }
    
    [Fact]
    public void LEX_EqualsCloseAngleBracketToken()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "=>";
        var lexerOutput = CSharpLexer.Lex(resourceUri, sourceText);

        Assert.Equal(2, lexerOutput.SyntaxTokenList.Count);
        var equalsCloseAngleBracketToken = (EqualsCloseAngleBracketToken)lexerOutput.SyntaxTokenList[0];
        var endOfFileToken = (EndOfFileToken)lexerOutput.SyntaxTokenList[1];

        Assert.Equal(SyntaxKind.EqualsCloseAngleBracketToken, equalsCloseAngleBracketToken.SyntaxKind);
    }

    [Fact]
    public void LEX_EqualsToken()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "=";
        var lexerOutput = CSharpLexer.Lex(resourceUri, sourceText);

        Assert.Equal(2, lexerOutput.SyntaxTokenList.Count);
        var equalsToken = (EqualsToken)lexerOutput.SyntaxTokenList[0];
        var endOfFileToken = (EndOfFileToken)lexerOutput.SyntaxTokenList[1];

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
        var lexerOutput = CSharpLexer.Lex(resourceUri, sourceText);

        Assert.Equal(2, lexerOutput.SyntaxTokenList.Count);
        var identifierToken = (IdentifierToken)lexerOutput.SyntaxTokenList[0];
        var endOfFileToken = (EndOfFileToken)lexerOutput.SyntaxTokenList[1];

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
        var lexerOutput = CSharpLexer.Lex(resourceUri, sourceText);

        Assert.Equal(2, lexerOutput.SyntaxTokenList.Count);
        var memberAccessToken = (MemberAccessToken)lexerOutput.SyntaxTokenList[0];
        var endOfFileToken = (EndOfFileToken)lexerOutput.SyntaxTokenList[1];

        Assert.Equal(SyntaxKind.MemberAccessToken, memberAccessToken.SyntaxKind);
    }

    [Fact]
    public void LEX_MinusMinusToken()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "--";
        var lexerOutput = CSharpLexer.Lex(resourceUri, sourceText);

        Assert.Equal(2, lexerOutput.SyntaxTokenList.Count);
        var minusMinusToken = (MinusMinusToken)lexerOutput.SyntaxTokenList[0];
        var endOfFileToken = (EndOfFileToken)lexerOutput.SyntaxTokenList[1];

        Assert.Equal(SyntaxKind.MinusMinusToken, minusMinusToken.SyntaxKind);
    }

    [Fact]
    public void LEX_MinusToken()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "-";
        var lexerOutput = CSharpLexer.Lex(resourceUri, sourceText);

        Assert.Equal(2, lexerOutput.SyntaxTokenList.Count);
        var minusToken = (MinusToken)lexerOutput.SyntaxTokenList[0];
        var endOfFileToken = (EndOfFileToken)lexerOutput.SyntaxTokenList[1];

        Assert.Equal(SyntaxKind.MinusToken, minusToken.SyntaxKind);
    }

    [Fact]
    public void LEX_NumericLiteralToken()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "2";
        var lexerOutput = CSharpLexer.Lex(resourceUri, sourceText);

        Assert.Equal(2, lexerOutput.SyntaxTokenList.Count);
        var numericLiteralToken = (NumericLiteralToken)lexerOutput.SyntaxTokenList[0];
        var endOfFileToken = (EndOfFileToken)lexerOutput.SyntaxTokenList[1];

        Assert.Equal(SyntaxKind.NumericLiteralToken, numericLiteralToken.SyntaxKind);
    }

    [Fact]
    public void LEX_OpenAngleBracketToken()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "<";
        var lexerOutput = CSharpLexer.Lex(resourceUri, sourceText);

        Assert.Equal(2, lexerOutput.SyntaxTokenList.Count);
        var openAngleBracketToken = (OpenAngleBracketToken)lexerOutput.SyntaxTokenList[0];
        var endOfFileToken = (EndOfFileToken)lexerOutput.SyntaxTokenList[1];

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
        var lexerOutput = CSharpLexer.Lex(resourceUri, sourceText);

        Assert.Equal(2, lexerOutput.SyntaxTokenList.Count);
        var openBraceToken = (OpenBraceToken)lexerOutput.SyntaxTokenList[0];
        var endOfFileToken = (EndOfFileToken)lexerOutput.SyntaxTokenList[1];

        Assert.Equal(SyntaxKind.OpenBraceToken, openBraceToken.SyntaxKind);
    }

    [Fact]
    public void LEX_OpenParenthesisToken()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "(";
        var lexerOutput = CSharpLexer.Lex(resourceUri, sourceText);

        Assert.Equal(2, lexerOutput.SyntaxTokenList.Count);
        var openParenthesisToken = (OpenParenthesisToken)lexerOutput.SyntaxTokenList[0];
        var endOfFileToken = (EndOfFileToken)lexerOutput.SyntaxTokenList[1];

        Assert.Equal(SyntaxKind.OpenParenthesisToken, openParenthesisToken.SyntaxKind);
    }

    [Fact]
    public void LEX_OpenSquareBracketToken()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "[";
        var lexerOutput = CSharpLexer.Lex(resourceUri, sourceText);

        Assert.Equal(2, lexerOutput.SyntaxTokenList.Count);
        var openSquareBracketToken = (OpenSquareBracketToken)lexerOutput.SyntaxTokenList[0];
        var endOfFileToken = (EndOfFileToken)lexerOutput.SyntaxTokenList[1];

        Assert.Equal(SyntaxKind.OpenSquareBracketToken, openSquareBracketToken.SyntaxKind);
    }

    [Fact]
    public void LEX_PlusPlusToken()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "++";
        var lexerOutput = CSharpLexer.Lex(resourceUri, sourceText);

        Assert.Equal(2, lexerOutput.SyntaxTokenList.Count);
        var plusPlusToken = (PlusPlusToken)lexerOutput.SyntaxTokenList[0];
        var endOfFileToken = (EndOfFileToken)lexerOutput.SyntaxTokenList[1];

        Assert.Equal(SyntaxKind.PlusPlusToken, plusPlusToken.SyntaxKind);
    }

    [Fact]
    public void LEX_PlusToken()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "+";
        var lexerOutput = CSharpLexer.Lex(resourceUri, sourceText);

        Assert.Equal(2, lexerOutput.SyntaxTokenList.Count);
        var plusToken = (PlusToken)lexerOutput.SyntaxTokenList[0];
        var endOfFileToken = (EndOfFileToken)lexerOutput.SyntaxTokenList[1];

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
        var lexerOutput = CSharpLexer.Lex(resourceUri, sourceText);

        Assert.Equal(2, lexerOutput.SyntaxTokenList.Count);
        var questionMarkQuestionMarkToken = (QuestionMarkQuestionMarkToken)lexerOutput.SyntaxTokenList[0];
        var endOfFileToken = (EndOfFileToken)lexerOutput.SyntaxTokenList[1];

        Assert.Equal(SyntaxKind.QuestionMarkQuestionMarkToken, questionMarkQuestionMarkToken.SyntaxKind);
    }

    [Fact]
    public void LEX_QuestionMarkToken()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "?";
        var lexerOutput = CSharpLexer.Lex(resourceUri, sourceText);

        Assert.Equal(2, lexerOutput.SyntaxTokenList.Count);
        var questionMarkToken = (QuestionMarkToken)lexerOutput.SyntaxTokenList[0];
        var endOfFileToken = (EndOfFileToken)lexerOutput.SyntaxTokenList[1];

        Assert.Equal(SyntaxKind.QuestionMarkToken, questionMarkToken.SyntaxKind);
    }

    [Fact]
    public void LEX_StarToken()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "*";
        var lexerOutput = CSharpLexer.Lex(resourceUri, sourceText);

        Assert.Equal(2, lexerOutput.SyntaxTokenList.Count);
        var starToken = (StarToken)lexerOutput.SyntaxTokenList[0];
        var endOfFileToken = (EndOfFileToken)lexerOutput.SyntaxTokenList[1];

        Assert.Equal(SyntaxKind.StarToken, starToken.SyntaxKind);
    }

    [Fact]
    public void LEX_StatementDelimiterToken()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = ";";
        var lexerOutput = CSharpLexer.Lex(resourceUri, sourceText);

        Assert.Equal(2, lexerOutput.SyntaxTokenList.Count);
        var statementDelimiterToken = (StatementDelimiterToken)lexerOutput.SyntaxTokenList[0];
        var endOfFileToken = (EndOfFileToken)lexerOutput.SyntaxTokenList[1];

        Assert.Equal(SyntaxKind.StatementDelimiterToken, statementDelimiterToken.SyntaxKind);
    }

    [Fact]
    public void LEX_StringLiteralToken()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "\"Hello World!\"";
        var lexerOutput = CSharpLexer.Lex(resourceUri, sourceText);

        Assert.Equal(2, lexerOutput.SyntaxTokenList.Count);
        var stringLiteralToken = (StringLiteralToken)lexerOutput.SyntaxTokenList[0];
        var endOfFileToken = (EndOfFileToken)lexerOutput.SyntaxTokenList[1];

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
        var lexerOutput = CSharpLexer.Lex(resourceUri, sourceText);

        throw new NotImplementedException();
    }

    [Fact]
    public void LEX_ClassDefinition()
    {
        var sourceText = @"public class MyClass
{
}".ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri("UnitTests");
        var lexerOutput = CSharpLexer.Lex(resourceUri, sourceText);

        // Tokens: 'public' 'class' 'MyClass' '{' '}' 'EndOfFileToken'
        Assert.Equal(6, lexerOutput.SyntaxTokenList.Count);
    }
    
    private void WriteChildrenIndented(ISyntaxNode node, string name = "node")
    {
    	Console.WriteLine($"foreach (var child in {name}.GetChildList())");
		foreach (var child in node.GetChildList())
		{
			Console.WriteLine("\t" + child.SyntaxKind);
		}
		Console.WriteLine();
    }
    
    private void WriteChildrenIndentedRecursive(ISyntaxNode node, string name = "node", int indentation = 0)
    {
    	var indentationStringBuilder = new StringBuilder();
    	for (int i = 0; i < indentation; i++)
    		indentationStringBuilder.Append('\t');
    	
    	Console.WriteLine($"{indentationStringBuilder.ToString()}{node.SyntaxKind}");
    	
    	// For the child tokens
    	indentationStringBuilder.Append('\t');
    	var childIndentation = indentationStringBuilder.ToString();
    	
		foreach (var child in node.GetChildList())
		{
			if (child is ISyntaxNode syntaxNode)
			{
				WriteChildrenIndentedRecursive(syntaxNode, "node", indentation + 1);
			}
			else if (child is ISyntaxToken syntaxToken)
			{
				Console.WriteLine($"{childIndentation}{child.SyntaxKind}__{syntaxToken.TextSpan.GetText()}");
			}
		}
		
		if (indentation == 0)
			Console.WriteLine();
    }
}
