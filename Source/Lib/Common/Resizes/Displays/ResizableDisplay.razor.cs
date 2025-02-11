using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Luthetus.Common.RazorLib.Drags.Displays;
using Luthetus.Common.RazorLib.Drags.Models;
using Luthetus.Common.RazorLib.Resizes.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;

namespace Luthetus.Common.RazorLib.Resizes.Displays;

public partial class ResizableDisplay : ComponentBase, IDisposable
{
    [Inject]
    private IDragService DragService { get; set; } = null!;
    [Inject]
    private IAppDimensionService AppDimensionService { get; set; } = null!;

    [Parameter, EditorRequired]
    public ElementDimensions ElementDimensions { get; set; }
    [Parameter, EditorRequired]
    public Func<Task> ReRenderFuncAsync { get; set; } = null!;

	[Parameter]
    public IDrag? Drag { get; set; } = null!;

    public const double RESIZE_HANDLE_SQUARE_PIXELS = 10;

    private Func<(MouseEventArgs firstMouseEventArgs, MouseEventArgs secondMouseEventArgs), Task>? _dragEventHandler;
    private MouseEventArgs? _previousDragMouseEventArgs;

    private ElementDimensions _northResizeHandleDimensions = new();
    private ElementDimensions _eastResizeHandleDimensions = new();
    private ElementDimensions _southResizeHandleDimensions = new();
    private ElementDimensions _westResizeHandleDimensions = new();
    private ElementDimensions _northEastResizeHandleDimensions = new();
    private ElementDimensions _southEastResizeHandleDimensions = new();
    private ElementDimensions _southWestResizeHandleDimensions = new();
    private ElementDimensions _northWestResizeHandleDimensions = new();

    protected override void OnInitialized()
    {
        DragService.DragStateChanged += DragStateWrapOnStateChanged;

        base.OnInitialized();
    }

    private async void DragStateWrapOnStateChanged()
    {
        if (!DragService.GetDragState().ShouldDisplay)
        {
			var wasTargetOfDragging = _dragEventHandler is not null;

            _dragEventHandler = null;
            _previousDragMouseEventArgs = null;

			if (wasTargetOfDragging)
				AppDimensionService.ReduceNotifyIntraAppResizeAction();
        }
        else
        {
            var mouseEventArgs = DragService.GetDragState().MouseEventArgs;

            if (_dragEventHandler is not null)
            {
                if (_previousDragMouseEventArgs is not null && mouseEventArgs is not null)
                {
                    await _dragEventHandler
                        .Invoke((_previousDragMouseEventArgs, mouseEventArgs))
                        .ConfigureAwait(false);
                }

                _previousDragMouseEventArgs = mouseEventArgs;
                await ReRenderFuncAsync.Invoke().ConfigureAwait(false);
            }
        }
    }

    private async Task SubscribeToDragEventAsync(
        Func<(MouseEventArgs firstMouseEventArgs, MouseEventArgs secondMouseEventArgs), Task> dragEventHandler)
    {
        _dragEventHandler = dragEventHandler;

        if (Drag is not null)
            await Drag.OnDragStartAsync().ConfigureAwait(false);

		DragService.ReduceShouldDisplayAndMouseEventArgsAndDragSetAction(true, null, Drag);
    }

    public Task SubscribeToDragEventWithMoveHandle()
    {
        return SubscribeToDragEventAsync(DragEventHandlerMoveHandleAsync);
    }

    #region ResizeHandleStyleCss

