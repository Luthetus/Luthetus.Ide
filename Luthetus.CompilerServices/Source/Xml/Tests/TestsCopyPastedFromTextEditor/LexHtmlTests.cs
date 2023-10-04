using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.CompilerServices.Lang.Xml.Html.Decoration;
using Luthetus.CompilerServices.Lang.Xml.Html.SyntaxActors;
using Luthetus.CompilerServices.Lang.Xml.Tests.TestDataFolder;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System.Collections.Immutable;

namespace Luthetus.CompilerServices.Lang.Xml.Tests.TestsCopyPastedFromTextEditor;

public class LexHtmlTests
{
    [Fact]
    public async Task LexTagNames()
    {
        var sourceText = TestData.Html.EXAMPLE_TEXT
            .ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var expectedTextEditorTextSpans = new[]
        {
        new TextEditorTextSpan(1, 5, (byte)HtmlDecorationKind.TagName, resourceUri, sourceText),
        new TextEditorTextSpan(10, 15, (byte)HtmlDecorationKind.TagName, resourceUri, sourceText),
        new TextEditorTextSpan(42, 47, (byte)HtmlDecorationKind.TagName, resourceUri, sourceText),
        new TextEditorTextSpan(54, 59, (byte)HtmlDecorationKind.TagName, resourceUri, sourceText),
        new TextEditorTextSpan(110, 115, (byte)HtmlDecorationKind.TagName, resourceUri, sourceText),
        new TextEditorTextSpan(142, 147, (byte)HtmlDecorationKind.TagName, resourceUri, sourceText),
        new TextEditorTextSpan(154, 159, (byte)HtmlDecorationKind.TagName, resourceUri, sourceText),
        new TextEditorTextSpan(214, 219, (byte)HtmlDecorationKind.TagName, resourceUri, sourceText),
        new TextEditorTextSpan(267, 269, (byte)HtmlDecorationKind.TagName, resourceUri, sourceText),
        new TextEditorTextSpan(278, 283, (byte)HtmlDecorationKind.TagName, resourceUri, sourceText),
        new TextEditorTextSpan(335, 337, (byte)HtmlDecorationKind.TagName, resourceUri, sourceText),
        new TextEditorTextSpan(346, 351, (byte)HtmlDecorationKind.TagName, resourceUri, sourceText),
        new TextEditorTextSpan(410, 415, (byte)HtmlDecorationKind.TagName, resourceUri, sourceText),
        new TextEditorTextSpan(447, 455, (byte)HtmlDecorationKind.TagName, resourceUri, sourceText),
        new TextEditorTextSpan(479, 485, (byte)HtmlDecorationKind.TagName, resourceUri, sourceText),
        new TextEditorTextSpan(504, 510, (byte)HtmlDecorationKind.TagName, resourceUri, sourceText),
        new TextEditorTextSpan(517, 523, (byte)HtmlDecorationKind.TagName, resourceUri, sourceText),
        new TextEditorTextSpan(542, 548, (byte)HtmlDecorationKind.TagName, resourceUri, sourceText),
        new TextEditorTextSpan(555, 561, (byte)HtmlDecorationKind.TagName, resourceUri, sourceText),
        new TextEditorTextSpan(580, 586, (byte)HtmlDecorationKind.TagName, resourceUri, sourceText),
        new TextEditorTextSpan(594, 602, (byte)HtmlDecorationKind.TagName, resourceUri, sourceText),
        new TextEditorTextSpan(610, 615, (byte)HtmlDecorationKind.TagName, resourceUri, sourceText),
        new TextEditorTextSpan(653, 658, (byte)HtmlDecorationKind.TagName, resourceUri, sourceText),
        new TextEditorTextSpan(684, 689, (byte)HtmlDecorationKind.TagName, resourceUri, sourceText),
        new TextEditorTextSpan(757, 761, (byte)HtmlDecorationKind.TagName, resourceUri, sourceText),
    };

        var htmlLexer = new TextEditorHtmlLexer(resourceUri);

        var textEditorTextSpans = await htmlLexer.Lex(
            sourceText,
            Key<RenderState>.NewKey());

        textEditorTextSpans = textEditorTextSpans
            .Where(x => x.DecorationByte == (byte)HtmlDecorationKind.TagName)
            .OrderBy(x => x.StartingIndexInclusive)
            .ToImmutableArray();

        Assert.Equal(expectedTextEditorTextSpans, textEditorTextSpans);
    }

