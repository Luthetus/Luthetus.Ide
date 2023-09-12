using Luthetus.Common.RazorLib.FileSystem.Interfaces;

namespace Luthetus.Ide.ClassLib.Store.CompilerServiceExplorerCase.InnerDetails;

public class CompilerServiceExplorerModel
{
    public IAbsolutePath? AbsoluteFilePath { get; }
    public bool IsLoadingCompilerServiceExplorer { get; }
}
