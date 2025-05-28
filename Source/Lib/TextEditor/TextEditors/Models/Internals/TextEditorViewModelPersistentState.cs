using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Exceptions;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.JavaScriptObjects.Models;
using Luthetus.Common.RazorLib.JsRuntimes.Models;
using Luthetus.Common.RazorLib.Panels.Models;
using Luthetus.Common.RazorLib.Tabs.Displays;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Commands.Models;
using Luthetus.TextEditor.RazorLib.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib.Groups.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

/// <summary>
/// This type reduces the amount of properties that need to be copied from one TextEditorViewModel instance to another
/// by chosing to have some of the state shared between instances.
/// </summary>
public class TextEditorViewModelPersistentState : IDisposable, ITab, IPanelTab, IDialog, IDrag
{
	public TextEditorViewModelPersistentState(
	    Key<TextEditorViewModel> viewModelKey,
	    ResourceUri resourceUri,
	    TextEditorService textEditorService,
	    Category category,
	    Action<TextEditorModel>? onSaveRequested,
	    Func<TextEditorModel, string>? getTabDisplayNameFunc,
	    List<Key<TextEditorPresentationModel>> firstPresentationLayerKeysList,
	    List<Key<TextEditorPresentationModel>> lastPresentationLayerKeysList,
	    bool showFindOverlay,
	    string replaceValueInFindOverlay,
	    bool showReplaceButtonInFindOverlay,
	    string findOverlayValue,
	    bool findOverlayValueExternallyChangedMarker,
	    MenuKind menuKind,
	    TooltipViewModel tooltipViewModel,
	    bool shouldRevealCursor,
		VirtualAssociativityKind virtualAssociativityKind,
		IPanelService panelService,
        IDialogService dialogService,
        CommonBackgroundTaskApi commonBackgroundTaskApi)
	{
	    ViewModelKey = viewModelKey;
	    ResourceUri = resourceUri;
	    TextEditorService = textEditorService;
	    Category = category;
	    OnSaveRequested = onSaveRequested;
	    GetTabDisplayNameFunc = getTabDisplayNameFunc;
	    FirstPresentationLayerKeysList = firstPresentationLayerKeysList;
	    LastPresentationLayerKeysList = lastPresentationLayerKeysList;
	    
	    ShowFindOverlay = showFindOverlay;
	    ReplaceValueInFindOverlay = replaceValueInFindOverlay;
	    ShowReplaceButtonInFindOverlay = showReplaceButtonInFindOverlay;
	    FindOverlayValue = findOverlayValue;
	    FindOverlayValueExternallyChangedMarker = findOverlayValueExternallyChangedMarker;
	    
	    MenuKind = menuKind;
	    TooltipViewModel = tooltipViewModel;

	    ShouldRevealCursor = shouldRevealCursor;
		VirtualAssociativityKind = virtualAssociativityKind;
		
		PanelService = panelService;
        DialogService = dialogService;
        CommonBackgroundTaskApi = commonBackgroundTaskApi;
        
        ComponentType = typeof(TextEditorViewModelDisplay);
        ComponentParameterMap = new()
        {
            { nameof(TextEditorViewModelDisplay.TextEditorViewModelKey), ViewModelKey }
        };

        _dragTabComponentType = typeof(TabDisplay);
        _dragTabComponentParameterMap = new()
        {
            { nameof(TabDisplay.Tab), this },
            { nameof(TabDisplay.IsBeingDragged), true }
        };

        DialogFocusPointHtmlElementId = $"luth_dialog-focus-point_{DynamicViewModelKey.Guid}";
	}

