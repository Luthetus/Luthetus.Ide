using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Immutable;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.TextEditor.RazorLib.Autocompletes.Models;
using Luthetus.TextEditor.RazorLib.ComponentRenderers.Models;
using Luthetus.TextEditor.RazorLib.Options.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.States;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Options.States;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.Clipboards.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.TextEditor.RazorLib.Keymaps.Models.Defaults;
using Luthetus.TextEditor.RazorLib.Exceptions;
using Luthetus.TextEditor.RazorLib.JsRuntimes.Models;
using Luthetus.TextEditor.RazorLib.Events.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays;

public partial class TextEditorViewModelDisplay : ComponentBase, IDisposable
{
    [Inject]
    private IState<TextEditorModelState> TextEditorModelStateWrap { get; set; } = null!;
    [Inject]
    private IState<TextEditorViewModelState> TextEditorViewModelsStateWrap { get; set; } = null!;
    [Inject]
    private IState<TextEditorOptionsState> TextEditorOptionsStateWrap { get; set; } = null!;
    [Inject]
    internal IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    internal IServiceProvider ServiceProvider { get; set; } = null!;
    [Inject]
    internal ITextEditorService TextEditorService { get; set; } = null!;
    [Inject]
    internal IAutocompleteIndexer AutocompleteIndexer { get; set; } = null!;
    [Inject]
    internal IJSRuntime JsRuntime { get; set; } = null!;
    [Inject]
    internal IClipboardService ClipboardService { get; set; } = null!;
    [Inject]
    private IBackgroundTaskService BackgroundTaskService { get; set; } = null!;
    [Inject]
    internal ILuthetusTextEditorComponentRenderers LuthetusTextEditorComponentRenderers { get; set; } = null!;
    [Inject]
    internal LuthetusTextEditorConfig TextEditorConfig { get; set; } = null!;

    [Parameter, EditorRequired]
    public Key<TextEditorViewModel> TextEditorViewModelKey { get; set; } = Key<TextEditorViewModel>.Empty;
    
    [Parameter]
    public ViewModelDisplayOptions ViewModelDisplayOptions { get; set; } = new();

    internal readonly Guid _textEditorHtmlElementId = Guid.NewGuid();
    /// <summary>Using this lock in order to avoid the Dispose implementation decrementing when it shouldn't</summary>
    private readonly object _linkedViewModelLock = new();
    // private readonly ThrottleAvailability _throttleAvailabilityShouldRender = new(TimeSpan.FromMilliseconds(30));

    internal TextEditorEvents _events = null!;
    private bool _thinksTouchIsOccurring;
    private TouchEventArgs? _previousTouchEventArgs = null;
    private DateTime? _touchStartDateTime = null;
    private BodySection? _bodySectionComponent;
    private MeasureCharacterWidthAndRowHeight? _measureCharacterWidthAndRowHeightComponent;
    private bool _userMouseIsInside;
    private TextEditorRenderBatchUnsafe _storedRenderBatch = null!;
    private TextEditorRenderBatchUnsafe? _previousRenderBatch;
    private TextEditorViewModel? _linkedViewModel;
	private Task _shouldRenderSkipTask = Task.CompletedTask;

    internal CursorDisplay? CursorDisplay => _bodySectionComponent?.CursorDisplayComponent;
    private string MeasureCharacterWidthAndRowHeightElementId => $"luth_te_measure-character-width-and-row-height_{_textEditorHtmlElementId}";
    private string ContentElementId => $"luth_te_text-editor-content_{_textEditorHtmlElementId}";
    internal string ProportionalFontMeasurementsContainerElementId => $"luth_te_text-editor-proportional-font-measurement-container_{_textEditorHtmlElementId}";

    protected override async Task OnParametersSetAsync()
    {
        HandleTextEditorViewModelKeyChange();

        await base.OnParametersSetAsync();
    }

    protected override void OnInitialized()
    {
        ConstructRenderBatch();

        _events = new(this, _storedRenderBatch.Options);

        TextEditorViewModelsStateWrap.StateChanged += GeneralOnStateChangedEventHandler;
        TextEditorOptionsStateWrap.StateChanged += GeneralOnStateChangedEventHandler;

        base.OnInitialized();
    }

