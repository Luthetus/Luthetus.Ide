using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.CompilerServices.Lang.Json.Json.Decoration;
using Luthetus.CompilerServices.Lang.Json.Json.SyntaxActors;
using Luthetus.CompilerServices.Lang.Json.Tests.TestDataFolder;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System.Collections.Immutable;

namespace Luthetus.CompilerServices.Lang.Json.Tests.TestsCopyPastedFromTextEditor;

public class LexJsonTests
{
    [Fact]
    public async Task LexPropertyKey()
    {
        var sourceText = TestData.Json.EXAMPLE_TEXT_LAUNCH_SETTINGS
            .ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var expectedTextEditorTextSpans = new[]
        {
        new TextEditorTextSpan(5, 16, (byte)JsonDecorationKind.PropertyKey, resourceUri, sourceText),
        new TextEditorTextSpan(30, 51, (byte)JsonDecorationKind.PropertyKey, resourceUri, sourceText),
        new TextEditorTextSpan(70, 93, (byte)JsonDecorationKind.PropertyKey, resourceUri, sourceText),
        new TextEditorTextSpan(111, 121, (byte)JsonDecorationKind.PropertyKey, resourceUri, sourceText),
        new TextEditorTextSpan(139, 153, (byte)JsonDecorationKind.PropertyKey, resourceUri, sourceText),
        new TextEditorTextSpan(195, 202, (byte)JsonDecorationKind.PropertyKey, resourceUri, sourceText),
        new TextEditorTextSpan(233, 241, (byte)JsonDecorationKind.PropertyKey, resourceUri, sourceText),
        new TextEditorTextSpan(255, 287, (byte)JsonDecorationKind.PropertyKey, resourceUri, sourceText),
        new TextEditorTextSpan(305, 316, (byte)JsonDecorationKind.PropertyKey, resourceUri, sourceText),
        new TextEditorTextSpan(343, 360, (byte)JsonDecorationKind.PropertyKey, resourceUri, sourceText),
        new TextEditorTextSpan(382, 395, (byte)JsonDecorationKind.PropertyKey, resourceUri, sourceText),
        new TextEditorTextSpan(417, 431, (byte)JsonDecorationKind.PropertyKey, resourceUri, sourceText),
        new TextEditorTextSpan(495, 515, (byte)JsonDecorationKind.PropertyKey, resourceUri, sourceText),
        new TextEditorTextSpan(537, 559, (byte)JsonDecorationKind.PropertyKey, resourceUri, sourceText),
        new TextEditorTextSpan(610, 621, (byte)JsonDecorationKind.PropertyKey, resourceUri, sourceText),
        new TextEditorTextSpan(639, 650, (byte)JsonDecorationKind.PropertyKey, resourceUri, sourceText),
        new TextEditorTextSpan(680, 693, (byte)JsonDecorationKind.PropertyKey, resourceUri, sourceText),
        new TextEditorTextSpan(715, 735, (byte)JsonDecorationKind.PropertyKey, resourceUri, sourceText),
        new TextEditorTextSpan(757, 779, (byte)JsonDecorationKind.PropertyKey, resourceUri, sourceText),
    };

        var jsonLexer = new TextEditorJsonLexer(resourceUri);

        var textEditorTextSpans = await jsonLexer.Lex(
            sourceText,
            Key<RenderState>.NewKey());

        textEditorTextSpans = textEditorTextSpans
            .Where(x => x.DecorationByte == (byte)JsonDecorationKind.PropertyKey)
            .OrderBy(x => x.StartingIndexInclusive)
            .ToImmutableArray();

        Assert.Equal(expectedTextEditorTextSpans, textEditorTextSpans);
    }

