using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax.SyntaxTokens;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.Razor.LexerCase;
using Luthetus.TextEditor.RazorLib.Analysis.GenericLexer.Decoration;
using Luthetus.TextEditor.RazorLib.Lexing;
using System.Text;

namespace Luthetus.Ide.Tests.Basics.CompilerServices.Languages.Razor;

public class LexerTests
{
    [Fact]
    public void SHOULD_LEX_AT_EXPRESSION_IMPLICIT()
    {
        var sourceText = "@_count".ReplaceLineEndings("\n");
        throw new NotImplementedException();
    }
    
    [Fact]
    public void SHOULD_LEX_AT_EXPRESSION_EXPLICIT()
    {
        var sourceText = "@(_count)".ReplaceLineEndings("\n");
        throw new NotImplementedException();
    }
    
    [Fact]
    public void SHOULD_LEX_TEXT_ELEMENT_TAG()
    {
        var sourceText = "<text>my text</text>".ReplaceLineEndings("\n");
        throw new NotImplementedException();
    }
    
    [Fact]
    public void SHOULD_LEX_SINGLE_LINE_TEXT_OPERATOR()
    {
        var sourceText = "@: my text".ReplaceLineEndings("\n");
        throw new NotImplementedException();
    }
    
    [Fact]
    public void SHOULD_LEX_RAZOR_DIRECTIVE()
    {
        var sourceText = "@onclick=\"IncrementCountOnClick\"".ReplaceLineEndings("\n");
        throw new NotImplementedException();
    }
    
    [Fact]
    public void SHOULD_LEX_RAZOR_TAG_HELPER()
    {
        var sourceText = "@page \"/route\"".ReplaceLineEndings("\n");
        throw new NotImplementedException();
    }

    [Fact]
    public void SHOULD_LEX_RAZOR_DIRECTIVE_WITH_ATTRIBUTE()
    {
        var sourceText = "@onclick:stopPropagation=\"true\"".ReplaceLineEndings("\n");
        throw new NotImplementedException();
    }

    [Theory]
    [InlineData("if")]
    [InlineData("for")]
    [InlineData("foreach")]
    [InlineData("while")]
    [InlineData("do")]
    public void SHOULD_LEX_AT_CONTROL_KEYWORD(string controlKeyword)
    {
        var testNoSpace = $"@{controlKeyword}}}";
        var testWithSpace = $"@{controlKeyword} }}";
        var testWithCarriageReturn = $"@{controlKeyword}\r}}";
        var testWithNewLine = $"@{controlKeyword}\n}}";
        var testWithCarriageReturnNewLine = $"@{controlKeyword}\r\n}}";

        var testInputs = new string[] 
        {
            testNoSpace,
            testWithSpace,
            testWithCarriageReturn,
            testWithNewLine,
            testWithCarriageReturnNewLine,
        };

        var resourceUri = new ResourceUri(string.Empty);

        foreach (var sourceText in testInputs)
        {
            var lexer = new Lexer(resourceUri, sourceText);
            lexer.Lex();
        }

        throw new NotImplementedException();
    }

    [Fact]
    public void SHOULD_LEX_ELSE_KEYWORD()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void SHOULD_LEX_ELSE_IF_KEYWORD()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void SHOULD_LEX_ELSE_IF_AND_ELSE_KEYWORD()
    {
        throw new NotImplementedException();
    }

    /// <summary>Off top my head I cannot remember but early Blazor used "@functions" or something instead of "@code"</summary>
    [Theory]
    [InlineData("@functions{", "")]
    [InlineData("@functions{", " ")]
    [InlineData("@functions {", "")]
    [InlineData("@functions {", " ")]
    [InlineData("@functions\n{", "")]
    [InlineData("@functions\r{", "")]
    [InlineData("@functions\r\n{", "")]
    [InlineData("@functions\r\n{\n", "\n")]
    [InlineData("@functions{", "private int _count;")]
    public void SHOULD_LEX_AT_FUNCTIONS_BLOCK(
        string openingText,
        string bodyText)
    {
        var closingText = "}";

        var sourceText = (openingText + bodyText + closingText)
            .ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(resourceUri, sourceText);

        lexer.Lex();

        var functionsBlockOpeningToken = lexer.SyntaxTokens[0];
        var bodyToken = lexer.SyntaxTokens[1];
        var closingToken = lexer.SyntaxTokens[2];
        var endOfFileToken = lexer.SyntaxTokens[3];

        Assert.Equal(SyntaxKind.RazorFunctionsBlockOpeningToken, functionsBlockOpeningToken.SyntaxKind);
        Assert.Equal(SyntaxKind.RazorBlockBodyToken, bodyToken.SyntaxKind);
        Assert.Equal(SyntaxKind.RazorBlockClosingToken, closingToken.SyntaxKind);
        Assert.Equal(SyntaxKind.EndOfFileToken, endOfFileToken.SyntaxKind);

        Assert.Equal(openingText, functionsBlockOpeningToken.TextSpan.GetText());
        Assert.Equal(bodyText, bodyToken.TextSpan.GetText());
        Assert.Equal(closingText, closingToken.TextSpan.GetText());
    }

