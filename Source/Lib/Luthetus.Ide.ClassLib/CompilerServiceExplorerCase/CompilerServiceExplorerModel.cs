using Luthetus.Common.RazorLib.FileSystem.Interfaces;

namespace Luthetus.Ide.ClassLib.CompilerServiceExplorerCase;

public class CompilerServiceExplorerModel
{
    public IAbsolutePath? AbsolutePath { get; }
    public bool IsLoadingCompilerServiceExplorer { get; }
}
