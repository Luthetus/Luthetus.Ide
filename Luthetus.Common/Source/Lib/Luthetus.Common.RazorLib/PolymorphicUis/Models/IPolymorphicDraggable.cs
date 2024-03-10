using Luthetus.Common.RazorLib.Dimensions.Models;

namespace Luthetus.Common.RazorLib.PolymorphicUis.Models;

public interface IPolymorphicDraggable : IPolymorphicUiRecord
{
	public Type DraggableRendererType { get; }
	public Dictionary<string, object?> DraggableParameterMap { get; }
	public ElementDimensions DraggableElementDimensions { get; }

	public Task OnDragStopAsync();
}
