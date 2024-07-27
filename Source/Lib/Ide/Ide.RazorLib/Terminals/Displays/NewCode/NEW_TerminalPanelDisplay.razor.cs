using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.Terminals.Models.NewCode;

namespace Luthetus.Ide.RazorLib.Terminals.Displays.NewCode;

public partial class NEW_TerminalPanelDisplay : ComponentBase, IDisposable
{
	[Inject]
	private IBackgroundTaskService BackgroundTaskService { get; set; } = null!;

	private NEW_Terminal? _terminal;
	private string _textArea = "abc123";
	
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
	
	public void Dispose()
	{
		_terminal?.Dispose();
	}
}