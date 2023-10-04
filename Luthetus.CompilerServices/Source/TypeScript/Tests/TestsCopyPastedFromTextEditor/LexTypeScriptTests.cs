namespace Luthetus.CompilerServices.Lang.TypeScript.Tests.TestsCopyPastedFromTextEditor;

public class LexTypeScriptTests
{
    // 2023-02-23: Tests need to be rewritten with the new GenericSyntaxTree
    //
    // [Fact]
    // public async Task LexKeywords()
    // {
    //     var text = TestData.TypeScript.EXAMPLE_TEXT
    //         .ReplaceLineEndings("\n");
    //
    //     var expectedKeywordTextEditorTextSpans = new[]
    //     {
    //         new TextEditorTextSpan(0, 6, (byte)TypeScriptDecorationKind.Keyword),
    //         new TextEditorTextSpan(31, 35, (byte)TypeScriptDecorationKind.Keyword),
    //         new TextEditorTextSpan(60, 66, (byte)TypeScriptDecorationKind.Keyword),
    //         new TextEditorTextSpan(82, 86, (byte)TypeScriptDecorationKind.Keyword),
    //         new TextEditorTextSpan(108, 113, (byte)TypeScriptDecorationKind.Keyword),
    //         new TextEditorTextSpan(148, 157, (byte)TypeScriptDecorationKind.Keyword),
    //         new TextEditorTextSpan(481, 489, (byte)TypeScriptDecorationKind.Keyword),
    //         new TextEditorTextSpan(556, 562, (byte)TypeScriptDecorationKind.Keyword),
    //         new TextEditorTextSpan(739, 744, (byte)TypeScriptDecorationKind.Keyword),
    //         new TextEditorTextSpan(825, 830, (byte)TypeScriptDecorationKind.Keyword),
    //     };
    //     
    //     var typeScriptLexer = new TextEditorTypeScriptLexer();
    //
    //     var textEditorTextSpans = 
    //         await typeScriptLexer.Lex(text);
    //
    //     textEditorTextSpans = textEditorTextSpans
    //         .Where(x => x.DecorationByte == (byte)TypeScriptDecorationKind.Keyword)
    //         .OrderBy(x => x.StartingIndexInclusive)
    //         .ToImmutableArray();
    //     
    //     Assert.Equal(expectedKeywordTextEditorTextSpans, textEditorTextSpans);
    // }
    //
    // [Fact]
    // public async Task LexComments()
    // {
    //     var text = TestData.TypeScript.EXAMPLE_TEXT
    //         .ReplaceLineEndings("\n");
    //
    //     var expectedKeywordTextEditorTextSpans = new[]
    //     {
    //         new TextEditorTextSpan(181, 241, (byte)TypeScriptDecorationKind.Comment),
    //         new TextEditorTextSpan(264, 479, (byte)TypeScriptDecorationKind.Comment),
    //     };
    //     
    //     var javaScriptLexer = new TextEditorTypeScriptLexer();
    //
    //     var textEditorTextSpans = 
    //         await javaScriptLexer.Lex(text);
    //
    //     textEditorTextSpans = textEditorTextSpans
    //         .Where(x => x.DecorationByte == (byte)TypeScriptDecorationKind.Comment)
    //         .OrderBy(x => x.StartingIndexInclusive)
    //         .ToImmutableArray();
    //     
    //     Assert.Equal(expectedKeywordTextEditorTextSpans, textEditorTextSpans);
    // }
    //
    // [Fact]
    // public async Task LexStrings()
    // {
    //     var text = TestData.TypeScript.EXAMPLE_TEXT
    //         .ReplaceLineEndings("\n");
    //
    //     var expectedKeywordTextEditorTextSpans = new[]
    //     {
    //         new TextEditorTextSpan(36, 57, (byte)TypeScriptDecorationKind.String),
    //         new TextEditorTextSpan(87, 105, (byte)TypeScriptDecorationKind.String),
    //         new TextEditorTextSpan(808, 822, (byte)TypeScriptDecorationKind.String),
    //         new TextEditorTextSpan(895, 911, (byte)TypeScriptDecorationKind.String),
    //     };
    //     
    //     var javaScriptLexer = new TextEditorTypeScriptLexer();
    //
    //     var textEditorTextSpans = 
    //         await javaScriptLexer.Lex(text);
    //
    //     textEditorTextSpans = textEditorTextSpans
    //         .Where(x => x.DecorationByte == (byte)TypeScriptDecorationKind.String)
    //         .OrderBy(x => x.StartingIndexInclusive)
    //         .ToImmutableArray();
    //     
    //     Assert.Equal(expectedKeywordTextEditorTextSpans, textEditorTextSpans);
    // }
}