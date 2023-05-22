using Microsoft.AspNetCore.Components;
using System.Collections.Immutable;
using Luthetus.Common.RazorLib.ComponentRenderers;
using Luthetus.Common.RazorLib.ComponentRenderers.Types;
using Luthetus.Common.RazorLib.Dialog;
using Luthetus.Common.RazorLib.Dimensions;
using Luthetus.Common.RazorLib.Dropdown;
using Luthetus.Common.RazorLib.Menu;
using Luthetus.Common.RazorLib.Notification;
using Luthetus.Common.RazorLib.Store.DialogCase;
using Luthetus.Common.RazorLib.Store.NotificationCase;
using Luthetus.Common.RazorLib.TreeView;
using Luthetus.Common.RazorLib.TreeView.Commands;
using Luthetus.Common.RazorLib.TreeView.TreeViewClasses;
using Luthetus.Ide.ClassLib.InputFile;
using Luthetus.Ide.ClassLib.Store.DotNetSolutionCase;
using Luthetus.Ide.ClassLib.Store.InputFileCase;
using Luthetus.Ide.ClassLib.Store.TerminalCase;
using Luthetus.Ide.ClassLib.FileConstants;
using Luthetus.Ide.RazorLib.CSharpProjectForm;
using Luthetus.Ide.ClassLib.FileSystem.Interfaces;
using Luthetus.Ide.ClassLib.Store.ProgramExecutionCase;
using Luthetus.Ide.ClassLib.TreeViewImplementations;
using Luthetus.Ide.ClassLib.Namespaces;
using Luthetus.Ide.ClassLib.CommandLine;
using Fluxor;

namespace Luthetus.Ide.RazorLib.SolutionExplorer;

public partial class SolutionExplorerContextMenu : ComponentBase
{
    [Inject]
    private IState<TerminalSessionsState> TerminalSessionsStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private ClassLib.Menu.ICommonMenuOptionsFactory CommonMenuOptionsFactory { get; set; } = null!;
    [Inject]
    private ILuthetusCommonComponentRenderers LuthetusCommonComponentRenderers { get; set; } = null!;
    [Inject]
    private ITreeViewService TreeViewService { get; set; } = null!;
    [Inject]
    private IFileSystemProvider FileSystemProvider { get; set; } = null!;
    [Inject]
    private IEnvironmentProvider EnvironmentProvider { get; set; } = null!;

    [Parameter, EditorRequired]
    public ITreeViewCommandParameter TreeViewCommandParameter { get; set; } = null!;

    public static readonly DropdownKey ContextMenuEventDropdownKey = DropdownKey.NewDropdownKey();

    /// <summary>
    /// The program is currently running using Photino locally on the user's computer
    /// therefore this static solution works without leaking any information.
    /// </summary>
    public static TreeViewNoType? ParentOfCutFile;

