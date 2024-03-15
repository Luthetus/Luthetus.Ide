using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.PolymorphicUis.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.PolymorphicUis.Displays;
using Luthetus.Common.RazorLib.JavaScriptObjects.Models;
using Luthetus.Common.RazorLib.Panels.States;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Immutable;
using Fluxor;

namespace Luthetus.Common.RazorLib.Panels.Models;

public partial record Panel : IPolymorphicDraggable
{
	public bool IsBeingDragged { get; set; }
	public Type DraggableRendererType => RendererType;
	public Dictionary<string, object?> DraggableParameterMap => TabParameterMap;
	public ElementDimensions DraggableElementDimensions => DialogElementDimensions;
	public ImmutableArray<IPolymorphicDropzone> DropzoneList { get; set; } = ImmutableArray<IPolymorphicDropzone>.Empty;

	public Task OnDragStopAsync(MouseEventArgs mouseEventArgs, IPolymorphicDropzone? dropzone)
	{
		if (dropzone is not PanelDropzone panelDropzone)
			return Task.CompletedTask;

		if (panelDropzone.PanelGroupKey is null)
		{
			Dispatcher.Dispatch(new PanelsState.DisposePanelTabAction(
				PanelGroup.Key,
				Key));

			var dialogRecord = new DialogRecord(
    			DialogKey,
    			Title,
    			ContentRendererType,
    			ParameterMap,
    			null,
				true);

			DialogService.RegisterDialogRecord(dialogRecord);
		}
		else
		{
			Dispatcher.Dispatch(new PanelsState.DisposePanelTabAction(
				PanelGroup.Key,
				Key));

			var verticalHalfwayPoint = dropzone.MeasuredHtmlElementDimensions.TopInPixels +
				(dropzone.MeasuredHtmlElementDimensions.HeightInPixels / 2);

			var insertAtIndexZero = mouseEventArgs.ClientY < verticalHalfwayPoint
				? true
				: false;

			Dispatcher.Dispatch(new PanelsState.RegisterPanelTabAction(
				panelDropzone.PanelGroupKey.Value,
				this,
				insertAtIndexZero));
		}

		return Task.CompletedTask;
	}

	public async Task<ImmutableArray<IPolymorphicDropzone>> GetDropzonesAsync()
	{
		var dropzoneList = new List<IPolymorphicDropzone>();
		AddFallbackDropzone(dropzoneList);

		var panelGroupHtmlIdTupleList = new (Key<PanelGroup> PanelGroupKey, string HtmlElementId)[]
		{
			(PanelFacts.LeftPanelRecordKey, "luth_ide_panel_left"),
			(PanelFacts.RightPanelRecordKey, "luth_ide_panel_right"),
			(PanelFacts.BottomPanelRecordKey, "luth_ide_panel_bottom"),
		};

		foreach (var panelGroupHtmlIdTuple in panelGroupHtmlIdTupleList)
		{
			var measuredHtmlElementDimensions = await JsRuntime.InvokeAsync<MeasuredHtmlElementDimensions>(
	            "luthetusIde.measureElementById",
	            panelGroupHtmlIdTuple.HtmlElementId);
	
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
	
			dropzoneList.Add(new PanelDropzone(
				measuredHtmlElementDimensions,
				elementDimensions,
				panelGroupHtmlIdTuple.PanelGroupKey));
		}

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

		dropzoneList.Add(new PanelDropzone(
			new MeasuredHtmlElementDimensions(0, 0, 0, 0, 0),
			fallbackElementDimensions,
			null));
	}
}
