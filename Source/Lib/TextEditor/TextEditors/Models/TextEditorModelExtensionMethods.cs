using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Facts;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

public static class TextEditorModelExtensionMethods
{
    /// <summary>
    /// Short but inaccurate description:<br/>
    /// ∙∙∙ One provides a rowIndex, and gets the line-ending of the previous row.<br/>
    /// 
    /// Special cases:<br/>
    /// ∙∙∙ Index 0 and index ^1 (the final index) return 'StartOfFile' and 'EndOfFile' respectively.<br/>
    /// 
    /// Algorithm:<br/>
	/// ∙∙∙ rowIndex of 0 returns StartOfFile<br/>
	/// ∙∙∙ rowIndex of 1 returns the line-ending of row 0<br/>
	/// ∙∙∙ rowIndex of 2 returns the line-ending of row 1<br/>
	/// ∙∙∙ etc...<br/>
	/// ∙∙∙ rowIndex of ^1 (the final index) returns EndOfFile<br/>
    /// </summary>
    public static LineEnd GetLineOpening(this ITextEditorModel model, int lineIndex)
    {
        // Index is out of range (large-ly)? Then change the index to the last index.
        lineIndex = Math.Min(lineIndex, model.LineEndPositionList.Count - 1);

        // Index is the last index? Then return EndOfFile
        if (lineIndex == model.LineEndPositionList.Count - 1)
            return model.LineEndPositionList[^1];

        // Index in range (small-ly, and large-ly)? Then return the previous line's line ending.
        if (lineIndex > 0)
            return model.LineEndPositionList[lineIndex - 1];

        // Index is out of range (small-ly)? Then return StartOfFile.
        return new(0, 0, LineEndKind.StartOfFile);
    }

    /// <summary>
    /// Short but inaccurate description:<br/>
    /// ∙∙∙ One provides a rowIndex, and gets the line-ending of that row.<br/>
    /// 
    /// Special cases:<br/>
    /// ∙∙∙ Index ^1 (the final index) returns 'EndOfFile'.<br/>
    /// 
    /// Algorithm:<br/>
	/// ∙∙∙ rowIndex of 0 returns the line-ending of row 0<br/>
	/// ∙∙∙ rowIndex of 1 returns the line-ending of row 1<br/>
	/// ∙∙∙ rowIndex of 2 returns the line-ending of row 2<br/>
	/// ∙∙∙ etc...<br/>
	/// ∙∙∙ rowIndex of ^1 (the final index) returns EndOfFile<br/>
    /// </summary>
    public static LineEnd GetLineClosing(this ITextEditorModel model, int lineIndex)
    {
        // Index is out of range (large-ly)? Then change the index to the last index.
        lineIndex = Math.Min(lineIndex, model.LineEndPositionList.Count - 1);

        // Index is the last index? Then return EndOfFile.
        if (lineIndex == model.LineEndPositionList.Count - 1)
            return model.LineEndPositionList[^1];

        // Index in range (small-ly, and large-ly)? Then return that line's line ending.
        if (lineIndex >= 0)
            return model.LineEndPositionList[lineIndex];

        // Index is out of range (small-ly)? Then return EndOfFile.
        return model.LineEndPositionList[^1];
    }

    public static int GetLineStartPositionIndexInclusive(this ITextEditorModel model, int lineIndex)
    {
        return model.GetLineOpening(lineIndex).EndPositionIndexExclusive;
    }

    public static int GetLineEndPositionIndexExclusive(this ITextEditorModel model, int lineIndex)
    {
        return model.GetLineClosing(lineIndex).EndPositionIndexExclusive;
    }

    /// <summary>
	/// Returns the Length of a line however it does not include the line ending characters by default.
	/// To include line ending characters the parameter <see cref="includeLineEndingCharacters" /> must be true.
	/// </summary>
    public static int GetLengthOfLine(this ITextEditorModel model, int lineIndex, bool includeLineEndingCharacters = false)
    {
        if (!model.LineEndPositionList.Any())
            return 0;

        if (lineIndex > model.LineEndPositionList.Count - 1)
            lineIndex = model.LineEndPositionList.Count - 1;

        if (lineIndex < 0)
            lineIndex = 0;

        var startOfLineTupleInclusive = model.GetLineOpening(lineIndex);
        // TODO: Index was out of range exception on 2023-04-18
        var endOfLineTupleExclusive = model.LineEndPositionList[lineIndex];
        var lineLengthWithLineEndings = endOfLineTupleExclusive.EndPositionIndexExclusive - startOfLineTupleInclusive.EndPositionIndexExclusive;

        if (includeLineEndingCharacters)
            return lineLengthWithLineEndings;

        return lineLengthWithLineEndings - endOfLineTupleExclusive.LineEndKind.AsCharacters().Length;
    }

