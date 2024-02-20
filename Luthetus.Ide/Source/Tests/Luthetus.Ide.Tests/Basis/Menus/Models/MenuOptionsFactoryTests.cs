namespace Luthetus.Ide.Tests.Basis.Menus.Models;

public class MenuOptionsFactoryTests
{
    [Fact]
    public void Constructor()
    {
        //public MenuOptionsFactory(
        //    ILuthetusIdeComponentRenderers ideComponentRenderers,
        //    ILuthetusCommonComponentRenderers commonComponentRenderers,
        //    IFileSystemProvider fileSystemProvider,
        //    IEnvironmentProvider environmentProvider,
        //    IClipboardService clipboardService,
        //    IBackgroundTaskService backgroundTaskService)
    }

    [Fact]
    public void NewEmptyFile()
    {
        //public MenuOptionRecord (IAbsolutePath parentDirectory, Func<Task> onAfterCompletion)
    }

    [Fact]
    public void NewTemplatedFile()
    {
        //public MenuOptionRecord (NamespacePath parentDirectory, Func<Task> onAfterCompletion)
    }

    [Fact]
    public void NewDirectory()
    {
        //public MenuOptionRecord (IAbsolutePath parentDirectory, Func<Task> onAfterCompletion)
    }

    [Fact]
    public void DeleteFile()
    {
        //public MenuOptionRecord (IAbsolutePath absolutePath, Func<Task> onAfterCompletion)
    }

    [Fact]
    public void RenameFile()
    {
        //public MenuOptionRecord (IAbsolutePath sourceAbsolutePath, IDispatcher dispatcher, Func<Task> onAfterCompletion)
    }

    [Fact]
    public void CopyFile()
    {
        //public MenuOptionRecord (IAbsolutePath absolutePath, Func<Task> onAfterCompletion)
    }

    [Fact]
    public void CutFile()
    {
        //public MenuOptionRecord (IAbsolutePath absolutePath, Func<Task> onAfterCompletion)
    }

    [Fact]
    public void PasteClipboard()
    {
        //public MenuOptionRecord (IAbsolutePath directoryAbsolutePath, Func<Task> onAfterCompletion)
    }

    [Fact]
    public void RemoveCSharpProjectReferenceFromSolution()
    {
        //public MenuOptionRecord (
        //    TreeViewSolution treeViewSolution,
        //    TreeViewNamespacePath projectNode,
        //    TerminalSession terminalSession,
        //    IDispatcher dispatcher,
        //    Func<Task> onAfterCompletion)
    }

    [Fact]
    public void AddProjectToProjectReference()
    {
        //public MenuOptionRecord (
        //    TreeViewNamespacePath projectReceivingReference,
        //    TerminalSession terminalSession,
        //    IDispatcher dispatcher,
        //    InputFileSync inputFileSync,
        //    Func<Task> onAfterCompletion)
    }

    [Fact]
    public void RemoveProjectToProjectReference()
    {
        //public MenuOptionRecord (
        //    TreeViewCSharpProjectToProjectReference treeViewCSharpProjectToProjectReference,
        //    TerminalSession terminalSession,
        //    IDispatcher dispatcher,
        //    Func<Task> onAfterCompletion)
    }

    [Fact]
    public void MoveProjectToSolutionFolder()
    {
        //public MenuOptionRecord (
        //    TreeViewSolution treeViewSolution,
        //    TreeViewNamespacePath treeViewProjectToMove,
        //    TerminalSession terminalSession,
        //    IDispatcher dispatcher,
        //    Func<Task> onAfterCompletion)
    }

    [Fact]
    public void RemoveNuGetPackageReferenceFromProject()
    {
        //public MenuOptionRecord (
        //    NamespacePath modifyProjectNamespacePath,
        //    TreeViewCSharpProjectNugetPackageReference treeViewCSharpProjectNugetPackageReference,
        //    TerminalSession terminalSession,
        //    IDispatcher dispatcher,
        //    Func<Task> onAfterCompletion)
    }

    [Fact]
    public void PerformAddProjectToProjectReferenceAction()
    {
        //public void (
        //    TreeViewNamespacePath projectReceivingReference,
        //    TerminalSession terminalSession,
        //    IDispatcher dispatcher,
        //    InputFileSync inputFileSync,
        //    Func<Task> onAfterCompletion)
    }

    [Fact]
    public void PerformRemoveProjectToProjectReferenceAction()
    {
        //public void (
        //    TreeViewCSharpProjectToProjectReference treeViewCSharpProjectToProjectReference,
        //    TerminalSession terminalSession,
        //    IDispatcher dispatcher,
        //    Func<Task> onAfterCompletion)
    }

    [Fact]
    public void PerformMoveProjectToSolutionFolderAction()
    {
        //public void (
        //    TreeViewSolution treeViewSolution,
        //    TreeViewNamespacePath treeViewProjectToMove,
        //    string solutionFolderPath,
        //    TerminalSession terminalSession,
        //    IDispatcher dispatcher,
        //    Func<Task> onAfterCompletion)
    }

    [Fact]
    public void PerformRemoveNuGetPackageReferenceFromProjectAction()
    {
        //public void (
        //    NamespacePath modifyProjectNamespacePath,
        //    TreeViewCSharpProjectNugetPackageReference treeViewCSharpProjectNugetPackageReference,
        //    TerminalSession terminalSession,
        //    IDispatcher dispatcher,
        //    Func<Task> onAfterCompletion)
    }

    [Fact]
    public void CopyFilesRecursively()
    {
        //public static DirectoryInfo (DirectoryInfo source, DirectoryInfo target)
    }
}