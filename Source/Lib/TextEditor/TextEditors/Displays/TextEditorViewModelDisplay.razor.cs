using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Web;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.Common.RazorLib.Clipboards.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.TextEditor.RazorLib.Edits.Models;
using Luthetus.TextEditor.RazorLib.Autocompletes.Models;
using Luthetus.TextEditor.RazorLib.ComponentRenderers.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Options.Models;
using Luthetus.TextEditor.RazorLib.Commands.Models.Defaults;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.Events.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.FindAlls.Models;

// GutterDriver.cs
using Luthetus.TextEditor.RazorLib.Virtualizations.Models;
using Luthetus.TextEditor.RazorLib.Characters.Models;

// BodyDriver.cs
using System.Text;

// CursorDriver.cs
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.TextEditor.RazorLib.Keymaps.Models;
using Luthetus.TextEditor.RazorLib.Exceptions;

// ScrollbarSection.razor.cs
using Luthetus.Common.RazorLib.Drags.Models;

// VirtualizationDriver.cs

// PresentationAndSelectionDriver.cs
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays;

public sealed partial class TextEditorViewModelDisplay : ComponentBase, IDisposable
{
    [Inject]
    private IAppOptionsService AppOptionsService { get; set; } = null!;
    [Inject]
    public IServiceProvider ServiceProvider { get; set; } = null!;
    [Inject]
    public ITextEditorService TextEditorService { get; set; } = null!;
    [Inject]
    public IDirtyResourceUriService DirtyResourceUriService { get; set; } = null!;
    [Inject]
    public IAutocompleteIndexer AutocompleteIndexer { get; set; } = null!;
    [Inject]
    public IJSRuntime JsRuntime { get; set; } = null!;
    [Inject]
    public IClipboardService ClipboardService { get; set; } = null!;
    [Inject]
    public IBackgroundTaskService BackgroundTaskService { get; set; } = null!;
    [Inject]
    public ILuthetusTextEditorComponentRenderers TextEditorComponentRenderers { get; set; } = null!;
    [Inject]
    public ICommonComponentRenderers CommonComponentRenderers { get; set; } = null!;
    [Inject]
    public INotificationService NotificationService { get; set; } = null!;
    [Inject]
    public IEnvironmentProvider EnvironmentProvider { get; set; } = null!;
    [Inject]
    public IDialogService DialogService { get; set; } = null!;
    [Inject]
    public IDropdownService DropdownService { get; set; } = null!;
    [Inject]
    public LuthetusTextEditorConfig TextEditorConfig { get; set; } = null!;
    [Inject]
    private IFindAllService FindAllService { get; set; } = null!;
    // ScrollbarSection.razor.cs
    [Inject]
    private IDragService DragService { get; set; } = null!;

    [Parameter, EditorRequired]
    public Key<TextEditorViewModel> TextEditorViewModelKey { get; set; } = Key<TextEditorViewModel>.Empty;
    
    [Parameter]
    public ViewModelDisplayOptions ViewModelDisplayOptions { get; set; } = new();

    private readonly Guid _textEditorHtmlElementId = Guid.NewGuid();
    /// <summary>Using this lock in order to avoid the Dispose implementation decrementing when it shouldn't</summary>
    private readonly object _linkedViewModelLock = new();
    // private readonly ThrottleAvailability _throttleAvailabilityShouldRender = new(TimeSpan.FromMilliseconds(30));

	private double _previousGutterWidthInPixels = 0;

    private TextEditorComponentData _componentData = null!;
    
    // Active is the one given to the UI after the current was validated and found to be valid.
    public TextEditorRenderBatch _currentRenderBatch;
    private TextEditorRenderBatch? _previousRenderBatch;
    public TextEditorRenderBatch? _activeRenderBatch;
    
    private TextEditorViewModel? _linkedViewModel;
    
    private Dictionary<string, object?> DependentComponentParameters;
    
    private bool _thinksTouchIsOccurring;
    private DateTime? _touchStartDateTime = null;
    private TouchEventArgs? _previousTouchEventArgs = null;
    private bool _userMouseIsInside;
    
    /// <summary>
    /// Share this StringBuilder when used for rendering and no other function is currently using it.
    /// (i.e.: only use this for methods that were invoked from the .razor file)
    /// </summary>
    private StringBuilder _uiStringBuilder = new();
    
    /* MeasureCharacterWidthAndRowHeight.razor Open */
    private const string TEST_STRING_FOR_MEASUREMENT = "abcdefghijklmnopqrstuvwxyz0123456789";
    private const int TEST_STRING_REPEAT_COUNT = 6;
    public int COUNT_OF_TEST_CHARACTERS => TEST_STRING_REPEAT_COUNT * TEST_STRING_FOR_MEASUREMENT.Length;
    /* MeasureCharacterWidthAndRowHeight.razor Close */

    private string MeasureCharacterWidthAndRowHeightElementId => $"luth_te_measure-character-width-and-row-height_{_textEditorHtmlElementId}";
    private string ContentElementId => $"luth_te_text-editor-content_{_textEditorHtmlElementId}";

	public TextEditorComponentData ComponentData => _componentData;
	
	/// <summary>
	/// Any external UI that isn't a child component of this can subscribe to this event,
	/// and then synchronize with what the text editor is rendering.<br/><br/>
	///
	/// A result of this, is that one can have 'extra' components that are low priority rendering.
	/// Because one can throttle the rendering of those low priority components, and they
	/// will be up to date eventually regardless of how long the throttle is.<br/><br/>
	/// </summary>
	public event Action? RenderBatchChanged;

    protected override void OnInitialized()
    {
    	DependentComponentParameters = new()
    	{
    		{
    			nameof(TextEditorViewModelDisplay),
    			this
    		}
    	};
    	
    	VERTICAL_ScrollbarElementId = $"luth_te_{VERTICAL_scrollbarGuid}";
	    VERTICAL_ScrollbarSliderElementId = $"luth_te_{VERTICAL_scrollbarGuid}-slider";
	    
	    HORIZONTAL_ScrollbarElementId = $"luth_te_{HORIZONTAL_scrollbarGuid}";
	    HORIZONTAL_ScrollbarSliderElementId = $"luth_te_{HORIZONTAL_scrollbarGuid}-slider";

        ConstructRenderBatch();

		SetComponentData();

        TextEditorService.TextEditorStateChanged += GeneralOnStateChangedEventHandler;
        TextEditorService.OptionsApi.TextEditorOptionsStateChanged += TextEditorOptionsStateWrap_StateChanged;
        TextEditorService.ViewModelApi.CursorShouldBlinkChanged += ViewModel_CursorShouldBlinkChanged;
        
        // ScrollbarSection.razor.cs
        DragService.DragStateChanged += DragStateWrapOnStateChanged;

        base.OnInitialized();
    }
    
    protected override void OnParametersSet()
    {
    	HandleTextEditorViewModelKeyChange();
    	base.OnParametersSet();
    }

    protected override bool ShouldRender()
    {
        var shouldRender = true;

        if (_linkedViewModel is null)
            HandleTextEditorViewModelKeyChange();

        if (shouldRender)
            ConstructRenderBatch();

        if (_currentRenderBatch.ViewModel is not null && _currentRenderBatch.Options is not null)
        {
            var isFirstDisplay = _currentRenderBatch.ViewModel.DisplayTracker.ConsumeIsFirstDisplay();

            var previousOptionsRenderStateKey = _previousRenderBatch.Options?.RenderStateKey ?? Key<RenderState>.Empty;
            var currentOptionsRenderStateKey = _currentRenderBatch.Options.RenderStateKey;

			if (previousOptionsRenderStateKey != currentOptionsRenderStateKey || isFirstDisplay)
            {
                QueueRemeasureBackgroundTask(
                    _currentRenderBatch,
                    MeasureCharacterWidthAndRowHeightElementId,
                    COUNT_OF_TEST_CHARACTERS,
                    CancellationToken.None);
            }

            if (isFirstDisplay)
				QueueCalculateVirtualizationResultBackgroundTask(_currentRenderBatch);
        }
        
        // Check if the gutter width changed. If so, re-measure text editor.
        var viewModel = _activeRenderBatch?.ViewModel;
        var gutterWidthInPixels = _activeRenderBatch?.GutterWidthInPixels;
        
        if (viewModel is not null && gutterWidthInPixels is not null)
		{
			if (_previousGutterWidthInPixels >= 0 && gutterWidthInPixels >= 0)
			{
	        	var absoluteValueDifference = Math.Abs(_previousGutterWidthInPixels - gutterWidthInPixels.Value);
	        	
	        	if (absoluteValueDifference >= 0.5)
	        	{
	        		_previousGutterWidthInPixels = gutterWidthInPixels.Value;
	        		viewModel.DisplayTracker.PostScrollAndRemeasure();
	        	}
			}
		}

        return shouldRender;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await TextEditorService.JsRuntimeTextEditorApi
                .PreventDefaultOnWheelEvents(ContentElementId)
                .ConfigureAwait(false);

            QueueRemeasureBackgroundTask(
                _currentRenderBatch,
                MeasureCharacterWidthAndRowHeightElementId,
                COUNT_OF_TEST_CHARACTERS,
                CancellationToken.None);

            QueueCalculateVirtualizationResultBackgroundTask(_currentRenderBatch);
        }

