using System.Collections.Immutable;
using Microsoft.AspNetCore.Components.Web;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.States;
using static Luthetus.TextEditor.RazorLib.TextEditors.States.TextEditorModelState;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.Edits.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.Common.RazorLib.Keyboards.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

public partial class TextEditorModel
{
    /// <summary>The cursor is a separate Blazor Component and at times will try to access out of bounds locations.<br/><br/>When cursor accesses out of bounds location return the final RowIndex, and that row's final ColumnIndex</summary>
    public (int positionIndex, RowEndingKind rowEndingKind) GetStartOfRowTuple(int rowIndex)
    {
        if (rowIndex > _rowEndingPositionsBag.Count - 1)
            rowIndex = _rowEndingPositionsBag.Count - 1;

        if (rowIndex > 0)
            return _rowEndingPositionsBag[rowIndex - 1];

        return (0, RowEndingKind.StartOfFile);
    }

    /// <summary>Returns the Length of a row however it does not include the line ending characters by default. To include line ending characters the parameter <see cref="includeLineEndingCharacters" /> must be true.</summary>
    public int GetLengthOfRow(int rowIndex, bool includeLineEndingCharacters = false)
    {
        if (!_rowEndingPositionsBag.Any())
            return 0;

        if (rowIndex > _rowEndingPositionsBag.Count - 1)
            rowIndex = _rowEndingPositionsBag.Count - 1;

        if (rowIndex < 0)
            rowIndex = 0;

        var startOfRowTupleInclusive = GetStartOfRowTuple(rowIndex);

        // TODO: Index was out of range exception on 2023-04-18
        var endOfRowTupleExclusive = _rowEndingPositionsBag[rowIndex];

        var lengthOfRowWithLineEndings = endOfRowTupleExclusive.positionIndex - startOfRowTupleInclusive.positionIndex;

        if (includeLineEndingCharacters)
            return lengthOfRowWithLineEndings;

        return lengthOfRowWithLineEndings - endOfRowTupleExclusive.rowEndingKind.AsCharacters().Length;
    }

