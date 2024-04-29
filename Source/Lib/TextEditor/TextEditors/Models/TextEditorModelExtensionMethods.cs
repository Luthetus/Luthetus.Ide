using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Facts;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Exceptions;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

public static class TextEditorModelExtensionMethods
{
    /// <summary>
	/// Returns the Length of a line however it does not include the line ending characters by default.
	/// To include line ending characters the parameter <see cref="includeLineEndingCharacters" /> must be true.
	/// </summary>
    public static int GetLineLength(this ITextEditorModel model, int lineIndex, bool includeLineEndingCharacters = false)
    {
        if (!model.LineEndList.Any())
            return 0;

        if (lineIndex > model.LineEndList.Count - 1)
            lineIndex = model.LineEndList.Count - 1;

        if (lineIndex < 0)
            lineIndex = 0;

        var line = model.GetLineInformation(lineIndex);
        var lineLengthWithLineEndings = line.EndPositionIndexExclusive - line.StartPositionIndexInclusive;

        if (includeLineEndingCharacters)
            return lineLengthWithLineEndings;

        return lineLengthWithLineEndings - line.UpperLineEnd.LineEndKind.AsCharacters().Length;
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
    public static List<List<RichCharacter>> GetLineRichCharacterRange(this ITextEditorModel model, int startingLineIndex, int count)
    {
        var lineCountAvailable = model.LineEndList.Count - startingLineIndex;

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
            var startOfLineInclusive = model.GetLineInformation(i).StartPositionIndexInclusive;
            var endOfLineExclusive = model.LineEndList[i].EndPositionIndexExclusive;

            var line = model.RichCharacterList
                .Skip(startOfLineInclusive)
                .Take(endOfLineExclusive - startOfLineInclusive)
                .ToList();

            lineList.Add(line);
        }

        return lineList;
    }

    public static int GetTabCountOnSameLineBeforeCursor(this ITextEditorModel model, int lineIndex, int columnIndex)
    {
        var line = model.GetLineInformation(lineIndex);

        model.AssertColumnIndex(line, columnIndex);

        var tabs = model.TabKeyPositionList
            .SkipWhile(positionIndex => positionIndex < line.StartPositionIndexInclusive)
            .TakeWhile(positionIndex => positionIndex < line.StartPositionIndexInclusive + columnIndex);

        return tabs.Count();
    }

    /// <summary>
    /// Implementations of this method are expected to have caching.
    /// </summary>
    public static string GetAllText(this ITextEditorModel model)
    {
        return model.AllText;
    }

    public static int GetPositionIndex(this ITextEditorModel model, TextEditorCursor cursor)
    {
        return model.GetPositionIndex(cursor.LineIndex, cursor.ColumnIndex);
    }

    public static int GetPositionIndex(this ITextEditorModel model, TextEditorCursorModifier cursorModifier)
    {
        return model.GetPositionIndex(cursorModifier.LineIndex, cursorModifier.ColumnIndex);
    }

    public static int GetPositionIndex(this ITextEditorModel model, int lineIndex, int columnIndex)
    {
        var line = model.GetLineInformation(lineIndex);

        model.AssertColumnIndex(line, columnIndex);

        return line.StartPositionIndexInclusive + columnIndex;
    }

    public static (int lineIndex, int columnIndex) GetLineAndColumnIndicesFromPositionIndex(
        this ITextEditorModel model,
        int positionIndex)
    {
        var lineInformation = model.GetLineInformationFromPositionIndex(positionIndex);

        return (
            lineInformation.Index,
            positionIndex - lineInformation.StartPositionIndexInclusive);
    }

    /// <summary>
    /// To receive a <see cref="string"/> value, one can use <see cref="GetString"/> instead.
    /// </summary>
    public static char GetCharacter(this ITextEditorModel model, int positionIndex)
    {
        model.AssertPositionIndex(positionIndex);

        if (positionIndex == model.DocumentLength)
            return ParserFacts.END_OF_FILE;

        return model.RichCharacterList[positionIndex].Value;
    }

    /// <summary>
    /// To receive a <see cref="char"/> value, one can use <see cref="GetCharacter"/> instead.
    /// </summary>
    public static string GetString(this ITextEditorModel model, int positionIndex, int count)
    {
        model.AssertPositionIndex(positionIndex);
        model.AssertCount(count);

        return new string(model.RichCharacterList
            .Skip(positionIndex)
            .Take(count)
            .Select(x => x.Value)
            .ToArray());
    }

