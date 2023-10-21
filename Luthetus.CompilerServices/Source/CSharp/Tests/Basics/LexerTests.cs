using Luthetus.CompilerServices.Lang.CSharp.LexerCase;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.CompilerServices.Lang.CSharp.Tests.Basics;

public partial class LexerTests
{
    [Fact]
    public void LEX_PlusToken()
    {
        var plusTokenAsString = "+";
        var sourceText = $"{plusTokenAsString}".ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new CSharpLexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var plusToken = lexer.SyntaxTokens.First();

        Assert.Equal(SyntaxKind.PlusToken, plusToken.SyntaxKind);

        var text = plusToken.TextSpan.GetText();
        Assert.Equal(plusTokenAsString, text);
    }

    [Fact]
    public void LEX_EqualsToken()
    {
        var equalsTokenAsString = "=";
        var sourceText = $"{equalsTokenAsString}".ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new CSharpLexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var equalsToken = lexer.SyntaxTokens.First();

        Assert.Equal(SyntaxKind.EqualsToken, equalsToken.SyntaxKind);

        var text = equalsToken.TextSpan.GetText();
        Assert.Equal(equalsTokenAsString, text);
    }
    
    [Fact]
    public void LEX_StarToken()
    {
        var starTokenAsString = "*";
        var sourceText = $"{starTokenAsString}".ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new CSharpLexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var starToken = lexer.SyntaxTokens.First();

        Assert.Equal(SyntaxKind.StarToken, starToken.SyntaxKind);

        var text = starToken.TextSpan.GetText();
        Assert.Equal(starTokenAsString, text);
    }
    
    [Fact]
    public void LEX_DivisionToken()
    {
        var divisionTokenAsString = "/";
        var sourceText = $"{divisionTokenAsString}".ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new CSharpLexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var divisionToken = lexer.SyntaxTokens.First();

        Assert.Equal(SyntaxKind.DivisionToken, divisionToken.SyntaxKind);

        var text = divisionToken.TextSpan.GetText();
        Assert.Equal(divisionTokenAsString, text);
    }

    [Fact]
    public void LEX_StatementDelimiterToken()
    {
        var statementDelimiterTokenAsString = ";";
        var sourceText = $"{statementDelimiterTokenAsString}".ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new CSharpLexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var statementDelimiterToken = lexer.SyntaxTokens.First();

        Assert.Equal(SyntaxKind.StatementDelimiterToken, statementDelimiterToken.SyntaxKind);

        var text = statementDelimiterToken.TextSpan.GetText();
        Assert.Equal(statementDelimiterTokenAsString, text);
    }

    [Fact]
    public void LEX_OpenParenthesisToken()
    {
        var openParenthesisTokenAsString = "(";
        var sourceText = $"{openParenthesisTokenAsString}".ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new CSharpLexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var openParenthesisToken = lexer.SyntaxTokens.First();

        Assert.Equal(SyntaxKind.OpenParenthesisToken, openParenthesisToken.SyntaxKind);

        var text = openParenthesisToken.TextSpan.GetText();
        Assert.Equal(openParenthesisTokenAsString, text);
    }

    [Fact]
    public void LEX_CloseParenthesisToken()
    {
        var closeParenthesisTokenAsString = ")";
        var sourceText = $"{closeParenthesisTokenAsString}".ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new CSharpLexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var closeParenthesisToken = lexer.SyntaxTokens.First();

        Assert.Equal(SyntaxKind.CloseParenthesisToken, closeParenthesisToken.SyntaxKind);

        var text = closeParenthesisToken.TextSpan.GetText();
        Assert.Equal(closeParenthesisTokenAsString, text);
    }

    [Fact]
    public void LEX_OpenBraceToken()
    {
        var openBraceTokenAsString = "{";
        var sourceText = $"{openBraceTokenAsString}".ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new CSharpLexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var openBraceToken = lexer.SyntaxTokens.First();

        Assert.Equal(SyntaxKind.OpenBraceToken, openBraceToken.SyntaxKind);

        var text = openBraceToken.TextSpan.GetText();
        Assert.Equal(openBraceTokenAsString, text);
    }

    [Fact]
    public void LEX_CloseBraceToken()
    {
        var closeBraceTokenAsString = "}";
        var sourceText = $"{closeBraceTokenAsString}".ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new CSharpLexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var closeBraceToken = lexer.SyntaxTokens.First();

        Assert.Equal(SyntaxKind.CloseBraceToken, closeBraceToken.SyntaxKind);

        var text = closeBraceToken.TextSpan.GetText();
        Assert.Equal(closeBraceTokenAsString, text);
    }

