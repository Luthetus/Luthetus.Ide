using Luthetus.CompilerServices.Lang.CSharp.LexerCase;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.CompilerServices.Lang.CSharp.Tests.Basics;

public partial class LexerTests
{
    [Fact]
    public void SHOULD_LEX_PLUS_TOKEN()
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
    public void SHOULD_LEX_EQUALS_TOKEN()
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
    public void SHOULD_LEX_STAR_TOKEN()
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
    public void SHOULD_LEX_DIVISION_TOKEN()
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
    public void SHOULD_LEX_STATEMENT_DELIMITER_TOKEN()
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
    public void SHOULD_LEX_OPEN_PARENTHESIS_TOKEN()
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
    public void SHOULD_LEX_CLOSE_PARENTHESIS_TOKEN()
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
    public void SHOULD_LEX_OPEN_BRACE_TOKEN()
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
    public void SHOULD_LEX_CLOSE_BRACE_TOKEN()
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
    public void SHOULD_LEX_COLON_TOKEN()
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
    public void SHOULD_LEX_MEMBER_ACCESS_TOKEN()
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
    public void SHOULD_LEX_COMMA_TOKEN()
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
    public void SHOULD_LEX_BANG_TOKEN()
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
    public void SHOULD_LEX_QUESTION_MARK_TOKEN()
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
    public void SHOULD_LEX_QUESTION_MARK_QUESTION_MARK_TOKEN()
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
    public void SHOULD_LEX_OPEN_ANGLE_BRACKET_TOKEN()
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
    public void SHOULD_LEX_CLOSE_ANGLE_BRACKET_TOKEN()
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
    public void SHOULD_LEX_DOLLAR_SIGN_TOKEN()
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
    public void SHOULD_LEX_OPEN_SQUARE_BRACKET_TOKEN()
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
    public void SHOULD_LEX_CLOSE_SQUARE_BRACKET_TOKEN()
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
    public void SHOULD_LEX_PREPROCESSOR_DIRECTIVE_TOKEN()
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
    public void SHOULD_LEX_IDENTIFIER_TOKEN(string identifierAsString)
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
    public void SHOULD_LEX_COMMENT_SINGLE_LINE_TOKEN_WITH_ENDING_AS_END_OF_FILE()
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
    public void SHOULD_LEX_COMMENT_SINGLE_LINE_TOKEN_WITH_ENDING_AS_NEW_LINE()
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
    public void SHOULD_LEX_COMMENT_MULTI_LINE_TOKEN_WRITTEN_ON_MULTIPLE_LINES()
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
    public void SHOULD_LEX_COMMENT_MULTI_LINE_TOKEN_WRITTEN_ON_SINGLE_LINE()
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
    // public void SHOULD_LEX_KEYWORD_TOKEN()
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