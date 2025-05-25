using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Namespaces.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.CompilerServices.DotNetSolution.Models;
using Luthetus.Ide.RazorLib.InputFiles.Models;
using Luthetus.Ide.RazorLib.Menus.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.FormsGenerics.Displays;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.StartupControls.Models;
using Luthetus.Extensions.DotNet.CSharpProjects.Displays;
using Luthetus.Extensions.DotNet.Menus.Models;
using Luthetus.Extensions.DotNet.CSharpProjects.Models;
using Luthetus.Extensions.DotNet.BackgroundTasks.Models;
using Luthetus.Extensions.DotNet.DotNetSolutions.Models;
using Luthetus.Extensions.DotNet.CommandLines.Models;
using Luthetus.Extensions.DotNet.Namespaces.Models;

namespace Luthetus.Extensions.DotNet.DotNetSolutions.Displays.Internals;

public partial class SolutionExplorerContextMenu : ComponentBase
{
	[Inject]
	private ITerminalService TerminalService { get; set; } = null!;
	[Inject]
	private IStartupControlService StartupControlService { get; set; } = null!;
	[Inject]
	private IMenuOptionsFactory MenuOptionsFactory { get; set; } = null!;
	[Inject]
	private IDotNetMenuOptionsFactory DotNetMenuOptionsFactory { get; set; } = null!;
	[Inject]
	private ICommonComponentRenderers CommonComponentRenderers { get; set; } = null!;
	[Inject]
	private IIdeComponentRenderers IdeComponentRenderers { get; set; } = null!;
	[Inject]
	private ITreeViewService TreeViewService { get; set; } = null!;
	[Inject]
	private DotNetBackgroundTaskApi CompilerServicesBackgroundTaskApi { get; set; } = null!;
	[Inject]
	private IdeBackgroundTaskApi IdeBackgroundTaskApi { get; set; } = null!;
	[Inject]
	private IEnvironmentProvider EnvironmentProvider { get; set; } = null!;
	[Inject]
	private IDialogService DialogService { get; set; } = null!;
    [Inject]
    private INotificationService NotificationService { get; set; } = null!;
	[Inject]
	private TextEditorService TextEditorService { get; set; } = null!;
	[Inject]
	private DotNetBackgroundTaskApi DotNetBackgroundTaskApi { get; set; } = null!;

	[Parameter, EditorRequired]
	public TreeViewCommandArgs TreeViewCommandArgs { get; set; }

	private static readonly Key<IDynamicViewModel> _solutionEditorDialogKey = Key<IDynamicViewModel>.NewKey();
	private static readonly Key<IDynamicViewModel> _solutionPropertiesDialogKey = Key<IDynamicViewModel>.NewKey();
	private static readonly Key<IDynamicViewModel> _newCSharpProjectDialogKey = Key<IDynamicViewModel>.NewKey();

	public static readonly Key<DropdownRecord> ContextMenuEventDropdownKey = Key<DropdownRecord>.NewKey();

	/// <summary>
	/// The program is currently running using Photino locally on the user's computer
	/// therefore this static solution works without leaking any information.
	/// </summary>
	public static TreeViewNoType? ParentOfCutFile;

	private (TreeViewCommandArgs treeViewCommandArgs, MenuRecord menuRecord) _previousGetMenuRecordInvocation;

