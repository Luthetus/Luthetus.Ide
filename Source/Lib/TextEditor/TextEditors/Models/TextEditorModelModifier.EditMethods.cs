using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.Commands.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Edits.Models;
using Luthetus.TextEditor.RazorLib.Exceptions;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.Options.Models;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Immutable;
using System.Text;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

/// <summary>
/// Any modified state needs to be 'null coallesce assigned' to the existing TextEditorModel's value
///
/// When reading state, if the state had been 'null coallesce assigned' then the field will
/// be read. Otherwise, the existing TextEditorModel's value will be read.
/// <br/><br/>
/// <inheritdoc cref="ITextEditorModel"/>
/// </summary>
public partial class TextEditorModelModifier : ITextEditorModel
{
    public void ClearContent()
    {
        // Any modified state needs to be 'null coallesce assigned' to the existing TextEditorModel's value. When reading state, if the state had been 'null coallesce assigned' then the field will be read. Otherwise, the existing TextEditorModel's value will be read.
        {
            _mostCharactersOnASingleLineTuple ??= _textEditorModel.MostCharactersOnASingleLineTuple;
            _lineEndList ??= _textEditorModel.LineEndList.ToList();
            _tabKeyPositionsList ??= _textEditorModel.TabKeyPositionList.ToList();
            _lineEndKindCountList ??= _textEditorModel.LineEndKindCountList.ToList();
        }

        _mostCharactersOnASingleLineTuple = (0, TextEditorModel.MOST_CHARACTERS_ON_A_SINGLE_ROW_MARGIN);

        _partitionList = new TextEditorPartition[] { new(Array.Empty<RichCharacter>().ToImmutableList()) }.ToImmutableList();

        _lineEndList = new List<LineEnd> 
        {
            new LineEnd(0, 0, LineEndKind.EndOfFile)
        };

        _lineEndKindCountList = new List<(LineEndKind rowEndingKind, int count)>
        {
            (LineEndKind.CarriageReturn, 0),
            (LineEndKind.LineFeed, 0),
            (LineEndKind.CarriageReturnLineFeed, 0),
        };

        _tabKeyPositionsList = new List<int>();

        SetIsDirtyTrue();
    }

    public void ClearOnlyRowEndingKind()
    {
        _onlyLineEndKind = null;
        _onlyLineEndKindWasModified = true;
    }

    public void SetLineEndKindPreference(LineEndKind rowEndingKind)
    {
        _usingLineEndKind = rowEndingKind;
    }

    public void SetResourceData(ResourceUri resourceUri, DateTime resourceLastWriteTime)
    {
        _resourceUri = resourceUri;
        _resourceLastWriteTime = resourceLastWriteTime;
    }

    public void SetDecorationMapper(IDecorationMapper decorationMapper)
    {
        _decorationMapper = decorationMapper;
    }

    public void SetCompilerService(ILuthCompilerService compilerService)
    {
        _compilerService = compilerService;
    }

    public void SetTextEditorSaveFileHelper(SaveFileHelper textEditorSaveFileHelper)
    {
        _textEditorSaveFileHelper = textEditorSaveFileHelper;
    }

    public void SetContent(string content)
    {
        SetIsDirtyTrue();

        // Any modified state needs to be 'null coallesce assigned' to the existing TextEditorModel's value. When reading state, if the state had been 'null coallesce assigned' then the field will be read. Otherwise, the existing TextEditorModel's value will be read.
        {
            _mostCharactersOnASingleLineTuple ??= _textEditorModel.MostCharactersOnASingleLineTuple;
            _lineEndList ??= _textEditorModel.LineEndList.ToList();
            _tabKeyPositionsList ??= _textEditorModel.TabKeyPositionList.ToList();
            _lineEndKindCountList ??= _textEditorModel.LineEndKindCountList.ToList();
            _editBlocksList ??= _textEditorModel.EditBlockList.ToList();
            _editBlockIndex ??= _textEditorModel.EditBlockIndex;
        }

        ClearAllStatesButKeepEditHistory();

		if (_editBlocksList.Count == 0 && _editBlockIndex == 0)
			_editBlocksList.Add(new TextEditorEditConstructor());

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
                if (charactersOnRow > MostCharactersOnASingleLineTuple.lineLength - TextEditorModel.MOST_CHARACTERS_ON_A_SINGLE_ROW_MARGIN)
                    _mostCharactersOnASingleLineTuple = (rowIndex, charactersOnRow + TextEditorModel.MOST_CHARACTERS_ON_A_SINGLE_ROW_MARGIN);

                LineEndList.Insert(rowIndex, new(index, index + 1, LineEndKind.CarriageReturn));
                rowIndex++;

                charactersOnRow = 0;

                carriageReturnCount++;
            }
            else if (character == KeyboardKeyFacts.WhitespaceCharacters.NEW_LINE)
            {
                if (charactersOnRow > MostCharactersOnASingleLineTuple.lineLength - TextEditorModel.MOST_CHARACTERS_ON_A_SINGLE_ROW_MARGIN)
                    _mostCharactersOnASingleLineTuple = (rowIndex, charactersOnRow + TextEditorModel.MOST_CHARACTERS_ON_A_SINGLE_ROW_MARGIN);

                if (previousCharacter == KeyboardKeyFacts.WhitespaceCharacters.CARRIAGE_RETURN)
                {
                    var lineEnding = LineEndList[rowIndex - 1];
                    lineEnding.EndPositionIndexExclusive++;
                    lineEnding.LineEndKind = LineEndKind.CarriageReturnLineFeed;

                    carriageReturnCount--;
                    carriageReturnLinefeedCount++;
                }
                else
                {
                    LineEndList.Insert(rowIndex, new(index, index + 1, LineEndKind.LineFeed));
                    rowIndex++;

                    linefeedCount++;
                }

                charactersOnRow = 0;
            }

            if (character == KeyboardKeyFacts.WhitespaceCharacters.TAB)
                TabKeyPositionList.Add(index);

