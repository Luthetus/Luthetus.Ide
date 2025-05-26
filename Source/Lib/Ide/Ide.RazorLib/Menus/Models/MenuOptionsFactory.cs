using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Namespaces.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.Clipboards.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.FileSystems.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.Clipboards.Models;

namespace Luthetus.Ide.RazorLib.Menus.Models;

public class MenuOptionsFactory : IMenuOptionsFactory, IBackgroundTaskGroup
{
    private readonly IIdeComponentRenderers _ideComponentRenderers;
    private readonly ICommonComponentRenderers _commonComponentRenderers;
    private readonly IFileSystemProvider _fileSystemProvider;
    private readonly IEnvironmentProvider _environmentProvider;
    private readonly IClipboardService _clipboardService;
    private readonly BackgroundTaskService _backgroundTaskService;

    public MenuOptionsFactory(
        IIdeComponentRenderers ideComponentRenderers,
        ICommonComponentRenderers commonComponentRenderers,
        IFileSystemProvider fileSystemProvider,
        IEnvironmentProvider environmentProvider,
        IClipboardService clipboardService,
        BackgroundTaskService backgroundTaskService)
    {
        _ideComponentRenderers = ideComponentRenderers;
        _commonComponentRenderers = commonComponentRenderers;
        _fileSystemProvider = fileSystemProvider;
        _environmentProvider = environmentProvider;
        _clipboardService = clipboardService;
        _backgroundTaskService = backgroundTaskService;
    }

    public Key<IBackgroundTaskGroup> BackgroundTaskKey { get; } = Key<IBackgroundTaskGroup>.NewKey();
    public string Name { get; } = nameof(MenuOptionsFactory);
    public bool EarlyBatchEnabled { get; } = false;

    public bool __TaskCompletionSourceWasCreated { get; set; }

    private readonly Queue<MenuOptionsFactoryWorkKind> _workKindQueue = new();
    private readonly object _workLock = new();

    public MenuOptionRecord NewEmptyFile(AbsolutePath parentDirectory, Func<Task> onAfterCompletion)
    {
        return new MenuOptionRecord("New Empty File", MenuOptionKind.Create,
            widgetRendererType: _ideComponentRenderers.FileFormRendererType,
            widgetParameterMap: new Dictionary<string, object?>
            {
                { nameof(IFileFormRendererType.FileName), string.Empty },
                { nameof(IFileFormRendererType.CheckForTemplates), false },
                {
                    nameof(IFileFormRendererType.OnAfterSubmitFunc),
                    new Func<string, IFileTemplate?, List<IFileTemplate>, Task>(
                        (fileName, exactMatchFileTemplate, relatedMatchFileTemplates) =>
						{
                            Enqueue_PerformNewFile(
                                fileName,
                                exactMatchFileTemplate,
                                relatedMatchFileTemplates,
                                new NamespacePath(string.Empty, parentDirectory),
                                onAfterCompletion);

							return Task.CompletedTask;
						})
                },
            });
    }

    public MenuOptionRecord NewTemplatedFile(NamespacePath parentDirectory, Func<Task> onAfterCompletion)
    {
        return new MenuOptionRecord("New Templated File", MenuOptionKind.Create,
            widgetRendererType: _ideComponentRenderers.FileFormRendererType,
            widgetParameterMap: new Dictionary<string, object?>
            {
                { nameof(IFileFormRendererType.FileName), string.Empty },
                { nameof(IFileFormRendererType.CheckForTemplates), true },
                {
                    nameof(IFileFormRendererType.OnAfterSubmitFunc),
                    new Func<string, IFileTemplate?, List<IFileTemplate>, Task>(
                        (fileName, exactMatchFileTemplate, relatedMatchFileTemplates) =>
						{
                            Enqueue_PerformNewFile(
                                fileName,
                                exactMatchFileTemplate,
                                relatedMatchFileTemplates,
                                parentDirectory,
                                onAfterCompletion);

							return Task.CompletedTask;
						})
                },
            });
    }

    public MenuOptionRecord NewDirectory(AbsolutePath parentDirectory, Func<Task> onAfterCompletion)
    {
        return new MenuOptionRecord("New Directory", MenuOptionKind.Create,
            widgetRendererType: _ideComponentRenderers.FileFormRendererType,
            widgetParameterMap: new Dictionary<string, object?>
            {
                { nameof(IFileFormRendererType.FileName), string.Empty },
                { nameof(IFileFormRendererType.IsDirectory), true },
                {
                    nameof(IFileFormRendererType.OnAfterSubmitFunc),
                    new Func<string, IFileTemplate?, List<IFileTemplate>, Task>(
                        (directoryName, _, _) =>
						{
                            Enqueue_PerformNewDirectory(directoryName, parentDirectory, onAfterCompletion);
							return Task.CompletedTask;
						})
                },
            });
    }

