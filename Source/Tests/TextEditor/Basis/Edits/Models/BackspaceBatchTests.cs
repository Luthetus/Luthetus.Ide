using Luthetus.TextEditor.RazorLib.Exceptions;
using Luthetus.TextEditor.RazorLib.Edits.Models;

namespace Luthetus.TextEditor.Tests.Basis.Edits.Models;

public class BackspaceBatchTests
{
	[Fact]
	public void Backspace_Batches()
	{
		// Construct
		var textEditor = new OptimizeTextEditor("abc123Do-Re-Mi");

		var lengthAbc = "abc".Length;
		var length123 = "123".Length;
		var lengthDoReMi = "Do-Re-Mi".Length;

		// Backspace One (normal delete)
		textEditor.Backspace(textEditor.AllText.Length, lengthDoReMi);
		Assert.Equal("abc123", textEditor.AllText);
		Assert.Equal(2, textEditor.EditList.Count);
		Assert.Equal(1, textEditor.EditIndex);

		// Backspace Two (cause a batch to be created)
		textEditor.Backspace(textEditor.AllText.Length, length123);
		Assert.Equal("abc", textEditor.AllText);
		Assert.Equal(2, textEditor.EditList.Count);
		Assert.Equal(1, textEditor.EditIndex);

		// Backspace Three (add to existing batch)
		textEditor.Backspace(textEditor.AllText.Length, lengthAbc);
		Assert.Equal(string.Empty, textEditor.AllText);
		Assert.Equal(2, textEditor.EditList.Count);
		Assert.Equal(1, textEditor.EditIndex);

		// undo, undo, redo, redo
		
		// Undo One
		textEditor.Undo();
		Assert.Equal("abc123Do-Re-Mi", textEditor.AllText);
		Assert.Equal(2, textEditor.EditList.Count);
		Assert.Equal(0, textEditor.EditIndex);

		// Undo Two
		Assert.Throws<LuthetusTextEditorException>(textEditor.Undo);

		// Redo One
		textEditor.Redo();
		Assert.Equal(string.Empty, textEditor.AllText);
		Assert.Equal(2, textEditor.EditList.Count);
		Assert.Equal(1, textEditor.EditIndex);

		// Redo Two
		Assert.Throws<LuthetusTextEditorException>(textEditor.Redo);
	}
}