    private string GetNorthResizeHandleStyleCss()
    {
        var parentElementWidth = ElementDimensions.WidthDimensionAttribute;

        _northResizeHandleDimensions.ElementPositionKind = ElementPositionKind.Absolute;

        // Width
        {
            _northResizeHandleDimensions.WidthDimensionAttribute.DimensionUnitList.Clear();

            _northResizeHandleDimensions.WidthDimensionAttribute.DimensionUnitList.AddRange(parentElementWidth.DimensionUnitList);

            // width: calc(60vw - 42px);
            _northResizeHandleDimensions.WidthDimensionAttribute.DimensionUnitList.Add(new DimensionUnit(
            	RESIZE_HANDLE_SQUARE_PIXELS,
            	DimensionUnitKind.Pixels,
            	DimensionOperatorKind.Subtract));
        }

        // Height
        {
            _northResizeHandleDimensions.HeightDimensionAttribute.DimensionUnitList.Clear();

            _northResizeHandleDimensions.HeightDimensionAttribute.DimensionUnitList.Add(new DimensionUnit(
            	RESIZE_HANDLE_SQUARE_PIXELS,
            	DimensionUnitKind.Pixels));
        }

        // Left
        {
            _northResizeHandleDimensions.LeftDimensionAttribute.DimensionUnitList.Clear();

            _northResizeHandleDimensions.LeftDimensionAttribute.DimensionUnitList.Add(new DimensionUnit(
            	RESIZE_HANDLE_SQUARE_PIXELS / 2,
            	DimensionUnitKind.Pixels));
        }

        // Top
        {
            _northResizeHandleDimensions.TopDimensionAttribute.DimensionUnitList.Clear();

            _northResizeHandleDimensions.TopDimensionAttribute.DimensionUnitList.Add(new DimensionUnit(
            	-1 * RESIZE_HANDLE_SQUARE_PIXELS / 2,
            	DimensionUnitKind.Pixels));
        }

        return _northResizeHandleDimensions.StyleString;
    }

    private string GetEastResizeHandleStyleCss()
    {
        var parentElementWidth = ElementDimensions.WidthDimensionAttribute;

        var parentElementHeight = ElementDimensions.HeightDimensionAttribute;

        _eastResizeHandleDimensions.ElementPositionKind = ElementPositionKind.Absolute;

        // Width
        {
            _eastResizeHandleDimensions.WidthDimensionAttribute.DimensionUnitList.Clear();

            _eastResizeHandleDimensions.WidthDimensionAttribute.DimensionUnitList.Add(new DimensionUnit(
            	RESIZE_HANDLE_SQUARE_PIXELS,
            	DimensionUnitKind.Pixels));
        }

        // Height
        {
            _eastResizeHandleDimensions.HeightDimensionAttribute.DimensionUnitList.Clear();

            _eastResizeHandleDimensions.HeightDimensionAttribute.DimensionUnitList.AddRange(parentElementHeight.DimensionUnitList);

            _eastResizeHandleDimensions.HeightDimensionAttribute.DimensionUnitList.Add(new DimensionUnit(
            	RESIZE_HANDLE_SQUARE_PIXELS,
            	DimensionUnitKind.Pixels,
            	DimensionOperatorKind.Subtract));
        }

        // Left
        {
            _eastResizeHandleDimensions.LeftDimensionAttribute.DimensionUnitList.Clear();

            _eastResizeHandleDimensions.LeftDimensionAttribute.DimensionUnitList.AddRange(parentElementWidth.DimensionUnitList);

            _eastResizeHandleDimensions.LeftDimensionAttribute.DimensionUnitList.Add(new DimensionUnit(
            	RESIZE_HANDLE_SQUARE_PIXELS / 2,
            	DimensionUnitKind.Pixels,
            	DimensionOperatorKind.Subtract));
        }

        // Top
        {
            _eastResizeHandleDimensions.TopDimensionAttribute.DimensionUnitList.Clear();

            _eastResizeHandleDimensions.TopDimensionAttribute.DimensionUnitList.Add(new DimensionUnit(
            	RESIZE_HANDLE_SQUARE_PIXELS / 2,
            	DimensionUnitKind.Pixels));
        }

        return _eastResizeHandleDimensions.StyleString;
    }