    /// <summary>
	/// Line endings are included in the individual lines which get returned.
	/// </summary>
	/// <param name="startingLineIndex">The starting index of the lines to return</param>
    /// <param name="count">
    /// A count of 0 returns 0 rows.<br/>
    /// A count of 1 returns lines[startingLineIndex] only.<br/>
    /// A count of 2 returns lines[startingLineIndex] and lines[startingLineIndex + 1].<br/>
    /// </param>
    public static List<List<RichCharacter>> GetLines(this ITextEditorModel model, int startingLineIndex, int count)
    {
        var lineCountAvailable = model.LineEndPositionList.Count - startingLineIndex;

        var lineCountToReturn = count < lineCountAvailable
            ? count
            : lineCountAvailable;

        var endingLineIndexExclusive = startingLineIndex + lineCountToReturn;
        var lineList = new List<List<RichCharacter>>();

        if (lineCountToReturn < 0 || startingLineIndex < 0 || endingLineIndexExclusive < 0)
            return lineList;

        for (var i = startingLineIndex; i < endingLineIndexExclusive; i++)
        {
            // Previous line's end-position-exclusive is this row's start.
            var startOfLineInclusive = model.GetLineStartPositionIndexInclusive(i);
            var endOfLineExclusive = model.LineEndPositionList[i].EndPositionIndexExclusive;

            var line = model.GetRichCharacters(
                skip: startOfLineInclusive,
                take: endOfLineExclusive - startOfLineInclusive);

            lineList.Add(line);
        }

        return lineList;
    }

    public static int GetTabsCountOnSameLineBeforeCursor(this ITextEditorModel model, int rowIndex, int columnIndex)
    {
        var startOfLinePositionIndex = model.GetLineStartPositionIndexInclusive(rowIndex);

        var tabs = model.TabKeyPositionList
            .SkipWhile(positionIndex => positionIndex < startOfLinePositionIndex)
            .TakeWhile(positionIndex => positionIndex < startOfLinePositionIndex + columnIndex);

        return tabs.Count();
    }

    /// <summary>
    /// TODO: Given that the text editor is now immutable (2023-12-17), when this is invoked...
    /// ...it should be cached.
    /// </summary>
    public static string GetAllText(this ITextEditorModel model)
    {
        return new string(model.CharList.Select(x => x).ToArray());
    }

    public static int GetPositionIndex(this ITextEditorModel model, TextEditorCursor cursor)
    {
        return model.GetPositionIndex(cursor.LineIndex, cursor.ColumnIndex);
    }

    public static int GetPositionIndex(this ITextEditorModel model, TextEditorCursorModifier cursorModifier)
    {
        return model.GetPositionIndex(cursorModifier.LineIndex, cursorModifier.ColumnIndex);
    }

    /// <summary>
    /// TODO: How should this method handle input which is out of bounds?
    /// </summary>
    public static int GetPositionIndex(this ITextEditorModel model, int lineIndex, int columnIndex)
    {
        var lineStartPositionIndexInclusive = model.GetLineStartPositionIndexInclusive(lineIndex);
        return lineStartPositionIndexInclusive + columnIndex;
    }

    public static (int lineIndex, int columnIndex) GetLineAndColumnIndicesFromPositionIndex(
        this ITextEditorModel model,
        int positionIndex)
    {
        var lineInformation = model.GetLineInformationFromPositionIndex(positionIndex);

        return (
            lineInformation.LineIndex,
            positionIndex - lineInformation.LineStartPositionIndexInclusive);
    }

    /// <summary>
    /// To receive a <see cref="string"/> value, one can use <see cref="GetString"/> instead.
    /// </summary>
    public static char GetCharacter(this ITextEditorModel model, int positionIndex)
    {
        if (positionIndex < 0 || positionIndex >= model.CharList.Count)
            return ParserFacts.END_OF_FILE;

        return model.CharList[positionIndex];
    }

    /// <summary>
    /// To receive a <see cref="char"/> value, one can use <see cref="GetCharacter"/> instead.
    /// </summary>
    public static string GetString(this ITextEditorModel model, int startingPositionIndex, int count)
    {
        return new string(model.CharList
            .Skip(startingPositionIndex)
            .Take(count)
            .Select(x => x)
            .ToArray());
    }

