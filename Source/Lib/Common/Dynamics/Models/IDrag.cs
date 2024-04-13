using Luthetus.Common.RazorLib.Dimensions.Models;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Immutable;

namespace Luthetus.Common.RazorLib.Dynamics.Models;

public interface IDrag : IDynamicViewModel
{
    public ImmutableArray<IDropzone> DropzoneList { get; set; }

	public Type DragComponentType { get; }
	public Dictionary<string, object?>? DragComponentParameterMap { get; }
	public ElementDimensions DragElementDimensions { get; set; }
	public string? DragCssClass { get; set; }
	public string? DragCssStyle { get; set; }

	public Task OnDragStartAsync();
	public Task OnDragEndAsync(MouseEventArgs mouseEventArgs, IDropzone? dropzone);
}
