namespace Luthetus.Ide.Tests.Basics.CompilerServices.Languages.CSharp.LexerCase;

public partial class LexerTests
{
    [Fact]
    public void SHOULD_LEX_NUMERIC_LITERAL_TOKEN()
    {
        var numericValue = 4135;
        var sourceText = $"{numericValue}".ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new CSharpLexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var numericLiteralToken = lexer.SyntaxTokens.First();

        Assert.Equal(SyntaxKind.NumericLiteralToken, numericLiteralToken.SyntaxKind);

        var text = numericLiteralToken.TextSpan.GetText();
        Assert.Equal(numericValue, int.Parse(text));
    }

    [Fact]
    public void SHOULD_LEX_STRING_LITERAL_TOKEN()
    {
        var stringValue = "\"Apple Sauce\"";
        var sourceText = $"{stringValue}".ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new CSharpLexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var stringLiteralToken = lexer.SyntaxTokens.First();

        Assert.Equal(SyntaxKind.StringLiteralToken, stringLiteralToken.SyntaxKind);

        var text = stringLiteralToken.TextSpan.GetText();
        Assert.Equal(stringValue, text);
    }
}
