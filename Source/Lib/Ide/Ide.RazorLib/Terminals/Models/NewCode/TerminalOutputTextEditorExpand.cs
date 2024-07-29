using System.Collections.Immutable;
using System.Text;
using Microsoft.AspNetCore.Components.Web;
using Fluxor;
using CliWrap.EventStream;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Dialogs.States;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.CompilerServices.Facts;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Symbols;
using Luthetus.Ide.RazorLib.Terminals.Displays.NewCode;

namespace Luthetus.Ide.RazorLib.Terminals.Models.NewCode;

public class TerminalOutputTextEditorExpand : ITerminalOutput
{
    public static ResourceUri TextEditorModelResourceUri { get; } = new(
        ResourceUriFacts.Terminal_ReservedResourceUri_Prefix + nameof(TerminalOutputTextEditorExpand));

    public static Key<TextEditorViewModel> TextEditorViewModelKey { get; } = Key<TextEditorViewModel>.NewKey();

	// TODO: This property is horrific to look at its defined over 3 lines? Don't do this?
	private readonly 
		List<(TerminalCommandParsed terminalCommandParsed, StringBuilder outputBuilder)>
		_commandOutputList = new(); 
		
	private readonly StringBuilder _inputBuilder = new();
		
	private readonly object _commandOutputListLock = new();

    private readonly ITerminal _terminal;
	private readonly ITextEditorService _textEditorService;
	private readonly ICompilerServiceRegistry _compilerServiceRegistry;
	private readonly IDispatcher _dispatcher;
	
	private readonly List<ITextEditorSymbol> _textEditorSymbolList = new();
	private readonly List<TextEditorTextSpan> _textEditorTextSpanList = new();

	public TerminalOutputTextEditorExpand(
		ITerminal terminal,
		ITextEditorService textEditorService,
		ICompilerServiceRegistry compilerServiceRegistry,
		IDispatcher dispatcher)
	{
		_terminal = terminal;
		_terminal.TerminalInteractive.WorkingDirectoryChanged += OnWorkingDirectoryChanged;
		
		_textEditorService = textEditorService;
		_compilerServiceRegistry = compilerServiceRegistry;
		_dispatcher = dispatcher;
		
		CreateTextEditor();
	}
	
	private string _output = string.Empty;
	
	public string OutputRaw
	{
		get => _output;
		private set
		{
			_output = value;
		}
	}

	public List<TextEditorTextSpan> TextEditorTextSpanList => _textEditorTextSpanList;	
	public List<ITextEditorSymbol> TextEditorSymbolList => _textEditorSymbolList;
	
	public StringBuilder OutputBuilder { get; } = new();
	
	public event Action? OnWriteOutput;
	
	public ImmutableList<(TerminalCommandParsed terminalCommandParsed, StringBuilder outputBuilder)> GetCommandOutputList()
	{
		lock (_commandOutputListLock)
		{
			return _commandOutputList.ToImmutableList();
		}
	}

	public void OnWorkingDirectoryChanged()
	{
	}
	
	public void OnHandleCommandStarting()
	{
	}
	
