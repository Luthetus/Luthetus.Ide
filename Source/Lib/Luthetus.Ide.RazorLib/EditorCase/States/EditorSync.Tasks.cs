using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTaskCase.BaseTypes;
using Luthetus.Common.RazorLib.FileSystem.Interfaces;
using Luthetus.Common.RazorLib.Notification;
using Luthetus.Common.RazorLib.Store.NotificationCase;
using Luthetus.Ide.RazorLib.ComponentRenderersCase.Models;
using Luthetus.Ide.RazorLib.FileSystemCase.Models;
using Luthetus.Ide.RazorLib.InputFileCase.Models;
using Luthetus.Ide.RazorLib.InputFileCase.States;
using Luthetus.TextEditor.RazorLib.CompilerServiceCase;
using Luthetus.TextEditor.RazorLib.Lexing;
using Luthetus.TextEditor.RazorLib.Model;
using Luthetus.TextEditor.RazorLib.ViewModel.InternalClasses;
using System.Collections.Immutable;
using static Luthetus.Ide.RazorLib.EditorCase.States.EditorState;

namespace Luthetus.Ide.RazorLib.EditorCase.States;

public partial class EditorSync
{
    public Task ShowInputFile(ShowInputFileAction showInputFileAction)
    {
        Dispatcher.Dispatch(new InputFileRegistry.RequestInputFileStateFormAction(
            "TextEditor",
            async afp => await OpenInEditor(new OpenInEditorAction(this, afp, true)),
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

    public async Task OpenInEditor(OpenInEditorAction openInEditorAction)
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
            Dispatcher,
            inputFileAbsolutePathString,
            textEditorModel);

        var viewModel = GetOrCreateTextEditorViewModel(
            openInEditorAction.AbsolutePath,
            openInEditorAction.ShouldSetFocusToEditor,
            Dispatcher,
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
                                BackgroundTaskService.Enqueue(BackgroundTaskKey.NewKey(), ContinuousBackgroundTaskWorker.Queue.Key,
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

            var saveFileAction = new FileSystemCase.States.FileSystemRegistry.SaveFileAction(
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
