using Fluxor;
using System.Collections.Immutable;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.Notifications.States;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.Groups.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Facts;
using Luthetus.TextEditor.RazorLib.Diffs.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.Ide.RazorLib.InputFiles.Models;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;

namespace Luthetus.Ide.RazorLib.Editors.Models;

public class LuthetusIdeEditorBackgroundTaskApi
{
    public static readonly Key<TextEditorGroup> EditorTextEditorGroupKey = Key<TextEditorGroup>.NewKey();
    
    private readonly LuthetusIdeBackgroundTaskApi _ideBackgroundTaskApi;
    private readonly IBackgroundTaskService _backgroundTaskService;
    private readonly ITextEditorService _textEditorService;
    private readonly ILuthetusIdeComponentRenderers _ideComponentRenderers;
    private readonly IFileSystemProvider _fileSystemProvider;
    private readonly IEnvironmentProvider _environmentProvider;
    private readonly IDecorationMapperRegistry _decorationMapperRegistry;
    private readonly ICompilerServiceRegistry _compilerServiceRegistry;
    private readonly IDispatcher _dispatcher;
    private readonly IServiceProvider _serviceProvider;

    public LuthetusIdeEditorBackgroundTaskApi(
        LuthetusIdeBackgroundTaskApi ideBackgroundTaskApi,
        IBackgroundTaskService backgroundTaskService,
        ITextEditorService textEditorService,
        ILuthetusIdeComponentRenderers ideComponentRenderers,
        IFileSystemProvider fileSystemProvider,
        IEnvironmentProvider environmentProvider,
        IDecorationMapperRegistry decorationMapperRegistry,
        ICompilerServiceRegistry compilerServiceRegistry,
        IDispatcher dispatcher,
        IServiceProvider serviceProvider)
    {
        _ideBackgroundTaskApi = ideBackgroundTaskApi;
        _backgroundTaskService = backgroundTaskService;
        _textEditorService = textEditorService;
        _ideComponentRenderers = ideComponentRenderers;
        _fileSystemProvider = fileSystemProvider;
        _environmentProvider = environmentProvider;
        _decorationMapperRegistry = decorationMapperRegistry;
        _compilerServiceRegistry = compilerServiceRegistry;
        _dispatcher = dispatcher;
        _serviceProvider = serviceProvider;
    }

    public Task OpenInEditor(
        IAbsolutePath? absolutePath,
        bool shouldSetFocusToEditor,
        Key<TextEditorGroup>? editorTextEditorGroupKey = null)
    {
        return _backgroundTaskService.EnqueueAsync(
            Key<BackgroundTask>.NewKey(),
            ContinuousBackgroundTaskWorker.GetQueueKey(),
            "OpenInEditor",
            async () => await OpenInEditorAsync(
                absolutePath,
                shouldSetFocusToEditor,
                editorTextEditorGroupKey));
    }

    public Task ShowInputFile()
    {
        return _ideBackgroundTaskApi.InputFile.RequestInputFileStateForm(
            "TextEditor",
            absolutePath =>
            {
                return OpenInEditor(absolutePath, true);
            },
            absolutePath =>
            {
                if (absolutePath is null || absolutePath.IsDirectory)
                    return Task.FromResult(false);

                return Task.FromResult(true);
            },
            new[]
            {
                    new InputFilePattern("File", absolutePath => !absolutePath.IsDirectory)
            }.ToImmutableArray());
    }

