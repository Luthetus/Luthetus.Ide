using System.Collections.Immutable;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.Commands.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Edits.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.States;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

public partial class TextEditorModel
{
    private void EnsureUndoPoint(TextEditKind textEditKind, string? otherTextEditKindIdentifier = null)
    {
        if (textEditKind == TextEditKind.Other && otherTextEditKindIdentifier is null)
            CommandTextEditor.ThrowOtherTextEditKindIdentifierWasExpectedException(textEditKind);

        var mostRecentEditBlock = _editBlocksPersistedBag.LastOrDefault();

        if (mostRecentEditBlock is null || mostRecentEditBlock.TextEditKind != textEditKind)
        {
            var newEditBlockIndex = EditBlockIndex;

            _editBlocksPersistedBag.Insert(newEditBlockIndex, new EditBlock(
                textEditKind,
                textEditKind.ToString(),
                GetAllText(),
                otherTextEditKindIdentifier));

            var removeBlocksStartingAt = newEditBlockIndex + 1;

            _editBlocksPersistedBag.RemoveRange(removeBlocksStartingAt, _editBlocksPersistedBag.Count - removeBlocksStartingAt);

            EditBlockIndex++;
        }

        while (_editBlocksPersistedBag.Count > MAXIMUM_EDIT_BLOCKS && _editBlocksPersistedBag.Count != 0)
        {
            EditBlockIndex--;
            _editBlocksPersistedBag.RemoveAt(0);
        }
    }

