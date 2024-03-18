using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Luthetus.Common.RazorLib.Drags.Displays;
using Luthetus.Common.RazorLib.Resizes.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;

namespace Luthetus.Common.RazorLib.Resizes.Displays;

public partial class ResizableDisplay : ComponentBase, IDisposable
{
    [Inject]
    private IState<DragState> DragStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Parameter, EditorRequired]
    public ElementDimensions ElementDimensions { get; set; } = null!;
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
        DragStateWrap.StateChanged += DragStateWrapOnStateChanged;

        base.OnInitialized();
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
                        .Invoke((_previousDragMouseEventArgs, mouseEventArgs));
                }

                _previousDragMouseEventArgs = mouseEventArgs;
                await ReRenderFuncAsync.Invoke();
            }
        }
    }

    private async Task SubscribeToDragEventAsync(
        Func<(MouseEventArgs firstMouseEventArgs, MouseEventArgs secondMouseEventArgs), Task> dragEventHandler)
    {
        _dragEventHandler = dragEventHandler;

        if (Drag is not null)
            await Drag.OnDragStartAsync();

        Dispatcher.Dispatch(new DragState.WithAction(inState => inState with
        {
            ShouldDisplay = true,
            MouseEventArgs = null,
			Drag = Drag,
        }));
    }

    public void SubscribeToDragEventWithMoveHandle()
    {
        SubscribeToDragEventAsync(DragEventHandlerMoveHandleAsync);
    }

    #region ResizeHandleStyleCss

    private string GetNorthResizeHandleStyleCss()
    {
        var parentElementWidth = ElementDimensions.DimensionAttributeList.Single(
            x => x.DimensionAttributeKind == DimensionAttributeKind.Width);

        _northResizeHandleDimensions.ElementPositionKind = ElementPositionKind.Absolute;

        // Width
        {
            var resizeHandleWidth = _northResizeHandleDimensions.DimensionAttributeList.Single(
                x => x.DimensionAttributeKind == DimensionAttributeKind.Width);

            resizeHandleWidth.DimensionUnitList.Clear();

            resizeHandleWidth.DimensionUnitList.AddRange(parentElementWidth.DimensionUnitList);

            // width: calc(60vw - 42px);
            resizeHandleWidth.DimensionUnitList.Add(new DimensionUnit
            {
                Value = RESIZE_HANDLE_SQUARE_PIXELS,
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionOperatorKind = DimensionOperatorKind.Subtract
            });
        }

        // Height
        {
            var resizeHandleHeight = _northResizeHandleDimensions.DimensionAttributeList.Single(
                x => x.DimensionAttributeKind == DimensionAttributeKind.Height);

            resizeHandleHeight.DimensionUnitList.Clear();

            resizeHandleHeight.DimensionUnitList.Add(new DimensionUnit
            {
                Value = RESIZE_HANDLE_SQUARE_PIXELS,
                DimensionUnitKind = DimensionUnitKind.Pixels
            });
        }

        // Left
        {
            var resizeHandleLeft = _northResizeHandleDimensions.DimensionAttributeList.Single(
                x => x.DimensionAttributeKind == DimensionAttributeKind.Left);

            resizeHandleLeft.DimensionUnitList.Clear();

            resizeHandleLeft.DimensionUnitList.Add(new DimensionUnit
            {
                Value = RESIZE_HANDLE_SQUARE_PIXELS / 2,
                DimensionUnitKind = DimensionUnitKind.Pixels
            });
        }

        // Top
        {
            var resizeHandleTop = _northResizeHandleDimensions.DimensionAttributeList.Single(
                x => x.DimensionAttributeKind == DimensionAttributeKind.Top);

            resizeHandleTop.DimensionUnitList.Clear();

            resizeHandleTop.DimensionUnitList.Add(new DimensionUnit
            {
                Value = -1 * RESIZE_HANDLE_SQUARE_PIXELS / 2,
                DimensionUnitKind = DimensionUnitKind.Pixels
            });
        }

        return _northResizeHandleDimensions.StyleString;
    }

    private string GetEastResizeHandleStyleCss()
    {
        var parentElementWidth = ElementDimensions.DimensionAttributeList.Single(
            x => x.DimensionAttributeKind == DimensionAttributeKind.Width);

        var parentElementHeight = ElementDimensions.DimensionAttributeList.Single(
            x => x.DimensionAttributeKind == DimensionAttributeKind.Height);

        _eastResizeHandleDimensions.ElementPositionKind = ElementPositionKind.Absolute;

        // Width
        {
            var resizeHandleWidth = _eastResizeHandleDimensions.DimensionAttributeList.Single(
                x => x.DimensionAttributeKind == DimensionAttributeKind.Width);

            resizeHandleWidth.DimensionUnitList.Clear();

            resizeHandleWidth.DimensionUnitList.Add(new DimensionUnit
            {
                Value = RESIZE_HANDLE_SQUARE_PIXELS,
                DimensionUnitKind = DimensionUnitKind.Pixels
            });
        }

        // Height
        {
            var resizeHandleHeight = _eastResizeHandleDimensions.DimensionAttributeList.Single(
                x => x.DimensionAttributeKind == DimensionAttributeKind.Height);

            resizeHandleHeight.DimensionUnitList.Clear();

            resizeHandleHeight.DimensionUnitList.AddRange(parentElementHeight.DimensionUnitList);

            resizeHandleHeight.DimensionUnitList.Add(new DimensionUnit
            {
                Value = RESIZE_HANDLE_SQUARE_PIXELS,
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionOperatorKind = DimensionOperatorKind.Subtract
            });
        }

        // Left
        {
            var resizeHandleLeft = _eastResizeHandleDimensions.DimensionAttributeList.Single(
                x => x.DimensionAttributeKind == DimensionAttributeKind.Left);

            resizeHandleLeft.DimensionUnitList.Clear();

            resizeHandleLeft.DimensionUnitList.AddRange(parentElementWidth.DimensionUnitList);

            resizeHandleLeft.DimensionUnitList.Add(new DimensionUnit
            {
                Value = RESIZE_HANDLE_SQUARE_PIXELS / 2,
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionOperatorKind = DimensionOperatorKind.Subtract
            });
        }

        // Top
        {
            var resizeHandleTop = _eastResizeHandleDimensions.DimensionAttributeList.Single(
                x => x.DimensionAttributeKind == DimensionAttributeKind.Top);

            resizeHandleTop.DimensionUnitList.Clear();

            resizeHandleTop.DimensionUnitList.Add(new DimensionUnit
            {
                Value = RESIZE_HANDLE_SQUARE_PIXELS / 2,
                DimensionUnitKind = DimensionUnitKind.Pixels
            });
        }

        return _eastResizeHandleDimensions.StyleString;
    }

    private string GetSouthResizeHandleStyleCss()
    {
        var parentElementWidth = ElementDimensions.DimensionAttributeList.Single(
            x => x.DimensionAttributeKind == DimensionAttributeKind.Width);

        var parentElementHeight = ElementDimensions.DimensionAttributeList.Single(
            x => x.DimensionAttributeKind == DimensionAttributeKind.Height);

        _southResizeHandleDimensions.ElementPositionKind = ElementPositionKind.Absolute;

        // Width
        {
            var resizeHandleWidth = _southResizeHandleDimensions.DimensionAttributeList.Single(
                x => x.DimensionAttributeKind == DimensionAttributeKind.Width);

            resizeHandleWidth.DimensionUnitList.Clear();

            resizeHandleWidth.DimensionUnitList.AddRange(parentElementWidth.DimensionUnitList);

            resizeHandleWidth.DimensionUnitList.Add(new DimensionUnit
            {
                Value = RESIZE_HANDLE_SQUARE_PIXELS,
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionOperatorKind = DimensionOperatorKind.Subtract
            });
        }

        // Height
        {
            var resizeHandleHeight = _southResizeHandleDimensions.DimensionAttributeList.Single(
                x => x.DimensionAttributeKind == DimensionAttributeKind.Height);

            resizeHandleHeight.DimensionUnitList.Clear();

            resizeHandleHeight.DimensionUnitList.Add(new DimensionUnit
            {
                Value = RESIZE_HANDLE_SQUARE_PIXELS,
                DimensionUnitKind = DimensionUnitKind.Pixels
            });
        }

        // Left
        {
            var resizeHandleLeft = _southResizeHandleDimensions.DimensionAttributeList.Single(
                x => x.DimensionAttributeKind == DimensionAttributeKind.Left);

            resizeHandleLeft.DimensionUnitList.Clear();

            resizeHandleLeft.DimensionUnitList.Add(new DimensionUnit
            {
                Value = RESIZE_HANDLE_SQUARE_PIXELS / 2,
                DimensionUnitKind = DimensionUnitKind.Pixels
            });
        }

        // Top
        {
            var resizeHandleTop = _southResizeHandleDimensions.DimensionAttributeList.Single(
                x => x.DimensionAttributeKind == DimensionAttributeKind.Top);

            resizeHandleTop.DimensionUnitList.Clear();

            resizeHandleTop.DimensionUnitList.AddRange(parentElementHeight.DimensionUnitList);

            resizeHandleTop.DimensionUnitList.Add(new DimensionUnit
            {
                Value = RESIZE_HANDLE_SQUARE_PIXELS / 2,
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionOperatorKind = DimensionOperatorKind.Subtract
            });
        }

        return _southResizeHandleDimensions.StyleString;
    }

    private string GetWestResizeHandleStyleCss()
    {
        var parentElementHeight = ElementDimensions.DimensionAttributeList.Single(
            x => x.DimensionAttributeKind == DimensionAttributeKind.Height);

        _westResizeHandleDimensions.ElementPositionKind = ElementPositionKind.Absolute;

        // Width
        {
            var resizeHandleWidth = _westResizeHandleDimensions.DimensionAttributeList.Single(
                x => x.DimensionAttributeKind == DimensionAttributeKind.Width);

            resizeHandleWidth.DimensionUnitList.Clear();

            resizeHandleWidth.DimensionUnitList.Add(new DimensionUnit
            {
                Value = RESIZE_HANDLE_SQUARE_PIXELS,
                DimensionUnitKind = DimensionUnitKind.Pixels
            });
        }

        // Height
        {
            var resizeHandleHeight = _westResizeHandleDimensions.DimensionAttributeList.Single(
                x => x.DimensionAttributeKind == DimensionAttributeKind.Height);

            resizeHandleHeight.DimensionUnitList.Clear();

            resizeHandleHeight.DimensionUnitList.AddRange(parentElementHeight.DimensionUnitList);

            resizeHandleHeight.DimensionUnitList.Add(new DimensionUnit
            {
                Value = RESIZE_HANDLE_SQUARE_PIXELS,
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionOperatorKind = DimensionOperatorKind.Subtract
            });
        }

        // Left
        {
            var resizeHandleLeft = _westResizeHandleDimensions.DimensionAttributeList.Single(
                x => x.DimensionAttributeKind == DimensionAttributeKind.Left);

            resizeHandleLeft.DimensionUnitList.Clear();

            resizeHandleLeft.DimensionUnitList.Add(new DimensionUnit
            {
                Value = -1 * RESIZE_HANDLE_SQUARE_PIXELS / 2,
                DimensionUnitKind = DimensionUnitKind.Pixels
            });
        }

        // Top
        {
            var resizeHandleTop = _westResizeHandleDimensions.DimensionAttributeList.Single(
                x => x.DimensionAttributeKind == DimensionAttributeKind.Top);

            resizeHandleTop.DimensionUnitList.Clear();

            resizeHandleTop.DimensionUnitList.Add(new DimensionUnit
            {
                Value = RESIZE_HANDLE_SQUARE_PIXELS / 2,
                DimensionUnitKind = DimensionUnitKind.Pixels
            });
        }

        return _westResizeHandleDimensions.StyleString;
    }

    private string GetNorthEastResizeHandleStyleCss()
    {
        var parentElementWidth = ElementDimensions.DimensionAttributeList.Single(
            x => x.DimensionAttributeKind == DimensionAttributeKind.Width);

        _northEastResizeHandleDimensions.ElementPositionKind = ElementPositionKind.Absolute;

        // Width
        {
            var resizeHandleWidth = _northEastResizeHandleDimensions.DimensionAttributeList.Single(
                x => x.DimensionAttributeKind == DimensionAttributeKind.Width);

            resizeHandleWidth.DimensionUnitList.Clear();

            resizeHandleWidth.DimensionUnitList.Add(new DimensionUnit
            {
                Value = RESIZE_HANDLE_SQUARE_PIXELS,
                DimensionUnitKind = DimensionUnitKind.Pixels
            });
        }

        // Height
        {
            var resizeHandleHeight = _northEastResizeHandleDimensions.DimensionAttributeList.Single(
                x => x.DimensionAttributeKind == DimensionAttributeKind.Height);

            resizeHandleHeight.DimensionUnitList.Clear();

            resizeHandleHeight.DimensionUnitList.Add(new DimensionUnit
            {
                Value = RESIZE_HANDLE_SQUARE_PIXELS,
                DimensionUnitKind = DimensionUnitKind.Pixels
            });
        }

        // Left
        {
            var resizeHandleLeft = _northEastResizeHandleDimensions.DimensionAttributeList.Single(
                x => x.DimensionAttributeKind == DimensionAttributeKind.Left);

            resizeHandleLeft.DimensionUnitList.Clear();

            resizeHandleLeft.DimensionUnitList.AddRange(parentElementWidth.DimensionUnitList);

            resizeHandleLeft.DimensionUnitList.Add(new DimensionUnit
            {
                Value = RESIZE_HANDLE_SQUARE_PIXELS / 2,
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionOperatorKind = DimensionOperatorKind.Subtract
            });
        }

        // Top
        {
            var resizeHandleTop = _northEastResizeHandleDimensions.DimensionAttributeList.Single(
                x => x.DimensionAttributeKind == DimensionAttributeKind.Top);

            resizeHandleTop.DimensionUnitList.Clear();

            resizeHandleTop.DimensionUnitList.Add(new DimensionUnit
            {
                Value = -1 * RESIZE_HANDLE_SQUARE_PIXELS / 2,
                DimensionUnitKind = DimensionUnitKind.Pixels
            });
        }

        return _northEastResizeHandleDimensions.StyleString;
    }

    private string GetSouthEastResizeHandleStyleCss()
    {
        var parentElementWidth = ElementDimensions.DimensionAttributeList.Single(
            x => x.DimensionAttributeKind == DimensionAttributeKind.Width);

        var parentElementHeight = ElementDimensions.DimensionAttributeList.Single(
            x => x.DimensionAttributeKind == DimensionAttributeKind.Height);

        _southEastResizeHandleDimensions.ElementPositionKind = ElementPositionKind.Absolute;

        // Width
        {
            var resizeHandleWidth = _southEastResizeHandleDimensions.DimensionAttributeList.Single(
                x => x.DimensionAttributeKind == DimensionAttributeKind.Width);

            resizeHandleWidth.DimensionUnitList.Clear();

            resizeHandleWidth.DimensionUnitList.Add(new DimensionUnit
            {
                Value = RESIZE_HANDLE_SQUARE_PIXELS,
                DimensionUnitKind = DimensionUnitKind.Pixels
            });
        }

        // Height
        {
            var resizeHandleHeight = _southEastResizeHandleDimensions.DimensionAttributeList.Single(
                x => x.DimensionAttributeKind == DimensionAttributeKind.Height);

            resizeHandleHeight.DimensionUnitList.Clear();

            resizeHandleHeight.DimensionUnitList.Add(new DimensionUnit
            {
                Value = RESIZE_HANDLE_SQUARE_PIXELS,
                DimensionUnitKind = DimensionUnitKind.Pixels
            });
        }

        // Left
        {
            var resizeHandleLeft = _southEastResizeHandleDimensions.DimensionAttributeList.Single(
                x => x.DimensionAttributeKind == DimensionAttributeKind.Left);

            resizeHandleLeft.DimensionUnitList.Clear();

            resizeHandleLeft.DimensionUnitList.AddRange(parentElementWidth.DimensionUnitList);

            resizeHandleLeft.DimensionUnitList.Add(new DimensionUnit
            {
                Value = RESIZE_HANDLE_SQUARE_PIXELS / 2,
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionOperatorKind = DimensionOperatorKind.Subtract
            });
        }

        // Top
        {
            var resizeHandleTop = _southEastResizeHandleDimensions.DimensionAttributeList.Single(
                x => x.DimensionAttributeKind == DimensionAttributeKind.Top);

            resizeHandleTop.DimensionUnitList.Clear();

            resizeHandleTop.DimensionUnitList.AddRange(parentElementHeight.DimensionUnitList);

            resizeHandleTop.DimensionUnitList.Add(new DimensionUnit
            {
                Value = RESIZE_HANDLE_SQUARE_PIXELS / 2,
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionOperatorKind = DimensionOperatorKind.Subtract
            });
        }

        return _southEastResizeHandleDimensions.StyleString;
    }

    private string GetSouthWestResizeHandleStyleCss()
    {
        var parentElementHeight = ElementDimensions.DimensionAttributeList.Single(
            x => x.DimensionAttributeKind == DimensionAttributeKind.Height);

        _southWestResizeHandleDimensions.ElementPositionKind = ElementPositionKind.Absolute;

        // Width
        {
            var resizeHandleWidth = _southWestResizeHandleDimensions.DimensionAttributeList.Single(
                x => x.DimensionAttributeKind == DimensionAttributeKind.Width);

            resizeHandleWidth.DimensionUnitList.Clear();

            resizeHandleWidth.DimensionUnitList.Add(new DimensionUnit
            {
                Value = RESIZE_HANDLE_SQUARE_PIXELS,
                DimensionUnitKind = DimensionUnitKind.Pixels
            });
        }

        // Height
        {
            var resizeHandleHeight = _southWestResizeHandleDimensions.DimensionAttributeList.Single(
                x => x.DimensionAttributeKind == DimensionAttributeKind.Height);

            resizeHandleHeight.DimensionUnitList.Clear();

            resizeHandleHeight.DimensionUnitList.Add(new DimensionUnit
            {
                Value = RESIZE_HANDLE_SQUARE_PIXELS,
                DimensionUnitKind = DimensionUnitKind.Pixels
            });
        }

        // Left
        {
            var resizeHandleLeft = _southWestResizeHandleDimensions.DimensionAttributeList.Single(
                x => x.DimensionAttributeKind == DimensionAttributeKind.Left);

            resizeHandleLeft.DimensionUnitList.Clear();

            resizeHandleLeft.DimensionUnitList.Add(new DimensionUnit
            {
                Value = -1 * RESIZE_HANDLE_SQUARE_PIXELS / 2,
                DimensionUnitKind = DimensionUnitKind.Pixels
            });
        }

        // Top
        {
            var resizeHandleTop = _southWestResizeHandleDimensions.DimensionAttributeList.Single(
                x => x.DimensionAttributeKind == DimensionAttributeKind.Top);

            resizeHandleTop.DimensionUnitList.Clear();

            resizeHandleTop.DimensionUnitList.AddRange(parentElementHeight.DimensionUnitList);

            resizeHandleTop.DimensionUnitList.Add(new DimensionUnit
            {
                Value = RESIZE_HANDLE_SQUARE_PIXELS / 2,
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionOperatorKind = DimensionOperatorKind.Subtract
            });
        }

        return _southWestResizeHandleDimensions.StyleString;
    }

    private string GetNorthWestResizeHandleStyleCss()
    {
        _northWestResizeHandleDimensions.ElementPositionKind = ElementPositionKind.Absolute;

        // Width
        {
            var resizeHandleWidth = _northWestResizeHandleDimensions.DimensionAttributeList.Single(
                x => x.DimensionAttributeKind == DimensionAttributeKind.Width);

            resizeHandleWidth.DimensionUnitList.Clear();

            resizeHandleWidth.DimensionUnitList.Add(new DimensionUnit
            {
                Value = RESIZE_HANDLE_SQUARE_PIXELS,
                DimensionUnitKind = DimensionUnitKind.Pixels
            });
        }

        // Height
        {
            var resizeHandleHeight = _northWestResizeHandleDimensions.DimensionAttributeList.Single(
                x => x.DimensionAttributeKind == DimensionAttributeKind.Height);

            resizeHandleHeight.DimensionUnitList.Clear();

            resizeHandleHeight.DimensionUnitList.Add(new DimensionUnit
            {
                Value = RESIZE_HANDLE_SQUARE_PIXELS,
                DimensionUnitKind = DimensionUnitKind.Pixels
            });
        }

        // Left
        {
            var resizeHandleLeft = _northWestResizeHandleDimensions.DimensionAttributeList.Single(
                x => x.DimensionAttributeKind == DimensionAttributeKind.Left);

            resizeHandleLeft.DimensionUnitList.Clear();

            resizeHandleLeft.DimensionUnitList.Add(new DimensionUnit
            {
                Value = -1 * RESIZE_HANDLE_SQUARE_PIXELS / 2,
                DimensionUnitKind = DimensionUnitKind.Pixels
            });
        }

        // Top
        {
            var resizeHandleTop = _northWestResizeHandleDimensions.DimensionAttributeList.Single(
                x => x.DimensionAttributeKind == DimensionAttributeKind.Top);

            resizeHandleTop.DimensionUnitList.Clear();

            resizeHandleTop.DimensionUnitList.Add(new DimensionUnit
            {
                Value = -1 * RESIZE_HANDLE_SQUARE_PIXELS / 2,
                DimensionUnitKind = DimensionUnitKind.Pixels
            });
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
        DragStateWrap.StateChanged -= DragStateWrapOnStateChanged;
    }
}