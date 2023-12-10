using Xunit;

namespace Luthetus.TextEditor.Tests.Basis.TextEditors.Models;

public class TextEditorViewModelTests
{
	[Fact]
	public void TextEditorViewModel()
	{
		//public TextEditorViewModel(
//       Key<TextEditorViewModel> viewModelKey,
//       ResourceUri resourceUri,
//       ITextEditorService textEditorService,
//       VirtualizationResult<List<RichCharacter>> virtualizationResult,
//       bool displayCommandBar)
		throw new NotImplementedException();
	}

	[Fact]
	public void ThrottleRemeasure()
	{
		//public IThrottle ThrottleRemeasure { get; } = new Throttle(IThrottle.DefaultThrottleTimeSpan);
		throw new NotImplementedException();
	}

	[Fact]
	public void ThrottleCalculateVirtualizationResult()
	{
		//public IThrottle ThrottleCalculateVirtualizationResult { get; } = new Throttle(IThrottle.DefaultThrottleTimeSpan);
		throw new NotImplementedException();
	}

	[Fact]
	public void PrimaryCursor()
	{
		//public TextEditorCursor PrimaryCursor { get; } = new(true);
		throw new NotImplementedException();
	}

	[Fact]
	public void DisplayTracker()
	{
		//public DisplayTracker DisplayTracker { get; }
		throw new NotImplementedException();
	}

	[Fact]
	public void ViewModelKey()
	{
		//public Key<TextEditorViewModel> ViewModelKey { get; init; }
		throw new NotImplementedException();
	}

	[Fact]
	public void ResourceUri()
	{
		//public ResourceUri ResourceUri { get; init; }
		throw new NotImplementedException();
	}

	[Fact]
	public void TextEditorService()
	{
		//public ITextEditorService TextEditorService { get; init; }
		throw new NotImplementedException();
	}

	[Fact]
	public void VirtualizationResult()
	{
		//public VirtualizationResult<List<RichCharacter>> VirtualizationResult { get; init; }
		throw new NotImplementedException();
	}

	[Fact]
	public void DisplayCommandBar()
	{
		//public bool DisplayCommandBar { get; init; }
		throw new NotImplementedException();
	}

	[Fact]
	public void OnSaveRequested()
	{
		//public Action<TextEditorModel>? OnSaveRequested { get; init; }
		throw new NotImplementedException();
	}

	[Fact]
	public void GetTabDisplayNameFunc()
	{
		//public Func<TextEditorModel, string>? GetTabDisplayNameFunc { get; init; }
		throw new NotImplementedException();
	}

	[Fact]
	public void FirstPresentationLayerKeysBag()
	{
		//public ImmutableList<Key<TextEditorPresentationModel>> FirstPresentationLayerKeysBag { get; init; } = ImmutableList<Key<TextEditorPresentationModel>>.Empty;
		throw new NotImplementedException();
	}

	[Fact]
	public void LastPresentationLayerKeysBag()
	{
		//public ImmutableList<Key<TextEditorPresentationModel>> LastPresentationLayerKeysBag { get; init; } = ImmutableList<Key<TextEditorPresentationModel>>.Empty;
		throw new NotImplementedException();
	}

	[Fact]
	public void SeenModelRenderStateKeysBag()
	{
		//public HashSet<Key<RenderState>> SeenModelRenderStateKeysBag { get; init; } = new();
		throw new NotImplementedException();
	}
	
    [Fact]
	public void SeenOptionsRenderStateKeysBag()
	{
		//public HashSet<Key<RenderState>> SeenOptionsRenderStateKeysBag { get; init; } = new();
		throw new NotImplementedException();
	}

	[Fact]
	public void CommandBarValue()
	{
		//public string CommandBarValue { get; set; } = string.Empty;
		throw new NotImplementedException();
	}

	[Fact]
	public void ShouldSetFocusAfterNextRender()
	{
		//public bool ShouldSetFocusAfterNextRender { get; set; }
		throw new NotImplementedException();
	}

	[Fact]
	public void BodyElementId()
	{
		//public string BodyElementId => $"luth_te_text-editor-content_{ViewModelKey.Guid}";
		throw new NotImplementedException();
	}

	[Fact]
	public void PrimaryCursorContentId()
	{
		//public string PrimaryCursorContentId => $"luth_te_text-editor-content_{ViewModelKey.Guid}_primary-cursor";
		throw new NotImplementedException();
	}

	[Fact]
	public void GutterElementId()
	{
		//public string GutterElementId => $"luth_te_text-editor-gutter_{ViewModelKey.Guid}";
		throw new NotImplementedException();
	}

	[Fact]
	public void CursorMovePageTop()
	{
		//public void CursorMovePageTop()
		throw new NotImplementedException();
	}

	[Fact]
	public void CursorMovePageBottom()
	{
		//public void CursorMovePageBottom()
		throw new NotImplementedException();
	}

	[Fact]
	public void MutateScrollHorizontalPositionByPixelsAsync()
	{
		//public async Task MutateScrollHorizontalPositionByPixelsAsync(double pixels)
		throw new NotImplementedException();
	}

	[Fact]
	public void MutateScrollVerticalPositionByPixelsAsync()
	{
		//public async Task MutateScrollVerticalPositionByPixelsAsync(double pixels)
		throw new NotImplementedException();
	}

	[Fact]
	public void MutateScrollVerticalPositionByPagesAsync()
	{
		//public async Task MutateScrollVerticalPositionByPagesAsync(double pages)
		throw new NotImplementedException();
	}

	[Fact]
	public void MutateScrollVerticalPositionByLinesAsync()
	{
		//public async Task MutateScrollVerticalPositionByLinesAsync(double lines)
		throw new NotImplementedException();
	}

	[Fact]
	public void SetScrollPositionAsync()
	{
		//public async Task SetScrollPositionAsync(double? scrollLeft, double? scrollTop)
		throw new NotImplementedException();
	}

	[Fact]
	public void FocusAsync()
	{
		//public async Task FocusAsync()
		throw new NotImplementedException();
	}

	[Fact]
	public void RemeasureAsync()
	{
		//public async Task RemeasureAsync(
	 //       TextEditorOptions options,
	 //       string measureCharacterWidthAndRowHeightElementId,
	 //       int countOfTestCharacters,
	 //       CancellationToken cancellationToken)
		throw new NotImplementedException();
	}

	[Fact]
	public void CalculateVirtualizationResultAsync()
	{
		//public async Task CalculateVirtualizationResultAsync(
	 //       TextEditorModel? model,
	 //       TextEditorMeasurements? textEditorMeasurements,
	 //       CancellationToken cancellationToken)
		throw new NotImplementedException();
	}

	[Fact]
	public void Dispose()
	{
		//public void Dispose()
		throw new NotImplementedException();
	}
}