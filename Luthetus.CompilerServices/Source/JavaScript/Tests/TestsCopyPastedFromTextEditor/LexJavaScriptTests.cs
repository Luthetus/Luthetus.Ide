namespace Luthetus.CompilerServices.Lang.JavaScript.Tests.TestsCopyPastedFromTextEditor;

public class LexJavaScriptTests
{
    // 2023-02-23: Tests need to be rewritten with the new GenericSyntaxTree
    //
    // [Fact]
    // public async Task LexKeywords()
    // {
    //     var text = TestData.JavaScript.EXAMPLE_TEXT
    //         .ReplaceLineEndings("\n");
    //
    //     var expectedKeywordTextEditorTextSpans = new[]
    //     {
    //         new TextEditorTextSpan(137, 140, (byte)JavaScriptDecorationKind.Keyword),
    //         new TextEditorTextSpan(246, 254, (byte)JavaScriptDecorationKind.Keyword),
    //         new TextEditorTextSpan(321, 326, (byte)JavaScriptDecorationKind.Keyword),
    //         new TextEditorTextSpan(334, 337, (byte)JavaScriptDecorationKind.Keyword),
    //         new TextEditorTextSpan(353, 358, (byte)JavaScriptDecorationKind.Keyword),
    //         new TextEditorTextSpan(366, 369, (byte)JavaScriptDecorationKind.Keyword),
    //         new TextEditorTextSpan(386, 389, (byte)JavaScriptDecorationKind.Keyword),
    //         new TextEditorTextSpan(420, 423, (byte)JavaScriptDecorationKind.Keyword),
    //         new TextEditorTextSpan(446, 449, (byte)JavaScriptDecorationKind.Keyword),
    //         new TextEditorTextSpan(451, 454, (byte)JavaScriptDecorationKind.Keyword),
    //         new TextEditorTextSpan(481, 483, (byte)JavaScriptDecorationKind.Keyword),
    //         new TextEditorTextSpan(574, 580, (byte)JavaScriptDecorationKind.Keyword),
    //         new TextEditorTextSpan(605, 608, (byte)JavaScriptDecorationKind.Keyword),
    //         new TextEditorTextSpan(627, 632, (byte)JavaScriptDecorationKind.Keyword),
    //         new TextEditorTextSpan(659, 664, (byte)JavaScriptDecorationKind.Keyword),
    //         new TextEditorTextSpan(689, 694, (byte)JavaScriptDecorationKind.Keyword),
    //     };
    //     
    //     var javaScriptLexer = new TextEditorJavaScriptLexer();
    //
    //     var textEditorTextSpans = 
    //         await javaScriptLexer.Lex(text);
    //
    //     textEditorTextSpans = textEditorTextSpans
    //         .Where(x => x.DecorationByte == (byte)JavaScriptDecorationKind.Keyword)
    //         .OrderBy(x => x.StartingIndexInclusive)
    //         .ToImmutableArray();
    //     
    //     Assert.Equal(expectedKeywordTextEditorTextSpans, textEditorTextSpans);
    // }
    //
    // [Fact]
    // public async Task LexComments()
    // {
    //     var text = TestData.JavaScript.EXAMPLE_TEXT
    //         .ReplaceLineEndings("\n");
    //
    //     var expectedKeywordTextEditorTextSpans = new[]
    //     {
    //         new TextEditorTextSpan(0, 63, (byte)JavaScriptDecorationKind.Comment),
    //         new TextEditorTextSpan(64, 135, (byte)JavaScriptDecorationKind.Comment),
    //         new TextEditorTextSpan(185, 209, (byte)JavaScriptDecorationKind.Comment),
    //         new TextEditorTextSpan(211, 244, (byte)JavaScriptDecorationKind.Comment),
    //         new TextEditorTextSpan(294, 316, (byte)JavaScriptDecorationKind.Comment),
    //     };
    //     
    //     var javaScriptLexer = new TextEditorJavaScriptLexer();
    //
    //     var textEditorTextSpans = 
    //         await javaScriptLexer.Lex(text);
    //
    //     textEditorTextSpans = textEditorTextSpans
    //         .Where(x => x.DecorationByte == (byte)JavaScriptDecorationKind.Comment)
    //         .OrderBy(x => x.StartingIndexInclusive)
    //         .ToImmutableArray();
    //     
    //     Assert.Equal(expectedKeywordTextEditorTextSpans, textEditorTextSpans);
    // }
    //
    // [Fact]
    // public async Task LexStrings()
    // {
    //     var text = TestData.JavaScript.EXAMPLE_TEXT
    //         .ReplaceLineEndings("\n");
    //
    //     var expectedKeywordTextEditorTextSpans = new[]
    //     {
    //         new TextEditorTextSpan(154,182, (byte)JavaScriptDecorationKind.String),
    //         new TextEditorTextSpan(432,439, (byte)JavaScriptDecorationKind.String),
    //         new TextEditorTextSpan(617,624, (byte)JavaScriptDecorationKind.String),
    //     };
    //     
    //     var javaScriptLexer = new TextEditorJavaScriptLexer();
    //
    //     var textEditorTextSpans = 
    //         await javaScriptLexer.Lex(text);
    //
    //     textEditorTextSpans = textEditorTextSpans
    //         .Where(x => x.DecorationByte == (byte)JavaScriptDecorationKind.String)
    //         .OrderBy(x => x.StartingIndexInclusive)
    //         .ToImmutableArray();
    //     
    //     Assert.Equal(expectedKeywordTextEditorTextSpans, textEditorTextSpans);
    // }
}