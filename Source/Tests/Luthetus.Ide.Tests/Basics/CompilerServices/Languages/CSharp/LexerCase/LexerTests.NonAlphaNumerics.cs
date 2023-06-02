using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.LexerCase;
using Luthetus.TextEditor.RazorLib.Lexing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luthetus.Ide.Tests.Basics.CompilerServices.Languages.CSharp.LexerCase;

public partial class LexerTests
{
    [Fact]
    public void SHOULD_LEX_PLUS_TOKEN()
    {
        var plusTokenAsString = "+";
        var sourceText = $"{plusTokenAsString}".ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(
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

        var lexer = new Lexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var equalsToken = lexer.SyntaxTokens.First();

        Assert.Equal(SyntaxKind.EqualsToken, equalsToken.SyntaxKind);

        var text = equalsToken.TextSpan.GetText();
        Assert.Equal(equalsTokenAsString, text);
    }

    [Fact]
    public void SHOULD_LEX_STATEMENT_DELIMITER_TOKEN()
    {
        var statementDelimiterTokenAsString = ";";
        var sourceText = $"{statementDelimiterTokenAsString}".ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(
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

        var lexer = new Lexer(
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

        var lexer = new Lexer(
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

        var lexer = new Lexer(
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

        var lexer = new Lexer(
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

        var lexer = new Lexer(
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

        var lexer = new Lexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var memberAccessToken = lexer.SyntaxTokens.First();

        Assert.Equal(SyntaxKind.MemberAccessToken, memberAccessToken.SyntaxKind);

        var text = memberAccessToken.TextSpan.GetText();
        Assert.Equal(memberAccessTokenAsString, text);
    }

    [Fact]
    public void SHOULD_LEX_BANG_TOKEN()
    {
        var bangTokenAsString = "!";
        var sourceText = $"{bangTokenAsString}".ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(
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

        var lexer = new Lexer(
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

        var lexer = new Lexer(
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

        var lexer = new Lexer(
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

        var lexer = new Lexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var closeAngleBracketToken = lexer.SyntaxTokens.First();

        Assert.Equal(SyntaxKind.CloseAngleBracketToken, closeAngleBracketToken.SyntaxKind);

        var text = closeAngleBracketToken.TextSpan.GetText();
        Assert.Equal(closeAngleBracketTokenAsString, text);
    }
}
