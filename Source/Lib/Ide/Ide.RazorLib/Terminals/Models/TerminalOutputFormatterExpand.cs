using System.Text;
using Microsoft.JSInterop;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Panels.Models;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Lines.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.Virtualizations.Models;
using Luthetus.TextEditor.RazorLib.JavaScriptObjects.Models;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.Extensions.CompilerServices.Syntax;

namespace Luthetus.Ide.RazorLib.Terminals.Models;

public class TerminalOutputFormatterExpand : ITerminalOutputFormatter
{
	public Guid Id { get; } = Guid.NewGuid();

	public ResourceUri TextEditorModelResourceUri { get; }

    public Key<TextEditorViewModel> TextEditorViewModelKey { get; }

	private readonly ITerminal _terminal;
	private readonly TextEditorService _textEditorService;
	private readonly ICompilerServiceRegistry _compilerServiceRegistry;
	private readonly IDialogService _dialogService;
	private readonly IPanelService _panelService;
	private readonly CommonBackgroundTaskApi _commonBackgroundTaskApi;

	public TerminalOutputFormatterExpand(
		ITerminal terminal,
		TextEditorService textEditorService,
		ICompilerServiceRegistry compilerServiceRegistry,
		IDialogService dialogService,
		IPanelService panelService,
        CommonBackgroundTaskApi commonBackgroundTaskApi)
	{
		_terminal = terminal;
		_textEditorService = textEditorService;
		_compilerServiceRegistry = compilerServiceRegistry;
		_dialogService = dialogService;
		_panelService = panelService;
		_commonBackgroundTaskApi = commonBackgroundTaskApi;
		
		TextEditorModelResourceUri = new(
			ResourceUriFacts.Terminal_ReservedResourceUri_Prefix + Id.ToString());
		
		TextEditorViewModelKey = new Key<TextEditorViewModel>(Id);
		
		CreateTextEditor();
	}

	public string Name { get; } = nameof(TerminalOutputFormatterExpand);
	
	public ITerminalOutputFormatted Format()
	{
		var outSymbolList = new List<Symbol>();
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
					    StartInclusiveIndex = textSpan.StartInclusiveIndex + outputBuilder.Length,
					    EndExclusiveIndex = textSpan.EndExclusiveIndex + outputBuilder.Length,
			    	}));
			}
			
			outputBuilder.Append(parsedCommand.OutputCache.ToString());
		}
		
		return new TerminalOutputFormattedTextEditor(
			outputBuilder.ToString(),
			parsedCommandList,
			outTextSpanList,
			outSymbolList);
	}
	
	private void CreateTextEditor()
    {
    	_textEditorService.WorkerArbitrary.PostUnique(nameof(TerminalOutputFormatterExpand), editContext =>
    	{
    		var model = new TextEditorModel(
	            TextEditorModelResourceUri,
	            DateTime.UtcNow,
	            "terminal",
	            string.Empty,
	            new TerminalDecorationMapper(),
	            _compilerServiceRegistry.GetCompilerService(ExtensionNoPeriodFacts.TERMINAL),
            	_textEditorService)
	        {
	        	UseUnsetOverride = true,
	        	UnsetOverrideLineEndKind = LineEndKind.LineFeed,
	        };
	            
	        var modelModifier = new TextEditorModel(model);
	        modelModifier.PerformRegisterPresentationModelAction(TerminalPresentationFacts.EmptyPresentationModel);
	        modelModifier.PerformRegisterPresentationModelAction(CompilerServiceDiagnosticPresentationFacts.EmptyPresentationModel);
	        modelModifier.PerformRegisterPresentationModelAction(FindOverlayPresentationFacts.EmptyPresentationModel);
	        
	        model = modelModifier;
	
	        _textEditorService.ModelApi.RegisterCustom(editContext, model);
	        
			model.PersistentState.CompilerService.RegisterResource(
				model.PersistentState.ResourceUri,
				shouldTriggerResourceWasModified: true);
				
	        var viewModel = new TextEditorViewModel(
	            TextEditorViewModelKey,
	            TextEditorModelResourceUri,
	            _textEditorService,
	            _panelService,
	            _dialogService,
	            _commonBackgroundTaskApi,
	            VirtualizationGrid.Empty,
				new TextEditorDimensions(0, 0, 0, 0),
				new ScrollbarDimensions(0, 0, 0, 0, 0),
	            new Category("terminal"));
	
	        var firstPresentationLayerKeys = new List<Key<TextEditorPresentationModel>>()
	        {
	            TerminalPresentationFacts.PresentationKey,
	            CompilerServiceDiagnosticPresentationFacts.PresentationKey,
	            FindOverlayPresentationFacts.PresentationKey,
	        };
	            
	        viewModel.PersistentState.FirstPresentationLayerKeysList = firstPresentationLayerKeys;
	        
	        _textEditorService.ViewModelApi.Register(editContext, viewModel);
	        
	        return ValueTask.CompletedTask;
    	});
    }
    
    public void Dispose()
    {
    	// TODO: Dispose of the text editor resources
    }
}
