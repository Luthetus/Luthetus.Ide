using Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.LexerCase;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.ParserCase;
using Luthetus.TextEditor.RazorLib.Lexing;

namespace Luthetus.Ide.Tests.UserStories.CompilerServices.Languages.CSharp;

/// <summary>
/// User Story Description:
/// User wants to type up a hello world program in the C# programming language.
/// </summary>
public class USER_STORY_HELLO_WORLD
{
    [Fact]
    public void Enact()
    {
        string sourceText = @"// Hello World! program
namespace HelloWorld
{
    class Hello {         
        static void Main(string[] args)
        {
            System.Console.WriteLine(""Hello World!"");
        }
    }
}".ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var parser = new Parser(
            lexer.SyntaxTokens,
            sourceText,
            lexer.Diagnostics);

        var compilationUnit = parser.Parse();

        throw new NotImplementedException("TODO: Perform assertions");
    }
}