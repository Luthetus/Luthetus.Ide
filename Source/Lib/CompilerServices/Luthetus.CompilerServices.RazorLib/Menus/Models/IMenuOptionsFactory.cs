using Luthetus.Ide.RazorLib.CSharpProjects.Models;
using Luthetus.Ide.RazorLib.DotNetSolutions.Models;

namespace Luthetus.CompilerServices.RazorLib.Menus.Models;

public interface IMenuOptionsFactory
{
	public MenuOptionRecord RemoveCSharpProjectReferenceFromSolution(
        TreeViewSolution solutionNode,
        TreeViewNamespacePath projectNode,
        Terminal terminal,
        IDispatcher dispatcher,
        Func<Task> onAfterCompletion);

    public MenuOptionRecord AddProjectToProjectReference(
        TreeViewNamespacePath projectReceivingReference,
        Terminal terminal,
        IDispatcher dispatcher,
        IdeBackgroundTaskApi ideBackgroundTaskApi,
        Func<Task> onAfterCompletion);

    public MenuOptionRecord RemoveProjectToProjectReference(
        TreeViewCSharpProjectToProjectReference treeViewCSharpProjectToProjectReference,
        Terminal terminal,
        IDispatcher dispatcher,
        Func<Task> onAfterCompletion);

    public MenuOptionRecord MoveProjectToSolutionFolder(
        TreeViewSolution treeViewSolution,
        TreeViewNamespacePath treeViewProjectToMove,
        Terminal terminal,
        IDispatcher dispatcher,
        Func<Task> onAfterCompletion);

    public MenuOptionRecord RemoveNuGetPackageReferenceFromProject(
        NamespacePath modifyProjectNamespacePath,
        TreeViewCSharpProjectNugetPackageReference treeViewCSharpProjectNugetPackageReference,
        Terminal terminal,
        IDispatcher dispatcher,
        Func<Task> onAfterCompletion);
}
