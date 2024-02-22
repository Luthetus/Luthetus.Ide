using System.Collections.Immutable;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.CompilerServices.Lang.C.Facts;
using Luthetus.TextEditor.RazorLib.CompilerServices.Facts;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.SyntaxActors;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.CompilerServices.Lang.C;

public class CLexer
{
    public static readonly GenericPreprocessorDefinition CPreprocessorDefinition = new(
        "#",
        ImmutableArray<DeliminationExtendedSyntaxDefinition>.Empty);

    public static readonly GenericLanguageDefinition CLanguageDefinition = new GenericLanguageDefinition(
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
        CLanguageFacts.Keywords.ALL_LIST,
        CPreprocessorDefinition);

    private readonly GenericSyntaxTree _cSyntaxTree;

    public CLexer(ResourceUri resourceUri)
    {
        _cSyntaxTree = new GenericSyntaxTree(CLanguageDefinition);
        ResourceUri = resourceUri;
    }

    public Key<RenderState> ModelRenderStateKey { get; private set; } = Key<RenderState>.Empty;

    public ResourceUri ResourceUri { get; }

    public Task<ImmutableArray<TextEditorTextSpan>> Lex(
        string sourceText,
        Key<RenderState> modelRenderStateKey)
    {
        var cSyntaxUnit = _cSyntaxTree.ParseText(
            ResourceUri,
            sourceText);

        var cSyntaxWalker = new GenericSyntaxWalker();

        cSyntaxWalker.Visit(cSyntaxUnit.GenericDocumentSyntax);

        var textEditorTextSpans = new List<TextEditorTextSpan>();

        textEditorTextSpans
            .AddRange(cSyntaxWalker.StringSyntaxList
                .Select(x => x.TextSpan));

        textEditorTextSpans
            .AddRange(cSyntaxWalker.CommentSingleLineSyntaxList
                .Select(x => x.TextSpan));

        textEditorTextSpans
            .AddRange(cSyntaxWalker.CommentMultiLineSyntaxList
                .Select(x => x.TextSpan));

        textEditorTextSpans
            .AddRange(cSyntaxWalker.KeywordSyntaxList
                .Select(x => x.TextSpan));

        textEditorTextSpans
            .AddRange(cSyntaxWalker.FunctionSyntaxList
                .Select(x => x.TextSpan));

        return Task.FromResult(textEditorTextSpans.ToImmutableArray());
    }
}