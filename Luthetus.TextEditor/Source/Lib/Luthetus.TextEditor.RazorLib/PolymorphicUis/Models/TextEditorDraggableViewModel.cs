using Luthetus.Common.RazorLib.PolymorphicViewModels.Models;
using Luthetus.Common.RazorLib.Drags.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.JavaScriptObjects.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.Groups.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.PolymorphicUis.Models;

public partial record TextEditorDraggableViewModel : IDraggableViewModel
{
	public TextEditorDraggableViewModel(
		Key<TextEditorViewModel> viewModelKey,
		TextEditorGroup textEditorGroup,
		TextEditorViewModelDisplayOptions textEditorViewModelDisplayOptions,
		ITextEditorService textEditorService,
		IDialogService dialogService,
		IJSRuntime jsRuntime)
	{
		ViewModelKey = viewModelKey;
		TextEditorGroup = textEditorGroup;
		TextEditorService = textEditorService;
		DialogService = dialogService;
		JsRuntime = jsRuntime;
	}

	public Key<TextEditorViewModel> ViewModelKey { get; init; }
	public TextEditorGroup TextEditorGroup { get; init; }
	public TextEditorViewModelDisplayOptions TextEditorViewModelDisplayOptions { get; init; }
	public ITextEditorService TextEditorService { get; init; }
	public IDialogService DialogService { get; init; }
	public IJSRuntime JsRuntime { get; init; }

	public IPolymorphicViewModel? PolymorphicViewModel { get; init; }
	public Key<IDraggableViewModel> Key { get; init; }
	public Type RendererType { get; init; }
	public Dictionary<string, object?> ParameterMap { get; init; }
	public ElementDimensions ElementDimensions { get; init; }
	public ImmutableArray<IDropzoneViewModel> DropzoneViewModelList { get; set; } = ImmutableArray<IDropzoneViewModel>.Empty;

	public Task OnDragStopAsync(MouseEventArgs mouseEventArgs, IDropzoneViewModel? dropzone)
	{
		if (dropzone is not TextEditorDropzoneViewModel textEditorDropzoneViewModel)
			return Task.CompletedTask;

		if (textEditorDropzoneViewModel.TextEditorGroupKey is null)
		{
			TextEditorService.GroupApi.RemoveViewModel(TextEditorGroup.GroupKey, ViewModelKey);

			var dialogRecord = new DialogViewModel(
    			Key<IDialogViewModel>.NewKey(),
    			"TODO: Title",
    			typeof(TextEditorViewModelDisplay),
    			new Dictionary<string, object?>()
				{
					{
						nameof(TextEditorViewModelDisplay.TextEditorViewModelKey),
						ViewModelKey
					},
					{
						nameof(TextEditorViewModelDisplay.ViewModelDisplayOptions),
						TextEditorViewModelDisplayOptions
					}
				},
    			null,
				true,
				PolymorphicViewModel);

			DialogService.RegisterDialogRecord(dialogRecord);
		}

		return Task.CompletedTask;
	}

