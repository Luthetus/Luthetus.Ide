using System.Collections.Immutable;
using System.Text;
using Microsoft.AspNetCore.Components.Web;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Edits.Models;
using Luthetus.TextEditor.RazorLib.Exceptions;
using Luthetus.TextEditor.RazorLib.Rows.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

/// <summary>
/// (2024-06-08) I belive there are too many ways to edit a text editor. There should only be 'Insert(...)' and 'Remove(...)' methods.
///              Any code I currently have in 'TextEditorModelModifier.cs' that I deem as technical debt or a bad idea will be put in this file.
///              Then, once organized I hope to make sense of what the "lean" solution is.
/// </summary>
public partial class TextEditorModelModifier
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

        _partitionList = new TextEditorPartition[] { new TextEditorPartition(new List<RichCharacter>()) }.ToImmutableList();

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
				{
                    valueToInsert = LineEndKindPreference.AsCharacters();
					
					// GOAL: Match indentation on newline keystroke (2024-07-04)
					var line = this.GetLineInformation(cursor.LineIndex);

					var cursorPositionIndex = line.StartPositionIndexInclusive + cursor.ColumnIndex;
					var indentationPositionIndex = line.StartPositionIndexInclusive;

					var indentationBuilder = new StringBuilder();

					while (indentationPositionIndex < cursorPositionIndex)
					{
						var possibleIndentationChar = RichCharacterList[indentationPositionIndex++].Value;

						if (possibleIndentationChar == '\t' || possibleIndentationChar == ' ')
							indentationBuilder.Append(possibleIndentationChar);
						else
							break;
					}

					valueToInsert += indentationBuilder.ToString();
				}
                else if (keyboardEventArgs.Code == KeyboardKeyFacts.WhitespaceCodes.TAB_CODE)
				{
                    valueToInsert = "\t";
				}

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
	
		Insert(content, cursorModifierBag, false, CancellationToken.None, shouldCreateEditHistory: false);
	}

	private void PerformBackspace(int positionIndex, int count)
	{
		var (lineIndex, columnIndex) = this.GetLineAndColumnIndicesFromPositionIndex(positionIndex);
		var cursor = new TextEditorCursor(lineIndex, columnIndex, true);
		var cursorModifierBag = new CursorModifierBagTextEditor(
			Key<TextEditorViewModel>.Empty,
			new List<TextEditorCursorModifier> { new(cursor) });

		Delete(
			cursorModifierBag,
			count,
			false,
			DeleteKind.Backspace,
			CancellationToken.None,
			shouldCreateEditHistory: false);
	}

	private void PerformDelete(int positionIndex, int count)
	{
		var (lineIndex, columnIndex) = this.GetLineAndColumnIndicesFromPositionIndex(positionIndex);
		var cursor = new TextEditorCursor(lineIndex, columnIndex, true);
		var cursorModifierBag = new CursorModifierBagTextEditor(
			Key<TextEditorViewModel>.Empty,
			new List<TextEditorCursorModifier> { new(cursor) });

		Delete(
			cursorModifierBag,
			count,
			false,
			DeleteKind.Delete,
			CancellationToken.None,
			shouldCreateEditHistory: false);
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

	public void SetContent(string content)
    {
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

                    LineEndList[rowIndex - 1] = lineEnding with
					{
						EndPositionIndexExclusive = lineEnding.EndPositionIndexExclusive + 1,
                        LineEndKind = LineEndKind.CarriageReturnLineFeed
					};

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

        __InsertRange(0, content.Select(x => new RichCharacter(x, default)));

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

            _lineEndList[^1] = endOfFile with
			{
				StartPositionIndexInclusive = content.Length,
				EndPositionIndexExclusive = content.Length,
			};
        }

        CheckRowEndingPositions(true);
        SetIsDirtyTrue();
		ShouldReloadVirtualizationResult = true;
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

		if (_editBlockIndex.Value < _editBlocksList.Count - 1)
		{
			// Clear redo history
			for (int i = _editBlockIndex.Value + 1; i < _editBlocksList.Count; i++)
			{
				_editBlocksList.RemoveAt(i);
			}
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
		// Any modified state needs to be 'null coallesce assigned' to the existing TextEditorModel's value. When reading state, if the state had been 'null coallesce assigned' then the field will be read. Otherwise, the existing TextEditorModel's value will be read.
        {
            _editBlocksList ??= _textEditorModel.EditBlockList.ToList();
            _editBlockIndex ??= _textEditorModel.EditBlockIndex;
        }

		OtherEditStack.Push(editOther);
		_editBlocksList.Add(editOther);
		_editBlockIndex++;
	}

	public void CloseOtherEdit(string predictedTag)
	{
		// Any modified state needs to be 'null coallesce assigned' to the existing TextEditorModel's value. When reading state, if the state had been 'null coallesce assigned' then the field will be read. Otherwise, the existing TextEditorModel's value will be read.
        {
            _editBlocksList ??= _textEditorModel.EditBlockList.ToList();
            _editBlockIndex ??= _textEditorModel.EditBlockIndex;
        }

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
}
