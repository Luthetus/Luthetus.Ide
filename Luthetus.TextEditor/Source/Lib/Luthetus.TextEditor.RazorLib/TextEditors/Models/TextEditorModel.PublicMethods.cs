using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using static Luthetus.TextEditor.RazorLib.TextEditors.States.TextEditorModelState;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.Edits.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

public partial class TextEditorModel
{
    /// <summary>The cursor is a separate Blazor Component and at times will try to access out of bounds locations.<br/><br/>When cursor accesses out of bounds location return the final RowIndex, and that row's final ColumnIndex</summary>
    public (int positionIndex, RowEndingKind rowEndingKind) GetStartOfRowTuple(int rowIndex)
    {
        if (rowIndex > RowEndingPositionsBag.Count - 1)
            rowIndex = RowEndingPositionsBag.Count - 1;

        if (rowIndex > 0)
            return RowEndingPositionsBag[rowIndex - 1];

        return (0, RowEndingKind.StartOfFile);
    }

    /// <summary>Returns the Length of a row however it does not include the line ending characters by default. To include line ending characters the parameter <see cref="includeLineEndingCharacters" /> must be true.</summary>
    public int GetLengthOfRow(int rowIndex, bool includeLineEndingCharacters = false)
    {
        if (!RowEndingPositionsBag.Any())
            return 0;

        if (rowIndex > RowEndingPositionsBag.Count - 1)
            rowIndex = RowEndingPositionsBag.Count - 1;

        if (rowIndex < 0)
            rowIndex = 0;

        var startOfRowTupleInclusive = GetStartOfRowTuple(rowIndex);

        // TODO: Index was out of range exception on 2023-04-18
        var endOfRowTupleExclusive = RowEndingPositionsBag[rowIndex];

        var lengthOfRowWithLineEndings = endOfRowTupleExclusive.positionIndex - startOfRowTupleInclusive.positionIndex;

        if (includeLineEndingCharacters)
            return lengthOfRowWithLineEndings;

        return lengthOfRowWithLineEndings - endOfRowTupleExclusive.rowEndingKind.AsCharacters().Length;
    }

    /// <param name="startingRowIndex">The starting index of the rows to return</param>
    /// <param name="count">count of 0 returns 0 rows. count of 1 returns the startingRowIndex.</param>
    public List<List<RichCharacter>> GetRows(int startingRowIndex, int count)
    {
        var rowCountAvailable = RowEndingPositionsBag.Count - startingRowIndex;

        var rowCountToReturn = count < rowCountAvailable
            ? count
            : rowCountAvailable;

        var endingRowIndexExclusive = startingRowIndex + rowCountToReturn;

        var rowsBag = new List<List<RichCharacter>>();

        for (var i = startingRowIndex; i < endingRowIndexExclusive; i++)
        {
            // Previous row's line ending position is this row's start.
            var startOfRowInclusive = GetStartOfRowTuple(i).positionIndex;

            var endOfRowExclusive = RowEndingPositionsBag[i].positionIndex;

            var row = ContentBag
                .Skip(startOfRowInclusive)
                .Take(endOfRowExclusive - startOfRowInclusive)
                .ToList();

            rowsBag.Add(row);
        }

        return rowsBag;
    }

    public int GetTabsCountOnSameRowBeforeCursor(int rowIndex, int columnIndex)
    {
        var startOfRowPositionIndex = GetStartOfRowTuple(rowIndex).positionIndex;

        var tabs = TabKeyPositionsBag
            .SkipWhile(positionIndex => positionIndex < startOfRowPositionIndex)
            .TakeWhile(positionIndex => positionIndex < startOfRowPositionIndex + columnIndex);

        return tabs.Count();
    }

    public TextEditorModel PerformForceRerenderAction(ForceRerenderAction forceRerenderAction)
    {
        var modelModifier = new TextEditorModelModifier(this);
        return modelModifier.ToTextEditorModel();
    }

    public TextEditorModel PerformEditTextEditorAction(KeyboardEventAction keyboardEventAction)
    {
        var modelModifier = new TextEditorModelModifier(this);
        modelModifier.PerformEditTextEditorAction(keyboardEventAction);
		return modelModifier.ToTextEditorModel();
    }

