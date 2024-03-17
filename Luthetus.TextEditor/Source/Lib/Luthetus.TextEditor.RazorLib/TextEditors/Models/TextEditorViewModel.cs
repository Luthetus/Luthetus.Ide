using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.Virtualizations.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.Common.RazorLib.Panels.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Microsoft.AspNetCore.Components.Web;
using Luthetus.Common.RazorLib.Contexts.Models;
using Fluxor;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Microsoft.JSInterop;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.TextEditor.RazorLib.Groups.Models;
using Luthetus.Common.RazorLib.Panels.States;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays;
using Luthetus.Common.RazorLib.JavaScriptObjects.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

/// <summary>
/// Stores the state of the user interface.<br/><br/>
/// 
/// For example, the user's <see cref="TextEditorCursor"/> instances are stored here.<br/><br/>
/// 
/// Each <see cref="TextEditorViewModel"/> has a unique underlying <see cref="TextEditorModel"/>.<br/><br/>
/// 
/// Therefore, if one has a <see cref="TextEditorModel"/> of a text file named "myHomework.txt", then arbitrary amount of <see cref="TextEditorViewModel"/>(s) can reference that <see cref="TextEditorModel"/>.<br/><br/>
/// 
/// For example, maybe one has a main text editor, but also a peek window open of the same underlying
/// <see cref="TextEditorModel"/>. The main text editor is one <see cref="TextEditorViewModel"/> and the peek window is a separate <see cref="TextEditorViewModel"/>.
/// Both of those <see cref="TextEditorViewModel"/>(s) are referencing the same <see cref="TextEditorModel"/>. Therefore typing into the peek window will also result in the main
/// text editor re-rendering with the updated text and vice versa.
/// </summary>
public record TextEditorViewModel  : ITextEditorTab, IPanelTab, IDialog, IDrag, IDisposable
{
    public TextEditorViewModel(
        Key<TextEditorViewModel> viewModelKey,
        ResourceUri resourceUri,
        ITextEditorService textEditorService,
        VirtualizationResult<List<RichCharacter>> virtualizationResult,
        bool displayCommandBar,
        TextEditorCategory category)
    {
        ViewModelKey = viewModelKey;
        ResourceUri = resourceUri;
        TextEditorService = textEditorService;
        VirtualizationResult = virtualizationResult;
        ShowCommandBar = displayCommandBar;
        Category = category;

        var primaryCursor = new TextEditorCursor(true);

        CursorList = new TextEditorCursor[]
        {
            primaryCursor
        }.ToImmutableArray();

        DisplayTracker = new(
            textEditorService,
            () => textEditorService.ViewModelApi.GetOrDefault(viewModelKey),
            () => textEditorService.ViewModelApi.GetModelOrDefault(viewModelKey));

        UnsafeState = new();
    }


    /// <summary>
    /// The first entry of <see cref="CursorList"/> should be the PrimaryCursor
    /// </summary>
    public TextEditorCursor PrimaryCursor => CursorList.First();
    /// <summary>
    /// The first entry of <see cref="CursorList"/> should be the PrimaryCursor
    /// </summary>
    public ImmutableArray<TextEditorCursor> CursorList { get; init; }

    /// <summary>
    /// This tracks which view models are actively rendered from Blazor's perspective.
    /// Thus, using this allows lazy recalculation of view model state when an underlying model changes.
    /// That is to say, if a view model isn't actively rendered, then do not re-calculate its state
    /// until it becomes rendered.
    /// </summary>
    public DisplayTracker DisplayTracker { get; }

