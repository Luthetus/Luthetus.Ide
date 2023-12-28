using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.CompilerServices.Lang.Xml.Html.SyntaxActors;
using Luthetus.TextEditor.RazorLib.Autocompletes.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Luthetus.TextEditor.RazorLib.TextEditors.States;
using System.Collections.Immutable;

namespace Luthetus.CompilerServices.Lang.CSharpProject.CompilerServiceCase;

public class CSharpProjectCompilerService : ICompilerService
{
    private readonly Dictionary<ResourceUri, CSharpProjectResource> _cSharpProjectResourceMap = new();
    private readonly object _cSharpProjectResourceMapLock = new();
    private readonly ITextEditorService _textEditorService;
    private readonly IBackgroundTaskService _backgroundTaskService;
    private readonly IDispatcher _dispatcher;

    public CSharpProjectCompilerService(
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
        _cSharpProjectResourceMap.Values
            .Select(dnsr => (ICompilerServiceResource)dnsr)
            .ToImmutableArray();

    public void RegisterResource(ResourceUri resourceUri)
    {
        lock (_cSharpProjectResourceMapLock)
        {
            if (_cSharpProjectResourceMap.ContainsKey(resourceUri))
                return;

            _cSharpProjectResourceMap.Add(
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

        lock (_cSharpProjectResourceMapLock)
        {
            if (!_cSharpProjectResourceMap.ContainsKey(resourceUri))
                return null;

            return _cSharpProjectResourceMap[resourceUri];
        }
    }

    public ImmutableArray<TextEditorTextSpan> GetSyntacticTextSpansFor(ResourceUri resourceUri)
    {
        lock (_cSharpProjectResourceMapLock)
        {
            if (!_cSharpProjectResourceMap.ContainsKey(resourceUri))
                return ImmutableArray<TextEditorTextSpan>.Empty;

            return _cSharpProjectResourceMap[resourceUri].SyntacticTextSpans;
        }
    }

    public ImmutableArray<ITextEditorSymbol> GetSymbolsFor(ResourceUri resourceUri)
    {
        lock (_cSharpProjectResourceMapLock)
        {
            if (!_cSharpProjectResourceMap.ContainsKey(resourceUri))
                return ImmutableArray<ITextEditorSymbol>.Empty;

            return _cSharpProjectResourceMap[resourceUri].Symbols;
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
        lock (_cSharpProjectResourceMapLock)
        {
            _cSharpProjectResourceMap.Remove(resourceUri);
        }

        ResourceDisposed?.Invoke();
    }

    private void QueueParseRequest(ResourceUri resourceUri)
    {
        _textEditorService.Post(async editContext =>
        {
            var modelModifier = editContext.GetModelModifier(resourceUri);

            if (modelModifier is null)
                return;

            var text = TextEditorModelHelper.GetAllText(modelModifier);

            await _textEditorService.ModelApi.CalculatePresentationModelFactory(
                    modelModifier.ResourceUri,
                    CompilerServiceDiagnosticPresentationFacts.PresentationKey)
                .Invoke(editContext);

            var pendingCalculation = modelModifier.PresentationModelsBag.FirstOrDefault<TextEditor.RazorLib.Decorations.Models.TextEditorPresentationModel>((Func<TextEditor.RazorLib.Decorations.Models.TextEditorPresentationModel, bool>)(x =>
                x.TextEditorPresentationKey == CompilerServiceDiagnosticPresentationFacts.PresentationKey))
                ?.PendingCalculation;

            if (pendingCalculation is null)
                pendingCalculation = new(TextEditorModelHelper.GetAllText(modelModifier));

            var lexer = new TextEditorHtmlLexer(modelModifier.ResourceUri);
            var lexResult = await lexer.Lex(text, modelModifier.RenderStateKey);

            lock (_cSharpProjectResourceMapLock)
            {
                if (!_cSharpProjectResourceMap.ContainsKey(resourceUri))
                    return;

				_cSharpProjectResourceMap[resourceUri]
                    .SyntacticTextSpans = lexResult;
            }

            await TextEditorModelHelper.ApplySyntaxHighlightingAsync(modelModifier);

            ResourceParsed?.Invoke();

            return;
        });
    }
}