    private string GetSouthResizeHandleStyleCss()
    {
        var parentElementWidth = ElementDimensions.WidthDimensionAttribute;
        var parentElementHeight = ElementDimensions.HeightDimensionAttribute;

        _southResizeHandleDimensions.ElementPositionKind = ElementPositionKind.Absolute;

        // Width
        {
            _southResizeHandleDimensions.WidthDimensionAttribute.DimensionUnitList.Clear();

            _southResizeHandleDimensions.WidthDimensionAttribute.DimensionUnitList.AddRange(parentElementWidth.DimensionUnitList);

            _southResizeHandleDimensions.WidthDimensionAttribute.DimensionUnitList.Add(new DimensionUnit(
            	RESIZE_HANDLE_SQUARE_PIXELS,
            	DimensionUnitKind.Pixels,
            	DimensionOperatorKind.Subtract));
        }

        // Height
        {
            _southResizeHandleDimensions.HeightDimensionAttribute.DimensionUnitList.Clear();

            _southResizeHandleDimensions.HeightDimensionAttribute.DimensionUnitList.Add(new DimensionUnit(
            	RESIZE_HANDLE_SQUARE_PIXELS,
            	DimensionUnitKind.Pixels));
        }

        // Left
        {
            _southResizeHandleDimensions.LeftDimensionAttribute.DimensionUnitList.Clear();

            _southResizeHandleDimensions.LeftDimensionAttribute.DimensionUnitList.Add(new DimensionUnit(
            	RESIZE_HANDLE_SQUARE_PIXELS / 2,
            	DimensionUnitKind.Pixels));
        }

        // Top
        {
            _southResizeHandleDimensions.TopDimensionAttribute.DimensionUnitList.Clear();

            _southResizeHandleDimensions.TopDimensionAttribute.DimensionUnitList.AddRange(parentElementHeight.DimensionUnitList);

            _southResizeHandleDimensions.TopDimensionAttribute.DimensionUnitList.Add(new DimensionUnit(
            	RESIZE_HANDLE_SQUARE_PIXELS / 2,
            	DimensionUnitKind.Pixels,
            	DimensionOperatorKind.Subtract));
        }

        return _southResizeHandleDimensions.StyleString;
    }

    private string GetWestResizeHandleStyleCss()
    {
        var parentElementHeight = ElementDimensions.HeightDimensionAttribute;

        _westResizeHandleDimensions.ElementPositionKind = ElementPositionKind.Absolute;

        // Width
        {
            _westResizeHandleDimensions.WidthDimensionAttribute.DimensionUnitList.Clear();

            _westResizeHandleDimensions.WidthDimensionAttribute.DimensionUnitList.Add(new DimensionUnit(
            	RESIZE_HANDLE_SQUARE_PIXELS,
            	DimensionUnitKind.Pixels));
        }

        // Height
        {
            _westResizeHandleDimensions.HeightDimensionAttribute.DimensionUnitList.Clear();

            _westResizeHandleDimensions.HeightDimensionAttribute.DimensionUnitList.AddRange(parentElementHeight.DimensionUnitList);

            _westResizeHandleDimensions.HeightDimensionAttribute.DimensionUnitList.Add(new DimensionUnit(
            	RESIZE_HANDLE_SQUARE_PIXELS,
            	DimensionUnitKind.Pixels,
            	DimensionOperatorKind.Subtract));
        }

        // Left
        {
            _westResizeHandleDimensions.LeftDimensionAttribute.DimensionUnitList.Clear();

            _westResizeHandleDimensions.LeftDimensionAttribute.DimensionUnitList.Add(new DimensionUnit(
            	-1 * RESIZE_HANDLE_SQUARE_PIXELS / 2,
            	DimensionUnitKind.Pixels));
        }

        // Top
        {
            _westResizeHandleDimensions.TopDimensionAttribute.DimensionUnitList.Clear();

            _westResizeHandleDimensions.TopDimensionAttribute.DimensionUnitList.Add(new DimensionUnit(
            	RESIZE_HANDLE_SQUARE_PIXELS / 2,
            	DimensionUnitKind.Pixels));
        }

        return _westResizeHandleDimensions.StyleString;
    }

