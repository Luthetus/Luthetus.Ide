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
    /// <see cref="TextEditorModelExtensionMethods.GetLengthOfLine(ITextEditorModel, int, bool)"/>
    /// </summary>
    [Fact]
    public void GetLengthOfLine()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModelExtensionMethods.GetLineRichCharacterRange(ITextEditorModel, int, int)"/>
    /// </summary>
    [Fact]
    public void GetLines()
    {
        var (inModel, modelModifier) = NotEmptyEditor_TestData_And_PerformPreAssertions(
            resourceUri: new ResourceUri($"/{nameof(NotEmptyEditor_TestData_And_PerformPreAssertions)}.txt"),
            resourceLastWriteTime: DateTime.MinValue,
            fileExtension: ExtensionNoPeriodFacts.TXT,
            content: "\nb9\r9B\r\n\t$; ",
            decorationMapper: null,
            compilerService: null,
            partitionSize: 4096);

        var zero_index_line = modelModifier.GetLineText(0);
        var one_index_line = modelModifier.GetLineText(1);
        var two_index_line = modelModifier.GetLineText(2);
        var three_index_line = modelModifier.GetLineText(3);
        var four_index_line = modelModifier.GetLineText(4);
        var five_index_line = modelModifier.GetLineText(5);

        //Assert.Equal("\n", );

        var lineEndListCount = modelModifier.LineEndList.Count;

        var zero_line_index_to_position_index = modelModifier.GetPositionIndex(lineIndex: 0, columnIndex: 0);
        var one_line_index_to_position_index = modelModifier.GetPositionIndex(lineIndex: 1, columnIndex: 0);
        var two_line_index_to_position_index = modelModifier.GetPositionIndex(lineIndex: 2, columnIndex: 0);
        var three_line_index_to_position_index = modelModifier.GetPositionIndex(lineIndex: 3, columnIndex: 0);
        var four_line_index_to_position_index = modelModifier.GetPositionIndex(lineIndex: 4, columnIndex: 0);

        throw new NotImplementedException();
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
            .Select(y => y.Value)
            .ToArray());

        Assert.Equal(all_text, linesString);
    }

    /// <summary>
    /// <see cref="TextEditorModelExtensionMethods.GetTabsCountOnSameLineBeforeCursor(ITextEditorModel, int, int)"/>
    /// </summary>
    [Fact]
    public void GetTabsCountOnSameLineBeforeCursor()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModelExtensionMethods.GetAllText(ITextEditorModel)"/>
    /// </summary>
    [Fact]
    public void GetAllText()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModelExtensionMethods.GetPositionIndex(ITextEditorModel, TextEditorCursor)"/>
    /// </summary>
    [Fact]
    public void GetPositionIndex_TextEditorCursor()
    {
        throw new NotImplementedException();
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

        var zero_length = modelModifier.GetLengthOfLine(0, true);
        var one_length = modelModifier.GetLengthOfLine(1, true);
        var two_length = modelModifier.GetLengthOfLine(2, true);
        var three_length = modelModifier.GetLengthOfLine(3, true);

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
