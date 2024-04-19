using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using System.Linq;

namespace Luthetus.TextEditor.Tests.Basis.TextEditors.Models;

/// <summary>
/// <see cref="TextEditorModelExtensionMethods"/>
/// </summary>
public class TextEditorModelExtensionMethodsTests : TextEditorTestBase
{
    /// <summary>
    /// <see cref="TextEditorModelExtensionMethods.GetLineLength(ITextEditorModel, int, bool)"/>
    /// </summary>
    [Fact]
    public void GetLengthOfLine()
    {
        var (inModel, modelModifier) = NotEmptyEditor_TestData_And_PerformPreAssertions(
            resourceUri: new ResourceUri($"/{nameof(NotEmptyEditor_TestData_And_PerformPreAssertions)}.txt"),
            resourceLastWriteTime: DateTime.MinValue,
            fileExtension: ExtensionNoPeriodFacts.TXT,
            content: "\nb9\r9B\r\n\t$; ",
            decorationMapper: null,
            compilerService: null,
            partitionSize: 4096);

        // Line_Index_0: "\n"
        Assert.Equal(0, modelModifier.GetLineLength(lineIndex: 0, includeLineEndingCharacters: false));
        Assert.Equal(1, modelModifier.GetLineLength(lineIndex: 0, includeLineEndingCharacters: true));

        // Line_Index_1: "b9\r"
        Assert.Equal(2, modelModifier.GetLineLength(lineIndex: 1, includeLineEndingCharacters: false));
        Assert.Equal(3, modelModifier.GetLineLength(lineIndex: 1, includeLineEndingCharacters: true));

        // Line_Index_2: "9B\r\n"
        Assert.Equal(2, modelModifier.GetLineLength(lineIndex: 2, includeLineEndingCharacters: false));
        Assert.Equal(4, modelModifier.GetLineLength(lineIndex: 2, includeLineEndingCharacters: true));

        // Line_Index_3: "--->$;∙EOF"
        Assert.Equal(4, modelModifier.GetLineLength(lineIndex: 3, includeLineEndingCharacters: false));
        Assert.Equal(4, modelModifier.GetLineLength(lineIndex: 3, includeLineEndingCharacters: true));
    }

    /// <summary>
    /// <see cref="TextEditorModelExtensionMethods.GetLineRichCharacterRange(ITextEditorModel, int, int)"/>
    /// </summary>
    [Fact]
    public void GetLineRichCharacterRange()
    {
        var content = "\nb9\r9B\r\n\t$; ";

        var (inModel, modelModifier) = NotEmptyEditor_TestData_And_PerformPreAssertions(
            resourceUri: new ResourceUri($"/{nameof(NotEmptyEditor_TestData_And_PerformPreAssertions)}.txt"),
            resourceLastWriteTime: DateTime.MinValue,
            fileExtension: ExtensionNoPeriodFacts.TXT,
            content: content,
            decorationMapper: null,
            compilerService: null,
            partitionSize: 4096);

        var lineRichCharacterRange = modelModifier.GetLineRichCharacterRange(0, modelModifier.LineEndList.Count);

        var lineRangeString = new string(lineRichCharacterRange
            .SelectMany(x => x)
            .Select(x => x.Value)
            .ToArray());
        
        Assert.Equal(content, lineRangeString);
    }

