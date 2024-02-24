﻿using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Partitions.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.Commands.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Edits.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.Options.Models;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;

/// <summary>
/// Any modified state needs to be 'null coallesce assigned' to the existing TextEditorModel's value
///
/// When reading state, if the state had been 'null coallesce assigned' then the field will
/// be read. Otherwise, the existing TextEditorModel's value will be read.
/// <br/><br/>
/// <inheritdoc cref="ITextEditorModel"/>
/// </summary>
public partial class TextEditorModelModifier
{
    private readonly TextEditorModel _textEditorModel;

    public TextEditorModelModifier(TextEditorModel textEditorModel)
    {
        _textEditorModel = textEditorModel;
    }

    private PartitionedImmutableList<RichCharacter>? _contentList;
    private List<EditBlock>? _editBlocksList;
    private List<RowEnding>? _rowEndingPositionsList;
    private List<(RowEndingKind rowEndingKind, int count)>? _rowEndingKindCountsList;
    private List<TextEditorPresentationModel>? _presentationModelsList;
    private List<int>? _tabKeyPositionsList;

    private RowEndingKind? _onlyRowEndingKind;
    /// <summary>
    /// Awkward special case here: <see cref="_onlyRowEndingKind"/> is allowed to be null.
    /// So, the design of this class where null means unmodified, doesn't work well here.
    /// </summary>
    private bool _onlyRowEndingKindWasModified;

    private RowEndingKind? _usingRowEndingKind;
    private ResourceUri? _resourceUri;
    private DateTime? _resourceLastWriteTime;
    private string? _fileExtension;
    private IDecorationMapper? _decorationMapper;
    private ILuthCompilerService? _compilerService;
    private TextEditorSaveFileHelper? _textEditorSaveFileHelper;
    private int? _editBlockIndex;
    private (int rowIndex, int rowLength)? _mostCharactersOnASingleRowTuple;
    private Key<RenderState>? _renderStateKey = Key<RenderState>.NewKey();
    private Keymap? _textEditorKeymap;
    private TextEditorOptions? _textEditorOptions;

    public bool WasModified { get; internal set; }

    public TextEditorModel ToModel()
    {
        return new TextEditorModel(
            _contentList is null ? _textEditorModel.ContentList : _contentList,
            _editBlocksList is null ? _textEditorModel.EditBlocksList : _editBlocksList.ToImmutableList(),
            _rowEndingPositionsList is null ? _textEditorModel.RowEndingPositionsList : _rowEndingPositionsList.ToImmutableList(),
            _rowEndingKindCountsList is null ? _textEditorModel.RowEndingKindCountsList : _rowEndingKindCountsList.ToImmutableList(),
            _presentationModelsList is null ? _textEditorModel.PresentationModelsList : _presentationModelsList.ToImmutableList(),
            _tabKeyPositionsList is null ? _textEditorModel.TabKeyPositionsList : _tabKeyPositionsList.ToImmutableList(),
            _onlyRowEndingKindWasModified ? _onlyRowEndingKind : _textEditorModel.OnlyRowEndingKind,
            _usingRowEndingKind ?? _textEditorModel.UsingRowEndingKind,
            _resourceUri ?? _textEditorModel.ResourceUri,
            _resourceLastWriteTime ?? _textEditorModel.ResourceLastWriteTime,
            _fileExtension ?? _textEditorModel.FileExtension,
            _decorationMapper ?? _textEditorModel.DecorationMapper,
            _compilerService ?? _textEditorModel.CompilerService,
            _textEditorSaveFileHelper ?? _textEditorModel.TextEditorSaveFileHelper,
            _editBlockIndex ?? _textEditorModel.EditBlockIndex,
            _mostCharactersOnASingleRowTuple ?? _textEditorModel.MostCharactersOnASingleRowTuple,
            _renderStateKey ?? _textEditorModel.RenderStateKey);
    }

    public void ClearContentList()
    {
        _contentList = TextEditorModel.PARTITION_EMPTY;
    }

    public void ClearRowEndingPositionsList()
    {
        _rowEndingPositionsList = new();
    }

    public void ClearRowEndingKindCountsList()
    {
        _rowEndingKindCountsList = new();
    }

    public void ClearTabKeyPositionsList()
    {
        _tabKeyPositionsList = new();
    }

    public void ClearOnlyRowEndingKind()
    {
        _onlyRowEndingKind = null;
        _onlyRowEndingKindWasModified = true;
    }

    public void ModifyUsingRowEndingKind(RowEndingKind rowEndingKind)
    {
        _usingRowEndingKind = rowEndingKind;
    }

    public void ModifyResourceData(ResourceUri resourceUri, DateTime resourceLastWriteTime)
    {
        _resourceUri = resourceUri;
        _resourceLastWriteTime = resourceLastWriteTime;
    }

