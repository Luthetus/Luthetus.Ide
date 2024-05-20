using Fluxor;
using System.Collections.Immutable;
using Luthetus.Ide.RazorLib.FileSystems.Models;
using Luthetus.Ide.RazorLib.InputFiles.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Namespaces.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.Clipboards.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.Clipboards.Models;
using Luthetus.Ide.RazorLib.CommandLines.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.DotNetSolutions.Models;
using Luthetus.Ide.RazorLib.CSharpProjects.Models;
using Luthetus.Ide.RazorLib.TreeViewUtils.Models;

namespace Luthetus.Ide.RazorLib.Menus.Models;

public class MenuOptionsFactory : IMenuOptionsFactory
{
    private readonly ILuthetusIdeComponentRenderers _ideComponentRenderers;
    private readonly ILuthetusCommonComponentRenderers _commonComponentRenderers;
    private readonly IFileSystemProvider _fileSystemProvider;
    private readonly IEnvironmentProvider _environmentProvider;
    private readonly IClipboardService _clipboardService;
    private readonly IBackgroundTaskService _backgroundTaskService;

    public MenuOptionsFactory(
        ILuthetusIdeComponentRenderers ideComponentRenderers,
        ILuthetusCommonComponentRenderers commonComponentRenderers,
        IFileSystemProvider fileSystemProvider,
        IEnvironmentProvider environmentProvider,
        IClipboardService clipboardService,
        IBackgroundTaskService backgroundTaskService)
    {
        _ideComponentRenderers = ideComponentRenderers;
        _commonComponentRenderers = commonComponentRenderers;
        _fileSystemProvider = fileSystemProvider;
        _environmentProvider = environmentProvider;
        _clipboardService = clipboardService;
        _backgroundTaskService = backgroundTaskService;
    }

    public MenuOptionRecord NewEmptyFile(IAbsolutePath parentDirectory, Func<Task> onAfterCompletion)
    {
        return new MenuOptionRecord("New Empty File", MenuOptionKind.Create,
            WidgetRendererType: _ideComponentRenderers.FileFormRendererType,
            WidgetParameterMap: new Dictionary<string, object?>
            {
                { nameof(IFileFormRendererType.FileName), string.Empty },
                { nameof(IFileFormRendererType.CheckForTemplates), false },
                {
                    nameof(IFileFormRendererType.OnAfterSubmitFunc),
                    new Func<string, IFileTemplate?, ImmutableArray<IFileTemplate>, Task>(
                        async (fileName, exactMatchFileTemplate, relatedMatchFileTemplates) =>
                            await PerformNewFile(
                                fileName,
                                exactMatchFileTemplate,
                                relatedMatchFileTemplates,
                                new NamespacePath(string.Empty, parentDirectory),
                                onAfterCompletion))
                },
            });
    }

    public MenuOptionRecord NewTemplatedFile(NamespacePath parentDirectory, Func<Task> onAfterCompletion)
    {
        return new MenuOptionRecord("New Templated File", MenuOptionKind.Create,
            WidgetRendererType: _ideComponentRenderers.FileFormRendererType,
            WidgetParameterMap: new Dictionary<string, object?>
            {
                { nameof(IFileFormRendererType.FileName), string.Empty },
                { nameof(IFileFormRendererType.CheckForTemplates), true },
                {
                    nameof(IFileFormRendererType.OnAfterSubmitFunc),
                    new Func<string, IFileTemplate?, ImmutableArray<IFileTemplate>, Task>(
                        async (fileName, exactMatchFileTemplate, relatedMatchFileTemplates) =>
                            await PerformNewFile(
                                fileName,
                                exactMatchFileTemplate,
                                relatedMatchFileTemplates,
                                parentDirectory,
                                onAfterCompletion))
                },
            });
    }