    /// <param name="startingRowIndex">The starting index of the rows to return</param>
    /// <param name="count">count of 0 returns 0 rows. count of 1 returns the startingRowIndex.</param>
    public List<List<RichCharacter>> GetRows(int startingRowIndex, int count)
    {
        var rowCountAvailable = _rowEndingPositionsBag.Count - startingRowIndex;

        var rowCountToReturn = count < rowCountAvailable
            ? count
            : rowCountAvailable;

        var endingRowIndexExclusive = startingRowIndex + rowCountToReturn;

        var rowsBag = new List<List<RichCharacter>>();

        for (var i = startingRowIndex; i < endingRowIndexExclusive; i++)
        {
            // Previous row's line ending position is this row's start.
            var startOfRowInclusive = GetStartOfRowTuple(i).positionIndex;

            var endOfRowExclusive = _rowEndingPositionsBag[i].positionIndex;

            var row = _contentBag
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

        var tabs = _tabKeyPositionsBag
            .SkipWhile(positionIndex => positionIndex < startOfRowPositionIndex)
            .TakeWhile(positionIndex => positionIndex < startOfRowPositionIndex + columnIndex);

        return tabs.Count();
    }

    public TextEditorModel PerformForceRerenderAction(TextEditorModelState.ForceRerenderAction forceRerenderAction)
    {
        return new TextEditorModel(this);
    }

    public TextEditorModel PerformEditTextEditorAction(
        TextEditorModelState.KeyboardEventAction keyboardEventAction)
    {
        if (KeyboardKeyFacts.IsMetaKey(keyboardEventAction.KeyboardEventArgs))
        {
            if (KeyboardKeyFacts.MetaKeys.BACKSPACE == keyboardEventAction.KeyboardEventArgs.Key ||
                KeyboardKeyFacts.MetaKeys.DELETE == keyboardEventAction.KeyboardEventArgs.Key)
            {
                PerformDeletions(keyboardEventAction);
            }
        }
        else
        {
            var cursorSnapshotsBag = TextEditorCursorSnapshot.TakeSnapshots(
                keyboardEventAction.CursorSnapshotsBag.Select(x => x.UserCursor).ToArray());

            var primaryCursorSnapshot = cursorSnapshotsBag.FirstOrDefault(x => x.UserCursor.IsPrimaryCursor);

            if (primaryCursorSnapshot is null)
                return new TextEditorModel(this);

            /*
             * TODO: 2022-11-18 one must not use the UserCursor while
             * calculating but instead make a copy of the mutable cursor
             * by looking at the snapshot and mutate that local 'userCursor'
             * then once the transaction is done offer the 'userCursor' to the
             * user interface such that it can choose to move the actual user cursor
             * to that position
             */

            // TODO: start making a mutable copy of their immutable cursor snapshot
            // so if the user moves the cursor
            // while calculating nothing can go wrong causing exception
            //
            // See the var localCursor in this contiguous code block.
            //
            // var localCursor = new TextEditorCursor(
            //     (primaryCursorSnapshot.ImmutableCursor.RowIndex, primaryCursorSnapshot.ImmutableCursor.ColumnIndex), 
            //     true);

            if (TextEditorSelectionHelper.HasSelectedText(primaryCursorSnapshot.ImmutableCursor.ImmutableSelection))
            {
                PerformDeletions(new KeyboardEventAction(
                    keyboardEventAction.ResourceUri,
                    cursorSnapshotsBag,
                    new KeyboardEventArgs
                    {
                        Code = KeyboardKeyFacts.MetaKeys.DELETE,
                        Key = KeyboardKeyFacts.MetaKeys.DELETE,
                    },
                    CancellationToken.None));
            }

            var innerCursorSnapshotsBag = TextEditorCursorSnapshot.TakeSnapshots(
                keyboardEventAction.CursorSnapshotsBag.Select(x => x.UserCursor).ToArray());

            PerformInsertions(keyboardEventAction with
            {
                CursorSnapshotsBag = innerCursorSnapshotsBag
            });
        }

        return new TextEditorModel(this);
    }

    public TextEditorModel PerformEditTextEditorAction(TextEditorModelState.InsertTextAction insertTextAction)
    {
        var cursorSnapshotsBag = TextEditorCursorSnapshot.TakeSnapshots(
            insertTextAction.CursorSnapshotsBag.Select(x => x.UserCursor).ToArray());

        var primaryCursorSnapshot = cursorSnapshotsBag.FirstOrDefault(x => x.UserCursor.IsPrimaryCursor);

        if (primaryCursorSnapshot is null)
            return new TextEditorModel(this);

        /*
         * TODO: 2022-11-18 one must not use the UserCursor while
         * calculating but instead make a copy of the mutable cursor
         * by looking at the snapshot and mutate that local 'userCursor'
         * then once the transaction is done offer the 'userCursor' to the
         * user interface such that it can choose to move the actual user cursor
         * to that position
         */

        // TODO: start making a mutable copy of their immutable cursor snapshot
        // so if the user moves the cursor
        // while calculating nothing can go wrong causing exception
        //
        // See the var localCursor in this contiguous code block.
        //
        // var localCursor = new TextEditorCursor(
        //     (primaryCursorSnapshot.ImmutableCursor.RowIndex, primaryCursorSnapshot.ImmutableCursor.ColumnIndex), 
        //     true);

        if (TextEditorSelectionHelper.HasSelectedText(primaryCursorSnapshot.ImmutableCursor.ImmutableSelection))
        {
            PerformDeletions(new KeyboardEventAction(
                insertTextAction.ResourceUri,
                cursorSnapshotsBag,
                new KeyboardEventArgs
                {
                    Code = KeyboardKeyFacts.MetaKeys.DELETE,
                    Key = KeyboardKeyFacts.MetaKeys.DELETE,
                },
                CancellationToken.None));
        }

        var localContent = insertTextAction.Content.Replace("\r\n", "\n");

        foreach (var character in localContent)
        {
            // TODO: This needs to be rewritten everything should be inserted at the same time not a foreach loop insertion for each character
            //
            // Need innerCursorSnapshots because need
            // after every loop of the foreach that the
            // cursor snapshots are updated
            var innerCursorSnapshotsBag = TextEditorCursorSnapshot.TakeSnapshots(
                insertTextAction.CursorSnapshotsBag.Select(x => x.UserCursor).ToArray());

            var code = character switch
            {
                '\r' => KeyboardKeyFacts.WhitespaceCodes.CARRIAGE_RETURN_CODE,
                '\n' => KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE,
                '\t' => KeyboardKeyFacts.WhitespaceCodes.TAB_CODE,
                ' ' => KeyboardKeyFacts.WhitespaceCodes.SPACE_CODE,
                _ => character.ToString(),
            };

            var keyboardEventTextEditorModelAction = new KeyboardEventAction(
                insertTextAction.ResourceUri,
                innerCursorSnapshotsBag,
                new KeyboardEventArgs
                {
                    Code = code,
                    Key = character.ToString(),
                },
                CancellationToken.None);

            PerformEditTextEditorAction(keyboardEventTextEditorModelAction);
        }

        return new TextEditorModel(this);
    }

    public TextEditorModel PerformEditTextEditorAction(DeleteTextByMotionAction deleteTextByMotionAction)
    {
        var keyboardEventArgs = deleteTextByMotionAction.MotionKind switch
        {
            MotionKind.Backspace => new KeyboardEventArgs { Key = KeyboardKeyFacts.MetaKeys.BACKSPACE },
            MotionKind.Delete => new KeyboardEventArgs { Key = KeyboardKeyFacts.MetaKeys.DELETE },
            _ => throw new ApplicationException($"The {nameof(MotionKind)}: {deleteTextByMotionAction.MotionKind} was not recognized.")
        };

        var keyboardEventTextEditorModelAction = new KeyboardEventAction(
            deleteTextByMotionAction.ResourceUri,
            deleteTextByMotionAction.CursorSnapshotsBag,
            keyboardEventArgs,
            CancellationToken.None);

        PerformEditTextEditorAction(keyboardEventTextEditorModelAction);

        return new TextEditorModel(this);
    }

    public TextEditorModel PerformEditTextEditorAction(DeleteTextByRangeAction deleteTextByRangeAction)
    {
        // TODO: This needs to be rewritten everything should be deleted at the same time not a foreach loop for each character
        for (var i = 0; i < deleteTextByRangeAction.Count; i++)
        {
            // Need innerCursorSnapshots because need
            // after every loop of the foreach that the
            // cursor snapshots are updated
            var innerCursorSnapshotsBag = TextEditorCursorSnapshot.TakeSnapshots(
                deleteTextByRangeAction.CursorSnapshotsBag.Select(x => x.UserCursor).ToArray());

            var keyboardEventTextEditorModelAction = new KeyboardEventAction(
                deleteTextByRangeAction.ResourceUri,
                innerCursorSnapshotsBag,
                new KeyboardEventArgs
                {
                    Code = KeyboardKeyFacts.MetaKeys.DELETE,
                    Key = KeyboardKeyFacts.MetaKeys.DELETE,
                },
                CancellationToken.None);

            PerformEditTextEditorAction(keyboardEventTextEditorModelAction);
        }

        return new TextEditorModel(this);
    }

    public TextEditorModel PerformRegisterPresentationModelAction(RegisterPresentationModelAction registerPresentationModelAction)
    {
        if (!_presentationModelsBag.Any(x => x.TextEditorPresentationKey == registerPresentationModelAction.PresentationModel.TextEditorPresentationKey))
            _presentationModelsBag.Add(registerPresentationModelAction.PresentationModel);

        return new TextEditorModel(this);
    }
    
    public TextEditorModel PerformCalculatePresentationModelAction(CalculatePresentationModelAction calculatePresentationModelAction)
    {
        var indexOfPresentationModel = _presentationModelsBag.FindIndex(
            x => x.TextEditorPresentationKey == calculatePresentationModelAction.PresentationKey);

        if (indexOfPresentationModel == -1)
            return new TextEditorModel(this);

        var presentationModel = _presentationModelsBag[indexOfPresentationModel];

        presentationModel.PendingCalculation = new(GetAllText());

        return new TextEditorModel(this);
    }

    /// <summary>If applying syntax highlighting it may be preferred to use <see cref="ApplySyntaxHighlightingAsync" />. It is effectively just invoking the lexer and then <see cref="ApplyDecorationRange" /></summary>
    public void ApplyDecorationRange(IEnumerable<TextEditorTextSpan> textEditorTextSpans)
    {
        var localContent = _contentBag;

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
        return new string(_contentBag.Select(rc => rc.Value).ToArray());
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
        if (positionIndex < 0 || positionIndex >= _contentBag.Count)
            return ParserFacts.END_OF_FILE;

        return _contentBag[positionIndex].Value;
    }

    public string GetTextRange(int startingPositionIndex, int count)
    {
        return new string(_contentBag
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
        for (var i = _rowEndingPositionsBag.Count - 1; i >= 0; i--)
        {
            var rowEndingTuple = _rowEndingPositionsBag[i];

            if (positionIndex >= rowEndingTuple.positionIndex)
            {
                return (i + 1, rowEndingTuple.positionIndex,
                    i == _rowEndingPositionsBag.Count - 1
                        ? rowEndingTuple
                        : _rowEndingPositionsBag[i + 1]);
            }
        }

        return (0, 0, _rowEndingPositionsBag[0]);
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

        if (rowIndex > _rowEndingPositionsBag.Count - 1)
            return -1;

        var lastPositionIndexOnRow = _rowEndingPositionsBag[rowIndex].positionIndex - 1;

        var positionIndex = GetPositionIndex(rowIndex, columnIndex);

        if (moveBackwards)
        {
            if (positionIndex <= startOfRowPositionIndex)
                return -1;

            positionIndex -= 1;
        }

        if (positionIndex < 0 || positionIndex >= _contentBag.Count)
            return -1;

        var startingCharacterKind = _contentBag[positionIndex].GetCharacterKind();

        while (true)
        {
            if (positionIndex >= _contentBag.Count ||
                positionIndex > lastPositionIndexOnRow ||
                positionIndex < startOfRowPositionIndex)
            {
                return -1;
            }

            var currentCharacterKind = _contentBag[positionIndex].GetCharacterKind();

            if (currentCharacterKind != startingCharacterKind)
                break;

            positionIndex += iterateBy;
        }

        if (moveBackwards)
            positionIndex += 1;

        return positionIndex - startOfRowPositionIndex;
    }

    public void SetDecorationMapper(IDecorationMapper? decorationMapper)
    {
        DecorationMapper = decorationMapper ?? new TextEditorDecorationMapperDefault();
        // TODO: Invoke an event?
    }

    public void SetCompilerService(ICompilerService compilerService)
    {
        CompilerService = compilerService;
        // TODO: Invoke an event?
    }

    public TextEditorModel SetResourceData(ResourceUri resourceUri, DateTime resourceLastWriteTime)
    {
        var nextTextEditor = new TextEditorModel(this);

        nextTextEditor.ResourceUri = resourceUri;
        nextTextEditor.ResourceLastWriteTime = resourceLastWriteTime;

        return nextTextEditor;
    }

    public TextEditorModel SetUsingRowEndingKind(RowEndingKind rowEndingKind)
    {
        UsingRowEndingKind = rowEndingKind;
        return new TextEditorModel(this);
    }

    public ImmutableArray<RichCharacter> GetAllRichCharacters()
    {
        return _contentBag.ToImmutableArray();
    }

    public void ClearEditBlocks()
    {
        EditBlockIndex = 0;
        _editBlocksPersistedBag.Clear();
    }

    public bool CanUndoEdit()
    {
        return EditBlockIndex > 0;
    }

    public bool CanRedoEdit()
    {
        return EditBlockIndex < _editBlocksPersistedBag.Count - 1;
    }

    /// <summary>The "if (EditBlockIndex == _editBlocksPersisted.Count)"<br/><br/>Is done because the active EditBlock is not yet persisted.<br/><br/>The active EditBlock is instead being 'created' as the user continues to make edits of the same <see cref="TextEditKind"/><br/><br/>For complete clarity, this comment refers to one possibly expecting to see "if (EditBlockIndex == _editBlocksPersisted.Count - 1)"</summary>
    public TextEditorModel UndoEdit()
    {
        if (!CanUndoEdit())
            return this;

        if (EditBlockIndex == _editBlocksPersistedBag.Count)
        {
            // If the edit block is pending then persist it
            // before reverting back to the previous persisted edit block.

            EnsureUndoPoint(TextEditKind.ForcePersistEditBlock);
            EditBlockIndex--;
        }

        EditBlockIndex--;

        var restoreEditBlock = _editBlocksPersistedBag[EditBlockIndex];

        SetContent(restoreEditBlock.ContentSnapshot);

        return new TextEditorModel(this);
    }

    public TextEditorModel RedoEdit()
    {
        if (!CanRedoEdit())
            return this;

        EditBlockIndex++;

        var restoreEditBlock = _editBlocksPersistedBag[EditBlockIndex];

        SetContent(restoreEditBlock.ContentSnapshot);

        return new TextEditorModel(this);
    }

    public CharacterKind GetCharacterKindAt(int positionIndex)
    {
        try
        {
            return _contentBag[positionIndex].GetCharacterKind();
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