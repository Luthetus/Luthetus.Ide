using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.Tests.Basis.TextEditors.Models.TextEditorServices;
using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.Virtualizations.Models;

namespace Luthetus.TextEditor.Tests.Basis.TextEditors.Models;

/// <summary>
/// <see cref="TextEditorViewModel"/>
/// </summary>
public class TextEditorViewModelTests
{
    /// <summary>
    /// <see cref="TextEditorViewModel(Common.RazorLib.Keys.Models.Key{TextEditorViewModel}, RazorLib.Lexes.Models.ResourceUri, RazorLib.TextEditors.Models.TextEditorServices.ITextEditorService, RazorLib.Virtualizations.Models.VirtualizationResult{List{RazorLib.Characters.Models.RichCharacter}}, bool)"/>
    /// <br/>----<br/>
    /// <see cref="TextEditorViewModel.ViewModelKey"/>
    /// <see cref="TextEditorViewModel.ResourceUri"/>
    /// <see cref="TextEditorViewModel.TextEditorService"/>
    /// </summary>
    [Fact]
	public void Constructor()
	{
        TextEditorServicesTestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
			out _,
            out var serviceProvider);

		var viewModelKey = Common.RazorLib.Keys.Models.Key<TextEditorViewModel>.NewKey();
		var resourceUri = inModel.ResourceUri;
		var virtualizationResult = VirtualizationResult<List<RichCharacter>>.GetEmptyRichCharacters();
		var displayCommandBar = false;

        var viewModel = new TextEditorViewModel(
			viewModelKey,
			resourceUri,
			textEditorService,
            virtualizationResult,
            displayCommandBar,
            new TextEditorCategory("UnitTesting"));

        Assert.Equal(viewModelKey, viewModel.ViewModelKey);
        Assert.Equal(resourceUri, viewModel.ResourceUri);
        Assert.Equal(textEditorService, viewModel.TextEditorService);
        Assert.Equal(virtualizationResult, viewModel.VirtualizationResult);
		Assert.Equal(displayCommandBar, viewModel.ShowCommandBar);
	}

    /// <summary>
    /// <see cref="TextEditorViewModel.CalculateVirtualizationResultAsync(RazorLib.TextEditors.Models.TextEditorModels.TextEditorModel?, RazorLib.JavaScriptObjects.Models.TextEditorMeasurements?, CancellationToken)"/>
    /// </summary>
    [Fact]
    public void CalculateVirtualizationResultAsync()
    {
		throw new NotImplementedException();
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
        TextEditorServicesTestsHelper.InitializeTextEditorServicesTestsHelper(
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
	/// <see cref="TextEditorViewModel.CursorMovePageTop()"/>
	/// </summary>
	[Fact]
	public void CursorMovePageTop()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorViewModel.CursorMovePageBottom()"/>
	/// </summary>
	[Fact]
	public void CursorMovePageBottom()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorViewModel.MutateScrollHorizontalPositionByPixels(double)"/>
	/// </summary>
	[Fact]
	public void MutateScrollHorizontalPositionByPixelsAsync()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorViewModel.MutateScrollVerticalPositionByPixels(double)"/>
	/// </summary>
	[Fact]
	public void MutateScrollVerticalPositionByPixelsAsync()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorViewModel.MutateScrollVerticalPositionByPages(double)"/>
	/// </summary>
	[Fact]
	public void MutateScrollVerticalPositionByPagesAsync()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorViewModel.MutateScrollVerticalPositionByLines(double)"/>
	/// </summary>
	[Fact]
	public void MutateScrollVerticalPositionByLinesAsync()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorViewModel.SetScrollPosition(double?, double?)"/>
	/// </summary>
	[Fact]
	public void SetScrollPositionAsync()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorViewModel.Focus()"/>
	/// </summary>
	[Fact]
	public void FocusAsync()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorViewModel.RemeasureAsync(RazorLib.Options.Models.TextEditorOptions, string, int, CancellationToken)"/>
	/// </summary>
	[Fact]
	public void RemeasureAsync()
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