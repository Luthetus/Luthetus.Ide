using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.LexerCase;
using Luthetus.TextEditor.RazorLib.Lexing;

namespace Luthetus.Ide.Tests.Basics.CompilerServices.Languages.CSharp.LexerCase;

public partial class LexerTests
{
    [Fact]
    public void SHOULD_LEX_COMMENT_SINGLE_LINE_TOKEN_WITH_ENDING_AS_END_OF_FILE()
    {
        var singleLineCommentAsString = @"// C:\Users\hunte\Repos\Aaa\";
        var sourceText = $"{singleLineCommentAsString}".ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var commentSingleLineToken = lexer.SyntaxTokens.First();

        Assert.Equal(SyntaxKind.CommentSingleLineToken, commentSingleLineToken.SyntaxKind);

        var text = commentSingleLineToken.TextSpan.GetText();
        Assert.Equal(singleLineCommentAsString, text);
    }

    [Fact]
    public void SHOULD_LEX_COMMENT_SINGLE_LINE_TOKEN_WITH_ENDING_AS_NEW_LINE()
    {
        var singleLineCommentAsString = @"// C:\Users\hunte\Repos\Aaa\";
        var sourceText = $@"{singleLineCommentAsString}
".ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var commentSingleLineToken = lexer.SyntaxTokens.First();

        Assert.Equal(SyntaxKind.CommentSingleLineToken, commentSingleLineToken.SyntaxKind);

        var text = commentSingleLineToken.TextSpan.GetText();
        Assert.Equal(singleLineCommentAsString, text);
    }

    [Fact]
    public void SHOULD_LEX_COMMENT_MULTI_LINE_TOKEN_WRITTEN_ON_MULTIPLE_LINES()
    {
        var multiLineCommentAsString = @"/*
	A Multi-Line Comment
*/".ReplaceLineEndings("\n");

        var sourceText = $"{multiLineCommentAsString}".ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var commentMultiLineToken = lexer.SyntaxTokens.First();

        Assert.Equal(SyntaxKind.CommentMultiLineToken, commentMultiLineToken.SyntaxKind);

        var text = commentMultiLineToken.TextSpan.GetText();
        Assert.Equal(multiLineCommentAsString, text);
    }

    [Fact]
    public void SHOULD_LEX_COMMENT_MULTI_LINE_TOKEN_WRITTEN_ON_SINGLE_LINE()
    {
        var multiLineCommentAsString = @"/* Another Multi-Line Comment */"
            .ReplaceLineEndings("\n");

        var sourceText = $"{multiLineCommentAsString}".ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var commentMultiLineToken = lexer.SyntaxTokens.First();

        Assert.Equal(SyntaxKind.CommentMultiLineToken, commentMultiLineToken.SyntaxKind);

        var text = commentMultiLineToken.TextSpan.GetText();
        Assert.Equal(multiLineCommentAsString, text);
    }
}
