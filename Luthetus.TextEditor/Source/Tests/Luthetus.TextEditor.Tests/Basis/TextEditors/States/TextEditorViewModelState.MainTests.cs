using Xunit;
using Luthetus.TextEditor.RazorLib.TextEditors.States;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.Virtualizations.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.Tests.Basis.TextEditors.Models.TextEditorServices;

namespace Luthetus.TextEditor.Tests.Basis.TextEditors.States;

/// <summary>
/// <see cref="TextEditorViewModelState"/>
/// </summary>
public class TextEditorViewModelStateMainTests
{
	/// <summary>
	/// <see cref="TextEditorViewModelState()"/>
	/// </summary>
	[Fact]
	public void Constructor()
	{
		var viewModelState = new TextEditorViewModelState();
		Assert.Equal(ImmutableList<TextEditorViewModel>.Empty, viewModelState.ViewModelBag);
	}

	/// <summary>
	/// <see cref="TextEditorViewModelState.ViewModelBag"/>
	/// </summary>
	[Fact]
	public void ViewModelBag()
	{
        TextEditorServicesTestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

        var viewModelState = new TextEditorViewModelState();
        Assert.Equal(ImmutableList<TextEditorViewModel>.Empty, viewModelState.ViewModelBag);

		var viewModel = new TextEditorViewModel(
            Key<TextEditorViewModel>.NewKey(),
            new ResourceUri("/unitTesting.txt"),
            textEditorService,
            VirtualizationResult<List<RichCharacter>>.GetEmptyRichCharacters(),
            false);

		var outViewModelBag = viewModelState.ViewModelBag.Add(viewModel);
        Assert.NotEqual(ImmutableList<TextEditorViewModel>.Empty, outViewModelBag);

		var outViewModelState = new TextEditorViewModelState
		{
			ViewModelBag = outViewModelBag
        };

		Assert.Equal(outViewModelBag, outViewModelState.ViewModelBag);
	}
}