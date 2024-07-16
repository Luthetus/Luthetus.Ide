using System.Collections.Immutable;
using Fluxor;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.Namespaces.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.CompilerServices.RazorLib.DotNetSolutions.Models;
using Luthetus.CompilerServices.RazorLib.CSharpProjects.Models;
using Luthetus.CompilerServices.RazorLib.ComponentRenderers.Models;
using Luthetus.CompilerServices.RazorLib.CommandLines.Models;
using Luthetus.Ide.RazorLib.InputFiles.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.Namespaces.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.FileSystems.Models;

namespace Luthetus.CompilerServices.RazorLib.Menus.Models;

public class MenuOptionsFactory : IMenuOptionsFactory
{
	private readonly IIdeComponentRenderers _ideComponentRenderers;
	private readonly IBackgroundTaskService _backgroundTaskService;
	private readonly ICommonComponentRenderers _commonComponentRenderers;

	public MenuOptionsFactory(
		IIdeComponentRenderers ideComponentRenderers,
		IBackgroundTaskService backgroundTaskService,
		ICommonComponentRenderers commonComponentRenderers)
	{
		_ideComponentRenderers = ideComponentRenderers;
		_backgroundTaskService = backgroundTaskService;
		_commonComponentRenderers = commonComponentRenderers;
	}

	public MenuOptionRecord RemoveCSharpProjectReferenceFromSolution(
        TreeViewSolution treeViewSolution,
        TreeViewNamespacePath projectNode,
        Terminal terminal,
        IDispatcher dispatcher,
        Func<Task> onAfterCompletion)
    {
        return new MenuOptionRecord("Remove (no files are deleted)", MenuOptionKind.Delete,
            WidgetRendererType: _ideComponentRenderers.RemoveCSharpProjectFromSolutionRendererType,
            WidgetParameterMap: new Dictionary<string, object?>
            {
                {
                    nameof(IRemoveCSharpProjectFromSolutionRendererType.AbsolutePath),
                    projectNode.Item.AbsolutePath
                },
                {
                    nameof(IDeleteFileFormRendererType.OnAfterSubmitFunc),
                    new Func<IAbsolutePath, Task>(
						_ =>
						{
							PerformRemoveCSharpProjectReferenceFromSolution(
		                        treeViewSolution,
		                        projectNode,
		                        terminal,
		                        dispatcher,
		                        onAfterCompletion);

							return Task.CompletedTask;
						})
                },
            });
    }
    
    public MenuOptionRecord AddProjectToProjectReference(
        TreeViewNamespacePath projectReceivingReference,
        Terminal terminal,
        IDispatcher dispatcher,
        IdeBackgroundTaskApi ideBackgroundTaskApi,
        Func<Task> onAfterCompletion)
    {
        return new MenuOptionRecord("Add Project Reference", MenuOptionKind.Other,
            OnClickFunc: 
			() =>
			{
				PerformAddProjectToProjectReference(
	                projectReceivingReference,
	                terminal,
	                dispatcher,
	                ideBackgroundTaskApi,
	                onAfterCompletion);

				return Task.CompletedTask;
			});
    }
    
    public MenuOptionRecord RemoveProjectToProjectReference(
        TreeViewCSharpProjectToProjectReference treeViewCSharpProjectToProjectReference,
        Terminal terminal,
        IDispatcher dispatcher,
        Func<Task> onAfterCompletion)
    {
        return new MenuOptionRecord("Remove Project Reference", MenuOptionKind.Other,
            OnClickFunc: 
				() =>
				{
					PerformRemoveProjectToProjectReference(
		                treeViewCSharpProjectToProjectReference,
		                terminal,
		                dispatcher,
		                onAfterCompletion);

						return Task.CompletedTask;
				});
    }
    
