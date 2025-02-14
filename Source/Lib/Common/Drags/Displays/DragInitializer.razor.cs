using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;

namespace Luthetus.Common.RazorLib.Drags.Displays;

public partial class DragInitializer : ComponentBase, IDisposable
{
    [Inject]
    private LuthetusCommonApi CommonApi { get; set; } = null!;

    private string StyleCss => CommonApi.DragApi.GetDragState().ShouldDisplay
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
		CommonApi.DragApi.DragStateChanged += OnDragStateChanged;
    
    	_throttle = new(ThrottleFacts.TwentyFour_Frames_Per_Second, async (args, _) =>
	    {
	    	if (args.IsOnMouseMove)
	    	{
	    		if ((args.MouseEventArgs.Buttons & 1) != 1)
	                DispatchClearDragStateAction();
	            else
	                CommonApi.DragApi.ReduceShouldDisplayAndMouseEventArgsSetAction(true, args.MouseEventArgs);
	
	            return;
	    	}
	    	else
	    	{
	    		var dragState = CommonApi.DragApi.GetDragState();
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
    
    private async void OnDragStateChanged()
    {
    	await InvokeAsync(StateHasChanged);
    }

    private void DispatchClearDragStateAction()
    {
		_onMouseOverDropzone = null;
		
        CommonApi.DragApi.ReduceShouldDisplayAndMouseEventArgsAndDragSetAction(
        	false,
            null,
			null);
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
	
	public void Dispose()
	{
		CommonApi.DragApi.DragStateChanged -= OnDragStateChanged;
	}
}