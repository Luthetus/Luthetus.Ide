using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Web;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.Common.RazorLib.Clipboards.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Dimensions.States;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Options.States;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.TextEditor.RazorLib.Autocompletes.Models;
using Luthetus.TextEditor.RazorLib.ComponentRenderers.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.States;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Options.States;
using Luthetus.TextEditor.RazorLib.Commands.Models.Defaults;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.Events.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

// GutterDriver.cs
using Luthetus.TextEditor.RazorLib.Virtualizations.Models;
using Luthetus.TextEditor.RazorLib.Characters.Models;

// BodyDriver.cs
using System.Text;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays;

public sealed partial class TextEditorViewModelDisplay : ComponentBase, IDisposable
{
    [Inject]
    public IState<TextEditorState> TextEditorStateWrap { get; set; } = null!;
    [Inject]
    public IState<TextEditorOptionsState> TextEditorOptionsStateWrap { get; set; } = null!;
    [Inject]
    public IState<AppDimensionState> AppDimensionStateWrap { get; set; } = null!;
    [Inject]
    private IState<AppOptionsState> AppOptionsStateWrap { get; set; } = null!;
    [Inject]
    public IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    public IServiceProvider ServiceProvider { get; set; } = null!;
    [Inject]
    public ITextEditorService TextEditorService { get; set; } = null!;
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
    public IEnvironmentProvider EnvironmentProvider { get; set; } = null!;
    [Inject]
    public IDialogService DialogService { get; set; } = null!;
    [Inject]
    public LuthetusTextEditorConfig TextEditorConfig { get; set; } = null!;

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
    
    public VirtualizationDriver _gutterVirtualizationDriver;
    public VirtualizationDriver _bodyVirtualizationDriver;
    // TODO: awkward public
    public PresentationAndSelectionDriver _presentationAndSelectionDriver;
    public CursorDriver _cursorDriver;
    
    private Dictionary<string, object?> DependentComponentParameters;
    
    private bool _thinksTouchIsOccurring;
    private DateTime? _touchStartDateTime = null;
    private TouchEventArgs? _previousTouchEventArgs = null;
    private bool _userMouseIsInside;
    
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
    
    	_gutterVirtualizationDriver = new(
    		this,
    		useHorizontalVirtualization: false,
    		useVerticalVirtualization: true);
    	_bodyVirtualizationDriver = new(
    		this,
    		useHorizontalVirtualization: true,
    		useVerticalVirtualization: true);
    	_presentationAndSelectionDriver = new(this);
    	_cursorDriver = new(this);
    
        ConstructRenderBatch();

		SetComponentData();

        TextEditorStateWrap.StateChanged += GeneralOnStateChangedEventHandler;
        TextEditorOptionsStateWrap.StateChanged += TextEditorOptionsStateWrap_StateChanged;
        TextEditorService.ViewModelApi.CursorShouldBlinkChanged += ViewModel_CursorShouldBlinkChanged;

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
    	var localTextEditorState = TextEditorStateWrap.Value;
    
    	var model_viewmodel_tuple = localTextEditorState.GetModelAndViewModelOrDefault(
			TextEditorViewModelKey);
				
        var renderBatchUnsafe = new TextEditorRenderBatch(
            model_viewmodel_tuple.Model,
            model_viewmodel_tuple.ViewModel,
            TextEditorOptionsStateWrap.Value.Options,
            TextEditorRenderBatch.DEFAULT_FONT_FAMILY,
            TextEditorOptionsState.DEFAULT_FONT_SIZE_IN_PIXELS,
            ViewModelDisplayOptions,
			_componentData);

        if (!string.IsNullOrWhiteSpace(renderBatchUnsafe.Options?.CommonOptions?.FontFamily))
        	renderBatchUnsafe.FontFamily = renderBatchUnsafe.Options!.CommonOptions!.FontFamily;

        if (renderBatchUnsafe.Options!.CommonOptions?.FontSizeInPixels is not null)
            renderBatchUnsafe.FontSizeInPixels = renderBatchUnsafe.Options!.CommonOptions.FontSizeInPixels;
        