    [Fact]
    public void LEX_ColonToken()
    {
        var colonTokenAsString = ":";
        var sourceText = $"{colonTokenAsString}".ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new CSharpLexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var colonToken = lexer.SyntaxTokens.First();

        Assert.Equal(SyntaxKind.ColonToken, colonToken.SyntaxKind);

        var text = colonToken.TextSpan.GetText();
        Assert.Equal(colonTokenAsString, text);
    }

    [Fact]
    public void LEX_MemberAccessToken()
    {
        var memberAccessTokenAsString = ".";
        var sourceText = $"{memberAccessTokenAsString}".ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new CSharpLexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var memberAccessToken = lexer.SyntaxTokens.First();

        Assert.Equal(SyntaxKind.MemberAccessToken, memberAccessToken.SyntaxKind);

        var text = memberAccessToken.TextSpan.GetText();
        Assert.Equal(memberAccessTokenAsString, text);
    }

    [Fact]
    public void LEX_CommaToken()
    {
        var commaTokenAsString = ",";
        var sourceText = $"{commaTokenAsString}".ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new CSharpLexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var commaToken = lexer.SyntaxTokens.First();

        Assert.Equal(SyntaxKind.CommaToken, commaToken.SyntaxKind);

        var text = commaToken.TextSpan.GetText();
        Assert.Equal(commaTokenAsString, text);
    }

    [Fact]
    public void LEX_BangToken()
    {
        var bangTokenAsString = "!";
        var sourceText = $"{bangTokenAsString}".ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new CSharpLexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var bangToken = lexer.SyntaxTokens.First();

        Assert.Equal(SyntaxKind.BangToken, bangToken.SyntaxKind);

        var text = bangToken.TextSpan.GetText();
        Assert.Equal(bangTokenAsString, text);
    }

    [Fact]
    public void LEX_QuestionMarkToken()
    {
        var questionMarkTokenAsString = "?";
        var sourceText = $"{questionMarkTokenAsString}".ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new CSharpLexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var questionMarkToken = lexer.SyntaxTokens.First();

        Assert.Equal(SyntaxKind.QuestionMarkToken, questionMarkToken.SyntaxKind);

        var text = questionMarkToken.TextSpan.GetText();
        Assert.Equal(questionMarkTokenAsString, text);
    }

    [Fact]
    public void LEX_QuestionMarkQuestionMarkToken()
    {
        var questionMarkQuestionMarkTokenAsString = "??";
        var sourceText = $"{questionMarkQuestionMarkTokenAsString}".ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new CSharpLexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var questionMarkQuestionMarkToken = lexer.SyntaxTokens.First();

        Assert.Equal(SyntaxKind.QuestionMarkQuestionMarkToken, questionMarkQuestionMarkToken.SyntaxKind);

        var text = questionMarkQuestionMarkToken.TextSpan.GetText();
        Assert.Equal(questionMarkQuestionMarkTokenAsString, text);
    }

    [Fact]
    public void LEX_OpenAngleBracketToken()
    {
        var openAngleBracketTokenAsString = "<";
        var sourceText = $"{openAngleBracketTokenAsString}".ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new CSharpLexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var openAngleBracketToken = lexer.SyntaxTokens.First();

        Assert.Equal(SyntaxKind.OpenAngleBracketToken, openAngleBracketToken.SyntaxKind);

        var text = openAngleBracketToken.TextSpan.GetText();
        Assert.Equal(openAngleBracketTokenAsString, text);
    }

    [Fact]
    public void LEX_CloseAngleBracketToken()
    {
        var closeAngleBracketTokenAsString = ">";
        var sourceText = $"{closeAngleBracketTokenAsString}".ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new CSharpLexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var closeAngleBracketToken = lexer.SyntaxTokens.First();

        Assert.Equal(SyntaxKind.CloseAngleBracketToken, closeAngleBracketToken.SyntaxKind);

        var text = closeAngleBracketToken.TextSpan.GetText();
        Assert.Equal(closeAngleBracketTokenAsString, text);
    }

    [Fact]
    public void LEX_DollarSignToken()
    {
        var dollarSignTokenAsString = "$";
        var sourceText = $"{dollarSignTokenAsString}".ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new CSharpLexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var dollarSignToken = lexer.SyntaxTokens.First();

        Assert.Equal(SyntaxKind.DollarSignToken, dollarSignToken.SyntaxKind);

        var text = dollarSignToken.TextSpan.GetText();
        Assert.Equal(dollarSignTokenAsString, text);
    }

    [Fact]
    public void LEX_OpenSquareBracketToken()
    {
        var openSquareBracketTokenAsString = "[";
        var sourceText = $"{openSquareBracketTokenAsString}".ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new CSharpLexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var openSquareBracketToken = lexer.SyntaxTokens.First();

        Assert.Equal(SyntaxKind.OpenSquareBracketToken, openSquareBracketToken.SyntaxKind);

        var text = openSquareBracketToken.TextSpan.GetText();
        Assert.Equal(openSquareBracketTokenAsString, text);
    }

