using Luthetus.Ide.ClassLib.FileSystem.Interfaces;

namespace Luthetus.Ide.ClassLib.Store.FolderExplorerCase;

public record SetFolderExplorerStateAction(IAbsoluteFilePath? AbsoluteFilePath);