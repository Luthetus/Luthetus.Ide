using Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.LexerCase;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.ParserCase;
using Luthetus.TextEditor.RazorLib.Lexing;

namespace Luthetus.Ide.Tests.UserStories.CompilerServices.Languages.CSharp;

/// <summary>
/// User Story Description:
/// User wants to type up a "Program.cs" file which uses top level statements in the C# programming language.
/// </summary>
public class USER_STORY_PROGRAM_CS_FILE_TOP_LEVEL_STATEMENTS
{
    [Fact]
    public void Enact()
    {
        string sourceText = @"using BlazorWasmApp;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>(""#app"");
builder.RootComponents.Add<HeadOutlet>(""head::after"");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();
".ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new CSharpLexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var parser = new CSharpParser(
            lexer.SyntaxTokens,
            lexer.Diagnostics);

        var compilationUnit = parser.Parse();

        throw new NotImplementedException("TODO: Perform assertions");
    }
}