using Luthetus.Ide.ClassLib.FileTemplates;
using Luthetus.Ide.ClassLib.InputFile;
using Luthetus.Ide.ClassLib.Store.InputFileCase;
using Luthetus.Ide.ClassLib.Store.TerminalCase;
using Luthetus.Ide.ClassLib.Clipboard;
using Luthetus.Ide.ClassLib.CommandLine;
using Luthetus.Ide.ClassLib.ComponentRenderers;
using Luthetus.Ide.ClassLib.ComponentRenderers.Types;
using Luthetus.Ide.ClassLib.FileConstants;
using Luthetus.Ide.ClassLib.TreeViewImplementations;
using Luthetus.Common.RazorLib.BackgroundTaskCase.Usage;
using Luthetus.Common.RazorLib.Menu;
using Fluxor;
using System.Collections.Immutable;
using Luthetus.Common.RazorLib.BackgroundTaskCase.BaseTypes;
using Luthetus.Common.RazorLib.Notification;
using Luthetus.Common.RazorLib.ComponentRenderers.Types;
using Luthetus.Common.RazorLib.Store.NotificationCase;
using Luthetus.Common.RazorLib.Clipboard;
using Luthetus.Common.RazorLib.Namespaces;
using Luthetus.Common.RazorLib.FileSystem.Classes.FilePath;
using Luthetus.Common.RazorLib.FileSystem.Interfaces;
using Luthetus.Common.RazorLib.ComponentRenderers;

namespace Luthetus.Ide.ClassLib.Menu;

public class MenuOptionsFactory : IMenuOptionsFactory
{
    private readonly ILuthetusIdeComponentRenderers _luthetusIdeComponentRenderers;
    private readonly ILuthetusCommonComponentRenderers _luthetusCommonComponentRenderers;
    private readonly IFileSystemProvider _fileSystemProvider;
    private readonly IEnvironmentProvider _environmentProvider;
    private readonly IClipboardService _clipboardService;
    private readonly ILuthetusCommonBackgroundTaskService _luthetusCommonBackgroundTaskService;

    public MenuOptionsFactory(
        ILuthetusIdeComponentRenderers luthetusIdeComponentRenderers,
        ILuthetusCommonComponentRenderers luthetusCommonComponentRenderers,
        IFileSystemProvider fileSystemProvider,
        IEnvironmentProvider environmentProvider,
        IClipboardService clipboardService,
        ILuthetusCommonBackgroundTaskService luthetusCommonBackgroundTaskService)
    {
        _luthetusIdeComponentRenderers = luthetusIdeComponentRenderers;
        _luthetusCommonComponentRenderers = luthetusCommonComponentRenderers;
        _fileSystemProvider = fileSystemProvider;
        _environmentProvider = environmentProvider;
        _clipboardService = clipboardService;
        _luthetusCommonBackgroundTaskService = luthetusCommonBackgroundTaskService;
    }