    /// <summary>
    /// The main unique identifier for a <see cref="TextEditorViewModel"/>, used in many API.
    /// </summary>
    public Key<TextEditorViewModel> ViewModelKey { get; init; }
    /// <summary>
    /// The unique identifier for a <see cref="TextEditorModel"/>. The model is to say
    /// a representation of the file on a filesystem. The contents and such. Whereas
    /// the viewmodel is to track state regarding a rendered editor for that file,
    /// for example the cursor position.
    /// </summary>
    public ResourceUri ResourceUri { get; init; }
    /// <summary>
    /// Most API invocation (if not all) occurs through the <see cref="ITextEditorService"/>
    /// </summary>
    public ITextEditorService TextEditorService { get; init; }
    /// <summary>
    /// Given the dimensions of the rendered text editor, this provides a subset of the
    /// file's content, such that "only what is visible when rendered" is in this.
    /// There is some padding added so that scrolling looks a bit more natural however,
    /// so some content offscreen does get included here for a smoother experience.
    /// </summary>
    public VirtualizationResult<List<RichCharacter>> VirtualizationResult { get; init; }
    /// <summary>
    /// The command bar is referring to the <see cref="Keymaps.Models.Vims.TextEditorKeymapVim"/>.
    /// In the command line program "Vim" one can hit 'colon' to bring up a command bar.
    /// </summary>
    public bool ShowCommandBar { get; init; }
    /// <summary>
    /// <inheritdoc cref="Models.TextEditorCategory"/>
    /// </summary>
    public TextEditorCategory Category { get; }
    /// <summary>
    /// The find overlay refers to hitting the keymap { Ctrl + f } when browser focus is within a text editor.
    /// </summary>
    public bool ShowFindOverlay { get; init; }
    /// <summary>
    /// If one hits the keymap { Ctrl + s } when browser focus is within a text editor.
    /// </summary>
    public Action<ITextEditorModel>? OnSaveRequested { get; init; }
    /// <summary>
    /// When a view model is rendered within a <see cref="Groups.Models.TextEditorGroup"/>,
    /// this func can be used to rener a more friendly tab display name.
    /// </summary>
    public Func<TextEditorModel, string>? GetTabDisplayNameFunc { get; init; }
    /// <summary>
    /// <see cref="FirstPresentationLayerKeysList"/> is painted prior to any internal workings of the text editor.
    /// <br/><br/>
    /// Therefore the selected text background is rendered after anything in the <see cref="FirstPresentationLayerKeysList"/>.
    /// <br/><br/>
    /// When using the <see cref="FirstPresentationLayerKeysList"/> one might find their css overriden by for example, text being selected.
    /// </summary>
    public ImmutableList<Key<TextEditorPresentationModel>> FirstPresentationLayerKeysList { get; init; } = ImmutableList<Key<TextEditorPresentationModel>>.Empty;
    /// <summary>
    /// <see cref="LastPresentationLayerKeysList"/> is painted after any internal workings of the text editor.
    /// <br/><br/>
    /// Therefore the selected text background is rendered before anything in the <see cref="LastPresentationLayerKeysList"/>.
    /// <br/><br/>
    /// When using the <see cref="LastPresentationLayerKeysList"/> one might find the selected text background
    /// not being rendered with the text selection css if it were overriden by something in the <see cref="LastPresentationLayerKeysList"/>.
    /// </summary>
    public ImmutableList<Key<TextEditorPresentationModel>> LastPresentationLayerKeysList { get; init; } = ImmutableList<Key<TextEditorPresentationModel>>.Empty;

    /// <summary>
    /// The command bar is referring to the <see cref="Keymaps.Models.Vims.TextEditorKeymapVim"/>.
    /// In the command line program "Vim" one can hit 'colon' to bring up a command bar.
    /// This property is what the input element binds to
    /// </summary>
    public string CommandBarValue { get; set; } = string.Empty;
    /// <summary>
    /// The find overlay refers to hitting the keymap { Ctrl + f } when browser focus is within a text editor.
    /// This property is what the find overlay input element binds to.
    /// </summary>
    public string FindOverlayValue { get; set; } = string.Empty;
    public TextEditorViewModelUnsafeState UnsafeState { get; }

    public string BodyElementId => $"luth_te_text-editor-content_{ViewModelKey.Guid}";
    public string PrimaryCursorContentId => $"luth_te_text-editor-content_{ViewModelKey.Guid}_primary-cursor";
    public string GutterElementId => $"luth_te_text-editor-gutter_{ViewModelKey.Guid}";
    public string FindOverlayId => $"luth_te_find-overlay_{ViewModelKey.Guid}";

	public Key<Panel> Key { get; }

