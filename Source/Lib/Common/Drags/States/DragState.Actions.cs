using Microsoft.AspNetCore.Components.Web;
using Luthetus.Common.RazorLib.Dynamics.Models;

namespace Luthetus.Common.RazorLib.Drags.Displays;

public partial record DragState
{
    public struct ShouldDisplayAndMouseEventArgsSetAction
    {
    	public ShouldDisplayAndMouseEventArgsSetAction(bool shouldDisplay, MouseEventArgs? mouseEventArgs)
    	{
    		ShouldDisplay = shouldDisplay;
    		MouseEventArgs = mouseEventArgs;
    	}
    	
    	public bool ShouldDisplay { get; }
    	public MouseEventArgs? MouseEventArgs { get; }
    }
    
    public struct ShouldDisplayAndMouseEventArgsAndDragSetAction
    {
    	public ShouldDisplayAndMouseEventArgsAndDragSetAction(
    		bool shouldDisplay,
    		MouseEventArgs? mouseEventArgs,
    		IDrag? drag)
    	{
    		ShouldDisplay = shouldDisplay;
    		MouseEventArgs = mouseEventArgs;
    		Drag = drag;
    	}
    	
    	public bool ShouldDisplay { get; }
    	public MouseEventArgs? MouseEventArgs { get; }
    	public IDrag? Drag { get; }
    }
}