    public MenuOptionRecord DeleteFile(AbsolutePath absolutePath, Func<Task> onAfterCompletion)
    {
        return new MenuOptionRecord("Delete", MenuOptionKind.Delete,
            widgetRendererType: _ideComponentRenderers.DeleteFileFormRendererType,
            widgetParameterMap: new Dictionary<string, object?>
            {
                { nameof(IDeleteFileFormRendererType.AbsolutePath), absolutePath },
                { nameof(IDeleteFileFormRendererType.IsDirectory), true },
                {
                    nameof(IDeleteFileFormRendererType.OnAfterSubmitFunc),
                    new Func<AbsolutePath, Task>(
						x => 
						{
							Enqueue_PerformDeleteFile(x, onAfterCompletion);
							return Task.CompletedTask;
						})
                },
            });
    }

    public MenuOptionRecord RenameFile(AbsolutePath sourceAbsolutePath, INotificationService notificationService, Func<Task> onAfterCompletion)
    {
        return new MenuOptionRecord("Rename", MenuOptionKind.Update,
            widgetRendererType: _ideComponentRenderers.FileFormRendererType,
            widgetParameterMap: new Dictionary<string, object?>
            {
                {
                    nameof(IFileFormRendererType.FileName),
                    sourceAbsolutePath.IsDirectory
                        ? sourceAbsolutePath.NameNoExtension
                        : sourceAbsolutePath.NameWithExtension
                },
                { nameof(IFileFormRendererType.IsDirectory), sourceAbsolutePath.IsDirectory },
                {
                    nameof(IFileFormRendererType.OnAfterSubmitFunc),
                    new Func<string, IFileTemplate?, List<IFileTemplate>, Task>((nextName, _, _) =>
                    {
                        PerformRename(sourceAbsolutePath, nextName, notificationService, onAfterCompletion);
                        return Task.CompletedTask;
                    })
                },
            });
    }

    public MenuOptionRecord CopyFile(AbsolutePath absolutePath, Func<Task> onAfterCompletion)
    {
        return new MenuOptionRecord("Copy", MenuOptionKind.Update,
            onClickFunc: () =>
			{
                Enqueue_PerformCopyFile(absolutePath, onAfterCompletion);
				return Task.CompletedTask;
			});
    }

    public MenuOptionRecord CutFile(AbsolutePath absolutePath, Func<Task> onAfterCompletion)
    {
        return new MenuOptionRecord("Cut", MenuOptionKind.Update,
            onClickFunc: () =>
			{
                Enqueue_PerformCutFile(absolutePath, onAfterCompletion);
				return Task.CompletedTask;
			});
    }

    public MenuOptionRecord PasteClipboard(AbsolutePath directoryAbsolutePath, Func<Task> onAfterCompletion)
    {
        return new MenuOptionRecord("Paste", MenuOptionKind.Update,
            onClickFunc: () =>
			{
                Enqueue_PerformPasteFile(directoryAbsolutePath, onAfterCompletion);
				return Task.CompletedTask;
			});
    }

    private readonly
        Queue<(string fileName, IFileTemplate? exactMatchFileTemplate, List<IFileTemplate> relatedMatchFileTemplatesList, NamespacePath namespacePath, Func<Task> onAfterCompletion)>
        _queue_PerformNewFile = new();

    private void Enqueue_PerformNewFile(
        string fileName,
        IFileTemplate? exactMatchFileTemplate,
        List<IFileTemplate> relatedMatchFileTemplatesList,
        NamespacePath namespacePath,
        Func<Task> onAfterCompletion)
    {
        lock (_workLock)
        {
            _workKindQueue.Enqueue(MenuOptionsFactoryWorkKind.PerformNewFile);

            _queue_PerformNewFile.Enqueue((
                fileName,
                exactMatchFileTemplate,
                relatedMatchFileTemplatesList,
                namespacePath,
                onAfterCompletion));

            _backgroundTaskService.Continuous_EnqueueGroup(this);
        }
    }
    
