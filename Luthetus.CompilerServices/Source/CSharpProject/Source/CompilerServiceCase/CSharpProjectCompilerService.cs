using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.CompilerServices.Lang.Xml.Html.SyntaxActors;
using Luthetus.TextEditor.RazorLib.Autocompletes.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Facts;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using System.Collections.Immutable;

namespace Luthetus.CompilerServices.Lang.CSharpProject.CompilerServiceCase;

public sealed class CSharpProjectCompilerService : LuthCompilerService
{
    private readonly IBackgroundTaskService _backgroundTaskService;
    private readonly IDispatcher _dispatcher;

    public CSharpProjectCompilerService(
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
                new CSharpProjectResource(resourceUri, this));

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

                var text = TextEditorModelHelper.GetAllText(modelModifier);

                await _textEditorService.ModelApi.CalculatePresentationModelFactory(
                        modelModifier.ResourceUri,
                        CompilerServiceDiagnosticPresentationFacts.PresentationKey)
                    .Invoke(editContext)
					.ConfigureAwait(false);

                var pendingCalculation = modelModifier.PresentationModelsList.FirstOrDefault<TextEditor.RazorLib.Decorations.Models.TextEditorPresentationModel>((Func<TextEditor.RazorLib.Decorations.Models.TextEditorPresentationModel, bool>)(x =>
                    x.TextEditorPresentationKey == CompilerServiceDiagnosticPresentationFacts.PresentationKey))
                    ?.PendingCalculation;

                if (pendingCalculation is null)
                    pendingCalculation = new(TextEditorModelHelper.GetAllText(modelModifier));

                var lexer = new TextEditorHtmlLexer(modelModifier.ResourceUri);
                var lexResult = await lexer.Lex(text, modelModifier.RenderStateKey).ConfigureAwait(false);

                lock (_resourceMapLock)
                {
                    if (!_resourceMap.ContainsKey(resourceUri))
                        return;

                    var cSharpProject = (CSharpProjectResource)_resourceMap[resourceUri];
                    cSharpProject.TokenTextSpanList = lexResult;
                }

                await TextEditorModelHelper.ApplySyntaxHighlightingAsync(modelModifier).ConfigureAwait(false);

                OnResourceParsed();
            });
    }
}