using Microsoft.AspNetCore.Components.Web;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Commands.Models;
using Luthetus.TextEditor.RazorLib.ComponentRenderers.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.Extensions.CompilerServices;
using Luthetus.Extensions.CompilerServices.Syntax;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes;
using Luthetus.CompilerServices.DotNetSolution.SyntaxActors;

namespace Luthetus.CompilerServices.DotNetSolution.CompilerServiceCase;

public sealed class DotNetSolutionCompilerService : ICompilerService
{
    private readonly ITextEditorService _textEditorService;
    
    private readonly Dictionary<ResourceUri, DotNetSolutionResource> _resourceMap = new();
    private readonly object _resourceMapLock = new();

	public DotNetSolutionCompilerService(ITextEditorService textEditorService)
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

            _resourceMap.Add(resourceUri, new DotNetSolutionResource(resourceUri, this));
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
    	_textEditorService.WorkerArbitrary.PostUnique(nameof(ICompilerService), editContext =>
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
		return autocompleteMenu.GetDefaultMenuRecord();
	}
    
    public ValueTask<MenuRecord> GetQuickActionsSlashRefactorMenu(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        TextEditorViewModel viewModelModifier,
        CursorModifierBagTextEditor cursorModifierBag)
    {
    	return ValueTask.FromResult(new MenuRecord(MenuRecord.NoMenuOptionsExistList));
    }
    
    public ValueTask OnInspect(
		TextEditorEditContext editContext,
		TextEditorModel modelModifier,
		TextEditorViewModel viewModelModifier,
		MouseEventArgs mouseEventArgs,
		TextEditorComponentData componentData,
		ILuthetusTextEditorComponentRenderers textEditorComponentRenderers,
        ResourceUri resourceUri)
    {
    	return ValueTask.CompletedTask;
    }
    
    public ValueTask ShowCallingSignature(
		TextEditorEditContext editContext,
		TextEditorModel modelModifier,
		TextEditorViewModel viewModelModifier,
		int positionIndex,
		TextEditorComponentData componentData,
		ILuthetusTextEditorComponentRenderers textEditorComponentRenderers,
        ResourceUri resourceUri)
    {
    	return ValueTask.CompletedTask;
    }
    
    public ValueTask GoToDefinition(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        TextEditorViewModel viewModelModifier,
        CursorModifierBagTextEditor cursorModifierBag,
        Category category)
    {
    	return ValueTask.CompletedTask;
    }

	public ValueTask ParseAsync(TextEditorEditContext editContext, TextEditorModel modelModifier, bool shouldApplySyntaxHighlighting)
    {
    	var lexer = new DotNetSolutionLexer(modelModifier.ResourceUri, modelModifier.GetAllText());
    	lexer.Lex();
    	
        var parser = new DotNetSolutionParser(lexer);
        var compilationUnit = parser.Parse();
    
    	lock (_resourceMapLock)
		{
			if (_resourceMap.ContainsKey(modelModifier.ResourceUri))
			{
				var resource = (CompilerServiceResource)_resourceMap[modelModifier.ResourceUri];
				
				resource.CompilationUnit = new ExtendedCompilationUnit
				{
					TokenList = lexer.SyntaxTokenList
				};
			}
		}
		
		editContext.TextEditorService.ModelApi.ApplySyntaxHighlighting(
			editContext,
			modelModifier);

		ResourceParsed?.Invoke();
		
		return ValueTask.CompletedTask;
    }
	
	public ValueTask FastParseAsync(TextEditorEditContext editContext, ResourceUri resourceUri, IFileSystemProvider fileSystemProvider)
	{
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
    	return null;
    }

	/// <summary>
	/// Returns the text span at which the definition exists in the source code.
	/// </summary>
    public TextEditorTextSpan? GetDefinitionTextSpan(TextEditorTextSpan textSpan, ICompilerServiceResource compilerServiceResource)
    {
    	return null;
    }

	public Scope GetScopeByPositionIndex(ResourceUri resourceUri, int positionIndex)
    {
    	return default;
    }
	
	/// <summary>
    /// Returns the <see cref="ISyntaxNode"/> that represents the definition in the <see cref="CompilationUnit"/>.
    ///
    /// The option argument 'symbol' can be provided if available. It might provide additional information to the method's implementation
    /// that is necessary to find certain nodes (ones that are in a separate file are most common to need a symbol to find).
    /// </summary>
    public ISyntaxNode? GetDefinitionNode(TextEditorTextSpan textSpan, ICompilerServiceResource compilerServiceResource, Symbol? symbol = null)
    {
    	return null;
    }
}