    private void PerformInsertions(TextEditorModelState.KeyboardEventAction keyboardEventAction)
    {
        EnsureUndoPoint(TextEditKind.Insertion);

        foreach (var cursorSnapshot in keyboardEventAction.CursorSnapshotsBag)
        {
            if (TextEditorSelectionHelper.HasSelectedText(cursorSnapshot.ImmutableCursor.ImmutableSelection))
            {
                PerformDeletions(keyboardEventAction);

                var selectionBounds = TextEditorSelectionHelper.GetSelectionBounds(cursorSnapshot.ImmutableCursor.ImmutableSelection);

                var lowerRowData = FindRowInformation(selectionBounds.lowerPositionIndexInclusive);

                var lowerColumnIndex = selectionBounds.lowerPositionIndexInclusive - lowerRowData.rowStartPositionIndex;

                // Move cursor to lower bound of text selection
                cursorSnapshot.UserCursor.IndexCoordinates = (lowerRowData.rowIndex, lowerColumnIndex);

                var nextEdit = keyboardEventAction with
                {
                    CursorSnapshotsBag = new[]
                    {
                        new TextEditorCursorSnapshot(cursorSnapshot.UserCursor)
                    }.ToImmutableArray()
                };

                // Because one cannot move reference of foreach variable,
                // one has to re-invoke the method with different paramters
                PerformInsertions(nextEdit);
                return;
            }

            var startOfRowPositionIndex = GetStartOfRowTuple(cursorSnapshot.ImmutableCursor.RowIndex).positionIndex;
            var cursorPositionIndex = startOfRowPositionIndex + cursorSnapshot.ImmutableCursor.ColumnIndex;

            // If cursor is out of bounds then continue
            if (cursorPositionIndex > _contentBag.Count)
                continue;

            var wasTabCode = false;
            var wasEnterCode = false;

            var characterValueToInsert = keyboardEventAction.KeyboardEventArgs.Key.First();

            if (KeyboardKeyFacts.IsWhitespaceCode(keyboardEventAction.KeyboardEventArgs.Code))
            {
                characterValueToInsert = KeyboardKeyFacts.ConvertWhitespaceCodeToCharacter(keyboardEventAction.KeyboardEventArgs.Code);
                
                wasTabCode = KeyboardKeyFacts.WhitespaceCodes.TAB_CODE == keyboardEventAction.KeyboardEventArgs.Code;
                wasEnterCode = KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE == keyboardEventAction.KeyboardEventArgs.Code;
            }

            var characterCountInserted = 1;

            if (wasEnterCode)
            {
                var rowEndingKindToInsert = UsingRowEndingKind;

                var richCharacters = rowEndingKindToInsert.AsCharacters().Select(character => new RichCharacter
                {
                    Value = character,
                    DecorationByte = default,
                });

                characterCountInserted = rowEndingKindToInsert.AsCharacters().Length;

                _contentBag.InsertRange(cursorPositionIndex, richCharacters);

                _rowEndingPositionsBag.Insert(cursorSnapshot.ImmutableCursor.RowIndex,
                    (cursorPositionIndex + characterCountInserted, rowEndingKindToInsert));

                MutateRowEndingKindCount(UsingRowEndingKind, 1);

                var indexCoordinates = cursorSnapshot.UserCursor.IndexCoordinates;

                cursorSnapshot.UserCursor.IndexCoordinates = (indexCoordinates.rowIndex + 1, 0);

                cursorSnapshot.UserCursor.PreferredColumnIndex = cursorSnapshot.UserCursor.IndexCoordinates.columnIndex;
            }
            else
            {
                if (wasTabCode)
                {
                    var index = _tabKeyPositionsBag.FindIndex(x => x >= cursorPositionIndex);

                    if (index == -1)
                    {
                        _tabKeyPositionsBag.Add(cursorPositionIndex);
                    }
                    else
                    {
                        for (var i = index; i < _tabKeyPositionsBag.Count; i++)
                        {
                            _tabKeyPositionsBag[i]++;
                        }
                        
                        _tabKeyPositionsBag.Insert(index, cursorPositionIndex);
                    }
                }

                var richCharacterToInsert = new RichCharacter
                {
                    Value = characterValueToInsert,
                    DecorationByte = default,
                };

                _contentBag.Insert(cursorPositionIndex, richCharacterToInsert);

                var indexCoordinates = cursorSnapshot.UserCursor.IndexCoordinates;

                cursorSnapshot.UserCursor.IndexCoordinates = (indexCoordinates.rowIndex, indexCoordinates.columnIndex + 1);
                cursorSnapshot.UserCursor.PreferredColumnIndex = cursorSnapshot.UserCursor.IndexCoordinates.columnIndex;
            }

            var firstRowIndexToModify = wasEnterCode
                ? cursorSnapshot.ImmutableCursor.RowIndex + 1
                : cursorSnapshot.ImmutableCursor.RowIndex;

            for (var i = firstRowIndexToModify; i < _rowEndingPositionsBag.Count; i++)
            {
                var rowEndingTuple = _rowEndingPositionsBag[i];

                _rowEndingPositionsBag[i] = (rowEndingTuple.positionIndex + characterCountInserted, rowEndingTuple.rowEndingKind);
            }

            if (!wasTabCode)
            {
                var firstTabKeyPositionIndexToModify = _tabKeyPositionsBag.FindIndex(x => x >= cursorPositionIndex);

                if (firstTabKeyPositionIndexToModify != -1)
                {
                    for (var i = firstTabKeyPositionIndexToModify; i < _tabKeyPositionsBag.Count; i++)
                    {
                        _tabKeyPositionsBag[i] += characterCountInserted;
                    }
                }
            }

            // Reposition the Diagnostic Squigglies
            {
                var textSpanForInsertion = new TextEditorTextSpan(
                    cursorPositionIndex,
                    cursorPositionIndex + characterCountInserted,
                    0,
                    new(string.Empty),
                    string.Empty);

                var textModification = new TextEditorTextModification(true, textSpanForInsertion);

                foreach (var presentationModel in _presentationModelsBag)
                {
                    if (presentationModel.CompletedCalculation is not null)
                        presentationModel.CompletedCalculation.TextModificationsSinceRequestBag.Add(textModification);
                    
                    if (presentationModel.PendingCalculation is not null)
                        presentationModel.PendingCalculation.TextModificationsSinceRequestBag.Add(textModification);
                }
            }
        }

        // TODO: Fix tracking the MostCharactersOnASingleRowTuple this way is possibly inefficient - should instead only check the rows that changed
        {
            (int rowIndex, int rowLength) localMostCharactersOnASingleRowTuple = (0, 0);

            for (var i = 0; i < _rowEndingPositionsBag.Count; i++)
            {
                var lengthOfRow = GetLengthOfRow(i);

                if (lengthOfRow > localMostCharactersOnASingleRowTuple.rowLength)
                {
                    localMostCharactersOnASingleRowTuple = (i, lengthOfRow);
                }
            }

            localMostCharactersOnASingleRowTuple = (localMostCharactersOnASingleRowTuple.rowIndex,
                localMostCharactersOnASingleRowTuple.rowLength + MOST_CHARACTERS_ON_A_SINGLE_ROW_MARGIN);

            MostCharactersOnASingleRowTuple = localMostCharactersOnASingleRowTuple;
        }
    }

