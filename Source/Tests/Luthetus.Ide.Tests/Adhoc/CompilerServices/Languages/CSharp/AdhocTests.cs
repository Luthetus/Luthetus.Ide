namespace Luthetus.Ide.Tests.Adhoc.CompilerServices.Languages.CSharp.ParserCase;

/// <summary>If I find something wrong with the UI I'll put a test here if I feel like I need to in order to solve the UI bug.</summary>
public partial class AdhocTests
{
    [Fact]
    public void SHOULD_SYNTAX_HIGHLIGHT_FUNCTION_PARAMETER_TYPE_AND_IDENTIFIER()
    {
        string sourceText = @"var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();".ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();

        // Assertions
        {
            Assert.Empty(compilationUnit.Children);
        }
    }
}
