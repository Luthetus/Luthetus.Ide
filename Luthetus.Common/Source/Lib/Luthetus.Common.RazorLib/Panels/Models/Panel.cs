using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.PolymorphicUis.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.PolymorphicUis.Displays;
using Luthetus.Common.RazorLib.JavaScriptObjects.Models;
using Luthetus.Common.RazorLib.Panels.States;
using Microsoft.JSInterop;
using System.Collections.Immutable;
using Fluxor;

namespace Luthetus.Common.RazorLib.Panels.Models;

/// <summary>
/// Each PanelTab maintains its own element dimensions as
/// each panel might need different amounts of space to be functionally usable.
/// </summary>
public record Panel : IPolymorphicTab, IPolymorphicDialog, IPolymorphicDraggable
{
    public ElementDimensions ElementDimensions { get; }
    public ElementDimensions BeingDraggedDimensions { get; }
    public Type IconRendererType { get; }

	public Panel(
		Key<Panel> key,
		ElementDimensions elementDimensions,
		Type contentRendererType,
		string title)
	{
		Key = key;
		ElementDimensions = elementDimensions;
		ContentRendererType = contentRendererType;
		Title = title;

		DialogElementDimensions = DialogConstructDefaultElementDimensions();
	}

	public Key<Panel> Key { get; }
	public Key<IPolymorphicUiRecord> PolymorphicUiKey { get; } = Key<IPolymorphicUiRecord>.NewKey();
	public string? CssClass { get; }
	public string? CssStyle { get; }
	public string Title { get; }
	public Type ContentRendererType { get; }
	public Type RendererType { get; } = typeof(PolymorphicTabDisplay);

	public IJSRuntime? JsRuntime { get; set; }
	public IDialogService DialogService { get; set; }
	public IDispatcher Dispatcher { get; set; }
	public PanelGroup PanelGroup { get; set; }

    public bool IsBeingDragged { get; set; }

    /// <summary>
    /// TODO: In progress feature: working on keymap that sets focus to a context record...
    /// ... and if the JavaScript set focus returns false (implying focus was NOT set) then
    /// perhaps the ContextRecord is tied to a PanelTab. If so, set the PanelTab as active
    /// then try again to set focus to the now rendered ContextRecord.
    /// </summary>
    public Key<ContextRecord>? ContextRecordKey { get; set; }

	private Key<DialogRecord> _dialogKey = Key<DialogRecord>.NewKey();

	public Dictionary<string, object?>? ParameterMap => new Dictionary<string, object?>();

	public Dictionary<string, object?>? TabParameterMap => new Dictionary<string, object?>
	{
		{
			nameof(PolymorphicTabDisplay.Tab),
			this
		},
		{
			nameof(PolymorphicTabDisplay.IsBeingDragged),
			true
		}
	};

	public ElementDimensions DialogElementDimensions { get; }
    public bool DialogIsMinimized { get; set; }
    public bool DialogIsMaximized { get; set; }
    public bool DialogIsResizable { get; set; }
    public string DialogFocusPointHtmlElementId => $"luth_dialog-focus-point_{Key.Guid}";

	public Type DraggableRendererType => RendererType;
	public Dictionary<string, object?> DraggableParameterMap => ParameterMap;
	public ElementDimensions DraggableElementDimensions => DialogElementDimensions;
	public ImmutableArray<IPolymorphicDropzone> DropzoneList { get; set; } = ImmutableArray<IPolymorphicDropzone>.Empty;

    public ElementDimensions DialogConstructDefaultElementDimensions()
	{
		var elementDimensions = new ElementDimensions
        {
            ElementPositionKind = ElementPositionKind.Fixed
        };

        // Width
        {
            var width = elementDimensions.DimensionAttributeList.Single(
                x => x.DimensionAttributeKind == DimensionAttributeKind.Width);

            width.DimensionUnitList.Add(new DimensionUnit
            {
                Value = 60,
                DimensionUnitKind = DimensionUnitKind.ViewportWidth
            });
        }

        // Height
        {
            var height = elementDimensions.DimensionAttributeList.Single(
                x => x.DimensionAttributeKind == DimensionAttributeKind.Height);

            height.DimensionUnitList.Add(new DimensionUnit
            {
                Value = 60,
                DimensionUnitKind = DimensionUnitKind.ViewportHeight
            });
        }

        // Left
        {
            var left = elementDimensions.DimensionAttributeList.Single(
                x => x.DimensionAttributeKind == DimensionAttributeKind.Left);

            left.DimensionUnitList.Add(new DimensionUnit
            {
                Value = 20,
                DimensionUnitKind = DimensionUnitKind.ViewportWidth
            });
        }

        // Top
        {
            var top = elementDimensions.DimensionAttributeList.Single(
                x => x.DimensionAttributeKind == DimensionAttributeKind.Top);

            top.DimensionUnitList.Add(new DimensionUnit
            {
                Value = 20,
                DimensionUnitKind = DimensionUnitKind.ViewportHeight
            });
        }

        return elementDimensions;
	}

	public Task OnDragStopAsync(IPolymorphicDropzone? dropzone)
	{
		if (dropzone is not PanelDropzone panelDropzone)
			return Task.CompletedTask;

		if (panelDropzone.PanelGroupKey is null)
		{
			Dispatcher.Dispatch(new PanelsState.DisposePanelTabAction(
				PanelGroup.Key,
				Key));

			var dialogRecord = new DialogRecord(
    			_dialogKey,
    			Title,
    			ContentRendererType,
    			ParameterMap,
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
		return ImmutableArray<IPolymorphicDropzone>.Empty;

		/*
		var dropzoneList = new List<IPolymorphicDropzone>();

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

		dropzoneList.Add(new PanelDropzone(
			measuredHtmlElementDimensions,
			elementDimensions,
			TextEditorGroup.GroupKey));

		// Fallback dropzone
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

		var result = dropzoneList.ToImmutableArray();
		DropzoneList = result;
		return result;
		*/
	}

	public Task TabSetAsActiveAsync()
	{
		Dispatcher.Dispatch(new PanelsState.SetActivePanelTabAction(PanelGroup.Key, Key));
		return Task.CompletedTask;
	}

	public string TabGetDynamicCss()
	{
		return PanelGroup.ActiveTabKey == Key
			? "luth_active"
		    : string.Empty;
	}

	public Task TabCloseAsync()
	{
		Dispatcher.Dispatch(new PanelsState.DisposePanelTabAction(PanelGroup.Key, Key));
		return Task.CompletedTask;
	}
}
