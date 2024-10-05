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

    private ThrottleOptimized<MouseEvent> _throttle;
    
    public struct MouseEvent
    {
    	public MouseEvent(bool isOnMouseMove, MouseEventArgs mouseEventArgs)
    	{
    		IsOnMouseMove = isOnMouseMove;
    		MouseEventArgs = mouseEventArgs;
    	}
    
    	public bool IsOnMouseMove { get; }
    	public MouseEventArgs MouseEventArgs { get; }
    }

    private IDropzone? _onMouseOverDropzone = null;
    
    protected override void OnInitialized()
    {
    	_throttle = new(ThrottleFacts.TwentyFour_Frames_Per_Second, async (args, _) =>
	    {
	    	if (args.IsOnMouseMove)
	    	{
	    		if ((args.MouseEventArgs.Buttons & 1) != 1)
	                DispatchClearDragStateAction();
	            else
	                Dispatcher.Dispatch(new DragState.ShouldDisplayAndMouseEventArgsSetAction(true, args.MouseEventArgs));
	
	            return;
	    	}
	    	else
	    	{
	    		var dragState = DragStateWrap.Value;
				var localOnMouseOverDropzone = _onMouseOverDropzone;
	    	
	    		DispatchClearDragStateAction();
	
	            var draggableViewModel = dragState.Drag;
	            if (draggableViewModel is not null)
	            {
	                await draggableViewModel
	                    .OnDragEndAsync(args.MouseEventArgs, localOnMouseOverDropzone)
	                    .ConfigureAwait(false);
	            }
	    	}
	    });
    	
    	base.OnInitialized();
    }

    private void DispatchClearDragStateAction()
    {
		_onMouseOverDropzone = null;
		
        Dispatcher.Dispatch(new DragState.ShouldDisplayAndMouseEventArgsAndDragSetAction(
        	false,
            null,
			null));
    }

    private void DispatchSetDragStateActionOnMouseMove(MouseEventArgs mouseEventArgs)
    {
        _throttle.Run(new(isOnMouseMove: true, mouseEventArgs));
    }

    private void DispatchSetDragStateActionOnMouseUp(MouseEventArgs mouseEventArgs)
    {
        _throttle.Run(new(isOnMouseMove: false, mouseEventArgs));
    }

	private string GetIsActiveCssClass(IDropzone dropzone)
	{
		var onMouseOverDropzoneKey = _onMouseOverDropzone?.DropzoneKey ?? Key<IDropzone>.Empty;

		return onMouseOverDropzoneKey == dropzone.DropzoneKey
            ? "luth_active"
			: string.Empty;
	}
}