    protected override bool ShouldRender()
    {
        //var shouldRender = _throttleAvailabilityShouldRender.CheckAvailability();

   //     if (!shouldRender)
   //     {
   //         _countShouldRenderFalse++;

			//if (_shouldRenderSkipTask.IsCompleted)
			//{
			//	_shouldRenderSkipTask = Task.Run(async () =>
			//	{
			//		await InvokeAsync(StateHasChanged);
			//	});
			//}
            
   //         return false;
   //     }

        var shouldRender = true;

        if (_linkedViewModel is null)
            HandleTextEditorViewModelKeyChange();

        if (shouldRender)
            ConstructRenderBatch();

        if (_storedRenderBatch?.ViewModel is not null && _storedRenderBatch?.Options is not null)
        {
            var isFirstDisplay = _storedRenderBatch.ViewModel.DisplayTracker.ConsumeIsFirstDisplay();

            var previousOptionsRenderStateKey = _previousRenderBatch?.Options?.RenderStateKey ?? Key<RenderState>.Empty;
            var currentOptionsRenderStateKey = _storedRenderBatch.Options.RenderStateKey;

			_ = Task.Run(async () =>
			{
				if (previousOptionsRenderStateKey != currentOptionsRenderStateKey || isFirstDisplay)
	            {
	                await QueueRemeasureBackgroundTask(
	                    _storedRenderBatch,
	                    MeasureCharacterWidthAndRowHeightElementId,
	                    _measureCharacterWidthAndRowHeightComponent?.CountOfTestCharacters ?? 0,
	                    CancellationToken.None);
	            }
	
	            if (isFirstDisplay)
					await QueueCalculateVirtualizationResultBackgroundTask(_storedRenderBatch);
			});
        }

        return shouldRender;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JsRuntime.GetLuthetusTextEditorApi()
                .PreventDefaultOnWheelEvents(ContentElementId)
                .ConfigureAwait(false);

            await QueueRemeasureBackgroundTask(
                _storedRenderBatch,
                MeasureCharacterWidthAndRowHeightElementId,
                _measureCharacterWidthAndRowHeightComponent?.CountOfTestCharacters ?? 0,
                CancellationToken.None);

            await QueueCalculateVirtualizationResultBackgroundTask(_storedRenderBatch);
        }

