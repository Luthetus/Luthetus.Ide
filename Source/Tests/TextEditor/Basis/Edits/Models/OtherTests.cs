using Luthetus.TextEditor.RazorLib.Exceptions;
using Luthetus.TextEditor.Tests.Basis.Edits.Models.OptimizeEditBlockLib;

namespace Luthetus.TextEditor.Tests.Basis.Edits.Models;

/// The wording 'other' in this class is
/// in reference to <see cref="TextEditorEditKind.Other"/>
public class OtherTests
{
	[Fact]
	public void Paste()
	{
		// Construct
		var textEditor = new OptimizeTextEditor();

		var otherEdit = new TextEditorEditOther("paste");

		// Paste
		textEditor.OpenOtherEdit(otherEdit);

		var clipboardContent = "abc123";
		textEditor.Insert(0, clipboardContent);

		textEditor.CloseOtherEdit(otherEdit.Tag);

		Assert.Equal("abc123", textEditor.AllText);
		Assert.Equal(4, textEditor.EditList.Count);
		Assert.Equal(3, textEditor.EditIndex);

		textEditor.Undo();
		Assert.Equal(string.Empty, textEditor.AllText);
		Assert.Equal(4, textEditor.EditList.Count);
		Assert.Equal(0, textEditor.EditIndex);

		textEditor.Redo();
		Assert.Equal(4, textEditor.EditList.Count);
		Assert.Equal(3, textEditor.EditIndex);

		Assert.Equal("abc123", textEditor.AllText);

	}
}
