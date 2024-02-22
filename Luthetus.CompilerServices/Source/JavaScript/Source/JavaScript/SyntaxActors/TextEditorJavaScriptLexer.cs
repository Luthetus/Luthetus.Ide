using System.Collections.Immutable;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.CompilerServices.Lang.JavaScript.JavaScript.Facts;
using Luthetus.TextEditor.RazorLib.CompilerServices.Facts;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.SyntaxActors;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.CompilerServices.Lang.JavaScript.JavaScript.SyntaxActors;

public class TextEditorJavaScriptLexer
{
    public static readonly GenericPreprocessorDefinition JavaScriptPreprocessorDefinition = new(
        "#",
        ImmutableArray<DeliminationExtendedSyntaxDefinition>.Empty);

    public static readonly GenericLanguageDefinition JavaScriptLanguageDefinition = new GenericLanguageDefinition(
        "\"",
        "\"",
        "(",
        ")",
        ".",
        "//",
        new[]
        {
            WhitespaceFacts.CARRIAGE_RETURN.ToString(),
            WhitespaceFacts.LINE_FEED.ToString()
        }.ToImmutableArray(),
        "/*",
        "*/",
        JavaScriptKeywords.ALL,
        JavaScriptPreprocessorDefinition);

    private readonly GenericSyntaxTree _javaScriptSyntaxTree;

    public TextEditorJavaScriptLexer(ResourceUri resourceUri)
    {
        _javaScriptSyntaxTree = new GenericSyntaxTree(JavaScriptLanguageDefinition);
        ResourceUri = resourceUri;
    }

    public Key<RenderState> ModelRenderStateKey { get; private set; } = Key<RenderState>.Empty;

    public ResourceUri ResourceUri { get; }

    public Task<ImmutableArray<TextEditorTextSpan>> Lex(
        string sourceText,
        Key<RenderState> modelRenderStateKey)
    {
        var javaScriptSyntaxUnit = _javaScriptSyntaxTree.ParseText(
            ResourceUri,
            sourceText);

        var javaScriptSyntaxWalker = new GenericSyntaxWalker();

        javaScriptSyntaxWalker.Visit(javaScriptSyntaxUnit.GenericDocumentSyntax);

        var textEditorTextSpans = new List<TextEditorTextSpan>();

        textEditorTextSpans
            .AddRange(javaScriptSyntaxWalker.StringSyntaxList
                .Select(x => x.TextSpan));

        textEditorTextSpans
            .AddRange(javaScriptSyntaxWalker.CommentSingleLineSyntaxList
                .Select(x => x.TextSpan));

        textEditorTextSpans
            .AddRange(javaScriptSyntaxWalker.CommentMultiLineSyntaxList
                .Select(x => x.TextSpan));

        textEditorTextSpans
            .AddRange(javaScriptSyntaxWalker.KeywordSyntaxList
                .Select(x => x.TextSpan));

        textEditorTextSpans
            .AddRange(javaScriptSyntaxWalker.FunctionSyntaxList
                .Select(x => x.TextSpan));

        return Task.FromResult(textEditorTextSpans.ToImmutableArray());
    }
}