    [Fact]
    public async Task LexValueString()
    {
        var sourceText = TestData.Json.EXAMPLE_TEXT_LAUNCH_SETTINGS
            .ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var expectedTextEditorTextSpans = new[]
        {
        new TextEditorTextSpan(157, 179, (byte)JsonDecorationKind.String, resourceUri, sourceText),
        new TextEditorTextSpan(320, 327, (byte)JsonDecorationKind.String, resourceUri, sourceText),
        new TextEditorTextSpan(435, 479, (byte)JsonDecorationKind.String, resourceUri, sourceText),
        new TextEditorTextSpan(563, 574, (byte)JsonDecorationKind.String, resourceUri, sourceText),
        new TextEditorTextSpan(654, 664, (byte)JsonDecorationKind.String, resourceUri, sourceText),
        new TextEditorTextSpan(783, 794, (byte)JsonDecorationKind.String, resourceUri, sourceText),
    };

        var jsonLexer = new TextEditorJsonLexer(resourceUri);

        var textEditorTextSpans = await jsonLexer.Lex(
            sourceText,
            Key<RenderState>.NewKey());

        textEditorTextSpans = textEditorTextSpans
            .Where(x => x.DecorationByte == (byte)JsonDecorationKind.String)
            .OrderBy(x => x.StartingIndexInclusive)
            .ToImmutableArray();

        Assert.Equal(expectedTextEditorTextSpans, textEditorTextSpans);
    }

    [Fact]
    public async Task LexValueKeyword()
    {
        var sourceText = TestData.Json.EXAMPLE_TEXT_LAUNCH_SETTINGS
            .ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var expectedTextEditorTextSpans = new[]
        {
        new TextEditorTextSpan(54, 59, (byte)JsonDecorationKind.Keyword, resourceUri, sourceText),
        new TextEditorTextSpan(96, 100, (byte)JsonDecorationKind.Keyword, resourceUri, sourceText),
        new TextEditorTextSpan(363, 367, (byte)JsonDecorationKind.Keyword, resourceUri, sourceText),
        new TextEditorTextSpan(398, 402, (byte)JsonDecorationKind.Keyword, resourceUri, sourceText),
        new TextEditorTextSpan(696, 700, (byte)JsonDecorationKind.Keyword, resourceUri, sourceText),
    };

        var jsonLexer = new TextEditorJsonLexer(resourceUri);

        var textEditorTextSpans = await jsonLexer.Lex(
            sourceText,
            Key<RenderState>.NewKey());

        textEditorTextSpans = textEditorTextSpans
            .Where(x => x.DecorationByte == (byte)JsonDecorationKind.Keyword)
            .OrderBy(x => x.StartingIndexInclusive)
            .ToImmutableArray();

        Assert.Equal(expectedTextEditorTextSpans, textEditorTextSpans);
    }

    [Fact]
    public async Task LexValueInteger()
    {
        var sourceText = TestData.Json.EXAMPLE_TEXT_WITH_COMMENTS
            .ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var expectedTextEditorTextSpans = new[]
        {
        new TextEditorTextSpan(78, 81, (byte)JsonDecorationKind.Integer, resourceUri, sourceText),
        new TextEditorTextSpan(390, 395, (byte)JsonDecorationKind.Integer, resourceUri, sourceText),
    };

        var jsonLexer = new TextEditorJsonLexer(resourceUri);

        var textEditorTextSpans = await jsonLexer.Lex(
            sourceText,
            Key<RenderState>.NewKey());

        textEditorTextSpans = textEditorTextSpans
            .Where(x => x.DecorationByte == (byte)JsonDecorationKind.Integer)
            .OrderBy(x => x.StartingIndexInclusive)
            .ToImmutableArray();

        Assert.Equal(expectedTextEditorTextSpans, textEditorTextSpans);
    }

    [Fact]
    public async Task LexValueNumber()
    {
        var sourceText = TestData.Json.EXAMPLE_TEXT_WITH_COMMENTS
            .ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var expectedTextEditorTextSpans = new[]
        {
        new TextEditorTextSpan(36, 41, (byte)JsonDecorationKind.Number, resourceUri, sourceText),
    };

        var jsonLexer = new TextEditorJsonLexer(resourceUri);

        var textEditorTextSpans = await jsonLexer.Lex(
            sourceText,
            Key<RenderState>.NewKey());

        textEditorTextSpans = textEditorTextSpans
            .Where(x => x.DecorationByte == (byte)JsonDecorationKind.Number)
            .OrderBy(x => x.StartingIndexInclusive)
            .ToImmutableArray();

        Assert.Equal(expectedTextEditorTextSpans, textEditorTextSpans);
    }

    [Fact]
    public Task LexCommentLine()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public Task LexCommentBlock()
    {
        throw new NotImplementedException();
    }
}