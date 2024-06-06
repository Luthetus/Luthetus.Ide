namespace Luthetus.TextEditor.Tests.Basis.Edits.Models.OptimizeEditBlockLib;

public struct TextEditorEditOther : ITextEditorEdit
{
	public TextEditorEditOther(string tag)
	{
		Tag = tag;
	}

	public string Tag { get; }

	public TextEditorEditKind EditKind => TextEditorEditKind.Other;
}