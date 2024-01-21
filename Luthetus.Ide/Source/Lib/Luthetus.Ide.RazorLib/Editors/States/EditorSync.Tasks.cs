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
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Microsoft.AspNetCore.Components.Forms;

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

            _textEditorService.ModelApi.RegisterCustom(textEditorModel);

            _textEditorService.Post(
                nameof(_textEditorService.ModelApi.AddPresentationModelFactory),
                async editContext =>
                {
                    await _textEditorService.ModelApi.AddPresentationModelFactory(
                            textEditorModel.ResourceUri,
                            CompilerServiceDiagnosticPresentationFacts.EmptyPresentationModel)
                        .Invoke(editContext);

                    await _textEditorService.ModelApi.AddPresentationModelFactory(
                            textEditorModel.ResourceUri,
                            DiffPresentationFacts.EmptyInPresentationModel)
                        .Invoke(editContext);

                    await _textEditorService.ModelApi.AddPresentationModelFactory(
                            textEditorModel.ResourceUri,
                            DiffPresentationFacts.EmptyOutPresentationModel)
                        .Invoke(editContext);
            
                    textEditorModel.CompilerService.RegisterResource(textEditorModel.ResourceUri);
                });
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
                                    (async () =>
                                    {
                                        dispatcher.Dispatch(new NotificationState.DisposeAction(
                                            notificationInformativeKey));

                                        var content = await _fileSystemProvider.File
                                            .ReadAllTextAsync(inputFileAbsolutePathString);

                                        _textEditorService.Post(
                                            nameof(CheckIfContentsWereModifiedAsync),
                                            async editContext =>
                                            {
                                                await _textEditorService.ModelApi
                                                    .ReloadFactory(
                                                        textEditorModel.ResourceUri,
                                                        content,
                                                        fileLastWriteTime)
                                                    .Invoke(editContext);

                                                await textEditorModel.ApplySyntaxHighlightingAsync();
                                            });
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

            _textEditorService.Post(
                nameof(GetOrCreateTextEditorViewModel),
                _textEditorService.ViewModelApi.WithValueFactory(
                    viewModelKey,
                    textEditorViewModel => textEditorViewModel with
                    {
                        OnSaveRequested = HandleOnSaveRequested,
                        GetTabDisplayNameFunc = _ => absolutePath.NameWithExtension,
                        ShouldSetFocusAfterNextRender = shouldSetFocusToEditor,
                        FirstPresentationLayerKeysList = presentationKeys.ToImmutableList()
                    }));
        }
        else
        {
            viewModel.ShouldSetFocusAfterNextRender = shouldSetFocusToEditor;
        }

        return viewModelKey;

        void HandleOnSaveRequested(ITextEditorModel innerTextEditor)
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
                        _textEditorService.Post(
                            nameof(HandleOnSaveRequested),
                            _textEditorService.ModelApi.SetResourceDataFactory(
                                innerTextEditor.ResourceUri,
                                writtenDateTime.Value));
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
