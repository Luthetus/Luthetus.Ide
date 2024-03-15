using Luthetus.Common.RazorLib.Dimensions.Models;
using System.Collections.Immutable;
using Microsoft.AspNetCore.Components.Web;

namespace Luthetus.Common.RazorLib.PolymorphicViewModels.Models;

public interface IDraggableViewModel
{
	public Type RendererType { get; }
	public Dictionary<string, object?> ParameterMap { get; }
	public ElementDimensions ElementDimensions { get; }
	public ImmutableArray<IPolymorphicDropzone> DropzoneList { get; }

	public Task OnDragStopAsync(MouseEventArgs mouseEventArgs, IPolymorphicDropzone? dropzone);
	public Task<ImmutableArray<IPolymorphicDropzone>> GetDropzonesAsync();
}

public record DraggableViewModel : IDraggableViewModel
{
	public Type RendererType { get; init; }
	public Dictionary<string, object?> ParameterMap { get; init; }
	public ElementDimensions ElementDimensions { get; init; }
	public ImmutableArray<IPolymorphicDropzone> DropzoneList { get; init; }

	public virtual Task OnDragStopAsync(MouseEventArgs mouseEventArgs, IPolymorphicDropzone? dropzone)
	{
		throw new NotImplementedException();
	}

	public virtual Task<ImmutableArray<IPolymorphicDropzone>> GetDropzonesAsync()
	{
		throw new NotImplementedException();
	}
}
