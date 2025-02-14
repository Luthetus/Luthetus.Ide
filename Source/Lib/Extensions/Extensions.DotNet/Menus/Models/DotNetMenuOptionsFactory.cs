using System.Collections.Immutable;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.Namespaces.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.Ide.RazorLib.InputFiles.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.FileSystems.Models;
using Luthetus.Extensions.DotNet.CSharpProjects.Models;
using Luthetus.Extensions.DotNet.ComponentRenderers.Models;
using Luthetus.Extensions.DotNet.DotNetSolutions.Models;
using Luthetus.Extensions.DotNet.CommandLines.Models;
using Luthetus.Extensions.DotNet.Namespaces.Models;

namespace Luthetus.Extensions.DotNet.Menus.Models;

public class DotNetMenuOptionsFactory : IDotNetMenuOptionsFactory
{
	private readonly LuthetusCommonApi _commonApi;
	private readonly IDotNetComponentRenderers _dotNetComponentRenderers;
	private readonly IIdeComponentRenderers _ideComponentRenderers;

	public DotNetMenuOptionsFactory(
		LuthetusCommonApi commonApi,
		IDotNetComponentRenderers dotNetComponentRenderers,
		IIdeComponentRenderers ideComponentRenderers)
	{
		_commonApi = commonApi;
		_dotNetComponentRenderers = dotNetComponentRenderers;
		_ideComponentRenderers = ideComponentRenderers;
	}

	public MenuOptionRecord RemoveCSharpProjectReferenceFromSolution(
		TreeViewSolution treeViewSolution,
		TreeViewNamespacePath projectNode,
		ITerminal terminal,
		INotificationService notificationService,
		Func<Task> onAfterCompletion)
	{
		return new MenuOptionRecord("Remove (no files are deleted)", MenuOptionKind.Delete,
			widgetRendererType: _dotNetComponentRenderers.RemoveCSharpProjectFromSolutionRendererType,
			widgetParameterMap: new Dictionary<string, object?>
			{
				{
					nameof(IRemoveCSharpProjectFromSolutionRendererType.AbsolutePath),
					projectNode.Item.AbsolutePath
				},
				{
					nameof(IDeleteFileFormRendererType.OnAfterSubmitFunc),
					new Func<AbsolutePath, Task>(
						_ =>
						{
							PerformRemoveCSharpProjectReferenceFromSolution(
								treeViewSolution,
								projectNode,
								terminal,
								notificationService,
								onAfterCompletion);

							return Task.CompletedTask;
						})
				},
			});
	}

	public MenuOptionRecord AddProjectToProjectReference(
		TreeViewNamespacePath projectReceivingReference,
		ITerminal terminal,
		INotificationService notificationService,
		IdeBackgroundTaskApi ideBackgroundTaskApi,
		Func<Task> onAfterCompletion)
	{
		return new MenuOptionRecord("Add Project Reference", MenuOptionKind.Other,
			onClickFunc:
			() =>
			{
				PerformAddProjectToProjectReference(
					projectReceivingReference,
					terminal,
					notificationService,
					ideBackgroundTaskApi,
					onAfterCompletion);

				return Task.CompletedTask;
			});
	}

	public MenuOptionRecord RemoveProjectToProjectReference(
		TreeViewCSharpProjectToProjectReference treeViewCSharpProjectToProjectReference,
		ITerminal terminal,
		INotificationService notificationService,
		Func<Task> onAfterCompletion)
	{
		return new MenuOptionRecord("Remove Project Reference", MenuOptionKind.Other,
			onClickFunc:
				() =>
				{
					PerformRemoveProjectToProjectReference(
						treeViewCSharpProjectToProjectReference,
						terminal,
						notificationService,
						onAfterCompletion);

					return Task.CompletedTask;
				});
	}

	public MenuOptionRecord MoveProjectToSolutionFolder(
		TreeViewSolution treeViewSolution,
		TreeViewNamespacePath treeViewProjectToMove,
		ITerminal terminal,
		INotificationService notificationService,
		Func<Task> onAfterCompletion)
	{
		return new MenuOptionRecord("Move to Solution Folder", MenuOptionKind.Other,
			widgetRendererType: _ideComponentRenderers.FileFormRendererType,
			widgetParameterMap: new Dictionary<string, object?>
			{
				{ nameof(IFileFormRendererType.FileName), string.Empty },
				{ nameof(IFileFormRendererType.IsDirectory), false },
				{
					nameof(IFileFormRendererType.OnAfterSubmitFunc),
					new Func<string, IFileTemplate?, ImmutableArray<IFileTemplate>, Task>((nextName, _, _) =>
					{
						PerformMoveProjectToSolutionFolder(
							treeViewSolution,
							treeViewProjectToMove,
							nextName,
							terminal,
							notificationService,
							onAfterCompletion);

						return Task.CompletedTask;
					})
				},
			});
	}