    public void ModifyDecorationMapper(IDecorationMapper decorationMapper)
    {
        _decorationMapper = decorationMapper;
    }

    public void ModifyCompilerService(ILuthCompilerService compilerService)
    {
        _compilerService = compilerService;
    }

    public void ModifyTextEditorSaveFileHelper(TextEditorSaveFileHelper textEditorSaveFileHelper)
    {
        _textEditorSaveFileHelper = textEditorSaveFileHelper;
    }

    private void EnsureUndoPoint(TextEditKind textEditKind, string? otherTextEditKindIdentifier = null)
    {
        // Any modified state needs to be 'null coallesce assigned' to the existing TextEditorModel's value. When reading state, if the state had been 'null coallesce assigned' then the field will be read. Otherwise, the existing TextEditorModel's value will be read.
        {
            _editBlocksList ??= _textEditorModel.EditBlocksList.ToList();
            _editBlockIndex ??= _textEditorModel.EditBlockIndex;
        }

        if (textEditKind == TextEditKind.Other && otherTextEditKindIdentifier is null)
            TextEditorCommand.ThrowOtherTextEditKindIdentifierWasExpectedException(textEditKind);

        var mostRecentEditBlock = EditBlocksList.LastOrDefault();

        if (mostRecentEditBlock is null || mostRecentEditBlock.TextEditKind != textEditKind)
        {
            var newEditBlockIndex = EditBlockIndex;

            EditBlocksList.Insert(newEditBlockIndex, new EditBlock(
                textEditKind,
                textEditKind.ToString(),
                this.GetAllText(),
                otherTextEditKindIdentifier));

            var removeBlocksStartingAt = newEditBlockIndex + 1;

            _editBlocksList.RemoveRange(removeBlocksStartingAt, EditBlocksList.Count - removeBlocksStartingAt);

            _editBlockIndex++;
        }

        while (EditBlocksList.Count > TextEditorModel.MAXIMUM_EDIT_BLOCKS && EditBlocksList.Count != 0)
        {
            _editBlockIndex--;
            EditBlocksList.RemoveAt(0);
        }
    }

