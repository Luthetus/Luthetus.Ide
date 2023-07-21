namespace Luthetus.Ide.Tests.Basics.CompilerServices.Languages.Razor;

public class LexerTests
{
    [Fact]
    public void Aaa()
    {
        var singleLineCommentAsString = @"// C:\Users\hunte\Repos\Aaa\";
        var sourceText = $"{singleLineCommentAsString}".ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new CSharpLexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var commentSingleLineToken = lexer.SyntaxTokens.First();

        Assert.Equal(SyntaxKind.CommentSingleLineToken, commentSingleLineToken.SyntaxKind);

        var text = commentSingleLineToken.TextSpan.GetText();
        Assert.Equal(singleLineCommentAsString, text);
    }
}