    private async ValueTask Do_PerformNewFile(
        string fileName,
        IFileTemplate? exactMatchFileTemplate,
        List<IFileTemplate> relatedMatchFileTemplatesList,
        NamespacePath namespacePath,
        Func<Task> onAfterCompletion)
    {
        if (exactMatchFileTemplate is null)
        {
            var emptyFileAbsolutePathString = namespacePath.AbsolutePath.Value + fileName;

            var emptyFileAbsolutePath = _environmentProvider.AbsolutePathFactory(
                emptyFileAbsolutePathString,
                false);

            await _fileSystemProvider.File.WriteAllTextAsync(
                    emptyFileAbsolutePath.Value,
                    string.Empty,
                    CancellationToken.None)
                .ConfigureAwait(false);
        }
        else
        {
            var allTemplates = new[] { exactMatchFileTemplate }
                .Union(relatedMatchFileTemplatesList)
                .ToArray();

            foreach (var fileTemplate in allTemplates)
            {
                var templateResult = fileTemplate.ConstructFileContents.Invoke(
                    new FileTemplateParameter(fileName, namespacePath, _environmentProvider));

                await _fileSystemProvider.File.WriteAllTextAsync(
                        templateResult.FileNamespacePath.AbsolutePath.Value,
                        templateResult.Contents,
                        CancellationToken.None)
                    .ConfigureAwait(false);
            }
        }

        await onAfterCompletion.Invoke().ConfigureAwait(false);
    }

    private readonly
        Queue<(string directoryName, AbsolutePath parentDirectory, Func<Task> onAfterCompletion)>
        _queue_PerformNewDirectory = new();

    private void Enqueue_PerformNewDirectory(string directoryName, AbsolutePath parentDirectory, Func<Task> onAfterCompletion)
    {
        lock (_workLock)
        {
            _workKindQueue.Enqueue(MenuOptionsFactoryWorkKind.PerformNewDirectory);
            _queue_PerformNewDirectory.Enqueue((directoryName, parentDirectory, onAfterCompletion));
            _backgroundTaskService.Continuous_EnqueueGroup(this);
        }
    }
    
    private async ValueTask Do_PerformNewDirectory(string directoryName, AbsolutePath parentDirectory, Func<Task> onAfterCompletion)
    {
        var directoryAbsolutePathString = parentDirectory.Value + directoryName;
        var directoryAbsolutePath = _environmentProvider.AbsolutePathFactory(directoryAbsolutePathString, true);

        await _fileSystemProvider.Directory.CreateDirectoryAsync(
                directoryAbsolutePath.Value,
                CancellationToken.None)
            .ConfigureAwait(false);

        await onAfterCompletion.Invoke().ConfigureAwait(false);
    }

    private readonly
        Queue<(AbsolutePath absolutePath, Func<Task> onAfterCompletion)>
        _queue_general_AbsolutePath_FuncTask = new();

    private void Enqueue_PerformDeleteFile(AbsolutePath absolutePath, Func<Task> onAfterCompletion)
    {
        lock (_workLock)
        {
            _workKindQueue.Enqueue(MenuOptionsFactoryWorkKind.PerformDeleteFile);
            _queue_general_AbsolutePath_FuncTask.Enqueue((absolutePath, onAfterCompletion));
            _backgroundTaskService.Continuous_EnqueueGroup(this);
        }
    }
    
    private async ValueTask Do_PerformDeleteFile(AbsolutePath absolutePath, Func<Task> onAfterCompletion)
    {
        if (absolutePath.IsDirectory)
        {
            await _fileSystemProvider.Directory
                .DeleteAsync(absolutePath.Value, true, CancellationToken.None)
                .ConfigureAwait(false);
        }
        else
        {
            await _fileSystemProvider.File
                .DeleteAsync(absolutePath.Value)
                .ConfigureAwait(false);
        }

        await onAfterCompletion.Invoke().ConfigureAwait(false);
    }

    private void Enqueue_PerformCopyFile(AbsolutePath absolutePath, Func<Task> onAfterCompletion)
    {
        lock (_workLock)
        {
            _workKindQueue.Enqueue(MenuOptionsFactoryWorkKind.PerformCopyFile);
            _queue_general_AbsolutePath_FuncTask.Enqueue((absolutePath, onAfterCompletion));
            _backgroundTaskService.Continuous_EnqueueGroup(this);
        }
    }

    private async ValueTask Do_PerformCopyFile(AbsolutePath absolutePath, Func<Task> onAfterCompletion)
    {
        await _clipboardService.SetClipboard(ClipboardFacts.FormatPhrase(
                ClipboardFacts.CopyCommand,
                ClipboardFacts.AbsolutePathDataType,
                absolutePath.Value))
            .ConfigureAwait(false);

        await onAfterCompletion.Invoke().ConfigureAwait(false);
    }

