using Luthetus.Common.RazorLib.Dimensions;
using Luthetus.Common.RazorLib.Resize;
using Luthetus.Common.RazorLib.Store.DragCase;
using Luthetus.Ide.ClassLib.Store.PanelCase;
using Luthetus.Ide.ClassLib.Panel;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Luthetus.Ide.RazorLib.Panel;

public partial class PanelTabDisplay : ComponentBase, IDisposable
{
    [Inject]
    private IState<DragState> DragStateWrap { get; set; } = null!;
    [Inject]
    private IState<PanelsCollection> PanelsCollectionWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Parameter, EditorRequired]
    public PanelTab PanelTab { get; set; } = null!;
    [Parameter, EditorRequired]
    public PanelRecord PanelRecord { get; set; } = null!;

    private bool IsActive => PanelRecord.ActivePanelTabKey == PanelTab.PanelTabKey;

    private string IsActiveCssClassString => IsActive
        ? "luth_active"
        : string.Empty;

    private bool _thinksLeftMouseButtonIsDown;

    private Func<(MouseEventArgs firstMouseEventArgs, MouseEventArgs secondMouseEventArgs), Task>? _dragEventHandler;
    private MouseEventArgs? _previousDragMouseEventArgs;

    protected override void OnInitialized()
    {
        DragStateWrap.StateChanged += DragStateWrapOnStateChanged;
        PanelsCollectionWrap.StateChanged += PanelsCollectionWrapOnStateChanged;

        base.OnInitialized();
    }

    private async void PanelsCollectionWrapOnStateChanged(object? sender, EventArgs e)
    {
        await InvokeAsync(StateHasChanged);
    }

    private void DispatchSetActivePanelTabActionOnClick()
    {
        if (IsActive)
        {
            Dispatcher.Dispatch(new PanelsCollection.SetActivePanelTabAction(
                PanelRecord.PanelRecordKey,
                PanelTabKey.Empty));
        }
        else
        {
            Dispatcher.Dispatch(new PanelsCollection.SetActivePanelTabAction(
                PanelRecord.PanelRecordKey,
                PanelTab.PanelTabKey));
        }
    }

    private Task HandleOnMouseDownAsync(MouseEventArgs mouseEventArgs)
    {
        _thinksLeftMouseButtonIsDown = true;

        return Task.CompletedTask;
    }

    private Task HandleOnMouseOutAsync(MouseEventArgs mouseEventArgs)
    {
        if (_thinksLeftMouseButtonIsDown &&
            PanelsCollectionWrap.Value.PanelDragEventArgs is null)
        {
            var leftDimensionAttribute = PanelTab.BeingDraggedDimensions.DimensionAttributes
                .First(x =>
                    x.DimensionAttributeKind == DimensionAttributeKind.Left);

            leftDimensionAttribute.DimensionUnits.Clear();

            leftDimensionAttribute.DimensionUnits.Add(new DimensionUnit
            {
                Value = mouseEventArgs.ClientX,
                DimensionUnitKind = DimensionUnitKind.Pixels
            });

            var topDimensionAttribute = PanelTab.BeingDraggedDimensions.DimensionAttributes
                .First(x =>
                    x.DimensionAttributeKind == DimensionAttributeKind.Top);

            topDimensionAttribute.DimensionUnits.Clear();

            topDimensionAttribute.DimensionUnits.Add(new DimensionUnit
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

    private Task TopDropzoneOnMouseUp(MouseEventArgs mouseEventArgs)
    {
        var panelDragEventArgs = PanelsCollectionWrap.Value.PanelDragEventArgs;

        if (panelDragEventArgs is not null)
        {
            Dispatcher.Dispatch(new PanelsCollection.DisposePanelTabAction(
                panelDragEventArgs.Value.ParentPanelRecord.PanelRecordKey,
                panelDragEventArgs.Value.TagDragTarget.PanelTabKey));

            Dispatcher.Dispatch(new PanelsCollection.RegisterPanelTabAction(
                PanelRecord.PanelRecordKey,
                panelDragEventArgs.Value.TagDragTarget));
        }

        return Task.CompletedTask;
    }

    private Task BottomDropzoneOnMouseUp(MouseEventArgs mouseEventArgs)
    {
        var panelDragEventArgs = PanelsCollectionWrap.Value.PanelDragEventArgs;

        if (panelDragEventArgs is not null)
        {
            Dispatcher.Dispatch(new PanelsCollection.DisposePanelTabAction(
                panelDragEventArgs.Value.ParentPanelRecord.PanelRecordKey,
                panelDragEventArgs.Value.TagDragTarget.PanelTabKey));

            Dispatcher.Dispatch(new PanelsCollection.RegisterPanelTabAction(
                PanelRecord.PanelRecordKey,
                panelDragEventArgs.Value.TagDragTarget));
        }

        return Task.CompletedTask;
    }

    private async void DragStateWrapOnStateChanged(object? sender, EventArgs e)
    {
        if (!DragStateWrap.Value.ShouldDisplay)
        {
            _dragEventHandler = null;
            _previousDragMouseEventArgs = null;
            _thinksLeftMouseButtonIsDown = false;
            Dispatcher.Dispatch(new PanelsCollection.SetPanelDragEventArgsAction(null));
            PanelTab.IsBeingDragged = false;
        }
        else
        {
            var mouseEventArgs = DragStateWrap.Value.MouseEventArgs;

            if (_dragEventHandler is not null)
            {
                if (_previousDragMouseEventArgs is not null &&
                    mouseEventArgs is not null)
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

        Dispatcher.Dispatch(new PanelsCollection.SetPanelDragEventArgsAction(
            (PanelTab, PanelRecord)));

        Dispatcher.Dispatch(new DragState.SetDragStateAction(true, null));
    }

    private async Task DragEventHandlerScrollAsync(
        (MouseEventArgs firstMouseEventArgs, MouseEventArgs secondMouseEventArgs) mouseEventArgsTuple)
    {
        var localThinksLeftMouseButtonIsDown = _thinksLeftMouseButtonIsDown;

        if (!localThinksLeftMouseButtonIsDown)
            return;

        // Buttons is a bit flag
        // '& 1' gets if left mouse button is held
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
            Dispatcher.Dispatch(new PanelsCollection.SetPanelDragEventArgsAction(null));
            PanelTab.IsBeingDragged = false;
        }
    }

    public void Dispose()
    {
        DragStateWrap.StateChanged -= DragStateWrapOnStateChanged;
        PanelsCollectionWrap.StateChanged -= PanelsCollectionWrapOnStateChanged;
    }
}