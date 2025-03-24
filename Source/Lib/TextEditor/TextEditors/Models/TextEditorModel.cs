using System.Text;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Edits.Models;
using Luthetus.TextEditor.RazorLib.Exceptions;
using Luthetus.TextEditor.RazorLib.Rows.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

/// <inheritdoc cref="TextEditorModel"/>
public partial class TextEditorModel
{
	#region TextEditorModelBad
	/// <summary>
	/// (2024-06-08) I belive there are too many ways to edit a text editor. There should only be 'Insert(...)' and 'Remove(...)' methods.
	///              Any code I currently have in 'TextEditorModelModifier.cs' that I deem as technical debt or a bad idea will be put in this file.
	///              Then, once organized I hope to make sense of what the "lean" solution is.
	/// </summary>
	
	public void ClearContent()
    {
        MostCharactersOnASingleLineTuple = (0, TextEditorModel.MOST_CHARACTERS_ON_A_SINGLE_ROW_MARGIN);

        PartitionList = new List<TextEditorPartition> { new TextEditorPartition(new List<RichCharacter>()) };
        _partitionListChanged = true;

        LineEndList = new List<LineEnd> 
        {
            new LineEnd(0, 0, LineEndKind.EndOfFile)
        };

        LineEndKindCountList = new List<(LineEndKind rowEndingKind, int count)>
        {
            (LineEndKind.CarriageReturn, 0),
            (LineEndKind.LineFeed, 0),
            (LineEndKind.CarriageReturnLineFeed, 0),
        };

        TabKeyPositionList = new List<int>();

        SetIsDirtyTrue();
    }

