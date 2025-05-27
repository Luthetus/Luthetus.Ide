using Luthetus.Common.RazorLib.FileSystems.Models;

namespace Luthetus.Ide.RazorLib.FileSystems.Models;

public struct FileSystemIdeApiWorkArgs
{
	public FileSystemIdeApiWorkKind WorkKind { get; set; }
    public AbsolutePath AbsolutePath { get; set; }
    public string Content { get; set; }
    public Func<DateTime?, Task> OnAfterSaveCompletedWrittenDateTimeFunc { get; set; }
    public CancellationToken CancellationToken { get; set; }
}