    private MenuRecord GetMenuRecord(
        ITreeViewCommandParameter treeViewCommandParameter)
    {
        if (treeViewCommandParameter.TargetNode is null)
            return MenuRecord.Empty;

        var menuRecords = new List<MenuOptionRecord>();

        var treeViewModel = treeViewCommandParameter.TargetNode;
        var parentTreeViewModel = treeViewModel.Parent;

        var parentTreeViewNamespacePath = parentTreeViewModel as TreeViewNamespacePath;

        if (treeViewModel is TreeViewNamespacePath treeViewNamespacePath)
        {
            if (treeViewNamespacePath.Item.AbsoluteFilePath.IsDirectory)
            {
                menuRecords.AddRange(
                    GetFileMenuOptions(treeViewNamespacePath, parentTreeViewNamespacePath)
                        .Union(GetDirectoryMenuOptions(treeViewNamespacePath))
                        .Union(GetDebugMenuOptions(treeViewNamespacePath)));
            }
            else
            {
                switch (treeViewNamespacePath.Item.AbsoluteFilePath.ExtensionNoPeriod)
                {
                    case ExtensionNoPeriodFacts.C_SHARP_PROJECT:
                        menuRecords.AddRange(
                            GetCSharpProjectMenuOptions(treeViewNamespacePath)
                                .Union(GetDebugMenuOptions(treeViewNamespacePath)));
                        break;
                    default:
                        menuRecords.AddRange(
                            GetFileMenuOptions(treeViewNamespacePath, parentTreeViewNamespacePath)
                                .Union(GetDebugMenuOptions(treeViewNamespacePath)));
                        break;
                }
            }
        }
        else if (treeViewModel is TreeViewSolution treeViewSolution)
        {
            if (treeViewSolution.Item.NamespacePath.AbsoluteFilePath.ExtensionNoPeriod ==
                ExtensionNoPeriodFacts.DOT_NET_SOLUTION)
            {
                if (treeViewSolution.Parent is null ||
                    treeViewSolution.Parent is TreeViewAdhoc)
                {
                    menuRecords.AddRange(
                        GetDotNetSolutionMenuOptions(treeViewSolution));
                }
            }
        }
        else if (treeViewModel is TreeViewCSharpProjectToProjectReference treeViewCSharpProjectToProjectReference)
        {
            menuRecords.AddRange(
                GetCSharpProjectToProjectReferenceMenuOptions(
                    treeViewCSharpProjectToProjectReference));
        }
        else if (treeViewModel is TreeViewLightWeightNugetPackageRecord treeViewLightWeightNugetPackageRecord)
        {
            menuRecords.AddRange(
                GetTreeViewLightWeightNugetPackageRecordMenuOptions(
                    treeViewLightWeightNugetPackageRecord));
        }

        if (!menuRecords.Any())
            return MenuRecord.Empty;

        return new MenuRecord(
            menuRecords.ToImmutableArray());
    }

    private MenuOptionRecord[] GetDotNetSolutionMenuOptions(
        TreeViewSolution treeViewSolution)
    {
        if (treeViewSolution.Item is null)
            return Array.Empty<MenuOptionRecord>();

        // TODO: Add menu options for non C# projects perhaps a more generic option is good

        var addNewCSharpProject = new MenuOptionRecord(
            "New C# Project",
            MenuOptionKind.Other,
            () => OpenNewCSharpProjectDialog(treeViewSolution.Item.NamespacePath));

        var addExistingCSharpProject = new MenuOptionRecord(
            "Existing C# Project",
            MenuOptionKind.Other,
            () => AddExistingProjectToSolution(treeViewSolution.Item.NamespacePath));

        return new[]
        {
            new MenuOptionRecord(
                "Add",
                MenuOptionKind.Other,
                SubMenu: new MenuRecord(
                    new MenuOptionRecord[]
                    {
                        addNewCSharpProject,
                        addExistingCSharpProject
                    }.ToImmutableArray())),
        };
    }

