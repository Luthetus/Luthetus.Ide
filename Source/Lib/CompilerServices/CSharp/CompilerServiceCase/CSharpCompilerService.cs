using Microsoft.AspNetCore.Components.Web;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.JavaScriptObjects.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.Groups.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Autocompletes.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Commands.Models;
using Luthetus.TextEditor.RazorLib.ComponentRenderers.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.Events.Models;
using Luthetus.Extensions.CompilerServices;
using Luthetus.Extensions.CompilerServices.Syntax;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes;
using Luthetus.CompilerServices.CSharp.BinderCase;
using Luthetus.CompilerServices.CSharp.LexerCase;
using Luthetus.CompilerServices.CSharp.ParserCase;
using Luthetus.CompilerServices.CSharp.RuntimeAssemblies;

namespace Luthetus.CompilerServices.CSharp.CompilerServiceCase;

public sealed class CSharpCompilerService : IExtendedCompilerService
{
	// <summary>Public because the RazorCompilerService uses it.</summary>
    public readonly CSharpBinder __CSharpBinder = new();
    
    private readonly Dictionary<ResourceUri, CSharpResource> _resourceMap = new();
    private readonly object _resourceMapLock = new();
    
    // Service dependencies
    private readonly ITextEditorService _textEditorService;
    
    public CSharpCompilerService(ITextEditorService textEditorService)
    {
    	_textEditorService = textEditorService;
    }

    public event Action? ResourceRegistered;
    public event Action? ResourceParsed;
    public event Action? ResourceDisposed;

    public IReadOnlyList<ICompilerServiceResource> CompilerServiceResources { get; }
    
    public IReadOnlyDictionary<string, TypeDefinitionNode> AllTypeDefinitions { get; }
    
    /// <summary>
    /// This overrides the default Blazor component: <see cref="Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals.SymbolDisplay"/>.
    /// It is shown when hovering with the cursor over a <see cref="Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Symbols.ISymbol"/>
    /// (as well other actions will show it).
    ///
    /// If only a small change is necessary, It is recommended to replicate <see cref="Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals.SymbolDisplay"/>
    /// but with a component of your own name.
    ///
    /// There is a switch statement that renders content based on the symbol's SyntaxKind.
    ///
    /// So, if the small change is for a particular SyntaxKind, copy over the entire switch statement,
    /// and change that case in particular.
    ///
    /// There are optimizations in the SymbolDisplay's codebehind to stop it from re-rendering
    /// unnecessarily. So check the codebehind and copy over the code from there too if desired (this is recommended).
    ///
    /// The "all in" approach to overriding the default 'SymbolRenderer' was decided on over
    /// a more fine tuned override of each individual case in the UI's switch statement.
    ///
    /// This was because it is firstly believed that the properties necessary to customize
    /// the SymbolRenderer would massively increase.
    /// 
    /// And secondly because it is believed that the Nodes shouldn't even be shared
    /// amongst the TextEditor and the ICompilerService.
    ///
    /// That is to say, it feels quite odd that a Node and SyntaxKind enum member needs
    /// to be defined by the text editor, rather than the ICompilerService doing it.
    ///
    /// The solution to this isn't yet known but it is always in the back of the mind
    /// while working on the text editor.
    /// </summary>
    public Type? SymbolRendererType { get; }
    public Type? DiagnosticRendererType { get; }

    public void RegisterResource(ResourceUri resourceUri, bool shouldTriggerResourceWasModified)
    {
    	lock (_resourceMapLock)
        {
            if (_resourceMap.ContainsKey(resourceUri))
                return;

            _resourceMap.Add(resourceUri, new CSharpResource(resourceUri, this));
        }

		if (shouldTriggerResourceWasModified)
	        ResourceWasModified(resourceUri, Array.Empty<TextEditorTextSpan>());
	        
        ResourceRegistered?.Invoke();
    }
    
    public void DisposeResource(ResourceUri resourceUri)
    {
    	lock (_resourceMapLock)
        {
            _resourceMap.Remove(resourceUri);
        }

        ResourceDisposed?.Invoke();
    }

    public void ResourceWasModified(ResourceUri resourceUri, IReadOnlyList<TextEditorTextSpan> editTextSpansList)
    {
    	_textEditorService.TextEditorWorker.PostUnique(nameof(CSharpCompilerService), editContext =>
        {
			var modelModifier = editContext.GetModelModifier(resourceUri);

			if (modelModifier is null)
				return ValueTask.CompletedTask;

			return ParseAsync(editContext, modelModifier, shouldApplySyntaxHighlighting: true);
        });
    }

    public ICompilerServiceResource? GetResource(ResourceUri resourceUri)
    {
    	var model = _textEditorService.ModelApi.GetOrDefault(resourceUri);

        if (model is null)
            return null;

        lock (_resourceMapLock)
        {
            if (!_resourceMap.ContainsKey(resourceUri))
                return null;

            return _resourceMap[resourceUri];
        }
    }
    
