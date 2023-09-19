using Fluxor;
using Luthetus.Common.RazorLib.Dimensions;
using Luthetus.CompilerServices.Lang.DotNetSolution.RewriteForImmutability;
using Microsoft.AspNetCore.Components;
using System.Collections.Immutable;
using Luthetus.Ide.RazorLib.DotNetSolutionCase.States;
using Luthetus.Ide.RazorLib.CommandLineCase.Models;
using Luthetus.Ide.RazorLib.FileSystemCase.Models;
using Luthetus.Ide.RazorLib.MenuCase.Models;
using Luthetus.Ide.RazorLib.TerminalCase.Models;
using Luthetus.Ide.RazorLib.TerminalCase.States;
using Luthetus.Ide.RazorLib.TreeViewImplementationsCase.Models;
using Luthetus.Ide.RazorLib.ProgramExecutionCase.States;
using Luthetus.Ide.RazorLib.CSharpProjectFormCase.Displays;
using Luthetus.Ide.RazorLib.InputFileCase.States;
using Luthetus.Ide.RazorLib.InputFileCase.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Dialog.Models;
using Luthetus.Common.RazorLib.Dropdown.Models;
using Luthetus.Common.RazorLib.Menu.Models;
using Luthetus.Common.RazorLib.Notification.Models;
using Luthetus.Common.RazorLib.TreeView.Models;
using Luthetus.Common.RazorLib.Namespaces.Models;
using Luthetus.Common.RazorLib.Dialog.States;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.KeyCase.Models;

namespace Luthetus.Ide.RazorLib.DotNetSolutionCase.Displays;

public partial class SolutionExplorerContextMenu : ComponentBase
{
    [Inject]
    private IState<TerminalSessionState> TerminalSessionsStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private IMenuOptionsFactory MenuOptionsFactory { get; set; } = null!;
    [Inject]
    private ILuthetusCommonComponentRenderers LuthetusCommonComponentRenderers { get; set; } = null!;
    [Inject]
    private ITreeViewService TreeViewService { get; set; } = null!;
    [Inject]
    private DotNetSolutionSync DotNetSolutionSync { get; set; } = null!;
    [Inject]
    private InputFileSync InputFileSync { get; set; } = null!;

    [Parameter, EditorRequired]
    public TreeViewCommandParameter TreeViewCommandParameter { get; set; } = null!;

    public static readonly Key<DropdownRecord> ContextMenuEventDropdownKey = Key<DropdownRecord>.NewKey();

    /// <summary>
    /// The program is currently running using Photino locally on the user's computer
    /// therefore this static solution works without leaking any information.
    /// </summary>
    public static TreeViewNoType? ParentOfCutFile;

