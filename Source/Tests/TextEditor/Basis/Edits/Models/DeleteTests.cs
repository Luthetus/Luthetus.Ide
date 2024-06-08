using Luthetus.TextEditor.RazorLib.Exceptions;
using Luthetus.TextEditor.RazorLib.Edits.Models;

namespace Luthetus.TextEditor.Tests.Basis.Edits.Models;

public class DeleteTests
{
	[Fact]
	public void Delete_Case_One()
	{
		// Construct
		var textEditor = new OptimizeTextEditor("AbcHello");

		var lengthHello = "Hello".Length;
		var lengthAbc = "Abc".Length;

		// Delete One
		textEditor.Delete(3, lengthHello);
		Assert.Equal("Abc", textEditor.AllText);
		Assert.Equal(2, textEditor.EditList.Count);
		Assert.Equal(1, textEditor.EditIndex);

		// Delete Two		
		textEditor.Delete(0, lengthAbc);
		Assert.Equal(string.Empty, textEditor.AllText);
		Assert.Equal(3, textEditor.EditList.Count);
		Assert.Equal(2, textEditor.EditIndex);

		// undo, undo, redo, redo
		
		// Undo One
		textEditor.Undo();
		Assert.Equal("Abc", textEditor.AllText);
		Assert.Equal(3, textEditor.EditList.Count);
		Assert.Equal(1, textEditor.EditIndex);

		// Undo Two
		textEditor.Undo();
		Assert.Equal("AbcHello", textEditor.AllText);
		Assert.Equal(3, textEditor.EditList.Count);
		Assert.Equal(0, textEditor.EditIndex);

		// Redo One
		textEditor.Redo();
		Assert.Equal("Abc", textEditor.AllText);
		Assert.Equal(3, textEditor.EditList.Count);
		Assert.Equal(1, textEditor.EditIndex);

		// Redo Two
		textEditor.Redo();
		Assert.Equal(string.Empty, textEditor.AllText);
		Assert.Equal(3, textEditor.EditList.Count);
		Assert.Equal(2, textEditor.EditIndex);
	}

	[Fact]
	public void Delete_Case_Two()
	{
		// Construct
		var textEditor = new OptimizeTextEditor("AbcHello");

		var lengthHello = "Hello".Length;
		var lengthAbc = "Abc".Length;

		// Delete One
		textEditor.Delete(3, lengthHello);
		Assert.Equal("Abc", textEditor.AllText);
		Assert.Equal(2, textEditor.EditList.Count);
		Assert.Equal(1, textEditor.EditIndex);

		// Delete Two		
		textEditor.Delete(0, lengthAbc);
		Assert.Equal(string.Empty, textEditor.AllText);
		Assert.Equal(3, textEditor.EditList.Count);
		Assert.Equal(2, textEditor.EditIndex);

		// redo, undo, undo, redo
		
		// Redo One
		Assert.Throws<LuthetusTextEditorException>(textEditor.Redo);

		// Undo One
		textEditor.Undo();
		Assert.Equal("Abc", textEditor.AllText);
		Assert.Equal(3, textEditor.EditList.Count);
		Assert.Equal(1, textEditor.EditIndex);

		// Undo Two
		textEditor.Undo();
		Assert.Equal("AbcHello", textEditor.AllText);
		Assert.Equal(3, textEditor.EditList.Count);
		Assert.Equal(0, textEditor.EditIndex);

		// Redo Two
		textEditor.Redo();
		Assert.Equal("Abc", textEditor.AllText);
		Assert.Equal(3, textEditor.EditList.Count);
		Assert.Equal(1, textEditor.EditIndex);
	}

	[Fact]
	public void Delete_Case_Three()
	{
		// Construct
		var textEditor = new OptimizeTextEditor("AbcHello");

		var lengthHello = "Hello".Length;
		var lengthAbc = "Abc".Length;

		// Delete One
		textEditor.Delete(3, lengthHello);
		Assert.Equal("Abc", textEditor.AllText);
		Assert.Equal(2, textEditor.EditList.Count);
		Assert.Equal(1, textEditor.EditIndex);

		// Delete Two		
		textEditor.Delete(0, lengthAbc);
		Assert.Equal(string.Empty, textEditor.AllText);
		Assert.Equal(3, textEditor.EditList.Count);
		Assert.Equal(2, textEditor.EditIndex);

		// undo, redo, undo, redo
		
		// Undo One
		textEditor.Undo();
		Assert.Equal("Abc", textEditor.AllText);
		Assert.Equal(3, textEditor.EditList.Count);
		Assert.Equal(1, textEditor.EditIndex);

		// Redo One
		textEditor.Redo();
		Assert.Equal(string.Empty, textEditor.AllText);
		Assert.Equal(3, textEditor.EditList.Count);
		Assert.Equal(2, textEditor.EditIndex);

		// Undo Two
		textEditor.Undo();
		Assert.Equal("Abc", textEditor.AllText);
		Assert.Equal(3, textEditor.EditList.Count);
		Assert.Equal(1, textEditor.EditIndex);

		// Redo Two
		textEditor.Redo();
		Assert.Equal(string.Empty, textEditor.AllText);
		Assert.Equal(3, textEditor.EditList.Count);
		Assert.Equal(2, textEditor.EditIndex);
	}

