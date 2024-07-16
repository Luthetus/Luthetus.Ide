using Fluxor;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.Namespaces.Models;
using Luthetus.CompilerServices.RazorLib.CSharpProjects.Models;
using Luthetus.CompilerServices.RazorLib.DotNetSolutions.Models;
using Luthetus.Ide.RazorLib.Namespaces.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;

namespace Luthetus.CompilerServices.RazorLib.Menus.Models;

public interface ICompilerServicesMenuOptionsFactory
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
