using System.Text;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
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
			var lexerOutput = CSharpLexer.Lex(ResourceUri, SourceText);
	        CSharpParser.Parse(CompilationUnit, ref lexerOutput);
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
        Assert.Equal(SyntaxKind.PipeToken, lexerOutput.SyntaxTokenList[0].SyntaxKind);
        Assert.Equal(SyntaxKind.EndOfFileToken, lexerOutput.SyntaxTokenList[1].SyntaxKind);
    }

    [Fact]
    public void LEX_PipePipeToken()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "||";
        var lexerOutput = CSharpLexer.Lex(resourceUri, sourceText);

        Assert.Equal(2, lexerOutput.SyntaxTokenList.Count);
        Assert.Equal(SyntaxKind.PipePipeToken, lexerOutput.SyntaxTokenList[0].SyntaxKind);
        Assert.Equal(SyntaxKind.EndOfFileToken, lexerOutput.SyntaxTokenList[1].SyntaxKind);
    }
    
    [Fact]
    public void LEX_AmpersandToken()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "&";
        var lexerOutput = CSharpLexer.Lex(resourceUri, sourceText);

        Assert.Equal(2, lexerOutput.SyntaxTokenList.Count);
        Assert.Equal(SyntaxKind.AmpersandToken, lexerOutput.SyntaxTokenList[0].SyntaxKind);
        Assert.Equal(SyntaxKind.EndOfFileToken, lexerOutput.SyntaxTokenList[1].SyntaxKind);
    }

    [Fact]
    public void LEX_AmpersandAmpersandToken()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "&&";
        var lexerOutput = CSharpLexer.Lex(resourceUri, sourceText);

        Assert.Equal(2, lexerOutput.SyntaxTokenList.Count);
        Assert.Equal(SyntaxKind.AmpersandAmpersandToken, lexerOutput.SyntaxTokenList[0].SyntaxKind);
        Assert.Equal(SyntaxKind.EndOfFileToken, lexerOutput.SyntaxTokenList[1].SyntaxKind);
    }

    [Fact]
    public void LEX_BangToken()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "!";
        var lexerOutput = CSharpLexer.Lex(resourceUri, sourceText);

        Assert.Equal(2, lexerOutput.SyntaxTokenList.Count);
        Assert.Equal(SyntaxKind.BangToken, lexerOutput.SyntaxTokenList[0].SyntaxKind);
        Assert.Equal(SyntaxKind.EndOfFileToken, lexerOutput.SyntaxTokenList[1].SyntaxKind);
    }
    
    [Fact]
    public void LEX_BangEqualsToken()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "!=";
        var lexerOutput = CSharpLexer.Lex(resourceUri, sourceText);

        Assert.Equal(2, lexerOutput.SyntaxTokenList.Count);
        Assert.Equal(SyntaxKind.BangEqualsToken, lexerOutput.SyntaxTokenList[0].SyntaxKind);
        Assert.Equal(SyntaxKind.EndOfFileToken, lexerOutput.SyntaxTokenList[1].SyntaxKind);
    }
    
    [Fact]
    public void LEX_CloseAngleBracketEqualsToken()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = ">=";
        var lexerOutput = CSharpLexer.Lex(resourceUri, sourceText);

        Assert.Equal(2, lexerOutput.SyntaxTokenList.Count);
        Assert.Equal(SyntaxKind.CloseAngleBracketEqualsToken, lexerOutput.SyntaxTokenList[0].SyntaxKind);
        Assert.Equal(SyntaxKind.EndOfFileToken, lexerOutput.SyntaxTokenList[1].SyntaxKind);
    }
    
    [Fact]
    public void LEX_OpenAngleBracketEqualsToken()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "<=";
        var lexerOutput = CSharpLexer.Lex(resourceUri, sourceText);

        Assert.Equal(2, lexerOutput.SyntaxTokenList.Count);
        Assert.Equal(SyntaxKind.OpenAngleBracketEqualsToken, lexerOutput.SyntaxTokenList[0].SyntaxKind);
        Assert.Equal(SyntaxKind.EndOfFileToken, lexerOutput.SyntaxTokenList[1].SyntaxKind);
    }

    [Fact]
    public void LEX_CloseAngleBracketToken()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = ">";
        var lexerOutput = CSharpLexer.Lex(resourceUri, sourceText);

        Assert.Equal(2, lexerOutput.SyntaxTokenList.Count);
        Assert.Equal(SyntaxKind.CloseAngleBracketToken, lexerOutput.SyntaxTokenList[0].SyntaxKind);
        Assert.Equal(SyntaxKind.EndOfFileToken, lexerOutput.SyntaxTokenList[1].SyntaxKind);
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
        Assert.Equal(SyntaxKind.CloseBraceToken, lexerOutput.SyntaxTokenList[0].SyntaxKind);
        Assert.Equal(SyntaxKind.EndOfFileToken, lexerOutput.SyntaxTokenList[1].SyntaxKind);
    }

    [Fact]
    public void LEX_CloseParenthesisToken()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = ")";
        var lexerOutput = CSharpLexer.Lex(resourceUri, sourceText);

        Assert.Equal(2, lexerOutput.SyntaxTokenList.Count);
        Assert.Equal(SyntaxKind.CloseParenthesisToken, lexerOutput.SyntaxTokenList[0].SyntaxKind);
        Assert.Equal(SyntaxKind.EndOfFileToken, lexerOutput.SyntaxTokenList[1].SyntaxKind);
    }

    [Fact]
    public void LEX_CloseSquareBracketToken()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "]";
        var lexerOutput = CSharpLexer.Lex(resourceUri, sourceText);

        Assert.Equal(2, lexerOutput.SyntaxTokenList.Count);
        Assert.Equal(SyntaxKind.CloseSquareBracketToken, lexerOutput.SyntaxTokenList[0].SyntaxKind);
        Assert.Equal(SyntaxKind.EndOfFileToken, lexerOutput.SyntaxTokenList[1].SyntaxKind);
    }

    [Fact]
    public void LEX_ColonToken()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = ":";
        var lexerOutput = CSharpLexer.Lex(resourceUri, sourceText);

        Assert.Equal(2, lexerOutput.SyntaxTokenList.Count);
        Assert.Equal(SyntaxKind.ColonToken, lexerOutput.SyntaxTokenList[0].SyntaxKind);
        Assert.Equal(SyntaxKind.EndOfFileToken, lexerOutput.SyntaxTokenList[1].SyntaxKind);
    }

    [Fact]
    public void LEX_CommaToken()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = ",";
        var lexerOutput = CSharpLexer.Lex(resourceUri, sourceText);

        Assert.Equal(2, lexerOutput.SyntaxTokenList.Count);
        Assert.Equal(SyntaxKind.CommaToken, lexerOutput.SyntaxTokenList[0].SyntaxKind);
        Assert.Equal(SyntaxKind.EndOfFileToken, lexerOutput.SyntaxTokenList[1].SyntaxKind);
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
        Assert.Equal(SyntaxKind.DollarSignToken, lexerOutput.SyntaxTokenList[0].SyntaxKind);
        Assert.Equal(SyntaxKind.EndOfFileToken, lexerOutput.SyntaxTokenList[1].SyntaxKind);
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
        Assert.Equal(SyntaxKind.EqualsEqualsToken, lexerOutput.SyntaxTokenList[0].SyntaxKind);
        Assert.Equal(SyntaxKind.EndOfFileToken, lexerOutput.SyntaxTokenList[1].SyntaxKind);
    }
    
    [Fact]
    public void LEX_EqualsCloseAngleBracketToken()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "=>";
        var lexerOutput = CSharpLexer.Lex(resourceUri, sourceText);

        Assert.Equal(2, lexerOutput.SyntaxTokenList.Count);
        Assert.Equal(SyntaxKind.EqualsCloseAngleBracketToken, lexerOutput.SyntaxTokenList[0].SyntaxKind);
        Assert.Equal(SyntaxKind.EndOfFileToken, lexerOutput.SyntaxTokenList[1].SyntaxKind);
    }

    [Fact]
    public void LEX_EqualsToken()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "=";
        var lexerOutput = CSharpLexer.Lex(resourceUri, sourceText);

        Assert.Equal(2, lexerOutput.SyntaxTokenList.Count);
        Assert.Equal(SyntaxKind.EqualsToken, lexerOutput.SyntaxTokenList[0].SyntaxKind);
        Assert.Equal(SyntaxKind.EndOfFileToken, lexerOutput.SyntaxTokenList[1].SyntaxKind);
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
        Assert.Equal(SyntaxKind.IdentifierToken, lexerOutput.SyntaxTokenList[0].SyntaxKind);
        Assert.Equal(SyntaxKind.EndOfFileToken, lexerOutput.SyntaxTokenList[1].SyntaxKind);
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
        Assert.Equal(SyntaxKind.MemberAccessToken, lexerOutput.SyntaxTokenList[0].SyntaxKind);
        Assert.Equal(SyntaxKind.EndOfFileToken, lexerOutput.SyntaxTokenList[1].SyntaxKind);
    }

    [Fact]
    public void LEX_MinusMinusToken()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "--";
        var lexerOutput = CSharpLexer.Lex(resourceUri, sourceText);

        Assert.Equal(2, lexerOutput.SyntaxTokenList.Count);
        Assert.Equal(SyntaxKind.MinusMinusToken, lexerOutput.SyntaxTokenList[0].SyntaxKind);
        Assert.Equal(SyntaxKind.EndOfFileToken, lexerOutput.SyntaxTokenList[1].SyntaxKind);
    }

    [Fact]
    public void LEX_MinusToken()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "-";
        var lexerOutput = CSharpLexer.Lex(resourceUri, sourceText);

        Assert.Equal(2, lexerOutput.SyntaxTokenList.Count);
        Assert.Equal(SyntaxKind.MinusToken, lexerOutput.SyntaxTokenList[0].SyntaxKind);
        Assert.Equal(SyntaxKind.EndOfFileToken, lexerOutput.SyntaxTokenList[1].SyntaxKind);
    }

    [Fact]
    public void LEX_NumericLiteralToken()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "2";
        var lexerOutput = CSharpLexer.Lex(resourceUri, sourceText);

        Assert.Equal(2, lexerOutput.SyntaxTokenList.Count);
        Assert.Equal(SyntaxKind.NumericLiteralToken, lexerOutput.SyntaxTokenList[0].SyntaxKind);
        Assert.Equal(SyntaxKind.EndOfFileToken, lexerOutput.SyntaxTokenList[1].SyntaxKind);
    }

    [Fact]
    public void LEX_OpenAngleBracketToken()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "<";
        var lexerOutput = CSharpLexer.Lex(resourceUri, sourceText);

        Assert.Equal(2, lexerOutput.SyntaxTokenList.Count);
        Assert.Equal(SyntaxKind.OpenAngleBracketToken, lexerOutput.SyntaxTokenList[0].SyntaxKind);
        Assert.Equal(SyntaxKind.EndOfFileToken, lexerOutput.SyntaxTokenList[1].SyntaxKind);
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
        Assert.Equal(SyntaxKind.OpenBraceToken, lexerOutput.SyntaxTokenList[0].SyntaxKind);
        Assert.Equal(SyntaxKind.EndOfFileToken, lexerOutput.SyntaxTokenList[1].SyntaxKind);
    }

    [Fact]
    public void LEX_OpenParenthesisToken()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "(";
        var lexerOutput = CSharpLexer.Lex(resourceUri, sourceText);

        Assert.Equal(2, lexerOutput.SyntaxTokenList.Count);
        Assert.Equal(SyntaxKind.OpenParenthesisToken, lexerOutput.SyntaxTokenList[0].SyntaxKind);
        Assert.Equal(SyntaxKind.EndOfFileToken, lexerOutput.SyntaxTokenList[1].SyntaxKind);
    }

    [Fact]
    public void LEX_OpenSquareBracketToken()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "[";
        var lexerOutput = CSharpLexer.Lex(resourceUri, sourceText);

        Assert.Equal(2, lexerOutput.SyntaxTokenList.Count);
        Assert.Equal(SyntaxKind.OpenSquareBracketToken, lexerOutput.SyntaxTokenList[0].SyntaxKind);
        Assert.Equal(SyntaxKind.EndOfFileToken, lexerOutput.SyntaxTokenList[1].SyntaxKind);
    }

    [Fact]
    public void LEX_PlusPlusToken()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "++";
        var lexerOutput = CSharpLexer.Lex(resourceUri, sourceText);

        Assert.Equal(2, lexerOutput.SyntaxTokenList.Count);
        Assert.Equal(SyntaxKind.PlusPlusToken, lexerOutput.SyntaxTokenList[0].SyntaxKind);
        Assert.Equal(SyntaxKind.EndOfFileToken, lexerOutput.SyntaxTokenList[1].SyntaxKind);
    }

    [Fact]
    public void LEX_PlusToken()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "+";
        var lexerOutput = CSharpLexer.Lex(resourceUri, sourceText);

        Assert.Equal(2, lexerOutput.SyntaxTokenList.Count);
        Assert.Equal(SyntaxKind.PlusToken, lexerOutput.SyntaxTokenList[0].SyntaxKind);
        Assert.Equal(SyntaxKind.EndOfFileToken, lexerOutput.SyntaxTokenList[1].SyntaxKind);
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
        Assert.Equal(SyntaxKind.QuestionMarkQuestionMarkToken, lexerOutput.SyntaxTokenList[0].SyntaxKind);
        Assert.Equal(SyntaxKind.EndOfFileToken, lexerOutput.SyntaxTokenList[1].SyntaxKind);
    }

    [Fact]
    public void LEX_QuestionMarkToken()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "?";
        var lexerOutput = CSharpLexer.Lex(resourceUri, sourceText);

        Assert.Equal(2, lexerOutput.SyntaxTokenList.Count);
        Assert.Equal(SyntaxKind.QuestionMarkToken, lexerOutput.SyntaxTokenList[0].SyntaxKind);
        Assert.Equal(SyntaxKind.EndOfFileToken, lexerOutput.SyntaxTokenList[1].SyntaxKind);
    }

    [Fact]
    public void LEX_StarToken()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "*";
        var lexerOutput = CSharpLexer.Lex(resourceUri, sourceText);

        Assert.Equal(2, lexerOutput.SyntaxTokenList.Count);
        Assert.Equal(SyntaxKind.StarToken, lexerOutput.SyntaxTokenList[0].SyntaxKind);
        Assert.Equal(SyntaxKind.EndOfFileToken, lexerOutput.SyntaxTokenList[1].SyntaxKind);
    }

    [Fact]
    public void LEX_StatementDelimiterToken()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = ";";
        var lexerOutput = CSharpLexer.Lex(resourceUri, sourceText);

        Assert.Equal(2, lexerOutput.SyntaxTokenList.Count);
        Assert.Equal(SyntaxKind.StatementDelimiterToken, lexerOutput.SyntaxTokenList[0].SyntaxKind);
        Assert.Equal(SyntaxKind.EndOfFileToken, lexerOutput.SyntaxTokenList[1].SyntaxKind);
    }

    [Fact]
    public void LEX_StringLiteralToken()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "\"Hello World!\"";
        var lexerOutput = CSharpLexer.Lex(resourceUri, sourceText);

        Assert.Equal(2, lexerOutput.SyntaxTokenList.Count);
        Assert.Equal(SyntaxKind.StringLiteralToken, lexerOutput.SyntaxTokenList[0].SyntaxKind);
        Assert.Equal(SyntaxKind.EndOfFileToken, lexerOutput.SyntaxTokenList[1].SyntaxKind);
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
			else if (child is SyntaxToken syntaxToken)
			{
				Console.WriteLine($"{childIndentation}{child.SyntaxKind}__{syntaxToken.TextSpan.GetText()}");
			}
		}
		
		if (indentation == 0)
			Console.WriteLine();
    }
}
