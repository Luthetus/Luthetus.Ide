using System.Collections.Immutable;
using Luthetus.Common.RazorLib.Misc;
using Luthetus.TextEditor.RazorLib.Analysis.Html.InjectedLanguage;
using Luthetus.TextEditor.RazorLib.Analysis.Html.SyntaxActors;
using Luthetus.TextEditor.RazorLib.Analysis.Razor.Facts;
using Luthetus.TextEditor.RazorLib.Lexing;

namespace Luthetus.Ide.ClassLib.CompilerServices.Languages.Razor;

public class RazorLexer
{
    public RazorLexer(ResourceUri resourceUri)
    {
        ResourceUri = resourceUri;
    }

    public RazorSyntaxTree IdeRazorSyntaxTree = new();

    public RenderStateKey ModelRenderStateKey { get; private set; } = RenderStateKey.Empty;

    public ResourceUri ResourceUri { get; }
    public ImmutableArray<TextEditorTextSpan> RecentResult { get; private set; } = ImmutableArray<TextEditorTextSpan>.Empty;

    public Task<ImmutableArray<TextEditorTextSpan>> Lex(
        string sourceText,
        RenderStateKey modelRenderStateKey)
    {
        if (ModelRenderStateKey == modelRenderStateKey)
            return Task.FromResult(RecentResult);

        ModelRenderStateKey = modelRenderStateKey;

        IdeRazorSyntaxTree = new RazorSyntaxTree();

        InjectedLanguageDefinition razorInjectedLanguageDefinition = new(
            RazorFacts.TRANSITION_SUBSTRING,
            RazorFacts.TRANSITION_SUBSTRING_ESCAPED,
            IdeRazorSyntaxTree.ParseInjectedLanguageFragment,
            RazorSyntaxTree.ParseAttributeName,
            RazorSyntaxTree.ParseAttributeValue);

        var htmlSyntaxUnit = HtmlSyntaxTree.ParseText(
            ResourceUri,
            sourceText,
            razorInjectedLanguageDefinition);

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

        RecentResult = textEditorTextSpans.ToImmutableArray();

        return Task.FromResult(RecentResult);
    }
}