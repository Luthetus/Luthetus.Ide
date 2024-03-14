using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.PolymorphicUis.Displays;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.Groups.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays;
using Luthetus.Common.RazorLib.PolymorphicUis.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.JavaScriptObjects.Models;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.PolymorphicUis.Models;

public partial record TextEditorViewModelPolymorphicUi : IPolymorphicDraggable
{
	public Type DraggableRendererType => RendererType;
	public Dictionary<string, object?> DraggableParameterMap => TabParameterMap;
	public ElementDimensions DraggableElementDimensions => DialogElementDimensions;
	public ImmutableArray<IPolymorphicDropzone> DropzoneList { get; set; } = ImmutableArray<IPolymorphicDropzone>.Empty;

	public Task OnDragStopAsync(IPolymorphicDropzone? dropzone)
	{
		if (dropzone is not TextEditorViewModelPolymorphicDropzone textEditorViewModelPolymorphicDropzone)
			return Task.CompletedTask;

		if (textEditorViewModelPolymorphicDropzone.TextEditorGroupKey is null)
		{
			TextEditorService.GroupApi.RemoveViewModel(TextEditorGroup.GroupKey, ViewModelKey);

			var dialogRecord = new DialogRecord(
    			_dialogKey,
    			GetTitle(),
    			typeof(TextEditorViewModelDisplay),
    			new Dictionary<string, object?>()
				{
					{
						nameof(TextEditorViewModelDisplay.TextEditorViewModelKey),
						ViewModelKey
					},
					{
						nameof(TextEditorViewModelDisplay.ViewModelDisplayOptions),
						_textEditorViewModelDisplayOptions
					}
				},
    			null)
			{
				IsResizable = true
			};

			DialogService.RegisterDialogRecord(dialogRecord);
		}

		return Task.CompletedTask;
	}

	public async Task<ImmutableArray<IPolymorphicDropzone>> GetDropzonesAsync()
	{
		if (TextEditorService is null)
			return ImmutableArray<IPolymorphicDropzone>.Empty;

		var dropzoneList = new List<IPolymorphicDropzone>();
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

		dropzoneList.Add(new TextEditorViewModelPolymorphicDropzone(
			measuredHtmlElementDimensions,
			elementDimensions,
			TextEditorGroup.GroupKey));

		var result = dropzoneList.ToImmutableArray();
		DropzoneList = result;
		return result;
	}

	private void AddFallbackDropzone(List<IPolymorphicDropzone> dropzoneList)
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

		dropzoneList.Add(new TextEditorViewModelPolymorphicDropzone(
			new MeasuredHtmlElementDimensions(0, 0, 0, 0, 0),
			fallbackElementDimensions,
			null));
	}
}