    [Fact]
    public async Task LexAttributeName()
    {
        var sourceText = TestData.Html.EXAMPLE_TEXT
            .ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var expectedTextEditorTextSpans = new[]
        {
        new TextEditorTextSpan(16, 19, (byte)HtmlDecorationKind.AttributeName, resourceUri, sourceText),
        new TextEditorTextSpan(60, 64, (byte)HtmlDecorationKind.AttributeName, resourceUri, sourceText),
        new TextEditorTextSpan(72, 76, (byte)HtmlDecorationKind.AttributeName, resourceUri, sourceText),
        new TextEditorTextSpan(88, 90, (byte)HtmlDecorationKind.AttributeName, resourceUri, sourceText),
        new TextEditorTextSpan(116, 119, (byte)HtmlDecorationKind.AttributeName, resourceUri, sourceText),
        new TextEditorTextSpan(160, 164, (byte)HtmlDecorationKind.AttributeName, resourceUri, sourceText),
        new TextEditorTextSpan(176, 180, (byte)HtmlDecorationKind.AttributeName, resourceUri, sourceText),
        new TextEditorTextSpan(192, 194, (byte)HtmlDecorationKind.AttributeName, resourceUri, sourceText),
        new TextEditorTextSpan(220, 224, (byte)HtmlDecorationKind.AttributeName, resourceUri, sourceText),
        new TextEditorTextSpan(233, 237, (byte)HtmlDecorationKind.AttributeName, resourceUri, sourceText),
        new TextEditorTextSpan(247, 252, (byte)HtmlDecorationKind.AttributeName, resourceUri, sourceText),
        new TextEditorTextSpan(284, 288, (byte)HtmlDecorationKind.AttributeName, resourceUri, sourceText),
        new TextEditorTextSpan(297, 301, (byte)HtmlDecorationKind.AttributeName, resourceUri, sourceText),
        new TextEditorTextSpan(311, 316, (byte)HtmlDecorationKind.AttributeName, resourceUri, sourceText),
        new TextEditorTextSpan(352, 356, (byte)HtmlDecorationKind.AttributeName, resourceUri, sourceText),
        new TextEditorTextSpan(365, 369, (byte)HtmlDecorationKind.AttributeName, resourceUri, sourceText),
        new TextEditorTextSpan(379, 384, (byte)HtmlDecorationKind.AttributeName, resourceUri, sourceText),
        new TextEditorTextSpan(416, 420, (byte)HtmlDecorationKind.AttributeName, resourceUri, sourceText),
        new TextEditorTextSpan(456, 458, (byte)HtmlDecorationKind.AttributeName, resourceUri, sourceText),
        new TextEditorTextSpan(486, 491, (byte)HtmlDecorationKind.AttributeName, resourceUri, sourceText),
        new TextEditorTextSpan(524, 529, (byte)HtmlDecorationKind.AttributeName, resourceUri, sourceText),
        new TextEditorTextSpan(562, 567, (byte)HtmlDecorationKind.AttributeName, resourceUri, sourceText),
        new TextEditorTextSpan(616, 620, (byte)HtmlDecorationKind.AttributeName, resourceUri, sourceText),
        new TextEditorTextSpan(630, 635, (byte)HtmlDecorationKind.AttributeName, resourceUri, sourceText),
        new TextEditorTextSpan(659, 663, (byte)HtmlDecorationKind.AttributeName, resourceUri, sourceText),
        new TextEditorTextSpan(690, 694, (byte)HtmlDecorationKind.AttributeName, resourceUri, sourceText),
        new TextEditorTextSpan(706, 710, (byte)HtmlDecorationKind.AttributeName, resourceUri, sourceText),
        new TextEditorTextSpan(721, 726, (byte)HtmlDecorationKind.AttributeName, resourceUri, sourceText),
    };

        var htmlLexer = new TextEditorHtmlLexer(resourceUri);

        var textEditorTextSpans = await htmlLexer.Lex(
            sourceText,
            Key<RenderState>.NewKey());

        textEditorTextSpans = textEditorTextSpans
            .Where(x => x.DecorationByte == (byte)HtmlDecorationKind.AttributeName)
            .OrderBy(x => x.StartingIndexInclusive)
            .ToImmutableArray();

        Assert.Equal(expectedTextEditorTextSpans, textEditorTextSpans);
    }

