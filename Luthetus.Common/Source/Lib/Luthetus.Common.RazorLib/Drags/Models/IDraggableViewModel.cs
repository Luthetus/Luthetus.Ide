using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.PolymorphicViewModels.Models;
using System.Collections.Immutable;
using Microsoft.AspNetCore.Components.Web;

namespace Luthetus.Common.RazorLib.Drags.Models;

public interface IDraggableViewModel
{
	public IPolymorphicViewModel? PolymorphicViewModel { get; }
	public Key<IDraggableViewModel> Key { get; }
	public Type RendererType { get; }
	public Dictionary<string, object?> ParameterMap { get; }
	public ElementDimensions ElementDimensions { get; }
	public ImmutableArray<IDropzoneViewModel> DropzoneViewModelList { get; }

	public Task OnDragStopAsync(MouseEventArgs mouseEventArgs, IDropzoneViewModel? dropzone);
	public Task<ImmutableArray<IDropzoneViewModel>> GetDropzonesAsync();
}
