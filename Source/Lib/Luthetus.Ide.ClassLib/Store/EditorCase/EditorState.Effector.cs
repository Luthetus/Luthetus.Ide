using Luthetus.Ide.ClassLib.InputFile;
using Luthetus.Ide.ClassLib.Store.FileSystemCase;
using Luthetus.Ide.ClassLib.Store.InputFileCase;
using Luthetus.Ide.ClassLib.ComponentRenderers;
using Luthetus.Ide.ClassLib.FileConstants;
using Luthetus.Ide.ClassLib.FileSystem.Interfaces;
using Luthetus.Ide.ClassLib.Store.SemanticContextCase;

namespace Luthetus.Ide.ClassLib.Store.EditorCase;

public partial class EditorState
{
    public static readonly TextEditorGroupKey EditorTextEditorGroupKey = TextEditorGroupKey.NewTextEditorGroupKey();

    public static readonly CSharpBinder SharedBinder = new();

    private class Effector
    {
        private readonly ITextEditorService _textEditorService;
        private readonly ILuthetusIdeComponentRenderers _luthetusIdeComponentRenderers;
        private readonly IFileSystemProvider _fileSystemProvider;
        private readonly ICommonBackgroundTaskQueue _commonBackgroundTaskQueue;
        private readonly IState<SemanticContextState> _semanticContextStateWrap;
        private readonly TextEditorXmlCompilerService _xmlCompilerService;
        private readonly CSharpCompilerService _cSharpCompilerService;
        private readonly RazorCompilerService _razorCompilerService;
        private readonly TextEditorCssCompilerService _cssCompilerService;
        private readonly TextEditorJavaScriptCompilerService _javaScriptCompilerService;
        private readonly TextEditorTypeScriptCompilerService _typeScriptCompilerService;
        private readonly TextEditorJsonCompilerService _jsonCompilerService;

        public Effector(
            ITextEditorService textEditorService,
            ILuthetusIdeComponentRenderers luthetusIdeComponentRenderers,
            IFileSystemProvider fileSystemProvider,
            ICommonBackgroundTaskQueue commonBackgroundTaskQueue,
            IState<SemanticContextState> semanticContextStateWrap,
            TextEditorXmlCompilerService xmlCompilerService,
            CSharpCompilerService cSharpCompilerService,
            RazorCompilerService razorCompilerService,
            TextEditorCssCompilerService cssCompilerService,
            TextEditorJavaScriptCompilerService javaScriptCompilerService,
            TextEditorTypeScriptCompilerService typeScriptCompilerService,
            TextEditorJsonCompilerService jsonCompilerService)
        {
            _textEditorService = textEditorService;
            _luthetusIdeComponentRenderers = luthetusIdeComponentRenderers;
            _fileSystemProvider = fileSystemProvider;
            _commonBackgroundTaskQueue = commonBackgroundTaskQueue;
            _semanticContextStateWrap = semanticContextStateWrap;
            _xmlCompilerService = xmlCompilerService;
            _cSharpCompilerService = cSharpCompilerService;
            _razorCompilerService = razorCompilerService;
            _cssCompilerService = cssCompilerService;
            _javaScriptCompilerService = javaScriptCompilerService;
            _typeScriptCompilerService = typeScriptCompilerService;
            _jsonCompilerService = jsonCompilerService;
        }