        if (renderBatchUnsafe.ViewModelDisplayOptions.KeymapOverride is not null)
            renderBatchUnsafe.Options.Keymap = renderBatchUnsafe.ViewModelDisplayOptions.KeymapOverride;

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
			Dispatcher,
			ServiceProvider);
    }

    private async void GeneralOnStateChangedEventHandler(object? sender, EventArgs e) =>
        await InvokeAsync(StateHasChanged);
        
    private async void TextEditorOptionsStateWrap_StateChanged(object? sender, EventArgs e)
    {
    	if (TextEditorOptionsStateWrap.Value.Options.Keymap.Key != _componentData.Options.Keymap.Key)
    	{
    		ConstructRenderBatch();
    		SetComponentData();
    	}
    	
    	await InvokeAsync(StateHasChanged);
    }
        
    private async void ViewModel_CursorShouldBlinkChanged() =>
        await InvokeAsync(StateHasChanged);

    private void HandleTextEditorViewModelKeyChange()
    {
        lock (_linkedViewModelLock)
        {
            var localTextEditorViewModelKey = TextEditorViewModelKey;

            var nextViewModel = TextEditorStateWrap.Value.ViewModelGetOrDefault(localTextEditorViewModelKey);

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

      var nextViewModel = TextEditorStateWrap.Value.ViewModelGetOrDefault(localTextEditorViewModelKey);
      
      if (nextViewModel is not null)
      	await nextViewModel.FocusAsync();
  }

    private string GetGlobalHeightInPixelsStyling()
    {
        var heightInPixels = TextEditorService.OptionsStateWrap.Value.Options.TextEditorHeightInPixels;

        if (heightInPixels is null)
            return string.Empty;

        var heightInPixelsInvariantCulture = heightInPixels.Value.ToCssValue();

        return $"height: {heightInPixelsInvariantCulture}px;";
    }
    
    private void ReceiveOnKeyDown(KeyboardEventArgs keyboardEventArgs)
    {
        if (EventUtils.IsKeyboardEventArgsNoise(keyboardEventArgs))
            return;
        
        var (model, viewModel) = TextEditorStateWrap.Value.GetModelAndViewModelOrDefault(
        	TextEditorViewModelKey);

        var resourceUri = model?.ResourceUri ?? ResourceUri.Empty;
        var viewModelKey = viewModel?.ViewModelKey ?? Key<TextEditorViewModel>.Empty;

        if (resourceUri == ResourceUri.Empty ||
            viewModelKey == Key<TextEditorViewModel>.Empty)
        {
			return;
		}

		var onKeyDown = new OnKeyDownLateBatching(
			_componentData,
            new KeymapArgs(keyboardEventArgs),
            resourceUri,
            viewModelKey);

        TextEditorService.Post(onKeyDown);
	}

    private void ReceiveOnContextMenu()
    {
		var localViewModelKey = TextEditorViewModelKey;

		TextEditorService.PostUnique(
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
			        Dispatcher,
			        ComponentData);
				
				return ValueTask.CompletedTask;
			});
    }

    private void ReceiveOnDoubleClick(MouseEventArgs mouseEventArgs)
    {
        var (model, viewModel) = TextEditorStateWrap.Value.GetModelAndViewModelOrDefault(
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

        TextEditorService.Post(onDoubleClick);
    }

    private void ReceiveContentOnMouseDown(MouseEventArgs mouseEventArgs)
    {
        _componentData.ThinksLeftMouseButtonIsDown = true;

        var (model, viewModel) = TextEditorStateWrap.Value.GetModelAndViewModelOrDefault(
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

        TextEditorService.Post(onMouseDown);
    }

    private void ReceiveContentOnMouseMove(MouseEventArgs mouseEventArgs)
    {
        _userMouseIsInside = true;

        if ((mouseEventArgs.Buttons & 1) == 0)
            _componentData.ThinksLeftMouseButtonIsDown = false;

        var localThinksLeftMouseButtonIsDown = _componentData.ThinksLeftMouseButtonIsDown;

        var (model, viewModel) = TextEditorStateWrap.Value.GetModelAndViewModelOrDefault(
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
						TextEditorService.PostUnique(
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

                    TextEditorService.PostUnique(
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

            TextEditorService.Post(onMouseMove);
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
    	var viewModel = TextEditorStateWrap.Value.ViewModelGetOrDefault(
        	TextEditorViewModelKey);
		
        var viewModelKey = viewModel?.ViewModelKey ?? Key<TextEditorViewModel>.Empty;
        if (viewModelKey == Key<TextEditorViewModel>.Empty)
            return;

		var onWheel = new OnWheel(
            wheelEventArgs,
            _componentData,
            viewModelKey);

        TextEditorService.Post(onWheel);
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

        var viewModel = TextEditorStateWrap.Value.ViewModelGetOrDefault(TextEditorViewModelKey);
        if (viewModel is null)
			return;

        // Natural scrolling for touch devices
        var diffX = previousTouchPoint.ClientX - currentTouchPoint.ClientX;
        var diffY = previousTouchPoint.ClientY - currentTouchPoint.ClientY;

        TextEditorService.PostUnique(
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
        var (model, viewModel) = TextEditorStateWrap.Value.GetModelAndViewModelOrDefault(
            TextEditorViewModelKey);

        var resourceUri = model?.ResourceUri ?? ResourceUri.Empty;
        var viewModelKey = viewModel?.ViewModelKey ?? Key<TextEditorViewModel>.Empty;

        if (resourceUri == ResourceUri.Empty ||
            viewModelKey == Key<TextEditorViewModel>.Empty)
        {
            return;
        }

        TextEditorService.PostRedundant(
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
        var (model, viewModel) = TextEditorStateWrap.Value.GetModelAndViewModelOrDefault(
            TextEditorViewModelKey);

        var resourceUri = model?.ResourceUri ?? ResourceUri.Empty;
        var viewModelKey = viewModel?.ViewModelKey ?? Key<TextEditorViewModel>.Empty;

        if (resourceUri == ResourceUri.Empty ||
            viewModelKey == Key<TextEditorViewModel>.Empty)
        {
            return;
        }

        TextEditorService.PostRedundant(
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
        var measurements = renderBatchLocal.ViewModel.CharAndLineMeasurements;

        var topInPixelsInvariantCulture = (index * measurements.LineHeight).ToCssValue();
        var top = $"top: {topInPixelsInvariantCulture}px;";

        var heightInPixelsInvariantCulture = measurements.LineHeight.ToCssValue();
        var height = $"height: {heightInPixelsInvariantCulture}px;";

        var widthInPixelsInvariantCulture = renderBatchLocal.GutterWidthInPixels.ToCssValue();
        var width = $"width: {widthInPixelsInvariantCulture}px;";

        var paddingLeftInPixelsInvariantCulture = TextEditorModel.GUTTER_PADDING_LEFT_IN_PIXELS.ToCssValue();
        var paddingLeft = $"padding-left: {paddingLeftInPixelsInvariantCulture}px;";

        var paddingRightInPixelsInvariantCulture = TextEditorModel.GUTTER_PADDING_RIGHT_IN_PIXELS.ToCssValue();
        var paddingRight = $"padding-right: {paddingRightInPixelsInvariantCulture}px;";

        return $"{width} {height} {top} {paddingLeft} {paddingRight}";
    }

    public string GetGutterSectionStyleCss(TextEditorRenderBatch renderBatchLocal)
    {
        var widthInPixelsInvariantCulture = renderBatchLocal.GutterWidthInPixels.ToCssValue();
        var width = $"width: {widthInPixelsInvariantCulture}px;";

        return width;
    }
    
    #endregion GutterDriverClose
    
    #region BodyDriverOpen
    public bool GlobalShowNewlines => TextEditorService.OptionsStateWrap.Value.Options.ShowNewlines;
    
    public string GetBodyStyleCss(TextEditorRenderBatch renderBatchLocal)
    {
        var gutterWidthInPixelsInvariantCulture = renderBatchLocal.GutterWidthInPixels.ToCssValue();

        var width = $"width: calc(100% - {gutterWidthInPixelsInvariantCulture}px);";
        var left = $"left: {gutterWidthInPixelsInvariantCulture}px;";

        return $"{width} {left}";
    }
    
    /* RowSection.razor Open */
    public string RowSection_GetRowStyleCss(TextEditorRenderBatch renderBatchLocal, int index, double? virtualizedRowLeftInPixels)
    {
        var charMeasurements = renderBatchLocal.ViewModel.CharAndLineMeasurements;

        var topInPixelsInvariantCulture = (index * charMeasurements.LineHeight).ToCssValue();
        var top = $"top: {topInPixelsInvariantCulture}px;";

        var heightInPixelsInvariantCulture = charMeasurements.LineHeight.ToCssValue();
        var height = $"height: {heightInPixelsInvariantCulture}px;";

        var virtualizedRowLeftInPixelsInvariantCulture = virtualizedRowLeftInPixels.GetValueOrDefault().ToCssValue();
        var left = $"left: {virtualizedRowLeftInPixelsInvariantCulture}px;";

        return $"{top} {height} {left}";
    }

    public void RowSection_AppendTextEscaped(
    	TextEditorRenderBatch renderBatchLocal,
        StringBuilder spanBuilder,
        RichCharacter richCharacter,
        string tabKeyOutput,
        string spaceKeyOutput)
    {
        switch (richCharacter.Value)
        {
            case '\t':
                spanBuilder.Append(tabKeyOutput);
                break;
            case ' ':
                spanBuilder.Append(spaceKeyOutput);
                break;
            case '\r':
                break;
            case '\n':
                break;
            case '<':
                spanBuilder.Append("&lt;");
                break;
            case '>':
                spanBuilder.Append("&gt;");
                break;
            case '"':
                spanBuilder.Append("&quot;");
                break;
            case '\'':
                spanBuilder.Append("&#39;");
                break;
            case '&':
                spanBuilder.Append("&amp;");
                break;
            default:
                spanBuilder.Append(richCharacter.Value);
                break;
        }
    }
    
    #endregion BodyDriverClose

    public void Dispose()
    {
        TextEditorStateWrap.StateChanged -= GeneralOnStateChangedEventHandler;
        TextEditorOptionsStateWrap.StateChanged -= TextEditorOptionsStateWrap_StateChanged;
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