    public MenuRecord GetContextMenu(TextEditorRenderBatch renderBatch, ContextMenu contextMenu)
	{
		return contextMenu.GetDefaultMenuRecord();
	}

    public ValueTask<MenuRecord> GetAutocompleteMenu(
    	ITextEditorEditContext editContext,
        TextEditorModelModifier modelModifier,
        TextEditorViewModelModifier viewModelModifier,
        CursorModifierBagTextEditor cursorModifierBag,
        TextEditorCommandArgs commandArgs)
    {
    	return ValueTask.FromResult(new MenuRecord(MenuRecord.NoMenuOptionsExistList));
    }
    
    public ValueTask<MenuRecord> GetQuickActionsSlashRefactorMenu(
        ITextEditorEditContext editContext,
        TextEditorModelModifier modelModifier,
        TextEditorViewModelModifier viewModelModifier,
        CursorModifierBagTextEditor cursorModifierBag,
        TextEditorCommandArgs commandArgs)
    {
    	return ValueTask.FromResult(new MenuRecord(MenuRecord.NoMenuOptionsExistList));
    }
	
	public async ValueTask OnInspect(
		ITextEditorEditContext editContext,
		TextEditorModelModifier modelModifier,
		TextEditorViewModelModifier viewModelModifier,
		MouseEventArgs mouseEventArgs,
		TextEditorComponentData componentData,
		ILuthetusTextEditorComponentRenderers textEditorComponentRenderers,
        ResourceUri resourceUri)
    {
    	// Lazily calculate row and column index a second time. Otherwise one has to calculate it every mouse moved event.
        var rowAndColumnIndex = await EventUtils.CalculateRowAndColumnIndex(
				resourceUri,
				viewModelModifier.ViewModel.ViewModelKey,
				mouseEventArgs,
				componentData,
				editContext)
			.ConfigureAwait(false);

		var textEditorDimensions = viewModelModifier.ViewModel.TextEditorDimensions;
		var scrollbarDimensions = viewModelModifier.ViewModel.ScrollbarDimensions;
	
		var relativeCoordinatesOnClick = new RelativeCoordinates(
		    mouseEventArgs.ClientX - textEditorDimensions.BoundingClientRectLeft,
		    mouseEventArgs.ClientY - textEditorDimensions.BoundingClientRectTop,
		    scrollbarDimensions.ScrollLeft,
		    scrollbarDimensions.ScrollTop);

        var cursorPositionIndex = modelModifier.GetPositionIndex(new TextEditorCursor(
            rowAndColumnIndex.rowIndex,
            rowAndColumnIndex.columnIndex,
            true));

        var foundMatch = false;
        
        var resource = GetResource(modelModifier.ResourceUri);
        var compilationUnitLocal = (CSharpCompilationUnit)resource.CompilationUnit;
        
        var symbols = compilationUnitLocal.SymbolList;
        var diagnostics = compilationUnitLocal.DiagnosticList;

        if (diagnostics.Count != 0)
        {
            foreach (var diagnostic in diagnostics)
            {
                if (cursorPositionIndex >= diagnostic.TextSpan.StartingIndexInclusive &&
                    cursorPositionIndex < diagnostic.TextSpan.EndingIndexExclusive)
                {
                    // Prefer showing a diagnostic over a symbol when both exist at the mouse location.
                    foundMatch = true;

                    var parameterMap = new Dictionary<string, object?>
                    {
                        {
                            nameof(ITextEditorDiagnosticRenderer.Diagnostic),
                            diagnostic
                        }
                    };

                    viewModelModifier.ViewModel = viewModelModifier.ViewModel with
					{
						TooltipViewModel = new(
		                    modelModifier.CompilerService.DiagnosticRendererType ?? textEditorComponentRenderers.DiagnosticRendererType,
		                    parameterMap,
		                    relativeCoordinatesOnClick,
		                    null,
		                    componentData.ContinueRenderingTooltipAsync)
					};
                }
            }
        }

        if (!foundMatch && symbols.Count != 0)
        {
            foreach (var symbol in symbols)
            {
                if (cursorPositionIndex >= symbol.TextSpan.StartingIndexInclusive &&
                    cursorPositionIndex < symbol.TextSpan.EndingIndexExclusive)
                {
                    foundMatch = true;

                    var parameters = new Dictionary<string, object?>
                    {
                        {
                            "Symbol",
                            symbol
                        }
                    };

                    viewModelModifier.ViewModel = viewModelModifier.ViewModel with
					{
						TooltipViewModel = new(
	                        typeof(Luthetus.Extensions.CompilerServices.Displays.SymbolDisplay),
	                        parameters,
	                        relativeCoordinatesOnClick,
	                        null,
	                        componentData.ContinueRenderingTooltipAsync)
					};
                }
            }
        }

        if (!foundMatch)
        {
			viewModelModifier.ViewModel = viewModelModifier.ViewModel with
			{
            	TooltipViewModel = null
			};
        }

        // TODO: Measure the tooltip, and reposition if it would go offscreen.
    }
    