    public MenuOptionRecord NewDirectory(IAbsolutePath parentDirectory, Func<Task> onAfterCompletion)
    {
        return new MenuOptionRecord("New Directory", MenuOptionKind.Create,
            WidgetRendererType: _ideComponentRenderers.FileFormRendererType,
            WidgetParameterMap: new Dictionary<string, object?>
            {
                { nameof(IFileFormRendererType.FileName), string.Empty },
                { nameof(IFileFormRendererType.IsDirectory), true },
                {
                    nameof(IFileFormRendererType.OnAfterSubmitFunc),
                    new Func<string, IFileTemplate?, ImmutableArray<IFileTemplate>, Task>(
                        async (directoryName, _, _) =>
                            await PerformNewDirectory(directoryName, parentDirectory, onAfterCompletion))
                },
            });
    }

    public MenuOptionRecord DeleteFile(IAbsolutePath absolutePath, Func<Task> onAfterCompletion)
    {
        return new MenuOptionRecord("Delete", MenuOptionKind.Delete,
            WidgetRendererType: _ideComponentRenderers.DeleteFileFormRendererType,
            WidgetParameterMap: new Dictionary<string, object?>
            {
                { nameof(IDeleteFileFormRendererType.AbsolutePath), absolutePath },
                { nameof(IDeleteFileFormRendererType.IsDirectory), true },
                {
                    nameof(IDeleteFileFormRendererType.OnAfterSubmitFunc),
                    new Func<IAbsolutePath, Task>(async x => await PerformDeleteFile(x, onAfterCompletion))
                },
            });
    }

