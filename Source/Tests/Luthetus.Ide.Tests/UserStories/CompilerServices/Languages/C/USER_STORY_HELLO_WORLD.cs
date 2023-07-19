using Luthetus.Ide.ClassLib.CompilerServices.Languages.C.LexerCase;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.C.ParserCase;
using Luthetus.TextEditor.RazorLib.Lexing;

namespace Luthetus.Ide.Tests.UserStories.CompilerServices.Languages.C;

/// <summary>
/// User Story Description:
/// User wants to type up a hello world program in the C programming language.
/// </summary>
public class USER_STORY_HELLO_WORLD
{
    [Fact]
    public void Enact()
    {
        string sourceText = @"#include <stdio.h>

int main() {
   // printf() displays the string inside quotation
   printf(""Hello, World!"");

   return 0;
}".ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new CLexerSession(
            resourceUri,
            sourceText);

        lexer.Lex();

        var parser = new CParserSession(lexer);

        var compilationUnit = parser.Parse();

        throw new NotImplementedException("TODO: Perform assertions");
    }
}