    public async Task RegisterModelFunc(RegisterModelArgs registerModelArgs)
    {
        var model = _textEditorService.ModelApi.GetOrDefault(registerModelArgs.ResourceUri);

        if (model is null)
        {
            var resourceUri = registerModelArgs.ResourceUri;

            var fileLastWriteTime = await _fileSystemProvider.File
                .GetLastWriteTimeAsync(resourceUri.Value)
                .ConfigureAwait(false);

            var content = await _fileSystemProvider.File
                .ReadAllTextAsync(resourceUri.Value)
                .ConfigureAwait(false);

            var absolutePath = _environmentProvider.AbsolutePathFactory(resourceUri.Value, false);

            var decorationMapper = _decorationMapperRegistry.GetDecorationMapper(absolutePath.ExtensionNoPeriod);
            var compilerService = _compilerServiceRegistry.GetCompilerService(absolutePath.ExtensionNoPeriod);

            model = new TextEditorModel(
                resourceUri,
                fileLastWriteTime,
                absolutePath.ExtensionNoPeriod,
                content,
                decorationMapper,
                compilerService);

            _textEditorService.ModelApi.RegisterCustom(model);

            await _textEditorService.Post(
				nameof(RegisterModelFunc),
                model.ResourceUri,
                Key<TextEditorViewModel>.Empty,
                async editContext =>
                {
                    await _textEditorService.ModelApi.AddPresentationModelFactory(
                            model.ResourceUri,
                            CompilerServiceDiagnosticPresentationFacts.EmptyPresentationModel)
                        .Invoke(editContext)
                        .ConfigureAwait(false);

                    await _textEditorService.ModelApi.AddPresentationModelFactory(
                            model.ResourceUri,
                            FindOverlayPresentationFacts.EmptyPresentationModel)
                        .Invoke(editContext)
                        .ConfigureAwait(false);

                    await _textEditorService.ModelApi.AddPresentationModelFactory(
                            model.ResourceUri,
                            DiffPresentationFacts.EmptyInPresentationModel)
                        .Invoke(editContext)
                        .ConfigureAwait(false);

                    await _textEditorService.ModelApi.AddPresentationModelFactory(
                            model.ResourceUri,
                            DiffPresentationFacts.EmptyOutPresentationModel)
                        .Invoke(editContext)
                        .ConfigureAwait(false);

                    model.CompilerService.RegisterResource(model.ResourceUri);
                });
        }

        await CheckIfContentsWereModifiedAsync(
                _dispatcher,
                registerModelArgs.ResourceUri.Value,
                model)
            .ConfigureAwait(false);
    }

    public async Task<Key<TextEditorViewModel>> TryRegisterViewModelFunc(TryRegisterViewModelArgs registerViewModelArgs)
    {
        var model = _textEditorService.ModelApi.GetOrDefault(registerViewModelArgs.ResourceUri);

        if (model is null)
            return Key<TextEditorViewModel>.Empty;

        var viewModel = _textEditorService.ModelApi
            .GetViewModelsOrEmpty(registerViewModelArgs.ResourceUri)
            .FirstOrDefault(x => x.Category == registerViewModelArgs.Category);

        if (viewModel is not null)
            return viewModel.ViewModelKey;

        var viewModelKey = Key<TextEditorViewModel>.NewKey();

        _textEditorService.ViewModelApi.Register(
            viewModelKey,
            registerViewModelArgs.ResourceUri,
            registerViewModelArgs.Category);

        var layerLastPresentationKeys = new[]
        {
            CompilerServiceDiagnosticPresentationFacts.PresentationKey,
            FindOverlayPresentationFacts.PresentationKey,
        }.ToImmutableArray();

        var absolutePath = _environmentProvider.AbsolutePathFactory(
            registerViewModelArgs.ResourceUri.Value,
            false);

        await _textEditorService.Post(
			nameof(TryRegisterViewModelFunc),
            registerViewModelArgs.ResourceUri,
            viewModelKey,
            _textEditorService.ViewModelApi.WithValueFactory(
                viewModelKey,
                textEditorViewModel =>
                {
                    textEditorViewModel.UnsafeState.ShouldSetFocusAfterNextRender = registerViewModelArgs.ShouldSetFocusToEditor;

                    return textEditorViewModel with
                    {
                        OnSaveRequested = HandleOnSaveRequested,
                        GetTabDisplayNameFunc = _ => absolutePath.NameWithExtension,
                        LastPresentationLayerKeysList = layerLastPresentationKeys.ToImmutableList()
                    };
                }));

        return viewModelKey;

        void HandleOnSaveRequested(ITextEditorModel innerTextEditor)
        {
            var innerContent = innerTextEditor.GetAllText();

            var cancellationToken = model.TextEditorSaveFileHelper.GetCancellationToken();

            _ideBackgroundTaskApi.FileSystem.SaveFile(
                absolutePath,
                innerContent,
                async writtenDateTime =>
                {
                    if (writtenDateTime is not null)
                    {
                        await _textEditorService.Post(
							nameof(HandleOnSaveRequested),
                            innerTextEditor.ResourceUri,
                            Key<TextEditorViewModel>.Empty,
                            _textEditorService.ModelApi.SetResourceDataFactory(
                                innerTextEditor.ResourceUri,
                                writtenDateTime.Value));
                    }
                },
                cancellationToken);
        }
    }