	/// <summary>
	/// The main unique identifier for a <see cref="TextEditorViewModel"/>, used in many API.
	/// </summary>
	public Key<TextEditorViewModel> ViewModelKey { get; set; }
	/// <summary>
	/// The unique identifier for a <see cref="TextEditorModel"/>. The model is to say a representation of the file on a filesystem.
	/// The contents and such. Whereas the viewmodel is to track state regarding a rendered editor for that file, for example the cursor position.
	/// </summary>
	public ResourceUri ResourceUri { get; set; }
	/// <summary>
	/// Most API invocation (if not all) occurs through the <see cref="ITextEditorService"/>
	/// </summary>
	public TextEditorService TextEditorService { get; set; }
	/// <summary>
	/// <inheritdoc cref="Models.Category"/>
	/// </summary>
	public Category Category { get; set; }
	/// <summary>
	/// If one hits the keymap { Ctrl + s } when browser focus is within a text editor.
	/// </summary>
	public Action<TextEditorModel>? OnSaveRequested { get; set; }
	/// <summary>
	/// When a view model is rendered within a <see cref="TextEditorGroup"/>, this Func can be used to render a more friendly tab name, than the resource uri path.
	/// </summary>
	public Func<TextEditorModel, string>? GetTabDisplayNameFunc { get; set; }
	/// <summary>
	/// <see cref="FirstPresentationLayerKeysList"/> is painted prior to any internal workings of the text editor.<br/><br/>
	/// Therefore the selected text background is rendered after anything in the <see cref="FirstPresentationLayerKeysList"/>.<br/><br/>
	/// When using the <see cref="FirstPresentationLayerKeysList"/> one might find their css overriden by for example, text being selected.
	/// </summary>
	public List<Key<TextEditorPresentationModel>> FirstPresentationLayerKeysList { get; set; }
	/// <summary>
	/// <see cref="LastPresentationLayerKeysList"/> is painted after any internal workings of the text editor.<br/><br/>
	/// Therefore the selected text background is rendered before anything in the <see cref="LastPresentationLayerKeysList"/>.<br/><br/>
	/// When using the <see cref="LastPresentationLayerKeysList"/> one might find the selected text background
	/// not being rendered with the text selection css if it were overriden by something in the <see cref="LastPresentationLayerKeysList"/>.
	/// </summary>
	public List<Key<TextEditorPresentationModel>> LastPresentationLayerKeysList { get; set; }
	
	/// <summary>
    /// The find overlay refers to hitting the keymap { Ctrl + f } when browser focus is within a text editor.
    /// </summary>
    public bool ShowFindOverlay { get; set; }
    public bool ShowReplaceButtonInFindOverlay { get; set; }
    /// <summary>
    /// The find overlay refers to hitting the keymap { Ctrl + f } when browser focus is within a text editor.
    /// This property is what the find overlay input element binds to.
    /// </summary>
    public string FindOverlayValue { get; set; }
    /// <summary>
    /// If the user presses the keybind to show the FindOverlayDisplay while focused on the Text Editor,
    /// check if the user has a text selection.
    ///
    /// If they do have a text selection, then populate the FindOverlayDisplay with their selection.
    ///
    /// The issue arises however, how does one know whether FindOverlayValue changed due to
    /// the input element itself being typed into, versus some 'background action'.
    ///
    /// Because the UI already will update properly if the input element itself is interacted with.
    ///
    /// We only need to solve the case where it was a 'background action'.
    ///
    /// So, if this bool toggles to a different value than what the UI last saw,
    /// then the UI is to set the input element's value equal to the 'FindOverlayValue'
    /// because a 'background action' modified the value.
    /// </summary>
    public bool FindOverlayValueExternallyChangedMarker { get; set; }
    public string ReplaceValueInFindOverlay { get; set; }
    
    /// <summary>
	/// This property determines the menu that is shown in the text editor.
	///
	/// For example, when this property is <see cref="MenuKind.AutoCompleteMenu"/>,
	/// then the autocomplete menu is displayed in the text editor.
	/// </summary>
    public MenuKind MenuKind { get; set; }
	/// <summary>
	/// This property determines the tooltip that is shown in the text editor.
	/// </summary>
    public TooltipViewModel? TooltipViewModel { get; set; }
    
    public bool ShouldRevealCursor { get; set; }
    public VirtualAssociativityKind VirtualAssociativityKind { get; set; } = VirtualAssociativityKind.None;
    
    #region DisplayTracker
	/// <summary>
	/// One must track whether the ViewModel is currently being rendered.<br/><br/>
	/// 
	/// The reason for this is that the UI logic is lazily invoked.
	/// That is to say, if a ViewModel has its underlying Model change, BUT the ViewModel is not currently being rendered.
	/// Then that ViewModel does not react to the Model having changed.
	/// </summary>
	
    
    /// <summary>
    /// The initial solution wide parse will no longer apply syntax highlighting.
    ///
    /// As a result, the first time a view model is rendered, it needs to
    /// trigger the syntax highlighting to be applied.
    ///
    /// Preferably this would work with a 'SyntaxHighlightingIsDirty'
    /// sort of pattern.
    ///
    /// I couldn't get 'SyntaxHighlightingIsDirty' working and am tired.
    /// This is too big of an optimization to miss out on
    /// so I'll do the easier answer and leave this note.
    /// </summary>
    private bool _hasBeenDisplayedAtLeastOnceBefore;

