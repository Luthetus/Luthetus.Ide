using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.Terminals.Models.NewCode;

namespace Luthetus.Ide.RazorLib.Terminals.Displays.NewCode;

public partial class NEW_TerminalPanelDisplay : ComponentBase, IDisposable
{
	[Inject]
	private IBackgroundTaskService BackgroundTaskService { get; set; } = null!;
	[Inject]
	private ICommonComponentRenderers CommonComponentRenderers { get; set; } = null!;
	[Inject]
	private IDispatcher Dispatcher { get; set; } = null!;

	private NEW_Terminal? _terminal;
	private string _command;
	private string _terminalOutputSelection = "TerminalOutputTextEditorExpand";
	
	protected override void OnInitialized()
	{
		_terminal = new NEW_Terminal(
			"test?",
			terminal => new TerminalInteractive(terminal),
			terminal => new TerminalInputStringBuilder(terminal),
			terminal => new TerminalOutputStringBuilderAll(terminal),
			BackgroundTaskService,
			CommonComponentRenderers,
			Dispatcher);
			
		_terminal.TerminalInteractive.WorkingDirectoryChanged += OnWorkingDirectoryChanged;
		_terminal.TerminalOutput.OnWriteOutput += OnWriteOutput;
			
		base.OnInitialized();
	}
	
	private void HandleOnKeyDown(KeyboardEventArgs keyboardEventArgs)
	{
		if (keyboardEventArgs.Code == "Enter")
		{
			_terminal.EnqueueCommand(new TerminalCommandRequest(
				commandText: _command,
				workingDirectory: null));
		}
	}
	
	private async void OnWorkingDirectoryChanged()
	{
		await InvokeAsync(StateHasChanged);
	}
	
	private async void OnWriteOutput()
	{
		await InvokeAsync(StateHasChanged);
	}
	
	public void Dispose()
	{
		_terminal.TerminalInteractive.WorkingDirectoryChanged -= OnWorkingDirectoryChanged;
		_terminal.TerminalOutput.OnWriteOutput -= OnWriteOutput;
		_terminal?.Dispose();
	}
}