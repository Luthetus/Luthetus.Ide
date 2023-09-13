using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.Group;
using Fluxor;
using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.Model;
using Luthetus.TextEditor.RazorLib.Lexing;
using Luthetus.Common.RazorLib.Notification;
using Luthetus.Common.RazorLib.BackgroundTaskCase.BaseTypes;
using Luthetus.Common.RazorLib.Store.NotificationCase;
using Luthetus.CompilerServices.Lang.CSharp.BinderCase;
using Luthetus.CompilerServices.Lang.Xml;
using Luthetus.CompilerServices.Lang.CSharp.CompilerServiceCase;
using Luthetus.CompilerServices.Lang.Razor.CompilerServiceCase;
using Luthetus.CompilerServices.Lang.Css;
using Luthetus.CompilerServices.Lang.JavaScript;
using Luthetus.CompilerServices.Lang.TypeScript;
using Luthetus.CompilerServices.Lang.Json;
using Luthetus.Common.RazorLib.FileSystem.Interfaces;
using Luthetus.CompilerServices.Lang.DotNetSolution.CompilerServiceCase;
using Luthetus.CompilerServices.Lang.CSharpProject.CompilerServiceCase;
using Luthetus.CompilerServices.Lang.FSharp;
using Luthetus.TextEditor.RazorLib.CompilerServiceCase;
using Luthetus.TextEditor.RazorLib.ViewModel.InternalClasses;
using Luthetus.Ide.RazorLib.ComponentRenderersCase;
using Luthetus.Ide.RazorLib.FileSystemCase;
using Luthetus.Ide.RazorLib.ComponentRenderersCase.Types;
using Luthetus.Ide.ClassLib.Store.InputFileCase;
using Luthetus.Ide.RazorLib.InputFileCase;

namespace Luthetus.Ide.RazorLib.EditorCase;

public partial class EditorRegistry
{
    public static readonly TextEditorGroupKey EditorTextEditorGroupKey = TextEditorGroupKey.NewKey();

    public static readonly CSharpBinder SharedBinder = new();

    private class Effector
    {
        private readonly ITextEditorService _textEditorService;
        private readonly ILuthetusIdeComponentRenderers _luthetusIdeComponentRenderers;
        private readonly IFileSystemProvider _fileSystemProvider;
        private readonly IBackgroundTaskService _backgroundTaskService;
        private readonly XmlCompilerService _xmlCompilerService;
        private readonly DotNetSolutionCompilerService _dotNetCompilerService;
        private readonly CSharpProjectCompilerService _cSharpProjectCompilerService;
        private readonly CSharpCompilerService _cSharpCompilerService;
        private readonly RazorCompilerService _razorCompilerService;
        private readonly CssCompilerService _cssCompilerService;
        private readonly FSharpCompilerService _fSharpCompilerService;
        private readonly JavaScriptCompilerService _javaScriptCompilerService;
        private readonly TypeScriptCompilerService _typeScriptCompilerService;
        private readonly JsonCompilerService _jsonCompilerService;

        public Effector(
            ITextEditorService textEditorService,
            ILuthetusIdeComponentRenderers luthetusIdeComponentRenderers,
            IFileSystemProvider fileSystemProvider,
            IBackgroundTaskService backgroundTaskService,
            XmlCompilerService xmlCompilerService,
            DotNetSolutionCompilerService dotNetCompilerService,
            CSharpProjectCompilerService cSharpProjectCompilerService,
            CSharpCompilerService cSharpCompilerService,
            RazorCompilerService razorCompilerService,
            CssCompilerService cssCompilerService,
            FSharpCompilerService fSharpCompilerService,
            JavaScriptCompilerService javaScriptCompilerService,
            TypeScriptCompilerService typeScriptCompilerService,
            JsonCompilerService jsonCompilerService)
        {
            _textEditorService = textEditorService;
            _luthetusIdeComponentRenderers = luthetusIdeComponentRenderers;
            _fileSystemProvider = fileSystemProvider;
            _backgroundTaskService = backgroundTaskService;
            _xmlCompilerService = xmlCompilerService;
            _dotNetCompilerService = dotNetCompilerService;
            _cSharpProjectCompilerService = cSharpProjectCompilerService;
            _cSharpCompilerService = cSharpCompilerService;
            _razorCompilerService = razorCompilerService;
            _cssCompilerService = cssCompilerService;
            _fSharpCompilerService = fSharpCompilerService;
            _javaScriptCompilerService = javaScriptCompilerService;
            _typeScriptCompilerService = typeScriptCompilerService;
            _jsonCompilerService = jsonCompilerService;
        }

