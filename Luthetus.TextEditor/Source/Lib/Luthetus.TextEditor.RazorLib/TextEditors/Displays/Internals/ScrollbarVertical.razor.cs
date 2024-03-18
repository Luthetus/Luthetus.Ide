using Fluxor;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Drags.Displays;
using Luthetus.Common.RazorLib.JavaScriptObjects.Models;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals.UiEvent;
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

    private string GetSliderVerticalStyleCss()
    {
        var elementMeasurements = RenderBatch.ViewModel!.VirtualizationResult.TextEditorMeasurements;

        var scrollbarHeightInPixels = elementMeasurements.Height - ScrollbarFacts.SCROLLBAR_SIZE_IN_PIXELS;

        // Proportional Top
        var sliderProportionalTopInPixels = elementMeasurements.ScrollTop *
            scrollbarHeightInPixels /
            elementMeasurements.ScrollHeight;

        var sliderProportionalTopInPixelsInvariantCulture = sliderProportionalTopInPixels.ToCssValue();

        var top = $"top: {sliderProportionalTopInPixelsInvariantCulture}px;";

        // Proportional Height
        var pageHeight = elementMeasurements.Height;

        var sliderProportionalHeightInPixels = pageHeight *
            scrollbarHeightInPixels /
            elementMeasurements.ScrollHeight;

        var sliderProportionalHeightInPixelsInvariantCulture = sliderProportionalHeightInPixels.ToCssValue();

        var height = $"height: {sliderProportionalHeightInPixelsInvariantCulture}px;";

        return $"{top} {height}";
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

            var yPosition = relativeCoordinatesOfDragEvent.RelativeY - _relativeCoordinatesOnMouseDown.RelativeY;
            yPosition = Math.Max(0, yPosition);

            var elementMeasurements = RenderBatch.ViewModel!.VirtualizationResult.TextEditorMeasurements;

            if (yPosition > elementMeasurements.Height)
                yPosition = elementMeasurements.Height;

            var scrollbarHeightInPixels = elementMeasurements.Height - ScrollbarFacts.SCROLLBAR_SIZE_IN_PIXELS;

            var scrollTop = yPosition *
                elementMeasurements.ScrollHeight /
                scrollbarHeightInPixels;

            if (scrollTop + elementMeasurements.Height > elementMeasurements.ScrollHeight)
                scrollTop = elementMeasurements.ScrollHeight - elementMeasurements.Height;

			var throttleEventOnScrollVertical = new ThrottleEventOnScrollVertical(
				scrollTop,
				RenderBatch.Events,
				RenderBatch.ViewModel!.ViewModelKey);

			TextEditorService.Post(throttleEventOnScrollVertical);
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