    public static string GetLineRange(this ITextEditorModel model, int startLineIndex, int count)
    {
        if (startLineIndex > model.LineCount - 1)
            return string.Empty;

        var startPositionIndexInclusive = model.GetPositionIndex(startLineIndex, 0);
        var lastLineIndexExclusive = startLineIndex + count;
        int endPositionIndexExclusive;

        if (lastLineIndexExclusive > model.LineCount - 1)
        {
            endPositionIndexExclusive = model.DocumentLength;
        }
        else
        {
            endPositionIndexExclusive = model.GetPositionIndex(lastLineIndexExclusive, 0);
        }

        return model.GetString(
            startPositionIndexInclusive,
            endPositionIndexExclusive - startPositionIndexInclusive);
    }

    /// <summary>
    /// Given a <see cref="TextEditorModel"/> with a preference for the right side of the cursor, the following conditional branch will play out:<br/><br/>
    ///     -IF the cursor is amongst a word, that word will be returned.<br/><br/>
    ///     -ELSE IF the start of a word is to the right of the cursor that word will be returned.<br/><br/>
    ///     -ELSE IF the end of a word is to the left of the cursor that word will be returned.</summary>
    public static TextEditorTextSpan? GetWordTextSpan(this ITextEditorModel model, int positionIndex)
    {
        var previousCharacter = model.GetCharacter(positionIndex - 1);
        var currentCharacter = model.GetCharacter(positionIndex);

        var previousCharacterKind = CharacterKindHelper.CharToCharacterKind(previousCharacter);
        var currentCharacterKind = CharacterKindHelper.CharToCharacterKind(currentCharacter);

        var lineInformation = model.GetLineInformationFromPositionIndex(positionIndex);
        var columnIndex = positionIndex - lineInformation.LineStartPositionIndexInclusive;

        if (previousCharacterKind == CharacterKind.LetterOrDigit && currentCharacterKind == CharacterKind.LetterOrDigit)
        {
            var wordColumnIndexStartInclusive = model.GetColumnIndexOfCharacterWithDifferingKind(
                lineInformation.LineIndex,
                columnIndex,
                true);

            if (wordColumnIndexStartInclusive == -1)
                wordColumnIndexStartInclusive = 0;

            var wordColumnIndexEndExclusive = model.GetColumnIndexOfCharacterWithDifferingKind(
                lineInformation.LineIndex,
                columnIndex,
                false);

            if (wordColumnIndexEndExclusive == -1)
                wordColumnIndexEndExclusive = model.GetLengthOfLine(lineInformation.LineIndex);

            return new TextEditorTextSpan(
                wordColumnIndexStartInclusive + lineInformation.LineStartPositionIndexInclusive,
                wordColumnIndexEndExclusive + lineInformation.LineStartPositionIndexInclusive,
                0,
                model.ResourceUri,
                model.GetAllText());
        }
        else if (currentCharacterKind == CharacterKind.LetterOrDigit)
        {
            var wordColumnIndexEndExclusive = model.GetColumnIndexOfCharacterWithDifferingKind(
                lineInformation.LineIndex,
                columnIndex,
                false);

            if (wordColumnIndexEndExclusive == -1)
                wordColumnIndexEndExclusive = model.GetLengthOfLine(lineInformation.LineIndex);

            return new TextEditorTextSpan(
                columnIndex + lineInformation.LineStartPositionIndexInclusive,
                wordColumnIndexEndExclusive + lineInformation.LineStartPositionIndexInclusive,
                0,
                model.ResourceUri,
                model.GetAllText());
        }
        else if (previousCharacterKind == CharacterKind.LetterOrDigit)
        {
            var wordColumnIndexStartInclusive = model.GetColumnIndexOfCharacterWithDifferingKind(
                lineInformation.LineIndex,
                columnIndex,
                true);

            if (wordColumnIndexStartInclusive == -1)
                wordColumnIndexStartInclusive = 0;

            return new TextEditorTextSpan(
                wordColumnIndexStartInclusive + lineInformation.LineStartPositionIndexInclusive,
                columnIndex + lineInformation.LineStartPositionIndexInclusive,
                0,
                model.ResourceUri,
                model.GetAllText());
        }

        return null;
    }

    public static ImmutableArray<TextEditorTextSpan> FindMatches(this ITextEditorModel model, string query)
    {
        var text = model.GetAllText();
        var matchedTextSpans = new List<TextEditorTextSpan>();

        for (int outerI = 0; outerI < text.Length; outerI++)
        {
            if (outerI + query.Length <= text.Length)
            {
                int innerI = 0;
                for (; innerI < query.Length; innerI++)
                {
                    if (text[outerI + innerI] != query[innerI])
                        break;
                }

                if (innerI == query.Length)
                {
                    // Then the entire query was matched
                    matchedTextSpans.Add(new TextEditorTextSpan(
                        outerI,
                        outerI + innerI,
                        (byte)FindOverlayDecorationKind.LongestCommonSubsequence,
                        model.ResourceUri,
                        text));
                }
            }
        }

        return matchedTextSpans.ToImmutableArray();
    }