    public void GoToDefinition(
        ITextEditorEditContext editContext,
        TextEditorModelModifier modelModifier,
        TextEditorViewModelModifier viewModelModifier,
        CursorModifierBagTextEditor cursorModifierBag,
        TextEditorCommandArgs commandArgs)
    {
        var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

        var positionIndex = modelModifier.GetPositionIndex(primaryCursorModifier);
        
        var wordTextSpan = modelModifier.GetWordTextSpan(positionIndex);
        if (wordTextSpan is null)
            return;

		var compilerServiceResource = modelModifier.CompilerService.GetResource(modelModifier.ResourceUri);
		if (compilerServiceResource?.CompilationUnit is null)
			return;

        var definitionTextSpan = GetDefinitionTextSpan(wordTextSpan.Value, compilerServiceResource);
        if (definitionTextSpan is null)
            return;

        var definitionModel = commandArgs.TextEditorService.ModelApi.GetOrDefault(definitionTextSpan.Value.ResourceUri);
        if (definitionModel is null)
        {
            if (commandArgs.TextEditorService.TextEditorConfig.RegisterModelFunc is not null)
            {
                commandArgs.TextEditorService.TextEditorConfig.RegisterModelFunc.Invoke(
                    new RegisterModelArgs(definitionTextSpan.Value.ResourceUri, commandArgs.ServiceProvider));

                var definitionModelModifier = editContext.GetModelModifier(definitionTextSpan.Value.ResourceUri);

                if (definitionModel is null) // TODO: Should this be null checking instead: 'definitionModelModifier'?
                    return;
            }
            else
            {
                return;
            }
        }

        var definitionViewModels = commandArgs.TextEditorService.ModelApi.GetViewModelsOrEmpty(definitionTextSpan.Value.ResourceUri);

        if (!definitionViewModels.Any())
        {
            if (commandArgs.TextEditorService.TextEditorConfig.TryRegisterViewModelFunc is not null)
            {
                commandArgs.TextEditorService.TextEditorConfig.TryRegisterViewModelFunc.Invoke(new TryRegisterViewModelArgs(
                    Key<TextEditorViewModel>.NewKey(),
                    definitionTextSpan.Value.ResourceUri,
                    new Category("main"),
                    true,
                    commandArgs.ServiceProvider));

                definitionViewModels = commandArgs.TextEditorService.ModelApi.GetViewModelsOrEmpty(definitionTextSpan.Value.ResourceUri);

                if (!definitionViewModels.Any())
                    return;
            }
            else
            {
                return;
            }
        }

        var definitionViewModelKey = definitionViewModels.First().ViewModelKey;
        
        var definitionViewModelModifier = editContext.GetViewModelModifier(definitionViewModelKey);
        var definitionCursorModifierBag = editContext.GetCursorModifierBag(definitionViewModelModifier?.ViewModel);
        var definitionPrimaryCursorModifier = editContext.GetPrimaryCursorModifier(definitionCursorModifierBag);

        if (definitionViewModelModifier is null || !definitionCursorModifierBag.ConstructorWasInvoked || definitionPrimaryCursorModifier is null)
            return;

        var rowData = definitionModel.GetLineInformationFromPositionIndex(definitionTextSpan.Value.StartingIndexInclusive);
        var columnIndex = definitionTextSpan.Value.StartingIndexInclusive - rowData.StartPositionIndexInclusive;

        definitionPrimaryCursorModifier.SelectionAnchorPositionIndex = null;
        definitionPrimaryCursorModifier.LineIndex = rowData.Index;
        definitionPrimaryCursorModifier.ColumnIndex = columnIndex;
        definitionPrimaryCursorModifier.PreferredColumnIndex = columnIndex;

        if (commandArgs.TextEditorService.TextEditorConfig.TryShowViewModelFunc is not null)
        {
            commandArgs.TextEditorService.TextEditorConfig.TryShowViewModelFunc.Invoke(new TryShowViewModelArgs(
                definitionViewModelKey,
                Key<TextEditorGroup>.Empty,
                true,
                commandArgs.ServiceProvider));
        }
    }
    