    private MenuRecord GetMenuRecord(
        TreeViewCommandParameter treeViewCommandParameter)
    {
        if (treeViewCommandParameter.TargetNode is null)
            return MenuRecord.Empty;

        var menuRecords = new List<MenuOptionRecord>();

        var treeViewModel = treeViewCommandParameter.TargetNode;
        var parentTreeViewModel = treeViewModel.Parent;

        var parentTreeViewNamespacePath = parentTreeViewModel as TreeViewNamespacePath;

        if (treeViewModel is TreeViewNamespacePath treeViewNamespacePath)
        {
            if (treeViewNamespacePath.Item.AbsolutePath.IsDirectory)
            {
                menuRecords.AddRange(
                    GetFileMenuOptions(treeViewNamespacePath, parentTreeViewNamespacePath)
                        .Union(GetDirectoryMenuOptions(treeViewNamespacePath))
                        .Union(GetDebugMenuOptions(treeViewNamespacePath)));
            }
            else
            {
                switch (treeViewNamespacePath.Item.AbsolutePath.ExtensionNoPeriod)
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
            if (treeViewSolution.Item.NamespacePath.AbsolutePath.ExtensionNoPeriod ==
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
        var parentDirectory = treeViewModel.Item.AbsolutePath.AncestorDirectories.Last();

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
                InputFileSync,
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
                    Dispatcher.Dispatch(new DotNetSolutionState.SetDotNetSolutionTask(
                        treeViewSolution.Item.NamespacePath.AbsolutePath,
                        DotNetSolutionSync));

                    return Task.CompletedTask;
                }),
            new MenuOptionRecord(
                "Set as Startup Project",
                MenuOptionKind.Other,
                () => Dispatcher.Dispatch(new ProgramExecutionState.SetStartupProjectAbsolutePathAction(
                    treeViewModel.Item.AbsolutePath))),
            MenuOptionsFactory.RemoveCSharpProjectReferenceFromSolution(
                treeViewSolution,
                treeViewModel,
                TerminalSessionsStateWrap.Value
                    .TerminalSessionMap[
                        TerminalSessionFacts.GENERAL_TERMINAL_SESSION_KEY],
                Dispatcher,
                () =>
                {
                    Dispatcher.Dispatch(new DotNetSolutionState.SetDotNetSolutionTask(
                        treeViewSolution.Item.NamespacePath.AbsolutePath,
                        DotNetSolutionSync));

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
            treeViewModel.Item.AbsolutePath,
            async () => await ReloadTreeViewModel(treeViewModel)),
        MenuOptionsFactory.NewTemplatedFile(
            treeViewModel.Item,
            async () => await ReloadTreeViewModel(treeViewModel)),
        MenuOptionsFactory.NewDirectory(
            treeViewModel.Item.AbsolutePath,
            async () => await ReloadTreeViewModel(treeViewModel)),
        MenuOptionsFactory.PasteClipboard(
            treeViewModel.Item.AbsolutePath,
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
            MenuOptionsFactory.CopyFile(treeViewModel.Item.AbsolutePath, () => {
                NotificationHelper.DispatchInformative("Copy Action", $"Copied: {treeViewModel.Item.AbsolutePath.NameWithExtension}", LuthetusCommonComponentRenderers, Dispatcher);
                return Task.CompletedTask;
            }),
            MenuOptionsFactory.CutFile(treeViewModel.Item.AbsolutePath, () => {
                ParentOfCutFile = parentTreeViewModel;
                NotificationHelper.DispatchInformative("Cut Action", $"Cut: {treeViewModel.Item.AbsolutePath.NameWithExtension}", LuthetusCommonComponentRenderers, Dispatcher);
                return Task.CompletedTask;
            }),
            MenuOptionsFactory.DeleteFile(treeViewModel.Item.AbsolutePath, async () => await ReloadTreeViewModel(parentTreeViewModel)),
            MenuOptionsFactory.RenameFile(treeViewModel.Item.AbsolutePath, Dispatcher, async ()  => await ReloadTreeViewModel(parentTreeViewModel)),
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
            Key<DialogRecord>.NewKey(),
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

        Dispatcher.Dispatch(new DialogState.RegisterAction(
            dialogRecord));
    }

    private void AddExistingProjectToSolution(DotNetSolutionModel dotNetSolutionModel)
    {
        Dispatcher.Dispatch(new InputFileState.RequestInputFileStateFormAction(
            InputFileSync,
            "Existing C# Project to add to solution",
            async afp =>
            {
                if (afp is null)
                    return;

                var localFormattedAddExistingProjectToSolutionCommand = DotNetCliCommandFormatter
                    .FormatAddExistingProjectToSolution(
                        dotNetSolutionModel.NamespacePath.AbsolutePath.FormattedInput,
                        afp.FormattedInput);

                var addExistingProjectToSolutionTerminalCommand = new TerminalCommand(
                    Key<TerminalCommand>.NewKey(),
                    localFormattedAddExistingProjectToSolutionCommand,
                    null,
                    CancellationToken.None,
                    () =>
                    {
                        Dispatcher.Dispatch(new DotNetSolutionState.SetDotNetSolutionTask(
                            dotNetSolutionModel.NamespacePath.AbsolutePath,
                            DotNetSolutionSync));

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
            DotNetSolutionState.TreeViewSolutionExplorerStateKey,
            treeViewModel);

        TreeViewService.MoveUp(
            DotNetSolutionState.TreeViewSolutionExplorerStateKey,
            false);
    }

    public static string GetContextMenuCssStyleString(
        TreeViewCommandParameter? treeViewCommandParameter)
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