        if (_currentRenderBatch.ViewModel is not null && _currentRenderBatch.ViewModel.UnsafeState.ShouldSetFocusAfterNextRender)
        {
            _currentRenderBatch.ViewModel.UnsafeState.ShouldSetFocusAfterNextRender = false;
            // await FocusTextEditorAsync().ConfigureAwait(false);
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    private void ConstructRenderBatch()
    {
    	var localTextEditorState = TextEditorService.TextEditorState;
    
    	var model_viewmodel_tuple = localTextEditorState.GetModelAndViewModelOrDefault(
			TextEditorViewModelKey);
				
        var renderBatchUnsafe = new TextEditorRenderBatch(
            model_viewmodel_tuple.Model,
            model_viewmodel_tuple.ViewModel,
            TextEditorService.OptionsApi.GetTextEditorOptionsState().Options,
            TextEditorRenderBatch.DEFAULT_FONT_FAMILY,
            TextEditorOptionsState.DEFAULT_FONT_SIZE_IN_PIXELS,
            ViewModelDisplayOptions,
			_componentData);

        if (!string.IsNullOrWhiteSpace(renderBatchUnsafe.Options?.CommonOptions?.FontFamily))
        	renderBatchUnsafe.FontFamily = renderBatchUnsafe.Options!.CommonOptions!.FontFamily;

        if (renderBatchUnsafe.Options!.CommonOptions?.FontSizeInPixels is not null)
            renderBatchUnsafe.FontSizeInPixels = renderBatchUnsafe.Options!.CommonOptions.FontSizeInPixels;
        
        /*if (renderBatchUnsafe.ViewModelDisplayOptions.KeymapOverride is not null)
            renderBatchUnsafe.Options.Keymap = renderBatchUnsafe.ViewModelDisplayOptions.KeymapOverride;*/

        _previousRenderBatch = _currentRenderBatch;
        
        _currentRenderBatch = renderBatchUnsafe;
        
        _activeRenderBatch = renderBatchUnsafe.Validate() ? renderBatchUnsafe : null;
        
        RenderBatchChanged?.Invoke();
    }
    
    private void SetComponentData()
    {
		_componentData = new(
			_textEditorHtmlElementId,
			ViewModelDisplayOptions,
			_currentRenderBatch.Options,
			this,
			DropdownService,
			ClipboardService,
			CommonComponentRenderers,
			NotificationService,
			TextEditorService,
			TextEditorComponentRenderers,
			FindAllService,
			ServiceProvider);
    }

    private async void GeneralOnStateChangedEventHandler() =>
        await InvokeAsync(StateHasChanged);
        
    private async void TextEditorOptionsStateWrap_StateChanged()
    {
    	/*if (TextEditorService.OptionsApi.GetTextEditorOptionsState().Options.Keymap.Key != _componentData.Options.Keymap.Key)
    	{
    		ConstructRenderBatch();
    		SetComponentData();
    	}*/
    	
    	await InvokeAsync(StateHasChanged);
    }
        
    private async void ViewModel_CursorShouldBlinkChanged() =>
        await InvokeAsync(StateHasChanged);

    private void HandleTextEditorViewModelKeyChange()
    {
        lock (_linkedViewModelLock)
        {
            var localTextEditorViewModelKey = TextEditorViewModelKey;

            var nextViewModel = TextEditorService.TextEditorState.ViewModelGetOrDefault(localTextEditorViewModelKey);

            Key<TextEditorViewModel> nextViewModelKey;

            if (nextViewModel is null)
                nextViewModelKey = Key<TextEditorViewModel>.Empty;
            else
                nextViewModelKey = nextViewModel.ViewModelKey;

            var linkedViewModelKey = _linkedViewModel?.ViewModelKey ?? Key<TextEditorViewModel>.Empty;
            var viewKeyChanged = nextViewModelKey != linkedViewModelKey;

            if (viewKeyChanged)
            {
                _linkedViewModel?.DisplayTracker.DecrementLinks();
                nextViewModel?.DisplayTracker.IncrementLinks();

                _linkedViewModel = nextViewModel;

                if (nextViewModel is not null)
                    nextViewModel.UnsafeState.ShouldRevealCursor = true;
            }
        }
    }

  public async Task FocusTextEditorAsync()
  {
  	var localTextEditorViewModelKey = TextEditorViewModelKey;

      var nextViewModel = TextEditorService.TextEditorState.ViewModelGetOrDefault(localTextEditorViewModelKey);
      
      if (nextViewModel is not null)
      	await nextViewModel.FocusAsync();
  }

    private string GetGlobalHeightInPixelsStyling()
    {
        var heightInPixels = TextEditorService.OptionsApi.GetTextEditorOptionsState().Options.TextEditorHeightInPixels;

        if (heightInPixels is null)
            return string.Empty;

        var heightInPixelsInvariantCulture = heightInPixels.Value.ToCssValue();
        
        _uiStringBuilder.Clear();
        _uiStringBuilder.Append("height: ");
        _uiStringBuilder.Append(heightInPixelsInvariantCulture);
        _uiStringBuilder.Append("px;");

        return _uiStringBuilder.ToString();
    }
    
    private void ReceiveOnKeyDown(KeyboardEventArgs keyboardEventArgs)
    {
        if (EventUtils.IsKeyboardEventArgsNoise(keyboardEventArgs))
            return;
        
        var (model, viewModel) = TextEditorService.TextEditorState.GetModelAndViewModelOrDefault(
        	TextEditorViewModelKey);

        var resourceUri = model?.ResourceUri ?? ResourceUri.Empty;
        var viewModelKey = viewModel?.ViewModelKey ?? Key<TextEditorViewModel>.Empty;

        if (resourceUri == ResourceUri.Empty ||
            viewModelKey == Key<TextEditorViewModel>.Empty)
        {
			return;
		}

		var onKeyDown = new OnKeyDown(
			_componentData,
            new KeymapArgs(keyboardEventArgs),
            resourceUri,
            viewModelKey);

        TextEditorService.TextEditorWorker.EnqueueOnKeyDown(onKeyDown);
	}

    private void ReceiveOnContextMenu()
    {
		var localViewModelKey = TextEditorViewModelKey;

		TextEditorService.TextEditorWorker.PostUnique(
			nameof(ReceiveOnContextMenu),
			editContext =>
			{
				var viewModelModifier = editContext.GetViewModelModifier(localViewModelKey);
				
				var modelModifier = editContext.GetModelModifier(viewModelModifier.ViewModel.ResourceUri);
				
				var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier.ViewModel);
        		var primaryCursor = editContext.GetPrimaryCursorModifier(cursorModifierBag);

				if (modelModifier is null || viewModelModifier is null || !cursorModifierBag.ConstructorWasInvoked || primaryCursor is null)
					return ValueTask.CompletedTask;

				TextEditorCommandDefaultFunctions.ShowContextMenu(
			        editContext,
			        modelModifier,
			        viewModelModifier,
			        cursorModifierBag,
			        primaryCursor,
			        DropdownService,
			        ComponentData);
				
				return ValueTask.CompletedTask;
			});
    }

    private void ReceiveOnDoubleClick(MouseEventArgs mouseEventArgs)
    {
        var (model, viewModel) = TextEditorService.TextEditorState.GetModelAndViewModelOrDefault(
            TextEditorViewModelKey);

        var resourceUri = model?.ResourceUri ?? ResourceUri.Empty;
        var viewModelKey = viewModel?.ViewModelKey ?? Key<TextEditorViewModel>.Empty;

        if (resourceUri == ResourceUri.Empty ||
            viewModelKey == Key<TextEditorViewModel>.Empty)
        {
            return;
        }

        var onDoubleClick = new OnDoubleClick(
            mouseEventArgs,
			_componentData,
            resourceUri,
            viewModelKey);

        TextEditorService.TextEditorWorker.EnqueueOnDoubleClick(onDoubleClick);
    }

    private void ReceiveContentOnMouseDown(MouseEventArgs mouseEventArgs)
    {
        _componentData.ThinksLeftMouseButtonIsDown = true;

        var (model, viewModel) = TextEditorService.TextEditorState.GetModelAndViewModelOrDefault(
            TextEditorViewModelKey);

        var resourceUri = model?.ResourceUri ?? ResourceUri.Empty;
        var viewModelKey = viewModel?.ViewModelKey ?? Key<TextEditorViewModel>.Empty;

        if (resourceUri == ResourceUri.Empty ||
            viewModelKey == Key<TextEditorViewModel>.Empty)
        {
            return;
        }

        if (viewModel is not null)
            viewModel.UnsafeState.ShouldRevealCursor = false;

		var onMouseDown = new OnMouseDown(
            mouseEventArgs,
			_componentData,
            resourceUri,
            viewModelKey);

        TextEditorService.TextEditorWorker.EnqueueOnMouseDown(onMouseDown);
    }