        [EffectMethod]
        public Task HandleShowInputFileAction(
            ShowInputFileAction showInputFileAction,
            IDispatcher dispatcher)
        {
            dispatcher.Dispatch(
                new InputFileState.RequestInputFileStateFormAction(
                    "TextEditor",
                    async afp => await HandleOpenInEditorAction(
                                    new OpenInEditorAction(afp, true),
                                    dispatcher),
                    afp =>
                    {
                        if (afp is null ||
                            afp.IsDirectory)
                        {
                            return Task.FromResult(false);
                        }

                        return Task.FromResult(true);
                    },
                    new[]
                    {
                new InputFilePattern(
                    "File",
                    afp => !afp.IsDirectory)
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

            if (openInEditorAction.AbsoluteFilePath is null ||
                openInEditorAction.AbsoluteFilePath.IsDirectory)
            {
                return;
            }

            _textEditorService.Group.Register(editorTextEditorGroupKey);

            var inputFileAbsoluteFilePathString = openInEditorAction.AbsoluteFilePath.GetAbsoluteFilePathString();

            var textEditorModel = await GetOrCreateTextEditorModelAsync(
                openInEditorAction.AbsoluteFilePath,
                inputFileAbsoluteFilePathString);

            if (textEditorModel is null)
                return;

            await CheckIfContentsWereModifiedAsync(
                dispatcher,
                inputFileAbsoluteFilePathString,
                textEditorModel);

            var viewModel = GetOrCreateTextEditorViewModel(
                openInEditorAction.AbsoluteFilePath,
                openInEditorAction.ShouldSetFocusToEditor,
                dispatcher,
                textEditorModel,
                inputFileAbsoluteFilePathString);

            _textEditorService.Group.AddViewModel(
                editorTextEditorGroupKey,
                viewModel);

            _textEditorService.Group.SetActiveViewModel(
                editorTextEditorGroupKey,
                viewModel);
        }

        private async Task<TextEditorModel?> GetOrCreateTextEditorModelAsync(
            IAbsoluteFilePath absoluteFilePath,
            string absoluteFilePathString)
        {
            var textEditorModel = _textEditorService.Model
                .FindOrDefaultByResourceUri(new(absoluteFilePathString));

            if (textEditorModel is null)
            {
                var resourceUri = new ResourceUri(absoluteFilePathString);

                var fileLastWriteTime = await _fileSystemProvider.File.GetLastWriteTimeAsync(
                    absoluteFilePathString);

                var content = await _fileSystemProvider.File.ReadAllTextAsync(
                    absoluteFilePathString);

                var compilerService = ExtensionNoPeriodFacts.GetCompilerService(
                    absoluteFilePath.ExtensionNoPeriod,
                    _xmlCompilerService,
                    _cSharpCompilerService,
                    _razorCompilerService,
                    _cssCompilerService,
                    _javaScriptCompilerService,
                    _typeScriptCompilerService,
                    _jsonCompilerService);

                var decorationMapper = ExtensionNoPeriodFacts.GetDecorationMapper(
                    absoluteFilePath.ExtensionNoPeriod);

                textEditorModel = new TextEditorModel(
                    resourceUri,
                    fileLastWriteTime,
                    absoluteFilePath.ExtensionNoPeriod,
                    content,
                    compilerService,
                    decorationMapper,
                    null,
                    new(),
                    TextEditorModelKey.NewTextEditorModelKey()
                );

                textEditorModel.CompilerService.RegisterModel(textEditorModel);

                _textEditorService.Model.RegisterCustom(textEditorModel);

                _ = Task.Run(async () =>
                    await textEditorModel.ApplySyntaxHighlightingAsync());
            }

            return textEditorModel;
        }

        private async Task CheckIfContentsWereModifiedAsync(
            IDispatcher dispatcher,
            string inputFileAbsoluteFilePathString,
            TextEditorModel textEditorModel)
        {
            var fileLastWriteTime = await _fileSystemProvider.File.GetLastWriteTimeAsync(
                inputFileAbsoluteFilePathString);

            if (fileLastWriteTime > textEditorModel.ResourceLastWriteTime &&
                _luthetusIdeComponentRenderers.BooleanPromptOrCancelRendererType is not null)
            {
                var notificationInformativeKey = NotificationKey.NewNotificationKey();

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
                        var backgroundTask = new BackgroundTask(
                            async cancellationToken =>
                            {
                                dispatcher.Dispatch(
                                    new NotificationRecordsCollection.DisposeAction(
                                        notificationInformativeKey));

                                var content = await _fileSystemProvider.File
                                    .ReadAllTextAsync(inputFileAbsoluteFilePathString);

                                _textEditorService.Model.Reload(
                                    textEditorModel.ModelKey,
                                    content,
                                    fileLastWriteTime);

                                await textEditorModel.ApplySyntaxHighlightingAsync();
                            },
                            "FileContentsWereModifiedOnDiskTask",
                            "TODO: Describe this task",
                            false,
                            _ => Task.CompletedTask,
                            dispatcher,
                            CancellationToken.None);

                        _commonBackgroundTaskQueue.QueueBackgroundWorkItem(backgroundTask);
                    })
                },
                {
                    nameof(IBooleanPromptOrCancelRendererType.OnAfterDeclineAction),
                    new Action(() =>
                    {
                        dispatcher.Dispatch(
                            new NotificationRecordsCollection.DisposeAction(
                                notificationInformativeKey));
                    })
                },
                    },
                    TimeSpan.FromSeconds(20),
                    null);

                dispatcher.Dispatch(
                    new NotificationRecordsCollection.RegisterAction(
                        notificationInformative));
            }
        }

        private TextEditorViewModelKey GetOrCreateTextEditorViewModel(
            IAbsoluteFilePath absoluteFilePath,
            bool shouldSetFocusToEditor,
            IDispatcher dispatcher,
            TextEditorModel textEditorModel,
            string inputFileAbsoluteFilePathString)
        {
            var viewModel = _textEditorService.Model
                .GetViewModelsOrEmpty(textEditorModel.ModelKey)
                .FirstOrDefault();

            var viewModelKey = viewModel?.ViewModelKey ?? TextEditorViewModelKey.Empty;

            if (viewModel is null)
            {
                viewModelKey = TextEditorViewModelKey.NewTextEditorViewModelKey();

                _textEditorService.ViewModel.Register(
                    viewModelKey,
                    textEditorModel.ModelKey);

                _textEditorService.ViewModel.With(
                    viewModelKey,
                    textEditorViewModel => textEditorViewModel with
                    {
                        OnSaveRequested = HandleOnSaveRequested,
                        GetTabDisplayNameFunc = _ => absoluteFilePath.FilenameWithExtension,
                        ShouldSetFocusAfterNextRender = shouldSetFocusToEditor
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

                var saveFileAction = new FileSystemState.SaveFileAction(
                    absoluteFilePath,
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