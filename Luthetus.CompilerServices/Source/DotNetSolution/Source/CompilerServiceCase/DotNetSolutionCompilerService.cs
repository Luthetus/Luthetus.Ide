using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Namespaces.Models;
using Luthetus.CompilerServices.Lang.DotNetSolution.SyntaxActors;
using Luthetus.TextEditor.RazorLib.Autocompletes.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using System.Collections.Immutable;

namespace Luthetus.CompilerServices.Lang.DotNetSolution.CompilerServiceCase;

public class DotNetSolutionCompilerService : ICompilerService
{
    private readonly Dictionary<ResourceUri, DotNetSolutionResource> _dotNetSolutionResourceMap = new();
    private readonly object _dotNetSolutionResourceMapLock = new();
    private readonly ITextEditorService _textEditorService;
    private readonly IBackgroundTaskService _backgroundTaskService;
    private readonly IEnvironmentProvider _environmentProvider;
    private readonly IDispatcher _dispatcher;

    public DotNetSolutionCompilerService(
        ITextEditorService textEditorService,
        IBackgroundTaskService backgroundTaskService,
        IEnvironmentProvider environmentProvider,
        IDispatcher dispatcher)
    {
        _textEditorService = textEditorService;
        _backgroundTaskService = backgroundTaskService;
        _environmentProvider = environmentProvider;
        _dispatcher = dispatcher;
    }

    public event Action? ResourceRegistered;
    public event Action? ResourceParsed;
    public event Action? ResourceDisposed;

    public IBinder? Binder => null;

    public ImmutableArray<ICompilerServiceResource> CompilerServiceResources =>
        _dotNetSolutionResourceMap.Values
            .Select(dnsr => (ICompilerServiceResource)dnsr)
            .ToImmutableArray();

    public void RegisterResource(ResourceUri resourceUri)
    {
        lock (_dotNetSolutionResourceMapLock)
        {
            if (_dotNetSolutionResourceMap.ContainsKey(resourceUri))
                return;

            _dotNetSolutionResourceMap.Add(
                resourceUri,
                new(resourceUri, this));

            QueueParseRequest(resourceUri);
        }

        ResourceRegistered?.Invoke();
    }

    public ICompilerServiceResource? GetCompilerServiceResourceFor(ResourceUri resourceUri)
    {
        var model = _textEditorService.ModelApi.GetOrDefault(resourceUri);

        if (model is null)
            return null;

        lock (_dotNetSolutionResourceMapLock)
        {
            if (!_dotNetSolutionResourceMap.ContainsKey(resourceUri))
                return null;

            return _dotNetSolutionResourceMap[resourceUri];
        }
    }

    public ImmutableArray<TextEditorTextSpan> GetSyntacticTextSpansFor(ResourceUri resourceUri)
    {
        lock (_dotNetSolutionResourceMapLock)
        {
            if (!_dotNetSolutionResourceMap.ContainsKey(resourceUri))
                return ImmutableArray<TextEditorTextSpan>.Empty;

            return _dotNetSolutionResourceMap[resourceUri].SyntacticTextSpanList;
        }
    }

    public ImmutableArray<ITextEditorSymbol> GetSymbolsFor(ResourceUri resourceUri)
    {
        lock (_dotNetSolutionResourceMapLock)
        {
            if (!_dotNetSolutionResourceMap.ContainsKey(resourceUri))
                return ImmutableArray<ITextEditorSymbol>.Empty;

            return _dotNetSolutionResourceMap[resourceUri].SymbolList;
        }
    }

    public ImmutableArray<TextEditorDiagnostic> GetDiagnosticsFor(ResourceUri resourceUri)
    {
        return ImmutableArray<TextEditorDiagnostic>.Empty;
    }

    public void ResourceWasModified(ResourceUri resourceUri, ImmutableArray<TextEditorTextSpan> editTextSpans)
    {
        QueueParseRequest(resourceUri);
    }

    public ImmutableArray<AutocompleteEntry> GetAutocompleteEntries(string word, TextEditorTextSpan textSpan)
    {
        return ImmutableArray<AutocompleteEntry>.Empty;
    }

    public void DisposeResource(ResourceUri resourceUri)
    {
        lock (_dotNetSolutionResourceMapLock)
        {
            _dotNetSolutionResourceMap.Remove(resourceUri);
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

                var absolutePath = new AbsolutePath(
                    modelModifier.ResourceUri.Value,
                    false,
                    _environmentProvider);

                var namespacePath = new NamespacePath(
                    string.Empty,
                    absolutePath);

                await _textEditorService.ModelApi.CalculatePresentationModelFactory(
                        modelModifier.ResourceUri,
                        CompilerServiceDiagnosticPresentationFacts.PresentationKey)
                    .Invoke(editContext);

                var pendingCalculation = modelModifier.PresentationModelsList.FirstOrDefault(x =>
                    x.TextEditorPresentationKey == CompilerServiceDiagnosticPresentationFacts.PresentationKey)
                    ?.PendingCalculation;

                if (pendingCalculation is null)
                    pendingCalculation = new(modelModifier.GetAllText());

                var lexer = new DotNetSolutionLexer(resourceUri, modelModifier.GetAllText());
                lexer.Lex();

                var parser = new DotNetSolutionParser(lexer);

                var compilationUnit = parser.Parse();

                lock (_dotNetSolutionResourceMapLock)
                {
                    if (!_dotNetSolutionResourceMap.ContainsKey(resourceUri))
                        return;

                    var dotNetSolutionResource = _dotNetSolutionResourceMap[resourceUri];
                    dotNetSolutionResource.SyntaxTokenList = lexer.SyntaxTokens;
                    dotNetSolutionResource.CompilationUnit = compilationUnit;
                }

                await modelModifier.ApplySyntaxHighlightingAsync();

                ResourceParsed?.Invoke();

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

                return;
            });
    }
}