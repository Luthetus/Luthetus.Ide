namespace Luthetus.TextEditor.Tests.Basis.Edits.Models.OptimizeEditBlockLib;

public struct TextEditorEditConstructor : ITextEditorEdit
{
	public string Tag { get; }

	public TextEditorEditKind EditKind => TextEditorEditKind.Constructor;
}
