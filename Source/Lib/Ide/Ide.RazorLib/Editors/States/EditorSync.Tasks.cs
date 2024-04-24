using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Notifications.States;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.TextEditor.RazorLib.Groups.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;

namespace Luthetus.Ide.RazorLib.Editors.States;

public partial class EditorSync
{
    private async Task OpenInEditorAsync(
        IAbsolutePath? absolutePath,
        bool shouldSetFocusToEditor,
        Key<TextEditorGroup>? editorTextEditorGroupKey = null)
    {
        editorTextEditorGroupKey ??= EditorTextEditorGroupKey;

        if (absolutePath is null || absolutePath.IsDirectory)
            return;

        _textEditorService.GroupApi.Register(editorTextEditorGroupKey.Value);

        var resourceUri = new ResourceUri(absolutePath.Value);

        await RegisterModelFunc(new RegisterModelArgs(
            resourceUri,
            _serviceProvider));

        var viewModelKey = await TryRegisterViewModelFunc(new TryRegisterViewModelArgs(
            Key<TextEditorViewModel>.NewKey(),
            resourceUri,
            new Category("main"),
            shouldSetFocusToEditor,
            _serviceProvider));

        _textEditorService.GroupApi.AddViewModel(
            editorTextEditorGroupKey.Value,
            viewModelKey);

        _textEditorService.GroupApi.SetActiveViewModel(
            editorTextEditorGroupKey.Value,
            viewModelKey);
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
            var notificationInformativeKey = Key<IDynamicViewModel>.NewKey();

            var notificationInformative = new NotificationViewModel(
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
                            nameof(IBooleanPromptOrCancelRendererType.OnAfterAcceptFunc),
                            new Func<Task>(() =>
                            {
								BackgroundTaskService.Enqueue(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.GetQueueKey(),
                                    "Check If Contexts Were Modified",
                                    async () =>
                                    {
                                        dispatcher.Dispatch(new NotificationState.DisposeAction(
                                            notificationInformativeKey));

                                        var content = await _fileSystemProvider.File
                                            .ReadAllTextAsync(inputFileAbsolutePathString);

                                        _textEditorService.PostIndependent(
                                            nameof(CheckIfContentsWereModifiedAsync),
                                            async editContext =>
                                            {
                                                await _textEditorService.ModelApi
                                                    .ReloadFactory(
                                                        textEditorModel.ResourceUri,
                                                        content,
                                                        fileLastWriteTime)
                                                    .Invoke(editContext);

                                                await editContext.TextEditorService.ModelApi.ApplySyntaxHighlightingFactory(
                                                        textEditorModel.ResourceUri)
                                                    .Invoke(editContext)
                                                    .ConfigureAwait(false);
                                            });
                                    });

                                return Task.CompletedTask;
                            })
                        },
                        {
                            nameof(IBooleanPromptOrCancelRendererType.OnAfterDeclineFunc),
                            new Func<Task>(() =>
                            {
                                dispatcher.Dispatch(new NotificationState.DisposeAction(
                                    notificationInformativeKey));

                                return Task.CompletedTask;
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
}