    /// <summary>
    /// <see cref="TextEditorModelExtensionMethods.GetAllText(ITextEditorModel)"/> equals
    /// <see cref="String(char*)"/> constructor used on 
    /// <see cref="TextEditorModelExtensionMethods.GetLineRichCharacterRange(ITextEditorModel, int, int)"/>
    /// after using .SelectMany() to get the values out, and doing .ToArray().
    /// </summary>
    [Fact]
    public void GetAllText_Equals_GetLines_MadeIntoA_String()
    {
        var (inModel, modelModifier) = NotEmptyEditor_TestData_And_PerformPreAssertions(
            resourceUri: new ResourceUri($"/{nameof(NotEmptyEditor_TestData_And_PerformPreAssertions)}.txt"),
            resourceLastWriteTime: DateTime.MinValue,
            fileExtension: ExtensionNoPeriodFacts.TXT,
            content: "\nb9\r9B\r\n\t$; ",
            decorationMapper: null,
            compilerService: null,
            partitionSize: 4096);

        var all_text = modelModifier.GetAllText();
        var all_lines = modelModifier.GetLineRichCharacterRange(0, modelModifier.LineCount);

        var linesString = new string(all_lines
            .SelectMany(x => x)
            .Select(x => x.Value)
            .ToArray());

        Assert.Equal(all_text, linesString);
    }

    /// <summary>
    /// <see cref="TextEditorModelExtensionMethods.GetTabsCountOnSameLineBeforeCursor(ITextEditorModel, int, int)"/>
    /// </summary>
    [Fact]
    public void GetTabsCountOnSameLineBeforeCursor()
    {
        var (inModel, modelModifier) = NotEmptyEditor_TestData_And_PerformPreAssertions(
            resourceUri: new ResourceUri($"/{nameof(NotEmptyEditor_TestData_And_PerformPreAssertions)}.txt"),
            resourceLastWriteTime: DateTime.MinValue,
            fileExtension: ExtensionNoPeriodFacts.TXT,
            content: "\nb9\r9B\r\n\t$; ",
            decorationMapper: null,
            compilerService: null,
            partitionSize: 4096);

        // Pattern:
        // -------
        // Invoke 'GetTabsCountOnSameLineBeforeCursor' with the start of the line,
        // then separately with the end of the line.

        // Line_Index_0: "\n"
        Assert.Equal(0, modelModifier.GetTabsCountOnSameLineBeforeCursor(lineIndex: 0, columnIndex: 0));
        Assert.Equal(0, modelModifier.GetTabsCountOnSameLineBeforeCursor(lineIndex: 0, columnIndex: 1));

        // Line_Index_1: "b9\r"
        Assert.Equal(0, modelModifier.GetTabsCountOnSameLineBeforeCursor(lineIndex: 1, columnIndex: 0));
        Assert.Equal(0, modelModifier.GetTabsCountOnSameLineBeforeCursor(lineIndex: 1, columnIndex: 2));

        // Line_Index_2: "9B\r\n"
        Assert.Equal(0, modelModifier.GetTabsCountOnSameLineBeforeCursor(lineIndex: 2, columnIndex: 0));
        Assert.Equal(0, modelModifier.GetTabsCountOnSameLineBeforeCursor(lineIndex: 2, columnIndex: 2));

        // Line_Index_3: "--->$;∙EOF"
        Assert.Equal(0, modelModifier.GetTabsCountOnSameLineBeforeCursor(lineIndex: 3, columnIndex: 0));
        Assert.Equal(1, modelModifier.GetTabsCountOnSameLineBeforeCursor(lineIndex: 3, columnIndex: 4));
    }

    /// <summary>
    /// <see cref="TextEditorModelExtensionMethods.GetAllText(ITextEditorModel)"/>
    /// </summary>
    [Fact]
    public void GetAllText()
    {
        var content = "\nb9\r9B\r\n\t$; ";

        var (inModel, modelModifier) = NotEmptyEditor_TestData_And_PerformPreAssertions(
            resourceUri: new ResourceUri($"/{nameof(NotEmptyEditor_TestData_And_PerformPreAssertions)}.txt"),
            resourceLastWriteTime: DateTime.MinValue,
            fileExtension: ExtensionNoPeriodFacts.TXT,
            content: content,
            decorationMapper: null,
            compilerService: null,
            partitionSize: 4096);

        Assert.Equal(content, modelModifier.GetAllText());
    }

