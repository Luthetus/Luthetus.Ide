using Fluxor;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Immutable;
using Luthetus.Common.RazorLib.Drags.Models;

namespace Luthetus.Common.RazorLib.Drags.Displays;

[FeatureState]
public partial record DragState(
    bool ShouldDisplay,
    MouseEventArgs? MouseEventArgs,
	IDraggableViewModel? DraggableViewModel,
	ImmutableArray<IDropzoneViewModel>? DropzoneViewModelList)
{
    public DragState() : this (false, null, null, null)
    {
        
    }
}