    private MenuOptionRecord[] GetCSharpProjectMenuOptions(
        TreeViewNamespacePath treeViewModel)
    {
        var parentDirectory = (IAbsoluteFilePath)treeViewModel.Item.AbsoluteFilePath.Directories.Last();

        var treeViewSolution = treeViewModel.Parent as TreeViewSolution;

        if (treeViewSolution is null)
        {
            var ancestorTreeView = treeViewModel.Parent;

            if (ancestorTreeView.Parent is null)
                return Array.Empty<MenuOptionRecord>();

            // Parent could be a could be one or many levels of solution folders
            while (ancestorTreeView.Parent is not null)
                ancestorTreeView = ancestorTreeView.Parent;

            treeViewSolution = ancestorTreeView as TreeViewSolution;

            if (treeViewSolution is null)
                return Array.Empty<MenuOptionRecord>();
        }

        return new[]
        {
            CommonMenuOptionsFactory.NewEmptyFile(
                parentDirectory,
                async () => await ReloadTreeViewModel(treeViewModel)),
            CommonMenuOptionsFactory.NewTemplatedFile(
                new NamespacePath(treeViewModel.Item.Namespace, parentDirectory),
                async () => await ReloadTreeViewModel(treeViewModel)),
            CommonMenuOptionsFactory.NewDirectory(
                parentDirectory,
                async () => await ReloadTreeViewModel(treeViewModel)),
            CommonMenuOptionsFactory.PasteClipboard(
                parentDirectory,
                async () =>
                {
                    var localParentOfCutFile =
                        ParentOfCutFile;

                    ParentOfCutFile = null;

                    if (localParentOfCutFile is not null)
                        await ReloadTreeViewModel(localParentOfCutFile);

                    await ReloadTreeViewModel(treeViewModel);
                }),
            CommonMenuOptionsFactory.AddProjectToProjectReference(
                treeViewModel,
                TerminalSessionsStateWrap.Value
                    .TerminalSessionMap[
                        TerminalSessionFacts.GENERAL_TERMINAL_SESSION_KEY],
                Dispatcher,
                () => Task.CompletedTask),
            CommonMenuOptionsFactory.MoveProjectToSolutionFolder(
                treeViewSolution,
                treeViewModel,
                TerminalSessionsStateWrap.Value
                    .TerminalSessionMap[
                        TerminalSessionFacts.GENERAL_TERMINAL_SESSION_KEY],
                Dispatcher,
                async () =>
                {
                    await DotNetSolutionState.SetActiveSolutionAsync(
                        treeViewSolution.Item.NamespacePath.AbsoluteFilePath.GetAbsoluteFilePathString(),
                        FileSystemProvider,
                        EnvironmentProvider,
                        Dispatcher);
                }),
            new MenuOptionRecord(
                "Set as Startup Project",
                MenuOptionKind.Other,
                () => Dispatcher.Dispatch(
                    new ProgramExecutionState.SetStartupProjectAbsoluteFilePathAction(
                        treeViewModel.Item.AbsoluteFilePath))),
            CommonMenuOptionsFactory.RemoveCSharpProjectReferenceFromSolution(
                treeViewSolution,
                treeViewModel,
                TerminalSessionsStateWrap.Value
                    .TerminalSessionMap[
                        TerminalSessionFacts.GENERAL_TERMINAL_SESSION_KEY],
                Dispatcher,
                async () =>
                {
                    if (treeViewSolution?.Item is not null)
                    {
                        await DotNetSolutionState.SetActiveSolutionAsync(
                            treeViewSolution.Item.NamespacePath.AbsoluteFilePath.GetAbsoluteFilePathString(),
                            FileSystemProvider,
                            EnvironmentProvider,
                            Dispatcher);
                    }
                }),
        };
    }

    private MenuOptionRecord[] GetCSharpProjectToProjectReferenceMenuOptions(
        TreeViewCSharpProjectToProjectReference treeViewCSharpProjectToProjectReference)
    {
        return new[]
        {
            CommonMenuOptionsFactory.RemoveProjectToProjectReference(
                treeViewCSharpProjectToProjectReference,
                TerminalSessionsStateWrap.Value
                    .TerminalSessionMap[
                        TerminalSessionFacts.GENERAL_TERMINAL_SESSION_KEY],
                Dispatcher, () => Task.CompletedTask),
        };
    }

    private MenuOptionRecord[] GetTreeViewLightWeightNugetPackageRecordMenuOptions(
        TreeViewLightWeightNugetPackageRecord treeViewLightWeightNugetPackageRecord)
    {
        var treeViewCSharpProjectNugetPackageReferences =
            treeViewLightWeightNugetPackageRecord.Parent as TreeViewCSharpProjectNugetPackageReferences;

        return new[]
        {
            CommonMenuOptionsFactory.RemoveNuGetPackageReferenceFromProject(
                treeViewCSharpProjectNugetPackageReferences.Item.CSharpProjectNamespacePath,
                treeViewLightWeightNugetPackageRecord,
                TerminalSessionsStateWrap.Value
                    .TerminalSessionMap[
                        TerminalSessionFacts.GENERAL_TERMINAL_SESSION_KEY],
                Dispatcher, () => Task.CompletedTask),
        };
    }

