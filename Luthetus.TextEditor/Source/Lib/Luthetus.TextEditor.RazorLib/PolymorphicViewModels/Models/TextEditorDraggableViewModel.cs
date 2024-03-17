using Luthetus.Common.RazorLib.PolymorphicViewModels.Models;
using Luthetus.Common.RazorLib.Drags.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.JavaScriptObjects.Models;
using Luthetus.Common.RazorLib.Tabs.Displays;
using Luthetus.Common.RazorLib.Tabs.Models;
using Luthetus.Common.RazorLib.Panels.Models;
using Luthetus.Common.RazorLib.Panels.States;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.Groups.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Immutable;
using Fluxor;

namespace Luthetus.TextEditor.RazorLib.PolymorphicViewModels.Models;

public partial record TextEditorDraggableViewModel : IDraggableViewModel
{
	public TextEditorDraggableViewModel(TextEditorPolymorphicViewModel textEditorPolymorphicViewModel)
	{
		TextEditorPolymorphicViewModel = textEditorPolymorphicViewModel;
		PolymorphicViewModel = textEditorPolymorphicViewModel;

		Key = new(TextEditorPolymorphicViewModel.ViewModelKey.Guid);
	}

	public TextEditorPolymorphicViewModel TextEditorPolymorphicViewModel { get; init; }
	public IPolymorphicViewModel? PolymorphicViewModel { get; init; }
	public Key<IDraggableViewModel> Key { get; init; }
	public Type RendererType { get; set; }

	public Dictionary<string, object?>? ParameterMap { get; set; }

	public ElementDimensions ElementDimensions { get; init; } = IDialogViewModel.ConstructDefaultElementDimensions();
	public ImmutableArray<IDropzoneViewModel> DropzoneViewModelList { get; set; } = ImmutableArray<IDropzoneViewModel>.Empty;

	public Task OnDragStopAsync(MouseEventArgs mouseEventArgs, IDropzoneViewModel? dropzone)
	{
		if (dropzone is TextEditorDropzoneViewModel textEditorDropzoneViewModel)
		{
			if (textEditorDropzoneViewModel.TextEditorGroupKey is null)
			{
				if (TextEditorPolymorphicViewModel.TextEditorGroup is not null)
				{
					TextEditorPolymorphicViewModel.TextEditorService.GroupApi.RemoveViewModel(
						TextEditorPolymorphicViewModel.TextEditorGroup.GroupKey,
						TextEditorPolymorphicViewModel.ViewModelKey);
				}
	
				TextEditorPolymorphicViewModel.TextEditorGroup = null;
				TextEditorPolymorphicViewModel.ActiveViewModelType = typeof(IDialogViewModel);

				TextEditorPolymorphicViewModel.DialogService.RegisterDialogRecord(
					TextEditorPolymorphicViewModel.DialogViewModel);
			}
			else
			{
				TextEditorPolymorphicViewModel.DialogService.DisposeDialogRecord(
					TextEditorPolymorphicViewModel.DialogViewModel.Key);

				var textEditorGroupKey = TextEditorPolymorphicViewModel.TextEditorGroup?.GroupKey ?? Key<TextEditorGroup>.Empty;

				if (textEditorDropzoneViewModel.TextEditorGroupKey.Value != Key<TextEditorGroup>.Empty &&
				    textEditorGroupKey != textEditorDropzoneViewModel.TextEditorGroupKey.Value)
				{
					TextEditorPolymorphicViewModel.ActiveViewModelType = typeof(ITabViewModel);

					TextEditorPolymorphicViewModel.TextEditorService.GroupApi.AddViewModel(
						textEditorDropzoneViewModel.TextEditorGroupKey.Value,
						TextEditorPolymorphicViewModel.ViewModelKey);
				}
			}

			return Task.CompletedTask;
		}
		else if (dropzone is PanelDropzoneViewModel panelDropzone)
		{
			if (TextEditorPolymorphicViewModel.TextEditorGroup is not null)
			{
				TextEditorPolymorphicViewModel.TextEditorService.GroupApi.RemoveViewModel(
					TextEditorPolymorphicViewModel.TextEditorGroup.GroupKey,
					TextEditorPolymorphicViewModel.ViewModelKey);
			}

			TextEditorPolymorphicViewModel.TextEditorGroup = null;

			TextEditorPolymorphicViewModel.Dispatcher.Dispatch(new PanelsState.RegisterPanelTabAction(
				panelDropzone.PanelGroupKey,
				new Panel(
					new Key<Panel>(TextEditorPolymorphicViewModel.ViewModelKey.Guid),
					new(),
					typeof(TextEditorViewModelDisplay),
					TextEditorPolymorphicViewModel.GetTitle())
					{
						ParameterMap = TextEditorPolymorphicViewModel.DialogViewModel.ParameterMap
					},
				true));

			return Task.CompletedTask;
		}
		else
		{
			return Task.CompletedTask;
		}
	}

