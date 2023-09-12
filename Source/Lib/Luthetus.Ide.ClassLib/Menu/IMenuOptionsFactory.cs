using Fluxor;
using Luthetus.Common.RazorLib.FileSystem.Interfaces;
using Luthetus.Common.RazorLib.Menu;
using Luthetus.Common.RazorLib.Namespaces;
using Luthetus.Ide.ClassLib.Store.TerminalCase;
using Luthetus.Ide.ClassLib.TreeViewImplementations;

namespace Luthetus.Ide.ClassLib.Menu;

public interface IMenuOptionsFactory
{
    public MenuOptionRecord NewEmptyFile(
        IAbsolutePath parentDirectory,
        Func<Task> onAfterCompletion);

    public MenuOptionRecord NewTemplatedFile(
        NamespacePath parentDirectory,
        Func<Task> onAfterCompletion);

    public MenuOptionRecord NewDirectory(
        IAbsolutePath parentDirectory,
        Func<Task> onAfterCompletion);

    public MenuOptionRecord DeleteFile(
        IAbsolutePath absoluteFilePath,
        Func<Task> onAfterCompletion);

    public MenuOptionRecord CopyFile(
        IAbsolutePath absoluteFilePath,
        Func<Task> onAfterCompletion);

    public MenuOptionRecord CutFile(
        IAbsolutePath absoluteFilePath,
        Func<Task> onAfterCompletion);

    public MenuOptionRecord RenameFile(
        IAbsolutePath sourceAbsoluteFilePath,
        IDispatcher dispatcher,
        Func<Task> onAfterCompletion);

    public MenuOptionRecord PasteClipboard(
        IAbsolutePath directoryAbsoluteFilePath,
        Func<Task> onAfterCompletion);

    public MenuOptionRecord RemoveCSharpProjectReferenceFromSolution(
        TreeViewSolution solutionNode,
        TreeViewNamespacePath projectNode,
        TerminalSession terminalSession,
        IDispatcher dispatcher,
        Func<Task> onAfterCompletion);

    public MenuOptionRecord AddProjectToProjectReference(
        TreeViewNamespacePath projectReceivingReference,
        TerminalSession terminalSession,
        IDispatcher dispatcher,
        Func<Task> onAfterCompletion);

    public MenuOptionRecord RemoveProjectToProjectReference(
        TreeViewCSharpProjectToProjectReference treeViewCSharpProjectToProjectReference,
        TerminalSession terminalSession,
        IDispatcher dispatcher,
        Func<Task> onAfterCompletion);

    public MenuOptionRecord MoveProjectToSolutionFolder(
        TreeViewSolution treeViewSolution,
        TreeViewNamespacePath treeViewProjectToMove,
        TerminalSession terminalSession,
        IDispatcher dispatcher,
        Func<Task> onAfterCompletion);

    public MenuOptionRecord RemoveNuGetPackageReferenceFromProject(
        NamespacePath modifyProjectNamespacePath,
        TreeViewCSharpProjectNugetPackageReference treeViewCSharpProjectNugetPackageReference,
        TerminalSession terminalSession,
        IDispatcher dispatcher,
        Func<Task> onAfterCompletion);
}