    /// <summary>
    /// <see cref="TextEditorModelExtensionMethods.GetPositionIndex(ITextEditorModel, TextEditorCursor)"/>
    /// </summary>
    [Fact]
    public void GetPositionIndex_TextEditorCursor()
    {
        var content = "\nb9\r9B\r\n\t$; ";

        var (inModel, modelModifier) = NotEmptyEditor_TestData_And_PerformPreAssertions(
            resourceUri: new ResourceUri($"/{nameof(NotEmptyEditor_TestData_And_PerformPreAssertions)}.txt"),
            resourceLastWriteTime: DateTime.MinValue,
            fileExtension: ExtensionNoPeriodFacts.TXT,
            content: content,
            decorationMapper: null,
            compilerService: null,
            partitionSize: 4096);

        // Pattern:
        // -------
        // Foreach line in the 'content', invoke 'GetPositionIndex' on every (line, column) and assert the result.
        // These are expected to NOT-throw exceptions.

        // Line_Index_0: "\n"
        Assert.Equal(0, modelModifier.GetPositionIndex(cursor: new TextEditorCursor(lineIndex: 0, columnIndex: 0, isPrimaryCursor: true)));

        // Line_Index_1: "b9\r"
        Assert.Equal(1, modelModifier.GetPositionIndex(cursor: new TextEditorCursor(lineIndex: 1, columnIndex: 0, isPrimaryCursor: true)));
        Assert.Equal(2, modelModifier.GetPositionIndex(cursor: new TextEditorCursor(lineIndex: 1, columnIndex: 1, isPrimaryCursor: true)));
        Assert.Equal(3, modelModifier.GetPositionIndex(cursor: new TextEditorCursor(lineIndex: 1, columnIndex: 2, isPrimaryCursor: true)));

        // Line_Index_2: "9B\r\n"
        Assert.Equal(4, modelModifier.GetPositionIndex(cursor: new TextEditorCursor(lineIndex: 2, columnIndex: 0, isPrimaryCursor: true)));
        Assert.Equal(5, modelModifier.GetPositionIndex(cursor: new TextEditorCursor(lineIndex: 2, columnIndex: 1, isPrimaryCursor: true)));
        Assert.Equal(6, modelModifier.GetPositionIndex(cursor: new TextEditorCursor(lineIndex: 2, columnIndex: 2, isPrimaryCursor: true)));

        // Line_Index_3: "--->$;∙EOF"
        Assert.Equal(8, modelModifier.GetPositionIndex(cursor: new TextEditorCursor(lineIndex: 3, columnIndex: 0, isPrimaryCursor: true)));
        Assert.Equal(9, modelModifier.GetPositionIndex(cursor: new TextEditorCursor(lineIndex: 3, columnIndex: 1, isPrimaryCursor: true)));
        Assert.Equal(10, modelModifier.GetPositionIndex(cursor: new TextEditorCursor(lineIndex: 3, columnIndex: 2, isPrimaryCursor: true)));
        Assert.Equal(11, modelModifier.GetPositionIndex(cursor: new TextEditorCursor(lineIndex: 3, columnIndex: 3, isPrimaryCursor: true)));
        Assert.Equal(12, modelModifier.GetPositionIndex(cursor: new TextEditorCursor(lineIndex: 3, columnIndex: 4, isPrimaryCursor: true)));

        // Pattern:
        // -------
        // Foreach unique out-of-bounds possibility, invoke 'GetPositionIndex' and assert the result.
        // These are expected to throw exceptions.

        // Case: lineIndex out-of-bounds
        // ----
        // Greater-than out-of-bounds: (lineIndex: 4, columnIndex: 0) throws because the modelModifier.LineCount is 4, thereby putting a lineIndex of 4 out-of-bounds.
        Assert.Throws<ApplicationException>(() => modelModifier.GetPositionIndex(cursor: new TextEditorCursor(lineIndex: 0, columnIndex: 1, isPrimaryCursor: true)));
        // Less-than out-of-bounds: (lineIndex: -1, columnIndex: 0) throws because the lineIndex is negative, and would never be valid, thereby putting the lineIndex out-of-bounds.
        Assert.Throws<ApplicationException>(() => modelModifier.GetPositionIndex(cursor: new TextEditorCursor(lineIndex: 0, columnIndex: 1, isPrimaryCursor: true)));

        // Case: columnIndex out-of-bounds
        // ----
        // Greater-than out-of-bounds: (lineIndex: 0, columnIndex: 1) throws because the 0th line is a length of 0, thereby putting the columnIndex out-of-bounds.
        Assert.Throws<ApplicationException>(() => modelModifier.GetPositionIndex(cursor: new TextEditorCursor(lineIndex: 0, columnIndex: 1, isPrimaryCursor: true)));
        // Less-than out-of-bounds: (lineIndex: 0, columnIndex: -1) throws because the columnIndex is negative, and would never be valid, thereby putting the columnIndex out-of-bounds.
        Assert.Throws<ApplicationException>(() => modelModifier.GetPositionIndex(cursor: new TextEditorCursor(lineIndex: 0, columnIndex: 1, isPrimaryCursor: true)));

        // Case: mix the previous bad cases
        // ----
        // Greater-than out-of-bounds: lineIndex; Greater-than out-of-bounds: columnIndex;
        Assert.Throws<ApplicationException>(() => modelModifier.GetPositionIndex(cursor: new TextEditorCursor(lineIndex: 4, columnIndex: 1, isPrimaryCursor: true)));
        // Greater-than out-of-bounds: lineIndex; Less-than out-of-bounds: columnIndex;
        Assert.Throws<ApplicationException>(() => modelModifier.GetPositionIndex(cursor: new TextEditorCursor(lineIndex: 4, columnIndex: -1, isPrimaryCursor: true)));
        // Less-than out-of-bounds: lineIndex; Greater-than out-of-bounds: columnIndex;
        Assert.Throws<ApplicationException>(() => modelModifier.GetPositionIndex(cursor: new TextEditorCursor(lineIndex: -1, columnIndex: 1, isPrimaryCursor: true)));
        // Less-than out-of-bounds: lineIndex; Less-than out-of-bounds: columnIndex;
        Assert.Throws<ApplicationException>(() => modelModifier.GetPositionIndex(cursor: new TextEditorCursor(lineIndex: -1, columnIndex: -1, isPrimaryCursor: true)));
    }

