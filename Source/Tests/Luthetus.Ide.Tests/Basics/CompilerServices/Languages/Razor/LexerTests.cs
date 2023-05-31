using Luthetus.Common.RazorLib.Misc;
using Luthetus.TextEditor.RazorLib.Analysis.Html.SyntaxActors;
using Luthetus.TextEditor.RazorLib.Analysis.Razor.Facts;
using Luthetus.TextEditor.RazorLib.Analysis.Razor.SyntaxActors;
using Luthetus.TextEditor.RazorLib.Lexing;

namespace Luthetus.Ide.Tests.Basics.CompilerServices.Languages.Razor;

public class LexerTests
{
    /// <summary>Trying to figure out where I want to go with this file. So I'm going to use this adhoc test to reason out my plan.</summary>
    [Fact]
    public void ADHOC()
    {
        var sourceText = @"<div class=""bwa_counter""
     @onclick=""IncrementCountOnClick"">

	Count: @_count
</div>

@code {
	private int _count;

	private void IncrementCountOnClick()
	{
		_count++;
	}
}";

        var resourceUri = new ResourceUri("Counter.razor");

        var textEditorRazorLexer = new TextEditorRazorLexer(resourceUri);

        var htmlSyntaxUnit = HtmlSyntaxTree.ParseText(
            resourceUri,
            sourceText,
            RazorInjectedLanguageFacts.RazorInjectedLanguageDefinition);

        var z = 2;
    }
}
