namespace Luthetus.Common.RazorLib.PolymorphicViewModels.Models;

public interface IDrag
{
	public Task OnDragStartAsync(MouseEventArgs mouseEventArgs);
	public Task OnDragEndAsync(MouseEventArgs mouseEventArgs, IDropzoneViewModel? dropzone);
}
