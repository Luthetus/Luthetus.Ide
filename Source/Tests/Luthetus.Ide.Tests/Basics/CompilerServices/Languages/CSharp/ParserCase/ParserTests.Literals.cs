using Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Expression;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.LexerCase;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.ParserCase;
using Luthetus.TextEditor.RazorLib.Lexing;

namespace Luthetus.Ide.Tests.Basics.CompilerServices.Languages.CSharp.ParserCase;

public partial class ParserTests
{
    [Fact]
    public void SHOULD_PARSE_NUMERIC_LITERAL_EXPRESSION()
    {
        string sourceText = "3".ReplaceLineEndings("\n");
        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();

        // Assertions
        {
            Assert.Single(compilationUnit.Children);

            var boundLiteralExpressionNode = (BoundLiteralExpressionNode)compilationUnit
                .Children[0];

            Assert.Equal(typeof(int), boundLiteralExpressionNode.ResultType);
        }
    }

    [Fact]
    public void SHOULD_PARSE_STRING_LITERAL_EXPRESSION()
    {
        string sourceText = "\"123abc\"".ReplaceLineEndings("\n");
        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();

        // Assertions
        {
            Assert.Single(compilationUnit.Children);

            var boundLiteralExpressionNode = (BoundLiteralExpressionNode)compilationUnit
                .Children[0];

            Assert.Equal(typeof(string), boundLiteralExpressionNode.ResultType);
        }
    }
}
