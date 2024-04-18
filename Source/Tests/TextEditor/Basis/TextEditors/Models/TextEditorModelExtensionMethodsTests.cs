using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.TextEditor.Tests.Basis.TextEditors.Models;

/// <summary>
/// <see cref="TextEditorModelExtensionMethods"/>
/// </summary>
public class TextEditorModelExtensionMethodsTests : TextEditorTestBase
{
    [Fact]
    public void GetPositionIndex_recent()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void GetLengthOfLine()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void GetLines()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void GetTabsCountOnSameLineBeforeCursor()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void GetAllText()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void GetPositionIndex_TextEditorCursor()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void GetPositionIndex_TextEditorCursorModifier()
    {
        throw new NotImplementedException();
    }

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

    [Fact]
    public void GetLineAndColumnIndicesFromPositionIndex()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void GetCharacter()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void GetString()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void GetLineRange()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void GetWordTextSpan()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void FindMatches()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void GetLineInformation()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void GetLineInformationFromPositionIndex()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void GetColumnIndexOfCharacterWithDifferingKind()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void CanUndoEdit()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void CanRedoEdit()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void GetCharacterKind()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void ReadPreviousWordOrDefault()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void ReadNextWordOrDefault()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void GetTextOffsettingCursor()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void GetLineText()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void GetRichCharacterOrDefault()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void GetAllRichCharacters()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void GetRichCharacters()
    {
        throw new NotImplementedException();
    }
}