        [EffectMethod]
        public Task HandleShowInputFileAction(
            ShowInputFileAction showInputFileAction,
            IDispatcher dispatcher)
        {
            dispatcher.Dispatch(new InputFileRegistry.RequestInputFileStateFormAction(
                "TextEditor",
                async afp => await HandleOpenInEditorAction(new OpenInEditorAction(afp, true), dispatcher),
                afp =>
                {
                    if (afp is null || afp.IsDirectory)
                        return Task.FromResult(false);

                    return Task.FromResult(true);
                },
                new[]
                {
                    new InputFilePattern("File", afp => !afp.IsDirectory)
                }.ToImmutableArray()));

            return Task.CompletedTask;
        }

        [EffectMethod]
        public async Task HandleOpenInEditorAction(
            OpenInEditorAction openInEditorAction,
            IDispatcher dispatcher)
        {
            var editorTextEditorGroupKey =
                openInEditorAction.EditorTextEditorGroupKey ?? EditorTextEditorGroupKey;

            if (openInEditorAction.AbsolutePath is null ||
                openInEditorAction.AbsolutePath.IsDirectory)
            {
                return;
            }

            _textEditorService.Group.Register(editorTextEditorGroupKey);

            var inputFileAbsolutePathString = openInEditorAction.AbsolutePath.FormattedInput;

            var textEditorModel = await GetOrCreateTextEditorModelAsync(
                openInEditorAction.AbsolutePath,
                inputFileAbsolutePathString);

            if (textEditorModel is null)
                return;

            await CheckIfContentsWereModifiedAsync(
                dispatcher,
                inputFileAbsolutePathString,
                textEditorModel);

            var viewModel = GetOrCreateTextEditorViewModel(
                openInEditorAction.AbsolutePath,
                openInEditorAction.ShouldSetFocusToEditor,
                dispatcher,
                textEditorModel,
                inputFileAbsolutePathString);

            _textEditorService.Group.AddViewModel(
                editorTextEditorGroupKey,
                viewModel);

            _textEditorService.Group.SetActiveViewModel(
                editorTextEditorGroupKey,
                viewModel);
        }

        private async Task<TextEditorModel?> GetOrCreateTextEditorModelAsync(
            IAbsolutePath absolutePath,
            string absolutePathString)
        {
            var textEditorModel = _textEditorService.Model
                .FindOrDefaultByResourceUri(new(absolutePathString));

            if (textEditorModel is null)
            {
                var resourceUri = new ResourceUri(absolutePathString);

                var fileLastWriteTime = await _fileSystemProvider.File.GetLastWriteTimeAsync(
                    absolutePathString);

                var content = await _fileSystemProvider.File.ReadAllTextAsync(
                    absolutePathString);

                var compilerService = ExtensionNoPeriodFacts.GetCompilerService(
                    absolutePath.ExtensionNoPeriod,
                    _xmlCompilerService,
                    _dotNetCompilerService,
                    _cSharpProjectCompilerService,
                    _cSharpCompilerService,
                    _razorCompilerService,
                    _cssCompilerService,
                    _fSharpCompilerService,
                    _javaScriptCompilerService,
                    _typeScriptCompilerService,
                    _jsonCompilerService);

                var decorationMapper = ExtensionNoPeriodFacts.GetDecorationMapper(
                    absolutePath.ExtensionNoPeriod);

                textEditorModel = new TextEditorModel(
                    resourceUri,
                    fileLastWriteTime,
                    absolutePath.ExtensionNoPeriod,
                    content,
                    compilerService,
                    decorationMapper,
                    null,
                    new(),
                    TextEditorModelKey.NewKey()
                );

                textEditorModel.CompilerService.RegisterModel(textEditorModel);

                _textEditorService.Model.RegisterCustom(textEditorModel);
                
                _textEditorService.Model.RegisterPresentationModel(
                    textEditorModel.ModelKey,
                    CompilerServiceDiagnosticPresentationFacts.EmptyPresentationModel);

                _ = Task.Run(async () =>
                    await textEditorModel.ApplySyntaxHighlightingAsync());
            }

            return textEditorModel;
        }

