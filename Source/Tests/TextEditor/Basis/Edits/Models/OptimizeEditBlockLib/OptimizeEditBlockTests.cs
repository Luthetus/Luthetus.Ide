namespace Luthetus.TextEditor.Tests.Basis.Edits.Models.OptimizeEditBlockLib;

public class OptimizeEditBlockTests
{
	// TODO: IDEA: Is the context menu a presentation layer?

    [Fact]
	public void Insert_One()
	{
		var textEditor = new OptimizeTextEditor();
		Assert.Equal(string.Empty, textEditor.AllText);
		Assert.Equal(1, textEditor.EditList.Count);
		Assert.Equal(0, textEditor.EditIndex);
		Assert.IsType<TextEditorEditConstructor>(textEditor.EditList.Single());

		textEditor.Insert(0, "Hello");
		Assert.Equal("Hello", textEditor.AllText);
		Assert.Equal(2, textEditor.EditList.Count);
		Assert.Equal(1, textEditor.EditIndex);

		textEditor.Undo();
		Assert.Equal(string.Empty, textEditor.AllText);
		Assert.Equal(2, textEditor.EditList.Count);
		Assert.Equal(0, textEditor.EditIndex);

		textEditor.Redo();
		Assert.Equal("Hello", textEditor.AllText);
		Assert.Equal(1, textEditor.EditIndex);
	}

	[Fact]
	public void Insert_Two_SecondInsert_At_Start()
	{
		var textEditor = new OptimizeTextEditor();

		textEditor.Insert(0, "Hello");
		textEditor.Insert(0, "Abc");
		Assert.Equal("AbcHello", textEditor.AllText);
		Assert.Equal(2, textEditor.EditList.Count);

		textEditor.Undo();
		Assert.Equal("Hello", textEditor.AllText);
		Assert.Equal(1, textEditor.EditList.Count);
	}

	[Fact]
	public void Insert_Two_SecondInsert_At_Middle()
	{
		var textEditor = new OptimizeTextEditor();

		textEditor.Insert(0, "Hello");
		textEditor.Insert(2, "Abc");
		Assert.Equal("HeAbcllo", textEditor.AllText);
		Assert.Equal(2, textEditor.EditList.Count);

		textEditor.Undo();
		Assert.Equal("Hello", textEditor.AllText);
		Assert.Equal(1, textEditor.EditList.Count);
	}

	[Fact]
	public void Insert_Two_SecondInsert_At_End()
	{
		var textEditor = new OptimizeTextEditor();

		textEditor.Insert(0, "Hello");
		textEditor.Insert(textEditor.AllText.Length, "Abc");
		Assert.Equal("HelloAbc", textEditor.AllText);
		Assert.Equal(2, textEditor.EditList.Count);

		textEditor.Undo();
		Assert.Equal("Hello", textEditor.AllText);
		Assert.Equal(1, textEditor.EditList.Count);
	}

	[Fact]
	public void Delete()
	{
		var initialContent = "Hello World!";
		var textEditor = new OptimizeTextEditor(initialContent);
		Assert.Equal(initialContent, textEditor.AllText);
		Assert.Equal(0, textEditor.EditList.Count);

		textEditor.Delete(0, initialContent.Length);
		Assert.Equal(string.Empty, textEditor.AllText);
		Assert.Equal(1, textEditor.EditList.Count);

		textEditor.Undo();
		Assert.Equal(initialContent, textEditor.AllText);
		Assert.Equal(1, textEditor.EditList.Count);
	}
}