    /// <summary>
    /// <see cref="TextEditorModelExtensionMethods.GetPositionIndex(ITextEditorModel, TextEditorCursorModifier)"/>
    /// </summary>
    [Fact]
    public void GetPositionIndex_TextEditorCursorModifier()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModelExtensionMethods.GetPositionIndex(ITextEditorModel, int, int)"/>
    /// </summary>
    [Fact]
    public void GetPositionIndex_LineIndex_ColumnIndex()
    {
        var (inModel, modelModifier) = NotEmptyEditor_TestData_And_PerformPreAssertions(
            resourceUri: new ResourceUri($"/{nameof(NotEmptyEditor_TestData_And_PerformPreAssertions)}.txt"),
            resourceLastWriteTime: DateTime.MinValue,
            fileExtension: ExtensionNoPeriodFacts.TXT,
            content:
                (
                    "\n" +   // LineFeed
                    "b9" +   // LetterOrDigit-Lowercase
                    "\r" +   // CarriageReturn
                    "9B" +   // LetterOrDigit-Uppercase
                    "\r\n" + // CarriageReturnLineFeed
                    "\t" +   // Tab
                    "$" +    // SpecialCharacter
                    ";" +    // Punctuation
                    " "      // Space
                ),
            decorationMapper: null,
            compilerService: null,
            partitionSize: 4096);

        // Do something
        var cursor = new TextEditorCursor(
            lineIndex: modelModifier.LineCount - 1,
            columnIndex: 0,
            isPrimaryCursor: true);

        var cursorModifier = new TextEditorCursorModifier(cursor);
        var cursorModifierBag = new CursorModifierBagTextEditor(
            Key<TextEditorViewModel>.Empty,
            new List<TextEditorCursorModifier>() { cursorModifier });

        var zero_positionIndex = modelModifier.GetPositionIndex(0, 0);
        var one_positionIndex = modelModifier.GetPositionIndex(1, 0);
        var two_positionIndex = modelModifier.GetPositionIndex(2, 0);
        var three_positionIndex = modelModifier.GetPositionIndex(3, 0);

        var zero_length = modelModifier.GetLineLength(0, true);
        var one_length = modelModifier.GetLineLength(1, true);
        var two_length = modelModifier.GetLineLength(2, true);
        var three_length = modelModifier.GetLineLength(3, true);

        // Post-assertions

        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModelExtensionMethods.GetLineAndColumnIndicesFromPositionIndex(ITextEditorModel, int)"/>
    /// </summary>
    [Fact]
    public void GetLineAndColumnIndicesFromPositionIndex()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModelExtensionMethods.GetCharacter(ITextEditorModel, int)"/>
    /// </summary>
    [Fact]
    public void GetCharacter()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModelExtensionMethods.GetString(ITextEditorModel, int, int)"/>
    /// </summary>
    [Fact]
    public void GetString()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModelExtensionMethods.GetLineTextRange(ITextEditorModel, int, int)"/>
    /// </summary>
    [Fact]
    public void GetLineRange()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModelExtensionMethods.GetWordTextSpan(ITextEditorModel, int)"/>
    /// </summary>
    [Fact]
    public void GetWordTextSpan()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModelExtensionMethods.FindMatches(ITextEditorModel, string)"/>
    /// </summary>
    [Fact]
    public void FindMatches()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModelExtensionMethods.GetLineInformation(ITextEditorModel, int)"/>
    /// </summary>
    [Fact]
    public void GetLineInformation()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModelExtensionMethods.GetLineInformationFromPositionIndex(ITextEditorModel, int)"/>
    /// </summary>
    [Fact]
    public void GetLineInformationFromPositionIndex()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModelExtensionMethods.GetColumnIndexOfCharacterWithDifferingKind(ITextEditorModel, int, int, bool)"/>
    /// </summary>
    [Fact]
    public void GetColumnIndexOfCharacterWithDifferingKind()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModelExtensionMethods.CanUndoEdit(ITextEditorModel)"/>
    /// </summary>
    [Fact]
    public void CanUndoEdit()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModelExtensionMethods.CanRedoEdit(ITextEditorModel)"/>
    /// </summary>
    [Fact]
    public void CanRedoEdit()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModelExtensionMethods.GetCharacterKind(ITextEditorModel, int)"/>
    /// </summary>
    [Fact]
    public void GetCharacterKind()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModelExtensionMethods.ReadPreviousWordOrDefault(ITextEditorModel, int, int, bool)"/>
    /// </summary>
    [Fact]
    public void ReadPreviousWordOrDefault()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModelExtensionMethods.ReadNextWordOrDefault(ITextEditorModel, int, int)"/>
    /// </summary>
    [Fact]
    public void ReadNextWordOrDefault()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModelExtensionMethods.GetTextOffsettingCursor(ITextEditorModel, TextEditorCursor)"/>
    /// </summary>
    [Fact]
    public void GetTextOffsettingCursor()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModelExtensionMethods.GetLineText(ITextEditorModel, int)"/>
    /// </summary>
    [Fact]
    public void GetLineText()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModelExtensionMethods.GetRichCharacterOrDefault(ITextEditorModel, int)"/>
    /// </summary>
    [Fact]
    public void GetRichCharacterOrDefault()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModelExtensionMethods.GetAllRichCharacters(ITextEditorModel)"/>
    /// </summary>
    [Fact]
    public void GetAllRichCharacters()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModelExtensionMethods.GetRichCharacters(ITextEditorModel, int, int)"/>
    /// </summary>
    [Fact]
    public void GetRichCharacters()
    {
        throw new NotImplementedException();
    }
}
