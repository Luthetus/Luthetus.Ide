using Luthetus.TextEditor.RazorLib.Exceptions;
using Luthetus.TextEditor.Tests.Basis.Edits.Models.OptimizeEditBlockLib;

namespace Luthetus.TextEditor.Tests.Basis.Edits.Models;

public class OptimizeEditDeleteBatch
{
	[Fact]
	public void Delete_Batches()
	{
		// Construct
		var textEditor = new OptimizeTextEditor("abc123Do-Re-Mi");

		var lengthAbc = "abc".Length;
		var length123 = "123".Length;
		var lengthDoReMi = "Do-Re-Mi".Length;

		// Delete One (normal delete)
		textEditor.Delete(0, lengthAbc);
		Assert.Equal("123Do-Re-Mi", textEditor.AllText);
		Assert.Equal(2, textEditor.EditList.Count);
		Assert.Equal(1, textEditor.EditIndex);

		// Delete Two (cause a batch to be created)
		textEditor.Delete(0, length123);
		Assert.Equal("Do-Re-Mi", textEditor.AllText);
		Assert.Equal(2, textEditor.EditList.Count);
		Assert.Equal(1, textEditor.EditIndex);

		// Delete Three (add to existing batch)
		textEditor.Delete(0, lengthDoReMi);
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
