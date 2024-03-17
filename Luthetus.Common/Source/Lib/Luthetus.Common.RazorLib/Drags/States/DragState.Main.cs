using Fluxor;
using Microsoft.AspNetCore.Components.Web;
using Luthetus.Common.RazorLib.Dynamics.Models;

namespace Luthetus.Common.RazorLib.Drags.Displays;

[FeatureState]
public partial record DragState(
    bool ShouldDisplay,
    MouseEventArgs? MouseEventArgs,
	IDrag? Drag)
{
    public DragState() : this (false, null, null)
    {
        
    }
}