using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.CompilerServices.CSharp.LexerCase;

namespace Luthetus.CompilerServices.CSharp.Tests.SmokeTests.Lexers;

public class LexerTests
{
	public class Test
	{
		public Test(string sourceText)
		{
			SourceText = sourceText;
			ResourceUri = new ResourceUri("./unitTesting.txt");
			Lexer = new CSharpLexer(ResourceUri, SourceText);
	        Lexer.Lex();
	        Parser = new CSharpParser(Lexer);
	        CompilationUnit = Parser.Parse();
		}
		
		public string SourceText { get; set; }
		public ResourceUri ResourceUri { get; set; }
		public CSharpLexer Lexer { get; set; }
		public CSharpParser Parser { get; set; }
		public CompilationUnit CompilationUnit { get; set; }
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
".ReplaceLineEndings("\n");
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
    public void LEX_BangToken()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "!";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();

        Assert.Equal(2, lexer.SyntaxTokenList.Length);
        var bangToken = (BangToken)lexer.SyntaxTokenList[0];
        var endOfFileToken = (EndOfFileToken)lexer.SyntaxTokenList[1];

        Assert.Equal(SyntaxKind.BangToken, bangToken.SyntaxKind);
    }

    [Fact]
    public void LEX_CloseAngleBracketToken()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = ">";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();

        Assert.Equal(2, lexer.SyntaxTokenList.Length);
        var closeAngleBracketToken = (CloseAngleBracketToken)lexer.SyntaxTokenList[0];
        var endOfFileToken = (EndOfFileToken)lexer.SyntaxTokenList[1];

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

        Assert.Equal(2, lexer.SyntaxTokenList.Length);
        var closeBraceToken = (CloseBraceToken)lexer.SyntaxTokenList[0];
        var endOfFileToken = (EndOfFileToken)lexer.SyntaxTokenList[1];