	public MenuOptionRecord RemoveNuGetPackageReferenceFromProject(
		NamespacePath modifyProjectNamespacePath,
		TreeViewCSharpProjectNugetPackageReference treeViewCSharpProjectNugetPackageReference,
		ITerminal terminal,
		INotificationService notificationService,
		Func<Task> onAfterCompletion)
	{
		return new MenuOptionRecord("Remove NuGet Package Reference", MenuOptionKind.Other,
			onClickFunc: () =>
			{
				PerformRemoveNuGetPackageReferenceFromProject(
					modifyProjectNamespacePath,
					treeViewCSharpProjectNugetPackageReference,
					terminal,
					notificationService,
					onAfterCompletion);

				return Task.CompletedTask;
			});
	}

	private void PerformRemoveCSharpProjectReferenceFromSolution(
		TreeViewSolution treeViewSolution,
		TreeViewNamespacePath projectNode,
		ITerminal terminal,
		INotificationService notificationService,
		Func<Task> onAfterCompletion)
	{
        _commonApi.BackgroundTaskApi.Enqueue(
			Key<IBackgroundTask>.NewKey(),
			BackgroundTaskFacts.ContinuousQueueKey,
			"Remove C# Project Reference from Solution Action",
			() =>
			{
				var workingDirectory = treeViewSolution.Item.NamespacePath.AbsolutePath.ParentDirectory!;

				var formattedCommand = DotNetCliCommandFormatter.FormatRemoveCSharpProjectReferenceFromSolutionAction(
					treeViewSolution.Item.NamespacePath.AbsolutePath.Value,
					projectNode.Item.AbsolutePath.Value);

				var terminalCommandRequest = new TerminalCommandRequest(
					formattedCommand.Value,
					workingDirectory)
				{
					ContinueWithFunc = parsedCommand => onAfterCompletion.Invoke()
				};

				terminal.EnqueueCommand(terminalCommandRequest);
				return ValueTask.CompletedTask;
			});
	}

