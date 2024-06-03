using System.Text;

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
	}

	public readonly List<ITextEditorEdit> EditList = new();

	private readonly StringBuilder _content = new();

	public string AllText => _content.ToString();

	public void Insert(int positionIndex, string content)
	{
		PerformInsert(positionIndex, content);
		EditList.Add(new TextEditorEditInsert(positionIndex, content));
	}

	public void Backspace(int positionIndex, int count)
	{
		PerformBackspace(positionIndex, count);
		EditList.Add(new TextEditorEditBackspace(positionIndex, count));
	}

	public void Delete(int positionIndex, int count)
	{
		PerformDelete(positionIndex, count);
		EditList.Add(new TextEditorEditDelete(positionIndex, count));
	}

	public void Undo()
	{
		var mostRecentEdit = EditList[EditList.Count - 1];
		EditList.RemoveAt(EditList.Count - 1);

		var undoEdit = (TextEditorEditDelete)mostRecentEdit.ToUndo();
		PerformDelete(undoEdit.PositionIndex, undoEdit.Count);

		switch (undoEdit.EditKind)
		{
			case TextEditorEditKind.Insert:
				PerformInsert(((TextEditorEditInsert)undoEdit).PositionIndex, ((TextEditorEditInsert)undoEdit).Content);
				break;
			case TextEditorEditKind.Backspace: 
				PerformBackspace(((TextEditorEditBackspace)undoEdit.PositionIndex), ((TextEditorEditBackspace)undoEdit.Count));
			case TextEditorEditKind.Delete: 
				PerformDelete(((TextEditorEditDelete)undoEdit.PositionIndex), ((TextEditorEditDelete)undoEdit.Count));
			case TextEditorEditKind.Other: 
				throw new NotImplementedException("TODO: Handle {nameof(TextEditorEditKind)}.{edit.EditKind}");
			default:
				throw new NotImplementedException($"The {nameof(TextEditorEditKind)}: {edit.EditKind} was not recognized.");
		}
	}

	private void PerformInsert(int positionIndex, string content)
	{
		_content.Insert(positionIndex, content);
	}

	private void PerformBackspace(int positionIndex, int count)
	{
		positionIndex = positionIndex - count;
		
		if (positionIndex < 0)
		{
			var underflow = Math.Abs(positionIndex);
			positionIndex = 0;
			count -= underflow;
		}

		_content.Remove(positionIndex, count);
	}

	private void PerformDelete(int positionIndex, int count)
	{
		_content.Remove(positionIndex, count);
	}
}