    public TextEditorModel PerformEditTextEditorAction(InsertTextAction insertTextAction)
    {
		var modelModifier = new TextEditorModelModifier(this);
		modelModifier.PerformEditTextEditorAction(insertTextAction);
		return modelModifier.ToTextEditorModel();
    }

    public TextEditorModel PerformEditTextEditorAction(DeleteTextByMotionAction deleteTextByMotionAction)
    {
		var modelModifier = new TextEditorModelModifier(this);
		modelModifier.PerformEditTextEditorAction(deleteTextByMotionAction);
		return modelModifier.ToTextEditorModel();
    }

    public TextEditorModel PerformEditTextEditorAction(DeleteTextByRangeAction deleteTextByRangeAction)
    {
		var modelModifier = new TextEditorModelModifier(this);
        modelModifier.PerformEditTextEditorAction(deleteTextByRangeAction);
		return modelModifier.ToTextEditorModel();
    }

    public TextEditorModel PerformRegisterPresentationModelAction(RegisterPresentationModelAction registerPresentationModelAction)
    {
		var modelModifier = new TextEditorModelModifier(this);
		modelModifier.PerformRegisterPresentationModelAction(registerPresentationModelAction);
		return modelModifier.ToTextEditorModel();
    }
    
    public TextEditorModel PerformCalculatePresentationModelAction(CalculatePresentationModelAction calculatePresentationModelAction)
    {
		var modelModifier = new TextEditorModelModifier(this);
		modelModifier.PerformCalculatePresentationModelAction(calculatePresentationModelAction);
		return modelModifier.ToTextEditorModel();
    }

    /// <summary>If applying syntax highlighting it may be preferred to use <see cref="ApplySyntaxHighlightingAsync" />. It is effectively just invoking the lexer and then <see cref="ApplyDecorationRange" /></summary>
    public void ApplyDecorationRange(IEnumerable<TextEditorTextSpan> textEditorTextSpans)
    {
        var localContent = ContentBag;

        var positionsPainted = new HashSet<int>();

        foreach (var textEditorTextSpan in textEditorTextSpans)
        {
            for (var i = textEditorTextSpan.StartingIndexInclusive; i < textEditorTextSpan.EndingIndexExclusive; i++)
            {
                if (i >= localContent.Count)
                    continue;

                localContent[i].DecorationByte = textEditorTextSpan.DecorationByte;

                positionsPainted.Add(i);
            }
        }

        for (var i = 0; i < localContent.Count - 1; i++)
        {
            if (!positionsPainted.Contains(i))
            {
                // DecorationByte of 0 is to be 'None'
                localContent[i].DecorationByte = 0;
            }
        }
    }

    public Task ApplySyntaxHighlightingAsync()
    {
        var syntacticTextSpansBag = CompilerService.GetSyntacticTextSpansFor(ResourceUri);
        var symbolsBag = CompilerService.GetSymbolsFor(ResourceUri);

        var symbolTextSpansBag = symbolsBag.Select(s => s.TextSpan);

        ApplyDecorationRange(syntacticTextSpansBag.Union(symbolTextSpansBag));

        return Task.CompletedTask;
    }

    public string GetAllText()
    {
        return new string(ContentBag.Select(rc => rc.Value).ToArray());
    }

    public int GetCursorPositionIndex(TextEditorCursor textEditorCursor)
    {
        return GetPositionIndex(textEditorCursor.IndexCoordinates.rowIndex, textEditorCursor.IndexCoordinates.columnIndex);
    }

    public int GetCursorPositionIndex(ImmutableTextEditorCursor immutableTextEditorCursor)
    {
        return GetPositionIndex(immutableTextEditorCursor.RowIndex, immutableTextEditorCursor.ColumnIndex);
    }

    public int GetPositionIndex(int rowIndex, int columnIndex)
    {
        var startOfRowPositionIndex = GetStartOfRowTuple(rowIndex).positionIndex;
        return startOfRowPositionIndex + columnIndex;
    }

    public char GetTextAt(int positionIndex)
    {
        if (positionIndex < 0 || positionIndex >= ContentBag.Count)
            return ParserFacts.END_OF_FILE;

        return ContentBag[positionIndex].Value;
    }

