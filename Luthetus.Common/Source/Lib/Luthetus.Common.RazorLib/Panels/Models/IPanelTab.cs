namespace Luthetus.Common.RazorLib.Panels.Models;

public interface IPanelTab : ITab
{
	public Key<Panel> Key { get; }
}
