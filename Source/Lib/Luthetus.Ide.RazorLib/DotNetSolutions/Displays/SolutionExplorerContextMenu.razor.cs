using Fluxor;
using Microsoft.AspNetCore.Components;
using System.Collections.Immutable;
using Luthetus.Ide.RazorLib.DotNetSolutions.States;
using Luthetus.Ide.RazorLib.Terminals.States;
using Luthetus.Ide.RazorLib.ProgramExecutions.States;
using Luthetus.Ide.RazorLib.InputFiles.States;
using Luthetus.Ide.RazorLib.InputFiles.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Namespaces.Models;
using Luthetus.Common.RazorLib.Dialogs.States;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Ide.RazorLib.CSharpProjectForms.Displays;
using Luthetus.Ide.RazorLib.TreeViewImplementations.Models;
using Luthetus.Ide.RazorLib.Menus.Models;
using Luthetus.Ide.RazorLib.CommandLines.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.FileSystems.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.CompilerServices.Lang.DotNetSolution.Obsolete.RewriteForImmutability;

namespace Luthetus.Ide.RazorLib.DotNetSolutions.Displays;

public partial class SolutionExplorerContextMenu : ComponentBase
{
    [Inject]
    private IState<TerminalSessionState> TerminalSessionStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private IMenuOptionsFactory MenuOptionsFactory { get; set; } = null!;
    [Inject]
    private ILuthetusCommonComponentRenderers CommonComponentRenderers { get; set; } = null!;
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

    private MenuRecord GetMenuRecord(TreeViewCommandParameter commandParameter)
    {
        if (commandParameter.TargetNode is null)
            return MenuRecord.Empty;

        var menuRecordsBag = new List<MenuOptionRecord>();
        var treeViewModel = commandParameter.TargetNode;
        var parentTreeViewModel = treeViewModel.Parent;
        var parentTreeViewNamespacePath = parentTreeViewModel as TreeViewNamespacePath;

        if (treeViewModel is TreeViewNamespacePath treeViewNamespacePath)
        {
            if (treeViewNamespacePath.Item.AbsolutePath.IsDirectory)
            {
                menuRecordsBag.AddRange(GetFileMenuOptions(treeViewNamespacePath, parentTreeViewNamespacePath)
                    .Union(GetDirectoryMenuOptions(treeViewNamespacePath))
                    .Union(GetDebugMenuOptions(treeViewNamespacePath)));
            }
            else
            {
                switch (treeViewNamespacePath.Item.AbsolutePath.ExtensionNoPeriod)
                {
                    case ExtensionNoPeriodFacts.C_SHARP_PROJECT:
                        menuRecordsBag.AddRange(GetCSharpProjectMenuOptions(treeViewNamespacePath)
                            .Union(GetDebugMenuOptions(treeViewNamespacePath)));
                        break;
                    default:
                        menuRecordsBag.AddRange(GetFileMenuOptions(treeViewNamespacePath, parentTreeViewNamespacePath)
                            .Union(GetDebugMenuOptions(treeViewNamespacePath)));
                        break;
                }
            }
        }
        else if (treeViewModel is TreeViewSolution treeViewSolution)
        {
            if (ExtensionNoPeriodFacts.DOT_NET_SOLUTION == treeViewSolution.Item.NamespacePath.AbsolutePath.ExtensionNoPeriod)
            {
                if (treeViewSolution.Parent is null || treeViewSolution.Parent is TreeViewAdhoc)
                    menuRecordsBag.AddRange(GetDotNetSolutionMenuOptions(treeViewSolution));
            }
        }
        else if (treeViewModel is TreeViewCSharpProjectToProjectReference treeViewCSharpProjectToProjectReference)
        {
            menuRecordsBag.AddRange(GetCSharpProjectToProjectReferenceMenuOptions(
                treeViewCSharpProjectToProjectReference));
        }
        else if (treeViewModel is TreeViewCSharpProjectNugetPackageReference treeViewCSharpProjectNugetPackageReference)
        {
            menuRecordsBag.AddRange(GetTreeViewLightWeightNugetPackageRecordMenuOptions(
                treeViewCSharpProjectNugetPackageReference));
        }

        if (!menuRecordsBag.Any())
            return MenuRecord.Empty;

        return new MenuRecord(menuRecordsBag.ToImmutableArray());
    }

    private MenuOptionRecord[] GetDotNetSolutionMenuOptions(TreeViewSolution treeViewSolution)
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

        var createOptions = new MenuOptionRecord("Add", MenuOptionKind.Create,
            SubMenu: new MenuRecord(new[]
            {
                addNewCSharpProject,
                addExistingCSharpProject,
            }.ToImmutableArray()));

