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

public partial class TerminalOutputTextEditorAllDisplay : ComponentBase, IDisposable
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

	private readonly Throttle _throttle = new Throttle(TimeSpan.FromMilliseconds(1_000));
	
	private NEW_Terminal? _terminal;
	private string _command;
	
	protected override void OnInitialized()
	{
		_terminal = new NEW_Terminal(
			"test?",
			terminal => new TerminalInteractive(terminal),
			terminal => new TerminalInputStringBuilder(terminal),
			terminal => new TerminalOutputTextEditorAll(terminal, TextEditorService, CompilerServiceRegistry),
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
	
	private void OnWriteOutput()
	{
		_throttle.Run(_ =>
        {
        	TextEditorService.PostUnique(
				nameof(TerminalOutputTextEditorAllDisplay),
				editContext =>
				{
					var modelModifier = editContext.GetModelModifier(TerminalOutputTextEditorAll.TextEditorModelResourceUri);
					var viewModelModifier = editContext.GetViewModelModifier(TerminalOutputTextEditorAll.TextEditorViewModelKey);
					var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
					var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

					if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
						return Task.CompletedTask;

					var localTerminal = _terminal;
					
					modelModifier.SetContent(localTerminal.TerminalOutput.OutputRaw ?? string.Empty);
					
					primaryCursorModifier.LineIndex = 0;
					primaryCursorModifier.SetColumnIndexAndPreferred(0);
					
					viewModelModifier.ViewModel.UnsafeState.ShouldRevealCursor = true;
					return Task.CompletedTask;
				});
			return Task.CompletedTask;
        });
	}
	
	public void Dispose()
	{
		_terminal.TerminalInteractive.WorkingDirectoryChanged -= OnWorkingDirectoryChanged;
		_terminal.TerminalOutput.OnWriteOutput -= OnWriteOutput;
		_terminal?.Dispose();
	}
}