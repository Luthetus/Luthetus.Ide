using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Fluxor;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Drags.Displays;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.Events.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

public partial class ScrollbarSection : ComponentBase, IDisposable
{
	[Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;
    [Inject]
    private IState<DragState> DragStateWrap { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Parameter, EditorRequired]
    public TextEditorRenderBatchValidated? RenderBatch { get; set; }

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

    private string VERTICAL_ScrollbarElementId => $"luth_te_{VERTICAL_scrollbarGuid}";
    private string VERTICAL_ScrollbarSliderElementId => $"luth_te_{VERTICAL_scrollbarGuid}-slider";

    private bool HORIZONTAL_thinksLeftMouseButtonIsDown;
	private double HORIZONTAL_clientYThresholdToResetScrollLeftPosition;
	private double HORIZONTAL_scrollLeftOnMouseDown;

    private string HORIZONTAL_ScrollbarElementId => $"luth_te_{HORIZONTAL_scrollbarGuid}";
    private string HORIZONTAL_ScrollbarSliderElementId => $"luth_te_{HORIZONTAL_scrollbarGuid}-slider";
	
	private Func<MouseEventArgs, MouseEventArgs, Task>? _dragEventHandler = null;

    protected override void OnInitialized()
    {
    	DragStateWrap.StateChanged += DragStateWrapOnStateChanged;
        base.OnInitialized();
    }

    private string HORIZONTAL_GetScrollbarHorizontalStyleCss()
    {
    	var renderBatchLocal = RenderBatch;
    	if (renderBatchLocal is null)
    		return string.Empty;
    	
        var scrollbarWidthInPixels = renderBatchLocal.ViewModel.TextEditorDimensions.Width -
            ScrollbarFacts.SCROLLBAR_SIZE_IN_PIXELS;

        var scrollbarWidthInPixelsInvariantCulture = scrollbarWidthInPixels.ToCssValue();
        var width = $"width: {scrollbarWidthInPixelsInvariantCulture}px;";

        return width;
    }

    private string HORIZONTAL_GetSliderHorizontalStyleCss()
    {
    	var renderBatchLocal = RenderBatch;
    	if (renderBatchLocal is null)
    		return string.Empty;
    	
        var scrollbarWidthInPixels = renderBatchLocal.ViewModel.TextEditorDimensions.Width -
            ScrollbarFacts.SCROLLBAR_SIZE_IN_PIXELS;

        // Proportional Left
        var sliderProportionalLeftInPixels = renderBatchLocal.ViewModel.ScrollbarDimensions.ScrollLeft *
            scrollbarWidthInPixels /
            renderBatchLocal.ViewModel.ScrollbarDimensions.ScrollWidth;

        var sliderProportionalLeftInPixelsInvariantCulture = sliderProportionalLeftInPixels.ToCssValue();
        var left = $"left: {sliderProportionalLeftInPixelsInvariantCulture}px;";

        // Proportional Width
        var pageWidth = renderBatchLocal.ViewModel.TextEditorDimensions.Width;

        var sliderProportionalWidthInPixels = pageWidth *
            scrollbarWidthInPixels /
            renderBatchLocal.ViewModel.ScrollbarDimensions.ScrollWidth;

        var sliderProportionalWidthInPixelsInvariantCulture = sliderProportionalWidthInPixels.ToCssValue();
        var width = $"width: {sliderProportionalWidthInPixelsInvariantCulture}px;";

        return $"{left} {width}";
    }
    
    private string VERTICAL_GetSliderVerticalStyleCss()
    {
    	var renderBatchLocal = RenderBatch;
    	if (renderBatchLocal is null)
    		return string.Empty;
    	
        var textEditorDimensions = renderBatchLocal.ViewModel.TextEditorDimensions;
        var scrollBarDimensions = renderBatchLocal.ViewModel.ScrollbarDimensions;

        var scrollbarHeightInPixels = textEditorDimensions.Height - ScrollbarFacts.SCROLLBAR_SIZE_IN_PIXELS;

        // Proportional Top
        var sliderProportionalTopInPixels = scrollBarDimensions.ScrollTop *
            scrollbarHeightInPixels /
            scrollBarDimensions.ScrollHeight;

        var sliderProportionalTopInPixelsInvariantCulture = sliderProportionalTopInPixels.ToCssValue();

        var top = $"top: {sliderProportionalTopInPixelsInvariantCulture}px;";

        // Proportional Height
        var pageHeight = textEditorDimensions.Height;

        var sliderProportionalHeightInPixels = pageHeight *
            scrollbarHeightInPixels /
            scrollBarDimensions.ScrollHeight;

        var sliderProportionalHeightInPixelsInvariantCulture = sliderProportionalHeightInPixels.ToCssValue();

        var height = $"height: {sliderProportionalHeightInPixelsInvariantCulture}px;";

        return $"{top} {height}";
    }

    private async Task HORIZONTAL_HandleOnMouseDownAsync(MouseEventArgs mouseEventArgs)
    {
    	var renderBatchLocal = RenderBatch;
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
	
			Dispatcher.Dispatch(new DragState.ShouldDisplayAndMouseEventArgsSetAction(true, null));
		}
    }
    
    private async Task VERTICAL_HandleOnMouseDownAsync(MouseEventArgs mouseEventArgs)
    {
    	var renderBatchLocal = RenderBatch;
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
	
			Dispatcher.Dispatch(new DragState.ShouldDisplayAndMouseEventArgsSetAction(true, null));
		}     
    }

    private async void DragStateWrapOnStateChanged(object? sender, EventArgs e)
    {
        if (!DragStateWrap.Value.ShouldDisplay)
        {
            // NOTE: '_mouseDownEventArgs' being non-null is what indicates that the subscription is active.
			//       So be wary if one intends to move its assignment elsewhere.
            _mouseDownEventArgs = null;
        }
        else
        {
            var localMouseDownEventArgs = _mouseDownEventArgs;
            var dragEventArgs = DragStateWrap.Value.MouseEventArgs;
			var localDragEventHandler = _dragEventHandler;

            if (localMouseDownEventArgs is not null && dragEventArgs is not null)
                await localDragEventHandler.Invoke(localMouseDownEventArgs, dragEventArgs).ConfigureAwait(false);
        }
    }

    private Task HORIZONTAL_DragEventHandlerScrollAsync(MouseEventArgs localMouseDownEventArgs, MouseEventArgs onDragMouseEventArgs)
    {
    	var renderBatchLocal = RenderBatch;
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

			TextEditorService.Post(onScrollHorizontal);
        }
        else
        {
            HORIZONTAL_thinksLeftMouseButtonIsDown = false;
        }

        return Task.CompletedTask;
    }
    
    private Task VERTICAL_DragEventHandlerScrollAsync(MouseEventArgs localMouseDownEventArgs, MouseEventArgs onDragMouseEventArgs)
    {
    	var renderBatchLocal = RenderBatch;
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

			TextEditorService.Post(onScrollVertical);
        }
        else
        {
            VERTICAL_thinksLeftMouseButtonIsDown = false;
        }

        return Task.CompletedTask;
    }

    public void Dispose()
    {
    	DragStateWrap.StateChanged -= DragStateWrapOnStateChanged;
    }
}