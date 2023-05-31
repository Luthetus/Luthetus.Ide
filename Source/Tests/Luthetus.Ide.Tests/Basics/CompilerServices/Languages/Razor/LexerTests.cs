using Luthetus.Common.RazorLib.Misc;
using Luthetus.TextEditor.RazorLib.Analysis.Razor.SyntaxActors;
using Luthetus.TextEditor.RazorLib.Lexing;

namespace Luthetus.Ide.Tests.Basics.CompilerServices.Languages.Razor;

public class LexerTests
{
    /// <summary>Trying to figure out where I want to go with this file. So I'm going to use this adhoc test to reason out my plan.</summary>
    [Fact]
    public void ADHOC()
    {
        var sourceText = @"<h1>Hello, world!</h1>".ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri("Adhoc.razor");

        var textEditorRazorLexer = new TextEditorRazorLexer(resourceUri);

        var textSpans = textEditorRazorLexer.Lex(
            sourceText,
            RenderStateKey.NewRenderStateKey());

        var z = 2;
    }
}
