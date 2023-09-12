using Fluxor;
using Luthetus.Common.RazorLib.ComponentRenderers;
using Luthetus.Common.RazorLib.ComponentRenderers.Types;
using Luthetus.Common.RazorLib.Dialog;
using Luthetus.Common.RazorLib.Dimensions;
using Luthetus.Common.RazorLib.Dropdown;
using Luthetus.Common.RazorLib.FileSystem.Interfaces;
using Luthetus.Common.RazorLib.Menu;
using Luthetus.Common.RazorLib.Namespaces;
using Luthetus.Common.RazorLib.Notification;
using Luthetus.Common.RazorLib.Store.DialogCase;
using Luthetus.Common.RazorLib.Store.NotificationCase;
using Luthetus.Common.RazorLib.TreeView;
using Luthetus.Common.RazorLib.TreeView.Commands;
using Luthetus.Common.RazorLib.TreeView.TreeViewClasses;
using Luthetus.CompilerServices.Lang.DotNetSolution.RewriteForImmutability;
using Luthetus.Ide.ClassLib.CommandLine;
using Luthetus.Ide.ClassLib.FileConstants;
using Luthetus.Ide.ClassLib.InputFile;
using Luthetus.Ide.ClassLib.Menu;
using Luthetus.Ide.ClassLib.Store.DotNetSolutionCase;
using Luthetus.Ide.ClassLib.Store.InputFileCase;
using Luthetus.Ide.ClassLib.Store.ProgramExecutionCase;
using Luthetus.Ide.ClassLib.Store.TerminalCase;
using Luthetus.Ide.ClassLib.TreeViewImplementations;
using Luthetus.Ide.RazorLib.CSharpProjectForm;
using Microsoft.AspNetCore.Components;
using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.SolutionExplorer;

public partial class SolutionExplorerContextMenu : ComponentBase
{
    [Inject]
    private IState<TerminalSessionRegistry> TerminalSessionsStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private IMenuOptionsFactory MenuOptionsFactory { get; set; } = null!;
    [Inject]
    private ILuthetusCommonComponentRenderers LuthetusCommonComponentRenderers { get; set; } = null!;
    [Inject]
    private ITreeViewService TreeViewService { get; set; } = null!;

    [Parameter, EditorRequired]
    public ITreeViewCommandParameter TreeViewCommandParameter { get; set; } = null!;