    private string GetNorthEastResizeHandleStyleCss()
    {
        var parentElementWidth = ElementDimensions.WidthDimensionAttribute;

        _northEastResizeHandleDimensions.ElementPositionKind = ElementPositionKind.Absolute;

        // Width
        {
            _northEastResizeHandleDimensions.WidthDimensionAttribute.DimensionUnitList.Clear();

            _northEastResizeHandleDimensions.WidthDimensionAttribute.DimensionUnitList.Add(new DimensionUnit(
            	RESIZE_HANDLE_SQUARE_PIXELS,
            	DimensionUnitKind.Pixels));
        }

        // Height
        {
            _northEastResizeHandleDimensions.HeightDimensionAttribute.DimensionUnitList.Clear();

            _northEastResizeHandleDimensions.HeightDimensionAttribute.DimensionUnitList.Add(new DimensionUnit(
            	RESIZE_HANDLE_SQUARE_PIXELS,
            	DimensionUnitKind.Pixels));
        }

        // Left
        {
            _northEastResizeHandleDimensions.LeftDimensionAttribute.DimensionUnitList.Clear();

            _northEastResizeHandleDimensions.LeftDimensionAttribute.DimensionUnitList.AddRange(parentElementWidth.DimensionUnitList);

            _northEastResizeHandleDimensions.LeftDimensionAttribute.DimensionUnitList.Add(new DimensionUnit(
            	RESIZE_HANDLE_SQUARE_PIXELS / 2,
            	DimensionUnitKind.Pixels,
            	DimensionOperatorKind.Subtract));
        }

        // Top
        {
            _northEastResizeHandleDimensions.TopDimensionAttribute.DimensionUnitList.Clear();

            _northEastResizeHandleDimensions.TopDimensionAttribute.DimensionUnitList.Add(new DimensionUnit(
            	-1 * RESIZE_HANDLE_SQUARE_PIXELS / 2,
            	DimensionUnitKind.Pixels));
        }

        return _northEastResizeHandleDimensions.StyleString;
    }

    private string GetSouthEastResizeHandleStyleCss()
    {
        var parentElementWidth = ElementDimensions.WidthDimensionAttribute;
        var parentElementHeight = ElementDimensions.HeightDimensionAttribute;

        _southEastResizeHandleDimensions.ElementPositionKind = ElementPositionKind.Absolute;

        // Width
        {
            _southEastResizeHandleDimensions.WidthDimensionAttribute.DimensionUnitList.Clear();

            _southEastResizeHandleDimensions.WidthDimensionAttribute.DimensionUnitList.Add(new DimensionUnit(
            	RESIZE_HANDLE_SQUARE_PIXELS,
            	DimensionUnitKind.Pixels));
        }

        // Height
        {
            _southEastResizeHandleDimensions.HeightDimensionAttribute.DimensionUnitList.Clear();

            _southEastResizeHandleDimensions.HeightDimensionAttribute.DimensionUnitList.Add(new DimensionUnit(
            	RESIZE_HANDLE_SQUARE_PIXELS,
            	DimensionUnitKind.Pixels));
        }

        // Left
        {
            _southEastResizeHandleDimensions.LeftDimensionAttribute.DimensionUnitList.Clear();

            _southEastResizeHandleDimensions.LeftDimensionAttribute.DimensionUnitList.AddRange(parentElementWidth.DimensionUnitList);

            _southEastResizeHandleDimensions.LeftDimensionAttribute.DimensionUnitList.Add(new DimensionUnit(
            	RESIZE_HANDLE_SQUARE_PIXELS / 2,
            	DimensionUnitKind.Pixels,
            	DimensionOperatorKind.Subtract));
        }

        // Top
        {
            _southEastResizeHandleDimensions.TopDimensionAttribute.DimensionUnitList.Clear();

            _southEastResizeHandleDimensions.TopDimensionAttribute.DimensionUnitList.AddRange(parentElementHeight.DimensionUnitList);

            _southEastResizeHandleDimensions.TopDimensionAttribute.DimensionUnitList.Add(new DimensionUnit(
            	RESIZE_HANDLE_SQUARE_PIXELS / 2,
            	DimensionUnitKind.Pixels,
            	DimensionOperatorKind.Subtract));
        }

        return _southEastResizeHandleDimensions.StyleString;
    }

