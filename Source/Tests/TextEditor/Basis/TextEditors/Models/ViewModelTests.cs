using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.Virtualizations.Models;
using Fluxor;
using Microsoft.JSInterop;

namespace Luthetus.TextEditor.Tests.Basis.TextEditors.Models;

/// <summary>
/// <see cref="TextEditorViewModel"/>
/// </summary>
public class ViewModelTests
{
    /// <summary>
    /// <see cref="TextEditorViewModel(Key{TextEditorViewModel}, ResourceUri, ITextEditorService, IDispatcher, IDialogService, IJSRuntime, VirtualizationResult{List{RichCharacter}}, bool, Category)"/>
    /// <br/>----<br/>
    /// <see cref="TextEditorViewModel.ViewModelKey"/>
    /// <see cref="TextEditorViewModel.ResourceUri"/>
    /// <see cref="TextEditorViewModel.TextEditorService"/>
    /// </summary>
    [Fact]
	public void Constructor()
	{
        throw new NotImplementedException("Test was broken on (2024-04-08)");
  //      TextEditorServicesTestsHelper.InitializeTextEditorServicesTestsHelper(
  //          out var textEditorService,
  //          out var inModel,
		//	out _,
  //          out var serviceProvider);

		//var viewModelKey = Common.RazorLib.Keys.Models.Key<TextEditorViewModel>.NewKey();
		//var resourceUri = inModel.ResourceUri;
		//var virtualizationResult = VirtualizationResult<List<RichCharacter>>.GetEmptyRichCharacters();
		//var displayCommandBar = false;

  //      var viewModel = new TextEditorViewModel(
		//	viewModelKey,
		//	resourceUri,
		//	textEditorService,
  //          virtualizationResult,
  //          displayCommandBar,
  //          new TextEditorCategory("UnitTesting"));

  //      Assert.Equal(viewModelKey, viewModel.ViewModelKey);
  //      Assert.Equal(resourceUri, viewModel.ResourceUri);
  //      Assert.Equal(textEditorService, viewModel.TextEditorService);
  //      Assert.Equal(virtualizationResult, viewModel.VirtualizationResult);
		//Assert.Equal(displayCommandBar, viewModel.ShowCommandBar);
	}

    /// <summary>
    /// <see cref="TextEditorViewModel.ThrottleRemeasure"/>
	/// <see cref="TextEditorViewModel.ThrottleCalculateVirtualizationResult"/>
	/// <see cref="TextEditorViewModel.PrimaryCursor"/>
	/// <see cref="TextEditorViewModel.DisplayTracker"/>
	/// <see cref="TextEditorViewModel.VirtualizationResult"/>
	/// <see cref="TextEditorViewModel.ShowCommandBar"/>
	/// <see cref="TextEditorViewModel.OnSaveRequested"/>
	/// <see cref="TextEditorViewModel.GetTabDisplayNameFunc"/>
	/// <see cref="TextEditorViewModel.FirstPresentationLayerKeysList"/>
	/// <see cref="TextEditorViewModel.LastPresentationLayerKeysList"/>
	/// <see cref="TextEditorViewModel.CommandBarValue"/>
	/// <see cref="TextEditorViewModel.ShouldSetFocusAfterNextRender"/>
    /// <see cref="TextEditorViewModel.BodyElementId"/>
	/// <see cref="TextEditorViewModel.PrimaryCursorContentId"/>
	/// <see cref="TextEditorViewModel.GutterElementId"/>
    /// </summary>
    [Fact]
	public void Properties()
	{
        TestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

        Assert.Equal(
            $"luth_te_text-editor-content_{inViewModel.ViewModelKey.Guid}",
            inViewModel.BodyElementId);

        Assert.Equal(
            $"luth_te_text-editor-content_{inViewModel.ViewModelKey.Guid}_primary-cursor",
            inViewModel.PrimaryCursorContentId);

        Assert.Equal(
            $"luth_te_text-editor-gutter_{inViewModel.ViewModelKey.Guid}",
            inViewModel.GutterElementId);

        throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorViewModel.MutateScrollHorizontalPositionByPixelsFactory(double)"/>
	/// </summary>
	[Fact]
	public void MutateScrollHorizontalPositionByPixelsFactory()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorViewModel.MutateScrollVerticalPositionByPixelsFactory(double)"/>
	/// </summary>
	[Fact]
	public void MutateScrollVerticalPositionByPixelsFactory()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorViewModel.MutateScrollVerticalPositionByPagesFactory(double)"/>
	/// </summary>
	[Fact]
	public void MutateScrollVerticalPositionByPagesFactory()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorViewModel.MutateScrollVerticalPositionByLinesFactory(double)"/>
	/// </summary>
	[Fact]
	public void MutateScrollVerticalPositionByLinesFactory()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorViewModel.SetScrollPositionFactory(double?, double?)"/>
	/// </summary>
	[Fact]
	public void SetScrollPositionFactory()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorViewModel.FocusFactory()"/>
	/// </summary>
	[Fact]
	public void FocusFactory()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorViewModel.Dispose()"/>
	/// </summary>
	[Fact]
	public void Dispose()
	{
		throw new NotImplementedException();
	}
}