        private async Task CheckIfContentsWereModifiedAsync(
            IDispatcher dispatcher,
            string inputFileAbsolutePathString,
            TextEditorModel textEditorModel)
        {
            var fileLastWriteTime = await _fileSystemProvider.File.GetLastWriteTimeAsync(
                inputFileAbsolutePathString);

            if (fileLastWriteTime > textEditorModel.ResourceLastWriteTime &&
                _luthetusIdeComponentRenderers.BooleanPromptOrCancelRendererType is not null)
            {
                var notificationInformativeKey = NotificationKey.NewKey();

                var notificationInformative = new NotificationRecord(
                    notificationInformativeKey,
                    "File contents were modified on disk",
                    _luthetusIdeComponentRenderers.BooleanPromptOrCancelRendererType,
                    new Dictionary<string, object?>
                    {
                        {
                            nameof(IBooleanPromptOrCancelRendererType.Message),
                            "File contents were modified on disk"
                        },
                        {
                            nameof(IBooleanPromptOrCancelRendererType.AcceptOptionTextOverride),
                            "Reload"
                        },
                        {
                            nameof(IBooleanPromptOrCancelRendererType.OnAfterAcceptAction),
                            new Action(() =>
                            {
                                _backgroundTaskService.Enqueue(BackgroundTaskKey.NewKey(), CommonBackgroundTaskWorker.Queue.Key,
                                    "Check If Contexts Were Modified",
                                    async () =>
                                    {
                                        dispatcher.Dispatch(new NotificationRegistry.DisposeAction(
                                            notificationInformativeKey));

                                        var content = await _fileSystemProvider.File
                                            .ReadAllTextAsync(inputFileAbsolutePathString);

                                        _textEditorService.Model.Reload(
                                            textEditorModel.ModelKey,
                                            content,
                                            fileLastWriteTime);

                                        await textEditorModel.ApplySyntaxHighlightingAsync();
                                    });
                            })
                        },
                        {
                            nameof(IBooleanPromptOrCancelRendererType.OnAfterDeclineAction),
                            new Action(() =>
                            {
                                dispatcher.Dispatch(new NotificationRegistry.DisposeAction(
                                    notificationInformativeKey));
                            })
                        },
                    },
                    TimeSpan.FromSeconds(20),
                    true,
                    null);

                dispatcher.Dispatch(new NotificationRegistry.RegisterAction(
                    notificationInformative));
            }
        }

        private TextEditorViewModelKey GetOrCreateTextEditorViewModel(
            IAbsolutePath absolutePath,
            bool shouldSetFocusToEditor,
            IDispatcher dispatcher,
            TextEditorModel textEditorModel,
            string inputFileAbsolutePathString)
        {
            var viewModel = _textEditorService.Model
                .GetViewModelsOrEmpty(textEditorModel.ModelKey)
                .FirstOrDefault();

            var viewModelKey = viewModel?.ViewModelKey ?? TextEditorViewModelKey.Empty;

            if (viewModel is null)
            {
                viewModelKey = TextEditorViewModelKey.NewKey();

                _textEditorService.ViewModel.Register(
                    viewModelKey,
                    textEditorModel.ModelKey);

                var presentationKeys = new[]
                {
                    CompilerServiceDiagnosticPresentationFacts.PresentationKey
                }.ToImmutableArray();

                _textEditorService.ViewModel.With(
                    viewModelKey,
                    textEditorViewModel => textEditorViewModel with
                    {
                        OnSaveRequested = HandleOnSaveRequested,
                        GetTabDisplayNameFunc = _ => absolutePath.NameWithExtension,
                        ShouldSetFocusAfterNextRender = shouldSetFocusToEditor,
                        FirstPresentationLayerKeys = presentationKeys.ToImmutableList()
                    });
            }
            else
            {
                viewModel.ShouldSetFocusAfterNextRender = shouldSetFocusToEditor;
            }

            return viewModelKey;

            void HandleOnSaveRequested(TextEditorModel innerTextEditor)
            {
                var innerContent = innerTextEditor.GetAllText();

                var cancellationToken = textEditorModel.TextEditorSaveFileHelper.GetCancellationToken();

                var saveFileAction = new FileSystemCase.FileSystemRegistry.SaveFileAction(
                    absolutePath,
                    innerContent,
                    writtenDateTime =>
                    {
                        if (writtenDateTime is not null)
                        {
                            _textEditorService.Model.SetResourceData(
                                innerTextEditor.ModelKey,
                                innerTextEditor.ResourceUri,
                                writtenDateTime.Value);
                        }
                    },
                    cancellationToken);

                dispatcher.Dispatch(saveFileAction);
            }
        }
    }
}