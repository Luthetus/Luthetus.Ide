using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using System.Collections.Immutable;
using Microsoft.AspNetCore.Components.Web;

namespace Luthetus.Common.RazorLib.Drags.Models;

public record DraggableViewModel : IDraggableViewModel
{
	public Key<IDraggableViewModel> Key { get; init; }
	public Type RendererType { get; init; }
	public Dictionary<string, object?> ParameterMap { get; init; }
	public ElementDimensions ElementDimensions { get; init; }

	public virtual Task OnDragStopAsync(MouseEventArgs mouseEventArgs, IDropzoneViewModel? dropzone)
	{
		throw new NotImplementedException();
	}
}
