using Microsoft.AspNetCore.Components.Web;
using Luthetus.Common.RazorLib.Dynamics.Models;

namespace Luthetus.Common.RazorLib.Drags.Models;

public record struct DragState(
    bool ShouldDisplay,
    MouseEventArgs? MouseEventArgs,
	IDrag? Drag)
{
    public DragState() : this (false, null, null)
    {
        
    }
}