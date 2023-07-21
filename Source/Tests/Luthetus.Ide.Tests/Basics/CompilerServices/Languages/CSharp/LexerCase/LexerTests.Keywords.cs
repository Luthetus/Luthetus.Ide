namespace Luthetus.Ide.Tests.Basics.CompilerServices.Languages.CSharp.LexerCase;

public partial class LexerTests
{
    [Fact]
    public void SHOULD_LEX_KEYWORD_TOKEN()
    {
        var keywordAsString = "int";
        var sourceText = $"{keywordAsString}".ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new CSharpLexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var keywordToken = lexer.SyntaxTokens.First();

        Assert.Equal(SyntaxKind.KeywordToken, keywordToken.SyntaxKind);

        var text = keywordToken.TextSpan.GetText();
        Assert.Equal(keywordAsString, text);
    }
}
