using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.Autocompletes.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.TextEditor.RazorLib.CompilerServices.Facts;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.CompilerServices.CSharp.BinderCase;
using Luthetus.CompilerServices.CSharp.LexerCase;
using Luthetus.CompilerServices.CSharp.ParserCase;
using Luthetus.CompilerServices.CSharp.RuntimeAssemblies;

namespace Luthetus.CompilerServices.CSharp.CompilerServiceCase;

public sealed class CSharpCompilerService : CompilerService
{
    /// <summary>
    /// TODO: The CSharpBinder should be private, but for now I'm making it public to be usable in the CompilerServiceExplorer Blazor component.
    /// </summary>
    public readonly CSharpBinder CSharpBinder = new();

    public CSharpCompilerService(ITextEditorService textEditorService)
        : base(textEditorService)
    {
        _compilerServiceOptions = new()
        {
            RegisterResourceFunc = resourceUri => new CSharpResource(resourceUri, this),
        };

        RuntimeAssembliesLoaderFactory.LoadDotNet6(CSharpBinder);
    }

	public override Type? SymbolRendererType { get; protected set; }
    public override Type? DiagnosticRendererType { get; protected set; }
    
    public event Action? CursorMovedInSyntaxTree;
    
    public void SetSymbolRendererType(Type? symbolRendererType)
    {
    	SymbolRendererType = symbolRendererType;
    }
    
    public void SetDiagnosticRendererType(Type? diagnosticRendererType)
    {
    	DiagnosticRendererType = diagnosticRendererType;
    }
    
    public override ValueTask ParseAsync(ITextEditorEditContext editContext, TextEditorModelModifier modelModifier, bool shouldApplySyntaxHighlighting)
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
			CSharpBinder.StartCompilationUnit(resourceUri);
			CSharpParser.Parse(cSharpCompilationUnit, CSharpBinder, ref lexerOutput);
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

			OnResourceParsed();
        }
		
        return ValueTask.CompletedTask;
	}

    public override List<AutocompleteEntry> GetAutocompleteEntries(string word, TextEditorTextSpan textSpan)
    {
    	var boundScope = CSharpBinder.GetScope(null, textSpan);

        if (!boundScope.ConstructorWasInvoked)
            return base._emptyAutocompleteEntryList;
        
        var autocompleteEntryList = new List<AutocompleteEntry>();

        var targetScope = boundScope;
        
        if (textSpan.GetText() == ".")
        {
        	var textEditorModel = _textEditorService.ModelApi.GetOrDefault(textSpan.ResourceUri);
	    	if (textEditorModel is null)
	    		return autocompleteEntryList.DistinctBy(x => x.DisplayName).ToList();
	    	
	    	var compilerService = textEditorModel.CompilerService;
	    	
	    	var compilerServiceResource = compilerService.GetCompilerServiceResourceFor(textEditorModel.ResourceUri);
	    	if (compilerServiceResource is null)
	    		return autocompleteEntryList.DistinctBy(x => x.DisplayName).ToList();
	
	    	var targetNode = CSharpBinder.GetSyntaxNode(
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
			
			var maybeTypeDefinitionNode = CSharpBinder.GetDefinitionNode((CSharpCompilationUnit)compilerServiceResource.CompilationUnit, typeClauseNode.TypeIdentifierToken.TextSpan, SyntaxKind.TypeClauseNode);
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
	            	CSharpBinder.GetVariableDeclarationNodesByScope(cSharpCompilationUnit: null, textSpan.ResourceUri, targetScope.IndexKey)
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
	                CSharpBinder.GetFunctionDefinitionNodesByScope(cSharpCompilationUnit: null, textSpan.ResourceUri, targetScope.IndexKey)
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
	            	targetScope = CSharpBinder.GetScopeByScopeIndexKey(compilationUnit: null, textSpan.ResourceUri, targetScope.ParentIndexKey);
	        }
        
	        var allTypeDefinitions = CSharpBinder.AllTypeDefinitions;
	
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
	                                    var viewModelCursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
	
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
	        editContext =>
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

	            var primaryCursorModifier = editContext.GetPrimaryCursorModifier(
	            	editContext.GetCursorModifierBag(viewModelModifier?.ViewModel));
	            
	            if (viewModelModifier is null || primaryCursorModifier is null)
	            	return ValueTask.CompletedTask;
	
	            var cursorModifierBag = new CursorModifierBagTextEditor(
	                Key<TextEditorViewModel>.Empty,
	                new List<TextEditorCursorModifier> { primaryCursorModifier });
	                
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
			                new List<TextEditorCursorModifier> { new(behindCursor) }),
				        1,
				        expandWord: false,
				        TextEditorModelModifier.DeleteKind.Delete);
	            }
	
	            modelModifier.CompilerService.ResourceWasModified(
	            	modelModifier.ResourceUri,
	            	Array.Empty<TextEditorTextSpan>());
	            	
	            return ValueTask.CompletedTask;
	        });
	        
	    return Task.CompletedTask;
    }
}