    /// <summary>
    /// <see cref="Links"/> refers to a Blazor TextEditorViewModelSlimDisplay having had its OnParametersSet invoked
    /// and the ViewModelKey that was passed as a parameter matches this encompasing ViewModel's key. In this situation
    /// <see cref="Links"/> would be incremented by 1 in a concurrency safe manner.<br/><br/>
    /// 
    /// As well OnParametersSet includes the case where the ViewModelKey that was passed as a parameter is changed.
    /// In this situation the previous ViewModel would have its <see cref="Links"/> decremented by 1 in a concurrency safe manner.<br/><br/>
    /// 
    /// TextEditorViewModelSlimDisplay implements IDisposable. In the Dispose implementation,
    /// the active ViewModel would have its <see cref="Links"/> decremented by 1 in a concurrency safe manner.
    /// </summary>
    public TextEditorComponentData? ComponentData { get; private set; }

    public void RegisterComponentData(TextEditorEditContext editContext, TextEditorComponentData componentData)
    {
    	if (ComponentData is not null)
    	{
    		if (componentData.TextEditorHtmlElementId == ComponentData.TextEditorHtmlElementId)
	    	{
	    		Console.WriteLine($"TODO: {nameof(TextEditorViewModelPersistentState)} {nameof(RegisterComponentData)} - ComponentData is not null (same component tried registering twice)");
	    		return;
	    	}
	    	
    		Console.WriteLine($"TODO: {nameof(TextEditorViewModelPersistentState)} {nameof(RegisterComponentData)} - ComponentData is not null");
    		return;
    	}
    
        ComponentData = componentData;
		TextEditorService.AppDimensionService.AppDimensionStateChanged += AppDimensionStateWrap_StateChanged;

		// Tell the view model what the (already known) font-size measurements and text-editor measurements are.
		PostScrollAndRemeasure();
		
		if (!_hasBeenDisplayedAtLeastOnceBefore)
		{
			_hasBeenDisplayedAtLeastOnceBefore = true;
			
			var modelModifier = editContext.GetModelModifier(ResourceUri);
			if (modelModifier is not null)
			{
				// If this 'ApplySyntaxHighlighting(...)' isn't redundantly invoked prior to
				// the upcoming 'ResourceWasModified(...)' invocation,
				// then there is an obnoxious "flicker" upon opening a file for the first time.
				//
				// This is because it initially opens with 'plain text' syntax highlighting
				// for all the text.
				//
				// Then very soon after it gets the correct syntax highlighting applied.
				// The issue is specifically how quickly it gets the correct syntax highlighting.
				//
				// It is the same issue as putting a 'loading...' icon or text
				// for an asynchronous event, but that event finishes in sub 200ms so the user
				// sees a "flicker" of the 'loading...' text and it just is disorienting to see.
				editContext.TextEditorService.ModelApi.ApplySyntaxHighlighting(
					editContext,
					modelModifier);
				
				if (modelModifier.PersistentState.CompilerService is not null)	
					modelModifier.PersistentState.CompilerService.ResourceWasModified(ResourceUri, Array.Empty<TextEditorTextSpan>());
			}
		}
    }

    public void DisposeComponentData(TextEditorEditContext editContext, TextEditorComponentData componentData)
    {
    	if (componentData is null || ComponentData is null)
    	{
    		Console.WriteLine($"TODO: {nameof(TextEditorViewModelPersistentState)} {nameof(DisposeComponentData)} - componentData is null || ComponentData is null.");
			return;
    	}
    	else if (componentData.TextEditorHtmlElementId != ComponentData.TextEditorHtmlElementId)
    	{
    		Console.WriteLine($"TODO: {nameof(TextEditorViewModelPersistentState)} {nameof(DisposeComponentData)} - ComponentData.TextEditorHtmlElementId does not match.");
			return;
    	}
    
        ComponentData = null;
		TextEditorService.AppDimensionService.AppDimensionStateChanged -= AppDimensionStateWrap_StateChanged;
    }

