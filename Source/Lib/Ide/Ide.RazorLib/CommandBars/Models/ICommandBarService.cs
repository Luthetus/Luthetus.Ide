namespace Luthetus.Ide.RazorLib.CommandBars.Models;

public interface ICommandBarService
{
	public event Action? CommandBarStateChanged;
	
	public CommandBarState GetCommandBarState();
}