    public MenuOptionRecord RenameFile(IAbsolutePath sourceAbsolutePath, IDispatcher dispatcher, Func<Task> onAfterCompletion)
    {
        return new MenuOptionRecord("Rename", MenuOptionKind.Update,
            WidgetRendererType: _ideComponentRenderers.FileFormRendererType,
            WidgetParameterMap: new Dictionary<string, object?>
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
                    new Func<string, IFileTemplate?, ImmutableArray<IFileTemplate>, Task>((nextName, _, _) =>
                    {
                        PerformRename(sourceAbsolutePath, nextName, dispatcher, onAfterCompletion);
                        return Task.CompletedTask;
                    })
                },
            });
    }

    public MenuOptionRecord CopyFile(IAbsolutePath absolutePath, Func<Task> onAfterCompletion)
    {
        return new MenuOptionRecord("Copy", MenuOptionKind.Update,
            OnClickFunc: () => PerformCopyFile(absolutePath, onAfterCompletion));
    }

    public MenuOptionRecord CutFile(IAbsolutePath absolutePath, Func<Task> onAfterCompletion)
    {
        return new MenuOptionRecord("Cut", MenuOptionKind.Update,
            OnClickFunc: () => PerformCutFile(absolutePath, onAfterCompletion));
    }

    public MenuOptionRecord PasteClipboard(IAbsolutePath directoryAbsolutePath, Func<Task> onAfterCompletion)
    {
        return new MenuOptionRecord("Paste", MenuOptionKind.Update,
            OnClickFunc: () => PerformPasteFile(directoryAbsolutePath, onAfterCompletion));
    }

    public MenuOptionRecord RemoveCSharpProjectReferenceFromSolution(
        TreeViewSolution treeViewSolution,
        TreeViewNamespacePath projectNode,
        Terminal terminal,
        IDispatcher dispatcher,
        Func<Task> onAfterCompletion)
    {
        return new MenuOptionRecord("Remove (no files are deleted)", MenuOptionKind.Delete,
            WidgetRendererType: _ideComponentRenderers.RemoveCSharpProjectFromSolutionRendererType,
            WidgetParameterMap: new Dictionary<string, object?>
            {
                {
                    nameof(IRemoveCSharpProjectFromSolutionRendererType.AbsolutePath),
                    projectNode.Item.AbsolutePath
                },
                {
                    nameof(IDeleteFileFormRendererType.OnAfterSubmitFunc),
                    new Func<IAbsolutePath, Task>(async _ => await PerformRemoveCSharpProjectReferenceFromSolution(
                        treeViewSolution,
                        projectNode,
                        terminal,
                        dispatcher,
                        onAfterCompletion))
                },
            });
    }

    public MenuOptionRecord AddProjectToProjectReference(
        TreeViewNamespacePath projectReceivingReference,
        Terminal terminal,
        IDispatcher dispatcher,
        LuthetusIdeBackgroundTaskApi ideBackgroundTaskApi,
        Func<Task> onAfterCompletion)
    {
        return new MenuOptionRecord("Add Project Reference", MenuOptionKind.Other,
            OnClickFunc: () => PerformAddProjectToProjectReference(
                projectReceivingReference,
                terminal,
                dispatcher,
                ideBackgroundTaskApi,
                onAfterCompletion));
    }

    public MenuOptionRecord RemoveProjectToProjectReference(
        TreeViewCSharpProjectToProjectReference treeViewCSharpProjectToProjectReference,
        Terminal terminal,
        IDispatcher dispatcher,
        Func<Task> onAfterCompletion)
    {
        return new MenuOptionRecord("Remove Project Reference", MenuOptionKind.Other,
            OnClickFunc: () => PerformRemoveProjectToProjectReference(
                treeViewCSharpProjectToProjectReference,
                terminal,
                dispatcher,
                onAfterCompletion));
    }

    public MenuOptionRecord MoveProjectToSolutionFolder(
        TreeViewSolution treeViewSolution,
        TreeViewNamespacePath treeViewProjectToMove,
        Terminal terminal,
        IDispatcher dispatcher,
        Func<Task> onAfterCompletion)
    {
        return new MenuOptionRecord("Move to Solution Folder", MenuOptionKind.Other,
            WidgetRendererType: _ideComponentRenderers.FileFormRendererType,
            WidgetParameterMap: new Dictionary<string, object?>
            {
                { nameof(IFileFormRendererType.FileName), string.Empty },
                { nameof(IFileFormRendererType.IsDirectory), false },
                {
                    nameof(IFileFormRendererType.OnAfterSubmitFunc),
                    new Func<string, IFileTemplate?, ImmutableArray<IFileTemplate>, Task>(async (nextName, _, _) =>
                        await PerformMoveProjectToSolutionFolder(
                            treeViewSolution,
                            treeViewProjectToMove,
                            nextName,
                            terminal,
                            dispatcher,
                            onAfterCompletion))
                },
            });
    }

    public MenuOptionRecord RemoveNuGetPackageReferenceFromProject(
        NamespacePath modifyProjectNamespacePath,
        TreeViewCSharpProjectNugetPackageReference treeViewCSharpProjectNugetPackageReference,
        Terminal terminal,
        IDispatcher dispatcher,
        Func<Task> onAfterCompletion)
    {
        return new MenuOptionRecord("Remove NuGet Package Reference", MenuOptionKind.Other,
            OnClickFunc: () => PerformRemoveNuGetPackageReferenceFromProject(
                modifyProjectNamespacePath,
                treeViewCSharpProjectNugetPackageReference,
                terminal,
                dispatcher,
                onAfterCompletion));
    }

    private Task PerformNewFile(
        string fileName,
        IFileTemplate? exactMatchFileTemplate,
        ImmutableArray<IFileTemplate> relatedMatchFileTemplatesList,
        NamespacePath namespacePath,
        Func<Task> onAfterCompletion)
    {
        return _backgroundTaskService.EnqueueAsync(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.GetQueueKey(),
            "New File Action",
            async () =>
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
            });
    }

    private Task PerformNewDirectory(string directoryName, IAbsolutePath parentDirectory, Func<Task> onAfterCompletion)
    {
        var directoryAbsolutePathString = parentDirectory.Value + directoryName;
        var directoryAbsolutePath = _environmentProvider.AbsolutePathFactory(directoryAbsolutePathString, true);

        return _backgroundTaskService.EnqueueAsync(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.GetQueueKey(),
            "New Directory Action",
            async () =>
            {
                await _fileSystemProvider.Directory.CreateDirectoryAsync(
                        directoryAbsolutePath.Value,
                        CancellationToken.None)
                    .ConfigureAwait(false);

                await onAfterCompletion.Invoke().ConfigureAwait(false);
            });
    }

    private Task PerformDeleteFile(IAbsolutePath absolutePath, Func<Task> onAfterCompletion)
    {
        return _backgroundTaskService.EnqueueAsync(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.GetQueueKey(),
            "Delete File Action",
            async () =>
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
            });
    }

    private Task PerformCopyFile(IAbsolutePath absolutePath, Func<Task> onAfterCompletion)
    {
        return _backgroundTaskService.EnqueueAsync(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.GetQueueKey(),
            "Copy File Action",
            async () =>
            {
                await _clipboardService.SetClipboard(ClipboardFacts.FormatPhrase(
                        ClipboardFacts.CopyCommand,
                        ClipboardFacts.AbsolutePathDataType,
                        absolutePath.Value))
                    .ConfigureAwait(false);

                await onAfterCompletion.Invoke().ConfigureAwait(false);
            });
    }

    private Task PerformCutFile(
        IAbsolutePath absolutePath,
        Func<Task> onAfterCompletion)
    {
        return _backgroundTaskService.EnqueueAsync(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.GetQueueKey(),
            "Cut File Action",
            async () =>
            {
                await _clipboardService.SetClipboard(ClipboardFacts.FormatPhrase(
                        ClipboardFacts.CutCommand,
                        ClipboardFacts.AbsolutePathDataType,
                        absolutePath.Value))
                    .ConfigureAwait(false);

                await onAfterCompletion.Invoke().ConfigureAwait(false);
            });
    }

    private Task PerformPasteFile(IAbsolutePath receivingDirectory, Func<Task> onAfterCompletion)
    {
        return _backgroundTaskService.EnqueueAsync(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.GetQueueKey(),
            "Paste File Action",
            async () =>
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
                            IAbsolutePath? clipboardAbsolutePath = null;

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

                            if (clipboardAbsolutePath is not null)
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
                                    await PerformDeleteFile(clipboardAbsolutePath, onAfterCompletion).ConfigureAwait(false);
                                }
                                else
                                {
                                    await onAfterCompletion.Invoke().ConfigureAwait(false);
                                }
                            }
                        }
                    }
                }
            });
    }

    private IAbsolutePath? PerformRename(IAbsolutePath sourceAbsolutePath, string nextName, IDispatcher dispatcher, Func<Task> onAfterCompletion)
    {
        // Check if the current and next name match when compared with case insensitivity
        if (0 == string.Compare(sourceAbsolutePath.NameWithExtension, nextName, StringComparison.OrdinalIgnoreCase))
        {
            var temporaryNextName = _environmentProvider.GetRandomFileName();

            var temporaryRenameResult = PerformRename(
                sourceAbsolutePath,
                temporaryNextName,
                dispatcher,
                () => Task.CompletedTask);

            if (temporaryRenameResult is null)
            {
                onAfterCompletion.Invoke();
                return null;
            }
            else
            {
                sourceAbsolutePath = temporaryRenameResult;
            }
        }

        var sourceAbsolutePathString = sourceAbsolutePath.Value;
        var parentOfSource = sourceAbsolutePath.AncestorDirectoryList.Last().Value;
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
            NotificationHelper.DispatchError("Rename Action", e.Message, _commonComponentRenderers, dispatcher, TimeSpan.FromSeconds(14));
            onAfterCompletion.Invoke();
            return null;
        }

        onAfterCompletion.Invoke();

        return _environmentProvider.AbsolutePathFactory(destinationAbsolutePathString, sourceAbsolutePath.IsDirectory);
    }

    private Task PerformRemoveCSharpProjectReferenceFromSolution(
        TreeViewSolution treeViewSolution,
        TreeViewNamespacePath projectNode,
        Terminal terminal,
        IDispatcher dispatcher,
        Func<Task> onAfterCompletion)
    {
        return _backgroundTaskService.EnqueueAsync(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.GetQueueKey(),
            "Remove C# Project Reference from Solution Action",
            async () =>
            {
                var workingDirectory = treeViewSolution.Item.NamespacePath.AbsolutePath.ParentDirectory!;

                var formattedCommand = DotNetCliCommandFormatter.FormatRemoveCSharpProjectReferenceFromSolutionAction(
                    treeViewSolution.Item.NamespacePath.AbsolutePath.Value,
                    projectNode.Item.AbsolutePath.Value);

                var terminalCommand = new TerminalCommand(
                    Key<TerminalCommand>.NewKey(),
                    formattedCommand,
                    workingDirectory.Value,
                    CancellationToken.None,
                    async () => await onAfterCompletion.Invoke().ConfigureAwait(false));

                await terminal.EnqueueCommandAsync(terminalCommand).ConfigureAwait(false);
            });
    }

    public Task PerformAddProjectToProjectReference(
        TreeViewNamespacePath projectReceivingReference,
        Terminal terminal,
        IDispatcher dispatcher,
        LuthetusIdeBackgroundTaskApi ideBackgroundTaskApi,
        Func<Task> onAfterCompletion)
    {
        return ideBackgroundTaskApi.InputFile.RequestInputFileStateForm($"Add Project reference to {projectReceivingReference.Item.AbsolutePath.NameWithExtension}",
            async referencedProject =>
            {
                if (referencedProject is null)
                    return;

                var formattedCommand = DotNetCliCommandFormatter.FormatAddProjectToProjectReference(
                    projectReceivingReference.Item.AbsolutePath.Value,
                    referencedProject.Value);

                var terminalCommand = new TerminalCommand(
                    Key<TerminalCommand>.NewKey(),
                    formattedCommand,
                    null,
                    CancellationToken.None,
                    async () =>
                    {
                        NotificationHelper.DispatchInformative("Add Project Reference", $"Modified {projectReceivingReference.Item.AbsolutePath.NameWithExtension} to have a reference to {referencedProject.NameWithExtension}", _commonComponentRenderers, dispatcher, TimeSpan.FromSeconds(7));
                        await onAfterCompletion.Invoke().ConfigureAwait(false);
                    });

                await terminal.EnqueueCommandAsync(terminalCommand).ConfigureAwait(false);
            },
            absolutePath =>
            {
                if (absolutePath is null || absolutePath.IsDirectory)
                    return Task.FromResult(false);

                return Task.FromResult(
                    absolutePath.ExtensionNoPeriod.EndsWith(ExtensionNoPeriodFacts.C_SHARP_PROJECT));
            },
            (new[]
            {
                new InputFilePattern(
                    "C# Project",
                    absolutePath => absolutePath.ExtensionNoPeriod.EndsWith(ExtensionNoPeriodFacts.C_SHARP_PROJECT))
            }).ToImmutableArray());
    }

    public Task PerformRemoveProjectToProjectReference(
        TreeViewCSharpProjectToProjectReference treeViewCSharpProjectToProjectReference,
        Terminal terminal,
        IDispatcher dispatcher,
        Func<Task> onAfterCompletion)
    {
        return _backgroundTaskService.EnqueueAsync(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.GetQueueKey(),
            "Remove Project Reference to Project",
            async () =>
            {
                var formattedCommand = DotNetCliCommandFormatter.FormatRemoveProjectToProjectReference(
                    treeViewCSharpProjectToProjectReference.Item.ModifyProjectNamespacePath.AbsolutePath.Value,
                    treeViewCSharpProjectToProjectReference.Item.ReferenceProjectAbsolutePath.Value);

                var removeProjectToProjectReferenceTerminalCommand = new TerminalCommand(
                    Key<TerminalCommand>.NewKey(),
                    formattedCommand,
                    null,
                    CancellationToken.None,
                    async () =>
                    {
                        NotificationHelper.DispatchInformative("Remove Project Reference", $"Modified {treeViewCSharpProjectToProjectReference.Item.ModifyProjectNamespacePath.AbsolutePath.NameWithExtension} to have a reference to {treeViewCSharpProjectToProjectReference.Item.ReferenceProjectAbsolutePath.NameWithExtension}", _commonComponentRenderers, dispatcher, TimeSpan.FromSeconds(7));
                        await onAfterCompletion.Invoke().ConfigureAwait(false);
                    });

                await terminal.EnqueueCommandAsync(removeProjectToProjectReferenceTerminalCommand).ConfigureAwait(false);
            });
    }

    public Task PerformMoveProjectToSolutionFolder(
        TreeViewSolution treeViewSolution,
        TreeViewNamespacePath treeViewProjectToMove,
        string solutionFolderPath,
        Terminal terminal,
        IDispatcher dispatcher,
        Func<Task> onAfterCompletion)
    {
        return _backgroundTaskService.EnqueueAsync(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.GetQueueKey(),
            "Move Project to Solution Folder",
            () =>
            {
                var formattedCommand = DotNetCliCommandFormatter.FormatMoveProjectToSolutionFolder(
                    treeViewSolution.Item.NamespacePath.AbsolutePath.Value,
                    treeViewProjectToMove.Item.AbsolutePath.Value,
                    solutionFolderPath);

                var moveProjectToSolutionFolderTerminalCommand = new TerminalCommand(
                    Key<TerminalCommand>.NewKey(),
                    formattedCommand,
                    null,
                    CancellationToken.None,
                    async () =>
                    {
                        NotificationHelper.DispatchInformative("Move Project To Solution Folder", $"Moved {treeViewProjectToMove.Item.AbsolutePath.NameWithExtension} to the Solution Folder path: {solutionFolderPath}", _commonComponentRenderers, dispatcher, TimeSpan.FromSeconds(7));
                        await onAfterCompletion.Invoke().ConfigureAwait(false);
                    });

                PerformRemoveCSharpProjectReferenceFromSolution(
                    treeViewSolution,
                    treeViewProjectToMove,
                    terminal,
                    dispatcher,
                    async () => await terminal.EnqueueCommandAsync(moveProjectToSolutionFolderTerminalCommand).ConfigureAwait(false));

                return Task.CompletedTask;
            });
    }

    public Task PerformRemoveNuGetPackageReferenceFromProject(
        NamespacePath modifyProjectNamespacePath,
        TreeViewCSharpProjectNugetPackageReference treeViewCSharpProjectNugetPackageReference,
        Terminal terminal,
        IDispatcher dispatcher,
        Func<Task> onAfterCompletion)
    {
        return _backgroundTaskService.EnqueueAsync(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.GetQueueKey(),
            "Remove NuGet Package Reference from Project",
            async () =>
            {
                var formattedCommand = DotNetCliCommandFormatter.FormatRemoveNugetPackageReferenceFromProject(
                    modifyProjectNamespacePath.AbsolutePath.Value,
                    treeViewCSharpProjectNugetPackageReference.Item.LightWeightNugetPackageRecord.Id);

                var removeNugetPackageReferenceFromProjectTerminalCommand = new TerminalCommand(
                    Key<TerminalCommand>.NewKey(),
                    formattedCommand,
                    null,
                    CancellationToken.None,
                    async () =>
                    {
                        NotificationHelper.DispatchInformative("Remove Project Reference", $"Modified {modifyProjectNamespacePath.AbsolutePath.NameWithExtension} to NOT have a reference to {treeViewCSharpProjectNugetPackageReference.Item.LightWeightNugetPackageRecord.Id}", _commonComponentRenderers, dispatcher, TimeSpan.FromSeconds(7));
                        await onAfterCompletion.Invoke().ConfigureAwait(false);
                    });

                await terminal.EnqueueCommandAsync(removeNugetPackageReferenceFromProjectTerminalCommand).ConfigureAwait(false);
            });
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
}