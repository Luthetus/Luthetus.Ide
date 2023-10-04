using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.CompilerServices.Lang.Css.Css.Decoration;
using Luthetus.CompilerServices.Lang.Css.Css.SyntaxActors;
using Luthetus.CompilerServices.Lang.Css.Tests.TestDataFolder;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System.Collections.Immutable;

namespace Luthetus.CompilerServices.Lang.Css.Tests.TestsCopyPastedFromTextEditor;

public class LexCssTests
{
    [Fact]
    public async Task LexCommentManyValid()
    {
        var sourceText = TestData.Css.EXAMPLE_TEXT_21_LINES
            .ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var expectedTextEditorTextSpans = new[]
        {
        new TextEditorTextSpan(0, 107, (byte)CssDecorationKind.Comment, resourceUri, sourceText),
        new TextEditorTextSpan(225, 268, (byte)CssDecorationKind.Comment, resourceUri, sourceText),
        new TextEditorTextSpan(338, 407, (byte)CssDecorationKind.Comment, resourceUri, sourceText),
        new TextEditorTextSpan(436, 498, (byte)CssDecorationKind.Comment, resourceUri, sourceText),
    };

        var cssLexer = new TextEditorCssLexer(resourceUri);

        var textEditorTextSpans = await cssLexer.Lex(
            sourceText,
            Key<RenderState>.NewKey());

        textEditorTextSpans = textEditorTextSpans
            .Where(x => x.DecorationByte == (byte)CssDecorationKind.Comment)
            .OrderBy(x => x.StartingIndexInclusive)
            .ToImmutableArray();

        Assert.Equal(expectedTextEditorTextSpans, textEditorTextSpans);
    }

    [Fact]
    public async Task LexPropertyName()
    {
        var sourceText = TestData.Css.EXAMPLE_TEXT_21_LINES
            .ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var expectedTextEditorTextSpans = new[]
        {
        new TextEditorTextSpan(117, 123, (byte)CssDecorationKind.PropertyName, resourceUri, sourceText),
        new TextEditorTextSpan(133, 149, (byte)CssDecorationKind.PropertyName, resourceUri, sourceText),
        new TextEditorTextSpan(171, 182, (byte)CssDecorationKind.PropertyName, resourceUri, sourceText),
        new TextEditorTextSpan(205, 214, (byte)CssDecorationKind.PropertyName, resourceUri, sourceText),
        new TextEditorTextSpan(276, 285, (byte)CssDecorationKind.PropertyName, resourceUri, sourceText),
        new TextEditorTextSpan(295, 306, (byte)CssDecorationKind.PropertyName, resourceUri, sourceText),
        new TextEditorTextSpan(318, 328, (byte)CssDecorationKind.PropertyName, resourceUri, sourceText),
        new TextEditorTextSpan(421, 426, (byte)CssDecorationKind.PropertyName, resourceUri, sourceText),
        new TextEditorTextSpan(509, 514, (byte)CssDecorationKind.PropertyName, resourceUri, sourceText),
    };

        var cssLexer = new TextEditorCssLexer(resourceUri);

        var textEditorTextSpans = await cssLexer.Lex(
            sourceText,
            Key<RenderState>.NewKey());

        textEditorTextSpans = textEditorTextSpans
            .Where(x => x.DecorationByte == (byte)CssDecorationKind.PropertyName)
            .OrderBy(x => x.StartingIndexInclusive)
            .ToImmutableArray();

        Assert.Equal(expectedTextEditorTextSpans, textEditorTextSpans);
    }

    [Fact]
    public async Task LexPropertyValue()
    {
        var sourceText = TestData.Css.EXAMPLE_TEXT_21_LINES
            .ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var expectedTextEditorTextSpans = new[]
        {
        new TextEditorTextSpan(125, 129, (byte)CssDecorationKind.PropertyValue, resourceUri, sourceText),
        new TextEditorTextSpan(151, 167, (byte)CssDecorationKind.PropertyValue, resourceUri, sourceText),
        new TextEditorTextSpan(184, 201, (byte)CssDecorationKind.PropertyValue, resourceUri, sourceText),
        new TextEditorTextSpan(216, 220, (byte)CssDecorationKind.PropertyValue, resourceUri, sourceText),
        new TextEditorTextSpan(287, 291, (byte)CssDecorationKind.PropertyValue, resourceUri, sourceText),
        new TextEditorTextSpan(308, 314, (byte)CssDecorationKind.PropertyValue, resourceUri, sourceText),
        new TextEditorTextSpan(330, 333, (byte)CssDecorationKind.PropertyValue, resourceUri, sourceText),
        new TextEditorTextSpan(428, 431, (byte)CssDecorationKind.PropertyValue, resourceUri, sourceText),
        new TextEditorTextSpan(516, 521, (byte)CssDecorationKind.PropertyValue, resourceUri, sourceText),
    };

        var cssLexer = new TextEditorCssLexer(resourceUri);

        var textEditorTextSpans = await cssLexer.Lex(
            sourceText,
            Key<RenderState>.NewKey());

        textEditorTextSpans = textEditorTextSpans
            .Where(x => x.DecorationByte == (byte)CssDecorationKind.PropertyValue)
            .OrderBy(x => x.StartingIndexInclusive)
            .ToImmutableArray();

        Assert.Equal(expectedTextEditorTextSpans, textEditorTextSpans);
    }

    [Fact]
    public async Task LexIdentifier()
    {
        var sourceText = TestData.Css.EXAMPLE_TEXT_21_LINES
            .ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var expectedTextEditorTextSpans = new[]
        {
        new TextEditorTextSpan( 108, 112, (byte)CssDecorationKind.Identifier, resourceUri, sourceText),
        new TextEditorTextSpan( 269, 271, (byte)CssDecorationKind.Identifier, resourceUri, sourceText),
        new TextEditorTextSpan(409, 418, (byte)CssDecorationKind.Identifier, resourceUri, sourceText),
        new TextEditorTextSpan(500, 506, (byte)CssDecorationKind.Identifier, resourceUri, sourceText),
    };

        var cssLexer = new TextEditorCssLexer(resourceUri);

        var textEditorTextSpans = await cssLexer.Lex(
            sourceText,
            Key<RenderState>.NewKey());

        textEditorTextSpans = textEditorTextSpans
            .Where(x => x.DecorationByte == (byte)CssDecorationKind.Identifier)
            .OrderBy(x => x.StartingIndexInclusive)
            .ToImmutableArray();

        Assert.Equal(expectedTextEditorTextSpans, textEditorTextSpans);
    }
}