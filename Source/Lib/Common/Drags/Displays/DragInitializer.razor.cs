using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Reactives.Models;

namespace Luthetus.Common.RazorLib.Drags.Displays;

public partial class DragInitializer : FluxorComponent
{
    [Inject]
    private IState<DragState> DragStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    private string StyleCss => DragStateWrap.Value.ShouldDisplay
        ? string.Empty
        : "display: none;";

    public static ThrottleAsync Throttle = new(ThrottleAsync.Sixty_Frames_Per_Second);

    private IDropzone? _onMouseOverDropzone = null;

    private DragState.WithAction ConstructClearDragStateAction()
    {
		_onMouseOverDropzone = null;

        return new DragState.WithAction(inState => inState with
        {
            ShouldDisplay = false,
            MouseEventArgs = null,
			Drag = null,
        });
    }

    private async Task DispatchSetDragStateActionOnMouseMoveAsync(MouseEventArgs mouseEventArgs)
    {
        await Throttle.PushEvent(_ =>
        {
            if ((mouseEventArgs.Buttons & 1) != 1)
            {
                Dispatcher.Dispatch(ConstructClearDragStateAction());
            }
            else
            {
                Dispatcher.Dispatch(new DragState.WithAction(inState => inState with
                {
                    ShouldDisplay = true,
                    MouseEventArgs = mouseEventArgs,
                }));
            }

            return Task.CompletedTask;
        }).ConfigureAwait(false);
    }

    private async Task DispatchSetDragStateActionOnMouseUp(MouseEventArgs mouseEventArgs)
    {
		var dragState = DragStateWrap.Value;
		var localOnMouseOverDropzone = _onMouseOverDropzone;

        await Throttle.PushEvent(async _ =>
        {
            Dispatcher.Dispatch(ConstructClearDragStateAction());

            var draggableViewModel = dragState.Drag;
            if (draggableViewModel is not null)
            {
                await draggableViewModel
                    .OnDragEndAsync(mouseEventArgs, localOnMouseOverDropzone)
                    .ConfigureAwait(false);
            }
        }).ConfigureAwait(false);
    }

	private string GetIsActiveCssClass(IDropzone dropzone)
	{
		var onMouseOverDropzoneKey = _onMouseOverDropzone?.DropzoneKey ?? Key<IDropzone>.Empty;

		return onMouseOverDropzoneKey == dropzone.DropzoneKey
            ? "luth_active"
			: string.Empty;
	}
}