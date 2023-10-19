using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.CompilerServices.Lang.Css.Css.SyntaxActors;
using Luthetus.TextEditor.RazorLib.Autocompletes.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.States;
using System.Collections.Immutable;

namespace Luthetus.CompilerServices.Lang.Css;

public class CssCompilerService : ICompilerService
{
    private readonly Dictionary<ResourceUri, CssResource> _cssResourceMap = new();
    private readonly object _cssResourceMapLock = new();
    private readonly ITextEditorService _textEditorService;
    private readonly IBackgroundTaskService _backgroundTaskService;
    private readonly IDispatcher _dispatcher;

    public CssCompilerService(
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
        _cssResourceMap.Values
            .Select(cssr => (ICompilerServiceResource)cssr)
            .ToImmutableArray();

    public void RegisterResource(ResourceUri resourceUri)
    {
        lock (_cssResourceMapLock)
        {
            if (_cssResourceMap.ContainsKey(resourceUri))
                return;

            _cssResourceMap.Add(
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

        lock (_cssResourceMapLock)
        {
            if (!_cssResourceMap.ContainsKey(resourceUri))
                return null;

            return _cssResourceMap[resourceUri];
        }
    }

    public ImmutableArray<TextEditorTextSpan> GetSyntacticTextSpansFor(ResourceUri resourceUri)
    {
        lock (_cssResourceMapLock)
        {
            if (!_cssResourceMap.ContainsKey(resourceUri))
                return ImmutableArray<TextEditorTextSpan>.Empty;

            return _cssResourceMap[resourceUri].SyntacticTextSpans ??
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

    public ImmutableArray<AutocompleteEntry> GetAutocompleteEntries(string word, TextEditorCursorSnapshot cursorSnapshot)
    {
        return ImmutableArray<AutocompleteEntry>.Empty;
    }

    public void DisposeResource(ResourceUri resourceUri)
    {
        lock (_cssResourceMapLock)
        {
            _cssResourceMap.Remove(resourceUri);
        }

        ResourceDisposed?.Invoke();
    }

    private void QueueParseRequest(ResourceUri resourceUri)
    {
        _backgroundTaskService.Enqueue(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.Queue.Key,
            "CSS Compiler Service - Parse",
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

                var lexer = new TextEditorCssLexer(model.ResourceUri);
                var lexResult = await lexer.Lex(text, model.RenderStateKey);

                lock (_cssResourceMapLock)
                {
                    if (!_cssResourceMap.ContainsKey(resourceUri))
                        return;

                    _cssResourceMap[resourceUri].SyntacticTextSpans = lexResult;
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