    private void PerformInsertions(
        KeyboardEventArgs keyboardEventArgs,
        TextEditorCursorModifierBag cursorModifierBag,
        CancellationToken cancellationToken)
    {
        // Any modified state needs to be 'null coallesce assigned' to the existing TextEditorModel's value. When reading state, if the state had been 'null coallesce assigned' then the field will be read. Otherwise, the existing TextEditorModel's value will be read.
        {
            _contentList ??= _textEditorModel.ContentList;
            _rowEndingPositionsList ??= _textEditorModel.RowEndingPositionsList.ToList();
            _tabKeyPositionsList ??= _textEditorModel.TabKeyPositionsList.ToList();
            _mostCharactersOnASingleRowTuple ??= _textEditorModel.MostCharactersOnASingleRowTuple;
        }

        EnsureUndoPoint(TextEditKind.Insertion);

        foreach (var cursorModifier in cursorModifierBag.List)
        {
            if (TextEditorSelectionHelper.HasSelectedText(cursorModifier))
            {
                var selectionBounds = TextEditorSelectionHelper.GetSelectionBounds(cursorModifier);

                var lowerRowData = this.GetRowInformationFromPositionIndex(selectionBounds.lowerPositionIndexInclusive);
                var lowerColumnIndex = selectionBounds.lowerPositionIndexInclusive - lowerRowData.RowStartPositionIndexInclusive;

                PerformDeletions(
                    new KeyboardEventArgs
                    {
                        Code = KeyboardKeyFacts.MetaKeys.DELETE,
                        Key = KeyboardKeyFacts.MetaKeys.DELETE,
                    },
                    cursorModifierBag,
                    CancellationToken.None);

                // Move cursor to lower bound of text selection
                cursorModifier.RowIndex = lowerRowData.RowIndex;
                cursorModifier.SetColumnIndexAndPreferred(lowerColumnIndex);
            }

            var startOfRowPositionIndex = this.GetRowEndingThatCreatedRow(cursorModifier.RowIndex).EndPositionIndexExclusive;
            var cursorPositionIndex = startOfRowPositionIndex + cursorModifier.ColumnIndex;

            // If cursor is out of bounds then continue
            if (cursorPositionIndex > ContentList.Count)
                continue;

            var wasTabCode = false;
            var wasEnterCode = false;

            var characterValueToInsert = keyboardEventArgs.Key.First();

            if (KeyboardKeyFacts.IsWhitespaceCode(keyboardEventArgs.Code))
            {
                characterValueToInsert = KeyboardKeyFacts.ConvertWhitespaceCodeToCharacter(keyboardEventArgs.Code);

                wasTabCode = KeyboardKeyFacts.WhitespaceCodes.TAB_CODE == keyboardEventArgs.Code;
                wasEnterCode = KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE == keyboardEventArgs.Code;
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

                _contentList = _contentList.InsertRange(cursorPositionIndex, richCharacters);

                RowEndingPositionsList.Insert(
                    cursorModifier.RowIndex,
                    new (cursorPositionIndex, cursorPositionIndex + characterCountInserted, rowEndingKindToInsert));

                MutateRowEndingKindCount(rowEndingKindToInsert, 1);

                cursorModifier.RowIndex++;
                cursorModifier.ColumnIndex = 0;
                cursorModifier.PreferredColumnIndex = cursorModifier.ColumnIndex;
            }
            else
            {
                if (wasTabCode)
                {
                    var index = _tabKeyPositionsList.FindIndex(x => x >= cursorPositionIndex);

                    if (index == -1)
                    {
                        TabKeyPositionsList.Add(cursorPositionIndex);
                    }
                    else
                    {
                        for (var i = index; i < TabKeyPositionsList.Count; i++)
                        {
                            _tabKeyPositionsList[i]++;
                        }

                        TabKeyPositionsList.Insert(index, cursorPositionIndex);
                    }
                }

                var richCharacterToInsert = new RichCharacter
                {
                    Value = characterValueToInsert,
                    DecorationByte = default,
                };

                _contentList = ContentList.Insert(cursorPositionIndex, richCharacterToInsert);

                cursorModifier.ColumnIndex++;
                cursorModifier.PreferredColumnIndex = cursorModifier.ColumnIndex;
            }

            // Reposition the Row Endings
            {
                for (var i = cursorModifier.RowIndex; i < RowEndingPositionsList.Count; i++)
                {
                    var rowEndingTuple = RowEndingPositionsList[i];
                    rowEndingTuple.StartPositionIndexInclusive += characterCountInserted;
                    rowEndingTuple.EndPositionIndexExclusive += characterCountInserted;
                }
            }

            if (!wasTabCode)
            {
                var firstTabKeyPositionIndexToModify = _tabKeyPositionsList.FindIndex(x => x >= cursorPositionIndex);

                if (firstTabKeyPositionIndexToModify != -1)
                {
                    for (var i = firstTabKeyPositionIndexToModify; i < TabKeyPositionsList.Count; i++)
                    {
                        TabKeyPositionsList[i] += characterCountInserted;
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

                foreach (var presentationModel in PresentationModelsList)
                {
                    presentationModel.CompletedCalculation?.TextModificationsSinceRequestList.Add(textModification);
                    presentationModel.PendingCalculation?.TextModificationsSinceRequestList.Add(textModification);
                }
            }
        }

        // TODO: Fix tracking the MostCharactersOnASingleRowTuple this way is possibly inefficient - should instead only check the rows that changed
        {
            (int rowIndex, int rowLength) localMostCharactersOnASingleRowTuple = (0, 0);

            for (var i = 0; i < RowEndingPositionsList.Count; i++)
            {
                var lengthOfRow = this.GetLengthOfRow(i);

                if (lengthOfRow > localMostCharactersOnASingleRowTuple.rowLength)
                {
                    localMostCharactersOnASingleRowTuple = (i, lengthOfRow);
                }
            }

            localMostCharactersOnASingleRowTuple = (localMostCharactersOnASingleRowTuple.rowIndex,
                localMostCharactersOnASingleRowTuple.rowLength + TextEditorModel.MOST_CHARACTERS_ON_A_SINGLE_ROW_MARGIN);

            _mostCharactersOnASingleRowTuple = localMostCharactersOnASingleRowTuple;
        }
    }

    private void PerformDeletions(
        KeyboardEventArgs keyboardEventArgs,
        TextEditorCursorModifierBag cursorModifierBag,
        CancellationToken cancellationToken)
    {
        // Any modified state needs to be 'null coallesce assigned' to the existing TextEditorModel's value. When reading state, if the state had been 'null coallesce assigned' then the field will be read. Otherwise, the existing TextEditorModel's value will be read.
        {
            _rowEndingPositionsList ??= _textEditorModel.RowEndingPositionsList.ToList();
            _tabKeyPositionsList ??= _textEditorModel.TabKeyPositionsList.ToList();
            _contentList ??= _textEditorModel.ContentList;
            _mostCharactersOnASingleRowTuple ??= _textEditorModel.MostCharactersOnASingleRowTuple;
        }

        EnsureUndoPoint(TextEditKind.Deletion);

        foreach (var cursorModifier in cursorModifierBag.List)
        {
            var startOfRowPositionIndex = this.GetRowEndingThatCreatedRow(cursorModifier.RowIndex).EndPositionIndexExclusive;
            var cursorPositionIndex = startOfRowPositionIndex + cursorModifier.ColumnIndex;

            // If cursor is out of bounds then continue
            if (cursorPositionIndex > ContentList.Count)
                continue;

            int startingPositionIndexToRemoveInclusive;
            int countToRemove;
            bool moveBackwards;

            // Cannot calculate this after text was deleted - it would be wrong
            int? selectionUpperBoundRowIndex = null;
            // Needed for when text selection is deleted
            (int rowIndex, int columnIndex)? selectionLowerBoundIndexCoordinates = null;

            // TODO: The deletion logic should be the same whether it be 'Delete' 'Backspace' 'CtrlModified' or 'DeleteSelection'. What should change is one needs to calculate the starting and ending index appropriately foreach case.
            if (TextEditorSelectionHelper.HasSelectedText(cursorModifier))
            {
                var lowerPositionIndexInclusiveBound = cursorModifier.SelectionAnchorPositionIndex ?? 0;
                var upperPositionIndexExclusive = cursorModifier.SelectionEndingPositionIndex;

                if (lowerPositionIndexInclusiveBound > upperPositionIndexExclusive)
                    (lowerPositionIndexInclusiveBound, upperPositionIndexExclusive) = (upperPositionIndexExclusive, lowerPositionIndexInclusiveBound);

                var lowerRowMetaData = this.GetRowInformationFromPositionIndex(lowerPositionIndexInclusiveBound);
                var upperRowMetaData = this.GetRowInformationFromPositionIndex(upperPositionIndexExclusive);

                // Value is needed when knowing what row ending positions to update after deletion is done
                selectionUpperBoundRowIndex = upperRowMetaData.RowIndex;

                // Value is needed when knowing where to position the cursor after deletion is done
                selectionLowerBoundIndexCoordinates = (lowerRowMetaData.RowIndex,
                    lowerPositionIndexInclusiveBound - lowerRowMetaData.RowStartPositionIndexInclusive);

                startingPositionIndexToRemoveInclusive = upperPositionIndexExclusive - 1;
                countToRemove = upperPositionIndexExclusive - lowerPositionIndexInclusiveBound;
                moveBackwards = true;

                cursorModifier.SelectionAnchorPositionIndex = null;
            }
            else if (KeyboardKeyFacts.MetaKeys.BACKSPACE == keyboardEventArgs.Key)
            {
                moveBackwards = true;

                if (keyboardEventArgs.CtrlKey)
                {
                    var columnIndexOfCharacterWithDifferingKind = this.GetColumnIndexOfCharacterWithDifferingKind(
                        cursorModifier.RowIndex,
                        cursorModifier.ColumnIndex,
                        moveBackwards);

                    columnIndexOfCharacterWithDifferingKind = columnIndexOfCharacterWithDifferingKind == -1
                        ? 0
                        : columnIndexOfCharacterWithDifferingKind;

                    countToRemove = cursorModifier.ColumnIndex -
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
            else if (KeyboardKeyFacts.MetaKeys.DELETE == keyboardEventArgs.Key)
            {
                moveBackwards = false;

                if (keyboardEventArgs.CtrlKey)
                {
                    var columnIndexOfCharacterWithDifferingKind = this.GetColumnIndexOfCharacterWithDifferingKind(
                        cursorModifier.RowIndex,
                        cursorModifier.ColumnIndex,
                        moveBackwards);

                    columnIndexOfCharacterWithDifferingKind = columnIndexOfCharacterWithDifferingKind == -1
                        ? this.GetLengthOfRow(cursorModifier.RowIndex)
                        : columnIndexOfCharacterWithDifferingKind;

                    countToRemove = columnIndexOfCharacterWithDifferingKind -
                        cursorModifier.ColumnIndex;

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
                throw new ApplicationException($"The keyboard key: {keyboardEventArgs.Key} was not recognized");
            }

            var charactersRemovedCount = 0;
            var rowsRemovedCount = 0;

            var indexToRemove = startingPositionIndexToRemoveInclusive;

            while (countToRemove-- > 0)
            {
                if (indexToRemove < 0 || indexToRemove > ContentList.Count - 1)
                    break;

                var characterToDelete = ContentList[indexToRemove];

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
                    var rowEndingTupleIndex = _rowEndingPositionsList.FindIndex(rep =>
                        rep.EndPositionIndexExclusive == indexToRemove + 1 ||
                        rep.EndPositionIndexExclusive == indexToRemove + 2);

                    var rowEndingTuple = RowEndingPositionsList[rowEndingTupleIndex];

                    RowEndingPositionsList.RemoveAt(rowEndingTupleIndex);

                    var lengthOfRowEnding = rowEndingTuple.RowEndingKind.AsCharacters().Length;

                    if (moveBackwards)
                        startingIndexToRemoveRange = indexToRemove - (lengthOfRowEnding - 1);
                    else
                        startingIndexToRemoveRange = indexToRemove;

                    countToRemove -= lengthOfRowEnding - 1;
                    countToRemoveRange = lengthOfRowEnding;

                    MutateRowEndingKindCount(rowEndingTuple.RowEndingKind, -1);
                }
                else
                {
                    if (characterToDelete.Value == KeyboardKeyFacts.WhitespaceCharacters.TAB)
                        TabKeyPositionsList.Remove(indexToRemove);

                    startingIndexToRemoveRange = indexToRemove;
                    countToRemoveRange = 1;
                }

                charactersRemovedCount += countToRemoveRange;

                _contentList = _contentList.RemoveRange(startingIndexToRemoveRange, countToRemoveRange);

                if (moveBackwards)
                    indexToRemove -= countToRemoveRange;
            }

            if (charactersRemovedCount == 0 && rowsRemovedCount == 0)
                return;

            if (moveBackwards && !selectionUpperBoundRowIndex.HasValue)
            {
                var startOfCurrentRowPositionIndex = this
                    .GetRowEndingThatCreatedRow(cursorModifier.RowIndex - rowsRemovedCount)
                    .EndPositionIndexExclusive;

                var endingPositionIndex = cursorPositionIndex - charactersRemovedCount;
                
                cursorModifier.RowIndex -= rowsRemovedCount;
                cursorModifier.SetColumnIndexAndPreferred(endingPositionIndex - startOfCurrentRowPositionIndex);
            }

            int firstRowIndexToModify;

            if (selectionUpperBoundRowIndex.HasValue)
            {
                firstRowIndexToModify = selectionLowerBoundIndexCoordinates!.Value.rowIndex;
                cursorModifier.RowIndex = selectionLowerBoundIndexCoordinates!.Value.rowIndex;
                cursorModifier.SetColumnIndexAndPreferred(selectionLowerBoundIndexCoordinates!.Value.columnIndex);
            }
            else
            {
                firstRowIndexToModify = cursorModifier.RowIndex;
            }

            for (var i = firstRowIndexToModify; i < RowEndingPositionsList.Count; i++)
            {
                var rowEndingTuple = RowEndingPositionsList[i];
                rowEndingTuple.StartPositionIndexInclusive -= charactersRemovedCount;
                rowEndingTuple.EndPositionIndexExclusive -= charactersRemovedCount;
            }

            var firstTabKeyPositionIndexToModify = _tabKeyPositionsList.FindIndex(x => x >= startingPositionIndexToRemoveInclusive);

            if (firstTabKeyPositionIndexToModify != -1)
            {
                for (var i = firstTabKeyPositionIndexToModify; i < TabKeyPositionsList.Count; i++)
                {
                    TabKeyPositionsList[i] -= charactersRemovedCount;
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

                foreach (var presentationModel in PresentationModelsList)
                {
                    presentationModel.CompletedCalculation?.TextModificationsSinceRequestList.Add(textModification);
                    presentationModel.PendingCalculation?.TextModificationsSinceRequestList.Add(textModification);
                }
            }
        }

        // TODO: Fix tracking the MostCharactersOnASingleRowTuple this way is possibly inefficient - should instead only check the rows that changed
        {
            (int rowIndex, int rowLength) localMostCharactersOnASingleRowTuple = (0, 0);

            for (var i = 0; i < RowEndingPositionsList.Count; i++)
            {
                var lengthOfRow = this.GetLengthOfRow(i);

                if (lengthOfRow > localMostCharactersOnASingleRowTuple.rowLength)
                {
                    localMostCharactersOnASingleRowTuple = (i, lengthOfRow);
                }
            }

            localMostCharactersOnASingleRowTuple = (localMostCharactersOnASingleRowTuple.rowIndex,
                localMostCharactersOnASingleRowTuple.rowLength + TextEditorModel.MOST_CHARACTERS_ON_A_SINGLE_ROW_MARGIN);

            _mostCharactersOnASingleRowTuple = localMostCharactersOnASingleRowTuple;
        }
    }

    private void MutateRowEndingKindCount(RowEndingKind rowEndingKind, int changeBy)
    {
        // Any modified state needs to be 'null coallesce assigned' to the existing TextEditorModel's value. When reading state, if the state had been 'null coallesce assigned' then the field will be read. Otherwise, the existing TextEditorModel's value will be read.
        {
            _rowEndingKindCountsList ??= _textEditorModel.RowEndingKindCountsList.ToList();
        }

        var indexOfRowEndingKindCount = _rowEndingKindCountsList.FindIndex(x => x.rowEndingKind == rowEndingKind);
        var currentRowEndingKindCount = RowEndingKindCountsList[indexOfRowEndingKindCount].count;

        RowEndingKindCountsList[indexOfRowEndingKindCount] = (rowEndingKind, currentRowEndingKindCount + changeBy);

        CheckRowEndingPositions(false);
    }

    private void CheckRowEndingPositions(bool setUsingRowEndingKind)
    {
        // Any modified state needs to be 'null coallesce assigned' to the existing TextEditorModel's value. When reading state, if the state had been 'null coallesce assigned' then the field will be read. Otherwise, the existing TextEditorModel's value will be read.
        {
            _rowEndingKindCountsList ??= _textEditorModel.RowEndingKindCountsList.ToList();
            _onlyRowEndingKind ??= _textEditorModel.OnlyRowEndingKind;
            _usingRowEndingKind ??= _textEditorModel.UsingRowEndingKind;
        }

        var existingRowEndingsList = RowEndingKindCountsList
            .Where(x => x.count > 0)
            .ToArray();

        if (!existingRowEndingsList.Any())
        {
            _onlyRowEndingKind = RowEndingKind.Unset;
            _usingRowEndingKind = RowEndingKind.Linefeed;
        }
        else
        {
            if (existingRowEndingsList.Length == 1)
            {
                var rowEndingKind = existingRowEndingsList.Single().rowEndingKind;

                if (setUsingRowEndingKind)
                    _usingRowEndingKind = rowEndingKind;

                _onlyRowEndingKind = rowEndingKind;
            }
            else
            {
                if (setUsingRowEndingKind)
                    _usingRowEndingKind = existingRowEndingsList.MaxBy(x => x.count).rowEndingKind;

                _onlyRowEndingKind = null;
            }
        }
    }

    public void ModifyContent(string content)
    {
        // Any modified state needs to be 'null coallesce assigned' to the existing TextEditorModel's value. When reading state, if the state had been 'null coallesce assigned' then the field will be read. Otherwise, the existing TextEditorModel's value will be read.
        {
            _mostCharactersOnASingleRowTuple ??= _textEditorModel.MostCharactersOnASingleRowTuple;
            _rowEndingPositionsList ??= _textEditorModel.RowEndingPositionsList.ToList();
            _tabKeyPositionsList ??= _textEditorModel.TabKeyPositionsList.ToList();
            _contentList ??= _textEditorModel.ContentList;
            _rowEndingKindCountsList ??= _textEditorModel.RowEndingKindCountsList.ToList();
        }

        ModifyResetStateButNotEditHistory();

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
                if (charactersOnRow > MostCharactersOnASingleRowTuple.rowLength - TextEditorModel.MOST_CHARACTERS_ON_A_SINGLE_ROW_MARGIN)
                    _mostCharactersOnASingleRowTuple = (rowIndex, charactersOnRow + TextEditorModel.MOST_CHARACTERS_ON_A_SINGLE_ROW_MARGIN);

                RowEndingPositionsList.Add(new(index, index + 1, RowEndingKind.CarriageReturn));
                rowIndex++;

                charactersOnRow = 0;

                carriageReturnCount++;
            }
            else if (character == KeyboardKeyFacts.WhitespaceCharacters.NEW_LINE)
            {
                if (charactersOnRow > MostCharactersOnASingleRowTuple.rowLength - TextEditorModel.MOST_CHARACTERS_ON_A_SINGLE_ROW_MARGIN)
                    _mostCharactersOnASingleRowTuple = (rowIndex, charactersOnRow + TextEditorModel.MOST_CHARACTERS_ON_A_SINGLE_ROW_MARGIN);

                if (previousCharacter == KeyboardKeyFacts.WhitespaceCharacters.CARRIAGE_RETURN)
                {
                    var lineEnding = RowEndingPositionsList[rowIndex - 1];
                    lineEnding.EndPositionIndexExclusive++;
                    lineEnding.RowEndingKind = RowEndingKind.CarriageReturnLinefeed;

                    carriageReturnCount--;
                    carriageReturnLinefeedCount++;
                }
                else
                {
                    RowEndingPositionsList.Add(new (index, index + 1, RowEndingKind.Linefeed));
                    rowIndex++;

                    linefeedCount++;
                }

                charactersOnRow = 0;
            }

            if (character == KeyboardKeyFacts.WhitespaceCharacters.TAB)
                TabKeyPositionsList.Add(index);

            previousCharacter = character;

            _contentList = ContentList.Add(new RichCharacter
            {
                Value = character,
                DecorationByte = default,
            });
        }

        _rowEndingKindCountsList.AddRange(new List<(RowEndingKind rowEndingKind, int count)>
        {
            (RowEndingKind.CarriageReturn, carriageReturnCount),
            (RowEndingKind.Linefeed, linefeedCount),
            (RowEndingKind.CarriageReturnLinefeed, carriageReturnLinefeedCount),
        });

        CheckRowEndingPositions(true);

        RowEndingPositionsList.Add(new(content.Length, content.Length, RowEndingKind.EndOfFile));
    }

    public void ModifyResetStateButNotEditHistory()
    {
        ClearContentList();
        ClearRowEndingKindCountsList();
        ClearRowEndingPositionsList();
        ClearTabKeyPositionsList();
        ClearOnlyRowEndingKind();
        ModifyUsingRowEndingKind(RowEndingKind.Unset);
    }

    public void HandleKeyboardEvent(
        KeyboardEventArgs keyboardEventArgs,
        TextEditorCursorModifierBag cursorModifierBag,
        CancellationToken cancellationToken)
    {
        if (KeyboardKeyFacts.IsMetaKey(keyboardEventArgs))
        {
            if (KeyboardKeyFacts.MetaKeys.BACKSPACE == keyboardEventArgs.Key ||
                KeyboardKeyFacts.MetaKeys.DELETE == keyboardEventArgs.Key)
            {
                PerformDeletions(
                    keyboardEventArgs,
                    cursorModifierBag,
                    cancellationToken);
            }
        }
        else
        {
            for (int i = cursorModifierBag.List.Count - 1; i >= 0; i--)
            {
                var cursor = cursorModifierBag.List[i];

                var singledCursorModifierBag = new TextEditorCursorModifierBag(
                    cursorModifierBag.ViewModelKey,
                    new List<TextEditorCursorModifier> { cursor });

                PerformInsertions(
                    keyboardEventArgs,
                    singledCursorModifierBag,
                    cancellationToken);
            }
        }
    }

    public void EditByInsertion(
        string content,
        TextEditorCursorModifierBag cursorModifierBag,
        CancellationToken cancellationToken)
    {
        var localContent = content.Replace("\r\n", "\n");

        for (int i = cursorModifierBag.List.Count - 1; i >= 0; i--)
        {
            var cursor = cursorModifierBag.List[i];

            var singledCursorModifierBag = new TextEditorCursorModifierBag(
                cursorModifierBag.ViewModelKey,
                new List<TextEditorCursorModifier> { cursor });

            foreach (var character in localContent)
            {
                // TODO: This needs to be rewritten everything should be inserted at the same time not a foreach loop insertion for each character
                var code = character switch
                {
                    '\r' => KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE,
                    '\n' => KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE,
                    '\t' => KeyboardKeyFacts.WhitespaceCodes.TAB_CODE,
                    ' ' => KeyboardKeyFacts.WhitespaceCodes.SPACE_CODE,
                    _ => character.ToString(),
                };

                HandleKeyboardEvent(
                    new KeyboardEventArgs
                    {
                        Code = code,
                        Key = character.ToString(),
                    },
                    singledCursorModifierBag,
                    CancellationToken.None);
            }
        }
    }

    public void DeleteTextByMotion(
        MotionKind motionKind,
        TextEditorCursorModifierBag cursorModifierBag,
        CancellationToken cancellationToken)
    {
        var keyboardEventArgs = motionKind switch
        {
            MotionKind.Backspace => new KeyboardEventArgs { Key = KeyboardKeyFacts.MetaKeys.BACKSPACE },
            MotionKind.Delete => new KeyboardEventArgs { Key = KeyboardKeyFacts.MetaKeys.DELETE },
            _ => throw new ApplicationException($"The {nameof(MotionKind)}: {motionKind} was not recognized.")
        };
        
        HandleKeyboardEvent(
            keyboardEventArgs,
            cursorModifierBag,
            CancellationToken.None);
    }

    public void DeleteByRange(
        int count,
        TextEditorCursorModifierBag cursorModifierBag,
        CancellationToken cancellationToken)
    {
        for (int cursorIndex = cursorModifierBag.List.Count - 1; cursorIndex >= 0; cursorIndex--)
        {
            var cursor = cursorModifierBag.List[cursorIndex];

            var singledCursorModifierBag = new TextEditorCursorModifierBag(
                cursorModifierBag.ViewModelKey,
                new List<TextEditorCursorModifier> { cursor });

            // TODO: This needs to be rewritten everything should be deleted at the same time not a foreach loop for each character
            for (var deleteIndex = 0; deleteIndex < count; deleteIndex++)
            {
                HandleKeyboardEvent(
                    new KeyboardEventArgs
                    {
                        Code = KeyboardKeyFacts.MetaKeys.DELETE,
                        Key = KeyboardKeyFacts.MetaKeys.DELETE,
                    },
                    singledCursorModifierBag,
                    CancellationToken.None);
            }
        }
    }

    public void PerformRegisterPresentationModelAction(
        TextEditorPresentationModel presentationModel)
    {
        // Any modified state needs to be 'null coallesce assigned' to the existing TextEditorModel's value. When reading state, if the state had been 'null coallesce assigned' then the field will be read. Otherwise, the existing TextEditorModel's value will be read.
        {
            _presentationModelsList ??= _textEditorModel.PresentationModelsList.ToList();
        }

        if (!PresentationModelsList.Any(x => x.TextEditorPresentationKey == presentationModel.TextEditorPresentationKey))
            PresentationModelsList.Add(presentationModel);
    }

    public void StartPendingCalculatePresentationModel(
        Key<TextEditorPresentationModel> presentationKey,
        TextEditorPresentationModel emptyPresentationModel)
    {
        // Any modified state needs to be 'null coallesce assigned' to the existing TextEditorModel's value. When reading state, if the state had been 'null coallesce assigned' then the field will be read. Otherwise, the existing TextEditorModel's value will be read.
        {
            _presentationModelsList ??= _textEditorModel.PresentationModelsList.ToList();
        }

        // If the presentation model has not yet been registered, then this will register it.
        PerformRegisterPresentationModelAction(emptyPresentationModel);

        var indexOfPresentationModel = _presentationModelsList.FindIndex(
            x => x.TextEditorPresentationKey == presentationKey);

        // The presentation model is expected to always be registered at this point.

        var presentationModel = PresentationModelsList[indexOfPresentationModel];
        PresentationModelsList[indexOfPresentationModel] = presentationModel with
        {
            PendingCalculation = new(this.GetAllText())
        };
    }
    
    public void CompletePendingCalculatePresentationModel(
        Key<TextEditorPresentationModel> presentationKey,
        TextEditorPresentationModel emptyPresentationModel,
        ImmutableArray<TextEditorTextSpan> calculatedTextSpans)
    {
        // Any modified state needs to be 'null coallesce assigned' to the existing TextEditorModel's value. When reading state, if the state had been 'null coallesce assigned' then the field will be read. Otherwise, the existing TextEditorModel's value will be read.
        {
            _presentationModelsList ??= _textEditorModel.PresentationModelsList.ToList();
        }

        // If the presentation model has not yet been registered, then this will register it.
        PerformRegisterPresentationModelAction(emptyPresentationModel);

        var indexOfPresentationModel = _presentationModelsList.FindIndex(
            x => x.TextEditorPresentationKey == presentationKey);

        // The presentation model is expected to always be registered at this point.

        var presentationModel = PresentationModelsList[indexOfPresentationModel];

        if (presentationModel.PendingCalculation is null)
            return;

        var calculation = presentationModel.PendingCalculation with
        {
            TextSpanList = calculatedTextSpans
        };

        PresentationModelsList[indexOfPresentationModel] = presentationModel with
        {
            PendingCalculation = null,
            CompletedCalculation = calculation,
        };
    }

    public void ClearEditBlocks()
    {
        // Any modified state needs to be 'null coallesce assigned' to the existing TextEditorModel's value. When reading state, if the state had been 'null coallesce assigned' then the field will be read. Otherwise, the existing TextEditorModel's value will be read.
        {
            _editBlocksList ??= _textEditorModel.EditBlocksList.ToList();
            _editBlockIndex ??= _textEditorModel.EditBlockIndex;
        }

        _editBlockIndex = 0;
        EditBlocksList.Clear();
    }

    /// <summary>The "if (EditBlockIndex == _editBlocksPersisted.Count)"<br/><br/>Is done because the active EditBlock is not yet persisted.<br/><br/>The active EditBlock is instead being 'created' as the user continues to make edits of the same <see cref="TextEditKind"/><br/><br/>For complete clarity, this comment refers to one possibly expecting to see "if (EditBlockIndex == _editBlocksPersisted.Count - 1)"</summary>
    public void UndoEdit()
    {
        // Any modified state needs to be 'null coallesce assigned' to the existing TextEditorModel's value. When reading state, if the state had been 'null coallesce assigned' then the field will be read. Otherwise, the existing TextEditorModel's value will be read.
        {
            _editBlocksList ??= _textEditorModel.EditBlocksList.ToList();
            _editBlockIndex ??= _textEditorModel.EditBlockIndex;
        }

        if (!this.CanUndoEdit())
            return;

        if (EditBlockIndex == EditBlocksList.Count)
        {
            // If the edit block is pending then persist it before
            // reverting back to the previous persisted edit block.
            EnsureUndoPoint(TextEditKind.ForcePersistEditBlock);
            _editBlockIndex--;
        }

        _editBlockIndex--;

        var restoreEditBlock = EditBlocksList[EditBlockIndex];

        ModifyContent(restoreEditBlock.ContentSnapshot);
    }

    public void RedoEdit()
    {
        // Any modified state needs to be 'null coallesce assigned' to the existing TextEditorModel's value. When reading state, if the state had been 'null coallesce assigned' then the field will be read. Otherwise, the existing TextEditorModel's value will be read.
        {
            _editBlocksList ??= _textEditorModel.EditBlocksList.ToList();
            _editBlockIndex ??= _textEditorModel.EditBlockIndex;
        }
        
        if (!this.CanRedoEdit())
            return;

        _editBlockIndex++;

        var restoreEditBlock = EditBlocksList[EditBlockIndex];

        ModifyContent(restoreEditBlock.ContentSnapshot);
    }

    public TextEditorModel ForceRerenderAction()
    {
        return ToModel();
    }
}