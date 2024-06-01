using System.Text;

namespace Luthetus.TextEditor.Tests.Basis.Edits.Models.OptimizeEditBlockLib;

public enum TextEditorEditKind
{
	Insert,
	Backspace,
	Delete,
	Other,
}

public interface ITextEditorEdit
{
	public TextEditorEditKind EditKind { get; }
}

public struct TextEditorEditInsert : ITextEditorEdit
{
	public TextEditorEditInsert(int positionIndex, string content)
	{
		PositionIndex = positionIndex;
		Content = content;
	}

	public int PositionIndex { get; }
	public string Content { get; }

	public TextEditorEditKind EditKind => TextEditorEditKind.Insert;
}

public struct TextEditorEditBackspace : ITextEditorEdit
{
	public TextEditorEditBackspace(int positionIndex, int count)
	{
		PositionIndex = positionIndex;
		Count = count;
	}

	public int PositionIndex { get; }
	public int Count { get; }

	public TextEditorEditKind EditKind => TextEditorEditKind.Backspace;
}

public struct TextEditorEditDelete : ITextEditorEdit
{
	public TextEditorEditDelete(int positionIndex, int count)
	{
		PositionIndex = positionIndex;
		Count = count;
	}

	public int PositionIndex { get; }
	public int Count { get; }

	public TextEditorEditKind EditKind => TextEditorEditKind.Delete;
}

public struct TextEditorEditOther : ITextEditorEdit
{
	public string Tag { get; }

	public TextEditorEditKind EditKind => TextEditorEditKind.Other;
}

// public class TextEditorEditGroup
// {
// 	public TextEditorEditGroup(TextEditorEditKind editKind)
// 	{
// 		TextEditorEditKind = editKind;
// 	}
// 
// 	public TextEditorEditKind TextEditorEditKind { get; }
// 	public List<ITextEditorEdit> EditList { get; } = new();
// }

public class OptimizeTextEditor
{
	public readonly List<ITextEditorEdit> EditGroupList = new();

	private readonly StringBuilder _content = new();

	public string AllText => _content.ToString();

	public void Insert(int positionIndex, string content)
	{
		PerformInsert(positionIndex, content);
		EditGroupList.Add(new TextEditorEditInsert(positionIndex, content));
	}

	public void Backspace(int positionIndex, int count)
	{
		PerformBackspace(positionIndex, count);
		EditGroupList.Add(new TextEditorEditBackspace(positionIndex, count));
	}

	public void Delete(int positionIndex, int count)
	{
		PerformDelete(positionIndex, count);
		EditGroupList.Add(new TextEditorEditDelete(positionIndex, count));
	}

	private void PerformInsert(int positionIndex, string content)
	{
		_content.Insert(positionIndex, content);
	}

	private void PerformBackspace(int positionIndex, int count)
	{
		var positionIndex = positionIndex - count;
		
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
