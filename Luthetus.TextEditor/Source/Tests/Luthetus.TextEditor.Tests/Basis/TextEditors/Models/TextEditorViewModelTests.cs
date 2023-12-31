using Xunit;
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
            displayCommandBar);

        Assert.Equal(viewModelKey, viewModel.ViewModelKey);
        Assert.Equal(resourceUri, viewModel.ResourceUri);
        Assert.Equal(textEditorService, viewModel.TextEditorService);
        Assert.Equal(virtualizationResult, viewModel.VirtualizationResult);
		Assert.Equal(displayCommandBar, viewModel.DisplayCommandBar);
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
    /// </summary>
    [Fact]
	public void ThrottleRemeasure()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorViewModel.ThrottleCalculateVirtualizationResult"/>
	/// </summary>
	[Fact]
	public void ThrottleCalculateVirtualizationResult()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorViewModel.PrimaryCursor"/>
	/// </summary>
	[Fact]
	public void PrimaryCursor()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorViewModel.DisplayTracker"/>
	/// </summary>
	[Fact]
	public void DisplayTracker()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorViewModel.VirtualizationResult"/>
	/// </summary>
	[Fact]
	public void VirtualizationResult()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorViewModel.DisplayCommandBar"/>
	/// </summary>
	[Fact]
	public void DisplayCommandBar()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorViewModel.OnSaveRequested"/>
	/// </summary>
	[Fact]
	public void OnSaveRequested()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorViewModel.GetTabDisplayNameFunc"/>
	/// </summary>
	[Fact]
	public void GetTabDisplayNameFunc()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorViewModel.FirstPresentationLayerKeysBag"/>
	/// </summary>
	[Fact]
	public void FirstPresentationLayerKeysBag()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorViewModel.LastPresentationLayerKeysBag"/>
	/// </summary>
	[Fact]
	public void LastPresentationLayerKeysBag()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorViewModel.SeenModelRenderStateKeysBag"/>
	/// </summary>
	[Fact]
	public void SeenModelRenderStateKeysBag()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorViewModel.SeenOptionsRenderStateKeysBag"/>
	/// </summary>
	[Fact]
	public void SeenOptionsRenderStateKeysBag()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorViewModel.CommandBarValue"/>
	/// </summary>
	[Fact]
	public void CommandBarValue()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorViewModel.ShouldSetFocusAfterNextRender"/>
	/// </summary>
	[Fact]
	public void ShouldSetFocusAfterNextRender()
	{
		throw new NotImplementedException();
	}

    /// <summary>
    /// <see cref="TextEditorViewModel.BodyElementId"/>
    /// <br/>----<br/>
	/// <see cref="TextEditorViewModel.PrimaryCursorContentId"/>
	/// <see cref="TextEditorViewModel.GutterElementId"/>
    /// </summary>
    [Fact]
	public void BodyElementId()
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