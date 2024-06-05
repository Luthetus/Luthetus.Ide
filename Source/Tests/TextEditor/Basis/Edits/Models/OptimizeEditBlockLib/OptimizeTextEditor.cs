using System.Text;
using Luthetus.TextEditor.RazorLib.Exceptions;

namespace Luthetus.TextEditor.Tests.Basis.Edits.Models.OptimizeEditBlockLib;

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

	public void Insert(int positionIndex, string content)
	{
		PerformInsert(positionIndex, content);

		var mostRecentEdit = EditList[EditIndex];

		// Each if statement should return if it modifies the edit list.
		//
		// I'm unsure how I feel about 'editListWasModified' because you either return
		// from every if statement, or you set a boolean like this.
		//
		// Yet, here I am doing both.
		// The reason is just an anxious feeling that I should have both, and is not
		// at all backed by evidence. In fact I strongly believe I do not need this boolean.
		// But yeah, TODO: Delete this boolean.
		var editListWasModified = false;

		if (!editListWasModified && mostRecentEdit.EditKind == TextEditorEditKind.Insert)
		{
			var mostRecentEditInsert = (TextEditorEditInsert)mostRecentEdit;

			// Only batch consecutive, and contiguous, insertions.
			if (positionIndex == mostRecentEditInsert.PositionIndex + mostRecentEditInsert.Content.Length)
			{
				var contentBuilder = new StringBuilder();
				contentBuilder.Append(mostRecentEditInsert.Content);
				contentBuilder.Append(content);
	
				var insertBatch = new TextEditorEditInsertBatch(
					mostRecentEditInsert.PositionIndex,
					contentBuilder);
	
				editListWasModified = true;
				EditList[EditIndex] = insertBatch;
				return;
			}
		}
		
		if (!editListWasModified && mostRecentEdit.EditKind == TextEditorEditKind.InsertBatch)
		{
			var mostRecentEditInsertBatch = (TextEditorEditInsertBatch)mostRecentEdit;

			editListWasModified = true;
			mostRecentEditInsertBatch.ContentBuilder.Append(content);
			return;
		}
		
		if (!editListWasModified)
		{
			editListWasModified = true;
			EditList.Add(new TextEditorEditInsert(positionIndex, content));
			EditIndex++;
			return;
		}
	}

	public void Backspace(int positionIndex, int count)
	{
		var editBackspace = new TextEditorEditBackspace(positionIndex, count);
		editBackspace.TextDeleted = PerformBackspace(positionIndex, count);
		EditList.Add(editBackspace);
		EditIndex++;
	}

	public void Delete(int positionIndex, int count)
	{
		var editDelete = new TextEditorEditDelete(positionIndex, count);
		editDelete.TextDeleted = _content.ToString(positionIndex, count);
		PerformDelete(positionIndex, count);
		EditList.Add(editDelete);
		EditIndex++;
	}

	public void Undo()
	{
		if (EditIndex <= 0)
			throw new LuthetusTextEditorException("No edits are available to perform 'undo' on");

		var mostRecentEdit = EditList[EditIndex];
		EditIndex--;
		var undoEdit = mostRecentEdit.ToUndo();

		switch (undoEdit.EditKind)
		{
			case TextEditorEditKind.Insert:
				var insertEdit = (TextEditorEditInsert)undoEdit;
				Console.WriteLine($"{insertEdit.PositionIndex}, '{insertEdit.Content}'");
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
				throw new NotImplementedException("TODO: Handle {nameof(TextEditorEditKind)}.{undoEdit.EditKind}");
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
			case TextEditorEditKind.Delete: 
				var deleteEdit = (TextEditorEditDelete)redoEdit;
				PerformDelete(deleteEdit.PositionIndex, deleteEdit.Count);
				break;
			case TextEditorEditKind.Other: 
				throw new NotImplementedException("TODO: Handle {nameof(TextEditorEditKind)}.{redoEdit.EditKind}");
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
