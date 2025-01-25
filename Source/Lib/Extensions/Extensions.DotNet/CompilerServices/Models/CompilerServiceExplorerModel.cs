using Luthetus.Common.RazorLib.FileSystems.Models;

namespace Luthetus.Extensions.DotNet.CompilerServices.Models;

public class CompilerServiceExplorerModel
{
	public AbsolutePath? AbsolutePath { get; }
	public bool IsLoadingCompilerServiceExplorer { get; }
}