    public string GetTextRange(int startingPositionIndex, int count)
    {
        return new string(ContentBag
            .Skip(startingPositionIndex)
            .Take(count)
            .Select(rc => rc.Value)
            .ToArray());
    }

    public string GetLinesRange(int startingRowIndex, int count)
    {
        var startingPositionIndexInclusive = GetPositionIndex(startingRowIndex, 0);
        var lastRowIndexExclusive = startingRowIndex + count;
        var endingPositionIndexExclusive = GetPositionIndex(lastRowIndexExclusive, 0);

        return GetTextRange(
            startingPositionIndexInclusive,
            endingPositionIndexExclusive - startingPositionIndexInclusive);
    }

    /// <summary>Given a <see cref="TextEditorModel"/> with a preference for the right side of the cursor, the following conditional branch will play out.<br/><br/>-IF the cursor is amongst a word, that word will be returned.<br/><br/>-ELSE IF the start of a word is to the right of the cursor that word will be returned.<br/><br/>-ELSE IF the end of a word is to the left of the cursor that word will be returned.</summary>
    public TextEditorTextSpan? GetWordAt(int positionIndex)
    {
        var previousCharacter = GetTextAt(positionIndex - 1);
        var currentCharacter = GetTextAt(positionIndex);

        var previousCharacterKind = CharacterKindHelper.CharToCharacterKind(previousCharacter);
        var currentCharacterKind = CharacterKindHelper.CharToCharacterKind(currentCharacter);

        var rowInformation = FindRowInformation(positionIndex);

        var columnIndex = positionIndex - rowInformation.rowStartPositionIndex;

        if (previousCharacterKind == CharacterKind.LetterOrDigit && currentCharacterKind == CharacterKind.LetterOrDigit)
        {
            var wordColumnIndexStartInclusive = GetColumnIndexOfCharacterWithDifferingKind(
                rowInformation.rowIndex,
                columnIndex,
                true);

            if (wordColumnIndexStartInclusive == -1)
                wordColumnIndexStartInclusive = 0;

            var wordColumnIndexEndExclusive = GetColumnIndexOfCharacterWithDifferingKind(
                rowInformation.rowIndex,
                columnIndex,
                false);

            if (wordColumnIndexEndExclusive == -1)
                wordColumnIndexEndExclusive = GetLengthOfRow(rowInformation.rowIndex);

            return new TextEditorTextSpan(
                wordColumnIndexStartInclusive + rowInformation.rowStartPositionIndex,
                wordColumnIndexEndExclusive + rowInformation.rowStartPositionIndex,
                0,
                ResourceUri,
                GetAllText());
        }
        else if (currentCharacterKind == CharacterKind.LetterOrDigit)
        {
            var wordColumnIndexEndExclusive = GetColumnIndexOfCharacterWithDifferingKind(
                rowInformation.rowIndex,
                columnIndex,
                false);

            if (wordColumnIndexEndExclusive == -1)
                wordColumnIndexEndExclusive = GetLengthOfRow(rowInformation.rowIndex);

            return new TextEditorTextSpan(
                columnIndex + rowInformation.rowStartPositionIndex,
                wordColumnIndexEndExclusive + rowInformation.rowStartPositionIndex,
                0,
                ResourceUri,
                GetAllText());
        }
        else if (previousCharacterKind == CharacterKind.LetterOrDigit)
        {
            var wordColumnIndexStartInclusive = GetColumnIndexOfCharacterWithDifferingKind(
                rowInformation.rowIndex,
                columnIndex,
                true);

            if (wordColumnIndexStartInclusive == -1)
                wordColumnIndexStartInclusive = 0;

            return new TextEditorTextSpan(
                wordColumnIndexStartInclusive + rowInformation.rowStartPositionIndex,
                columnIndex + rowInformation.rowStartPositionIndex,
                0,
                ResourceUri,
                GetAllText());
        }

        return null;
    }

    public (int rowIndex, int rowStartPositionIndex, (int positionIndex, RowEndingKind rowEndingKind) rowEndingTuple)
        FindRowInformation(int positionIndex)
    {
        for (var i = RowEndingPositionsBag.Count - 1; i >= 0; i--)
        {
            var rowEndingTuple = RowEndingPositionsBag[i];

            if (positionIndex >= rowEndingTuple.positionIndex)
            {
                return (i + 1, rowEndingTuple.positionIndex,
                    i == RowEndingPositionsBag.Count - 1
                        ? rowEndingTuple
                        : RowEndingPositionsBag[i + 1]);
            }
        }

        return (0, 0, RowEndingPositionsBag[0]);
    }

