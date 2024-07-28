using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.Ide.RazorLib.Terminals.Models.NewCode;

namespace Luthetus.Ide.RazorLib.Terminals.Displays.NewCode;

public partial class TerminalOutputTextEditorExpandDisplay : ComponentBase, IDisposable
{
	[Inject]
	private IBackgroundTaskService BackgroundTaskService { get; set; } = null!;
	[Inject]
	private ICommonComponentRenderers CommonComponentRenderers { get; set; } = null!;
	[Inject]
	private ITextEditorService TextEditorService { get; set; } = null!;
	[Inject]
	private ICompilerServiceRegistry CompilerServiceRegistry { get; set; } = null!;
	[Inject]
	private IDispatcher Dispatcher { get; set; } = null!;

	private readonly Throttle _throttle = new Throttle(TimeSpan.FromMilliseconds(700));
	
	private NEW_Terminal? _terminal;
	private string _command;
	
	protected override void OnInitialized()
	{
		_terminal = new NEW_Terminal(
			"test?",
			terminal => new TerminalInteractive(terminal),
			terminal => new TerminalInputStringBuilder(terminal),
			terminal => new TerminalOutputTextEditorExpand(terminal, TextEditorService, CompilerServiceRegistry),
			BackgroundTaskService,
			CommonComponentRenderers,
			Dispatcher);
			
		_terminal.TerminalInteractive.WorkingDirectoryChanged += OnWorkingDirectoryChanged;
		_terminal.TerminalOutput.OnWriteOutput += OnWriteOutput;
			
		base.OnInitialized();
	}
	
    protected override void OnAfterRender(bool firstRender)
    {
		if (firstRender)
		{
            _throttle.Run(_ =>
            {
                var textEditorViewModel = TextEditorService.ViewModelApi.GetOrDefault(
                    TerminalOutputTextEditorExpand.TextEditorViewModelKey);

                if (textEditorViewModel is null)
                    return Task.CompletedTask;

                return Task.CompletedTask;
            });
        }

        base.OnAfterRender(firstRender);
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