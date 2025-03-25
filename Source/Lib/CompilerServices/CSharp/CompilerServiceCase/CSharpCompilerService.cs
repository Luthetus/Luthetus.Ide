using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.JavaScriptObjects.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Clipboards.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
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
using Luthetus.Extensions.CompilerServices.Displays;
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
    private readonly IClipboardService _clipboardService;
    
    public CSharpCompilerService(ITextEditorService textEditorService, IClipboardService clipboardService)
    {
    	_textEditorService = textEditorService;
    	_clipboardService = clipboardService;
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

	public MenuRecord GetAutocompleteMenu(TextEditorRenderBatch renderBatch, AutocompleteMenu autocompleteMenu)
	{
		var primaryCursor = renderBatch.ViewModel.PrimaryCursor;
        
        var positionIndex = renderBatch.Model.GetPositionIndex(primaryCursor);
        
        var word = renderBatch.Model.ReadPreviousWordOrDefault(
	        primaryCursor.LineIndex,
	        primaryCursor.ColumnIndex);
	
		// The cursor is 1 character ahead.
        var textSpan = new TextEditorTextSpan(
            positionIndex - 1,
            positionIndex,
            0,
            renderBatch.Model.ResourceUri,
            renderBatch.Model.GetAllText());
	
		var compilerServiceAutocompleteEntryList = OBSOLETE_GetAutocompleteEntries(
            word,
            textSpan);
	
		return autocompleteMenu.GetDefaultMenuRecord(compilerServiceAutocompleteEntryList);
	}
    
    public ValueTask<MenuRecord> GetQuickActionsSlashRefactorMenu(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        TextEditorViewModel viewModelModifier,
        CursorModifierBagTextEditor cursorModifierBag)
    {
		var primaryCursorModifier = cursorModifierBag.CursorModifier;
		var compilerService = modelModifier.CompilerService;
	
		var compilerServiceResource = viewModelModifier is null
			? null
			: compilerService.GetResource(modelModifier.ResourceUri);

		int? primaryCursorPositionIndex = modelModifier is null || viewModelModifier is null
			? null
			: modelModifier.GetPositionIndex(primaryCursorModifier);

		var syntaxNode = primaryCursorPositionIndex is null || __CSharpBinder is null || compilerServiceResource?.CompilationUnit is null
			? null
			: __CSharpBinder.GetSyntaxNode(null, primaryCursorPositionIndex.Value, compilerServiceResource.ResourceUri, (CSharpResource)compilerServiceResource);
			
		var menuOptionList = new List<MenuOptionRecord>();
			
		menuOptionList.Add(new MenuOptionRecord(
			"QuickActionsSlashRefactorMenu",
			MenuOptionKind.Other));
			
		if (syntaxNode is null)
		{
			menuOptionList.Add(new MenuOptionRecord(
				"syntaxNode was null",
				MenuOptionKind.Other,
				onClickFunc: async () => {}));
		}
		else
		{
			if (syntaxNode.SyntaxKind == SyntaxKind.TypeClauseNode)
			{
				var allTypeDefinitions = __CSharpBinder.AllTypeDefinitions;
				
				var typeClauseNode = (TypeClauseNode)syntaxNode;
				
				if (allTypeDefinitions.TryGetValue(typeClauseNode.TypeIdentifierToken.TextSpan.GetText(), out var typeDefinitionNode))
				{
					var usingStatementText = $"using {typeDefinitionNode.NamespaceName};";
						
					menuOptionList.Add(new MenuOptionRecord(
						$"Copy: {usingStatementText}",
						MenuOptionKind.Other,
						onClickFunc: async () =>
						{
							await _clipboardService.SetClipboard(usingStatementText).ConfigureAwait(false);
						}));
				}
				else
				{
					menuOptionList.Add(new MenuOptionRecord(
						"type not found",
						MenuOptionKind.Other,
						onClickFunc: async () => {}));
				}
			}
			else
			{
				menuOptionList.Add(new MenuOptionRecord(
					syntaxNode.SyntaxKind.ToString(),
					MenuOptionKind.Other,
					onClickFunc: async () => {}));
			}
		}
		
		MenuRecord menu;
		
		if (menuOptionList.Count == 0)
			menu = new MenuRecord(MenuRecord.NoMenuOptionsExistList);
		else
			menu = new MenuRecord(menuOptionList);
    
    	return ValueTask.FromResult(menu);
    }
	
	public async ValueTask OnInspect(
		TextEditorEditContext editContext,
		TextEditorModel modelModifier,
		TextEditorViewModel viewModelModifier,
		MouseEventArgs mouseEventArgs,
		TextEditorComponentData componentData,
		ILuthetusTextEditorComponentRenderers textEditorComponentRenderers,
        ResourceUri resourceUri)
    {
    	// Lazily calculate row and column index a second time. Otherwise one has to calculate it every mouse moved event.
        var rowAndColumnIndex = await EventUtils.CalculateRowAndColumnIndex(
				resourceUri,
				viewModelModifier.ViewModelKey,
				mouseEventArgs,
				componentData,
				editContext)
			.ConfigureAwait(false);

		var textEditorDimensions = viewModelModifier.TextEditorDimensions;
		var scrollbarDimensions = viewModelModifier.ScrollbarDimensions;
	
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

                    viewModelModifier.TooltipViewModel = new(
	                    modelModifier.CompilerService.DiagnosticRendererType ?? textEditorComponentRenderers.DiagnosticRendererType,
	                    parameterMap,
	                    relativeCoordinatesOnClick,
	                    null,
	                    componentData.ContinueRenderingTooltipAsync);
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

                    viewModelModifier.TooltipViewModel = new(
                        typeof(Luthetus.Extensions.CompilerServices.Displays.SymbolDisplay),
                        parameters,
                        relativeCoordinatesOnClick,
                        null,
                        componentData.ContinueRenderingTooltipAsync);
                }
            }
        }

        if (!foundMatch)
        {
			viewModelModifier.TooltipViewModel = null;
        }

        // TODO: Measure the tooltip, and reposition if it would go offscreen.
    }
    
    public async ValueTask GoToDefinition(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        TextEditorViewModel viewModelModifier,
        CursorModifierBagTextEditor cursorModifierBag)
    {
    	var primaryCursorModifier = cursorModifierBag.CursorModifier;
    	
        var cursorPositionIndex = modelModifier.GetPositionIndex(primaryCursorModifier);

        var foundMatch = false;
        
        var resource = GetResource(modelModifier.ResourceUri);
        var compilationUnitLocal = (CSharpCompilationUnit)resource.CompilationUnit;
        
        var symbolList = compilationUnitLocal.SymbolList;
        var foundSymbol = default(Symbol);
        
        foreach (var symbol in symbolList)
        {
            if (cursorPositionIndex >= symbol.TextSpan.StartingIndexInclusive &&
                cursorPositionIndex < symbol.TextSpan.EndingIndexExclusive)
            {
                foundMatch = true;
				foundSymbol = symbol;
            }
        }
        
        if (!foundMatch)
        	return;
    
    	var symbolLocal = foundSymbol;
		var targetNode = SymbolDisplay.GetTargetNode(_textEditorService, symbolLocal);
		var definitionNode = SymbolDisplay.GetDefinitionNode(_textEditorService, symbolLocal, targetNode);
		
		if (definitionNode is null)
			return;
			
		// TODO: Do not duplicate this code from SyntaxViewModel.HandleOnClick(...)
		
		string? resourceUriValue = null;
		var indexInclusiveStart = -1;
		
		if (definitionNode.SyntaxKind == SyntaxKind.TypeDefinitionNode)
		{
			var typeDefinitionNode = (TypeDefinitionNode)definitionNode;
			resourceUriValue = typeDefinitionNode.TypeIdentifierToken.TextSpan.ResourceUri.Value;
			indexInclusiveStart = typeDefinitionNode.TypeIdentifierToken.TextSpan.StartingIndexInclusive;
		}
		else if (definitionNode.SyntaxKind == SyntaxKind.VariableDeclarationNode)
		{
			var variableDeclarationNode = (VariableDeclarationNode)definitionNode;
			resourceUriValue = variableDeclarationNode.IdentifierToken.TextSpan.ResourceUri.Value;
			indexInclusiveStart = variableDeclarationNode.IdentifierToken.TextSpan.StartingIndexInclusive;
		}
		else if (definitionNode.SyntaxKind == SyntaxKind.NamespaceStatementNode)
		{
			var namespaceStatementNode = (NamespaceStatementNode)definitionNode;
			resourceUriValue = namespaceStatementNode.IdentifierToken.TextSpan.ResourceUri.Value;
			indexInclusiveStart = namespaceStatementNode.IdentifierToken.TextSpan.StartingIndexInclusive;
		}
		else if (definitionNode.SyntaxKind == SyntaxKind.FunctionDefinitionNode)
		{
			var functionDefinitionNode = (FunctionDefinitionNode)definitionNode;
			resourceUriValue = functionDefinitionNode.FunctionIdentifierToken.TextSpan.ResourceUri.Value;
			indexInclusiveStart = functionDefinitionNode.FunctionIdentifierToken.TextSpan.StartingIndexInclusive;
		}
		else if (definitionNode.SyntaxKind == SyntaxKind.ConstructorDefinitionNode)
		{
			var constructorDefinitionNode = (ConstructorDefinitionNode)definitionNode;
			resourceUriValue = constructorDefinitionNode.FunctionIdentifier.TextSpan.ResourceUri.Value;
			indexInclusiveStart = constructorDefinitionNode.FunctionIdentifier.TextSpan.StartingIndexInclusive;
		}
		
		if (resourceUriValue is null || indexInclusiveStart == -1)
			return;
		
		_textEditorService.TextEditorWorker.PostUnique(nameof(SyntaxViewModel), async editContext =>
		{
			await _textEditorService.OpenInEditorAsync(
					editContext,
					resourceUriValue,
					true,
					indexInclusiveStart,
					new Category("main"),
					Key<TextEditorViewModel>.NewKey())
				.ContinueWith(_ => _textEditorService.ViewModelApi.SetCursorShouldBlink(false));
		});
    }
    
    /// <summary>
    /// This implementation is NOT thread safe.
    /// </summary>
    public ValueTask ParseAsync(TextEditorEditContext editContext, TextEditorModel modelModifier, bool shouldApplySyntaxHighlighting)
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
    
    public async ValueTask FastParseAsync(TextEditorEditContext editContext, ResourceUri resourceUri, IFileSystemProvider fileSystemProvider)
	{
		var content = await fileSystemProvider.File
            .ReadAllTextAsync(resourceUri.Value)
            .ConfigureAwait(false);
	
		if (!_resourceMap.ContainsKey(resourceUri))
			return;

		var cSharpCompilationUnit = new CSharpCompilationUnit(resourceUri);
		
		var lexerOutput = CSharpLexer.Lex(resourceUri, content);
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
			
			ResourceParsed?.Invoke();
        }
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
    
    public List<AutocompleteEntry>? OBSOLETE_GetAutocompleteEntries(string word, TextEditorTextSpan textSpan)
    {
    	if (word is null)
			return null;
			
    	var boundScope = __CSharpBinder.GetScope(null, textSpan);

        if (!boundScope.ConstructorWasInvoked)
            return null;
        
        var autocompleteEntryList = new List<AutocompleteEntry>();

        var targetScope = boundScope;
        
        if (textSpan.GetText() == ".")
        {
        	var textEditorModel = _textEditorService.ModelApi.GetOrDefault(textSpan.ResourceUri);
	    	if (textEditorModel is null)
	    		return autocompleteEntryList.DistinctBy(x => x.DisplayName).ToList();
	    	
	    	var compilerService = textEditorModel.CompilerService;
	    	
	    	var compilerServiceResource = compilerService.GetResource(textEditorModel.ResourceUri);
	    	if (compilerServiceResource is null)
	    		return autocompleteEntryList.DistinctBy(x => x.DisplayName).ToList();
	
	    	var targetNode = __CSharpBinder.GetSyntaxNode(
	    		(CSharpCompilationUnit)compilerServiceResource.CompilationUnit,
	    		textSpan.StartingIndexInclusive - 1,
	    		textSpan.ResourceUri,
	    		(CSharpResource)compilerServiceResource);
	    		
	    	if (targetNode is null)
	    		return autocompleteEntryList.DistinctBy(x => x.DisplayName).ToList();
        
        	TypeClauseNode? typeClauseNode = null;
	
			if (targetNode.SyntaxKind == SyntaxKind.VariableReferenceNode)
				typeClauseNode = ((VariableReferenceNode)targetNode).VariableDeclarationNode?.TypeClauseNode;
			else if (targetNode.SyntaxKind == SyntaxKind.VariableDeclarationNode)
				typeClauseNode = ((VariableDeclarationNode)targetNode).TypeClauseNode;
			else if (targetNode.SyntaxKind == SyntaxKind.TypeClauseNode)
				typeClauseNode = (TypeClauseNode)targetNode;
			else if (targetNode.SyntaxKind == SyntaxKind.TypeDefinitionNode)
				typeClauseNode = ((TypeDefinitionNode)targetNode).ToTypeClause();
			else if (targetNode.SyntaxKind == SyntaxKind.ConstructorDefinitionNode)
				typeClauseNode = ((ConstructorDefinitionNode)targetNode).ReturnTypeClauseNode;
			
			if (typeClauseNode is null)
				return autocompleteEntryList.DistinctBy(x => x.DisplayName).ToList();
			
			var maybeTypeDefinitionNode = __CSharpBinder.GetDefinitionNode((CSharpCompilationUnit)compilerServiceResource.CompilationUnit, typeClauseNode.TypeIdentifierToken.TextSpan, SyntaxKind.TypeClauseNode);
			if (maybeTypeDefinitionNode is null || maybeTypeDefinitionNode.SyntaxKind != SyntaxKind.TypeDefinitionNode)
				return autocompleteEntryList.DistinctBy(x => x.DisplayName).ToList();
			
			var typeDefinitionNode = (TypeDefinitionNode)maybeTypeDefinitionNode;
			var memberList = typeDefinitionNode.GetMemberList();
			
			autocompleteEntryList.AddRange(
	        	memberList
	        	.Select(node => 
	        	{
	        		if (node.SyntaxKind == SyntaxKind.VariableDeclarationNode)
	        		{
	        			var variableDeclarationNode = (VariableDeclarationNode)node;
	        			return variableDeclarationNode.IdentifierToken.TextSpan.GetText();
	        		}
	        		else if (node.SyntaxKind == SyntaxKind.TypeDefinitionNode)
	        		{
	        			var typeDefinitionNode = (TypeDefinitionNode)node;
	        			return typeDefinitionNode.TypeIdentifierToken.TextSpan.GetText();
	        		}
	        		else if (node.SyntaxKind == SyntaxKind.FunctionDefinitionNode)
	        		{
	        			var functionDefinitionNode = (FunctionDefinitionNode)node;
	        			return functionDefinitionNode.FunctionIdentifierToken.TextSpan.GetText();
	        		}
	        		else
	        		{
	        			return string.Empty;
	        		}
	        	})
	            .ToArray()
	            //.Where(x => x.Contains(word, StringComparison.InvariantCulture))
	            .Distinct()
	            .Take(5)
	            .Select(x =>
	            {
	                return new AutocompleteEntry(
	                    x,
	                    AutocompleteEntryKind.Variable,
	                    null);
	            }));
        }
		else
		{
			while (targetScope.ConstructorWasInvoked)
	        {
	            autocompleteEntryList.AddRange(
	            	__CSharpBinder.GetVariableDeclarationNodesByScope(cSharpCompilationUnit: null, textSpan.ResourceUri, targetScope.IndexKey)
	            	.Select(x => x.IdentifierToken.TextSpan.GetText())
	                .ToArray()
	                .Where(x => x.Contains(word, StringComparison.InvariantCulture))
	                .Distinct()
	                .Take(5)
	                .Select(x =>
	                {
	                    return new AutocompleteEntry(
	                        x,
	                        AutocompleteEntryKind.Variable,
	                        null);
	                }));
	
	            autocompleteEntryList.AddRange(
	                __CSharpBinder.GetFunctionDefinitionNodesByScope(cSharpCompilationUnit: null, textSpan.ResourceUri, targetScope.IndexKey)
	            	.Select(x => x.FunctionIdentifierToken.TextSpan.GetText())
	                .ToArray()
	                .Where(x => x.Contains(word, StringComparison.InvariantCulture))
	                .Distinct()
	                .Take(5)
	                .Select(x =>
	                {
	                    return new AutocompleteEntry(
	                        x,
	                        AutocompleteEntryKind.Function,
	                        null);
	                }));
	
				if (targetScope.ParentIndexKey == -1)
					targetScope = default;
				else
	            	targetScope = __CSharpBinder.GetScopeByScopeIndexKey(compilationUnit: null, textSpan.ResourceUri, targetScope.ParentIndexKey);
	        }
        
	        var allTypeDefinitions = __CSharpBinder.AllTypeDefinitions;
	
	        autocompleteEntryList.AddRange(
	            allTypeDefinitions
	            .Where(x => x.Key.Contains(word, StringComparison.InvariantCulture))
	            .Distinct()
	            .Take(5)
	            .Select(x =>
	            {
	                return new AutocompleteEntry(
	                    x.Key,
	                    AutocompleteEntryKind.Type,
	                    () =>
	                    {
	                    	// TODO: The namespace code is buggy at the moment.
	                    	//       It is annoying how this keeps adding the wrong namespace.
	                    	//       Just have it do nothing for now. (2024-08-24)
	                    	// ===============================================================
	                        /*if (boundScope.EncompassingNamespaceStatementNode.IdentifierToken.TextSpan.GetText() == x.Key.NamespaceIdentifier ||
	                            boundScope.CurrentUsingStatementNodeList.Any(usn => usn.NamespaceIdentifier.TextSpan.GetText() == x.Key.NamespaceIdentifier))
	                        {
	                            return Task.CompletedTask;
	                        }
	
	                        _textEditorService.PostUnique(
	                            "Add using statement",
	                            editContext =>
	                            {
	                                var modelModifier = editContext.GetModelModifier(textSpan.ResourceUri);
	
	                                if (modelModifier is null)
	                                    return Task.CompletedTask;
	
	                                var viewModelList = _textEditorService.ModelApi.GetViewModelsOrEmpty(textSpan.ResourceUri);
	
	                                var cursor = new TextEditorCursor(0, 0, true);
	                                var cursorModifierBag = new CursorModifierBagTextEditor(
	                                    Key<TextEditorViewModel>.Empty,
	                                    new List<TextEditorCursorModifier> { new(cursor) });
	
	                                var textToInsert = $"using {x.Key.NamespaceIdentifier};\n";
	
	                                modelModifier.Insert(
	                                    textToInsert,
	                                    cursorModifierBag,
	                                    cancellationToken: CancellationToken.None);
	
	                                foreach (var unsafeViewModel in viewModelList)
	                                {
	                                    var viewModelModifier = editContext.GetViewModelModifier(unsafeViewModel.ViewModelKey);
	                                    var viewModelCursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier);
	
	                                    if (viewModelModifier is null || viewModelCursorModifierBag is null)
	                                        continue;
	
	                                    foreach (var cursorModifier in viewModelCursorModifierBag.List)
	                                    {
	                                        for (int i = 0; i < textToInsert.Length; i++)
	                                        {
	                                            _textEditorService.ViewModelApi.MoveCursor(
	                                            	new KeyboardEventArgs
	                                                {
	                                                    Key = KeyboardKeyFacts.MovementKeys.ARROW_RIGHT,
	                                                },
											        editContext,
											        modelModifier,
											        viewModelModifier,
											        viewModelCursorModifierBag);
	                                        }
	                                    }
	
	                                    editContext.TextEditorService.ModelApi.ApplySyntaxHighlighting(
	                                        editContext,
	                                        modelModifier);
	                                }
	
	                                return Task.CompletedTask;
	                            });*/
							return Task.CompletedTask;
	                    });
	            }));
	    }
            
        AddSnippets(autocompleteEntryList, word, textSpan);

        return autocompleteEntryList.DistinctBy(x => x.DisplayName).ToList();
    }
    
    private void AddSnippets(List<AutocompleteEntry> autocompleteEntryList, string word, TextEditorTextSpan textSpan)
    {
    	if ("prop".Contains(word))
    	{
	    	autocompleteEntryList.Add(new AutocompleteEntry(
	        	"prop",
		        AutocompleteEntryKind.Snippet,
		        () => PropSnippet(word, textSpan, "public TYPE NAME { get; set; }")));
		}
		
		if ("propnn".Contains(word))
    	{
	    	autocompleteEntryList.Add(new AutocompleteEntry(
	        	"propnn",
		        AutocompleteEntryKind.Snippet,
		        () => PropSnippet(word, textSpan, "public TYPE NAME { get; set; } = null!;")));
		}
    }
    
    private Task PropSnippet(string word, TextEditorTextSpan textSpan, string textToInsert)
    {
        _textEditorService.TextEditorWorker.PostUnique(
	        nameof(PropSnippet),
	        (Func<TextEditorEditContext, ValueTask>)(	        editContext =>
	        {
	            var modelModifier = editContext.GetModelModifier(textSpan.ResourceUri);
	
	            if (modelModifier is null)
	                return ValueTask.CompletedTask;
	
	            var viewModelList = _textEditorService.ModelApi.GetViewModelsOrEmpty(textSpan.ResourceUri);
	            
	            var viewModel = viewModelList.FirstOrDefault(x => x.Category.Value == "main")
	            	?? viewModelList.FirstOrDefault();
	            
	            if (viewModel is null)
	            	return ValueTask.CompletedTask;
	            	
	            var viewModelModifier = editContext.GetViewModelModifier(viewModel.ViewModelKey);

	            var primaryCursorModifier = editContext.GetCursorModifierBag(viewModelModifier).CursorModifier;
	            
	            if (viewModelModifier is null || primaryCursorModifier is null)
	            	return ValueTask.CompletedTask;
	
	            var cursorModifierBag = new CursorModifierBagTextEditor(
                    Key<TextEditorViewModel>.Empty,
	                primaryCursorModifier);
	                
	            var cursorPositionIndex = modelModifier.GetPositionIndex(primaryCursorModifier);
	            var behindPositionIndex = cursorPositionIndex - 1;
				
				var behindCursor = new TextEditorCursor(
	        		primaryCursorModifier.LineIndex,
	        		primaryCursorModifier.ColumnIndex - 1,
	        		primaryCursorModifier.IsPrimaryCursor);
	            		
	            modelModifier.Insert(
	                textToInsert,
	                cursorModifierBag,
	                cancellationToken: CancellationToken.None);
	                
	            if (behindPositionIndex > 0 && modelModifier.GetCharacter(behindPositionIndex) == 'p')
	            {
	            	modelModifier.Delete(
				        new CursorModifierBagTextEditor(
                            Key<TextEditorViewModel>.Empty,
			                new(behindCursor)),
				        1,
				        expandWord: false,
                        TextEditorModel.DeleteKind.Delete);
	            }
	
	            modelModifier.CompilerService.ResourceWasModified(
	            	(ResourceUri)modelModifier.ResourceUri,
	            	(IReadOnlyList<TextEditorTextSpan>)Array.Empty<TextEditorTextSpan>());
	            	
	            return ValueTask.CompletedTask;
	        }));
	        
	    return Task.CompletedTask;
    }
}