    public static string GetLineTextRange(this ITextEditorModel model, int lineIndex, int count)
    {
        model.AssertCount(count);

        var startPositionIndexInclusive = model.GetPositionIndex(lineIndex, 0);
        var lastLineIndexExclusive = lineIndex + count;
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
        var columnIndex = positionIndex - lineInformation.StartPositionIndexInclusive;

        if (previousCharacterKind == CharacterKind.LetterOrDigit && currentCharacterKind == CharacterKind.LetterOrDigit)
        {
            var wordColumnIndexStartInclusive = model.GetColumnIndexOfCharacterWithDifferingKind(
                lineInformation.Index,
                columnIndex,
                true);

            if (wordColumnIndexStartInclusive == -1)
                wordColumnIndexStartInclusive = 0;

            var wordColumnIndexEndExclusive = model.GetColumnIndexOfCharacterWithDifferingKind(
                lineInformation.Index,
                columnIndex,
                false);

            if (wordColumnIndexEndExclusive == -1)
                wordColumnIndexEndExclusive = model.GetLineLength(lineInformation.Index);

            return new TextEditorTextSpan(
                wordColumnIndexStartInclusive + lineInformation.StartPositionIndexInclusive,
                wordColumnIndexEndExclusive + lineInformation.StartPositionIndexInclusive,
                0,
                model.ResourceUri,
                model.GetAllText());
        }
        else if (currentCharacterKind == CharacterKind.LetterOrDigit)
        {
            var wordColumnIndexEndExclusive = model.GetColumnIndexOfCharacterWithDifferingKind(
                lineInformation.Index,
                columnIndex,
                false);

            if (wordColumnIndexEndExclusive == -1)
                wordColumnIndexEndExclusive = model.GetLineLength(lineInformation.Index);

            return new TextEditorTextSpan(
                columnIndex + lineInformation.StartPositionIndexInclusive,
                wordColumnIndexEndExclusive + lineInformation.StartPositionIndexInclusive,
                0,
                model.ResourceUri,
                model.GetAllText());
        }
        else if (previousCharacterKind == CharacterKind.LetterOrDigit)
        {
            var wordColumnIndexStartInclusive = model.GetColumnIndexOfCharacterWithDifferingKind(
                lineInformation.Index,
                columnIndex,
                true);

            if (wordColumnIndexStartInclusive == -1)
                wordColumnIndexStartInclusive = 0;

            return new TextEditorTextSpan(
                wordColumnIndexStartInclusive + lineInformation.StartPositionIndexInclusive,
                columnIndex + lineInformation.StartPositionIndexInclusive,
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

    /// <summary>
    /// 'lineIndex' equal to '0' returns the first line.<br/><br/>
    /// 
    /// An index for <see cref="ITextEditorModel.LineEndList"/> maps 1 to 1 with this method.
    /// (i.e.) the 0th line-end index will end up returning the 0th line.<br/><br/>
    /// 
    /// Given a 'lineIndex', return the <see cref="LineEnd"/> at <see cref="ITextEditorModel.LineEndList"/>[lineIndex - 1],
    /// and <see cref="ITextEditorModel.LineEndList"/>[lineIndex]
    /// in the form of the type <see cref="LineInformation"/>.
    /// </summary>
    /// <remarks>
    /// When 'lineIndex' == 0, then a "made-up" line ending named <see cref="LineEnd.StartOfFile"/> will be used
    /// in place of indexing at '<see cref="ITextEditorModel.LineEndList"/>[-1]'
    /// </remarks>
    public static LineInformation GetLineInformation(this ITextEditorModel model, int lineIndex)
    {
        model.AssertLineIndex(lineIndex);

        LineEnd GetLineEndLower(int lineIndex)
        {
            // Large index? Then set the index to the last index.
            lineIndex = Math.Min(lineIndex, model.LineEndList.Count - 1);

            // Small index? Then return StartOfFile.
            if (lineIndex <= 0)
                return new(0, 0, LineEndKind.StartOfFile);

            // In-range index? Then return the previous line's line ending.
            return model.LineEndList[lineIndex - 1];
        }

        LineEnd GetLineEndUpper(int lineIndex)
        {
            // Large index? Then set the index to the last index.
            lineIndex = Math.Min(lineIndex, model.LineEndList.Count - 1);

            // Small index? Then return the first LineEnd
            if (lineIndex <= 0)
                return model.LineEndList[0];

            // In-range index? Then return the LineEnd at that index.
            return model.LineEndList[lineIndex];
        }
        
        var lineEndLower = GetLineEndLower(lineIndex);
        var lineEndUpper = GetLineEndUpper(lineIndex);

        return new LineInformation(
            lineIndex,
            lineEndLower.EndPositionIndexExclusive,
            lineEndUpper.EndPositionIndexExclusive,
            lineEndLower,
            lineEndUpper);
    }

    public static LineInformation GetLineInformationFromPositionIndex(this ITextEditorModel model, int positionIndex)
    {
        model.AssertPositionIndex(positionIndex);

        int GetLineIndexFromPositionIndex()
        {
            // StartOfFile
            if (model.LineEndList[0].EndPositionIndexExclusive > positionIndex)
                return 0;

            // EndOfFile
            if (model.LineEndList[^1].EndPositionIndexExclusive <= positionIndex)
                return model.LineEndList.Count - 1;

            // In-between
            for (var i = 1; i < model.LineEndList.Count; i++)
            {
                var lineEndTuple = model.LineEndList[i];

                if (lineEndTuple.EndPositionIndexExclusive > positionIndex)
                    return i;
            }

            // Fallback return StartOfFile
            return 0;
        }

        return model.GetLineInformation(GetLineIndexFromPositionIndex());
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

        var lineStartPositionIndex = model.GetLineInformation(lineIndex).StartPositionIndexInclusive;

        if (lineIndex > model.LineEndList.Count - 1)
            return -1;

        var lastPositionIndexOnRow = model.LineEndList[lineIndex].EndPositionIndexExclusive - 1;
        var positionIndex = model.GetPositionIndex(lineIndex, columnIndex);

        if (moveBackwards)
        {
            if (positionIndex <= lineStartPositionIndex)
                return -1;

            positionIndex -= 1;
        }

        if (positionIndex < 0 || positionIndex >= model.RichCharacterList.Count)
            return -1;

        var startCharacterKind = CharacterKindHelper.CharToCharacterKind(model.RichCharacterList[positionIndex].Value);

        while (true)
        {
            if (positionIndex >= model.RichCharacterList.Count ||
                positionIndex > lastPositionIndexOnRow ||
                positionIndex < lineStartPositionIndex)
            {
                return -1;
            }

            var currentCharacterKind = CharacterKindHelper.CharToCharacterKind(model.RichCharacterList[positionIndex].Value);

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
        model.AssertPositionIndex(positionIndex);

        if (positionIndex == model.DocumentLength)
            return CharacterKind.Bad;

        return CharacterKindHelper.CharToCharacterKind(model.RichCharacterList[positionIndex].Value);
    }

    /// <summary>
    /// This method and <see cref="ReadNextWordOrDefault(ITextEditorModel, int, int)"/>
    /// are separate because of 'Ctrl + Space' bring up autocomplete when at a period.
    /// </summary>
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

    /// <summary>
    /// This method and <see cref="ReadPreviousWordOrDefault(ITextEditorModel, int, int, bool)"/>
    /// are separate because of 'Ctrl + Space' bring up autocomplete when at a period.
    /// </summary>
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
                wordColumnIndexEndExclusive = model.GetLineLength(lineIndex);

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
        var lineStartPositionIndexInclusive = model.GetLineInformation(textEditorCursor.LineIndex).StartPositionIndexInclusive;

        return model.GetString(lineStartPositionIndexInclusive, cursorPositionIndex - lineStartPositionIndexInclusive);
    }

    public static string GetLineText(this ITextEditorModel model, int lineIndex)
    {
        var lineStartPositionIndexInclusive = model.GetLineInformation(lineIndex).StartPositionIndexInclusive;
        var lengthOfLine = model.GetLineLength(lineIndex, true);

        return model.GetString(lineStartPositionIndexInclusive, lengthOfLine);
    }

    public static void AssertColumnIndex(this ITextEditorModel model, LineInformation line, int columnIndex)
    {
        if (columnIndex < 0)
            throw new LuthetusTextEditorException($"'{nameof(columnIndex)}={columnIndex}' < 0");
        
        if (columnIndex > line.LastValidColumnIndex)
            throw new LuthetusTextEditorException($"'{nameof(columnIndex)}={columnIndex}' > {nameof(line)}.{nameof(line.LastValidColumnIndex)}");
    }
    
    public static void AssertLineIndex(this ITextEditorModel model, int lineIndex)
    {
        if (lineIndex < 0)
            throw new LuthetusTextEditorException($"'{nameof(lineIndex)}={lineIndex}' < 0");
        
        if (lineIndex >= model.LineCount)
            throw new LuthetusTextEditorException($"'{nameof(lineIndex)}={lineIndex}' >= {nameof(model)}.{nameof(model.LineCount)}");
    }

    public static void AssertPositionIndex(this ITextEditorModel model, int positionIndex)
    {
        if (positionIndex < 0)
            throw new LuthetusTextEditorException($"'{nameof(positionIndex)}={positionIndex}' < 0");
        
        // NOTE: model.DocumentLength is a valid position for the cursor to be at.
        if (positionIndex > model.DocumentLength)
            throw new LuthetusTextEditorException($"'{nameof(positionIndex)}={positionIndex}' > {nameof(model)}.{nameof(model.DocumentLength)}");
    }
    
    public static void AssertCount(this ITextEditorModel model, int count)
    {
        if (count < 0)
            throw new LuthetusTextEditorException($"'{nameof(count)}={count}' < 0");
    }
}