            previousCharacter = character;
        }

        __InsertRange(0, content.Select(x => new RichCharacter
        {
            Value = x,
            DecorationByte = default,
        }));

        // Update the line end count list (TODO: Fix the awkward tuple not a variable logic going on here)
        {
            {
                var indexCarriageReturn = _lineEndKindCountList.FindIndex(x => x.lineEndKind == LineEndKind.CarriageReturn);
                _lineEndKindCountList[indexCarriageReturn] = (LineEndKind.CarriageReturn, carriageReturnCount);
            }
            {
                var indexLineFeed = _lineEndKindCountList.FindIndex(x => x.lineEndKind == LineEndKind.LineFeed);
                _lineEndKindCountList[indexLineFeed] = (LineEndKind.LineFeed, linefeedCount);
            }
            {
                var indexCarriageReturnLineFeed = _lineEndKindCountList.FindIndex(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed);
                _lineEndKindCountList[indexCarriageReturnLineFeed] = (LineEndKind.CarriageReturnLineFeed, carriageReturnLinefeedCount);
            }
        }

        // Update the EndOfFile line end.
        {
            var endOfFile = _lineEndList[^1];

            if (endOfFile.LineEndKind != LineEndKind.EndOfFile)
                throw new LuthetusTextEditorException($"The text editor model is malformed; the final entry of {nameof(_lineEndList)} must be the {nameof(LineEndKind)}.{nameof(LineEndKind.EndOfFile)}");

            endOfFile.StartPositionIndexInclusive = content.Length;
            endOfFile.EndPositionIndexExclusive = content.Length;
        }

        CheckRowEndingPositions(true);
    }

    public void ClearAllStatesButKeepEditHistory()
    {
        ClearContent();
        ClearOnlyRowEndingKind();
        SetLineEndKindPreference(LineEndKind.Unset);
    }

    public void HandleKeyboardEvent(
        KeyboardEventArgs keyboardEventArgs,
        CursorModifierBagTextEditor cursorModifierBag,
        CancellationToken cancellationToken)
    {
        if (KeyboardKeyFacts.IsMetaKey(keyboardEventArgs))
        {
            if (KeyboardKeyFacts.MetaKeys.BACKSPACE == keyboardEventArgs.Key)
            {
                Delete(
                    cursorModifierBag,
                    1,
                    keyboardEventArgs.CtrlKey,
                    DeleteKind.Backspace,
                    cancellationToken);
            }
            else if (KeyboardKeyFacts.MetaKeys.DELETE == keyboardEventArgs.Key)
            {
                Delete(
                    cursorModifierBag,
                    1,
                    keyboardEventArgs.CtrlKey,
                    DeleteKind.Delete,
                    cancellationToken);
            }
        }
        else
        {
            for (int i = cursorModifierBag.List.Count - 1; i >= 0; i--)
            {
                var cursor = cursorModifierBag.List[i];

                var singledCursorModifierBag = new CursorModifierBagTextEditor(
                    cursorModifierBag.ViewModelKey,
                    new List<TextEditorCursorModifier> { cursor });

                var valueToInsert = keyboardEventArgs.Key.First().ToString();

                if (keyboardEventArgs.Code == KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE)
                    valueToInsert = LineEndKindPreference.AsCharacters();
                else if (keyboardEventArgs.Code == KeyboardKeyFacts.WhitespaceCodes.TAB_CODE)
                    valueToInsert = "\t";

                Insert(
                    valueToInsert,
                    singledCursorModifierBag,
                    cancellationToken: cancellationToken);
            }
        }
    }

	private void PerformInsert(int positionIndex, string content)
	{
		var (lineIndex, columnIndex) = this.GetLineAndColumnIndicesFromPositionIndex(positionIndex);
		var cursor = new TextEditorCursor(lineIndex, columnIndex, true);
		var cursorModifierBag = new CursorModifierBagTextEditor(
			Key<TextEditorViewModel>.Empty,
			new List<TextEditorCursorModifier> { new(cursor) });
	
		Insert(content, cursorModifierBag, false, CancellationToken.None);
	}

    /// <param name="useLineEndKindPreference">
    /// If false, then the string will be inserted as is.
    /// If true, then the string will have its line endings replaced with the <see cref="LineEndKindPreference"/>
    /// </param>
    public void Insert(
        string value,
        CursorModifierBagTextEditor cursorModifierBag,
        bool useLineEndKindPreference = true,
        CancellationToken cancellationToken = default)
    {
        // Any modified state needs to be 'null coallesce assigned' to the existing TextEditorModel's value. When reading state, if the state had been 'null coallesce assigned' then the field will be read. Otherwise, the existing TextEditorModel's value will be read.
        {
            _lineEndList ??= _textEditorModel.LineEndList.ToList();
            _tabKeyPositionsList ??= _textEditorModel.TabKeyPositionList.ToList();
            _mostCharactersOnASingleLineTuple ??= _textEditorModel.MostCharactersOnASingleLineTuple;
        }

        for (var cursorIndex = cursorModifierBag.List.Count - 1; cursorIndex >= 0; cursorIndex--)
        {
            var cursorModifier = cursorModifierBag.List[cursorIndex];

            if (TextEditorSelectionHelper.HasSelectedText(cursorModifier))
            {
                Delete(
                    cursorModifierBag,
                    1,
                    false,
                    DeleteKind.Delete,
                    CancellationToken.None);
            }

            {
                // TODO: If one inserts a carriage return character,
                //       meanwhile the text editor model happens to have a line feed character at the position
                //       you are inserting at.
                //       |
                //       Then, the '\r' takes the position of the '\n', and the '\n' is shifted further
                //       by 1 position in order to allow space the '\r'.
                //       |
                //       Well, now the text editor model sees its contents as "\r\n".
                //       What is to be done in this scenario?
                //       (2024-04-22)
            }

            // Remember the cursorPositionIndex
            var initialCursorPositionIndex = this.GetPositionIndex(cursorModifier);

            // Track metadata with the cursorModifier itself
            //
            // Metadata must be done prior to 'InsertValue'
            //
            // 'value' is replaced by the original with any line endings changed (based on 'useLineEndKindPreference').
            value = InsertMetadata(value, cursorModifier, useLineEndKindPreference, cancellationToken);

            // Now the text still needs to be inserted.
            // The cursorModifier is invalid, because the metadata step moved its position.
            // So, use the 'cursorPositionIndex' variable that was calculated prior to the metadata step.
            InsertValue(value, initialCursorPositionIndex, useLineEndKindPreference, cancellationToken);

			EnsureUndoPoint(new TextEditorEditInsert(initialCursorPositionIndex, value));

            // NOTE: One cannot obtain the 'MostCharactersOnASingleLineTuple' from within the 'InsertMetadata(...)'
            //       method because this specific metadata is being calculated by counting the characters, which
            //       in the case of 'InsertMetadata(...)' wouldn't have been inserted yet.
            //
            // TODO: Fix tracking the MostCharactersOnASingleRowTuple this way is possibly inefficient - should instead only check the rows that changed
            {
                (int rowIndex, int rowLength) localMostCharactersOnASingleRowTuple = (0, 0);

                for (var i = 0; i < LineEndList.Count; i++)
                {
                    var lengthOfRow = this.GetLineLength(i);

                    if (lengthOfRow > localMostCharactersOnASingleRowTuple.rowLength)
                        localMostCharactersOnASingleRowTuple = (i, lengthOfRow);
                }

                localMostCharactersOnASingleRowTuple = (localMostCharactersOnASingleRowTuple.rowIndex,
                    localMostCharactersOnASingleRowTuple.rowLength + TextEditorModel.MOST_CHARACTERS_ON_A_SINGLE_ROW_MARGIN);

                _mostCharactersOnASingleLineTuple = localMostCharactersOnASingleRowTuple;
            }
        }

        SetIsDirtyTrue();
    }

    private string InsertMetadata(
        string value,
        TextEditorCursorModifier cursorModifier,
        bool useLineEndKindPreference,
        CancellationToken cancellationToken)
    {
        // Any modified state needs to be 'null coallesce assigned' to the existing TextEditorModel's value. When reading state, if the state had been 'null coallesce assigned' then the field will be read. Otherwise, the existing TextEditorModel's value will be read.
        {
            _lineEndList ??= _textEditorModel.LineEndList.ToList();
            _tabKeyPositionsList ??= _textEditorModel.TabKeyPositionList.ToList();
            _mostCharactersOnASingleLineTuple ??= _textEditorModel.MostCharactersOnASingleLineTuple;
        }

        var initialCursorPositionIndex = this.GetPositionIndex(cursorModifier);
        var initialCursorLineIndex = cursorModifier.LineIndex;

        this.AssertPositionIndex(initialCursorPositionIndex);

        bool isTab = false;
        bool isCarriageReturn = false;
        bool isLineFeed = false;
        bool isCarriageReturnLineFeed = false;

        (int? index, List<LineEnd> localLineEndList) lineEndPositionLazyInsertRange = (null, new());
        (int? index, List<int> localTabPositionList) tabPositionLazyInsertRange = (null, new());

        var lineEndingsChangedValueBuilder = new StringBuilder();

        for (int charIndex = 0; charIndex < value.Length; charIndex++)
        {
            var charValue = value[charIndex];

            isTab = charValue == '\t';
            isCarriageReturn = charValue == '\r';
            isLineFeed = charValue == '\n';
            // The CRLF boolean must be checked prior to CR, as one is a "substring" of the other
            isCarriageReturnLineFeed = isCarriageReturn && charIndex != value.Length - 1 && value[1 + charIndex] == '\n';

            {
                // TODO: If one inserts a carriage return character, meanwhile the text editor model
                //       happens to have a line feed character at the position you are inserting at.
                //       |
                //       Then, the '\r' takes the position of the '\n', and the '\n' is shifted further
                //       by 1 position in order to allow space the '\r'.
                //       |
                //       Well, now the text editor model sees its contents as "\r\n".
                //       What is to be done in this scenario?
                //       (2024-04-22)
            }

            if (isLineFeed || isCarriageReturn || isCarriageReturnLineFeed)
            {
                // Regardless of which line ending is used, since the source text
                // is CRLF, one must increment the for loop one character further.
                if (isCarriageReturnLineFeed)
                    charIndex++;

                LineEndKind lineEndKind;
                
                if (useLineEndKindPreference)
                {
                    lineEndKind = LineEndKindPreference;
                }
                else
                {
                    lineEndKind =
                        // CRLF must be checked prior to CR, as one is a "substring" of the other.
                        isCarriageReturnLineFeed ? LineEndKind.CarriageReturnLineFeed :
                        isCarriageReturn ? LineEndKind.CarriageReturn :
                        isLineFeed ? LineEndKind.LineFeed :
                        LineEndKindPreference;
                }

                // The LineEndKindPreference can invalidate the booleans
                //
                // Additionally, by clearing all the booleans and then setting only one of them,
                //
                //     -"CRLF must be checked prior to CR, as one is a "substring" of the other"
                //
                // can be avoided.
                {
                    isCarriageReturnLineFeed = false;
                    isCarriageReturn = false;
                    isLineFeed = false;

                    if (lineEndKind == LineEndKind.CarriageReturnLineFeed)
                        isCarriageReturnLineFeed = true;
                    else if (lineEndKind == LineEndKind.CarriageReturn)
                        isCarriageReturn = true;
                    else if (lineEndKind == LineEndKind.LineFeed)
                        isLineFeed = true;
                }

                lineEndPositionLazyInsertRange.index ??= cursorModifier.LineIndex;

                var lineEndCharacters = lineEndKind.AsCharacters();

                lineEndPositionLazyInsertRange.localLineEndList.Add(new LineEnd(
                    initialCursorPositionIndex + lineEndingsChangedValueBuilder.Length,
                    lineEndCharacters.Length + initialCursorPositionIndex + lineEndingsChangedValueBuilder.Length,
                    lineEndKind));

                lineEndingsChangedValueBuilder.Append(lineEndCharacters);

                MutateLineEndKindCount(lineEndKind, 1);

                cursorModifier.LineIndex++;
                cursorModifier.SetColumnIndexAndPreferred(0);
            }
            else
            {
                if (isTab)
                {
                    if (tabPositionLazyInsertRange.index is null)
                    {
                        tabPositionLazyInsertRange.index = _tabKeyPositionsList.FindIndex(x => x >= initialCursorPositionIndex);

                        if (tabPositionLazyInsertRange.index == -1)
                            tabPositionLazyInsertRange.index = _tabKeyPositionsList.Count;
                    }

                    tabPositionLazyInsertRange.localTabPositionList.Add(initialCursorPositionIndex + lineEndingsChangedValueBuilder.Length);
                }

                lineEndingsChangedValueBuilder.Append(charValue);
                cursorModifier.SetColumnIndexAndPreferred(1 + cursorModifier.ColumnIndex);
            }
        }

        // Reposition the Row Endings
        {
            for (var i = initialCursorLineIndex; i < LineEndList.Count; i++)
            {
                var rowEndingTuple = LineEndList[i];
                rowEndingTuple.StartPositionIndexInclusive += lineEndingsChangedValueBuilder.Length;
                rowEndingTuple.EndPositionIndexExclusive += lineEndingsChangedValueBuilder.Length;
            }
        }

        // Reposition the Tabs
        {
            var firstTabKeyPositionIndexToModify = _tabKeyPositionsList.FindIndex(x => x >= initialCursorPositionIndex);

            if (firstTabKeyPositionIndexToModify != -1)
            {
                for (var i = firstTabKeyPositionIndexToModify; i < TabKeyPositionList.Count; i++)
                {
                    TabKeyPositionList[i] += lineEndingsChangedValueBuilder.Length;
                }
            }
        }

        // Reposition the Diagnostic Squigglies
        {
            var textSpanForInsertion = new TextEditorTextSpan(
                initialCursorPositionIndex,
                initialCursorPositionIndex + lineEndingsChangedValueBuilder.Length,
                0,
                new(string.Empty),
                string.Empty);

            var textModification = new TextEditorTextModification(true, textSpanForInsertion);

            foreach (var presentationModel in PresentationModelList)
            {
                presentationModel.CompletedCalculation?.TextModificationsSinceRequestList.Add(textModification);
                presentationModel.PendingCalculation?.TextModificationsSinceRequestList.Add(textModification);
            }
        }

        // Add in any new metadata
        {
            if (lineEndPositionLazyInsertRange.index is not null)
            {
                _lineEndList.InsertRange(
                    lineEndPositionLazyInsertRange.index.Value,
                    lineEndPositionLazyInsertRange.localLineEndList);
            }

            if (tabPositionLazyInsertRange.index is not null)
            {
                _tabKeyPositionsList.InsertRange(
                    tabPositionLazyInsertRange.index.Value,
                    tabPositionLazyInsertRange.localTabPositionList);
            }
        }

        return lineEndingsChangedValueBuilder.ToString();
    }

    private void InsertValue(
        string value,
        int cursorPositionIndex,
        bool useLineEndKindPreference,
        CancellationToken cancellationToken)
    {
        // If cursor is out of bounds then continue
        if (cursorPositionIndex > _richCharacterList.Count)
            return;

        __InsertRange(
            cursorPositionIndex,
            value.Select(character => new RichCharacter
            {
                Value = character,
                DecorationByte = 0,
            }));
    }

