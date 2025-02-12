namespace Luthetus.Ide.RazorLib.CommandBars.Models;

public class CommandBarService : ICommandBarService
{
	private CommandBarState _commandBarState = new();

	public event Action? CommandBarStateChanged;
	
	public CommandBarState GetCommandBarState() => _commandBarState;
}
