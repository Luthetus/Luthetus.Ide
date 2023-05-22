using Luthetus.Ide.ClassLib.CodeAnalysis.C.Syntax;

namespace Luthetus.Ide.Tests.UserStories.SemanticParsing.C;

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

        var lexer = new LexSession(sourceText);

        lexer.Lex();

        var parser = new ParserSession(
            lexer.SyntaxTokens,
            sourceText,
            lexer.Diagnostics);

        var compilationUnit = parser.Parse();

        throw new NotImplementedException("TODO: Perform assertions");
    }
}