﻿using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Extensions.Config.BackgroundTasks.Models;
using Luthetus.Extensions.Config.Decorations;
using Luthetus.Extensions.DotNet.BackgroundTasks.Models;
using Luthetus.Extensions.DotNet.DotNetSolutions.Models;
using Luthetus.Extensions.DotNet.Websites.ProjectTemplates.Models;
using Luthetus.Ide.Wasm.Facts;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Facts;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.Diffs.Models;

namespace Luthetus.Website.RazorLib;

public class WebsiteInitializationBackgroundTaskGroup : IBackgroundTaskGroup
{
    public WebsiteInitializationBackgroundTaskGroup(
        IBackgroundTaskService backgroundTaskService,
        ITreeViewService treeViewService,
        IFileSystemProvider fileSystemProvider,
        IEnvironmentProvider environmentProvider,
        ITextEditorHeaderRegistry textEditorHeaderRegistry,
        DotNetBackgroundTaskApi dotNetBackgroundTaskApi,
        ITextEditorService textEditorService,
        IDecorationMapperRegistry decorationMapperRegistry,
        ICompilerServiceRegistry compilerServiceRegistry)
    {
        _backgroundTaskService = backgroundTaskService;
        _treeViewService = treeViewService;
        _fileSystemProvider = fileSystemProvider;
        _environmentProvider = environmentProvider;
        _textEditorHeaderRegistry = textEditorHeaderRegistry;
        _dotNetBackgroundTaskApi = dotNetBackgroundTaskApi;
        _textEditorService = textEditorService;
        _decorationMapperRegistry = decorationMapperRegistry;
        _compilerServiceRegistry = compilerServiceRegistry;
    }

    public Key<IBackgroundTask> BackgroundTaskKey { get; } = Key<IBackgroundTask>.NewKey();
    public Key<IBackgroundTaskQueue> QueueKey { get; } = BackgroundTaskFacts.ContinuousQueueKey;
    public string Name { get; } = nameof(ConfigBackgroundTaskApi);
    public bool EarlyBatchEnabled { get; } = false;

    public bool __TaskCompletionSourceWasCreated { get; set; }

    private readonly Queue<WebsiteInitializationBackgroundTaskGroupWorkKind> _workKindQueue = new();
    private readonly object _workLock = new();
    private readonly IBackgroundTaskService _backgroundTaskService;
    private readonly ITreeViewService _treeViewService;
    private readonly IFileSystemProvider _fileSystemProvider;
    private readonly IEnvironmentProvider _environmentProvider;
    private readonly ITextEditorHeaderRegistry _textEditorHeaderRegistry;
    private readonly DotNetBackgroundTaskApi _dotNetBackgroundTaskApi;
    private readonly ITextEditorService _textEditorService;
    private readonly IDecorationMapperRegistry _decorationMapperRegistry;
    private readonly ICompilerServiceRegistry _compilerServiceRegistry;

    public IBackgroundTask? EarlyBatchOrDefault(IBackgroundTask oldEvent)
    {
        return null;
    }

    public void Enqueue_LuthetusWebsiteInitializerOnAfterRenderAsync()
    {
        lock (_workLock)
        {
            _workKindQueue.Enqueue(WebsiteInitializationBackgroundTaskGroupWorkKind.LuthetusWebsiteInitializerOnAfterRenderAsync);
            _backgroundTaskService.EnqueueGroup(this);
        }
    }
    
    public async ValueTask Do_LuthetusWebsiteInitializerOnAfterRenderAsync()
    {
        await WriteFileSystemInMemoryAsync().ConfigureAwait(false);

        await ParseSolutionAsync().ConfigureAwait(false);

        // This code block is hacky. I want the Solution Explorer to from the get-go be fully expanded, so the user can see 'Program.cs'
        {
            _treeViewService.MoveRight(
                DotNetSolutionState.TreeViewSolutionExplorerStateKey,
                false,
                false);

            _treeViewService.MoveRight(
                DotNetSolutionState.TreeViewSolutionExplorerStateKey,
                false,
                false);

            _treeViewService.MoveRight(
                DotNetSolutionState.TreeViewSolutionExplorerStateKey,
            false,
                false);
        }
    }

