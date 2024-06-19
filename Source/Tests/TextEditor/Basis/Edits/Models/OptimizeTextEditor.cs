using System.Text;
using Luthetus.TextEditor.RazorLib.Exceptions;
using Luthetus.TextEditor.RazorLib.Edits.Models;

namespace Luthetus.TextEditor.Tests.Basis.Edits.Models;

public class OptimizeTextEditor
{
	public OptimizeTextEditor()
		: this(string.Empty)
	{
	}

	public OptimizeTextEditor(string initialContent)
	{
		_content.Append(initialContent);
		EditList.Add(new TextEditorEditConstructor());
	}

    /// <inheritdoc cref="EditIndex"/>
	public List<ITextEditorEdit> EditList { get; } = new();

	/// <summary>
	/// The <see cref="TextEditorEditOther"/> works by invoking 'Open' then when finished invoking 'Close'.
	/// </summary>
	public Stack<TextEditorEditOther> OtherEditStack { get; } = new();

	/// <summary>
	/// <see cref="EditIndex"/> identifies the index within <see cref="EditList"/> that the text editor is synced with.
	/// i.e.: the most recently applied edit is <see cref="EditList"/>[<see cref="EditIndex"/>]
	/// </summary>
	/// <remarks>
	/// If <see cref="EditIndex"/> == 0 && <see cref="EditList"/>.Count == 0:
	///     Then: a 'redo' is NOT-available AND an 'undo' is NOT-available <br/><br/>
	///
	/// If <see cref="EditIndex"/> &lt; <see cref="EditList"/>.Count:
	///     Then: a 'redo' is available <br/><br/>
	///	
	/// If <see cref="EditIndex"/> == <see cref="EditList"/>.Count - 1 && <see cref="EditIndex"/> != 0:
	///     Then: an 'undo' is available <br/><br/>
	///
	/// If <see cref="EditIndex"/> &gt;= <see cref="EditList"/>.Count:
	///     Then: an index out of bounds exception should be thrown
	/// </remarks>
	public int EditIndex { get; private set; }

	private readonly StringBuilder _content = new();

	public string AllText => _content.ToString();

	private void EnsureUndoPoint(ITextEditorEdit newEdit)
	{
		if (mostRecentEdit.EditKind == TextEditorEditKind.Insert)
		{
			var mostRecentEdit = EditList[EditIndex];

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
		
					EditList[EditIndex] = insertBatch;
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
				EditList.Add(new TextEditorEditInsert(positionIndex, content));
				EditIndex++;
				return;
			}
		}
		else if (mostRecentEdit.EditKind == TextEditorEditKind.Backspace)
		{
			var mostRecentEdit = EditList[EditIndex];

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
	
					EditList[EditIndex] = editBackspaceBatch;
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
				EditList.Add(editBackspace);
				EditIndex++;
				return;
			}
		}
		else if (mostRecentEdit.EditKind == TextEditorEditKind.Delete)
		{
			var mostRecentEdit = EditList[EditIndex];

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
	
					EditList[EditIndex] = editDeleteBatch;
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
				EditList.Add(editDelete);
				EditIndex++;
				return;
			}
		}
	}

	public void Insert(int positionIndex, string content)
	{
		PerformInsert(positionIndex, content);
		EnsureUndoPoint(new TextEditorEditInsert(positionIndex, content));
	}

	public void Backspace(int positionIndex, int count)
	{
		var textRemoved = PerformBackspace(positionIndex, count);
		EnsureUndoPoint(new TextEditorEditBackspace(positionIndex, count));
	}

	public void Delete(int positionIndex, int count)
	{
		var textRemoved = _content.ToString(positionIndex, count);
		PerformDelete(positionIndex, count);
		EnsureUndoPoint(new TextEditorEditDelete(positionIndex, count));
	}

	public void OpenOtherEdit(TextEditorEditOther editOther)
	{
		OtherEditStack.Push(editOther);
		EditList.Add(editOther);
		EditIndex++;
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
		EditList.Add(pop);
		EditIndex++;
	}

	public void Undo()
	{
		if (EditIndex <= 0)
			throw new LuthetusTextEditorException("No edits are available to perform 'undo' on");

		var mostRecentEdit = EditList[EditIndex];
		var undoEdit = mostRecentEdit.ToUndo();
		
		// In case the 'ToUndo(...)' throws an exception, the decrement to the EditIndex
		// is being done only after a successful ToUndo(...)
		EditIndex--;

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
					if (EditIndex == 0)
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

					mostRecentEdit = EditList[EditIndex];

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
							EditIndex--;
							break;
						}
					}
					else
					{
						Undo();
					}
				}
				break;
			default:
				throw new NotImplementedException($"The {nameof(TextEditorEditKind)}: {undoEdit.EditKind} was not recognized.");
		}
	}

	public void Redo()
	{
		// If there is no next then throw exception
		if (EditIndex >= EditList.Count - 1)
			throw new LuthetusTextEditorException("No edits are available to perform 'redo' on");

		EditIndex++;
		var redoEdit = EditList[EditIndex];

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
					if (EditIndex >= EditList.Count - 1)
					{
						// The 'Redo()' method deals with the next-edit
						// as opposed to the 'Undo()' method that deals with the current-edit
						//
						// Therefore, if there is no 'next-edit' then break out
						break;
					}

					var nextEdit = EditList[EditIndex + 1];

					if (nextEdit.EditKind == TextEditorEditKind.Other)
					{
						var nextEditOther = (TextEditorEditOther)nextEdit;

						// Regardless of the tag of the next edit. One will need to increment EditIndex.
						EditIndex++;

						if (nextEditOther.Tag == (((TextEditorEditOther)redoEdit).Tag))
							break;
					}
					else
					{
						Redo();
					}
				}
				break;
			default:
				throw new NotImplementedException($"The {nameof(TextEditorEditKind)}: {redoEdit.EditKind} was not recognized.");
		}
	}

	private void PerformInsert(int positionIndex, string content)
	{
		_content.Insert(positionIndex, content);
	}

	private string PerformBackspace(int positionIndex, int count)
	{
		positionIndex = positionIndex - count;
		
		if (positionIndex < 0)
		{
			var underflow = Math.Abs(positionIndex);
			positionIndex = 0;
			count -= underflow;
		}

		var toBeDeletedText = _content.ToString(positionIndex, count);
		_content.Remove(positionIndex, count);
		return toBeDeletedText;
	}

	private void PerformDelete(int positionIndex, int count)
	{
		_content.Remove(positionIndex, count);
	}
}
