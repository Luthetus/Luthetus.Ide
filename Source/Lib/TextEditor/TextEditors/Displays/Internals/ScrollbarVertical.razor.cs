using Fluxor;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Drags.Displays;
using Luthetus.Common.RazorLib.JavaScriptObjects.Models;
using Luthetus.TextEditor.RazorLib.Events.Models;
using Luthetus.TextEditor.RazorLib.JsRuntimes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

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

    private bool _thinksLeftMouseButtonIsDown;
    private RelativeCoordinates _relativeCoordinatesOnMouseDown = new(0, 0, 0, 0);
    private Func<(MouseEventArgs firstMouseEventArgs, MouseEventArgs secondMouseEventArgs), Task>? _dragEventHandler;
    private MouseEventArgs? _previousDragMouseEventArgs;

    private string ScrollbarElementId => $"luth_te_{_scrollbarGuid}";
    private string ScrollbarSliderElementId => $"luth_te_{_scrollbarGuid}-slider";

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

		var textEditorDimensions = RenderBatch.ViewModel.TextEditorDimensions;
		var scrollbarDimensions = RenderBatch.ViewModel.ScrollbarDimensions;
	
		_relativeCoordinatesOnMouseDown = new RelativeCoordinates(
		    mouseEventArgs.ClientX - textEditorDimensions.BoundingClientRectLeft,
		    mouseEventArgs.ClientY - textEditorDimensions.BoundingClientRectTop,
		    scrollbarDimensions.ScrollLeft,
		    scrollbarDimensions.ScrollTop);

        SubscribeToDragEventForScrolling();
    }

    private async void DragStateWrapOnStateChanged(object? sender, EventArgs e)
    {
        if (!DragStateWrap.Value.ShouldDisplay)
        {
            _dragEventHandler = null;
            _previousDragMouseEventArgs = null;
        }
        else
        {
            var mouseEventArgs = DragStateWrap.Value.MouseEventArgs;

            if (_dragEventHandler is not null)
            {
                if (_previousDragMouseEventArgs is not null && mouseEventArgs is not null)
                {
                    await _dragEventHandler
                        .Invoke((_previousDragMouseEventArgs, mouseEventArgs))
                        .ConfigureAwait(false);
                }

                _previousDragMouseEventArgs = mouseEventArgs;
                await InvokeAsync(StateHasChanged);
            }
        }
    }

    public void SubscribeToDragEventForScrolling()
    {
        _dragEventHandler = DragEventHandlerScrollAsync;

        Dispatcher.Dispatch(new DragState.WithAction(inState => inState with
        {
            ShouldDisplay = true,
            MouseEventArgs = null,
        }));
    }

    private async Task DragEventHandlerScrollAsync(
        (MouseEventArgs firstMouseEventArgs, MouseEventArgs secondMouseEventArgs) mouseEventArgsTuple)
    {
        var localThinksLeftMouseButtonIsDown = _thinksLeftMouseButtonIsDown;

        if (!localThinksLeftMouseButtonIsDown)
            return;

        // Buttons is a bit flag '& 1' gets if left mouse button is held
        if (localThinksLeftMouseButtonIsDown && (mouseEventArgsTuple.secondMouseEventArgs.Buttons & 1) == 1)
        {
			var textEditorDimensions = RenderBatch.ViewModel.TextEditorDimensions;
			var scrollbarDimensions = RenderBatch.ViewModel.ScrollbarDimensions;
		
			var relativeCoordinatesOfDragEvent = new RelativeCoordinates(
			    mouseEventArgsTuple.secondMouseEventArgs.ClientX - textEditorDimensions.BoundingClientRectLeft,
			    mouseEventArgsTuple.secondMouseEventArgs.ClientY - textEditorDimensions.BoundingClientRectTop,
			    scrollbarDimensions.ScrollLeft,
			    scrollbarDimensions.ScrollTop);

            var yPosition = relativeCoordinatesOfDragEvent.RelativeY - _relativeCoordinatesOnMouseDown.RelativeY;
            yPosition = Math.Max(0, yPosition);

            if (yPosition > textEditorDimensions.Height)
                yPosition = textEditorDimensions.Height;

            var scrollbarHeightInPixels = textEditorDimensions.Height - ScrollbarFacts.SCROLLBAR_SIZE_IN_PIXELS;

            var scrollTop = yPosition *
                scrollbarDimensions.ScrollHeight /
                scrollbarHeightInPixels;

            if (scrollTop + textEditorDimensions.Height > scrollbarDimensions.ScrollHeight)
                scrollTop = scrollbarDimensions.ScrollHeight - textEditorDimensions.Height;

			var throttleEventOnScrollVertical = new OnScrollVertical(
				scrollTop,
				RenderBatch.ComponentData,
				RenderBatch.ViewModel.ViewModelKey);

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