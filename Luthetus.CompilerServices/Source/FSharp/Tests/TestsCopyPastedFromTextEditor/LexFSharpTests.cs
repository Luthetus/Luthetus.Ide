namespace Luthetus.CompilerServices.Lang.FSharp.Tests.TestsCopyPastedFromTextEditor;

public class LexFSharpTests
{
    // 2023-02-23: Tests need to be rewritten with the new GenericSyntaxTree
    //
    // [Fact]
    // public async Task LexKeywords()
    // {
    //     var text = TestData.FSharp.EXAMPLE_TEXT_21_LINES
    //         .ReplaceLineEndings("\n");
    //
    //     var expectedKeywordTextEditorTextSpans = new[]
    //     {
    //         new TextEditorTextSpan(0, 3, (byte)FSharpDecorationKind.Keyword),
    //         new TextEditorTextSpan(18, 23, (byte)FSharpDecorationKind.Keyword),
    //         new TextEditorTextSpan(26, 30, (byte)FSharpDecorationKind.Keyword),
    //         new TextEditorTextSpan(65, 68, (byte)FSharpDecorationKind.Keyword),
    //         new TextEditorTextSpan(69, 76, (byte)FSharpDecorationKind.Keyword),
    //         new TextEditorTextSpan(94, 97, (byte)FSharpDecorationKind.Keyword),
    //         new TextEditorTextSpan(98, 105, (byte)FSharpDecorationKind.Keyword),
    //         new TextEditorTextSpan(123, 126, (byte)FSharpDecorationKind.Keyword),
    //         new TextEditorTextSpan(129, 131, (byte)FSharpDecorationKind.Keyword),
    //         new TextEditorTextSpan(145, 147, (byte)FSharpDecorationKind.Keyword),
    //         new TextEditorTextSpan(160, 163, (byte)FSharpDecorationKind.Keyword),
    //         new TextEditorTextSpan(381, 384, (byte)FSharpDecorationKind.Keyword),
    //     };
    //     
    //     var fSharpLexer = new TextEditorFSharpLexer();
    //
    //     var textEditorTextSpans = 
    //         await fSharpLexer.Lex(text);
    //
    //     textEditorTextSpans = textEditorTextSpans
    //         .Where(x => x.DecorationByte == (byte)FSharpDecorationKind.Keyword)
    //         .OrderBy(x => x.StartingIndexInclusive)
    //         .ToImmutableArray();
    //
    //     Assert.Equal(expectedKeywordTextEditorTextSpans, textEditorTextSpans);
    // }
    //
    // [Fact]
    // public async Task LexComments()
    // {
    //     var text = TestData.FSharp.EXAMPLE_TEXT_21_LINES
    //         .ReplaceLineEndings("\n");
    //
    //     var expectedKeywordTextEditorTextSpans = new[]
    //     {
    //         new TextEditorTextSpan(247, 278, (byte)FSharpDecorationKind.Comment),
    //         new TextEditorTextSpan(280, 345, (byte)FSharpDecorationKind.Comment),
    //         new TextEditorTextSpan(347, 379, (byte)FSharpDecorationKind.Comment),
    //     };
    //     
    //     var fSharpLexer = new TextEditorFSharpLexer();
    //
    //     var textEditorTextSpans = 
    //         await fSharpLexer.Lex(text);
    //
    //     textEditorTextSpans = textEditorTextSpans
    //         .Where(x => x.DecorationByte == (byte)FSharpDecorationKind.Comment)
    //         .OrderBy(x => x.StartingIndexInclusive)
    //         .ToImmutableArray();
    //
    //     Assert.Equal(expectedKeywordTextEditorTextSpans, textEditorTextSpans);
    // }
    //
    // [Fact]
    // public async Task LexStrings()
    // {
    //     var text = TestData.FSharp.EXAMPLE_TEXT_21_LINES
    //         .ReplaceLineEndings("\n");
    //
    //     var expectedKeywordTextEditorTextSpans = new[]
    //     {
    //         new TextEditorTextSpan(247, 278, 2),
    //         new TextEditorTextSpan(280, 345, 2),
    //         new TextEditorTextSpan(347, 379, 2),
    //     };
    //     
    //     var fSharpLexer = new TextEditorFSharpLexer();
    //
    //     var textEditorTextSpans = 
    //         await fSharpLexer.Lex(text);
    //
    //     textEditorTextSpans = textEditorTextSpans
    //         .Where(x => x.DecorationByte == (byte)FSharpDecorationKind.Comment)
    //         .OrderBy(x => x.StartingIndexInclusive)
    //         .ToImmutableArray();
    //
    //     Assert.Equal(expectedKeywordTextEditorTextSpans, textEditorTextSpans);
    // }
}