using System.Collections.Immutable;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
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
	
	private string _command = string.Empty;
	
	/// <summary>
	/// Accidentally hitting ArrowUp and losing the command you are typing out,
	/// then ArrowDown will restore what you were typing through this field.
	/// </summary>
	private string _cachedCommand = string.Empty;
	
	private int _indexHistory;
	
	private ITerminal? _previousTerminal = null;
	private ImmutableList<TerminalCommandRequest>? _terminalCommandRequestHistory;
	
	private ViewModelDisplayOptions _textEditorViewModelDisplayOptions = new()
	{
		HeaderComponentType = null,
		FooterComponentType = null,
		IncludeGutterComponent = false,
		ContextRecord = ContextFacts.TerminalContext,
	};
	
	private string CommandUiInputBinding
	{
		get => _command;
		set
		{
			_command = value;
			
			// If not browsing history, cache what the user is typing
			//
			// This means that while the '_cachedCommand' protects against
			// accidental pressing of the ArrowUp key,
			//
			// If one is modifying a command that was populated into the
			// input element by browsing history,
			// then until the enter key is pressed to stop browsing history,
			// nothing will be cached and an accidental ArrowUp here could lose information.
			//
			// But, the priority for protecting against lost information by accidental ArrowUp
			// is on an original newly being typed command, not the history.
			if (_terminalCommandRequestHistory is null)
				_cachedCommand = value;
		}
	}
	
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
	
	private TerminalCommandRequest? GetTerminalCommandRequestAtIndexHistory(
		int indexLocal,
		ImmutableList<TerminalCommandRequest> historyLocal)
	{
		if (indexLocal < historyLocal.Count)
			return historyLocal[indexLocal];
			
		return null;
	}
	
	private void HandleOnKeyDown(KeyboardEventArgs keyboardEventArgs)
	{
		if (keyboardEventArgs.Code == "Enter")
		{
			var commandLocal = _command;
			_terminalCommandRequestHistory = null;
			_indexHistory = 0;
		
			Terminal.EnqueueCommand(new TerminalCommandRequest(
				commandText: commandLocal,
				workingDirectory: null));
		}
		else if (keyboardEventArgs.Key == "ArrowUp")
		{
			if (_terminalCommandRequestHistory is null)
			{
				_terminalCommandRequestHistory = Terminal.TerminalInteractive.GetTerminalCommandRequestHistory();
				_indexHistory = 0;
				
				var commandAtIndex = GetTerminalCommandRequestAtIndexHistory(
					_indexHistory,
					_terminalCommandRequestHistory);
					
				if (commandAtIndex is not null)
					_command = commandAtIndex.CommandText;
			}
			else
			{
				if (_indexHistory < _terminalCommandRequestHistory.Count - 1)
					_indexHistory++;
					
				var commandAtIndex = GetTerminalCommandRequestAtIndexHistory(
					_indexHistory,
					_terminalCommandRequestHistory);
					
				if (commandAtIndex is not null)
					_command = commandAtIndex.CommandText;
			}
		}
		else if (keyboardEventArgs.Key == "ArrowDown")
		{
			if (_terminalCommandRequestHistory is not null)
			{
				if (_indexHistory == 0)
				{
					_command = _cachedCommand;
					_terminalCommandRequestHistory = null;
				}
				else
				{
					if (_indexHistory < 0)
						_indexHistory = 0;
					else
						_indexHistory--;
						
					var commandAtIndex = GetTerminalCommandRequestAtIndexHistory(
						_indexHistory,
						_terminalCommandRequestHistory);
						
					if (commandAtIndex is not null)
						_command = commandAtIndex.CommandText;
				}
			}
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
						return ValueTask.CompletedTask;
					
					var modelModifier = editContext.GetModelModifier(terminalOutputFormatterExpand.TextEditorModelResourceUri);
					var viewModelModifier = editContext.GetViewModelModifier(terminalOutputFormatterExpand.TextEditorViewModelKey);
					var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
					var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

					if (modelModifier is null || viewModelModifier is null || !cursorModifierBag.ConstructorWasInvoked || primaryCursorModifier is null)
						return ValueTask.CompletedTask;

					var localTerminal = Terminal;

					var outputFormatted = (TerminalOutputFormattedTextEditor)localTerminal.TerminalOutput
						.GetOutputFormatted(nameof(TerminalOutputFormatterExpand));
					
					modelModifier.SetContent(outputFormatted.Text);
					
					var lineIndexOriginal = primaryCursorModifier.LineIndex;
					var columnIndexOriginal = primaryCursorModifier.ColumnIndex;
					
					// Move Cursor, try to preserve the current cursor position.
					{
						if (primaryCursorModifier.LineIndex > modelModifier.LineCount - 1)
							primaryCursorModifier.LineIndex = modelModifier.LineCount - 1;
						
						var lineInformation = modelModifier.GetLineInformation(primaryCursorModifier.LineIndex);
						
						if (primaryCursorModifier.ColumnIndex > lineInformation.LastValidColumnIndex)
							primaryCursorModifier.SetColumnIndexAndPreferred(lineInformation.LastValidColumnIndex);
							
						if (lineIndexOriginal != primaryCursorModifier.LineIndex ||
							columnIndexOriginal != primaryCursorModifier.ColumnIndex)
						{
							viewModelModifier.ViewModel.UnsafeState.ShouldRevealCursor = true;
						}
					}
					
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
					
					return ValueTask.CompletedTask;
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