using Luthetus.Common.RazorLib.Tabs.Models;

namespace Luthetus.Common.RazorLib.Panels.Models;

public interface IPanelContainableTab : ITabViewModel
{
	/// <summary>
	/// The intent here, is that 'Panel.razor' component sets this 'PanelGroup' property,
	/// each time the 'Panel.razor' renders.
	/// </summary>
	public PanelGroup PanelGroup { get; set; }
	public Panel Panel { get; set; }
}