    private MenuOptionRecord[] GetDirectoryMenuOptions(
        TreeViewNamespacePath treeViewModel)
    {
        return new[]
        {
            CommonMenuOptionsFactory.NewEmptyFile(
                treeViewModel.Item.AbsoluteFilePath,
                async () => await ReloadTreeViewModel(treeViewModel)),
            CommonMenuOptionsFactory.NewTemplatedFile(
                treeViewModel.Item,
                async () => await ReloadTreeViewModel(treeViewModel)),
            CommonMenuOptionsFactory.NewDirectory(
                treeViewModel.Item.AbsoluteFilePath,
                async () => await ReloadTreeViewModel(treeViewModel)),
            CommonMenuOptionsFactory.PasteClipboard(
                treeViewModel.Item.AbsoluteFilePath,
                async () =>
                {
                    var localParentOfCutFile =
                        ParentOfCutFile;

                    ParentOfCutFile = null;

                    if (localParentOfCutFile is not null)
                        await ReloadTreeViewModel(localParentOfCutFile);

                    await ReloadTreeViewModel(treeViewModel);
                }),
        };
    }

    private MenuOptionRecord[] GetFileMenuOptions(
        TreeViewNamespacePath treeViewModel,
        TreeViewNamespacePath? parentTreeViewModel)
    {
        return new[]
        {
            CommonMenuOptionsFactory.CopyFile(
                treeViewModel.Item.AbsoluteFilePath,
                () => NotifyCopyCompleted(treeViewModel.Item)),
            CommonMenuOptionsFactory.CutFile(
                treeViewModel.Item.AbsoluteFilePath,
                () => NotifyCutCompleted(treeViewModel.Item, parentTreeViewModel)),
            CommonMenuOptionsFactory.DeleteFile(
                treeViewModel.Item.AbsoluteFilePath,
                async () =>
                {
                    await ReloadTreeViewModel(parentTreeViewModel);
                }),
            CommonMenuOptionsFactory.RenameFile(
                treeViewModel.Item.AbsoluteFilePath,
                Dispatcher,
                async ()  =>
                {
                    await ReloadTreeViewModel(parentTreeViewModel);
                }),
        };
    }

    private MenuOptionRecord[] GetDebugMenuOptions(
        TreeViewNamespacePath treeViewModel)
    {
        return new MenuOptionRecord[]
        {
            // new MenuOptionRecord(
            //     $"namespace: {treeViewModel.Item.Namespace}",
            //     MenuOptionKind.Read)
        };
    }

    private void OpenNewCSharpProjectDialog(
        NamespacePath solutionNamespacePath)
    {
        var dialogRecord = new DialogRecord(
            DialogKey.NewDialogKey(),
            "New C# Project",
            typeof(CSharpProjectFormDisplay),
            new Dictionary<string, object?>
            {
                {
                    nameof(CSharpProjectFormDisplay.SolutionNamespacePath),
                    solutionNamespacePath
                }
            },
            null)
        {
            IsResizable = true
        };

        Dispatcher.Dispatch(
            new DialogRecordsCollection.RegisterAction(
                dialogRecord));
    }

    private void AddExistingProjectToSolution(
        NamespacePath solutionNamespacePath)
    {
        Dispatcher.Dispatch(
            new InputFileState.RequestInputFileStateFormAction(
                "Existing C# Project to add to solution",
                async afp =>
                {
                    if (afp is null)
                        return;

                    var localFormattedAddExistingProjectToSolutionCommand = DotNetCliFacts
                        .FormatAddExistingProjectToSolution(
                            solutionNamespacePath.AbsoluteFilePath.GetAbsoluteFilePathString(),
                            afp.GetAbsoluteFilePathString());

                    var addExistingProjectToSolutionTerminalCommand = new TerminalCommand(
                        TerminalCommandKey.NewTerminalCommandKey(),
                        localFormattedAddExistingProjectToSolutionCommand.targetFileName,
                        localFormattedAddExistingProjectToSolutionCommand.arguments,
                        null,
                        CancellationToken.None,
                        async () =>
                        {
                            await DotNetSolutionState.SetActiveSolutionAsync(
                                solutionNamespacePath.AbsoluteFilePath.GetAbsoluteFilePathString(),
                                FileSystemProvider,
                                EnvironmentProvider,
                                Dispatcher);
                        });

                    var generalTerminalSession = TerminalSessionsStateWrap.Value.TerminalSessionMap[
                        TerminalSessionFacts.GENERAL_TERMINAL_SESSION_KEY];

                    await generalTerminalSession
                        .EnqueueCommandAsync(addExistingProjectToSolutionTerminalCommand);
                },
                afp =>
                {
                    if (afp is null ||
                        afp.IsDirectory)
                    {
                        return Task.FromResult(false);
                    }

                    return Task.FromResult(
                        afp.ExtensionNoPeriod
                            .EndsWith(ExtensionNoPeriodFacts.C_SHARP_PROJECT));
                },
                new[]
                {
                    new InputFilePattern(
                        "C# Project",
                        afp =>
                            afp.ExtensionNoPeriod
                                .EndsWith(ExtensionNoPeriodFacts.C_SHARP_PROJECT))
                }.ToImmutableArray()));
    }