    public MenuOptionRecord NewEmptyFile(
        IAbsolutePath parentDirectory,
        Func<Task> onAfterCompletion)
    {
        return new MenuOptionRecord(
            "New Empty File",
            MenuOptionKind.Create,
            WidgetRendererType: _luthetusIdeComponentRenderers.FileFormRendererType,
            WidgetParameters: new Dictionary<string, object?>
            {
                {
                    nameof(IFileFormRendererType.FileName),
                    string.Empty
                },
                {
                    nameof(IFileFormRendererType.CheckForTemplates),
                    false
                },
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

    public MenuOptionRecord NewTemplatedFile(
        NamespacePath parentDirectory,
        Func<Task> onAfterCompletion)
    {
        return new MenuOptionRecord(
            "New Templated File",
            MenuOptionKind.Create,
            WidgetRendererType: _luthetusIdeComponentRenderers.FileFormRendererType,
            WidgetParameters: new Dictionary<string, object?>
            {
                {
                    nameof(IFileFormRendererType.FileName),
                    string.Empty
                },
                {
                    nameof(IFileFormRendererType.CheckForTemplates),
                    true
                },
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

    public MenuOptionRecord NewDirectory(
        IAbsolutePath parentDirectory,
        Func<Task> onAfterCompletion)
    {
        return new MenuOptionRecord(
            "New Directory",
            MenuOptionKind.Create,
            WidgetRendererType: _luthetusIdeComponentRenderers.FileFormRendererType,
            WidgetParameters: new Dictionary<string, object?>
            {
                {
                    nameof(IFileFormRendererType.FileName),
                    string.Empty
                },
                {
                    nameof(IFileFormRendererType.IsDirectory),
                    true
                },
                {
                    nameof(IFileFormRendererType.OnAfterSubmitAction),
                    new Action<string, IFileTemplate?, ImmutableArray<IFileTemplate>>(
                        (directoryName, _, _) =>
                            PerformNewDirectoryAction(
                                directoryName,
                                parentDirectory,
                                onAfterCompletion))
                },
            });
    }

    public MenuOptionRecord DeleteFile(
        IAbsolutePath absoluteFilePath,
        Func<Task> onAfterCompletion)
    {
        return new MenuOptionRecord(
            "Delete",
            MenuOptionKind.Delete,
            WidgetRendererType: _luthetusIdeComponentRenderers.DeleteFileFormRendererType,
            WidgetParameters: new Dictionary<string, object?>
            {
                {
                    nameof(IDeleteFileFormRendererType.AbsoluteFilePath),
                    absoluteFilePath
                },
                {
                    nameof(IDeleteFileFormRendererType.IsDirectory),
                    true
                },
                {
                    nameof(IDeleteFileFormRendererType.OnAfterSubmitAction),
                    new Action<IAbsolutePath>(afp =>
                        PerformDeleteFileAction(afp, onAfterCompletion))
                },
            });
    }

    public MenuOptionRecord RenameFile(
        IAbsolutePath sourceAbsoluteFilePath,
        IDispatcher dispatcher,
        Func<Task> onAfterCompletion)
    {
        return new MenuOptionRecord(
            "Rename",
            MenuOptionKind.Update,
            WidgetRendererType: _luthetusIdeComponentRenderers.FileFormRendererType,
            WidgetParameters: new Dictionary<string, object?>
            {
                {
                    nameof(IFileFormRendererType.FileName),
                    sourceAbsoluteFilePath.IsDirectory
                        ? sourceAbsoluteFilePath.NameNoExtension
                        : sourceAbsoluteFilePath.NameWithExtension
                },
                {
                    nameof(IFileFormRendererType.IsDirectory),
                    sourceAbsoluteFilePath.IsDirectory
                },
                {
                    nameof(IFileFormRendererType.OnAfterSubmitAction),
                    new Action<string, IFileTemplate?, ImmutableArray<IFileTemplate>>(
                        (nextName, _, _) =>
                            PerformRenameAction(
                                sourceAbsoluteFilePath,
                                nextName,
                                dispatcher,
                                onAfterCompletion))
                },
            });
    }

    public MenuOptionRecord CopyFile(
        IAbsolutePath absoluteFilePath,
        Func<Task> onAfterCompletion)
    {
        return new MenuOptionRecord(
            "Copy",
            MenuOptionKind.Update,
            OnClick: () => PerformCopyFileAction(
                absoluteFilePath,
                onAfterCompletion));
    }

    public MenuOptionRecord CutFile(
        IAbsolutePath absoluteFilePath,
        Func<Task> onAfterCompletion)
    {
        return new MenuOptionRecord(
            "Cut",
            MenuOptionKind.Update,
            OnClick: () => PerformCutFileAction(
                absoluteFilePath,
                onAfterCompletion));
    }

    public MenuOptionRecord PasteClipboard(
        IAbsolutePath directoryAbsoluteFilePath,
        Func<Task> onAfterCompletion)
    {
        return new MenuOptionRecord(
            "Paste",
            MenuOptionKind.Update,
            OnClick: () => PerformPasteFileAction(
                directoryAbsoluteFilePath,
                onAfterCompletion));
    }

    public MenuOptionRecord RemoveCSharpProjectReferenceFromSolution(
        TreeViewSolution treeViewSolution,
        TreeViewNamespacePath projectNode,
        TerminalSession terminalSession,
        IDispatcher dispatcher,
        Func<Task> onAfterCompletion)
    {
        return new MenuOptionRecord(
            "Remove (no files are deleted)",
            MenuOptionKind.Delete,
            WidgetRendererType: _luthetusIdeComponentRenderers.RemoveCSharpProjectFromSolutionRendererType,
            WidgetParameters: new Dictionary<string, object?>
            {
                {
                    nameof(IRemoveCSharpProjectFromSolutionRendererType.AbsoluteFilePath),
                    projectNode.Item.AbsoluteFilePath
                },
                {
                    nameof(IDeleteFileFormRendererType.OnAfterSubmitAction),
                    new Action<IAbsolutePath>(_ =>
                        PerformRemoveCSharpProjectReferenceFromSolutionAction(
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
        Func<Task> onAfterCompletion)
    {
        return new MenuOptionRecord(
            "Add Project Reference",
            MenuOptionKind.Other,
            OnClick: () => PerformAddProjectToProjectReferenceAction(
                projectReceivingReference,
                terminalSession,
                dispatcher,
                onAfterCompletion));
    }

    public MenuOptionRecord RemoveProjectToProjectReference(
        TreeViewCSharpProjectToProjectReference treeViewCSharpProjectToProjectReference,
        TerminalSession terminalSession,
        IDispatcher dispatcher,
        Func<Task> onAfterCompletion)
    {
        return new MenuOptionRecord(
            "Remove Project Reference",
            MenuOptionKind.Other,
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
        return new MenuOptionRecord(
            "Move to Solution Folder",
            MenuOptionKind.Other,
            WidgetRendererType: _luthetusIdeComponentRenderers.FileFormRendererType,
            WidgetParameters: new Dictionary<string, object?>
            {
                {
                    nameof(IFileFormRendererType.FileName),
                    string.Empty
                },
                {
                    nameof(IFileFormRendererType.IsDirectory),
                    false
                },
                {
                    nameof(IFileFormRendererType.OnAfterSubmitAction),
                    new Action<string, IFileTemplate?, ImmutableArray<IFileTemplate>>(
                        (nextName, _, _) =>
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
        return new MenuOptionRecord(
            "Remove NuGet Package Reference",
            MenuOptionKind.Other,
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
        var backgroundTask = new BackgroundTask(
            async cancellationToken =>
            {
                if (exactMatchFileTemplate is null)
                {
                    var emptyFileAbsoluteFilePathString = 
                        namespacePath.AbsoluteFilePath.FormattedInput + fileName;

                    var emptyFileAbsoluteFilePath = new AbsolutePath(
                        emptyFileAbsoluteFilePathString,
                        false,
                        _environmentProvider);

                    await _fileSystemProvider.File.WriteAllTextAsync(
                        emptyFileAbsoluteFilePath.FormattedInput,
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
                            templateResult.FileNamespacePath.AbsoluteFilePath.FormattedInput,
                            templateResult.Contents,
                            CancellationToken.None);
                    }
                }

                await onAfterCompletion.Invoke();
            },
            "PerformNewFileActionTask",
            "TODO: Describe this task",
            false,
            _ => Task.CompletedTask,
            null,
            CancellationToken.None);

        _luthetusCommonBackgroundTaskService.QueueBackgroundWorkItem(backgroundTask);
    }

    private void PerformNewDirectoryAction(
        string directoryName,
        IAbsolutePath parentDirectory,
        Func<Task> onAfterCompletion)
    {
        var directoryAbsoluteFilePathString = parentDirectory.FormattedInput + directoryName;

        var directoryAbsoluteFilePath = new AbsolutePath(
            directoryAbsoluteFilePathString,
            true,
            _environmentProvider);

        var backgroundTask = new BackgroundTask(
            async cancellationToken =>
            {
                await _fileSystemProvider.Directory.CreateDirectoryAsync(
                    directoryAbsoluteFilePath.FormattedInput,
                    CancellationToken.None);

                await onAfterCompletion.Invoke();
            },
            "PerformNewDirectoryActionTask",
            "TODO: Describe this task",
            false,
            _ => Task.CompletedTask,
            null,
            CancellationToken.None);

        _luthetusCommonBackgroundTaskService.QueueBackgroundWorkItem(backgroundTask);
    }

    private void PerformDeleteFileAction(
        IAbsolutePath absoluteFilePath,
        Func<Task> onAfterCompletion)
    {
        var backgroundTask = new BackgroundTask(
            async cancellationToken =>
            {
                if (absoluteFilePath.IsDirectory)
                {
                    await _fileSystemProvider.Directory.DeleteAsync(
                        absoluteFilePath.FormattedInput,
                        true,
                        CancellationToken.None);
                }
                else
                {
                    await _fileSystemProvider.File.DeleteAsync(
                        absoluteFilePath.FormattedInput);
                }

                await onAfterCompletion.Invoke();
            },
            "PerformDeleteFileActionTask",
            "TODO: Describe this task",
            false,
            _ => Task.CompletedTask,
            null,
            CancellationToken.None);

        _luthetusCommonBackgroundTaskService.QueueBackgroundWorkItem(backgroundTask);
    }

    private void PerformCopyFileAction(
        IAbsolutePath absoluteFilePath,
        Func<Task> onAfterCompletion)
    {
        var backgroundTask = new BackgroundTask(
            async cancellationToken =>
            {
                await _clipboardService
                    .SetClipboard(
                        ClipboardFacts.FormatPhrase(
                            ClipboardFacts.CopyCommand,
                            ClipboardFacts.AbsoluteFilePathDataType,
                            absoluteFilePath.FormattedInput));

                await onAfterCompletion.Invoke();
            },
            "PerformCopyFileActionTask",
            "TODO: Describe this task",
            false,
            _ => Task.CompletedTask,
            null,
            CancellationToken.None);

        _luthetusCommonBackgroundTaskService.QueueBackgroundWorkItem(backgroundTask);
    }

    private void PerformCutFileAction(
        IAbsolutePath absoluteFilePath,
        Func<Task> onAfterCompletion)
    {
        var backgroundTask = new BackgroundTask(
            async cancellationToken =>
            {
                await _clipboardService
                    .SetClipboard(
                        ClipboardFacts.FormatPhrase(
                            ClipboardFacts.CutCommand,
                            ClipboardFacts.AbsoluteFilePathDataType,
                            absoluteFilePath.FormattedInput));

                await onAfterCompletion.Invoke();
            },
            "PerformCutFileActionTask",
            "TODO: Describe this task",
            false,
            _ => Task.CompletedTask,
            null,
            CancellationToken.None);

        _luthetusCommonBackgroundTaskService.QueueBackgroundWorkItem(backgroundTask);
    }

    private void PerformPasteFileAction(
        IAbsolutePath receivingDirectory,
        Func<Task> onAfterCompletion)
    {
        var backgroundTask = new BackgroundTask(
            async cancellationToken =>
            {
                var clipboardContents = await _clipboardService.ReadClipboard();

                if (ClipboardFacts.TryParseString(clipboardContents, out var clipboardPhrase))
                {
                    if (clipboardPhrase is not null &&
                        clipboardPhrase.DataType == ClipboardFacts.AbsoluteFilePathDataType)
                    {
                        if (clipboardPhrase.Command == ClipboardFacts.CopyCommand ||
                            clipboardPhrase.Command == ClipboardFacts.CutCommand)
                        {
                            IAbsolutePath? clipboardAbsoluteFilePath = null;

                            if (await _fileSystemProvider.Directory.ExistsAsync(clipboardPhrase.Value))
                            {
                                clipboardAbsoluteFilePath = new AbsolutePath(
                                    clipboardPhrase.Value,
                                    true,
                                    _environmentProvider);
                            }
                            else if (await _fileSystemProvider.File.ExistsAsync(clipboardPhrase.Value))
                            {
                                clipboardAbsoluteFilePath = new AbsolutePath(
                                    clipboardPhrase.Value,
                                    false,
                                    _environmentProvider);
                            }

                            if (clipboardAbsoluteFilePath is not null)
                            {
                                var successfullyPasted = true;

                                try
                                {
                                    if (clipboardAbsoluteFilePath.IsDirectory)
                                    {
                                        var clipboardDirectoryInfo = new DirectoryInfo(
                                            clipboardAbsoluteFilePath.FormattedInput);

                                        var receivingDirectoryInfo = new DirectoryInfo(
                                            receivingDirectory.FormattedInput);

                                        CopyFilesRecursively(clipboardDirectoryInfo, receivingDirectoryInfo);
                                    }
                                    else
                                    {
                                        var destinationAbsoluteFilePathString =
                                            receivingDirectory.FormattedInput +
                                            clipboardAbsoluteFilePath.NameWithExtension;

                                        var sourceAbsoluteFilePathString = clipboardAbsoluteFilePath
                                            .FormattedInput;

                                        await _fileSystemProvider.File.CopyAsync(
                                            sourceAbsoluteFilePathString,
                                            destinationAbsoluteFilePathString);
                                    }
                                }
                                catch (Exception)
                                {
                                    successfullyPasted = false;
                                }

                                if (successfullyPasted && clipboardPhrase.Command == ClipboardFacts.CutCommand)
                                {
                                    // TODO: Rerender the parent of the deleted due to cut file
                                    PerformDeleteFileAction(clipboardAbsoluteFilePath, onAfterCompletion);
                                }
                                else
                                {
                                    await onAfterCompletion.Invoke();
                                }
                            }
                        }
                    }
                }
            },
            "PerformPasteFileActionTask",
            "TODO: Describe this task",
            false,
            _ => Task.CompletedTask,
            null,
            CancellationToken.None);

        _luthetusCommonBackgroundTaskService.QueueBackgroundWorkItem(backgroundTask);
    }

    private IAbsolutePath? PerformRenameAction(
        IAbsolutePath sourceAbsoluteFilePath,
        string nextName,
        IDispatcher dispatcher,
        Func<Task> onAfterCompletion)
    {
        // If the current and next name match when compared
        // with case insensitivity
        if (string.Compare(
                sourceAbsoluteFilePath.NameWithExtension,
                nextName,
                StringComparison.OrdinalIgnoreCase)
                    == 0)
        {
            var temporaryNextName = _environmentProvider.GetRandomFileName();

            var temporaryRenameResult = PerformRenameAction(
                sourceAbsoluteFilePath,
                temporaryNextName,
                dispatcher,
                () => Task.CompletedTask);

            if (temporaryRenameResult is null)
            {
                onAfterCompletion.Invoke();
                return null;
            }
            else
                sourceAbsoluteFilePath = temporaryRenameResult;
        }

        var sourceAbsoluteFilePathString = sourceAbsoluteFilePath.FormattedInput;

        var parentOfSource = sourceAbsoluteFilePath.AncestorDirectories.Last();

        var destinationAbsoluteFilePathString = parentOfSource.FormattedInput + nextName;

        try
        {
            if (sourceAbsoluteFilePath.IsDirectory)
                _fileSystemProvider.Directory.MoveAsync(sourceAbsoluteFilePathString, destinationAbsoluteFilePathString);
            else
                _fileSystemProvider.File.MoveAsync(sourceAbsoluteFilePathString, destinationAbsoluteFilePathString);
        }
        catch (Exception e)
        {
            if (_luthetusCommonComponentRenderers.ErrorNotificationRendererType is not null)
            {
                var notificationError = new NotificationRecord(
                    NotificationKey.NewKey(),
                    "Rename Action",
                    _luthetusCommonComponentRenderers.ErrorNotificationRendererType,
                    new Dictionary<string, object?>
                    {
                        {
                            nameof(IErrorNotificationRendererType.Message),
                            $"ERROR: {e.Message}"
                        },
                    },
                    TimeSpan.FromSeconds(15),
                    true,
                    IErrorNotificationRendererType.CSS_CLASS_STRING);

                dispatcher.Dispatch(new NotificationRegistry.RegisterAction(notificationError));
            }

            onAfterCompletion.Invoke();
            return null;
        }

        onAfterCompletion.Invoke();

        return new AbsolutePath(
            destinationAbsoluteFilePathString,
            sourceAbsoluteFilePath.IsDirectory,
            _environmentProvider);
    }

    private void PerformRemoveCSharpProjectReferenceFromSolutionAction(
        TreeViewSolution treeViewSolution,
        TreeViewNamespacePath projectNode,
        TerminalSession terminalSession,
        IDispatcher dispatcher,
        Func<Task> onAfterCompletion)
    {
        var backgroundTask = new BackgroundTask(
            async cancellationToken =>
            {
                var workingDirectory = 
                    treeViewSolution.Item.NamespacePath.AbsoluteFilePath.ParentDirectory!;

                var removeCSharpProjectReferenceFromSolutionFormattedCommand =
                    DotNetCliFacts.FormatRemoveCSharpProjectReferenceFromSolutionAction(
                        treeViewSolution.Item.NamespacePath.AbsoluteFilePath.FormattedInput,
                        projectNode.Item.AbsoluteFilePath.FormattedInput);

                var removeCSharpProjectReferenceFromSolutionCommand = new TerminalCommand(
                    TerminalCommandKey.NewKey(),
                    removeCSharpProjectReferenceFromSolutionFormattedCommand,
                    workingDirectory.FormattedInput,
                    CancellationToken.None,
                    async () => await onAfterCompletion.Invoke());

                await terminalSession.EnqueueCommandAsync(
                    removeCSharpProjectReferenceFromSolutionCommand);
            },
            "PerformRemoveCSharpProjectReferenceFromSolutionActionTask",
            "TODO: Describe this task",
            false,
            _ => Task.CompletedTask,
            dispatcher,
            CancellationToken.None);

        _luthetusCommonBackgroundTaskService.QueueBackgroundWorkItem(backgroundTask);
    }

    public void PerformAddProjectToProjectReferenceAction(
        TreeViewNamespacePath projectReceivingReference,
        TerminalSession terminalSession,
        IDispatcher dispatcher,
        Func<Task> onAfterCompletion)
    {
        var backgroundTask = new BackgroundTask(
            cancellationToken =>
            {
                var requestInputFileStateFormAction = new InputFileRegistry.RequestInputFileStateFormAction(
                    $"Add Project reference to {projectReceivingReference.Item.AbsoluteFilePath.NameWithExtension}",
                    async referencedProject =>
                    {
                        if (referencedProject is null)
                            return;

                        var formattedCommand = DotNetCliFacts.FormatAddProjectToProjectReference(
                            projectReceivingReference.Item.AbsoluteFilePath.FormattedInput,
                            referencedProject.FormattedInput);

                        var addProjectToProjectReferenceTerminalCommand = new TerminalCommand(
                            TerminalCommandKey.NewKey(),
                            formattedCommand,
                            null,
                            CancellationToken.None,
                            async () =>
                            {
                                var notificationInformative = new NotificationRecord(
                                    NotificationKey.NewKey(),
                                    "Add Project Reference",
                                    _luthetusCommonComponentRenderers.InformativeNotificationRendererType,
                                    new Dictionary<string, object?>
                                    {
                                        {
                                            nameof(IInformativeNotificationRendererType.Message),
                                            $"Modified {projectReceivingReference.Item.AbsoluteFilePath.NameWithExtension} to have a reference to {referencedProject.NameWithExtension}"
                                        },
                                    },
                                    TimeSpan.FromSeconds(7),
                                    true,
                                    null);

                                dispatcher.Dispatch(new NotificationRegistry.RegisterAction(notificationInformative));

                                await onAfterCompletion.Invoke();
                            });

                        await terminalSession.EnqueueCommandAsync(addProjectToProjectReferenceTerminalCommand);
                    },
                    afp =>
                    {
                        if (afp is null || afp.IsDirectory)
                            return Task.FromResult(false);

                        return Task.FromResult(
                            afp.ExtensionNoPeriod.EndsWith(ExtensionNoPeriodFacts.C_SHARP_PROJECT));
                    },
                    new[]
                    {
                        new InputFilePattern(
                            "C# Project",
                            afp => afp.ExtensionNoPeriod.EndsWith(ExtensionNoPeriodFacts.C_SHARP_PROJECT))
                    }.ToImmutableArray());

                dispatcher.Dispatch(requestInputFileStateFormAction);

                return Task.CompletedTask;
            },
            "PerformAddProjectToProjectReferenceActionTask",
            "TODO: Describe this task",
            false,
            _ => Task.CompletedTask,
            dispatcher,
            CancellationToken.None);

        _luthetusCommonBackgroundTaskService.QueueBackgroundWorkItem(backgroundTask);
    }

    public void PerformRemoveProjectToProjectReferenceAction(
        TreeViewCSharpProjectToProjectReference treeViewCSharpProjectToProjectReference,
        TerminalSession terminalSession,
        IDispatcher dispatcher,
        Func<Task> onAfterCompletion)
    {
        var backgroundTask = new BackgroundTask(
            async cancellationToken =>
            {
                var formattedCommand = DotNetCliFacts.FormatRemoveProjectToProjectReference(
                    treeViewCSharpProjectToProjectReference.Item.ModifyProjectNamespacePath.AbsoluteFilePath.FormattedInput,
                    treeViewCSharpProjectToProjectReference.Item.ReferenceProjectAbsoluteFilePath.FormattedInput);

                var removeProjectToProjectReferenceTerminalCommand = new TerminalCommand(
                    TerminalCommandKey.NewKey(),
                    formattedCommand,
                    null,
                    CancellationToken.None,
                    async () =>
                    {
                        var notificationInformative = new NotificationRecord(
                            NotificationKey.NewKey(),
                            "Remove Project Reference",
                            _luthetusCommonComponentRenderers.InformativeNotificationRendererType,
                            new Dictionary<string, object?>
                            {
                                {
                                    nameof(IInformativeNotificationRendererType.Message),
                                    $"Modified {treeViewCSharpProjectToProjectReference.Item.ModifyProjectNamespacePath.AbsoluteFilePath.NameWithExtension} to have a reference to {treeViewCSharpProjectToProjectReference.Item.ReferenceProjectAbsoluteFilePath.NameWithExtension}"
                                },
                            },
                            TimeSpan.FromSeconds(7),
                            true,
                            null);

                        dispatcher.Dispatch(new NotificationRegistry.RegisterAction(notificationInformative));

                        await onAfterCompletion.Invoke();
                    });

                await terminalSession.EnqueueCommandAsync(removeProjectToProjectReferenceTerminalCommand);
            },
            "PerformRemoveProjectToProjectReferenceActionTask",
            "TODO: Describe this task",
            false,
            _ => Task.CompletedTask,
            dispatcher,
            CancellationToken.None);

        _luthetusCommonBackgroundTaskService.QueueBackgroundWorkItem(backgroundTask);
    }

    public void PerformMoveProjectToSolutionFolderAction(
        TreeViewSolution treeViewSolution,
        TreeViewNamespacePath treeViewProjectToMove,
        string solutionFolderPath,
        TerminalSession terminalSession,
        IDispatcher dispatcher,
        Func<Task> onAfterCompletion)
    {
        var backgroundTask = new BackgroundTask(
            cancellationToken =>
            {
                var formattedCommand = DotNetCliFacts.FormatMoveProjectToSolutionFolder(
                    treeViewSolution.Item.NamespacePath.AbsoluteFilePath.FormattedInput,
                    treeViewProjectToMove.Item.AbsoluteFilePath.FormattedInput,
                    solutionFolderPath);

                var moveProjectToSolutionFolderTerminalCommand = new TerminalCommand(
                    TerminalCommandKey.NewKey(),
                    formattedCommand,
                    null,
                    CancellationToken.None,
                    async () =>
                    {
                        var notificationInformative = new NotificationRecord(
                            NotificationKey.NewKey(),
                            "Move Project To Solution Folder",
                            _luthetusCommonComponentRenderers.InformativeNotificationRendererType,
                            new Dictionary<string, object?>
                            {
                                {
                                    nameof(IInformativeNotificationRendererType.Message),
                                    $"Moved {treeViewProjectToMove.Item.AbsoluteFilePath.NameWithExtension} to the Solution Folder path: {solutionFolderPath}"
                                },
                            },
                            TimeSpan.FromSeconds(7),
                            true,
                            null);

                        dispatcher.Dispatch(new NotificationRegistry.RegisterAction(notificationInformative));

                        await onAfterCompletion.Invoke();
                    });

                PerformRemoveCSharpProjectReferenceFromSolutionAction(
                    treeViewSolution,
                    treeViewProjectToMove,
                    terminalSession,
                    dispatcher,
                    async () => await terminalSession.EnqueueCommandAsync(moveProjectToSolutionFolderTerminalCommand));

                return Task.CompletedTask;
            },
            "PerformMoveProjectToSolutionFolderActionTask",
            "TODO: Describe this task",
            false,
            _ => Task.CompletedTask,
            dispatcher,
            CancellationToken.None);

        _luthetusCommonBackgroundTaskService.QueueBackgroundWorkItem(backgroundTask);
    }

    public void PerformRemoveNuGetPackageReferenceFromProjectAction(
        NamespacePath modifyProjectNamespacePath,
        TreeViewCSharpProjectNugetPackageReference treeViewCSharpProjectNugetPackageReference,
        TerminalSession terminalSession,
        IDispatcher dispatcher,
        Func<Task> onAfterCompletion)
    {
        var backgroundTask = new BackgroundTask(
            async cancellationToken =>
            {
                var formattedCommand = DotNetCliFacts.FormatRemoveNugetPackageReferenceFromProject(
                    modifyProjectNamespacePath.AbsoluteFilePath.FormattedInput,
                    treeViewCSharpProjectNugetPackageReference.Item.LightWeightNugetPackageRecord.Id);

                var removeNugetPackageReferenceFromProjectTerminalCommand = new TerminalCommand(
                    TerminalCommandKey.NewKey(),
                    formattedCommand,
                    null,
                    CancellationToken.None,
                    async () =>
                    {
                        var notificationInformative = new NotificationRecord(
                            NotificationKey.NewKey(),
                            "Remove Project Reference",
                            _luthetusCommonComponentRenderers.InformativeNotificationRendererType,
                            new Dictionary<string, object?>
                            {
                                {
                                    nameof(IInformativeNotificationRendererType.Message),
                                    $"Modified {modifyProjectNamespacePath.AbsoluteFilePath.NameWithExtension} to NOT have a reference to {treeViewCSharpProjectNugetPackageReference.Item.LightWeightNugetPackageRecord.Id}"
                                },
                            },
                            TimeSpan.FromSeconds(7),
                            true,
                            null);

                        dispatcher.Dispatch(new NotificationRegistry.RegisterAction(notificationInformative));

                        await onAfterCompletion.Invoke();
                    });

                await terminalSession.EnqueueCommandAsync(removeNugetPackageReferenceFromProjectTerminalCommand);
            },
            "PerformRemoveNuGetPackageReferenceFromProjectActionTask",
            "TODO: Describe this task",
            false,
            _ => Task.CompletedTask,
            dispatcher,
            CancellationToken.None);

        _luthetusCommonBackgroundTaskService.QueueBackgroundWorkItem(backgroundTask);
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