	public Key<ContextRecord> ContextRecordKey { get; }

	public IDispatcher Dispatcher { get; set; }
	public IDialogService DialogService { get; set; }
	public JSRuntime JsRuntime { get; set; }
	public ITabGroup TabGroup { get; set; }

	public Key<IDynamicViewModel> DynamicViewModelKey { get; }

	public string Title { get; }

	public Type ComponentType { get; }

	public Dictionary<string, object?>? ComponentParameterMap { get; }

	public string? CssClass { get; set; }
	public string? CssStyle { get; set; }
	public ElementDimensions ElementDimensions { get; set; }
	public bool DialogIsMinimized { get; set; }
	public bool DialogIsMaximized { get; set; }
	public bool DialogIsResizable { get; set; }
	public string DialogFocusPointHtmlElementId { get; set; }
	public ImmutableArray<IDropzone> DropzoneList { get; set; }

	public TextEditorEdit MutateScrollHorizontalPositionByPixelsFactory(double pixels)
    {
        return TextEditorService.ViewModelApi.MutateScrollHorizontalPositionFactory(
            BodyElementId,
            GutterElementId,
            pixels);
    }

    public TextEditorEdit MutateScrollVerticalPositionByPixelsFactory(double pixels)
    {
        return TextEditorService.ViewModelApi.MutateScrollVerticalPositionFactory(
            BodyElementId,
            GutterElementId,
            pixels);
    }

    public TextEditorEdit MutateScrollVerticalPositionByPagesFactory(double pages)
    {
        return MutateScrollVerticalPositionByPixelsFactory(
            pages * VirtualizationResult.TextEditorMeasurements.Height);
    }

    public TextEditorEdit MutateScrollVerticalPositionByLinesFactory(double lines)
    {
        return MutateScrollVerticalPositionByPixelsFactory(
            lines * VirtualizationResult.CharAndRowMeasurements.RowHeight);
    }

    /// <summary>If a parameter is null the JavaScript will not modify that value</summary>
    public TextEditorEdit SetScrollPositionFactory(double? scrollLeft, double? scrollTop)
    {
        return TextEditorService.ViewModelApi.SetScrollPositionFactory(
	        BodyElementId,
	        GutterElementId,
	        scrollLeft,
	        scrollTop);
    }

    public TextEditorEdit FocusFactory()
    {
        return TextEditorService.ViewModelApi.FocusPrimaryCursorFactory(
        	PrimaryCursorContentId);
    }

	public Task OnDragStopAsync(MouseEventArgs mouseEventArgs, IDropzone? dropzone)
	{
		var localTextEditorGroup = TabGroup as TextEditorGroup;
		var localPanelGroup = TabGroup as PanelGroup;

		if (dropzone is TextEditorGroupDropzone textEditorGroupDropzone)
		{
			// Create Dialog
			if (textEditorGroupDropzone.TextEditorGroupKey == Key<TextEditorGroup>.Empty)
			{
				// Delete the current UI
				{
					if (localTextEditorGroup is not null)
					{
						TextEditorService.GroupApi.RemoveViewModel(
							localTextEditorGroup.GroupKey,
							ViewModelKey);
					}
					else if (localPanelGroup is not null)
					{
						throw new NotImplementedException();
					}
					else
					{
						// Is a dialog
						//
						// Already a dialog, so nothing needs to be done here.
						return Task.CompletedTask;
					}

					TabGroup = null;
				}

				DialogService.RegisterDialogRecord(this);
			}

			// Create TextEditor Tab
			{
				// Check if to-be group is the same as current group.
				{
					if (localTextEditorGroup is not null)
					{
						if (localTextEditorGroup.GroupKey == textEditorGroupDropzone.TextEditorGroupKey)
							return Task.CompletedTask;
					}
				}

				// Delete the current UI
				{
					if (localTextEditorGroup is not null)
					{
						TextEditorService.GroupApi.RemoveViewModel(
							localTextEditorGroup.GroupKey,
							ViewModelKey);
					}
					else if (localPanelGroup is not null)
					{
						throw new NotImplementedException();
					}
					else
					{
						// Is a dialog
						DialogService.DisposeDialogRecord(DynamicViewModelKey);
					}
					
					TabGroup = null;
				}

				TextEditorService.GroupApi.AddViewModel(
					textEditorGroupDropzone.TextEditorGroupKey,
					ViewModelKey);
			}

			return Task.CompletedTask;
		}

		// Create Panel Tab
		if (dropzone is PanelGroupDropzone panelDropzone)
		{
			// Delete the current UI
			{
				if (localTextEditorGroup is not null)
				{
					TextEditorService.GroupApi.RemoveViewModel(
						localTextEditorGroup.GroupKey,
						ViewModelKey);
				}
				else if (localPanelGroup is not null)
				{
					throw new NotImplementedException();
				}
				else
				{
					// Is a dialog
					DialogService.DisposeDialogRecord(DynamicViewModelKey);
				}

				TabGroup = null;
			}

			Dispatcher.Dispatch(new PanelsState.RegisterPanelTabAction(
				panelDropzone.PanelGroupKey,
				new Panel(
					Title,
					new Key<Panel>(ViewModelKey.Guid),
					DynamicViewModelKey,
					Key<ContextRecord>.Empty,
					typeof(TextEditorViewModelDisplay),
					new Dictionary<string, object?>
					{
						{
							nameof(TextEditorViewModelDisplay.TextEditorViewModelKey),
							ViewModelKey
						},
					}),
				true));

			return Task.CompletedTask;
		}
		else
		{
			return Task.CompletedTask;
		}
	}

