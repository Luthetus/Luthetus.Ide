using Luthetus.Common.RazorLib.PolymorphicViewModels.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Microsoft.AspNetCore.Components.Web;

namespace Luthetus.Common.RazorLib.Tabs.Models;

public interface ITabViewModel
{
	public IPolymorphicViewModel? PolymorphicViewModel { get; }
	public Key<ITabViewModel> Key { get; }
	public string Title { get; }

	public bool GetIsActive();
	public string GetDynamicCss();
    public Task OnClickAsync(MouseEventArgs mouseEventArgs);
	public Task CloseAsync();
}
