using Luthetus.TextEditor.RazorLib.Autocompletes.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Facts;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Exceptions;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;

/// <summary>
/// If inheriting this type:
/// 	- The ICompilerServiceResource must be of type (or inherit) CompilerServiceResource.
/// 	- The ICompilationUnit must be of type (or inherit) CompilationUnit.
/// </summary>
public class CompilerService : ICompilerService
{
	protected readonly List<AutocompleteEntry> _emptyAutocompleteEntryList = new();
    protected readonly Dictionary<ResourceUri, ICompilerServiceResource> _resourceMap = new();
    protected readonly object _resourceMapLock = new();
    protected readonly ITextEditorService _textEditorService;

    protected CompilerServiceOptions _compilerServiceOptions = new();

    /// <param name="getLexerFunc">Takes as arguments the resource uri and the source text.</param>
    public CompilerService(ITextEditorService textEditorService)
    {
        _textEditorService = textEditorService;
    }

    /// <summary>
    /// Derived types can invoke <see cref="OnResourceRegistered"/> to fire this event
    /// </summary>
    public event Action? ResourceRegistered;
    /// <summary>
    /// Derived types can invoke <see cref="OnResourceParsed"/> to fire this event
    /// </summary>
    public event Action? ResourceParsed;
    /// <summary>
    /// Derived types can invoke <see cref="OnResourceDisposed"/> to fire this event
    /// </summary>
    public event Action? ResourceDisposed;

    public virtual IBinder Binder { get; protected set; } = new Binder();
    
    public virtual Type? SymbolRendererType { get; protected set; }
    public virtual Type? DiagnosticRendererType { get; protected set; }

    public virtual IReadOnlyList<ICompilerServiceResource> CompilerServiceResources => _resourceMap.Values.ToArray();

    public virtual void RegisterResource(ResourceUri resourceUri, bool shouldTriggerResourceWasModified)
    {
        lock (_resourceMapLock)
        {
            if (_resourceMap.ContainsKey(resourceUri))
                return;

            var resource = _compilerServiceOptions.RegisterResourceFunc is not null
                ? _compilerServiceOptions.RegisterResourceFunc.Invoke(resourceUri)
                : new CompilerServiceResource(resourceUri, this);

            _resourceMap.Add(resourceUri, resource);
        }

		if (shouldTriggerResourceWasModified)
	        ResourceWasModified(resourceUri, Array.Empty<TextEditorTextSpan>());
	        
        ResourceRegistered?.Invoke();
    }

    public virtual ICompilerServiceResource? GetCompilerServiceResourceFor(ResourceUri resourceUri)
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

    public virtual IReadOnlyList<SyntaxToken> GetTokensFor(ResourceUri resourceUri)
    {
        lock (_resourceMapLock)
        {
            if (!_resourceMap.ContainsKey(resourceUri))
                return Array.Empty<SyntaxToken>();

            return _resourceMap[resourceUri].GetTokens();
        }
    }

    public virtual IReadOnlyList<Symbol> GetSymbolsFor(ResourceUri resourceUri)
    {
        lock (_resourceMapLock)
        {
            if (!_resourceMap.ContainsKey(resourceUri))
                return Array.Empty<Symbol>();

            return _resourceMap[resourceUri].GetSymbols();
        }
    }

    public virtual IReadOnlyList<TextEditorDiagnostic> GetDiagnosticsFor(ResourceUri resourceUri)
    {
        lock (_resourceMapLock)
        {
            if (!_resourceMap.ContainsKey(resourceUri))
                return Array.Empty<TextEditorDiagnostic>();

            return _resourceMap[resourceUri].GetDiagnostics();
        }
    }

    public virtual void ResourceWasModified(ResourceUri resourceUri, IReadOnlyList<TextEditorTextSpan> editTextSpansList)
    {
        _textEditorService.TextEditorWorker.PostUnique(
            nameof(CompilerService),
            editContext =>
            {
				var modelModifier = editContext.GetModelModifier(resourceUri);

				if (modelModifier is null)
					return ValueTask.CompletedTask;

				return ParseAsync(editContext, modelModifier, shouldApplySyntaxHighlighting: true);
            });
    }

    public virtual void CursorWasModified(ResourceUri resourceUri, TextEditorCursor cursor)
    {
    }

    public virtual List<AutocompleteEntry> GetAutocompleteEntries(string word, TextEditorTextSpan textSpan)
    {
        return _emptyAutocompleteEntryList;
    }
    