    private void AppDimensionStateWrap_StateChanged()
    {
    	// The UI was resized, and therefore the text-editor measurements need to be re-measured.
    	//
    	// The font-size is theoretically un-changed,
    	// but will be measured anyway just because its part of the same method that does the text-editor measurements.
		PostScrollAndRemeasure();
    }

	public void PostScrollAndRemeasure()
	{
		var model = TextEditorService.ModelApi.GetOrDefault(ResourceUri);
        var viewModel = TextEditorService.ViewModelApi.GetOrDefault(ViewModelKey);

        if (model is null || viewModel is null)
        {
        	Console.WriteLine("FAIL:PostScrollAndRemeasure()");
            return;
        }

		TextEditorService.WorkerArbitrary.PostUnique(async editContext =>
		{
			var modelModifier = editContext.GetModelModifier(viewModel.PersistentState.ResourceUri);
			var viewModelModifier = editContext.GetViewModelModifier(viewModel.PersistentState.ViewModelKey);
            if (modelModifier is null || viewModelModifier is null)
            {
            	Console.WriteLine("FAIL:PostScrollAndRemeasure()");
                return;
            }
            
            var componentData = viewModel.PersistentState.ComponentData;
            if (componentData is null)
            	return;
			
			var textEditorDimensions = await TextEditorService.ViewModelApi
				.GetTextEditorMeasurementsAsync(componentData.RowSectionElementId)
				.ConfigureAwait(false);
	
			viewModelModifier.TextEditorDimensions = textEditorDimensions;
			viewModelModifier.ShouldCalculateVirtualizationResult = true;
			
			// TODO: Where does the method: 'ValidateMaximumScrollLeftAndScrollTop(...)' belong?
			TextEditorService.ValidateMaximumScrollLeftAndScrollTop(editContext, modelModifier, viewModelModifier, textEditorDimensionsChanged: true);
			
			componentData.Virtualized_LineIndexCache_IsInvalid = true;
		});
	}

    public void Dispose()
    {
    	TextEditorService.WorkerArbitrary.PostUnique(async editContext =>
    	{
    		DisposeComponentData(editContext, ComponentData);
    	});
    }
    #endregion
    
    #region DynamicViewModelAdapterTextEditor
    /// <summary>
	/// This type contains all data, and logic, necessary to render a text editor from within a dialog, a panel tab, or a text editor group tab.
	/// </summary>
    private readonly Type _dragTabComponentType;
    private readonly Dictionary<string, object?>? _dragTabComponentParameterMap;

    private readonly Type? _dragDialogComponentType = null;
    private readonly Dictionary<string, object?>? _dragDialogComponentParameterMap = null;

    public IPanelService PanelService { get; }
    public IDialogService DialogService { get; }
    public CommonBackgroundTaskApi CommonBackgroundTaskApi { get; }

    public Key<Panel> Key { get; }
    public Key<ContextRecord> ContextRecordKey { get; }
    public Key<IDynamicViewModel> DynamicViewModelKey { get; } = Key<IDynamicViewModel>.NewKey();
	public string? SetFocusOnCloseElementId { get; set; }

    public ITabGroup? TabGroup { get; set; }

    public string Title => GetTitle();

	public string TitleVerbose =>
		TextEditorService.ViewModelApi.GetModelOrDefault(ViewModelKey)?.PersistentState.ResourceUri.Value
			?? Title;

    public Type ComponentType { get; }

    public Dictionary<string, object?>? ComponentParameterMap { get; }

    public string? DialogCssClass { get; set; }
    public string? DialogCssStyle { get; set; }
    public ElementDimensions DialogElementDimensions { get; set; } = DialogHelper.ConstructDefaultElementDimensions();
    public bool DialogIsMinimized { get; set; }
    public bool DialogIsMaximized { get; set; }
    public bool DialogIsResizable { get; set; } = true;
    public string DialogFocusPointHtmlElementId { get; init; }
    public List<IDropzone> DropzoneList { get; set; }

    public ElementDimensions DragElementDimensions { get; set; } = DialogHelper.ConstructDefaultElementDimensions();

