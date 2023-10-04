// using Luthetus.CompilerServices.Lang.C.LexerCase;
// using Luthetus.CompilerServices.Lang.C.ParserCase;

namespace Luthetus.CompilerServices.Lang.C.Tests.UserStories;

/// <summary>
/// TODO: Fix USER_STORY_HELLO_WORLD, it broke on (2023-07-26)
/// 
/// User Story Description:
/// User wants to type up a hello world program in the C programming language.
/// </summary>
// public class USER_STORY_HELLO_WORLD
// {
//     [Fact]
//     public void Enact()
//     {
//         string sourceText = @"#include <stdio.h>
//
// int main() {
//    // printf() displays the string inside quotation
//    printf(""Hello, World!"");
//
//    return 0;
// }".ReplaceLineEndings("\n");
//
//         var resourceUri = new ResourceUri(string.Empty);
//
//         var lexer = new CLexerSession(
//             resourceUri,
//             sourceText);
//
//         lexer.Lex();
//
//         var parser = new CParserSession(lexer);
//
//         var compilationUnit = parser.Parse();
//
//         throw new NotImplementedException("TODO: Perform assertions");
//     }
// }