    [Fact]
    public async Task LexAttributeValue()
    {
        var sourceText = TestData.Html.EXAMPLE_TEXT
            .ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var expectedTextEditorTextSpans = new[]
        {
        new TextEditorTextSpan(21, 29, (byte)HtmlDecorationKind.AttributeValue, resourceUri, sourceText),
        new TextEditorTextSpan(66, 70, (byte)HtmlDecorationKind.AttributeValue, resourceUri, sourceText),
        new TextEditorTextSpan(78, 86, (byte)HtmlDecorationKind.AttributeValue, resourceUri, sourceText),
        new TextEditorTextSpan(92, 100, (byte)HtmlDecorationKind.AttributeValue, resourceUri, sourceText),
        new TextEditorTextSpan(121, 129, (byte)HtmlDecorationKind.AttributeValue, resourceUri, sourceText),
        new TextEditorTextSpan(166, 174, (byte)HtmlDecorationKind.AttributeValue, resourceUri, sourceText),
        new TextEditorTextSpan(182, 190, (byte)HtmlDecorationKind.AttributeValue, resourceUri, sourceText),
        new TextEditorTextSpan(196, 204, (byte)HtmlDecorationKind.AttributeValue, resourceUri, sourceText),
        new TextEditorTextSpan(226, 231, (byte)HtmlDecorationKind.AttributeValue, resourceUri, sourceText),
        new TextEditorTextSpan(239, 245, (byte)HtmlDecorationKind.AttributeValue, resourceUri, sourceText),
        new TextEditorTextSpan(254, 258, (byte)HtmlDecorationKind.AttributeValue, resourceUri, sourceText),
        new TextEditorTextSpan(290, 295, (byte)HtmlDecorationKind.AttributeValue, resourceUri, sourceText),
        new TextEditorTextSpan(303, 309, (byte)HtmlDecorationKind.AttributeValue, resourceUri, sourceText),
        new TextEditorTextSpan(318, 324, (byte)HtmlDecorationKind.AttributeValue, resourceUri, sourceText),
        new TextEditorTextSpan(358, 363, (byte)HtmlDecorationKind.AttributeValue, resourceUri, sourceText),
        new TextEditorTextSpan(371, 377, (byte)HtmlDecorationKind.AttributeValue, resourceUri, sourceText),
        new TextEditorTextSpan(386, 391, (byte)HtmlDecorationKind.AttributeValue, resourceUri, sourceText),
        new TextEditorTextSpan(422, 429, (byte)HtmlDecorationKind.AttributeValue, resourceUri, sourceText),
        new TextEditorTextSpan(460, 467, (byte)HtmlDecorationKind.AttributeValue, resourceUri, sourceText),
        new TextEditorTextSpan(493, 500, (byte)HtmlDecorationKind.AttributeValue, resourceUri, sourceText),
        new TextEditorTextSpan(531, 538, (byte)HtmlDecorationKind.AttributeValue, resourceUri, sourceText),
        new TextEditorTextSpan(569, 576, (byte)HtmlDecorationKind.AttributeValue, resourceUri, sourceText),
        new TextEditorTextSpan(622, 628, (byte)HtmlDecorationKind.AttributeValue, resourceUri, sourceText),
        new TextEditorTextSpan(637, 643, (byte)HtmlDecorationKind.AttributeValue, resourceUri, sourceText),
        new TextEditorTextSpan(665, 670, (byte)HtmlDecorationKind.AttributeValue, resourceUri, sourceText),
        new TextEditorTextSpan(696, 704, (byte)HtmlDecorationKind.AttributeValue, resourceUri, sourceText),
        new TextEditorTextSpan(712, 719, (byte)HtmlDecorationKind.AttributeValue, resourceUri, sourceText),
        new TextEditorTextSpan(728, 735, (byte)HtmlDecorationKind.AttributeValue, resourceUri, sourceText),
    };

        var htmlLexer = new TextEditorHtmlLexer(resourceUri);

        var textEditorTextSpans = await htmlLexer.Lex(
            sourceText,
            Key<RenderState>.NewKey());

        textEditorTextSpans = textEditorTextSpans
            .Where(x => x.DecorationByte == (byte)HtmlDecorationKind.AttributeValue)
            .OrderBy(x => x.StartingIndexInclusive)
            .ToImmutableArray();

        Assert.Equal(expectedTextEditorTextSpans, textEditorTextSpans);
    }

    // TODO: A more extensive test should be made this is only top level comment tags. I checked manually and a HTML element containing a comment works fine but there should be a test for deeply nested comments.
    [Fact]
    public async Task LexComment()
    {
        var sourceText = TestData.Html.EXAMPLE_TEXT
            .ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var expectedTextEditorTextSpans = new[]
        {
        new TextEditorTextSpan(35, 82, (byte)HtmlDecorationKind.Comment, resourceUri, sourceText),
        new TextEditorTextSpan(118, 140, (byte)HtmlDecorationKind.Comment, resourceUri, sourceText),
        new TextEditorTextSpan(154, 210, (byte)HtmlDecorationKind.Comment, resourceUri, sourceText),
        new TextEditorTextSpan(426, 450, (byte)HtmlDecorationKind.Comment, resourceUri, sourceText),
    };

        var htmlLexer = new TextEditorHtmlLexer(resourceUri);

        var textEditorTextSpans = await htmlLexer.Lex(
            sourceText,
            Key<RenderState>.NewKey());

        textEditorTextSpans = textEditorTextSpans
            .Where(x => x.DecorationByte == (byte)HtmlDecorationKind.Comment)
            .OrderBy(x => x.StartingIndexInclusive)
            .ToImmutableArray();

        Assert.Equal(expectedTextEditorTextSpans, textEditorTextSpans);
    }
}