    public Type DragComponentType => TabGroup is null
        ? _dragDialogComponentType
        : _dragTabComponentType;

    public Dictionary<string, object?>? DragComponentParameterMap => TabGroup is null
        ? _dragDialogComponentParameterMap
        : _dragTabComponentParameterMap;

    public string? DragCssClass { get; set; }
    public string? DragCssStyle { get; set; }

	public IDialog SetDialogIsMaximized(bool isMaximized)
	{
		DialogIsMaximized = isMaximized;
		return this;
	}

    private string GetTitle()
    {
        if (TextEditorService is null)
            return "TextEditorService was null";

        var model = TextEditorService.ViewModelApi.GetModelOrDefault(ViewModelKey);
        var viewModel = TextEditorService.ViewModelApi.GetOrDefault(ViewModelKey);

        if (viewModel is null)
        {
            return "ViewModel not found";
        }
        else if (model is null)
        {
            return "Model not found";
        }
        else
        {
            var displayName = viewModel.PersistentState.GetTabDisplayNameFunc?.Invoke(model)
                ?? model.PersistentState.ResourceUri.Value;

            if (model.IsDirty)
                displayName += '*';

            return displayName;
        }
    }

    public async Task OnDragStartAsync()
    {
        if (TextEditorService is null)
            return;

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
        await AddPanelDropzonesAsync(dropzoneList).ConfigureAwait(false);

        var measuredHtmlElementDimensions = await CommonBackgroundTaskApi.JsRuntimeCommonApi
            .MeasureElementById(
                $"luth_te_group_{TextEditorService.GroupApi.GetTextEditorGroupState().GroupList.Single().GroupKey.Guid}")
            .ConfigureAwait(false);

        measuredHtmlElementDimensions = measuredHtmlElementDimensions with
        {
            ZIndex = 1,
        };

        var elementDimensions = new ElementDimensions();

        elementDimensions.ElementPositionKind = ElementPositionKind.Fixed;

        // Width
        {
            elementDimensions.WidthDimensionAttribute.DimensionUnitList.Clear();
            elementDimensions.WidthDimensionAttribute.DimensionUnitList.Add(new DimensionUnit(
            	measuredHtmlElementDimensions.WidthInPixels,
            	DimensionUnitKind.Pixels));
        }

        // Height
        {
            elementDimensions.HeightDimensionAttribute.DimensionUnitList.Clear();
            elementDimensions.HeightDimensionAttribute.DimensionUnitList.Add(new DimensionUnit(
            	measuredHtmlElementDimensions.HeightInPixels,
            	DimensionUnitKind.Pixels));
        }

        // Left
        {
            elementDimensions.LeftDimensionAttribute.DimensionUnitList.Clear();
            elementDimensions.LeftDimensionAttribute.DimensionUnitList.Add(new DimensionUnit(
            	measuredHtmlElementDimensions.LeftInPixels,
            	DimensionUnitKind.Pixels));
        }

        // Top
        {
            elementDimensions.TopDimensionAttribute.DimensionUnitList.Clear();
            elementDimensions.TopDimensionAttribute.DimensionUnitList.Add(new DimensionUnit(
            	measuredHtmlElementDimensions.TopInPixels,
            	DimensionUnitKind.Pixels));
        }

        dropzoneList.Add(new TextEditorGroupDropzone(
            measuredHtmlElementDimensions,
            TextEditorService.GroupApi.GetTextEditorGroupState().GroupList.Single().GroupKey,
            elementDimensions));

        DropzoneList = dropzoneList;
    }

    public Task OnDragEndAsync(MouseEventArgs mouseEventArgs, IDropzone? dropzone)
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

