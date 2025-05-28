namespace Luthetus.Ide.RazorLib.BackgroundTasks.Models;

public enum IdeBackgroundTaskApiWorkKind
{
	None,
    LuthetusIdeInitializerOnInit,
    IdeHeaderOnInit,
    FileContentsWereModifiedOnDisk,
    SaveFile,
    SetFolderExplorerState,
    SetFolderExplorerTreeView,
    RequestInputFileStateForm,
}
