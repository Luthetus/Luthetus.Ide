using System.Collections.Immutable;
using Luthetus.Common.RazorLib.Misc;
using Luthetus.TextEditor.RazorLib.Analysis.Html.InjectedLanguage;
using Luthetus.TextEditor.RazorLib.Analysis.Html.SyntaxActors;
using Luthetus.TextEditor.RazorLib.Analysis.Razor.Facts;
using Luthetus.TextEditor.RazorLib.Lexing;

namespace Luthetus.Ide.ClassLib.CompilerServices.Languages.Razor.TextEditorCase;

public class OverriteTextEditorRazorLexer : ITextEditorLexer
{
    public OverriteTextEditorRazorLexer(ResourceUri resourceUri)
    {
        ResourceUri = resourceUri;
    }

    public RenderStateKey ModelRenderStateKey { get; private set; } = RenderStateKey.Empty;

    public ResourceUri ResourceUri { get; }

    public Task<ImmutableArray<TextEditorTextSpan>> Lex(
        string sourceText,
        RenderStateKey modelRenderStateKey)
    {
        var TEST_RazorSyntaxTree = new TEST_RazorSyntaxTree();

        InjectedLanguageDefinition TEST_InjectedLanguageDefinitionRazorInjectedLanguageDefinition = new(
            RazorFacts.TRANSITION_SUBSTRING,
            RazorFacts.TRANSITION_SUBSTRING_ESCAPED,
            TEST_RazorSyntaxTree
                .ParseInjectedLanguageFragment);

        var htmlSyntaxUnit = HtmlSyntaxTree.ParseText(
            ResourceUri,
            sourceText,
            TEST_InjectedLanguageDefinitionRazorInjectedLanguageDefinition);

        var symbols = TEST_RazorSyntaxTree.TEST_Finalize();

        var syntaxNodeRoot = htmlSyntaxUnit.RootTagSyntax;

        var htmlSyntaxWalker = new HtmlSyntaxWalker();

        htmlSyntaxWalker.Visit(syntaxNodeRoot);

        List<TextEditorTextSpan> textEditorTextSpans = new();

        // Tag Names
        {
            textEditorTextSpans.AddRange(htmlSyntaxWalker.TagNameNodes
                .Select(tns => tns.TextEditorTextSpan));
        }

        // InjectedLanguageFragmentSyntaxes
        {
            textEditorTextSpans.AddRange(htmlSyntaxWalker.InjectedLanguageFragmentNodes
                .Select(ilfs => ilfs.TextEditorTextSpan));
        }

        // Attribute Names
        {
            textEditorTextSpans.AddRange(htmlSyntaxWalker.AttributeNameNodes
                .Select(an => an.TextEditorTextSpan));
        }

        // Attribute Values
        {
            textEditorTextSpans.AddRange(htmlSyntaxWalker.AttributeValueNodes
                .Select(av => av.TextEditorTextSpan));
        }

        // Comments
        {
            textEditorTextSpans.AddRange(htmlSyntaxWalker.CommentNodes
                .Select(c => c.TextEditorTextSpan));
        }

        // TEST_Finalize
        {
            textEditorTextSpans.AddRange(symbols
                .Select(s => s.TextSpan));
        }

        return Task.FromResult(textEditorTextSpans.ToImmutableArray());
    }
}