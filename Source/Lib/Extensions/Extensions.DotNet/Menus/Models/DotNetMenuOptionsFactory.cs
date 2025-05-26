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

public class DotNetMenuOptionsFactory : IDotNetMenuOptionsFactory, IBackgroundTaskGroup
{
	private readonly BackgroundTaskService _backgroundTaskService;
	private readonly IDotNetComponentRenderers _dotNetComponentRenderers;
	private readonly IIdeComponentRenderers _ideComponentRenderers;
	private readonly ICommonComponentRenderers _commonComponentRenderers;

	public DotNetMenuOptionsFactory(
		BackgroundTaskService backgroundTaskService,
		IDotNetComponentRenderers dotNetComponentRenderers,
		IIdeComponentRenderers ideComponentRenderers,
		ICommonComponentRenderers commonComponentRenderers)
	{
		_backgroundTaskService = backgroundTaskService;
		_dotNetComponentRenderers = dotNetComponentRenderers;
		_ideComponentRenderers = ideComponentRenderers;
		_commonComponentRenderers = commonComponentRenderers;
	}

    public Key<IBackgroundTaskGroup> BackgroundTaskKey { get; } = Key<IBackgroundTaskGroup>.NewKey();
    public string Name { get; } = nameof(DotNetMenuOptionsFactory);
    public bool EarlyBatchEnabled { get; } = false;

    public bool __TaskCompletionSourceWasCreated { get; set; }

    private readonly Queue<DotNetMenuOptionsFactoryWorkKind> _workKindQueue = new();

