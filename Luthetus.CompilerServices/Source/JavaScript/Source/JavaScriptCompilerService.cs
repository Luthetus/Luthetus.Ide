using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.CompilerServices.Lang.JavaScript.JavaScript.SyntaxActors;
using Luthetus.TextEditor.RazorLib.Autocompletes.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Facts;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using System.Collections.Immutable;

namespace Luthetus.CompilerServices.Lang.JavaScript;

public sealed class JavaScriptCompilerService : LuthCompilerService
{
    private readonly IBackgroundTaskService _backgroundTaskService;
    private readonly IDispatcher _dispatcher;

    public JavaScriptCompilerService(
            ITextEditorService textEditorService,
            IBackgroundTaskService backgroundTaskService,
            IDispatcher dispatcher)
        : base(textEditorService, null, null)
    {
        _backgroundTaskService = backgroundTaskService;
        _dispatcher = dispatcher;
    }

    public override void RegisterResource(ResourceUri resourceUri)
    {
        lock (_resourceMapLock)
        {
            if (_resourceMap.ContainsKey(resourceUri))
                return;

            _resourceMap.Add(
                resourceUri,
                new JavaScriptResource(resourceUri, this));

            QueueParseRequest(resourceUri);
        }

        OnResourceRegistered();
    }

    public override ImmutableArray<AutocompleteEntry> GetAutocompleteEntries(string word, TextEditorTextSpan textSpan)
    {
        return ImmutableArray<AutocompleteEntry>.Empty;
    }

    protected override void QueueParseRequest(ResourceUri resourceUri)
    {
        _textEditorService.Post(
            nameof(QueueParseRequest),
            async editContext =>
            {
                var modelModifier = editContext.GetModelModifier(resourceUri);

                if (modelModifier is null)
                    return;

                var text = modelModifier.GetAllText();

                await _textEditorService.ModelApi.CalculatePresentationModelFactory(
                        modelModifier.ResourceUri,
                        CompilerServiceDiagnosticPresentationFacts.PresentationKey)
                    .Invoke(editContext)
					.ConfigureAwait(false);

                var pendingCalculation = modelModifier.PresentationModelsList.FirstOrDefault(x =>
                    x.TextEditorPresentationKey == CompilerServiceDiagnosticPresentationFacts.PresentationKey)
                    ?.PendingCalculation;

                pendingCalculation ??= new(modelModifier.GetAllText());

                var lexer = new TextEditorJavaScriptLexer(modelModifier.ResourceUri);
                var lexResult = await lexer.Lex(text, modelModifier.RenderStateKey).ConfigureAwait(false);

                lock (_resourceMapLock)
                {
                    if (!_resourceMap.ContainsKey(resourceUri))
                        return;

                    var jsResource = (JavaScriptResource)_resourceMap[resourceUri];
                    jsResource.TokenTextSpanList = lexResult;
                }

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
            });
    }
}