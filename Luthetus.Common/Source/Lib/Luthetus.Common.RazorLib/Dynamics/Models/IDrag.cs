namespace Luthetus.Common.RazorLib.Dynamics.Models;

public interface IDrag
{
	public Task OnDragStartAsync(MouseEventArgs mouseEventArgs);
	public Task OnDragEndAsync(MouseEventArgs mouseEventArgs, IDropzoneViewModel? dropzone);
}
