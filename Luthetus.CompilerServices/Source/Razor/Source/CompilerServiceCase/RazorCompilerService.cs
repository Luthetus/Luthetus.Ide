using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.CompilerServices.Lang.CSharp.CompilerServiceCase;
using Luthetus.TextEditor.RazorLib.Autocompletes.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Facts;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using System.Collections.Immutable;

namespace Luthetus.CompilerServices.Lang.Razor.CompilerServiceCase;

public sealed class RazorCompilerService : LuthCompilerService
{
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
        : base(textEditorService, null, null)
    {
        _backgroundTaskService = backgroundTaskService;
        _cSharpCompilerService = cSharpCompilerService;
        _environmentProvider = environmentProvider;
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
                new RazorResource(resourceUri, this, _textEditorService));

            QueueParseRequest(resourceUri);
        }

        OnResourceRegistered();
    }

    public override ImmutableArray<AutocompleteEntry> GetAutocompleteEntries(string word, TextEditorTextSpan textSpan)
    {
        return ImmutableArray<AutocompleteEntry>.Empty;
    }

    /// <summary>
    /// TODO: Revisit this method for RazorCompilerService...
    /// ...the issue of 'razorResource.RazorSyntaxTree = lexer.RazorSyntaxTree;'
    /// and 'razorResource.HtmlSymbols.Clear();'
    /// These two pieces of custom logic, require the entirety of QueueParseRequest to be
    /// duplicated here.
    /// </summary>
    /// <param name="resourceUri"></param>
    protected override void QueueParseRequest(ResourceUri resourceUri)
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

                lock (_resourceMapLock)
                {
                    if (!_resourceMap.ContainsKey(resourceUri))
                        return;

                    var razorResource = (RazorResource)_resourceMap[resourceUri];
                    razorResource.HtmlSymbols.Clear();
                }

                lexer.Lex();

                lock (_resourceMapLock)
                {
                    if (!_resourceMap.ContainsKey(resourceUri))
                        return;

                    var razorResource = (RazorResource)_resourceMap[resourceUri];

                    razorResource.SyntaxTokenList = lexer.SyntaxTokenList;
                    razorResource.RazorSyntaxTree = lexer.RazorSyntaxTree;
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