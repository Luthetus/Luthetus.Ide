using Luthetus.TextEditor.RazorLib.Analysis.Html.SyntaxActors;
using Luthetus.TextEditor.RazorLib.Analysis.Razor.Facts;
using Luthetus.TextEditor.RazorLib.Lexing;
using Luthetus.TextEditor.RazorLib.Analysis.Html.InjectedLanguage;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.Razor.TextEditorCase;

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

        var TEST_RazorSyntaxTree = new IdeRazorSyntaxTree();

        InjectedLanguageDefinition TEST_InjectedLanguageDefinitionRazorInjectedLanguageDefinition = new(
            RazorFacts.TRANSITION_SUBSTRING,
            RazorFacts.TRANSITION_SUBSTRING_ESCAPED,
            TEST_RazorSyntaxTree
                .ParseInjectedLanguageFragment);

        var htmlSyntaxUnit = HtmlSyntaxTree.ParseText(
            resourceUri,
            sourceText,
            TEST_InjectedLanguageDefinitionRazorInjectedLanguageDefinition);

        TEST_RazorSyntaxTree.ParseAdhocCSharpClass();

        var z = 2;
    }
}
