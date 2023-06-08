using Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.LexerCase;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.ParserCase;
using Luthetus.TextEditor.RazorLib.Lexing;

namespace Luthetus.Ide.Tests.Basics.CompilerServices.Languages.CSharp.ParserCase;

public partial class ParserTests
{
    [Fact]
    public void SHOULD_PARSE_INTERFACE_FUNCTION_DEFINITION()
    {
        string sourceText = @$"public interface IPersonRepository {{ public void AddPerson(IPersonModel personModel); }}".ReplaceLineEndings("\n");
        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new Parser(lexer.SyntaxTokens, lexer.Diagnostics);
        var compilationUnit = parser.Parse();

        // Assertions
        {
            throw new NotImplementedException();
        }
    }
}