    [Theory]
    [InlineData("@code{", "")]
    [InlineData("@code{", " ")]
    [InlineData("@code {", "")]
    [InlineData("@code {", " ")]
    [InlineData("@code\n{", "")]
    [InlineData("@code\r{", "")]
    [InlineData("@code\r\n{", "")]
    [InlineData("@code\r\n{\n", "\n")]
    [InlineData("@code{", "private int _count;")]
    public void SHOULD_LEX_AT_CODE_BLOCK(
        string openingText,
        string bodyText)
    {
        var closingText = "}";

        var sourceText = (openingText + bodyText + closingText)
            .ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(resourceUri, sourceText);

        lexer.Lex();

        var functionsBlockOpeningToken = lexer.SyntaxTokens[0];
        var bodyToken = lexer.SyntaxTokens[1];
        var closingToken = lexer.SyntaxTokens[2];
        var endOfFileToken = lexer.SyntaxTokens[3];

        Assert.Equal(SyntaxKind.RazorFunctionsBlockOpeningToken, functionsBlockOpeningToken.SyntaxKind);
        Assert.Equal(SyntaxKind.RazorBlockBodyToken, bodyToken.SyntaxKind);
        Assert.Equal(SyntaxKind.RazorBlockClosingToken, closingToken.SyntaxKind);
        Assert.Equal(SyntaxKind.EndOfFileToken, endOfFileToken.SyntaxKind);

        Assert.Equal(openingText, functionsBlockOpeningToken.TextSpan.GetText());
        Assert.Equal(bodyText, bodyToken.TextSpan.GetText());
        Assert.Equal(closingText, closingToken.TextSpan.GetText());
    }

    [Theory]
    [InlineData("@{", "")]
    [InlineData("@{", " ")]
    [InlineData("@{", "private int _count;")]
    public void SHOULD_LEX_AT_C_SHARP_BLOCK(
        string openingText,
        string bodyText)
    {
        var closingText = "}";

        var sourceText = (openingText + bodyText + closingText)
            .ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(resourceUri, sourceText);

        lexer.Lex();

        var functionsBlockOpeningToken = lexer.SyntaxTokens[0];
        var bodyToken = lexer.SyntaxTokens[1];
        var closingToken = lexer.SyntaxTokens[2];
        var endOfFileToken = lexer.SyntaxTokens[3];

        Assert.Equal(SyntaxKind.RazorFunctionsBlockOpeningToken, functionsBlockOpeningToken.SyntaxKind);
        Assert.Equal(SyntaxKind.RazorBlockBodyToken, bodyToken.SyntaxKind);
        Assert.Equal(SyntaxKind.RazorBlockClosingToken, closingToken.SyntaxKind);
        Assert.Equal(SyntaxKind.EndOfFileToken, endOfFileToken.SyntaxKind);

        Assert.Equal(openingText, functionsBlockOpeningToken.TextSpan.GetText());
        Assert.Equal(bodyText, bodyToken.TextSpan.GetText());
        Assert.Equal(closingText, closingToken.TextSpan.GetText());
    }

    [Fact]
    public void SHOULD_SPLIT_HTML_AND_CSHARP_TOKENS()
    {
        var sourceText = @"<div class=""bwa_counter""
     @onclick=""IncrementCountOnClick"">

	Count: @_count
</div>

@code {
	private int _count;

	private void IncrementCountOnClick()
	{
		_count++;
	}
}".ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var expectedSyntaxTokens = new List<ISyntaxToken>();

        // "<div class=\"bwa_counter\"\n     "
        {
            var textSpan = new TextEditorTextSpan(0, 30, (byte)GenericDecorationKind.None, resourceUri, sourceText);
            expectedSyntaxTokens.Add(new RazorHtmlToken(textSpan));
        }

        // "@onclick=\"IncrementCountOnClick\""
        {
            var textSpan = new TextEditorTextSpan(30, 62, (byte)GenericDecorationKind.None, resourceUri, sourceText);
            expectedSyntaxTokens.Add(new RazorDirectiveToken(textSpan));
        }

        // ">\n\n\tCount: "
        {
            var textSpan = new TextEditorTextSpan(62, 73, (byte)GenericDecorationKind.None, resourceUri, sourceText);
            expectedSyntaxTokens.Add(new RazorHtmlToken(textSpan));
        }

        // "@_count"
        {
            var textSpan = new TextEditorTextSpan(73, 80, (byte)GenericDecorationKind.None, resourceUri, sourceText);
            expectedSyntaxTokens.Add(new RazorCSharpToken(textSpan));
        }

        // "\n</div>\n\n"
        {
            var textSpan = new TextEditorTextSpan(80, 89, (byte)GenericDecorationKind.None, resourceUri, sourceText);
            expectedSyntaxTokens.Add(new RazorHtmlToken(textSpan));
        }

        // "@code {\n\tprivate int _count;\n\n\tprivate void IncrementCountOnClick()\n\t{\n\t\t_count++;\n\t}\n}"
        {
            var textSpan = new TextEditorTextSpan(89, 176, (byte)GenericDecorationKind.None, resourceUri, sourceText);
            expectedSyntaxTokens.Add(new RazorCSharpToken(textSpan));
        }

        var htmlOnlyStringBuilder = new StringBuilder();
        var nonHtmlStringBuilder = new StringBuilder();

        foreach (var token in expectedSyntaxTokens)
        {
            if (token.SyntaxKind == SyntaxKind.RazorHtmlToken)
                htmlOnlyStringBuilder.Append(token.TextSpan.GetText());
            else
                nonHtmlStringBuilder.Append(token.TextSpan.GetText());
        }

        var htmlOnlyString = htmlOnlyStringBuilder.ToString();
        var nonHtmlString = nonHtmlStringBuilder.ToString();

        var lexer = new Lexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        throw new NotImplementedException();
    }
}
