using Microsoft.AspNetCore.Components.Web;

namespace Luthetus.Common.RazorLib.Dynamics.Models;

public interface ITabGroup
{
	public bool GetIsActive(ITab tab);
	public string GetDynamicCss(ITab tab);
    public Task OnClickAsync(ITab tab, MouseEventArgs mouseEventArgs);
	public Task CloseAsync(ITab tab);
}