    /// <summary>
    /// <see cref="moveBackwards"/> is to mean earlier in the document
    /// (lower column index or lower row index depending on position) 
    /// </summary>
    /// <returns>Will return -1 if no valid result was found.</returns>
    public int GetColumnIndexOfCharacterWithDifferingKind(int rowIndex, int columnIndex, bool moveBackwards)
    {
        var iterateBy = moveBackwards
            ? -1
            : 1;

        var startOfRowPositionIndex = GetStartOfRowTuple(rowIndex).positionIndex;

        if (rowIndex > RowEndingPositionsBag.Count - 1)
            return -1;

        var lastPositionIndexOnRow = RowEndingPositionsBag[rowIndex].positionIndex - 1;

        var positionIndex = GetPositionIndex(rowIndex, columnIndex);

        if (moveBackwards)
        {
            if (positionIndex <= startOfRowPositionIndex)
                return -1;

            positionIndex -= 1;
        }

        if (positionIndex < 0 || positionIndex >= ContentBag.Count)
            return -1;

        var startingCharacterKind = ContentBag[positionIndex].GetCharacterKind();

        while (true)
        {
            if (positionIndex >= ContentBag.Count ||
                positionIndex > lastPositionIndexOnRow ||
                positionIndex < startOfRowPositionIndex)
            {
                return -1;
            }

            var currentCharacterKind = ContentBag[positionIndex].GetCharacterKind();

            if (currentCharacterKind != startingCharacterKind)
                break;

            positionIndex += iterateBy;
        }

        if (moveBackwards)
            positionIndex += 1;

        return positionIndex - startOfRowPositionIndex;
    }

    public TextEditorModel SetDecorationMapper(IDecorationMapper decorationMapper)
    {
		var modelModifier = new TextEditorModelModifier(this);
		modelModifier.ModifyDecorationMapper(decorationMapper);
		return modelModifier.ToTextEditorModel();
    }

    public TextEditorModel SetCompilerService(ICompilerService compilerService)
    {
		var modelModifier = new TextEditorModelModifier(this);
		modelModifier.ModifyCompilerService(compilerService);
		return modelModifier.ToTextEditorModel();
    }
    
    public TextEditorModel SetTextEditorSaveFileHelper(TextEditorSaveFileHelper textEditorSaveFileHelper)
    {
		var modelModifier = new TextEditorModelModifier(this);
		modelModifier.ModifyTextEditorSaveFileHelper(textEditorSaveFileHelper);
		return modelModifier.ToTextEditorModel();
    }

    public TextEditorModel SetResourceData(ResourceUri resourceUri, DateTime resourceLastWriteTime)
    {
		var modelModifier = new TextEditorModelModifier(this);
		modelModifier.ModifyResourceData(resourceUri, resourceLastWriteTime);
		return modelModifier.ToTextEditorModel();
    }

    public TextEditorModel SetUsingRowEndingKind(RowEndingKind rowEndingKind)
    {
		var modelModifier = new TextEditorModelModifier(this);
		modelModifier.ModifyUsingRowEndingKind(rowEndingKind);
		return modelModifier.ToTextEditorModel();
    }

    public ImmutableArray<RichCharacter> GetAllRichCharacters()
    {
        return ContentBag.ToImmutableArray();
    }

    public TextEditorModel ClearEditBlocks()
    {
		var modelModifier = new TextEditorModelModifier(this);
		modelModifier.ClearEditBlocks();
		return modelModifier.ToTextEditorModel();
    }

    public bool CanUndoEdit()
    {
        return EditBlockIndex > 0;
    }

    public bool CanRedoEdit()
    {
        return EditBlockIndex < EditBlocksBag.Count - 1;
    }