    private void ReceiveContentOnMouseMove(MouseEventArgs mouseEventArgs)
    {
        _userMouseIsInside = true;

        if ((mouseEventArgs.Buttons & 1) == 0)
            _componentData.ThinksLeftMouseButtonIsDown = false;

        var localThinksLeftMouseButtonIsDown = _componentData.ThinksLeftMouseButtonIsDown;

        var (model, viewModel) = TextEditorService.TextEditorState.GetModelAndViewModelOrDefault(
            TextEditorViewModelKey);

        var resourceUri = model?.ResourceUri ?? ResourceUri.Empty;
        var viewModelKey = viewModel?.ViewModelKey ?? Key<TextEditorViewModel>.Empty;

        if (resourceUri == ResourceUri.Empty ||
            viewModelKey == Key<TextEditorViewModel>.Empty)
        {
            return;
        }

        // MouseStoppedMovingEvent
        if (viewModel is not null)
        {
            // Hide the tooltip, if the user moves their cursor out of the tooltips UI.
            if (viewModel.TooltipViewModel is not null && _componentData.MouseNoLongerOverTooltipTask.IsCompleted)
            {
                var mouseNoLongerOverTooltipCancellationToken = _componentData.MouseNoLongerOverTooltipCancellationTokenSource.Token;

                _componentData.MouseNoLongerOverTooltipTask = Task.Run(async () =>
                {
                    await Task.Delay(TextEditorComponentData.OnMouseOutTooltipDelay, mouseNoLongerOverTooltipCancellationToken).ConfigureAwait(false);

                    if (!mouseNoLongerOverTooltipCancellationToken.IsCancellationRequested)
                    {
						TextEditorService.TextEditorWorker.PostUnique(
							nameof(ContextMenu),
							editContext =>
							{
								var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);

                                if (viewModelModifier is null)
                                    return ValueTask.CompletedTask;

                                viewModelModifier.ViewModel = viewModelModifier.ViewModel with 
								{
									TooltipViewModel = null
								};

								return ValueTask.CompletedTask;
							});
                    }
                });
            }

            _componentData.MouseStoppedMovingCancellationTokenSource.Cancel();
            _componentData.MouseStoppedMovingCancellationTokenSource = new();

            var mouseStoppedMovingCancellationToken = _componentData.MouseStoppedMovingCancellationTokenSource.Token;

            _componentData.MouseStoppedMovingTask = Task.Run(async () =>
            {
                await Task.Delay(TextEditorComponentData.MouseStoppedMovingDelay, mouseStoppedMovingCancellationToken).ConfigureAwait(false);

                if (!mouseStoppedMovingCancellationToken.IsCancellationRequested && _userMouseIsInside)
                {
                    await _componentData.ContinueRenderingTooltipAsync().ConfigureAwait(false);

                    TextEditorService.TextEditorWorker.PostUnique(
						nameof(TextEditorCommandDefaultFunctions.HandleMouseStoppedMovingEventAsync),
						editContext =>
						{
							var modelModifier = editContext.GetModelModifier(resourceUri);
			                var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);
			                var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
			                var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);
			
			                if (modelModifier is null || viewModelModifier is null || !cursorModifierBag.ConstructorWasInvoked || primaryCursorModifier is null)
			                    return ValueTask.CompletedTask;
						
							return TextEditorCommandDefaultFunctions.HandleMouseStoppedMovingEventAsync(
								editContext,
								modelModifier,
								viewModelModifier,
								mouseEventArgs,		
								_componentData,
								TextEditorComponentRenderers,
						        resourceUri);
						});
                }
            });
        }

        if (!_componentData.ThinksLeftMouseButtonIsDown)
            return;

        if (resourceUri == ResourceUri.Empty ||
        	viewModelKey == Key<TextEditorViewModel>.Empty)
        {
            return;
        }

        // Buttons is a bit flag '& 1' gets if left mouse button is held
        if (localThinksLeftMouseButtonIsDown && (mouseEventArgs.Buttons & 1) == 1)
        {
			var onMouseMove = new OnMouseMove(
                mouseEventArgs,
                _componentData,
                resourceUri,
                viewModelKey);

            TextEditorService.TextEditorWorker.EnqueueOnMouseMove(onMouseMove);
        }
        else
        {
            _componentData.ThinksLeftMouseButtonIsDown = false;
        }
    }

    private void ReceiveContentOnMouseOut(MouseEventArgs mouseEventArgs)
    {
        _userMouseIsInside = false;
    }
    
    private void ReceiveOnWheel(WheelEventArgs wheelEventArgs)
    {
    	var viewModel = TextEditorService.TextEditorState.ViewModelGetOrDefault(
        	TextEditorViewModelKey);
		
        var viewModelKey = viewModel?.ViewModelKey ?? Key<TextEditorViewModel>.Empty;
        if (viewModelKey == Key<TextEditorViewModel>.Empty)
            return;

		var onWheel = new OnWheel(
            wheelEventArgs,
            _componentData,
            viewModelKey);

        TextEditorService.TextEditorWorker.EnqueueOnWheel(onWheel);
    }

    private void ReceiveOnTouchStart(TouchEventArgs touchEventArgs)
    {
        _touchStartDateTime = DateTime.UtcNow;

        _previousTouchEventArgs = touchEventArgs;
        _thinksTouchIsOccurring = true;
    }

    private void ReceiveOnTouchMove(TouchEventArgs touchEventArgs)
    {
        var localThinksTouchIsOccurring = _thinksTouchIsOccurring;

        if (!_thinksTouchIsOccurring)
             return;

        var previousTouchPoint = _previousTouchEventArgs?.ChangedTouches.FirstOrDefault(x => x.Identifier == 0);
        var currentTouchPoint = touchEventArgs.ChangedTouches.FirstOrDefault(x => x.Identifier == 0);

        if (previousTouchPoint is null || currentTouchPoint is null)
             return;

        var viewModel = TextEditorService.TextEditorState.ViewModelGetOrDefault(TextEditorViewModelKey);
        if (viewModel is null)
			return;

        // Natural scrolling for touch devices
        var diffX = previousTouchPoint.ClientX - currentTouchPoint.ClientX;
        var diffY = previousTouchPoint.ClientY - currentTouchPoint.ClientY;

        TextEditorService.TextEditorWorker.PostUnique(
            nameof(ReceiveOnTouchMove),
            editContext =>
			{
				var viewModelModifier = editContext.GetViewModelModifier(viewModel.ViewModelKey);
				
				if (viewModelModifier is null)
					return ValueTask.CompletedTask;
				
                TextEditorService.ViewModelApi.MutateScrollHorizontalPosition(
                	editContext,
			        viewModelModifier,
                	diffX);

                TextEditorService.ViewModelApi.MutateScrollVerticalPosition(
                	editContext,
			        viewModelModifier,
                	diffY);
                	
                return ValueTask.CompletedTask;
			});

        _previousTouchEventArgs = touchEventArgs;
    }
    
    private void ClearTouch(TouchEventArgs touchEventArgs)
    {
        var rememberStartTouchEventArgs = _previousTouchEventArgs;

        _thinksTouchIsOccurring = false;
        _previousTouchEventArgs = null;

        var clearTouchDateTime = DateTime.UtcNow;
        var touchTimespan = clearTouchDateTime - _touchStartDateTime;

        if (touchTimespan is null)
            return;

        if (touchTimespan.Value.TotalMilliseconds < 200)
        {
            var startTouchPoint = rememberStartTouchEventArgs?.ChangedTouches.FirstOrDefault(x => x.Identifier == 0);

            if (startTouchPoint is null)
                return;

            ReceiveContentOnMouseDown(new MouseEventArgs
            {
                Buttons = 1,
                ClientX = startTouchPoint.ClientX,
                ClientY = startTouchPoint.ClientY,
            });
        }
    }

    private void QueueRemeasureBackgroundTask(
        TextEditorRenderBatch localRefCurrentRenderBatch,
        string localMeasureCharacterWidthAndRowHeightElementId,
        int countOfTestCharacters,
        CancellationToken cancellationToken)
    {
        var (model, viewModel) = TextEditorService.TextEditorState.GetModelAndViewModelOrDefault(
            TextEditorViewModelKey);

        var resourceUri = model?.ResourceUri ?? ResourceUri.Empty;
        var viewModelKey = viewModel?.ViewModelKey ?? Key<TextEditorViewModel>.Empty;

        if (resourceUri == ResourceUri.Empty ||
            viewModelKey == Key<TextEditorViewModel>.Empty)
        {
            return;
        }

        TextEditorService.TextEditorWorker.PostRedundant(
            nameof(QueueRemeasureBackgroundTask),
			resourceUri,
			viewModelKey,
            editContext =>
            {
            	var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);

				if (viewModelModifier is null)
					return ValueTask.CompletedTask;

            	return TextEditorService.ViewModelApi.RemeasureAsync(
            		editContext,
			        viewModelModifier,
			        localMeasureCharacterWidthAndRowHeightElementId,
	                countOfTestCharacters,
			        CancellationToken.None);
	        });
    }

    private void QueueCalculateVirtualizationResultBackgroundTask(
		TextEditorRenderBatch localCurrentRenderBatch)
    {
        var (model, viewModel) = TextEditorService.TextEditorState.GetModelAndViewModelOrDefault(
            TextEditorViewModelKey);

        var resourceUri = model?.ResourceUri ?? ResourceUri.Empty;
        var viewModelKey = viewModel?.ViewModelKey ?? Key<TextEditorViewModel>.Empty;

        if (resourceUri == ResourceUri.Empty ||
            viewModelKey == Key<TextEditorViewModel>.Empty)
        {
            return;
        }

        TextEditorService.TextEditorWorker.PostRedundant(
            nameof(QueueCalculateVirtualizationResultBackgroundTask),
			resourceUri,
			viewModelKey,
            editContext =>
            {
            	var modelModifier = editContext.GetModelModifier(resourceUri);
            	var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);

				if (modelModifier is null || viewModelModifier is null)
					return ValueTask.CompletedTask;
            	
            	TextEditorService.ViewModelApi.CalculateVirtualizationResult(
            		editContext,
			        modelModifier,
			        viewModelModifier,
			        CancellationToken.None);
			    return ValueTask.CompletedTask;
            });
    }
    
    #region GutterDriverOpen

    public string GetGutterStyleCss(TextEditorRenderBatch renderBatchLocal, int index)
    {
    	_uiStringBuilder.Clear();
    
        var measurements = renderBatchLocal.ViewModel.CharAndLineMeasurements;

        var topInPixelsInvariantCulture = (index * measurements.LineHeight).ToCssValue();
        _uiStringBuilder.Append("top: ");
        _uiStringBuilder.Append(topInPixelsInvariantCulture);
        _uiStringBuilder.Append("px;");

        var heightInPixelsInvariantCulture = measurements.LineHeight.ToCssValue();
        _uiStringBuilder.Append("height: ");
        _uiStringBuilder.Append(heightInPixelsInvariantCulture);
        _uiStringBuilder.Append("px;");

        var widthInPixelsInvariantCulture = renderBatchLocal.GutterWidthInPixels.ToCssValue();
        _uiStringBuilder.Append("width: ");
        _uiStringBuilder.Append(widthInPixelsInvariantCulture);
        _uiStringBuilder.Append("px;");

        var paddingLeftInPixelsInvariantCulture = TextEditorModel.GUTTER_PADDING_LEFT_IN_PIXELS.ToCssValue();
        _uiStringBuilder.Append("padding-left: ");
        _uiStringBuilder.Append(paddingLeftInPixelsInvariantCulture);
        _uiStringBuilder.Append("px;");

        var paddingRightInPixelsInvariantCulture = TextEditorModel.GUTTER_PADDING_RIGHT_IN_PIXELS.ToCssValue();
        _uiStringBuilder.Append("padding-right: ");
        _uiStringBuilder.Append(paddingRightInPixelsInvariantCulture);
        _uiStringBuilder.Append("px;");

        return _uiStringBuilder.ToString();;
    }

    public string GetGutterSectionStyleCss(TextEditorRenderBatch renderBatchLocal)
    {
    	_uiStringBuilder.Clear();
    
        var widthInPixelsInvariantCulture = renderBatchLocal.GutterWidthInPixels.ToCssValue();
        _uiStringBuilder.Append("width: ");
        _uiStringBuilder.Append(widthInPixelsInvariantCulture);
        _uiStringBuilder.Append("px;");

        return _uiStringBuilder.ToString();
    }
    
    #endregion GutterDriverClose
    
    #region BodyDriverOpen
    public bool GlobalShowNewlines => TextEditorService.OptionsApi.GetTextEditorOptionsState().Options.ShowNewlines;
    
    public string GetBodyStyleCss(TextEditorRenderBatch renderBatchLocal)
    {
    	_uiStringBuilder.Clear();
    
        var gutterWidthInPixelsInvariantCulture = renderBatchLocal.GutterWidthInPixels.ToCssValue();

		// Width
		_uiStringBuilder.Append("width: calc(100% - ");
		_uiStringBuilder.Append(gutterWidthInPixelsInvariantCulture);
		_uiStringBuilder.Append("px);");
		
		// Left
		_uiStringBuilder.Append("left: ");
		_uiStringBuilder.Append(gutterWidthInPixelsInvariantCulture);
		_uiStringBuilder.Append("px;");

        return _uiStringBuilder.ToString();
    }
    
    /* RowSection.razor Open */
    public string RowSection_GetRowStyleCss(TextEditorRenderBatch renderBatchLocal, int index, double? virtualizedRowLeftInPixels)
    {
    	_uiStringBuilder.Clear();
    
        var charMeasurements = renderBatchLocal.ViewModel.CharAndLineMeasurements;

        var topInPixelsInvariantCulture = (index * charMeasurements.LineHeight).ToCssValue();
        _uiStringBuilder.Append("top: ");
        _uiStringBuilder.Append(topInPixelsInvariantCulture);
        _uiStringBuilder.Append("px;");

        var heightInPixelsInvariantCulture = charMeasurements.LineHeight.ToCssValue();
        _uiStringBuilder.Append("height: ");
        _uiStringBuilder.Append(heightInPixelsInvariantCulture);
        _uiStringBuilder.Append("px;");

        var virtualizedRowLeftInPixelsInvariantCulture = virtualizedRowLeftInPixels.GetValueOrDefault().ToCssValue();
        _uiStringBuilder.Append("left: ");
        _uiStringBuilder.Append(virtualizedRowLeftInPixelsInvariantCulture);
        _uiStringBuilder.Append("px;");

        return _uiStringBuilder.ToString();
    }
    
    #endregion BodyDriverClose
    
    #region CursorDriverOpen

    /// <summary>This property will need to be used when multi-cursor is added.</summary>
    public bool IsFocusTarget => true;

    public readonly Guid _intersectionObserverMapKey = Guid.NewGuid();
    public readonly Throttle _throttleShouldRevealCursor = new(TimeSpan.FromMilliseconds(333));

    public ElementReference? _cursorDisplayElementReference;
    public int _menuShouldGetFocusRequestCount;
    public string _previousGetCursorStyleCss = string.Empty;
    public string _previousGetCaretRowStyleCss = string.Empty;
    public string _previousGetMenuStyleCss = string.Empty;

    public string _previouslyObservedCursorDisplayId = string.Empty;
    public double _leftRelativeToParentInPixels;
        
    public bool GetIncludeContextMenuHelperComponent(TextEditorRenderBatch renderBatchLocal)
    {
    	return renderBatchLocal.ViewModelDisplayOptions.IncludeContextMenuHelperComponent;
    }

	public string GetScrollableContainerId(TextEditorRenderBatch renderBatchLocal)
	{ 
		return renderBatchLocal.ViewModel.BodyElementId;
	}

    public string GetCursorDisplayId(TextEditorRenderBatch renderBatchLocal)
    {
    	return renderBatchLocal.ViewModel.PrimaryCursor.IsPrimaryCursor
	        ? renderBatchLocal?.ViewModel?.PrimaryCursorContentId ?? string.Empty
	        : string.Empty;
    }

    public string BlinkAnimationCssClass => TextEditorService.ViewModelApi.CursorShouldBlink
        ? "luth_te_blink"
        : string.Empty;

    public string GetCursorStyleCss(TextEditorRenderBatch renderBatchLocal)
    {
        try
        {
            var measurements = renderBatchLocal.ViewModel.CharAndLineMeasurements;

            var leftInPixels = 0d;

            // Tab key column offset
            {
                var tabsOnSameRowBeforeCursor = renderBatchLocal.Model.GetTabCountOnSameLineBeforeCursor(
                    renderBatchLocal.ViewModel.PrimaryCursor.LineIndex,
                    renderBatchLocal.ViewModel.PrimaryCursor.ColumnIndex);

                // 1 of the character width is already accounted for

                var extraWidthPerTabKey = TextEditorModel.TAB_WIDTH - 1;

                leftInPixels += extraWidthPerTabKey *
                    tabsOnSameRowBeforeCursor *
                    measurements.CharacterWidth;
            }

            leftInPixels += measurements.CharacterWidth * renderBatchLocal.ViewModel.PrimaryCursor.ColumnIndex;
            
            _uiStringBuilder.Clear();

            var leftInPixelsInvariantCulture = leftInPixels.ToCssValue();
            _uiStringBuilder.Append("left: ");
            _uiStringBuilder.Append(leftInPixelsInvariantCulture);
            _uiStringBuilder.Append("px;");

            var topInPixelsInvariantCulture = (measurements.LineHeight * renderBatchLocal.ViewModel.PrimaryCursor.LineIndex)
                .ToCssValue();

			_uiStringBuilder.Append("top: ");
			_uiStringBuilder.Append(topInPixelsInvariantCulture);
			_uiStringBuilder.Append("px;");

            var heightInPixelsInvariantCulture = measurements.LineHeight.ToCssValue();
            _uiStringBuilder.Append("height: ");
            _uiStringBuilder.Append(heightInPixelsInvariantCulture);
            _uiStringBuilder.Append("px;");

            var widthInPixelsInvariantCulture = renderBatchLocal.Options.CursorWidthInPixels.ToCssValue();
            _uiStringBuilder.Append("width: ");
            _uiStringBuilder.Append(widthInPixelsInvariantCulture);
            _uiStringBuilder.Append("px;");

            _uiStringBuilder.Append(((ITextEditorKeymap)renderBatchLocal.Options.Keymap).GetCursorCssStyleString(
                renderBatchLocal.Model,
                renderBatchLocal.ViewModel,
                renderBatchLocal.Options));
            
            // This feels a bit hacky, exceptions are happening because the UI isn't accessing
            // the text editor in a thread safe way.
            //
            // When an exception does occur though, the cursor should receive a 'text editor changed'
            // event and re-render anyhow however.
            // 
            // So store the result of this method incase an exception occurs in future invocations,
            // to keep the cursor on screen while the state works itself out.
            return _previousGetCursorStyleCss = _uiStringBuilder.ToString();
        }
        catch (LuthetusTextEditorException)
        {
            return _previousGetCursorStyleCss;
        }
    }

    public string GetCaretRowStyleCss(TextEditorRenderBatch renderBatchLocal)
    {
        try
        {
            var measurements = renderBatchLocal.ViewModel.CharAndLineMeasurements;

            var topInPixelsInvariantCulture = (measurements.LineHeight * renderBatchLocal.ViewModel.PrimaryCursor.LineIndex)
                .ToCssValue();
			
			_uiStringBuilder.Clear();
			
			_uiStringBuilder.Append("top: ");
			_uiStringBuilder.Append(topInPixelsInvariantCulture);
			_uiStringBuilder.Append("px;");

            var heightInPixelsInvariantCulture = measurements.LineHeight.ToCssValue();
            _uiStringBuilder.Append("height: ");
            _uiStringBuilder.Append(heightInPixelsInvariantCulture);
            _uiStringBuilder.Append("px;");

            var widthOfBodyInPixelsInvariantCulture =
                (renderBatchLocal.Model.MostCharactersOnASingleLineTuple.lineLength * measurements.CharacterWidth)
                .ToCssValue();

			_uiStringBuilder.Append("width: ");
			_uiStringBuilder.Append(widthOfBodyInPixelsInvariantCulture);
			_uiStringBuilder.Append("px;");

            // This feels a bit hacky, exceptions are happening because the UI isn't accessing
            // the text editor in a thread safe way.
            //
            // When an exception does occur though, the cursor should receive a 'text editor changed'
            // event and re-render anyhow however.
            // 
            // So store the result of this method incase an exception occurs in future invocations,
            // to keep the cursor on screen while the state works itself out.
            return _previousGetCaretRowStyleCss = _uiStringBuilder.ToString();
        }
        catch (LuthetusTextEditorException)
        {
            return _previousGetCaretRowStyleCss;
        }
    }

    public string GetMenuStyleCss(TextEditorRenderBatch renderBatchLocal)
    {
        try
        {
            var measurements = renderBatchLocal.ViewModel.CharAndLineMeasurements;

            var leftInPixels = 0d;

            // Tab key column offset
            {
                var tabsOnSameRowBeforeCursor = renderBatchLocal.Model.GetTabCountOnSameLineBeforeCursor(
                    renderBatchLocal.ViewModel.PrimaryCursor.LineIndex,
                    renderBatchLocal.ViewModel.PrimaryCursor.ColumnIndex);

                // 1 of the character width is already accounted for
                var extraWidthPerTabKey = TextEditorModel.TAB_WIDTH - 1;

                leftInPixels += extraWidthPerTabKey * tabsOnSameRowBeforeCursor *
                    measurements.CharacterWidth;
            }

            leftInPixels += measurements.CharacterWidth * renderBatchLocal.ViewModel.PrimaryCursor.ColumnIndex;
            
            _uiStringBuilder.Clear();

            var leftInPixelsInvariantCulture = leftInPixels.ToCssValue();
            _uiStringBuilder.Append("left: ");
            _uiStringBuilder.Append(leftInPixelsInvariantCulture);
            _uiStringBuilder.Append("px;");

            var topInPixelsInvariantCulture = (measurements.LineHeight * (renderBatchLocal.ViewModel.PrimaryCursor.LineIndex + 1))
                .ToCssValue();

            // Top is 1 row further than the cursor so it does not cover text at cursor position.
            _uiStringBuilder.Append("top: ");
            _uiStringBuilder.Append(topInPixelsInvariantCulture);
            _uiStringBuilder.Append("px;");

            var minWidthInPixelsInvariantCulture = (measurements.CharacterWidth * 16).ToCssValue();
            _uiStringBuilder.Append("min-Width: ");
            _uiStringBuilder.Append(minWidthInPixelsInvariantCulture);
            _uiStringBuilder.Append("px;");

            var minHeightInPixelsInvariantCulture = (measurements.LineHeight * 4).ToCssValue();
            _uiStringBuilder.Append("min-height: ");
            _uiStringBuilder.Append(minHeightInPixelsInvariantCulture);
            _uiStringBuilder.Append("px;");

            // This feels a bit hacky, exceptions are happening because the UI isn't accessing
            // the text editor in a thread safe way.
            //
            // When an exception does occur though, the cursor should receive a 'text editor changed'
            // event and re-render anyhow however.
            // 
            // So store the result of this method incase an exception occurs in future invocations,
            // to keep the cursor on screen while the state works itself out.
            return _previousGetMenuStyleCss = _uiStringBuilder.ToString();
        }
        catch (LuthetusTextEditorException)
        {
            return _previousGetMenuStyleCss;
        }
    }

    public async Task FocusAsync()
    {
        try
        {
            if (_cursorDisplayElementReference is not null)
            {
                await _cursorDisplayElementReference.Value
                    .FocusAsync(preventScroll: true)
                    .ConfigureAwait(false);
            }
        }
        catch (Exception)
        {
            // 2023-04-18: The app has had a bug where it "freezes" and must be restarted.
            //             This bug is seemingly happening randomly. I have a suspicion
            //             that there are race-condition exceptions occurring with "FocusAsync"
            //             on an ElementReference.
        }
    }

    public async Task SetFocusToActiveMenuAsync()
    {
        _menuShouldGetFocusRequestCount++;
        // await InvokeAsync(StateHasChanged);
    }

    public bool TextEditorMenuShouldTakeFocus()
    {
        if (_menuShouldGetFocusRequestCount > 0)
        {
            _menuShouldGetFocusRequestCount = 0;
            return true;
        }

        return false;
    }

    public int GetTabIndex(TextEditorRenderBatch renderBatchLocal)
    {
        if (!IsFocusTarget)
            return -1;

        return renderBatchLocal.ViewModelDisplayOptions.TabIndex;;
    }
    
    #endregion CursorDriverClose
    
    #region ScrollbarSectionOpen

    [Parameter, EditorRequired]
    public TextEditorRenderBatch? RenderBatch { get; set; }

	/// <summary>
	/// Unit of measurement is pixels (px).
	/// </summary>
	private const int DISTANCE_TO_RESET_SCROLL_POSITION = 300;

	private MouseEventArgs? _mouseDownEventArgs;

    private readonly Guid VERTICAL_scrollbarGuid = Guid.NewGuid();
	private readonly Guid HORIZONTAL_scrollbarGuid = Guid.NewGuid();

    private bool VERTICAL_thinksLeftMouseButtonIsDown;

	private double VERTICAL_clientXThresholdToResetScrollTopPosition;
	private double VERTICAL_scrollTopOnMouseDown;

    private string VERTICAL_ScrollbarElementId;
    private string VERTICAL_ScrollbarSliderElementId;

    private bool HORIZONTAL_thinksLeftMouseButtonIsDown;
	private double HORIZONTAL_clientYThresholdToResetScrollLeftPosition;
	private double HORIZONTAL_scrollLeftOnMouseDown;

    private string HORIZONTAL_ScrollbarElementId;
    private string HORIZONTAL_ScrollbarSliderElementId;
	
	private Func<MouseEventArgs, MouseEventArgs, Task>? _dragEventHandler = null;

    private string HORIZONTAL_GetScrollbarHorizontalStyleCss(TextEditorRenderBatch renderBatchLocal)
    {    
        var scrollbarWidthInPixels = renderBatchLocal.ViewModel.TextEditorDimensions.Width -
            ScrollbarFacts.SCROLLBAR_SIZE_IN_PIXELS;

        var scrollbarWidthInPixelsInvariantCulture = scrollbarWidthInPixels.ToCssValue();
        
        _uiStringBuilder.Clear();
        
        _uiStringBuilder.Append("width: ");
        _uiStringBuilder.Append(scrollbarWidthInPixelsInvariantCulture);
        _uiStringBuilder.Append("px;");

        return _uiStringBuilder.ToString();
    }

    private string HORIZONTAL_GetSliderHorizontalStyleCss(TextEditorRenderBatch renderBatchLocal)
    {
    	var scrollbarWidthInPixels = renderBatchLocal.ViewModel.TextEditorDimensions.Width -
            ScrollbarFacts.SCROLLBAR_SIZE_IN_PIXELS;

        // Proportional Left
        var sliderProportionalLeftInPixels = renderBatchLocal.ViewModel.ScrollbarDimensions.ScrollLeft *
            scrollbarWidthInPixels /
            renderBatchLocal.ViewModel.ScrollbarDimensions.ScrollWidth;

		_uiStringBuilder.Clear();
        
        var sliderProportionalLeftInPixelsInvariantCulture = sliderProportionalLeftInPixels.ToCssValue();
        _uiStringBuilder.Append("left: ");
        _uiStringBuilder.Append(sliderProportionalLeftInPixelsInvariantCulture);
        _uiStringBuilder.Append("px;");

        // Proportional Width
        var pageWidth = renderBatchLocal.ViewModel.TextEditorDimensions.Width;

        var sliderProportionalWidthInPixels = pageWidth *
            scrollbarWidthInPixels /
            renderBatchLocal.ViewModel.ScrollbarDimensions.ScrollWidth;

        var sliderProportionalWidthInPixelsInvariantCulture = sliderProportionalWidthInPixels.ToCssValue();
        _uiStringBuilder.Append("width: ");
        _uiStringBuilder.Append(sliderProportionalWidthInPixelsInvariantCulture);
        _uiStringBuilder.Append("px;");

        return _uiStringBuilder.ToString();
    }
    
    private string VERTICAL_GetSliderVerticalStyleCss(TextEditorRenderBatch renderBatchLocal)
    {
    	var textEditorDimensions = renderBatchLocal.ViewModel.TextEditorDimensions;
        var scrollBarDimensions = renderBatchLocal.ViewModel.ScrollbarDimensions;

        var scrollbarHeightInPixels = textEditorDimensions.Height - ScrollbarFacts.SCROLLBAR_SIZE_IN_PIXELS;

        // Proportional Top
        var sliderProportionalTopInPixels = scrollBarDimensions.ScrollTop *
            scrollbarHeightInPixels /
            scrollBarDimensions.ScrollHeight;

        var sliderProportionalTopInPixelsInvariantCulture = sliderProportionalTopInPixels.ToCssValue();

		_uiStringBuilder.Clear();
		
		_uiStringBuilder.Append("top: ");
		_uiStringBuilder.Append(sliderProportionalTopInPixelsInvariantCulture);
		_uiStringBuilder.Append("px;");

        // Proportional Height
        var pageHeight = textEditorDimensions.Height;

        var sliderProportionalHeightInPixels = pageHeight *
            scrollbarHeightInPixels /
            scrollBarDimensions.ScrollHeight;

        var sliderProportionalHeightInPixelsInvariantCulture = sliderProportionalHeightInPixels.ToCssValue();

		_uiStringBuilder.Append("height: ");
		_uiStringBuilder.Append(sliderProportionalHeightInPixelsInvariantCulture);
		_uiStringBuilder.Append("px;");

        return _uiStringBuilder.ToString();
    }

    private async Task HORIZONTAL_HandleOnMouseDownAsync(MouseEventArgs mouseEventArgs)
    {
    	var renderBatchLocal = _activeRenderBatch;
    	if (renderBatchLocal is null)
    		return;
    
    	HORIZONTAL_thinksLeftMouseButtonIsDown = true;
		HORIZONTAL_scrollLeftOnMouseDown = renderBatchLocal.ViewModel.ScrollbarDimensions.ScrollLeft;

		var scrollbarBoundingClientRect = await TextEditorService.JsRuntimeCommonApi
			.MeasureElementById(HORIZONTAL_ScrollbarElementId)
			.ConfigureAwait(false);

		// Drag far up to reset scroll to original
		var textEditorDimensions = renderBatchLocal.ViewModel.TextEditorDimensions;
		var distanceBetweenTopEditorAndTopScrollbar = scrollbarBoundingClientRect.TopInPixels - textEditorDimensions.BoundingClientRectTop;
		HORIZONTAL_clientYThresholdToResetScrollLeftPosition = scrollbarBoundingClientRect.TopInPixels - DISTANCE_TO_RESET_SCROLL_POSITION;

		// Subscribe to the drag events
		//
		// NOTE: '_mouseDownEventArgs' being non-null is what indicates that the subscription is active.
		//       So be wary if one intends to move its assignment elsewhere.
		{
			_mouseDownEventArgs = mouseEventArgs;
			_dragEventHandler = HORIZONTAL_DragEventHandlerScrollAsync;
	
			DragService.ReduceShouldDisplayAndMouseEventArgsSetAction(true, null);
		}
    }
    
    private async Task VERTICAL_HandleOnMouseDownAsync(MouseEventArgs mouseEventArgs)
    {
    	var renderBatchLocal = _activeRenderBatch;
    	if (renderBatchLocal is null)
    		return;
    
    	VERTICAL_thinksLeftMouseButtonIsDown = true;
		VERTICAL_scrollTopOnMouseDown = renderBatchLocal.ViewModel.ScrollbarDimensions.ScrollTop;

		var scrollbarBoundingClientRect = await TextEditorService.JsRuntimeCommonApi
			.MeasureElementById(VERTICAL_ScrollbarElementId)
			.ConfigureAwait(false);

		// Drag far left to reset scroll to original
		var textEditorDimensions = renderBatchLocal.ViewModel.TextEditorDimensions;
		var distanceBetweenLeftEditorAndLeftScrollbar = scrollbarBoundingClientRect.LeftInPixels - textEditorDimensions.BoundingClientRectLeft;
		VERTICAL_clientXThresholdToResetScrollTopPosition = scrollbarBoundingClientRect.LeftInPixels - DISTANCE_TO_RESET_SCROLL_POSITION;

		// Subscribe to the drag events
		//
		// NOTE: '_mouseDownEventArgs' being non-null is what indicates that the subscription is active.
		//       So be wary if one intends to move its assignment elsewhere.
		{
			_mouseDownEventArgs = mouseEventArgs;
			_dragEventHandler = VERTICAL_DragEventHandlerScrollAsync;
	
			DragService.ReduceShouldDisplayAndMouseEventArgsSetAction(true, null);
		}     
    }

    private async void DragStateWrapOnStateChanged()
    {
        if (!DragService.GetDragState().ShouldDisplay)
        {
            // NOTE: '_mouseDownEventArgs' being non-null is what indicates that the subscription is active.
			//       So be wary if one intends to move its assignment elsewhere.
            _mouseDownEventArgs = null;
        }
        else
        {
            var localMouseDownEventArgs = _mouseDownEventArgs;
            var dragEventArgs = DragService.GetDragState().MouseEventArgs;
			var localDragEventHandler = _dragEventHandler;

            if (localMouseDownEventArgs is not null && dragEventArgs is not null)
                await localDragEventHandler.Invoke(localMouseDownEventArgs, dragEventArgs).ConfigureAwait(false);
        }
    }

    private Task HORIZONTAL_DragEventHandlerScrollAsync(MouseEventArgs localMouseDownEventArgs, MouseEventArgs onDragMouseEventArgs)
    {
    	var renderBatchLocal = _activeRenderBatch;
    	if (renderBatchLocal is null)
    		return Task.CompletedTask;
    
    	var localThinksLeftMouseButtonIsDown = HORIZONTAL_thinksLeftMouseButtonIsDown;

        if (!localThinksLeftMouseButtonIsDown)
            return Task.CompletedTask;

        // Buttons is a bit flag '& 1' gets if left mouse button is held
        if (localThinksLeftMouseButtonIsDown && (onDragMouseEventArgs.Buttons & 1) == 1)
        {
			var textEditorDimensions = renderBatchLocal.ViewModel.TextEditorDimensions;
			var scrollbarDimensions = renderBatchLocal.ViewModel.ScrollbarDimensions;
		
			OnScrollHorizontal onScrollHorizontal;

			if (onDragMouseEventArgs.ClientY < HORIZONTAL_clientYThresholdToResetScrollLeftPosition)
			{
				// Drag far left to reset scroll to original
				onScrollHorizontal = new OnScrollHorizontal(
					HORIZONTAL_scrollLeftOnMouseDown,
					renderBatchLocal.ComponentData,
					renderBatchLocal.ViewModel.ViewModelKey);
			}
			else
			{
				var diffX = onDragMouseEventArgs.ClientX - localMouseDownEventArgs.ClientX;
	
	            var scrollbarWidthInPixels = textEditorDimensions.Width - ScrollbarFacts.SCROLLBAR_SIZE_IN_PIXELS;
	
	            var scrollLeft = HORIZONTAL_scrollLeftOnMouseDown +
					diffX *
	                scrollbarDimensions.ScrollWidth /
	                scrollbarWidthInPixels;
	
	            if (scrollLeft + textEditorDimensions.Width > scrollbarDimensions.ScrollWidth)
	                scrollLeft = scrollbarDimensions.ScrollWidth - textEditorDimensions.Width;

				if (scrollLeft < 0)
					scrollLeft = 0;
	
				onScrollHorizontal = new OnScrollHorizontal(
					scrollLeft,
					renderBatchLocal.ComponentData,
					renderBatchLocal.ViewModel.ViewModelKey);
			}

			TextEditorService.TextEditorWorker.EnqueueOnScrollHorizontal(onScrollHorizontal);
        }
        else
        {
            HORIZONTAL_thinksLeftMouseButtonIsDown = false;
        }

        return Task.CompletedTask;
    }
    
    private Task VERTICAL_DragEventHandlerScrollAsync(MouseEventArgs localMouseDownEventArgs, MouseEventArgs onDragMouseEventArgs)
    {
    	var renderBatchLocal = _activeRenderBatch;
    	if (renderBatchLocal is null)
    		return Task.CompletedTask;
    
    	var localThinksLeftMouseButtonIsDown = VERTICAL_thinksLeftMouseButtonIsDown;

        if (!localThinksLeftMouseButtonIsDown)
            return Task.CompletedTask;

        // Buttons is a bit flag '& 1' gets if left mouse button is held
        if (localThinksLeftMouseButtonIsDown && (onDragMouseEventArgs.Buttons & 1) == 1)
        {
			var textEditorDimensions = renderBatchLocal.ViewModel.TextEditorDimensions;
			var scrollbarDimensions = renderBatchLocal.ViewModel.ScrollbarDimensions;

			OnScrollVertical onScrollVertical;

			if (onDragMouseEventArgs.ClientX < VERTICAL_clientXThresholdToResetScrollTopPosition)
			{
				// Drag far left to reset scroll to original
				onScrollVertical = new OnScrollVertical(
					VERTICAL_scrollTopOnMouseDown,
					renderBatchLocal.ComponentData,
					renderBatchLocal.ViewModel.ViewModelKey);
			}
			else
			{
	    		var diffY = onDragMouseEventArgs.ClientY - localMouseDownEventArgs.ClientY;
	
	            var scrollbarHeightInPixels = textEditorDimensions.Height - ScrollbarFacts.SCROLLBAR_SIZE_IN_PIXELS;
	
	            var scrollTop = VERTICAL_scrollTopOnMouseDown +
					diffY *
	                scrollbarDimensions.ScrollHeight /
	                scrollbarHeightInPixels;
	
	            if (scrollTop + textEditorDimensions.Height > scrollbarDimensions.ScrollHeight)
	                scrollTop = scrollbarDimensions.ScrollHeight - textEditorDimensions.Height;

				if (scrollTop < 0)
					scrollTop = 0;
	
				onScrollVertical = new OnScrollVertical(
					scrollTop,
					renderBatchLocal.ComponentData,
					renderBatchLocal.ViewModel.ViewModelKey);
			}

			TextEditorService.TextEditorWorker.EnqueueOnScrollVertical(onScrollVertical);
        }
        else
        {
            VERTICAL_thinksLeftMouseButtonIsDown = false;
        }

        return Task.CompletedTask;
    }
    
    #endregion ScrollbarSectionClose

    #region VirtualizationDriverOpen

    public string Virtualization_GetStyleCssString(VirtualizationBoundary virtualizationBoundary)
    {
    	_uiStringBuilder.Clear();
    
        // Width
        if (virtualizationBoundary.WidthInPixels == -1)
        {
            _uiStringBuilder.Append(" width: 100%;");
        }
        else
        {
            var widthInPixelsInvariantCulture = virtualizationBoundary.WidthInPixels.ToCssValue();
            _uiStringBuilder.Append($" width: {widthInPixelsInvariantCulture}px;");
        }

        // Height
        if (virtualizationBoundary.HeightInPixels == -1)
        {
            _uiStringBuilder.Append(" height: 100%;");
        }
        else
        {
            var heightInPixelsInvariantCulture = virtualizationBoundary.HeightInPixels.ToCssValue();
            _uiStringBuilder.Append($" height: {heightInPixelsInvariantCulture}px;");
        }

        // Left
        if (virtualizationBoundary.LeftInPixels == -1)
        {
            _uiStringBuilder.Append(" left: 100%;");
        }
        else
        {
            var leftInPixelsInvariantCulture = virtualizationBoundary.LeftInPixels.ToCssValue();
            _uiStringBuilder.Append($" left: {leftInPixelsInvariantCulture}px;");
        }

        // Top
        if (virtualizationBoundary.TopInPixels == -1)
        {
            _uiStringBuilder.Append(" top: 100%;");
        }
        else
        {
            var topInPixelsInvariantCulture = virtualizationBoundary.TopInPixels.ToCssValue();
            _uiStringBuilder.Append($" top: {topInPixelsInvariantCulture}px;");
        }

        return _uiStringBuilder.ToString();
    }

    #endregion ScrollbarSectionClose
    
    #region PresentationAndSelectionDriverOpen
    
    public List<TextEditorPresentationModel> GetTextEditorPresentationModels(
    	TextEditorRenderBatch renderBatchLocal,
    	IReadOnlyList<Key<TextEditorPresentationModel>> textEditorPresentationKeys)
    {
    	var textEditorPresentationModelList = new List<TextEditorPresentationModel>();

        foreach (var presentationKey in textEditorPresentationKeys)
        {
            var textEditorPresentationModel = renderBatchLocal.Model.PresentationModelList.FirstOrDefault(x =>
                x.TextEditorPresentationKey == presentationKey);

            if (textEditorPresentationModel is not null)
                textEditorPresentationModelList.Add(textEditorPresentationModel);
        }

        return textEditorPresentationModelList;
    }

    public string PresentationGetCssStyleString(
    	TextEditorRenderBatch renderBatchLocal,
        int lowerPositionIndexInclusive,
        int upperPositionIndexExclusive,
        int rowIndex)
    {
    	try
        {
            var charMeasurements = renderBatchLocal.ViewModel.CharAndLineMeasurements;
			var textEditorDimensions = renderBatchLocal.ViewModel.TextEditorDimensions;
            var scrollbarDimensions = renderBatchLocal.ViewModel.ScrollbarDimensions;

            if (rowIndex >= renderBatchLocal.Model.LineEndList.Count)
                return string.Empty;

            var line = renderBatchLocal.Model.GetLineInformation(rowIndex);

            var startingColumnIndex = 0;
            var endingColumnIndex = line.EndPositionIndexExclusive - 1;

            var fullWidthOfRowIsSelected = true;

            if (lowerPositionIndexInclusive > line.StartPositionIndexInclusive)
            {
                startingColumnIndex = lowerPositionIndexInclusive - line.StartPositionIndexInclusive;
                fullWidthOfRowIsSelected = false;
            }

            if (upperPositionIndexExclusive < line.EndPositionIndexExclusive)
            {
                endingColumnIndex = upperPositionIndexExclusive - line.StartPositionIndexInclusive;
                fullWidthOfRowIsSelected = false;
            }

            var topInPixelsInvariantCulture = (rowIndex * charMeasurements.LineHeight).ToCssValue();
            
            _uiStringBuilder.Clear();
            _uiStringBuilder.Append("position: absolute; ");

			_uiStringBuilder.Append("top: ");
			_uiStringBuilder.Append(topInPixelsInvariantCulture);
			_uiStringBuilder.Append("px;");

            var heightInPixelsInvariantCulture = charMeasurements.LineHeight.ToCssValue();

            _uiStringBuilder.Append("height: ");
            _uiStringBuilder.Append(heightInPixelsInvariantCulture);
            _uiStringBuilder.Append("px;");

            var startInPixels = startingColumnIndex * charMeasurements.CharacterWidth;

            // startInPixels offset from Tab keys a width of many characters
            {
                var tabsOnSameRowBeforeCursor = renderBatchLocal.Model.GetTabCountOnSameLineBeforeCursor(
                    rowIndex,
                    startingColumnIndex);

                // 1 of the character width is already accounted for
                var extraWidthPerTabKey = TextEditorModel.TAB_WIDTH - 1;

                startInPixels += extraWidthPerTabKey * tabsOnSameRowBeforeCursor * charMeasurements.CharacterWidth;
            }

            var startInPixelsInvariantCulture = startInPixels.ToCssValue();
            _uiStringBuilder.Append("left: ");
            _uiStringBuilder.Append(startInPixelsInvariantCulture);
            _uiStringBuilder.Append("px;");

            var widthInPixels = endingColumnIndex * charMeasurements.CharacterWidth - startInPixels;

            // Tab keys a width of many characters
            {
                var tabsOnSameRowBeforeCursor = renderBatchLocal.Model.GetTabCountOnSameLineBeforeCursor(
                    rowIndex,
                    line.LastValidColumnIndex);

                // 1 of the character width is already accounted for
                var extraWidthPerTabKey = TextEditorModel.TAB_WIDTH - 1;

                widthInPixels += extraWidthPerTabKey * tabsOnSameRowBeforeCursor * charMeasurements.CharacterWidth;
            }

            _uiStringBuilder.Append("width: ");

            var fullWidthValue = scrollbarDimensions.ScrollWidth;

            if (textEditorDimensions.Width > scrollbarDimensions.ScrollWidth)
                fullWidthValue = textEditorDimensions.Width; // If content does not fill the viewable width of the Text Editor User Interface

            var fullWidthValueInPixelsInvariantCulture = fullWidthValue.ToCssValue();

            var widthInPixelsInvariantCulture = widthInPixels.ToCssValue();

            if (fullWidthOfRowIsSelected)
            {
                _uiStringBuilder.Append(fullWidthValueInPixelsInvariantCulture);
                _uiStringBuilder.Append("px;");
            }
            else if (startingColumnIndex != 0 && upperPositionIndexExclusive > line.EndPositionIndexExclusive - 1)
            {
            	_uiStringBuilder.Append("calc(");
            	_uiStringBuilder.Append(fullWidthValueInPixelsInvariantCulture);
            	_uiStringBuilder.Append("px - ");
            	_uiStringBuilder.Append(startInPixelsInvariantCulture);
            	_uiStringBuilder.Append("px);");
            }
            else
            {
            	_uiStringBuilder.Append(widthInPixelsInvariantCulture);
            	_uiStringBuilder.Append("px;");;
            }

            return _uiStringBuilder.ToString();
        }
        catch (LuthetusTextEditorException)
        {
            return string.Empty;
        }
    }

    public string PresentationGetCssClass(TextEditorPresentationModel presentationModel, byte decorationByte)
    {
        return presentationModel.DecorationMapper.Map(decorationByte);
    }

    public IReadOnlyList<TextEditorTextSpan> PresentationVirtualizeAndShiftTextSpans(
    	TextEditorRenderBatch renderBatchLocal,
        IReadOnlyList<TextEditorTextModification> textModifications,
        IReadOnlyList<TextEditorTextSpan> inTextSpanList)
    {
    	// (2025-01-22)
    	// ============
    	// The text spans need to be tied to the partitions they reside in
    	// (careful of a textspan that overlaps two partitions).
    	//
    	// This shouldn't be done here, it should be done during the editContext.
    	
    	try
        {
            // Virtualize the text spans
            var virtualizedTextSpanList = new List<TextEditorTextSpan>();
            if (renderBatchLocal.ViewModel.VirtualizationResult.EntryList.Any())
            {
                var lowerLineIndexInclusive = renderBatchLocal.ViewModel.VirtualizationResult.EntryList.First().LineIndex;
                var upperLineIndexInclusive = renderBatchLocal.ViewModel.VirtualizationResult.EntryList.Last().LineIndex;

                var lowerLine = renderBatchLocal.Model.GetLineInformation(lowerLineIndexInclusive);
                var upperLine = renderBatchLocal.Model.GetLineInformation(upperLineIndexInclusive);

				// Awkward enumeration was modified 'for loop' (2025-01-22)
				// Also, this shouldn't be done here, it should be done during the editContext.
				var count = inTextSpanList.Count;
                for (int i = 0; i < count; i++)
                {
                	var textSpan = inTextSpanList[i];
                	
                    if (lowerLine.StartPositionIndexInclusive <= textSpan.StartingIndexInclusive &&
                        upperLine.EndPositionIndexExclusive >= textSpan.StartingIndexInclusive)
                    {
                        virtualizedTextSpanList.Add(textSpan);
                    }
                }
            }
            else
            {
                // No 'VirtualizationResult', so don't render any text spans.
                return Array.Empty<TextEditorTextSpan>();
            }

            var outTextSpansList = new List<TextEditorTextSpan>();
            // Shift the text spans
            {
                foreach (var textSpan in virtualizedTextSpanList)
                {
                    var startingIndexInclusive = textSpan.StartingIndexInclusive;
                    var endingIndexExclusive = textSpan.EndingIndexExclusive;

					// Awkward enumeration was modified 'for loop' (2025-01-22)
					// Also, this shouldn't be done here, it should be done during the editContext.
					var count = textModifications.Count;
                    for (int i = 0; i < count; i++)
                    {
                    	var textModification = textModifications[i];
                    
                        if (textModification.WasInsertion)
                        {
                            if (startingIndexInclusive >= textModification.TextEditorTextSpan.StartingIndexInclusive)
                            {
                                startingIndexInclusive += textModification.TextEditorTextSpan.Length;
                                endingIndexExclusive += textModification.TextEditorTextSpan.Length;
                            }
                        }
                        else // was deletion
                        {
                            if (startingIndexInclusive >= textModification.TextEditorTextSpan.StartingIndexInclusive)
                            {
                                startingIndexInclusive -= textModification.TextEditorTextSpan.Length;
                                endingIndexExclusive -= textModification.TextEditorTextSpan.Length;
                            }
                        }
                    }

                    outTextSpansList.Add(textSpan with
                    {
                        StartingIndexInclusive = startingIndexInclusive,
                        EndingIndexExclusive = endingIndexExclusive
                    });
                }
            }

            return outTextSpansList;
        }
        catch (LuthetusTextEditorException)
        {
            return Array.Empty<TextEditorTextSpan>();
        }
    }

    public (int FirstRowToSelectDataInclusive, int LastRowToSelectDataExclusive) PresentationGetBoundsInRowIndexUnits(
    	TextEditorRenderBatch renderBatchLocal,
    	TextEditorModel model,
    	(int StartingIndexInclusive, int EndingIndexExclusive) boundsInPositionIndexUnits)
    {
    	try
        {
            var firstRowToSelectDataInclusive = renderBatchLocal.Model
                .GetLineInformationFromPositionIndex(boundsInPositionIndexUnits.StartingIndexInclusive)
                .Index;

            var lastRowToSelectDataExclusive = renderBatchLocal.Model
                .GetLineInformationFromPositionIndex(boundsInPositionIndexUnits.EndingIndexExclusive)
                .Index +
                1;

            return (firstRowToSelectDataInclusive, lastRowToSelectDataExclusive);
        }
        catch (LuthetusTextEditorException)
        {
            return (0, 0);
        }
    }
    
    public string GetTextSelectionStyleCss(
    	TextEditorRenderBatch renderBatchLocal,
        int lowerPositionIndexInclusive,
        int upperPositionIndexExclusive,
        int rowIndex)
    {
    	try
		{
	        if (rowIndex >= renderBatchLocal.Model.LineEndList.Count)
	            return string.Empty;
	
	        var line = renderBatchLocal.Model.GetLineInformation(rowIndex);
	
	        var selectionStartingColumnIndex = 0;
	        var selectionEndingColumnIndex = line.EndPositionIndexExclusive - 1;
	
	        var fullWidthOfRowIsSelected = true;
	
	        if (lowerPositionIndexInclusive > line.StartPositionIndexInclusive)
	        {
	            selectionStartingColumnIndex = lowerPositionIndexInclusive - line.StartPositionIndexInclusive;
	            fullWidthOfRowIsSelected = false;
	        }
	
	        if (upperPositionIndexExclusive < line.EndPositionIndexExclusive)
	        {
	            selectionEndingColumnIndex = upperPositionIndexExclusive - line.StartPositionIndexInclusive;
	            fullWidthOfRowIsSelected = false;
	        }
	
	        var charMeasurements = renderBatchLocal.ViewModel.CharAndLineMeasurements;
	
	        _uiStringBuilder.Clear();
	        
	        var topInPixelsInvariantCulture = (rowIndex * charMeasurements.LineHeight).ToCssValue();
	        _uiStringBuilder.Append("top: ");
	        _uiStringBuilder.Append(topInPixelsInvariantCulture);
	        _uiStringBuilder.Append("px;");
	
	        var heightInPixelsInvariantCulture = charMeasurements.LineHeight.ToCssValue();
	        _uiStringBuilder.Append("height: ");
	        _uiStringBuilder.Append(heightInPixelsInvariantCulture);
	        _uiStringBuilder.Append("px;");
	
	        var selectionStartInPixels = selectionStartingColumnIndex * charMeasurements.CharacterWidth;
	
	        // selectionStartInPixels offset from Tab keys a width of many characters
	        {
	            var tabsOnSameRowBeforeCursor = renderBatchLocal.Model.GetTabCountOnSameLineBeforeCursor(
	                rowIndex,
	                selectionStartingColumnIndex);
	
	            // 1 of the character width is already accounted for
	            var extraWidthPerTabKey = TextEditorModel.TAB_WIDTH - 1;
	
	            selectionStartInPixels += 
	                extraWidthPerTabKey * tabsOnSameRowBeforeCursor * charMeasurements.CharacterWidth;
	        }
	
	        var selectionStartInPixelsInvariantCulture = selectionStartInPixels.ToCssValue();
	        _uiStringBuilder.Append("left: ");
	        _uiStringBuilder.Append(selectionStartInPixelsInvariantCulture);
	        _uiStringBuilder.Append("px;");
	
	        var selectionWidthInPixels = 
	            selectionEndingColumnIndex * charMeasurements.CharacterWidth - selectionStartInPixels;
	
	        // Tab keys a width of many characters
	        {
	            var lineInformation = renderBatchLocal.Model.GetLineInformation(rowIndex);
	
	            selectionEndingColumnIndex = Math.Min(
	                selectionEndingColumnIndex,
	                lineInformation.LastValidColumnIndex);
	
	            var tabsOnSameRowBeforeCursor = renderBatchLocal.Model.GetTabCountOnSameLineBeforeCursor(
	                rowIndex,
	                selectionEndingColumnIndex);
	
	            // 1 of the character width is already accounted for
	            var extraWidthPerTabKey = TextEditorModel.TAB_WIDTH - 1;
	
	            selectionWidthInPixels += extraWidthPerTabKey * tabsOnSameRowBeforeCursor * charMeasurements.CharacterWidth;
	        }
	
	        _uiStringBuilder.Append("width: ");
	        var fullWidthValue = renderBatchLocal.ViewModel.ScrollbarDimensions.ScrollWidth;
	
	        if (renderBatchLocal.ViewModel.TextEditorDimensions.Width >
	            renderBatchLocal.ViewModel.ScrollbarDimensions.ScrollWidth)
	        {
	            // If content does not fill the viewable width of the Text Editor User Interface
	            fullWidthValue = renderBatchLocal.ViewModel.TextEditorDimensions.Width;
	        }
	
	        var fullWidthValueInPixelsInvariantCulture = fullWidthValue.ToCssValue();
	
	        var selectionWidthInPixelsInvariantCulture = selectionWidthInPixels.ToCssValue();
	
	        if (fullWidthOfRowIsSelected)
	        {
	        	_uiStringBuilder.Append(fullWidthValueInPixelsInvariantCulture);
	        	_uiStringBuilder.Append("px;");
	        }
	        else if (selectionStartingColumnIndex != 0 &&
	                 upperPositionIndexExclusive > line.EndPositionIndexExclusive - 1)
	        {
	        	_uiStringBuilder.Append("calc(");
	        	_uiStringBuilder.Append(fullWidthValueInPixelsInvariantCulture);
	        	_uiStringBuilder.Append("px - ");
	        	_uiStringBuilder.Append(selectionStartInPixelsInvariantCulture);
	        	_uiStringBuilder.Append("px);");
	        }
	        else
	        {
	        	_uiStringBuilder.Append(selectionWidthInPixelsInvariantCulture);
	        	_uiStringBuilder.Append("px;");
	        }
	
	        return _uiStringBuilder.ToString();
		}
		catch (LuthetusTextEditorException e)
		{
			Console.WriteLine(e);
			return "display: none;";
		}
    }

    public (int lowerRowIndexInclusive, int upperRowIndexExclusive) GetSelectionBoundsInRowIndexUnits(
    	TextEditorRenderBatch renderBatchLocal,
    	(int lowerPositionIndexInclusive, int upperPositionIndexExclusive) selectionBoundsInPositionIndexUnits)
    {
    	try
        {
            return TextEditorSelectionHelper.ConvertSelectionOfPositionIndexUnitsToRowIndexUnits(
                renderBatchLocal.Model,
                selectionBoundsInPositionIndexUnits);
        }
        catch (LuthetusTextEditorException)
        {
            return (0, 0);
        }
    }
    
    #endregion PresentationAndSelectionDriverClose

    public void Dispose()
    {
    	// ScrollbarSection.razor.cs
    	DragService.DragStateChanged -= DragStateWrapOnStateChanged;
    
    	// TextEditorViewModelDisplay.razor.cs
        TextEditorService.TextEditorStateChanged -= GeneralOnStateChangedEventHandler;
        TextEditorService.OptionsApi.TextEditorOptionsStateChanged -= TextEditorOptionsStateWrap_StateChanged;
		TextEditorService.ViewModelApi.CursorShouldBlinkChanged -= ViewModel_CursorShouldBlinkChanged;

        lock (_linkedViewModelLock)
        {
            if (_linkedViewModel is not null)
            {
                _linkedViewModel.DisplayTracker.DecrementLinks();
                _linkedViewModel = null;
            }
        }

        _componentData.MouseStoppedMovingCancellationTokenSource.Cancel();
    }
}
