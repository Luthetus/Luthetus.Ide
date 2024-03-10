using Fluxor;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Drags.Displays;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Panels.States;
using Luthetus.Common.RazorLib.Panels.Models;
using Luthetus.Common.RazorLib.Resizes.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Luthetus.Common.RazorLib.Panels.Displays;

public partial class PanelTabDisplay : ComponentBase, IDisposable
{
    [Inject]
    private IState<DragState> DragStateWrap { get; set; } = null!;
    [Inject]
    private IState<PanelsState> PanelsStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Parameter, EditorRequired]
    public PanelTab PanelTab { get; set; } = null!;
    [Parameter, EditorRequired]
    public PanelGroup PanelRecord { get; set; } = null!;

    private bool _thinksLeftMouseButtonIsDown;
    private Func<(MouseEventArgs firstMouseEventArgs, MouseEventArgs secondMouseEventArgs), Task>? _dragEventHandler;
    private MouseEventArgs? _previousDragMouseEventArgs;

    private bool IsActive => PanelRecord.ActiveTabKey == PanelTab.Key;

    private string IsActiveCssClassString => IsActive ? "luth_active" : string.Empty;

    protected override void OnInitialized()
    {
        PanelsStateWrap.StateChanged += PanelsCollectionWrapOnStateChanged;
        DragStateWrap.StateChanged += DragStateWrapOnStateChanged;

        base.OnInitialized();
    }

    private async void PanelsCollectionWrapOnStateChanged(object? sender, EventArgs e)
    {
        await InvokeAsync(StateHasChanged);
    }

    private async void DragStateWrapOnStateChanged(object? sender, EventArgs e)
    {
        if (!DragStateWrap.Value.ShouldDisplay)
        {
            _dragEventHandler = null;
            _previousDragMouseEventArgs = null;
            _thinksLeftMouseButtonIsDown = false;

            Dispatcher.Dispatch(new PanelsState.SetDragEventArgsAction(null));

            PanelTab.IsBeingDragged = false;
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
                await InvokeAsync(StateHasChanged);
            }
        }
    }

    private void DispatchSetActivePanelTabActionOnClick()
    {
        if (IsActive)
        {
            Dispatcher.Dispatch(new PanelsState.SetActivePanelTabAction(
                PanelRecord.Key,
                Key<PanelTab>.Empty));
        }
        else
        {
            Dispatcher.Dispatch(new PanelsState.SetActivePanelTabAction(
                PanelRecord.Key,
                PanelTab.Key));
        }
    }

    private Task HandleOnMouseDownAsync(MouseEventArgs mouseEventArgs)
    {
        _thinksLeftMouseButtonIsDown = true;
        return Task.CompletedTask;
    }

    private Task HandleOnMouseOutAsync(MouseEventArgs mouseEventArgs)
    {
        if (_thinksLeftMouseButtonIsDown && PanelsStateWrap.Value.DragEventArgs is null)
        {
            var leftDimensionAttribute = PanelTab.BeingDraggedDimensions.DimensionAttributeList.First(
                x => x.DimensionAttributeKind == DimensionAttributeKind.Left);

            leftDimensionAttribute.DimensionUnitList.Clear();

            leftDimensionAttribute.DimensionUnitList.Add(new DimensionUnit
            {
                Value = mouseEventArgs.ClientX,
                DimensionUnitKind = DimensionUnitKind.Pixels
            });

            var topDimensionAttribute = PanelTab.BeingDraggedDimensions.DimensionAttributeList.First(
                x => x.DimensionAttributeKind == DimensionAttributeKind.Top);

            topDimensionAttribute.DimensionUnitList.Clear();

            topDimensionAttribute.DimensionUnitList.Add(new DimensionUnit
            {
                Value = mouseEventArgs.ClientY,
                DimensionUnitKind = DimensionUnitKind.Pixels
            });

            PanelTab.BeingDraggedDimensions.ElementPositionKind = ElementPositionKind.Fixed;
            PanelTab.IsBeingDragged = true;

            SubscribeToDragEventForScrolling();
        }

        return Task.CompletedTask;
    }

    private Task HandleOnMouseUpAsync(MouseEventArgs mouseEventArgs)
    {
        _thinksLeftMouseButtonIsDown = false;

        return Task.CompletedTask;
    }

    public void SubscribeToDragEventForScrolling()
    {
        _dragEventHandler = DragEventHandlerScrollAsync;

        Dispatcher.Dispatch(new PanelsState.SetDragEventArgsAction((PanelTab, PanelRecord)));

        Dispatcher.Dispatch(new DragState.WithAction(inState => inState with
        {
            ShouldDisplay = true,
            MouseEventArgs = null,
        }));
    }

    private Task DragEventHandlerScrollAsync(
        (MouseEventArgs firstMouseEventArgs, MouseEventArgs secondMouseEventArgs) mouseEventArgsTuple)
    {
        var localThinksLeftMouseButtonIsDown = _thinksLeftMouseButtonIsDown;

        if (!localThinksLeftMouseButtonIsDown)
            return Task.CompletedTask;

        // Buttons is a bit flag '& 1' gets if left mouse button is held
        if (localThinksLeftMouseButtonIsDown &&
            (mouseEventArgsTuple.secondMouseEventArgs.Buttons & 1) == 1)
        {
            ResizeHelper.Move(
                PanelTab.BeingDraggedDimensions,
                mouseEventArgsTuple.firstMouseEventArgs,
                mouseEventArgsTuple.secondMouseEventArgs);
        }
        else
        {
            _thinksLeftMouseButtonIsDown = false;

            Dispatcher.Dispatch(new PanelsState.SetDragEventArgsAction(null));

            PanelTab.IsBeingDragged = false;
        }

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        PanelsStateWrap.StateChanged -= PanelsCollectionWrapOnStateChanged;
        DragStateWrap.StateChanged -= DragStateWrapOnStateChanged;
    }
}