	public void WriteOutput(TerminalCommandParsed terminalCommandParsed, CommandEvent commandEvent)
	{
		var output = (string?)null;

		switch (commandEvent)
		{
			case StartedCommandEvent started:
				
				// Delete any output of the previous invocation.
				lock (_commandOutputListLock)
				{
					var indexPreviousOutput = _commandOutputList.FindIndex(x =>
						x.terminalCommandParsed.SourceTerminalCommandRequest.Key ==
							terminalCommandParsed.SourceTerminalCommandRequest.Key);
							
					if (indexPreviousOutput != -1)
						_commandOutputList.RemoveAt(indexPreviousOutput);
				}
				
				var workingDirectoryText = _terminal.TerminalInteractive.WorkingDirectory + "> ";
				
				var workingDirectoryTextSpan = new TextEditorTextSpan(
					_inputBuilder.Length,
			        _inputBuilder.Length + workingDirectoryText.Length,
			        (byte)TerminalDecorationKind.Keyword,
			        ResourceUri.Empty,
			        string.Empty,
			        workingDirectoryText);
			    _textEditorTextSpanList.Add(workingDirectoryTextSpan);
				
				_inputBuilder.Append(workingDirectoryText);
				
				var commandTextTextSpan = new TextEditorTextSpan(
					_inputBuilder.Length,
			        _inputBuilder.Length + terminalCommandParsed.SourceTerminalCommandRequest.CommandText.Length,
			        (byte)0,
			        ResourceUri.Empty,
			        string.Empty,
			        terminalCommandParsed.SourceTerminalCommandRequest.CommandText);
			        
				var commandTextSymbol = new OnClickSymbol(
					commandTextTextSpan,
					"View Output",
					() => OpenInEditor(terminalCommandParsed));
					
				_textEditorSymbolList.Add(commandTextSymbol);
				
				var targetFileNameTextSpan = new TextEditorTextSpan(
					_inputBuilder.Length,
			        _inputBuilder.Length + terminalCommandParsed.TargetFileName.Length,
			        (byte)TerminalDecorationKind.TargetFilePath,
			        ResourceUri.Empty,
			        string.Empty,
			        terminalCommandParsed.TargetFileName);
			    _textEditorTextSpanList.Add(targetFileNameTextSpan);
				
				_inputBuilder.Append($"{terminalCommandParsed.SourceTerminalCommandRequest.CommandText}\n");
				
				break;
			case StandardOutputCommandEvent stdOut:
				output = $"{stdOut.Text}\n";
				break;
			case StandardErrorCommandEvent stdErr:
				output = $"{stdErr.Text}\n";
				break;
			case ExitedCommandEvent exited:
				output = $"Process exited; Code: {exited.ExitCode}\n";
				break;
		}
		
		if (output is null)
		{
			lock (_commandOutputListLock)
			{
				OutputRaw = _inputBuilder.ToString();
			}
		}
		else
		{
			lock (_commandOutputListLock)
			{
				var indexPreviousOutput = _commandOutputList.FindIndex(x =>
					x.terminalCommandParsed.SourceTerminalCommandRequest.Key ==
						terminalCommandParsed.SourceTerminalCommandRequest.Key);
			
				if (indexPreviousOutput == -1)
				{
					_commandOutputList.Add(
						(terminalCommandParsed, new StringBuilder(output)));
				}
				else
				{
					var commandTuple = _commandOutputList[indexPreviousOutput];
					
					if (commandTuple.outputBuilder is null)
						commandTuple.outputBuilder = new StringBuilder(output);
						
					commandTuple.outputBuilder.Append(output);
				}
			}
		}
		
		OnWriteOutput?.Invoke();
	}
	
