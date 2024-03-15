namespace Luthetus.Common.RazorLib.Tabs.Models;

public interface ITabViewModel
{
	public Key<ITabViewModel> Key { get; }
	public bool IsActive { get; }

	public string GetDynamicCss();
    public Task OnClickAsync(MouseEventArgs mouseEventArgs);
	public Task CloseAsync();
}
