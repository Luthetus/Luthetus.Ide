using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Fluxor;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Drags.Displays;
using Luthetus.Common.RazorLib.JavaScriptObjects.Models;
using Luthetus.Common.RazorLib.JsRuntimes.Models;
using Luthetus.TextEditor.RazorLib.Events.Models;
using Luthetus.TextEditor.RazorLib.JsRuntimes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

public partial class ScrollbarVertical : ComponentBase, IDisposable
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
	private double _clientXThresholdToResetScrollTopPosition;
	private double _scrollTopOnMouseDown;

    private string ScrollbarElementId => $"luth_te_{_scrollbarGuid}";
    private string ScrollbarSliderElementId => $"luth_te_{_scrollbarGuid}-slider";
	private LuthetusCommonJavaScriptInteropApi CommonJavaScriptInteropApi => _commonJavaScriptInteropApi ??= JsRuntime.GetLuthetusCommonApi();

    protected override void OnInitialized()
    {
        DragStateWrap.StateChanged += DragStateWrapOnStateChanged;
        base.OnInitialized();
    }

    private string GetSliderVerticalStyleCss()
    {
        var textEditorDimensions = RenderBatch.ViewModel.TextEditorDimensions;
        var scrollBarDimensions = RenderBatch.ViewModel.ScrollbarDimensions;

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

    private async Task HandleOnMouseDownAsync(MouseEventArgs mouseEventArgs)
    {
        _thinksLeftMouseButtonIsDown = true;
		_scrollTopOnMouseDown = RenderBatch.ViewModel.ScrollbarDimensions.ScrollTop;

		var scrollbarBoundingClientRect = await CommonJavaScriptInteropApi
			.MeasureElementById(ScrollbarElementId)
			.ConfigureAwait(false);

		// Drag far left to reset scroll to original
		var textEditorDimensions = RenderBatch.ViewModel.TextEditorDimensions;
		var distanceBetweenLeftEditorAndLeftScrollbar = scrollbarBoundingClientRect.LeftInPixels - textEditorDimensions.BoundingClientRectLeft;
		_clientXThresholdToResetScrollTopPosition = scrollbarBoundingClientRect.LeftInPixels - (distanceBetweenLeftEditorAndLeftScrollbar / 2);

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

    private async Task DragEventHandlerScrollAsync(MouseEventArgs localMouseDownEventArgs, MouseEventArgs onDragMouseEventArgs)
    {
        var localThinksLeftMouseButtonIsDown = _thinksLeftMouseButtonIsDown;

        if (!localThinksLeftMouseButtonIsDown)
            return;

        // Buttons is a bit flag '& 1' gets if left mouse button is held
        if (localThinksLeftMouseButtonIsDown && (onDragMouseEventArgs.Buttons & 1) == 1)
        {
			var textEditorDimensions = RenderBatch.ViewModel.TextEditorDimensions;
			var scrollbarDimensions = RenderBatch.ViewModel.ScrollbarDimensions;

			OnScrollVertical throttleEventOnScrollVertical;

			if (onDragMouseEventArgs.ClientX < _clientXThresholdToResetScrollTopPosition)
			{
				// Drag far left to reset scroll to original
				throttleEventOnScrollVertical = new OnScrollVertical(
					_scrollTopOnMouseDown,
					RenderBatch.ComponentData,
					RenderBatch.ViewModel.ViewModelKey);
			}
			else
			{
	    		var diffY = onDragMouseEventArgs.ClientY - localMouseDownEventArgs.ClientY;
	
	            var scrollbarHeightInPixels = textEditorDimensions.Height - ScrollbarFacts.SCROLLBAR_SIZE_IN_PIXELS;
	
	            var scrollTop = _scrollTopOnMouseDown +
					diffY *
	                scrollbarDimensions.ScrollHeight /
	                scrollbarHeightInPixels;
	
	            if (scrollTop + textEditorDimensions.Height > scrollbarDimensions.ScrollHeight)
	                scrollTop = scrollbarDimensions.ScrollHeight - textEditorDimensions.Height;

				if (scrollTop < 0)
					scrollTop = 0;
	
				throttleEventOnScrollVertical = new OnScrollVertical(
					scrollTop,
					RenderBatch.ComponentData,
					RenderBatch.ViewModel.ViewModelKey);
			}

			await TextEditorService.Post(throttleEventOnScrollVertical);
        }
        else
        {
            _thinksLeftMouseButtonIsDown = false;
        }
    }

    public void Dispose()
    {
        DragStateWrap.StateChanged -= DragStateWrapOnStateChanged;
    }
}