    private void PerformDeletions(TextEditorModelState.KeyboardEventAction keyboardEventAction)
    {
        EnsureUndoPoint(TextEditKind.Deletion);

        foreach (var cursorSnapshot in keyboardEventAction.CursorSnapshotsBag)
        {
            var startOfRowPositionIndex = GetStartOfRowTuple(cursorSnapshot.ImmutableCursor.RowIndex).positionIndex;
            var cursorPositionIndex = startOfRowPositionIndex + cursorSnapshot.ImmutableCursor.ColumnIndex;

            // If cursor is out of bounds then continue
            if (cursorPositionIndex > _contentBag.Count)
                continue;

            int startingPositionIndexToRemoveInclusive;
            int countToRemove;
            bool moveBackwards;

            // Cannot calculate this after text was deleted - it would be wrong
            int? selectionUpperBoundRowIndex = null;
            // Needed for when text selection is deleted
            (int rowIndex, int columnIndex)? selectionLowerBoundIndexCoordinates = null;

            // TODO: The deletion logic should be the same whether it be 'Delete' 'Backspace' 'CtrlModified' or 'DeleteSelection'. What should change is one needs to calculate the starting and ending index appropriately foreach case.
            if (TextEditorSelectionHelper.HasSelectedText(cursorSnapshot.ImmutableCursor.ImmutableSelection))
            {
                var lowerPositionIndexInclusiveBound = cursorSnapshot.ImmutableCursor.ImmutableSelection.AnchorPositionIndex ?? 0;
                var upperPositionIndexExclusive = cursorSnapshot.ImmutableCursor.ImmutableSelection.EndingPositionIndex;

                if (lowerPositionIndexInclusiveBound > upperPositionIndexExclusive)
                    (lowerPositionIndexInclusiveBound, upperPositionIndexExclusive) = (upperPositionIndexExclusive, lowerPositionIndexInclusiveBound);

                var lowerRowMetaData = FindRowInformation(lowerPositionIndexInclusiveBound);
                var upperRowMetaData = FindRowInformation(upperPositionIndexExclusive);

                // Value is needed when knowing what row ending positions to update after deletion is done
                selectionUpperBoundRowIndex = upperRowMetaData.rowIndex;

                // Value is needed when knowing where to position the cursor after deletion is done
                selectionLowerBoundIndexCoordinates = (lowerRowMetaData.rowIndex,
                    lowerPositionIndexInclusiveBound - lowerRowMetaData.rowStartPositionIndex);

                startingPositionIndexToRemoveInclusive = upperPositionIndexExclusive - 1;
                countToRemove = upperPositionIndexExclusive - lowerPositionIndexInclusiveBound;
                moveBackwards = true;

                cursorSnapshot.UserCursor.Selection.AnchorPositionIndex = null;
            }
            else if (KeyboardKeyFacts.MetaKeys.BACKSPACE == keyboardEventAction.KeyboardEventArgs.Key)
            {
                moveBackwards = true;

                if (keyboardEventAction.KeyboardEventArgs.CtrlKey)
                {
                    var columnIndexOfCharacterWithDifferingKind = GetColumnIndexOfCharacterWithDifferingKind(
                        cursorSnapshot.ImmutableCursor.RowIndex,
                        cursorSnapshot.ImmutableCursor.ColumnIndex,
                        moveBackwards);

                    columnIndexOfCharacterWithDifferingKind = columnIndexOfCharacterWithDifferingKind == -1
                        ? 0
                        : columnIndexOfCharacterWithDifferingKind;

                    countToRemove = cursorSnapshot.ImmutableCursor.ColumnIndex -
                        columnIndexOfCharacterWithDifferingKind;

                    countToRemove = countToRemove == 0
                        ? 1
                        : countToRemove;
                }
                else
                {
                    countToRemove = 1;
                }

                startingPositionIndexToRemoveInclusive = cursorPositionIndex - 1;
            }
            else if (KeyboardKeyFacts.MetaKeys.DELETE == keyboardEventAction.KeyboardEventArgs.Key)
            {
                moveBackwards = false;

                if (keyboardEventAction.KeyboardEventArgs.CtrlKey)
                {
                    var columnIndexOfCharacterWithDifferingKind = GetColumnIndexOfCharacterWithDifferingKind(
                        cursorSnapshot.ImmutableCursor.RowIndex,
                        cursorSnapshot.ImmutableCursor.ColumnIndex,
                        moveBackwards);

                    columnIndexOfCharacterWithDifferingKind = columnIndexOfCharacterWithDifferingKind == -1
                        ? GetLengthOfRow(cursorSnapshot.ImmutableCursor.RowIndex)
                        : columnIndexOfCharacterWithDifferingKind;

                    countToRemove = columnIndexOfCharacterWithDifferingKind -
                        cursorSnapshot.ImmutableCursor.ColumnIndex;

                    countToRemove = countToRemove == 0
                        ? 1
                        : countToRemove;
                }
                else
                {
                    countToRemove = 1;
                }

                startingPositionIndexToRemoveInclusive = cursorPositionIndex;
            }
            else
            {
                throw new ApplicationException($"The keyboard key: {keyboardEventAction.KeyboardEventArgs.Key} was not recognized");
            }

            var charactersRemovedCount = 0;
            var rowsRemovedCount = 0;

            var indexToRemove = startingPositionIndexToRemoveInclusive;

            while (countToRemove-- > 0)
            {
                if (indexToRemove < 0 || indexToRemove > _contentBag.Count - 1)
                    break;

                var characterToDelete = _contentBag[indexToRemove];

                int startingIndexToRemoveRange;
                int countToRemoveRange;

                if (KeyboardKeyFacts.IsLineEndingCharacter(characterToDelete.Value))
                {
                    rowsRemovedCount++;

                    // rep.positionIndex == indexToRemove + 1
                    //     ^is for backspace
                    //
                    // rep.positionIndex == indexToRemove + 2
                    //     ^is for delete
                    var rowEndingTupleIndex = _rowEndingPositionsBag.FindIndex(rep =>
                        rep.positionIndex == indexToRemove + 1 ||
                        rep.positionIndex == indexToRemove + 2);

                    var rowEndingTuple = _rowEndingPositionsBag[rowEndingTupleIndex];

                    _rowEndingPositionsBag.RemoveAt(rowEndingTupleIndex);

                    var lengthOfRowEnding = rowEndingTuple.rowEndingKind.AsCharacters().Length;

                    if (moveBackwards)
                        startingIndexToRemoveRange = indexToRemove - (lengthOfRowEnding - 1);
                    else
                        startingIndexToRemoveRange = indexToRemove;

                    countToRemove -= lengthOfRowEnding - 1;
                    countToRemoveRange = lengthOfRowEnding;

                    MutateRowEndingKindCount(rowEndingTuple.rowEndingKind, -1);
                }
                else
                {
                    if (characterToDelete.Value == KeyboardKeyFacts.WhitespaceCharacters.TAB)
                        _tabKeyPositionsBag.Remove(indexToRemove);

                    startingIndexToRemoveRange = indexToRemove;
                    countToRemoveRange = 1;
                }

                charactersRemovedCount += countToRemoveRange;

                _contentBag.RemoveRange(startingIndexToRemoveRange, countToRemoveRange);

                if (moveBackwards)
                    indexToRemove -= countToRemoveRange;
            }

            if (charactersRemovedCount == 0 && rowsRemovedCount == 0)
                return;

            if (moveBackwards && !selectionUpperBoundRowIndex.HasValue)
            {
                var modifyRowsBy = -1 * rowsRemovedCount;

                var startOfCurrentRowPositionIndex = GetStartOfRowTuple(cursorSnapshot.ImmutableCursor.RowIndex + modifyRowsBy)
                    .positionIndex;

                var modifyPositionIndexBy = -1 * charactersRemovedCount;

                var endingPositionIndex = cursorPositionIndex + modifyPositionIndexBy;

                var columnIndex = endingPositionIndex - startOfCurrentRowPositionIndex;

                var indexCoordinates = cursorSnapshot.UserCursor.IndexCoordinates;

                cursorSnapshot.UserCursor.IndexCoordinates = (indexCoordinates.rowIndex + modifyRowsBy, columnIndex);
            }

            int firstRowIndexToModify;

            if (selectionUpperBoundRowIndex.HasValue)
            {
                firstRowIndexToModify = selectionLowerBoundIndexCoordinates!.Value.rowIndex;
                cursorSnapshot.UserCursor.IndexCoordinates = selectionLowerBoundIndexCoordinates!.Value;
            }
            else if (moveBackwards)
            {
                firstRowIndexToModify = cursorSnapshot.ImmutableCursor.RowIndex - rowsRemovedCount;
            }
            else
            {
                firstRowIndexToModify = cursorSnapshot.ImmutableCursor.RowIndex;
            }

            for (var i = firstRowIndexToModify; i < _rowEndingPositionsBag.Count; i++)
            {
                var rowEndingTuple = _rowEndingPositionsBag[i];
                _rowEndingPositionsBag[i] = (rowEndingTuple.positionIndex - charactersRemovedCount, rowEndingTuple.rowEndingKind);
            }

            var firstTabKeyPositionIndexToModify = _tabKeyPositionsBag.FindIndex(x => x >= startingPositionIndexToRemoveInclusive);

            if (firstTabKeyPositionIndexToModify != -1)
            {
                for (var i = firstTabKeyPositionIndexToModify; i < _tabKeyPositionsBag.Count; i++)
                {
                    _tabKeyPositionsBag[i] -= charactersRemovedCount;
                }
            }

            // Reposition the Diagnostic Squigglies
            {
                var textSpanForInsertion = new TextEditorTextSpan(
                    cursorPositionIndex,
                    cursorPositionIndex + charactersRemovedCount,
                    0,
                    new(string.Empty),
                    string.Empty);

                var textModification = new TextEditorTextModification(false, textSpanForInsertion);

                foreach (var presentationModel in _presentationModelsBag)
                {
                    if (presentationModel.CompletedCalculation is not null)
                    {
                        presentationModel.CompletedCalculation.TextModificationsSinceRequestBag.Add(textModification);
                    }

                    if (presentationModel.PendingCalculation is not null)
                    {
                        presentationModel.PendingCalculation.TextModificationsSinceRequestBag.Add(textModification);
                    }
                }
            }
        }

        // TODO: Fix tracking the MostCharactersOnASingleRowTuple this way is possibly inefficient - should instead only check the rows that changed
        {
            (int rowIndex, int rowLength) localMostCharactersOnASingleRowTuple = (0, 0);

            for (var i = 0; i < _rowEndingPositionsBag.Count; i++)
            {
                var lengthOfRow = GetLengthOfRow(i);

                if (lengthOfRow > localMostCharactersOnASingleRowTuple.rowLength)
                {
                    localMostCharactersOnASingleRowTuple = (i, lengthOfRow);
                }
            }

            localMostCharactersOnASingleRowTuple = (localMostCharactersOnASingleRowTuple.rowIndex,
                localMostCharactersOnASingleRowTuple.rowLength + MOST_CHARACTERS_ON_A_SINGLE_ROW_MARGIN);

            MostCharactersOnASingleRowTuple = localMostCharactersOnASingleRowTuple;
        }
    }

