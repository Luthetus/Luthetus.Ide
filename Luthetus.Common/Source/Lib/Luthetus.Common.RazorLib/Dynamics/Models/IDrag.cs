using Microsoft.AspNetCore.Components.Web;
using System.Collections.Immutable;

namespace Luthetus.Common.RazorLib.Dynamics.Models;

public interface IDrag : IDynamicViewModel
{
    public ImmutableArray<IDropzone> DropzoneList { get; set; }

    public Task OnDragStartAsync(MouseEventArgs mouseEventArgs);
	public Task OnDragEndAsync(MouseEventArgs mouseEventArgs, IDropzone? dropzone);
    public Task<ImmutableArray<IDropzone>> GetDropzonesAsync();
}