        if (_storedRenderBatch?.ViewModel is not null && _storedRenderBatch.ViewModel.UnsafeState.ShouldSetFocusAfterNextRender)
        {
            _storedRenderBatch.ViewModel.UnsafeState.ShouldSetFocusAfterNextRender = false;
            await FocusTextEditorAsync().ConfigureAwait(false);
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    public TextEditorModel? GetModel() => TextEditorService.ViewModelApi.GetModelOrDefault(TextEditorViewModelKey);

    public TextEditorViewModel? GetViewModel() => TextEditorViewModelsStateWrap.Value.ViewModelList.FirstOrDefault(
        x => x.ViewModelKey == TextEditorViewModelKey);

    public TextEditorOptions? GetOptions() => TextEditorOptionsStateWrap.Value.Options;

    private void ConstructRenderBatch()
    {
        var renderBatch = new TextEditorRenderBatchUnsafe(
            GetModel(),
            GetViewModel(),
            GetOptions(),
            ITextEditorRenderBatch.DEFAULT_FONT_FAMILY,
            TextEditorOptionsState.DEFAULT_FONT_SIZE_IN_PIXELS,
            ViewModelDisplayOptions,
            _events);

        if (!string.IsNullOrWhiteSpace(renderBatch.Options?.CommonOptions?.FontFamily))
        {
            renderBatch = renderBatch with
            {
                FontFamily = renderBatch.Options!.CommonOptions!.FontFamily
            };
        }

        if (renderBatch.Options!.CommonOptions?.FontSizeInPixels is not null)
        {
            renderBatch = renderBatch with
            {
                FontSizeInPixels = renderBatch.Options!.CommonOptions.FontSizeInPixels
            };
        }
        
        if (renderBatch.ViewModelDisplayOptions.KeymapOverride is not null)
        {
            renderBatch = renderBatch with
            {
                Options = renderBatch.Options with
                {
                    Keymap = renderBatch.ViewModelDisplayOptions.KeymapOverride
				}
            };
        }

        _previousRenderBatch = _storedRenderBatch;
        _storedRenderBatch = renderBatch;
    }

    private async void GeneralOnStateChangedEventHandler(object? sender, EventArgs e) =>
        await InvokeAsync(StateHasChanged);

    private void HandleTextEditorViewModelKeyChange()
    {
        lock (_linkedViewModelLock)
        {
            var localTextEditorViewModelKey = TextEditorViewModelKey;

            // Don't use the method 'GetViewModel()'. The logic here needs to be transactional, the TextEditorViewModelKey must not change.
            var nextViewModel = TextEditorViewModelsStateWrap.Value.ViewModelList.FirstOrDefault(
                x => x.ViewModelKey == localTextEditorViewModelKey);

            Key<TextEditorViewModel> nextViewModelKey;

            if (nextViewModel is null)
                nextViewModelKey = Key<TextEditorViewModel>.Empty;
            else
                nextViewModelKey = nextViewModel.ViewModelKey;

            var linkedViewModelKey = _linkedViewModel?.ViewModelKey ?? Key<TextEditorViewModel>.Empty;
            var viewKeyChanged = nextViewModelKey != linkedViewModelKey;

            if (viewKeyChanged)
            {
                _linkedViewModel?.DisplayTracker.DecrementLinks(TextEditorModelStateWrap);
                nextViewModel?.DisplayTracker.IncrementLinks(TextEditorModelStateWrap);

                _linkedViewModel = nextViewModel;

                if (nextViewModel is not null)
                    nextViewModel.UnsafeState.ShouldRevealCursor = true;
            }
        }
    }

    public async Task FocusTextEditorAsync()
    {
        if (CursorDisplay is not null)
            await CursorDisplay.FocusAsync().ConfigureAwait(false);
    }

    private async Task ReceiveOnKeyDown(KeyboardEventArgs keyboardEventArgs)
    {
        if (EventUtils.IsKeyboardEventArgsNoise(keyboardEventArgs))
            return;

        var resourceUri = GetModel()?.ResourceUri;
        
		var viewModel = GetViewModel();
		var viewModelKey = viewModel?.ViewModelKey;

        if (resourceUri is null || viewModelKey is null)
			return;

		var workKeyDown = new TextEditorWorkKeyDown(
			resourceUri,
			viewModel.PrimaryCursor.Key,
			null,
			keyboardEventArgs,
			_events,
			viewModelKey.Value);

        await TextEditorService.Post(workKeyDown).ConfigureAwait(false);
	}

    private async Task ReceiveOnContextMenuAsync()
    {
        var localCursorDisplay = CursorDisplay;

        if (localCursorDisplay is not null)
            await localCursorDisplay.SetShouldDisplayMenuAsync(MenuKind.ContextMenu).ConfigureAwait(false);
    }

    private async Task ReceiveOnDoubleClick(MouseEventArgs mouseEventArgs)
    {
        var modelResourceUri = GetModel()?.ResourceUri;
        var viewModelKey = GetViewModel()?.ViewModelKey;

        if (modelResourceUri is null || viewModelKey is null)
            return;

        var workDoubleClick = new TextEditorWorkDoubleClick(
            modelResourceUri,
			Key<TextEditorCursor>.Empty,
			null,
			mouseEventArgs,
            _events,
            viewModelKey.Value);

        await TextEditorService.Post(workDoubleClick).ConfigureAwait(false);
    }

    private async Task ReceiveContentOnMouseDown(MouseEventArgs mouseEventArgs)
    {
        _events.ThinksLeftMouseButtonIsDown = true;


        var modelResourceUri = GetModel()?.ResourceUri;
        var viewModel = GetViewModel();

        var viewModelKey = GetViewModel()?.ViewModelKey;

        if (modelResourceUri is null || viewModelKey is null)
            return;
		
        if (viewModel is not null)
            viewModel.UnsafeState.ShouldRevealCursor = false;

		var workMouseDown = new TextEditorWorkMouseDown(
            modelResourceUri,
			Key<TextEditorCursor>.Empty,
			null,
			mouseEventArgs,
            _events,
            viewModelKey.Value);

        await TextEditorService.Post(workMouseDown).ConfigureAwait(false);
    }

    /// <summary>OnMouseUp is un-necessary</summary>
    private async Task ReceiveContentOnMouseMove(MouseEventArgs mouseEventArgs)
    {
        _userMouseIsInside = true;

        if ((mouseEventArgs.Buttons & 1) == 0)
            _events.ThinksLeftMouseButtonIsDown = false;

        var localThinksLeftMouseButtonIsDown = _events.ThinksLeftMouseButtonIsDown;

        // MouseStoppedMovingEvent
        {
            // Hide the tooltip, if the user moves their cursor out of the tooltips UI.
            if (_events.TooltipViewModel is not null && _events.MouseNoLongerOverTooltipTask.IsCompleted)
            {
                var mouseNoLongerOverTooltipCancellationToken = _events.MouseNoLongerOverTooltipCancellationTokenSource.Token;

                _events.MouseNoLongerOverTooltipTask = Task.Run(async () =>
                {
                    await Task.Delay(TextEditorEvents.OnMouseOutTooltipDelay, mouseNoLongerOverTooltipCancellationToken).ConfigureAwait(false);

                    if (!mouseNoLongerOverTooltipCancellationToken.IsCancellationRequested)
                    {
                        _events.TooltipViewModel = null;
                        await InvokeAsync(StateHasChanged);
                    }
                });
            }

            _events.MouseStoppedMovingCancellationTokenSource.Cancel();
            _events.MouseStoppedMovingCancellationTokenSource = new();

            var mouseStoppedMovingCancellationToken = _events.MouseStoppedMovingCancellationTokenSource.Token;

            _events.MouseStoppedMovingTask = Task.Run(async () =>
            {
                await Task.Delay(TextEditorEvents.MouseStoppedMovingDelay, mouseStoppedMovingCancellationToken).ConfigureAwait(false);

                if (!mouseStoppedMovingCancellationToken.IsCancellationRequested && _userMouseIsInside)
                {
                    await _events.ContinueRenderingTooltipAsync().ConfigureAwait(false);
                    await _events.HandleMouseStoppedMovingEventAsync(mouseEventArgs).ConfigureAwait(false);
                }
            });
        }

        if (!_events.ThinksLeftMouseButtonIsDown)
            return;

        var modelResourceUri = GetModel()?.ResourceUri;
        var viewModelKey = GetViewModel()?.ViewModelKey;

        if (modelResourceUri is null || viewModelKey is null)
            return;

        // Buttons is a bit flag '& 1' gets if left mouse button is held
        if (localThinksLeftMouseButtonIsDown && (mouseEventArgs.Buttons & 1) == 1)
        {
			var workMouseMove = new TextEditorWorkMouseMove(
                resourceUri,
				Key<TextEditorCursor>.Empty,
				null,
				mouseEventArgs,
                _events,
                viewModelKey.Value);

            await TextEditorService.Post(workMouseMove).ConfigureAwait(false);
        }
        else
        {
            _events.ThinksLeftMouseButtonIsDown = false;
        }
    }

    private void ReceiveContentOnMouseOut(MouseEventArgs mouseEventArgs)
    {
        _userMouseIsInside = false;
    }
    
    private async Task ReceiveOnWheel(WheelEventArgs wheelEventArgs)
    {
        var viewModelKey = GetViewModel()?.ViewModelKey;

        if (viewModelKey is null)
            return;

		var onWheel = new OnWheel(
            wheelEventArgs,
            _events,
            viewModelKey.Value);

        await TextEditorService.Post(onWheel).ConfigureAwait(false);
    }

    private Task ReceiveOnTouchStartAsync(TouchEventArgs touchEventArgs)
    {
        _touchStartDateTime = DateTime.UtcNow;

        _previousTouchEventArgs = touchEventArgs;
        _thinksTouchIsOccurring = true;

        return Task.CompletedTask;
    }

    private async Task ReceiveOnTouchMoveAsync(TouchEventArgs touchEventArgs)
    {
        var localThinksTouchIsOccurring = _thinksTouchIsOccurring;

        if (!_thinksTouchIsOccurring)
             return;

        var previousTouchPoint = _previousTouchEventArgs?.ChangedTouches.FirstOrDefault(x => x.Identifier == 0);
        var currentTouchPoint = touchEventArgs.ChangedTouches.FirstOrDefault(x => x.Identifier == 0);

        if (previousTouchPoint is null || currentTouchPoint is null)
             return;

        var viewModel = GetViewModel();

        if (viewModel is null)
			return;

        // Natural scrolling for touch devices
        var diffX = previousTouchPoint.ClientX - currentTouchPoint.ClientX;
        var diffY = previousTouchPoint.ClientY - currentTouchPoint.ClientY;

        await TextEditorService.Post(
            viewModel.ResourceUri,
			viewModel.ViewModelKey,
            async editContext =>
			{
                await editContext.TextEditorService.ViewModelApi
                    .MutateScrollHorizontalPositionFactory(viewModel.ViewModelKey, diffX)
                    .Invoke(editContext)
                    .ConfigureAwait(false);

                await editContext.TextEditorService.ViewModelApi
                    .MutateScrollVerticalPositionFactory(viewModel.ViewModelKey, diffY)
                    .Invoke(editContext)
                    .ConfigureAwait(false);
			}).ConfigureAwait(false);

        _previousTouchEventArgs = touchEventArgs;
    }

    private string GetGlobalHeightInPixelsStyling()
    {
        var heightInPixels = TextEditorService.OptionsStateWrap.Value.Options.TextEditorHeightInPixels;

        if (heightInPixels is null)
            return string.Empty;

        var heightInPixelsInvariantCulture = heightInPixels.Value.ToCssValue();

        return $"height: {heightInPixelsInvariantCulture}px;";
    }

    private async Task ClearTouch(TouchEventArgs touchEventArgs)
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

            await ReceiveContentOnMouseDown(new MouseEventArgs
            {
                Buttons = 1,
                ClientX = startTouchPoint.ClientX,
                ClientY = startTouchPoint.ClientY,
            });
        }
    }

    private async Task QueueRemeasureBackgroundTask(
        ITextEditorRenderBatch localRefCurrentRenderBatch,
        string localMeasureCharacterWidthAndRowHeightElementId,
        int countOfTestCharacters,
        CancellationToken cancellationToken)
    {
        var modelResourceUri = GetModel()?.ResourceUri;
        var viewModelKey = GetViewModel()?.ViewModelKey;

        if (modelResourceUri is null || viewModelKey is null)
            return;

        await TextEditorService.Post(
                modelResourceUri,
                viewModelKey.Value,
                TextEditorService.ViewModelApi.RemeasureFactory(
                    modelResourceUri,
                    viewModelKey.Value,
                    localMeasureCharacterWidthAndRowHeightElementId,
                    countOfTestCharacters,
                    CancellationToken.None))
            .ConfigureAwait(false);
    }

    private async Task QueueCalculateVirtualizationResultBackgroundTask(
		ITextEditorRenderBatch localCurrentRenderBatch)
    {
        var modelResourceUri = GetModel()?.ResourceUri;
        var viewModelKey = GetViewModel()?.ViewModelKey;

        if (modelResourceUri is null || viewModelKey is null)
            return;

        await TextEditorService.Post(
                modelResourceUri,
				viewModelKey.Value,
                TextEditorService.ViewModelApi.CalculateVirtualizationResultFactory(
                    modelResourceUri,
                    viewModelKey.Value,
                    CancellationToken.None))
            .ConfigureAwait(false);
    }

    public void Dispose()
    {
        TextEditorViewModelsStateWrap.StateChanged -= GeneralOnStateChangedEventHandler;
        TextEditorOptionsStateWrap.StateChanged -= GeneralOnStateChangedEventHandler;

        lock (_linkedViewModelLock)
        {
            if (_linkedViewModel is not null)
            {
                _linkedViewModel.DisplayTracker.DecrementLinks(TextEditorModelStateWrap);
                _linkedViewModel = null;
            }
        }

        _events.MouseStoppedMovingCancellationTokenSource.Cancel();
    }

	internal Task Hack_ReRender()
	{
		return InvokeAsync(StateHasChanged);
	}
}