using Luthetus.Common.RazorLib.Dimensions.Models;
using System.Collections.Immutable;

namespace Luthetus.Common.RazorLib.PolymorphicUis.Models;

public interface IPolymorphicDraggable : IPolymorphicUiRecord
{
	public Type DraggableRendererType { get; }
	public Dictionary<string, object?> DraggableParameterMap { get; }
	public ElementDimensions DraggableElementDimensions { get; }
	public ImmutableArray<IPolymorphicDropzone> DropzoneList { get; set; }

	public Task OnDragStopAsync(IPolymorphicDropzone? dropzone);
	public Task<ImmutableArray<IPolymorphicDropzone>> GetDropzonesAsync();
}