    private string GetSouthWestResizeHandleStyleCss()
    {
        var parentElementHeight = ElementDimensions.HeightDimensionAttribute;

        _southWestResizeHandleDimensions.ElementPositionKind = ElementPositionKind.Absolute;

        // Width
        {
            _southWestResizeHandleDimensions.WidthDimensionAttribute.DimensionUnitList.Clear();

            _southWestResizeHandleDimensions.WidthDimensionAttribute.DimensionUnitList.Add(new DimensionUnit(
            	RESIZE_HANDLE_SQUARE_PIXELS,
            	DimensionUnitKind.Pixels));
        }

        // Height
        {
            _southWestResizeHandleDimensions.HeightDimensionAttribute.DimensionUnitList.Clear();

            _southWestResizeHandleDimensions.HeightDimensionAttribute.DimensionUnitList.Add(new DimensionUnit(
            	RESIZE_HANDLE_SQUARE_PIXELS,
            	DimensionUnitKind.Pixels));
        }

        // Left
        {
            _southWestResizeHandleDimensions.LeftDimensionAttribute.DimensionUnitList.Clear();

            _southWestResizeHandleDimensions.LeftDimensionAttribute.DimensionUnitList.Add(new DimensionUnit(
            	-1 * RESIZE_HANDLE_SQUARE_PIXELS / 2,
            	DimensionUnitKind.Pixels));
        }

        // Top
        {
            _southWestResizeHandleDimensions.TopDimensionAttribute.DimensionUnitList.Clear();

            _southWestResizeHandleDimensions.TopDimensionAttribute.DimensionUnitList.AddRange(parentElementHeight.DimensionUnitList);

            _southWestResizeHandleDimensions.TopDimensionAttribute.DimensionUnitList.Add(new DimensionUnit(
            	RESIZE_HANDLE_SQUARE_PIXELS / 2,
            	DimensionUnitKind.Pixels,
            	DimensionOperatorKind.Subtract));
        }

        return _southWestResizeHandleDimensions.StyleString;
    }

    private string GetNorthWestResizeHandleStyleCss()
    {
        _northWestResizeHandleDimensions.ElementPositionKind = ElementPositionKind.Absolute;

        // Width
        {
            _northWestResizeHandleDimensions.WidthDimensionAttribute.DimensionUnitList.Clear();

            _northWestResizeHandleDimensions.WidthDimensionAttribute.DimensionUnitList.Add(new DimensionUnit(
            	RESIZE_HANDLE_SQUARE_PIXELS,
            	DimensionUnitKind.Pixels));
        }

        // Height
        {
            _northWestResizeHandleDimensions.HeightDimensionAttribute.DimensionUnitList.Clear();

            _northWestResizeHandleDimensions.HeightDimensionAttribute.DimensionUnitList.Add(new DimensionUnit(
            	RESIZE_HANDLE_SQUARE_PIXELS,
            	DimensionUnitKind.Pixels));
        }

        // Left
        {
            _northWestResizeHandleDimensions.LeftDimensionAttribute.DimensionUnitList.Clear();

            _northWestResizeHandleDimensions.LeftDimensionAttribute.DimensionUnitList.Add(new DimensionUnit(
            	-1 * RESIZE_HANDLE_SQUARE_PIXELS / 2,
            	DimensionUnitKind.Pixels));
        }

        // Top
        {
            _northWestResizeHandleDimensions.TopDimensionAttribute.DimensionUnitList.Clear();

            _northWestResizeHandleDimensions.TopDimensionAttribute.DimensionUnitList.Add(new DimensionUnit(
            	-1 * RESIZE_HANDLE_SQUARE_PIXELS / 2,
            	DimensionUnitKind.Pixels));
        }

        return _northWestResizeHandleDimensions.StyleString;
    }

    #endregion

    #region DragEventHandlers

    private async Task DragEventHandlerNorthResizeHandleAsync(
        (MouseEventArgs firstMouseEventArgs, MouseEventArgs secondMouseEventArgs) mouseEventArgsTuple)
    {
        ResizeHelper.ResizeNorth(
            ElementDimensions,
            mouseEventArgsTuple.firstMouseEventArgs,
            mouseEventArgsTuple.secondMouseEventArgs);

        await InvokeAsync(StateHasChanged);
    }