    private void Enqueue_PerformCutFile(
        AbsolutePath absolutePath,
        Func<Task> onAfterCompletion)
    {
        lock (_workLock)
        {
            _workKindQueue.Enqueue(MenuOptionsFactoryWorkKind.PerformCutFile);
            _queue_general_AbsolutePath_FuncTask.Enqueue((absolutePath, onAfterCompletion));
            _backgroundTaskService.Continuous_EnqueueGroup(this);
        }
    }
    
    private async ValueTask Do_PerformCutFile(
        AbsolutePath absolutePath,
        Func<Task> onAfterCompletion)
    {
        await _clipboardService.SetClipboard(ClipboardFacts.FormatPhrase(
                ClipboardFacts.CutCommand,
                ClipboardFacts.AbsolutePathDataType,
                absolutePath.Value))
            .ConfigureAwait(false);

        await onAfterCompletion.Invoke().ConfigureAwait(false);
    }

    private void Enqueue_PerformPasteFile(AbsolutePath receivingDirectory, Func<Task> onAfterCompletion)
    {
        lock (_workLock)
        {
            _workKindQueue.Enqueue(MenuOptionsFactoryWorkKind.PerformPasteFile);
            _queue_general_AbsolutePath_FuncTask.Enqueue((receivingDirectory, onAfterCompletion));
            _backgroundTaskService.Continuous_EnqueueGroup(this);
        }
    }
    
    private async ValueTask Do_PerformPasteFile(AbsolutePath receivingDirectory, Func<Task> onAfterCompletion)
    {
        var clipboardContents = await _clipboardService.ReadClipboard().ConfigureAwait(false);

        if (ClipboardFacts.TryParseString(clipboardContents, out var clipboardPhrase))
        {
            if (clipboardPhrase is not null &&
                clipboardPhrase.DataType == ClipboardFacts.AbsolutePathDataType)
            {
                if (clipboardPhrase.Command == ClipboardFacts.CopyCommand ||
                    clipboardPhrase.Command == ClipboardFacts.CutCommand)
                {
                    AbsolutePath clipboardAbsolutePath = default;

                    // Should the if and else if be kept as inline awaits?
                    // If kept as inline awaits then the else if won't execute if the first one succeeds.
                    if (await _fileSystemProvider.Directory.ExistsAsync(clipboardPhrase.Value).ConfigureAwait(false))
                    {
                        clipboardAbsolutePath = _environmentProvider.AbsolutePathFactory(
                            clipboardPhrase.Value,
                            true);
                    }
                    else if (await _fileSystemProvider.File.ExistsAsync(clipboardPhrase.Value).ConfigureAwait(false))
                    {
                        clipboardAbsolutePath = _environmentProvider.AbsolutePathFactory(
                            clipboardPhrase.Value,
                            false);
                    }

                    if (clipboardAbsolutePath.ExactInput is not null)
                    {
                        var successfullyPasted = true;

                        try
                        {
                            if (clipboardAbsolutePath.IsDirectory)
                            {
                                var clipboardDirectoryInfo = new DirectoryInfo(clipboardAbsolutePath.Value);
                                var receivingDirectoryInfo = new DirectoryInfo(receivingDirectory.Value);

                                CopyFilesRecursively(clipboardDirectoryInfo, receivingDirectoryInfo);
                            }
                            else
                            {
                                var destinationAbsolutePathString = receivingDirectory.Value +
                                    clipboardAbsolutePath.NameWithExtension;

                                var sourceAbsolutePathString = clipboardAbsolutePath.Value;

                                await _fileSystemProvider.File.CopyAsync(
                                        sourceAbsolutePathString,
                                        destinationAbsolutePathString)
                                    .ConfigureAwait(false);
                            }
                        }
                        catch (Exception)
                        {
                            successfullyPasted = false;
                        }

                        if (successfullyPasted && clipboardPhrase.Command == ClipboardFacts.CutCommand)
                        {
                            // TODO: Rerender the parent of the deleted due to cut file
                            Enqueue_PerformDeleteFile(clipboardAbsolutePath, onAfterCompletion);
                        }
                        else
                        {
                            await onAfterCompletion.Invoke().ConfigureAwait(false);
                        }
                    }
                }
            }
        }
    }

