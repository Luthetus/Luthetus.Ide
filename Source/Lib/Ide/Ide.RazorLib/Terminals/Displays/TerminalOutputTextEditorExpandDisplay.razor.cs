using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.Terminals.States;

namespace Luthetus.Ide.RazorLib.Terminals.Displays;

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
	private IState<TerminalState> TerminalStateWrap { get; set; } = null!;
	[Inject]
	private IDispatcher Dispatcher { get; set; } = null!;

	private readonly Throttle _throttle = new Throttle(TimeSpan.FromMilliseconds(700));
	
	private ITerminal? _terminal => TerminalStateWrap.Value.GeneralTerminal;
	private string _command;
	
	protected override void OnInitialized()
	{
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
				nameof(TerminalOutput),
				editContext =>
				{
					var formatter = _terminal.TerminalOutput.OutputFormatterList.FirstOrDefault(
						x => x.Name == nameof(TerminalOutputFormatterExpand));
						
					if (formatter is not TerminalOutputFormatterExpand terminalOutputFormatterExpand)
						return Task.CompletedTask;
					
					var modelModifier = editContext.GetModelModifier(terminalOutputFormatterExpand.TextEditorModelResourceUri);
					var viewModelModifier = editContext.GetViewModelModifier(terminalOutputFormatterExpand.TextEditorViewModelKey);
					var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
					var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

					if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
						return Task.CompletedTask;

					var localTerminal = _terminal;

					var outputFormatted = (TerminalOutputFormattedTextEditor)localTerminal.TerminalOutput
						.GetOutputFormatted(nameof(TerminalOutputFormatterExpand));
					
					modelModifier.SetContent(outputFormatted.Text);
					
					primaryCursorModifier.LineIndex = 0;
					primaryCursorModifier.SetColumnIndexAndPreferred(0);
					
					var compilerServiceResource = modelModifier.CompilerService.GetCompilerServiceResourceFor(
						terminalOutputFormatterExpand.TextEditorModelResourceUri);

					if (compilerServiceResource is TerminalResource terminalResource)
					{					
						terminalResource.ManualDecorationTextSpanList.Clear();
						terminalResource.ManualDecorationTextSpanList.AddRange(
							outputFormatted.SymbolList.Select(x => x.TextSpan));
								
						terminalResource.ManualSymbolList.Clear();
						terminalResource.ManualSymbolList.AddRange(outputFormatted.SymbolList);

						editContext.TextEditorService.ModelApi.ApplyDecorationRange(
							editContext,
							modelModifier,
							terminalResource.GetTokenTextSpans());
							
							editContext.TextEditorService.ModelApi.ApplyDecorationRange(
							editContext,
							modelModifier,
							outputFormatted.TextSpanList);
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