    public MenuOptionRecord MoveProjectToSolutionFolder(
        TreeViewSolution treeViewSolution,
        TreeViewNamespacePath treeViewProjectToMove,
        Terminal terminal,
        IDispatcher dispatcher,
        Func<Task> onAfterCompletion)
    {
        return new MenuOptionRecord("Move to Solution Folder", MenuOptionKind.Other,
            WidgetRendererType: _ideComponentRenderers.FileFormRendererType,
            WidgetParameterMap: new Dictionary<string, object?>
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
                            dispatcher,
                            onAfterCompletion);

						return Task.CompletedTask;
					})
                },
            });
    }
    
    public MenuOptionRecord RemoveNuGetPackageReferenceFromProject(
        NamespacePath modifyProjectNamespacePath,
        TreeViewCSharpProjectNugetPackageReference treeViewCSharpProjectNugetPackageReference,
        Terminal terminal,
        IDispatcher dispatcher,
        Func<Task> onAfterCompletion)
    {
        return new MenuOptionRecord("Remove NuGet Package Reference", MenuOptionKind.Other,
            OnClickFunc: () =>
			{
				PerformRemoveNuGetPackageReferenceFromProject(
	                modifyProjectNamespacePath,
	                treeViewCSharpProjectNugetPackageReference,
	                terminal,
	                dispatcher,
	                onAfterCompletion);
	
				return Task.CompletedTask;
			});
    }
    
    private void PerformRemoveCSharpProjectReferenceFromSolution(
        TreeViewSolution treeViewSolution,
        TreeViewNamespacePath projectNode,
        Terminal terminal,
        IDispatcher dispatcher,
        Func<Task> onAfterCompletion)
    {
        _backgroundTaskService.Enqueue(
            Key<IBackgroundTask>.NewKey(),
            ContinuousBackgroundTaskWorker.GetQueueKey(),
            "Remove C# Project Reference from Solution Action",
            () =>
            {
                var workingDirectory = treeViewSolution.Item.NamespacePath.AbsolutePath.ParentDirectory!;

                var formattedCommand = DotNetCliCommandFormatter.FormatRemoveCSharpProjectReferenceFromSolutionAction(
                    treeViewSolution.Item.NamespacePath.AbsolutePath.Value,
                    projectNode.Item.AbsolutePath.Value);

                var terminalCommand = new TerminalCommand(
                    Key<TerminalCommand>.NewKey(),
                    formattedCommand,
                    workingDirectory.Value,
                    CancellationToken.None,
                    async () => await onAfterCompletion.Invoke().ConfigureAwait(false));

                terminal.EnqueueCommand(terminalCommand);
                return Task.CompletedTask;
            });
    }
    
    public void PerformAddProjectToProjectReference(
        TreeViewNamespacePath projectReceivingReference,
        Terminal terminal,
        IDispatcher dispatcher,
        IdeBackgroundTaskApi ideBackgroundTaskApi,
        Func<Task> onAfterCompletion)
    {
        ideBackgroundTaskApi.InputFile.RequestInputFileStateForm(
            $"Add Project reference to {projectReceivingReference.Item.AbsolutePath.NameWithExtension}",
            referencedProject =>
            {
                if (referencedProject is null)
                    return Task.CompletedTask;

                var formattedCommand = DotNetCliCommandFormatter.FormatAddProjectToProjectReference(
                    projectReceivingReference.Item.AbsolutePath.Value,
                    referencedProject.Value);

                var terminalCommand = new TerminalCommand(
                    Key<TerminalCommand>.NewKey(),
                    formattedCommand,
                    null,
                    CancellationToken.None,
                    async () =>
                    {
                        NotificationHelper.DispatchInformative("Add Project Reference", $"Modified {projectReceivingReference.Item.AbsolutePath.NameWithExtension} to have a reference to {referencedProject.NameWithExtension}", _commonComponentRenderers, dispatcher, TimeSpan.FromSeconds(7));
                        await onAfterCompletion.Invoke().ConfigureAwait(false);
                    });

                terminal.EnqueueCommand(terminalCommand);
                return Task.CompletedTask;
            },
            absolutePath =>
            {
                if (absolutePath is null || absolutePath.IsDirectory)
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
        Terminal terminal,
        IDispatcher dispatcher,
        Func<Task> onAfterCompletion)
    {
        _backgroundTaskService.Enqueue(
            Key<IBackgroundTask>.NewKey(),
            ContinuousBackgroundTaskWorker.GetQueueKey(),
            "Remove Project Reference to Project",
            () =>
            {
                var formattedCommand = DotNetCliCommandFormatter.FormatRemoveProjectToProjectReference(
                    treeViewCSharpProjectToProjectReference.Item.ModifyProjectNamespacePath.AbsolutePath.Value,
                    treeViewCSharpProjectToProjectReference.Item.ReferenceProjectAbsolutePath.Value);

                var removeProjectToProjectReferenceTerminalCommand = new TerminalCommand(
                    Key<TerminalCommand>.NewKey(),
                    formattedCommand,
                    null,
                    CancellationToken.None,
                    async () =>
                    {
                        NotificationHelper.DispatchInformative("Remove Project Reference", $"Modified {treeViewCSharpProjectToProjectReference.Item.ModifyProjectNamespacePath.AbsolutePath.NameWithExtension} to have a reference to {treeViewCSharpProjectToProjectReference.Item.ReferenceProjectAbsolutePath.NameWithExtension}", _commonComponentRenderers, dispatcher, TimeSpan.FromSeconds(7));
                        await onAfterCompletion.Invoke().ConfigureAwait(false);
                    });

                terminal.EnqueueCommand(removeProjectToProjectReferenceTerminalCommand);
                return Task.CompletedTask;
            });
    }
    
    public void PerformMoveProjectToSolutionFolder(
        TreeViewSolution treeViewSolution,
        TreeViewNamespacePath treeViewProjectToMove,
        string solutionFolderPath,
        Terminal terminal,
        IDispatcher dispatcher,
        Func<Task> onAfterCompletion)
    {
        _backgroundTaskService.Enqueue(
            Key<IBackgroundTask>.NewKey(),
            ContinuousBackgroundTaskWorker.GetQueueKey(),
            "Move Project to Solution Folder",
            () =>
            {
                var formattedCommand = DotNetCliCommandFormatter.FormatMoveProjectToSolutionFolder(
                    treeViewSolution.Item.NamespacePath.AbsolutePath.Value,
                    treeViewProjectToMove.Item.AbsolutePath.Value,
                    solutionFolderPath);

                var moveProjectToSolutionFolderTerminalCommand = new TerminalCommand(
                    Key<TerminalCommand>.NewKey(),
                    formattedCommand,
                    null,
                    CancellationToken.None,
                    async () =>
                    {
                        NotificationHelper.DispatchInformative("Move Project To Solution Folder", $"Moved {treeViewProjectToMove.Item.AbsolutePath.NameWithExtension} to the Solution Folder path: {solutionFolderPath}", _commonComponentRenderers, dispatcher, TimeSpan.FromSeconds(7));
                        await onAfterCompletion.Invoke().ConfigureAwait(false);
                    });

                PerformRemoveCSharpProjectReferenceFromSolution(
                    treeViewSolution,
                    treeViewProjectToMove,
                    terminal,
                    dispatcher,
                    () =>
					{
						terminal.EnqueueCommand(moveProjectToSolutionFolderTerminalCommand);
						return Task.CompletedTask;
					});

                return Task.CompletedTask;
            });
    }
    
    public void PerformRemoveNuGetPackageReferenceFromProject(
        NamespacePath modifyProjectNamespacePath,
        TreeViewCSharpProjectNugetPackageReference treeViewCSharpProjectNugetPackageReference,
        Terminal terminal,
        IDispatcher dispatcher,
        Func<Task> onAfterCompletion)
    {
        _backgroundTaskService.Enqueue(
            Key<IBackgroundTask>.NewKey(),
            ContinuousBackgroundTaskWorker.GetQueueKey(),
            "Remove NuGet Package Reference from Project",
            () =>
            {
                var formattedCommand = DotNetCliCommandFormatter.FormatRemoveNugetPackageReferenceFromProject(
                    modifyProjectNamespacePath.AbsolutePath.Value,
                    treeViewCSharpProjectNugetPackageReference.Item.LightWeightNugetPackageRecord.Id);

                var removeNugetPackageReferenceFromProjectTerminalCommand = new TerminalCommand(
                    Key<TerminalCommand>.NewKey(),
                    formattedCommand,
                    null,
                    CancellationToken.None,
                    async () =>
                    {
                        NotificationHelper.DispatchInformative("Remove Project Reference", $"Modified {modifyProjectNamespacePath.AbsolutePath.NameWithExtension} to NOT have a reference to {treeViewCSharpProjectNugetPackageReference.Item.LightWeightNugetPackageRecord.Id}", _commonComponentRenderers, dispatcher, TimeSpan.FromSeconds(7));
                        await onAfterCompletion.Invoke().ConfigureAwait(false);
                    });

                terminal.EnqueueCommand(removeNugetPackageReferenceFromProjectTerminalCommand);
                return Task.CompletedTask;
            });
    }
}
