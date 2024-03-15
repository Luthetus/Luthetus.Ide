using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using System.Collections.Immutable;
using Microsoft.AspNetCore.Components.Web;

namespace Luthetus.Common.RazorLib.Drags.Models;

public interface IDraggableViewModel
{
	public Key<IDraggableViewModel> Key { get; }
	public Type RendererType { get; }
	public Dictionary<string, object?> ParameterMap { get; }
	public ElementDimensions ElementDimensions { get; }

	public Task OnDragStopAsync(MouseEventArgs mouseEventArgs, IDropzoneViewModel? dropzone);
}
