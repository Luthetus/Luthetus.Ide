using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.States;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.Tests.Basis.TextEditors.States;

/// <summary>
/// <see cref="TextEditorModelState"/>
/// </summary>
public class TextEditorModelStateMainTests
{
	/// <summary>
	/// <see cref="TextEditorModelState()"/>
	/// </summary>
	[Fact]
	public void Constructor()
	{
		var modelState = new TextEditorModelState();
		Assert.Equal(ImmutableList<TextEditorModel>.Empty, modelState.ModelList);
	}

	/// <summary>
	/// <see cref="TextEditorModelState.ModelList"/>
	/// </summary>
	[Fact]
	public void ModelList()
	{
        throw new NotImplementedException("Test was broken on (2024-04-08)");
        //      var modelState = new TextEditorModelState();
        //      Assert.Equal(ImmutableList<TextEditorModel>.Empty, modelState.ModelList);

        //var model = new TextEditorModel(
        //          new ResourceUri("/unitTesting.txt"),
        //          DateTime.UtcNow,
        //          ExtensionNoPeriodFacts.TXT,
        //          "AlphabetSoup",
        //          new TextEditorDecorationMapperDefault(),
        //          new LuthCompilerService(null));

        //var outModelList = modelState.ModelList.Add(model);
        //      Assert.NotEqual(ImmutableList<TextEditorModel>.Empty, outModelList);

        //var outModelState = new TextEditorModelState
        //{
        //	ModelList = outModelList
        //};

        //Assert.Equal(outModelList, outModelState.ModelList);
    }
}