    [Fact]
    public void LEX_CloseSquareBracketToken()
    {
        var closeSquareBracketTokenAsString = "]";
        var sourceText = $"{closeSquareBracketTokenAsString}".ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new CSharpLexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var closeSquareBracketToken = lexer.SyntaxTokens.First();

        Assert.Equal(SyntaxKind.CloseSquareBracketToken, closeSquareBracketToken.SyntaxKind);

        var text = closeSquareBracketToken.TextSpan.GetText();
        Assert.Equal(closeSquareBracketTokenAsString, text);
    }

    [Fact]
    public void LEX_PreprocessorDirectiveToken()
    {
        var preprocessorDirectiveAsString = "#region regionIdentifierHere";
        var sourceText = $"{preprocessorDirectiveAsString}".ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new CSharpLexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var preprocessorDirectiveToken = lexer.SyntaxTokens.First();

        Assert.Equal(SyntaxKind.PreprocessorDirectiveToken, preprocessorDirectiveToken.SyntaxKind);

        var text = preprocessorDirectiveToken.TextSpan.GetText();
        Assert.Equal(preprocessorDirectiveAsString, text);
    }

    [Fact]
    public void LEX_NumericLiteralToken()
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
    public void LEX_StringLiteralToken()
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

    [Theory]
    /* Lowercase Letters */
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
    /* Uppercase Letters */
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
    /* Underscore */
    [InlineData("_")]
    /* Misc */
    [InlineData("abc")]
    [InlineData("aBc")]
    [InlineData("Abc")]
    [InlineData("ABc")]
    [InlineData("_a")]
    [InlineData("_A")]
    public void LEX_IdentifierToken(string identifierAsString)
    {
        var sourceText = $"{identifierAsString}".ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new CSharpLexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var identifierToken = lexer.SyntaxTokens.First();

        Assert.Equal(SyntaxKind.IdentifierToken, identifierToken.SyntaxKind);

        var text = identifierToken.TextSpan.GetText();
        Assert.Equal(identifierAsString, text);
    }

    [Fact]
    public void LEX_CommentSingleLineToken_WITH_EndingAsEndOfFile()
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

    [Fact]
    public void LEX_CommentSingleLineToken_WITH_EndingAsNewLine()
    {
        var singleLineCommentAsString = @"// C:\Users\hunte\Repos\Aaa\";
        var sourceText = $@"{singleLineCommentAsString}
".ReplaceLineEndings("\n");

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

    [Fact]
    public void LEX_CommentMultiLineToken_WITH_SpansMultipleLines()
    {
        var multiLineCommentAsString = @"/*
	A Multi-Line Comment
*/".ReplaceLineEndings("\n");

        var sourceText = $"{multiLineCommentAsString}".ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new CSharpLexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var commentMultiLineToken = lexer.SyntaxTokens.First();

        Assert.Equal(SyntaxKind.CommentMultiLineToken, commentMultiLineToken.SyntaxKind);

        var text = commentMultiLineToken.TextSpan.GetText();
        Assert.Equal(multiLineCommentAsString, text);
    }

    [Fact]
    public void LEX_CommentMultiLineToken_WITH_SpansSingleLine()
    {
        var multiLineCommentAsString = @"/* Another Multi-Line Comment */"
            .ReplaceLineEndings("\n");

        var sourceText = $"{multiLineCommentAsString}".ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new CSharpLexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var commentMultiLineToken = lexer.SyntaxTokens.First();

        Assert.Equal(SyntaxKind.CommentMultiLineToken, commentMultiLineToken.SyntaxKind);

        var text = commentMultiLineToken.TextSpan.GetText();
        Assert.Equal(multiLineCommentAsString, text);
    }

    // TODO: Fix this unit test that broke due to individual SyntaxKinds being made per keyword.
    // [Fact]
    // public void LEX_KeywordToken()
    // {
    //     var keywordAsString = "int";
    //     var sourceText = $"{keywordAsString}".ReplaceLineEndings("\n");
    //
    //     var resourceUri = new ResourceUri(string.Empty);
    //
    //     var lexer = new CSharpLexer(
    //         resourceUri,
    //         sourceText);
    //
    //     lexer.Lex();
    //
    //     var keywordToken = lexer.SyntaxTokens.First();
    //
    //     Assert.Equal(SyntaxKind.KeywordToken, keywordToken.SyntaxKind);
    //
    //     var text = keywordToken.TextSpan.GetText();
    //     Assert.Equal(keywordAsString, text);
    // }
}