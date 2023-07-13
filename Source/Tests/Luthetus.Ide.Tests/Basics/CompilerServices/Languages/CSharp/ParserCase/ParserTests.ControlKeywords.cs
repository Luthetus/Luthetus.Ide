using Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Statements;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.LexerCase;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.ParserCase;
using Luthetus.TextEditor.RazorLib.Lexing;

namespace Luthetus.Ide.Tests.Basics.CompilerServices.Languages.CSharp.ParserCase;

public partial class ParserTests
{
    [Fact]
    public void SHOULD_PARSE_IF_STATEMENT()
    {
        var sourceText = @"if (true) { }".ReplaceLineEndings("\n");
        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer.SyntaxTokens, lexer.Diagnostics);
        var compilationUnit = parser.Parse();

        // Assertions
        {
            var boundIfStatementNode =
            (BoundIfStatementNode)compilationUnit.Children.Single();

            Assert.NotNull(boundIfStatementNode.KeywordToken);
            Assert.NotNull(boundIfStatementNode.BoundExpressionNode);
            Assert.NotNull(boundIfStatementNode.IfStatementBodyCompilationUnit);
        }
    }
}
