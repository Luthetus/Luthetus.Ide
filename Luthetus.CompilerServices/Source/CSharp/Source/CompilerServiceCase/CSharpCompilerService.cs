using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.CompilerServices.Lang.CSharp.BinderCase;
using Luthetus.CompilerServices.Lang.CSharp.LexerCase;
using Luthetus.CompilerServices.Lang.CSharp.ParserCase;
using Luthetus.CompilerServices.Lang.CSharp.RuntimeAssemblies;
using Luthetus.TextEditor.RazorLib.Autocompletes.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using System.Collections.Immutable;

namespace Luthetus.CompilerServices.Lang.CSharp.CompilerServiceCase;

public class CSharpCompilerService : ICompilerService
{
    private readonly Dictionary<ResourceUri, CSharpResource> _cSharpResourceMap = new();
    private readonly object _cSharpResourceMapLock = new();
    private readonly ITextEditorService _textEditorService;
    private readonly IBackgroundTaskService _backgroundTaskService;
    private readonly IDispatcher _dispatcher;
    
    /// <summary>
    /// TODO: The CSharpBinder should be private, but for now I'm making it public to be usable in the CompilerServiceExplorer Blazor component.
    /// </summary>
    public readonly CSharpBinder CSharpBinder = new();

    public CSharpCompilerService(
        ITextEditorService textEditorService,
        IBackgroundTaskService backgroundTaskService,
        IDispatcher dispatcher)
    {
        _textEditorService = textEditorService;
        _backgroundTaskService = backgroundTaskService;
        _dispatcher = dispatcher;

        RuntimeAssembliesLoaderFactory.LoadDotNet6(CSharpBinder);
    }

    public event Action? ResourceRegistered;
    public event Action? ResourceParsed;
    public event Action? ResourceDisposed;

    public IBinder? Binder => CSharpBinder;

    public ImmutableArray<ICompilerServiceResource> CompilerServiceResources =>
        _cSharpResourceMap.Values
            .Select(csr => (ICompilerServiceResource)csr)
            .ToImmutableArray();

    public void RegisterResource(ResourceUri resourceUri)
    {
        lock (_cSharpResourceMapLock)
        {
            if (_cSharpResourceMap.ContainsKey(resourceUri))
                return;

            _cSharpResourceMap.Add(
                resourceUri,
                new(resourceUri, this));
        }

        QueueParseRequest(resourceUri);
        ResourceRegistered?.Invoke();
    }

    public ICompilerServiceResource? GetCompilerServiceResourceFor(ResourceUri resourceUri)
    {
        var model = _textEditorService.ModelApi.GetOrDefault(resourceUri);

        if (model is null)
            return null;

        lock (_cSharpResourceMapLock)
        {
            if (!_cSharpResourceMap.ContainsKey(resourceUri))
                return null;

            return _cSharpResourceMap[resourceUri];
        }
    }

    public ImmutableArray<TextEditorTextSpan> GetSyntacticTextSpansFor(ResourceUri resourceUri)
    {
        lock (_cSharpResourceMapLock)
        {
            if (!_cSharpResourceMap.ContainsKey(resourceUri))
                return ImmutableArray<TextEditorTextSpan>.Empty;

            return _cSharpResourceMap[resourceUri].SyntacticTextSpans;
        }
    }

    public ImmutableArray<ITextEditorSymbol> GetSymbolsFor(ResourceUri resourceUri)
    {
        lock (_cSharpResourceMapLock)
        {
            if (!_cSharpResourceMap.ContainsKey(resourceUri))
                return ImmutableArray<ITextEditorSymbol>.Empty;

            return _cSharpResourceMap[resourceUri].Symbols;
        }
    }

    public ImmutableArray<TextEditorDiagnostic> GetDiagnosticsFor(ResourceUri resourceUri)
    {
        lock (_cSharpResourceMapLock)
        {
            if (!_cSharpResourceMap.ContainsKey(resourceUri))
                return ImmutableArray<TextEditorDiagnostic>.Empty;

            return _cSharpResourceMap[resourceUri].Diagnostics;
        }
    }

    public void ResourceWasModified(ResourceUri resourceUri, ImmutableArray<TextEditorTextSpan> editTextSpans)
    {
        QueueParseRequest(resourceUri);
    }