    public virtual ValueTask ParseAsync(ITextEditorEditContext editContext, TextEditorModelModifier modelModifier, bool shouldApplySyntaxHighlighting)
	{
		_textEditorService.ModelApi.StartPendingCalculatePresentationModel(
			editContext,
	        modelModifier,
	        CompilerServiceDiagnosticPresentationFacts.PresentationKey,
			CompilerServiceDiagnosticPresentationFacts.EmptyPresentationModel);

		var presentationModel = modelModifier.PresentationModelList.First(
			x => x.TextEditorPresentationKey == CompilerServiceDiagnosticPresentationFacts.PresentationKey);

		if (presentationModel.PendingCalculation is null)
			throw new LuthetusTextEditorException($"{nameof(presentationModel)}.{nameof(presentationModel.PendingCalculation)} was not expected to be null here.");

		if (_compilerServiceOptions.GetLexerFunc is null)
            return ValueTask.CompletedTask;
            
        var resourceUri = modelModifier.ResourceUri;

        ILexer lexer;
		lock (_resourceMapLock)
		{
			if (!_resourceMap.ContainsKey(resourceUri))
				return ValueTask.CompletedTask;

			var resource = _resourceMap[resourceUri];
			lexer = _compilerServiceOptions.GetLexerFunc.Invoke(resource, presentationModel.PendingCalculation.ContentAtRequest);
		}

		lexer.Lex();
		lock (_resourceMapLock)
		{
			if (!_resourceMap.ContainsKey(resourceUri))
                return ValueTask.CompletedTask;

            var resource = _resourceMap[resourceUri];
			_compilerServiceOptions.OnAfterLexAction?.Invoke(resource, lexer);
		}

		CompilationUnit? compilationUnit = null;
		// Even if the parser throws an exception, be sure to
		// make use of the Lexer to do whatever syntax highlighting is possible.
		try
		{
			if (_compilerServiceOptions.GetParserFunc is null || Binder is null)
                return ValueTask.CompletedTask;

            IParser parser;
			lock (_resourceMapLock)
			{
				if (!_resourceMap.ContainsKey(resourceUri))
					return ValueTask.CompletedTask;

				var resource = _resourceMap[resourceUri];
				parser = _compilerServiceOptions.GetParserFunc.Invoke(resource, lexer);
			}

			compilationUnit = (CompilationUnit)parser.Parse(Binder, resourceUri);
		}
		finally
		{
			lock (_resourceMapLock)
			{
				if (_resourceMap.ContainsKey(resourceUri))
				{
					var resource = (CompilerServiceResource)_resourceMap[resourceUri];

					resource.SyntaxTokenList = lexer.SyntaxTokenList;

					if (compilationUnit is not null)
						resource.CompilationUnit = compilationUnit;

					_compilerServiceOptions.OnAfterParseAction?.Invoke(resource, compilationUnit);
				}
			}

			var diagnosticTextSpans = GetDiagnosticsFor(modelModifier.ResourceUri)
				.Select(x => x.TextSpan)
				.ToList();

			modelModifier.CompletePendingCalculatePresentationModel(
				CompilerServiceDiagnosticPresentationFacts.PresentationKey,
				CompilerServiceDiagnosticPresentationFacts.EmptyPresentationModel,
				diagnosticTextSpans);

			editContext.TextEditorService.ModelApi.ApplySyntaxHighlighting(
				editContext,
				modelModifier);

			OnResourceParsed();
        }

        return ValueTask.CompletedTask;
	}

    public virtual void DisposeResource(ResourceUri resourceUri)
    {
        lock (_resourceMapLock)
        {
            _resourceMap.Remove(resourceUri);
        }

        OnResourceDisposed();
    }

    /// <summary>
    /// A method wrapper for <see cref="ResourceRegistered"/> as to allow dervied types
    /// to invoke the event.
    /// </summary>
    protected virtual void OnResourceRegistered()
    {
        ResourceRegistered?.Invoke();
    }

    /// <summary>
    /// A method wrapper for <see cref="ResourceParsed"/> as to allow dervied types
    /// to invoke the event.
    /// </summary>
    protected virtual void OnResourceParsed()
    {
        ResourceParsed?.Invoke();
    }

    /// <summary>
    /// A method wrapper for <see cref="ResourceDisposed"/> as to allow derived types to invoke the event.
    /// </summary>
    protected virtual void OnResourceDisposed()
    {
        ResourceDisposed?.Invoke();
    }
}
