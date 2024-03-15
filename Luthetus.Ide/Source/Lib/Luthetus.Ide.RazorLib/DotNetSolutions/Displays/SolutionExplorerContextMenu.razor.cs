using Fluxor;
using Microsoft.AspNetCore.Components;
using System.Collections.Immutable;
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
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.CompilerServices.Lang.DotNetSolution.Models;
using Luthetus.Ide.RazorLib.DotNetSolutions.States;
using Luthetus.Ide.RazorLib.Terminals.States;
using Luthetus.Ide.RazorLib.ProgramExecutions.States;
using Luthetus.Ide.RazorLib.InputFiles.States;
using Luthetus.Ide.RazorLib.InputFiles.Models;
using Luthetus.Ide.RazorLib.CSharpProjectForms.Displays;
using Luthetus.Ide.RazorLib.TreeViewImplementations.Models;
using Luthetus.Ide.RazorLib.Menus.Models;
using Luthetus.Ide.RazorLib.CommandLines.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.FormsGenerics.Displays;

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
    private ILuthetusIdeComponentRenderers IdeComponentRenderers { get; set; } = null!;
    [Inject]
    private ITreeViewService TreeViewService { get; set; } = null!;
    [Inject]
    private DotNetSolutionSync DotNetSolutionSync { get; set; } = null!;
    [Inject]
    private InputFileSync InputFileSync { get; set; } = null!;
    [Inject]
    private IEnvironmentProvider EnvironmentProvider { get; set; } = null!;
    [Inject]
    private IFileSystemProvider FileSystemProvider { get; set; } = null!;
    [Inject]
    private IBackgroundTaskService BackgroundTaskService { get; set; } = null!;

    [Parameter, EditorRequired]
    public TreeViewCommandArgs TreeViewCommandArgs { get; set; } = null!;

	private static readonly Key<IPolymorphicUiRecord> _solutionEditorDialogKey = Key<IPolymorphicUiRecord>.NewKey();
	private static readonly Key<IPolymorphicUiRecord> _newCSharpProjectDialogKey = Key<IPolymorphicUiRecord>.NewKey();

    public static readonly Key<DropdownRecord> ContextMenuEventDropdownKey = Key<DropdownRecord>.NewKey();

    /// <summary>
    /// The program is currently running using Photino locally on the user's computer
    /// therefore this static solution works without leaking any information.
    /// </summary>
    public static TreeViewNoType? ParentOfCutFile;

    private MenuRecord GetMenuRecord(TreeViewCommandArgs commandArgs)
    {
        if (commandArgs.TreeViewContainer.SelectedNodeList.Count > 1)
            return GetMenuRecordManySelections(commandArgs);

        if (commandArgs.TreeViewContainer.ActiveNode is null)
            return MenuRecord.Empty;

        var menuOptionList = new List<MenuOptionRecord>();
        var treeViewModel = commandArgs.TreeViewContainer.ActiveNode;
        var parentTreeViewModel = treeViewModel.Parent;
        var parentTreeViewNamespacePath = parentTreeViewModel as TreeViewNamespacePath;

        if (treeViewModel is TreeViewNamespacePath treeViewNamespacePath)
        {
            if (treeViewNamespacePath.Item.AbsolutePath.IsDirectory)
            {
                menuOptionList.AddRange(GetFileMenuOptions(treeViewNamespacePath, parentTreeViewNamespacePath)
                    .Union(GetDirectoryMenuOptions(treeViewNamespacePath))
                    .Union(GetDebugMenuOptions(treeViewNamespacePath)));
            }
            else
            {
                switch (treeViewNamespacePath.Item.AbsolutePath.ExtensionNoPeriod)
                {
                    case ExtensionNoPeriodFacts.C_SHARP_PROJECT:
                        menuOptionList.AddRange(GetCSharpProjectMenuOptions(treeViewNamespacePath)
                            .Union(GetDebugMenuOptions(treeViewNamespacePath)));
                        break;
                    default:
                        menuOptionList.AddRange(GetFileMenuOptions(treeViewNamespacePath, parentTreeViewNamespacePath)
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
                    menuOptionList.AddRange(GetDotNetSolutionMenuOptions(treeViewSolution));
            }
        }
        else if (treeViewModel is TreeViewCSharpProjectToProjectReference treeViewCSharpProjectToProjectReference)
        {
            menuOptionList.AddRange(GetCSharpProjectToProjectReferenceMenuOptions(
                treeViewCSharpProjectToProjectReference));
        }
        else if (treeViewModel is TreeViewCSharpProjectNugetPackageReference treeViewCSharpProjectNugetPackageReference)
        {
            menuOptionList.AddRange(GetTreeViewLightWeightNugetPackageRecordMenuOptions(
                treeViewCSharpProjectNugetPackageReference));
        }

        if (!menuOptionList.Any())
            return MenuRecord.Empty;

        return new MenuRecord(menuOptionList.ToImmutableArray());
    }

    private MenuRecord GetMenuRecordManySelections(TreeViewCommandArgs commandArgs)
    {
        var menuOptionList = new List<MenuOptionRecord>();

        var getFileOptions = true;
        var filenameList = new List<string>();

        foreach (var selectedNode in commandArgs.TreeViewContainer.SelectedNodeList)
        {
            if (selectedNode is TreeViewNamespacePath treeViewNamespacePath)
            {
                if (treeViewNamespacePath.Item.AbsolutePath.ExtensionNoPeriod == ExtensionNoPeriodFacts.C_SHARP_PROJECT)
                    getFileOptions = false;
                else if (getFileOptions)
                    filenameList.Add(treeViewNamespacePath.Item.AbsolutePath.NameWithExtension + " __FROM__ " + (treeViewNamespacePath.Item.AbsolutePath.ParentDirectory?.Value ?? "null"));
            }
            else
            {
                getFileOptions = false;
            }
        }

        if (getFileOptions)
        {
            menuOptionList.Add(new MenuOptionRecord(
                "Delete",
                MenuOptionKind.Delete,
                WidgetRendererType: IdeComponentRenderers.BooleanPromptOrCancelRendererType,
                WidgetParameterMap: new Dictionary<string, object?>
                {
                    { nameof(IBooleanPromptOrCancelRendererType.IncludeCancelOption), false },
                    { nameof(IBooleanPromptOrCancelRendererType.Message), $"DELETE:" },
                    { nameof(BooleanPromptOrCancelDisplay.ListOfMessages), filenameList },
                    { nameof(IBooleanPromptOrCancelRendererType.AcceptOptionTextOverride), null },
                    { nameof(IBooleanPromptOrCancelRendererType.DeclineOptionTextOverride), null },
                    { 
                        nameof(IBooleanPromptOrCancelRendererType.OnAfterAcceptFunc),
                        async () =>
                        {
                            await commandArgs.RestoreFocusToTreeView.Invoke();
                            BackgroundTaskService.Enqueue(
                                Key<BackgroundTask>.NewKey(),
                                ContinuousBackgroundTaskWorker.GetQueueKey(),
                                "SolutionExplorer_TreeView_MultiSelect_DeleteFiles",
                                async () =>
                                {
                                    foreach (var node in commandArgs.TreeViewContainer.SelectedNodeList)
                                    {
                                        var treeViewNamespacePath = (TreeViewNamespacePath)node;

                                        if (treeViewNamespacePath.Item.AbsolutePath.IsDirectory)
                                            await FileSystemProvider.Directory.DeleteAsync(treeViewNamespacePath.Item.AbsolutePath.Value, true, CancellationToken.None);
                                        else
                                            await FileSystemProvider.File.DeleteAsync(treeViewNamespacePath.Item.AbsolutePath.Value);

                                        if (TreeViewService.TryGetTreeViewContainer(commandArgs.TreeViewContainer.Key, out var mostRecentContainer) &&
                                            mostRecentContainer is not null)
                                        {
                                            var localParent = node.Parent;

                                            if (localParent is not null)
                                            {
                                                await localParent.LoadChildListAsync();
                                                TreeViewService.ReRenderNode(mostRecentContainer.Key, localParent);
                                            }
                                        }
                                    }
                                });
                        }
                    },
                    { nameof(IBooleanPromptOrCancelRendererType.OnAfterDeclineFunc), commandArgs.RestoreFocusToTreeView },
                    { nameof(IBooleanPromptOrCancelRendererType.OnAfterCancelFunc), commandArgs.RestoreFocusToTreeView },
                }));
        }

        if (!menuOptionList.Any())
            return MenuRecord.Empty;

        return new MenuRecord(menuOptionList.ToImmutableArray());
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
            () => OpenSolutionEditorDialog(treeViewSolution.Item));

        return new[]
        {
            createOptions,
            openSolutionEditor,
        };
    }

    private MenuOptionRecord[] GetCSharpProjectMenuOptions(TreeViewNamespacePath treeViewModel)
    {
        var parentDirectory = treeViewModel.Item.AbsolutePath.AncestorDirectoryList.Last();
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

        var parentDirectoryAbsolutePath = EnvironmentProvider.AbsolutePathFactory(parentDirectory.Value, true);

        return new[]
        {
            MenuOptionsFactory.NewEmptyFile(parentDirectoryAbsolutePath, async () => await ReloadTreeViewModel(treeViewModel)),
            MenuOptionsFactory.NewTemplatedFile(new NamespacePath(treeViewModel.Item.Namespace, parentDirectoryAbsolutePath), async () => await ReloadTreeViewModel(treeViewModel)),
            MenuOptionsFactory.NewDirectory(parentDirectoryAbsolutePath, async () => await ReloadTreeViewModel(treeViewModel)),
            MenuOptionsFactory.PasteClipboard(parentDirectoryAbsolutePath, async () =>
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
        if (treeViewCSharpProjectNugetPackageReference.Parent 
                is not TreeViewCSharpProjectNugetPackageReferences treeViewCSharpProjectNugetPackageReferences)
        {
            return MenuRecord.Empty.MenuOptionList.ToArray();
        }
        
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
                NotificationHelper.DispatchInformative("Copy Action", $"Copied: {treeViewModel.Item.AbsolutePath.NameWithExtension}", CommonComponentRenderers, Dispatcher, TimeSpan.FromSeconds(7));
                return Task.CompletedTask;
            }),
            MenuOptionsFactory.CutFile(treeViewModel.Item.AbsolutePath, () => {
                ParentOfCutFile = parentTreeViewModel;
                NotificationHelper.DispatchInformative("Cut Action", $"Cut: {treeViewModel.Item.AbsolutePath.NameWithExtension}", CommonComponentRenderers, Dispatcher, TimeSpan.FromSeconds(7));
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
            _newCSharpProjectDialogKey,
            "New C# Project",
            typeof(CSharpProjectFormDisplay),
            new Dictionary<string, object?>
            {
                {
                    nameof(CSharpProjectFormDisplay.DotNetSolutionModelKey),
                    dotNetSolutionModel.Key
                },
            },
            null,
			true);

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
                        dotNetSolutionModel.NamespacePath.AbsolutePath.Value,
                        afp.Value);

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

    private void OpenSolutionEditorDialog(DotNetSolutionModel dotNetSolutionModel)
    {
        var dialogRecord = new DialogRecord(
            _solutionEditorDialogKey,
            "Solution Editor",
            typeof(SolutionEditorDisplay),
            new Dictionary<string, object?>
            {
                {
                    nameof(SolutionEditorDisplay.DotNetSolutionModelKey),
                    dotNetSolutionModel.Key
                },
                {
                    nameof(SolutionEditorDisplay.DotNetSolutionResourceUri),
                    new ResourceUri(dotNetSolutionModel.NamespacePath.AbsolutePath.Value)
                },
            },
            null,
			true);

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

        await treeViewModel.LoadChildListAsync();

        TreeViewService.ReRenderNode(DotNetSolutionState.TreeViewSolutionExplorerStateKey, treeViewModel);
        
		TreeViewService.MoveUp(
			DotNetSolutionState.TreeViewSolutionExplorerStateKey,
			false,
			false);
    }

    public static string GetContextMenuCssStyleString(TreeViewCommandArgs? commandArgs)
    {
        if (commandArgs?.ContextMenuFixedPosition is null)
            return "display: none;";

        var left =
            $"left: {commandArgs.ContextMenuFixedPosition.LeftPositionInPixels.ToCssValue()}px;";

        var top =
            $"top: {commandArgs.ContextMenuFixedPosition.TopPositionInPixels.ToCssValue()}px;";

        return $"{left} {top}";
    }
}