    private void MutateRowEndingKindCount(RowEndingKind rowEndingKind, int changeBy)
    {
        var indexOfRowEndingKindCount = _rowEndingKindCountsBag.FindIndex(x => x.rowEndingKind == rowEndingKind);
        var currentRowEndingKindCount = _rowEndingKindCountsBag[indexOfRowEndingKindCount].count;

        _rowEndingKindCountsBag[indexOfRowEndingKindCount] = (rowEndingKind, currentRowEndingKindCount + changeBy);

        CheckRowEndingPositions(false);
    }

    private void CheckRowEndingPositions(bool setUsingRowEndingKind)
    {
        var existingRowEndingsBag = _rowEndingKindCountsBag
            .Where(x => x.count > 0)
            .ToArray();

        if (!existingRowEndingsBag.Any())
        {
            OnlyRowEndingKind = RowEndingKind.Unset;
            UsingRowEndingKind = RowEndingKind.Linefeed;
        }
        else
        {
            if (existingRowEndingsBag.Length == 1)
            {
                var rowEndingKind = existingRowEndingsBag.Single().rowEndingKind;

                if (setUsingRowEndingKind)
                    UsingRowEndingKind = rowEndingKind;

                OnlyRowEndingKind = rowEndingKind;
            }
            else
            {
                if (setUsingRowEndingKind)
                    UsingRowEndingKind = existingRowEndingsBag.MaxBy(x => x.count).rowEndingKind;

                OnlyRowEndingKind = null;
            }
        }
    }

