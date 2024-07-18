using Luthetus.Common.RazorLib.FileSystems.Models;

namespace Luthetus.Extensions.DotNet.CompilerServices.Models;

public class CompilerServiceExplorerModel
{
	public IAbsolutePath? AbsolutePath { get; }
	public bool IsLoadingCompilerServiceExplorer { get; }
}
