using Luthetus.Ide.ClassLib.InputFile;
using Luthetus.Ide.ClassLib.Store.FileSystemCase;
using Luthetus.Ide.ClassLib.Store.InputFileCase;
using Luthetus.Ide.ClassLib.ComponentRenderers;
using Luthetus.Ide.ClassLib.FileConstants;
using Luthetus.Common.RazorLib.BackgroundTaskCase.Usage;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.Group;
using Fluxor;
using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.Model;
using Luthetus.TextEditor.RazorLib.Lexing;
using Luthetus.Common.RazorLib.Notification;
using Luthetus.Common.RazorLib.ComponentRenderers.Types;
using Luthetus.Common.RazorLib.BackgroundTaskCase.BaseTypes;
using Luthetus.Common.RazorLib.Store.NotificationCase;
using Luthetus.TextEditor.RazorLib.ViewModel;
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
using Luthetus.TextEditor.RazorLib.Decoration;

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
        private readonly ILuthetusCommonBackgroundTaskService _luthetusCommonBackgroundTaskService;
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
            ILuthetusCommonBackgroundTaskService luthetusCommonBackgroundTaskService,
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
            _luthetusCommonBackgroundTaskService = luthetusCommonBackgroundTaskService;
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
            dispatcher.Dispatch(new InputFileState.RequestInputFileStateFormAction(
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

            if (openInEditorAction.AbsoluteFilePath is null ||
                openInEditorAction.AbsoluteFilePath.IsDirectory)
            {
                return;
            }

            _textEditorService.Group.Register(editorTextEditorGroupKey);

            var inputFileAbsoluteFilePathString = openInEditorAction.AbsoluteFilePath.FormattedInput;

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
                                        dispatcher.Dispatch(new NotificationRecordsCollection.DisposeAction(
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

                                _luthetusCommonBackgroundTaskService.QueueBackgroundWorkItem(backgroundTask);
                            })
                        },
                        {
                            nameof(IBooleanPromptOrCancelRendererType.OnAfterDeclineAction),
                            new Action(() =>
                            {
                                dispatcher.Dispatch(new NotificationRecordsCollection.DisposeAction(
                                    notificationInformativeKey));
                            })
                        },
                    },
                    TimeSpan.FromSeconds(20),
                    true,
                    null);

                dispatcher.Dispatch(new NotificationRecordsCollection.RegisterAction(
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

                var presentationKeys = new[]
                {
                    CompilerServiceDiagnosticPresentationFacts.PresentationKey
                }.ToImmutableArray();

                _textEditorService.ViewModel.With(
                    viewModelKey,
                    textEditorViewModel => textEditorViewModel with
                    {
                        OnSaveRequested = HandleOnSaveRequested,
                        GetTabDisplayNameFunc = _ => absoluteFilePath.FilenameWithExtension,
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