using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.CompilerServices.Lang.CSharp.CompilerServiceCase;
using Luthetus.TextEditor.RazorLib.Autocompletes.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using System.Collections.Immutable;

namespace Luthetus.CompilerServices.Lang.Razor.CompilerServiceCase;

public class RazorCompilerService : ICompilerService
{
    private readonly Dictionary<ResourceUri, RazorResource> _razorResourceMap = new();
    private readonly object _razorResourceMapLock = new();
    private readonly ITextEditorService _textEditorService;
    private readonly IBackgroundTaskService _backgroundTaskService;
    private readonly CSharpCompilerService _cSharpCompilerService;
    private readonly IEnvironmentProvider _environmentProvider;
    private readonly IDispatcher _dispatcher;

    public RazorCompilerService(
        ITextEditorService textEditorService,
        IBackgroundTaskService backgroundTaskService,
        CSharpCompilerService cSharpCompilerService,
        IEnvironmentProvider environmentProvider,
        IDispatcher dispatcher)
    {
        _textEditorService = textEditorService;
        _backgroundTaskService = backgroundTaskService;
        _cSharpCompilerService = cSharpCompilerService;
        _environmentProvider = environmentProvider;
        _dispatcher = dispatcher;
    }

    public event Action? ResourceRegistered;
    public event Action? ResourceParsed;
    public event Action? ResourceDisposed;

    public IBinder? Binder => null;

    public ImmutableArray<ICompilerServiceResource> CompilerServiceResources =>
        _razorResourceMap.Values
            .Select(rr => (ICompilerServiceResource)rr)
            .ToImmutableArray();

    public void RegisterResource(ResourceUri resourceUri)
    {
        lock (_razorResourceMapLock)
        {
            if (_razorResourceMap.ContainsKey(resourceUri))
                return;

            _razorResourceMap.Add(
                resourceUri,
                new(resourceUri, this, _textEditorService));

            QueueParseRequest(resourceUri);
        }

        ResourceRegistered?.Invoke();
    }

    public ICompilerServiceResource? GetCompilerServiceResourceFor(ResourceUri resourceUri)
    {
        var model = _textEditorService.ModelApi.GetOrDefault(resourceUri);

        if (model is null)
            return null;

        lock (_razorResourceMapLock)
        {
            if (!_razorResourceMap.ContainsKey(resourceUri))
                return null;

            return _razorResourceMap[resourceUri];
        }
    }

    public ImmutableArray<TextEditorTextSpan> GetSyntacticTextSpansFor(ResourceUri resourceUri)
    {
        lock (_razorResourceMapLock)
        {
            if (!_razorResourceMap.ContainsKey(resourceUri))
                return ImmutableArray<TextEditorTextSpan>.Empty;

            return _razorResourceMap[resourceUri].SyntacticTextSpans;
        }
    }

    public ImmutableArray<ITextEditorSymbol> GetSymbolsFor(ResourceUri resourceUri)
    {
        lock (_razorResourceMapLock)
        {
            if (!_razorResourceMap.ContainsKey(resourceUri))
                return ImmutableArray<ITextEditorSymbol>.Empty;

            return _razorResourceMap[resourceUri].Symbols;
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

    public void CursorWasModified(ResourceUri resourceUri, TextEditorCursor cursor)
    {
    }

    public ImmutableArray<AutocompleteEntry> GetAutocompleteEntries(string word, TextEditorTextSpan textSpan)
    {
        return ImmutableArray<AutocompleteEntry>.Empty;
    }

    public void DisposeResource(ResourceUri resourceUri)
    {
        lock (_razorResourceMapLock)
        {
            _razorResourceMap.Remove(resourceUri);
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
                    .Invoke(editContext)
					.ConfigureAwait(false);

                var pendingCalculation = modelModifier.PresentationModelsList.FirstOrDefault(x =>
                    x.TextEditorPresentationKey == CompilerServiceDiagnosticPresentationFacts.PresentationKey)
                    ?.PendingCalculation;

                if (pendingCalculation is null)
                    pendingCalculation = new(modelModifier.GetAllText());

                var lexer = new RazorLexer(
                    modelModifier.ResourceUri,
                    modelModifier.GetAllText(),
                    this,
                    _cSharpCompilerService,
                    _environmentProvider);

                lock (_razorResourceMapLock)
                {
                    if (!_razorResourceMap.ContainsKey(resourceUri))
                        return;

                    var razorResource = _razorResourceMap[resourceUri];

                    razorResource.HtmlSymbols.Clear();
                }

                lexer.Lex();

                lock (_razorResourceMapLock)
                {
                    if (!_razorResourceMap.ContainsKey(resourceUri))
                        return;

                    var razorResource = _razorResourceMap[resourceUri];

                    razorResource.SyntacticTextSpans = lexer.TextEditorTextSpans;
                    razorResource.RazorSyntaxTree = lexer.RazorSyntaxTree;
                }

                await modelModifier.ApplySyntaxHighlightingAsync().ConfigureAwait(false);

                ResourceParsed?.Invoke();

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

                return;
            });
    }
}