	public async Task<ImmutableArray<IDropzone>> GetDropzonesAsync()
	{
		if (TextEditorService is null)
			return ImmutableArray<IDropzone>.Empty;

		if (TabGroup is not null)
		{
			// TODO: Setup the component that renders while dragging
			//
			// throw new NotImplementedException();
		}
		else
		{
			// TODO: Setup the component that renders while dragging
			//
			// throw new NotImplementedException();
		}

		var dropzoneList = new List<IDropzone>();
		AddFallbackDropzone(dropzoneList);
		await AddPanelDropzonesAsync(dropzoneList);

		var measuredHtmlElementDimensions = await JsRuntime.InvokeAsync<MeasuredHtmlElementDimensions>(
            "luthetusIde.measureElementById",
            $"luth_te_group_{TextEditorService.GroupStateWrap.Value.GroupList.Single().GroupKey.Guid}");
	
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

		dropzoneList.Add(new TextEditorGroupDropzone(
			measuredHtmlElementDimensions,
			TextEditorService.GroupStateWrap.Value.GroupList.Single().GroupKey,
			elementDimensions));

		var result = dropzoneList.ToImmutableArray();
		DropzoneList = result;
		return result;
	}

	private async Task AddPanelDropzonesAsync(List<IDropzone> dropzoneList)
	{
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
	
			dropzoneList.Add(new PanelGroupDropzone(
				measuredHtmlElementDimensions,
				panelGroupHtmlIdTuple.PanelGroupKey,
				elementDimensions));
		}
	}

	private void AddFallbackDropzone(List<IDropzone> dropzoneList)
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

		dropzoneList.Add(new TextEditorGroupDropzone(
			new MeasuredHtmlElementDimensions(0, 0, 0, 0, 0),
			Key<TextEditorGroup>.Empty,
			fallbackElementDimensions));
	}

    public void Dispose()
    {
        DisplayTracker.Dispose();
    }

	public IDialog SetIsMaximized(bool isMaximized)
	{
		throw new NotImplementedException();
	}

	public IDialog SetParameterMap(Dictionary<string, object?>? componentParameterMap)
	{
		throw new NotImplementedException();
	}

	public Task OnDragStartAsync(MouseEventArgs mouseEventArgs)
	{
		throw new NotImplementedException();
	}

	public Task OnDragEndAsync(MouseEventArgs mouseEventArgs, IDropzone? dropzone)
	{
		throw new NotImplementedException();
	}

	Task<ImmutableArray<IDropzone>> IDrag.GetDropzonesAsync()
	{
		throw new NotImplementedException();
	}
}