	public void PerformAddProjectToProjectReference(
		TreeViewNamespacePath projectReceivingReference,
		ITerminal terminal,
		INotificationService notificationService,
		IdeBackgroundTaskApi ideBackgroundTaskApi,
		Func<Task> onAfterCompletion)
	{
		ideBackgroundTaskApi.InputFile.RequestInputFileStateForm(
			$"Add Project reference to {projectReceivingReference.Item.AbsolutePath.NameWithExtension}",
			referencedProject =>
			{
				if (referencedProject.ExactInput is null)
					return Task.CompletedTask;

				var formattedCommand = DotNetCliCommandFormatter.FormatAddProjectToProjectReference(
					projectReceivingReference.Item.AbsolutePath.Value,
					referencedProject.Value);

				var terminalCommandRequest = new TerminalCommandRequest(
					formattedCommand.Value,
					null)
				{
					ContinueWithFunc = parsedCommand =>
					{
						NotificationHelper.DispatchInformative("Add Project Reference", $"Modified {projectReceivingReference.Item.AbsolutePath.NameWithExtension} to have a reference to {referencedProject.NameWithExtension}", _commonApi.ComponentRendererApi, notificationService, TimeSpan.FromSeconds(7));
						return onAfterCompletion.Invoke();
					}
				};

				terminal.EnqueueCommand(terminalCommandRequest);
				return Task.CompletedTask;
			},
			absolutePath =>
			{
				if (absolutePath.ExactInput is null || absolutePath.IsDirectory)
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

	public void PerformRemoveProjectToProjectReference(
		TreeViewCSharpProjectToProjectReference treeViewCSharpProjectToProjectReference,
		ITerminal terminal,
		INotificationService notificationService,
		Func<Task> onAfterCompletion)
	{
		_commonApi.BackgroundTaskApi.Enqueue(
			Key<IBackgroundTask>.NewKey(),
			BackgroundTaskFacts.ContinuousQueueKey,
			"Remove Project Reference to Project",
			() =>
			{
				var formattedCommand = DotNetCliCommandFormatter.FormatRemoveProjectToProjectReference(
					treeViewCSharpProjectToProjectReference.Item.ModifyProjectNamespacePath.AbsolutePath.Value,
					treeViewCSharpProjectToProjectReference.Item.ReferenceProjectAbsolutePath.Value);

				var terminalCommandRequest = new TerminalCommandRequest(
					formattedCommand.Value,
					null)
				{
					ContinueWithFunc = parsedCommand =>
					{
						NotificationHelper.DispatchInformative("Remove Project Reference", $"Modified {treeViewCSharpProjectToProjectReference.Item.ModifyProjectNamespacePath.AbsolutePath.NameWithExtension} to have a reference to {treeViewCSharpProjectToProjectReference.Item.ReferenceProjectAbsolutePath.NameWithExtension}", _commonApi.ComponentRendererApi, notificationService, TimeSpan.FromSeconds(7));
						return onAfterCompletion.Invoke();
					}
				};

				terminal.EnqueueCommand(terminalCommandRequest);
				return ValueTask.CompletedTask;
			});
	}

	public void PerformMoveProjectToSolutionFolder(
		TreeViewSolution treeViewSolution,
		TreeViewNamespacePath treeViewProjectToMove,
		string solutionFolderPath,
		ITerminal terminal,
		INotificationService notificationService,
		Func<Task> onAfterCompletion)
	{
		_commonApi.BackgroundTaskApi.Enqueue(
			Key<IBackgroundTask>.NewKey(),
			BackgroundTaskFacts.ContinuousQueueKey,
			"Move Project to Solution Folder",
			() =>
			{
				var formattedCommand = DotNetCliCommandFormatter.FormatMoveProjectToSolutionFolder(
					treeViewSolution.Item.NamespacePath.AbsolutePath.Value,
					treeViewProjectToMove.Item.AbsolutePath.Value,
					solutionFolderPath);

				var terminalCommandRequest = new TerminalCommandRequest(
					formattedCommand.Value,
					null)
				{
					ContinueWithFunc = parsedCommand =>
					{
						NotificationHelper.DispatchInformative("Move Project To Solution Folder", $"Moved {treeViewProjectToMove.Item.AbsolutePath.NameWithExtension} to the Solution Folder path: {solutionFolderPath}", _commonApi.ComponentRendererApi, notificationService, TimeSpan.FromSeconds(7));
						return onAfterCompletion.Invoke();
					}
				};

				PerformRemoveCSharpProjectReferenceFromSolution(
					treeViewSolution,
					treeViewProjectToMove,
					terminal,
					notificationService,
					() =>
					{
						terminal.EnqueueCommand(terminalCommandRequest);
						return Task.CompletedTask;
					});

				return ValueTask.CompletedTask;
			});
	}

	public void PerformRemoveNuGetPackageReferenceFromProject(
		NamespacePath modifyProjectNamespacePath,
		TreeViewCSharpProjectNugetPackageReference treeViewCSharpProjectNugetPackageReference,
		ITerminal terminal,
		INotificationService notificationService,
		Func<Task> onAfterCompletion)
	{
		_commonApi.BackgroundTaskApi.Enqueue(
			Key<IBackgroundTask>.NewKey(),
			BackgroundTaskFacts.ContinuousQueueKey,
			"Remove NuGet Package Reference from Project",
			() =>
			{
				var formattedCommand = DotNetCliCommandFormatter.FormatRemoveNugetPackageReferenceFromProject(
					modifyProjectNamespacePath.AbsolutePath.Value,
					treeViewCSharpProjectNugetPackageReference.Item.LightWeightNugetPackageRecord.Id);

				var terminalCommandRequest = new TerminalCommandRequest(
					formattedCommand.Value,
					null)
				{
					ContinueWithFunc = parsedCommand =>
					{
						NotificationHelper.DispatchInformative("Remove Project Reference", $"Modified {modifyProjectNamespacePath.AbsolutePath.NameWithExtension} to NOT have a reference to {treeViewCSharpProjectNugetPackageReference.Item.LightWeightNugetPackageRecord.Id}", _commonApi.ComponentRendererApi, notificationService, TimeSpan.FromSeconds(7));
						return onAfterCompletion.Invoke();
					}
				};

				terminal.EnqueueCommand(terminalCommandRequest);
				return ValueTask.CompletedTask;
			});
	}
}
