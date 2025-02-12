using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.Namespaces.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Extensions.DotNet.CSharpProjects.Models;
using Luthetus.Extensions.DotNet.DotNetSolutions.Models;
using Luthetus.Extensions.DotNet.Namespaces.Models;

namespace Luthetus.Extensions.DotNet.Menus.Models;

public interface IDotNetMenuOptionsFactory
{
	public MenuOptionRecord RemoveCSharpProjectReferenceFromSolution(
		TreeViewSolution solutionNode,
		TreeViewNamespacePath projectNode,
		ITerminal terminal,
		INotificationService notificationService,
		Func<Task> onAfterCompletion);

	public MenuOptionRecord AddProjectToProjectReference(
		TreeViewNamespacePath projectReceivingReference,
		ITerminal terminal,
		INotificationService notificationService,
		IdeBackgroundTaskApi ideBackgroundTaskApi,
		Func<Task> onAfterCompletion);

	public MenuOptionRecord RemoveProjectToProjectReference(
		TreeViewCSharpProjectToProjectReference treeViewCSharpProjectToProjectReference,
		ITerminal terminal,
		INotificationService notificationService,
		Func<Task> onAfterCompletion);

	public MenuOptionRecord MoveProjectToSolutionFolder(
		TreeViewSolution treeViewSolution,
		TreeViewNamespacePath treeViewProjectToMove,
		ITerminal terminal,
		INotificationService notificationService,
		Func<Task> onAfterCompletion);

	public MenuOptionRecord RemoveNuGetPackageReferenceFromProject(
		NamespacePath modifyProjectNamespacePath,
		TreeViewCSharpProjectNugetPackageReference treeViewCSharpProjectNugetPackageReference,
		ITerminal terminal,
		INotificationService notificationService,
		Func<Task> onAfterCompletion);
}
