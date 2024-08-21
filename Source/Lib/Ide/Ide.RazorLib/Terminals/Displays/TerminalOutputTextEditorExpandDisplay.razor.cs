using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
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
	
	[Parameter, EditorRequired]
	public ITerminal Terminal { get; set; } = null!;

	private readonly Throttle _throttle = new Throttle(TimeSpan.FromMilliseconds(700));
	
	private string _command;
	private ITerminal? _previousTerminal = null;
	
	private ViewModelDisplayOptions _textEditorViewModelDisplayOptions = new()
	{
		IncludeHeaderHelperComponent = false,
		IncludeFooterHelperComponent = false,
		IncludeGutterComponent = false,
		ContextRecord = ContextFacts.TerminalContext,
	};
	
	protected override void OnParametersSet()
	{
		var nextTerminal = Terminal;
		
		if (_previousTerminal is null ||
		    _previousTerminal.Key != nextTerminal.Key)
		{
			if (_previousTerminal is not null)
			{
				_previousTerminal.TerminalInteractive.WorkingDirectoryChanged -= OnWorkingDirectoryChanged;
				_previousTerminal.TerminalOutput.OnWriteOutput -= OnWriteOutput;
			}
			
			if (nextTerminal is not null)
			{
				nextTerminal.TerminalInteractive.WorkingDirectoryChanged += OnWorkingDirectoryChanged;
				nextTerminal.TerminalOutput.OnWriteOutput += OnWriteOutput;
			}
			
			// TODO: Is it possible for the Dispose() method to be invoked prior to...
			//       ...OnParametersSet() finishing?
			//       |
			//       It is being presumed that 'Dispose()' will not fire until 'OnParametersSet()'
			//       finishes. But, this should be proven to be the case.
			_previousTerminal = nextTerminal;
			
			// The name of the method 'OnWriteOutput()' is awkward.
			// The invocation here is to reload the text since the terminal changed.
			OnWriteOutput();
		}
	
		
		base.OnParametersSet();
	}
	
	private void HandleOnKeyDown(KeyboardEventArgs keyboardEventArgs)
	{
		if (keyboardEventArgs.Code == "Enter")
		{
			Terminal.EnqueueCommand(new TerminalCommandRequest(
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
					var formatter = Terminal.TerminalOutput.OutputFormatterList.FirstOrDefault(
						x => x.Name == nameof(TerminalOutputFormatterExpand));
						
					if (formatter is not TerminalOutputFormatterExpand terminalOutputFormatterExpand)
						return Task.CompletedTask;
					
					var modelModifier = editContext.GetModelModifier(terminalOutputFormatterExpand.TextEditorModelResourceUri);
					var viewModelModifier = editContext.GetViewModelModifier(terminalOutputFormatterExpand.TextEditorViewModelKey);
					var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
					var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

					if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
						return Task.CompletedTask;

					var localTerminal = Terminal;

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
		var localPreviousTerminal = _previousTerminal;
	
		localPreviousTerminal.TerminalInteractive.WorkingDirectoryChanged -= OnWorkingDirectoryChanged;
		localPreviousTerminal.TerminalOutput.OnWriteOutput -= OnWriteOutput;
	}
}