    private readonly object _workLock = new();

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
							Enqueue_PerformRemoveCSharpProjectReferenceFromSolution(
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
					Enqueue_PerformRemoveProjectToProjectReference(
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
					new Func<string, IFileTemplate?, List<IFileTemplate>, Task>((nextName, _, _) =>
					{
						Enqueue_PerformMoveProjectToSolutionFolder(
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
				Enqueue_PerformRemoveNuGetPackageReferenceFromProject(
					modifyProjectNamespacePath,
					treeViewCSharpProjectNugetPackageReference,
					terminal,
					notificationService,
					onAfterCompletion);

				return Task.CompletedTask;
			});
	}

	private readonly
		Queue<(TreeViewSolution treeViewSolution, TreeViewNamespacePath projectNode, ITerminal terminal, INotificationService notificationService, Func<Task> onAfterCompletion)>
		_queue_PerformRemoveCSharpProjectReferenceFromSolution = new();


    private void Enqueue_PerformRemoveCSharpProjectReferenceFromSolution(
		TreeViewSolution treeViewSolution,
		TreeViewNamespacePath projectNode,
		ITerminal terminal,
		INotificationService notificationService,
		Func<Task> onAfterCompletion)
	{
        lock (_workLock)
        {
            _workKindQueue.Enqueue(DotNetMenuOptionsFactoryWorkKind.PerformRemoveCSharpProjectReferenceFromSolution);

            _queue_PerformRemoveCSharpProjectReferenceFromSolution.Enqueue(
				(treeViewSolution, projectNode, terminal, notificationService, onAfterCompletion));

            _backgroundTaskService.Continuous_EnqueueGroup(this);
        }
	}
	
	private ValueTask Do_PerformRemoveCSharpProjectReferenceFromSolution(
		TreeViewSolution treeViewSolution,
		TreeViewNamespacePath projectNode,
		ITerminal terminal,
		INotificationService notificationService,
		Func<Task> onAfterCompletion)
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
    }

	public void PerformAddProjectToProjectReference(
		TreeViewNamespacePath projectReceivingReference,
		ITerminal terminal,
		INotificationService notificationService,
		IdeBackgroundTaskApi ideBackgroundTaskApi,
		Func<Task> onAfterCompletion)
	{
		ideBackgroundTaskApi.InputFile.Enqueue_RequestInputFileStateForm(
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
						NotificationHelper.DispatchInformative("Add Project Reference", $"Modified {projectReceivingReference.Item.AbsolutePath.NameWithExtension} to have a reference to {referencedProject.NameWithExtension}", _commonComponentRenderers, notificationService, TimeSpan.FromSeconds(7));
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
			new()
			{
				new InputFilePattern(
					"C# Project",
					absolutePath => absolutePath.ExtensionNoPeriod.EndsWith(ExtensionNoPeriodFacts.C_SHARP_PROJECT))
			});
	}

	private readonly
		Queue<(TreeViewCSharpProjectToProjectReference treeViewCSharpProjectToProjectReference, ITerminal terminal, INotificationService notificationService, Func<Task> onAfterCompletion)>
		_queue_PerformRemoveProjectToProjectReference = new();

    public void Enqueue_PerformRemoveProjectToProjectReference(
		TreeViewCSharpProjectToProjectReference treeViewCSharpProjectToProjectReference,
		ITerminal terminal,
		INotificationService notificationService,
		Func<Task> onAfterCompletion)
	{
        lock (_workLock)
        {
            _workKindQueue.Enqueue(DotNetMenuOptionsFactoryWorkKind.PerformRemoveProjectToProjectReference);

            _queue_PerformRemoveProjectToProjectReference.Enqueue(
				(treeViewCSharpProjectToProjectReference, terminal, notificationService, onAfterCompletion));

            _backgroundTaskService.Continuous_EnqueueGroup(this);
        }
	}
	
	public ValueTask Do_PerformRemoveProjectToProjectReference(
		TreeViewCSharpProjectToProjectReference treeViewCSharpProjectToProjectReference,
		ITerminal terminal,
		INotificationService notificationService,
		Func<Task> onAfterCompletion)
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
                NotificationHelper.DispatchInformative("Remove Project Reference", $"Modified {treeViewCSharpProjectToProjectReference.Item.ModifyProjectNamespacePath.AbsolutePath.NameWithExtension} to have a reference to {treeViewCSharpProjectToProjectReference.Item.ReferenceProjectAbsolutePath.NameWithExtension}", _commonComponentRenderers, notificationService, TimeSpan.FromSeconds(7));
                return onAfterCompletion.Invoke();
            }
        };

        terminal.EnqueueCommand(terminalCommandRequest);
        return ValueTask.CompletedTask;
    }

	private readonly
		Queue<(TreeViewSolution treeViewSolution, TreeViewNamespacePath treeViewProjectToMove, string solutionFolderPath, ITerminal terminal, INotificationService notificationService, Func<Task> onAfterCompletion)>
		_queue_PerformMoveProjectToSolutionFolder = new();

    public void Enqueue_PerformMoveProjectToSolutionFolder(
		TreeViewSolution treeViewSolution,
		TreeViewNamespacePath treeViewProjectToMove,
		string solutionFolderPath,
		ITerminal terminal,
		INotificationService notificationService,
		Func<Task> onAfterCompletion)
	{
        lock (_workLock)
        {
            _workKindQueue.Enqueue(DotNetMenuOptionsFactoryWorkKind.PerformMoveProjectToSolutionFolder);

            _queue_PerformMoveProjectToSolutionFolder.Enqueue(
				(treeViewSolution, treeViewProjectToMove, solutionFolderPath, terminal, notificationService, onAfterCompletion));

            _backgroundTaskService.Continuous_EnqueueGroup(this);
        }
	}
	
	public ValueTask Do_PerformMoveProjectToSolutionFolder(
		TreeViewSolution treeViewSolution,
		TreeViewNamespacePath treeViewProjectToMove,
		string solutionFolderPath,
		ITerminal terminal,
		INotificationService notificationService,
		Func<Task> onAfterCompletion)
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
                NotificationHelper.DispatchInformative("Move Project To Solution Folder", $"Moved {treeViewProjectToMove.Item.AbsolutePath.NameWithExtension} to the Solution Folder path: {solutionFolderPath}", _commonComponentRenderers, notificationService, TimeSpan.FromSeconds(7));
                return onAfterCompletion.Invoke();
            }
        };

        Enqueue_PerformRemoveCSharpProjectReferenceFromSolution(
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
    }

	private readonly
		Queue<(NamespacePath modifyProjectNamespacePath, TreeViewCSharpProjectNugetPackageReference treeViewCSharpProjectNugetPackageReference, ITerminal terminal, INotificationService notificationService, Func<Task> onAfterCompletion)>
		_queue_PerformRemoveNuGetPackageReferenceFromProject = new();

    public void Enqueue_PerformRemoveNuGetPackageReferenceFromProject(
		NamespacePath modifyProjectNamespacePath,
		TreeViewCSharpProjectNugetPackageReference treeViewCSharpProjectNugetPackageReference,
		ITerminal terminal,
		INotificationService notificationService,
		Func<Task> onAfterCompletion)
	{
        lock (_workLock)
        {
            _workKindQueue.Enqueue(DotNetMenuOptionsFactoryWorkKind.PerformRemoveNuGetPackageReferenceFromProject);

            _queue_PerformRemoveNuGetPackageReferenceFromProject.Enqueue(
				(modifyProjectNamespacePath, treeViewCSharpProjectNugetPackageReference, terminal, notificationService, onAfterCompletion));

            _backgroundTaskService.Continuous_EnqueueGroup(this);
        }
	}
	
	public ValueTask Do_PerformRemoveNuGetPackageReferenceFromProject(
		NamespacePath modifyProjectNamespacePath,
		TreeViewCSharpProjectNugetPackageReference treeViewCSharpProjectNugetPackageReference,
		ITerminal terminal,
		INotificationService notificationService,
		Func<Task> onAfterCompletion)
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
                NotificationHelper.DispatchInformative("Remove Project Reference", $"Modified {modifyProjectNamespacePath.AbsolutePath.NameWithExtension} to NOT have a reference to {treeViewCSharpProjectNugetPackageReference.Item.LightWeightNugetPackageRecord.Id}", _commonComponentRenderers, notificationService, TimeSpan.FromSeconds(7));
                return onAfterCompletion.Invoke();
            }
        };

        terminal.EnqueueCommand(terminalCommandRequest);
        return ValueTask.CompletedTask;
    }

    public IBackgroundTaskGroup? EarlyBatchOrDefault(IBackgroundTaskGroup oldEvent)
    {
        return null;
    }

    public ValueTask HandleEvent(CancellationToken cancellationToken)
    {
        DotNetMenuOptionsFactoryWorkKind workKind;

        lock (_workLock)
        {
            if (!_workKindQueue.TryDequeue(out workKind))
                return ValueTask.CompletedTask;
        }

        switch (workKind)
        {
            case DotNetMenuOptionsFactoryWorkKind.PerformRemoveCSharpProjectReferenceFromSolution:
            {
                var args = _queue_PerformRemoveCSharpProjectReferenceFromSolution.Dequeue();
                return Do_PerformRemoveCSharpProjectReferenceFromSolution(
					args.treeViewSolution, args.projectNode, args.terminal, args.notificationService, args.onAfterCompletion);
            }
			case DotNetMenuOptionsFactoryWorkKind.PerformRemoveProjectToProjectReference:
            {
                var args = _queue_PerformRemoveProjectToProjectReference.Dequeue();
                return Do_PerformRemoveProjectToProjectReference(
                    args.treeViewCSharpProjectToProjectReference,
					args.terminal,
					args.notificationService,
                    args.onAfterCompletion);
            }
			case DotNetMenuOptionsFactoryWorkKind.PerformMoveProjectToSolutionFolder:
            {
                var args = _queue_PerformMoveProjectToSolutionFolder.Dequeue();
                return Do_PerformMoveProjectToSolutionFolder(
                    args.treeViewSolution,
                    args.treeViewProjectToMove,
					args.solutionFolderPath,
					args.terminal,
					args.notificationService,
                    args.onAfterCompletion);
            }
			case DotNetMenuOptionsFactoryWorkKind.PerformRemoveNuGetPackageReferenceFromProject:
            {
                var args = _queue_PerformRemoveNuGetPackageReferenceFromProject.Dequeue();
                return Do_PerformRemoveNuGetPackageReferenceFromProject(
                    args.modifyProjectNamespacePath,
                    args.treeViewCSharpProjectNugetPackageReference,
                    args.terminal,
                    args.notificationService,
                    args.onAfterCompletion);
            }
            default:
            {
                Console.WriteLine($"{nameof(DotNetMenuOptionsFactory)} {nameof(HandleEvent)} default case");
				return ValueTask.CompletedTask;
            }
        }
    }
}
