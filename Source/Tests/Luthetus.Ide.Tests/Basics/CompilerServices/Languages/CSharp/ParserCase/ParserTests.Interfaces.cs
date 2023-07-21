namespace Luthetus.Ide.Tests.Basics.CompilerServices.Languages.CSharp.ParserCase;

public partial class ParserTests
{
    [Fact]
    public void SHOULD_PARSE_INTERFACE_FUNCTION_DEFINITION()
    {
        string sourceText = @$"public interface IPersonRepository {{ public void AddPerson(IPersonModel personModel); }}".ReplaceLineEndings("\n");
        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();

        // Assertions
        {
            throw new NotImplementedException();
        }
    }
}
