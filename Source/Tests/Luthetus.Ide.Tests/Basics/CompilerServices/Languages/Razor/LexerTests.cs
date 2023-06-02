using Luthetus.TextEditor.RazorLib.Lexing;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.Razor.TextEditorCase;
using Luthetus.Common.RazorLib.Misc;

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
  
	public class MyClass
	{
	}
}"
        .ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri("Adhoc.razor");

        var lexer = new IdeRazorLexer(resourceUri);

        lexer.Lex(sourceText, RenderStateKey.NewRenderStateKey());

        var z = 2;
    }
}
