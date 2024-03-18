using Fluxor;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Drags.Displays;
using Luthetus.Common.RazorLib.JavaScriptObjects.Models;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals.UiEvent;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

public partial class ScrollbarHorizontal : ComponentBase, IDisposable
{
    [Inject]
    private IState<DragState> DragStateWrap { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;

    [CascadingParameter]
    public TextEditorRenderBatch RenderBatch { get; set; } = null!;

    private readonly Guid _scrollbarGuid = Guid.NewGuid();
    private readonly IThrottle _throttleScroll = new Throttle(TimeSpan.FromMilliseconds(100));

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

    private string GetScrollbarHorizontalStyleCss()
    {
        var scrollbarWidthInPixels = RenderBatch.ViewModel!.VirtualizationResult.TextEditorMeasurements.Width -
            ScrollbarFacts.SCROLLBAR_SIZE_IN_PIXELS;

        var scrollbarWidthInPixelsInvariantCulture = scrollbarWidthInPixels.ToCssValue();
        var width = $"width: {scrollbarWidthInPixelsInvariantCulture}px;";

        return width;
    }

    private string GetSliderHorizontalStyleCss()
    {
        var scrollbarWidthInPixels = RenderBatch.ViewModel!.VirtualizationResult.TextEditorMeasurements.Width -
            ScrollbarFacts.SCROLLBAR_SIZE_IN_PIXELS;

        // Proportional Left
        var sliderProportionalLeftInPixels = RenderBatch.ViewModel!.VirtualizationResult.TextEditorMeasurements.ScrollLeft *
            scrollbarWidthInPixels /
            RenderBatch.ViewModel!.VirtualizationResult.TextEditorMeasurements.ScrollWidth;

        var sliderProportionalLeftInPixelsInvariantCulture = sliderProportionalLeftInPixels.ToCssValue();
        var left = $"left: {sliderProportionalLeftInPixelsInvariantCulture}px;";

        // Proportional Width
        var pageWidth = RenderBatch.ViewModel!.VirtualizationResult.TextEditorMeasurements.Width;

        var sliderProportionalWidthInPixels = pageWidth *
            scrollbarWidthInPixels /
            RenderBatch.ViewModel!.VirtualizationResult.TextEditorMeasurements.ScrollWidth;

        var sliderProportionalWidthInPixelsInvariantCulture = sliderProportionalWidthInPixels.ToCssValue();
        var width = $"width: {sliderProportionalWidthInPixelsInvariantCulture}px;";

        return $"{left} {width}";
    }

    private async Task HandleOnMouseDownAsync(MouseEventArgs mouseEventArgs)
    {
        _thinksLeftMouseButtonIsDown = true;

        _relativeCoordinatesOnMouseDown = await JsRuntime.InvokeAsync<RelativeCoordinates>(
                "luthetusTextEditor.getRelativePosition",
                ScrollbarSliderElementId,
                mouseEventArgs.ClientX,
                mouseEventArgs.ClientY);

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
                    await _dragEventHandler.Invoke((_previousDragMouseEventArgs, mouseEventArgs));
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
            var relativeCoordinatesOfDragEvent = await JsRuntime.InvokeAsync<RelativeCoordinates>(
                "luthetusTextEditor.getRelativePosition",
                ScrollbarElementId,
                mouseEventArgsTuple.secondMouseEventArgs.ClientX,
                mouseEventArgsTuple.secondMouseEventArgs.ClientY);

            var xPosition = relativeCoordinatesOfDragEvent.RelativeX - _relativeCoordinatesOnMouseDown.RelativeX;

            xPosition = Math.Max(0, xPosition);

            if (xPosition > RenderBatch.ViewModel!.VirtualizationResult.TextEditorMeasurements.Height)
                xPosition = RenderBatch.ViewModel!.VirtualizationResult.TextEditorMeasurements.Height;

            var scrollbarWidthInPixels = RenderBatch.ViewModel!.VirtualizationResult.TextEditorMeasurements.Width -
                ScrollbarFacts.SCROLLBAR_SIZE_IN_PIXELS;

            var scrollLeft = xPosition *
                RenderBatch.ViewModel!.VirtualizationResult.TextEditorMeasurements.ScrollWidth /
                scrollbarWidthInPixels;

            if (scrollLeft + RenderBatch.ViewModel!.VirtualizationResult.TextEditorMeasurements.Width >
                RenderBatch.ViewModel!.VirtualizationResult.TextEditorMeasurements.ScrollWidth)
            {
                scrollLeft = RenderBatch.ViewModel!.VirtualizationResult.TextEditorMeasurements.ScrollWidth -
                    RenderBatch.ViewModel!.VirtualizationResult.TextEditorMeasurements.Width;
            }

			var throttleEventOnScrollHorizontal = new ThrottleEventOnScrollHorizontal(
				scrollLeft,
				RenderBatch.Events,
				RenderBatch.ViewModel!.ViewModelKey);

			TextEditorService.Post(throttleEventOnScrollHorizontal);
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