    private async Task DragEventHandlerEastResizeHandleAsync(
        (MouseEventArgs firstMouseEventArgs, MouseEventArgs secondMouseEventArgs) mouseEventArgsTuple)
    {
        ResizeHelper.ResizeEast(
            ElementDimensions,
            mouseEventArgsTuple.firstMouseEventArgs,
            mouseEventArgsTuple.secondMouseEventArgs);

        await InvokeAsync(StateHasChanged);
    }

    private async Task DragEventHandlerSouthResizeHandleAsync(
        (MouseEventArgs firstMouseEventArgs, MouseEventArgs secondMouseEventArgs) mouseEventArgsTuple)
    {
        ResizeHelper.ResizeSouth(
            ElementDimensions,
            mouseEventArgsTuple.firstMouseEventArgs,
            mouseEventArgsTuple.secondMouseEventArgs);

        await InvokeAsync(StateHasChanged);
    }

    private async Task DragEventHandlerWestResizeHandleAsync(
        (MouseEventArgs firstMouseEventArgs, MouseEventArgs secondMouseEventArgs) mouseEventArgsTuple)
    {
        ResizeHelper.ResizeWest(
            ElementDimensions,
            mouseEventArgsTuple.firstMouseEventArgs,
            mouseEventArgsTuple.secondMouseEventArgs);

        await InvokeAsync(StateHasChanged);
    }

    private async Task DragEventHandlerNorthEastResizeHandleAsync(
        (MouseEventArgs firstMouseEventArgs, MouseEventArgs secondMouseEventArgs) mouseEventArgsTuple)
    {
        ResizeHelper.ResizeNorthEast(
            ElementDimensions,
            mouseEventArgsTuple.firstMouseEventArgs,
            mouseEventArgsTuple.secondMouseEventArgs);

        await InvokeAsync(StateHasChanged);
    }

    private async Task DragEventHandlerSouthEastResizeHandleAsync(
        (MouseEventArgs firstMouseEventArgs, MouseEventArgs secondMouseEventArgs) mouseEventArgsTuple)
    {
        ResizeHelper.ResizeSouthEast(
            ElementDimensions,
            mouseEventArgsTuple.firstMouseEventArgs,
            mouseEventArgsTuple.secondMouseEventArgs);

        await InvokeAsync(StateHasChanged);
    }

    private async Task DragEventHandlerSouthWestResizeHandleAsync(
        (MouseEventArgs firstMouseEventArgs, MouseEventArgs secondMouseEventArgs) mouseEventArgsTuple)
    {
        ResizeHelper.ResizeSouthWest(
            ElementDimensions,
            mouseEventArgsTuple.firstMouseEventArgs,
            mouseEventArgsTuple.secondMouseEventArgs);

        await InvokeAsync(StateHasChanged);
    }

    private async Task DragEventHandlerNorthWestResizeHandleAsync(
        (MouseEventArgs firstMouseEventArgs, MouseEventArgs secondMouseEventArgs) mouseEventArgsTuple)
    {
        ResizeHelper.ResizeNorthWest(
            ElementDimensions,
            mouseEventArgsTuple.firstMouseEventArgs,
            mouseEventArgsTuple.secondMouseEventArgs);

        await InvokeAsync(StateHasChanged);
    }

    private async Task DragEventHandlerMoveHandleAsync(
        (MouseEventArgs firstMouseEventArgs, MouseEventArgs secondMouseEventArgs) mouseEventArgsTuple)
    {
        ResizeHelper.Move(
            ElementDimensions,
            mouseEventArgsTuple.firstMouseEventArgs,
            mouseEventArgsTuple.secondMouseEventArgs);

        await InvokeAsync(StateHasChanged);
    }

    #endregion

    public void Dispose()
    {
        DragService.DragStateChanged -= DragStateWrapOnStateChanged;
    }
}