	[Fact]
	public void Delete_Case_Four()
	{
		// Construct
		var textEditor = new OptimizeTextEditor("AbcHello");

		var lengthHello = "Hello".Length;
		var lengthAbc = "Abc".Length;

		// Delete One
		textEditor.Delete(3, lengthHello);
		Assert.Equal("Abc", textEditor.AllText);
		Assert.Equal(2, textEditor.EditList.Count);
		Assert.Equal(1, textEditor.EditIndex);

		// Delete Two		
		textEditor.Delete(0, lengthAbc);
		Assert.Equal(string.Empty, textEditor.AllText);
		Assert.Equal(3, textEditor.EditList.Count);
		Assert.Equal(2, textEditor.EditIndex);

		// undo, redo, redo, undo
		
		// Undo One
		textEditor.Undo();
		Assert.Equal("Abc", textEditor.AllText);
		Assert.Equal(3, textEditor.EditList.Count);
		Assert.Equal(1, textEditor.EditIndex);

		// Redo One
		textEditor.Redo();
		Assert.Equal(string.Empty, textEditor.AllText);
		Assert.Equal(3, textEditor.EditList.Count);
		Assert.Equal(2, textEditor.EditIndex);

		// Redo Two
		Assert.Throws<LuthetusTextEditorException>(textEditor.Redo);

		// Undo Two
		textEditor.Undo();
		Assert.Equal("Abc", textEditor.AllText);
		Assert.Equal(3, textEditor.EditList.Count);
		Assert.Equal(1, textEditor.EditIndex);
	}

	[Fact]
	public void Delete_Case_Five()
	{
		// Construct
		var textEditor = new OptimizeTextEditor("AbcHello");

		var lengthHello = "Hello".Length;
		var lengthAbc = "Abc".Length;

		// Delete One
		textEditor.Delete(3, lengthHello);
		Assert.Equal("Abc", textEditor.AllText);
		Assert.Equal(2, textEditor.EditList.Count);
		Assert.Equal(1, textEditor.EditIndex);

		// Delete Two		
		textEditor.Delete(0, lengthAbc);
		Assert.Equal(string.Empty, textEditor.AllText);
		Assert.Equal(3, textEditor.EditList.Count);
		Assert.Equal(2, textEditor.EditIndex);

		// redo, undo, redo, undo
		
		// Redo One
		Assert.Throws<LuthetusTextEditorException>(textEditor.Redo);

		// Undo One
		textEditor.Undo();
		Assert.Equal("Abc", textEditor.AllText);
		Assert.Equal(3, textEditor.EditList.Count);
		Assert.Equal(1, textEditor.EditIndex);

		// Redo Two
		textEditor.Redo();
		Assert.Equal(string.Empty, textEditor.AllText);
		Assert.Equal(3, textEditor.EditList.Count);
		Assert.Equal(2, textEditor.EditIndex);

		// Undo Two
		textEditor.Undo();
		Assert.Equal("Abc", textEditor.AllText);
		Assert.Equal(3, textEditor.EditList.Count);
		Assert.Equal(1, textEditor.EditIndex);
	}

	[Fact]
	public void Delete_Case_Six()
	{
		// Construct
		var textEditor = new OptimizeTextEditor("AbcHello");

		var lengthHello = "Hello".Length;
		var lengthAbc = "Abc".Length;

		// Delete One
		textEditor.Delete(3, lengthHello);
		Assert.Equal("Abc", textEditor.AllText);
		Assert.Equal(2, textEditor.EditList.Count);
		Assert.Equal(1, textEditor.EditIndex);

		// Delete Two		
		textEditor.Delete(0, lengthAbc);
		Assert.Equal(string.Empty, textEditor.AllText);
		Assert.Equal(3, textEditor.EditList.Count);
		Assert.Equal(2, textEditor.EditIndex);

		// redo, redo, undo, undo
		
		// Redo One
		Assert.Throws<LuthetusTextEditorException>(textEditor.Redo);

		// Redo Two
		Assert.Throws<LuthetusTextEditorException>(textEditor.Redo);

		// Undo One
		textEditor.Undo();
		Assert.Equal("Abc", textEditor.AllText);
		Assert.Equal(3, textEditor.EditList.Count);
		Assert.Equal(1, textEditor.EditIndex);

		// Undo Two
		textEditor.Undo();
		Assert.Equal("AbcHello", textEditor.AllText);
		Assert.Equal(3, textEditor.EditList.Count);
		Assert.Equal(0, textEditor.EditIndex);
	}
}