private void PerformBackspace(int positionIndex, int count)
{
/*
CursorModifierBagTextEditor cursorModifierBag,
        int columnCount,
        bool expandWord,
        DeleteKind deleteKind,
        CancellationToken cancellationToken = default
*/

	Delete(
null,
count,
false,
DeleteKind.Backspace,
CancellationToken.None);
}

private void PerformDelete(int positionIndex, int count)
{
}

    /// <summary>
    /// This method allows for a "RemoveRange" like operation on the text editor's contents.
    /// Any meta-data will automatically be updated (e.g. <see cref="ITextEditorModel.LineEndKindCountList"/>.
    /// </summary>
    /// <param name="cursorModifierBag">
    /// The list of cursors that indicate the positionIndex to start a "RemoveRange" operation.
    /// The cursors are iterated backwards, with each cursor being its own "RemoveRange" operation.
    /// </param>
    /// <param name="columnCount">
    /// The amount of columns to delete. If a the value of a column is of 2-char length (e.g. "\r\n"),
    /// then internally this columnCount will be converted to a 'charCount' of the corrected length.
    /// </param>
    /// <param name="expandWord">
    /// Applied after moving by the 'count' parameter.<br/>
    /// Ex:
    ///     count of 1, and expandWord of true;
    ///     will move 1 char-value from the initialPositionIndex.
    ///     Afterwards, if expandWord is true, then the cursor is checked to be within a word, or at the start or end of one.
    ///     If the cursor is at the start or end of one, then the selection to delete is expanded such that it contains
    ///     the entire word that the cursor ended at.
    /// </param>
    /// <param name="cancellationToken"></param>
    /// <param name="deleteKind">
    /// The default <see cref="DeleteKind.Delete"/> will do logic similar to a "RemoveRange" like
    /// operation on a collection type.<br/>
    /// If one has keyboard input from a user, one might want to have the user's backspace key
    /// pass in the <see cref="DeleteKind.Backspace"/> parameter.
    /// Then, for a user's delete key, pass in <see cref="DeleteKind.Delete"/>.
    /// </param>
    public void Delete(
        CursorModifierBagTextEditor cursorModifierBag,
        int columnCount,
        bool expandWord,
        DeleteKind deleteKind,
        CancellationToken cancellationToken = default)
    {
        if (columnCount < 0)
            throw new LuthetusTextEditorException($"{nameof(columnCount)} < 0");

        // Any modified state needs to be 'null coallesce assigned' to the existing TextEditorModel's value. When reading state, if the state had been 'null coallesce assigned' then the field will be read. Otherwise, the existing TextEditorModel's value will be read.
        {
            _lineEndList ??= _textEditorModel.LineEndList.ToList();
            _tabKeyPositionsList ??= _textEditorModel.TabKeyPositionList.ToList();
            _mostCharactersOnASingleLineTuple ??= _textEditorModel.MostCharactersOnASingleLineTuple;
        }

        for (var cursorIndex = cursorModifierBag.List.Count - 1; cursorIndex >= 0; cursorIndex--)
        {
            var cursorModifier = cursorModifierBag.List[cursorIndex];

            var tuple = DeleteMetadata(columnCount, cursorModifier, expandWord, deleteKind, cancellationToken);

            if (tuple is null)
            {
                SetIsDirtyTrue();
                return;
            }

            var (positionIndex, charCount) = tuple.Value;
            DeleteValue(positionIndex, charCount, cancellationToken);

			if (deleteKind == DeleteKind.Delete)
				EnsureUndoPoint(new TextEditorEditDelete(positionIndex, charCount));
			else if (deleteKind == DeleteKind.Backspace)
				EnsureUndoPoint(new TextEditorEditBackspace(positionIndex, charCount));
			else
				throw new NotImplementedException($"The {nameof(DeleteKind)}: {deleteKind} was not recognized.");

            // NOTE: One cannot obtain the 'MostCharactersOnASingleLineTuple' from within the 'DeleteMetadata(...)'
            //       method because this specific metadata is being calculated by counting the characters, which
            //       in the case of 'DeleteMetadata(...)' wouldn't have been deleted yet.
            //
            // TODO: Fix tracking the MostCharactersOnASingleLineTuple this way is possibly inefficient - should instead only check the rows that changed
            {
                (int lineIndex, int lineLength) localMostCharactersOnASingleLineTuple = (0, 0);

                for (var i = 0; i < LineEndList.Count; i++)
                {
                    var lengthOfLine = this.GetLineLength(i);

                    if (lengthOfLine > localMostCharactersOnASingleLineTuple.lineLength)
                    {
                        localMostCharactersOnASingleLineTuple = (i, lengthOfLine);
                    }
                }

                localMostCharactersOnASingleLineTuple = (
                    localMostCharactersOnASingleLineTuple.lineIndex,
                    localMostCharactersOnASingleLineTuple.lineLength + TextEditorModel.MOST_CHARACTERS_ON_A_SINGLE_ROW_MARGIN);

                _mostCharactersOnASingleLineTuple = localMostCharactersOnASingleLineTuple;
            }
        }

        SetIsDirtyTrue();
    }

    /// <summary>
    /// The text editor sees "\r\n" as 1 character, even though that is made up of 2 char values.
    /// The <see cref="TextEditorPartition"/> however, sees "\r\n" as 2 char values.<br/><br/>
    /// 
    /// This different means, to delete "\r\n" one tells the text editor to delete 1 character,
    /// where as one tells the <see cref="TextEditorPartition"/> to delete 2 char values.<br/><br/>
    /// 
    /// This method returns the 'int charValueCount', so that it can be used
    /// in the <see cref="DeleteValue(int, int, CancellationToken)"/> method.
    /// </summary>
    private (int positionIndex, int charCount)? DeleteMetadata(
        int columnCount,
        TextEditorCursorModifier cursorModifier,
        bool expandWord,
        DeleteKind deleteKind,
        CancellationToken cancellationToken)
    {
        // Any modified state needs to be 'null coallesce assigned' to the existing TextEditorModel's value. When reading state, if the state had been 'null coallesce assigned' then the field will be read. Otherwise, the existing TextEditorModel's value will be read.
        {
            _lineEndList ??= _textEditorModel.LineEndList.ToList();
            _tabKeyPositionsList ??= _textEditorModel.TabKeyPositionList.ToList();
            _mostCharactersOnASingleLineTuple ??= _textEditorModel.MostCharactersOnASingleLineTuple;
        }

        var initiallyHadSelection = TextEditorSelectionHelper.HasSelectedText(cursorModifier);
        var initialLineIndex = cursorModifier.LineIndex;
        var positionIndex = this.GetPositionIndex(cursorModifier);

        if (initiallyHadSelection && cursorModifier.SelectionAnchorPositionIndex is not null)
        {
            // If user's cursor has a selection, then set the variables so the positionIndex is the
            // selection.AnchorPositionIndex and the count is selection.EndPositionIndex - selection.AnchorPositionIndex
            // and that the 'DeleteKind.Delete' logic runs.
            var (lowerPositionIndexInclusive, upperPositionIndexExclusive) = TextEditorSelectionHelper.GetSelectionBounds(cursorModifier);

            var lowerLineData = this.GetLineInformationFromPositionIndex(lowerPositionIndexInclusive);
            var lowerColumnIndex = lowerPositionIndexInclusive - lowerLineData.StartPositionIndexInclusive;

            cursorModifier.LineIndex = lowerLineData.Index;
            initialLineIndex = cursorModifier.LineIndex;
            cursorModifier.SetColumnIndexAndPreferred(lowerColumnIndex);
            positionIndex = lowerPositionIndexInclusive;

            // The deletion of a selection logic does not check for multibyte characters.
            // Therefore, later in this method, if a multibyte character is found, the columnCount must be reduced. (2024-05-01)
            columnCount = upperPositionIndexExclusive - lowerPositionIndexInclusive;
            deleteKind = DeleteKind.Delete;

            cursorModifier.SelectionAnchorPositionIndex = null;
            cursorModifier.SelectionEndingPositionIndex = 0;
        }

        this.AssertPositionIndex(positionIndex);

        (int? index, int count) lineEndPositionLazyRemoveRange = (null, 0);
        (int? index, int count) tabPositionLazyRemoveRange = (null, 0);

        var charCount = 0;

        if (deleteKind == DeleteKind.Delete)
        {
            if (expandWord && !initiallyHadSelection)
            {
                var columnIndexOfCharacterWithDifferingKind = this.GetColumnIndexOfCharacterWithDifferingKind(
                    cursorModifier.LineIndex,
                    cursorModifier.ColumnIndex,
                    false);

                // -1 implies that no differing kind was found on the current line.
                if (columnIndexOfCharacterWithDifferingKind == -1)
                {
                    var line = this.GetLineInformation(cursorModifier.LineIndex);
                    columnIndexOfCharacterWithDifferingKind = line.LastValidColumnIndex;
                }

                columnCount = columnIndexOfCharacterWithDifferingKind - cursorModifier.ColumnIndex;

                // Cursor is at the start of a row
                if (columnCount == 0)
                    columnCount = 1;
            }

            for (int i = 0; i < columnCount; i++)
            {
                var toDeletePositionIndex = positionIndex + charCount;
                if (toDeletePositionIndex < 0 || toDeletePositionIndex >= CharCount)
                    break;

                var richCharacterToDelete = _richCharacterList[toDeletePositionIndex];

                if (KeyboardKeyFacts.IsLineEndingCharacter(richCharacterToDelete.Value))
                {
                    // A delete is a contiguous operation. Therefore, all that is needed to update the LineEndList
                    // is a starting index, and a count.
                    var indexLineEnd = _lineEndList.FindIndex(
                        x => x.StartPositionIndexInclusive == toDeletePositionIndex);

                    var lineEnd = LineEndList[indexLineEnd];

                    // Delete starts at the lowest index, therefore use '??=' to only assign once.
                    lineEndPositionLazyRemoveRange.index ??= indexLineEnd;
                    lineEndPositionLazyRemoveRange.count++;

                    var lengthOfLineEnd = LineEndList[indexLineEnd].LineEndKind.AsCharacters().Length;
                    charCount += lengthOfLineEnd;

                    MutateLineEndKindCount(lineEnd.LineEndKind, -1);

                    if (lineEnd.LineEndKind == LineEndKind.CarriageReturnLineFeed && initiallyHadSelection)
                    {
                        // The deletion of a selection logic does not check for multibyte characters.
                        // Therefore, if a multibyte character is found, the columnCount must be reduced. (2024-05-01)
                        columnCount--;
                    }
                }
                else
                {
                    charCount++;

                    if (richCharacterToDelete.Value == KeyboardKeyFacts.WhitespaceCharacters.TAB)
                    {
                        var indexTabKey = _tabKeyPositionsList.FindIndex(
                            x => x == toDeletePositionIndex);

                        // Delete starts at the lowest index, therefore use '??=' to only assign once.
                        tabPositionLazyRemoveRange.index ??= indexTabKey;
                        tabPositionLazyRemoveRange.count++;
                    }
                }
            }
        }
        else if (deleteKind == DeleteKind.Backspace)
        {
            if (expandWord && !initiallyHadSelection)
            {
                var columnIndexOfCharacterWithDifferingKind = this.GetColumnIndexOfCharacterWithDifferingKind(
                    cursorModifier.LineIndex,
                    cursorModifier.ColumnIndex,
                    true);

                // -1 implies that no differing kind was found on the current line.
                if (columnIndexOfCharacterWithDifferingKind == -1)
                    columnIndexOfCharacterWithDifferingKind = 0;

                columnCount = cursorModifier.ColumnIndex - columnIndexOfCharacterWithDifferingKind;

                // Cursor is at the start of a row
                if (columnCount == 0)
                    columnCount = 1;
            }

            for (int i = 0; i < columnCount; i++)
            {
                // Minus 1 here because 'Backspace' deletes the previous character.
                var toDeletePositionIndex = positionIndex - charCount - 1;
                if (toDeletePositionIndex < 0 || toDeletePositionIndex >= CharCount)
                    break;

                var richCharacterToDelete = _richCharacterList[toDeletePositionIndex];

                if (KeyboardKeyFacts.IsLineEndingCharacter(richCharacterToDelete.Value))
                {
                    // A delete is a contiguous operation. Therefore, all that is needed to update the LineEndList
                    // is a starting index, and a count.
                    var indexLineEnd = _lineEndList.FindIndex(
                        // Check for '\n' or '\r'
                        x => x.EndPositionIndexExclusive == toDeletePositionIndex + 1 ||
                        // Check for "\r\n"
                        x.EndPositionIndexExclusive == toDeletePositionIndex + 2);

                    var lineEnd = LineEndList[indexLineEnd];

                    // Backspace starts at the highest index, therefore use '=' to only assign everytime.
                    lineEndPositionLazyRemoveRange.index = indexLineEnd;
                    lineEndPositionLazyRemoveRange.count++;

                    var lengthOfLineEnd = LineEndList[indexLineEnd].LineEndKind.AsCharacters().Length;
                    charCount += lengthOfLineEnd;

                    MutateLineEndKindCount(lineEnd.LineEndKind, -1);
                }
                else
                {
                    charCount++;

                    if (richCharacterToDelete.Value == KeyboardKeyFacts.WhitespaceCharacters.TAB)
                    {
                        var indexTabKey = _tabKeyPositionsList.FindIndex(
                            x => x == toDeletePositionIndex);

                        // Backspace starts at the highest index, therefore use '=' to only assign everytime.
                        tabPositionLazyRemoveRange.index = indexTabKey;
                        tabPositionLazyRemoveRange.count++;
                    }
                }
            }
        }
        else
        {
            throw new NotImplementedException();
        }

        // Reposition the LineEnd(s)
        {
            for (var i = initialLineIndex; i < LineEndList.Count; i++)
            {
                var lineEnd = LineEndList[i];
                lineEnd.StartPositionIndexInclusive -= charCount;
                lineEnd.EndPositionIndexExclusive -= charCount;
            }
        }

        // Reposition the Tab(s)
        {
            var firstTabKeyPositionIndexToModify = _tabKeyPositionsList.FindIndex(x => x >= positionIndex);

            if (firstTabKeyPositionIndexToModify != -1)
            {
                for (var i = firstTabKeyPositionIndexToModify; i < TabKeyPositionList.Count; i++)
                {
                    TabKeyPositionList[i] -= charCount;
                }
            }
        }

        // Reposition the PresentationModel(s)
        {
            var textSpanForInsertion = new TextEditorTextSpan(
                positionIndex,
                positionIndex + charCount,
                0,
                new(string.Empty),
                string.Empty);

            var textModification = new TextEditorTextModification(false, textSpanForInsertion);

            foreach (var presentationModel in PresentationModelList)
            {
                presentationModel.CompletedCalculation?.TextModificationsSinceRequestList.Add(textModification);
                presentationModel.PendingCalculation?.TextModificationsSinceRequestList.Add(textModification);
            }
        }

        // Delete metadata
        {
            if (lineEndPositionLazyRemoveRange.index is not null)
            {
                _lineEndList.RemoveRange(
                    lineEndPositionLazyRemoveRange.index.Value,
                    lineEndPositionLazyRemoveRange.count);
            }

            if (tabPositionLazyRemoveRange.index is not null)
            {
                _tabKeyPositionsList.RemoveRange(
                    tabPositionLazyRemoveRange.index.Value,
                    tabPositionLazyRemoveRange.count);
            }
        }

        if (deleteKind == DeleteKind.Delete)
        {
            // Reposition the cursor
            {
                var (lineIndex, columnIndex) = this.GetLineAndColumnIndicesFromPositionIndex(positionIndex);
                cursorModifier.LineIndex = lineIndex;
                cursorModifier.SetColumnIndexAndPreferred(columnIndex);
            }

            return (positionIndex, charCount);
        }
        else if (deleteKind == DeleteKind.Backspace)
        {
            var calculatedPositionIndex = positionIndex - charCount;

            // Reposition the cursor
            {
                var (lineIndex, columnIndex) = this.GetLineAndColumnIndicesFromPositionIndex(calculatedPositionIndex);
                cursorModifier.LineIndex = lineIndex;
                cursorModifier.SetColumnIndexAndPreferred(columnIndex);
            }

            return (calculatedPositionIndex, charCount);
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    private void DeleteValue(int positionIndex, int count, CancellationToken cancellationToken)
    {
        // If cursor is out of bounds then continue
        if (positionIndex >= _richCharacterList.Count)
            return;

        __RemoveRange(positionIndex, count);
    }

    public void DeleteTextByMotion(
        MotionKind motionKind,
        CursorModifierBagTextEditor cursorModifierBag,
        CancellationToken cancellationToken)
    {
        var keyboardEventArgs = motionKind switch
        {
            MotionKind.Backspace => new KeyboardEventArgs { Key = KeyboardKeyFacts.MetaKeys.BACKSPACE },
            MotionKind.Delete => new KeyboardEventArgs { Key = KeyboardKeyFacts.MetaKeys.DELETE },
            _ => throw new LuthetusTextEditorException($"The {nameof(MotionKind)}: {motionKind} was not recognized.")
        };

        HandleKeyboardEvent(
            keyboardEventArgs,
            cursorModifierBag,
            CancellationToken.None);
    }

    public void DeleteByRange(
        int count,
        CursorModifierBagTextEditor cursorModifierBag,
        CancellationToken cancellationToken)
    {
        for (int cursorIndex = cursorModifierBag.List.Count - 1; cursorIndex >= 0; cursorIndex--)
        {
            var cursor = cursorModifierBag.List[cursorIndex];

            var singledCursorModifierBag = new CursorModifierBagTextEditor(
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

    public void ClearEditBlocks()
    {
        // Any modified state needs to be 'null coallesce assigned' to the existing TextEditorModel's value. When reading state, if the state had been 'null coallesce assigned' then the field will be read. Otherwise, the existing TextEditorModel's value will be read.
        {
            _editBlocksList ??= _textEditorModel.EditBlockList.ToList();
            _editBlockIndex ??= _textEditorModel.EditBlockIndex;
        }

        _editBlockIndex = 0;
        EditBlockList.Clear();
    }

	private void EnsureUndoPoint(ITextEditorEdit newEdit)
	{
		// Any modified state needs to be 'null coallesce assigned' to the existing TextEditorModel's value. When reading state, if the state had been 'null coallesce assigned' then the field will be read. Otherwise, the existing TextEditorModel's value will be read.
        {
            _editBlocksList ??= _textEditorModel.EditBlockList.ToList();
            _editBlockIndex ??= _textEditorModel.EditBlockIndex;
        }

		if (newEdit.EditKind == TextEditorEditKind.Insert)
		{
			var mostRecentEdit = _editBlocksList[_editBlockIndex.Value];

			var newEditInsert = (TextEditorEditInsert)newEdit;
			var positionIndex = newEditInsert.PositionIndex;
			var content = newEditInsert.Content;

			if (mostRecentEdit.EditKind == TextEditorEditKind.Insert)
			{
				var mostRecentEditInsert = (TextEditorEditInsert)mostRecentEdit;
	
				// Only batch if consecutive, and contiguous.
				if (positionIndex == mostRecentEditInsert.PositionIndex + mostRecentEditInsert.Content.Length)
				{
					var contentBuilder = new StringBuilder();
					contentBuilder.Append(mostRecentEditInsert.Content);
					contentBuilder.Append(content);
		
					var insertBatch = new TextEditorEditInsertBatch(
						mostRecentEditInsert.PositionIndex,
						contentBuilder);
		
					_editBlocksList[_editBlockIndex.Value] = insertBatch;
					return;
				}
			}
			
			if (mostRecentEdit.EditKind == TextEditorEditKind.InsertBatch)
			{
				var mostRecentEditInsertBatch = (TextEditorEditInsertBatch)mostRecentEdit;
	
				// Only batch if consecutive, and contiguous.
				if (positionIndex == mostRecentEditInsertBatch.PositionIndex + mostRecentEditInsertBatch.ContentBuilder.Length)
				{
					mostRecentEditInsertBatch.ContentBuilder.Append(content);
					return;
				}
			}
			
			// Default case
			{
				_editBlocksList.Add(new TextEditorEditInsert(positionIndex, content));
				_editBlockIndex++;
				return;
			}
		}
		else if (newEdit.EditKind == TextEditorEditKind.Backspace)
		{
			var mostRecentEdit = _editBlocksList[_editBlockIndex.Value];

			var newEditBackspace = (TextEditorEditBackspace)newEdit;
			var positionIndex = newEditBackspace.PositionIndex;
			var count = newEditBackspace.Count;
			var textRemoved = newEditBackspace.TextRemoved;

			if (mostRecentEdit.EditKind == TextEditorEditKind.Backspace)
			{
				var mostRecentEditBackspace = (TextEditorEditBackspace)mostRecentEdit;
	
				// Only batch if consecutive, and contiguous.
				if (positionIndex == mostRecentEditBackspace.PositionIndex - mostRecentEditBackspace.TextRemoved.Length)
				{
					// NOTE: The most recently removed text should go first, this is contrary to the Delete(...) method.
					var textRemovedBuilder = new StringBuilder();
					textRemovedBuilder.Append(textRemoved);
					textRemovedBuilder.Append(mostRecentEditBackspace.TextRemoved);
					
	
					var editBackspaceBatch = new TextEditorEditBackspaceBatch(
						mostRecentEditBackspace.PositionIndex,
						count + mostRecentEditBackspace.Count,
						textRemovedBuilder);
	
					_editBlocksList[_editBlockIndex.Value] = editBackspaceBatch;
					return;
				}
			}
	
			if (mostRecentEdit.EditKind == TextEditorEditKind.BackspaceBatch)
			{
				var mostRecentEditBackspaceBatch = (TextEditorEditBackspaceBatch)mostRecentEdit;
	
				// Only batch if consecutive, and contiguous.
				if (positionIndex == mostRecentEditBackspaceBatch.PositionIndex - mostRecentEditBackspaceBatch.TextRemovedBuilder.Length)
				{
					mostRecentEditBackspaceBatch.Add(count, textRemoved);
					return;
				}
			}
			
			// Default case
			{
				var editBackspace = new TextEditorEditBackspace(positionIndex, count);
				editBackspace.TextRemoved = textRemoved;
				_editBlocksList.Add(editBackspace);
				_editBlockIndex++;
				return;
			}
		}
		else if (newEdit.EditKind == TextEditorEditKind.Delete)
		{
			var mostRecentEdit = _editBlocksList[_editBlockIndex.Value];

			var newEditDelete = (TextEditorEditDelete)newEdit;
			var positionIndex = newEditDelete.PositionIndex;
			var count = newEditDelete.Count;
			var textRemoved = newEditDelete.TextRemoved;

			if (mostRecentEdit.EditKind == TextEditorEditKind.Delete)
			{
				var mostRecentEditDelete = (TextEditorEditDelete)mostRecentEdit;
	
				// Only batch if consecutive, and contiguous.
				if (positionIndex == mostRecentEditDelete.PositionIndex)
				{
					var textRemovedBuilder = new StringBuilder();
					textRemovedBuilder.Append(mostRecentEditDelete.TextRemoved);
					textRemovedBuilder.Append(textRemoved);
	
					var editDeleteBatch = new TextEditorEditDeleteBatch(
						positionIndex,
						count + mostRecentEditDelete.Count,
						textRemovedBuilder);
	
					_editBlocksList[_editBlockIndex.Value] = editDeleteBatch;
					return;
				}
			}
	
			if (mostRecentEdit.EditKind == TextEditorEditKind.DeleteBatch)
			{
				var mostRecentEditDeleteBatch = (TextEditorEditDeleteBatch)mostRecentEdit;
	
				// Only batch if consecutive, and contiguous.
				if (positionIndex == mostRecentEditDeleteBatch.PositionIndex)
				{
					mostRecentEditDeleteBatch.Add(count, textRemoved);
					return;
				}
			}
			
			// Default case
			{
				var editDelete = new TextEditorEditDelete(positionIndex, count);
				editDelete.TextRemoved = textRemoved;
				_editBlocksList.Add(editDelete);
				_editBlockIndex++;
				return;
			}
		}

	// TODO: the following multi line comment contains code from the original implementation...
	//       ...which deleted outdated history. This logic needs to be re-added in some way.
	/*
		var mostRecentEditBlock = EditBlockList.LastOrDefault();
	
	    if (mostRecentEditBlock is null || mostRecentEditBlock.TextEditKind != textEditKind)
	    {
	        var newEditBlockIndex = EditBlockIndex;
	
	        EditBlockList.Insert(newEditBlockIndex, new EditBlock(
	            textEditKind,
	            textEditKind.ToString(),
	            this.GetAllText(),
	            otherTextEditKindIdentifier));
	
	        var removeBlocksStartingAt = newEditBlockIndex + 1;
	
	        _editBlocksList.RemoveRange(removeBlocksStartingAt, EditBlockList.Count - removeBlocksStartingAt);
	
	        _editBlockIndex++;
	    }
	
	    while (EditBlockList.Count > TextEditorModel.MAXIMUM_EDIT_BLOCKS && EditBlockList.Count != 0)
	    {
	        _editBlockIndex--;
	        EditBlockList.RemoveAt(0);
	    }
	*/
	}

	public void OpenOtherEdit(TextEditorEditOther editOther)
	{
		OtherEditStack.Push(editOther);
		_editBlocksList.Add(editOther);
		_editBlockIndex++;
	}

	public void CloseOtherEdit(string predictedTag)
	{
		var peek = OtherEditStack.Peek();
		if (peek.Tag != predictedTag)
		{
			throw new LuthetusTextEditorException(
				$"Attempted to close other edit with {nameof(TextEditorEditOther.Tag)}: '{peek.Tag}'." + 
				$" but, the {nameof(predictedTag)} was: '{predictedTag}'");
		}

		var pop = OtherEditStack.Pop();
		_editBlocksList.Add(pop);
		_editBlockIndex++;
	}

	public void UndoEdit()
	{
		// Any modified state needs to be 'null coallesce assigned' to the existing TextEditorModel's value. When reading state, if the state had been 'null coallesce assigned' then the field will be read. Otherwise, the existing TextEditorModel's value will be read.
        {
            _editBlocksList ??= _textEditorModel.EditBlockList.ToList();
            _editBlockIndex ??= _textEditorModel.EditBlockIndex;
        }

		if (_editBlockIndex <= 0)
			throw new LuthetusTextEditorException("No edits are available to perform 'undo' on");

		var mostRecentEdit = _editBlocksList[_editBlockIndex.Value];
		var undoEdit = mostRecentEdit.ToUndo();
		
		// In case the 'ToUndo(...)' throws an exception, the decrement to the EditIndex
		// is being done only after a successful ToUndo(...)
		_editBlockIndex--;

		switch (undoEdit.EditKind)
		{
			case TextEditorEditKind.Insert:
				var insertEdit = (TextEditorEditInsert)undoEdit;
				PerformInsert(insertEdit.PositionIndex, insertEdit.Content);
				break;
			case TextEditorEditKind.Backspace:
				var backspaceEdit = (TextEditorEditBackspace)undoEdit;
				PerformBackspace(backspaceEdit.PositionIndex, backspaceEdit.Count);
				break;
			case TextEditorEditKind.Delete: 
				var deleteEdit = (TextEditorEditDelete)undoEdit;
				PerformDelete(deleteEdit.PositionIndex, deleteEdit.Count);
				break;
			case TextEditorEditKind.Other:
				while (true)
				{
					if (_editBlockIndex == 0)
					{
						// TODO: How does one handle the 'undo limit'...
						//       ...with respect to 'other' edits?
						//       If one does an 'other edit' with more child edits than
						//       the amount of undo history one can have.
						//
						//       Then it would be impossible
						//       to handle that 'other edit' properly.
						//
						//       Furthermore, one could have a small 'other edit' yet,
						//       by way of future edits moving the undo history,
						//       the 'other edit' opening will be lost.
						break;
					}

					mostRecentEdit = _editBlocksList[_editBlockIndex.Value];

					if (mostRecentEdit.EditKind == TextEditorEditKind.Other)
					{
						var mostRecentEditOther = (TextEditorEditOther)mostRecentEdit;
	
						// Nothing needs to be done when the tags don't match
						// other than continuing the while loop.
						//
						// Given that the 'CloseOtherEdit(...)'
						// will throw an exception when attempting to close a mismatching other edit.
						//
						// Finding the opening to the child 'other edit' is irrelevant since it is encompassed
						// within the parent.
						if (mostRecentEditOther.Tag == (((TextEditorEditOther)undoEdit).Tag))
						{
							// Need to go one further than the opening,
							_editBlockIndex--;
							break;
						}
					}
					else
					{
						UndoEdit();
					}
				}
				break;
			default:
				throw new NotImplementedException($"The {nameof(TextEditorEditKind)}: {undoEdit.EditKind} was not recognized.");
		}
	}

	public void RedoEdit()
	{
		// Any modified state needs to be 'null coallesce assigned' to the existing TextEditorModel's value. When reading state, if the state had been 'null coallesce assigned' then the field will be read. Otherwise, the existing TextEditorModel's value will be read.
        {
            _editBlocksList ??= _textEditorModel.EditBlockList.ToList();
            _editBlockIndex ??= _textEditorModel.EditBlockIndex;
        }

		// If there is no next then throw exception
		if (_editBlockIndex >= _editBlocksList.Count - 1)
			throw new LuthetusTextEditorException("No edits are available to perform 'redo' on");

		_editBlockIndex++;
		var redoEdit = _editBlocksList[_editBlockIndex.Value];

		switch (redoEdit.EditKind)
		{
			case TextEditorEditKind.Insert:
				var insertEdit = (TextEditorEditInsert)redoEdit;
				PerformInsert(insertEdit.PositionIndex, insertEdit.Content);
				break;
			case TextEditorEditKind.InsertBatch:
				var insertBatchEdit = (TextEditorEditInsertBatch)redoEdit;
				PerformInsert(insertBatchEdit.PositionIndex, insertBatchEdit.ContentBuilder.ToString());
				break;
			case TextEditorEditKind.Backspace:
				var backspaceEdit = (TextEditorEditBackspace)redoEdit;
				PerformBackspace(backspaceEdit.PositionIndex, backspaceEdit.Count);
				break;
			case TextEditorEditKind.BackspaceBatch:
				var backspaceBatchEdit = (TextEditorEditBackspaceBatch)redoEdit;
				PerformBackspace(backspaceBatchEdit.PositionIndex, backspaceBatchEdit.Count);
				break;
			case TextEditorEditKind.Delete: 
				var deleteEdit = (TextEditorEditDelete)redoEdit;
				PerformDelete(deleteEdit.PositionIndex, deleteEdit.Count);
				break;
			case TextEditorEditKind.DeleteBatch: 
				var deleteBatchEdit = (TextEditorEditDeleteBatch)redoEdit;
				PerformDelete(deleteBatchEdit.PositionIndex, deleteBatchEdit.Count);
				break;
			case TextEditorEditKind.Other:
				while (true)
				{
					if (_editBlockIndex >= _editBlocksList.Count - 1)
					{
						// The 'Redo()' method deals with the next-edit
						// as opposed to the 'Undo()' method that deals with the current-edit
						//
						// Therefore, if there is no 'next-edit' then break out
						break;
					}

					var nextEdit = _editBlocksList[_editBlockIndex.Value + 1];

					if (nextEdit.EditKind == TextEditorEditKind.Other)
					{
						var nextEditOther = (TextEditorEditOther)nextEdit;

						// Regardless of the tag of the next edit. One will need to increment EditIndex.
						_editBlockIndex++;

						if (nextEditOther.Tag == (((TextEditorEditOther)redoEdit).Tag))
							break;
					}
					else
					{
						RedoEdit();
					}
				}
				break;
			default:
				throw new NotImplementedException($"The {nameof(TextEditorEditKind)}: {redoEdit.EditKind} was not recognized.");
		}
	}

    public void SetIsDirtyTrue()
    {
        // Setting _allText to null will clear the 'cache' for the all 'AllText' property.
        _allText = null;
        _isDirty = true;
    }

    public void SetIsDirtyFalse()
    {
        _isDirty = false;
    }

    public void PerformRegisterPresentationModelAction(
    TextEditorPresentationModel presentationModel)
    {
        // Any modified state needs to be 'null coallesce assigned' to the existing TextEditorModel's value. When reading state, if the state had been 'null coallesce assigned' then the field will be read. Otherwise, the existing TextEditorModel's value will be read.
        {
            _presentationModelsList ??= _textEditorModel.PresentationModelList.ToList();
        }

        if (!PresentationModelList.Any(x => x.TextEditorPresentationKey == presentationModel.TextEditorPresentationKey))
            PresentationModelList.Add(presentationModel);
    }

    public void StartPendingCalculatePresentationModel(
        Key<TextEditorPresentationModel> presentationKey,
        TextEditorPresentationModel emptyPresentationModel)
    {
        // Any modified state needs to be 'null coallesce assigned' to the existing TextEditorModel's value. When reading state, if the state had been 'null coallesce assigned' then the field will be read. Otherwise, the existing TextEditorModel's value will be read.
        {
            _presentationModelsList ??= _textEditorModel.PresentationModelList.ToList();
        }

        // If the presentation model has not yet been registered, then this will register it.
        PerformRegisterPresentationModelAction(emptyPresentationModel);

        var indexOfPresentationModel = _presentationModelsList.FindIndex(
            x => x.TextEditorPresentationKey == presentationKey);

        // The presentation model is expected to always be registered at this point.

        var presentationModel = PresentationModelList[indexOfPresentationModel];
        PresentationModelList[indexOfPresentationModel] = presentationModel with
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
            _presentationModelsList ??= _textEditorModel.PresentationModelList.ToList();
        }

        // If the presentation model has not yet been registered, then this will register it.
        PerformRegisterPresentationModelAction(emptyPresentationModel);

        var indexOfPresentationModel = _presentationModelsList.FindIndex(
            x => x.TextEditorPresentationKey == presentationKey);

        // The presentation model is expected to always be registered at this point.

        var presentationModel = PresentationModelList[indexOfPresentationModel];

        if (presentationModel.PendingCalculation is null)
            return;

        var calculation = presentationModel.PendingCalculation with
        {
            TextSpanList = calculatedTextSpans
        };

        PresentationModelList[indexOfPresentationModel] = presentationModel with
        {
            PendingCalculation = null,
            CompletedCalculation = calculation,
        };
    }

    public TextEditorModel ForceRerenderAction()
    {
        return ToModel();
    }

    private void MutateLineEndKindCount(LineEndKind rowEndingKind, int changeBy)
    {
        // Any modified state needs to be 'null coallesce assigned' to the existing TextEditorModel's value. When reading state, if the state had been 'null coallesce assigned' then the field will be read. Otherwise, the existing TextEditorModel's value will be read.
        {
            _lineEndKindCountList ??= _textEditorModel.LineEndKindCountList.ToList();
        }

        var indexOfRowEndingKindCount = _lineEndKindCountList.FindIndex(x => x.lineEndKind == rowEndingKind);
        var currentRowEndingKindCount = LineEndKindCountList[indexOfRowEndingKindCount].count;

        LineEndKindCountList[indexOfRowEndingKindCount] = (rowEndingKind, currentRowEndingKindCount + changeBy);

        CheckRowEndingPositions(false);
    }

    private void CheckRowEndingPositions(bool setUsingRowEndingKind)
    {
        // Any modified state needs to be 'null coallesce assigned' to the existing TextEditorModel's value. When reading state, if the state had been 'null coallesce assigned' then the field will be read. Otherwise, the existing TextEditorModel's value will be read.
        {
            _lineEndKindCountList ??= _textEditorModel.LineEndKindCountList.ToList();
            _onlyLineEndKind ??= _textEditorModel.OnlyLineEndKind;
            _usingLineEndKind ??= _textEditorModel.LineEndKindPreference;
        }

        var existingRowEndingsList = LineEndKindCountList
            .Where(x => x.count > 0)
            .ToArray();

        if (!existingRowEndingsList.Any())
        {
            _onlyLineEndKind = LineEndKind.Unset;
            _usingLineEndKind = LineEndKind.LineFeed;
        }
        else
        {
            if (existingRowEndingsList.Length == 1)
            {
                var rowEndingKind = existingRowEndingsList.Single().lineEndKind;

                if (setUsingRowEndingKind)
                    _usingLineEndKind = rowEndingKind;

                _onlyLineEndKind = rowEndingKind;
            }
            else
            {
                if (setUsingRowEndingKind)
                    _usingLineEndKind = existingRowEndingsList.MaxBy(x => x.count).lineEndKind;

                _onlyLineEndKind = null;
            }
        }
    }
}