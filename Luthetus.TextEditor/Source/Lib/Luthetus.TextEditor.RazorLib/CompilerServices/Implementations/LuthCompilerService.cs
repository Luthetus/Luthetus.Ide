using Luthetus.TextEditor.RazorLib.Autocompletes.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Facts;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;

public class LuthCompilerService : ILuthCompilerService
{
    protected readonly Dictionary<ResourceUri, ILuthCompilerServiceResource> _resourceMap = new();
    protected readonly object _resourceMapLock = new();
    protected readonly ITextEditorService _textEditorService;

    /// <summary>
    /// Takes as arguments the resource uri and the source text.
    /// </summary>
    protected readonly Func<ResourceUri, string, ILuthLexer>? _getLexerFunc;
    protected readonly Func<ILuthLexer, ILuthParser>? _getParserFunc;

    /// <param name="getLexerFunc">Takes as arguments the resource uri and the source text.</param>
    public LuthCompilerService(
        ITextEditorService textEditorService,
        Func<ResourceUri, string, ILuthLexer>? getLexerFunc,
        Func<ILuthLexer, ILuthParser>? getParserFunc)
    {
        _textEditorService = textEditorService;
        _getLexerFunc = getLexerFunc;
        _getParserFunc = getParserFunc;
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

    public virtual ILuthBinder? Binder { get; protected set; } = new LuthBinder();

    public virtual ImmutableArray<ILuthCompilerServiceResource> CompilerServiceResources =>
        _resourceMap.Values.ToImmutableArray();

    public virtual void RegisterResource(ResourceUri resourceUri)
    {
        lock (_resourceMapLock)
        {
            if (_resourceMap.ContainsKey(resourceUri))
                return;

            _resourceMap.Add(
                resourceUri,
                new LuthCompilerServiceResource(resourceUri, this));
        }

        QueueParseRequest(resourceUri);
        ResourceRegistered?.Invoke();
    }

    public virtual ILuthCompilerServiceResource? GetCompilerServiceResourceFor(ResourceUri resourceUri)
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

    public virtual ImmutableArray<TextEditorTextSpan> GetSyntacticTextSpansFor(ResourceUri resourceUri)
    {
        lock (_resourceMapLock)
        {
            if (!_resourceMap.ContainsKey(resourceUri))
                return ImmutableArray<TextEditorTextSpan>.Empty;

            return _resourceMap[resourceUri].GetTokenTextSpans();
        }
    }

    public virtual ImmutableArray<ITextEditorSymbol> GetSymbolsFor(ResourceUri resourceUri)
    {
        lock (_resourceMapLock)
        {
            if (!_resourceMap.ContainsKey(resourceUri))
                return ImmutableArray<ITextEditorSymbol>.Empty;

            return _resourceMap[resourceUri].GetSymbols();
        }
    }

    public virtual ImmutableArray<TextEditorDiagnostic> GetDiagnosticsFor(ResourceUri resourceUri)
    {
        lock (_resourceMapLock)
        {
            if (!_resourceMap.ContainsKey(resourceUri))
                return ImmutableArray<TextEditorDiagnostic>.Empty;

            return _resourceMap[resourceUri].GetDiagnostics();
        }
    }

    public virtual void ResourceWasModified(ResourceUri resourceUri, ImmutableArray<TextEditorTextSpan> editTextSpansList)
    {
        QueueParseRequest(resourceUri);
    }

    public virtual void CursorWasModified(ResourceUri resourceUri, TextEditorCursor cursor)
    {
    }

    public virtual ImmutableArray<AutocompleteEntry> GetAutocompleteEntries(string word, TextEditorTextSpan textSpan)
    {
        return ImmutableArray<AutocompleteEntry>.Empty;
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
    /// A method wrapper for <see cref="ResourceDisposed"/> as to allow dervied types
    /// to invoke the event.
    /// </summary>
    protected virtual void OnResourceDisposed()
    {
        ResourceDisposed?.Invoke();
    }

    protected virtual void QueueParseRequest(ResourceUri resourceUri)
    {
        _textEditorService.Post(
            nameof(QueueParseRequest),
            async editContext =>
            {
                var modelModifier = editContext.GetModelModifier(resourceUri);

                if (modelModifier is null)
                    return;

                await _textEditorService.ModelApi.CalculatePresentationModelFactory(
                        modelModifier.ResourceUri,
                        CompilerServiceDiagnosticPresentationFacts.PresentationKey)
                    .Invoke(editContext)
                    .ConfigureAwait(false);

                var pendingCalculation = modelModifier.PresentationModelsList.FirstOrDefault(x =>
                    x.TextEditorPresentationKey == CompilerServiceDiagnosticPresentationFacts.PresentationKey)
                    ?.PendingCalculation;

                if (pendingCalculation is null)
                    pendingCalculation = new(modelModifier.GetAllText());

                if (_getLexerFunc is null)
                    return;
                var lexer = _getLexerFunc.Invoke(resourceUri, pendingCalculation.ContentAtRequest);
                lexer.Lex();

                CompilationUnit? compilationUnit = null;
                
                // Even if the parser throws an exception, be sure to
                // make use of the Lexer to do whatever syntax highlighting is possible.
                try
                {
                    if (_getParserFunc is null || Binder is null)
                        return;

                    var parser = _getParserFunc.Invoke(lexer);
                    compilationUnit = parser.Parse(Binder, resourceUri);
                }
                finally
                {
                    lock (_resourceMapLock)
                    {
                        if (_resourceMap.ContainsKey(resourceUri))
                        {
                            var resource = _resourceMap[resourceUri];

                            resource.SyntaxTokenList = lexer.SyntaxTokenList;

                            if (compilationUnit is not null)
                                resource.CompilationUnit = compilationUnit;
                        }
                    }

                    // TODO: Shouldn't one get a reference to the most recent TextEditorModel instance with the given key and invoke .ApplySyntaxHighlightingAsync() on that?
                    await modelModifier.ApplySyntaxHighlightingAsync().ConfigureAwait(false);

                    var presentationModel = modelModifier.PresentationModelsList.FirstOrDefault(x =>
                        x.TextEditorPresentationKey == CompilerServiceDiagnosticPresentationFacts.PresentationKey);

                    if (presentationModel?.PendingCalculation is not null)
                    {
                        presentationModel.PendingCalculation.TextSpanList =
                            GetDiagnosticsFor(modelModifier.ResourceUri)
                                .Select(x => x.TextSpan)
                                .ToImmutableArray();

                        (presentationModel.CompletedCalculation, presentationModel.PendingCalculation) =
                            (presentationModel.PendingCalculation, presentationModel.CompletedCalculation);
                    }

                    OnResourceParsed();
                }
            });
    }
}
