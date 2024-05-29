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
        var sliderProportionalLeftInPixels = RenderBatch.ViewModel.TextEditorDimensions.ScrollLeft *
            scrollbarWidthInPixels /
            RenderBatch.ViewModel.TextEditorDimensions.ScrollWidth;

        var sliderProportionalLeftInPixelsInvariantCulture = sliderProportionalLeftInPixels.ToCssValue();
        var left = $"left: {sliderProportionalLeftInPixelsInvariantCulture}px;";

        // Proportional Width
        var pageWidth = RenderBatch.ViewModel.TextEditorDimensions.Width;

        var sliderProportionalWidthInPixels = pageWidth *
            scrollbarWidthInPixels /
            RenderBatch.ViewModel.TextEditorDimensions.ScrollWidth;

        var sliderProportionalWidthInPixelsInvariantCulture = sliderProportionalWidthInPixels.ToCssValue();
        var width = $"width: {sliderProportionalWidthInPixelsInvariantCulture}px;";

        return $"{left} {width}";
    }

    private async Task HandleOnMouseDownAsync(MouseEventArgs mouseEventArgs)
    {
        _thinksLeftMouseButtonIsDown = true;

        _relativeCoordinatesOnMouseDown = await TextEditorService.JsRuntimeTextEditorApi
            .GetRelativePosition(
                ScrollbarSliderElementId,
                mouseEventArgs.ClientX,
                mouseEventArgs.ClientY)
            .ConfigureAwait(false);

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
            var relativeCoordinatesOfDragEvent = await TextEditorService.JsRuntimeTextEditorApi
                .GetRelativePosition(
                    ScrollbarElementId,
                    mouseEventArgsTuple.secondMouseEventArgs.ClientX,
                    mouseEventArgsTuple.secondMouseEventArgs.ClientY)
                .ConfigureAwait(false);

            var xPosition = relativeCoordinatesOfDragEvent.RelativeX - _relativeCoordinatesOnMouseDown.RelativeX;

            xPosition = Math.Max(0, xPosition);

            if (xPosition > RenderBatch.ViewModel.TextEditorDimensions.Height)
                xPosition = RenderBatch.ViewModel.TextEditorDimensions.Height;

            var scrollbarWidthInPixels = RenderBatch.ViewModel.TextEditorDimensions.Width -
                ScrollbarFacts.SCROLLBAR_SIZE_IN_PIXELS;

            var scrollLeft = xPosition *
                RenderBatch.ViewModel.TextEditorDimensions.ScrollWidth /
                scrollbarWidthInPixels;

            if (scrollLeft + RenderBatch.ViewModel.TextEditorDimensions.Width >
                RenderBatch.ViewModel.TextEditorDimensions.ScrollWidth)
            {
                scrollLeft = RenderBatch.ViewModel.TextEditorDimensions.ScrollWidth -
                    RenderBatch.ViewModel.TextEditorDimensions.Width;
            }

			var throttleEventOnScrollHorizontal = new OnScrollHorizontal(
				scrollLeft,
				RenderBatch.Events,
				RenderBatch.ViewModel.ViewModelKey);

            await TextEditorService.Post(throttleEventOnScrollHorizontal);
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