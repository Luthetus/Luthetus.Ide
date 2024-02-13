using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Namespaces.Models;
using Luthetus.CompilerServices.Lang.DotNetSolution.SyntaxActors;
using Luthetus.TextEditor.RazorLib.Autocompletes.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Facts;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using System.Collections.Immutable;

namespace Luthetus.CompilerServices.Lang.DotNetSolution.CompilerServiceCase;

public sealed class DotNetSolutionCompilerService : LuthCompilerService
{
    private readonly IBackgroundTaskService _backgroundTaskService;
    private readonly IEnvironmentProvider _environmentProvider;
    private readonly IDispatcher _dispatcher;

    public DotNetSolutionCompilerService(
            ITextEditorService textEditorService,
            IBackgroundTaskService backgroundTaskService,
            IEnvironmentProvider environmentProvider,
            IDispatcher dispatcher)
        : base(textEditorService, null, null)
    {
        _backgroundTaskService = backgroundTaskService;
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
                new DotNetSolutionResource(resourceUri, this));

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

                var absolutePath = _environmentProvider.AbsolutePathFactory(
                    modelModifier.ResourceUri.Value,
                    false);

                var namespacePath = new NamespacePath(
                    string.Empty,
                    absolutePath);

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

                var lexer = new DotNetSolutionLexer(resourceUri, modelModifier.GetAllText());
                lexer.Lex();

                var parser = new DotNetSolutionParser(lexer);

                var compilationUnit = parser.Parse();

                lock (_resourceMapLock)
                {
                    if (!_resourceMap.ContainsKey(resourceUri))
                        return;

                    var dotNetSolutionResource = _resourceMap[resourceUri];
                    dotNetSolutionResource.SyntaxTokenList = lexer.SyntaxTokens;
                    dotNetSolutionResource.CompilationUnit = compilationUnit;
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