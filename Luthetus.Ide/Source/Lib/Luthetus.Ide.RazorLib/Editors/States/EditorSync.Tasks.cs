using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Notifications.States;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Groups.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using System.Collections.Immutable;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.TextEditor.RazorLib.Diffs.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;

namespace Luthetus.Ide.RazorLib.Editors.States;

public partial class EditorSync
{
    private async Task<TextEditorModel?> GetOrCreateTextEditorModelAsync(
        IAbsolutePath absolutePath,
        string absolutePathString)
    {
        var textEditorModel = _textEditorService.ModelApi
            .GetOrDefault(new(absolutePathString));

        if (textEditorModel is null)
        {
            var resourceUri = new ResourceUri(absolutePathString);
            var fileLastWriteTime = await _fileSystemProvider.File.GetLastWriteTimeAsync(absolutePathString);
            var content = await _fileSystemProvider.File.ReadAllTextAsync(absolutePathString);

            var decorationMapper = _decorationMapperRegistry.GetDecorationMapper(absolutePath.ExtensionNoPeriod);
            var compilerService = _compilerServiceRegistry.GetCompilerService(absolutePath.ExtensionNoPeriod);

            textEditorModel = new TextEditorModel(
                resourceUri,
                fileLastWriteTime,
                absolutePath.ExtensionNoPeriod,
                content,
                decorationMapper,
                compilerService);

            textEditorModel.CompilerService.RegisterResource(textEditorModel.ResourceUri);

            _textEditorService.ModelApi.RegisterCustom(textEditorModel);

            _textEditorService.ModelApi.RegisterPresentationModel(
                textEditorModel.ResourceUri,
                CompilerServiceDiagnosticPresentationFacts.EmptyPresentationModel);

            _textEditorService.ModelApi.RegisterPresentationModel(
                textEditorModel.ResourceUri,
                DiffPresentationFacts.EmptyInPresentationModel);

            _textEditorService.ModelApi.RegisterPresentationModel(
                textEditorModel.ResourceUri,
                DiffPresentationFacts.EmptyOutPresentationModel);

            _ = Task.Run(async () => await textEditorModel.ApplySyntaxHighlightingAsync());
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
            _ideComponentRenderers.BooleanPromptOrCancelRendererType is not null)
        {
            var notificationInformativeKey = Key<NotificationRecord>.NewKey();

            var notificationInformative = new NotificationRecord(
                notificationInformativeKey,
                "File contents were modified on disk",
                _ideComponentRenderers.BooleanPromptOrCancelRendererType,
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
								BackgroundTaskService.Enqueue(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.GetQueueKey(),
                                    "Check If Contexts Were Modified",
                                    (Func<Task>)(async () =>
                                    {
                                        dispatcher.Dispatch(new NotificationState.DisposeAction(
                                            notificationInformativeKey));

                                        var content = await _fileSystemProvider.File
                                            .ReadAllTextAsync(inputFileAbsolutePathString);

										_textEditorService.ModelApi.ReloadEnqueue(
                                            textEditorModel.ResourceUri,
                                            content,
                                            fileLastWriteTime);

                                        await textEditorModel.ApplySyntaxHighlightingAsync();
                                    }));
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
        TextEditorModel textEditorModel)
    {
        var viewModel = _textEditorService.ModelApi
            .GetViewModelsOrEmpty(textEditorModel.ResourceUri)
            .FirstOrDefault();

        var viewModelKey = viewModel?.ViewModelKey ?? Key<TextEditorViewModel>.Empty;

        if (viewModel is null)
        {
            viewModelKey = Key<TextEditorViewModel>.NewKey();

            _textEditorService.ViewModelApi.Register(
                viewModelKey,
                textEditorModel.ResourceUri);

            var presentationKeys = new[]
            {
                CompilerServiceDiagnosticPresentationFacts.PresentationKey,
            }.ToImmutableArray();

            _textEditorService.ViewModelApi.WithValue(
                viewModelKey,
                textEditorViewModel => textEditorViewModel with
                {
                    OnSaveRequested = HandleOnSaveRequested,
                    GetTabDisplayNameFunc = _ => absolutePath.NameWithExtension,
                    ShouldSetFocusAfterNextRender = shouldSetFocusToEditor,
                    FirstPresentationLayerKeysBag = presentationKeys.ToImmutableList()
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
                (Action<DateTime?>)(                writtenDateTime =>
                {
                    if (writtenDateTime is not null)
                    {
						_textEditorService.ModelApi.SetResourceDataEnqueue(
                            innerTextEditor.ResourceUri,
                            writtenDateTime.Value);
                    }
                }),
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

        _textEditorService.GroupApi.Register(editorTextEditorGroupKey.Value);

        var inputFileAbsolutePathString = absolutePath.Value;

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
            textEditorModel);

        _textEditorService.GroupApi.AddViewModel(
            editorTextEditorGroupKey.Value,
            viewModel);

        _textEditorService.GroupApi.SetActiveViewModel(
            editorTextEditorGroupKey.Value,
            viewModel);
    }
}