    public ImmutableArray<AutocompleteEntry> GetAutocompleteEntries(string word, TextEditorTextSpan textSpan)
    {
        var boundScope = CSharpBinder.GetBoundScope(textSpan) as CSharpBoundScope;

        if (boundScope is null)
            return ImmutableArray<AutocompleteEntry>.Empty;

        var autocompleteEntryList = new List<AutocompleteEntry>();

        var targetScope = boundScope;

        while (targetScope is not null)
        {
            autocompleteEntryList.AddRange(
                targetScope.VariableDeclarationMap.Keys
                .ToArray()
                .Where(x => x.Contains(word, StringComparison.InvariantCulture))
                .Distinct()
                .Take(10)
                .Select(x =>
                {
                    return new AutocompleteEntry(
                        x,
                        AutocompleteEntryKind.Variable);
                }));

            autocompleteEntryList.AddRange(
                targetScope.FunctionDefinitionMap.Keys
                .ToArray()
                .Where(x => x.Contains(word, StringComparison.InvariantCulture))
                .Distinct()
                .Take(10)
                .Select(x =>
                {
                    return new AutocompleteEntry(
                        x,
                        AutocompleteEntryKind.Function);
                }));

            autocompleteEntryList.AddRange(
                targetScope.TypeDefinitionMap.Keys
                .ToArray()
                .Where(x => x.Contains(word, StringComparison.InvariantCulture))
                .Distinct()
                .Take(10)
                .Select(x =>
                {
                    return new AutocompleteEntry(
                        x,
                        AutocompleteEntryKind.Type);
                }));

            targetScope = targetScope.Parent;
        }

        return autocompleteEntryList.DistinctBy(x => x.DisplayName).ToImmutableArray();
    }

    public void DisposeResource(ResourceUri resourceUri)
    {
        lock (_cSharpResourceMapLock)
        {
            _cSharpResourceMap.Remove(resourceUri);
        }

        ResourceDisposed?.Invoke();
    }

    private void QueueParseRequest(ResourceUri resourceUri)
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
                    .Invoke(editContext);

                var pendingCalculation = modelModifier.PresentationModelsList.FirstOrDefault(x =>
                    x.TextEditorPresentationKey == CompilerServiceDiagnosticPresentationFacts.PresentationKey)
                    ?.PendingCalculation;

                if (pendingCalculation is null)
                    pendingCalculation = new(modelModifier.GetAllText());

                var lexer = new CSharpLexer(resourceUri, pendingCalculation.ContentAtRequest);
                lexer.Lex();

                CompilationUnit? compilationUnit = null;

                try
                {
                    // Even if the parser throughs an exception, be sure to
                    // make use of the Lexer to do whatever syntax highlighting is possible.
                    var parser = new CSharpParser(lexer);
                    compilationUnit = parser.Parse(CSharpBinder, resourceUri);
                }
                finally
                {
                    lock (_cSharpResourceMapLock)
                    {
                        if (_cSharpResourceMap.ContainsKey(resourceUri))
                        {
                            var cSharpResource = _cSharpResourceMap[resourceUri];

                            cSharpResource.SyntaxTokens = lexer.SyntaxTokens;

                            if (compilationUnit is not null)
                                cSharpResource.CompilationUnit = compilationUnit;
                        }
                    }

                    // TODO: Shouldn't one get a reference to the most recent TextEditorModel instance with the given key and invoke .ApplySyntaxHighlightingAsync() on that?
                    await modelModifier.ApplySyntaxHighlightingAsync();

                    var presentationModel = modelModifier.PresentationModelsList.FirstOrDefault(x =>
                        x.TextEditorPresentationKey == CompilerServiceDiagnosticPresentationFacts.PresentationKey);

                    if (presentationModel?.PendingCalculation is not null)
                    {
                        presentationModel.PendingCalculation.TextEditorTextSpanList =
                            GetDiagnosticsFor(modelModifier.ResourceUri)
                                .Select(x => x.TextSpan)
                                .ToImmutableArray();

                        (presentationModel.CompletedCalculation, presentationModel.PendingCalculation) =
                            (presentationModel.PendingCalculation, presentationModel.CompletedCalculation);
                    }

                    ResourceParsed?.Invoke();
                }
            });
    }
}