    /// <summary>The "if (EditBlockIndex == _editBlocksPersisted.Count)"<br/><br/>Is done because the active EditBlock is not yet persisted.<br/><br/>The active EditBlock is instead being 'created' as the user continues to make edits of the same <see cref="TextEditKind"/><br/><br/>For complete clarity, this comment refers to one possibly expecting to see "if (EditBlockIndex == _editBlocksPersisted.Count - 1)"</summary>
    public TextEditorModel UndoEdit()
    {
		var modelModifier = new TextEditorModelModifier(this);
		modelModifier.UndoEdit();
		return modelModifier.ToTextEditorModel();
    }

    public TextEditorModel RedoEdit()
    {
		var modelModifier = new TextEditorModelModifier(this);
		modelModifier.RedoEdit();
		return modelModifier.ToTextEditorModel();
    }

    public CharacterKind GetCharacterKindAt(int positionIndex)
    {
        try
        {
            return ContentBag[positionIndex].GetCharacterKind();
        }
        catch (ArgumentOutOfRangeException)
        {
            // The text editor's cursor is is likely
            // to have this occur at times
        }

        return CharacterKind.Bad;
    }

    public string? ReadPreviousWordOrDefault(int rowIndex, int columnIndex, bool isRecursiveCall = false)
    {
        var wordPositionIndexEndExclusive = GetPositionIndex(rowIndex, columnIndex);
        var wordCharacterKind = GetCharacterKindAt(wordPositionIndexEndExclusive - 1);

        if (wordCharacterKind == CharacterKind.Punctuation && !isRecursiveCall)
        {
            // If previous previous word is a punctuation character, then perhaps
            // the user hit { 'Ctrl' + 'Space' } to trigger the autocomplete
            // and was at a MemberAccessToken (or a period '.')
            //
            // So, read the word previous to the punctuation.

            var anotherAttemptColumnIndex = columnIndex - 1;

            if (anotherAttemptColumnIndex >= 0)
                return ReadPreviousWordOrDefault(rowIndex, anotherAttemptColumnIndex, true);
        }

        if (wordCharacterKind == CharacterKind.LetterOrDigit)
        {
            var wordColumnIndexStartInclusive = GetColumnIndexOfCharacterWithDifferingKind(
                rowIndex,
                columnIndex,
                true);

            if (wordColumnIndexStartInclusive == -1)
                wordColumnIndexStartInclusive = 0;

            var wordLength = columnIndex - wordColumnIndexStartInclusive;

            var wordPositionIndexStartInclusive = wordPositionIndexEndExclusive - wordLength;

            return GetTextRange(wordPositionIndexStartInclusive, wordLength);
        }

        return null;
    }

    public string? ReadNextWordOrDefault(int rowIndex, int columnIndex)
    {
        var wordPositionIndexStartInclusive = GetPositionIndex(rowIndex, columnIndex);

        var wordCharacterKind = GetCharacterKindAt(wordPositionIndexStartInclusive);

        if (wordCharacterKind == CharacterKind.LetterOrDigit)
        {
            var wordColumnIndexEndExclusive = GetColumnIndexOfCharacterWithDifferingKind(
                rowIndex,
                columnIndex,
                false);

            if (wordColumnIndexEndExclusive == -1)
                wordColumnIndexEndExclusive = GetLengthOfRow(rowIndex);

            var wordLength = wordColumnIndexEndExclusive - columnIndex;

            return GetTextRange(wordPositionIndexStartInclusive, wordLength);
        }

        return null;
    }

    /// <summary>This method returns the text to the left of the cursor in most cases. The method name is as such because of right to left written texts.<br/><br/>One uses this method most often to measure the position of the cursor when rendering the UI for a font-family which is proportional (i.e. not monospace).</summary>
    public string GetTextOffsettingCursor(TextEditorCursor textEditorCursor)
    {
        var cursorPositionIndex = GetCursorPositionIndex(textEditorCursor);
        var startOfRowTuple = GetStartOfRowTuple(textEditorCursor.IndexCoordinates.rowIndex);

        return GetTextRange(startOfRowTuple.positionIndex, cursorPositionIndex - startOfRowTuple.positionIndex);
    }

    public string GetTextOnRow(int rowIndex)
    {
        var startOfRowTuple = GetStartOfRowTuple(rowIndex);
        var lengthOfRow = GetLengthOfRow(rowIndex);

        return GetTextRange(startOfRowTuple.positionIndex, lengthOfRow);
    }
}