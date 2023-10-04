using Fluxor;
using Microsoft.AspNetCore.Components.Web;

namespace Luthetus.Common.RazorLib.Drags.Displays;

[FeatureState]
public partial record DragState(
    bool ShouldDisplay,
    MouseEventArgs? MouseEventArgs)
{
    private DragState() : this (false, null)
    {
        
    }
}