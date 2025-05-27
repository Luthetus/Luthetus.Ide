using Luthetus.Common.RazorLib.FileSystems.Models;

namespace Luthetus.Ide.RazorLib.FolderExplorers.Models;

public struct FolderExplorerIdeApiWorkArgs
{
	public FolderExplorerIdeApiWorkKind FolderExplorerIdeApiWorkKind { get; set; }
	public AbsolutePath AbsolutePath { get; set; }
}