	public async Task<ImmutableArray<IDropzoneViewModel>> GetDropzonesAsync()
	{
		if (TextEditorPolymorphicViewModel.TextEditorService is null)
			return ImmutableArray<IDropzoneViewModel>.Empty;

		if (TextEditorPolymorphicViewModel.ActiveViewModelType == typeof(ITabViewModel))
		{
			RendererType = typeof(TabDisplay);

			ParameterMap = new Dictionary<string, object?>
			{
				{
					nameof(TabDisplay.TabViewModel),
					PolymorphicViewModel.TabViewModel
				},
				{
					nameof(TabDisplay.IsBeingDragged),
					true
				}
			};
		}
		else if (TextEditorPolymorphicViewModel.ActiveViewModelType == typeof(IDialogViewModel))
		{
			RendererType = null;
			ParameterMap = null;
		}

		var dropzoneList = new List<IDropzoneViewModel>();
		AddFallbackDropzone(dropzoneList);
		await AddPanelDropzonesAsync(dropzoneList);

		var measuredHtmlElementDimensions = await TextEditorPolymorphicViewModel.JsRuntime.InvokeAsync<MeasuredHtmlElementDimensions>(
            "luthetusIde.measureElementById",
            $"luth_te_group_{TextEditorPolymorphicViewModel.TextEditorGroup.GroupKey.Guid}");
	
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
			TextEditorPolymorphicViewModel.TextEditorGroup.GroupKey,
			null,
			TextEditorPolymorphicViewModel));

		var result = dropzoneList.ToImmutableArray();
		DropzoneViewModelList = result;
		return result;
	}

	private async Task AddPanelDropzonesAsync(List<IDropzoneViewModel> dropzoneList)
	{
		var panelGroupHtmlIdTupleList = new (Key<PanelGroup> PanelGroupKey, string HtmlElementId)[]
		{
			(PanelFacts.LeftPanelRecordKey, "luth_ide_panel_left_tabs"),
			(PanelFacts.RightPanelRecordKey, "luth_ide_panel_right_tabs"),
			(PanelFacts.BottomPanelRecordKey, "luth_ide_panel_bottom_tabs"),
		};

		foreach (var panelGroupHtmlIdTuple in panelGroupHtmlIdTupleList)
		{
			var measuredHtmlElementDimensions = await TextEditorPolymorphicViewModel.JsRuntime.InvokeAsync<MeasuredHtmlElementDimensions>(
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
				TextEditorPolymorphicViewModel.PanelsStateWrap,
				TextEditorPolymorphicViewModel.Dispatcher,
				TextEditorPolymorphicViewModel.DialogService,
				TextEditorPolymorphicViewModel.JsRuntime,
				measuredHtmlElementDimensions,
				elementDimensions,
				null,
				PolymorphicViewModel));
		}
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
			"luth_dropzone-fallback",
			TextEditorPolymorphicViewModel));
	}
}