	private void CreateTextEditor()
    {
        _textEditorService.PostUnique(
            nameof(TerminalOutputTextEditorExpand),
            editContext =>
            {

                var line1 = "Integrated-Terminal";
                var line2 = "Try: cmd /c \"dir\"";

                var longestLineLength = Math.Max(line1.Length, line2.Length);

                var model = new TextEditorModel(
                    TextEditorModelResourceUri,
                    DateTime.UtcNow,
                    "terminal",
                    $"{line1}\n" +
                        $"{line2}\n" +
                        new string('=', longestLineLength) +
                        "\n\n",
                    new TerminalDecorationMapper(),
                    _compilerServiceRegistry.GetCompilerService(ExtensionNoPeriodFacts.TERMINAL));

                _textEditorService.ModelApi.RegisterCustom(model);

                var modelModifier = editContext.GetModelModifier(model.ResourceUri);

                if (modelModifier is null)
                    return Task.CompletedTask;

                _textEditorService.ModelApi.AddPresentationModel(
                    editContext,
                    modelModifier,
                    TerminalPresentationFacts.EmptyPresentationModel);

                _textEditorService.ModelApi.AddPresentationModel(
                    editContext,
                    modelModifier,
                    CompilerServiceDiagnosticPresentationFacts.EmptyPresentationModel);

                _textEditorService.ModelApi.AddPresentationModel(
                    editContext,
                    modelModifier,
                    FindOverlayPresentationFacts.EmptyPresentationModel);

                model.CompilerService.RegisterResource(model.ResourceUri);

                _textEditorService.ViewModelApi.Register(
                    TextEditorViewModelKey,
                    TextEditorModelResourceUri,
                    new Category("terminal"));

                var layerFirstPresentationKeys = new[]
                {
                    TerminalPresentationFacts.PresentationKey,
                    CompilerServiceDiagnosticPresentationFacts.PresentationKey,
                    FindOverlayPresentationFacts.PresentationKey,
                }.ToImmutableArray();

                var viewModelModifier = editContext.GetViewModelModifier(TextEditorViewModelKey);
                if (viewModelModifier is null)
                    return Task.CompletedTask;

                viewModelModifier.ViewModel = viewModelModifier.ViewModel with
                {
                    FirstPresentationLayerKeysList = layerFirstPresentationKeys.ToImmutableList()
                };

                var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
                var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

                if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                    return Task.CompletedTask;

                _textEditorService.ViewModelApi.MoveCursor(
                    new KeyboardEventArgs
                    {
                        Code = KeyboardKeyFacts.MovementKeys.END,
                        Key = KeyboardKeyFacts.MovementKeys.END,
                        CtrlKey = true,
                    },
                    editContext,
                    modelModifier,
                    viewModelModifier,
                    cursorModifierBag);

                var terminalCompilerService = (TerminalCompilerService)modelModifier.CompilerService;
                if (terminalCompilerService.GetCompilerServiceResourceFor(modelModifier.ResourceUri) is not TerminalResource terminalResource)
                    return Task.CompletedTask;

                terminalResource.ManualDecorationTextSpanList.Add(new TextEditorTextSpan(
                    0,
                    modelModifier.GetPositionIndex(primaryCursorModifier),
                    (byte)TerminalDecorationKind.Comment,
                    TextEditorModelResourceUri,
                    modelModifier.GetAllText()));

                editContext.TextEditorService.ModelApi.ApplyDecorationRange(
                    editContext,
                    modelModifier,
                    terminalResource.GetTokenTextSpans());

                return Task.CompletedTask;
            });
    }
    
    /// <summary>
    /// TODO: Do not forget that when clearing the terminal output...
    ///       ...any corresponding text editor data needs to be disposed.
    /// </summary>
    private Task OpenInEditor(TerminalCommandParsed terminalCommandParsed)
    {
    	_textEditorService.PostUnique(
            nameof(TerminalOutputTextEditorExpand),
            editContext =>
	    	{
	    		var viewModelModifier = editContext.GetViewModelModifier(TextEditorViewModelKey);
                if (viewModelModifier is null)
                    return Task.CompletedTask;
                    
                var widgetKey = Key<WidgetBlock>.NewKey();
	    	
	    		var widget = new WidgetBlock(
				    widgetKey,
				    "title goes here",
				    $"luth_ide_expand-widget_{widgetKey.Guid}",
				    1,
				    typeof(TerminalOutputViewOutputDisplay),
				    new Dictionary<string, object?>
		            {
		            	{
		            		nameof(TerminalOutputViewOutputDisplay.TerminalOutputTextEditorExpand),
		            		this
		            	},
		            	{
		            		nameof(TerminalOutputViewOutputDisplay.TerminalCommandParsed),
		            		terminalCommandParsed
		            	},
		            });
		            
		        viewModelModifier.ViewModel = viewModelModifier.ViewModel with
		        {
		        	WidgetBlockList = viewModelModifier.ViewModel.WidgetBlockList
		        		.Add(widget)
		        };
		            
		        return Task.CompletedTask;
	    	});
        
    	return Task.CompletedTask;
    }
	
	public void Dispose()
	{
		_terminal.TerminalInteractive.WorkingDirectoryChanged -= OnWorkingDirectoryChanged;
	}
}
