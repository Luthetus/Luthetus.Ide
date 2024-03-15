using Microsoft.AspNetCore.Components.Web;

namespace Luthetus.Common.RazorLib.Tabs.Models;

public class TabViewModel : ITabViewModel
{
	public Key<ITabViewModel> Key { get; }
	public bool IsActive { get; init; }

	public string GetDynamicCss();
    public Task OnClickAsync(MouseEventArgs mouseEventArgs);
	public Task CloseAsync();
}
