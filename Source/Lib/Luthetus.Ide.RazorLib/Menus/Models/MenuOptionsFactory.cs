using Fluxor;
using System.Collections.Immutable;
using Luthetus.Ide.RazorLib.FileSystems.Models;
using Luthetus.Ide.RazorLib.InputFiles.Models;
using Luthetus.Ide.RazorLib.InputFiles.States;
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
using Luthetus.Ide.RazorLib.FileSystems.Models;
using Luthetus.Ide.RazorLib.Clipboards.Models;
using Luthetus.Ide.RazorLib.TreeViewImplementations.Models;
using Luthetus.Ide.RazorLib.CommandLines.Models;

namespace Luthetus.Ide.RazorLib.Menus.Models;

public class MenuOptionsFactory : IMenuOptionsFactory
{
    private readonly ILuthetusIdeComponentRenderers _luthetusIdeComponentRenderers;
    private readonly ILuthetusCommonComponentRenderers _luthetusCommonComponentRenderers;
    private readonly IFileSystemProvider _fileSystemProvider;
    private readonly IEnvironmentProvider _environmentProvider;
    private readonly IClipboardService _clipboardService;
    private readonly IBackgroundTaskService _backgroundTaskService;

    public MenuOptionsFactory(
        ILuthetusIdeComponentRenderers luthetusIdeComponentRenderers,
        ILuthetusCommonComponentRenderers luthetusCommonComponentRenderers,
        IFileSystemProvider fileSystemProvider,
        IEnvironmentProvider environmentProvider,
        IClipboardService clipboardService,
        IBackgroundTaskService backgroundTaskService)
    {
        _luthetusIdeComponentRenderers = luthetusIdeComponentRenderers;
        _luthetusCommonComponentRenderers = luthetusCommonComponentRenderers;
        _fileSystemProvider = fileSystemProvider;
        _environmentProvider = environmentProvider;
        _clipboardService = clipboardService;
        _backgroundTaskService = backgroundTaskService;
    }