    /// <summary>
    /// This method I believe is causing bugs
    /// <br/><br/>
    /// For example, when removing a C# Project the
    /// solution is reloaded and a new root is made.
    /// <br/><br/>
    /// Then there is a timing issue where the new root is made and set
    /// as the root. But this method erroneously reloads the old root.
    /// </summary>
    /// <param name="treeViewModel"></param>
    private async Task ReloadTreeViewModel(
        TreeViewNoType? treeViewModel)
    {
        if (treeViewModel is null)
            return;

        await treeViewModel.LoadChildrenAsync();

        TreeViewService.ReRenderNode(
            SolutionExplorerDisplay.TreeViewSolutionExplorerStateKey,
            treeViewModel);

        TreeViewService.MoveUp(
            SolutionExplorerDisplay.TreeViewSolutionExplorerStateKey,
            false);
    }

    private Task NotifyCopyCompleted(
        NamespacePath namespacePath)
    {
        if (LuthetusCommonComponentRenderers.InformativeNotificationRendererType != null)
        {
            var notificationInformative = new NotificationRecord(
                NotificationKey.NewNotificationKey(),
                "Copy Action",
                LuthetusCommonComponentRenderers.InformativeNotificationRendererType,
                new Dictionary<string, object?>
                {
                    {
                        nameof(IInformativeNotificationRendererType.Message),
                        $"Copied: {namespacePath.AbsoluteFilePath.FilenameWithExtension}"
                    },
                },
                TimeSpan.FromSeconds(3),
                null);

            Dispatcher.Dispatch(
                new NotificationRecordsCollection.RegisterAction(
                    notificationInformative));
        }

        return Task.CompletedTask;
    }

    private Task NotifyCutCompleted(
        NamespacePath namespacePath,
        TreeViewNamespacePath? parentTreeViewModel)
    {
        ParentOfCutFile = parentTreeViewModel;

        if (LuthetusCommonComponentRenderers.InformativeNotificationRendererType != null)
        {
            var notificationInformative = new NotificationRecord(
                NotificationKey.NewNotificationKey(),
                "Cut Action",
                LuthetusCommonComponentRenderers.InformativeNotificationRendererType,
                new Dictionary<string, object?>
                {
                    {
                        nameof(IInformativeNotificationRendererType.Message),
                        $"Cut: {namespacePath.AbsoluteFilePath.FilenameWithExtension}"
                    },
                },
                TimeSpan.FromSeconds(3),
                null);

            Dispatcher.Dispatch(
                new NotificationRecordsCollection.RegisterAction(
                    notificationInformative));
        }

        return Task.CompletedTask;
    }

    public static string GetContextMenuCssStyleString(
        ITreeViewCommandParameter? treeViewCommandParameter)
    {
        if (treeViewCommandParameter?.ContextMenuFixedPosition is null)
            return "display: none;";

        var left =
            $"left: {treeViewCommandParameter.ContextMenuFixedPosition.LeftPositionInPixels.ToCssValue()}px;";

        var top =
            $"top: {treeViewCommandParameter.ContextMenuFixedPosition.TopPositionInPixels.ToCssValue()}px;";

        return $"{left} {top}";
    }
}
