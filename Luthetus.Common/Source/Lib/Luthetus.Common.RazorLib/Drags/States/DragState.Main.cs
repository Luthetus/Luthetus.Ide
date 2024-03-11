using Luthetus.Common.RazorLib.PolymorphicUis.Models;
using Fluxor;
using Microsoft.AspNetCore.Components.Web;

namespace Luthetus.Common.RazorLib.Drags.Displays;

[FeatureState]
public partial record DragState(
    bool ShouldDisplay,
    MouseEventArgs? MouseEventArgs,
	IPolymorphicDraggable? PolymorphicDraggable)
{
    public DragState() : this (false, null, null)
    {
        
    }
}