using System.Collections.Immutable;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.CompilerServices.Lang.FSharp.FSharp.Facts;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.SyntaxActors;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.CompilerServices.Lang.FSharp.FSharp.SyntaxActors;

public class TextEditorFSharpLexer
{
    public static readonly GenericPreprocessorDefinition FSharpPreprocessorDefinition = new(
        "#",
        ImmutableArray<DeliminationExtendedSyntaxDefinition>.Empty);

    public static readonly GenericLanguageDefinition FSharpLanguageDefinition = new GenericLanguageDefinition(
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
        "(*",
        "*)",
        FSharpKeywords.ALL,
        FSharpPreprocessorDefinition);

    private readonly GenericSyntaxTree _fSharpSyntaxTree;

    public TextEditorFSharpLexer(ResourceUri resourceUri)
    {
        _fSharpSyntaxTree = new GenericSyntaxTree(FSharpLanguageDefinition);
        ResourceUri = resourceUri;
    }

    public Key<RenderState> ModelRenderStateKey { get; private set; } = Key<RenderState>.Empty;

    public ResourceUri ResourceUri { get; }

    public Task<ImmutableArray<TextEditorTextSpan>> Lex(
        string sourceText,
        Key<RenderState> modelRenderStateKey)
    {
        var fSharpSyntaxUnit = _fSharpSyntaxTree.ParseText(
            ResourceUri,
            sourceText);

        var fSharpSyntaxWalker = new GenericSyntaxWalker();

        fSharpSyntaxWalker.Visit(fSharpSyntaxUnit.GenericDocumentSyntax);

        var textEditorTextSpans = new List<TextEditorTextSpan>();

        textEditorTextSpans
            .AddRange(fSharpSyntaxWalker.StringSyntaxList
                .Select(x => x.TextSpan));

        textEditorTextSpans
            .AddRange(fSharpSyntaxWalker.CommentSingleLineSyntaxList
                .Select(x => x.TextSpan));

        textEditorTextSpans
            .AddRange(fSharpSyntaxWalker.CommentMultiLineSyntaxList
                .Select(x => x.TextSpan));

        textEditorTextSpans
            .AddRange(fSharpSyntaxWalker.KeywordSyntaxList
                .Select(x => x.TextSpan));

        textEditorTextSpans
            .AddRange(fSharpSyntaxWalker.FunctionSyntaxList
                .Select(x => x.TextSpan));

        return Task.FromResult(textEditorTextSpans.ToImmutableArray());
    }
}