    private AbsolutePath PerformRename(AbsolutePath sourceAbsolutePath, string nextName, INotificationService notificationService, Func<Task> onAfterCompletion)
    {
        // Check if the current and next name match when compared with case insensitivity
        if (0 == string.Compare(sourceAbsolutePath.NameWithExtension, nextName, StringComparison.OrdinalIgnoreCase))
        {
            var temporaryNextName = _environmentProvider.GetRandomFileName();

            var temporaryRenameResult = PerformRename(
                sourceAbsolutePath,
                temporaryNextName,
                notificationService,
                () => Task.CompletedTask);

            if (temporaryRenameResult.ExactInput is null)
            {
                onAfterCompletion.Invoke();
                return default;
            }
            else
            {
                sourceAbsolutePath = temporaryRenameResult;
            }
        }

        var sourceAbsolutePathString = sourceAbsolutePath.Value;
        var parentOfSource = sourceAbsolutePath.ParentDirectory;
        var destinationAbsolutePathString = parentOfSource + nextName;

        try
        {
            if (sourceAbsolutePath.IsDirectory)
                _fileSystemProvider.Directory.MoveAsync(sourceAbsolutePathString, destinationAbsolutePathString);
            else
                _fileSystemProvider.File.MoveAsync(sourceAbsolutePathString, destinationAbsolutePathString);
        }
        catch (Exception e)
        {
            NotificationHelper.DispatchError("Rename Action", e.Message, _commonComponentRenderers, notificationService, TimeSpan.FromSeconds(14));
            onAfterCompletion.Invoke();
            return default;
        }

        onAfterCompletion.Invoke();

        return _environmentProvider.AbsolutePathFactory(destinationAbsolutePathString, sourceAbsolutePath.IsDirectory);
    }

    /// <summary>
    /// Looking into copying and pasting a directory
    /// https://stackoverflow.com/questions/58744/copy-the-entire-contents-of-a-directory-in-c-sharp
    /// </summary>
    public static DirectoryInfo CopyFilesRecursively(DirectoryInfo source, DirectoryInfo target)
    {
        var newDirectoryInfo = target.CreateSubdirectory(source.Name);
        foreach (var fileInfo in source.GetFiles())
            fileInfo.CopyTo(Path.Combine(newDirectoryInfo.FullName, fileInfo.Name));

        foreach (var childDirectoryInfo in source.GetDirectories())
            CopyFilesRecursively(childDirectoryInfo, newDirectoryInfo);

        return newDirectoryInfo;
    }

    public ValueTask HandleEvent(CancellationToken cancellationToken)
    {
        MenuOptionsFactoryWorkKind workKind;

        lock (_workLock)
        {
            if (!_workKindQueue.TryDequeue(out workKind))
                return ValueTask.CompletedTask;
        }

        switch (workKind)
        {
            case MenuOptionsFactoryWorkKind.PerformNewFile:
            {
                var args = _queue_PerformNewFile.Dequeue();
                return Do_PerformNewFile(
                    args.fileName, args.exactMatchFileTemplate, args.relatedMatchFileTemplatesList, args.namespacePath, args.onAfterCompletion);
            }
            case MenuOptionsFactoryWorkKind.PerformNewDirectory:
            {
                var args = _queue_PerformNewDirectory.Dequeue();
                return Do_PerformNewDirectory(
                    args.directoryName, args.parentDirectory, args.onAfterCompletion);
            }
            case MenuOptionsFactoryWorkKind.PerformDeleteFile:
            {
                var args = _queue_general_AbsolutePath_FuncTask.Dequeue();
                return Do_PerformDeleteFile(
                    args.absolutePath, args.onAfterCompletion);
            }
            case MenuOptionsFactoryWorkKind.PerformCopyFile:
            {
                var args = _queue_general_AbsolutePath_FuncTask.Dequeue();
                return Do_PerformCopyFile(
                    args.absolutePath, args.onAfterCompletion);
            }
            case MenuOptionsFactoryWorkKind.PerformCutFile:
            {
                var args = _queue_general_AbsolutePath_FuncTask.Dequeue();
                return Do_PerformCutFile(
                    args.absolutePath, args.onAfterCompletion);
            }
            case MenuOptionsFactoryWorkKind.PerformPasteFile:
            {
                var args = _queue_general_AbsolutePath_FuncTask.Dequeue();
                return Do_PerformPasteFile(
                    args.absolutePath, args.onAfterCompletion);
            }
            default:
            {
                Console.WriteLine($"{nameof(MenuOptionsFactory)} {nameof(HandleEvent)} default case");
				return ValueTask.CompletedTask;
            }
        }
    }
}