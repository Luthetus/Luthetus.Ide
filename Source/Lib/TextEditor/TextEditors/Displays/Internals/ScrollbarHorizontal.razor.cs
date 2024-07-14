using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Fluxor;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Drags.Displays;
using Luthetus.Common.RazorLib.JsRuntimes.Models;
using Luthetus.TextEditor.RazorLib.Events.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

public partial class ScrollbarHorizontal : ComponentBase, IDisposable
{
	[Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;
    [Inject]
    private IState<DragState> DragStateWrap { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    
    [CascadingParameter]
    public TextEditorRenderBatchValidated RenderBatch { get; set; } = null!;

    private readonly Guid _scrollbarGuid = Guid.NewGuid();

	private LuthetusCommonJavaScriptInteropApi? _commonJavaScriptInteropApi;
    private bool _thinksLeftMouseButtonIsDown;
    private MouseEventArgs? _mouseDownEventArgs;
	private double _clientYThresholdToResetScrollLeftPosition;
	private double _scrollLeftOnMouseDown;

    private string ScrollbarElementId => $"luth_te_{_scrollbarGuid}";
    private string ScrollbarSliderElementId => $"luth_te_{_scrollbarGuid}-slider";
	private LuthetusCommonJavaScriptInteropApi CommonJavaScriptInteropApi => _commonJavaScriptInteropApi ??= JsRuntime.GetLuthetusCommonApi();

    protected override void OnInitialized()
    {
        DragStateWrap.StateChanged += DragStateWrapOnStateChanged;
        base.OnInitialized();
    }

    private string GetScrollbarHorizontalStyleCss()
    {
        var scrollbarWidthInPixels = RenderBatch.ViewModel.TextEditorDimensions.Width -
            ScrollbarFacts.SCROLLBAR_SIZE_IN_PIXELS;

        var scrollbarWidthInPixelsInvariantCulture = scrollbarWidthInPixels.ToCssValue();
        var width = $"width: {scrollbarWidthInPixelsInvariantCulture}px;";

        return width;
    }

    private string GetSliderHorizontalStyleCss()
    {
        var scrollbarWidthInPixels = RenderBatch.ViewModel.TextEditorDimensions.Width -
            ScrollbarFacts.SCROLLBAR_SIZE_IN_PIXELS;

        // Proportional Left
        var sliderProportionalLeftInPixels = RenderBatch.ViewModel.ScrollbarDimensions.ScrollLeft *
            scrollbarWidthInPixels /
            RenderBatch.ViewModel.ScrollbarDimensions.ScrollWidth;

        var sliderProportionalLeftInPixelsInvariantCulture = sliderProportionalLeftInPixels.ToCssValue();
        var left = $"left: {sliderProportionalLeftInPixelsInvariantCulture}px;";

        // Proportional Width
        var pageWidth = RenderBatch.ViewModel.TextEditorDimensions.Width;

        var sliderProportionalWidthInPixels = pageWidth *
            scrollbarWidthInPixels /
            RenderBatch.ViewModel.ScrollbarDimensions.ScrollWidth;

        var sliderProportionalWidthInPixelsInvariantCulture = sliderProportionalWidthInPixels.ToCssValue();
        var width = $"width: {sliderProportionalWidthInPixelsInvariantCulture}px;";

        return $"{left} {width}";
    }

    private async Task HandleOnMouseDownAsync(MouseEventArgs mouseEventArgs)
    {
        _thinksLeftMouseButtonIsDown = true;
		_scrollLeftOnMouseDown = RenderBatch.ViewModel.ScrollbarDimensions.ScrollLeft;

		var scrollbarBoundingClientRect = await CommonJavaScriptInteropApi
			.MeasureElementById(ScrollbarElementId)
			.ConfigureAwait(false);

		// Drag far up to reset scroll to original
		var textEditorDimensions = RenderBatch.ViewModel.TextEditorDimensions;
		var distanceBetweenTopEditorAndTopScrollbar = scrollbarBoundingClientRect.TopInPixels - textEditorDimensions.BoundingClientRectTop;
		_clientYThresholdToResetScrollLeftPosition = scrollbarBoundingClientRect.TopInPixels - (distanceBetweenTopEditorAndTopScrollbar / 2);

		// Subscribe to the drag events
		//
		// NOTE: '_mouseDownEventArgs' being non-null is what indicates that the subscription is active.
		//       So be wary if one intends to move its assignment elsewhere.
		{
			_mouseDownEventArgs = mouseEventArgs;
	
	        Dispatcher.Dispatch(new DragState.WithAction(inState => inState with
	        {
	            ShouldDisplay = true,
	            MouseEventArgs = null,
	        }));
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

            if (localMouseDownEventArgs is not null && dragEventArgs is not null)
                await DragEventHandlerScrollAsync(localMouseDownEventArgs, dragEventArgs).ConfigureAwait(false);
        }
    }

    private Task DragEventHandlerScrollAsync(MouseEventArgs localMouseDownEventArgs, MouseEventArgs onDragMouseEventArgs)
    {
        var localThinksLeftMouseButtonIsDown = _thinksLeftMouseButtonIsDown;

        if (!localThinksLeftMouseButtonIsDown)
            return Task.CompletedTask;

        // Buttons is a bit flag '& 1' gets if left mouse button is held
        if (localThinksLeftMouseButtonIsDown && (onDragMouseEventArgs.Buttons & 1) == 1)
        {
			var textEditorDimensions = RenderBatch.ViewModel.TextEditorDimensions;
			var scrollbarDimensions = RenderBatch.ViewModel.ScrollbarDimensions;
		
			OnScrollHorizontal onScrollHorizontal;

			if (onDragMouseEventArgs.ClientY < _clientYThresholdToResetScrollLeftPosition)
			{
				// Drag far left to reset scroll to original
				onScrollHorizontal = new OnScrollHorizontal(
					_scrollLeftOnMouseDown,
					RenderBatch.ComponentData,
					RenderBatch.ViewModel.ViewModelKey);
			}
			else
			{
				var diffX = onDragMouseEventArgs.ClientX - localMouseDownEventArgs.ClientX;
	
	            var scrollbarWidthInPixels = textEditorDimensions.Width - ScrollbarFacts.SCROLLBAR_SIZE_IN_PIXELS;
	
	            var scrollLeft = _scrollLeftOnMouseDown +
					diffX *
	                scrollbarDimensions.ScrollWidth /
	                scrollbarWidthInPixels;
	
	            if (scrollLeft + textEditorDimensions.Width > scrollbarDimensions.ScrollWidth)
	                scrollLeft = scrollbarDimensions.ScrollWidth - textEditorDimensions.Width;

				if (scrollLeft < 0)
					scrollLeft = 0;
	
				onScrollHorizontal = new OnScrollHorizontal(
					scrollLeft,
					RenderBatch.ComponentData,
					RenderBatch.ViewModel.ViewModelKey);
			}

			TextEditorService.Post(onScrollHorizontal);
        }
        else
        {
            _thinksLeftMouseButtonIsDown = false;
        }

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        DragStateWrap.StateChanged -= DragStateWrapOnStateChanged;
    }
}