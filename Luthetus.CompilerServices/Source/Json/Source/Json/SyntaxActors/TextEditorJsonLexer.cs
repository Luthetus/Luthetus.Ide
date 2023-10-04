using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System.Collections.Immutable;

namespace Luthetus.CompilerServices.Lang.Json.Json.SyntaxActors;

public class TextEditorJsonLexer
{
    public TextEditorJsonLexer(ResourceUri resourceUri)
    {
        ResourceUri = resourceUri;
    }

    public Key<RenderState> ModelRenderStateKey { get; private set; } = Key<RenderState>.Empty;

    public ResourceUri ResourceUri { get; }

    public Task<ImmutableArray<TextEditorTextSpan>> Lex(
        string sourceText,
        Key<RenderState> modelRenderStateKey)
    {
        var jsonSyntaxUnit = JsonSyntaxTree.ParseText(ResourceUri, sourceText);

        var syntaxNodeRoot = jsonSyntaxUnit.JsonDocumentSyntax;

        var jsonSyntaxWalker = new JsonSyntaxWalker();

        jsonSyntaxWalker.Visit(syntaxNodeRoot);

        List<TextEditorTextSpan> textEditorTextSpans = new();

        textEditorTextSpans.AddRange(
            jsonSyntaxWalker.JsonPropertyKeySyntaxes.Select(propertyKey =>
                propertyKey.TextEditorTextSpan));

        textEditorTextSpans.AddRange(
            jsonSyntaxWalker.JsonBooleanSyntaxes.Select(boolean =>
                boolean.TextEditorTextSpan));

        textEditorTextSpans.AddRange(
            jsonSyntaxWalker.JsonIntegerSyntaxes.Select(integer =>
                integer.TextEditorTextSpan));

        textEditorTextSpans.AddRange(
            jsonSyntaxWalker.JsonNullSyntaxes.Select(n =>
                n.TextEditorTextSpan));

        textEditorTextSpans.AddRange(
            jsonSyntaxWalker.JsonNumberSyntaxes.Select(number =>
                number.TextEditorTextSpan));

        textEditorTextSpans.AddRange(
            jsonSyntaxWalker.JsonStringSyntaxes.Select(s =>
                s.TextEditorTextSpan));

        return Task.FromResult(textEditorTextSpans.ToImmutableArray());
    }
}