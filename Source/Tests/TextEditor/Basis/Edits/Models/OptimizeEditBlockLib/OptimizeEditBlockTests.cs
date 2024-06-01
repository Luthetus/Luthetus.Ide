namespace Luthetus.TextEditor.Tests.Basis.Edits.Models.OptimizeEditBlockLib;

public class OptimizeEditBlockTests
{
    [Fact]
	public void Constructor()
	{
		var textEditor = new OptimizeTextEditor();

		textEditor.Insert(0, "Hello");
		textEditor.Insert(0, "Abc");

		throw new NotImplementedException();
	}
}