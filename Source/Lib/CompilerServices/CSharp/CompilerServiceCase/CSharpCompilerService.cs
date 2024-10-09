using System.Collections.Immutable;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.Autocompletes.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
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
        Binder = CSharpBinder;

        _compilerServiceOptions = new()
        {
            RegisterResourceFunc = resourceUri => new CSharpResource(resourceUri, this),
            GetLexerFunc = (resource, sourceText) => new CSharpLexer(resource.ResourceUri, sourceText),
            GetParserFunc = (resource, lexer) => new CSharpParser((CSharpLexer)lexer),
            GetBinderFunc = (resource, parser) => Binder,
			OnAfterLexAction = (resource, lexer) =>
            {
                var cSharpResource = (CSharpResource)resource;
                var cSharpLexer = (CSharpLexer)lexer;

                cSharpResource.EscapeCharacterList = cSharpLexer.EscapeCharacterList;
            },
        };

        RuntimeAssembliesLoaderFactory.LoadDotNet6(CSharpBinder);
    }

    public event Action? CursorMovedInSyntaxTree;

    public override ImmutableArray<AutocompleteEntry> GetAutocompleteEntries(string word, TextEditorTextSpan textSpan)
    {
        var boundScope = CSharpBinder.GetScope(textSpan);

        if (boundScope is null)
            return ImmutableArray<AutocompleteEntry>.Empty;

        var autocompleteEntryList = new List<AutocompleteEntry>();

        var targetScope = boundScope;

        while (targetScope is not null)
        {
            autocompleteEntryList.AddRange(
            	CSharpBinder.GetVariableDeclarationNodesByScope(targetScope.ResourceUri, targetScope.Key)
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
                CSharpBinder.GetFunctionDefinitionNodesByScope(targetScope.ResourceUri, targetScope.Key)
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

			if (targetScope.ParentKey is null)
				targetScope = null;
			else
            	targetScope = CSharpBinder.GetScope(targetScope.ResourceUri, targetScope.ParentKey.Value);
        }
        
        var allTypeDefinitions = CSharpBinder.AllTypeDefinitions;

        autocompleteEntryList.AddRange(
            allTypeDefinitions
            .Where(x => x.Key.TypeIdentifier.Contains(word, StringComparison.InvariantCulture))
            .Distinct()
            .Take(5)
            .Select(x =>
            {
                return new AutocompleteEntry(
                    x.Key.TypeIdentifier,
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
            
        AddSnippets(autocompleteEntryList, word, textSpan);

        return autocompleteEntryList.DistinctBy(x => x.DisplayName).ToImmutableArray();
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
    	_textEditorService.PostUnique(
	        nameof(PropSnippet),
	        editContext =>
	        {
	            var modelModifier = editContext.GetModelModifier(textSpan.ResourceUri);
	
	            if (modelModifier is null)
	                return Task.CompletedTask;
	
	            var viewModelList = _textEditorService.ModelApi.GetViewModelsOrEmpty(textSpan.ResourceUri);
	            
	            var viewModel = viewModelList.FirstOrDefault(x => x.Category.Value == "main")
	            	?? viewModelList.FirstOrDefault();
	            
	            if (viewModel is null)
	            	return Task.CompletedTask;
	            	
	            var viewModelModifier = editContext.GetViewModelModifier(viewModel.ViewModelKey);

	            var primaryCursorModifier = editContext.GetPrimaryCursorModifier(
	            	editContext.GetCursorModifierBag(viewModelModifier?.ViewModel));
	            
	            if (viewModelModifier is null || primaryCursorModifier is null)
	            	return Task.CompletedTask;
	
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
	            	ImmutableArray<TextEditorTextSpan>.Empty);
	            	
	            return Task.CompletedTask;
	        });
	        
	    return Task.CompletedTask;
    }
}