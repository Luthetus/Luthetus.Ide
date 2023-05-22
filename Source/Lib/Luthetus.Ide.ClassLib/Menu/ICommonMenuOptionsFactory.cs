using Luthetus.Common.RazorLib.Menu;
using Fluxor;
using Luthetus.Ide.ClassLib.FileSystem.Interfaces;
using Luthetus.Ide.ClassLib.Namespaces;
using Luthetus.Ide.ClassLib.Store.TerminalCase;
using Luthetus.Ide.ClassLib.TreeViewImplementations;

namespace Luthetus.Ide.ClassLib.Menu;

public interface ICommonMenuOptionsFactory
{
    public MenuOptionRecord NewEmptyFile(
        IAbsoluteFilePath parentDirectory,
        Func<Task> onAfterCompletion);

    public MenuOptionRecord NewTemplatedFile(
        NamespacePath parentDirectory,
        Func<Task> onAfterCompletion);

    public MenuOptionRecord NewDirectory(
        IAbsoluteFilePath parentDirectory,
        Func<Task> onAfterCompletion);

    public MenuOptionRecord DeleteFile(
        IAbsoluteFilePath absoluteFilePath,
        Func<Task> onAfterCompletion);

    public MenuOptionRecord CopyFile(
        IAbsoluteFilePath absoluteFilePath,
        Func<Task> onAfterCompletion);

    public MenuOptionRecord CutFile(
        IAbsoluteFilePath absoluteFilePath,
        Func<Task> onAfterCompletion);

    public MenuOptionRecord RenameFile(
        IAbsoluteFilePath sourceAbsoluteFilePath,
        IDispatcher dispatcher,
        Func<Task> onAfterCompletion);

    public MenuOptionRecord PasteClipboard(
        IAbsoluteFilePath directoryAbsoluteFilePath,
        Func<Task> onAfterCompletion);

    public MenuOptionRecord RemoveCSharpProjectReferenceFromSolution(
        TreeViewSolution? solutionNode,
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
        TreeViewLightWeightNugetPackageRecord treeViewLightWeightNugetPackageRecord,
        TerminalSession terminalSession,
        IDispatcher dispatcher,
        Func<Task> onAfterCompletion);
}