    public Task<bool> TryShowViewModelFunc(TryShowViewModelArgs showViewModelArgs)
    {
        _textEditorService.GroupApi.Register(EditorTextEditorGroupKey);

        var viewModel = _textEditorService.ViewModelApi.GetOrDefault(showViewModelArgs.ViewModelKey);

        if (viewModel is null)
            return Task.FromResult(false);

        if (viewModel.Category == new Category("main") &&
            showViewModelArgs.GroupKey == Key<TextEditorGroup>.Empty)
        {
            showViewModelArgs = new TryShowViewModelArgs(
                showViewModelArgs.ViewModelKey,
                EditorTextEditorGroupKey,
                showViewModelArgs.ServiceProvider);
        }

        if (showViewModelArgs.ViewModelKey == Key<TextEditorViewModel>.Empty ||
            showViewModelArgs.GroupKey == Key<TextEditorGroup>.Empty)
        {
            return Task.FromResult(false);
        }

        _textEditorService.GroupApi.AddViewModel(
            showViewModelArgs.GroupKey,
            showViewModelArgs.ViewModelKey);

        _textEditorService.GroupApi.SetActiveViewModel(
            showViewModelArgs.GroupKey,
            showViewModelArgs.ViewModelKey);

        return Task.FromResult(true);
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

        var resourceUri = new ResourceUri(absolutePath.Value);

        await RegisterModelFunc(new RegisterModelArgs(
                resourceUri,
                _serviceProvider))
            .ConfigureAwait(false);

        var viewModelKey = await TryRegisterViewModelFunc(new TryRegisterViewModelArgs(
                Key<TextEditorViewModel>.NewKey(),
                resourceUri,
                new Category("main"),
                shouldSetFocusToEditor,
                _serviceProvider))
            .ConfigureAwait(false);

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
        var fileLastWriteTime = await _fileSystemProvider.File
            .GetLastWriteTimeAsync(inputFileAbsolutePathString)
            .ConfigureAwait(false);

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
                            new Func<Task>(async () =>
                            {
                                await _backgroundTaskService.EnqueueAsync(
                                        Key<BackgroundTask>.NewKey(),
                                        ContinuousBackgroundTaskWorker.GetQueueKey(),
                                        "Check If Contexts Were Modified",
                                        async () =>
                                        {
                                            dispatcher.Dispatch(new NotificationState.DisposeAction(
                                                notificationInformativeKey));

                                            var content = await _fileSystemProvider.File
                                                .ReadAllTextAsync(inputFileAbsolutePathString)
                                                .ConfigureAwait(false);

                                            await _textEditorService.Post(
												nameof(CheckIfContentsWereModifiedAsync),
                                                textEditorModel.ResourceUri,
                                                Key<TextEditorViewModel>.Empty,
                                                async editContext =>
                                                {
                                                    await _textEditorService.ModelApi
                                                        .ReloadFactory(
                                                            textEditorModel.ResourceUri,
                                                            content,
                                                            fileLastWriteTime)
                                                        .Invoke(editContext)
                                                        .ConfigureAwait(false);

                                                    await editContext.TextEditorService.ModelApi.ApplySyntaxHighlightingFactory(
                                                            textEditorModel.ResourceUri)
                                                        .Invoke(editContext)
                                                        .ConfigureAwait(false);
                                                });
                                        })
                                    .ConfigureAwait(false);
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
