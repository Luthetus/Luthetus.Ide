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
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.CompilerServices.Facts;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Symbols;
using Luthetus.Ide.RazorLib.Terminals.Displays.NewCode;

namespace Luthetus.Ide.RazorLib.Terminals.Models.NewCode;

public class TerminalOutputTextEditorExpand : ITerminalOutput, IDisposable
{
    public static ResourceUri TextEditorModelResourceUri { get; } = new(
        ResourceUriFacts.Terminal_ReservedResourceUri_Prefix + nameof(TerminalOutputTextEditorExpand));

    public static Key<TextEditorViewModel> TextEditorViewModelKey { get; } = Key<TextEditorViewModel>.NewKey();

    private readonly ITerminal _terminal;
	private readonly ITextEditorService _textEditorService;
	private readonly ICompilerServiceRegistry _compilerServiceRegistry;
	private readonly IDispatcher _dispatcher;
	private readonly List<ITerminalOutputFormatter> _outputFormatterList;
	private readonly List<ITextEditorSymbol> _symbolList = new();
	private readonly List<TextEditorTextSpan> _textSpanList = new();
	private readonly List<TerminalCommandParsed> _commandList = new();
	private readonly object _listLock = new();

	public TerminalOutputTextEditorExpand(
		ITerminal terminal,
		ITextEditorService textEditorService,
		ICompilerServiceRegistry compilerServiceRegistry,
		IDispatcher dispatcher)
	{
		_terminal = terminal;
		
		_textEditorService = textEditorService;
		_compilerServiceRegistry = compilerServiceRegistry;
		_dispatcher = dispatcher;
		
		_outputFormatterList = new()
		{
			new TerminalOutputFormatterAll(),
			new TerminalOutputFormatterExpand(),
		};
		
		CreateTextEditor();
	}
	
	public string OutputRaw { get; }

	public event Action? OnWriteOutput;
	
	public string? GetOutput(string terminalOutputFormatterName)
	{
		var outputFormatter = _outputFormatterList.FirstOrDefault(x =>
			x.Name == terminalOutputFormatterName);
			
		if (outputFormatter is null)
			return null;
			
		return outputFormatter.Format(_terminal);
	}
	
	public TerminalCommandParsed? GetParsedCommandOrDefault(Key<TerminalCommandRequest> terminalCommandRequestKey)
	{
		lock (_listLock)
		{
			return _commandList.FirstOrDefault(x =>
				x.SourceTerminalCommandRequest.Key == terminalCommandRequestKey);
		}
	}
	
	public ImmutableList<TerminalCommandParsed> GetCommandList()
	{
		lock (_listLock)
		{
			return _commandList.ToImmutableList();
		}
	}
	
	public ImmutableList<TextEditorTextSpan> GetTextSpanList()
	{
		lock (_listLock)
		{
			return _textSpanList.ToImmutableList();
		}
	}

	public ImmutableList<ITextEditorSymbol> GetSymbolList()
	{
		lock (_listLock)
		{
			return _symbolList.ToImmutableList();
		}
	}
	
	public void RegisterOutputFormatterCustom(ITerminalOutputFormatter outputFormatter)
	{
		lock (_listLock)
		{
			_outputFormatterList.Add(outputFormatter);
		}
	}
	
	public void WriteOutput(TerminalCommandParsed terminalCommandParsed, CommandEvent commandEvent)
	{
		var output = (string?)null;

		switch (commandEvent)
		{
			case StartedCommandEvent started:
				
				// Delete any output of the previous invocation.
				lock (_listLock)
				{
					var indexPreviousOutput = _commandList.FindIndex(x =>
						x.SourceTerminalCommandRequest.Key ==
							terminalCommandParsed.SourceTerminalCommandRequest.Key);
							
					if (indexPreviousOutput != -1)
						_commandList.RemoveAt(indexPreviousOutput);
						
					_commandList.Add(terminalCommandParsed);
				}
				
				break;
			case StandardOutputCommandEvent stdOut:
				terminalCommandParsed.OutputCache.AppendTwo(stdOut.Text, "\n");
				break;
			case StandardErrorCommandEvent stdErr:
				terminalCommandParsed.OutputCache.AppendTwo(stdErr.Text, "\n");
				break;
			case ExitedCommandEvent exited:
				break;
		}
		
		OnWriteOutput?.Invoke();
	}
	
	/// <summary>
	/// This method can be moved out of this class.
	/// There is no need for this class to be concerned with the text editor.
	/// </summary>
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
				    $"{terminalCommandParsed.SourceTerminalCommandRequest.CommandText}:",
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
    	// TODO: Dispose of the text editor resources
    }
}
