using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Luthetus.Common.RazorLib.Drags.Displays;
using Luthetus.Common.RazorLib.Resizes.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Dimensions.States;

namespace Luthetus.Common.RazorLib.Resizes.Displays;

public partial class ResizableColumn : ComponentBase, IDisposable
{
    [Inject]
    private IState<DragState> DragStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Parameter, EditorRequired]
    public ElementDimensions LeftElementDimensions { get; set; } = null!;
    [Parameter, EditorRequired]
    public ElementDimensions RightElementDimensions { get; set; } = null!;
    [Parameter, EditorRequired]
    public Func<Task> ReRenderFuncAsync { get; set; } = null!;

    public const double RESIZE_HANDLE_WIDTH_IN_PIXELS = 4;

    private Func<(MouseEventArgs firstMouseEventArgs, MouseEventArgs secondMouseEventArgs), Task>? _dragEventHandler;
    private MouseEventArgs? _previousDragMouseEventArgs;

    protected override void OnInitialized()
    {
        DragStateWrap.StateChanged += DragStateWrapOnStateChanged;

        base.OnInitialized();
    }

    private async void DragStateWrapOnStateChanged(object? sender, EventArgs e)
    {
        if (!DragStateWrap.Value.ShouldDisplay)
        {
			// Goal: track resizing of app, then notify any subscribers to the app size (2024-05-29)
			// i.e.: the TextEditorViewModelDisplay needs to re-measure if the app size changes.
			// NOTE: Intra-app size changes apply too (such as this component) because they'll change the measurements of the text editor.
			//
			// This seems to be the place to dispatch a sort of "OnResizeEndEvent".
			// Since this is once the user lets go of the left mouse button to stop dragging.
			//
			// An initial worry is the lifecycle of the 'DragStateWrapOnStateChanged' event subscription.
			// For any rendered 'ResizableColumn' it seems that 'DragStateWrapOnStateChanged' is always going to be subscribed.
			//
			// I don't want to fire the so called "OnResizeEndEvent" many times for no reason.
			// So, I'll say 'if (_dragEventHandler is not null) ...'
			// Since the '_dragEventHandler' is being set 'onmousedown', then set 'null' when the dragging is finished.
			bool wasTargetOfDragging = _dragEventHandler is null;

            _dragEventHandler = null;
            _previousDragMouseEventArgs = null;

			// I want to make sure the _dragEventHandler and other state gets set to null prior to dispatching the "OnResizeEndEvent".
			// The worry being that some odd race conditions could occur by nature of dispatching the event
			// causing some code to fire, but this component never null'd out its state(2024-05-29)
			if (wasTargetOfDragging)
				Dispatcher.Dispatch(new AppDimensionState.NotifyIntraAppResizeAction());
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
                await ReRenderFuncAsync.Invoke().ConfigureAwait(false);
            }
        }
    }

    private void SubscribeToDragEvent(
        Func<(MouseEventArgs firstMouseEventArgs, MouseEventArgs secondMouseEventArgs), Task> dragEventHandler)
    {
        _dragEventHandler = dragEventHandler;

        Dispatcher.Dispatch(new DragState.WithAction(inState => inState with
        {
            ShouldDisplay = true,
            MouseEventArgs = null,
        }));
    }

    private async Task DragEventHandlerResizeHandleAsync(
        (MouseEventArgs firstMouseEventArgs, MouseEventArgs secondMouseEventArgs) mouseEventArgsTuple)
    {
        ResizeHelper.ResizeWest(
            LeftElementDimensions,
            mouseEventArgsTuple.firstMouseEventArgs,
            mouseEventArgsTuple.secondMouseEventArgs);

        ResizeHelper.ResizeEast(
            RightElementDimensions,
            mouseEventArgsTuple.firstMouseEventArgs,
            mouseEventArgsTuple.secondMouseEventArgs);

        await InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        DragStateWrap.StateChanged -= DragStateWrapOnStateChanged;
    }
}