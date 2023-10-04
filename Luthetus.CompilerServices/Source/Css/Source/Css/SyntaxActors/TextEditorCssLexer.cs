using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System.Collections.Immutable;

namespace Luthetus.CompilerServices.Lang.Css.Css.SyntaxActors;

public class TextEditorCssLexer
{
    public TextEditorCssLexer(ResourceUri resourceUri)
    {
        ResourceUri = resourceUri;
    }

    public Key<RenderState> ModelRenderStateKey { get; private set; } = Key<RenderState>.Empty;

    public ResourceUri ResourceUri { get; }

    public Task<ImmutableArray<TextEditorTextSpan>> Lex(
        string sourceText,
        Key<RenderState> modelRenderStateKey)
    {
        var cssSyntaxUnit = CssSyntaxTree.ParseText(
            ResourceUri,
            sourceText);

        var syntaxNodeRoot = cssSyntaxUnit.CssDocumentSyntax;

        var cssSyntaxWalker = new CssSyntaxWalker();

        cssSyntaxWalker.Visit(syntaxNodeRoot);

        List<TextEditorTextSpan> textEditorTextSpans = new();

        textEditorTextSpans.AddRange(
            cssSyntaxWalker.CssIdentifierSyntaxes.Select(identifier =>
                identifier.TextEditorTextSpan));

        textEditorTextSpans.AddRange(
            cssSyntaxWalker.CssCommentSyntaxes.Select(comment =>
                comment.TextEditorTextSpan));

        textEditorTextSpans.AddRange(
            cssSyntaxWalker.CssPropertyNameSyntaxes.Select(propertyName =>
                propertyName.TextEditorTextSpan));

        textEditorTextSpans.AddRange(
            cssSyntaxWalker.CssPropertyValueSyntaxes.Select(propertyValue =>
                propertyValue.TextEditorTextSpan));

        return Task.FromResult(textEditorTextSpans.ToImmutableArray());
    }
}