	public void HandleKeyboardEvent(
        KeymapArgs keymapArgs,
        CursorModifierBagTextEditor cursorModifierBag,
        CancellationToken cancellationToken)
    {
        if (KeyboardKeyFacts.IsMetaKey(keymapArgs))
        {
            if (KeyboardKeyFacts.MetaKeys.BACKSPACE == keymapArgs.Key)
            {
                Delete(
                	cursorModifierBag,
                    1,
                    keymapArgs.CtrlKey,
                    DeleteKind.Backspace,
                    cancellationToken);
            }
            else if (KeyboardKeyFacts.MetaKeys.DELETE == keymapArgs.Key)
            {
                Delete(
                    cursorModifierBag,
                    1,
                    keymapArgs.CtrlKey,
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

                var valueToInsert = keymapArgs.Key.First().ToString();

                if (keymapArgs.Code == KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE)
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
                else if (keymapArgs.Code == KeyboardKeyFacts.WhitespaceCodes.TAB_CODE)
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
        var keymapArgs = motionKind switch
        {
            MotionKind.Backspace => new KeymapArgs { Key = KeyboardKeyFacts.MetaKeys.BACKSPACE },
            MotionKind.Delete => new KeymapArgs { Key = KeyboardKeyFacts.MetaKeys.DELETE },
            _ => throw new LuthetusTextEditorException($"The {nameof(MotionKind)}: {motionKind} was not recognized.")
        };

        HandleKeyboardEvent(
            keymapArgs,
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
                    new KeymapArgs
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
        ClearAllStatesButKeepEditHistory();

		if (EditBlockList.Count == 0 && EditBlockIndex == 0)
			EditBlockList.Add(new TextEditorEditConstructor());

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
                    MostCharactersOnASingleLineTuple = (rowIndex, charactersOnRow + TextEditorModel.MOST_CHARACTERS_ON_A_SINGLE_ROW_MARGIN);

                LineEndList.Insert(rowIndex, new(index, index + 1, LineEndKind.CarriageReturn));
                rowIndex++;

                charactersOnRow = 0;

                carriageReturnCount++;
            }
            else if (character == KeyboardKeyFacts.WhitespaceCharacters.NEW_LINE)
            {
                if (charactersOnRow > MostCharactersOnASingleLineTuple.lineLength - TextEditorModel.MOST_CHARACTERS_ON_A_SINGLE_ROW_MARGIN)
                    MostCharactersOnASingleLineTuple = (rowIndex, charactersOnRow + TextEditorModel.MOST_CHARACTERS_ON_A_SINGLE_ROW_MARGIN);

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
                var indexCarriageReturn = LineEndKindCountList.FindIndex(x => x.lineEndKind == LineEndKind.CarriageReturn);
                LineEndKindCountList[indexCarriageReturn] = (LineEndKind.CarriageReturn, carriageReturnCount);
            }
            {
                var indexLineFeed = LineEndKindCountList.FindIndex(x => x.lineEndKind == LineEndKind.LineFeed);
                LineEndKindCountList[indexLineFeed] = (LineEndKind.LineFeed, linefeedCount);
            }
            {
                var indexCarriageReturnLineFeed = LineEndKindCountList.FindIndex(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed);
                LineEndKindCountList[indexCarriageReturnLineFeed] = (LineEndKind.CarriageReturnLineFeed, carriageReturnLinefeedCount);
            }
        }

        // Update the EndOfFile line end.
        {
            var endOfFile = LineEndList[^1];

            if (endOfFile.LineEndKind != LineEndKind.EndOfFile)
                throw new LuthetusTextEditorException($"The text editor model is malformed; the final entry of {nameof(LineEndList)} must be the {nameof(LineEndKind)}.{nameof(LineEndKind.EndOfFile)}");

            LineEndList[^1] = endOfFile with
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
        EditBlockIndex = 0;
        EditBlockList.Clear();
    }

	private void EnsureUndoPoint(ITextEditorEdit newEdit)
	{
		if (EditBlockIndex < EditBlockList.Count - 1)
		{
			// Clear redo history
			for (int i = EditBlockIndex + 1; i < EditBlockList.Count; i++)
			{
				EditBlockList.RemoveAt(i);
			}
		}

		if (newEdit.EditKind == TextEditorEditKind.Insert)
		{
			var mostRecentEdit = EditBlockList[EditBlockIndex];

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
		
					EditBlockList[EditBlockIndex] = insertBatch;
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
				EditBlockList.Add(new TextEditorEditInsert(positionIndex, content));
				EditBlockIndex++;
				return;
			}
		}
		else if (newEdit.EditKind == TextEditorEditKind.Backspace)
		{
			var mostRecentEdit = EditBlockList[EditBlockIndex];

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
	
					EditBlockList[EditBlockIndex] = editBackspaceBatch;
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
				EditBlockList.Add(editBackspace);
				EditBlockIndex++;
				return;
			}
		}
		else if (newEdit.EditKind == TextEditorEditKind.Delete)
		{
			var mostRecentEdit = EditBlockList[EditBlockIndex];

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
	
					EditBlockList[EditBlockIndex] = editDeleteBatch;
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
				EditBlockList.Add(editDelete);
				EditBlockIndex++;
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
		EditBlockList.Add(editOther);
		EditBlockIndex++;
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
		EditBlockList.Add(pop);
		EditBlockIndex++;
	}

	public void UndoEdit()
	{
		if (EditBlockIndex <= 0)
			throw new LuthetusTextEditorException("No edits are available to perform 'undo' on");

		var mostRecentEdit = EditBlockList[EditBlockIndex];
		var undoEdit = mostRecentEdit.ToUndo();
		
		// In case the 'ToUndo(...)' throws an exception, the decrement to the EditIndex
		// is being done only after a successful ToUndo(...)
		EditBlockIndex--;

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
					if (EditBlockIndex == 0)
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

					mostRecentEdit = EditBlockList[EditBlockIndex];

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
							EditBlockIndex--;
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
		// If there is no next then throw exception
		if (EditBlockIndex >= EditBlockList.Count - 1)
			throw new LuthetusTextEditorException("No edits are available to perform 'redo' on");

		EditBlockIndex++;
		var redoEdit = EditBlockList[EditBlockIndex];

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
					if (EditBlockIndex >= EditBlockList.Count - 1)
					{
						// The 'Redo()' method deals with the next-edit
						// as opposed to the 'Undo()' method that deals with the current-edit
						//
						// Therefore, if there is no 'next-edit' then break out
						break;
					}

					var nextEdit = EditBlockList[EditBlockIndex + 1];

					if (nextEdit.EditKind == TextEditorEditKind.Other)
					{
						var nextEditOther = (TextEditorEditOther)nextEdit;

						// Regardless of the tag of the next edit. One will need to increment EditIndex.
						EditBlockIndex++;

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
	#endregion
	
	#region TextEditorModelEditMethods
	/// <summary>
	/// Any modified state needs to be 'null coallesce assigned' to the existing TextEditorModel's value
	///
	/// When reading state, if the state had been 'null coallesce assigned' then the field will
	/// be read. Otherwise, the existing TextEditorModel's value will be read.
	/// <br/><br/>
	/// <inheritdoc cref="TextEditorModel"/>
	/// </summary>

	public void ClearOnlyRowEndingKind()
    {
        OnlyLineEndKind = LineEndKind.Unset;
    }

    public void SetLineEndKindPreference(LineEndKind rowEndingKind)
    {
        LineEndKindPreference = rowEndingKind;
    }

    public void SetResourceData(ResourceUri resourceUri, DateTime resourceLastWriteTime)
    {
        ResourceUri = resourceUri;
        ResourceLastWriteTime = resourceLastWriteTime;
    }

    public void SetDecorationMapper(IDecorationMapper decorationMapper)
    {
        DecorationMapper = decorationMapper;
    }

    public void SetCompilerService(ICompilerService compilerService)
    {
        CompilerService = compilerService;
    }

    public void SetTextEditorSaveFileHelper(SaveFileHelper textEditorSaveFileHelper)
    {
        TextEditorSaveFileHelper = textEditorSaveFileHelper;
    }

    public void ClearAllStatesButKeepEditHistory()
    {
        ClearContent();
        ClearOnlyRowEndingKind();
        SetLineEndKindPreference(LineEndKind.Unset);
    }     

    public void SetIsDirtyTrue()
    {
        // Setting _allText to null will clear the 'cache' for the all 'AllText' property.
        _allText = null;
        IsDirty = true;
    }

    public void SetIsDirtyFalse()
    {
        IsDirty = false;
    }

    public void PerformRegisterPresentationModelAction(
    TextEditorPresentationModel presentationModel)
    {
        if (!PresentationModelList.Any(x => x.TextEditorPresentationKey == presentationModel.TextEditorPresentationKey))
            PresentationModelList.Add(presentationModel);
    }

    public void StartPendingCalculatePresentationModel(
        Key<TextEditorPresentationModel> presentationKey,
        TextEditorPresentationModel emptyPresentationModel)
    {
        // If the presentation model has not yet been registered, then this will register it.
        PerformRegisterPresentationModelAction(emptyPresentationModel);

        var indexOfPresentationModel = PresentationModelList.FindIndex(
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
        List<TextEditorTextSpan> calculatedTextSpans)
    {
        // If the presentation model has not yet been registered, then this will register it.
        PerformRegisterPresentationModelAction(emptyPresentationModel);

        var indexOfPresentationModel = PresentationModelList.FindIndex(
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
        var indexOfRowEndingKindCount = LineEndKindCountList.FindIndex(x => x.lineEndKind == rowEndingKind);
        var currentRowEndingKindCount = LineEndKindCountList[indexOfRowEndingKindCount].count;

        LineEndKindCountList[indexOfRowEndingKindCount] = (rowEndingKind, currentRowEndingKindCount + changeBy);

        CheckRowEndingPositions(false);
    }

    private void CheckRowEndingPositions(bool setUsingRowEndingKind)
    {
        var existingRowEndingsList = LineEndKindCountList
            .Where(x => x.count > 0)
            .ToArray();

        if (!existingRowEndingsList.Any())
        {
            OnlyLineEndKind = LineEndKind.Unset;
            LineEndKindPreference = LineEndKind.LineFeed;
        }
        else
        {
            if (existingRowEndingsList.Length == 1)
            {
                var rowEndingKind = existingRowEndingsList.Single().lineEndKind;

                if (setUsingRowEndingKind)
                    LineEndKindPreference = rowEndingKind;

                OnlyLineEndKind = rowEndingKind;
            }
            else
            {
                if (setUsingRowEndingKind)
                    LineEndKindPreference = existingRowEndingsList.MaxBy(x => x.count).lineEndKind;

                OnlyLineEndKind = LineEndKind.Unset;
            }
        }
    }
    #endregion
    
    #region TextEditorModelInProgress
	/// <summary>
	/// (2024-06-08) I've been dogfooding the IDE, and the 'TextEditorModelModifier.cs' file demonstrates some clear issues regarding text editor optimization.
	///              Im breaking up the 80,000 character file a bit here into partial classes for now. TODO: merge the partial classes back?
	/// </summary>

	/// <param name="useLineEndKindPreference">
    /// If false, then the string will be inserted as is.
    /// If true, then the string will have its line endings replaced with the <see cref="LineEndKindPreference"/>
    /// </param>
    public void Insert(
        string value,
        CursorModifierBagTextEditor cursorModifierBag,
        bool useLineEndKindPreference = true,
        CancellationToken cancellationToken = default,
		bool shouldCreateEditHistory = true)
    {
        for (var cursorIndex = cursorModifierBag.List.Count - 1; cursorIndex >= 0; cursorIndex--)
        {
            var cursorModifier = cursorModifierBag.List[cursorIndex];

            if (TextEditorSelectionHelper.HasSelectedText(cursorModifier))
            {
                Delete(
					// TODO: 'cursorModifierBag' is not the correcy parameter here...
					//       ...one needs to create a new cursor modifier bag which contains the single cursor that is being looked at. 
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

			if (shouldCreateEditHistory)
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

                MostCharactersOnASingleLineTuple = localMostCharactersOnASingleRowTuple;
            }
        }

        SetIsDirtyTrue();
		ShouldReloadVirtualizationResult = true;
    }

	private string InsertMetadata(
        string value,
        TextEditorCursorModifier cursorModifier,
        bool useLineEndKindPreference,
        CancellationToken cancellationToken)
    {
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
                        tabPositionLazyInsertRange.index = TabKeyPositionList.FindIndex(x => x >= initialCursorPositionIndex);

                        if (tabPositionLazyInsertRange.index == -1)
                            tabPositionLazyInsertRange.index = TabKeyPositionList.Count;
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

                LineEndList[i] = rowEndingTuple with
                {
                    StartPositionIndexInclusive = rowEndingTuple.StartPositionIndexInclusive + lineEndingsChangedValueBuilder.Length,
                    EndPositionIndexExclusive = rowEndingTuple.EndPositionIndexExclusive + lineEndingsChangedValueBuilder.Length,
                };
            }
        }

        // Reposition the Tabs
        {
            var firstTabKeyPositionIndexToModify = TabKeyPositionList.FindIndex(x => x >= initialCursorPositionIndex);

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
                ResourceUri.Empty,
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
                LineEndList.InsertRange(
                    lineEndPositionLazyInsertRange.index.Value,
                    lineEndPositionLazyInsertRange.localLineEndList);
            }

            if (tabPositionLazyInsertRange.index is not null)
            {
                TabKeyPositionList.InsertRange(
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
        if (cursorPositionIndex > CharCount)
            return;

        __InsertRange(
            cursorPositionIndex,
            value.Select(character => new RichCharacter(character, 0)));
    }

	/// <summary>
    /// This method allows for a "RemoveRange" like operation on the text editor's contents.
    /// Any meta-data will automatically be updated (e.g. <see cref="TextEditorModel.LineEndKindCountList"/>.
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
        CancellationToken cancellationToken = default,
		bool shouldCreateEditHistory = true)
	{
        if (columnCount < 0)
            throw new LuthetusTextEditorException($"{nameof(columnCount)} < 0");

        for (var cursorIndex = cursorModifierBag.List.Count - 1; cursorIndex >= 0; cursorIndex--)
        {
            var cursorModifier = cursorModifierBag.List[cursorIndex];

			var initialPositionIndex = this.GetPositionIndex(cursorModifier);

            var tuple = DeleteMetadata(columnCount, cursorModifier, expandWord, deleteKind, cancellationToken);

            if (tuple is null)
            {
                SetIsDirtyTrue();
                return;
			}

            var (positionIndex, charCount) = tuple.Value;

			var textRemoved = this.GetString(positionIndex, charCount);

            DeleteValue(positionIndex, charCount, cancellationToken);

			if (shouldCreateEditHistory)
			{
				if (deleteKind == DeleteKind.Delete)
					EnsureUndoPoint(new TextEditorEditDelete(positionIndex, charCount) { TextRemoved = textRemoved });
				else if (deleteKind == DeleteKind.Backspace)
					EnsureUndoPoint(new TextEditorEditBackspace(initialPositionIndex, charCount) { TextRemoved = textRemoved });
				else
					throw new NotImplementedException($"The {nameof(DeleteKind)}: {deleteKind} was not recognized.");
			}

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

                MostCharactersOnASingleLineTuple = localMostCharactersOnASingleLineTuple;
            }
        }

        SetIsDirtyTrue();
		ShouldReloadVirtualizationResult = true;
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

                var richCharacterToDelete = RichCharacterList[toDeletePositionIndex];

                if (KeyboardKeyFacts.IsLineEndingCharacter(richCharacterToDelete.Value))
                {
                    // A delete is a contiguous operation. Therefore, all that is needed to update the LineEndList
                    // is a starting index, and a count.
                    var indexLineEnd = LineEndList.FindIndex(
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
                        var indexTabKey = TabKeyPositionList.FindIndex(
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

                var richCharacterToDelete = RichCharacterList[toDeletePositionIndex];

                if (KeyboardKeyFacts.IsLineEndingCharacter(richCharacterToDelete.Value))
                {
                    // A delete is a contiguous operation. Therefore, all that is needed to update the LineEndList
                    // is a starting index, and a count.
                    var indexLineEnd = LineEndList.FindIndex(
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
                        var indexTabKey = TabKeyPositionList.FindIndex(
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

                LineEndList[i] = lineEnd with
                {
                    StartPositionIndexInclusive = lineEnd.StartPositionIndexInclusive - charCount,
                    EndPositionIndexExclusive = lineEnd.EndPositionIndexExclusive - charCount,
                };
            }
        }

        // Reposition the Tab(s)
        {
            var firstTabKeyPositionIndexToModify = TabKeyPositionList.FindIndex(x => x >= positionIndex);

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
                ResourceUri.Empty,
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
                LineEndList.RemoveRange(
                    lineEndPositionLazyRemoveRange.index.Value,
                    lineEndPositionLazyRemoveRange.count);
            }

            if (tabPositionLazyRemoveRange.index is not null)
            {
                TabKeyPositionList.RemoveRange(
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
        if (positionIndex >= CharCount)
            return;

        __RemoveRange(positionIndex, count);
	}
	#endregion
	
	#region TextEditorModelMain
    public TextEditorModel(
        ResourceUri resourceUri,
        DateTime resourceLastWriteTime,
        string fileExtension,
        string content,
        IDecorationMapper? decorationMapper,
        ICompilerService? compilerService,
		int partitionSize = 4_096)
    {
        ResourceUri = resourceUri;
        ResourceLastWriteTime = resourceLastWriteTime;
        FileExtension = fileExtension;
        DecorationMapper = decorationMapper ?? new TextEditorDecorationMapperDefault();
        CompilerService = compilerService ?? new CompilerServiceDoNothing();

		PartitionSize = partitionSize;
		var modifier = new TextEditorModel(this, __AllText);
		modifier.SetContent(content);

		__AllText = modifier.AllText;
        RichCharacterList = modifier.RichCharacterList;
        PartitionList = modifier.PartitionList;
        LineEndKindCountList = modifier.LineEndKindCountList;
		LineEndList = modifier.LineEndList;
		TabKeyPositionList = modifier.TabKeyPositionList;
		OnlyLineEndKind = modifier.OnlyLineEndKind;
		LineEndKindPreference = modifier.LineEndKindPreference;
		MostCharactersOnASingleLineTuple = modifier.MostCharactersOnASingleLineTuple;
		EditBlockList = modifier.EditBlockList;
		EditBlockIndex = modifier.EditBlockIndex;
	}

	public TextEditorModel(
        string allText,
        RichCharacter[] richCharacterList,
        int partitionSize,
        List<TextEditorPartition> partitionList,
		List<ITextEditorEdit> editBlocksList,
		List<LineEnd> rowEndingPositionsList,
		List<(LineEndKind rowEndingKind, int count)> rowEndingKindCountsList,
		List<TextEditorPresentationModel> presentationModelsList,
		List<int> tabKeyPositionsList,
		LineEndKind onlyRowEndingKind,
		LineEndKind usingRowEndingKind,
		ResourceUri resourceUri,
		DateTime resourceLastWriteTime,
		string fileExtension,
		IDecorationMapper decorationMapper,
		ICompilerService compilerService,
		SaveFileHelper textEditorSaveFileHelper,
		int editBlockIndex,
        bool isDirty,
        (int rowIndex, int rowLength) mostCharactersOnASingleRowTuple,
		Key<RenderState>  renderStateKey)
	{
		__AllText = allText;
        RichCharacterList = richCharacterList;
        PartitionSize = partitionSize;
        PartitionList = partitionList;
		EditBlockList = editBlocksList;
		LineEndList = rowEndingPositionsList;
		LineEndKindCountList = rowEndingKindCountsList;
		PresentationModelList = presentationModelsList;
		TabKeyPositionList = tabKeyPositionsList;
		OnlyLineEndKind = onlyRowEndingKind;
		LineEndKindPreference = usingRowEndingKind;
		ResourceUri = resourceUri;
		ResourceLastWriteTime = resourceLastWriteTime;
		FileExtension = fileExtension;
		DecorationMapper = decorationMapper;
		CompilerService = compilerService;
		TextEditorSaveFileHelper = textEditorSaveFileHelper;
		EditBlockIndex = editBlockIndex;
        IsDirty = isDirty;
		MostCharactersOnASingleLineTuple = mostCharactersOnASingleRowTuple;
		RenderStateKey = renderStateKey;
	}
	#endregion
	
	#region TextEditorModelMain2
	/// <summary>
	/// Any modified state needs to be 'null coallesce assigned' to the existing TextEditorModel's value
	///
	/// When reading state, if the state had been 'null coallesce assigned' then the field will
	/// be read. Otherwise, the existing TextEditorModel's value will be read.
	/// <br/><br/>
	/// <inheritdoc cref="TextEditorModel"/>
	/// </summary>

    /// <summary>
    /// <see cref="__SplitIntoTwoPartitions(int)"/> will divide by 2 and give the first split the remainder,
    /// then add 1 to the first split if there is a multibyte scenario.
    /// Therefore partition size of 3 would infinitely try to split itself.
    /// </summary>
    public const int MINIMUM_PARTITION_SIZE = 4;

	/// <summary>
	/// The first time a model is constructed it will throw an exception when accessing AllText,
	/// therefore pass it in as an argument.
	/// </summary>
    public TextEditorModel(TextEditorModel model, string? allText)
    {
        if (model.PartitionSize < MINIMUM_PARTITION_SIZE)
            throw new LuthetusTextEditorException($"{nameof(model)}.{nameof(PartitionSize)} must be >= {MINIMUM_PARTITION_SIZE}");

        PartitionSize = model.PartitionSize;
        WasDirty = model.IsDirty;

        IsDirty = model.IsDirty;

        _partitionList = model.PartitionList;
        _richCharacterList = model.RichCharacterList;
        
        EditBlockList = model.EditBlockList;
	    LineEndList = model.LineEndList;
	    LineEndKindCountList = model.LineEndKindCountList;
	    PresentationModelList = model.PresentationModelList;
	    TabKeyPositionList = model.TabKeyPositionList;
        
        OnlyLineEndKind = model.OnlyLineEndKind;
	    LineEndKindPreference = model.LineEndKindPreference;
	    ResourceUri = model.ResourceUri;
	    ResourceLastWriteTime = model.ResourceLastWriteTime;
	    FileExtension = model.FileExtension;
	    DecorationMapper = model.DecorationMapper;
	    CompilerService = model.CompilerService;
	    TextEditorSaveFileHelper = model.TextEditorSaveFileHelper;
	    EditBlockIndex = model.EditBlockIndex;
	    IsDirty = model.IsDirty;
	    MostCharactersOnASingleLineTuple = model.MostCharactersOnASingleLineTuple;
	    _allText = allText;
	    RenderStateKey = Key<RenderState>.NewKey();
	    
	    PreviousLineCount = model.LineEndList.Count;
    }

    public RichCharacter[] _richCharacterList;
    public RichCharacter[] RichCharacterList
    {
    	get
    	{
    		if (_partitionListChanged)
    		{
    			_partitionListChanged = false;
    			_richCharacterList = PartitionList.SelectMany(x => x.RichCharacterList).ToArray();
    		}
    		
    		return _richCharacterList;
    	}
    	set
    	{
    		_partitionListChanged = false;
    		_richCharacterList = value;
    	}
    }
    
    private bool _partitionListChanged;
    private bool _partitionListIsShallowCopy = false;
    public List<TextEditorPartition> _partitionList;
    public List<TextEditorPartition> PartitionList
    {
    	get
    	{
    		return _partitionList;
    	}
    	set
    	{
    		_partitionListChanged = true;
    		_partitionList = value;
    	}
    }

    public List<ITextEditorEdit> EditBlockList { get; set; }
    public List<LineEnd> LineEndList { get; set; }
    public List<(LineEndKind lineEndKind, int count)> LineEndKindCountList { get; set; }
    public List<TextEditorPresentationModel> PresentationModelList { get; set; }
    public List<int> TabKeyPositionList { get; set; }
    public LineEndKind OnlyLineEndKind { get; set; }
    public LineEndKind LineEndKindPreference { get; set; }
    public ResourceUri ResourceUri { get; set; }
    public DateTime ResourceLastWriteTime { get; set; }
    public string FileExtension { get; set; }
    public IDecorationMapper DecorationMapper { get; set; }
    public ICompilerService CompilerService { get; set; }
    public SaveFileHelper TextEditorSaveFileHelper { get; set; }
    public int EditBlockIndex { get; set; }
    public bool IsDirty { get; set; }
    public (int lineIndex, int lineLength) MostCharactersOnASingleLineTuple { get; set; }
    public (int lineIndex, int lineLength) PreviousMostCharactersOnASingleLineTuple { get; set; }
    public Key<RenderState> RenderStateKey { get; set; }

    public int LineCount => LineEndList.Count;
    public int PreviousLineCount { get; set; }
    
    // TODO: Remove Linq?
    public int CharCount => PartitionList.Sum(x => x.Count);

	/// <summary>
	/// The <see cref="TextEditorEditOther"/> works by invoking 'Open' then when finished invoking 'Close'.
	/// </summary>
	public Stack<TextEditorEditOther> OtherEditStack { get; } = new();

    /// <summary>
    /// This property optimizes the dirty state tracking. If _wasDirty != _isDirty then track the state change.
    /// This involves writing to dependency injectable state, then triggering a re-render in the <see cref="Edits.Displays.DirtyResourceUriInteractiveIconDisplay"/>
    /// </summary>
    public bool WasDirty { get; }

    private int PartitionSize { get; }

	/// <summary>
	/// This property decides whether or not to replace the existing model in IState<TextEditorState> with
	/// the instance that comes from this modifier.
	/// </summary>
    public bool WasModified { get; internal set; }
	
	/// <summary>
	/// This property decides whether or not to re-calculate the virtualization result that gets displayed on the UI.
	/// </summary>
    public bool ShouldReloadVirtualizationResult { get; internal set; }

	private string _allText;
    public string AllText => _allText ??= new string(RichCharacterList.Select(x => x.Value).ToArray());

    public TextEditorModel ToModel()
    {
        return new TextEditorModel(
            AllText,
            RichCharacterList,
            PartitionSize,
            PartitionList,
            EditBlockList,
            LineEndList,
            LineEndKindCountList,
            PresentationModelList,
            TabKeyPositionList,
            OnlyLineEndKind,
            LineEndKindPreference,
            ResourceUri,
            ResourceLastWriteTime,
            FileExtension,
            DecorationMapper,
            CompilerService,
            TextEditorSaveFileHelper,
            EditBlockIndex,
            IsDirty,
            MostCharactersOnASingleLineTuple,
            RenderStateKey);
    }

    public enum DeleteKind
    {
        Backspace,
        Delete,
    }
    #endregion
    
    #region TextEditorModelMethods
    /// <summary>
	/// Returns the Length of a line however it does not include the line ending characters by default.
	/// To include line ending characters the parameter <see cref="includeLineEndingCharacters" /> must be true.
	/// </summary>
    public static int GetLineLength(this TextEditorModel model, int lineIndex, bool includeLineEndingCharacters = false)
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
    public static RichCharacter[][] GetLineRichCharacterRange(this TextEditorModel model, int startingLineIndex, int count)
    {
        var lineCountAvailable = model.LineEndList.Count - startingLineIndex;

        var lineCountToReturn = count < lineCountAvailable
            ? count
            : lineCountAvailable;

        var endingLineIndexExclusive = startingLineIndex + lineCountToReturn;
        var lineList = new RichCharacter[lineCountToReturn][];

        if (lineCountToReturn < 0 || startingLineIndex < 0 || endingLineIndexExclusive < 0)
            return lineList;

		var addIndex = 0;

        for (var i = startingLineIndex; i < endingLineIndexExclusive; i++)
        {
            // Previous line's end-position-exclusive is this row's start.
            var startOfLineInclusive = model.GetLineInformation(i).StartPositionIndexInclusive;
            var endOfLineExclusive = model.LineEndList[i].EndPositionIndexExclusive;

			// TODO: LINQ used in a hot path (the virtualization invokes this method)
            var line = model.RichCharacterList
                .Skip(startOfLineInclusive)
                .Take(endOfLineExclusive - startOfLineInclusive)
                .ToArray();

            lineList[addIndex++] = line;
        }

        return lineList;
    }

    public static int GetTabCountOnSameLineBeforeCursor(this TextEditorModel model, int lineIndex, int columnIndex)
    {
        var line = model.GetLineInformation(lineIndex);

        model.AssertColumnIndex(line, columnIndex);
        
        var count = 0;
        var foundSpan = false;
        
        foreach (var positionIndex in model.TabKeyPositionList)
        {
        	if (!foundSpan && positionIndex < line.StartPositionIndexInclusive)
        		continue;
        	else
        		foundSpan = true;
        		
        	if (positionIndex < line.StartPositionIndexInclusive + columnIndex)
        		count++;
        	else
        		break;
        }
        
        return count;
    }

    /// <summary>
    /// Implementations of this method are expected to have caching.
    /// </summary>
    public static string GetAllText(this TextEditorModel model)
    {
        return model.AllText;
    }

    public static int GetPositionIndex(this TextEditorModel model, TextEditorCursor cursor)
    {
        return model.GetPositionIndex(cursor.LineIndex, cursor.ColumnIndex);
    }

    public static int GetPositionIndex(this TextEditorModel model, TextEditorCursorModifier cursorModifier)
    {
        return model.GetPositionIndex(cursorModifier.LineIndex, cursorModifier.ColumnIndex);
    }

    public static int GetPositionIndex(this TextEditorModel model, int lineIndex, int columnIndex)
    {
        var line = model.GetLineInformation(lineIndex);

        model.AssertColumnIndex(line, columnIndex);

        return line.StartPositionIndexInclusive + columnIndex;
    }

    public static (int lineIndex, int columnIndex) GetLineAndColumnIndicesFromPositionIndex(
        this TextEditorModel model,
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
    public static char GetCharacter(this TextEditorModel model, int positionIndex)
    {
        model.AssertPositionIndex(positionIndex);

        if (positionIndex == model.CharCount)
            return ParserFacts.END_OF_FILE;

        return model.RichCharacterList[positionIndex].Value;
    }

    /// <summary>
    /// To receive a <see cref="char"/> value, one can use <see cref="GetCharacter"/> instead.
    /// </summary>
    public static string GetString(this TextEditorModel model, int positionIndex, int count)
    {
        model.AssertPositionIndex(positionIndex);
        model.AssertCount(count);

        return new string(model.RichCharacterList
            .Skip(positionIndex)
            .Take(count)
            .Select(x => x.Value)
            .ToArray());
    }

    public static string GetLineTextRange(this TextEditorModel model, int lineIndex, int count)
    {
        model.AssertCount(count);

        var startPositionIndexInclusive = model.GetPositionIndex(lineIndex, 0);
        var lastLineIndexExclusive = lineIndex + count;
        int endPositionIndexExclusive;

        if (lastLineIndexExclusive > model.LineCount - 1)
        {
            endPositionIndexExclusive = model.CharCount;
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
    public static TextEditorTextSpan? GetWordTextSpan(this TextEditorModel model, int positionIndex)
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

    public static List<TextEditorTextSpan> FindMatches(this TextEditorModel model, string query)
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

        return matchedTextSpans;
    }

    /// <summary>
    /// 'lineIndex' equal to '0' returns the first line.<br/><br/>
    /// 
    /// An index for <see cref="TextEditorModel.LineEndList"/> maps 1 to 1 with this method.
    /// (i.e.) the 0th line-end index will end up returning the 0th line.<br/><br/>
    /// 
    /// Given a 'lineIndex', return the <see cref="LineEnd"/> at <see cref="TextEditorModel.LineEndList"/>[lineIndex - 1],
    /// and <see cref="TextEditorModel.LineEndList"/>[lineIndex]
    /// in the form of the type <see cref="LineInformation"/>.
    /// </summary>
    /// <remarks>
    /// When 'lineIndex' == 0, then a "made-up" line ending named <see cref="LineEnd.StartOfFile"/> will be used
    /// in place of indexing at '<see cref="TextEditorModel.LineEndList"/>[-1]'
    /// </remarks>
    public static LineInformation GetLineInformation(this TextEditorModel model, int lineIndex)
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

    public static LineInformation GetLineInformationFromPositionIndex(this TextEditorModel model, int positionIndex)
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
        this TextEditorModel model,
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

        if (positionIndex < 0 || positionIndex >= model.RichCharacterList.Length)
            return -1;

        var startCharacterKind = CharacterKindHelper.CharToCharacterKind(model.RichCharacterList[positionIndex].Value);

        while (true)
        {
            if (positionIndex >= model.RichCharacterList.Length ||
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

    public static bool CanUndoEdit(this TextEditorModel model)
    {
        return model.EditBlockIndex > 0;
    }

    public static bool CanRedoEdit(this TextEditorModel model)
    {
        return model.EditBlockIndex < model.EditBlockList.Count - 1;
    }

    public static CharacterKind GetCharacterKind(this TextEditorModel model, int positionIndex)
    {
        model.AssertPositionIndex(positionIndex);

        if (positionIndex == model.CharCount)
            return CharacterKind.Bad;

        return CharacterKindHelper.CharToCharacterKind(model.RichCharacterList[positionIndex].Value);
    }

    /// <summary>
    /// This method and <see cref="ReadNextWordOrDefault(TextEditorModel, int, int)"/>
    /// are separate because of 'Ctrl + Space' bring up autocomplete when at a period.
    /// </summary>
    public static string? ReadPreviousWordOrDefault(
        this TextEditorModel model,
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
    /// This method and <see cref="ReadPreviousWordOrDefault(TextEditorModel, int, int, bool)"/>
    /// are separate because of 'Ctrl + Space' bring up autocomplete when at a period.
    /// </summary>
    public static string? ReadNextWordOrDefault(this TextEditorModel model, int lineIndex, int columnIndex)
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
    public static string GetTextOffsettingCursor(this TextEditorModel model, TextEditorCursor textEditorCursor)
    {
        var cursorPositionIndex = model.GetPositionIndex(textEditorCursor);
        var lineStartPositionIndexInclusive = model.GetLineInformation(textEditorCursor.LineIndex).StartPositionIndexInclusive;

        return model.GetString(lineStartPositionIndexInclusive, cursorPositionIndex - lineStartPositionIndexInclusive);
    }

    public static string GetLineText(this TextEditorModel model, int lineIndex)
    {
        var lineStartPositionIndexInclusive = model.GetLineInformation(lineIndex).StartPositionIndexInclusive;
        var lengthOfLine = model.GetLineLength(lineIndex, true);

        return model.GetString(lineStartPositionIndexInclusive, lengthOfLine);
    }

    public static void AssertColumnIndex(this TextEditorModel model, LineInformation line, int columnIndex)
    {
        if (columnIndex < 0)
            throw new LuthetusTextEditorException($"'{nameof(columnIndex)}:{columnIndex}' < 0");
        
        if (columnIndex > line.LastValidColumnIndex)
            throw new LuthetusTextEditorException($"'{nameof(columnIndex)}:{columnIndex}' > {nameof(line)}.{nameof(line.LastValidColumnIndex)}:{line.LastValidColumnIndex}");
    }
    
    public static void AssertLineIndex(this TextEditorModel model, int lineIndex)
    {
        if (lineIndex < 0)
            throw new LuthetusTextEditorException($"'{nameof(lineIndex)}:{lineIndex}' < 0");
        
        if (lineIndex >= model.LineCount)
            throw new LuthetusTextEditorException($"'{nameof(lineIndex)}:{lineIndex}' >= {nameof(model)}.{nameof(model.LineCount)}:{model.LineCount}");
    }

    public static void AssertPositionIndex(this TextEditorModel model, int positionIndex)
    {
        if (positionIndex < 0)
            throw new LuthetusTextEditorException($"'{nameof(positionIndex)}:{positionIndex}' < 0");
        
        // NOTE: model.DocumentLength is a valid position for the cursor to be at.
        if (positionIndex > model.CharCount)
            throw new LuthetusTextEditorException($"'{nameof(positionIndex)}:{positionIndex}' > {nameof(model)}.{nameof(model.CharCount)}:{model.CharCount}");
    }
    
    public static void AssertCount(this TextEditorModel model, int count)
    {
        if (count < 0)
            throw new LuthetusTextEditorException($"'{nameof(count)}:{count}' < 0");
    }
    #endregion
    
    #region TextEditorModelPartitions
    public void __Insert(int globalPositionIndex, RichCharacter richCharacter)
    {
        int indexOfPartitionWithAvailableSpace = -1;
        int relativePositionIndex = -1;
        var runningCount = 0;

        for (int i = 0; i < PartitionList.Count; i++)
        {
            TextEditorPartition? partition = PartitionList[i];

            if (runningCount + partition.Count >= globalPositionIndex)
            {
                // This is the partition we want to modify.
                // But, we must first check if it has available space.
                if (partition.Count >= PartitionSize)
                {
                    __SplitIntoTwoPartitions(i);
                    i--;
                    continue;
                }

                relativePositionIndex = globalPositionIndex - runningCount;
                indexOfPartitionWithAvailableSpace = i;
                break;
            }
            else
            {
                runningCount += partition.Count;
            }
        }

        if (indexOfPartitionWithAvailableSpace == -1)
            throw new LuthetusTextEditorException("if (indexOfPartitionWithAvailableSpace == -1)");

        if (relativePositionIndex == -1)
            throw new LuthetusTextEditorException("if (relativePositionIndex == -1)");

        var inPartition = PartitionList[indexOfPartitionWithAvailableSpace];
        var outPartition = inPartition.Insert(relativePositionIndex, richCharacter);

        PartitionListSetItem(
            indexOfPartitionWithAvailableSpace,
            outPartition);
    }

    /// <summary>
    /// If either character or decoration byte are 'null', then the respective
    /// collection will be left unchanged.
    /// 
    /// i.e.: to change ONLY a character value invoke this method with decorationByte set to null,
    ///       and only the <see cref="CharList"/> will be changed.
    /// </summary>
    public void __SetItem(
        int globalPositionIndex,
        RichCharacter richCharacter)
    {
        int indexOfPartitionWithAvailableSpace = -1;
        int relativePositionIndex = -1;
        var runningCount = 0;

        for (int i = 0; i < PartitionList.Count; i++)
        {
            TextEditorPartition? partition = PartitionList[i];

            if (runningCount + partition.Count > globalPositionIndex)
            {
                // This is the partition we want to modify.
                relativePositionIndex = globalPositionIndex - runningCount;
                indexOfPartitionWithAvailableSpace = i;
                break;
            }
            else
            {
                runningCount += partition.Count;
            }
        }

        if (indexOfPartitionWithAvailableSpace == -1)
            throw new LuthetusTextEditorException("if (indexOfPartitionWithAvailableSpace == -1)");

        if (relativePositionIndex == -1)
            throw new LuthetusTextEditorException("if (relativePositionIndex == -1)");

        var inPartition = PartitionList[indexOfPartitionWithAvailableSpace];
        var outPartition = inPartition.SetItem(relativePositionIndex, richCharacter);

        PartitionListSetItem(
            indexOfPartitionWithAvailableSpace,
            outPartition);
    }

    public void __SetDecorationByte(
        int globalPositionIndex,
        byte decorationByte)
    {
        int indexOfPartitionWithAvailableSpace = -1;
        int relativePositionIndex = -1;
        var runningCount = 0;

        for (int i = 0; i < PartitionList.Count; i++)
        {
            TextEditorPartition? partition = PartitionList[i];

            if (runningCount + partition.Count > globalPositionIndex)
            {
                // This is the partition we want to modify.
                relativePositionIndex = globalPositionIndex - runningCount;
                indexOfPartitionWithAvailableSpace = i;
                break;
            }
            else
            {
                runningCount += partition.Count;
            }
        }

        if (indexOfPartitionWithAvailableSpace == -1)
            throw new LuthetusTextEditorException("if (indexOfPartitionWithAvailableSpace == -1)");

        if (relativePositionIndex == -1)
            throw new LuthetusTextEditorException("if (relativePositionIndex == -1)");

        var inPartition = PartitionList[indexOfPartitionWithAvailableSpace];
        var targetRichCharacter = inPartition.RichCharacterList[relativePositionIndex];
        
        inPartition.RichCharacterList[relativePositionIndex] = new(
        	targetRichCharacter.Value,
        	decorationByte);
        _partitionListChanged = true;
    }

    public void __RemoveAt(int globalPositionIndex)
    {
        if (globalPositionIndex >= CharCount)
            return;

        int indexOfPartitionWithContent = -1;
        int relativePositionIndex = -1;
        var runningCount = 0;

        for (int i = 0; i < PartitionList.Count; i++)
        {
            TextEditorPartition? partition = PartitionList[i];

            if (runningCount + partition.Count > globalPositionIndex)
            {
                // This is the partition we want to modify.
                relativePositionIndex = globalPositionIndex - runningCount;
                indexOfPartitionWithContent = i;
                break;
            }
            else
            {
                runningCount += partition.Count;
            }
        }

        if (indexOfPartitionWithContent == -1)
            throw new LuthetusTextEditorException("if (indexOfPartitionWithContent == -1)");

        if (relativePositionIndex == -1)
            throw new LuthetusTextEditorException("if (relativePositionIndex == -1)");

        var inPartition = PartitionList[indexOfPartitionWithContent];
        var outPartition = inPartition.RemoveAt(relativePositionIndex);

        PartitionListSetItem(
            indexOfPartitionWithContent,
            outPartition);
    }

    private void __InsertNewPartition(int partitionIndex)
    {
        PartitionListInsert(partitionIndex, new TextEditorPartition(new List<RichCharacter>()));
    }

    private void __SplitIntoTwoPartitions(int partitionIndex)
    {
        var originalPartition = PartitionList[partitionIndex];

        var firstUnevenSplit = PartitionSize / 2 + PartitionSize % 2;
        var secondUnevenSplit = PartitionSize / 2;

        // Validate multi-byte characters go on same partition (i.e.: '\r\n')
        {
            // firstUnevenSplit is a count so -1 to make it an index
            if (originalPartition.RichCharacterList[firstUnevenSplit - 1].Value == '\r')
            {
                if (originalPartition.RichCharacterList[(firstUnevenSplit - 1) + 1].Value == '\n')
                {
                    firstUnevenSplit += 1;
                    secondUnevenSplit -= 1;
                }
            }

            // TODO: If the partition to split ends in '\r' and the cause for the split
            //       is to create space in order to insert a '\n',
            //       |
            //       Then this works out as a "happy accident" of sorts.
            //       This is not ideal, it should be more concrete than "oops it worked".
            //       |
            //       The reason it works is because a split won't check if the next partition
            //       has space (? source needed) and will always move the '\r' to the new partition,
            //       then return to the insert and put the '\n' immediately after.

            // One of the reasons for not having a multi-byte character span multiple partitions,
            // is that if a partition has capacity 4,096 but a count of 2,048,
            // one cannot insert between the bytes of a multi-byte character
            // so the first partition with only half its capacity used, would be unable to be used
            // any further than 2,048 because it would mean writing between its multi-byte character
            // than spans into the next partition.
        }

        // Replace old
        {
            var partition = new TextEditorPartition(originalPartition.RichCharacterList
                .Skip(0)
                .Take(firstUnevenSplit)
                .ToList());

            PartitionListSetItem(
                partitionIndex,
                partition);
        }

        // Insert new
        {
            var partition = new TextEditorPartition(originalPartition.RichCharacterList
                .Skip(firstUnevenSplit)
                .Take(secondUnevenSplit)
                .ToList());

            PartitionListInsert(
                partitionIndex + 1,
                partition);
        }
    }

    public void __InsertRange(int globalPositionIndex, IEnumerable<RichCharacter> richCharacterList)
    {
        var richCharacterEnumerator = richCharacterList.GetEnumerator();

        while (richCharacterEnumerator.MoveNext())
        {
            int indexOfPartitionWithAvailableSpace = -1;
            int relativePositionIndex = -1;
            var runningCount = 0;
            TextEditorPartition? partition;

            for (int i = 0; i < PartitionList.Count; i++)
            {
                partition = PartitionList[i];

                if (runningCount + partition.Count >= globalPositionIndex)
                {
                    if (partition.Count >= PartitionSize)
                    {
                        __SplitIntoTwoPartitions(i);
                        i--;
                        continue;
                    }

                    relativePositionIndex = globalPositionIndex - runningCount;
                    indexOfPartitionWithAvailableSpace = i;
                    break;
                }
                else
                {
                    runningCount += partition.Count;
                }
            }

            if (indexOfPartitionWithAvailableSpace == -1)
                throw new LuthetusTextEditorException("if (indexOfPartitionWithAvailableSpace == -1)");

            if (relativePositionIndex == -1)
                throw new LuthetusTextEditorException("if (relativePositionIndex == -1)");

            partition = PartitionList[indexOfPartitionWithAvailableSpace];
            var partitionAvailableSpace = PartitionSize - partition.Count;

            var richCharacterBatchInsertList = new List<RichCharacter> { richCharacterEnumerator.Current };

            while (richCharacterBatchInsertList.Count < partitionAvailableSpace && richCharacterEnumerator.MoveNext())
            {
                richCharacterBatchInsertList.Add(richCharacterEnumerator.Current);
            }

            var inPartition = PartitionList[indexOfPartitionWithAvailableSpace];
            var outPartition = inPartition.InsertRange(relativePositionIndex, richCharacterBatchInsertList);

            PartitionListSetItem(
                indexOfPartitionWithAvailableSpace,
                outPartition);

            globalPositionIndex += richCharacterBatchInsertList.Count;
        }
    }

    public void __RemoveRange(int targetGlobalPositionIndex, int targetDeleteCount)
    {
        var foundTargetGlobalPositionIndex = false;
        // The 'searchGlobalPositionIndex' is no longer updated after 'foundTargetGlobalPositionIndex' is true
        // It is just being used to find where the data that should be deleted starts.
        var searchGlobalPositionIndex = 0;
        var runningDeleteCount = 0;

        for (int partitionIndex = 0; partitionIndex < PartitionList.Count; partitionIndex++)
        {
            var partition = PartitionList[partitionIndex];
            var relativePositionIndex = 0;

            if (!foundTargetGlobalPositionIndex)
            {
                // It is '>' specifically, because '0 + partition.Count' is a count, therefore the
                // largest index that could exist in the partition is 1 less than the partition.Count.
                if (searchGlobalPositionIndex + partition.Count > targetGlobalPositionIndex)
                {
                    foundTargetGlobalPositionIndex = true;
                    relativePositionIndex = targetGlobalPositionIndex - searchGlobalPositionIndex;
                }
                else
                {
                    searchGlobalPositionIndex += partition.Count;
                    continue;
                }
            }

            // This section of code is dependent on the condition branch above it having performed
            // a 'continue' if it was entered, but still didn't find the 'targetGlobalPositionIndex'

            var availableDeletes = partition.Count - relativePositionIndex;
            var remainingDeletes = targetDeleteCount - runningDeleteCount;

            var deletes = availableDeletes < remainingDeletes
                ? availableDeletes
                : remainingDeletes;

            // WARNING: The code does not currently alter the _partitionList in any way other than this 'SetItem'...
            //          ...invocation, with regards to this method.
            //          If one adds other alterations to the _partitionList in this method,
            //          check if this logic would break.
            PartitionListSetItem(
                partitionIndex,
                partition.RemoveRange(relativePositionIndex, deletes));

            runningDeleteCount += deletes;

            if (runningDeleteCount >= targetDeleteCount)
                break;
        }
    }

    public void __Add(RichCharacter richCharacter)
    {
        __Insert(CharCount, richCharacter);
    }
    
    public void PartitionListSetItem(int index, TextEditorPartition partition)
    {
    	if (!_partitionListIsShallowCopy)
    	{
    		_partitionListIsShallowCopy = true;
    		PartitionList = new(PartitionList);
    	}
    	
    	PartitionList[index] = partition;
    	_partitionListChanged = true;
    }
    
    public void PartitionListInsert(int index, TextEditorPartition partition)
    {
    	if (!_partitionListIsShallowCopy)
    	{
    		_partitionListIsShallowCopy = true;
    		PartitionList = new(PartitionList);
    	}
    	
    	PartitionList.Insert(index, partition);
    	_partitionListChanged = true;
    }
    #endregion
    
    #region TextEditorModelVariables
    public const int TAB_WIDTH = 4;
    public const int GUTTER_PADDING_LEFT_IN_PIXELS = 5;
    public const int GUTTER_PADDING_RIGHT_IN_PIXELS = 15;
    public const int MAXIMUM_EDIT_BLOCKS = 10;
    public const int MOST_CHARACTERS_ON_A_SINGLE_ROW_MARGIN = 5;

    public string? __AllText;

    public RichCharacter[] RichCharacterList { get; init; }
    public List<TextEditorPartition> PartitionList { get; init; } = new List<TextEditorPartition> { new TextEditorPartition(new List<RichCharacter>()) };

    public List<ITextEditorEdit> EditBlockList { get; init; } = new();

    /// <inheritdoc cref="TextEditorModel.LineEndList"/>
	public List<LineEnd> LineEndList { get; init; } = new();
	public List<(LineEndKind lineEndKind, int count)> LineEndKindCountList { get; init; } = new();
	public List<TextEditorPresentationModel> PresentationModelList { get; init; } = new();
    
    /// <inheritdoc cref="TextEditorModel.TabKeyPositionList"/>
	public List<int> TabKeyPositionList { get; } = new();
    
    /// <inheritdoc cref="TextEditorModel.OnlyLineEndKind"/>
    public LineEndKind OnlyLineEndKind { get; init; }
    public LineEndKind LineEndKindPreference { get; init; }
    
    /// <inheritdoc cref="TextEditorModel.ResourceUri"/>
    public ResourceUri ResourceUri { get; init; }
    public DateTime ResourceLastWriteTime { get; init; }
    public int PartitionSize { get; init; }
    
    /// <inheritdoc cref="TextEditorModel.FileExtension"/>
    public string FileExtension { get; init; }
    public IDecorationMapper DecorationMapper { get; init; }
    public ICompilerService CompilerService { get; init; }
    public SaveFileHelper TextEditorSaveFileHelper { get; init; } = new();
    public int EditBlockIndex { get; init; }
    public bool IsDirty { get; init; }
	public (int lineIndex, int lineLength) MostCharactersOnASingleLineTuple { get; init; }
    public Key<RenderState> RenderStateKey { get; init; } = Key<RenderState>.NewKey();

    public string AllText => __AllText ??= new string (RichCharacterList.Select(x => x.Value).ToArray());

    public int LineCount => LineEndList.Count;
    public int DocumentLength => RichCharacterList.Length;
    
    RichCharacter[] TextEditorModel.RichCharacterList => RichCharacterList;

    int TextEditorModel.LineCount => LineCount;
    int TextEditorModel.CharCount => DocumentLength;
    #endregion
}
