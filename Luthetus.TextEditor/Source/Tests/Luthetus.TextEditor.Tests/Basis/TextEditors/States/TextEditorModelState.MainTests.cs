using Xunit;
using Luthetus.TextEditor.RazorLib.TextEditors.States;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

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
        var modelState = new TextEditorModelState();
        Assert.Equal(ImmutableList<TextEditorModel>.Empty, modelState.ModelList);

		var model = new TextEditorModel(
            new ResourceUri("/unitTesting.txt"),
            DateTime.UtcNow,
            ExtensionNoPeriodFacts.TXT,
            "AlphabetSoup",
            new TextEditorDecorationMapperDefault(),
            new TextEditorCompilerServiceDefault());

		var outModelList = modelState.ModelList.Add(model);
        Assert.NotEqual(ImmutableList<TextEditorModel>.Empty, outModelList);

		var outModelState = new TextEditorModelState
		{
			ModelList = outModelList
		};

		Assert.Equal(outModelList, outModelState.ModelList);
	}
}