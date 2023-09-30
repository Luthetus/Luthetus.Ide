using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTaskCase.Models;
using Luthetus.Common.RazorLib.FileSystem.Models;
using Luthetus.Common.RazorLib.KeyCase.Models;
using Luthetus.Common.RazorLib.Notification.Models;
using Luthetus.Common.RazorLib.Notification.States;
using Luthetus.Ide.RazorLib.ComponentRenderersCase.Models;
using Luthetus.Ide.RazorLib.FileSystemCase.Models;
using Luthetus.Ide.RazorLib.InputFileCase.Models;
using Luthetus.TextEditor.RazorLib.CompilerServiceCase;
using Luthetus.TextEditor.RazorLib.Group.Models;
using Luthetus.TextEditor.RazorLib.Lexing.Models;
using Luthetus.TextEditor.RazorLib.TextEditorCase.Model;
using Luthetus.TextEditor.RazorLib.TextEditorCase.Scenes;
using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.EditorCase.States;

public partial class EditorSync
{
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
                Key<TextEditorModel>.NewKey()
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
            var notificationInformativeKey = Key<NotificationRecord>.NewKey();

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
                                BackgroundTaskService.Enqueue(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.Queue.Key,
                                    "Check If Contexts Were Modified",
                                    async () =>
                                    {
                                        dispatcher.Dispatch(new NotificationState.DisposeAction(
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
                                dispatcher.Dispatch(new NotificationState.DisposeAction(
                                    notificationInformativeKey));
                            })
                        },
                },
                TimeSpan.FromSeconds(20),
                true,
                null);

            dispatcher.Dispatch(new NotificationState.RegisterAction(
                notificationInformative));
        }
    }

    private Key<TextEditorViewModel> GetOrCreateTextEditorViewModel(
        IAbsolutePath absolutePath,
        bool shouldSetFocusToEditor,
        IDispatcher dispatcher,
        TextEditorModel textEditorModel,
        string inputFileAbsolutePathString)
    {
        var viewModel = _textEditorService.Model
            .GetViewModelsOrEmpty(textEditorModel.ModelKey)
            .FirstOrDefault();

        var viewModelKey = viewModel?.ViewModelKey ?? Key<TextEditorViewModel>.Empty;

        if (viewModel is null)
        {
            viewModelKey = Key<TextEditorViewModel>.NewKey();

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

            _fileSystemSync.SaveFile(
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
        }
    }

    private async Task OpenInEditorAsync(
        IAbsolutePath? absolutePath,
        bool shouldSetFocusToEditor,
        Key<TextEditorGroup>? editorTextEditorGroupKey = null)
    {
        editorTextEditorGroupKey ??= EditorTextEditorGroupKey;

        if (absolutePath is null || absolutePath.IsDirectory)
            return;

        _textEditorService.Group.Register(editorTextEditorGroupKey.Value);

        var inputFileAbsolutePathString = absolutePath.FormattedInput;

        var textEditorModel = await GetOrCreateTextEditorModelAsync(
            absolutePath,
            inputFileAbsolutePathString);

        if (textEditorModel is null)
            return;

        await CheckIfContentsWereModifiedAsync(
            Dispatcher,
            inputFileAbsolutePathString,
            textEditorModel);

        var viewModel = GetOrCreateTextEditorViewModel(
            absolutePath,
            shouldSetFocusToEditor,
            Dispatcher,
            textEditorModel,
            inputFileAbsolutePathString);

        _textEditorService.Group.AddViewModel(
            editorTextEditorGroupKey.Value,
            viewModel);

        _textEditorService.Group.SetActiveViewModel(
            editorTextEditorGroupKey.Value,
            viewModel);
    }
}