        Assert.Equal(SyntaxKind.CloseBraceToken, closeBraceToken.SyntaxKind);
    }

    [Fact]
    public void LEX_CloseParenthesisToken()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = ")";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();

        Assert.Equal(2, lexer.SyntaxTokenList.Length);
        var closeParenthesisToken = (CloseParenthesisToken)lexer.SyntaxTokenList[0];
        var endOfFileToken = (EndOfFileToken)lexer.SyntaxTokenList[1];

        Assert.Equal(SyntaxKind.CloseParenthesisToken, closeParenthesisToken.SyntaxKind);
    }

    [Fact]
    public void LEX_CloseSquareBracketToken()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "]";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();

        Assert.Equal(2, lexer.SyntaxTokenList.Length);
        var closeSquareBracketToken = (CloseSquareBracketToken)lexer.SyntaxTokenList[0];
        var endOfFileToken = (EndOfFileToken)lexer.SyntaxTokenList[1];

        Assert.Equal(SyntaxKind.CloseSquareBracketToken, closeSquareBracketToken.SyntaxKind);
    }

    [Fact]
    public void LEX_ColonToken()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = ":";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();

        Assert.Equal(2, lexer.SyntaxTokenList.Length);
        var colonToken = (ColonToken)lexer.SyntaxTokenList[0];
        var endOfFileToken = (EndOfFileToken)lexer.SyntaxTokenList[1];

        Assert.Equal(SyntaxKind.ColonToken, colonToken.SyntaxKind);
    }

    [Fact]
    public void LEX_CommaToken()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = ",";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();

        Assert.Equal(2, lexer.SyntaxTokenList.Length);
        var commaToken = (CommaToken)lexer.SyntaxTokenList[0];
        var endOfFileToken = (EndOfFileToken)lexer.SyntaxTokenList[1];

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

        Assert.Equal(2, lexer.SyntaxTokenList.Length);
        var dollarSignToken = (DollarSignToken)lexer.SyntaxTokenList[0];
        var endOfFileToken = (EndOfFileToken)lexer.SyntaxTokenList[1];

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

        Assert.Equal(2, lexer.SyntaxTokenList.Length);
        var equalsEqualsToken = (EqualsEqualsToken)lexer.SyntaxTokenList[0];
        var endOfFileToken = (EndOfFileToken)lexer.SyntaxTokenList[1];

        Assert.Equal(SyntaxKind.EqualsEqualsToken, equalsEqualsToken.SyntaxKind);
    }

    [Fact]
    public void LEX_EqualsToken()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "=";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();

        Assert.Equal(2, lexer.SyntaxTokenList.Length);
        var equalsToken = (EqualsToken)lexer.SyntaxTokenList[0];
        var endOfFileToken = (EndOfFileToken)lexer.SyntaxTokenList[1];

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

        Assert.Equal(2, lexer.SyntaxTokenList.Length);
        var identifierToken = (IdentifierToken)lexer.SyntaxTokenList[0];
        var endOfFileToken = (EndOfFileToken)lexer.SyntaxTokenList[1];

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

        Assert.Equal(2, lexer.SyntaxTokenList.Length);
        var memberAccessToken = (MemberAccessToken)lexer.SyntaxTokenList[0];
        var endOfFileToken = (EndOfFileToken)lexer.SyntaxTokenList[1];

        Assert.Equal(SyntaxKind.MemberAccessToken, memberAccessToken.SyntaxKind);
    }

    [Fact]
    public void LEX_MinusMinusToken()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "--";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();

        Assert.Equal(2, lexer.SyntaxTokenList.Length);
        var minusMinusToken = (MinusMinusToken)lexer.SyntaxTokenList[0];
        var endOfFileToken = (EndOfFileToken)lexer.SyntaxTokenList[1];

        Assert.Equal(SyntaxKind.MinusMinusToken, minusMinusToken.SyntaxKind);
    }

    [Fact]
    public void LEX_MinusToken()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "-";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();

        Assert.Equal(2, lexer.SyntaxTokenList.Length);
        var minusToken = (MinusToken)lexer.SyntaxTokenList[0];
        var endOfFileToken = (EndOfFileToken)lexer.SyntaxTokenList[1];

        Assert.Equal(SyntaxKind.MinusToken, minusToken.SyntaxKind);
    }

    [Fact]
    public void LEX_NumericLiteralToken()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "2";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();

        Assert.Equal(2, lexer.SyntaxTokenList.Length);
        var numericLiteralToken = (NumericLiteralToken)lexer.SyntaxTokenList[0];
        var endOfFileToken = (EndOfFileToken)lexer.SyntaxTokenList[1];

        Assert.Equal(SyntaxKind.NumericLiteralToken, numericLiteralToken.SyntaxKind);
    }

    [Fact]
    public void LEX_OpenAngleBracketToken()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "<";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();

        Assert.Equal(2, lexer.SyntaxTokenList.Length);
        var openAngleBracketToken = (OpenAngleBracketToken)lexer.SyntaxTokenList[0];
        var endOfFileToken = (EndOfFileToken)lexer.SyntaxTokenList[1];

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

        Assert.Equal(2, lexer.SyntaxTokenList.Length);
        var openBraceToken = (OpenBraceToken)lexer.SyntaxTokenList[0];
        var endOfFileToken = (EndOfFileToken)lexer.SyntaxTokenList[1];

        Assert.Equal(SyntaxKind.OpenBraceToken, openBraceToken.SyntaxKind);
    }

    [Fact]
    public void LEX_OpenParenthesisToken()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "(";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();

        Assert.Equal(2, lexer.SyntaxTokenList.Length);
        var openParenthesisToken = (OpenParenthesisToken)lexer.SyntaxTokenList[0];
        var endOfFileToken = (EndOfFileToken)lexer.SyntaxTokenList[1];

        Assert.Equal(SyntaxKind.OpenParenthesisToken, openParenthesisToken.SyntaxKind);
    }

    [Fact]
    public void LEX_OpenSquareBracketToken()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "[";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();

        Assert.Equal(2, lexer.SyntaxTokenList.Length);
        var openSquareBracketToken = (OpenSquareBracketToken)lexer.SyntaxTokenList[0];
        var endOfFileToken = (EndOfFileToken)lexer.SyntaxTokenList[1];

        Assert.Equal(SyntaxKind.OpenSquareBracketToken, openSquareBracketToken.SyntaxKind);
    }

    [Fact]
    public void LEX_PlusPlusToken()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "++";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();

        Assert.Equal(2, lexer.SyntaxTokenList.Length);
        var plusPlusToken = (PlusPlusToken)lexer.SyntaxTokenList[0];
        var endOfFileToken = (EndOfFileToken)lexer.SyntaxTokenList[1];

        Assert.Equal(SyntaxKind.PlusPlusToken, plusPlusToken.SyntaxKind);
    }

    [Fact]
    public void LEX_PlusToken()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "+";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();

        Assert.Equal(2, lexer.SyntaxTokenList.Length);
        var plusToken = (PlusToken)lexer.SyntaxTokenList[0];
        var endOfFileToken = (EndOfFileToken)lexer.SyntaxTokenList[1];

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

        Assert.Equal(2, lexer.SyntaxTokenList.Length);
        var questionMarkQuestionMarkToken = (QuestionMarkQuestionMarkToken)lexer.SyntaxTokenList[0];
        var endOfFileToken = (EndOfFileToken)lexer.SyntaxTokenList[1];

        Assert.Equal(SyntaxKind.QuestionMarkQuestionMarkToken, questionMarkQuestionMarkToken.SyntaxKind);
    }

    [Fact]
    public void LEX_QuestionMarkToken()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "?";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();

        Assert.Equal(2, lexer.SyntaxTokenList.Length);
        var questionMarkToken = (QuestionMarkToken)lexer.SyntaxTokenList[0];
        var endOfFileToken = (EndOfFileToken)lexer.SyntaxTokenList[1];

        Assert.Equal(SyntaxKind.QuestionMarkToken, questionMarkToken.SyntaxKind);
    }

    [Fact]
    public void LEX_StarToken()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "*";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();

        Assert.Equal(2, lexer.SyntaxTokenList.Length);
        var starToken = (StarToken)lexer.SyntaxTokenList[0];
        var endOfFileToken = (EndOfFileToken)lexer.SyntaxTokenList[1];

        Assert.Equal(SyntaxKind.StarToken, starToken.SyntaxKind);
    }

    [Fact]
    public void LEX_StatementDelimiterToken()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = ";";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();

        Assert.Equal(2, lexer.SyntaxTokenList.Length);
        var statementDelimiterToken = (StatementDelimiterToken)lexer.SyntaxTokenList[0];
        var endOfFileToken = (EndOfFileToken)lexer.SyntaxTokenList[1];

        Assert.Equal(SyntaxKind.StatementDelimiterToken, statementDelimiterToken.SyntaxKind);
    }

    [Fact]
    public void LEX_StringLiteralToken()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "\"Hello World!\"";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();

        Assert.Equal(2, lexer.SyntaxTokenList.Length);
        var stringLiteralToken = (StringLiteralToken)lexer.SyntaxTokenList[0];
        var endOfFileToken = (EndOfFileToken)lexer.SyntaxTokenList[1];

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
        Assert.Equal(6, lexer.SyntaxTokenList.Length);
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