    public static readonly DropdownKey ContextMenuEventDropdownKey = DropdownKey.NewKey();

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
                if (treeViewSolution.Parent is null || treeViewSolution.Parent is TreeViewAdhoc)
                    menuRecords.AddRange(GetDotNetSolutionMenuOptions(treeViewSolution));
            }
        }
        else if (treeViewModel is TreeViewCSharpProjectToProjectReference treeViewCSharpProjectToProjectReference)
        {
            menuRecords.AddRange(
                GetCSharpProjectToProjectReferenceMenuOptions(
                    treeViewCSharpProjectToProjectReference));
        }
        else if (treeViewModel is TreeViewCSharpProjectNugetPackageReference treeViewCSharpProjectNugetPackageReference)
        {
            menuRecords.AddRange(
                GetTreeViewLightWeightNugetPackageRecordMenuOptions(
                    treeViewCSharpProjectNugetPackageReference));
        }

        if (!menuRecords.Any())
            return MenuRecord.Empty;

        return new MenuRecord(
            menuRecords.ToImmutableArray());
    }

    private MenuOptionRecord[] GetDotNetSolutionMenuOptions(
        TreeViewSolution treeViewSolution)
    {
        // TODO: Add menu options for non C# projects perhaps a more generic option is good

        var addNewCSharpProject = new MenuOptionRecord(
            "New C# Project",
            MenuOptionKind.Other,
            () => OpenNewCSharpProjectDialog(treeViewSolution.Item));

        var addExistingCSharpProject = new MenuOptionRecord(
            "Existing C# Project",
            MenuOptionKind.Other,
            () => AddExistingProjectToSolution(treeViewSolution.Item));

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
        var parentDirectory = (IAbsolutePath)treeViewModel.Item.AbsoluteFilePath.AncestorDirectories.Last();

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
            MenuOptionsFactory.NewEmptyFile(
                parentDirectory,
                async () => await ReloadTreeViewModel(treeViewModel)),
            MenuOptionsFactory.NewTemplatedFile(
                new NamespacePath(treeViewModel.Item.Namespace, parentDirectory),
                async () => await ReloadTreeViewModel(treeViewModel)),
            MenuOptionsFactory.NewDirectory(
                parentDirectory,
                async () => await ReloadTreeViewModel(treeViewModel)),
            MenuOptionsFactory.PasteClipboard(
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
            MenuOptionsFactory.AddProjectToProjectReference(
                treeViewModel,
                TerminalSessionsStateWrap.Value
                    .TerminalSessionMap[
                        TerminalSessionFacts.GENERAL_TERMINAL_SESSION_KEY],
                Dispatcher,
                () => Task.CompletedTask),
            MenuOptionsFactory.MoveProjectToSolutionFolder(
                treeViewSolution,
                treeViewModel,
                TerminalSessionsStateWrap.Value
                    .TerminalSessionMap[
                        TerminalSessionFacts.GENERAL_TERMINAL_SESSION_KEY],
                Dispatcher,
                () =>
                {
                    Dispatcher.Dispatch(new DotNetSolutionRegistry.SetDotNetSolutionAction(
                        treeViewSolution.Item.NamespacePath.AbsoluteFilePath));

                    return Task.CompletedTask;
                }),
            new MenuOptionRecord(
                "Set as Startup Project",
                MenuOptionKind.Other,
                () => Dispatcher.Dispatch(new ProgramExecutionRegistry.SetStartupProjectAbsoluteFilePathAction(
                    treeViewModel.Item.AbsoluteFilePath))),
            MenuOptionsFactory.RemoveCSharpProjectReferenceFromSolution(
                treeViewSolution,
                treeViewModel,
                TerminalSessionsStateWrap.Value
                    .TerminalSessionMap[
                        TerminalSessionFacts.GENERAL_TERMINAL_SESSION_KEY],
                Dispatcher,
                () =>
                {
                    Dispatcher.Dispatch(new DotNetSolutionRegistry.SetDotNetSolutionAction(
                        treeViewSolution.Item.NamespacePath.AbsoluteFilePath));

                    return Task.CompletedTask;
                }),
        };
    }

    private MenuOptionRecord[] GetCSharpProjectToProjectReferenceMenuOptions(
        TreeViewCSharpProjectToProjectReference treeViewCSharpProjectToProjectReference)
    {
        return new[]
        {
            MenuOptionsFactory.RemoveProjectToProjectReference(
                treeViewCSharpProjectToProjectReference,
                TerminalSessionsStateWrap.Value
                    .TerminalSessionMap[
                        TerminalSessionFacts.GENERAL_TERMINAL_SESSION_KEY],
                Dispatcher, () => Task.CompletedTask),
        };
    }

    private MenuOptionRecord[] GetTreeViewLightWeightNugetPackageRecordMenuOptions(
        TreeViewCSharpProjectNugetPackageReference treeViewCSharpProjectNugetPackageReference)
    {
        var treeViewCSharpProjectNugetPackageReferences =
            treeViewCSharpProjectNugetPackageReference.Parent as TreeViewCSharpProjectNugetPackageReferences;

        return new[]
        {
        MenuOptionsFactory.RemoveNuGetPackageReferenceFromProject(
            treeViewCSharpProjectNugetPackageReferences.Item.CSharpProjectNamespacePath,
            treeViewCSharpProjectNugetPackageReference,
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
        MenuOptionsFactory.NewEmptyFile(
            treeViewModel.Item.AbsoluteFilePath,
            async () => await ReloadTreeViewModel(treeViewModel)),
        MenuOptionsFactory.NewTemplatedFile(
            treeViewModel.Item,
            async () => await ReloadTreeViewModel(treeViewModel)),
        MenuOptionsFactory.NewDirectory(
            treeViewModel.Item.AbsoluteFilePath,
            async () => await ReloadTreeViewModel(treeViewModel)),
        MenuOptionsFactory.PasteClipboard(
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
        MenuOptionsFactory.CopyFile(
            treeViewModel.Item.AbsoluteFilePath,
            () => NotifyCopyCompleted(treeViewModel.Item)),
        MenuOptionsFactory.CutFile(
            treeViewModel.Item.AbsoluteFilePath,
            () => NotifyCutCompleted(treeViewModel.Item, parentTreeViewModel)),
        MenuOptionsFactory.DeleteFile(
            treeViewModel.Item.AbsoluteFilePath,
            async () =>
            {
                await ReloadTreeViewModel(parentTreeViewModel);
            }),
        MenuOptionsFactory.RenameFile(
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

    private void OpenNewCSharpProjectDialog(DotNetSolutionModel dotNetSolutionModel)
    {
        var dialogRecord = new DialogRecord(
            DialogKey.NewKey(),
            "New C# Project",
            typeof(CSharpProjectFormDisplay),
            new Dictionary<string, object?>
            {
                {
                    nameof(CSharpProjectFormDisplay.DotNetSolutionModelKey),
                    dotNetSolutionModel.DotNetSolutionModelKey
                },
            },
            null)
        {
            IsResizable = true
        };

        Dispatcher.Dispatch(new DialogRegistry.RegisterAction(
            dialogRecord));
    }

    private void AddExistingProjectToSolution(DotNetSolutionModel dotNetSolutionModel)
    {
        Dispatcher.Dispatch(new InputFileRegistry.RequestInputFileStateFormAction(
            "Existing C# Project to add to solution",
            async afp =>
            {
                if (afp is null)
                    return;

                var localFormattedAddExistingProjectToSolutionCommand = DotNetCliFacts
                    .FormatAddExistingProjectToSolution(
                        dotNetSolutionModel.NamespacePath.AbsoluteFilePath.FormattedInput,
                        afp.FormattedInput);

                var addExistingProjectToSolutionTerminalCommand = new TerminalCommand(
                    TerminalCommandKey.NewKey(),
                    localFormattedAddExistingProjectToSolutionCommand,
                    null,
                    CancellationToken.None,
                    () =>
                    {
                        Dispatcher.Dispatch(new DotNetSolutionRegistry.SetDotNetSolutionAction(
                            dotNetSolutionModel.NamespacePath.AbsoluteFilePath));

                        return Task.CompletedTask;
                    });

                var generalTerminalSession = TerminalSessionsStateWrap.Value.TerminalSessionMap[
                    TerminalSessionFacts.GENERAL_TERMINAL_SESSION_KEY];

                await generalTerminalSession.EnqueueCommandAsync(addExistingProjectToSolutionTerminalCommand);
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
            DotNetSolutionRegistry.TreeViewSolutionExplorerStateKey,
            treeViewModel);

        TreeViewService.MoveUp(
            DotNetSolutionRegistry.TreeViewSolutionExplorerStateKey,
            false);
    }

    private Task NotifyCopyCompleted(
        NamespacePath namespacePath)
    {
        if (LuthetusCommonComponentRenderers.InformativeNotificationRendererType != null)
        {
            var notificationInformative = new NotificationRecord(
                NotificationKey.NewKey(),
                "Copy Action",
                LuthetusCommonComponentRenderers.InformativeNotificationRendererType,
                new Dictionary<string, object?>
                {
                {
                    nameof(IInformativeNotificationRendererType.Message),
                    $"Copied: {namespacePath.AbsoluteFilePath.NameWithExtension}"
                },
                },
                TimeSpan.FromSeconds(3),
                true,
                null);

            Dispatcher.Dispatch(new NotificationRegistry.RegisterAction(
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
                NotificationKey.NewKey(),
                "Cut Action",
                LuthetusCommonComponentRenderers.InformativeNotificationRendererType,
                new Dictionary<string, object?>
                {
                {
                    nameof(IInformativeNotificationRendererType.Message),
                    $"Cut: {namespacePath.AbsoluteFilePath.NameWithExtension}"
                },
                },
                TimeSpan.FromSeconds(3),
                true,
                null);

            Dispatcher.Dispatch(new NotificationRegistry.RegisterAction(
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