using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.Terminals.Models.NewCode;

namespace Luthetus.Ide.RazorLib.Terminals.Displays.NewCode;

public partial class NEW_TerminalPanelDisplay : ComponentBase, IDisposable
{
	[Inject]
	private IBackgroundTaskService BackgroundTaskService { get; set; } = null!;

	private NEW_Terminal? _terminal;
	private string _textArea = "abc123";
	private string _command;
	
	protected override void OnInitialized()
	{
		_terminal = new NEW_Terminal(
			"test?",
			terminal => new TerminalInteractive(terminal),
			terminal => new TerminalInputStringBuilder(terminal),
			terminal => new TerminalOutputStringBuilder(terminal),
			BackgroundTaskService);
		base.OnInitialized();
	}
	
	private void HandleOnKeyDown(KeyboardEventArgs keyboardEventArgs)
	{
		if (keyboardEventArgs.Code == "Enter")
		{
			_terminal.EnqueueCommand(_command);
		}
	}
	
	public void Dispose()
	{
		_terminal?.Dispose();
	}
}