    public ValueTask ParseAsync(ITextEditorEditContext editContext, TextEditorModelModifier modelModifier, bool shouldApplySyntaxHighlighting)
	{
		var resourceUri = modelModifier.ResourceUri;
	
		if (!_resourceMap.ContainsKey(resourceUri))
			return ValueTask.CompletedTask;
	
		_textEditorService.ModelApi.StartPendingCalculatePresentationModel(
			editContext,
	        modelModifier,
	        CompilerServiceDiagnosticPresentationFacts.PresentationKey,
			CompilerServiceDiagnosticPresentationFacts.EmptyPresentationModel);

		var presentationModel = modelModifier.PresentationModelList.First(
			x => x.TextEditorPresentationKey == CompilerServiceDiagnosticPresentationFacts.PresentationKey);
		
		var cSharpCompilationUnit = new CSharpCompilationUnit(resourceUri);
		
		var lexerOutput = CSharpLexer.Lex(resourceUri, presentationModel.PendingCalculation.ContentAtRequest);
		cSharpCompilationUnit.TokenList = lexerOutput.SyntaxTokenList;
		cSharpCompilationUnit.MiscTextSpanList = lexerOutput.MiscTextSpanList;

		// Even if the parser throws an exception, be sure to
		// make use of the Lexer to do whatever syntax highlighting is possible.
		try
		{
			__CSharpBinder.StartCompilationUnit(resourceUri);
			CSharpParser.Parse(cSharpCompilationUnit, __CSharpBinder, ref lexerOutput);
		}
		finally
		{
			lock (_resourceMapLock)
			{
				if (_resourceMap.ContainsKey(resourceUri))
				{
					var resource = (CSharpResource)_resourceMap[resourceUri];
					resource.CompilationUnit = cSharpCompilationUnit;
				}
			}
			
			var diagnosticTextSpans = cSharpCompilationUnit.DiagnosticList
				.Select(x => x.TextSpan)
				.ToList();

			modelModifier.CompletePendingCalculatePresentationModel(
				CompilerServiceDiagnosticPresentationFacts.PresentationKey,
				CompilerServiceDiagnosticPresentationFacts.EmptyPresentationModel,
				diagnosticTextSpans);
			
			if (shouldApplySyntaxHighlighting)
			{
				editContext.TextEditorService.ModelApi.ApplySyntaxHighlighting(
					editContext,
					modelModifier);
			}

			ResourceParsed?.Invoke();
        }
		
        return ValueTask.CompletedTask;
	}
    
    /// <summary>
    /// Looks up the <see cref="IScope"/> that encompasses the provided positionIndex.
    ///
    /// Then, checks the <see cref="IScope"/>.<see cref="IScope.CodeBlockOwner"/>'s children
    /// to determine which node exists at the positionIndex.
    ///
    /// If the <see cref="IScope"/> cannot be found, then as a fallback the provided compilationUnit's
    /// <see cref="CompilationUnit.RootCodeBlockNode"/> will be treated
    /// the same as if it were the <see cref="IScope"/>.<see cref="IScope.CodeBlockOwner"/>.
    ///
    /// If the provided compilerServiceResource?.CompilationUnit is null, then the fallback step will not occur.
    /// The fallback step is expected to occur due to the global scope being implemented with a null
    /// <see cref="IScope"/>.<see cref="IScope.CodeBlockOwner"/> at the time of this comment.
    /// </summary>
    public ISyntaxNode? GetSyntaxNode(int positionIndex, ResourceUri resourceUri, ICompilerServiceResource? compilerServiceResource)
    {
    	return __CSharpBinder.GetSyntaxNode(cSharpCompilationUnit: null, positionIndex, resourceUri, (CSharpResource)compilerServiceResource);
    }
    
    /// <summary>
    /// Returns the <see cref="ISyntaxNode"/> that represents the definition in the <see cref="CompilationUnit"/>.
    ///
    /// The option argument 'symbol' can be provided if available. It might provide additional information to the method's implementation
    /// that is necessary to find certain nodes (ones that are in a separate file are most common to need a symbol to find).
    /// </summary>
    public ISyntaxNode? GetDefinitionNode(TextEditorTextSpan textSpan, ICompilerServiceResource compilerServiceResource, Symbol? symbol = null)
    {
    	if (symbol is null)
    		return null;
    		
    	return __CSharpBinder.GetDefinitionNode(cSharpCompilationUnit: null, textSpan, symbol.Value.SyntaxKind, symbol);
    }

	/// <summary>
	/// Returns the text span at which the definition exists in the source code.
	/// </summary>
    public TextEditorTextSpan? GetDefinitionTextSpan(TextEditorTextSpan textSpan, ICompilerServiceResource compilerServiceResource)
    {
    	return __CSharpBinder.GetDefinitionTextSpan(textSpan, (CSharpResource)compilerServiceResource);
    }

	public Scope GetScopeByPositionIndex(ResourceUri resourceUri, int positionIndex)
    {
    	return __CSharpBinder.GetScopeByPositionIndex(null, resourceUri, positionIndex);
    }
}