using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Exceptions;

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
    /// <see cref="TextEditorModelExtensionMethods.GetTabCountOnSameLineBeforeCursor(ITextEditorModel, int, int)"/>
    /// </summary>
    [Fact]
    public void GetTabCountOnSameLineBeforeCursor()
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
        // Invoke 'GetTabsCountOnSameLineBeforeCursor' with the start of the line, then separately with the end of the line.
        // These are expected to NOT-throw exceptions.

        // Line_Index_0: "\n"
        Assert.Equal(0, modelModifier.GetTabCountOnSameLineBeforeCursor(lineIndex: 0, columnIndex: 0));

        // Line_Index_1: "b9\r"
        Assert.Equal(0, modelModifier.GetTabCountOnSameLineBeforeCursor(lineIndex: 1, columnIndex: 0));
        Assert.Equal(0, modelModifier.GetTabCountOnSameLineBeforeCursor(lineIndex: 1, columnIndex: 2));

        // Line_Index_2: "9B\r\n"
        Assert.Equal(0, modelModifier.GetTabCountOnSameLineBeforeCursor(lineIndex: 2, columnIndex: 0));
        Assert.Equal(0, modelModifier.GetTabCountOnSameLineBeforeCursor(lineIndex: 2, columnIndex: 2));

        // Line_Index_3: "--->$;∙EOF"
        Assert.Equal(0, modelModifier.GetTabCountOnSameLineBeforeCursor(lineIndex: 3, columnIndex: 0));
        Assert.Equal(1, modelModifier.GetTabCountOnSameLineBeforeCursor(lineIndex: 3, columnIndex: 4));

        // Pattern:
        // -------
        // Foreach unique out-of-bounds possibility, invoke 'GetTabCountOnSameLineBeforeCursor' and assert the result.
        // These are expected to throw exceptions.

        // Case: lineIndex out-of-bounds
        // ----
        // Greater-than out-of-bounds: (lineIndex: 4, columnIndex: 0) throws because the modelModifier.LineCount is 4, thereby putting a lineIndex of 4 out-of-bounds.
        Assert.Throws<LuthetusTextEditorException>(() => modelModifier.GetTabCountOnSameLineBeforeCursor(lineIndex: 4, columnIndex: 0));
        // Less-than out-of-bounds: (lineIndex: -1, columnIndex: 0) throws because the lineIndex is negative, and would never be valid, thereby putting the lineIndex out-of-bounds.
        Assert.Throws<LuthetusTextEditorException>(() => modelModifier.GetTabCountOnSameLineBeforeCursor(lineIndex: -1, columnIndex: 0));

        // Case: columnIndex out-of-bounds
        // ----
        // Greater-than out-of-bounds: (lineIndex: 0, columnIndex: 1) throws because the 0th line is a length of 0, thereby putting the columnIndex out-of-bounds.
        Assert.Throws<LuthetusTextEditorException>(() => modelModifier.GetTabCountOnSameLineBeforeCursor(lineIndex: 0, columnIndex: 1));
        // Less-than out-of-bounds: (lineIndex: 0, columnIndex: -1) throws because the columnIndex is negative, and would never be valid, thereby putting the columnIndex out-of-bounds.
        Assert.Throws<LuthetusTextEditorException>(() => modelModifier.GetTabCountOnSameLineBeforeCursor(lineIndex: 0, columnIndex: -1));

        // Case: mix the previous bad cases
        // ----
        // Greater-than out-of-bounds: lineIndex; Greater-than out-of-bounds: columnIndex;
        Assert.Throws<LuthetusTextEditorException>(() => modelModifier.GetTabCountOnSameLineBeforeCursor(lineIndex: 4, columnIndex: 1));
        // Greater-than out-of-bounds: lineIndex; Less-than out-of-bounds: columnIndex;
        Assert.Throws<LuthetusTextEditorException>(() => modelModifier.GetTabCountOnSameLineBeforeCursor(lineIndex: 4, columnIndex: -1));
        // Less-than out-of-bounds: lineIndex; Greater-than out-of-bounds: columnIndex;
        Assert.Throws<LuthetusTextEditorException>(() => modelModifier.GetTabCountOnSameLineBeforeCursor(lineIndex: -1, columnIndex: 1));
        // Less-than out-of-bounds: lineIndex; Less-than out-of-bounds: columnIndex;
        Assert.Throws<LuthetusTextEditorException>(() => modelModifier.GetTabCountOnSameLineBeforeCursor(lineIndex: -1, columnIndex: -1));
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
        Assert.Throws<LuthetusTextEditorException>(() => modelModifier.GetPositionIndex(cursor: new TextEditorCursor(lineIndex: 4, columnIndex: 0, isPrimaryCursor: true)));
        // Less-than out-of-bounds: (lineIndex: -1, columnIndex: 0) throws because the lineIndex is negative, and would never be valid, thereby putting the lineIndex out-of-bounds.
        Assert.Throws<LuthetusTextEditorException>(() => modelModifier.GetPositionIndex(cursor: new TextEditorCursor(lineIndex: -1, columnIndex: 0, isPrimaryCursor: true)));

        // Case: columnIndex out-of-bounds
        // ----
        // Greater-than out-of-bounds: (lineIndex: 0, columnIndex: 1) throws because the 0th line is a length of 0, thereby putting the columnIndex out-of-bounds.
        Assert.Throws<LuthetusTextEditorException>(() => modelModifier.GetPositionIndex(cursor: new TextEditorCursor(lineIndex: 0, columnIndex: 1, isPrimaryCursor: true)));
        // Less-than out-of-bounds: (lineIndex: 0, columnIndex: -1) throws because the columnIndex is negative, and would never be valid, thereby putting the columnIndex out-of-bounds.
        Assert.Throws<LuthetusTextEditorException>(() => modelModifier.GetPositionIndex(cursor: new TextEditorCursor(lineIndex: 0, columnIndex: -1, isPrimaryCursor: true)));

        // Case: mix the previous bad cases
        // ----
        // Greater-than out-of-bounds: lineIndex; Greater-than out-of-bounds: columnIndex;
        Assert.Throws<LuthetusTextEditorException>(() => modelModifier.GetPositionIndex(cursor: new TextEditorCursor(lineIndex: 4, columnIndex: 1, isPrimaryCursor: true)));
        // Greater-than out-of-bounds: lineIndex; Less-than out-of-bounds: columnIndex;
        Assert.Throws<LuthetusTextEditorException>(() => modelModifier.GetPositionIndex(cursor: new TextEditorCursor(lineIndex: 4, columnIndex: -1, isPrimaryCursor: true)));
        // Less-than out-of-bounds: lineIndex; Greater-than out-of-bounds: columnIndex;
        Assert.Throws<LuthetusTextEditorException>(() => modelModifier.GetPositionIndex(cursor: new TextEditorCursor(lineIndex: -1, columnIndex: 1, isPrimaryCursor: true)));
        // Less-than out-of-bounds: lineIndex; Less-than out-of-bounds: columnIndex;
        Assert.Throws<LuthetusTextEditorException>(() => modelModifier.GetPositionIndex(cursor: new TextEditorCursor(lineIndex: -1, columnIndex: -1, isPrimaryCursor: true)));
    }

    /// <summary>
    /// <see cref="TextEditorModelExtensionMethods.GetPositionIndex(ITextEditorModel, TextEditorCursorModifier)"/>
    /// </summary>
    [Fact]
    public void GetPositionIndex_TextEditorCursorModifier()
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
        Assert.Equal(0, modelModifier.GetPositionIndex(cursorModifier: new TextEditorCursorModifier(new(lineIndex: 0, columnIndex: 0, isPrimaryCursor: true))));

        // Line_Index_1: "b9\r"
        Assert.Equal(1, modelModifier.GetPositionIndex(cursorModifier: new TextEditorCursorModifier(new(lineIndex: 1, columnIndex: 0, isPrimaryCursor: true))));
        Assert.Equal(2, modelModifier.GetPositionIndex(cursorModifier: new TextEditorCursorModifier(new(lineIndex: 1, columnIndex: 1, isPrimaryCursor: true))));
        Assert.Equal(3, modelModifier.GetPositionIndex(cursorModifier: new TextEditorCursorModifier(new(lineIndex: 1, columnIndex: 2, isPrimaryCursor: true))));

        // Line_Index_2: "9B\r\n"
        Assert.Equal(4, modelModifier.GetPositionIndex(cursorModifier: new TextEditorCursorModifier(new(lineIndex: 2, columnIndex: 0, isPrimaryCursor: true))));
        Assert.Equal(5, modelModifier.GetPositionIndex(cursorModifier: new TextEditorCursorModifier(new(lineIndex: 2, columnIndex: 1, isPrimaryCursor: true))));
        Assert.Equal(6, modelModifier.GetPositionIndex(cursorModifier: new TextEditorCursorModifier(new(lineIndex: 2, columnIndex: 2, isPrimaryCursor: true))));

        // Line_Index_3: "--->$;∙EOF"
        Assert.Equal(8, modelModifier.GetPositionIndex(cursorModifier: new TextEditorCursorModifier(new(lineIndex: 3, columnIndex: 0, isPrimaryCursor: true))));
        Assert.Equal(9, modelModifier.GetPositionIndex(cursorModifier: new TextEditorCursorModifier(new(lineIndex: 3, columnIndex: 1, isPrimaryCursor: true))));
        Assert.Equal(10, modelModifier.GetPositionIndex(cursorModifier: new TextEditorCursorModifier(new(lineIndex: 3, columnIndex: 2, isPrimaryCursor: true))));
        Assert.Equal(11, modelModifier.GetPositionIndex(cursorModifier: new TextEditorCursorModifier(new(lineIndex: 3, columnIndex: 3, isPrimaryCursor: true))));
        Assert.Equal(12, modelModifier.GetPositionIndex(cursorModifier: new TextEditorCursorModifier(new(lineIndex: 3, columnIndex: 4, isPrimaryCursor: true))));

        // Pattern:
        // -------
        // Foreach unique out-of-bounds possibility, invoke 'GetPositionIndex' and assert the result.
        // These are expected to throw exceptions.

        // Case: lineIndex out-of-bounds
        // ----
        // Greater-than out-of-bounds: (lineIndex: 4, columnIndex: 0) throws because the modelModifier.LineCount is 4, thereby putting a lineIndex of 4 out-of-bounds.
        Assert.Throws<LuthetusTextEditorException>(() => modelModifier.GetPositionIndex(cursorModifier: new TextEditorCursorModifier(new(lineIndex: 4, columnIndex: 0, isPrimaryCursor: true))));
        // Less-than out-of-bounds: (lineIndex: -1, columnIndex: 0) throws because the lineIndex is negative, and would never be valid, thereby putting the lineIndex out-of-bounds.
        Assert.Throws<LuthetusTextEditorException>(() => modelModifier.GetPositionIndex(cursorModifier: new TextEditorCursorModifier(new(lineIndex: -1, columnIndex: 0, isPrimaryCursor: true))));

        // Case: columnIndex out-of-bounds
        // ----
        // Greater-than out-of-bounds: (lineIndex: 0, columnIndex: 1) throws because the 0th line is a length of 0, thereby putting the columnIndex out-of-bounds.
        Assert.Throws<LuthetusTextEditorException>(() => modelModifier.GetPositionIndex(cursorModifier: new TextEditorCursorModifier(new(lineIndex: 0, columnIndex: 1, isPrimaryCursor: true))));
        // Less-than out-of-bounds: (lineIndex: 0, columnIndex: -1) throws because the columnIndex is negative, and would never be valid, thereby putting the columnIndex out-of-bounds.
        Assert.Throws<LuthetusTextEditorException>(() => modelModifier.GetPositionIndex(cursorModifier: new TextEditorCursorModifier(new(lineIndex: 0, columnIndex: -1, isPrimaryCursor: true))));

        // Case: mix the previous bad cases
        // ----
        // Greater-than out-of-bounds: lineIndex; Greater-than out-of-bounds: columnIndex;
        Assert.Throws<LuthetusTextEditorException>(() => modelModifier.GetPositionIndex(cursorModifier: new TextEditorCursorModifier(new(lineIndex: 4, columnIndex: 1, isPrimaryCursor: true))));
        // Greater-than out-of-bounds: lineIndex; Less-than out-of-bounds: columnIndex;
        Assert.Throws<LuthetusTextEditorException>(() => modelModifier.GetPositionIndex(cursorModifier: new TextEditorCursorModifier(new(lineIndex: 4, columnIndex: -1, isPrimaryCursor: true))));
        // Less-than out-of-bounds: lineIndex; Greater-than out-of-bounds: columnIndex;
        Assert.Throws<LuthetusTextEditorException>(() => modelModifier.GetPositionIndex(cursorModifier: new TextEditorCursorModifier(new(lineIndex: -1, columnIndex: 1, isPrimaryCursor: true))));
        // Less-than out-of-bounds: lineIndex; Less-than out-of-bounds: columnIndex;
        Assert.Throws<LuthetusTextEditorException>(() => modelModifier.GetPositionIndex(cursorModifier: new TextEditorCursorModifier(new(lineIndex: -1, columnIndex: -1, isPrimaryCursor: true))));
    }

    /// <summary>
    /// <see cref="TextEditorModelExtensionMethods.GetPositionIndex(ITextEditorModel, int, int)"/>
    /// </summary>
    [Fact]
    public void GetPositionIndex_LineIndex_ColumnIndex()
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
        Assert.Equal(0, modelModifier.GetPositionIndex(lineIndex: 0, columnIndex: 0));

        // Line_Index_1: "b9\r"
        Assert.Equal(1, modelModifier.GetPositionIndex(lineIndex: 1, columnIndex: 0));
        Assert.Equal(2, modelModifier.GetPositionIndex(lineIndex: 1, columnIndex: 1));
        Assert.Equal(3, modelModifier.GetPositionIndex(lineIndex: 1, columnIndex: 2));

        // Line_Index_2: "9B\r\n"
        Assert.Equal(4, modelModifier.GetPositionIndex(lineIndex: 2, columnIndex: 0));
        Assert.Equal(5, modelModifier.GetPositionIndex(lineIndex: 2, columnIndex: 1));
        Assert.Equal(6, modelModifier.GetPositionIndex(lineIndex: 2, columnIndex: 2));

        // Line_Index_3: "--->$;∙EOF"
        Assert.Equal(8, modelModifier.GetPositionIndex(lineIndex: 3, columnIndex: 0));
        Assert.Equal(9, modelModifier.GetPositionIndex(lineIndex: 3, columnIndex: 1));
        Assert.Equal(10, modelModifier.GetPositionIndex(lineIndex: 3, columnIndex: 2));
        Assert.Equal(11, modelModifier.GetPositionIndex(lineIndex: 3, columnIndex: 3));
        Assert.Equal(12, modelModifier.GetPositionIndex(lineIndex: 3, columnIndex: 4));

        // Pattern:
        // -------
        // Foreach unique out-of-bounds possibility, invoke 'GetPositionIndex' and assert the result.
        // These are expected to throw exceptions.

        // Case: lineIndex out-of-bounds
        // ----
        // Greater-than out-of-bounds: (lineIndex: 4, columnIndex: 0) throws because the modelModifier.LineCount is 4, thereby putting a lineIndex of 4 out-of-bounds.
        Assert.Throws<LuthetusTextEditorException>(() => modelModifier.GetPositionIndex(lineIndex: 4, columnIndex: 0));
        // Less-than out-of-bounds: (lineIndex: -1, columnIndex: 0) throws because the lineIndex is negative, and would never be valid, thereby putting the lineIndex out-of-bounds.
        Assert.Throws<LuthetusTextEditorException>(() => modelModifier.GetPositionIndex(lineIndex: -1, columnIndex: 0));

        // Case: columnIndex out-of-bounds
        // ----
        // Greater-than out-of-bounds: (lineIndex: 0, columnIndex: 1) throws because the 0th line is a length of 0, thereby putting the columnIndex out-of-bounds.
        Assert.Throws<LuthetusTextEditorException>(() => modelModifier.GetPositionIndex(lineIndex: 0, columnIndex: 1));
        // Less-than out-of-bounds: (lineIndex: 0, columnIndex: -1) throws because the columnIndex is negative, and would never be valid, thereby putting the columnIndex out-of-bounds.
        Assert.Throws<LuthetusTextEditorException>(() => modelModifier.GetPositionIndex(lineIndex: 0, columnIndex: -1));

        // Case: mix the previous bad cases
        // ----
        // Greater-than out-of-bounds: lineIndex; Greater-than out-of-bounds: columnIndex;
        Assert.Throws<LuthetusTextEditorException>(() => modelModifier.GetPositionIndex(lineIndex: 4, columnIndex: 1));
        // Greater-than out-of-bounds: lineIndex; Less-than out-of-bounds: columnIndex;
        Assert.Throws<LuthetusTextEditorException>(() => modelModifier.GetPositionIndex(lineIndex: 4, columnIndex: -1));
        // Less-than out-of-bounds: lineIndex; Greater-than out-of-bounds: columnIndex;
        Assert.Throws<LuthetusTextEditorException>(() => modelModifier.GetPositionIndex(lineIndex: -1, columnIndex: 1));
        // Less-than out-of-bounds: lineIndex; Less-than out-of-bounds: columnIndex;
        Assert.Throws<LuthetusTextEditorException>(() => modelModifier.GetPositionIndex(lineIndex: -1, columnIndex: -1));
    }

    /// <summary>
    /// <see cref="TextEditorModelExtensionMethods.GetLineAndColumnIndicesFromPositionIndex(ITextEditorModel, int)"/>
    /// </summary>
    [Fact]
    public void GetLineAndColumnIndicesFromPositionIndex()
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
        // Foreach positionIndex in the 'content', invoke 'GetLineAndColumnIndicesFromPositionIndex' on every positionIndex and assert the result.
        // These are expected to NOT-throw exceptions.

        // Line_Index_0: "\n"
        Assert.Equal((lineIndex: 0, columnIndex: 0), modelModifier.GetLineAndColumnIndicesFromPositionIndex(0));

        // Line_Index_1: "b9\r"
        Assert.Equal((lineIndex: 1, columnIndex: 0), modelModifier.GetLineAndColumnIndicesFromPositionIndex(1));
        Assert.Equal((lineIndex: 1, columnIndex: 1), modelModifier.GetLineAndColumnIndicesFromPositionIndex(2));
        Assert.Equal((lineIndex: 1, columnIndex: 2), modelModifier.GetLineAndColumnIndicesFromPositionIndex(3));

        // Line_Index_2: "9B\r\n"
        Assert.Equal((lineIndex: 2, columnIndex: 0), modelModifier.GetLineAndColumnIndicesFromPositionIndex(4));
        Assert.Equal((lineIndex: 2, columnIndex: 1), modelModifier.GetLineAndColumnIndicesFromPositionIndex(5));
        Assert.Equal((lineIndex: 2, columnIndex: 2), modelModifier.GetLineAndColumnIndicesFromPositionIndex(6));

        // Line_Index_3: "--->$;∙EOF"
        Assert.Equal((lineIndex: 3, columnIndex: 0), modelModifier.GetLineAndColumnIndicesFromPositionIndex(8));
        Assert.Equal((lineIndex: 3, columnIndex: 1), modelModifier.GetLineAndColumnIndicesFromPositionIndex(9));
        Assert.Equal((lineIndex: 3, columnIndex: 2), modelModifier.GetLineAndColumnIndicesFromPositionIndex(10));
        Assert.Equal((lineIndex: 3, columnIndex: 3), modelModifier.GetLineAndColumnIndicesFromPositionIndex(11));
        Assert.Equal((lineIndex: 3, columnIndex: 4), modelModifier.GetLineAndColumnIndicesFromPositionIndex(12));

        // Pattern:
        // -------
        // Foreach unique out-of-bounds possibility, invoke 'GetLineAndColumnIndicesFromPositionIndex' and assert the result.
        // These are expected to throw exceptions.

        // Case: lineIndex out-of-bounds
        // ----
        // Greater-than out-of-bounds: positionIndex of 13 throws because the modelModifier.DocumentLength is 12, thereby putting a positionIndex of 13 out-of-bounds.
        Assert.Throws<LuthetusTextEditorException>(() => modelModifier.GetLineAndColumnIndicesFromPositionIndex(13));
        // Less-than out-of-bounds: positionIndex of -1 throws because the positionIndex is negative, and would never be valid, thereby putting the positionIndex out-of-bounds.
        Assert.Throws<LuthetusTextEditorException>(() => modelModifier.GetLineAndColumnIndicesFromPositionIndex(-1));
    }

    /// <summary>
    /// <see cref="TextEditorModelExtensionMethods.GetCharacter(ITextEditorModel, int)"/>
    /// </summary>
    [Fact]
    public void GetCharacter()
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
        // Foreach positionIndex in the 'content', invoke 'GetCharacter' on every positionIndex and assert the result.
        // These are expected to NOT-throw exceptions.

        // Line_Index_0: "\n"
        Assert.Equal('\n', modelModifier.GetCharacter(0));

        // Line_Index_1: "b9\r"
        Assert.Equal('b', modelModifier.GetCharacter(1));
        Assert.Equal('9', modelModifier.GetCharacter(2));
        Assert.Equal('\r', modelModifier.GetCharacter(3));

        // Line_Index_2: "9B\r\n"
        Assert.Equal('9', modelModifier.GetCharacter(4));
        Assert.Equal('B', modelModifier.GetCharacter(5));
        Assert.Equal('\r', modelModifier.GetCharacter(6));

        // Line_Index_3: "--->$;∙EOF"
        Assert.Equal('\t', modelModifier.GetCharacter(8));
        Assert.Equal('$', modelModifier.GetCharacter(9));
        Assert.Equal(';', modelModifier.GetCharacter(10));
        Assert.Equal(' ', modelModifier.GetCharacter(11));
        Assert.Equal('\0', modelModifier.GetCharacter(12));

        // Pattern:
        // -------
        // Foreach unique out-of-bounds possibility, invoke 'GetCharacter' and assert the result.
        // These are expected to throw exceptions.

        // Case: lineIndex out-of-bounds
        // ----
        // Greater-than out-of-bounds: positionIndex of 13 throws because the modelModifier.DocumentLength is 12, thereby putting a positionIndex of 13 out-of-bounds.
        Assert.Throws<LuthetusTextEditorException>(() => modelModifier.GetCharacter(13));
        // Less-than out-of-bounds: positionIndex of -1 throws because the positionIndex is negative, and would never be valid, thereby putting the positionIndex out-of-bounds.
        Assert.Throws<LuthetusTextEditorException>(() => modelModifier.GetCharacter(-1));
    }

    /// <summary>
    /// <see cref="TextEditorModelExtensionMethods.GetString(ITextEditorModel, int, int)"/>
    /// </summary>
    [Fact]
    public void GetString()
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
        // Foreach positionIndex in the 'content', invoke 'GetString' on every positionIndex and assert the result.
        // These are expected to NOT-throw exceptions.

        // Line_Index_0: "\n"
        Assert.Equal(string.Empty, modelModifier.GetString(0, 0));
        Assert.Equal("\n", modelModifier.GetString(0, 1));

        // Line_Index_1: "b9\r"
        Assert.Equal("b", modelModifier.GetString(1, 1));
        Assert.Equal("b9", modelModifier.GetString(1, 2));
        Assert.Equal(string.Empty, modelModifier.GetString(2, 0));
        Assert.Equal("\r", modelModifier.GetString(3, 1));

        // Line_Index_2: "9B\r\n"
        Assert.Equal("9B\r", modelModifier.GetString(4, 3));
        Assert.Equal("9B\r\n", modelModifier.GetString(4, 4));
        Assert.Equal("9B\r\n\t$", modelModifier.GetString(4, 6));
        Assert.Equal("\r\n", modelModifier.GetString(6, 2));

        // Line_Index_3: "--->$;∙EOF"
        Assert.Equal("$; ", modelModifier.GetString(9, 4));
        Assert.Equal(string.Empty, modelModifier.GetString(12, 1));

        // Pattern:
        // -------
        // Foreach unique out-of-bounds possibility, invoke 'GetString' and assert the result.
        // These are expected to throw exceptions.

        Assert.Throws<LuthetusTextEditorException>(() => modelModifier.GetString(-1, 1));
        Assert.Throws<LuthetusTextEditorException>(() => modelModifier.GetString(-1, -1));
        Assert.Throws<LuthetusTextEditorException>(() => modelModifier.GetString(-1, 13));
        Assert.Throws<LuthetusTextEditorException>(() => modelModifier.GetString(2, -1));
        Assert.Throws<LuthetusTextEditorException>(() => modelModifier.GetString(13, 1));
    }

    /// <summary>
    /// <see cref="TextEditorModelExtensionMethods.GetLineTextRange(ITextEditorModel, int, int)"/>
    /// </summary>
    [Fact]
    public void GetLineTextRange()
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
        // These are expected to NOT-throw exceptions.

        // Line_Index_0: "\n"
        Assert.Equal(string.Empty, modelModifier.GetLineTextRange(0, 0));
        Assert.Equal("\n", modelModifier.GetLineTextRange(0, 1));

        // Line_Index_1: "b9\r"
        Assert.Equal("b9\r9B\r\n", modelModifier.GetLineTextRange(1, 2));

        // Line_Index_2: "9B\r\n"
        Assert.Equal("9B\r\n\t$; ", modelModifier.GetLineTextRange(2, 3));

        // Line_Index_3: "--->$;∙EOF"
        Assert.Equal("\t$; ", modelModifier.GetLineTextRange(3, 1));

        // Pattern:
        // -------
        // Foreach unique out-of-bounds possibility, invoke 'GetLineTextRange' and assert the result.
        // These are expected to throw exceptions.

        Assert.Throws<LuthetusTextEditorException>(() => modelModifier.GetLineTextRange(-1, 1));
        Assert.Throws<LuthetusTextEditorException>(() => modelModifier.GetLineTextRange(4, 1));
        Assert.Throws<LuthetusTextEditorException>(() => modelModifier.GetLineTextRange(-1, -1));
        Assert.Throws<LuthetusTextEditorException>(() => modelModifier.GetLineTextRange(4, -1));
    }

    /// <summary>
    /// <see cref="TextEditorModelExtensionMethods.GetWordTextSpan(ITextEditorModel, int)"/>
    /// </summary>
    [Fact]
    public void GetWordTextSpan()
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

        // "\nb9\r9B\r\n\t$; "
        //     ^cursor is between the 'b' and the '9'
        {
            var textSpan = modelModifier.GetWordTextSpan(2);
            Assert.NotNull(textSpan);
            Assert.Equal(1, textSpan!.StartingIndexInclusive);
            Assert.Equal(3, textSpan.EndingIndexExclusive);
            Assert.Equal("b9", textSpan.GetText());
            Assert.Equal(2, textSpan.Length);
        }

        // "\nb9\r9B\r\n\t$; "
        //        ^cursor is to the left of the "9B" text.
        {
            var textSpan = modelModifier.GetWordTextSpan(4);
            Assert.NotNull(textSpan);
            Assert.Equal(4, textSpan!.StartingIndexInclusive);
            Assert.Equal(6, textSpan.EndingIndexExclusive);
            Assert.Equal("9B", textSpan.GetText());
            Assert.Equal(2, textSpan.Length);
        }

        // "\nb9\r9B\r\n\t$; "
        //          ^cursor is to the right of the "9B" text.
        {
            var textSpan = modelModifier.GetWordTextSpan(6);
            Assert.NotNull(textSpan);
            Assert.Equal(4, textSpan!.StartingIndexInclusive);
            Assert.Equal(6, textSpan.EndingIndexExclusive);
            Assert.Equal("9B", textSpan.GetText());
            Assert.Equal(2, textSpan.Length);
        }

        // Out-of-bounds small number
        Assert.Throws<LuthetusTextEditorException>(() => modelModifier.GetWordTextSpan(-1));
        // Out-of-bounds large number
        Assert.Throws<LuthetusTextEditorException>(() => modelModifier.GetWordTextSpan(13));
    }

    /// <summary>
    /// <see cref="TextEditorModelExtensionMethods.FindMatches(ITextEditorModel, string)"/>
    /// </summary>
    [Fact]
    public void FindMatches()
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

        // "\nb9\r9B\r\n\t$; "
        //     ^  ^           # 2 total
        {
            var matchList = modelModifier.FindMatches("9");
            Assert.Equal(2, matchList.Length);

            // Index 0
            {
                var match = matchList[0];
                Assert.Equal(2, match.StartingIndexInclusive);
                Assert.Equal(3, match.EndingIndexExclusive);
                Assert.Equal("9", match.GetText());
                Assert.Equal(1, match.Length);
            }

            // Index 1
            {
                var match = matchList[1];
                Assert.Equal(4, match.StartingIndexInclusive);
                Assert.Equal(5, match.EndingIndexExclusive);
                Assert.Equal("9", match.GetText());
                Assert.Equal(1, match.Length);
            }
        }

        // "\nb9\r9B\r\n\t$; "
        //        ^           # 1 total
        {
            var matchList = modelModifier.FindMatches("9B");
            Assert.Single(matchList);

            // Index 0
            {
                var match = matchList[0];
                Assert.Equal(4, match.StartingIndexInclusive);
                Assert.Equal(6, match.EndingIndexExclusive);
                Assert.Equal("9B", match.GetText());
                Assert.Equal(2, match.Length);
            }
        }
    }

    /// <summary>
    /// <see cref="TextEditorModelExtensionMethods.GetLineInformation(ITextEditorModel, int)"/>
    /// </summary>
    [Fact]
    public void GetLineInformation()
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

        // Line_Index_0: "\n"
        {
            var line = modelModifier.GetLineInformation(0);
            Assert.Equal(0, line.Index);
            Assert.Equal(0, line.StartPositionIndexInclusive);
            Assert.Equal(1, line.EndPositionIndexExclusive);
            Assert.Equal(LineEnd.StartOfFile, line.LowerLineEnd);
            Assert.Equal(modelModifier.LineEndList[0], line.UpperLineEnd);
        }

        // Line_Index_1: "b9\r"
        {
            var line = modelModifier.GetLineInformation(1);
            Assert.Equal(1, line.Index);
            Assert.Equal(1, line.StartPositionIndexInclusive);
            Assert.Equal(4, line.EndPositionIndexExclusive);
            Assert.Equal(modelModifier.LineEndList[0], line.LowerLineEnd);
            Assert.Equal(modelModifier.LineEndList[1], line.UpperLineEnd);
        }

        // Line_Index_2: "9B\r\n"
        {
            var line = modelModifier.GetLineInformation(2);
            Assert.Equal(2, line.Index);
            Assert.Equal(4, line.StartPositionIndexInclusive);
            Assert.Equal(8, line.EndPositionIndexExclusive);
            Assert.Equal(modelModifier.LineEndList[1], line.LowerLineEnd);
            Assert.Equal(modelModifier.LineEndList[2], line.UpperLineEnd);
        }

        // Line_Index_3: "--->$;∙EOF"
        {
            var line = modelModifier.GetLineInformation(3);
            Assert.Equal(3, line.Index);
            Assert.Equal(8, line.StartPositionIndexInclusive);
            Assert.Equal(12, line.EndPositionIndexExclusive);
            Assert.Equal(modelModifier.LineEndList[2], line.LowerLineEnd);
            Assert.Equal(modelModifier.LineEndList[3], line.UpperLineEnd);
        }

        // Out-of-range small number
        Assert.Throws<LuthetusTextEditorException>(() => modelModifier.GetLineInformation(-1));
        // Out-of-range large number
        Assert.Throws<LuthetusTextEditorException>(() => modelModifier.GetLineInformation(4));
    }

    /// <summary>
    /// <see cref="TextEditorModelExtensionMethods.GetLineInformationFromPositionIndex(ITextEditorModel, int)"/>
    /// </summary>
    [Fact]
    public void GetLineInformationFromPositionIndex()
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


        // Line_Index_0: "\n"
        {
            // Invoke at start of a line
            var line = modelModifier.GetLineInformationFromPositionIndex(0);
            Assert.Equal(0, line.Index);
            Assert.Equal(0, line.StartPositionIndexInclusive);
            Assert.Equal(1, line.EndPositionIndexExclusive);
            Assert.Equal(LineEnd.StartOfFile, line.LowerLineEnd);
            Assert.Equal(modelModifier.LineEndList[0], line.UpperLineEnd);
        }

        // Line_Index_1: "b9\r"
        {
            // Invoke at middle of word "b9"
            var line = modelModifier.GetLineInformationFromPositionIndex(2);
            Assert.Equal(1, line.Index);
            Assert.Equal(1, line.StartPositionIndexInclusive);
            Assert.Equal(4, line.EndPositionIndexExclusive);
            Assert.Equal(modelModifier.LineEndList[0], line.LowerLineEnd);
            Assert.Equal(modelModifier.LineEndList[1], line.UpperLineEnd);
        }

        // Line_Index_2: "9B\r\n"
        {
            // Invoke at middle of CarriageReturnLineFeed "\r\n"
            var line = modelModifier.GetLineInformationFromPositionIndex(7);
            Assert.Equal(2, line.Index);
            Assert.Equal(4, line.StartPositionIndexInclusive);
            Assert.Equal(8, line.EndPositionIndexExclusive);
            Assert.Equal(modelModifier.LineEndList[1], line.LowerLineEnd);
            Assert.Equal(modelModifier.LineEndList[2], line.UpperLineEnd);
        }

        // Line_Index_3: "--->$;∙EOF"
        {
            // Invoke at the final positionIndex
            var line = modelModifier.GetLineInformationFromPositionIndex(12);
            Assert.Equal(3, line.Index);
            Assert.Equal(8, line.StartPositionIndexInclusive);
            Assert.Equal(12, line.EndPositionIndexExclusive);
            Assert.Equal(modelModifier.LineEndList[2], line.LowerLineEnd);
            Assert.Equal(modelModifier.LineEndList[3], line.UpperLineEnd);
        }

        // Out-of-range small number
        Assert.Throws<LuthetusTextEditorException>(() => modelModifier.GetLineInformation(-1));
        // Out-of-range large number
        Assert.Throws<LuthetusTextEditorException>(() => modelModifier.GetLineInformation(13));
    }

    /// <summary>
    /// <see cref="TextEditorModelExtensionMethods.GetColumnIndexOfCharacterWithDifferingKind(ITextEditorModel, int, int, bool)"/>
    /// </summary>
    [Fact]
    public void GetColumnIndexOfCharacterWithDifferingKind()
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

        Assert.Equal(2, modelModifier.GetColumnIndexOfCharacterWithDifferingKind(lineIndex: 1, columnIndex: 0, moveBackwards: false));

        // -1 is the result because this method stays on the provided lineIndex, it will NOT wrap to the previous line.
        Assert.Equal(-1, modelModifier.GetColumnIndexOfCharacterWithDifferingKind(lineIndex: 1, columnIndex: 2, moveBackwards: true));

        // lineIndex out of bounds small number
        Assert.Throws<LuthetusTextEditorException>(() =>
            modelModifier.GetColumnIndexOfCharacterWithDifferingKind(lineIndex: -1, columnIndex: 0, moveBackwards: true));

        // lineIndex out of bounds large number
        Assert.Throws<LuthetusTextEditorException>(() =>
            modelModifier.GetColumnIndexOfCharacterWithDifferingKind(lineIndex: 4, columnIndex: 0, moveBackwards: true));

        // columnIndex out of bounds small number
        Assert.Throws<LuthetusTextEditorException>(() =>
            modelModifier.GetColumnIndexOfCharacterWithDifferingKind(lineIndex: 0, columnIndex: -1, moveBackwards: true));

        // columnIndex out of bounds large number
        Assert.Throws<LuthetusTextEditorException>(() =>
            modelModifier.GetColumnIndexOfCharacterWithDifferingKind(lineIndex: 0, columnIndex: 2, moveBackwards: true));
    }

    /// <summary>
    /// <see cref="TextEditorModelExtensionMethods.CanUndoEdit(ITextEditorModel)"/>
    /// </summary>
    [Fact]
    public void CanUndoEdit()
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

        // The constructor for the text editor does NOT set an EditBlock
        Assert.False(modelModifier.CanUndoEdit());

        // Insert text
        var insertedContent = "a";

        var cursor = new TextEditorCursor(
                    lineIndex: 0,
                    columnIndex: 0,
                    isPrimaryCursor: true);
        var cursorModifier = new TextEditorCursorModifier(cursor);
        var cursorModifierBag = new CursorModifierBagTextEditor(
            Key<TextEditorViewModel>.Empty,
            new List<TextEditorCursorModifier>() { cursorModifier });

        modelModifier.Insert(insertedContent, cursorModifierBag, cancellationToken: CancellationToken.None);
        Assert.Equal(insertedContent + content, modelModifier.GetAllText());

        // Text insertion will set an EditBlock
        Assert.True(modelModifier.CanUndoEdit());
    }

    /// <summary>
    /// <see cref="TextEditorModelExtensionMethods.CanRedoEdit(ITextEditorModel)"/>
    /// </summary>
    [Fact]
    public void CanRedoEdit()
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

        // The constructor for the text editor does NOT set an EditBlock
        Assert.False(modelModifier.CanUndoEdit());
        Assert.False(modelModifier.CanRedoEdit());

        // Insert text
        var insertedContent = "a";
        
        var cursor = new TextEditorCursor(
                    lineIndex: 0,
                    columnIndex: 0,
                    isPrimaryCursor: true);
        var cursorModifier = new TextEditorCursorModifier(cursor);
        var cursorModifierBag = new CursorModifierBagTextEditor(
            Key<TextEditorViewModel>.Empty,
            new List<TextEditorCursorModifier>() { cursorModifier });

        modelModifier.Insert(insertedContent, cursorModifierBag, cancellationToken: CancellationToken.None);
        Assert.Equal(insertedContent + content, modelModifier.GetAllText());

        // Text insertion will set an EditBlock
        Assert.True(modelModifier.CanUndoEdit());

        // Undo an edit
        modelModifier.UndoEdit();

        // Undoing an edit will allow Redo
        Assert.True(modelModifier.CanRedoEdit());
    }

    /// <summary>
    /// <see cref="TextEditorModelExtensionMethods.GetCharacterKind(ITextEditorModel, int)"/>
    /// </summary>
    [Fact]
    public void GetCharacterKind()
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

        // "\nb9\r9B\r\n\t$; "
        //  ^
        Assert.Equal(CharacterKind.Whitespace, modelModifier.GetCharacterKind(0));

        // "\nb9\r9B\r\n\t$; "
        //    ^
        Assert.Equal(CharacterKind.LetterOrDigit, modelModifier.GetCharacterKind(1));

        // "\nb9\r9B\r\n\t$; "
        //     ^
        Assert.Equal(CharacterKind.LetterOrDigit, modelModifier.GetCharacterKind(2));

        // "\nb9\r9B\r\n\t$; "
        //      ^
        Assert.Equal(CharacterKind.Whitespace, modelModifier.GetCharacterKind(3));

        // "\nb9\r9B\r\n\t$; "
        //        ^
        Assert.Equal(CharacterKind.LetterOrDigit, modelModifier.GetCharacterKind(4));

        // "\nb9\r9B\r\n\t$; "
        //         ^
        Assert.Equal(CharacterKind.LetterOrDigit, modelModifier.GetCharacterKind(5));

        // "\nb9\r9B\r\n\t$; "
        //          ^
        Assert.Equal(CharacterKind.Whitespace, modelModifier.GetCharacterKind(6));

        // "\nb9\r9B\r\n\t$; "
        //            ^
        Assert.Equal(CharacterKind.Whitespace, modelModifier.GetCharacterKind(7));

        // "\nb9\r9B\r\n\t$; "
        //              ^
        Assert.Equal(CharacterKind.Whitespace, modelModifier.GetCharacterKind(8));

        // "\nb9\r9B\r\n\t$; "
        //                ^
        Assert.Equal(CharacterKind.Punctuation, modelModifier.GetCharacterKind(9));

        // "\nb9\r9B\r\n\t$; "
        //                 ^
        Assert.Equal(CharacterKind.Punctuation, modelModifier.GetCharacterKind(10));

        // "\nb9\r9B\r\n\t$; "
        //                  ^
        Assert.Equal(CharacterKind.Whitespace, modelModifier.GetCharacterKind(11));

        // "\nb9\r9B\r\n\t$; "
        //                   ^
        Assert.Equal(CharacterKind.Bad, modelModifier.GetCharacterKind(12));

        Assert.Throws<LuthetusTextEditorException>(() => modelModifier.GetCharacterKind(-1));
        Assert.Throws<LuthetusTextEditorException>(() => modelModifier.GetCharacterKind(13));
    }

    /// <summary>
    /// <see cref="TextEditorModelExtensionMethods.ReadPreviousWordOrDefault(ITextEditorModel, int, int, bool)"/>
    /// </summary>
    [Fact]
    public void ReadPreviousWordOrDefault()
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

        // Case: after CharacterKind.LetterOrDigit
        //
        // "\nb9\r9B\r\n\t$; "
        //      ^cursor is after the text "b9"
        {
            var word = modelModifier.ReadPreviousWordOrDefault(1, 2);
            Assert.Equal("b9", word);
        }

        // Case: after CharacterKind.Whitespace
        //
        // "\nb9\r9B\r\n\t$; "
        //        ^cursor is to the right of the "\r" text.
        {
            var word = modelModifier.ReadPreviousWordOrDefault(2, 0);
            Assert.Null(word);
        }

        // Case: after CharacterKind.Punctuation
        //
        // "\nb9\r9B\r\n\t$; "
        //                 ^cursor is to the right of the "$" text.
        {
            var textSpan = modelModifier.ReadPreviousWordOrDefault(3, 2);
            Assert.Null(textSpan);
        }

        // These cases are expected to throw an exception
        {
            // lineIndex is bad
            {
                // Out-of-bounds small number
                Assert.Throws<LuthetusTextEditorException>(() => modelModifier.ReadPreviousWordOrDefault(-1, 0));
                // Out-of-bounds large number
                Assert.Throws<LuthetusTextEditorException>(() => modelModifier.ReadPreviousWordOrDefault(4, 0));
            }

            // columnIndex is bad
            {
                // Out-of-bounds small number
                Assert.Throws<LuthetusTextEditorException>(() => modelModifier.ReadPreviousWordOrDefault(2, -1));
                // Out-of-bounds large number
                Assert.Throws<LuthetusTextEditorException>(() => modelModifier.ReadPreviousWordOrDefault(0, 2));
            }

            // Mixture is bad
            {
                // Out-of-bounds (small number, small number)
                Assert.Throws<LuthetusTextEditorException>(() => modelModifier.ReadPreviousWordOrDefault(-1, -1));
                // Out-of-bounds (small number, large number)
                Assert.Throws<LuthetusTextEditorException>(() => modelModifier.ReadPreviousWordOrDefault(-1, 2));
                // Out-of-bounds (large number, small number)
                Assert.Throws<LuthetusTextEditorException>(() => modelModifier.ReadPreviousWordOrDefault(4, -1));
                // Out-of-bounds (large number, large number)
                Assert.Throws<LuthetusTextEditorException>(() => modelModifier.ReadPreviousWordOrDefault(4, 4));
            }
        }
    }

    /// <summary>
    /// <see cref="TextEditorModelExtensionMethods.ReadNextWordOrDefault(ITextEditorModel, int, int)"/>
    /// </summary>
    [Fact]
    public void ReadNextWordOrDefault()
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

        // Case: before CharacterKind.LetterOrDigit
        //
        // "\nb9\r9B\r\n\t$; "
        //    ^cursor is to the left of the "b9" text.
        {
            var word = modelModifier.ReadNextWordOrDefault(1, 0);
            Assert.Equal("b9", word);
        }

        // Case: before CharacterKind.Whitespace
        //
        // "\nb9\r9B\r\n\t$; "
        //      ^cursor is to the left of the "\r" text.
        {
            var word = modelModifier.ReadNextWordOrDefault(1, 2);
            Assert.Null(word);
        }

        // Case: before CharacterKind.Punctuation
        //
        // "\nb9\r9B\r\n\t$; "
        //                ^cursor is to the left of the "$" text.
        {
            var textSpan = modelModifier.ReadNextWordOrDefault(3, 1);
            Assert.Null(textSpan);
        }

        // These cases are expected to throw an exception
        {
            // lineIndex is bad
            {
                // Out-of-bounds small number
                Assert.Throws<LuthetusTextEditorException>(() => modelModifier.ReadNextWordOrDefault(-1, 0));
                // Out-of-bounds large number
                Assert.Throws<LuthetusTextEditorException>(() => modelModifier.ReadNextWordOrDefault(4, 0));
            }

            // columnIndex is bad
            {
                // Out-of-bounds small number
                Assert.Throws<LuthetusTextEditorException>(() => modelModifier.ReadNextWordOrDefault(2, -1));
                // Out-of-bounds large number
                Assert.Throws<LuthetusTextEditorException>(() => modelModifier.ReadNextWordOrDefault(0, 2));
            }

            // Mixture is bad
            {
                // Out-of-bounds (small number, small number)
                Assert.Throws<LuthetusTextEditorException>(() => modelModifier.ReadNextWordOrDefault(-1, -1));
                // Out-of-bounds (small number, large number)
                Assert.Throws<LuthetusTextEditorException>(() => modelModifier.ReadNextWordOrDefault(-1, 2));
                // Out-of-bounds (large number, small number)
                Assert.Throws<LuthetusTextEditorException>(() => modelModifier.ReadNextWordOrDefault(4, -1));
                // Out-of-bounds (large number, large number)
                Assert.Throws<LuthetusTextEditorException>(() => modelModifier.ReadNextWordOrDefault(4, 4));
            }
        }
    }

    /// <summary>
    /// <see cref="TextEditorModelExtensionMethods.GetTextOffsettingCursor(ITextEditorModel, TextEditorCursor)"/>
    /// </summary>
    [Fact]
    public void GetTextOffsettingCursor()
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

        // Case: start of file
        //
        // "\nb9\r9B\r\n\t$; "
        //  ^
        Assert.Equal(
            string.Empty,
            modelModifier.GetTextOffsettingCursor(new TextEditorCursor(lineIndex: 0, columnIndex: 0, isPrimaryCursor: true)));

        // Case: end of file
        //
        // "\nb9\r9B\r\n\t$; "
        //  ^
        Assert.Equal(
            "\t$; ",
            modelModifier.GetTextOffsettingCursor(new TextEditorCursor(lineIndex: 3, columnIndex: 4, isPrimaryCursor: true)));

        // Case: start of line && (NOT the start of the file)
        //
        // "\nb9\r9B\r\n\t$; "
        //  ^
        Assert.Equal(
            string.Empty,
            modelModifier.GetTextOffsettingCursor(new TextEditorCursor(lineIndex: 2, columnIndex: 0, isPrimaryCursor: true)));

        // Case: end of line && (NOT the end of the file)
        //
        // "\nb9\r9B\r\n\t$; "
        //  ^
        Assert.Equal(
            "9B",
            modelModifier.GetTextOffsettingCursor(new TextEditorCursor(lineIndex: 2, columnIndex: 2, isPrimaryCursor: true)));

        // Case: part of a line
        //
        // "\nb9\r9B\r\n\t$; "
        //  ^
        Assert.Equal(
            "9",
            modelModifier.GetTextOffsettingCursor(new TextEditorCursor(lineIndex: 2, columnIndex: 1, isPrimaryCursor: true)));

        // These cases are expected to throw an exception
        {
            // lineIndex is bad
            {
                // Out-of-bounds small number
                Assert.Throws<LuthetusTextEditorException>(() => modelModifier.GetTextOffsettingCursor(new TextEditorCursor(-1, 0, true)));
                // Out-of-bounds large number
                Assert.Throws<LuthetusTextEditorException>(() => modelModifier.GetTextOffsettingCursor(new TextEditorCursor(4, 0, true)));
            }

            // columnIndex is bad
            {
                // Out-of-bounds small number
                Assert.Throws<LuthetusTextEditorException>(() => modelModifier.GetTextOffsettingCursor(new TextEditorCursor(2, -1, true)));
                // Out-of-bounds large number
                Assert.Throws<LuthetusTextEditorException>(() => modelModifier.GetTextOffsettingCursor(new TextEditorCursor(0, 2, true)));
            }

            // Mixture is bad
            {
                // Out-of-bounds (small number, small number)
                Assert.Throws<LuthetusTextEditorException>(() => modelModifier.GetTextOffsettingCursor(new TextEditorCursor(-1, -1, true)));
                // Out-of-bounds (small number, large number)
                Assert.Throws<LuthetusTextEditorException>(() => modelModifier.GetTextOffsettingCursor(new TextEditorCursor(-1, 2, true)));
                // Out-of-bounds (large number, small number)
                Assert.Throws<LuthetusTextEditorException>(() => modelModifier.GetTextOffsettingCursor(new TextEditorCursor(4, -1, true)));
                // Out-of-bounds (large number, large number)
                Assert.Throws<LuthetusTextEditorException>(() => modelModifier.GetTextOffsettingCursor(new TextEditorCursor(4, 4, true)));
            }
        }
    }

    /// <summary>
    /// <see cref="TextEditorModelExtensionMethods.GetLineText(ITextEditorModel, int)"/>
    /// </summary>
    [Fact]
    public void GetLineText()
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
        Assert.Equal("\n", modelModifier.GetLineText(lineIndex: 0));

        // Line_Index_1: "b9\r"
        Assert.Equal("b9\r", modelModifier.GetLineText(lineIndex: 1));

        // Line_Index_2: "9B\r\n"
        Assert.Equal("9B\r\n", modelModifier.GetLineText(lineIndex: 2));

        // Line_Index_3: "--->$;∙EOF"
        Assert.Equal("\t$; ", modelModifier.GetLineText(lineIndex: 3));

        // Out-of-bounds small number
        Assert.Throws<LuthetusTextEditorException>(() => modelModifier.GetLineText(lineIndex: -1));
        // Out-of-bounds large number
        Assert.Throws<LuthetusTextEditorException>(() => modelModifier.GetLineText(lineIndex: 4));
    }

    /// <summary>
    /// <see cref="TextEditorModelExtensionMethods.GetRichCharacter(ITextEditorModel, int)"/>
    /// </summary>
    [Fact]
    public void GetRichCharacterOrDefault()
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
        Assert.Equal(new RichCharacter { Value = '\n', DecorationByte = 0 }, modelModifier.GetRichCharacter(0));

        // Line_Index_1: "b9\r"
        Assert.Equal(new RichCharacter { Value = 'b', DecorationByte = 0 }, modelModifier.GetRichCharacter(1));
        Assert.Equal(new RichCharacter { Value = '9', DecorationByte = 0 }, modelModifier.GetRichCharacter(2));
        Assert.Equal(new RichCharacter { Value = '\r', DecorationByte = 0 }, modelModifier.GetRichCharacter(3));

        // Line_Index_2: "9B\r\n"
        Assert.Equal(new RichCharacter { Value = '9', DecorationByte = 0 }, modelModifier.GetRichCharacter(4));
        Assert.Equal(new RichCharacter { Value = 'B', DecorationByte = 0 }, modelModifier.GetRichCharacter(5));
        Assert.Equal(new RichCharacter { Value = '\r', DecorationByte = 0 }, modelModifier.GetRichCharacter(6));

        // Line_Index_3: "--->$;∙EOF"
        Assert.Equal(new RichCharacter { Value = '\t', DecorationByte = 0 }, modelModifier.GetRichCharacter(8));
        Assert.Equal(new RichCharacter { Value = '$', DecorationByte = 0 }, modelModifier.GetRichCharacter(9));
        Assert.Equal(new RichCharacter { Value = ';', DecorationByte = 0 }, modelModifier.GetRichCharacter(10));
        Assert.Equal(new RichCharacter { Value = ' ', DecorationByte = 0 }, modelModifier.GetRichCharacter(11));
        Assert.Equal(new RichCharacter { Value = '\0', DecorationByte = 0 }, modelModifier.GetRichCharacter(12));

        // Greater-than out-of-bounds: positionIndex of 13 throws because the modelModifier.DocumentLength is 12, thereby putting a positionIndex of 13 out-of-bounds.
        Assert.Throws<LuthetusTextEditorException>(() => modelModifier.GetRichCharacter(13));
        // Less-than out-of-bounds: positionIndex of -1 throws because the positionIndex is negative, and would never be valid, thereby putting the positionIndex out-of-bounds.
        Assert.Throws<LuthetusTextEditorException>(() => modelModifier.GetRichCharacter(-1));
    }

    /// <summary>
    /// <see cref="TextEditorModelExtensionMethods.GetAllRichCharacters(ITextEditorModel)"/>
    /// </summary>
    [Fact]
    public void GetAllRichCharacters()
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

        var expectedRichCharacterList = content.Select(x => new RichCharacter { Value = x, DecorationByte = 0 }).ToList();
        var actualRichCharacterList = modelModifier.GetAllRichCharacters();

        Assert.Equal(expectedRichCharacterList.Count, actualRichCharacterList.Count);

        for (int i = 0; i < expectedRichCharacterList.Count; i++)
        {
            Assert.Equal(expectedRichCharacterList[i], actualRichCharacterList[i]);
        }
    }

    /// <summary>
    /// <see cref="TextEditorModelExtensionMethods.GetRichCharacters(ITextEditorModel, int, int)"/>
    /// </summary>
    [Fact]
    public void GetRichCharacters()
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

        // Case: from start of file
        {
            var expectedRichCharacterList = "\nb9\r".Select(x => new RichCharacter { Value = x, DecorationByte = 0 }).ToList();
            var actualRichCharacterList = modelModifier.GetRichCharacters(0, 4);

            Assert.Equal(expectedRichCharacterList.Count, actualRichCharacterList.Count);

            for (int i = 0; i < expectedRichCharacterList.Count; i++)
            {
                Assert.Equal(expectedRichCharacterList[i], actualRichCharacterList[i]);
            }
        }

        // Case: to end of file
        {
            var expectedRichCharacterList = "\t$; ".Select(x => new RichCharacter { Value = x, DecorationByte = 0 }).ToList();
            var actualRichCharacterList = modelModifier.GetRichCharacters(8, 4);

            Assert.Equal(expectedRichCharacterList.Count, actualRichCharacterList.Count);

            for (int i = 0; i < expectedRichCharacterList.Count; i++)
            {
                Assert.Equal(expectedRichCharacterList[i], actualRichCharacterList[i]);
            }
        }

        // Case: from negative start
        {
            Assert.Throws<LuthetusTextEditorException>(() => modelModifier.GetRichCharacters(-1, 4));
        }

        // Case: to out of bounds large number end
        {
            var expectedRichCharacterList = "\t$; ".Select(x => new RichCharacter { Value = x, DecorationByte = 0 }).ToList();
            var actualRichCharacterList = modelModifier.GetRichCharacters(8, 7);

            Assert.Equal(expectedRichCharacterList.Count, actualRichCharacterList.Count);

            for (int i = 0; i < expectedRichCharacterList.Count; i++)
            {
                Assert.Equal(expectedRichCharacterList[i], actualRichCharacterList[i]);
            }
        }
    }
}