                DialogService.ReduceRegisterAction(this);
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
                        DialogService.ReduceDisposeAction(DynamicViewModelKey);
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
                    DialogService.ReduceDisposeAction(DynamicViewModelKey);
                }

                TabGroup = null;
            }

            PanelService.RegisterPanelTab(
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
                    },
                    PanelService,
                    DialogService,
                    CommonBackgroundTaskApi),
                true);

            return Task.CompletedTask;
        }
        else
        {
            return Task.CompletedTask;
        }
    }

    private void AddFallbackDropzone(List<IDropzone> dropzoneList)
    {
        var fallbackElementDimensions = new ElementDimensions();

        fallbackElementDimensions.ElementPositionKind = ElementPositionKind.Fixed;

        // Width
        {
            fallbackElementDimensions.WidthDimensionAttribute.DimensionUnitList.Clear();
            fallbackElementDimensions.WidthDimensionAttribute.DimensionUnitList.Add(new DimensionUnit(
            	100,
            	DimensionUnitKind.ViewportWidth));
        }

        // Height
        {
            fallbackElementDimensions.HeightDimensionAttribute.DimensionUnitList.Clear();
            fallbackElementDimensions.HeightDimensionAttribute.DimensionUnitList.Add(new DimensionUnit(
            	100,
            	DimensionUnitKind.ViewportHeight));
        }

        // Left
        {
            fallbackElementDimensions.LeftDimensionAttribute.DimensionUnitList.Clear();
            fallbackElementDimensions.LeftDimensionAttribute.DimensionUnitList.Add(new DimensionUnit(
            	0,
            	DimensionUnitKind.Pixels));
        }

        // Top
        {
            fallbackElementDimensions.TopDimensionAttribute.DimensionUnitList.Clear();
            fallbackElementDimensions.TopDimensionAttribute.DimensionUnitList.Add(new DimensionUnit(
            	0,
            	DimensionUnitKind.Pixels));
        }

        dropzoneList.Add(new TextEditorGroupDropzone(
            new MeasuredHtmlElementDimensions(0, 0, 0, 0, 0),
            Key<TextEditorGroup>.Empty,
            fallbackElementDimensions)
        {
            CssClass = "luth_dropzone-fallback"
        });
    }

    private async Task AddPanelDropzonesAsync(List<IDropzone> dropzoneList)
    {
        var panelGroupHtmlIdTupleList = new (Key<PanelGroup> PanelGroupKey, string HtmlElementId)[]
        {
            (PanelFacts.LeftPanelGroupKey, "luth_ide_panel_left_tabs"),
            (PanelFacts.RightPanelGroupKey, "luth_ide_panel_right_tabs"),
            (PanelFacts.BottomPanelGroupKey, "luth_ide_panel_bottom_tabs"),
        };

        foreach (var panelGroupHtmlIdTuple in panelGroupHtmlIdTupleList)
        {
            var measuredHtmlElementDimensions = await CommonBackgroundTaskApi.JsRuntimeCommonApi
                .MeasureElementById(panelGroupHtmlIdTuple.HtmlElementId)
                .ConfigureAwait(false);

            measuredHtmlElementDimensions = measuredHtmlElementDimensions with
            {
                ZIndex = 1,
            };

            var elementDimensions = new ElementDimensions();

            elementDimensions.ElementPositionKind = ElementPositionKind.Fixed;

            // Width
            {
                elementDimensions.WidthDimensionAttribute.DimensionUnitList.Clear();
                elementDimensions.WidthDimensionAttribute.DimensionUnitList.Add(new DimensionUnit(
                	measuredHtmlElementDimensions.WidthInPixels,
                	DimensionUnitKind.Pixels));
            }

            // Height
            {
                elementDimensions.HeightDimensionAttribute.DimensionUnitList.Clear();
                elementDimensions.HeightDimensionAttribute.DimensionUnitList.Add(new DimensionUnit(
                	measuredHtmlElementDimensions.HeightInPixels,
                	DimensionUnitKind.Pixels));
            }

            // Left
            {
                elementDimensions.LeftDimensionAttribute.DimensionUnitList.Clear();
                elementDimensions.LeftDimensionAttribute.DimensionUnitList.Add(new DimensionUnit(
                	measuredHtmlElementDimensions.LeftInPixels,
                	DimensionUnitKind.Pixels));
            }

            // Top
            {
                elementDimensions.TopDimensionAttribute.DimensionUnitList.Clear();
                elementDimensions.TopDimensionAttribute.DimensionUnitList.Add(new DimensionUnit(
                	measuredHtmlElementDimensions.TopInPixels,
                	DimensionUnitKind.Pixels));
            }

            dropzoneList.Add(new PanelGroupDropzone(
                measuredHtmlElementDimensions,
                panelGroupHtmlIdTuple.PanelGroupKey,
                elementDimensions,
                Key<IDropzone>.NewKey(),
                null,
                null));
        }
    }
    #endregion
}
