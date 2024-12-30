using System.Text;
using System.Collections.Immutable;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Fluxor;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Facts;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.Virtualizations.Models;
using Luthetus.TextEditor.RazorLib.JavaScriptObjects.Models;
using Luthetus.Common.RazorLib.Keymaps.Models;

namespace Luthetus.Ide.RazorLib.Terminals.Models;

public class TerminalOutputFormatterExpand : ITerminalOutputFormatter
{
	public Guid Id { get; } = Guid.NewGuid();

	public ResourceUri TextEditorModelResourceUri { get; }

    public Key<TextEditorViewModel> TextEditorViewModelKey { get; }

	private readonly ITerminal _terminal;
	private readonly ITextEditorService _textEditorService;
	private readonly ICompilerServiceRegistry _compilerServiceRegistry;
	private readonly IDialogService _dialogService;
	private readonly IJSRuntime _jsRuntime;
	private readonly IDispatcher _dispatcher;

	public TerminalOutputFormatterExpand(
		ITerminal terminal,
		ITextEditorService textEditorService,
		ICompilerServiceRegistry compilerServiceRegistry,
		IDialogService dialogService,
        IJSRuntime jsRuntime,
		IDispatcher dispatcher)
	{
		_terminal = terminal;
		_textEditorService = textEditorService;
		_compilerServiceRegistry = compilerServiceRegistry;
		_dialogService = dialogService;
		_jsRuntime = jsRuntime;
		_dispatcher = dispatcher;
		
		TextEditorModelResourceUri = new(
			ResourceUriFacts.Terminal_ReservedResourceUri_Prefix + Id.ToString());
		
		TextEditorViewModelKey = new Key<TextEditorViewModel>(Id);
		
		CreateTextEditor();
	}

	public string Name { get; } = nameof(TerminalOutputFormatterExpand);
	
	public ITerminalOutputFormatted Format()
	{
		var outSymbolList = new List<ITextEditorSymbol>();
		var outTextSpanList = new List<TextEditorTextSpan>();
		
		var parsedCommandList = _terminal.TerminalOutput.GetParsedCommandList();
		
		var outputBuilder = new StringBuilder();
		
		foreach (var parsedCommand in parsedCommandList)
		{
			var workingDirectoryText = parsedCommand.SourceTerminalCommandRequest.WorkingDirectory + "> ";
		
			var workingDirectoryTextSpan = new TextEditorTextSpan(
				outputBuilder.Length,
		        outputBuilder.Length + workingDirectoryText.Length,
		        (byte)TerminalDecorationKind.Keyword,
		        ResourceUri.Empty,
		        string.Empty,
		        workingDirectoryText);
		    outTextSpanList.Add(workingDirectoryTextSpan);
		    
			outputBuilder
				.Append(workingDirectoryText)
				.Append(parsedCommand.SourceTerminalCommandRequest.CommandText)
				.Append('\n');
				
			var parsedCommandTextSpanList = parsedCommand.TextSpanList;
			
			if (parsedCommandTextSpanList is not null)
			{
			    outTextSpanList.AddRange(parsedCommandTextSpanList.Select(
			    	textSpan => textSpan with
			    	{
					    StartingIndexInclusive = textSpan.StartingIndexInclusive + outputBuilder.Length,
					    EndingIndexExclusive = textSpan.EndingIndexExclusive + outputBuilder.Length,
			    	}));
			}
			
			outputBuilder.Append(parsedCommand.OutputCache.ToString());
		}
		
		return new TerminalOutputFormattedTextEditor(
			outputBuilder.ToString(),
			parsedCommandList,
			outTextSpanList.ToImmutableList(),
			outSymbolList.ToImmutableList());
	}
	
	private void CreateTextEditor()
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
            
        var modelModifier = new TextEditorModelModifier(model);
        modelModifier.PerformRegisterPresentationModelAction(TerminalPresentationFacts.EmptyPresentationModel);
        modelModifier.PerformRegisterPresentationModelAction(CompilerServiceDiagnosticPresentationFacts.EmptyPresentationModel);
        modelModifier.PerformRegisterPresentationModelAction(FindOverlayPresentationFacts.EmptyPresentationModel);
        
        model = modelModifier.ToModel();

        _textEditorService.ModelApi.RegisterCustom(model);
        
		model.CompilerService.RegisterResource(
			model.ResourceUri,
			shouldTriggerResourceWasModified: true);
			
        var viewModel = new TextEditorViewModel(
            TextEditorViewModelKey,
            TextEditorModelResourceUri,
            _textEditorService,
            _dispatcher,
            _dialogService,
            _jsRuntime,
            VirtualizationGrid.Empty,
			new TextEditorDimensions(0, 0, 0, 0),
			new ScrollbarDimensions(0, 0, 0, 0, 0),
    		new CharAndLineMeasurements(0, 0),
            false,
            new Category("terminal"));

        var firstPresentationLayerKeys = new[]
        {
            TerminalPresentationFacts.PresentationKey,
            CompilerServiceDiagnosticPresentationFacts.PresentationKey,
            FindOverlayPresentationFacts.PresentationKey,
        }.ToImmutableArray();
            
        viewModel = viewModel with
        {
            FirstPresentationLayerKeysList = firstPresentationLayerKeys.ToImmutableList()
        };
        
        _textEditorService.ViewModelApi.Register(viewModel);

        _textEditorService.PostUnique(
            nameof(TerminalOutput),
            editContext =>
            {
                var modelModifier = editContext.GetModelModifier(model.ResourceUri);
                if (modelModifier is null)
                    return Task.CompletedTask;
                    
                var viewModelModifier = editContext.GetViewModelModifier(TextEditorViewModelKey);
		        if (viewModelModifier is null)
		            return Task.CompletedTask;

                var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
                var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

                if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                    return Task.CompletedTask;

                _textEditorService.ViewModelApi.MoveCursor(
                    new KeymapArgs
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
    
    public void Dispose()
    {
    	// TODO: Dispose of the text editor resources
    }
}
