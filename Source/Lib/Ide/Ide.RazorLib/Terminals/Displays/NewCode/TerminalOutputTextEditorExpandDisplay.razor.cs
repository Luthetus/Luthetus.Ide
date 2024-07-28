using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.Ide.RazorLib.Terminals.Models;
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
	
	private void OnWriteOutput()
	{
		_throttle.Run(_ =>
        {
        	TextEditorService.PostUnique(
				nameof(TerminalOutputTextEditorExpandDisplay),
				editContext =>
				{
					var modelModifier = editContext.GetModelModifier(TerminalOutputTextEditorExpand.TextEditorModelResourceUri);
					var viewModelModifier = editContext.GetViewModelModifier(TerminalOutputTextEditorExpand.TextEditorViewModelKey);
					var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
					var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

					if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
						return Task.CompletedTask;

					var localTerminal = _terminal;
					
					modelModifier.SetContent(localTerminal.TerminalOutput.OutputRaw ?? string.Empty);
					
					primaryCursorModifier.LineIndex = 0;
					primaryCursorModifier.SetColumnIndexAndPreferred(0);
					
					var compilerServiceResource = modelModifier.CompilerService.GetCompilerServiceResourceFor(
						TerminalOutputTextEditorExpand.TextEditorModelResourceUri);

					if (compilerServiceResource is TerminalResource terminalResource)
					{
						terminalResource.ManualDecorationTextSpanList.Clear();
						terminalResource.ManualDecorationTextSpanList.AddRange(
							((TerminalOutputTextEditorExpand)localTerminal.TerminalOutput).TextEditorSymbolList.Select(
								x => x.TextSpan));
								
						terminalResource.ManualSymbolList.Clear();
						terminalResource.ManualSymbolList.AddRange(
							((TerminalOutputTextEditorExpand)localTerminal.TerminalOutput).TextEditorSymbolList);

						editContext.TextEditorService.ModelApi.ApplyDecorationRange(
							editContext,
							modelModifier,
							terminalResource.GetTokenTextSpans());
					}
					
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