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
    private (TextEditorRenderBatchUnsafe Unsafe, TextEditorRenderBatchValidated? Validated)? _previousRenderBatchTuple;
    public (TextEditorRenderBatchUnsafe Unsafe, TextEditorRenderBatchValidated? Validated) _storedRenderBatchTuple;
    private TextEditorViewModel? _linkedViewModel;
    
    private GutterDriver _gutterDriver;
    public VirtualizationDriver _gutterVirtualizationDriver;
    private BodyDriver _bodyDriver;
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
    
    	_gutterDriver = new(this);
    	_gutterVirtualizationDriver = new(
    		this,
    		useHorizontalVirtualization: false,
    		useVerticalVirtualization: true);
    	_bodyDriver = new(this);
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

        if (_storedRenderBatchTuple.Unsafe.ViewModel is not null && _storedRenderBatchTuple.Unsafe.Options is not null)
        {
            var isFirstDisplay = _storedRenderBatchTuple.Unsafe.ViewModel.DisplayTracker.ConsumeIsFirstDisplay();

            var previousOptionsRenderStateKey = _previousRenderBatchTuple?.Unsafe?.Options?.RenderStateKey ?? Key<RenderState>.Empty;
            var currentOptionsRenderStateKey = _storedRenderBatchTuple.Unsafe.Options.RenderStateKey;

			if (previousOptionsRenderStateKey != currentOptionsRenderStateKey || isFirstDisplay)
            {
                QueueRemeasureBackgroundTask(
                    _storedRenderBatchTuple.Unsafe,
                    MeasureCharacterWidthAndRowHeightElementId,
                    COUNT_OF_TEST_CHARACTERS,
                    CancellationToken.None);
            }

            if (isFirstDisplay)
				QueueCalculateVirtualizationResultBackgroundTask(_storedRenderBatchTuple.Unsafe);
        }
        
        // Check if the gutter width changed. If so, re-measure text editor.
        var viewModel = _storedRenderBatchTuple.Validated?.ViewModel;
        var gutterWidthInPixels = _storedRenderBatchTuple.Validated?.GutterWidthInPixels;
        
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
                _storedRenderBatchTuple.Unsafe,
                MeasureCharacterWidthAndRowHeightElementId,
                COUNT_OF_TEST_CHARACTERS,
                CancellationToken.None);

            QueueCalculateVirtualizationResultBackgroundTask(_storedRenderBatchTuple.Unsafe);
        }

        if (_storedRenderBatchTuple.Unsafe.ViewModel is not null && _storedRenderBatchTuple.Unsafe.ViewModel.UnsafeState.ShouldSetFocusAfterNextRender)
        {
            _storedRenderBatchTuple.Unsafe.ViewModel.UnsafeState.ShouldSetFocusAfterNextRender = false;
            // await FocusTextEditorAsync().ConfigureAwait(false);
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    private void ConstructRenderBatch()
    {
    	var localTextEditorState = TextEditorStateWrap.Value;
    
    	var model_viewmodel_tuple = localTextEditorState.GetModelAndViewModelOrDefault(
			TextEditorViewModelKey);
				
        var renderBatchUnsafe = new TextEditorRenderBatchUnsafe(
            model_viewmodel_tuple.Model,
            model_viewmodel_tuple.ViewModel,
            TextEditorOptionsStateWrap.Value.Options,
            ITextEditorRenderBatch.DEFAULT_FONT_FAMILY,
            TextEditorOptionsState.DEFAULT_FONT_SIZE_IN_PIXELS,
            ViewModelDisplayOptions,
			_componentData);

        if (!string.IsNullOrWhiteSpace(renderBatchUnsafe.Options?.CommonOptions?.FontFamily))
        {
            renderBatchUnsafe = renderBatchUnsafe with
            {
                FontFamily = renderBatchUnsafe.Options!.CommonOptions!.FontFamily
            };
        }

        if (renderBatchUnsafe.Options!.CommonOptions?.FontSizeInPixels is not null)
        {
            renderBatchUnsafe = renderBatchUnsafe with
            {
                FontSizeInPixels = renderBatchUnsafe.Options!.CommonOptions.FontSizeInPixels
            };
        }
        
        if (renderBatchUnsafe.ViewModelDisplayOptions.KeymapOverride is not null)
        {
            renderBatchUnsafe = renderBatchUnsafe with
            {
                Options = renderBatchUnsafe.Options with
                {
                    Keymap = renderBatchUnsafe.ViewModelDisplayOptions.KeymapOverride
				}
            };
        }

        _previousRenderBatchTuple = _storedRenderBatchTuple;
        
        _storedRenderBatchTuple =
	        (renderBatchUnsafe,
	        renderBatchUnsafe.IsValid ? new TextEditorRenderBatchValidated(renderBatchUnsafe) : null);
        
        RenderBatchChanged?.Invoke();
    }
    
    private void SetComponentData()
    {
		_componentData = new(
			_textEditorHtmlElementId,
			ViewModelDisplayOptions,
			_storedRenderBatchTuple.Unsafe.Options,
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
        ITextEditorRenderBatch localRefCurrentRenderBatch,
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
		ITextEditorRenderBatch localCurrentRenderBatch)
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