    public static LineInformation GetLineInformationFromPositionIndex(this ITextEditorModel model, int positionIndex)
    {
        // If the EndPositionIndexExclusive of the first line end is greater than the position index
        if (model.LineEndPositionList[0].EndPositionIndexExclusive > positionIndex)
        {
            // Then the line-opening is a special case 'StartOfFile'
            var lineEnd = GetLineOpening(model, 0);
            return new LineInformation(0, lineEnd.StartPositionIndexInclusive, lineEnd);
        }

        // If the EndOfFile's EndPositionIndexExclusive is less than the positionIndex,
        // then the positionIndex is (large-ly) out of bounds.
        if (model.LineEndPositionList[^1].EndPositionIndexExclusive <= positionIndex)
        {
            // Return the 'EndOfFile' special case line-end
            var lineEnd = GetLineOpening(model, model.LineEndPositionList.Count - 1);
            return new LineInformation(0, lineEnd.StartPositionIndexInclusive, lineEnd);
        }

        // Any cases which are not at the start of the text editor,
        // nor the end the text editor,
        // are handled by this 'for' loop.
        for (var i = 1; i < model.LineEndPositionList.Count; i++)
        {
            var lineEndTuple = model.LineEndPositionList[i];

            if (lineEndTuple.EndPositionIndexExclusive > positionIndex)
            {
                return new LineInformation(
                    i,
                    lineEndTuple.StartPositionIndexInclusive,
                    GetLineOpening(model, model.LineEndPositionList.Count - 1));
            }
        }

        return new(0, 0, new LineEnd(0, 0, LineEndKind.StartOfFile));
    }

    /// <summary>
    /// <see cref="moveBackwards"/> is to mean earlier in the document
    /// (lower column index or lower row index depending on position) 
    /// </summary>
    /// <returns>Will return -1 if no valid result was found.</returns>
    public static int GetColumnIndexOfCharacterWithDifferingKind(
        this ITextEditorModel model,
        int lineIndex,
        int columnIndex,
        bool moveBackwards)
    {
        var iterateBy = moveBackwards
            ? -1
            : 1;

        var lineStartPositionIndex = model.GetLineStartPositionIndexInclusive(lineIndex);

        if (lineIndex > model.LineEndPositionList.Count - 1)
            return -1;

        var lastPositionIndexOnRow = model.LineEndPositionList[lineIndex].EndPositionIndexExclusive - 1;
        var positionIndex = model.GetPositionIndex(lineIndex, columnIndex);

        if (moveBackwards)
        {
            if (positionIndex <= lineStartPositionIndex)
                return -1;

            positionIndex -= 1;
        }

        if (positionIndex < 0 || positionIndex >= model.CharList.Count)
            return -1;

        var startCharacterKind = CharacterKindHelper.CharToCharacterKind(model.CharList[positionIndex]);

        while (true)
        {
            if (positionIndex >= model.CharList.Count ||
                positionIndex > lastPositionIndexOnRow ||
                positionIndex < lineStartPositionIndex)
            {
                return -1;
            }

            var currentCharacterKind = CharacterKindHelper.CharToCharacterKind(model.CharList[positionIndex]);

            if (currentCharacterKind != startCharacterKind)
                break;

            positionIndex += iterateBy;
        }

        if (moveBackwards)
            positionIndex += 1;

        return positionIndex - lineStartPositionIndex;
    }

    public static bool CanUndoEdit(this ITextEditorModel model)
    {
        return model.EditBlockIndex > 0;
    }

    public static bool CanRedoEdit(this ITextEditorModel model)
    {
        return model.EditBlockIndex < model.EditBlockList.Count - 1;
    }

    public static CharacterKind GetCharacterKind(this ITextEditorModel model, int positionIndex)
    {
        try
        {
            return CharacterKindHelper.CharToCharacterKind(model.CharList[positionIndex]);
        }
        catch (ArgumentOutOfRangeException)
        {
            // The text editor's cursor is is likely
            // to have this occur at times
        }

        return CharacterKind.Bad;
    }