    public MenuOptionRecord NewEmptyFile(IAbsolutePath parentDirectory, Func<Task> onAfterCompletion)
    {
        return new MenuOptionRecord("New Empty File", MenuOptionKind.Create,
            WidgetRendererType: _luthetusIdeComponentRenderers.FileFormRendererType,
            WidgetParameterMap: new Dictionary<string, object?>
            {
                { nameof(IFileFormRendererType.FileName), string.Empty },
                { nameof(IFileFormRendererType.CheckForTemplates), false },
                {
                    nameof(IFileFormRendererType.OnAfterSubmitAction),
                    new Action<string, IFileTemplate?, ImmutableArray<IFileTemplate>>(
                        (fileName, exactMatchFileTemplate, relatedMatchFileTemplates) =>
                            PerformNewFileAction(
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
            WidgetRendererType: _luthetusIdeComponentRenderers.FileFormRendererType,
            WidgetParameterMap: new Dictionary<string, object?>
            {
                { nameof(IFileFormRendererType.FileName), string.Empty },
                { nameof(IFileFormRendererType.CheckForTemplates), true },
                {
                    nameof(IFileFormRendererType.OnAfterSubmitAction),
                    new Action<string, IFileTemplate?, ImmutableArray<IFileTemplate>>(
                        (fileName, exactMatchFileTemplate, relatedMatchFileTemplates) =>
                            PerformNewFileAction(
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
            WidgetRendererType: _luthetusIdeComponentRenderers.FileFormRendererType,
            WidgetParameterMap: new Dictionary<string, object?>
            {
                { nameof(IFileFormRendererType.FileName), string.Empty },
                { nameof(IFileFormRendererType.IsDirectory), true },
                {
                    nameof(IFileFormRendererType.OnAfterSubmitAction),
                    new Action<string, IFileTemplate?, ImmutableArray<IFileTemplate>>(
                        (directoryName, _, _) =>
                            PerformNewDirectoryAction(directoryName, parentDirectory, onAfterCompletion))
                },
            });
    }

    public MenuOptionRecord DeleteFile(IAbsolutePath absolutePath, Func<Task> onAfterCompletion)
    {
        return new MenuOptionRecord("Delete", MenuOptionKind.Delete,
            WidgetRendererType: _luthetusIdeComponentRenderers.DeleteFileFormRendererType,
            WidgetParameterMap: new Dictionary<string, object?>
            {
                { nameof(IDeleteFileFormRendererType.AbsolutePath), absolutePath },
                { nameof(IDeleteFileFormRendererType.IsDirectory), true },
                {
                    nameof(IDeleteFileFormRendererType.OnAfterSubmitAction),
                    new Action<IAbsolutePath>(afp => PerformDeleteFileAction(afp, onAfterCompletion))
                },
            });
    }

    public MenuOptionRecord RenameFile(
        IAbsolutePath sourceAbsolutePath,
        IDispatcher dispatcher,
        Func<Task> onAfterCompletion)
    {
        return new MenuOptionRecord("Rename", MenuOptionKind.Update,
            WidgetRendererType: _luthetusIdeComponentRenderers.FileFormRendererType,
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
                    nameof(IFileFormRendererType.OnAfterSubmitAction),
                    new Action<string, IFileTemplate?, ImmutableArray<IFileTemplate>>((nextName, _, _) =>
                        PerformRenameAction(sourceAbsolutePath, nextName, dispatcher, onAfterCompletion))
                },
            });
    }

    public MenuOptionRecord CopyFile(IAbsolutePath absolutePath, Func<Task> onAfterCompletion)
    {
        return new MenuOptionRecord("Copy", MenuOptionKind.Update,
            OnClick: () => PerformCopyFileAction(absolutePath, onAfterCompletion));
    }

    public MenuOptionRecord CutFile(IAbsolutePath absolutePath, Func<Task> onAfterCompletion)
    {
        return new MenuOptionRecord("Cut", MenuOptionKind.Update,
            OnClick: () => PerformCutFileAction(absolutePath, onAfterCompletion));
    }

    public MenuOptionRecord PasteClipboard(
        IAbsolutePath directoryAbsolutePath,
        Func<Task> onAfterCompletion)
    {
        return new MenuOptionRecord("Paste", MenuOptionKind.Update,
            OnClick: () => PerformPasteFileAction(directoryAbsolutePath, onAfterCompletion));
    }

    public MenuOptionRecord RemoveCSharpProjectReferenceFromSolution(
        TreeViewSolution treeViewSolution,
        TreeViewNamespacePath projectNode,
        TerminalSession terminalSession,
        IDispatcher dispatcher,
        Func<Task> onAfterCompletion)
    {
        return new MenuOptionRecord("Remove (no files are deleted)", MenuOptionKind.Delete,
            WidgetRendererType: _luthetusIdeComponentRenderers.RemoveCSharpProjectFromSolutionRendererType,
            WidgetParameterMap: new Dictionary<string, object?>
            {
                {
                    nameof(IRemoveCSharpProjectFromSolutionRendererType.AbsolutePath),
                    projectNode.Item.AbsolutePath
                },
                {
                    nameof(IDeleteFileFormRendererType.OnAfterSubmitAction),
                    new Action<IAbsolutePath>(_ => PerformRemoveCSharpProjectReferenceFromSolutionAction(
                        treeViewSolution,
                        projectNode,
                        terminalSession,
                        dispatcher,
                        onAfterCompletion))
                },
            });
    }

    public MenuOptionRecord AddProjectToProjectReference(
        TreeViewNamespacePath projectReceivingReference,
        TerminalSession terminalSession,
        IDispatcher dispatcher,
        InputFileSync inputFileSync,
        Func<Task> onAfterCompletion)
    {
        return new MenuOptionRecord("Add Project Reference", MenuOptionKind.Other,
            OnClick: () => PerformAddProjectToProjectReferenceAction(
                projectReceivingReference,
                terminalSession,
                dispatcher,
                inputFileSync,
                onAfterCompletion));
    }

    public MenuOptionRecord RemoveProjectToProjectReference(
        TreeViewCSharpProjectToProjectReference treeViewCSharpProjectToProjectReference,
        TerminalSession terminalSession,
        IDispatcher dispatcher,
        Func<Task> onAfterCompletion)
    {
        return new MenuOptionRecord("Remove Project Reference", MenuOptionKind.Other,
            OnClick: () => PerformRemoveProjectToProjectReferenceAction(
                treeViewCSharpProjectToProjectReference,
                terminalSession,
                dispatcher,
                onAfterCompletion));
    }

    public MenuOptionRecord MoveProjectToSolutionFolder(
        TreeViewSolution treeViewSolution,
        TreeViewNamespacePath treeViewProjectToMove,
        TerminalSession terminalSession,
        IDispatcher dispatcher,
        Func<Task> onAfterCompletion)
    {
        return new MenuOptionRecord("Move to Solution Folder", MenuOptionKind.Other,
            WidgetRendererType: _luthetusIdeComponentRenderers.FileFormRendererType,
            WidgetParameterMap: new Dictionary<string, object?>
            {
                { nameof(IFileFormRendererType.FileName), string.Empty },
                { nameof(IFileFormRendererType.IsDirectory), false },
                {
                    nameof(IFileFormRendererType.OnAfterSubmitAction),
                    new Action<string, IFileTemplate?, ImmutableArray<IFileTemplate>>((nextName, _, _) =>
                        PerformMoveProjectToSolutionFolderAction(
                            treeViewSolution,
                            treeViewProjectToMove,
                            nextName,
                            terminalSession,
                            dispatcher,
                            onAfterCompletion))
                },
            });
    }

    public MenuOptionRecord RemoveNuGetPackageReferenceFromProject(
        NamespacePath modifyProjectNamespacePath,
        TreeViewCSharpProjectNugetPackageReference treeViewCSharpProjectNugetPackageReference,
        TerminalSession terminalSession,
        IDispatcher dispatcher,
        Func<Task> onAfterCompletion)
    {
        return new MenuOptionRecord("Remove NuGet Package Reference", MenuOptionKind.Other,
            OnClick: () => PerformRemoveNuGetPackageReferenceFromProjectAction(
                modifyProjectNamespacePath,
                treeViewCSharpProjectNugetPackageReference,
                terminalSession,
                dispatcher,
                onAfterCompletion));
    }

    private void PerformNewFileAction(
        string fileName,
        IFileTemplate? exactMatchFileTemplate,
        ImmutableArray<IFileTemplate> relatedMatchFileTemplates,
        NamespacePath namespacePath,
        Func<Task> onAfterCompletion)
    {
        _backgroundTaskService.Enqueue(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.Queue.Key,
            "New File Action",
            async () =>
            {
                if (exactMatchFileTemplate is null)
                {
                    var emptyFileAbsolutePathString = namespacePath.AbsolutePath.FormattedInput + fileName;

                    var emptyFileAbsolutePath = new AbsolutePath(
                        emptyFileAbsolutePathString,
                        false,
                        _environmentProvider);

                    await _fileSystemProvider.File.WriteAllTextAsync(
                        emptyFileAbsolutePath.FormattedInput,
                        string.Empty,
                        CancellationToken.None);
                }
                else
                {
                    var allTemplates = new[] { exactMatchFileTemplate }
                        .Union(relatedMatchFileTemplates)
                        .ToArray();

                    foreach (var fileTemplate in allTemplates)
                    {
                        var templateResult = fileTemplate.ConstructFileContents.Invoke(
                            new FileTemplateParameter(
                                fileName,
                                namespacePath,
                                _environmentProvider));

                        await _fileSystemProvider.File.WriteAllTextAsync(
                            templateResult.FileNamespacePath.AbsolutePath.FormattedInput,
                            templateResult.Contents,
                            CancellationToken.None);
                    }
                }

                await onAfterCompletion.Invoke();
            });
    }

    private void PerformNewDirectoryAction(
        string directoryName,
        IAbsolutePath parentDirectory,
        Func<Task> onAfterCompletion)
    {
        var directoryAbsolutePathString = parentDirectory.FormattedInput + directoryName;

        var directoryAbsolutePath = new AbsolutePath(
            directoryAbsolutePathString,
            true,
            _environmentProvider);

        _backgroundTaskService.Enqueue(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.Queue.Key,
            "New Directory Action",
            async () =>
            {
                await _fileSystemProvider.Directory.CreateDirectoryAsync(
                    directoryAbsolutePath.FormattedInput,
                    CancellationToken.None);

                await onAfterCompletion.Invoke();
            });
    }

    private void PerformDeleteFileAction(
        IAbsolutePath absolutePath,
        Func<Task> onAfterCompletion)
    {
        _backgroundTaskService.Enqueue(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.Queue.Key,
            "Delete File Action",
            async () =>
            {
                if (absolutePath.IsDirectory)
                {
                    await _fileSystemProvider.Directory.DeleteAsync(
                        absolutePath.FormattedInput,
                        true,
                        CancellationToken.None);
                }
                else
                {
                    await _fileSystemProvider.File.DeleteAsync(absolutePath.FormattedInput);
                }

                await onAfterCompletion.Invoke();
            });
    }

    private void PerformCopyFileAction(
        IAbsolutePath absolutePath,
        Func<Task> onAfterCompletion)
    {
        _backgroundTaskService.Enqueue(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.Queue.Key,
            "Copy File Action",
            async () =>
            {
                await _clipboardService.SetClipboard(ClipboardFacts.FormatPhrase(
                    ClipboardFacts.CopyCommand,
                    ClipboardFacts.AbsolutePathDataType,
                    absolutePath.FormattedInput));

                await onAfterCompletion.Invoke();
            });
    }

    private void PerformCutFileAction(
        IAbsolutePath absolutePath,
        Func<Task> onAfterCompletion)
    {
        _backgroundTaskService.Enqueue(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.Queue.Key,
            "Cut File Action",
            async () =>
            {
                await _clipboardService.SetClipboard(ClipboardFacts.FormatPhrase(
                    ClipboardFacts.CutCommand,
                    ClipboardFacts.AbsolutePathDataType,
                    absolutePath.FormattedInput));

                await onAfterCompletion.Invoke();
            });
    }

    private void PerformPasteFileAction(IAbsolutePath receivingDirectory, Func<Task> onAfterCompletion)
    {
        _backgroundTaskService.Enqueue(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.Queue.Key,
            "Paste File Action",
            async () =>
            {
                var clipboardContents = await _clipboardService.ReadClipboard();

                if (ClipboardFacts.TryParseString(clipboardContents, out var clipboardPhrase))
                {
                    if (clipboardPhrase is not null &&
                        clipboardPhrase.DataType == ClipboardFacts.AbsolutePathDataType)
                    {
                        if (clipboardPhrase.Command == ClipboardFacts.CopyCommand ||
                            clipboardPhrase.Command == ClipboardFacts.CutCommand)
                        {
                            IAbsolutePath? clipboardAbsolutePath = null;

                            if (await _fileSystemProvider.Directory.ExistsAsync(clipboardPhrase.Value))
                            {
                                clipboardAbsolutePath = new AbsolutePath(
                                    clipboardPhrase.Value,
                                    true,
                                    _environmentProvider);
                            }
                            else if (await _fileSystemProvider.File.ExistsAsync(clipboardPhrase.Value))
                            {
                                clipboardAbsolutePath = new AbsolutePath(
                                    clipboardPhrase.Value,
                                    false,
                                    _environmentProvider);
                            }

                            if (clipboardAbsolutePath is not null)
                            {
                                var successfullyPasted = true;

                                try
                                {
                                    if (clipboardAbsolutePath.IsDirectory)
                                    {
                                        var clipboardDirectoryInfo = new DirectoryInfo(
                                            clipboardAbsolutePath.FormattedInput);

                                        var receivingDirectoryInfo = new DirectoryInfo(
                                            receivingDirectory.FormattedInput);

                                        CopyFilesRecursively(clipboardDirectoryInfo, receivingDirectoryInfo);
                                    }
                                    else
                                    {
                                        var destinationAbsolutePathString =
                                            receivingDirectory.FormattedInput +
                                            clipboardAbsolutePath.NameWithExtension;

                                        var sourceAbsolutePathString = clipboardAbsolutePath
                                            .FormattedInput;

                                        await _fileSystemProvider.File.CopyAsync(
                                            sourceAbsolutePathString,
                                            destinationAbsolutePathString);
                                    }
                                }
                                catch (Exception)
                                {
                                    successfullyPasted = false;
                                }

                                if (successfullyPasted && clipboardPhrase.Command == ClipboardFacts.CutCommand)
                                {
                                    // TODO: Rerender the parent of the deleted due to cut file
                                    PerformDeleteFileAction(clipboardAbsolutePath, onAfterCompletion);
                                }
                                else
                                {
                                    await onAfterCompletion.Invoke();
                                }
                            }
                        }
                    }
                }
            });
    }

    private IAbsolutePath? PerformRenameAction(
        IAbsolutePath sourceAbsolutePath,
        string nextName,
        IDispatcher dispatcher,
        Func<Task> onAfterCompletion)
    {
        // If the current and next name match when compared
        // with case insensitivity
        if (string.Compare(sourceAbsolutePath.NameWithExtension, nextName, StringComparison.OrdinalIgnoreCase)
            == 0)
        {
            var temporaryNextName = _environmentProvider.GetRandomFileName();

            var temporaryRenameResult = PerformRenameAction(
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

        var sourceAbsolutePathString = sourceAbsolutePath.FormattedInput;
        var parentOfSource = sourceAbsolutePath.AncestorDirectoryBag.Last();
        var destinationAbsolutePathString = parentOfSource.FormattedInput + nextName;

        try
        {
            if (sourceAbsolutePath.IsDirectory)
                _fileSystemProvider.Directory.MoveAsync(sourceAbsolutePathString, destinationAbsolutePathString);
            else
                _fileSystemProvider.File.MoveAsync(sourceAbsolutePathString, destinationAbsolutePathString);
        }
        catch (Exception e)
        {
            NotificationHelper.DispatchError("Rename Action", e.Message, _luthetusCommonComponentRenderers, dispatcher);
            onAfterCompletion.Invoke();
            return null;
        }

        onAfterCompletion.Invoke();

        return new AbsolutePath(
            destinationAbsolutePathString,
            sourceAbsolutePath.IsDirectory,
            _environmentProvider);
    }

    private void PerformRemoveCSharpProjectReferenceFromSolutionAction(
        TreeViewSolution treeViewSolution,
        TreeViewNamespacePath projectNode,
        TerminalSession terminalSession,
        IDispatcher dispatcher,
        Func<Task> onAfterCompletion)
    {
        _backgroundTaskService.Enqueue(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.Queue.Key,
            "Remove C# Project Reference from Solution Action",
            async () =>
            {
                var workingDirectory = treeViewSolution.Item.NamespacePath.AbsolutePath.ParentDirectory!;

                var formattedCommand = DotNetCliCommandFormatter.FormatRemoveCSharpProjectReferenceFromSolutionAction(
                    treeViewSolution.Item.NamespacePath.AbsolutePath.FormattedInput,
                    projectNode.Item.AbsolutePath.FormattedInput);

                var terminalCommand = new TerminalCommand(
                    Key<TerminalCommand>.NewKey(),
                    formattedCommand,
                    workingDirectory.FormattedInput,
                    CancellationToken.None,
                    async () => await onAfterCompletion.Invoke());

                await terminalSession.EnqueueCommandAsync(terminalCommand);
            });
    }

    public void PerformAddProjectToProjectReferenceAction(
        TreeViewNamespacePath projectReceivingReference,
        TerminalSession terminalSession,
        IDispatcher dispatcher,
        InputFileSync inputFileSync,
        Func<Task> onAfterCompletion)
    {
        inputFileSync.RequestInputFileStateForm($"Add Project reference to {projectReceivingReference.Item.AbsolutePath.NameWithExtension}",
            async referencedProject =>
            {
                if (referencedProject is null)
                    return;

                var formattedCommand = DotNetCliCommandFormatter.FormatAddProjectToProjectReference(
                    projectReceivingReference.Item.AbsolutePath.FormattedInput,
                    referencedProject.FormattedInput);

                var terminalCommand = new TerminalCommand(
                    Key<TerminalCommand>.NewKey(),
                    formattedCommand,
                    null,
                    CancellationToken.None,
                    async () =>
                    {
                        NotificationHelper.DispatchInformative("Add Project Reference", $"Modified {projectReceivingReference.Item.AbsolutePath.NameWithExtension} to have a reference to {referencedProject.NameWithExtension}", _luthetusCommonComponentRenderers, dispatcher);
                        await onAfterCompletion.Invoke();
                    });

                await terminalSession.EnqueueCommandAsync(terminalCommand);
            },
            afp =>
            {
                if (afp is null || afp.IsDirectory)
                    return Task.FromResult(false);

                return Task.FromResult(
                    afp.ExtensionNoPeriod.EndsWith(ExtensionNoPeriodFacts.C_SHARP_PROJECT));
            },
            (new[]
            {
                new InputFilePattern(
                    "C# Project",
                    afp => afp.ExtensionNoPeriod.EndsWith(ExtensionNoPeriodFacts.C_SHARP_PROJECT))
            }).ToImmutableArray());
    }

    public void PerformRemoveProjectToProjectReferenceAction(
        TreeViewCSharpProjectToProjectReference treeViewCSharpProjectToProjectReference,
        TerminalSession terminalSession,
        IDispatcher dispatcher,
        Func<Task> onAfterCompletion)
    {
        _backgroundTaskService.Enqueue(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.Queue.Key,
            "Remove Project Reference to Project",
            async () =>
            {
                var formattedCommand = DotNetCliCommandFormatter.FormatRemoveProjectToProjectReference(
                    treeViewCSharpProjectToProjectReference.Item.ModifyProjectNamespacePath.AbsolutePath.FormattedInput,
                    treeViewCSharpProjectToProjectReference.Item.ReferenceProjectAbsolutePath.FormattedInput);

                var removeProjectToProjectReferenceTerminalCommand = new TerminalCommand(
                    Key<TerminalCommand>.NewKey(),
                    formattedCommand,
                    null,
                    CancellationToken.None,
                    async () =>
                    {
                        NotificationHelper.DispatchInformative("Remove Project Reference", $"Modified {treeViewCSharpProjectToProjectReference.Item.ModifyProjectNamespacePath.AbsolutePath.NameWithExtension} to have a reference to {treeViewCSharpProjectToProjectReference.Item.ReferenceProjectAbsolutePath.NameWithExtension}", _luthetusCommonComponentRenderers, dispatcher);
                        await onAfterCompletion.Invoke();
                    });

                await terminalSession.EnqueueCommandAsync(removeProjectToProjectReferenceTerminalCommand);
            });
    }

    public void PerformMoveProjectToSolutionFolderAction(
        TreeViewSolution treeViewSolution,
        TreeViewNamespacePath treeViewProjectToMove,
        string solutionFolderPath,
        TerminalSession terminalSession,
        IDispatcher dispatcher,
        Func<Task> onAfterCompletion)
    {
        _backgroundTaskService.Enqueue(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.Queue.Key,
            "Move Project to Solution Folder",
            () =>
            {
                var formattedCommand = DotNetCliCommandFormatter.FormatMoveProjectToSolutionFolder(
                    treeViewSolution.Item.NamespacePath.AbsolutePath.FormattedInput,
                    treeViewProjectToMove.Item.AbsolutePath.FormattedInput,
                    solutionFolderPath);

                var moveProjectToSolutionFolderTerminalCommand = new TerminalCommand(
                    Key<TerminalCommand>.NewKey(),
                    formattedCommand,
                    null,
                    CancellationToken.None,
                    async () =>
                    {
                        NotificationHelper.DispatchInformative("Move Project To Solution Folder", $"Moved {treeViewProjectToMove.Item.AbsolutePath.NameWithExtension} to the Solution Folder path: {solutionFolderPath}", _luthetusCommonComponentRenderers, dispatcher);
                        await onAfterCompletion.Invoke();
                    });

                PerformRemoveCSharpProjectReferenceFromSolutionAction(
                    treeViewSolution,
                    treeViewProjectToMove,
                    terminalSession,
                    dispatcher,
                    async () => await terminalSession.EnqueueCommandAsync(moveProjectToSolutionFolderTerminalCommand));

                return Task.CompletedTask;
            });
    }

    public void PerformRemoveNuGetPackageReferenceFromProjectAction(
        NamespacePath modifyProjectNamespacePath,
        TreeViewCSharpProjectNugetPackageReference treeViewCSharpProjectNugetPackageReference,
        TerminalSession terminalSession,
        IDispatcher dispatcher,
        Func<Task> onAfterCompletion)
    {
        _backgroundTaskService.Enqueue(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.Queue.Key,
            "Remove NuGet Package Reference from Project",
            async () =>
            {
                var formattedCommand = DotNetCliCommandFormatter.FormatRemoveNugetPackageReferenceFromProject(
                    modifyProjectNamespacePath.AbsolutePath.FormattedInput,
                    treeViewCSharpProjectNugetPackageReference.Item.LightWeightNugetPackageRecord.Id);

                var removeNugetPackageReferenceFromProjectTerminalCommand = new TerminalCommand(
                    Key<TerminalCommand>.NewKey(),
                    formattedCommand,
                    null,
                    CancellationToken.None,
                    async () =>
                    {
                        NotificationHelper.DispatchInformative("Remove Project Reference", $"Modified {modifyProjectNamespacePath.AbsolutePath.NameWithExtension} to NOT have a reference to {treeViewCSharpProjectNugetPackageReference.Item.LightWeightNugetPackageRecord.Id}", _luthetusCommonComponentRenderers, dispatcher);
                        await onAfterCompletion.Invoke();
                    });

                await terminalSession.EnqueueCommandAsync(removeNugetPackageReferenceFromProjectTerminalCommand);
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