using Luthetus.TextEditor.RazorLib.Exceptions;
using Luthetus.TextEditor.Tests.Basis.Edits.Models.OptimizeEditBlockLib;

namespace Luthetus.TextEditor.Tests.Basis.Edits.Models;

public class InsertBatchTests
{
	[Fact]
	public void Insert_Batches()
	{
		// Construct
		var textEditor = new OptimizeTextEditor();

		// Insert One (normal insert)
		textEditor.Insert(0, "abc");
		Assert.Equal("abc", textEditor.AllText);
		Assert.Equal(2, textEditor.EditList.Count);
		Assert.Equal(1, textEditor.EditIndex);

		// Insert Two (cause a batch to be created)
		textEditor.Insert(textEditor.AllText.Length, "123");
		Assert.Equal("abc123", textEditor.AllText);
		Assert.Equal(2, textEditor.EditList.Count);
		Assert.Equal(1, textEditor.EditIndex);

		// Insert Three (add to existing batch)
		textEditor.Insert(textEditor.AllText.Length, "Do-Re-Mi");
		Assert.Equal("abc123Do-Re-Mi", textEditor.AllText);
		Assert.Equal(2, textEditor.EditList.Count);
		Assert.Equal(1, textEditor.EditIndex);

		// undo, undo, redo, redo
		
		// Undo One
		textEditor.Undo();
		Assert.Equal(string.Empty, textEditor.AllText);
		Assert.Equal(2, textEditor.EditList.Count);
		Assert.Equal(0, textEditor.EditIndex);

		// Undo Two
		Assert.Throws<LuthetusTextEditorException>(textEditor.Undo);

		// Redo One
		textEditor.Redo();
		Assert.Equal("abc123Do-Re-Mi", textEditor.AllText);
		Assert.Equal(2, textEditor.EditList.Count);
		Assert.Equal(1, textEditor.EditIndex);

		// Redo Two
		Assert.Throws<LuthetusTextEditorException>(textEditor.Redo);
	}
}