    public static string? ReadPreviousWordOrDefault(
        this ITextEditorModel model,
        int lineIndex,
        int columnIndex,
        bool isRecursiveCall = false)
    {
        var wordPositionIndexEndExclusive = model.GetPositionIndex(lineIndex, columnIndex);
        var wordCharacterKind = model.GetCharacterKind(wordPositionIndexEndExclusive - 1);

        if (wordCharacterKind == CharacterKind.Punctuation && !isRecursiveCall)
        {
            // If previous previous word is a punctuation character, then perhaps
            // the user hit { 'Ctrl' + 'Space' } to trigger the autocomplete
            // and was at a MemberAccessToken (or a period '.')
            //
            // So, read the word previous to the punctuation.

            var anotherAttemptColumnIndex = columnIndex - 1;

            if (anotherAttemptColumnIndex >= 0)
                return model.ReadPreviousWordOrDefault(lineIndex, anotherAttemptColumnIndex, true);
        }

        if (wordCharacterKind == CharacterKind.LetterOrDigit)
        {
            var wordColumnIndexStartInclusive = model.GetColumnIndexOfCharacterWithDifferingKind(
                lineIndex,
                columnIndex,
                true);

            if (wordColumnIndexStartInclusive == -1)
                wordColumnIndexStartInclusive = 0;

            var wordLength = columnIndex - wordColumnIndexStartInclusive;
            var wordPositionIndexStartInclusive = wordPositionIndexEndExclusive - wordLength;

            return model.GetString(wordPositionIndexStartInclusive, wordLength);
        }

        return null;
    }

    public static string? ReadNextWordOrDefault(this ITextEditorModel model, int lineIndex, int columnIndex)
    {
        var wordPositionIndexStartInclusive = model.GetPositionIndex(lineIndex, columnIndex);
        var wordCharacterKind = model.GetCharacterKind(wordPositionIndexStartInclusive);

        if (wordCharacterKind == CharacterKind.LetterOrDigit)
        {
            var wordColumnIndexEndExclusive = model.GetColumnIndexOfCharacterWithDifferingKind(
                lineIndex,
                columnIndex,
                false);

            if (wordColumnIndexEndExclusive == -1)
                wordColumnIndexEndExclusive = model.GetLengthOfLine(lineIndex);

            var wordLength = wordColumnIndexEndExclusive - columnIndex;

            return model.GetString(wordPositionIndexStartInclusive, wordLength);
        }

        return null;
    }

    /// <summary>
    /// This method returns the text to the left of the cursor in most cases.
    /// The method name is as such because of right to left written texts.<br/><br/>
    /// One uses this method most often to measure the position of the cursor when rendering the
    /// UI for a font-family which is proportional (i.e. not monospace).
    /// </summary>
    public static string GetTextOffsettingCursor(this ITextEditorModel model, TextEditorCursor textEditorCursor)
    {
        var cursorPositionIndex = model.GetPositionIndex(textEditorCursor);
        var priorLineEnd = model.GetLineOpening(textEditorCursor.LineIndex);

        return model.GetString(priorLineEnd.EndPositionIndexExclusive, cursorPositionIndex - priorLineEnd.EndPositionIndexExclusive);
    }

    public static string GetLine(this ITextEditorModel model, int lineIndex)
    {
        if (lineIndex < 0 || lineIndex > model.LineCount - 1)
            return string.Empty;

        var priorLineEnd = model.GetLineOpening(lineIndex);
        var lengthOfRow = model.GetLengthOfLine(lineIndex, true);

        return model.GetString(priorLineEnd.EndPositionIndexExclusive, lengthOfRow);
    }

    public static RichCharacter? GetRichCharacterOrDefault(this ITextEditorModel model, int positionIndex)
    {
        if (positionIndex < 0 || positionIndex > model.DocumentLength - 1)
            return null;

        return new RichCharacter
        {
            Value = model.CharList[positionIndex],
            DecorationByte = model.DecorationByteList[positionIndex],
        };
    }

    public static List<RichCharacter> GetAllRichCharacters(this ITextEditorModel model)
    {
        var richCharacterList = new List<RichCharacter>();

        for (int i = 0; i < model.DocumentLength; i++)
        {
            richCharacterList.Add(new RichCharacter
            {
                Value = model.CharList[i],
                DecorationByte = model.DecorationByteList[i],
            });
        }

        return richCharacterList;
    }

    public static List<RichCharacter> GetRichCharacters(this ITextEditorModel model, int skip, int take)
    {
        var richCharacterList = new List<RichCharacter>();

        for (var i = 0; i < take; i++)
        {
            if (i >= model.DocumentLength)
                break;

            richCharacterList.Add(new RichCharacter
            {
                Value = model.CharList[skip + i],
                DecorationByte = model.DecorationByteList[skip + i]
            });
        }

        return richCharacterList;
    }
}
