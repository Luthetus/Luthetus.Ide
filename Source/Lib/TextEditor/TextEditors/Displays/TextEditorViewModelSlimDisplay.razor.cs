using System.Text;
using System.Diagnostics;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Web;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.Common.RazorLib.Drags.Models;
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
using Luthetus.Common.RazorLib.Icons.Displays;
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
using Luthetus.TextEditor.RazorLib.Virtualizations.Models;
using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.Keymaps.Models;
using Luthetus.TextEditor.RazorLib.Exceptions;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays;

public sealed partial class TextEditorViewModelSlimDisplay : ComponentBase, IDisposable
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
    public IFileSystemProvider FileSystemProvider { get; set; } = null!;
    [Inject]
    public IDialogService DialogService { get; set; } = null!;
    [Inject]
    public IDropdownService DropdownService { get; set; } = null!;
    [Inject]
    public LuthetusTextEditorConfig TextEditorConfig { get; set; } = null!;
    [Inject]
    private IFindAllService FindAllService { get; set; } = null!;
    [Inject]
    private IDragService DragService { get; set; } = null!;

    [Parameter, EditorRequired]
    public Key<TextEditorViewModel> TextEditorViewModelKey { get; set; } = Key<TextEditorViewModel>.Empty;
    
    [Parameter]
    public ViewModelDisplayOptions ViewModelDisplayOptions { get; set; } = new();

    private readonly Guid _textEditorHtmlElementId = Guid.NewGuid();
    /// <summary>Using this lock in order to avoid the Dispose implementation decrementing when it shouldn't</summary>
    private readonly object _linkedViewModelLock = new();

    private TextEditorComponentData _componentData = null!;
    
    private TextEditorViewModel? _linkedViewModel;
    
    private bool _thinksTouchIsOccurring;
    private DateTime? _touchStartDateTime = null;
    private TouchEventArgs? _previousTouchEventArgs = null;
    private bool _userMouseIsInside;

    private IconDriver _iconDriver = new IconDriver(widthInPixels: 15, heightInPixels: 15);
    
    private string ContentElementId { get; set; }
    
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
	
	public bool GlobalShowNewlines => TextEditorService.OptionsApi.GetTextEditorOptionsState().Options.ShowNewlines;
	
	/// <summary>This property will need to be used when multi-cursor is added.</summary>
    public bool IsFocusTarget => true;

    public ElementReference? _cursorDisplayElementReference;
    public int _menuShouldGetFocusRequestCount;

    private readonly CancellationTokenSource _onMouseMoveCancellationTokenSource = new();
    private MouseEventArgs? _onMouseMoveMouseEventArgs;
    private Task _onMouseMoveTask = Task.CompletedTask;

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
    	SetComponentData();
    	
    	CssOnInitializedStepOne();
  		  
        ConstructRenderBatch();
        
        CssOnInitializedStepTwo();

        TextEditorService.TextEditorStateChanged += GeneralOnStateChangedEventHandler;
        TextEditorService.OptionsApi.StaticStateChanged += OnOptionStaticStateChanged;
        TextEditorService.OptionsApi.MeasuredStateChanged += OnOptionMeasuredStateChanged;
        TextEditorService.ViewModelApi.CursorShouldBlinkChanged += ViewModel_CursorShouldBlinkChanged;
        DragService.DragStateChanged += DragStateWrapOnStateChanged;

        base.OnInitialized();
    }
    
    protected override void OnParametersSet()
    {
    	HandleTextEditorViewModelKeyChange();
    	base.OnParametersSet();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await TextEditorService.JsRuntimeTextEditorApi
                .PreventDefaultOnWheelEvents(ContentElementId)
                .ConfigureAwait(false);

            QueueCalculateVirtualizationResultBackgroundTask(_componentData._currentRenderBatch);
        }

        if (_componentData._currentRenderBatch.ViewModel is not null && _componentData._currentRenderBatch.ViewModel.ShouldSetFocusAfterNextRender)
        {
            _componentData._currentRenderBatch.ViewModel.ShouldSetFocusAfterNextRender = false;
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    private async void GeneralOnStateChangedEventHandler() =>
        await InvokeAsync(StateHasChanged);
        
    private async void ViewModel_CursorShouldBlinkChanged() =>
        await InvokeAsync(StateHasChanged);

    private void HandleTextEditorViewModelKeyChange()
    {
    	TextEditorService.WorkerArbitrary.PostUnique(nameof(HandleTextEditorViewModelKeyChange), editContext =>
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
                _linkedViewModel?.DisplayTracker.DisposeComponentData(_componentData);
                nextViewModel?.DisplayTracker.RegisterComponentData(_componentData);

                _linkedViewModel = nextViewModel;

                if (nextViewModel is not null)
                    nextViewModel.ShouldRevealCursor = true;
            }
            
            return ValueTask.CompletedTask;
    	});
    }

	public async Task FocusTextEditorAsync()
	{
	  var nextViewModel = TextEditorService.TextEditorState.ViewModelGetOrDefault(TextEditorViewModelKey);
	  
	  if (nextViewModel is not null)
	  	await nextViewModel.FocusAsync();
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

    public int GetTabIndex()
    {
        if (!IsFocusTarget)
            return -1;

        return _componentData._activeRenderBatch.ViewModelDisplayOptions.TabIndex;
    }
    
    private void ReceiveOnKeyDown(KeyboardEventArgs keyboardEventArgs)
    {
    	// Inlining: 'EventUtils.IsKeyboardEventArgsNoise(keyboardEventArgs)'
    	if (keyboardEventArgs.Key == "Shift" ||
            keyboardEventArgs.Key == "Control" ||
            keyboardEventArgs.Key == "Alt" ||
            keyboardEventArgs.Key == "Meta")
        {
            return;
        }
        
        TextEditorService.WorkerUi.EnqueueOnKeyDown(
        	new OnKeyDown(
				_componentData,
	            new KeymapArgs(keyboardEventArgs),
	            TextEditorViewModelKey));
	}

    private void ReceiveOnContextMenu()
    {
		var localViewModelKey = TextEditorViewModelKey;

		TextEditorService.WorkerArbitrary.PostUnique(
			nameof(ReceiveOnContextMenu),
			editContext =>
			{
				var viewModelModifier = editContext.GetViewModelModifier(localViewModelKey);
				var modelModifier = editContext.GetModelModifier(viewModelModifier.ResourceUri);
				var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier);
        		var primaryCursor = cursorModifierBag.CursorModifier;

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
        TextEditorService.WorkerUi.EnqueueOnDoubleClick(
        	new OnDoubleClick(
	            mouseEventArgs,
				_componentData,
	            TextEditorViewModelKey));
    }

    private void ReceiveContentOnMouseDown(MouseEventArgs mouseEventArgs)
    {
        _componentData.ThinksLeftMouseButtonIsDown = true;
        _onMouseMoveMouseEventArgs = null;

        TextEditorService.WorkerUi.EnqueueOnMouseDown(
        	new OnMouseDown(
	            mouseEventArgs,
				_componentData,
	            TextEditorViewModelKey));
    }

    private void ReceiveContentOnMouseMove(MouseEventArgs mouseEventArgs)
	{
	    _userMouseIsInside = true;
	
	    // Buttons is a bit flag '& 1' gets if left mouse button is held
	    if ((mouseEventArgs.Buttons & 1) == 0)
	        _componentData.ThinksLeftMouseButtonIsDown = false;
	
	    var localThinksLeftMouseButtonIsDown = _componentData.ThinksLeftMouseButtonIsDown;
	
	    // MouseStoppedMovingTask
	    _onMouseMoveMouseEventArgs = mouseEventArgs;
            
        if (_onMouseMoveTask.IsCompleted)
        {
            var cancellationToken = _onMouseMoveCancellationTokenSource.Token;

            _onMouseMoveTask = Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                	var mouseMoveMouseEventArgs = _onMouseMoveMouseEventArgs;
                	
                	if (!_userMouseIsInside || _componentData.ThinksLeftMouseButtonIsDown || mouseMoveMouseEventArgs is null)
                	{
                		TextEditorService.WorkerArbitrary.PostUnique(
							nameof(ReceiveContentOnMouseMove),
							editContext =>
							{
								var viewModelModifier = editContext.GetViewModelModifier(TextEditorViewModelKey);

                                if (viewModelModifier is null)
                                    return ValueTask.CompletedTask;

                                viewModelModifier.TooltipViewModel = null;

								return ValueTask.CompletedTask;
							});
						break;
                	}
                	
                    await Task.Delay(400).ConfigureAwait(false);
                    
                    if (mouseMoveMouseEventArgs == _onMouseMoveMouseEventArgs)
                    {
                        await _componentData.ContinueRenderingTooltipAsync().ConfigureAwait(false);

				        TextEditorService.WorkerArbitrary.PostUnique(
				            nameof(TextEditorCommandDefaultFunctions.HandleMouseStoppedMovingEventAsync),
				            editContext =>
				            {
				            	var viewModelModifier = editContext.GetViewModelModifier(TextEditorViewModelKey);
				                var modelModifier = editContext.GetModelModifier(viewModelModifier.ResourceUri);
				                var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier);
				                var primaryCursorModifier = cursorModifierBag.CursorModifier;
				                
				                if (modelModifier is null || viewModelModifier is null || !cursorModifierBag.ConstructorWasInvoked || primaryCursorModifier is null)
				                    return ValueTask.CompletedTask;
				    
				                return TextEditorCommandDefaultFunctions.HandleMouseStoppedMovingEventAsync(
				                    editContext,
				                    modelModifier,
				                    viewModelModifier,
				                    mouseMoveMouseEventArgs,
				                    _componentData,
				                    TextEditorComponentRenderers,
				                    viewModelModifier.ResourceUri);
				            });

                        break;
                    }
                }
            });
        }
	
	    if (!_componentData.ThinksLeftMouseButtonIsDown)
	        return;
	    
	    if (localThinksLeftMouseButtonIsDown)
	    {
	        TextEditorService.WorkerUi.EnqueueOnMouseMove(
	            new OnMouseMove(
	            mouseEventArgs,
	            _componentData,
	            TextEditorViewModelKey));
	    }
	}

    private void ReceiveContentOnMouseOut(MouseEventArgs mouseEventArgs)
    {
        _userMouseIsInside = false;
    }
    
    private void ReceiveOnWheel(WheelEventArgs wheelEventArgs)
    {
    	TextEditorService.WorkerUi.EnqueueOnWheel(
        	new OnWheel(
	            wheelEventArgs,
	            _componentData,
	            TextEditorViewModelKey));
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

        // Natural scrolling for touch devices
        var diffX = previousTouchPoint.ClientX - currentTouchPoint.ClientX;
        var diffY = previousTouchPoint.ClientY - currentTouchPoint.ClientY;

        TextEditorService.WorkerArbitrary.PostUnique(
            nameof(ReceiveOnTouchMove),
            editContext =>
			{
				var viewModelModifier = editContext.GetViewModelModifier(TextEditorViewModelKey);
				
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
        CancellationToken cancellationToken)
    {
        var viewModel = TextEditorService.TextEditorState.ViewModelGetOrDefault(TextEditorViewModelKey);
        if (viewModel is null)
            return;

        TextEditorService.WorkerArbitrary.PostRedundant(
            nameof(QueueRemeasureBackgroundTask),
			viewModel.ResourceUri,
			viewModel.ViewModelKey,
            editContext =>
            {
            	var viewModelModifier = editContext.GetViewModelModifier(viewModel.ViewModelKey);

				if (viewModelModifier is null)
					return ValueTask.CompletedTask;

            	return TextEditorService.ViewModelApi.RemeasureAsync(
            		editContext,
			        viewModelModifier,
			        CancellationToken.None);
	        });
    }

    private void QueueCalculateVirtualizationResultBackgroundTask(
		TextEditorRenderBatch localCurrentRenderBatch)
    {
        var viewModel = TextEditorService.TextEditorState.ViewModelGetOrDefault(TextEditorViewModelKey);
        if (viewModel is null)
            return;

        TextEditorService.WorkerArbitrary.PostRedundant(
            nameof(QueueCalculateVirtualizationResultBackgroundTask),
			viewModel.ResourceUri,
			viewModel.ViewModelKey,
            editContext =>
            {
            	var modelModifier = editContext.GetModelModifier(viewModel.ResourceUri);
            	var viewModelModifier = editContext.GetViewModelModifier(viewModel.ViewModelKey);

				if (modelModifier is null || viewModelModifier is null)
					return ValueTask.CompletedTask;
            	
            	viewModelModifier.CharAndLineMeasurements = TextEditorService.OptionsApi.GetOptions().CharAndLineMeasurements;
            	
            	TextEditorService.ViewModelApi.CalculateVirtualizationResult(
            		editContext,
			        modelModifier,
			        viewModelModifier,
			        CancellationToken.None);
			    return ValueTask.CompletedTask;
            });
    }
    
    private async Task HORIZONTAL_HandleOnMouseDownAsync(MouseEventArgs mouseEventArgs)
    {
    	var renderBatchLocal = ComponentData._activeRenderBatch;
    	if (renderBatchLocal is null)
    		return;
    		
    	HORIZONTAL_thinksLeftMouseButtonIsDown = true;
		HORIZONTAL_scrollLeftOnMouseDown = _componentData._activeRenderBatch.ViewModel.ScrollbarDimensions.ScrollLeft;

		var scrollbarBoundingClientRect = await TextEditorService.JsRuntimeCommonApi
			.MeasureElementById(HORIZONTAL_ScrollbarElementId)
			.ConfigureAwait(false);

		// Drag far up to reset scroll to original
		var textEditorDimensions = _componentData._activeRenderBatch.ViewModel.TextEditorDimensions;
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
    	var renderBatchLocal = _componentData._activeRenderBatch;
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
    	var renderBatchLocal = _componentData._activeRenderBatch;
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

			TextEditorService.WorkerUi.EnqueueOnScrollHorizontal(onScrollHorizontal);
        }
        else
        {
            HORIZONTAL_thinksLeftMouseButtonIsDown = false;
        }

        return Task.CompletedTask;
    }
    
    private Task VERTICAL_DragEventHandlerScrollAsync(MouseEventArgs localMouseDownEventArgs, MouseEventArgs onDragMouseEventArgs)
    {
    	var renderBatchLocal = _componentData._activeRenderBatch;
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

			TextEditorService.WorkerUi.EnqueueOnScrollVertical(onScrollVertical);
        }
        else
        {
            VERTICAL_thinksLeftMouseButtonIsDown = false;
        }

        return Task.CompletedTask;
    }
    
    private async void OnOptionMeasuredStateChanged()
    {
    	_componentData.SetWrapperCssAndStyle();
    	QueueCalculateVirtualizationResultBackgroundTask(_componentData._currentRenderBatch);
    }
    
    private async void OnOptionStaticStateChanged()
    {
    	_componentData.SetWrapperCssAndStyle();
    	await InvokeAsync(StateHasChanged);
    }
    
    private void CssOnInitializedStepOne()
	{
		_componentData.SetWrapperCssAndStyle();
    	
    	ContentElementId = $"luth_te_text-editor-content_{_textEditorHtmlElementId}";
	    
	    VERTICAL_ScrollbarElementId = $"luth_te_{VERTICAL_scrollbarGuid}";
	    VERTICAL_ScrollbarSliderElementId = $"luth_te_{VERTICAL_scrollbarGuid}-slider";
	    
	    HORIZONTAL_ScrollbarElementId = $"luth_te_{HORIZONTAL_scrollbarGuid}";
	    HORIZONTAL_ScrollbarSliderElementId = $"luth_te_{HORIZONTAL_scrollbarGuid}-slider";
	    
	    var paddingLeftInPixelsInvariantCulture = TextEditorModel.GUTTER_PADDING_LEFT_IN_PIXELS.ToCssValue();
	    var paddingRightInPixelsInvariantCulture = TextEditorModel.GUTTER_PADDING_RIGHT_IN_PIXELS.ToCssValue();
        _componentData._gutterPaddingStyleCssString = $"padding-left: {paddingLeftInPixelsInvariantCulture}px; padding-right: {paddingRightInPixelsInvariantCulture}px;";
        
        _componentData._scrollbarSizeInPixelsCssValue = ScrollbarFacts.SCROLLBAR_SIZE_IN_PIXELS.ToCssValue();
	}
	
	private void CssOnInitializedStepTwo()
	{
		_componentData._blinkAnimationCssClassOn = $"luth_te_text-editor-cursor luth_te_blink ";
	    _componentData._blinkAnimationCssClassOff = $"luth_te_text-editor-cursor ";
	    
	    var cursorCssClassString = _componentData._activeRenderBatch?.Options?.Keymap?.GetCursorCssClassString();
        if (cursorCssClassString is not null)
        {
        	_componentData._blinkAnimationCssClassOn += cursorCssClassString;
        	_componentData._blinkAnimationCssClassOff += cursorCssClassString;
        }
	}

	protected override bool ShouldRender()
    {
        if (_linkedViewModel is null)
            HandleTextEditorViewModelKeyChange();

        ConstructRenderBatch();

        if (_componentData._currentRenderBatch.ViewModel is not null && _componentData._currentRenderBatch.Options is not null)
        {
            if (_componentData._currentRenderBatch.ViewModel.DisplayTracker.ConsumeIsFirstDisplay())
				QueueCalculateVirtualizationResultBackgroundTask(ComponentData._currentRenderBatch);
        }
        
        // Check if the gutter width changed. If so, re-measure text editor.
        if (_componentData._activeRenderBatch?.ViewModel is not null)
		{
			var gutterWidthInPixels = _componentData._activeRenderBatch.GutterWidthInPixels;
		
			if (_componentData._previousGutterWidthInPixels >= 0 && gutterWidthInPixels >= 0)
			{
	        	var absoluteValueDifference = Math.Abs(_componentData._previousGutterWidthInPixels - gutterWidthInPixels);
	        	
	        	if (absoluteValueDifference >= 0.2)
	        	{
	        		_componentData._previousGutterWidthInPixels = gutterWidthInPixels;
	        		var widthInPixelsInvariantCulture = gutterWidthInPixels.ToCssValue();
	        		
	        		_componentData._uiStringBuilder.Clear();
	        		_componentData._uiStringBuilder.Append("width: ");
	        		_componentData._uiStringBuilder.Append(widthInPixelsInvariantCulture);
	        		_componentData._uiStringBuilder.Append("px;");
	        		_componentData._gutterWidthStyleCssString = _componentData._uiStringBuilder.ToString();
	        		
	        		_componentData._uiStringBuilder.Clear();
	        		_componentData._uiStringBuilder.Append(_componentData._lineHeightStyleCssString);
			        _componentData._uiStringBuilder.Append(_componentData._gutterWidthStyleCssString);
			        _componentData._uiStringBuilder.Append(_componentData._gutterPaddingStyleCssString);
	        		_componentData._gutterHeightWidthPaddingStyleCssString = _componentData._uiStringBuilder.ToString();
	        		
	        		_componentData._uiStringBuilder.Clear();
	        		_componentData._uiStringBuilder.Append("width: calc(100% - ");
			        _componentData._uiStringBuilder.Append(widthInPixelsInvariantCulture);
			        _componentData._uiStringBuilder.Append("px); left: ");
			        _componentData._uiStringBuilder.Append(widthInPixelsInvariantCulture);
			        _componentData._uiStringBuilder.Append("px;");
	        		_componentData._bodyStyle = _componentData._uiStringBuilder.ToString();
	        			        		
	        		// (2025-03-28)
	        		// Console.WriteLine("_activeRenderBatch.ViewModel.DisplayTracker.PostScrollAndRemeasure();");
	        		_componentData._activeRenderBatch.ViewModel.DisplayTracker.PostScrollAndRemeasure();
	        		return true;
	        	}
			}
			
			// NOTE: The 'gutterWidth' version of this will do a re-measure,...
			// ...and therefore will return if its condition branch was entered.
			var lineHeightInPixels = _componentData._activeRenderBatch.ViewModel.CharAndLineMeasurements.LineHeight;
			
			// The 'gutterWidth' version of this code was written first,
			// and this is just more or less copying the template.
			//
			// TODO: What was the reason for 'gutterWidth' not just doing an '!='?
			//
			if (_componentData._previousLineHeightInPixels >= 0 && lineHeightInPixels >= 0)
			{
				var absoluteValueDifference = Math.Abs(_componentData._previousLineHeightInPixels - lineHeightInPixels);
	        	
	        	if (absoluteValueDifference >= 0.2)
	        	{
	        		_componentData._previousLineHeightInPixels = lineHeightInPixels;
					var heightInPixelsInvariantCulture = lineHeightInPixels.ToCssValue();
					
					_componentData._uiStringBuilder.Clear();
	        		_componentData._uiStringBuilder.Append("height: ");
			        _componentData._uiStringBuilder.Append(heightInPixelsInvariantCulture);
			        _componentData._uiStringBuilder.Append("px;");
			        _componentData._lineHeightStyleCssString = _componentData._uiStringBuilder.ToString();
			        
			        _componentData._uiStringBuilder.Clear();
	        		_componentData._uiStringBuilder.Append(_componentData._lineHeightStyleCssString);
			        _componentData._uiStringBuilder.Append(_componentData._gutterWidthStyleCssString);
			        _componentData._uiStringBuilder.Append(_componentData._gutterPaddingStyleCssString);
	        		_componentData._gutterHeightWidthPaddingStyleCssString = _componentData._uiStringBuilder.ToString();
        		}
			}
			
			var textEditorWidth = _componentData._activeRenderBatch.ViewModel.TextEditorDimensions.Width;
			var textEditorHeight = _componentData._activeRenderBatch.ViewModel.TextEditorDimensions.Height;
			var scrollLeft = _componentData._activeRenderBatch.ViewModel.ScrollbarDimensions.ScrollLeft;
			var scrollWidth = _componentData._activeRenderBatch.ViewModel.ScrollbarDimensions.ScrollWidth;
			var scrollHeight = _componentData._activeRenderBatch.ViewModel.ScrollbarDimensions.ScrollHeight;
			var scrollTop = _componentData._activeRenderBatch.ViewModel.ScrollbarDimensions.ScrollTop;
			
			bool shouldCalculateVerticalSlider = false;
			bool shouldCalculateHorizontalSlider = false;
			bool shouldCalculateHorizontalScrollbar = false;
			
			if (_componentData._previousTextEditorHeightInPixels >= 0 && textEditorHeight >= 0)
			{
				var absoluteValueDifference = Math.Abs(_componentData._previousTextEditorHeightInPixels - textEditorHeight);
	        	
	        	if (absoluteValueDifference >= 0.2)
	        	{
	        		_componentData._previousTextEditorHeightInPixels = textEditorHeight;
	        		shouldCalculateVerticalSlider = true;
			    }
			}
			
			if (_componentData._previousScrollHeightInPixels >= 0 && scrollHeight >= 0)
			{
				var absoluteValueDifference = Math.Abs(_componentData._previousScrollHeightInPixels - scrollHeight);
	        	
	        	if (absoluteValueDifference >= 0.2)
	        	{
	        		_componentData._previousScrollHeightInPixels = scrollHeight;
	        		shouldCalculateVerticalSlider = true;
			    }
			}
			
			if (_componentData._previousScrollTopInPixels >= 0 && scrollTop >= 0)
			{
				var absoluteValueDifference = Math.Abs(_componentData._previousScrollTopInPixels - scrollTop);
	        	
	        	if (absoluteValueDifference >= 0.2)
	        	{
	        		_componentData._previousScrollTopInPixels = scrollTop;
	        		shouldCalculateVerticalSlider = true;
			    }
			}
			
			if (_componentData._previousTextEditorWidthInPixels >= 0 && textEditorWidth >= 0)
			{
				var absoluteValueDifference = Math.Abs(_componentData._previousTextEditorWidthInPixels - textEditorWidth);
	        	
	        	if (absoluteValueDifference >= 0.2)
	        	{
	        		_componentData._previousTextEditorWidthInPixels = textEditorWidth;
	        		shouldCalculateHorizontalSlider = true;
	        		shouldCalculateHorizontalScrollbar = true;
			    }
			}
			
			if (_componentData._previousScrollWidthInPixels >= 0 && scrollWidth >= 0)
			{
				var absoluteValueDifference = Math.Abs(_componentData._previousScrollWidthInPixels - scrollWidth);
	        	
	        	if (absoluteValueDifference >= 0.2)
	        	{
	        		_componentData._previousScrollWidthInPixels = scrollWidth;
	        		shouldCalculateHorizontalSlider = true;
			    }
			}
			
			if (_componentData._previousScrollLeftInPixels >= 0 && scrollLeft >= 0)
			{
				var absoluteValueDifference = Math.Abs(_componentData._previousScrollLeftInPixels - scrollLeft);
	        	
	        	if (absoluteValueDifference >= 0.2)
	        	{
	        		_componentData._previousScrollLeftInPixels = scrollLeft;
	        		shouldCalculateHorizontalSlider = true;
			    }
			}

			if (shouldCalculateVerticalSlider)
				_componentData.VERTICAL_GetSliderVerticalStyleCss();
			
			if (shouldCalculateHorizontalSlider)
				_componentData.HORIZONTAL_GetSliderHorizontalStyleCss();
			
			if (shouldCalculateHorizontalScrollbar)
				_componentData.HORIZONTAL_GetScrollbarHorizontalStyleCss();
			
			_componentData.GetCursorAndCaretRowStyleCss();
		}

        return true;
    }
    
    private void ConstructRenderBatch()
    {
    	var localTextEditorState = TextEditorService.TextEditorState;
    
    	var model_viewmodel_tuple = localTextEditorState.GetModelAndViewModelOrDefault(
			TextEditorViewModelKey);
		
		var textEditorOptions = TextEditorService.OptionsApi.GetTextEditorOptionsState().Options;
		
		string fontFamily;
		if (!string.IsNullOrWhiteSpace(textEditorOptions.CommonOptions?.FontFamily))
			fontFamily = textEditorOptions.CommonOptions!.FontFamily;
		else
			fontFamily = TextEditorRenderBatch.DEFAULT_FONT_FAMILY;
			
		int fontSizeInPixels;
		if (textEditorOptions.CommonOptions?.FontSizeInPixels is not null)
            fontSizeInPixels = textEditorOptions.CommonOptions.FontSizeInPixels;
        else
        	fontSizeInPixels = TextEditorOptionsState.DEFAULT_FONT_SIZE_IN_PIXELS;
				
        var renderBatchUnsafe = new TextEditorRenderBatch(
            model_viewmodel_tuple.Model,
            model_viewmodel_tuple.ViewModel,
            TextEditorService.OptionsApi.GetTextEditorOptionsState().Options,
            fontFamily,
            fontSizeInPixels,
            ViewModelDisplayOptions,
			_componentData);
        
        _componentData._previousRenderBatch = _componentData._currentRenderBatch;
        
        _componentData._currentRenderBatch = renderBatchUnsafe;
        
        _componentData._activeRenderBatch = renderBatchUnsafe.Validate() ? renderBatchUnsafe : null;
        
        RenderBatchChanged?.Invoke();
    }
    
    private void SetComponentData()
    {
		_componentData = new(
			_textEditorHtmlElementId,
			ViewModelDisplayOptions,
			TextEditorService.OptionsApi.GetTextEditorOptionsState().Options,
			this,
			DropdownService,
			ClipboardService,
			CommonComponentRenderers,
			NotificationService,
			TextEditorService,
			TextEditorComponentRenderers,
			FindAllService,
			EnvironmentProvider,
			FileSystemProvider,
			ServiceProvider);
    }
    
    public void Dispose()
    {
    	// ScrollbarSection.razor.cs
    	DragService.DragStateChanged -= DragStateWrapOnStateChanged;
    
    	// TextEditorViewModelDisplay.razor.cs
        TextEditorService.TextEditorStateChanged -= GeneralOnStateChangedEventHandler;
        TextEditorService.OptionsApi.StaticStateChanged -= OnOptionStaticStateChanged;
        TextEditorService.OptionsApi.MeasuredStateChanged -= OnOptionMeasuredStateChanged;
		TextEditorService.ViewModelApi.CursorShouldBlinkChanged -= ViewModel_CursorShouldBlinkChanged;

        lock (_linkedViewModelLock)
        {
            if (_linkedViewModel is not null)
            {
                _linkedViewModel.DisplayTracker.DisposeComponentData(_componentData);
                _linkedViewModel = null;
            }
        }

        _onMouseMoveCancellationTokenSource.Cancel();
        _onMouseMoveCancellationTokenSource.Dispose();
    }
}