	private MenuRecord GetMenuRecord(TreeViewCommandArgs commandArgs)
	{
		if (_previousGetMenuRecordInvocation.treeViewCommandArgs == commandArgs)
			return _previousGetMenuRecordInvocation.menuRecord;

		if (commandArgs.TreeViewContainer.SelectedNodeList.Count > 1)
			return GetMenuRecordManySelections(commandArgs);

		if (commandArgs.TreeViewContainer.ActiveNode is null)
		{
			var menuRecord = new MenuRecord(MenuRecord.NoMenuOptionsExistList);
			_previousGetMenuRecordInvocation = (commandArgs, menuRecord);
			return menuRecord;
		}

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
		{
			var menuRecord = new MenuRecord(MenuRecord.NoMenuOptionsExistList);
			_previousGetMenuRecordInvocation = (commandArgs, menuRecord);
			return menuRecord;
		}

		// Default case
		{
			var menuRecord = new MenuRecord(menuOptionList);
			_previousGetMenuRecordInvocation = (commandArgs, menuRecord);
			return menuRecord;
		}
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
					filenameList.Add(treeViewNamespacePath.Item.AbsolutePath.NameWithExtension + " __FROM__ " + (treeViewNamespacePath.Item.AbsolutePath.ParentDirectory ?? "null"));
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
				widgetRendererType: IdeComponentRenderers.BooleanPromptOrCancelRendererType,
				widgetParameterMap: new Dictionary<string, object?>
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
							await commandArgs.RestoreFocusToTreeView
								.Invoke()
								.ConfigureAwait(false);

							DotNetBackgroundTaskApi.Enqueue_SolutionExplorer_TreeView_MultiSelect_DeleteFiles(
                                commandArgs);
						}
					},
					{ nameof(IBooleanPromptOrCancelRendererType.OnAfterDeclineFunc), commandArgs.RestoreFocusToTreeView },
					{ nameof(IBooleanPromptOrCancelRendererType.OnAfterCancelFunc), commandArgs.RestoreFocusToTreeView },
				}));
		}

		if (!menuOptionList.Any())
		{
			var menuRecord = new MenuRecord(MenuRecord.NoMenuOptionsExistList);
			_previousGetMenuRecordInvocation = (commandArgs, menuRecord);
			return menuRecord;
		}

		// Default case
		{
			var menuRecord = new MenuRecord(menuOptionList);
			_previousGetMenuRecordInvocation = (commandArgs, menuRecord);
			return menuRecord;
		}
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
			() =>
			{
				AddExistingProjectToSolution(treeViewSolution.Item);
				return Task.CompletedTask;
			});

		var createOptions = new MenuOptionRecord("Add", MenuOptionKind.Create,
			subMenu: new MenuRecord(new List<MenuOptionRecord>
			{
				addNewCSharpProject,
				addExistingCSharpProject,
			}));

		var openInTextEditor = new MenuOptionRecord(
			"Open in text editor",
			MenuOptionKind.Update,
			() => OpenSolutionInTextEditor(treeViewSolution.Item));
			
		var properties = new MenuOptionRecord(
			"Properties",
			MenuOptionKind.Update,
			() => OpenSolutionProperties(treeViewSolution.Item));

		return new[]
		{
			createOptions,
			openInTextEditor,
			properties,
		};
	}

	private MenuOptionRecord[] GetCSharpProjectMenuOptions(TreeViewNamespacePath treeViewModel)
	{
		var parentDirectory = treeViewModel.Item.AbsolutePath.ParentDirectory;
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

		var parentDirectoryAbsolutePath = EnvironmentProvider.AbsolutePathFactory(parentDirectory, true);

		return new[]
		{
			MenuOptionsFactory.NewEmptyFile(
				parentDirectoryAbsolutePath,
				async () => await ReloadTreeViewModel(treeViewModel).ConfigureAwait(false)),
			MenuOptionsFactory.NewTemplatedFile(
				new NamespacePath(treeViewModel.Item.Namespace, parentDirectoryAbsolutePath),
				async () => await ReloadTreeViewModel(treeViewModel).ConfigureAwait(false)),
			MenuOptionsFactory.NewDirectory(
				parentDirectoryAbsolutePath,
				async () => await ReloadTreeViewModel(treeViewModel).ConfigureAwait(false)),
			MenuOptionsFactory.PasteClipboard(
				parentDirectoryAbsolutePath,
				async () =>
				{
					var localParentOfCutFile = ParentOfCutFile;
					ParentOfCutFile = null;

					if (localParentOfCutFile is not null)
						await ReloadTreeViewModel(localParentOfCutFile).ConfigureAwait(false);

					await ReloadTreeViewModel(treeViewModel).ConfigureAwait(false);
				}),
			DotNetMenuOptionsFactory.AddProjectToProjectReference(
				treeViewModel,
				TerminalService.GetTerminalState().TerminalMap[TerminalFacts.GENERAL_KEY],
				NotificationService,
				IdeBackgroundTaskApi,
				() => Task.CompletedTask),
			DotNetMenuOptionsFactory.MoveProjectToSolutionFolder(
				treeViewSolution,
				treeViewModel,
				TerminalService.GetTerminalState().TerminalMap[TerminalFacts.GENERAL_KEY],
				NotificationService,
				() =>
				{
					CompilerServicesBackgroundTaskApi.DotNetSolution.Enqueue_SetDotNetSolution(treeViewSolution.Item.NamespacePath.AbsolutePath);
					return Task.CompletedTask;
				}),
			new MenuOptionRecord(
				"Set as Startup Project",
				MenuOptionKind.Other,
				() =>
				{
					var startupControl = StartupControlService.GetStartupControlState().StartupControlList.FirstOrDefault(
						x => x.StartupProjectAbsolutePath.Value == treeViewModel.Item.AbsolutePath.Value);
						
					if (startupControl is null)
						return Task.CompletedTask;
					
					StartupControlService.SetActiveStartupControlKey(startupControl.Key);
					return Task.CompletedTask;
				}),
			DotNetMenuOptionsFactory.RemoveCSharpProjectReferenceFromSolution(
				treeViewSolution,
				treeViewModel,
				TerminalService.GetTerminalState().TerminalMap[TerminalFacts.GENERAL_KEY],
				NotificationService,
				() =>
				{
					CompilerServicesBackgroundTaskApi.DotNetSolution.Enqueue_SetDotNetSolution(treeViewSolution.Item.NamespacePath.AbsolutePath);
					return Task.CompletedTask;
				}),
		};
	}

	private MenuOptionRecord[] GetCSharpProjectToProjectReferenceMenuOptions(
		TreeViewCSharpProjectToProjectReference treeViewCSharpProjectToProjectReference)
	{
		return new[]
		{
			DotNetMenuOptionsFactory.RemoveProjectToProjectReference(
				treeViewCSharpProjectToProjectReference,
				TerminalService.GetTerminalState().TerminalMap[TerminalFacts.GENERAL_KEY],
				NotificationService,
				() => Task.CompletedTask),
		};
	}

	private IReadOnlyList<MenuOptionRecord> GetTreeViewLightWeightNugetPackageRecordMenuOptions(
		TreeViewCSharpProjectNugetPackageReference treeViewCSharpProjectNugetPackageReference)
	{
		if (treeViewCSharpProjectNugetPackageReference.Parent
				is not TreeViewCSharpProjectNugetPackageReferences treeViewCSharpProjectNugetPackageReferences)
		{
			return MenuRecord.NoMenuOptionsExistList;
		}

		return new List<MenuOptionRecord>
		{
			DotNetMenuOptionsFactory.RemoveNuGetPackageReferenceFromProject(
				treeViewCSharpProjectNugetPackageReferences.Item.CSharpProjectNamespacePath,
				treeViewCSharpProjectNugetPackageReference,
				TerminalService.GetTerminalState().TerminalMap[TerminalFacts.GENERAL_KEY],
				NotificationService,
				() => Task.CompletedTask),
		};
	}

	private MenuOptionRecord[] GetDirectoryMenuOptions(TreeViewNamespacePath treeViewModel)
	{
		return new[]
		{
			MenuOptionsFactory.NewEmptyFile(
				treeViewModel.Item.AbsolutePath,
				async () => await ReloadTreeViewModel(treeViewModel).ConfigureAwait(false)),
			MenuOptionsFactory.NewTemplatedFile(
				treeViewModel.Item,
				async () => await ReloadTreeViewModel(treeViewModel).ConfigureAwait(false)),
			MenuOptionsFactory.NewDirectory(
				treeViewModel.Item.AbsolutePath,
				async () => await ReloadTreeViewModel(treeViewModel).ConfigureAwait(false)),
			MenuOptionsFactory.PasteClipboard(
				treeViewModel.Item.AbsolutePath,
				async () =>
				{
					var localParentOfCutFile = ParentOfCutFile;
					ParentOfCutFile = null;

					if (localParentOfCutFile is not null)
						await ReloadTreeViewModel(localParentOfCutFile).ConfigureAwait(false);

					await ReloadTreeViewModel(treeViewModel).ConfigureAwait(false);
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
				treeViewModel.Item.AbsolutePath,
				() => {
					NotificationHelper.DispatchInformative("Copy Action", $"Copied: {treeViewModel.Item.AbsolutePath.NameWithExtension}", CommonComponentRenderers, NotificationService, TimeSpan.FromSeconds(7));
					return Task.CompletedTask;
				}),
			MenuOptionsFactory.CutFile(
				treeViewModel.Item.AbsolutePath,
				() => {
					ParentOfCutFile = parentTreeViewModel;
					NotificationHelper.DispatchInformative("Cut Action", $"Cut: {treeViewModel.Item.AbsolutePath.NameWithExtension}", CommonComponentRenderers, NotificationService, TimeSpan.FromSeconds(7));
					return Task.CompletedTask;
				}),
			MenuOptionsFactory.DeleteFile(
				treeViewModel.Item.AbsolutePath,
				async () => await ReloadTreeViewModel(parentTreeViewModel).ConfigureAwait(false)),
			MenuOptionsFactory.RenameFile(
				treeViewModel.Item.AbsolutePath,
				NotificationService,
				async ()  => await ReloadTreeViewModel(parentTreeViewModel).ConfigureAwait(false)),
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

	private Task OpenNewCSharpProjectDialog(DotNetSolutionModel dotNetSolutionModel)
	{
		var dialogRecord = new DialogViewModel(
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
			true,
			null);

		DialogService.ReduceRegisterAction(dialogRecord);
		return Task.CompletedTask;
	}

	private void AddExistingProjectToSolution(DotNetSolutionModel dotNetSolutionModel)
	{
		IdeBackgroundTaskApi.InputFile.Enqueue_RequestInputFileStateForm(
			"Existing C# Project to add to solution",
			absolutePath =>
			{
				if (absolutePath.ExactInput is null)
					return Task.CompletedTask;

				var localFormattedAddExistingProjectToSolutionCommand = DotNetCliCommandFormatter.FormatAddExistingProjectToSolution(
					dotNetSolutionModel.NamespacePath.AbsolutePath.Value,
					absolutePath.Value);

				var terminalCommandRequest = new TerminalCommandRequest(
		        	localFormattedAddExistingProjectToSolutionCommand.Value,
		        	null)
		        {
		        	ContinueWithFunc = parsedCommand =>
		        	{
		        		CompilerServicesBackgroundTaskApi.DotNetSolution.Enqueue_SetDotNetSolution(dotNetSolutionModel.NamespacePath.AbsolutePath);
						return Task.CompletedTask;
		        	}
		        };
		        	
		        TerminalService.GetTerminalState().TerminalMap[TerminalFacts.GENERAL_KEY].EnqueueCommand(terminalCommandRequest);
				return Task.CompletedTask;
			},
			absolutePath =>
			{
				if (absolutePath.ExactInput is null || absolutePath.IsDirectory)
					return Task.FromResult(false);

				return Task.FromResult(absolutePath.ExtensionNoPeriod.EndsWith(ExtensionNoPeriodFacts.C_SHARP_PROJECT));
			},
			new()
			{
				new InputFilePattern(
					"C# Project",
					absolutePath => absolutePath.ExtensionNoPeriod.EndsWith(ExtensionNoPeriodFacts.C_SHARP_PROJECT))
			});
	}

	private Task OpenSolutionInTextEditor(DotNetSolutionModel dotNetSolutionModel)
	{
		TextEditorService.WorkerArbitrary.PostUnique(nameof(SolutionExplorerContextMenu), async editContext =>
		{
			await TextEditorService.OpenInEditorAsync(
				editContext,
				dotNetSolutionModel.AbsolutePath.Value,
				true,
				null,
				new Category("main"),
				Key<TextEditorViewModel>.NewKey());
		});
		return Task.CompletedTask;
	}
	
	private Task OpenSolutionProperties(DotNetSolutionModel dotNetSolutionModel)
	{
		DialogService.ReduceRegisterAction(new DialogViewModel(
			dynamicViewModelKey: _solutionPropertiesDialogKey,
			title: "Solution Properties",
			componentType: typeof(SolutionPropertiesDisplay),
			componentParameterMap: null,
			cssClass: null,
			isResizable: true,
			setFocusOnCloseElementId: null));
		return Task.CompletedTask;
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

		await treeViewModel.LoadChildListAsync().ConfigureAwait(false);

		TreeViewService.ReduceReRenderNodeAction(DotNetSolutionState.TreeViewSolutionExplorerStateKey, treeViewModel);

		TreeViewService.ReduceMoveUpAction(
			DotNetSolutionState.TreeViewSolutionExplorerStateKey,
			false,
			false);
	}
}