using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.Ide.RazorLib.Terminals.Models;

namespace Luthetus.Ide.RazorLib.Terminals.Displays;

public partial class TerminalOutputTextEditorExpandDisplay : ComponentBase, IDisposable
{
	[Inject]
	private BackgroundTaskService BackgroundTaskService { get; set; } = null!;
	[Inject]
	private ICommonComponentRenderers CommonComponentRenderers { get; set; } = null!;
	[Inject]
	private TextEditorService TextEditorService { get; set; } = null!;
	[Inject]
	private ICompilerServiceRegistry CompilerServiceRegistry { get; set; } = null!;
	[Inject]
	private ITerminalService TerminalService { get; set; } = null!;
	
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
	private List<TerminalCommandRequest>? _terminalCommandRequestHistory;
	
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
		List<TerminalCommandRequest> historyLocal)
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
        	TextEditorService.WorkerArbitrary.PostUnique(
				nameof(TerminalOutput),
				editContext =>
				{
					var formatter = Terminal.TerminalOutput.OutputFormatterList.FirstOrDefault(
						x => x.Name == nameof(TerminalOutputFormatterExpand));
						
					if (formatter is not TerminalOutputFormatterExpand terminalOutputFormatterExpand)
						return ValueTask.CompletedTask;
					
					var modelModifier = editContext.GetModelModifier(terminalOutputFormatterExpand.TextEditorModelResourceUri);
					var viewModelModifier = editContext.GetViewModelModifier(terminalOutputFormatterExpand.TextEditorViewModelKey);

					if (modelModifier is null || viewModelModifier is null)
						return ValueTask.CompletedTask;

					var localTerminal = Terminal;
					
					var showingFinalLine = false;
    
				    if (viewModelModifier.VirtualizationResult.EntryList.Count > 0)
				    {
				        var last = viewModelModifier.VirtualizationResult.EntryList.Last();
				        if (last.LineIndex == modelModifier.LineCount - 1)
				            showingFinalLine = true;
				    }

					var outputFormatted = (TerminalOutputFormattedTextEditor)localTerminal.TerminalOutput
						.GetOutputFormatted(nameof(TerminalOutputFormatterExpand));
					
					modelModifier.SetContent(outputFormatted.Text);
					
					var lineIndexOriginal = viewModelModifier.LineIndex;
					var columnIndexOriginal = viewModelModifier.ColumnIndex;
					
					// Move Cursor, try to preserve the current cursor position.
					{
						if (viewModelModifier.LineIndex > modelModifier.LineCount - 1)
							viewModelModifier.LineIndex = modelModifier.LineCount - 1;
						
						var lineInformation = modelModifier.GetLineInformation(viewModelModifier.LineIndex);
						
						if (viewModelModifier.ColumnIndex > lineInformation.LastValidColumnIndex)
							viewModelModifier.SetColumnIndexAndPreferred(lineInformation.LastValidColumnIndex);
					}
					
					if (showingFinalLine)
				    {
				    	// Console.WriteLine($"showingFinalLine: {showingFinalLine}");
				    
				        var lineInformation = modelModifier.GetLineInformation(modelModifier.LineCount - 1);
				        
				        var originalScrollLeft = viewModelModifier.ScrollLeft;
				        
				        var textSpan = new TextEditorTextSpan(
				            startInclusiveIndex: lineInformation.Position_StartInclusiveIndex,
				            endExclusiveIndex: lineInformation.Position_StartInclusiveIndex + 1,
				            decorationByte: 0,
				            modelModifier.PersistentState.ResourceUri,
				            sourceText: string.Empty,
				            getTextPrecalculatedResult: string.Empty);
				        
				        TextEditorService.ViewModelApi.ScrollIntoView(
				            editContext,
				            modelModifier,
				            viewModelModifier,
				            textSpan);
				        
				        //viewModelModifier.ScrollbarDimensions = viewModelModifier.ScrollbarDimensions.WithMutateScrollTop(
				        //    (int)viewModelModifier.CharAndLineMeasurements.LineHeight,
				        //    viewModelModifier.TextEditorDimensions);
				        
				        viewModelModifier.SetScrollLeft(
				            (int)originalScrollLeft,
				            viewModelModifier.TextEditorDimensions);
				    }
				    else if (lineIndexOriginal != viewModelModifier.LineIndex ||
						     columnIndexOriginal != viewModelModifier.ColumnIndex)
					{
						viewModelModifier.PersistentState.ShouldRevealCursor = true;
					}  
					
					var compilerServiceResource = modelModifier.PersistentState.CompilerService.GetResource(
						terminalOutputFormatterExpand.TextEditorModelResourceUri);

					if (compilerServiceResource is TerminalResource terminalResource)
					{					
						terminalResource.CompilationUnit.ManualDecorationTextSpanList.Clear();
						terminalResource.CompilationUnit.ManualDecorationTextSpanList.AddRange(
							outputFormatted.SymbolList.Select(x => x.TextSpan));
								
						terminalResource.CompilationUnit.ManualSymbolList.Clear();
						terminalResource.CompilationUnit.ManualSymbolList.AddRange(outputFormatted.SymbolList);

						editContext.TextEditorService.ModelApi.ApplySyntaxHighlighting(
							editContext,
							modelModifier);
							
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