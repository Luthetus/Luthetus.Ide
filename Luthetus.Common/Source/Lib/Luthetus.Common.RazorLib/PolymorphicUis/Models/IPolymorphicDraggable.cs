using Luthetus.Common.RazorLib.Dimensions.Models;
using System.Collections.Immutable;
using Microsoft.AspNetCore.Components.Web;

namespace Luthetus.Common.RazorLib.PolymorphicUis.Models;

public interface IPolymorphicDraggable : IPolymorphicUiRecord
{
	public Type DraggableRendererType { get; }
	public Dictionary<string, object?> DraggableParameterMap { get; }
	public ElementDimensions DraggableElementDimensions { get; }
	public ImmutableArray<IPolymorphicDropzone> DropzoneList { get; set; }

	public Task OnDragStopAsync(MouseEventArgs mouseEventArgs, IPolymorphicDropzone? dropzone);
	public Task<ImmutableArray<IPolymorphicDropzone>> GetDropzonesAsync();
}