    private async Task WriteFileSystemInMemoryAsync()
    {
        // Create a Blazor Wasm app
        await WebsiteProjectTemplateFacts.HandleNewCSharpProjectAsync(
                WebsiteProjectTemplateFacts.BlazorWasmEmptyProjectTemplate.ShortName!,
                InitialSolutionFacts.BLAZOR_CRUD_APP_WASM_CSPROJ_ABSOLUTE_FILE_PATH,
                _fileSystemProvider,
                _environmentProvider)
            .ConfigureAwait(false);

        await _fileSystemProvider.File.WriteAllTextAsync(
                InitialSolutionFacts.PERSON_CS_ABSOLUTE_FILE_PATH,
                InitialSolutionFacts.PERSON_CS_CONTENTS)
            .ConfigureAwait(false);

        await _fileSystemProvider.File.WriteAllTextAsync(
                InitialSolutionFacts.PERSON_DISPLAY_RAZOR_CS_ABSOLUTE_FILE_PATH,
                InitialSolutionFacts.PERSON_DISPLAY_RAZOR_CS_CONTENTS)
            .ConfigureAwait(false);

        await _fileSystemProvider.File.WriteAllTextAsync(
                InitialSolutionFacts.PERSON_DISPLAY_RAZOR_ABSOLUTE_FILE_PATH,
                InitialSolutionFacts.PERSON_DISPLAY_RAZOR_CONTENTS)
            .ConfigureAwait(false);

        /*await _fileSystemProvider.File.WriteAllTextAsync(
                InitialSolutionFacts.BLAZOR_CRUD_APP_ALL_C_SHARP_SYNTAX_ABSOLUTE_FILE_PATH,
                InitialSolutionFacts.BLAZOR_CRUD_APP_ALL_C_SHARP_SYNTAX_CONTENTS)
            .ConfigureAwait(false);*/

        // ExampleSolution.sln
        await _fileSystemProvider.File.WriteAllTextAsync(
                InitialSolutionFacts.SLN_ABSOLUTE_FILE_PATH,
                InitialSolutionFacts.SLN_CONTENTS)
            .ConfigureAwait(false);

        var solutionAbsolutePath = _environmentProvider.AbsolutePathFactory(
            InitialSolutionFacts.SLN_ABSOLUTE_FILE_PATH,
            false);

        // This line is also in LuthetusExtensionsDotNetInitializer,
        // but its duplicated here because the website
        // won't open the first file correctly without this.
        _textEditorHeaderRegistry.UpsertHeader("cs", typeof(Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals.TextEditorCompilerServiceHeaderDisplay));

        _dotNetBackgroundTaskApi.DotNetSolution.Enqueue_SetDotNetSolution(solutionAbsolutePath);

        // Display a file from the get-go so the user is less confused on what the website is.
        var absolutePath = _environmentProvider.AbsolutePathFactory(
            InitialSolutionFacts.PERSON_CS_ABSOLUTE_FILE_PATH,
            false);

        await _textEditorService.OpenInEditorAsync(
            absolutePath.Value,
            false,
            null,
            new Category("main"),
        Key<TextEditorViewModel>.NewKey());
    }

    private async Task ParseSolutionAsync()
    {
        var allFiles = new List<string>();

        await RecursiveStep(
                new List<string> { "/" },
        allFiles)
        .ConfigureAwait(false);

        async Task RecursiveStep(IEnumerable<string> directories, List<string> allFiles)
        {
            foreach (var directory in directories)
            {
                var childDirectories = await _fileSystemProvider.Directory
                    .GetDirectoriesAsync(directory)
                    .ConfigureAwait(false);
                allFiles.AddRange(
                    await _fileSystemProvider.Directory.GetFilesAsync(directory).ConfigureAwait(false));

                await RecursiveStep(childDirectories, allFiles).ConfigureAwait(false);
            }
        }

        foreach (var file in allFiles)
        {
            var absolutePath = _environmentProvider.AbsolutePathFactory(file, false);
            var resourceUri = new ResourceUri(file);
            var fileLastWriteTime = await _fileSystemProvider.File.GetLastWriteTimeAsync(file).ConfigureAwait(false);
            var content = await _fileSystemProvider.File.ReadAllTextAsync(file).ConfigureAwait(false);

            var decorationMapper = _decorationMapperRegistry.GetDecorationMapper(absolutePath.ExtensionNoPeriod);
            var compilerService = _compilerServiceRegistry.GetCompilerService(absolutePath.ExtensionNoPeriod);

            var textEditorModel = new TextEditorModel(
                resourceUri,
                fileLastWriteTime,
                absolutePath.ExtensionNoPeriod,
                content,
                decorationMapper,
                compilerService);

            _textEditorService.ModelApi.RegisterCustom(textEditorModel);

            _textEditorService.TextEditorWorker.PostUnique(
                nameof(_textEditorService.ModelApi.AddPresentationModel),
                editContext =>
                {
                    var modelModifier = editContext.GetModelModifier(textEditorModel.ResourceUri);

                    if (modelModifier is null)
                        return ValueTask.CompletedTask;

                    _textEditorService.ModelApi.AddPresentationModel(
                        editContext,
                        modelModifier,
                        CompilerServiceDiagnosticPresentationFacts.EmptyPresentationModel);

                    _textEditorService.ModelApi.AddPresentationModel(
                        editContext,
                        modelModifier,
                        FindOverlayPresentationFacts.EmptyPresentationModel);

                    _textEditorService.ModelApi.AddPresentationModel(
                        editContext,
                        modelModifier,
                        DiffPresentationFacts.EmptyInPresentationModel);

                    _textEditorService.ModelApi.AddPresentationModel(
                        editContext,
                        modelModifier,
                        DiffPresentationFacts.EmptyOutPresentationModel);

                    textEditorModel.CompilerService.RegisterResource(
                        textEditorModel.ResourceUri,
                        shouldTriggerResourceWasModified: true);
                    return ValueTask.CompletedTask;
                });
        }
    }

    public ValueTask HandleEvent(CancellationToken cancellationToken)
    {
        WebsiteInitializationBackgroundTaskGroupWorkKind workKind;

        lock (_workLock)
        {
            if (!_workKindQueue.TryDequeue(out workKind))
                return ValueTask.CompletedTask;
        }

        switch (workKind)
        {
            case WebsiteInitializationBackgroundTaskGroupWorkKind.LuthetusWebsiteInitializerOnAfterRenderAsync:
            {
                return Do_LuthetusWebsiteInitializerOnAfterRenderAsync();
            }
            default:
            {
                return ValueTask.CompletedTask;
            }
        }
    }
}
