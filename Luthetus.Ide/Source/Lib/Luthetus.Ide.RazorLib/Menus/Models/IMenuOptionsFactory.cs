﻿using Fluxor;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.Namespaces.Models;
using Luthetus.Ide.RazorLib.InputFiles.States;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.TreeViewImplementations.Models;

namespace Luthetus.Ide.RazorLib.Menus.Models;

public interface IMenuOptionsFactory
{
    public MenuOptionRecord NewEmptyFile(IAbsolutePath parentDirectory, Func<Task> onAfterCompletion);
    public MenuOptionRecord NewTemplatedFile(NamespacePath parentDirectory, Func<Task> onAfterCompletion);
    public MenuOptionRecord NewDirectory(IAbsolutePath parentDirectory, Func<Task> onAfterCompletion);
    public MenuOptionRecord DeleteFile(IAbsolutePath absolutePath, Func<Task> onAfterCompletion);
    public MenuOptionRecord CopyFile(IAbsolutePath absolutePath, Func<Task> onAfterCompletion);
    public MenuOptionRecord CutFile(IAbsolutePath absolutePath, Func<Task> onAfterCompletion);

    public MenuOptionRecord RenameFile(
        IAbsolutePath sourceAbsolutePath,
        IDispatcher dispatcher,
        Func<Task> onAfterCompletion);

    public MenuOptionRecord PasteClipboard(
        IAbsolutePath directoryAbsolutePath,
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
        InputFileSync inputFileSync,
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