    public void SetContent(string content)
    {
        ResetStateButNotEditHistory();

        var rowIndex = 0;
        var previousCharacter = '\0';

        var charactersOnRow = 0;

        var carriageReturnCount = 0;
        var linefeedCount = 0;
        var carriageReturnLinefeedCount = 0;

        for (var index = 0; index < content.Length; index++)
        {
            var character = content[index];

            charactersOnRow++;

            if (character == KeyboardKeyFacts.WhitespaceCharacters.CARRIAGE_RETURN)
            {
                if (charactersOnRow > MostCharactersOnASingleRowTuple.rowLength - MOST_CHARACTERS_ON_A_SINGLE_ROW_MARGIN)
                    MostCharactersOnASingleRowTuple = (rowIndex, charactersOnRow + MOST_CHARACTERS_ON_A_SINGLE_ROW_MARGIN);

                _rowEndingPositionsBag.Add((index + 1, RowEndingKind.CarriageReturn));
                rowIndex++;

                charactersOnRow = 0;

                carriageReturnCount++;
            }
            else if (character == KeyboardKeyFacts.WhitespaceCharacters.NEW_LINE)
            {
                if (charactersOnRow > MostCharactersOnASingleRowTuple.rowLength - MOST_CHARACTERS_ON_A_SINGLE_ROW_MARGIN)
                    MostCharactersOnASingleRowTuple = (rowIndex, charactersOnRow + MOST_CHARACTERS_ON_A_SINGLE_ROW_MARGIN);

                if (previousCharacter == KeyboardKeyFacts.WhitespaceCharacters.CARRIAGE_RETURN)
                {
                    var lineEnding = _rowEndingPositionsBag[rowIndex - 1];

                    _rowEndingPositionsBag[rowIndex - 1] = (lineEnding.positionIndex + 1, RowEndingKind.CarriageReturnLinefeed);

                    carriageReturnCount--;
                    carriageReturnLinefeedCount++;
                }
                else
                {
                    _rowEndingPositionsBag.Add((index + 1, RowEndingKind.Linefeed));
                    rowIndex++;

                    linefeedCount++;
                }

                charactersOnRow = 0;
            }

            if (character == KeyboardKeyFacts.WhitespaceCharacters.TAB)
                _tabKeyPositionsBag.Add(index);

            previousCharacter = character;

            _contentBag.Add(new RichCharacter
            {
                Value = character,
                DecorationByte = default,
            });
        }

        _rowEndingKindCountsBag.AddRange(new List<(RowEndingKind rowEndingKind, int count)>
        {
            (RowEndingKind.CarriageReturn, carriageReturnCount),
            (RowEndingKind.Linefeed, linefeedCount),
            (RowEndingKind.CarriageReturnLinefeed, carriageReturnLinefeedCount),
        });

        CheckRowEndingPositions(true);

        _rowEndingPositionsBag.Add((content.Length, RowEndingKind.EndOfFile));
    }

    private void ResetStateButNotEditHistory()
    {
        _contentBag.Clear();
        _rowEndingKindCountsBag.Clear();
        _rowEndingPositionsBag.Clear();
        _tabKeyPositionsBag.Clear();
        OnlyRowEndingKind = null;
        UsingRowEndingKind = RowEndingKind.Unset;
    }
}