using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Namespaces.Models;
using Luthetus.CompilerServices.Lang.DotNetSolution.SyntaxActors;
using Luthetus.TextEditor.RazorLib.Autocompletes.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Luthetus.TextEditor.RazorLib.TextEditors.States;
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

            return _dotNetSolutionResourceMap[resourceUri].SyntacticTextSpanBag;
        }
    }

    public ImmutableArray<ITextEditorSymbol> GetSymbolsFor(ResourceUri resourceUri)
    {
        lock (_dotNetSolutionResourceMapLock)
        {
            if (!_dotNetSolutionResourceMap.ContainsKey(resourceUri))
                return ImmutableArray<ITextEditorSymbol>.Empty;

            return _dotNetSolutionResourceMap[resourceUri].SymbolBag;
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
        _textEditorService.Post(async editContext =>
        {
            var model = _textEditorService.ModelApi.GetOrDefault(resourceUri);

            if (model is null)
                return;

            var absolutePath = new AbsolutePath(
                model.ResourceUri.Value,
                false,
                _environmentProvider);

            var namespacePath = new NamespacePath(
                string.Empty,
                absolutePath);

            _dispatcher.Dispatch(new TextEditorModelState.CalculatePresentationModelAction(
                editContext,
                model.ResourceUri,
                CompilerServiceDiagnosticPresentationFacts.PresentationKey));

            var pendingCalculation = model.PresentationModelsBag.FirstOrDefault(x =>
                x.TextEditorPresentationKey == CompilerServiceDiagnosticPresentationFacts.PresentationKey)
                ?.PendingCalculation;

            if (pendingCalculation is null)
                pendingCalculation = new(model.GetAllText());

            var lexer = new DotNetSolutionLexer(resourceUri, model.GetAllText());
            lexer.Lex();

            var parser = new DotNetSolutionParser(lexer);

            var compilationUnit = parser.Parse();

            lock (_dotNetSolutionResourceMapLock)
            {
                if (!_dotNetSolutionResourceMap.ContainsKey(resourceUri))
                    return;

                var dotNetSolutionResource = _dotNetSolutionResourceMap[resourceUri];
                dotNetSolutionResource.SyntaxTokenBag = lexer.SyntaxTokens;
                dotNetSolutionResource.CompilationUnit = compilationUnit;
            }

            await model.ApplySyntaxHighlightingAsync();

            ResourceParsed?.Invoke();

            var presentationModel = model.PresentationModelsBag.FirstOrDefault(x =>
                x.TextEditorPresentationKey == CompilerServiceDiagnosticPresentationFacts.PresentationKey);

            if (presentationModel?.PendingCalculation is not null)
            {
                presentationModel.PendingCalculation.TextEditorTextSpanBag =
                    GetDiagnosticsFor(model.ResourceUri)
                        .Select(x => x.TextSpan)
                        .ToImmutableArray();

                (presentationModel.CompletedCalculation, presentationModel.PendingCalculation) =
                    (presentationModel.PendingCalculation, presentationModel.CompletedCalculation);
            }

            return;
        });
    }
}