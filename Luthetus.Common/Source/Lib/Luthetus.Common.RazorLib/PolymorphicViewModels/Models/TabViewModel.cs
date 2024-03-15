using Microsoft.AspNetCore.Components.Web;

namespace Luthetus.Common.RazorLib.PolymorphicViewModels.Models;

public interface ITabViewModel
{
	public bool IsActive { get; }

	public string GetDynamicCss();
    public Task OnClickAsync(MouseEventArgs mouseEventArgs);
	public Task CloseAsync();
}

public class TabViewModel : ITabViewModel
{
	public bool IsActive { get; init; }

	public string GetDynamicCss();
    public Task OnClickAsync(MouseEventArgs mouseEventArgs);
	public Task CloseAsync();
}