        var openSolutionEditor = new MenuOptionRecord(
            "Open Solution Editor",
            MenuOptionKind.Update,
            () => Aaa(treeViewSolution.Item));

        return new[]
        {
            createOptions,
            openSolutionEditor,
        };
    }

    private MenuOptionRecord[] GetCSharpProjectMenuOptions(TreeViewNamespacePath treeViewModel)
    {
        var parentDirectory = treeViewModel.Item.AbsolutePath.AncestorDirectoryBag.Last();
        var treeViewSolution = treeViewModel.Parent as TreeViewSolution;

        if (treeViewSolution is null)
        {
            var ancestorTreeView = treeViewModel.Parent;

            if (ancestorTreeView?.Parent is null)
                return Array.Empty<MenuOptionRecord>();

            // Parent could be a could be one or many levels of solution folders
            while (ancestorTreeView.Parent is not null)
            {
                ancestorTreeView = ancestorTreeView.Parent;
            }

            treeViewSolution = ancestorTreeView as TreeViewSolution;

            if (treeViewSolution is null)
                return Array.Empty<MenuOptionRecord>();
        }

        return new[]
        {
            MenuOptionsFactory.NewEmptyFile(parentDirectory, async () => await ReloadTreeViewModel(treeViewModel)),
            MenuOptionsFactory.NewTemplatedFile(new NamespacePath(treeViewModel.Item.Namespace, parentDirectory), async () => await ReloadTreeViewModel(treeViewModel)),
            MenuOptionsFactory.NewDirectory(parentDirectory, async () => await ReloadTreeViewModel(treeViewModel)),
            MenuOptionsFactory.PasteClipboard(parentDirectory, async () =>
            {
                var localParentOfCutFile = ParentOfCutFile;
                ParentOfCutFile = null;

                if (localParentOfCutFile is not null)
                    await ReloadTreeViewModel(localParentOfCutFile);

                await ReloadTreeViewModel(treeViewModel);
            }),
            MenuOptionsFactory.AddProjectToProjectReference(
                treeViewModel,
                TerminalSessionStateWrap.Value.TerminalSessionMap[TerminalSessionFacts.GENERAL_TERMINAL_SESSION_KEY],
                Dispatcher,
                InputFileSync,
                () => Task.CompletedTask),
            MenuOptionsFactory.MoveProjectToSolutionFolder(
                treeViewSolution,
                treeViewModel,
                TerminalSessionStateWrap.Value.TerminalSessionMap[TerminalSessionFacts.GENERAL_TERMINAL_SESSION_KEY],
                Dispatcher,
                () =>
                {
                    DotNetSolutionSync.SetDotNetSolution(treeViewSolution.Item.NamespacePath.AbsolutePath);
                    return Task.CompletedTask;
                }),
            new MenuOptionRecord(
                "Set as Startup Project",
                MenuOptionKind.Other,
                () => Dispatcher.Dispatch(new ProgramExecutionState.SetStartupProjectAbsolutePathAction(treeViewModel.Item.AbsolutePath))),
            MenuOptionsFactory.RemoveCSharpProjectReferenceFromSolution(
                treeViewSolution,
                treeViewModel,
                TerminalSessionStateWrap.Value.TerminalSessionMap[TerminalSessionFacts.GENERAL_TERMINAL_SESSION_KEY],
                Dispatcher,
                () =>
                {
                    DotNetSolutionSync.SetDotNetSolution(treeViewSolution.Item.NamespacePath.AbsolutePath);
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
                TerminalSessionStateWrap.Value.TerminalSessionMap[TerminalSessionFacts.GENERAL_TERMINAL_SESSION_KEY],
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
                TerminalSessionStateWrap.Value.TerminalSessionMap[TerminalSessionFacts.GENERAL_TERMINAL_SESSION_KEY],
                Dispatcher, () => Task.CompletedTask),
        };
    }

    private MenuOptionRecord[] GetDirectoryMenuOptions(TreeViewNamespacePath treeViewModel)
    {
        return new[]
        {
            MenuOptionsFactory.NewEmptyFile(treeViewModel.Item.AbsolutePath, async () => await ReloadTreeViewModel(treeViewModel)),
            MenuOptionsFactory.NewTemplatedFile(treeViewModel.Item, async () => await ReloadTreeViewModel(treeViewModel)),
            MenuOptionsFactory.NewDirectory(treeViewModel.Item.AbsolutePath, async () => await ReloadTreeViewModel(treeViewModel)),
            MenuOptionsFactory.PasteClipboard(treeViewModel.Item.AbsolutePath, async () =>
            {
                var localParentOfCutFile = ParentOfCutFile;
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
                NotificationHelper.DispatchInformative("Copy Action", $"Copied: {treeViewModel.Item.AbsolutePath.NameWithExtension}", CommonComponentRenderers, Dispatcher);
                return Task.CompletedTask;
            }),
            MenuOptionsFactory.CutFile(treeViewModel.Item.AbsolutePath, () => {
                ParentOfCutFile = parentTreeViewModel;
                NotificationHelper.DispatchInformative("Cut Action", $"Cut: {treeViewModel.Item.AbsolutePath.NameWithExtension}", CommonComponentRenderers, Dispatcher);
                return Task.CompletedTask;
            }),
            MenuOptionsFactory.DeleteFile(treeViewModel.Item.AbsolutePath, async () => await ReloadTreeViewModel(parentTreeViewModel)),
            MenuOptionsFactory.RenameFile(treeViewModel.Item.AbsolutePath, Dispatcher, async ()  => await ReloadTreeViewModel(parentTreeViewModel)),
        };
    }

    private MenuOptionRecord[] GetDebugMenuOptions(TreeViewNamespacePath treeViewModel)
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

        Dispatcher.Dispatch(new DialogState.RegisterAction(dialogRecord));
    }

    private void AddExistingProjectToSolution(DotNetSolutionModel dotNetSolutionModel)
    {
        InputFileSync.RequestInputFileStateForm("Existing C# Project to add to solution",
            async afp =>
            {
                if (afp is null)
                    return;

                var localFormattedAddExistingProjectToSolutionCommand = DotNetCliCommandFormatter.FormatAddExistingProjectToSolution(
                        dotNetSolutionModel.NamespacePath.AbsolutePath.FormattedInput,
                        afp.FormattedInput);

                var addExistingProjectToSolutionTerminalCommand = new TerminalCommand(
                    Key<TerminalCommand>.NewKey(),
                    localFormattedAddExistingProjectToSolutionCommand,
                    null,
                    CancellationToken.None,
                    () =>
                    {
                        DotNetSolutionSync.SetDotNetSolution(dotNetSolutionModel.NamespacePath.AbsolutePath);
                        return Task.CompletedTask;
                    });

                var generalTerminalSession = TerminalSessionStateWrap.Value.TerminalSessionMap[TerminalSessionFacts.GENERAL_TERMINAL_SESSION_KEY];
                await generalTerminalSession.EnqueueCommandAsync(addExistingProjectToSolutionTerminalCommand);
            },
            afp =>
            {
                if (afp is null || afp.IsDirectory)
                    return Task.FromResult(false);

                return Task.FromResult(afp.ExtensionNoPeriod.EndsWith(ExtensionNoPeriodFacts.C_SHARP_PROJECT));
            },
            new[]
            {
                new InputFilePattern(
                    "C# Project",
                    afp => afp.ExtensionNoPeriod.EndsWith(ExtensionNoPeriodFacts.C_SHARP_PROJECT))
            }.ToImmutableArray());
    }

    private void Aaa(DotNetSolutionModel dotNetSolutionModel)
    {
        var dialogRecord = new DialogRecord(
            Key<DialogRecord>.NewKey(),
            "Solution Editor",
            typeof(SolutionEditorDisplay),
            new Dictionary<string, object?>
            {
                {
                    nameof(SolutionEditorDisplay.DotNetSolutionModelKey),
                    dotNetSolutionModel.DotNetSolutionModelKey
                },
                {
                    nameof(SolutionEditorDisplay.DotNetSolutionResourceUri),
                    new ResourceUri(dotNetSolutionModel.NamespacePath.AbsolutePath.FormattedInput)
                },
            },
            null)
        {
            IsResizable = true
        };

        Dispatcher.Dispatch(new DialogState.RegisterAction(dialogRecord));
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
    private async Task ReloadTreeViewModel(TreeViewNoType? treeViewModel)
    {
        if (treeViewModel is null)
            return;

        await treeViewModel.LoadChildBagAsync();

        TreeViewService.ReRenderNode(DotNetSolutionState.TreeViewSolutionExplorerStateKey, treeViewModel);
        TreeViewService.MoveUp(DotNetSolutionState.TreeViewSolutionExplorerStateKey, false);
    }

    public static string GetContextMenuCssStyleString(TreeViewCommandParameter? commandParameter)
    {
        if (commandParameter?.ContextMenuFixedPosition is null)
            return "display: none;";

        var left =
            $"left: {commandParameter.ContextMenuFixedPosition.LeftPositionInPixels.ToCssValue()}px;";

        var top =
            $"top: {commandParameter.ContextMenuFixedPosition.TopPositionInPixels.ToCssValue()}px;";

        return $"{left} {top}";
    }
}