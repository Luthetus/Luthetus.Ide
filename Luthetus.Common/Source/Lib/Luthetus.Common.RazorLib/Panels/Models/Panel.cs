using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.JavaScriptObjects.Models;
using Luthetus.Common.RazorLib.Panels.States;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Immutable;
using Fluxor;

namespace Luthetus.Common.RazorLib.Panels.Models;

public record Panel : IPanelTab, IDialog, IDrag
{
	public Panel(
		string title,
		Key<Panel> key,
		Key<ContextRecord>? contextRecordKey,
		Type componentType,
		Dictionary<string, object?>? componentParameterMap)
	{
		Title = title;
		Key = key;
		ContextRecordKey = contextRecordKey;
		ComponentType = componentType;
		ComponentParameterMap = componentParameterMap;
	}

	public string Title { get; }
	public Key<Panel> Key { get; }
    public Key<ContextRecord> ContextRecordKey { get; }
	public Type ComponentType { get; }
	public Dictionary<string, object?>? ComponentParameterMap { get; }
	public string? CssClass { get; }
	public string? CssStyle { get; }

	public ITabGroup? TabGroup { get; set; }

	public Task OnDragStopAsync(MouseEventArgs mouseEventArgs, IDropzoneViewModel? dropzone)
	{
		if (dropzone is not PanelDropzoneViewModel panelDropzone)
			return Task.CompletedTask;
		
		var panelGroup = GetPanelGroup();
		var panel = GetPanel(panelGroup);
		
		if (panelGroup is null || panel is null)
			return Task.CompletedTask;

		if (panelDropzone.PanelGroupKey == Key<PanelGroup>.Empty)
		{
			Dispatcher.Dispatch(new PanelsState.DisposePanelTabAction(
				PanelGroupKey,
				PanelKey));

			DialogService.RegisterDialogRecord(PolymorphicViewModel.DialogViewModel);
		}
		else
		{
			Dispatcher.Dispatch(new PanelsState.DisposePanelTabAction(
				PanelGroupKey,
				PanelKey));

			var verticalHalfwayPoint = dropzone.MeasuredHtmlElementDimensions.TopInPixels +
				(dropzone.MeasuredHtmlElementDimensions.HeightInPixels / 2);

			var insertAtIndexZero = mouseEventArgs.ClientY < verticalHalfwayPoint
				? true
				: false;

			Dispatcher.Dispatch(new PanelsState.RegisterPanelTabAction(
				panelDropzone.PanelGroupKey,
				panel,
				insertAtIndexZero));
		}

		return Task.CompletedTask;
	}

	public async Task<ImmutableArray<IDropzoneViewModel>> GetDropzonesAsync()
	{
		var dropzoneList = new List<IDropzoneViewModel>();
		AddFallbackDropzone(dropzoneList);

		var panelGroupHtmlIdTupleList = new (Key<PanelGroup> PanelGroupKey, string HtmlElementId)[]
		{
			(PanelFacts.LeftPanelRecordKey, "luth_ide_panel_left_tabs"),
			(PanelFacts.RightPanelRecordKey, "luth_ide_panel_right_tabs"),
			(PanelFacts.BottomPanelRecordKey, "luth_ide_panel_bottom_tabs"),
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
	
			dropzoneList.Add(new PanelDropzoneViewModel(
				panelGroupHtmlIdTuple.PanelGroupKey,
				PanelsStateWrap,
				Dispatcher,
				DialogService,
				JsRuntime,
				measuredHtmlElementDimensions,
				elementDimensions,
				null,
				PolymorphicViewModel));
		}

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

		dropzoneList.Add(new PanelDropzoneViewModel(
			Key<PanelGroup>.Empty,
			PanelsStateWrap,
			Dispatcher,
			DialogService,
			JsRuntime,
			new MeasuredHtmlElementDimensions(0, 0, 0, 0, 0),
			fallbackElementDimensions,
			"luth_dropzone-fallback",
			PolymorphicViewModel));
	}
}
