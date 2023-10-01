using Luthetus.Common.RazorLib.FileSystems.Models;

namespace Luthetus.Ide.RazorLib.CompilerServices.Models;

public class CompilerServiceExplorerModel
{
    public IAbsolutePath? AbsolutePath { get; }
    public bool IsLoadingCompilerServiceExplorer { get; }
}