	public async Task<ImmutableArray<IDropzoneViewModel>> GetDropzonesAsync()
	{
		if (TextEditorService is null)
			return ImmutableArray<IDropzoneViewModel>.Empty;

		var dropzoneList = new List<IDropzoneViewModel>();
		AddFallbackDropzone(dropzoneList);

		var measuredHtmlElementDimensions = await JsRuntime.InvokeAsync<MeasuredHtmlElementDimensions>(
            "luthetusIde.measureElementById",
            $"luth_te_group_{TextEditorGroup.GroupKey.Guid}");
	
		measuredHtmlElementDimensions = measuredHtmlElementDimensions with
		{
			ZIndex = 1,
		};

		var elementDimensions = new ElementDimensions();

		elementDimensions.ElementPositionKind = ElementPositionKind.Fixed;

		// Width
		{
			var widthDimensionAttribute = elementDimensions.DimensionAttributeList.First(
                x => x.DimensionAttributeKind == DimensionAttributeKind.Width);

			widthDimensionAttribute.DimensionUnitList.Clear();
            widthDimensionAttribute.DimensionUnitList.Add(new DimensionUnit
            {
                Value = measuredHtmlElementDimensions.WidthInPixels,
                DimensionUnitKind = DimensionUnitKind.Pixels
            });
		}

		// Height
		{
			var heightDimensionAttribute = elementDimensions.DimensionAttributeList.First(
                x => x.DimensionAttributeKind == DimensionAttributeKind.Height);

			heightDimensionAttribute.DimensionUnitList.Clear();
            heightDimensionAttribute.DimensionUnitList.Add(new DimensionUnit
            {
                Value = measuredHtmlElementDimensions.HeightInPixels,
                DimensionUnitKind = DimensionUnitKind.Pixels
            });
		}

		// Left
		{
			var leftDimensionAttribute = elementDimensions.DimensionAttributeList.First(
                x => x.DimensionAttributeKind == DimensionAttributeKind.Left);

            leftDimensionAttribute.DimensionUnitList.Clear();
            leftDimensionAttribute.DimensionUnitList.Add(new DimensionUnit
            {
                Value = measuredHtmlElementDimensions.LeftInPixels,
                DimensionUnitKind = DimensionUnitKind.Pixels
            });
		}

		// Top
		{
			var topDimensionAttribute = elementDimensions.DimensionAttributeList.First(
                x => x.DimensionAttributeKind == DimensionAttributeKind.Top);

            topDimensionAttribute.DimensionUnitList.Clear();
            topDimensionAttribute.DimensionUnitList.Add(new DimensionUnit
            {
                Value = measuredHtmlElementDimensions.TopInPixels,
                DimensionUnitKind = DimensionUnitKind.Pixels
            });
		}

		dropzoneList.Add(new TextEditorDropzoneViewModel(
			measuredHtmlElementDimensions,
			elementDimensions,
			TextEditorGroup.GroupKey,
			PolymorphicViewModel));

		var result = dropzoneList.ToImmutableArray();
		DropzoneViewModelList = result;
		return result;
	}

	private void AddFallbackDropzone(List<IDropzoneViewModel> dropzoneList)
	{
		var fallbackElementDimensions = new ElementDimensions();

		fallbackElementDimensions.ElementPositionKind = ElementPositionKind.Fixed;

		// Width
		{
			var widthDimensionAttribute = fallbackElementDimensions.DimensionAttributeList.First(
                x => x.DimensionAttributeKind == DimensionAttributeKind.Width);

			widthDimensionAttribute.DimensionUnitList.Clear();
            widthDimensionAttribute.DimensionUnitList.Add(new DimensionUnit
            {
                Value = 100,
                DimensionUnitKind = DimensionUnitKind.ViewportWidth
            });
		}

		// Height
		{
			var heightDimensionAttribute = fallbackElementDimensions.DimensionAttributeList.First(
                x => x.DimensionAttributeKind == DimensionAttributeKind.Height);

			heightDimensionAttribute.DimensionUnitList.Clear();
            heightDimensionAttribute.DimensionUnitList.Add(new DimensionUnit
            {
                Value = 100,
                DimensionUnitKind = DimensionUnitKind.ViewportHeight
            });
		}

		// Left
		{
			var leftDimensionAttribute = fallbackElementDimensions.DimensionAttributeList.First(
                x => x.DimensionAttributeKind == DimensionAttributeKind.Left);

            leftDimensionAttribute.DimensionUnitList.Clear();
            leftDimensionAttribute.DimensionUnitList.Add(new DimensionUnit
            {
                Value = 0,
                DimensionUnitKind = DimensionUnitKind.Pixels
            });
		}

		// Top
		{
			var topDimensionAttribute = fallbackElementDimensions.DimensionAttributeList.First(
                x => x.DimensionAttributeKind == DimensionAttributeKind.Top);

            topDimensionAttribute.DimensionUnitList.Clear();
            topDimensionAttribute.DimensionUnitList.Add(new DimensionUnit
            {
                Value = 0,
                DimensionUnitKind = DimensionUnitKind.Pixels
            });
		}

		dropzoneList.Add(new TextEditorDropzoneViewModel(
			new MeasuredHtmlElementDimensions(0, 0, 0, 0, 0),
			fallbackElementDimensions,
			null,
			PolymorphicViewModel));
	}
}
