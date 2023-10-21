using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.CompilerServices.Lang.Json.Json.SyntaxActors;
using Luthetus.TextEditor.RazorLib.Autocompletes.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.States;
using System.Collections.Immutable;

namespace Luthetus.CompilerServices.Lang.Json;

public class JsonCompilerService : ICompilerService
{
    private readonly Dictionary<ResourceUri, JsonResource> _jsonResourceMap = new();
    private readonly object _jsonResourceMapLock = new();
    private readonly ITextEditorService _textEditorService;
    private readonly IBackgroundTaskService _backgroundTaskService;
    private readonly IDispatcher _dispatcher;

    public JsonCompilerService(
        ITextEditorService textEditorService,
        IBackgroundTaskService backgroundTaskService,
        IDispatcher dispatcher)
    {
        _textEditorService = textEditorService;
        _backgroundTaskService = backgroundTaskService;
        _dispatcher = dispatcher;
    }

    public event Action? ResourceRegistered;
    public event Action? ResourceParsed;
    public event Action? ResourceDisposed;

    public IBinder? Binder => null;

    public ImmutableArray<ICompilerServiceResource> CompilerServiceResources =>
        _jsonResourceMap.Values
            .Select(jr => (ICompilerServiceResource)jr)
            .ToImmutableArray();

    public void RegisterResource(ResourceUri resourceUri)
    {
        lock (_jsonResourceMapLock)
        {
            if (_jsonResourceMap.ContainsKey(resourceUri))
                return;

            _jsonResourceMap.Add(
                resourceUri,
                new(resourceUri, this));

            QueueParseRequest(resourceUri);
        }

        ResourceRegistered?.Invoke();
    }

    public ICompilerServiceResource? GetCompilerServiceResourceFor(ResourceUri resourceUri)
    {
        var model = _textEditorService.Model.FindOrDefault(resourceUri);

        if (model is null)
            return null;

        lock (_jsonResourceMapLock)
        {
            if (!_jsonResourceMap.ContainsKey(resourceUri))
                return null;

            return _jsonResourceMap[resourceUri];
        }
    }

    public ImmutableArray<TextEditorTextSpan> GetSyntacticTextSpansFor(ResourceUri resourceUri)
    {
        lock (_jsonResourceMapLock)
        {
            if (!_jsonResourceMap.ContainsKey(resourceUri))
                return ImmutableArray<TextEditorTextSpan>.Empty;

            return _jsonResourceMap[resourceUri].SyntacticTextSpans ??
                ImmutableArray<TextEditorTextSpan>.Empty;
        }
    }

    public ImmutableArray<ITextEditorSymbol> GetSymbolsFor(ResourceUri resourceUri)
    {
        return ImmutableArray<ITextEditorSymbol>.Empty;
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
        lock (_jsonResourceMapLock)
        {
            _jsonResourceMap.Remove(resourceUri);
        }

        ResourceDisposed?.Invoke();
    }

    private void QueueParseRequest(ResourceUri resourceUri)
    {
        _backgroundTaskService.Enqueue(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.Queue.Key,
            "JSON Compiler Service - Parse",
            async () =>
            {
                var model = _textEditorService.Model.FindOrDefault(resourceUri);

                if (model is null)
                    return;

                var text = model.GetAllText();

                _dispatcher.Dispatch(new TextEditorModelState.CalculatePresentationModelAction(
                    model.ResourceUri,
                    CompilerServiceDiagnosticPresentationFacts.PresentationKey));

                var pendingCalculation = model.PresentationModelsBag.FirstOrDefault(x =>
                    x.TextEditorPresentationKey == CompilerServiceDiagnosticPresentationFacts.PresentationKey)
                    ?.PendingCalculation;

                if (pendingCalculation is null)
                    pendingCalculation = new(model.GetAllText());

                var lexer = new TextEditorJsonLexer(model.ResourceUri);
                var lexResult = await lexer.Lex(text, model.RenderStateKey);

                lock (_jsonResourceMapLock)
                {
                    if (!_jsonResourceMap.ContainsKey(resourceUri))
                        return;

                    _jsonResourceMap[resourceUri]
                        .SyntacticTextSpans = lexResult;
                }

                await model.ApplySyntaxHighlightingAsync();

                ResourceParsed?.Invoke();

                var presentationModel = model.PresentationModelsBag.FirstOrDefault(x =>
                    x.TextEditorPresentationKey == CompilerServiceDiagnosticPresentationFacts.PresentationKey);

                if (presentationModel is not null)
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